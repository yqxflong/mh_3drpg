using System;
using EB.Sparx;
using System.Collections;
using EB;

namespace Hotfix_LT.UI
{

//修改
public class HonorArenaAPI : EB.Sparx.SparxAPI
{
	public HonorArenaAPI()
	{
		endPoint = Hub.Instance.ApiEndPoint;
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
			ProcessResult(response, dataHandler);
			LoadingSpinner.Hide();
		});
	}

	private int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
	{
		return endPoint.Service(request, delegate (EB.Sparx.Response response)
		{
			ProcessResult(response, dataHandler);
		});
	}
	
	public void BuyChallengeTimes(Action<Hashtable> dataHandler)
	{
		var request = endPoint.Post("/honorarena/buyTicket");
		BlockService(request, dataHandler);
	}

	public void RequestHonorArenaData(Action<Hashtable> action)
	{
		
	}
	
	public void GetInfo(System.Action<Hashtable> dataHandler)
	{
		var request = endPoint.Post("/honorarena/getInfo");
		BlockService(request, dataHandler);
	}
	
	public void RefreshChallenge(System.Action<Hashtable> dataHandler)
	{
		var request = endPoint.Post("/honorarena/refreshChallenge");
		BlockService(request, dataHandler);
	}
	
	/// <summary>
	/// 防守战力
	/// </summary>
	/// <param name="Power"></param>
	/// <param name="dataHandler"></param>
	public void setBR(int Power, System.Action<Hashtable> dataHandler)
	{
		
		var request = endPoint.Post("/honorarena/setBR");
		request.AddData("br", Power);
		Service(request, dataHandler);
	}

	public void StartChallenge(int index, Action<Hashtable> dataHandler, bool fast)
	{
		 var request = endPoint.Post("/honorarena/startChallenge");
		 request.AddData("fast", fast);
		 request.AddData("index", index);
		 BlockService(request, dataHandler);
	}

	public void GetOneHourReward(Action<Hashtable> dataHandler)
	{
		var request = endPoint.Post("/honorarena/getOneHourReward");
		BlockService(request, dataHandler);
	}
}
}