/*****************************************************************************
 * Author:   Valient Gough <vgough@pobox.com>
 *
 *****************************************************************************
 * Copyright (c) 2004-2005, Valient Gough
 *
 * This library is free software; you can distribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL), as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at your
 * option) any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the LGPL in the file COPYING for more
 * details.
 *
 */
		                                                                                

using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using RLog;


namespace Fuse
{
    /** @class Channel
      @brief Communication channel to FUSE kernel module.

      Fuse.Channel communicates with the FUSE kernel module.  Each Channel
      instance is tied to a filesystem mount point.

      When a channel receives a request from the kernel, it decodes the request
      header and emits the Channel.OnNewCommand event.  The event contains one
      argument - an Operation.  The Operation specifies which filesystem is
      involved, the command header, and the buffer of further data related to
      the request.

      OnNewCommand is emitted using an asynchronous callback, so subscribers
      should be thread safe. TODO: this might be configureable in the future..

      A Channel controls a single mount-point.  Multiple Channel instances may
      be created and served by either a single Reactor, or multiple Reactor
      instances.  A separate thread will be required for each Channel
      instance's event loop, since there is currently no support for
      multiplexing Channel event loops.

      @relates Reactor
      @relates FileSystem
      @author Valient Gough
    */
    public class Channel
    {
	/**
	    Command processing delegate.  

	    Process an operation and return a result object.  What's done with
	    the result will depend on its type.  The following are handled:
	    
	    (null) : no result sent for operation
	    (int) : an error code to return to the caller
	    (BufferWritable) : error code 0, encode buffer as reply
	    (Transactional) : play transaction, send result
	    anything else: return generic error code to caller
	*/
	public delegate object ProcessCommand( Operation op );

	/** event handler for new commands..
	  Every time a command is received from the kernel, it is emitted using
	  this event.
	*/
	public event ProcessCommand OnNewCommand;

	private string _mountPoint;

	private object syncObj = new object();
	// fuse_fd is a file descriptor for the communication channel to the
	// FUSE kernel module for our mountpoint.
	private int fuse_fd = -1;

	private FileSystem _fs;

	// Track available buffers, to avoid having to create one for every
	// request, which would put unecessary pressure on the garbage
	// collector.
	private Queue _bufferQueue;

	static LogNode Debug = Log.Debug;
	static LogNode Error = Log.Error;

	/*! Instanciate a channel for the given filesystem mount point.

	  This tells FUSE that we will implement the filesystem at the
	  specified mount point.

	  @param mountPoint A fully qualified directory name which will be the
	  root of the filesystem.
	*/
	public Channel( string mountPoint, FileSystem fs )
	{
	    _fs = fs;
	    _bufferQueue = Queue.Synchronized( new Queue() );
	    _mountPoint = mountPoint;
	}

	~Channel()
	{
	    // TODO: do we need to politely tell fuse that we're leaving?
	    FuseWrapper.close_fd( fuse_fd );
	    fuse_fd = -1;
	}

	public string MountPoint
	{
	    get { return _mountPoint; }
	}

	/*! Waits for events, and does not return until the filesystem is
	  unmounted.

	  Normally this should not be used directly - see Reactor which
	  provides a higher level interface.

	  Returns @e true if exited normally.
	*/
	public bool EventLoop()
	{
	    Debug.Log("entered event loop");
	    Error.Assert(fuse_fd >= 0, "Not Connected");

	    while(true)
	    {
		Operation op = NewOp();

		// read the next request from the FUSE kernel
		// blocks until either a request comes in, or an error occurs
		int readSize = op.inData.Read( fuse_fd );

		// if we get a partial read, then it may be an indication that
		// we've lost the channel (been unmounted)...
		if(readSize < fuse_in_header.structSize()) 
		{
		    if(readSize == (int)ErrorCode.EINTR
			    || readSize == (int)ErrorCode.EAGAIN)
			continue; // interrupted system call.. just try again..

		    // normal exit causes an ENODEV error..
		    if(readSize == (int)ErrorCode.ENODEV)
			return true;

		    Error.Log("Read error from kernel: {0}", readSize);
		    // TODO: another possibility might be to try and
		    // reconnect...
		    return false; // exited due to error..
		}

		// the command always begins with fuse_in_header
		op.header = new fuse_in_header();
		if( op.header.copyFrom( op.inData ) <= 0 )
		{
		    Error.Log("failed to read fuse_in_header");
		    return false;
		}

		Debug.Log("unique: {0}, opcode: {1}, ino {2}, insize {3}",
			op.header.unique, op.header.opcode, op.header.nodeid,
			op.inData.initialized);

		// setup a callback to notify us when the work is complete.
		AsyncCallback cback = new AsyncCallback( OperationComplete );

		// As a state object, pass the Operation
		//IAsyncResult asyncResult = 
		OnNewCommand.BeginInvoke(op, cback, op);
	    }
	}

	/*  This is called when the operation completes.  The result is sent
	    back to the kernel if necessary.

	    An added complication is that some commands are abortable -- the
	    kernel operation may have been aborted before the command
	    completed, and so we may have to undo any state information that
	    was created for the command.
	*/
	//[OneWayAttribute()]
	private void OperationComplete( IAsyncResult iar )
	{
	    // the state object is the delegate..
	    Operation op = (Operation)iar.AsyncState;
	    object result = OnNewCommand.EndInvoke( iar );
	   
	    if(result != null)
	    {
		try
		{
		    SendResults( op, result );
		} catch( System.Exception e )
		{
		    Error.Log("Caught exception on opcode {0}: {1}",
			    op.header.opcode.ToString(), e.Message);
		    Error.Log("Exception type: " + e.GetType());
		    Error.Log("Exception source: " + e.Source);
		    Error.Log("Exception target: " + e.TargetSite);
		    Error.Log("Exception Stack: " + e.StackTrace);

		    // try sending a simple error value instead..
		    result = (int)ErrorCode.EIO;
		    try
		    {
			SendResults( op, result );
		    }catch { }
		}
	    } else
		Debug.Log("unique {0}, no response", op.header.unique);

	    // put the buffer back in the queue
    	    _bufferQueue.Enqueue( op.inData );
	    op.inData = null;
	}
	
	private void SendResults( Operation op, object result )
	{
	    Debug.Log("unique {0}, got type {1}", op.header.unique, 
		    result.GetType());

	    // reuse the input buffer as the output buffer..
	    FWBuffer buf = op.inData;
	    buf.Reset();

	    object outputArg = result;
	    if(outputArg is Transactional)
	    {
		// start transaction.  Result should be an integer or
		// BufferWritable..
		Debug.Log("unique {0}, starting transaction",
			op.header.unique);
		outputArg = ((Transactional)outputArg).beginTransaction();
	    }

	    // in case we started a transaction, we have to finish it even if
	    // there is an error..
	    int res = 0;
	    try
	    {
		// add the header to the buffer first..
		int errorCode = (outputArg is int) ? (int)outputArg : 0;

		EncodeMessage( ref buf, op.header.unique, errorCode,
			outputArg as BufferWritable );

	       	res = SendToKernel( buf, op.header.unique );
	    } catch
	    {
		if(result is Transactional)
		    ((Transactional)result).abortTransaction(outputArg);
		throw;
	    }

	    // FUSE kernel module replies with ENOENT if the operation was
	    // aborted..
	    if(res < 0)
	    {
		Error.Log("unique {0}, Error from kernel: {1}", 
			op.header.unique, res);
		if(result is Transactional)
		    ((Transactional)result).abortTransaction(outputArg);
	    } else
	    {
		if(result is Transactional)
		{
		    Debug.Log("unique {0}, finishing transaction, code {1}", 
			    op.header.unique, res);
		    ((Transactional)result).commitTransaction(outputArg);
		}
	    }
	}

	private void ShowBuf( FWBuffer buf )
	{
	    int offset = buf.offset;

	    Console.WriteLine("Buffer data:");
	    int len = buf.initialized - offset;
	    for(int loc = 0; loc < len; ++loc)
	    {
		int ch = buf.GetByte();
		if(loc % 8 == 0)
		    Console.Write("\n{0,4}:  ", loc);
		if(ch >= 33 && ch <= 126)
		    Console.Write("  {0}",  (char)ch );
		else
		    Console.Write("{0,3}",  (uint)ch );
		Console.Write(" ");
	    }
	    Console.WriteLine();

	    buf.offset = offset;
	}

	private int SendToKernel( FWBuffer buf, ulong unique )
	{
	    int res;
	    do
	    {
		Debug.Log("unique {0}, writing {1} bytes", 
			unique, buf.initialized);
		// only allow one writer at a time.
		lock( syncObj )
		{
		    buf.Rewind(); // write the whole thing out..
		    //ShowBuf( buf );
		    res = buf.Write( fuse_fd );
		}
	    } while( res == (int)ErrorCode.EINTR );
	    return res;
	}

	
	public void Mount(params string[] options)
	{
	    if(fuse_fd != -1)
		return; // already mounted

	    // build up call to fusemount...
	    StringBuilder opts = new StringBuilder();
	    // see usage() in fuse/lib/helper.c
	    // TODO: untaint options, checking for invalid characters..
	    opts.AppendFormat("fsname={0}", _fs.Name);
	    // filesystem supplied arguments
	    if(_fs.RequiredMountArgs != null)
		foreach( string opt in _fs.RequiredMountArgs )
		    opts.AppendFormat(",{0}", opt);
	    // user supplied arguments
	    foreach(string opt in options)
		opts.AppendFormat(",{0}", opt);

	    Debug.Log("fuse_mount( \"{0}\", \"{1}\" )", _mountPoint,
		    opts.ToString());

	    // use libfuse to do the mounting..
	    fuse_fd = fuse_mount( _mountPoint, opts.ToString() );
	    if(fuse_fd == -1)
		throw new ApplicationException("fuse_mount failed");
	}
	
	public void Unmount()
	{
	    fuse_unmount( _mountPoint );
	}

	private Operation NewOp()
	{
	    Operation op = new Operation();
	  
	    op.channel = this;
	    op.fs = _fs;

	    // Use a buffer queue so that we don't have to keep creating
	    // and destroying buffers of a constant size..
	    try
	    {
		if( _bufferQueue.Count > 0 )
		{
		    op.inData = _bufferQueue.Dequeue() as FWBuffer;
		    op.inData.Reset();
		}
	    } catch
	    {
		op.inData = null;
	    }

	    if(op.inData == null)
		op.inData = new FWBuffer( FuseWrapper.FUSE_MAX_IN );

	    return op;
	}

	private void EncodeMessage( ref FWBuffer buf, 
		ulong unique, int errorCode, BufferWritable outputArg )
	{
	    Debug.Log("encoding out header, unique {0}, error {1}", 
		    unique, errorCode);

	    int offset = buf.offset;

	    fuse_out_header header = new fuse_out_header();
	    header.len = 0; // don't konw message length yet..
	    header.unique = unique;
	    header.error = errorCode;

	    if(header.copyTo( buf ) <= 0)
	    {
		Error.Log("unique {0}, header too big for output buffer",
			unique);
		throw new ApplicationException(
			"Header too big for output buffer!?");
	    }

	    if(outputArg != null)
		outputArg.copyTo( buf );

	    // now go back and write the correct message length..
	    int finalOffset = buf.offset;
	    header.len = (uint)buf.initialized;
	    buf.offset = offset;

	    if(header.copyTo( buf ) <= 0)
	    {
		Error.Log("unique {0}, header too big for output buffer",
			unique);
		throw new ApplicationException(
			"Header too big for output buffer!?");
	    }

	    buf.offset = finalOffset;
	}

	/** abuse fuse_main to have it show usage information.
	  Unfortunatly, libfuse will then directly call exit(), so we can't
	  continue after this..
	*/
	static public void Usage()
	{
	    string[] argv = new string[2];
	    argv[0] = "sulf";
	    argv[1] = "-h";
	    fuse_main(argv.Length, argv, "");
	}

	/*
	    Some helper functions to import from libfuse..
	*/
	private const string FuseFSLibName = "fuse";

	[DllImport (FuseFSLibName, EntryPoint="fuse_mount")]
	private extern static int fuse_mount(string mountPoint, string opts);
	
	[DllImport (FuseFSLibName, EntryPoint="fuse_unmount")]
	private extern static void fuse_unmount(string mountPoint );

	[DllImport (FuseFSLibName, EntryPoint="fuse_main")]
	private extern static void fuse_main(int argc, 
		string[] argv, string bogus);
    }
}


