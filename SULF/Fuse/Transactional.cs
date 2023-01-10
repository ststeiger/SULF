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
    /** @interface Transactional
      @brief Simple transaction definition.

      Fuse.Channel supports transactions.  This are used for operations which
      may be canceled by the kernel.  beginTransaction() is called before
      returning the result to the kernel.  If the kernel accepts, then
      commitTransaction() is called, otherwise abortTransaction() is called.

      @see Fuse.Channel
    */
    public interface Transactional
    {
	/// start the transaction
	object beginTransaction();

	/** complete the operation 
	 -- argument is the return value from beginTransaction
	 */
	bool commitTransaction( object data );

	/** abort the transaction
	  -- argument is the return value from beginTransaction
	 */
	void abortTransaction( object data );
    }
}

