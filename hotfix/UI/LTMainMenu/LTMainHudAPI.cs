using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTMainHudAPI : EB.Sparx.SparxAPI
    {
        public LTMainHudAPI()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable alliance)
        {
            EB.Debug.Log("LTMainHudAPI.DefaultDataHandler: call default data handler");
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
                if (!blockErrorFunc(response))
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

        public int BlockService(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
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

        public void RequestStartChallenge(int level, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/infinitechallenge/startChallenge");
            request.AddData("level", level);
            BlockService(request, dataHandler);
        }

        public void RequestDoubleAward(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/specialactivity/getGhostCombatDoubleReward");
            BlockService(request, dataHandler);
        }
    }
}