///////////////////////////////////////////////////////////////////////
//
//  SpawnerAggroEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class SpawnerEnemiesAggroEvent : ReplicatedEvent, IPlayMakerEvent
{
	public bool isAggroStarting;

	public SpawnerEnemiesAggroEvent()
		: base(null, null)
	{
		isAggroStarting = false;
	}

	public SpawnerEnemiesAggroEvent(GameObject spawner, bool isAggroStarting) : base(null, spawner)
	{
		this.isAggroStarting = isAggroStarting;
	}

	public GameObject[] GetRelevantGameObjects()
	{
		return new GameObject[] { target };
	}

	public void UpdateFsmEventData()
	{
	}

	public override void Serialize(EB.BitStream bs)
	{
		base.Serialize(bs);

		// Takes care of reading and writing
		bs.Serialize(ref isAggroStarting);
	}
}
