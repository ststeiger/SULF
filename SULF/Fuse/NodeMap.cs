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
		                                                                                
using System;
using System.Collections;
using System.Threading;
using RLog;

namespace Fuse
{
    /** @class NodeMap
      @brief Maintains mapping from integer node id to Fuse.Node.

    */
    public class NodeMap
    {
	object syncObj = new object();

	// mapping from (ulong)nodeId to Node
	internal Hashtable _nodeIdToNode;

	static private ulong _currentNodeId = 2;
	static private ulong _currentGeneration = 0;

	static LogNode Debug = Log.Debug;
	static LogNode Error = Log.Error;


	public NodeMap()
	{
	    _nodeIdToNode = new Hashtable();
	    //_nodeIdToNode = Hashtable.Synchronized( new Hashtable() );
	}

	/** Drop the specified node from the map.
	*/
	public void DropNode( ulong nodeId )
	{
	    Debug.Log("dropping node id {0}", nodeId );
	    Node node;
	    lock(syncObj)
	    {
		node = (Node)_nodeIdToNode[ nodeId ];
		if(node != null)
		{
		    _nodeIdToNode.Remove( nodeId );
		    Error.Assert(node.NodeId == nodeId, "nodeId mismatch");
		    node.NodeId = 0;
		}
	    }

	    if(node == null)
		Error.Log("DropNode: unable to find node id {0}", nodeId);
	    else
	    {
		// call Dispose if item is IDisposable
		IDisposable disp = node as IDisposable;
		if(disp != null)
		    disp.Dispose();
	    }
	}

	public Node this[ ulong nodeId ]
	{
	    get
	    {
		Debug.Log("lookup of node id {0}", nodeId );
		Node node;
		lock(syncObj)
		{
		    node = (Node)_nodeIdToNode[ nodeId ];
		}
		return node;
	    }
	}

	// Lookup node, assign inode number if it doesn't have one.
	public void TrackNode( Node node )
	{
	    if(node.NodeId == 0)
		AssignNodeId( node );
	}
	
	/** Assign an id to a node.  This assigned the next unused nodeId to
	 the node.

	  If this ever becomes a bottleneck, we could also use an interval map
	  do decrease lookup time..
	 */
	private void AssignNodeId( Fuse.Node node )
	{
	    ulong newId;
	    ulong newGen = _currentGeneration;

	    lock(syncObj)
	    {
		do
		{
		    newId = _currentNodeId++;
		    if(newId < 0)
		    {
			newGen = _currentGeneration++;
			_currentNodeId = 2;
		    }
		} while( (newId <= 1) || _nodeIdToNode.Contains( newId ));

		node.NodeId = newId;
		node.NodeGeneration = newGen;

		_nodeIdToNode[ newId ] = node;
	    }

	    Debug.Log("assigning node {0} id {1}", node.Name, newId );
	}
    }
}

