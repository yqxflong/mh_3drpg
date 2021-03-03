using EB.Sparx;

namespace Hotfix_LT.UI
{
    public class FriendAPI : EB.Sparx.SparxAPI
    {
        public FriendAPI()
        {
            endPoint = Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(EB.Sparx.Response friend)
        {
            EB.Debug.Log("FriendAPI.DefaultDataHandler: call default data handler");
        }

        public System.Func<string, bool> ExceptionFun;
        private void ProcessFriendResult(EB.Sparx.Response response, System.Action<EB.Sparx.Response> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<EB.Sparx.Response>(DefaultDataHandler);

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
                dataHandler(response);
            }
            else
            {
                dataHandler(null);
            }
        }

        private void ProcessFriendResultEx(EB.Sparx.Response response, System.Action<EB.Sparx.Response> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<EB.Sparx.Response>(DefaultDataHandler);
            if (ProcessResponse(response))
            {
                dataHandler(response);
            }
            else
            {
                dataHandler(null);
            }
        }

        private int BlockService(EB.Sparx.Request request, System.Action<EB.Sparx.Response> dataHandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();

                ProcessFriendResult(response, dataHandler);
            });
        }

        private int Service(EB.Sparx.Request request, System.Action<EB.Sparx.Response> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessFriendResult(response, dataHandler);
            });
        }

        public void GetInfo(long uid, long lastTime, bool isReqHistory, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/getInfo");
            request.AddData("uid", uid);
            request.AddData("lastTime", lastTime);
            request.AddData("isReqHistory", isReqHistory);
            BlockService(request, dataHandler);
        }

        public void Add(long uid, string verifyInfo, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/add");
            request.AddData("uid", uid);
            request.AddData("verifyInfo", verifyInfo);
            BlockService(request, dataHandler);
        }

        public void LaunchChat(long uid, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/launchChat");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void Delete(long uid, eFriendType type, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/delete");
            request.AddData("uid", uid);
            request.AddData("type", (int)type);
            BlockService(request, dataHandler);
        }

        public void Remove(long uid, eFriendType type, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/remove");
            request.AddData("uid", uid);
            request.AddData("type", (int)type);
            BlockService(request, dataHandler);
        }

        public void Blacklist(long uid, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/blacklist");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void AddFromBlacklist(long uid, string verifyInfo, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/addFromBlacklist");
            request.AddData("uid", uid);
            request.AddData("verifyInfo", verifyInfo);
            BlockService(request, dataHandler);
        }

        public void RemoveBlacklist(long uid, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/removeBlacklist");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void Agree(long uid, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/agree");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void Reject(long uid, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/reject");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void GetApplyList(System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/getApplyList");
            BlockService(request, dataHandler);
        }

        public void GetRecommendList(System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/getRecommendFriends");
            BlockService(request, dataHandler);
        }

        public void Search(string text, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/search");
            request.AddData("text", text);
            BlockService(request, dataHandler);
        }

        public void SendVigor(long uid, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/sendVigor");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void ReceiveVigor(long uid, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/receiveVigor");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void Like(long uid, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/like");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void UnLike(long uid, System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/unlike");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void ResetVigorTime(System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/findAndRefreshSendVigorStats");
            BlockService(request, dataHandler);
        }


        public void SendAllVigor(System.Action<EB.Sparx.Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/sendVigorOneKey");
            BlockService(request, dataHandler);
        }

        public void ReceiveAllVigor(System.Action<Response> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/friend/receiveVigorOneKey");
            BlockService(request, dataHandler);
        }
    }


}