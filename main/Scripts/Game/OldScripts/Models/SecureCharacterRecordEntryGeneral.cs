///////////////////////////////////////////////////////////////////////
//
//  CharacterRecordEntryGeneral.cs
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

public enum eGender
{
	Male,
	Female
}

public class SecureCharacterRecordEntryGeneral : SecureCharacterRecordEntry
{
	public string name;
	//public eClass characterClass;
	public eGender gender;
	public int xp;
	public int level;

	public HashSet<int> completedDungeons = new HashSet<int>();

	//public eClass Class
	//{
	//	get
	//	{
	//		return characterClass;
	//	}
	//	set
	//	{
	//		characterClass = value;
	//	}
	//}

	//private CharacterModel _characterModel;
	//public CharacterModel CharacterModel
	//{
	//	get 
	//	{
	//		if (_characterModel != null) 
	//		{
	//			return _characterModel;
	//		}
	//		_characterModel = CharacterCatalog.Instance.GetModel(Class.ToString());

	//		return _characterModel;
	//	}
	//}

	public SecureCharacterRecordEntryGeneral(CharacterRecord record) : base(record)
	{
	}

	//ICharacterRecordEntry
	public override void Initialize()
	{
		name = "New Character";

		xp = 0;
		level = 1;
	}

	public override void PrepareData()
	{
		
	}

	public override void SaveUniqueData(Hashtable record)
	{
		Hashtable completedDungeonsObject = Johny.HashtablePool.Claim();
		foreach (int dungeonId in completedDungeons) 
		{
			completedDungeonsObject.Add(dungeonId, 1);
		}
		record["completedDungeons"] = completedDungeonsObject;
		//record["class"] = (int)characterClass;
		record["gender"] = (int)gender;
	}

	public override void LoadUniqueData(Hashtable record)
	{
		completedDungeons.Clear();
		Hashtable completedDungeonsObject = EB.Dot.Object("completedDungeons", record, null);
		if(completedDungeonsObject != null){
			foreach (var i in completedDungeonsObject.Keys)
			{
				completedDungeons.Add(System.Convert.ToInt32(i));
			}
		}
		//characterClass = (eClass)EB.Dot.Integer("class", record, (int)eClass.Barbarian);
		gender = (eGender)EB.Dot.Integer("gender", record, (int)eGender.Male);
	}

	//SecureCharacterRecordEntry
	public override void Update() 
	{
		base.Update();
	}

	public override void Load(Hashtable record)
	{
		base.Load(record);

		CharacterRecord.ReadFromHashtable(this, record);	
		
		//Leaderboards.ReportAccountXP();
	}
}