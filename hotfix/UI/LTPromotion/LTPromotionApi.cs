using System.Collections;
using System;

namespace Hotfix_LT.UI {
    public class LTPromotionApi : EB.Sparx.SparxAPI {
        public LTPromotionApi() {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable data) {
            EB.Debug.Log("LTPromotionApi.DefaultDataHandler: call default data handler");
        }

        private void ProcessResult(EB.Sparx.Response response, Action<Hashtable> dataHandler) {
            dataHandler = dataHandler ?? new Action<Hashtable>(DefaultDataHandler);
            if (ProcessResponse(response)) {
                dataHandler(response.hashtable);
            } else {
                dataHandler(null);
            }
        }

        private int BlockService(EB.Sparx.Request request, Action<Hashtable> dataHandler) {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response) {
                LoadingSpinner.Hide();

                ProcessResult(response, dataHandler);
            });
        }

        private int Service(EB.Sparx.Request request, Action<Hashtable> dataHandler) {
            return endPoint.Service(request, delegate (EB.Sparx.Response response) {
                ProcessResult(response, dataHandler);
            });
        }

        /// <summary>
        /// 请求晋升
        /// </summary>
        /// <param name="nextPromotionId"></param>
        /// <param name="dataHandler"></param>
        public void RequestPromotion(int nextPromotionId, Action<Hashtable> dataHandler) {
            EB.Sparx.Request request = endPoint.Post("/promotion/starUp");
            request.AddData("id", nextPromotionId);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 请求训练
        /// </summary>
        /// <param name="trainingId"></param>
        /// <param name="dataHandler"></param>
        public void RequestTraining(int trainingId, Action<Hashtable> dataHandler) {
            EB.Sparx.Request request = endPoint.Post("/promotion/training");
            request.AddData("id", trainingId);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 请求保存训练结果
        /// </summary>
        /// <param name="isSave"></param>
        /// <param name="dataHandler"></param>
        public void RequestSave(bool isSave, Action<Hashtable> dataHandler) {
            EB.Sparx.Request request = endPoint.Post("/promotion/applyTraining");
            request.AddData("isSave", isSave);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 发送晋升功能已开启
        /// </summary>
        /// <param name="dataHandler"></param>
        public void SendPromotionFunctionIsEnabled(Action<Hashtable> dataHandler) {
            EB.Sparx.Request request = endPoint.Post("/promotion/open");
            BlockService(request, dataHandler);
        }
    }
}