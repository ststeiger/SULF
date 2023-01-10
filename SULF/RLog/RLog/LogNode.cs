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
using System.Collections;
using StackFrame = System.Diagnostics.StackFrame;

namespace RLog
{
    public delegate void PublishDelegate( Publication data );
    public delegate void NodeActivation( object node, bool activated );

    public class LogNode
    {
	// For publishing log messages.
	private event PublishDelegate publishMessage;

	// This is published when our node activation state changes.  When we
	// become active, emit <this,true>, when we become inactive emit
	// <this,false>.
	// Allows us to potentially save work because we can avoid publishing
	// messages to inactive nodes.
	private event NodeActivation nodeActivation;

	// have to track node activation state somehow. Keep list of active
	// subscribers to make it easy to count.
	private ArrayList _activeSubscribers = new ArrayList();

	// this nodes channel name
	private string _channel;

	private string _component;

	// _active is true iff at least one subscriber is active.
	// Forcing a node active causes the parent to also become active.
	// _active == (_activeSubscribers.Count != 0).  
	// This is denormalized in order to make testing it quick during a Log
	// call..  We want to optimize speed for dormant log messages.
	private bool _active = false; 
	private LogLevel _level = LogLevel.Undef;
	private bool _needSetup = true;

	internal LogNode( string channel, string component, 
		LogLevel level )
	{
	    _channel = channel;
	    _level = level;
	    _component = component;

	    // trim off '/' at front and end
	    _channel = _channel.Trim('/');
	    // remove whitespace at front and end
	    _channel = _channel.Trim();
	}

	internal LogNode( string channel)
	{
	    publishMessage += this.Publish;
	    nodeActivation += this.SubscriberStateChange;
	    
	    _channel = channel;
	    _level = LogLevel.Undef;
	    _component = "";

	    // avoid making connections later.
	    _needSetup = false;
	}

	// subscribe a node to our publication..
	// The node subscribes to our publication, and we subscribe to
	// their activation state.
	public void Subscribe( LogNode node )
	{
	    if(_needSetup)
		CompleteSetup(); // sets _needSetup = false
	    if(node._needSetup)
		node.CompleteSetup();

	    if(node.Active)
		SubscriberStateChange( node, true );
	    publishMessage += node.publishMessage;
	    node.nodeActivation += nodeActivation;
	}

	// the node unsubscribes to our publication, and we subscribe to
	// their activation state.
	public void Unsubscribe( LogNode node )
	{
	    publishMessage -= node.publishMessage;
	    node.nodeActivation -= nodeActivation;
	    SubscriberStateChange( node, false );
	}

	// subscribe a simple subscriber delegate (no support for activation
	// state).  So, the node will always be considered active..
	public void Subscribe( PublishDelegate subscriber )
	{
	    if(_needSetup)
		CompleteSetup(); // sets _needSetup = false

	    SubscriberStateChange( subscriber, true );
	    publishMessage += subscriber;
	}

	// unsubscribe a simple subscriber delegate (no support for activation
	// state). 
	public void Unsubscribe( PublishDelegate subscriber )
	{
	    publishMessage -= subscriber;
	    SubscriberStateChange( subscriber, false );
	}

	public void Assert( bool cond, string format, params object[] args )
	{
	    if(!cond)
	    {
		if(_needSetup)
		    CompleteSetup(); // sets _needSetup = false

		string msg = String.Format( format, args );
		if(_active)
		{
		    Publication data = new Publication();
		    data.sourceNode = this;
		    data.caller = new StackFrame(1, true);
		    data.message = msg;
		    Publish( data );
		}
		throw new System.ApplicationException( msg );
	    }
	}

	// Publish a log message.
	public void Log( string format, params object[] args )
	{
	    if(_needSetup)
		CompleteSetup(); // sets _needSetup = false

	    if(_active)
	    {
		Publication data = new Publication();
		data.sourceNode = this;

		// caller is up 1 frame..
		data.caller = new StackFrame(1, true);

		// format message
		data.message = String.Format( format, args );

		Publish( data );
	    }
	}

	public string ChannelName
	{
	    get { return _channel; }
	}

	public string ComponentName
	{
	    get { return _component; }
	}

	public bool Active
	{
	    get { return _active; }
	}
	
	public LogLevel Level
	{
	    get { return _level; }
	}



	void CompleteSetup()
	{
	    _needSetup = false;
	    
	    publishMessage += this.Publish;
	    nodeActivation += this.SubscriberStateChange;

	    // there are multiple hierarchies at work.
	    // - Every channel (besides the root node) is connected to a lesser
	    // specialized channel.  Eg, "/debug/foo" is connected to "/debug",
	    // is connected to "/" (root node).
	    int lastSep = _channel.LastIndexOf('/');
	    if(lastSep > 0)
	    {
		string parentChannel = _channel.Substring(0, lastSep).Trim();
		LogNode parent = RLog.Log.Lookup( parentChannel,
			_component, _level );
		this.Subscribe( parent );
	    } else
	    {
		if(_channel.Length > 0)
		{
		    LogNode parent = RLog.Log.Lookup( "", 
			    _component, _level );
		    this.Subscribe( parent );
		}
	    }
	    
	    // - Every componentized channel is connected to the
	    // uncomponentized version
	    if(_component != "")
	    {
		LogNode parent = RLog.Log.Lookup( _channel, "", _level );
		this.Subscribe( parent );
	    }

	    // - Every channel is connected to the unspecialized level
	    if(_level != LogLevel.Undef)
	    {
		LogNode parent = RLog.Log.Lookup( _channel, 
			_component, LogLevel.Undef );
		this.Subscribe( parent );
	    }
	}

	// Publish is the slot which receives and distributes publications
	void Publish( Publication data )
	{
	    if(_active && !data.receiptLog.Contains( this ))
	    {
		data.receiptLog.Add( this );
		publishMessage( data );
	    }
	}

	// Add or remove a subscriber from the active subscriber list.
	// This also may change the node's activation state.
	bool SubscriberActivated( object node )
	{
	    _activeSubscribers.Add( node );
	    return !_active;
	}

	bool SubscriberDeactivated( object node )
	{
	    _activeSubscribers.Remove( node );
	    return _active && (_activeSubscribers.Count == 0);
	}

	void SubscriberStateChange( object node, bool activate )
	{
	    bool stateChange = activate ? 
		SubscriberActivated( node ) : SubscriberDeactivated( node );
	    if(stateChange)
	    {
		_active = activate;
		nodeActivation( this, activate );
	    }
	}
    }
}

