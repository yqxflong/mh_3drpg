using System;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTDailyAPI : EB.Sparx.SparxAPI
    {
        public Func<string, bool> ExceptionFun;

        public LTDailyAPI()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable data)
        {
            EB.Debug.Log("LTPartnerAPI.DefaultDataHandler: call default data handler");
        }

        private void ProcessResult(EB.Sparx.Response response, Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new Action<Hashtable>(DefaultDataHandler);

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

        public void RequestVit(Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/sign_in/drawDailyVigor");
            BlockService(request, dataHandler);
        }

        public void RequestBuyVit(Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/sign_in/buyDailyVigor");
            BlockService(request, dataHandler);
        }

        public void RequestDailyData(Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/specialactivity/getEventSet");
            BlockService(request, dataHandler);
        }
    }
}