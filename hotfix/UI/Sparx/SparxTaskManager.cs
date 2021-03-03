using EB.Sparx;

namespace Hotfix_LT.UI
{
	public class TaskManager : ManagerUnit
	{
		private EB.Sparx.EndPoint m_endPoint;

		public override void Initialize(EB.Sparx.Config config)
		{
			m_endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
		}

		public override void Async(string message, object payload)
		{

		}

		public override void OnLoggedIn()
		{
			base.OnLoggedIn();
			CurrentIndex = 0;
		}

		public int CurrentIndex;

		public void RequestAccept(string taskid, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/mhjtasks/accept");
			request.AddData("task_id", taskid);

			m_endPoint.Service(request, callback);
		}

		public void RequestFinish(string taskid, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/mhjtasks/finish");
			request.AddData("task_id", taskid);

			m_endPoint.Service(request, callback);
		}

		public void RequestComplete(string taskid, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/mhjtasks/complete");
			request.AddData("task_id", taskid);

			m_endPoint.Service(request, callback);
		}

		public void RequestGiveUp(string taskid, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/mhjtasks/giveup");
			request.AddData("task_id", taskid);

			m_endPoint.Service(request, callback);
		}

		public void RequestChatTaskFinish(int task_id, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/mhjtasks/finish/talkInWorld");
			request.AddData("task_id", task_id.ToString());
			m_endPoint.Service(request, callback);
		}

		public void RequestSetBountyHero(int tplId, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/mhjtasks/setBountyHero");
			request.AddData("tplId", tplId.ToString());
			m_endPoint.Service(request, callback);
		}

		public void RequestUplevelTaskFinish(int task_id, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/mhjtasks/finish/clickBuddyLevelUp");
			request.AddData("task_id", task_id.ToString());
			m_endPoint.Service(request, delegate (Response response) { DataLookupsCache.Instance.CacheData(response.hashtable); });
		}
		
		public void RequestUpdateCombatPower(int power,int task_id, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/mhjtasks/update/br");
			request.AddData("br", power);
			request.AddData("task_id", task_id.ToString());
			m_endPoint.Service(request, delegate (Response response) { DataLookupsCache.Instance.CacheData(response.hashtable); });
		}
	}
}
