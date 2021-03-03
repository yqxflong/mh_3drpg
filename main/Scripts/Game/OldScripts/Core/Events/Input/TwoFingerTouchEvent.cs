///////////////////////////////////////////////////////////////////////
//
//  TwoFingerTouchEndEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class TwoFingerTouchEvent : GameEvent
{
	public enum eTwoFingerTouchEventType
	{
		Start,
		Update,
		End
	}

	public Transform target1 { get; set; }
	public Transform target2 { get; set; }
	public Transform targetCenter { get; set; }

	public Vector3 position1 { get; set; }
	public Vector3 position2 { get; set; }
	public Vector3 positionCenter { get; set; }

	public Vector3 screenPosition1 { get; set; }
	public Vector3 screenPosition2 { get; set; }
	public Vector3 screenPositionCenter { get; set; }

	public Vector3 direction1 { get; set; }
	public Vector3 direction2 { get; set; }
	public Vector3 directionCenter { get; set; }

	public Vector3 groundPosition1 { get; set; }
	public Vector3 groundPosition2 { get; set; }	
	public Vector3 groundPositionCenter { get; set; }

	public eTwoFingerTouchEventType touchEventType
	{
		get;
		private set;
	}
	
	public TwoFingerTouchEvent(eTwoFingerTouchEventType touchEventType, Vector3 screenPosition1, Vector3 screenPosition2)
	{
		this.touchEventType = touchEventType;
		this.screenPosition1 = screenPosition1;
		this.screenPosition2 = screenPosition2;
		this.screenPositionCenter = (screenPosition1 + screenPosition2) / 2.0f;
	}
	
	public override string ToString()
	{
		return base.ToString() + ": target1 " + (target1 ? target1.ToString() : "null") + ", position1 " + position1.ToString() + ", screenPosition1 " + screenPosition1.ToString() + "; target2 " + (target2 ? target2.ToString() : "null") + ", position2 " + position2.ToString() + ", screenPosition2 " + screenPosition2.ToString();
	}
}
