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
		                                                                                

namespace Fuse
{
    /** Wrapper around a simple string to provide a BufferWritable interface.
    */
    public class StringArg : BufferWritable
    {
	private string arg;

	public StringArg( string arg )
	{
	    this.arg = arg;
	}

	public int copyTo( FWBuffer buf )
	{
	    return buf.AddString( arg );
	}
    }
}
