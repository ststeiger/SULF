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

/** @namespace Sulf
  @brief Provides a high level C# interface on top of Fuse.

  For example, this code:
  @code
  Sulf.MapFS root = new Sulf.MapFS();
  root["hello"] = "Hello world!\n"

  Channel channel = Sulf.Mounter.Mount("HelloFS", root, "/tmp/hellofs");
  channel.EventLoop();
  @endcode

  Results in this filesystem:
  @verbatim
  > ls -l /tmp/hellofs
  -r--r--r--  1 root root 13 2005-04-29 17:04 hello
  > cat /tmp/hellofs/hello
  Hello world!
  @endverbatim

  The Mounter class is a simple helper to mount filesystems.

  The MapFS class provides a Fuse.DirNode implementation which make a map
  (IDictionary) appear as a directory.

  @note The Sulf library is under development.
  @see @ref sulf-design
  @author Valient Gough
*/
namespace Sulf
{
    /** @class Mounter
      @brief Provides helper methods for mounting a filesystem.

      @see sulf-design
    */
    public class Mounter
    {
	/** Mount a filesystem at the given mountPoint.  Uses the provided
	   Fuse.DirNode as the root of the filesystem.

	   Returns Fuse.Channel, which must have eventLoop() called in order to
	   process filesystem commands.
	*/
	public static Fuse.Channel 
	    Mount( string fsName, Fuse.DirNode rootDir, string mountPoint )
	{
	    MountFS fs = new MountFS( fsName, rootDir );

	    // create athe channel which is responsible for communicating with
	    // the FUSE kernel..
	    Fuse.Channel channel = new Fuse.Channel( mountPoint, fs );

	    // create a reactor to handle incoming requests for the filesystem
	    Fuse.Reactor reactor = new Fuse.Reactor();
	    reactor.SubscribeTo( channel );

	    // have fuse mount the filesystem.
	    channel.Mount();

	    // return the channel
	    return channel;
	}
	
	/** Mount a filesystem at the given mountPoint.
	  Simply a wrapper around standard calls to Fuse.Channel and
	  Fuse.Reactor.

	  Returns Fuse.Channel, which must have eventLoop() called in order to
	  process filesystem commands.
	*/
	public static Fuse.Channel 
	    Mount( Fuse.FileSystem fs, string mountPoint )
	{
	    // create athe channel which is responsible for communicating with
	    // the FUSE kernel..
	    Fuse.Channel channel = new Fuse.Channel( mountPoint, fs );

	    // create a reactor to handle incoming requests for the filesystem
	    Fuse.Reactor reactor = new Fuse.Reactor();
	    reactor.SubscribeTo( channel );

	    // have fuse mount the filesystem.
	    channel.Mount();

	    // return the channel
	    return channel;
	}
    }

    internal class MountFS : Fuse.FileSystem
    {
	string _name;
	Fuse.DirNode _root;

	public MountFS( string name, Fuse.DirNode rootNode )
	{
	    _name = name;
	    _root = rootNode;
	}

	override public string Name
	{
	    get { return _name; }
	}

	override public Fuse.DirNode RootNode
	{
	    get { return _root; }
	}
    }
}
