///////////////////////////////////////////////////////////////////////
//
//  TouchEndEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class TouchEndEvent : TouchEvent
{
	public void Initialize(Vector3 screenPosition)
	{
		base.Initialize(eTouchEventType.End, screenPosition);
	}

	public void Initialize(Vector3 screenPosition, Transform target, Vector3 groundPosition)
	{
		base.Initialize(eTouchEventType.End, screenPosition, target, groundPosition);
	}

	public void Initialize(Vector3 screenPosition, Vector3 position)
	{
		base.Initialize(eTouchEventType.End, screenPosition, position);
	}
}
