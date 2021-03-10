///////////////////////////////////////////////////////////////////////
//
//  TargetChangedEve.cs
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

[VisibleAtDesignLevel]
public class TargetChangedEvent : GameEvent, IPlayMakerEvent
{
	public GameObject owner;
	public GameObject previousTarget;
	public GameObject newTarget;

	public TargetChangedEvent(GameObject owner, GameObject newTarget, GameObject previousTarget)
	{
		this.owner = owner;
		this.previousTarget = previousTarget;
		this.newTarget = newTarget;
	}

	public GameObject[] GetRelevantGameObjects()
	{
		return new GameObject[] { owner };
	}

	public void UpdateFsmEventData()
	{
		HutongGames.PlayMaker.Fsm.EventData.GameObjectData = newTarget;
	}
}
