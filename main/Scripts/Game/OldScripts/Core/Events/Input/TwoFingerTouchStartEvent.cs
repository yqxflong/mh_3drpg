///////////////////////////////////////////////////////////////////////
//
//  TwoFingerTouchStartEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class TwoFingerTouchStartEvent : TwoFingerTouchEvent
{
	public TwoFingerTouchStartEvent(Vector3 screenPosition1, Vector3 screenPosition2) : base(TwoFingerTouchEvent.eTwoFingerTouchEventType.Start, screenPosition1, screenPosition2)
	{
	}
}
