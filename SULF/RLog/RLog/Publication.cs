/*****************************************************************************
 * Author:   Valient Gough <vgough@pobox.com>
 *
 *****************************************************************************
 * Copyright (c) 2004, Valient Gough
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
using System.Reflection;
using Path = System.IO.Path;
using StackFrame = System.Diagnostics.StackFrame;
using ArrayList = System.Collections.ArrayList;

namespace RLog
{
    public class Publication
    {
	// originating node
	public LogNode sourceNode;

	// track nodes which receive this message
	public ArrayList receiptLog = new ArrayList();


	public string message;
	public DateTime publishTime = DateTime.Now;
	public StackFrame caller;

	public string location
	{
	    get
	    {
		if(caller.GetFileName() == null ||
			caller.GetFileLineNumber() == 0)
		{
		    MethodBase cm = caller.GetMethod();
		    if(cm != null)
			return cm.ReflectedType.ToString() + "." + cm.Name;
		    else
			return "[unknown]";
		} else
		{
		    string fileName = Path.GetFileName( caller.GetFileName() );
		    return fileName + ":" 
			+ caller.GetFileLineNumber().ToString();
		}
	    }
	}
    }
}
