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
    /*! @interface FileNode
      @brief Interface for operation on file types.

      The FileNode interface defines operations which are possible on various
      types of files.  All operations may not be valid for every file type.
    */
    public interface FileNode : Fuse.Node
    {
	/// If the file is a symbolic link, return the link target.
	string ReadLink();

	/** Open the file with the given flags and return a file descriptor.
	  @return File descriptor on success (> 0), or a negative error code on
	  failure (See Fuse.ErrorCode)
	*/
	long Open( uint flags );

	/** Release the previously opened file descriptor.
	  @return Zero on success, or a negative error code on failure (See
	  Fuse.ErrorCode)
	*/
	int Release( ulong fh, uint flags );

	/** Flush is called on a close()..  However this does not mean that the
	 filehandle should be closed, because it might have been cloned.
	 Instead Release() will be called when the last reference is closed.
	 */
	int Flush( ulong fh );

	int Read( ulong fh, FWBuffer data, uint size, ulong offset );
    }
    
    /*! @interface MutableFileNode
      @brief Interface for operation on modifiable files.
    */
    public interface MutableFileNode : FileNode, Fuse.MutableNode
    {
	int Write( ulong fh, FWBuffer data, uint size, ulong offset );

	/** Truncate the file.

	  @note In Unix filesystems, truncation may @e enlarge a file as well
	  as shrink it.  If the file is enlarged, the new data should be filled
	  with null bytes ('\0').
	*/
	int Truncate( ulong size );

	int Sync( ulong fh, uint flags );
    }
}

