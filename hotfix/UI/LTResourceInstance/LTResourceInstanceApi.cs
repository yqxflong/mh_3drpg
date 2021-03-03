using System.Collections;
using System;

namespace Hotfix_LT.UI
{
    public class LTResourceInstanceApi : EB.Sparx.SparxAPI
    {
        public LTResourceInstanceApi()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable data)
        {
            EB.Debug.Log("LTResourceInstanceApi.DefaultDataHandler: call default data handler");
        }

        private void ProcessResult(EB.Sparx.Response response, Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new Action<Hashtable>(DefaultDataHandler);
            if (ProcessResponse(response))
            {
                dataHandler(response.hashtable);
            }
            else
            {
                dataHandler(null);
            }
        }

        private int BlockService(EB.Sparx.Request request, Action<Hashtable> dataHandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();

                ProcessResult(response, dataHandler);
            });
        }

        private int Service(EB.Sparx.Request request, Action<Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessResult(response, dataHandler);
            });
        }

        public void StartBattle(int levelId, int battleType, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/specialactivity/startChallenge");
            request.AddData("type", battleType);
            request.AddData("id", levelId);
            BlockService(request, dataHandler);
        }

        public void GetResourceInstanceTime(int activityId, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/specialactivity/getSpecialActivityInfo");
            Service(request, dataHandler);
        }

        public void RequestBlitzInstance(int levelId, int battleType, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/specialactivity/blitzSpecialActivity");
            request.AddData("id", levelId);
            BlockService(request, dataHandler);
        }
    }
}