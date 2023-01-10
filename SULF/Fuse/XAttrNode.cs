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

namespace Fuse
{
    /** @interface XAttrNode
      @brief Interface for nodes supporting eXtended Attribute calls.
    */
    public interface XAttrNode : Fuse.Node
    {
	int GetXAttr( string attrKey, FWBuffer output, int maxSize);
    }

    /** @interface MutableXAttrNode
      @brief Interface for nodes supporting eXtended Attribute calls.
    */
    public interface MutableXAttrNode : XAttrNode
    {
	int SetXAttr( string attrKey, FWBuffer inValue, 
		uint size, uint flags );
    }
}

