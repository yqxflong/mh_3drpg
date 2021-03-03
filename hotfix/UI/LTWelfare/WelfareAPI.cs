using System.Collections;

namespace Hotfix_LT.UI
{
	public class WelfareAPI : EB.Sparx.SparxAPI
	{
		public WelfareAPI()
		{
			endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
		}

		private void DefaultDataHandler(Hashtable payload)
		{
			EB.Debug.Log("ArenaAPI.DefaultDataHandler: call default data handler");
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

		public void Signin(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/signin");
			BlockService(request, dataHandler);
		}

		public void AdditionalSignin(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/additionalSignIn");
			BlockService(request, dataHandler);
		}

		//每日奖励
		//public void GetInfo(System.Action<Hashtable> dataHandler)
		//{
		//	var request = endPoint.Post("//getInfo");
		//	BlockService(request, dataHandler);
		//}

		public void DebugBuyMonthCard(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/debugBuyMonthCard");
			BlockService(request, dataHandler);
		}

		public void ReceiveMonthCard(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawMonthCardReward");
			BlockService(request, dataHandler);
		}

		public void ReceiveAllianceMonthCard(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawAllianceMonthCardReward");
			BlockService(request, dataHandler);
		}

		public void ReceiveShareGift(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/receiveShareGift");
			BlockService(request, dataHandler);
		}

		public void ReceiveEverydayGift(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawDailyLoginReward");
			BlockService(request, dataHandler);
		}

		public void ReceiveVigorGift(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawDailyVigor");
			BlockService(request, dataHandler);
		}

		public void ReceiveVipGift(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawVipDailyReward");
			BlockService(request, dataHandler);
		}

		//首充奖励
		public void GetFirstChargeInfo(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/GetFirstChargeInfo");
			BlockService(request, dataHandler);
		}

		public void ReceiveFirstChargeGift(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawFirstCharge");
			BlockService(request, dataHandler);
		}

		public void ReceiveSatAward(System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawFirstCharge");
			BlockService(request, dataHandler);
		}

		public void ReceiveSevendayAward(int id, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawSevenDayReward");
			request.AddData("achievementId", id);
			BlockService(request, dataHandler);
		}

		public void ReceiveWeekendAward(int id, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawWeekendReward");
			request.AddData("achievementId", id);
			BlockService(request, dataHandler);
		}

		public void ReceiveGrowPlanAward(int id, System.Action<Hashtable> dataHandler)
		{
			var request = endPoint.Post("/sign_in/drawGrowPlanReward");
			request.AddData("achievementId", id);
			BlockService(request, dataHandler);
		}
	}
}