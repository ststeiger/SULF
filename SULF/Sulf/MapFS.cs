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
    /** @class MapFS
      @brief An in-memory filesystem node.

      The way MapFS handles child nodes depends on their type:
      - implements Fuse.Node:
          Passed through directly to Fuse.dll layer.
      - implements IDictionary:
	  Treated as a subdirectory and a MapFS object is created to serve its
	  contents.
      - all others:
          A string type is served by StringFileNode

      For any other types, ToString() is used to convert the object to a
      string, and StringFileNode is used.

      See example/HelloFS.cs
      @see StringFileNode
    */
    public class MapFS : GenericNode, Fuse.DirNode
    {
	IDictionary _map;
  
	public MapFS( string name, IDictionary map )
	    : base( name )
	{
	    // C# doesn't allow octal constants..
	    // 365 == 0555 octal
	    StatMode = (uint)ModeFlags.Directory | 365;
	    StatLink = 2;

	    _map = map;
	}
	
	public MapFS( string name )
	    : this( name, new System.Collections.Hashtable() )
	{
	}
	
	public MapFS()
	    : this( "MapFS" )
	{
	}

	/** Allow get or set of filesystem objects.
	*/
	public object this[ string name ]
	{
	    get { return _map[name]; }
	    set { _map[name] = value; }
	}

	// --- from DirNode interface
	virtual public long Open( uint flags )
	{
	    return 1; // we don't track opendir
	}

	virtual public int Release( ulong fh, uint flags )
	{
	    return 0;
	}

	virtual public int ReadDir( ulong fh, ulong offset, ReadDirCallback cb)
	{
	    if(offset > 0)
		return 0; // entire directory is read at once.. no continuation

	    // every directory has "." and ".."
	    cb(".", 0,(int)ModeFlags.Directory,0);
	    cb("..", 0,(int)ModeFlags.Directory,0);

	    IDictionaryEnumerator it = _map.GetEnumerator();
	    while(it.MoveNext())
	    {
		// TODO: if we can set inode, and type flags here, it should
		// require less communication round-trips.
		cb( (string)it.Key, 0,0,0 ); 
	    }

	    return 0;
	}

	virtual public Fuse.Node Lookup( string name, ref int errCode )
	{
	    errCode = 0;
	    object val = _map[ name ];

	    return ToNode( name, val );
	}

	private Fuse.Node ToNode( string name, object val )
	{
	    if(val is Fuse.Node)
		return (Fuse.Node)val;
	    else if(val is IDictionary)
		return new MapFS( name, (IDictionary)val );
	    else
	    {
		// create a generic string node...
		return new StringNode( name, val );
	    }
	}
    }

}

