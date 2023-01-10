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

using System.Reflection;
using System.Runtime.CompilerServices;

/*
    Store some properties that are used when creating the .dll.
*/
[assembly:AssemblyTitle("RLog.Net")]
[assembly:AssemblyDescription("Logging library for .Net")]
[assembly:AssemblyCompany("Valient Gough")]
[assembly:AssemblyCopyright("(C) Copyright 2004, Valient Gough")]

[assembly:AssemblyVersion("0.3.*")]
//[assembly:AssemblyDelaySign(true)]
[assembly:AssemblyKeyFile("../rlog.snk")]

[assembly:RLog.LogComponent("RLog")]
