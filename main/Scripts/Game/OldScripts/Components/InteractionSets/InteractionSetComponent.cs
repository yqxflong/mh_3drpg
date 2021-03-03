///////////////////////////////////////////////////////////////////////
//
//  InteractionSetComponent.cs
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

public class InteractionSetComponent : MonoBehaviour 
{
	public const string activeTag = " (Active)";

	//[Flags]
	//public enum eInteractionCategory
	//{
	//	Puzzle = 1 << 0,
	//	Combat = 1 << 1,
	//	Treasure = 1 << 2,
	//	Timed = 1 << 3,
	//	Horde = 1 << 4,
	//	Escort = 1 << 5,
	//}

	//[SerializeField, HideInInspector]
	//public List<InteractionSetRequirement> requirements = new List<InteractionSetRequirement>();
	[SerializeField, HideInInspector]
	public string BaseName
	{
		get
		{
			string myName = gameObject.name;
			int index = myName.IndexOf(activeTag);
			while (index != -1)
			{
				myName = myName.Remove(index, activeTag.Length);
				index = myName.IndexOf(activeTag);
			}
			return myName;
		}
	}

	[SerializeField, HideInInspector]
	public List<SelectionRequirement> selectionRequirements = new List<SelectionRequirement>();

	[SerializeField]
	private int _weight = 0;
	public int Weight
	{
		get
		{
			return _weight;
		}
	}

	//[SerializeField, HideInInspector]
	//private eInteractionCategory _interactionCategory = eInteractionCategory.Combat;
	//public eInteractionCategory InteractionCategory
	//{
	//	get
	//	{
	//		return _interactionCategory;
	//	}
	//}
}
