///////////////////////////////////////////////////////////////////////
//
//  BaseDataModel.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Reflection;

// the interface for all data models
public abstract class BaseDataModel : ScriptableObject
{
	public abstract string GetName();
	public abstract int GetId();
	public abstract void SetId(int id);

	public void CreateId() 
	{
		while (true)
		{
			System.Guid newGuid = System.Guid.NewGuid();
			if (newGuid != System.Guid.Empty)
			{
				SetId(newGuid.GetHashCode());
				break;
			}
		}
	}

	public virtual OrderedHashtable GetServerData()
	{
		OrderedHashtable serverData = new OrderedHashtable();

		foreach (FieldInfo field in GetType().GetFields())
		{
			if (field.GetCustomAttributes(typeof(ServerDataAttribute), true).Length > 0)
			{
				var value = field.GetValue(this);
				//if (field.FieldType == typeof(GenericFormulaValueModel))
				//{
				//	value = (value as GenericFormulaValueModel).ToString();
				//}
				serverData[field.Name] = value;
			}
		}

		return serverData;
	}

	public virtual void Preload() { }
}
