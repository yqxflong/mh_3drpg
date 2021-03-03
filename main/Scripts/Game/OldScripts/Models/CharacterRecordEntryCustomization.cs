///////////////////////////////////////////////////////////////////////
//
//  CharacterRecordEntryCustomization.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;

public class CharacterRecordEntryCustomization : ICharacterRecordEntry
{
	public int skinDyeID;
	public int hairDyeID;
	public int armorDyeID;
	public int hairID;
	public int headID;

	public void Initialize()
	{
		skinDyeID = 150275595;		// the default value is from Dye mode id
		hairDyeID = 897783824;		// the default value is from Dye mode id
		armorDyeID = 1498456362;	// the default value is from Dye mode id
		//hairID = EquipmentCatalog.InvalidModelID;
		//headID = EquipmentCatalog.InvalidModelID;
	}

	public void PrepareData()
	{
		
	}
	
	public void SaveUniqueData(Hashtable record)
	{

	}

	public void LoadUniqueData(Hashtable record)
	{

	}
}
