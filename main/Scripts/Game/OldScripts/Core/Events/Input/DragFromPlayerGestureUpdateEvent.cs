///////////////////////////////////////////////////////////////////////
//
//  DragFromPlayerGestureUpdateEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class DragFromPlayerGestureUpdateEvent : DragFromPlayerGestureEvent
{
	public DragFromPlayerGestureUpdateEvent(Vector3 screenPosition) : base(DragFromPlayerGestureEvent.eDragFromPlayerGestureEventType.Update, screenPosition)
	{
	}

	public DragFromPlayerGestureUpdateEvent(Vector3 screenPosition, Transform target) : base(DragFromPlayerGestureEvent.eDragFromPlayerGestureEventType.Update, screenPosition, target)
	{
	}

	public DragFromPlayerGestureUpdateEvent(Vector3 screenPosition, Vector3 position) : base(DragFromPlayerGestureEvent.eDragFromPlayerGestureEventType.Update, screenPosition, position)
	{
	}
}
