/*****************************************************************************
 * Author:   Valient Gough <vgough@pobox.com>
 *
 *****************************************************************************
 * Copyright (c) 2004, Valient Gough
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
		                                                                                

using Fuse;
using RLog;
using System.Collections;

namespace Fuse
{
    /** @class OpenTransaction
      @brief Transaction for Open command.
    */
    public class OpenTransaction : Transactional
    {
	Node node;
	uint flags;
	static LogNode Debug = Log.Debug;
	bool isDir;

	public OpenTransaction( FileNode fnode, uint flags )
	{
	    this.node = fnode;
	    this.flags = flags;
	    this.isDir = false;
	}
	
	public OpenTransaction( DirNode dnode, uint flags )
	{
	    this.node = dnode;
	    this.flags = flags;
	    this.isDir = true;
	}

	public object beginTransaction()
	{
	    long res = isDir ? ((DirNode)node).Open( flags )
		: ((FileNode)node).Open( flags );
	    Debug.Log("in OpenTransaction.beginTransaction, got fd {0}", res);
	    if(res < 0)
		return (int)res;
	    else
	    {
		fuse_open_out outarg = new fuse_open_out();
		outarg.fh = checked((ulong)res);
		return outarg;
	    }
	}

	public bool commitTransaction( object data )
	{
	    return true;
	}

	/** If the command was aborted, then issue a Release to counteract the
	 * effect of an Open..
	*/
	public void abortTransaction( object data )
	{
	    Debug.Log("in OpenTransaction.abortTransaction");
	    fuse_open_out outarg = data as fuse_open_out;
	    if(outarg != null)
	    {
		if(isDir)
		    ((DirNode)node).Release( outarg.fh, flags );
		else
		    ((FileNode)node).Release( outarg.fh, flags );
	    }
	}
    }
}

