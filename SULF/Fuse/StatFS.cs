/*****************************************************************************
 * Author:   Valient Gough <vgough@pobox.com>
 *
 *****************************************************************************
 * Copyright (c) 2003, Valient Gough
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
    /** @struct StatFS
      @brief Structure for returning information about a filesystem.

      @warning This must match the definition in librawfs/rawfs.h, as it is
      used in P/Invoke calls.
    */
    public struct StatFS
    {
	public int blockSize;
	public long blocks;
	public long blocksFree;
	public long blocksAvail;
	public long files;
	public long filesFree;
	public int namelen;
    }

}


