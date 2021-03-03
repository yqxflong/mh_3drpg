using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using _HotfixScripts.Utils;
using Hotfix_LT.Player;

namespace Hotfix_LT.UI
{
	public class PlayerEquipmentDataLookup : DataLookupHotfix, IHotfixUpdate
	{
		public static readonly List<string> VALID_EQUIPMENT_SLOTS = new List<string>(
			new List<string>() { "Armor", "Weapon", "Head" }
		);


		public static readonly List<string> VALID_SCENE_EQUIP_SLOTS = new List<string>
		{
			"W",
			"H",
			"A",
		};

		private IDictionary cachedEquippedItems;
		private Dictionary<string, string> preEquipmentDic = new Dictionary<string, string>();
		private MoveController _mc;

		public MoveController moveController
		{
			get
			{
				if (_mc == null)
				{
					_mc = mDL.transform.GetComponent<MoveController>();
				}
				return _mc;
			}
		}

		public int kLobbyIdleHash = 0;
		public float kLobbyAnimTransitionTime = 0.0f;
		public float randomIdleTriggerTimeMin = 5.0f;
		public float randomIdleTriggerTimeMax = 8.0f;
		protected float randomIdleTriggerTime = -1;
		public bool needToTransitionToIdle = false;

		private bool firstonload;
		public bool FirstOnLoad
		{
			get
			{
				return firstonload;
			}
			set
			{
				firstonload = value;
			}
		}

		private string scenetype;
		private long userid;
		public string SceneType
		{
			set
			{
				scenetype = value;

			}
		}

		public string getDataIdPrefix()
		{
			return scenetype + "." + "pl" + "." + userid + ".";
		}

		public string GetDataId()
		{
			string dataId = string.Empty;
			//主角面板数据同步
			if (scenetype == null)
			{
				dataId = "buddyinventory.pos0.equippedItems";
			}
			else
			{
				dataId = getDataIdPrefix() + "equip";
			}
			return dataId;
		}

		public long UserId
		{
			set
			{
				userid = value;

				//主角面板数据同步
				/*if (scenetype == null)
				{
					string dataid = "buddyinventory.pos0.equippedItems";
					this.RegisterDataID(dataid);
				}
				else
				{
					string dataid = getDataIdPrefix()+"equip";
					this.RegisterDataID(dataid);
				}*/
				if (!mDL.IsDataIDRegistered(GetDataId()))
				{
					mDL.RegisterDataID(GetDataId());
				}
				else
				{
					string dataId = GetDataId();
					object equipData;
					DataLookupsCache.Instance.SearchDataByID<object>(dataId, out equipData);
					OnLookupUpdate(dataId, equipData);
				}
			}
			get
			{
				return userid;
			}
		}
		public override void OnEnable()
		{
			FirstOnLoad = true;

			preEquipmentDic.Clear();
			cachedEquippedItems = null;
		}

		public override void OnLookupUpdate(string dataID, object value)
		{
			if (!mDL.hasStarted)
			{
				return;
			}
			if (dataID == null) return;
			base.OnLookupUpdate(dataID, value);

			IDictionary incomingData =value as IDictionary;
			if (incomingData == null) return; // useful when previewing in InventoryView scene

			PlayerFashionDataLookUp pfdl = mDL.gameObject.GetDataLookupILRComponent<PlayerFashionDataLookUp>();
			if (pfdl != null && pfdl.IsLoadFashion)
			{
				return;
			}
			foreach (DictionaryEntry entry in incomingData)
			{
				string equipmentType = null;
				string economyid = null;
				if (scenetype == null)
				{
					economyid = mDL.GetLookupData<string>((string)entry.Value + ".economy_id");
					equipmentType = (string)entry.Key;
				}
				else
				{
					equipmentType = EconomyConstants.AbToEquipmentType((string)entry.Key);
					if (entry.Value == null)
					{//这个其实是脱下装备的逻辑 UpdateEquipment 与这里相呼应
						economyid = null;
					}
					else
					{
						economyid = (string)entry.Value;
					}
				}

				if (
					(cachedEquippedItems == null
						||
						!cachedEquippedItems.Contains(equipmentType)
						||
						cachedEquippedItems[equipmentType] == null
						||
						cachedEquippedItems[equipmentType].ToString() != economyid
					)
					&&
					VALID_EQUIPMENT_SLOTS.Contains(equipmentType)
				)
				{
					UpdateEquipment(equipmentType, economyid);


					if (preEquipmentDic.ContainsKey(equipmentType))
					{
						preEquipmentDic[equipmentType] = economyid;
					}
					else
					{
						preEquipmentDic.Add(equipmentType, economyid);
					}
				}
			}
			cachedEquippedItems = Johny.HashtablePool.Claim(incomingData);
			if (FirstOnLoad == true)
			{
				FirstOnLoad = false;
			}
		}

		/// <summary>
		/// 当玩家卸载时装且当前无装备时加载默认模型
		/// </summary>
		public void LoadDeafultEquip(string equipType)
		{
			if (VALID_EQUIPMENT_SLOTS.Contains(equipType))
			{
				//UpdateEquipment(equipType, null);//为了不影响新角色的加载暂时先干掉
			}
		}

		/// <summary>
		/// 随着不同的人物模垿装备的默认模型也跟随者变匿 这里限定了： 人物模型必须昿Name-Variant   装备模型的第一级必须是：Name_Model_Armor;
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="equipmentType"></param>
		/// <returns></returns>
		private string GetDefaltEquipAssetName(string userid, string equipmentType)
		{
			string model_name = BuddyAttributesManager.GetModelClass(userid);
			if (string.IsNullOrEmpty(model_name))
			{
				EB.Debug.LogError("Can not Find equipe model for {0} equip " , userid , equipmentType);
				return "Muniuma" + "_Model_" + equipmentType;
			}
			else
			{
				return model_name.Replace("-Variant", "") + "_Model_" + equipmentType;
			}
		}

		public static string GetModeAtributeName(string userid)
		{
			string model_name = BuddyAttributesManager.GetModelClass(userid);
			if (string.IsNullOrEmpty(model_name))
			{
				return string.Empty;
			}
			model_name = model_name.Replace("-Variant", "");
			if (string.IsNullOrEmpty(model_name))
			{
				return "model_muniuma";
			}
			else if (model_name.Equals("Muniuma"))
			{
				return "model_muniuma";
			}
			else if (model_name.Equals("Lieyan"))
			{
				return "model_lieyan";
			}
			else if (model_name.Equals("Yuanhao"))
			{
				return "model_yuanhao";
			}
			else if (model_name.Equals("Linglong"))
			{
				return "model_linglong";
			}
			else if (model_name.Equals("Huanggang"))
			{
				return "model_huanggang";
			}
			else
			{
				return "model_muniuma";
			}
		}
		/// <summary>
		/// Muniuma_Model   Muniuma+"_Model_"    Muniuma 取自Muniuma-Variant
		/// </summary>
		/// <param name="equipmentType"></param>
		/// <param name="inventoryDataID"></param>
		protected void UpdateEquipment(string equipmentType, string ecomomy_id)
		{
			string equipmentAssetName;
			//卸下逻辑
			if (string.IsNullOrEmpty(ecomomy_id))
			{
				equipmentAssetName = GetDefaltEquipAssetName(userid + "", equipmentType);
			}
			else
			{   //穿上逻辑
				if (string.IsNullOrEmpty(ecomomy_id))
				{
					equipmentAssetName = GetDefaltEquipAssetName(userid + "", equipmentType);
				}
				else
				{
					string raceModel = PlayerEquipmentDataLookup.GetModeAtributeName(userid.ToString());
					if (string.IsNullOrEmpty(raceModel))
					{
						equipmentAssetName = GetDefaltEquipAssetName(userid + "", equipmentType);
					}
					else
					{
						//string equipmentName = EconemyTemplateManager.GetPartitionName(raceModel, ecomomy_id); TODOX
						string equipmentName = string.Empty;
						if (string.IsNullOrEmpty(equipmentName))
						{
							equipmentAssetName = GetDefaltEquipAssetName(userid + "", equipmentType);
						}
						else
						{
							equipmentAssetName = equipmentName;
						}
					}
				}
			}

			//bool foundAsset = false;

			string nexteqpname = ecomomy_id;
			string preeqpname = null;
			if (preEquipmentDic.ContainsKey(equipmentType))
			{
				preeqpname = preEquipmentDic[equipmentType];
			}
			OnEquipmentChanged(equipmentType, preeqpname, nexteqpname);

			AvatarComponent avatar = mDL.transform.GetComponent<AvatarComponent>();
			if (avatar != null)
			{
				avatar.LoadEquipment(equipmentType, equipmentAssetName/*, inventoryDataID*/);
			}

			long uid = 0;
			DataLookupsCache.Instance.SearchDataByID<long>("user.uid", out uid);//LoginManager.Instance.LocalUserId.Value 
			if (userid == uid && !PerformanceManager.Instance.CurrentEnvironmentInfo.slowDevice)
				GameEngine.Instance.SetHideColorTarget(mDL.gameObject);
		}

		public void Update()
		{

			if (!GameEngine.Instance.IsTimeToRootScene)
			{
				return;
			}
			if (needToTransitionToIdle)
			{
				MoveEditor.Move move = moveController.GetMoveByState(MoveController.CombatantMoveState.kLobby);
				if (move != null)
				{
					needToTransitionToIdle = false;
					TransitionToMove(move);
					kLobbyIdleHash = moveController.m_lobby_hash; // first kLobby move
					randomIdleTriggerTime = UnityEngine.Random.Range(randomIdleTriggerTimeMin, randomIdleTriggerTimeMax);
				}
			}
			else if (moveController.CurrentState == MoveController.CombatantMoveState.kLobby)
			{
				if (!moveController.InTransition(0) && !moveController.InTransition(1))
				{
					AnimatorStateInfo asi = moveController.GetCurrentStateInfo();
					float threshHold = asi.fullPathHash != kLobbyIdleHash ? 1.0f : (randomIdleTriggerTime / moveController.CurrentMove._animationClip.length);
					if ((asi.normalizedTime + (kLobbyAnimTransitionTime / moveController.CurrentMove._animationClip.length)) >= threshHold)
					{
						MoveEditor.Move[] moves = moveController.GetMovesByState(MoveController.CombatantMoveState.kLobby);
						if (asi.fullPathHash != kLobbyIdleHash)
						{
							TransitionToMove(moves[0]);
						}
						else if (moves.Length > 1)
						{
							int rand = UnityEngine.Random.Range(1, moves.Length);
							TransitionToMove(moves[rand]);
						}
					}
				}
			}
		}

		public void ForceTransitionToAlternateIdle()
		{
			if (kLobbyIdleHash != moveController.m_lobby_hash)
			{
				return;
			}

			MoveEditor.Move[] moves = moveController.GetMovesByState(MoveController.CombatantMoveState.kLobby);
			if (moves.Length > 1)
			{
				int rand = UnityEngine.Random.Range(1, moves.Length);
				TransitionToMove(moves[rand]);
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

		protected void TransitionToMove(MoveEditor.Move theMove)
		{
			if (theMove != null)
			{
				moveController.TransitionTo(MoveController.CombatantMoveState.kLobby);
				moveController.m_lobby_hash = Animator.StringToHash("Base Layer.LobbyIdle." + theMove.name);
				moveController.SetMove(theMove);
				moveController.CrossFade(moveController.m_lobby_hash, kLobbyAnimTransitionTime / theMove._animationClip.length, 0, 0.0f);
			}

			//randomIdleTriggerTime = UnityEngine.Random.Range(randomIdleTriggerTimeMin, randomIdleTriggerTimeMax);
		}

		void OnEquipmentChanged(string equipmentType, string preeqp, string nexteqp)
		{
			// don't play sound first time.
			if (FirstOnLoad == true)
			{
				return;
			}

			if (preeqp != null && nexteqp != null && preeqp == nexteqp)
			{
				return;
			}
			//string defaulteapname = "Default" + equipmentType;
			if (equipmentType.CompareTo("Armor") == 0)
			{
				if (preeqp == null)
				{
					FusionAudio.PostEvent("UI/Inventory/Body/Equip", mDL.gameObject, true);
				}
				else if (nexteqp == null)
				{
					FusionAudio.PostEvent("UI/Inventory/Body/Unequip", mDL.gameObject, true);
				}
				else
				{
					FusionAudio.PostEvent("UI/Inventory/Body/Swap", mDL.gameObject, true);
				}

			}
			else if (equipmentType.CompareTo("Head") == 0)
			{
				if (preeqp == null)
				{
					FusionAudio.PostEvent("UI/Inventory/Head/Equip", mDL.gameObject, true);
				}
				else if (nexteqp == null)
				{
					FusionAudio.PostEvent("UI/Inventory/Head/Unequip", mDL.gameObject, true);
				}
				else
				{
					FusionAudio.PostEvent("UI/Inventory/Head/Swap", mDL.gameObject, true);
				}
			}
			else if (equipmentType.CompareTo("Weapon") == 0)
			{
				if (preeqp == null)
				{
					FusionAudio.PostEvent("UI/Inventory/Weapon/Equip", mDL.gameObject, true);
				}
				else if (nexteqp == null)
				{
					FusionAudio.PostEvent("UI/Inventory/Weapon/Unequip", mDL.gameObject, true);
				}
				else
				{
					FusionAudio.PostEvent("UI/Inventory/Weapon/Swap", mDL.gameObject, true);
				}
			}
		}
	}
}