using UnityEngine;
using System.Collections;


namespace Hotfix_LT.UI
{
	public class NationAPI : EB.Sparx.SparxAPI
	{
		public NationAPI()
		{
			endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
		}

		private void DefaultDataHandler(Hashtable payload)
		{
			EB.Debug.Log("NationAPI.DefaultDataHandler: call default data handler");
		}

		private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
		{
			dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);
			if (ProcessResponse(response))
			{
				dataHandler(response.hashtable);
			}
			else
			{
				dataHandler(null);
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

		public void GetInfo(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nations/getInfo");
			BlockService(request, dataHandler);
		}

		public void Select(string nationName, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nations/select");
			request.AddData("name", nationName);
			BlockService(request, dataHandler);
		}

		public void GetMemberList(string nationName, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nations/member/list");
			request.AddData("rank", nationName);
			BlockService(request, dataHandler);
		}

		public void ModifyNotice(string notice, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nations/setNotice");
			request.AddData("text", notice);
			BlockService(request, dataHandler);
		}

		public void ReceiveRankReward(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nations/receiveRankReward");
			BlockService(request, dataHandler);
		}

		public void GetTerritoryInfo(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/getTerritoryInfo");
			Service(request, dataHandler);
		}

		public void EnterField(int index, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/enterField");
			request.AddData("nodeId", index);
			BlockService(request, dataHandler);
		}

		public void StartAction(string path, string team, string[] emptyTeam, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/startAction");
			request.AddData("path", path);
			request.AddData("team", team);
			request.AddData("emptyTeam", emptyTeam);
			BlockService(request, dataHandler);
		}

		public void ChangeNationTeamStage(bool haveHero, string team, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/changeNationWarTeamStage");
			request.AddData("haveHero", haveHero);
			request.AddData("teamName", team);
			BlockService(request, dataHandler);
		}

		public void ContinueWalk(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/continueWalk");
			BlockService(request, dataHandler);
		}

		public void ExitField(int index, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/exitField");
			//request.AddData("nodeId", index);
			BlockService(request, dataHandler);
		}

		public void GetTeamInfo(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/getTeamInfo");
			BlockService(request, dataHandler);
		}

		public void Revive(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/revive");
			//request.AddData("team", teamName);
			BlockService(request, dataHandler);
		}

		public void DonateRank(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Get("/leaderboards/rankNationDegreeRank");
			request.AddData("start", 0);
			request.AddData("end", 3);
			BlockService(request, dataHandler);
		}

		public void ScoreRank(int territoryIndex, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/scoreRank");
			request.AddData("index", territoryIndex);
			BlockService(request, dataHandler);
		}

		public void RefreshCityScore(int status, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/refreshCityScore");
			request.AddData("status", status);
			Service(request, dataHandler);
		}

		public void SureEvent(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/sureEvent");
			BlockService(request, dataHandler);
		}

		public void Call(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/call");
			BlockService(request, dataHandler);
		}

		public void SetSkill(int index, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/setSkill");
			request.AddData("skillId", index);
			BlockService(request, dataHandler);
		}

		#region debug Func
		public void StartEvent(int realmId, int stage, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/startEvent");
			request.AddData("realmId", realmId);
			request.AddData("stage", stage);
			BlockService(request, dataHandler);
		}

		public void StopEvent(int realmId, int stage, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/stopEvent");
			request.AddData("realmId", realmId);
			request.AddData("stage", stage);
			BlockService(request, dataHandler);
		}

		public void ResetNationRank(int realmId, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/resetNationRank");
			request.AddData("realmId", realmId);
			BlockService(request, dataHandler);
		}

		public void AddRobot(string nation, string side, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/addRobot");
			request.AddData("nation", nation);
			request.AddData("side", side);
			BlockService(request, dataHandler);
		}

		public void RobotWork(string side, string path, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/robotWork");
			request.AddData("side", side);
			request.AddData("path", path);
			BlockService(request, dataHandler);
		}

		public void TestSetSkill(int skillId, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/nationswar/testSetSkill");
			request.AddData("skillId", skillId);
			BlockService(request, dataHandler);
		}
		#endregion
	}
}
