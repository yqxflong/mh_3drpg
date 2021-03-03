using System;
using System.Collections;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 爬塔副本的API
    /// </summary>
    public class LTClimingTowerAPI : EB.Sparx.SparxAPI
    {
        public System.Func<string, bool> ExceptionFun;
        public System.Func<EB.Sparx.Response, bool> errorProcessFun = null;
        public LTClimingTowerAPI()
        {
            endPoint = Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable data)
        {
            EB.Debug.Log("LTPartnerAPI.DefaultDataHandler: call default data handler");
        }

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

        private int BlockService(EB.Sparx.Request request, Action<Hashtable> dataHandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();

                ProcessResult(response, dataHandler);
            });
        }

        /// <summary>
        /// 请求挑战层
        /// </summary>
        /// <param name="layer">层数</param>
        /// <param name="dataHandler">回调处理</param>
        public void RequestChallengeLayer(int layer,int diffculty, Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/sleeptower/startChallenge");
            request.AddData("floor", layer);
            request.AddData("diffculty",diffculty);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 请求爬塔数据
        /// </summary>
        /// <param name="dataHandler">回调处理</param>
        public void RequestClimingTowerData(Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/sleeptower/getInfo");
            BlockService(request, dataHandler);
        }

        public void RequestRecordReward(int rewordIndex, Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/sleeptower/getRecordReward");
            request.AddData("rewordIndex", rewordIndex);
            BlockService(request, dataHandler);
        }
    }

}