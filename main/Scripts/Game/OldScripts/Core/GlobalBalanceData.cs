///////////////////////////////////////////////////////////////////////
//
//  GlobalBalanceData.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GlobalBalanceData : ScriptableObject
{
    #region Member
    [ServerData]
	public int regenInterval;
	
	[Parameter("Stamina Refill Cost (HC)", "Store"), ServerData]
	public int staminaRefillCost;
	[Parameter("Revive Cost (HC)", "Store"), ServerData]
	public int reviveCost;
	[Parameter("Add Slot Cost (HC)", "Store"), ServerData]
	public int addSlotCost;

	[Parameter("Melee Retarget Range", "Combat")]
	public int meleeRetargetRange;
	[Parameter("Ranged Retarget Range", "Combat")]
	public int rangedRetargetRange;
	[Parameter("Self Select Radius", "Combat")]
	public float selfSelectRadius = 2.5f;
	[Parameter("Health Orb Restore (%)", "Combat")]
	public float healthOrbRestore = 20f;
	[Parameter("Mana Orb Restore (%)", "Combat")]
	public float manaOrbRestore = 20f;
	[Parameter("Elemental Strong Multiplier", "Combat")]
	public float elementalStrongMultiplier = 1.5f;
	[Parameter("Elemental Weak Multiplier", "Combat")]
	public float elementalWeakMultiplier = 0.5f;

	public float[] monsterDamageScaling = new float[4];
	public float[] playerDamageScaling = new float[4];

	public float[] difficultyMonsterDamageScaling = new float[4];
	public float[] difficultyPlayerDamageScaling = new float[4];

	[ServerData]
	public int levelCap = 0;
	public List<int> levelXPCap;
	public List<int> levelStaminaValues;
	public List<int> levelAffinityValues;
	public bool xpExpanded = false;

	public List<int> spiritSlotLevelUnlocks = new List<int>();

	[Parameter("% Destructibles w/Gold", "Loot"), ServerData]
	public int percentDestructiblesWithGold = 25;
	[Parameter("% Total Gold in Destructibles", "Loot"), ServerData]
	public int percentOfGoldInDestructibles = 10;
	[Parameter("Monsters to Gold Piles Ratio", "Loot"), ServerData]
	public int monstersToGoldPilesProportion = 2;
	[Parameter("Gold Variance (%)", "Loot"), ServerData]
	public int goldVariance = 10;

	[Parameter("Minion Loot Weight", "Loot"), ServerData]
	public int minionLootWeight = 1;
	[Parameter("Standard Loot Weight", "Loot"), ServerData]
	public int standardLootWeight = 5;
	[Parameter("Champion Loot Weight", "Loot"), ServerData]
	public int championLootWeight = 10;

	[Parameter("Minion Gold Weight", "Loot"), ServerData]
	public int minionGoldWeight = 1;
	[Parameter("Standard Gold Weight", "Loot"), ServerData]
	public int standardGoldWeight = 2;
	[Parameter("Champion Gold Weight", "Loot"), ServerData]
	public int championGoldWeight = 4;

	public int MaxXP
	{
		get
		{
			return levelXPCap[levelXPCap.Count - 1];
		}
	}
    #endregion

    #region Static Instance
    private static GlobalBalanceData _instance;
	public static GlobalBalanceData Instance
	{
		get 
		{
			return _instance;
		}
	}
    #endregion

    #region When Game Start
    public static void Init(System.Action<bool> fn)
    {
		EB.Assets.LoadAsync("Bundles/DataModels/GlobalBalanceData", typeof(GlobalBalanceData), o =>
		{
			if(o){
				_instance = o as GlobalBalanceData;
				fn(true);
			}
		});
	}
    #endregion

    public float GetNumPlayersMonsterDamage()
	{
		int numPlayers = PlayerManager.sPlayerControllers.Count;
		numPlayers = System.Math.Max(System.Math.Min(numPlayers, 4), 1);
		return monsterDamageScaling[numPlayers - 1];
	}

	public float GetNumPlayersPlayerDamage()
	{
		int numPlayers = PlayerManager.sPlayerControllers.Count;
		numPlayers = System.Math.Max(System.Math.Min(numPlayers, 4), 1);
		return playerDamageScaling[numPlayers - 1];
	}

	public float GetDifficultyMonsterDamage() 
	{
		//DungeonModel dungeon = null;
		//if (dungeon != null)
		//{
		//	eDungeonDifficulty dungeonDifficulty = dungeon.difficulty;
		//	return difficultyMonsterDamageScaling[(int)dungeonDifficulty];	
		//}
		//else
		{
			return 1.0f;
		}
	}

	public float GetDifficultyPlayerDamage() 
	{
		//DungeonModel dungeon = null;
		//if (dungeon != null)
		//{
		//	eDungeonDifficulty dungeonDifficulty = dungeon.difficulty;
		//	return difficultyPlayerDamageScaling[(int)dungeonDifficulty];
		//}
		//else
		{
			return 1.0f;
		}
	}
}