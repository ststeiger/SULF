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
    /** Mode flags for some common types.  Used for testing / checking
     * Stat.mode value.
    */
    public enum ModeFlags
    {
	Socket       = 49152, //< octal 0140000
	SymLink      = 40960, //< octal 0120000
	RegularFile  = 32768, //< octal 0100000
	BlockDevice  = 24576, //< octal 0060000
	Directory    = 16384, //< octal 0040000
	CharDevice   = 8192,  //< octal 0020000
	FIFO         = 4096,  //< octal 0010000
	ReadWriteAll = 438,   //< octal 0000666, read+write permission for all
	ReadAll      = 292    //< octal 0000444, read permission for all
    }

    // XXX: Warning - this must be in sync with librawfs/rawfs.h : 
    //      struct FuseStat
    /** @struct Stat
      @brief Used for returning information about a file or directory.

      @warning This must match the definition in librawfs/rawfs.h, as it is
      used in P/Invoke calls.
    */
    public struct Stat
    {
	public uint mode;  // permissions bits
	public uint nlink; // number of hard links
	public uint uid;   // user id of owner
	public uint gid;   // group id of owner
	public uint rdev;  // device type (if inode device)
	public ulong size; // size in bytes
	public long blocks; // number of blocks
	public long atime; // time of last access
	public long mtime; // time of last data modification
	public long ctime; // time of last metadata change
	public long atimensec;
	public long mtimensec;
	public long ctimensec;

	public void SetTimes( System.DateTime time )
	{
	    // The times in this struct are relative to the Unix epoch -- Jan 1,
	    // 1970 at 1am.
	    // DateTime(1970,1,1,1,0,0) . ToFileTime() == 
	    const long UnixFileTimeBase = 116444736000000000;

	    // diff is in 100 nanosecond increments
	    long time_diff = time.ToFileTime() - UnixFileTimeBase;
	    long time_sec = time_diff / 10000000; // convert to seconds
	    long time_nsec = 100 * (time_diff - (time_sec * 10000000));
	    mtime = ctime = atime = time_sec;
	    mtimensec = ctimensec = atimensec = time_nsec;
	}
    }
}

