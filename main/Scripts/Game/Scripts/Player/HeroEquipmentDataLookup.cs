using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HeroEquipmentDataLookup : DataLookup
{
	private static readonly List<string> VALID_EQUIPMENT_SLOTS = new List<string>(
		new List<string>(){"Head", "Armor", "Weapon"}
	);

	private IDictionary cachedEquippedItems;

	private Dictionary<string ,string> preEquipmentDic = new Dictionary<string,string > ();

	private MoveController _mc;

	public MoveController moveController
	{
		get
		{
			if (_mc == null)
			{
				_mc = GetComponent<MoveController>();
			}
			return _mc;
		}
	}
	public int kLobbyIdleHash = Animator.StringToHash("Lobby.LobbyIdle");
	public float kLobbyAnimTransitionTime = 0.25f;
	public float randomIdleTriggerTimeMin = 5.0f;
	public float randomIdleTriggerTimeMax = 8.0f;
	private float randomIdleTriggerTime = -1;
	private bool playEquipAnim = false;
	private float lastBattleRating = 0.0f;
	public bool needToTransitionToIdle = false;

	private bool firstonload = true;



	private string  dataid_prefix;
	public string DataId_Prefix
	{
		set
		{
			dataid_prefix = value;
			string dataid = dataid_prefix + ".EquipItem";
			this.RegisterDataID(dataid);
		}
	}
	public override void OnEnable()
	{
		firstonload = true;

		preEquipmentDic.Clear();
		cachedEquippedItems = null;
	}

	public override void OnLookupUpdate(string dataID, object value)
	{
		if (!hasStarted)
		{
			return;
		}
		base.OnLookupUpdate(dataID, value);

		IDictionary incomingData = value as IDictionary;
		if (incomingData == null) return; // useful when previewing in InventoryView scene

		float battleRating;
		DataLookupsCache.Instance.SearchDataByID<float>("heroStats.battleRating", out battleRating);
		playEquipAnim = lastBattleRating < battleRating;
		lastBattleRating = battleRating;

		foreach (DictionaryEntry entry in incomingData)
		{
			string equipmentType = (string) entry.Key;
			string inventoryDataID = (string) entry.Value;

			if (
				(
					cachedEquippedItems == null
					||
					!cachedEquippedItems.Contains(equipmentType)
					||
					cachedEquippedItems[equipmentType].ToString() != inventoryDataID
				)
				&&
				VALID_EQUIPMENT_SLOTS.Contains(equipmentType)
			)
			{
				UpdateEquipment(equipmentType, inventoryDataID);


				if (preEquipmentDic.ContainsKey(equipmentType))
				{
					preEquipmentDic[equipmentType] = inventoryDataID;
				}
				else
				{
					preEquipmentDic.Add(equipmentType, inventoryDataID);
				}
			}
		}

		cachedEquippedItems = incomingData;

		EquipmentColorDataLookup color_lookup = GetComponent<EquipmentColorDataLookup>();
		if (color_lookup == null)
		{
			color_lookup = gameObject.AddComponent<EquipmentColorDataLookup>();
		}
		color_lookup.RegisterEquipments(preEquipmentDic);

		if (firstonload == true)
		{
			firstonload = false;
		}
	}

	/*
	protected void UpdateEquipmentColor(string equipmentType, int equipmentColorIndex)
	{
		CombinedMesh avatar = GetComponent<CombinedMesh>();
		if(avatar != null)
		{
			avatar.UpdateEquipmentColor(equipmentType, equipmentColorIndex);
		}
	}
	*/

	protected void UpdateEquipment(string equipmentType, string inventoryDataID)
	{
		string equipmentAssetName = !string.IsNullOrEmpty(inventoryDataID)
			? GetLookupData<string>(inventoryDataID + ".economy_id") + "-prefab"
				: "MaleWarrior_Default" + equipmentType;

		//bool foundAsset = false;

		string nexteqpname = inventoryDataID;
		string preeqpname = null;
		if (preEquipmentDic.ContainsKey(equipmentType))
		{
			preeqpname = preEquipmentDic[equipmentType];
		}
		OnEquipmentChanged(equipmentType, preeqpname, nexteqpname);

		AvatarComponent avatar = GetComponent<AvatarComponent>();
		if (avatar != null)
		{
			avatar.LoadEquipment(equipmentType, equipmentAssetName/*, inventoryDataID*/);
		}

		/*
		Combatant c = GetComponent<Combatant>();
		if (c != null) {
			if (c.Equipment != null) {
				for(int i = 0; i < c.Equipment.Length; i++)
				{
					if(c.Equipment[i].name == equipmentAssetName)
					{
						foundAsset = true;

						c.Equipment[i].SetActive(true);

						CombinedMesh avatar = GetComponent<CombinedMesh>();

						avatar.SetToCombineGameObjectByRowName(equipmentType, c.Equipment[i], equipmentDataID); 
						OnEquipmentChanged(equipmentType,preeqpname,nexteqpname);

						c.Equipment[i].SetActive(false);
					}
				}
			}
		}
		System.Action<string, GameObject, bool> onAssetLoaded = delegate(string arg1, GameObject arg2, bool arg3) {
			if(!arg3)
			{
				EB.Debug.LogError("Failed to load assset: " + arg1);
				return;
			}
			CombinedMesh avatar = GetComponent<CombinedMesh>();
			avatar.SetToCombineGameObjectByRowName(equipmentType, arg2, equipmentDataID); 
			OnEquipmentChanged(equipmentType,preeqpname,nexteqpname);
			Destroy(arg2);
			Resources.UnloadUnusedAssets(); // slow
		};

		if (!foundAsset) {
			GM.AssetManager.GetAsset<GameObject> (equipmentAssetName, onAssetLoaded, GameEngine.Instance.gameObject); 
		}
		*/
	}

	void Update()
	{
		if (needToTransitionToIdle)
		{
			needToTransitionToIdle = false;
			moveController.TransitionTo(MoveController.CombatantMoveState.kLobby);
			moveController.m_lobby_hash = kLobbyIdleHash;
			moveController.CrossFade(moveController.m_lobby_hash, 0.0f, 0, 0.0f);
			randomIdleTriggerTime = UnityEngine.Random.Range(randomIdleTriggerTimeMin, randomIdleTriggerTimeMax);
		}
		else if (moveController.CurrentState == MoveController.CombatantMoveState.kLobby)
		{
			if (!moveController.InTransition(0) && !moveController.InTransition(1))
			{
				AnimatorStateInfo asi = moveController.GetCurrentStateInfo();
				float threshHold = asi.fullPathHash != kLobbyIdleHash ? 1.0f : (randomIdleTriggerTime / moveController.CurrentMove._animationClip.length);
				if ((asi.normalizedTime + (kLobbyAnimTransitionTime / moveController.CurrentMove._animationClip.length)) >= threshHold)
				{
					if (asi.fullPathHash != kLobbyIdleHash)
					{
						TransitionToMoveByName("LobbyIdle");
					}
					else
					{
						TransitionToMoveByName("LobbyAlternateIdle");
					}
				}
			}
		}
	}

	public void TransitionToMoveByName(string moveName)
	{
		MoveEditor.Move theMove = moveController.GetMoveIfExists(moveName);
		if (theMove != null)
		{
			TransitionToMove(theMove);
		}
	}

	void TransitionToMove(MoveEditor.Move theMove)
	{
		moveController.TransitionTo(MoveController.CombatantMoveState.kLobby);
		moveController.m_lobby_hash = Animator.StringToHash("Lobby." + theMove.name);
		moveController.SetMove(theMove);
		moveController.CrossFade(moveController.m_lobby_hash, kLobbyAnimTransitionTime / theMove._animationClip.length, 0, 0.0f);

		randomIdleTriggerTime = UnityEngine.Random.Range(randomIdleTriggerTimeMin, randomIdleTriggerTimeMax);
	}

	void OnEquipmentChanged(string equipmentType, string preeqp, string nexteqp)
	{
		// skip playing if cachedEquippedItems is null. that means we're just setting up the dude
		bool doIt = playEquipAnim && cachedEquippedItems != null;
		if (doIt)
		{
			TransitionToMoveByName(equipmentType + "Changed");
		}

		// don't play sound first time.
		if (firstonload == true)
		{
			return;
		}

		if (preeqp != null && nexteqp != null && preeqp == nexteqp)
		{
			return;
		}
		//string defaulteapname = "Default" + equipmentType;
		if (equipmentType.CompareTo("Armor")==0)
		{
			if (preeqp == null)
			{
				FusionAudio.PostEvent("UI/Inventory/Body/Equip", gameObject, true);
			}
			else if (nexteqp == null)
			{
				FusionAudio.PostEvent("UI/Inventory/Body/Unequip", gameObject, true);
			}
			else
			{
				FusionAudio.PostEvent("UI/Inventory/Body/Swap", gameObject, true);
			}

		}
		else if (equipmentType.CompareTo("Head")==0)
		{
			if (preeqp == null)
			{
				FusionAudio.PostEvent("UI/Inventory/Head/Equip", gameObject, true);
			}
			else if (nexteqp == null)
			{
				FusionAudio.PostEvent("UI/Inventory/Head/Unequip", gameObject, true);
			}
			else
			{
				FusionAudio.PostEvent("UI/Inventory/Head/Swap", gameObject, true);
			}
		}
		else if (equipmentType.CompareTo("Weapon")==0)
		{
			if (preeqp == null)
			{
				FusionAudio.PostEvent("UI/Inventory/Weapon/Equip", gameObject, true);
			}
			else if (nexteqp == null)
			{
				FusionAudio.PostEvent("UI/Inventory/Weapon/Unequip", gameObject, true);
			}
			else
			{
				FusionAudio.PostEvent("UI/Inventory/Weapon/Swap", gameObject, true);
			}
		}
	}
}
