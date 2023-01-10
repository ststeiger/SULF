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
    /* Called by ReadDir method to fill in directory information.
       Returns != 0 if reading should stop.
    */
    public delegate int ReadDirCallback( string name, 
	    ulong inode, uint type, ulong nextOffset );

    /** @interface DirNode
      @brief Interface for operations on directories.
     */
    public interface DirNode : Fuse.Node
    {
	// return file handle (or < 0 for error)
	long Open( uint flags );
	int Release( ulong fh, uint flags );
	int ReadDir( ulong fh, ulong offset, ReadDirCallback filler );

	// if null is returned, then errCode will be used as the error code.
	Node Lookup( string name, ref int errCode );
    }

    /** @interface MutableDirNode
      @brief Interface for directories which support writes.
     */
    public interface MutableDirNode : Fuse.DirNode, Fuse.MutableNode
    {
	/// rename a node (possibly from another directory)
	int Rename( MutableDirNode source, string oldName, string newName );

	/// hard link between files
	int Link( DirNode source, string oldNode, string newName );

	/// soft link between files
	int SoftLink( string name, string target );

	/// create a new file
	int Mknod( string name, uint mode, uint rdev );

	/// create a new directory
	int Mkdir( string name, uint mode );

	/// remove file or directory..
	int Unlink( string name );
    }
}
 
