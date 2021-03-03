///////////////////////////////////////////////////////////////////////
//
//  DoubleTapEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class DoubleTapEvent : GameEvent
{
	public Transform target;
	public Vector3 position;
	public Vector3 direction;
	public Vector3 groundPosition;
	public Vector3 screenPosition;
	
	public DoubleTapEvent(Vector3 screenPosition, Transform t, Vector3 groundPosition)
	{
		this.screenPosition = screenPosition;
		this.target = t;
		if (t != null)
		{
			this.position = t.position;
		}
		else
		{
			this.position = groundPosition;
		}
		this.groundPosition = groundPosition;
	}

	public DoubleTapEvent(Vector3 screenPosition, Vector3 position)
	{
		this.screenPosition = screenPosition;
		this.target = null;
		this.position = position;
		this.groundPosition = position;
	}
}
