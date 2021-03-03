using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTAlienMazeInstanceApi : EB.Sparx.SparxAPI
    {
        public LTAlienMazeInstanceApi()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable data)
        {
            EB.Debug.Log("LTAlienMazeInstanceApi.DefaultDataHandler: call default data handler");
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

        public int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessResult(response, dataHandler);
            });
        }

        public int ServiceWithError(EB.Sparx.Request request, System.Action<object, Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response res)
            {
                if (res.error != null)
                {
                    dataHandler(res.error, null);
                }
                else
                {
                    ProcessResult(res, delegate (Hashtable data)
                    {
                        dataHandler(null, data);
                    });
                }
            });
        }

        public void RequestGetAlienMazeReward(int id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/getReward");
            request.AddData("level", id);
            request.AddData("type", LTInstanceConfig.AlienMazeTypeStr);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }
    }
}