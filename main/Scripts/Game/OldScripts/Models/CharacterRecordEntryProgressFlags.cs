///////////////////////////////////////////////////////////////////////
//
//  CharacterRecordEntryProgressFlags.cs
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

public class CharacterRecordEntryProgressFlags : ICharacterRecordEntry 
{
	public Dictionary<int, bool> abilityUsedFlags;
	public int hardCurrencySpent;
	public bool[] abilitySlotsOpenedFlags; 

	public bool debugUnlockAllDungeons = false;

	public bool IsFirstCreated
	{
		get; set;
	}

	public void Initialize()
	{
		abilityUsedFlags = new Dictionary<int, bool>();

		hardCurrencySpent = 0;
	}

	public void PrepareData()
	{

	}

	public void SaveUniqueData(Hashtable record)
	{
		record["abilityUsedFlags"] = Johny.HashtablePool.Claim(abilityUsedFlags);
		record["hardCurrencySpent"] = hardCurrencySpent;
		record["abilitySlotsOpenedFlags"] = new ArrayList(abilitySlotsOpenedFlags);
	}

	public void LoadUniqueData(Hashtable record)
	{
		Hashtable abilityUsedHashtable = EB.Dot.Object("abilityUsedFlags", record, null);
		if(abilityUsedHashtable != null){
			foreach (DictionaryEntry entry in abilityUsedHashtable)
			{
				int slotType = 0;
				if (EB.Serialization.Deserialize<int>(entry.Key, ref slotType))
				{
					abilityUsedFlags[slotType] = EB.Dot.Bool(entry.Key.ToString(), abilityUsedHashtable, false);
				}
			}
		}

		hardCurrencySpent = EB.Dot.Integer("hardCurrencySpent", record, 0);

		ArrayList abilityOpenArray = EB.Dot.Array("abilitySlotsOpenedFlags", record, new ArrayList());
		for (int i = 0; i < abilityOpenArray.Count; i++)
		{
			abilitySlotsOpenedFlags[i] = (bool)abilityOpenArray[i];
		}
	}
}
