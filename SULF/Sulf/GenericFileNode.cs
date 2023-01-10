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
    /** @class GenericFileNode
      @brief Provide a default implementation of methods.
    */
    public abstract class GenericFileNode : GenericNode, Fuse.FileNode
    {
	public GenericFileNode( string name )
	    : base( name )
	{
	}

	virtual public string ReadLink()
	{
	    return null;
	}

	virtual public long Open(uint flags)
	{
	    return 0;
	}

	virtual public int Release(ulong fh, uint flags)
	{
	    return 0;
	}

	virtual public int Flush(ulong fh)
	{
	    return 0;
	}

	// Must be implemented by derived class..
	abstract public int Read(ulong fh, FWBuffer data, 
		uint size, ulong offset);
    }

}

