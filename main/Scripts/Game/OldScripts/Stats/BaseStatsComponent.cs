///////////////////////////////////////////////////////////////////////
//
//  BaseStatsComponent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

//Declaring this as an abstract class rather than an interface so that it works with the generic GetComponent<T>
public abstract class BaseStatsComponent : BaseComponent
{
	public delegate void OnDeath(GameObject gameObject, GameObject killer, bool isForcedKill);
	public event OnDeath onDeath;

	public abstract bool IsAlive 
	{
		get;
	}
	public abstract eTeamId Team
	{
		get; set;
	}
	public abstract bool IsCountedAsTarget
	{
		get;
	}
	public abstract bool CanReceivePrefabEffects
	{
		get;
	}
	public abstract ReplicationView ReplicationView
	{
		get;
	}
	public bool LocalOnly 
	{
		get; set;
	}

	public long TargetSortValue
	{
		get 
		{
			return (IsCountedAsTarget) ? ReplicationView.viewId.n : -ReplicationView.viewId.n;
		}
	}

	public abstract void Heal(float amount, GameObject source);

	public abstract void ForceKill();

}
