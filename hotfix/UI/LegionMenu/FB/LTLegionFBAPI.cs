using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Hotfix_LT
{
    /// <summary>
    /// 军团副本的API
    /// </summary>
    public class LTLegionFBAPI : EB.Sparx.SparxAPI
    {
        public System.Func<string, bool> ExceptionFun;

        public LTLegionFBAPI()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable data)
        {
            EB.Debug.Log("LTPartnerAPI.DefaultDataHandler: call default data handler");
        }

        private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);

            if (ExceptionFun != null && response.error != null) //处理Lostemp的异常
            {
                string error = response.error.ToString();
                if (ExceptionFun(error))
                {
                    ExceptionFun = null;
                    return;
                }
            }

            ExceptionFun = null;

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

        /// <summary>
        /// 请求BOSS挑战请求
        /// </summary>
        /// <param name="bossID">BossID</param>
        /// <param name="dataHandler">回调处理</param>
        public void RequestBossChallenge(int bossID, System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/alliancecampaign/challenge");
            request.AddData("BossID", bossID);
            BlockService(request, dataHandler);
        }
        /// <summary>
        /// 请求Boss扫荡
        /// </summary>
        /// <param name="bossID"></param>
        /// <param name="dataHandler"></param>
        public void RequestBossWipeOut(int bossID, System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/alliancecampaign/blitzAllianceCampaign");
            request.AddData("BossID", bossID);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 请求排行榜数据
        /// </summary>
        /// <param name="bossID">BossID</param>
        /// <param name="dataHandler">回调处理</param>
        public void RequestRankData(int bossID, System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Get("/alliancecampaign/rank");
            request.AddData("BossID", bossID);
            BlockService(request, dataHandler);
        }
    }
}