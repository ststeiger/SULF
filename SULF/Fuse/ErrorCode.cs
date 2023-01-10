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
    // These values are taken from a linux system, may not be compatible with
    // others..

    /** @enum ErrorCode
      @brief Common error codes (negated).

      These are common error codes (negated for convenience) which may be
      returned by some filesystem operations.  Since FUSE is meant to work with
      Linux, the error codes are made to match the Linux error codes.

      Note that for most FUSE calls, an error code of 0 indicates success, and
      a negative value indicates the error code.  This is different from most
      Unix system calls which may return -1 on error and set @e errno to
      indicate a (positive valued) error code.
    */
    public enum ErrorCode
    {
	Success = 0,
	EPERM = -1,
	ENOENT = -2,
	EINTR = -4, //< interrupted system call
	EIO = -5,
	EAGAIN = -11, //< try again, temporary failure
	ENOMEM = -12, //< out of memory
	EXDEV = -18, //< not the same filesystem
	ENODEV = -19, //< no such device
	ENOTDIR = -20, //< not a directory
	EISDIR = -21, //< is a directory
	EINVAL = -22, //< invalid argument
	ENOSYS = -38,
	EPROTO = -71, //< protocol error
	ENOTSUP = -95, //< not supported
	ESTALE = -116 //< stale inode number...
    }

}


