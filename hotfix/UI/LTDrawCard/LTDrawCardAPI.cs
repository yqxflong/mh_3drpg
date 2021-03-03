using System.Collections;
using System;

namespace Hotfix_LT.UI
{
    public class LTDrawCardAPI : EB.Sparx.SparxAPI
    {
        public LTDrawCardAPI()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        public LTDrawCardAPI(EB.Sparx.EndPoint endPoint) : base(endPoint)
        {
        }

        private void DefaultDataHandler(Hashtable alliance)
        {
            EB.Debug.Log("LTDrawCardAPI.DefaultDataHandler: call default data handler");
        }

        public System.Func<EB.Sparx.Response, bool> blockErrorFunc;
        private void ProcessResult(EB.Sparx.Response response, Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new Action<Hashtable>(DefaultDataHandler);
            if (response.error != null)
            {
                EB.Debug.LogError(response.error);
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


        public void RequestBuy(string type, string tag, int times, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/lottery/buyBox");
            request.AddData("type", type);
            request.AddData("times", times);
            request.AddData("tag", tag);
            blockErrorFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                        case "nsf":
                            {
                                if (type == "gold")
                                {
                                    MessageTemplateManager.ShowMessage(901031, null, delegate (int r)
                                    {
                                        if (r == 0)
                                        {
                                            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                            GlobalMenuManager.Instance.Open("LTResourceShopUI");
                                        }
                                    });
                                }
                                else
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_DRAWCARD_BUY_HC"), delegate (int result)
                                    {
                                        if (result == 0)
                                        {
                                            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                            GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
                                        }
                                    });
                                }
                                return true;
                            }
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        public void RequestBuyLotteryItem(int times, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/userres/buyLotteryItem");
            request.AddData("times", times);
            blockErrorFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                        case "nsf":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_DRAWCARD_BUY_HC"), delegate (int result)
                                {
                                    if (result == 0)
                                    {
                                        InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                        GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
                                    }
                                });
                                return true;
                            }
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }
        
        public void GetDrawCardTimeGift(Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/lottery/getHcTimesReward");
            BlockService(request, dataHandler);
        }

        public void RequestLotteryLoginData(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/lottery/getLoginData");
            BlockService(request, dataHandler);
        }

    }
}
