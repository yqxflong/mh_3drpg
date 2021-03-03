///////////////////////////////////////////////////////////////////////
//
//  DragFromPlayerGestureEndEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class DragFromPlayerGestureEndEvent : DragFromPlayerGestureEvent
{
	public DragFromPlayerGestureEndEvent(Vector3 screenPosition) : base(DragFromPlayerGestureEvent.eDragFromPlayerGestureEventType.End, screenPosition)
	{
	}

	public DragFromPlayerGestureEndEvent(Vector3 screenPosition, Transform target) : base(DragFromPlayerGestureEvent.eDragFromPlayerGestureEventType.End, screenPosition, target)
	{
	}

	public DragFromPlayerGestureEndEvent(Vector3 screenPosition, Vector3 position) : base(DragFromPlayerGestureEvent.eDragFromPlayerGestureEventType.End, screenPosition, position)
	{
	}
}
