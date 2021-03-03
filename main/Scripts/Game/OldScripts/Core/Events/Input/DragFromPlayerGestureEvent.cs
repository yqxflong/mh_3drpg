///////////////////////////////////////////////////////////////////////
//
//  DragFromPlayerGestureEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class DragFromPlayerGestureEvent : GameEvent
{
	public enum eDragFromPlayerGestureEventType
	{
		Start,
		Update,
		End
	}

	public Transform target { get; set; }
	public Vector3 position { get; set; }
	public Vector3 screenPosition { get; set; }
	public Vector3 direction { get; set; }
	public Vector3 groundPosition { get; set; }
	public eDragFromPlayerGestureEventType eventType 
	{ 
		get;
		private set;
	}
	
	public DragFromPlayerGestureEvent(eDragFromPlayerGestureEventType eventType, Vector3 screenPosition)
	{
		this.eventType = eventType;
		this.screenPosition = screenPosition;
	}

	public DragFromPlayerGestureEvent(eDragFromPlayerGestureEventType eventType, Vector3 screenPosition, Transform target) : this(eventType, screenPosition)
	{
		this.target = target;
		if (target != null)
		{
			this.position = target.position;
		}
	}

	public DragFromPlayerGestureEvent(eDragFromPlayerGestureEventType eventType, Vector3 screenPosition, Vector3 position) : this(eventType, screenPosition)
	{
		this.target = null;
		this.position = position;
	}
	
	public override string ToString()
	{
		return base.ToString() + ": target " + (target ? target.ToString() : "null") + ", position " + position.ToString() + ", screenPosition " + screenPosition.ToString();
	}
}
