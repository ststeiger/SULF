/*****************************************************************************
 * Author:   Valient Gough <vgough@pobox.com>
 *
 *****************************************************************************
 * Copyright (c) 2005, Valient Gough
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
                                                                               
using System;
using Fuse;
using RLog;

namespace HelloFS
{
    public class HelloFS
    {
	static public int Main( string[] args )
	{
	    if(args.Length != 1)
	    {
		Console.WriteLine("Usage: hellofs.exe mountPoint");
		return 1;
	    }

	    // log everything at debug level and above to LogSubscriber
	    // method..
	    Log.Debug_Plus.Subscribe( new RLog.PublishDelegate(LogSubscriber) );

	    string mountPoint = args[0];

	    // make a simple filesystem, containing a "hello" file
	    Sulf.MapFS root = new Sulf.MapFS();
	    // have a file 'hello' in the top-level..
	    root["hello"] = "Hello world!\n";
	    root["README"] = 
		"This file comes from the 'hello' filesystem example.\n"
		+"It is part of the SULF C# project which builds on FUSE.\n"
		+"For more information, see http://pobox.com/~vgough/sulf \n"
		+"\n"
		+"Also, try \"cat hello\"\n";

	    Channel channel = Sulf.Mounter.Mount( "HelloFS", root, mountPoint );

	    // enter channel event loop, which continues until the filesystem
	    // is unmounted..
	    channel.EventLoop();

	    // cleanup...
	    Console.WriteLine("hellofs.exe exiting..");
	    return 0;
	}

	static void LogSubscriber( RLog.Publication data )
	{
	    Console.WriteLine("{0}: {1}", data.location, data.message);
	}
    }
}

