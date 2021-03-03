///////////////////////////////////////////////////////////////////////
//
//  DialogueEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[VisibleAtDesignLevel]
public class DialogueEvent : GameEvent, IPlayMakerEvent
{
	public enum EventType
	{
		start, 
		stop
	}
	private EventType _type = EventType.start;

	public DialogueEvent(EventType eventType)
	{
		this._type = eventType;
	}

	// get the type of dialogue event
	public EventType GetEventType()
	{
		return _type;
	}

	public GameObject[] GetRelevantGameObjects()
	{
		return new GameObject[] {};
	}

	public void UpdateFsmEventData()
	{
	}
}
