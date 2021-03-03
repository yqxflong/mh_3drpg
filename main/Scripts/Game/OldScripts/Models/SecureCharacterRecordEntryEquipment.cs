///////////////////////////////////////////////////////////////////////
//
//  SecureCharacterRecordEntryEquipment.cs
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
using System.Collections;

public class SecureCharacterRecordEntryEquipment : SecureCharacterRecordEntry
{
	public int[] equipmentIDs;// = new int[(int)Equipment.eSlot.CodeEnd];
	//public List<ItemInstance> items;
	//public List<ItemInstance> newItems;

	public int maxItems = 10;// InventoryComponent.DefaultMaxItems; // Number of inventory slots available

	protected EB.Sparx.EquipmentAPI _api = new EB.Sparx.EquipmentAPI(SparxHub.Instance.ApiEndPoint);

	public SecureCharacterRecordEntryEquipment(CharacterRecord record) : base(record)
	{
	}

	//ICharacterRecordEntry
	public override void Initialize()
	{
		for (int i = 0; i < equipmentIDs.Length; ++i)
		{
			equipmentIDs[i] = 0;
		}		
		
		//items = new List<ItemInstance>();
		//newItems = new List<ItemInstance>();
	}

	public override void PrepareData()
	{
		
	}

	public override void SaveUniqueData(Hashtable record)
	{
		//record["chest"] = equipmentIDs[(int)Equipment.eSlot.Chest];
		//record["leg"] = equipmentIDs[(int)Equipment.eSlot.Leg];
		//record["glove"] = equipmentIDs[(int)Equipment.eSlot.Glove];
		//record["helm"] = equipmentIDs[(int)Equipment.eSlot.Helm];
		//record["weapon"] = equipmentIDs[(int)Equipment.eSlot.Weapon];

		ArrayList itemArrayList = new ArrayList();
		//foreach (ItemInstance item in items)
		//{
		//	itemArrayList.Add(item.ToHashtable());
		//}
		record["items"] = itemArrayList;
	}

	public override void LoadUniqueData(Hashtable record)
	{
		//equipmentIDs[(int)Equipment.eSlot.Chest] = EB.Dot.Integer("chest", record, 0);
		//equipmentIDs[(int)Equipment.eSlot.Leg] = EB.Dot.Integer("leg", record, 0);
		//equipmentIDs[(int)Equipment.eSlot.Glove] = EB.Dot.Integer("glove", record, 0);
		//equipmentIDs[(int)Equipment.eSlot.Helm] = EB.Dot.Integer("helm", record, 0);
		//equipmentIDs[(int)Equipment.eSlot.Weapon] = EB.Dot.Integer("weapon", record, 0);

		for (int i = 0; i < equipmentIDs.Length; i++)
		{
			if (equipmentIDs[i] == -1) equipmentIDs[i] = 0;
		}
	
		//items = new List<ItemInstance>();
		//newItems = new List<ItemInstance>();
		ArrayList itemArrayList = EB.Dot.Array("items", record, new ArrayList());
		//for (int i = 0; i < itemArrayList.Count; i++)
		//{
		//	ItemInstance item = ItemInstance.FromHashtable(itemArrayList[i] as Hashtable);
		//	items.Add(item);

		//	if (item.IsNew)
		//	{
		//		newItems.Add(item);
		//	}
		//}
	}

	//SecureCharacterRecordEntry
	public override void Update() 
	{
		base.Update();

		Hashtable newRecord = GetHashtableForUpdate();
		if (newRecord != null)
		{
			_api.Update(_characterRecord.Id, newRecord);
		}
	}

	public Hashtable GetHashtableForUpdate()
	{
		var ht = Johny.HashtablePool.Claim();
		CharacterRecord.GetRecordHashtable(this, ht);
		if (HasRecordChanged(ht))
		{
			return ht;
		}
		return null;
	}

	public void ForceUpdate() 
	{
		var ht = Johny.HashtablePool.Claim();
		CharacterRecord.GetRecordHashtable(this, ht);
		_api.Update(_characterRecord.Id, ht, null, true);
	}

	public override void Load(Hashtable record)
	{
		base.Load(record);
		CharacterRecord.ReadFromHashtable(this, record);	
		
		//GameObject localPlayer = PlayerManager.LocalPlayerGameObject();
        //var invenory = localPlayer.GetComponent<InventoryComponent>();

        //if (localPlayer != null && invenory != null) 
		//{
        //    invenory.ValidateItemInstances();
		//}

		var ht = Johny.HashtablePool.Claim();
		CharacterRecord.GetRecordHashtable(this, ht);
		_lastRecordHash = EB.JSON.Stringify(ht).GetHashCode();
		Johny.HashtablePool.ReleaseRecursion(ht);
		ht = null;
	}
}