///////////////////////////////////////////////////////////////////////
//
//  CharacterManager.cs
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

public class CharacterManager
#if DEBUG
	: IDebuggable 
#endif
{
	private static CharacterManager _instance;
	public static CharacterManager Instance 
	{ 
		get 
		{
			if (_instance == null)
			{
				_instance = new CharacterManager();
			}
			
			return _instance; 
		} 
	}

	public event System.Action OnStaminaRefilled;

	public CharacterRecord CurrentCharacter { get; private set; }
	public bool IsLoaded { get; private set;}

	protected Dictionary<int, CharacterRecord> _characterRecordPreviewMap;
	protected EB.Sparx.CharacterAPI _api = new EB.Sparx.CharacterAPI(SparxHub.Instance.ApiEndPoint);
	protected bool _isLoading = false;

	//private EB.Sparx.CharacterNotificationManager _notifManager;

	public CharacterManager()
	{
		_characterRecordPreviewMap = new Dictionary<int, CharacterRecord>();
		IsLoaded = false;
	}

	public void Start()
	{
#if DEBUG
		//DebugSystem.RegisterInstance("Character Manager", this, "Character Manager");
#endif

		//_notifManager = SparxHub.Instance.GetManager<EB.Sparx.CharacterNotificationManager>();
		//_notifManager.OnSpiritsChanged += OnSpiritsChanged;
		//_notifManager.OnInventoryChanged += OnInventoryChanged;
		//_notifManager.OnGoldChanged += OnGoldChanged;
	}

	public void CreateCharacter(Hashtable properties, Hashtable generalRecord, System.Action<EB.Sparx.eResponseCode, CharacterRecord> callback)
	{
		_api.Create(properties, generalRecord, delegate(EB.Sparx.eResponseCode response, Hashtable result)
		{
			CharacterRecord characterRecord = null;

			if (response == EB.Sparx.eResponseCode.Success)
			{
				EB.Sparx.CharacterData data = new EB.Sparx.CharacterData(EB.Dot.Object("character", result, null));
				characterRecord = new CharacterRecord(data);
				_characterRecordPreviewMap[characterRecord.Id] = characterRecord;
			}
			
			if ( null != callback )
			{
				callback(response, characterRecord);
			}
		});
	}

	public void DeleteCharacter(int characterId, System.Action<EB.Sparx.eResponseCode> callback)
	{
		_api.Delete(characterId, delegate(EB.Sparx.eResponseCode response)
		{
			if (response == EB.Sparx.eResponseCode.Success)
			{
				CharacterRecord characterRecord = _characterRecordPreviewMap[characterId];
				
				characterRecord.Destroy();
				_characterRecordPreviewMap.Remove(characterId);
			}
			
			if ( null != callback )
			{
				callback(response);
			}
		});
	}

	public void GetAllCharacters(System.Action<EB.Sparx.eResponseCode> callback )
	{
		if ( _isLoading )
		{
			return;
		}
		
		_isLoading = true;
		
#if DEBUG
		DebugSystem.Log(this, "Loading Characters");
#endif
		
		_api.GetAll(delegate(EB.Sparx.eResponseCode response, Hashtable result)
		{
			_isLoading = false;
			
			if (response == EB.Sparx.eResponseCode.Success)
			{
				_characterRecordPreviewMap.Clear();
				ArrayList characters = EB.Dot.Array("characters", result, new ArrayList());
				
				foreach (Hashtable character in characters)
				{
					EB.Sparx.CharacterData characterData = new EB.Sparx.CharacterData(character);
					
					if ( characterData.Id >= 0 )
					{
						CharacterRecord characterRecord = new CharacterRecord(characterData);
						_characterRecordPreviewMap[characterData.Id] = characterRecord;
					}
				} 
				
				IsLoaded = true;
				//EventManager.instance.Raise(new OnCharacterLoadedEvent());
			}

			if ( null != callback )
			{
				callback(response);
			}
		});
	}

	//public void OpenCharacter( int characterId, bool blockUI, System.Action<EB.Sparx.eResponseCode> callback )
	//{
	//	_api.Open(characterId, blockUI, delegate(EB.Sparx.eResponseCode response, Hashtable result)
	//	{
	//		if (response == EB.Sparx.eResponseCode.Success)
	//		{
	//			EB.Sparx.CharacterData data = new EB.Sparx.CharacterData(EB.Dot.Object("character", result, null));

	//			if ( null != CurrentCharacter )
	//			{
	//				if (CurrentCharacter.Id == characterId)
	//				{
	//					CurrentCharacter.Load(data);
	//				}
	//				else
	//				{
	//					CurrentCharacter.Destroy();
	//					CurrentCharacter = new CharacterRecord(data);
	//				}
					
	//			}
	//			else
	//			{
	//				CurrentCharacter = new CharacterRecord(data);
	//			}

	//			if ( CurrentCharacter.PortraitId != null )
	//			{
	//				SparxHub.Instance.GetManager<EB.Sparx.SocialManager>().SetPortrait(LoginManager.Instance.LocalUserId.Value, CurrentCharacter.PortraitId, EB.Sparx.SocialManager.PortraitPriority.High);
	//			}
	//		}
			
	//		if ( null != callback )
	//		{
	//			callback(response);
	//		}
	//	});
	//}

	public void ResetCurrentCharacter()
	{
		CurrentCharacter = null;
	}

	public void UpdateCharacter( CharacterRecord characterRecord, bool blockUI, System.Action<EB.Sparx.eResponseCode> callback )
	{
		// TODO: Add properties change check
		Hashtable properties = Johny.HashtablePool.Claim();
		characterRecord.Dump(properties);
		_api.Update(characterRecord.Id, properties, blockUI, delegate(EB.Sparx.eResponseCode response, Hashtable result)
	    {
			if ( null != callback )
			{
				callback(response);
			}
		});
	}
    
	public CharacterRecord GetCharacterRecordPreview( int characterId )
	{
		if ( _characterRecordPreviewMap.ContainsKey(characterId) )
		{
			return _characterRecordPreviewMap[characterId];
		}
		
		return null;
	}
	
	public List<int> GetCharacterIdList()
	{		
		return new List<int>(_characterRecordPreviewMap.Keys);
	}
	
	// force a save at any point
	public void SaveProperties(CharacterRecord record)
	{
		if (null == record)
		{
			return;
		}

		Instance.UpdateCharacter(record, false, (EB.Sparx.eResponseCode response) =>
		{

		});
	}

	//public void OnGoldChanged(EB.Sparx.GoldChangedData changeData)
	//{
	//	if (CurrentCharacter.Id == changeData.CharacterId)
	//	{
	//		CurrentCharacter.SparxCharacterData.Gold = changeData.NewGold;
	//	}
	//}

	//public void OnSpiritsChanged(EB.Sparx.SpiritsChangedData changeData)
	//{
		//CurrentCharacter.SpiritsRecord.Load(changeData.NewSpiritsRecord);
	//}

	//public void OnInventoryChanged(EB.Sparx.InventoryChangedData changeData)
	//{
	//	CurrentCharacter.EquipmentRecord.Load(changeData.NewInventoryRecord);
	//}

#if DEBUG
	
	#region IDebuggable implementation

	public void OnDrawDebug()
	{

	}

	public void OnDebugGUI()
	{

	}

	private int _debugLevel = 0;
	private int _debugXP;
	public void OnDebugPanelGUI()
	{
		GUILayout.BeginVertical();

		if ( GUILayout.Button("Refill Stamina w/ Hard Currency (" + GlobalBalanceData.Instance.staminaRefillCost + ")"))
		{
			//RefillStamina(delegate(EB.Sparx.eResponseCode response) {});
		}
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Zero Gold"))
		{
			_api.DebugSetGold(CurrentCharacter.Id, 0);
		}
		if (GUILayout.Button("Max Gold"))
		{
			_api.DebugSetGold(CurrentCharacter.Id, 999999);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		
		if (_debugLevel == 0)
		{
			_debugLevel = CurrentCharacter.GeneralRecord.level;
			_debugXP = CurrentCharacter.GeneralRecord.xp;
		}

		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		try
		{
			GUILayout.Label("Level");
			int curLevel = _debugLevel;
			_debugLevel = System.Int32.Parse(GUILayout.TextField(_debugLevel + ""));
			if (_debugLevel != curLevel) {
				_debugXP = GlobalBalanceData.Instance.levelXPCap[_debugLevel - 1];
			}
		}
		catch (System.Exception) 
		{
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		try
		{
			GUILayout.Label("XP");
			_debugXP = System.Int32.Parse(GUILayout.TextField(_debugXP + ""));
		}
		catch (System.Exception) 
		{
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		if (GUILayout.Button("Save"))
		{
			if (_debugLevel > 1 && _debugLevel <= GlobalBalanceData.Instance.levelCap) 
			{
				_api.DebugSetLevel(CurrentCharacter.Id, _debugLevel, _debugXP, delegate() {
					_debugLevel = CurrentCharacter.GeneralRecord.level;
					_debugXP = CurrentCharacter.GeneralRecord.xp;
				});
			}
		}
		GUILayout.EndHorizontal();



		GUILayout.EndVertical();
	}
	
#endregion

#endif

}
