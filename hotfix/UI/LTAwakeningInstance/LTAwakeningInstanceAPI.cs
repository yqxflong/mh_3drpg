using EB.Sparx;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceAPI : SparxAPI
    {
        public LTAwakeningInstanceAPI()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        public LTAwakeningInstanceAPI(EB.Sparx.EndPoint endPoint) : base(endPoint)
        {
        }

        private void DefaultDataHandler(Hashtable alliance)
        {
            EB.Debug.Log("LTAwakeningInstanceAPI.DefaultDataHandler: call default data handler");
        }

        public System.Func<EB.Sparx.Response, bool> blockErrorFunc;
        private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);
            if (response.error != null)
            {
                EB.Debug.LogError(response.error);
            }

            if (blockErrorFunc != null)
            {
                if (!blockErrorFunc(response))//默认允许的错误return true;
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
                    else
                    {
                        dataHandler(null);
                    }
                }
                blockErrorFunc = null;
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

        public void StartBattle(int uid, int battleType, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awakencampaign/requestPvECombatTransition");
            request.AddData("uid", uid);
            request.AddData("id", battleType);
            BlockService(request, dataHandler);
        }

        public void RequestBlitzInstance(int uid, int battleType, int times, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awakencampaign/blitzCampaign");
            request.AddData("uid", uid);
            request.AddData("campaignId", battleType);
            request.AddData("times", times);
            BlockService(request, dataHandler);
        }

        public void RequestTrialStone(int uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awakencampaign/refreshCampaign");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void CompoundItem(string economyId, int num, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/gaminventory/compoundItem");
            request.AddData("economyId", economyId);
            request.AddData("num", num);
            BlockService(request, dataHandler);
        }
    }
}
