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

using System;
using Fuse;

namespace Sulf
{
    // XXX XXX XXX Having the Fuse.FileNode derivation here shouldn't be
    // necessary, but Mono seems to have a problem calling GetStat if it isn't
    // here.  Reported as mono bug #74773
    /** @class StringNode
      @brief Provides a read-only Fuse.FileNode for a string.
    */
    public class StringNode : GenericFileNode, Fuse.FileNode
    {
	object _data;

	public StringNode( string name, object data )
	    : base( name )
	{
	    StatMode = (int)ModeFlags.RegularFile | (int)ModeFlags.ReadAll;
	    _data = data;
	}

	override public int Read(ulong fh, FWBuffer data, 
		uint size, ulong offset)
	{
	    string val;
	    if(_data is string)
		val = (string)_data;
	    else
		val = _data.ToString();

	    int off = checked((int)offset);

	    if(off < val.Length)
	    {
		int len = val.Length - off;
		if(len > size)
		    len = checked((int)size);
		data.AddSubString( val, off, len );
		return len;
	    } else
		return 0;
	}

	override public int GetStat( out Fuse.Stat stat )
	{
	    int res = base.GetStat( out stat );
	   
	    if(res == 0)
	    {
		string val;
		if(_data is string)
		    val = (string)_data;
		else
		    val = _data.ToString();
		stat.size = (ulong)val.Length;

		// set times to now - this should probably be a GenericFileNode
		// thing...
		stat.SetTimes( System.DateTime.Now );
	    }
	    return res;
	}
    }
}
