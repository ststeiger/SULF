/*****************************************************************************
 * Author:   Valient Gough <vgough@pobox.com>
 *
 *****************************************************************************
 * Copyright (c) 2003-2005, Valient Gough
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
    /** @class FileSystem
      @brief Simple interface for a generic filesystem.

      This provides a very basic interface for a filesystem implementation.
      Basically a filesystem contains just a way to get StatFS information, a
      way to get the root node (which should be a DirNode), and a way to get
      the mountpoint.

      @author Valient Gough
    */
    abstract public class FileSystem
    {
	/** Return node corresponding to the root directory of the filesystem.
	*/
	abstract public Fuse.DirNode RootNode { get; }

	/** The filesystem's name.  This can be an implementation name, such as
	 * "RawFS".
	*/
	abstract public string Name { get; }

	/** If special mount flags are required for operation, they should be
	 * provided by this property.
	*/
	virtual public string[] RequiredMountArgs { get{ return null;} }

	/** Get filesystem statistics
	  @see man:statfs(2)
	*/
	virtual public int Statfs( out StatFS stat )
	{
	    stat = new StatFS();
	    stat.namelen = 255;
	    stat.blockSize = 512;
	    return 0;
	}
    }
}

