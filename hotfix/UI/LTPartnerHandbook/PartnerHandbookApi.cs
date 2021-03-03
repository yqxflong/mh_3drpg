using System.Collections;

namespace Hotfix_LT.UI
{
    public class PartnerHandbookApi : EB.Sparx.SparxAPI
    {
        public PartnerHandbookApi()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        public void GetHandbookList(int num, System.Action<Hashtable> dataHandler)
        {
            if (num == 0)
                return;
            EB.Sparx.Request request = endPoint.Post("/buddy_mannual/getMannualInfo");
            request.AddData("num", num);
            Service(request, dataHandler);
        }

        public void TakeTheField(string buddyId, int type, int position, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/buddy_mannual/insertBuddyInMannual");
            request.AddData("heroId", buddyId);
            request.AddData("type", type);
            request.AddData("position", position);
            BlockService(request, dataHandler);
        }

        public void QuitTheField(string buddyId, int type, int position, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/buddy_mannual/removeBuddyFromMannual");
            request.AddData("heroId", buddyId);
            request.AddData("type", type);
            request.AddData("position", position);
            BlockService(request, dataHandler);
        }

        public void TransferField(string buddyId, int fromType, int toType, int fromPosition, int toPosition, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/buddy_mannual/exchangeBuddyFromMannual");
            request.AddData("heroId", buddyId);
            request.AddData("fromType", fromType);
            request.AddData("toType", toType);
            request.AddData("fromPosition", fromPosition);
            request.AddData("toPosition", toPosition);
            BlockService(request, dataHandler);
        }

        public void UnLockCardPosition(int type, int position, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/buddy_mannual/unlock");
            request.AddData("type", type);
            request.AddData("position", position);
            BlockService(request, dataHandler);
        }

        public void BreakThrough(int type, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/buddy_mannual/break");
            request.AddData("type", type);
            BlockService(request, dataHandler);
        }

        public void GetPoint(int point, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/buddy_mannual/collectPoint");
            request.AddData("point", point);
            BlockService(request, dataHandler);
        }

        public void MagicBookLevelUp(int level, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/buddy_mannual/bookBreak");
            request.AddData("level", level);
            BlockService(request, dataHandler);
        }

        private int BlockService(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();

                ProcessHandBookResult(response, dataHandler);
            });
        }

        private int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessHandBookResult(response, dataHandler);
            });
        }

        private void DefaultDataHandler(Hashtable HandBook)
        {
            EB.Debug.Log("HandBookAPI.DefaultDataHandler: call default data handler");
        }
        private void ProcessHandBookResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);
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
}