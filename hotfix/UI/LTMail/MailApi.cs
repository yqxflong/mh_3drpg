using System;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class MailAPI : EB.Sparx.SparxAPI
    {
        public MailAPI()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        public void GetMailList(int num, Action<Hashtable> dataHandler)
        {
            if (num == 0)
                return;
            EB.Sparx.Request request = endPoint.Post("/mailbox/receiveMail");
            request.AddData("num", num);
            BlockService(request, dataHandler);
        }

        public void ReceiveGift(string mailId, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/mailbox/receiveGift");
            request.AddData("mailId", mailId);
            errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "MAX_EQUIPMENT_NUM":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailApi_1124"));
                                return true;
                            }
                    }
                    if (strObjects == "ID_ERROR_NUM_IS_TOO_BIG")
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailApi_1367"));
                        return true;
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        public void HasRead(string mailId, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/mailbox/hasRead");
            request.AddData("mailId", mailId);
            BlockService(request, dataHandler);
        }

        public void SendUserMail(long receiverId, string mailTitle, string mailText, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/mailbox/sendUserMail");
            request.AddData("receiverId", receiverId);
            request.AddData("mailTitle", mailTitle);
            request.AddData("mailText", mailText);
            ArrayList giftArr = Johny.ArrayListPool.Claim();
            IDictionary dic = new Dictionary<object, object> { { "type", "headframe" }, { "data", "8061" }, { "quantity", 1 } };
            IDictionary dic1 = new Dictionary<object, object> { { "type", "headframe" }, { "data", "8051" }, { "quantity", 1 } };
            IDictionary dic2 = new Dictionary<object, object> { { "type", "headframe" }, { "data", "8021" }, { "quantity", 1 } };
            IDictionary dic3 = new Dictionary<object, object> { { "type", "res" }, { "data", "gold" }, { "quantity", 100 } };
            giftArr.Add(dic);
            giftArr.Add(dic1);
            giftArr.Add(dic2);
            giftArr.Add(dic3);
            request.AddData("gift", giftArr);
            BlockService(request, dataHandler);
        }

        public void SendAllianceMail(string mailTitle, string mailText, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/mailbox/sendAllianceMail");
            request.AddData("mailTitle", mailTitle);
            request.AddData("mailText", mailText);
            BlockService(request, dataHandler);
        }

        public void OneKeyReceive(Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/mailbox/receiveGiftByOneKey");
            errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "MAX_EQUIPMENT_NUM":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailApi_1124"));
                                return true;
                            }
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        public void OneKeyDelete(Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/mailbox/delAllMail");
            BlockService(request, dataHandler);
        }


        public void DeleteReceive(string mailId, Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/mailbox/delMail");
            request.AddData("mailId", mailId);
            BlockService(request, dataHandler);
        }


        private int BlockService(EB.Sparx.Request request, Action<Hashtable> dataHandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();

                ProcessMailResult(response, dataHandler);
            });
        }

        private int Service(EB.Sparx.Request request, Action<Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessMailResult(response, dataHandler);
            });
        }

        private void DefaultDataHandler(Hashtable mail)
        {
            EB.Debug.Log("MailAPI.DefaultDataHandler: call default data handler");
        }

        public System.Func<EB.Sparx.Response, bool> errorProcessFun = null;
        private void ProcessMailResult(EB.Sparx.Response response, Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new Action<Hashtable>(DefaultDataHandler);
            if (response.error != null)
            {
                EB.Debug.LogError(response.error);
            }

            if (errorProcessFun != null)
            {
                if (!errorProcessFun(response))
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
                }
                errorProcessFun = null;
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
    }
}