using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTChargeAPI : EB.Sparx.SparxAPI
    {
        public System.Func<string, bool> ExceptionFun;

        public LTChargeAPI()
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

        private int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessResult(response, dataHandler);
            });
        }

        /// <summary>
        /// 请求领取月卡
        /// </summary>
        /// <param name="cardType"></param>
        /// <param name="dataHandler"></param>
        public void ReceiveMonthCard(string cardType, System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/sign_in/drawMonthCardReward");
            request.AddData("type", cardType);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 请求购买礼包
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="dataHandler"></param>
        public void ReceiveBuyGift(string Id, System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/lottery/getCultivateGift");
            request.AddData("id", Id);
            BlockService(request, dataHandler);
        }
        /// <summary>
        /// 请求购买免费礼包
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="dataHandler"></param>
        public void ReceiveBuyFreeGift(string type, System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/sign_in/buyFreePayments");
            request.AddData("type", type);
            BlockService(request, dataHandler);
        }

        public void RequestTriggerLimitedTimeGift(string TriggerID, int PayoutID, System.Action<Hashtable> dataHandler)
        {
            EB.Debug.Log("触发：{0};商品：{1}" ,TriggerID , PayoutID);
            var request = endPoint.Post("/usergifts/setGift");
            request.AddData("id", TriggerID);
            request.AddData("payoutId", PayoutID);
            request.AddData("overTime", 1585532955);
            BlockService(request, dataHandler);
        }
    }
}