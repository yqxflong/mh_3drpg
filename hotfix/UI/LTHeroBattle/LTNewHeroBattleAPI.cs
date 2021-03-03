using System;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTNewHeroBattleAPI : EB.Sparx.SparxAPI
    {
        public LTNewHeroBattleAPI()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        public LTNewHeroBattleAPI(EB.Sparx.EndPoint endPoint) : base(endPoint)
        {

        }

        private int BlockService(EB.Sparx.Request request, Func<EB.Sparx.Response, bool> responseFunc)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();
                if (responseFunc != null && !responseFunc(response)) //如果没有处理完成
            {
                    ProcessError(response);
                }
            });
        }

        private int Service(EB.Sparx.Request request, Func<EB.Sparx.Response, bool> responseFunc)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                if (responseFunc != null && !responseFunc(response)) //如果没有处理完成
            {
                    ProcessError(response);
                }
                else
                {
                    if (!response.sucessful)
                    {
                        ProcessError(response);
                    }
                }

            });
        }

        private void ProcessError(EB.Sparx.Response response)
        {
            if (response.fatal)
            {
                EB.Debug.LogError("LTNewHeroBattleAPI.ProcessError: error {0} occur when request {1}", response.error,
                    response.request.uri);
                ProcessError(response, CheckError(response.error.ToString()));
            }
            else
            {
                EB.Sparx.eResponseCode errCode = CheckError(response.error.ToString());
                if (errCode != EB.Sparx.eResponseCode.Success && !ProcessError(response, errCode))
                {
                    EB.Debug.LogError("LTNewHeroBattleAPI.ProcessError: request {0} failed, {1}", response.request.uri, response.error);
                }
            }
        }

        public void GetMatchBaseInfo(Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/clashofheroes/getInfo");
            BlockService(request, responseFunc);
        }

        public void StartCombat(int index, Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/clashofheroes/startCombat");
            request.AddData("index", index);
            BlockService(request, responseFunc);
        }
        
        public void StartCombat(List<int> data,int index, Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/clashofheroes/startCombatNewbie");
            request.AddData("index", index);
            request.AddData("data", data);
            BlockService(request, responseFunc);
        }
    }
}
