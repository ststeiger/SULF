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

using System;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
    
namespace RLog
{
    public enum LogLevel
    {
	Undef    = 0, //!< undefined
	Debug    = 1, //!< debug - useful for deep debugging
	Info     = 2, //!< informational - important debugging events?
	Notice   = 4, //!< normal, but significant conditions
	Warning  = 8, //!< warning - something wrong
	Error    = 16,//!< error - likely indication of a bug
	Critical = 32 //!< critical - may signal immenent failure
    }

    public class Log
    {
	internal static Hashtable _nodeMap = new Hashtable();

	public static LogNode Lookup( string channelName,
		string componentName, LogLevel level )
	{
	    string name = channelName + ":" + componentName + ":"
		+ level.ToString();

	    LogNode node = _nodeMap[ name ] as LogNode;
	    if(node == null)
	    {
		//Console.WriteLine("created node for {0}", name);
		node = new LogNode(channelName, componentName, level );
		_nodeMap[ name ] = node;
	    }
	    return node;
	}

	public static LogNode Lookup( string channelName, LogLevel level )
	{
	    Assembly asm = Assembly.GetCallingAssembly();
	    string component = LogComponent.Component( asm );
	    return Lookup( channelName, component, level );
	}
	
	public static LogNode Lookup( string channelName, Assembly asm,
		LogLevel level )
	{
	    string component = LogComponent.Component( asm );
	    return Lookup( channelName, component, level );
	}
	
	public static LogNode LookupGlobal( string channelName, LogLevel level )
	{
	    return Lookup( channelName, "", level );
	}
	
	public static LogNode Lookup( string channelName )
	{
	    Assembly asm = Assembly.GetCallingAssembly();
	    string component = LogComponent.Component( asm );
	    return Lookup(channelName, component, LogLevel.Undef );
	}
	
	public static LogNode LookupGlobal( string channelName )
	{
	    return Lookup( channelName, "", LogLevel.Undef );
	}


	// return a new node on each invokation, because LogNode constructor
	// will check which component is defined..

	static public LogNode Debug
	{
	    get 
	    { 
		return Lookup( "debug" , Assembly.GetCallingAssembly(),
			LogLevel.Debug ); 
	    }
	}
	static public LogNode Info
	{
	    get 
	    { 
		return Lookup( "info", Assembly.GetCallingAssembly(),
			LogLevel.Info ); 
	    }
	}
	static public LogNode Notice
	{
	    get 
	    { 
		return Lookup( "notice", Assembly.GetCallingAssembly(),
			LogLevel.Notice ); 
	    }
	}
	static public LogNode Warning
	{
	    get 
	    { 
		return Lookup( "warning", Assembly.GetCallingAssembly(),
			LogLevel.Warning ); 
	    }
	}
	static public LogNode Error
	{
	    get 
	    { 
		return Lookup( "error", Assembly.GetCallingAssembly(),
			LogLevel.Error ); 
	    }
	}
	static public LogNode Critical
	{
	    get 
	    { 
		return Lookup( "critical", Assembly.GetCallingAssembly(),
	    		LogLevel.Critical ); 
	    }
	}

	static private LogNode Node_Plus( string channel, LogLevel level )
	{
	    // subscribe to global channel - no component..
	    string name = channel + "::" + level.ToString();

	    LogNode node = _nodeMap[ name ] as LogNode;
	    if(node == null)
	    {
		node = new LogNode(channel);

		// TODO: LookupGlobal should take a log level?
		switch(level)
		{
		case LogLevel.Undef:
		    break;
		case LogLevel.Debug:
		    LookupGlobal("debug").Subscribe( node );
		    goto case LogLevel.Info;
		case LogLevel.Info:
		    LookupGlobal("info").Subscribe( node );
		    goto case LogLevel.Notice;
		case LogLevel.Notice:
		    LookupGlobal("notice").Subscribe( node );
		    goto case LogLevel.Warning;
		case LogLevel.Warning:
		    LookupGlobal("warning").Subscribe( node );
		    goto case LogLevel.Error;
		case LogLevel.Error:
		    LookupGlobal("error").Subscribe( node );
		    goto case LogLevel.Critical;
		case LogLevel.Critical:
		    LookupGlobal("critical").Subscribe( node );
		    break;
		}
		    
	       	_nodeMap[ name ] = node;
	    }
	    return node;
	}

	// some special channels..
	static public LogNode Debug_Plus
	{
	    get { return Node_Plus( "debug+", LogLevel.Debug ); }
	}
	
	static public LogNode Info_Plus
	{
	    get { return Node_Plus( "info+", LogLevel.Info ); }
	}

	static public LogNode Notice_Plus
	{
	    get { return Node_Plus( "notice+", LogLevel.Notice ); }
	}

	static public LogNode Warning_Plus
	{
	    get { return Node_Plus( "warning+", LogLevel.Warning ); }
	}

	static public LogNode Error_Plus
	{
	    get { return Node_Plus( "Error+", LogLevel.Error ); }
	}

	static public LogNode Critical_Plus
	{
	    get { return Node_Plus( "critical+", LogLevel.Critical ); }
	}
    }

}

