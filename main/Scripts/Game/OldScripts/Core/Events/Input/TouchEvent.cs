///////////////////////////////////////////////////////////////////////
//
//  TouchEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class TouchEvent : GameEvent
{
	public enum eTouchEventType
	{
		Start,
		Update,
		End
	}

	public Transform target { get; set; }
	public Vector3 position { get; set; }
	public Vector3 groundPosition { get; set; }
	public Vector3 screenPosition { get; set; }
	public Vector3 direction { get; set; }
	
	public eTouchEventType touchEventType 
	{ 
		get;
		private set;
	}
	
	public virtual void Initialize(eTouchEventType touchEventType, Vector3 screenPosition)
	{
		this.touchEventType = touchEventType;
		this.screenPosition = screenPosition;
	}

	public virtual void Initialize(eTouchEventType touchEventType, Vector3 screenPosition, Transform target, Vector3 groundPosition)
	{
		Initialize(touchEventType, screenPosition);
		this.target = target;
		if (target != null)
		{
			this.position = target.position;
		}
		this.groundPosition = groundPosition;
	}

	public virtual void Initialize(eTouchEventType touchEventType, Vector3 screenPosition, Vector3 position)
	{
		Initialize(touchEventType, screenPosition);
		this.target = null;
		this.position = position;
		this.groundPosition = position;
	}

	protected override void Reset()
	{
		target = null;
		position = default(Vector3);
		groundPosition = default(Vector3);
		screenPosition = default(Vector3);
		direction = default(Vector3);
	}
	
	public override string ToString()
	{
		return base.ToString() + ": target " + (target ? target.ToString() : "null") + ", position " + position.ToString() + ", screenPosition " + screenPosition.ToString();
	}
}
