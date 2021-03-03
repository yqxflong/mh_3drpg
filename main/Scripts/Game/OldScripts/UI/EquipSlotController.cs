///////////////////////////////////////////////////////////////////////
//
//  EquipSlotController.cs
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

public class EquipSlotController : MonoBehaviour 
{
	public enum eEquipSLotType
	{
		PowerUp,
		SpiritEquip,
		InventoryEquip,
		SpiritBag,
		InventoryBag,
		Trash
	}

	public enum eSpiritEquipSlotType
	{
		Auto = 0,
		Mobility = 1,
		Charge = 2,
		AOE = 3,
		Ultimate = 4,

		None = 99
	}

	public eEquipSLotType _slotType;
	public eSpiritEquipSlotType _spiritSlotType = eSpiritEquipSlotType.None;
}
