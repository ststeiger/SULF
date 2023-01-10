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

namespace Fuse
{
    /** @class StaleNodeException
      @brief Exception used internally by Fuse.dll.

      If the system requests a node by id, but that id is no longer in use,
      then a StaleNodeException exception is thrown.  This results in an ESTALE
      error being returned to the kernel for the operation.

      @internal
    */
    internal class StaleNodeException : ApplicationException
    {
	internal StaleNodeException( string message ) 
	    : base ( message )
	{
	}
    }
}
