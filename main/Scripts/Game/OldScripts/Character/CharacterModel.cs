using System;
///////////////////////////////////////////////////////////////////////
//
//  CharacterModel.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

public enum eMonsterType
{
	Minion,
	Standard,
	Champion
}

public enum eResourceDirectory
{
	Player,
	NPC,
	Enemies,
	Box
}

[System.Serializable]
public class CharacterModel: BaseDataModel
{
	public static string[] AllowedVariables = 
	{
		"Level"
	};

	//Basics
	[ServerData]
	[Parameter("Name", "Basics")]
	public new string name = "New Character";

	[Parameter("Display Name", "Basics")]
	public string displayName;

	[Parameter("Auto Attack", "Basics")]
	public AutoAttackSetModel autoAttack;

	[Parameter("Portrait Name", "Basics")]
	public string portraitName;

	[Parameter("Resource Directory", "Basics")]
	public eResourceDirectory resourceDirectory = eResourceDirectory.Enemies;

	[Parameter("Prefab Name", "Basics")]
	public string prefabName;

	[Parameter("Audio Name", "Basics")]
	public string audioName;

	[Parameter("Is NPC", "Basics"), ServerData]
	public bool isNPC;

	[Parameter("Dialogue Text Color", "Basics")]
	public Color dialogueTextColor = Color.black;

	[Parameter("Speed", "Movement")]
	public float speed;
	[Parameter("RVO Radius", "Movement")]
	public float rvoRadius;
	[Parameter("Height Offset", "Movement")]
	public float heightOffset;
	
	[Parameter("Slot Size", "AI")]
	public int slotSize = 1;

	[Parameter("Difficulty Rating", "AI"), ServerData]
	public int difficultyRating;

	[Parameter("Detection Area", "AI")]
	public float detectionArea;

	[Parameter("Chase Distance", "AI")]
	public float chaseDistance;

	public ResourcesReference aiTriggerRef;

	[Parameter("AI Trigger", "AI")]
	public FsmTemplate AITrigger
	{
		get
		{
			if (aiTriggerRef == null)
			{
				return null;
			}
			return aiTriggerRef.Value as FsmTemplate;
		}
		#if UNITY_EDITOR
		set
		{
			if (aiTriggerRef == null)
			{
				aiTriggerRef = new ResourcesReference(value);
			}
			else
			{
				aiTriggerRef.Value = value;
			}
		}
		#endif
	}

	[Parameter("Team", "AI")]
	public eTeamId team = eTeamId.Player;

	[Parameter("Slot Access Priority Modifier", "AI")]
	public int slotPriority = 1;

	public int id;
	public bool	isPlayer;

	public float CharacterRadiusNormalized
	{
		get
		{
			return rvoRadius / 0.7f;
		}
	}

	public override string GetName ()
	{
		return name;
	}

	public override int GetId ()
	{
		return id;
	}

	public override void SetId (int id)
	{
		this.id = id;
	}

	public string PrefabNameFromGenderMain (eGender gender)
	{
		string genderIdentifier = gender == eGender.Male ? "M" : "F";
		return ResourcePrefabNameMain.Replace ("{Gender}", genderIdentifier);
	}

	public string PrefabNameFromGender(eGender gender)
	{
		string genderIdentifier = gender == eGender.Male ? "M" : "F";
		return ResourcePrefabName.Replace("{Gender}", genderIdentifier);
	}

	public string NPC_Template;

	public string ResourcePrefabNameMain
	{
		get
		{
			if (string.IsNullOrEmpty(NPC_Template))
			{
				return string.Format("Bundles/{0}/Variants/{1}-M", resourceDirectory, prefabName);
			}
			else
			{
				return string.Format("Bundles/{0}/Variants/{1}-Variant-M", resourceDirectory, NPC_Template);
			}
		}
	}

	public string ResourcePrefabName
	{
		get
		{
			if (string.IsNullOrEmpty(NPC_Template))
			{
				return string.Format("Bundles/{0}/Variants/{1}", resourceDirectory, prefabName);
			}
			else
			{
				return string.Format("Bundles/{0}/Variants/{1}-Variant", resourceDirectory, NPC_Template);
			}
		}
	}

	public override OrderedHashtable GetServerData ()
	{
		OrderedHashtable serverData = base.GetServerData ();

		return serverData;
	}
}
