using System.Collections;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    public class LadderAPI : EB.Sparx.SparxAPI
    {
        public LadderAPI()
        {
            endPoint = Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable payload)
        {
            EB.Debug.Log("LadderAPI.DefaultDataHandler: call default data handler");
        }

        public System.Func<EB.Sparx.Response, bool> blockErrorFunc;
        private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);
            if (response.error != null)
            {
                if (!response.error.Equals("ID_NOT_IN_ACTIVITY_TIME"))
                {
                    EB.Debug.LogError(response.error);
                }
            }

            if (blockErrorFunc != null)
            {
                if (!blockErrorFunc(response))//默认允许的错误return true;
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
                    else
                    {
                        dataHandler(null);
                    }
                }
                blockErrorFunc = null;
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

        public void GetInfo(System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/ladder/getInfo");
            BlockService(request, dataHandler);
        }

        public void StartMatch(System.Action<Hashtable> dataHandler, ArrayList heros)
        {
            blockErrorFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObject = (string)response.error;
                    switch (strObject)
                    {
                        case "ID_NOT_IN_ACTIVITY_TIME":
                            {
                                response.ShowErrorModal(() => { });
                                return true;
                            }
                    }
                }
                return false;
            };
            var request = endPoint.Post("/ladder/startMatch");
            if (heros != null) request.AddData("heroes", heros);
            BlockService(request, dataHandler);
        }

        public void CancelMatch(System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/ladder/cancelMatch");
            BlockService(request, dataHandler);
        }

        public void ReceiveEveryAward(System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/ladder/receiveEveryAward");
            BlockService(request, dataHandler);
        }

        public void GiveUp(System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/ladder/giveUp");
            BlockService(request, dataHandler);
        }

        public void Prepare(System.Action<Hashtable> dataHandler)
        {
            var request = endPoint.Post("/ladder/prepare");
            blockErrorFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "ID_DATA_UNEXPECTION":
                            {
                                //MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        //public void SaveTeam(long battleRating, System.Action<Hashtable> dataHandler)
        //{
        //	var request = endPoint.Post("/ladder/saveTeam");
        //	request.AddData("battleRating", battleRating);
        //	Service(request, dataHandler);
        //}
    }
}