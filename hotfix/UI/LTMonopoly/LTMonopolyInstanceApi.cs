using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTMonopolyInstanceApi : EB.Sparx.SparxAPI
    {
        public LTMonopolyInstanceApi()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable data)
        {
            EB.Debug.Log("LTMonopolyInstanceApi.DefaultDataHandler: call default data handler");
        }

        private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            if (!string.IsNullOrEmpty(response.error))
            {
                if (response.error.Equals("Event not Running"))
                {
                    dataHandler(null);
                    return;
                }
            }
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


        public void RequestEnterMonopoly(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/monopoly/enterChapter");
            BlockService(request, dataHandler);
        }

        public void RequestFinish(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/monopoly/finish");
            BlockService(request, dataHandler);
        }

        public void RequestDice( string diceType, int index,bool isBuy, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/monopoly/rollDice");
            request.AddData("diceType", diceType);
            request.AddData("index", index);
            request.AddData("isBuy", isBuy);
            BlockService(request, dataHandler);
        }

        public void RequestMove(Hashtable pos,System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/monopoly/move");
            request.AddData("pos", pos);
            BlockService(request, dataHandler);
        }

        public void RequestFreeDice( System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/monopoly/getFreeDice");
            BlockService(request, dataHandler);
        }
    }
}