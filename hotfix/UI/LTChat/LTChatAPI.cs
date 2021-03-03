using EB.Sparx;

namespace Hotfix_LT.UI
{

    public class LTChatAPI : EB.Sparx.SparxAPI
    {
        public LTChatAPI()
        {
            endPoint = Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(EB.Sparx.Response friend)
        {
            EB.Debug.Log("LTChatAPI.DefaultDataHandler: call default data handler");
        }

        private void ProcessFriendResult(EB.Sparx.Response response, System.Action<EB.Sparx.Response> dataHandler)
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
    }

}