///////////////////////////////////////////////////////////////////////
//
//  CharacterRecord.cs
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
using System.Linq;
using System.Reflection;

public class CharacterRecord
#if DEBUG
	: IDebuggable 
#endif
{
	public enum LoadStateType
	{
		Loading = 0,
		Loaded,
		Failed,
	};

	public int Id { get; private set; }
	public string PortraitId { get; set; }


	public LoadStateType LoadState { get; private set; }

	private List<ICharacterRecordEntry> _characterRecordEntries = new List<ICharacterRecordEntry>();

	private Hashtable _stash; //Stored state before dungeon started

	public CharacterRecordEntryCustomization CustomizationRecord
	{
		get;
		private set;
	}

	public CharacterRecordEntryAbilities AbilitiesRecord
	{
		get;
		private set;
	}

	public CharacterRecordEntryStatistics StatisticsRecord
	{
		get;
		private set;
	}

	//public CharacterRecordEntryQuests QuestsRecord
	//{
	//	get;
	//	private set;
	//}

	public CharacterRecordEntryProgressFlags ProgressFlagsRecord
	{
		get;
		private set;
	}

	public SecureCharacterRecordEntryGeneral GeneralRecord
	{
		get; 
		private set;
	}

	public SecureCharacterRecordEntryEquipment EquipmentRecord
	{
		get;
		private set;
	}

	public EB.Sparx.CharacterData SparxCharacterData
	{
		get;
		set;
	}

	public bool Locked
	{
		get;
		private set;
	}

	public event System.Action LoadEvent;

	private	static System.Type[] _JSONTypes = new System.Type[] 
	{
		typeof(bool),
		typeof(float),
		typeof(long),
		typeof(int),
		typeof(uint),
		typeof(string)
	};

	public CharacterRecord( EB.Sparx.CharacterData data )
	{
		_characterRecordEntries = new List<ICharacterRecordEntry>();

		CustomizationRecord = new CharacterRecordEntryCustomization();
		_characterRecordEntries.Add(CustomizationRecord);

		AbilitiesRecord = new CharacterRecordEntryAbilities();
		_characterRecordEntries.Add(AbilitiesRecord);

		StatisticsRecord = new CharacterRecordEntryStatistics();
		_characterRecordEntries.Add(StatisticsRecord);

		//ToDo:
		//QuestsRecord = new CharacterRecordEntryQuests();
		//_characterRecordEntries.Add(QuestsRecord);

		ProgressFlagsRecord = new CharacterRecordEntryProgressFlags();
		_characterRecordEntries.Add(ProgressFlagsRecord);

		EquipmentRecord = new SecureCharacterRecordEntryEquipment(this);
		GeneralRecord = new SecureCharacterRecordEntryGeneral(this);

		foreach (ICharacterRecordEntry entry in _characterRecordEntries)
		{
			entry.Initialize();
		}
		EquipmentRecord.Initialize();
		GeneralRecord.Initialize();

		if ( null == data )
		{
			return;
		}

		int characterId = data.Id;
		if ( characterId < 0 )
		{
			LoadState = LoadStateType.Failed;
			return;
		}

		PortraitId = data.PortraitId;

#if DEBUG
		//DebugSystem.RegisterInstance("Character " + characterId, this, "Character");
#endif

		LoadState = LoadStateType.Loading;
		Load(data);

#if DEBUG
		if ( LoadStateType.Failed == LoadState )
		{
			//DebugSystem.UnregisterSystem(this);
		}
#endif
	}

	public void Destroy()
	{
#if DEBUG
		//DebugSystem.UnregisterSystem(this);
#endif
	}

	// Fails if you are in the middle of a dungeon (character record is locked)
	public void Dump( Hashtable properties )
	{
		foreach (ICharacterRecordEntry entry in _characterRecordEntries)
		{
			entry.PrepareData();
			var ht = Johny.HashtablePool.Claim();
			GetRecordHashtable(entry, ht);
			properties[entry.GetType().ToString()] = ht;
		}
	}

	public void Load( EB.Sparx.CharacterData data )
	{
		LoadState = LoadStateType.Loading;

		if ( data.Id < 0 )
		{
			LoadState = LoadStateType.Failed;
			return;
		}

		Id = data.Id;
		PortraitId = data.PortraitId;

#if DEBUG
		DebugSystem.Log(this, "load character: " + Id);
#endif

		foreach (ICharacterRecordEntry entry in _characterRecordEntries)
		{
			ReadFromHashtable(entry, data.Properties[entry.GetType().ToString()] as Hashtable);
		}

		EquipmentRecord.Load(data.Equipment);
		GeneralRecord.Load(data.General);

		if ( LoadStateType.Loading == LoadState )
		{
			SparxCharacterData = data;
			LoadState = LoadStateType.Loaded;
		}

		if (LoadEvent != null)
		{
			LoadEvent();
		}

		//EventManager.instance.Raise(new CharacterRecordReloadEvent());
	}

	public static void GetRecordHashtable(ICharacterRecordEntry entry, Hashtable ht)
	{
		foreach (FieldInfo field in entry.GetType().GetFields())
		{
			if (_JSONTypes.Contains(field.FieldType))
			{
				ht[field.Name] = field.GetValue(entry);
			}
		}		
		entry.SaveUniqueData(ht);
	}

	public static void ReadFromHashtable(ICharacterRecordEntry entry, Hashtable record)
	{
		foreach (FieldInfo field in entry.GetType().GetFields())
		{
			if (_JSONTypes.Contains(field.FieldType))
			{
				if (field.FieldType == typeof(bool))
				{
					field.SetValue(entry, EB.Dot.Bool(field.Name, record, (bool)field.GetValue(entry)));
				}
				else if (field.FieldType == typeof(float)) 
				{
					field.SetValue(entry, EB.Dot.Single(field.Name, record, (float)field.GetValue(entry)));
				}
				else if (field.FieldType == typeof(long)) 
				{
					field.SetValue(entry, EB.Dot.Long(field.Name, record, (long)field.GetValue(entry)));
				}
				else if (field.FieldType == typeof(int)) 
				{
					field.SetValue(entry, EB.Dot.Integer(field.Name, record, (int)field.GetValue(entry)));
				}
				else if (field.FieldType == typeof(uint))
				{
					field.SetValue(entry, EB.Dot.UInteger(field.Name, record, (uint)field.GetValue(entry)));
				}
				else if (field.FieldType == typeof(string)) 
				{
					field.SetValue(entry, EB.Dot.String(field.Name, record, (string)field.GetValue(entry)));
				}
				/*else if (field.FieldType == typeof(ArrayList)) 
				{
					field.SetValue(entry, EB.Dot.Array(field.Name, record, (ArrayList)field.GetValue(entry)));
				}
				else if (field.FieldType == typeof(Hashtable))
				{
					field.SetValue(entry, EB.Dot.Object(field.Name, record, (Hashtable)field.GetValue(entry)));
				}*/
			}
		}
		entry.LoadUniqueData(record);
	}

#if DEBUG

#region IDebuggable implementation

	public void OnDrawDebug()
	{

	}

	public void OnDebugGUI()
	{

	}
	
	public void OnDebugPanelGUI()
	{
		GUILayout.BeginVertical();
		
		GUILayout.Label("Name: " + GeneralRecord.name);
		//GUILayout.Label("Class: " + GeneralRecord.Class);
		GUILayout.Label("Ability level: ");

		/////////////////// existing ability ///////////////////
		AbilitiesRecord.abilityLevels.All(delegate(KeyValuePair<int, int> entry)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(entry.Key + ": " + entry.Value);

			if (GUILayout.Button("<"))
			{
				AbilitiesRecord.SetAbilityLevel(entry.Key, entry.Value - 1);
				CharacterManager.Instance.UpdateCharacter(this, false, null);
			}

			if (GUILayout.Button(">"))
			{
				AbilitiesRecord.SetAbilityLevel(entry.Key, entry.Value + 1);
				CharacterManager.Instance.UpdateCharacter(this, false, null);
			}
			
			GUILayout.EndHorizontal();
			
			return true;
		});

		/////////////////// Active ability ///////////////////
		GUILayout.Label("Active ability: ");
		AbilitiesRecord.currentAbilities.All(delegate(KeyValuePair<int, int> entry)
		{
			GUILayout.Label(entry.Key + ": " + entry.Value);
			
			return true;
		});
		/////////////////// existing ability ///////////////////

		if ( GUILayout.Button("Delete") )
		{
			CharacterManager.Instance.DeleteCharacter(this.Id, null);
		}

		GUILayout.EndVertical();
	}
#endregion

#endif

}
