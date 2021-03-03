///////////////////////////////////////////////////////////////////////
//
//  TouchUpdateEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class TouchUpdateEvent : TouchEvent
{
	public bool hasValidNavPoint = true;

	public void Initialize(Vector3 screenPosition)
	{
		hasValidNavPoint = true;
		base.Initialize(eTouchEventType.Update, screenPosition);
	}

	public void Initialize(Vector3 screenPosition, Transform target, Vector3 groundPosition)
	{
		hasValidNavPoint = true;
		base.Initialize(eTouchEventType.Update, screenPosition, target, groundPosition);
	}

	public void Initialize(Vector3 screenPosition, Vector3 position)
	{
		hasValidNavPoint = true;
		base.Initialize(eTouchEventType.Update, screenPosition, position);
	}

	protected override void Reset()
	{
		base.Reset();
		hasValidNavPoint = true;
	}
}
