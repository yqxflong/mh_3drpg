using System;


namespace Hotfix_LT.UI
{
    public class GuideNodeAPI : EB.Sparx.SparxAPI
    {
        public GuideNodeAPI()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private int BlockService(EB.Sparx.Request request, Func<EB.Sparx.Response, bool> responseFunc)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();
                if (responseFunc != null)
                {
                    if (!responseFunc(response))//如果没有处理完成
                {
                        ProcessError(response);
                    }
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

        private int Service(EB.Sparx.Request request, Func<EB.Sparx.Response, bool> responseFunc)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                if (responseFunc != null)
                {
                    if (!responseFunc(response)) //如果没有处理完成
                {
                        ProcessError(response);
                    }
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


        public void GetGuideNodeCompleted(Func<EB.Sparx.Response, bool> responseFunc)//得到完成步骤id
        {
            EB.Sparx.Request request = endPoint.Post("/users/guide_getCompleted");
            Service(request, responseFunc);
        }

        public void SaveGuideNode(string data, Func<EB.Sparx.Response, bool> responseFunc)//推送完成步骤id
        {
            EB.Sparx.Request request = endPoint.Post("/users/guide_saveCompleted");
            request.AddData("guideNodeInfo", data);
            Service(request, responseFunc);
        }

        private void ProcessError(EB.Sparx.Response response)
        {
            if (response.fatal)
            {
                EB.Debug.LogError("LTHeroBattleAPI.ProcessError: error {0} occur when request {1}", response.error,
                    response.request.uri);
                ProcessError(response, CheckError(response.error.ToString()));
            }
            else
            {
                EB.Sparx.eResponseCode errCode = CheckError(response.error.ToString());
                if (errCode != EB.Sparx.eResponseCode.Success && !ProcessError(response, errCode))
                {
                    EB.Debug.LogError("LTHeroBattleAPI.ProcessError: request {0} failed, {1}", response.request.uri, response.error);
                }
            }
        }
    }
}