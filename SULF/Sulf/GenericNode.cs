/*****************************************************************************
 * Author:   Valient Gough <vgough@pobox.com>
 *
 *****************************************************************************
 * Copyright (c) 2005, Valient Gough
 *
 * This library is free software; you can distribute it and/or modify it under
 * the terms of the GNU General Public License (GPL), as published by the Free
 * Software Foundation; either version 2 of the License, or (at your option)
 * any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GPL in the file COPYING for more
 * details.
 */

using System.Collections;
using Fuse;

namespace Sulf
{
    /** @class GenericNode
      @brief Provides default implementation for Fuse.Node

      Used as base class for Sulf nodes.
    */
    public class GenericNode : Fuse.Node
    {
	ulong _nodeId;
	ulong _nodeGeneration;
	
	string _name;
	uint _statMode = (uint)ModeFlags.ReadAll;
	uint _statLink = 1;

	protected uint StatMode
	{
	    get { return _statMode; }
	    set { _statMode = value; }
	}
	
	protected uint StatLink
	{
	    get { return _statLink; }
	    set { _statLink = value; }
	}


	public GenericNode( string name )
	{
	    _name = name;
	}

	virtual public string Name 
	{
	    get { return _name; }
	}

	virtual public int GetStat( out Stat stat )
	{
	    stat = new Fuse.Stat();
	    stat.mode = _statMode;
	    stat.nlink = _statLink;
	    return 0;
	}

	virtual public ulong NodeId 
	{ 
	    get { return _nodeId; }
	    set { _nodeId = value; }
	}
	
	virtual public ulong NodeGeneration 
	{ 
	    get { return _nodeGeneration; }
	    set { _nodeGeneration = value; }
	}
    }

}

