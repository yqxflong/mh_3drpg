using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class FetchPushMsgManager : SubSystem
	{
		private static EndPoint sEndPoint;
		private static EB.Sparx.PushManager sPushManager;

		public override void Initialize(Config config)
		{
			sEndPoint = SparxHub.Instance.ApiEndPoint;
			sPushManager = SparxHub.Instance.GetManager<EB.Sparx.PushManager>();
		}

		public override void Connect()
		{
			State = SubSystemState.Connected;
		}

		public override void Disconnect(bool isLogout)
		{
			State = SubSystemState.Disconnected;
		}

		public override void Async(string message, object payload)
		{
			switch (message.ToLower())
			{
				case "fetch_push_msg":
					OnFetchPushMsgReceived(payload);
					break;
				default:
					EB.Debug.LogError("FetchPushMsgManager: No response defined for async message " + message);
					break;
			}
		}

		private void OnFetchPushMsgReceived(object payload)
		{
			var request = sEndPoint.Post("/fetchpushmsg");
			request.numRetries = 1;

			int msgIndex = EB.Dot.Integer("data.msgIndex", payload, 0);
			request.AddData("msgIndex", msgIndex);

			EB.Debug.Log(System.DateTime.Now + " ~ OnFetchPushMsgReceived is running ... msgIndex = " + msgIndex);

			sEndPoint.Service(request, delegate (Response result)
			{
				if (!result.sucessful)
				{
					EB.Debug.LogError("FetchPushMsgReceived.OnFetchPushMsgReceived - /fetchPushMsg failed: " + result.error.ToString());
					if (result.fatal)
					{
						Hub.FatalError(result.localizedError);
					}
				}
				else
				{
					sPushManager.HandleResult(result.result);
				}
			});
		}
	}
}
