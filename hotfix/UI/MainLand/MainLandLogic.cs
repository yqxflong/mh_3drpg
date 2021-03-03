using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
	public class MainLandLogic : SceneLogic
	{
		protected static MainLandLogic m_Instance;
		public TaskManagerDataLookup m_TaskManager;

		protected static bool m_Inited = false;

		public static MainLandLogic GetInstance()
		{
			if (m_Inited)
			{
				return m_Instance;
			}

			m_Inited = true;
			GameObject go = new GameObject("_MainLandLogic");						
			DynamicMonoILR monoILR = go.AddComponent<DynamicMonoILR>();
			monoILR.hotfixClassPath = "Hotfix_LT.UI.MainLandLogic";
			m_Instance =  go.GetMonoILRComponent<MainLandLogic>();
			//m_Instance = go.AddComponent<MainLandLogic>();
			m_Instance.Initialize();			
			DataLookupILR dataup = go.AddComponent<DataLookupILR>();
			dataup.hotfixClassPath = "Hotfix_LT.UI.TaskManagerDataLookup";
			m_Instance.m_TaskManager = go.GetDataLookupILRComponent<TaskManagerDataLookup>();
			SceneTriggerManager.Instance.Init();
			return m_Instance;
		}

		public static void Dispose()
		{
			SceneState = eSceneState.NoneScene;
			if (m_Instance != null)
			{
				m_Instance.StopAllCoroutines();
			}
			GameObject go = GameObject.Find("_MainLandLogic");
			if (go != null)
			{
				GameObject.Destroy(go);
			}

			m_Instance = null;
			m_Inited = false;
		}

		public override void RegisterDataID()
		{
			if (m_TaskManager != null)
			{
				UnRegisterDataID();
				m_TaskManager.mDL.RegisterDataID("tasks");
			}
		}

		public override void UnRegisterDataID()
		{
			if (m_TaskManager != null)
			{
				m_TaskManager.mDL.UnregisterDataID("tasks");
			}
		}

		protected override void GoToSceneViewInFSM()
		{
			SendGameFlowEvent("GoToMainLandView");
		}

		private bool IsTransfering = false;
		public void RequestTransfer(string from_id, UnityEngine.Vector3 from_pos, float from_dir, string to_id, UnityEngine.Vector3 to_pos, float to_dir, System.Action<bool> callback = null, bool isRob = false)
		{
			if (IsTransfering)
			{
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			IsTransfering = true;

			SceneState = eSceneState.RequestingScene;
			m_SparxSceneManager.RequestTransfer(from_id, from_pos, from_dir, to_id, to_pos, to_dir, delegate (EB.Sparx.Response result)
			{
				IsTransfering = false;
				if (result.empty)
				{
					SceneState = eSceneState.SceneLoop;
					if (callback != null)
					{
						callback(false);
					}
					return;
				}
				if (callback != null)
				{
					callback(result.sucessful);
				}

				if (result.sucessful)
				{
					CleanMainLandData();
					DataLookupsCache.Instance.CacheData(result.hashtable);
					if (result.hashtable == null)
					{
						EB.Debug.LogError("mainland session data in DB is null.");
					}
					else
					{
						if (!result.hashtable.Contains("mainlands"))
						{
							EB.Debug.LogError("Wrong mainland session data format in response to campaign start request.");
							return;
						}
						Hashtable mainland = result.hashtable["mainlands"] as Hashtable;
						OnSceneEnter(mainland);
					}
				}
				else if (result.fatal)
				{
					SceneState = eSceneState.SceneLoop;
					SparxHub.Instance.FatalError(result.localizedError);
				}
				else
				{
					SceneState = eSceneState.SceneLoop;
					MessageDialog.Show(EB.Localizer.GetString("ID_MESSAGE_TITLE_STR"), result.localizedError, EB.Localizer.GetString("ID_MESSAGE_BUTTON_STR"), null, false, true, true, delegate (int ret)
					{
					}, NGUIText.Alignment.Center);
				}
			});
		}

		private bool IsRobDart = false;
		private long RobUid;
		private string RobDartId;
		public void RequestTransfer_Rob(string from_id, UnityEngine.Vector3 from_pos, float from_dir, string to_id, UnityEngine.Vector3 to_pos, float to_dir, long uid, string dart_id)
		{
			IsRobDart = true;
			RobUid = uid;
			RobDartId = dart_id;
			RequestTransfer(from_id, from_pos, from_dir, to_id, to_pos, to_dir);
		}

		protected void OnTransfer(EB.Sparx.Response result)
		{
			if (result.sucessful)
			{
				CleanMainLandData();
				DataLookupsCache.Instance.CacheData(result.hashtable);
				if (result.hashtable == null)
				{
					EB.Debug.LogError("mainland session data in DB is null.");
				}
				else
				{
					if (!result.hashtable.Contains("mainlands"))
					{
						EB.Debug.LogError("Wrong mainland session data format in response to campaign start request.");
						return;
					}
					Hashtable mainland = result.hashtable["mainlands"] as Hashtable;
					OnSceneEnter(mainland);
				}
			}
			else if (result.fatal)
			{
				SparxHub.Instance.FatalError(result.localizedError);
			}
			else
			{
				MessageDialog.Show(EB.Localizer.GetString("ID_MESSAGE_TITLE_STR"), result.localizedError, EB.Localizer.GetString("ID_MESSAGE_BUTTON_STR"), null, false, true, true, delegate (int ret)
				{
					UIStack.Instance.HideLoadingScreen();
				}, NGUIText.Alignment.Center);
			}
		}

		public void RobDartFight()
		{
			if (IsRobDart)
			{
				IsRobDart = false;
				AlliancesManager.Instance.Fight(RobUid, RobDartId);
			}
		}

		public void FreshMapName()
		{
			if (Hotfix_LT.Data.SceneTemplateManager.Instance == null)
				return;
			Hotfix_LT.Data.MainLandTemplate mt = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLand(m_CurrentSceneName);
		}

		public static void CleanMainLandData()
		{
			Hashtable root = Johny.HashtablePool.Claim();
			Hashtable mainlands = Johny.HashtablePool.Claim();
			root["mainlands"] = mainlands;
			mainlands["npc_list"] = null;
			mainlands["pl"] = null;
			DataLookupsCache.Instance.CacheData("mainlands.npc_list", root);
			Johny.HashtablePool.Release(root);
		}

		protected override void OnNpcSpawnFinish()
		{
			base.OnNpcSpawnFinish();
		}
	}
}