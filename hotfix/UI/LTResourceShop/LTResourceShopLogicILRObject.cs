using UnityEngine;
using System.Collections;
using EB.Sparx;
using System;
using Hotfix_LT.UI;


namespace Hotfix_LT.UI
{
    public enum ResourceShopChooseType
    {
        vigor = 0,
        gold,
        ticket,
        exp,
        Item,
        Items,
        Acticket,//����ʯ
    }

    public class LTResourceShopLogicILRObject : ManagerHotfix
    {
        public override void Initialize(Config config)
        {

        }

        public override void Dispose()
        {

        }

        public override void Connect()
        {

        }

        public override void Disconnect(bool isLogout)
        {

        }
    }

    public class LTResourceShopModel
    {
        private static LTResourceShopModel instance = null;
        public static LTResourceShopModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LTResourceShopModel();
                }
                return instance;
            }
        }

        public int BuyVigorTimes
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.vigor.current", out i);
                return i;
            }
        }
        public int BuyGoldTimes
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.gold.current", out i);
                return i;
            }
        }
        public int BuyPowerTimes
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.action-power.current", out i);
                return i;
            }
        }
        public int BuyExpTimes
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.buddy-exp.current", out i);
                return i;
            }
        }
        public int BuyActicketTimes
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.awake-ticket.current", out i);
                return i;
            }
        }

        public int GetCurrentRate(ResourceShopChooseType type)
        {
            int i = 0;//Ĭ�ϸ���Ϊ1;
            switch (type)
            {
                case ResourceShopChooseType.vigor:
                    {
                        DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.vigor.currentRate", out i);
                    }; break;
                case ResourceShopChooseType.gold:
                    {
                        DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.gold.currentRate", out i);
                    }; break;
                case ResourceShopChooseType.ticket:
                    {
                        DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.action-power.currentRate", out i);
                    }; break;
                case ResourceShopChooseType.exp:
                    {
                        DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.buddy-exp.currentRate", out i);
                    }; break;
                case ResourceShopChooseType.Acticket:
                    {
                        DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.awake-ticket.currentRate", out i);
                    }; break;
            }
            if (i == 0) i = 1;//���ʲ���Ϊ0����СΪ1����ֹ�����ݳ���Ϊ0
            return i;
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        private void InitAllPartner()
        {
        }

        public void BuyVigor(System.Action<bool> callback = null)
        {
            int CurHC = BalanceResourceUtil.GetUserDiamond();
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopAPI_2956"));
                                return true;
                            }
                        case "nsf":
                            {
                                LTHotfixGeneralFunc.ShowChargeMess();
                                return true;
                            }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/userres/buyVigor");
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable result) =>
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                if (callback != null)
                {
                    callback(result != null);
                }
                if (CurHC - BalanceResourceUtil.GetUserDiamond() > 0)
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, BalanceResourceUtil.GetUserDiamond() - CurHC, "��������");
            });
        }
        public void BuyGold(System.Action<bool> callback = null)
        {
            int CurHC = BalanceResourceUtil.GetUserDiamond();
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopAPI_2956"));
                                return true;
                            }
                        case "nsf":
                            {
                                LTHotfixGeneralFunc.ShowChargeMess();
                                return true;
                            }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/userres/buyGold");
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable result) =>
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                if (callback != null)
                {
                    callback(result != null);
                }
                if (CurHC - BalanceResourceUtil.GetUserDiamond() > 0)
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, BalanceResourceUtil.GetUserDiamond() - CurHC, "������");
            });
        }
        public void BuyActionPower(System.Action<bool> callback = null)
        {
            int CurHC = BalanceResourceUtil.GetUserDiamond();
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopAPI_2956"));
                                return true;
                            }
                        case "nsf":
                            {
                                LTHotfixGeneralFunc.ShowChargeMess();
                                return true;
                            }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/userres/buyChallCampPoint");
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable result) =>
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                if (callback != null)
                {
                    callback(result != null);
                }
                if (CurHC - BalanceResourceUtil.GetUserDiamond() > 0)
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc,  BalanceResourceUtil.GetUserDiamond() - CurHC, "������սȯ");
            });
        }
        public void BuyExp(System.Action<bool> callback = null)
        {
            int CurHC = BalanceResourceUtil.GetUserDiamond();
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopAPI_2956"));
                                return true;
                            }
                        case "nsf":
                            {
                                LTHotfixGeneralFunc.ShowChargeMess();
                                return true;
                            }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/userres/buyBuddyExp");
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable result) =>
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                if (callback != null)
                {
                    callback(result != null);
                }
                if (CurHC - BalanceResourceUtil.GetUserDiamond() > 0)
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, BalanceResourceUtil.GetUserDiamond() - CurHC, "�����龭��");
            });
        }

        public void BuyDrawCardItem(int times, System.Action<bool> callback = null)
        {
            int CurHC = BalanceResourceUtil.GetUserDiamond();
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopAPI_2956"));
                                return true;
                            }
                        case "nsf":
                            {
                                LTHotfixGeneralFunc.ShowChargeMess();
                                return true;
                            }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/userres/buyLotteryItem");
            request.AddData("times", times);
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable result) =>
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                if (callback != null)
                {
                    callback(result != null);
                }
                if (CurHC - BalanceResourceUtil.GetUserDiamond() > 0)
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, BalanceResourceUtil.GetUserDiamond() - CurHC, "����齱ȯ");
            });
        }

        //��������ʯ
        public void BuyActicket(System.Action<bool> callback = null)
        {
            int CurHC = BalanceResourceUtil.GetUserDiamond();
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopAPI_2956"));
                                return true;
                            }
                        case "nsf":
                            {
                                LTHotfixGeneralFunc.ShowChargeMess();
                                return true;
                            }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/userres/buyActicket");
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable result) =>
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                if (callback != null)
                {
                    callback(result != null);
                }
                if (CurHC - BalanceResourceUtil.GetUserDiamond() > 0)
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, BalanceResourceUtil.GetUserDiamond() - CurHC, "��������ʯ");
            });
        }


        public void ResetTimes(System.Action<bool> callback = null)
        {
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopAPI_2956"));
                                return true;
                            }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/userres/fetchLimitState");
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable result) =>
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }
    }
}
