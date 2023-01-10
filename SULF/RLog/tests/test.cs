/*****************************************************************************
 * Author:   Valient Gough <vgough@pobox.com>
 *
 *****************************************************************************
 * Copyright (c) 2004, Valient Gough
 * 
 * This program is free software; you can distribute it and/or modify it under 
 * the terms of the GNU General Public License (GPL), as published by the Free
 * Software Foundation; either version 2 of the License, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 */

using RLog;
using System;
using System.Reflection;
using System.Diagnostics;

// major.minor.build.revision
[assembly: AssemblyVersionAttribute("0.1.0.0")]

public class Test
{
    static private LogNode Debug = Log.Debug;
    static private LogNode Info = Log.Info;
    static private LogNode Error = Log.Error;
    static private LogNode DebugTest = Log.Lookup("debug/test", LogLevel.Debug);
    static private LogNode FooBar = Log.Lookup("foo/bar/bogus", LogLevel.Info);

    static void Main()
    {
	Debug.Log("testing.. should not be visible - no subscribed channels");

	// subscribe to the debug channel..
	Log.LookupGlobal("debug").Subscribe( 
		new RLog.PublishDelegate( LogSubscriber ) );


	for(int i=0; i<10; ++i)
	    Debug.Log("i = {0}", i);

	Debug.Log("Main exiting");
	Debug.Log("hello {0}", new object[] { "world" });

	DebugTest.Log("logged to debug/test channel");
	FooBar.Log("this should NOT be visible - no subscribers!");

	Error.Log("should not be visible...");

	// now subscribe to warning+
	Log.Warning_Plus.Subscribe( new RLog.PublishDelegate( LogSubscriber ));

	Error.Log("SHOULD be visible...");
	Info.Log("should NOT be visible...");
    }

    static void LogSubscriber(RLog.Publication data)
    {
	Console.WriteLine("{0} - {1}: {2}", data.sourceNode.Level.ToString(),
		data.location, data.message);
    }
}

