using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public class FormationAPI : EB.Sparx.SparxAPI
	{
		public FormationAPI()
		{
			endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
		}

		private void DefaultDataHandler(Hashtable payload)
		{
			EB.Debug.Log("ArenaAPI.DefaultDataHandler: call default data handler");
		}

        public System.Func<EB.Sparx.Response, bool> errorProcessFun = null;
        private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
		{
			dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);
            if (response.error != null)
            {
                EB.Debug.LogError(response.error);
            }

            if (errorProcessFun != null)
            {
                if (!errorProcessFun(response))
                {
                    if (ProcessResponse(response))
                    {
                        dataHandler(response.hashtable);
                    }
                    else
                    {
                        dataHandler(null);
                    }
                }
                else
                {
                    if (response.sucessful)
                    {
                        dataHandler(response.hashtable);
                    }
                }
                errorProcessFun = null;
            }
            else
            {
                if (ProcessResponse(response))
                {
                    dataHandler(response.hashtable);
                }
                else
                {
                    dataHandler(null);
                }
            }
        }

		private int BlockService(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
		{
			LoadingSpinner.Show();

			return endPoint.Service(request, delegate (EB.Sparx.Response response)
			{
				LoadingSpinner.Hide();

				ProcessResult(response, dataHandler);
			});
		}

		private int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
		{
			return endPoint.Service(request, delegate (EB.Sparx.Response response)
			{
				ProcessResult(response, dataHandler);
			});
		}

		/// <summary>
		/// 两个阵容交换
		/// </summary>
		/// <param name="teamName"></param>
		/// <param name="dataHandler"></param>
		public void RequestSwitchTeam(string teamName, System.Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/team/switchTeam");
			request.AddData("teamName", teamName);
			BlockService(request, dataHandler);
		}

		
		
		/// <summary>
		/// 请求上阵
		/// </summary>
		/// <param name="heroId"></param>
		/// <param name="teamName"></param>
		/// <param name="formationPos"></param>
		/// <param name="dataHandler"></param>
		public void RequestDragHeroToFormationPos(int heroId, string teamName, int formationPos, System.Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/team/dragHeroToFormationPos");
			request.AddData("heroId", heroId);
			request.AddData("teamName", teamName);
			request.AddData("formationPos", formationPos);
			BlockService(request, dataHandler);
		}

		
		/// <summary>
		/// 从一个阵容拖动到另外一个阵容
		/// </summary>
		/// <param name="fromHeroId"></param>
		/// <param name="fromTeamName"></param>
		/// <param name="fromFormationPos"></param>
		/// <param name="toHeroId"></param>
		/// <param name="toTeamName"></param>
		/// <param name="toFormationPos"></param>
		/// <param name="dataHandler"></param>
		public void RequestDragHeroToOtherTeam(int fromHeroId, string fromTeamName, int fromFormationPos, int toHeroId, string toTeamName, int toFormationPos, System.Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/team/switchHeros");
			request.AddData("fromHeroId", fromHeroId);
			request.AddData("fromTeamName", fromTeamName);
			request.AddData("fromFormationPos", fromFormationPos);
			request.AddData("toHeroId", toHeroId);
			request.AddData("toTeamName", toTeamName);
			request.AddData("toFormationPos", toFormationPos);
			BlockService(request, dataHandler);
		}

		
		/// <summary>
		/// 移除
		/// </summary>
		/// <param name="heroId"></param>
		/// <param name="teamName"></param>
		/// <param name="dataHandler"></param>
		public void RemoveHeroFromFormation(int heroId, string teamName, System.Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/team/removeHeroFromFormation");
			request.AddData("heroId", heroId);
			request.AddData("teamName", teamName);
			BlockService(request, dataHandler);
		}


		/// <summary>
		/// 请求上阵多个伙伴 在不同的阵容中
		/// </summary>
		/// <param name="data"></param>
		/// <param name="dataHandler"></param>
		public void RequestMulToFormationPos(List<Hashtable> data, System.Action<Hashtable> dataHandler)
		{
			if (data.Count > 0)
			{
				EB.Sparx.Request request = endPoint.Post("/team/dragHeroToFormationPosMulti");
				request.AddData("data", data);
				BlockService(request, dataHandler);
			}
		}
		
		public void RequestHonorArenaOpen(Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/honorarena/open");
			BlockService(request, dataHandler);
		}
	
		/// <summary>
		/// 交换伙伴
		/// </summary>
		/// <param name="data"></param>
		/// <param name="dataHandler"></param>
		public void switchHerosMulti(List<Hashtable> data, System.Action<Hashtable> dataHandler)
		{
			if (data.Count>0)
			{
				EB.Sparx.Request request = endPoint.Post("/team/switchHerosMulti");
				request.AddData("data", data);
				BlockService(request, dataHandler);
			}
			else
			{
				dataHandler(null);
			}
		}

		public void RequestOtherPlayerData(long playerUid, string type, string dataType, object data, System.Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/team/checkOtherPlayerData");
			request.AddData("uid", playerUid);
			request.AddData("type", type);
			request.AddData("data_type", dataType);
			request.AddData("data", data);
			BlockService(request, dataHandler);
		}
		
		public void RequestHonorOtherPlayerData(string playerUid,object data, System.Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/team/checkOtherPlayerData");
			request.AddData("uid", playerUid);
			request.AddData("type", "honor_arena");
			if(data!=null)request.AddData("data", data);
			// request.AddData("data_type", dataType);
			BlockService(request, dataHandler);
		}

		public void GetLineupPreset(long playerUid, System.Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/team/getLineupPreset");
			request.AddData("uid", playerUid);
			BlockService(request, dataHandler);
		}

		public void SaveLineupPreset(long playerUid, string lineupType, int lineupIndex, int[] lineupInfo, System.Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/team/saveLineupPreset");
			request.AddData("uid", playerUid);
			request.AddData("lineup_type", lineupType);
			request.AddData("lineup_index", lineupIndex);
			request.AddData("lineup_info", lineupInfo);
			BlockService(request, dataHandler);
		}

		public void UseAllianceMercenary(int heroId, int position, Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/mercenary/useAllianceMercenary");
			request.AddData("heroId", heroId);
			request.AddData("position", position);
			BlockService(request, dataHandler);
		}
		
		public void UnUseAllianceMercenary(int heroId, int position, Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/mercenary/unUseAllianceMercenary");
			request.AddData("heroId", heroId);
			request.AddData("position", position);
			BlockService(request, dataHandler);
		}
		
		public void GetHeroInfoForView(int heroId, Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/mercenary/getHeroInfoForView");
			request.AddData("heroId", heroId);
			BlockService(request, dataHandler);
		}
		
		
	}
}