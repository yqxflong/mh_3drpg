///////////////////////////////////////////////////////////////////////
//
//  CharacterRecordEntryAbilities.cs
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

public class CharacterRecordEntryAbilities : ICharacterRecordEntry
{
	public int specPointsAvailable;
	public int specPointsTotal;
	public Dictionary<int, int> abilityLevels;
	public Dictionary<int, int> currentAbilities;

	public void Initialize()
	{
		abilityLevels = new Dictionary<int, int>();
		currentAbilities = new Dictionary<int, int>();
	}

	public void PrepareData()
	{
		
	}
	
	public void SaveUniqueData(Hashtable record)
	{
		record["abilityLevel"] = Johny.HashtablePool.Claim(abilityLevels);
		record["currentAbility"] = Johny.HashtablePool.Claim(currentAbilities);	
	}

	public void LoadUniqueData(Hashtable record)
	{
		abilityLevels = GetTypeMapFromHashtable(EB.Dot.Object("abilityLevel", record, Johny.HashtablePool.Claim()));
		currentAbilities = GetTypeMapFromHashtable(EB.Dot.Object("currentAbility", record, Johny.HashtablePool.Claim()));
	}

	protected static Dictionary<int, int> GetTypeMapFromHashtable(Hashtable input)
	{
		Dictionary<int, int> result = new Dictionary<int, int>();

		foreach ( DictionaryEntry entry in input )
		{
			int type = 0;

			if ( EB.Serialization.Deserialize<int>(entry.Key, ref type) )
			{
				result[type] = EB.Dot.Integer(entry.Key.ToString(), input, 0);
			}
		}
		
		return result;
	}

	public int GetAbilityLevel( int abilityId )
	{
		if ( abilityLevels.ContainsKey(abilityId) )
		{
			return abilityLevels[abilityId];
		}
		
		return 0;
	}
	
	public int GetCurrentAbility( int slotId )
	{
		if ( currentAbilities.ContainsKey(slotId) )
		{
			return currentAbilities[slotId];
		}
		
		return -1;
	}

	public void SetAbilityLevel( int abilityId, int level )
	{
		abilityLevels[abilityId] = level;
		
		if (level < 1)
		{
			abilityLevels.Remove(abilityId);
		}
	}
	
	public void SetCurrentAbility( int slotId, int abilityId )
	{
		currentAbilities[slotId] = abilityId;
		
		if (abilityId == -1)
		{
			currentAbilities.Remove(slotId);
		}
	}
}