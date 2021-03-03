using System.Collections;
using System;
using EB.Sparx;

public class LTHotfixApi : EB.Sparx.SparxAPI
{
    static private LTHotfixApi _instance;
    static public LTHotfixApi GetInstance() {
        if (_instance == null)
        {
            _instance = new LTHotfixApi(Hub.Instance.ApiEndPoint);
        }

        return _instance; 
    }
    
    private LTHotfixApi(EndPoint endPoint) : base(endPoint)
    {
    }

    private void DefaultDataHandler(Hashtable alliance)
    {
        EB.Debug.Log("LTHotfixApi.DefaultDataHandler: call default data handler");
    }

    /// <summary>
    /// 返回false就是默认处理 返回true就是异常截断
    /// </summary>
    public System.Func<EB.Sparx.Response, bool> ExceptionFunc;
    private void ProcessResult(Response response,Action<Hashtable> dataHandler)
    {
        dataHandler = dataHandler ?? new Action<Hashtable>(DefaultDataHandler);
        if (ExceptionFunc != null)
        {
            if (!ExceptionFunc(response))
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

            ExceptionFunc = null;
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

    public int BlockService(Request request, Action<Hashtable> dataHandler)
    {
        LoadingSpinner.Show();

        return endPoint.Service(request, delegate (Response response)
        {
            LoadingSpinner.Hide();
            ProcessResult(response, dataHandler);
        });
    }

    public int Service(EB.Sparx.Request request, Action<Hashtable> dataHandler)
    {
        return endPoint.Service(request, delegate (Response response)
        {
            ProcessResult(response, dataHandler);
        });
    }
    
    public int Service(EB.Sparx.Request request, System.Action<Response> callback)
    {
        return endPoint.Service(request, delegate (Response response) { callback(response); });
    }

    public void FetchDataHandler(Hashtable data)
    {
        if (data != null)
        {
            //ToDo:
            //GameDataSparxManager.Instance.ProcessIncomingData(data, false);
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.GameDataSparxManager", "Instance", "ProcessIncomingData", data, false);

			if (data["playstate"] != null)
			{
				DataLookupsCache.Instance.CacheData(data);
			}
		}
    }

    public void MergeDataHandler(Hashtable data)
    {
        if (data != null)
        {
            //ToDo:
            //GameDataSparxManager.Instance.ProcessIncomingData(data, true);
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.GameDataSparxManager", "Instance", "ProcessIncomingData", data, true);
        }
    }
}
