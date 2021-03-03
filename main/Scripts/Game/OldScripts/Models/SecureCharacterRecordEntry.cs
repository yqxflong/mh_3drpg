///////////////////////////////////////////////////////////////////////
//
//  SecureCharacterRecordEntry.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;

public abstract class SecureCharacterRecordEntry : ICharacterRecordEntry
{
	protected CharacterRecord _characterRecord;
	protected int _lastRecordHash;

	public SecureCharacterRecordEntry(CharacterRecord characterRecord) 
	{
		_characterRecord = characterRecord;
	}

	public bool HasRecordChanged(Hashtable record) 
	{
		int newHash = EB.JSON.Stringify(record).GetHashCode();
		return  newHash != _lastRecordHash;
	}

	public virtual void Update() 
	{

	}

	public virtual void Load(Hashtable newData) 
	{

	}

	public static int GetNextUID() 
	{
		return System.Guid.NewGuid().GetHashCode();
	}
	
	public abstract void Initialize();
	public abstract void PrepareData();
	public abstract void SaveUniqueData(Hashtable record);
	public abstract void LoadUniqueData(Hashtable record);
}