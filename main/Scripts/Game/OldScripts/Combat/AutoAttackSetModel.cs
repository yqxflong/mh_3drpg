///////////////////////////////////////////////////////////////////////
//
//  AutoAttackSetModel.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;

public class AutoAttackSetModel : BaseDataModel, IEnumerable
{
	public string autoAttackName = "Auto Attack";
	public bool isExpanded;
	public int id;

	public string displayName;
	public string description;

	public bool isRanged = false;
	public float range = 2.5f;
	public float coneWidthDegrees = 120.0f;

	public List<AutoAttackModel> attacks;
	public eSetType setType;
	public float movementSlowFactor = 0.0f;

	public string iconName;

	public ResourcesReference iconRef;

	public string IconPath
	{
		get
		{
			if (iconRef != null)
			{
				return iconRef.fileName;
			}
			
			return string.Empty;
		}
	}

	public enum eSetType
	{
		Combo,
		Random
	}

	public bool IsRanged
	{
		get
		{
			return isRanged;
		}
	}

	public bool IsMelee
	{
		get
		{
			return !isRanged;
		}
	}

	public int Count
	{
		get 
		{
			return attacks.Count;
		}
	}

	public AutoAttackModel this[int index]
	{
		get 
		{
			return (attacks != null && (index >= 0 && index < attacks.Count)) ? attacks[index] : null;
		}
	}

	//IModel functions
	public override string GetName() 
	{
		return autoAttackName;
	}

	public override int GetId()
	{
		return id;
	}

	public override void SetId(int id)
	{
		this.id = id;
	}

	//IEnumerable functions
	public IEnumerator GetEnumerator()
	{
		return attacks.GetEnumerator();
	}

	public override void Preload() 
	{
		foreach (AutoAttackModel attack in attacks)
		{
			attack.Preload();
		}
	}
}
