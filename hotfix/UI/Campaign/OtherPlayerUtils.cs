//OtherPlayerManager
//其他玩家管理器
//Johny

using UnityEngine;

namespace Hotfix_LT.UI
{
	public static class OtherPlayerUtils
	{
		private const string RES_OtherPlayer = "Bundles/Prefab/OtherPlayer";

		/// <summary>
		/// from Class SceneLogic
		/// </summary>
		/// <param name="playerSpawnEntry"></param>
		/// <param name="themeLoadMgr"></param>
		/// <param name="pos"></param>
		/// <param name="rot"></param>
		/// <param name="userid"></param>
		public static void CreateOtherPlayer(SceneLogic.PlayerEntry playerSpawnEntry, ThemeLoadManager themeLoadMgr, Vector3 pos, Quaternion rot, long userid)
		{
			try
			{
				if (pos.Equals(Vector3.zero))//传送都是这个点 都要换算为出生点
				{
					pos = playerSpawnEntry.position;
					rot = playerSpawnEntry.rotation;
				}

				//lzt 需要排查原因为什么会得到空
				string modelName = BuddyAttributesManager.GetModelClass(userid.ToString());
				float scale_size = 1f;
				eDartState dartState = eDartState.None;
				double fDartState;
				if (!DataLookupsCache.Instance.SearchDataByID<double>(string.Format("mainlands.pl.{0}.state.TOR", userid), out fDartState))
				{
					EB.Debug.LogError("when intact search data dartState state fail");
				}
				dartState = (eDartState)System.Convert.ToInt32(fDartState);
				if (AllianceEscortUtil.GetIsInTransferDart(dartState))
				{
					string dartName = dartState.ToString();
					modelName = AllianceEscortUtil.GetTransportCartModel(AllianceEscortUtil.ToDartStateStr(dartState));
					scale_size = (modelName.IndexOf("M1003") >= 0 || modelName.IndexOf("M1004") >= 0) ? 0.6f : 1;
				}
				if (string.IsNullOrEmpty(modelName))
				{
					EB.Debug.Log("CreateOtherPlayer ModelClassIsNullOrEmpty for userid={0}" , userid);
					return;
				}

				#region Async Load OtherPlayer
				EB.Assets.LoadAsync(RES_OtherPlayer, typeof(GameObject), o =>
				{
					if(!o){
						return;
					}

					SceneRootEntry sceneRoot = themeLoadMgr.GetSceneRoot();
					GameObject localObj = Replication.Instantiate(o, pos, rot) as GameObject;
					Transform PlayerList = sceneRoot.m_SceneRoot.transform.Find("PlayerList");
					if (PlayerList == null)
					{
						GameObject partner = new GameObject("PlayerList");
						partner.transform.SetParent(sceneRoot.m_SceneRoot.transform);
						PlayerList = partner.transform;
					}
					PlayerList.gameObject.CustomSetActive(true);
					localObj.transform.SetParent(PlayerList);

					PlayerController player_controller = localObj.GetComponent<PlayerController>();
					player_controller.playerUid = userid;
					Player.PlayerHotfixController hotfix_controller = player_controller.transform.GetMonoILRComponent<Player.PlayerHotfixController>();
                    hotfix_controller.SetPlayerSpawnLocation(pos);
                    hotfix_controller.CreateOtherPlayer(modelName, userid, scale_size);//创建其他人
					hotfix_controller.InitDataLookupSet();

					OtherPlayerTidDataLookup OPTD = player_controller.transform .GetDataLookupILRComponent<OtherPlayerTidDataLookup>(false);
					if (OPTD == null)
					{
						OPTD = player_controller.gameObject.AddDataLookupILRComponent<OtherPlayerTidDataLookup>("Hotfix_LT.UI.OtherPlayerTidDataLookup");
						string path = string.Format("mainlands.pl.{0}.tid", userid);
						string torStatePath = string.Format("mainlands.pl.{0}.state.TOR", userid);
						DataLookupsCache.Instance.SearchDataByID<string>(path, out OPTD.ModelTid);
						OPTD.DartState = dartState;
						OPTD.mDL.RegisterDataID(path);
						OPTD.mDL.RegisterDataID(torStatePath);
						//OPTD.DefaultDataID= path;
					}
				});
				#endregion
			}
			catch (System.Exception e)
			{
				EB.Debug.LogError("Message: {0}, Stack: {1}", e.Message, e.StackTrace);
				UnityEngine.Debug.LogException(e);
			}
		}
	}

}

