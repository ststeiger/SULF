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
		                                                                                

using RLog;
using System;
using System.Collections;
using Fuse;

/** @namespace Fuse
  @brief Provides a low-level FUSE kernel <-> C# API translation layer.

  When implementing a filesystem based on the Fuse layer, you will need to
  create classes which implement the Fuse.FileSystem, Fuse.DirNode, and
  Fuse.FileNode interfaces.

  Fuse.Channel is used to communicate with the FUSE kernel module.
  Fuse.Reactor handles incoming messages and forwards them to the appropriate
  node.

  @see @ref fuse-design
  @author Valient Gough
*/
namespace Fuse
{
    /*! @class Reactor
      @brief Decodes and dispatches requests comming from the FUSE kernel.

      Reactor subscribes to events from a Fuse.Channel, decodes them, and
      converts them into requests on particular file or directory nodes
      (FileNode or DirNode).

      Typical usage for mounting a filesystem:
      @code
      // select a mount point where the filesystem will be visible
      string mountPoint = "/tmp/foo";

      // Create the filesystem which will handle the requests.
      Fuse.FileSystem fs = new RawFS.RawFileSystem( "/tmp/foo-raw" );

      // create the channel to the FUSE kernel module.
      Channel channel = new Fuse.Channel( mountPoint, fs );

      // create a reactor to dispatch the requests
      Fuse.Reactor reactor = new Fuse.Reactor();

      // subscribe to one or more channels..
      reactor.SubscribeTo( channel );

      // have channel process events until it is unmounted..
      channel.EventLoop();
      @endcode

      @see Channel
      @author Valient Gough
    */
    public class Reactor
    {
	// dispatch
	internal delegate object DE( Operation op );

	public const int ENTRY_REVALIDATE_TIME = 1; // 1 second
	public const int ATTR_REVALIDATE_TIME = 1;
	private static bool didSizeCheck = false;

	static LogNode Debug = Log.Debug;
	static LogNode Error = Log.Error;

	// Fuse 2.2 sends an INIT message after connecting.  If we don't get an
	// init first, then we must be connected to an unsupported version of
	// fuse...
	bool _gotInit = false;

	// mapping from all node ids to nodes
	private NodeMap _nodeMap = new NodeMap();

	private Hashtable _dispatch = new Hashtable();

	void addDispatch( fuse_opcode opcode, DE disp )
	{
	    _dispatch.Add( (int)opcode , disp );
	}

	public Reactor()
	{
	    addDispatch(fuse_opcode.FUSE_LOOKUP, new DE(do_lookup));
	    addDispatch(fuse_opcode.FUSE_FORGET, new DE(do_forget));
	    addDispatch(fuse_opcode.FUSE_GETATTR, new DE(do_getattr));
	    addDispatch(fuse_opcode.FUSE_SETATTR, new DE(do_setattr));
	    addDispatch(fuse_opcode.FUSE_READLINK, new DE(do_readlink));
	    addDispatch(fuse_opcode.FUSE_SYMLINK, new DE(do_symlink));
	    // GETDIR dropped from FUSE 2.2
	    //addDispatch(fuse_opcode.FUSE_GETDIR, new DE(do_getdir));
	    addDispatch(fuse_opcode.FUSE_MKNOD, new DE(do_mknod));
	    addDispatch(fuse_opcode.FUSE_MKDIR, new DE(do_mkdir));
	    addDispatch(fuse_opcode.FUSE_UNLINK, new DE(do_unlink));
	    addDispatch(fuse_opcode.FUSE_RMDIR, new DE(do_rmdir));
	    addDispatch(fuse_opcode.FUSE_RENAME, new DE(do_rename));
	    addDispatch(fuse_opcode.FUSE_LINK, new DE(do_link));
	    addDispatch(fuse_opcode.FUSE_OPEN, new DE(do_open));
	    addDispatch(fuse_opcode.FUSE_READ, new DE(do_read));
	    addDispatch(fuse_opcode.FUSE_WRITE, new DE(do_write));
	    addDispatch(fuse_opcode.FUSE_STATFS, new DE(do_statfs));
	    addDispatch(fuse_opcode.FUSE_RELEASE, new DE(do_release));
	    addDispatch(fuse_opcode.FUSE_FSYNC, new DE(do_fsync));
	    addDispatch(fuse_opcode.FUSE_SETXATTR, new DE(do_setxattr));
	    addDispatch(fuse_opcode.FUSE_GETXATTR, new DE(do_getxattr));
	    addDispatch(fuse_opcode.FUSE_LISTXATTR, new DE(do_listxattr));
	    addDispatch(fuse_opcode.FUSE_REMOVEXATTR, new DE(do_removexattr));
	    addDispatch(fuse_opcode.FUSE_FLUSH, new DE(do_flush));
	    // ---  New to FUSE 2.2 ---
	    addDispatch(fuse_opcode.FUSE_INIT, new DE(do_init));
	    addDispatch(fuse_opcode.FUSE_OPENDIR, new DE(do_opendir));
	    addDispatch(fuse_opcode.FUSE_READDIR, new DE(do_readdir));
	    addDispatch(fuse_opcode.FUSE_RELEASEDIR, new DE(do_releasedir));
	    // ---  New to FUSE 2.3 ---
	    addDispatch(fuse_opcode.FUSE_FSYNCDIR, new DE(do_fsync));
	}

	/** Subscribe the Reactor to events from the given Channel.
	*/
	public Reactor SubscribeTo( Channel channel )
	{
	    Channel.ProcessCommand incomingCmd = 
		new Channel.ProcessCommand( DispatchCommand );

	    channel.OnNewCommand += incomingCmd;
	    return this;
	}

	private object DispatchCommand( Fuse.Operation op )
	{
	    Debug.Log("unique: {0}, opcode: {1}",
		    op.header.unique, 
		    ((fuse_opcode)op.header.opcode).ToString());
	    if(!_gotInit && (op.header.opcode != (int)fuse_opcode.FUSE_INIT))
	    {
		Error.Log("Expecting INIT message.");
		return (int)ErrorCode.EPROTO;
	    }

	    try
	    {
		DE dispatch = (DE)_dispatch[ (int)op.header.opcode ];
		if(dispatch != null)
		{
		    return dispatch( op );
		} else
		{
		    Error.Log("unique: {0}, unknown opcode: {1}",
			op.header.unique, 
			op.header.opcode.ToString());
		    return (int)ErrorCode.ENOSYS;
		}
	    } catch( StaleNodeException e )
	    {
		Error.Log("caught StaleNodeException for unique {0}",
			op.header.unique);
		return (int)ErrorCode.ESTALE;
	    } catch( System.Exception e )
	    {
		Error.Log("Caught exception on opcode {0}: {1}",
			op.header.opcode.ToString(), e.Message );
		Error.Log("Exception type: " + e.GetType());
		Error.Log("Exception source: " + e.Source);
		Error.Log("Exception target: " + e.TargetSite);
		Error.Log("Exception Stack: " + e.StackTrace);
		return (int)ErrorCode.EIO;
	    }
	}

	private object do_lookup( Operation op )
	{
	    // 1 argument -- string
	    string name = op.inData.GetString();

	    Debug.Log("do_lookup for {0} in inode {1}", name, op.header.nodeid);

	    // The name isn't a fully qualified path - it is relative to the
	    // directory specified by the inode.  Lookup the inode first..
	    DirNode dirNode = LookupNodeId(op.fs, op.header.nodeid) as DirNode;
	    if(dirNode != null)
		return new LookupTransaction( _nodeMap, dirNode, name );
	    else
		return (int)ErrorCode.ENOENT; // path to file doesn't exist?
	}
	
	private object do_forget( Operation op )
	{
	    Debug.Log("forget for inode {0}", op.header.nodeid);
	    _nodeMap.DropNode( op.header.nodeid );
	    return null;
	}

	// Unlink removes the DNode entry, but not the inode entry..
	private object do_unlink( Operation op )
	{
	    // 1 argument -- string
	    string name = op.inData.GetString();

	    Debug.Log("unlink {0} / {1}", op.header.nodeid, name);

	    MutableDirNode wdn = 
		LookupNodeId( op.fs, op.header.nodeid ) as MutableDirNode;
	    if(wdn != null)
    		return wdn.Unlink( name );
	    else
		return (int)ErrorCode.EPERM;
	}
	
	
	private object do_release( Operation op )
	{
	    // should be called the same number of times as do_open()..
	    fuse_release_in arg = new fuse_release_in();
	    arg.copyFrom( op.inData );

	    Debug.Log("release on {0}, fh {1}", op.header.nodeid, arg.fh);

	    FileNode fnode = LookupNodeId(op.fs, op.header.nodeid) as FileNode;
	    if(fnode != null)
		return fnode.Release( arg.fh, arg.flags );
	    else
		return (int)ErrorCode.EIO; // shouldn't happen?
	}
	    
	private object do_releasedir( Operation op )
	{
	    // should be called the same number of times as do_open()..
	    fuse_release_in arg = new fuse_release_in();
	    arg.copyFrom( op.inData );

	    Debug.Log("releasedir on {0}, fh {1}", op.header.nodeid, arg.fh);

	    DirNode dnode = LookupNodeId( op.fs, op.header.nodeid ) as DirNode;
	    if(dnode != null)
		return dnode.Release( arg.fh, arg.flags );
	    else
		return (int)ErrorCode.EIO; // shouldn't happen?
	}
	
	private object do_getattr( Operation op )
	{
	    // no args
	    Debug.Log("getattr on {0}", op.header.nodeid);

	    // lookup Node based on inode value
	    Node node = LookupNodeId( op.fs, op.header.nodeid );
	    if(node != null)
	    {
		fuse_attr_out outArg = new fuse_attr_out();
		outArg.attr_valid = ATTR_REVALIDATE_TIME;
		outArg.attr_valid_nsec = 0;

		Fuse.Stat stat;
		int res = node.GetStat( out stat );
		if(res == 0)
		{
		    fuse_attr attr;
		    ConvertStat( stat, out attr );
		    outArg.attr = attr;

		    return outArg;
		} else
		    return res;
	    } else
		return (int)ErrorCode.EIO;
	}

	// this one call is used to do many things in FUSE..
	private object do_setattr( Operation op )
	{
	    // fuse_setattr_in arg
	    fuse_setattr_in arg = new fuse_setattr_in();
	    arg.copyFrom( op.inData );

	    Debug.Log("setattr on {0}", op.header.nodeid);

	    uint valid = arg.valid;
	    int res = (int)ErrorCode.EPERM;

	    MutableNode node = LookupNodeId( op.fs, op.header.nodeid )
		as MutableNode;
	    if(node != null)
	    {
		if(res == 0 && ((valid & FuseWrapper.FATTR_MODE) != 0))
		    res = node.ChangeMode( arg.attr.mode );
		if(res == 0 && 
			((valid & (FuseWrapper.FATTR_UID 
				  | FuseWrapper.FATTR_GID)) != 0))
		{
		    int uid = checked((int)arg.attr.uid);
		    int gid = checked((int)arg.attr.gid);
		    if((valid & FuseWrapper.FATTR_UID) == 0)
			uid = -1;
		    if((valid & FuseWrapper.FATTR_GID) == 0)
			gid = -1;
		    res = node.ChangeOwner( uid, gid );
		}
		if(res == 0 && ((valid & FuseWrapper.FATTR_SIZE) != 0))
		{
		    MutableFileNode fnode = node as MutableFileNode;
		    if(fnode != null)
			res = fnode.Truncate( arg.attr.size );
		    else
			res = (int)ErrorCode.EISDIR;
		}
		int timemask = FuseWrapper.FATTR_ATIME 
		    | FuseWrapper.FATTR_MTIME;
		if(res == 0 && ((valid & timemask) == timemask))
		{
		    res = node.UTime( arg.attr.atime, arg.attr.mtime );
		}
		if(res == 0)
		{
		    Fuse.Stat stat;
		    res = node.GetStat( out stat );

		    if(res == 0)
		    {
			fuse_attr attr;
			ConvertStat( stat, out attr );

			fuse_attr_out outarg = new fuse_attr_out();
			outarg.attr_valid = ATTR_REVALIDATE_TIME;
			outarg.attr_valid_nsec = 0;

			outarg.attr = attr;
			return outarg;
		    }
		}
	    }
	    return res;
	}
	
	private object do_readlink( Operation op )
	{
	    Debug.Log("readlink on {0}", op.header.nodeid);

	    // no args
	    FileNode node = LookupNodeId( op.fs, op.header.nodeid ) as FileNode;
	    if(node != null)
	    {
		string link = node.ReadLink();
		return new Fuse.StringArg( link );
	    } else
		return (int)ErrorCode.EINVAL;
	}
	
	private object do_mknod( Operation op )
	{
	    // 2 args: fuse_mknod_in and string
	    fuse_mknod_in arg = new fuse_mknod_in();
	    arg.copyFrom( op.inData );
	    string name = op.inData.GetString();

	    Debug.Log("mknod on inode {0}, name {1}, mode {2}",
		    op.header.nodeid, name, arg.mode);
	    
	    MutableDirNode dirNode = 
		LookupNodeId( op.fs, op.header.nodeid ) as MutableDirNode;
	    if(dirNode != null)
	    {
		return new MakeNodeTransaction( _nodeMap,
			dirNode, name, arg.mode, arg.rdev );
	    } else
    		return (int)ErrorCode.EPERM;
	}
	
	private object do_mkdir( Operation op )
	{
	    // 2 args:  fuse_mkdir_in, and string
	    fuse_mkdir_in arg = new fuse_mkdir_in();
	    arg.copyFrom( op.inData );
	    string name = op.inData.GetString();

	    Debug.Log("mkdir {0} / {1}", op.header.nodeid, name);

	    MutableDirNode dirNode = 
		LookupNodeId( op.fs, op.header.nodeid ) as MutableDirNode;
	    if(dirNode != null)
	    {
		return new MakeDirTransaction( _nodeMap, dirNode, 
			name, arg.mode );
	    } else
    		return (int)ErrorCode.EPERM;
	}

	// rmdir removes dnode, not inode
	private object do_rmdir( Operation op )
	{
	    string name = op.inData.GetString();

	    Debug.Log("rmdir {0} / {1}", op.header.nodeid, name);

	    MutableDirNode dirNode = 
		LookupNodeId( op.fs, op.header.nodeid ) as MutableDirNode;
	    if(dirNode != null)
	    {
		return dirNode.Unlink( name );
	    } else
		return (int)ErrorCode.EPERM;
	}
	
	private object do_symlink( Operation op )
	{
	    string name = op.inData.GetString();
	    string target = op.inData.GetString();

	    Debug.Log("symlink node {0} / {1} -> {2}",
		    op.header.nodeid, name, target);

	    // name is relative to the directory inode provided in header
	    MutableDirNode dirNode = 
		LookupNodeId( op.fs, op.header.nodeid ) as MutableDirNode;
	    if(dirNode != null)
	    {
		return new SymlinkTransaction( _nodeMap, dirNode, 
			name, target );
	    } else
		return (int)ErrorCode.EPERM;
	}

	// The filenames are relative to a directory node.
	// The old name is < header.nodeid : first string arg >
	// The new name is < arg.newdir : second string arg >
	// If the source and target directory nodes are the same, we can let
	// Node handle the rename.  If the rename crosses directory nodes, then
	// we have to check if both directory nodes are within the same
	// filesystem.
	private object do_rename( Operation op )
	{
	    fuse_rename_in arg = new fuse_rename_in();
	    arg.copyFrom( op.inData );
	    string oldname = op.inData.GetString();
	    string newname = op.inData.GetString();

	    Debug.Log( "rename {0} (dirnode {1}) -> {2} (dirnode {3})",
		    oldname, op.header.nodeid, 
		    newname, arg.newdir );

	    MutableDirNode srcDirNode = LookupNodeId( op.fs, op.header.nodeid ) 
		as MutableDirNode;

	    MutableDirNode dstDirNode = 
		(op.header.nodeid == arg.newdir) ?  srcDirNode : 
		LookupNodeId( op.fs, arg.newdir ) as MutableDirNode;

	    if(srcDirNode == null || dstDirNode == null)
		return (int)ErrorCode.EPERM;

	    int res = dstDirNode.Rename( srcDirNode, oldname, newname );
	    return res;
	}
	
	private object do_link( Operation op )
	{
	    fuse_link_in arg = new fuse_link_in();
	    arg.copyFrom( op.inData );
	    string name = op.inData.GetString();

	    // use StatNode, as link creates a new inode..
	    Debug.Log("link on oldNode {0}, inode {1}, name {2}",
		    arg.oldnodeid, op.header.nodeid, name);

	    return (int)ErrorCode.ENOSYS;
	}
	
	private object do_open( Operation op )
	{
	    fuse_open_in arg = new fuse_open_in();
	    arg.copyFrom( op.inData );
 
	    Debug.Log("open {0}, mode {1}", op.header.nodeid, arg.flags);

	    Node node = LookupNodeId( op.fs, op.header.nodeid );
	    if(node != null)
	    {
		FileNode fnode = node as FileNode;
		if(fnode != null)
		    return new OpenTransaction( fnode, arg.flags );
		else
		    return (int)ErrorCode.EISDIR; // not a file anyway..
	    } else
		    return (int)ErrorCode.EIO; // internal error..
	}

	// opendir looks just like open, but target is a directory..
	private object do_opendir( Operation op )
	{
	    fuse_open_in arg = new fuse_open_in();
	    arg.copyFrom( op.inData );
  
	    Debug.Log("opendir {0}, mode {1}", op.header.nodeid, arg.flags);

	    Node node = LookupNodeId( op.fs, op.header.nodeid );
	    if(node != null)
	    {
		DirNode dnode = node as DirNode;
		if(dnode != null)
		    return new OpenTransaction( dnode, arg.flags );
		else
		    return (int)ErrorCode.ENOTDIR; // not a dir anyway..
	    } else
    		return (int)ErrorCode.EIO; // internal error..
	}


	/*  Called when close() is called on an open file.  Doesn't mean that
	 *  the file is no longer open (clones), which is what RELEASE is for.
	 *  The return value is used as the return value from the close() call.
	*/
	private object do_flush( Operation op )
	{
	    fuse_flush_in arg = new fuse_flush_in();
	    arg.copyFrom( op.inData );
  
	    Debug.Log("flush {0}, handle {1}", op.header.nodeid, arg.fh);
  
	    FileNode node = LookupNodeId( op.fs, op.header.nodeid ) as FileNode;
	    if(node != null)
		return node.Flush( arg.fh );
	    else
		return (int)ErrorCode.EIO;
	}
	
	private object do_read( Operation op )
	{
	    fuse_read_in arg = new fuse_read_in();
	    arg.copyFrom( op.inData );
  
	    Debug.Log("read {0}, handle {1}, offset {2}, size {3}", 
		    op.header.nodeid, arg.fh, arg.offset, arg.size);

	    FileNode node = LookupNodeId( op.fs, op.header.nodeid ) as FileNode;
	    if(node != null)
	    {
		// TODO: read speed would be increased if we used a buffer
		// queue here as well..
		FWBuffer outbuf = new FWBuffer( checked((int)arg.size) );
		int res = node.Read( arg.fh, outbuf, 
			arg.size, arg.offset );
		if(res > 0)
		{
		    res = 0;
		    outbuf.Rewind();
		    return outbuf;
		}

	    	return res;
	    } else
		return (int)ErrorCode.EIO;
	}

	// same as do_read, except with directory target
	private object do_readdir( Operation op )
	{
	    fuse_read_in arg = new fuse_read_in();
	    arg.copyFrom( op.inData );
  
	    Debug.Log("readdir {0}, handle {1}, offset {2}", 
		    op.header.nodeid, arg.fh, arg.offset);

	    DirNode dnode = LookupNodeId( op.fs, op.header.nodeid ) as DirNode;
	    if(dnode != null)
	    {
		// TODO: would be faster to use a buffer queue..
		FWBuffer buffer = new FWBuffer( 256 );
		FileBuffer fb = new FileBuffer( buffer );

		int res = dnode.ReadDir( arg.fh, arg.offset, fb.addFile );
		if(res == 0)
		{
		    return fb;
		} else
		    return res;
	    } else
		return (int)ErrorCode.EIO;
	}
	
	private object do_write( Operation op )
	{
	    fuse_write_in arg = new fuse_write_in();
	    arg.copyFrom( op.inData );
  
	    Debug.Log("write {0}, handle {1}, offset {2}, size {3}", 
		    op.header.nodeid, arg.fh, arg.offset, arg.size);

	    MutableFileNode node = 
		LookupNodeId( op.fs, op.header.nodeid ) as MutableFileNode;
	    if(node != null)
	    {
		int res = node.Write( arg.fh, op.inData, 
			arg.size, arg.offset );
		if(res >= 0)
		{
		    fuse_write_out outarg = new fuse_write_out();
		    outarg.size = checked((uint)res);
		    return outarg;
		}
	    	return res;
	    } else
	    {
		// this case shouldn't happen anyway -- open() shouldn't
		// have returned success on a request for write access..
		return (int)ErrorCode.EINVAL; 
	    }
	}
	
	private object do_statfs( Operation op )
	{
	    Debug.Log("do_statfs called");
	    // no args

	    FileSystem fs = op.fs;

	    int res = (int)ErrorCode.ENOSYS;
	    if( fs != null )
	    {
		// query in StatFS structure
		StatFS stat;
		res = fs.Statfs( out stat );

		// convert to fuse_kstatfs structure..
		fuse_kstatfs kstat;
		ConvertStatFS( stat, out kstat );

		fuse_statfs_out outarg = new fuse_statfs_out();
		outarg.st = kstat;
		Debug.Log("returning type fuse_statfs_out");
		return outarg;
	    }
	    return res;
	}
	
	private object do_fsync( Operation op )
	{
	    fuse_fsync_in arg = new fuse_fsync_in();
	    arg.copyFrom( op.inData );
 
	    Debug.Log("fsync {0}, handle {1}, flags {2}", 
		    op.header.nodeid, arg.fh, arg.fsync_flags);

	    MutableFileNode node = LookupNodeId( op.fs, op.header.nodeid ) 
		as MutableFileNode;
	    if(node != null)
		return node.Sync( arg.fh, arg.fsync_flags );
	    else
		return (int)ErrorCode.EINVAL;
	}
	
	private object do_setxattr( Operation op )
	{
	    fuse_setxattr_in arg = new fuse_setxattr_in();
	    arg.copyFrom( op.inData );
	    string attrKey = op.inData.GetString();

	    Debug.Log("setxattr {0}, key {1}, size {2}",
		    op.header.nodeid, attrKey, arg.size);

	    MutableXAttrNode node = LookupNodeId( op.fs, op.header.nodeid ) 
		as MutableXAttrNode;
	    if(node != null)
		return node.SetXAttr( attrKey, op.inData, arg.size, arg.flags );
	    else
		return (int)ErrorCode.ENOTSUP;
	}
	
	private object do_getxattr( Operation op )
	{
	    fuse_getxattr_in arg = new fuse_getxattr_in();
	    arg.copyFrom( op.inData );
	    string attrKey = op.inData.GetString();
  
	    Debug.Log("getxattr {0}, key {1}", op.header.nodeid, attrKey);

	    XAttrNode node = LookupNodeId( op.fs, op.header.nodeid )
		as XAttrNode;
	    return (int)ErrorCode.ENOSYS;
	}
	
	private object do_listxattr( Operation op )
	{
	    // yeah, not a typo - listxattr also uses fuse_getxattr_in
	    fuse_getxattr_in arg = new fuse_getxattr_in();
	    arg.copyFrom( op.inData );
  
	    Debug.Log("listxattr {0}", op.header.nodeid);

	    return (int)ErrorCode.ENOSYS;
	}
	
	private object do_removexattr( Operation op )
	{
	    string key = op.inData.GetString();
   
	    Debug.Log("removexattr {0}, key {1}", op.header.nodeid, key);

	    return (int)ErrorCode.ENOSYS;
	}

	private object do_init( Operation op )
	{
	    _gotInit = true;

	    fuse_init_in_out arg = new fuse_init_in_out();
	    arg.copyFrom( op.inData );
	    
	    Debug.Log("Got INIT (recv: {0}.{1}, this: {2}.{3})", 
		    arg.major, arg.minor,
		    FuseWrapper.FUSE_KERNEL_VERSION,
		    FuseWrapper.FUSE_KERNEL_MINOR_VERSION);
	    
	    // check results..
	    if(arg.major != FuseWrapper.FUSE_KERNEL_VERSION)
	    {
		// no backward compatibility for now..
		Error.Log("Invalid fuse module version: {0}.{1} (current {2})",
			arg.major, arg.minor, 
			FuseWrapper.FUSE_KERNEL_VERSION);

		return (int)ErrorCode.EINVAL;
	    }

	    // send back a response with our version info
	    arg.major = FuseWrapper.FUSE_KERNEL_VERSION;
	    arg.minor = FuseWrapper.FUSE_KERNEL_MINOR_VERSION;

	    return arg;
	}
	    


	static public void ConvertStat( Fuse.Stat stat, out fuse_attr attr )
	{
	    attr = new fuse_attr();
	    attr.mode = stat.mode;
	    attr.nlink = stat.nlink;
	    attr.uid = stat.uid;
	    attr.rdev = stat.rdev;
	    attr.size = stat.size;
	    attr.blocks = checked((ulong)stat.blocks);
	    attr.atime = checked((ulong)stat.atime);
	    attr.mtime = checked((ulong)stat.mtime);
	    attr.ctime = checked((ulong)stat.ctime);
	    attr.atimensec = checked((uint)stat.atimensec);
	    attr.mtimensec = checked((uint)stat.mtimensec);
	    attr.ctimensec = checked((uint)stat.ctimensec);
	}
	
	static public void ConvertStat( fuse_attr attr, out Fuse.Stat stat )
	{
	    stat = new Fuse.Stat();
	    stat.mode = attr.mode;
	    stat.nlink = attr.nlink;
	    stat.uid = attr.uid;
	    stat.rdev = attr.rdev;
	    stat.size = attr.size;
	    stat.blocks = checked((long)attr.blocks);
	    stat.atime = checked((long)attr.atime);
	    stat.mtime = checked((long)attr.mtime);
	    stat.ctime = checked((long)attr.ctime);
	    stat.atimensec = checked((long)attr.atimensec);
	    stat.mtimensec = checked((long)attr.mtimensec);
	    stat.ctimensec = checked((long)attr.ctimensec);
	}
	
	static public void ConvertStatFS( Fuse.StatFS stat, 
		out fuse_kstatfs kstat )
	{
	    kstat = new fuse_kstatfs();
	    kstat.bsize = checked((uint)stat.blockSize);
	    kstat.blocks = checked((ulong)stat.blocks);
	    kstat.bfree = checked((ulong)stat.blocksFree);
	    kstat.bavail = checked((ulong)stat.blocksAvail);
	    kstat.files = checked((ulong)stat.files);
	    kstat.ffree = checked((ulong)stat.filesFree);
	    kstat.namelen = checked((uint)stat.namelen);
	}
	
	/*! Find a node given the node id.
	  Throws a StaleNodeException if the node doesn't exist..
	*/
	internal Node LookupNodeId( FileSystem fs, ulong nodeId )
	{
	    Node node;
	    if(nodeId == 1)
	    {
		// Node 1 is special case, it means root node.
		node = fs.RootNode as Node;
	    } else
    		node = _nodeMap[ nodeId ];

	    if(node != null)
		return node;
	    else
		throw new StaleNodeException( "stale node in LookupNodeId" );
	}

    }
}

