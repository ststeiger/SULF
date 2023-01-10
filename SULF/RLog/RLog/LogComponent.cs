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
using System.Reflection;

namespace RLog
{

    [AttributeUsage( AttributeTargets.Assembly, 
	    AllowMultiple = false )]
    public class LogComponent : System.Attribute
    {
	private string component;
	private static LogComponent _default = 
	    new LogComponent("_unspecified_");

	public LogComponent( string component )
	{
	    this.component = component;
	}

	public string Name
	{
	    get { return this.component; }
	}

	public static LogComponent DefaultComponent
	{
	    get { return _default; }
	}

	internal static string Component( Assembly caller )
	{
	    object[] attrs = caller.GetCustomAttributes(
		    typeof(LogComponent), true);
	    if(attrs.Length == 1)
	    {
		LogComponent component = attrs[0] as LogComponent;
		return component.Name;
	    } else
	    {
		return LogComponent.DefaultComponent.Name;
	    }
	}
    }

}

