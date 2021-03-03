using System;
using EB.Sparx;

namespace Hotfix_LT.UI
{
using System.Collections;

public class ArenaAPI : EB.Sparx.SparxAPI
{
	public ArenaAPI()
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
		var request = endPoint.Post("/arena/getInfo");
		BlockService(request, dataHandler);
	}

	public void StartChallenge(long uid, int rank, Action<Hashtable> dataHandler,bool fast=false)
	{
		var request = endPoint.Post("/arena/startChallenge");
		request.AddData("rank", rank);
		request.AddData("uid", uid);
		request.AddData("fast", fast);
		BlockService(request, dataHandler);
	}

	public void RefreshChallenge(System.Action<Hashtable> dataHandler)
	{
		var request = endPoint.Post("/arena/refreshChallenge");
		BlockService(request, dataHandler);
	}

	public void SaveTeam(long battleRating, System.Action<Hashtable> dataHandler)
	{
		var request = endPoint.Post("/arena/saveTeam");
		request.AddData("battleRating", battleRating);
		Service(request, dataHandler);
	}

	public void RefreshCooldown(System.Action<Hashtable> dataHandler)
	{
		var request = endPoint.Post("/arena/refreshCooldown");
		BlockService(request, dataHandler);
	}

	public void BuyChallengeTimes(System.Action<Hashtable> dataHandler)
	{
		var request = endPoint.Post("/arena/buyChallengeTimes");
		BlockService(request, dataHandler);
	}
}

}