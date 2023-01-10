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
		                                                                                

using Fuse;
using System;

/*
    Calls into fuse library to show the options which were build in..
*/
public class UsageTest
{
    static public int Main( string[] args )
    {
	DateTime unixStart = new DateTime(1970,1,1,1,0,0);

	Console.WriteLine("Unix start date: {0}", unixStart.ToFileTime());

	Channel.Usage();

	return 0;
    }
}

