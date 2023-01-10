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
    /** @interface Node
      @brief A generic node of a filesystem.

      A node can be any type of file (even non-existing files are represented
      by a Node instance).

      @author Valient Gough
    */
    public interface Node
    {	
	/// return local name (eg a file "/tmp/foo/bar" would return "bar");
	string Name { get; }

	/** Get statistics for this node.
	  @see man:stat(2)
	*/
	int GetStat( out Stat stat );

	// these are set and used by the reference counting mechanism.
	ulong NodeId { get; set; }
	ulong NodeGeneration { get; set; }
    }
    
    
    /** @interface MutableNode
      @brief A mutable (modifiable, writable) node of a filesystem.

      @author Valient Gough
     */
    public interface MutableNode : Fuse.Node
    {
	/** Change the permissions on the node.
	  @see man:chmod(2)
	*/
	int ChangeMode( uint mode );

	/** Change node ownership.
	  uid or gid may be -1 if not to be changed..
	*/
	int ChangeOwner( int uid, int gid );

	/** Change the Access (atime) and Modification (mtime) times.

	  @see man:utime(2)
	*/
	int UTime( ulong atime, ulong mtime );
    }
}

