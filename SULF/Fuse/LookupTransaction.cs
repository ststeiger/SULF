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

namespace Fuse
{
    /** @class LookupTransaction
      @brief Base transaction implementation for types returning fuse_entry_out

      If the lookup is aborted, then NodeMap.DropNode() is called to drop the
      node.
    */
    public class LookupTransaction : Transactional
    {
	protected int res = 0;
	private NodeMap nodeMap;
	private DirNode dirNode;
	private string name;
	private fuse_entry_out outArg = new fuse_entry_out();

	public LookupTransaction( NodeMap nodeMap, 
		DirNode dirNode, string name )
	{
	    this.nodeMap = nodeMap;
	    this.dirNode = dirNode;
	    this.name = name;
	}

	virtual public object beginTransaction()
	{
	    if(res == 0)
	    {
		res = (int)ErrorCode.EINVAL;
		Node node = dirNode.Lookup( name, ref res );
		if(node == null)
		    return res;

		Fuse.Stat stat;
		res = node.GetStat( out stat );

		if(res == 0)
		{
		    // have the map remember this node and fill in the outarg
		    // struct
		    nodeMap.TrackNode( node );
		    outArg.nodeid = node.NodeId;
		    outArg.generation = node.NodeGeneration;
		    outArg.entry_valid = Fuse.Reactor.ENTRY_REVALIDATE_TIME;
		    outArg.entry_valid_nsec = 0;
		    outArg.attr_valid = Fuse.Reactor.ATTR_REVALIDATE_TIME;
		    outArg.attr_valid_nsec = 0;

		    fuse_attr attr;
		    Fuse.Reactor.ConvertStat( stat, out attr );
		    outArg.attr = attr;

		    return outArg;
		}
	    }
    	    return res;
	}

	virtual public bool commitTransaction( object transaction )
	{
	    return true;
	}

	virtual public void abortTransaction( object transaction )
	{
	    if(outArg.nodeid > 1)
		nodeMap.DropNode( outArg.nodeid );
	}
    }

    /** @class MakeNodeTransaction
      @brief Transaction for Mknod.
     */
    public class MakeNodeTransaction : LookupTransaction
    {
	public MakeNodeTransaction( NodeMap nodeMap, MutableDirNode dirNode, 
		string name, uint mode, uint rdev )
	    : base( nodeMap, dirNode, name )
	{
	    res = dirNode.Mknod( name, mode, rdev );
	}
    }
    
    /** @class MakeDirTransaction
      @brief Transaction for Mkdir.
     */
    public class MakeDirTransaction : LookupTransaction
    {
	public MakeDirTransaction( NodeMap nodeMap, MutableDirNode dirNode, 
		string name, uint mode )
	    : base( nodeMap, dirNode, name )
	{
	    res = dirNode.Mkdir( name, mode );
	}
    }
    
    /** @class SymlinkTransaction
      @brief Transaction for symlink.
     */
    public class SymlinkTransaction : LookupTransaction
    {
	public SymlinkTransaction( NodeMap nodeMap, MutableDirNode dirNode, 
		string name, string target )
	    : base( nodeMap, dirNode, name )
	{
	    res = dirNode.SoftLink( name, target );
	}
    }
}

