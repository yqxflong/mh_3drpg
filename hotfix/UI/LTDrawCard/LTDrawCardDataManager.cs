using System;
using UnityEngine;
using System.Collections;
using EB.Sparx;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTDrawCardData
    {
        public string data;
        public int quantityNum;
        public string type;
        public string action;
    }

    public class LTDrawCardDataManager : ManagerUnit
    {
        private static LTDrawCardDataManager instance = null;
        public static LTDrawCardDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTDrawCardDataManager>();
                }
                return instance;
            }
        }

        private LTDrawCardAPI Api;

        public override void Initialize(Config config)
        {
            Instance.Api = new LTDrawCardAPI();
            Instance.Api.ErrorHandler += ErrorHandler;
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, Instance.DataRefresh);
        }

        public void DataRefresh()
        {
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.drawcardgold, (LTDrawCardDataManager.Instance.HasFreeGoldTenTimes > 0) ? 1 : 0);
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst .drawcardhc,(LTDrawCardDataManager.Instance.NextFreeHcTime - EB.Time.Now < 0 || LTDrawCardDataManager.Instance.GoldenLotteryStone >= 9)?1:0);
        }
        
        public override void OnLoggedIn()
        {
            timer = new resetTimer();
            DataLookupsCache.Instance.SearchIntByID("lottery.gold.resetTime.hour", out timer.hour);
            DataLookupsCache.Instance.SearchIntByID("lottery.gold.resetTime.minute", out timer.minute);
            DataLookupsCache.Instance.SearchIntByID("lottery.gold.resetTime.second", out timer.second);

            mDrawFailCount = PlayerPrefs.GetInt(string.Format("DrawFail{0}",LoginManager.Instance.LocalUserId.Value), 0);
        }

        /// <summary>
        /// 触发抽奖连续没抽中ssr的次数
        /// </summary>
        private int mDrawFailCount;
        public int DrawFailCount
        {
            get
            {
                return mDrawFailCount;
            }
            private set
            {
                if (mDrawFailCount != value)
                {
                    mDrawFailCount = value;
                    PlayerPrefs.SetInt(string.Format("DrawFail{0}", LoginManager.Instance.LocalUserId.Value), value);
                    PlayerPrefs.Save();
                }
            }
        }

        public class resetTimer
        {
            public int hour;
            public int minute;
            public int second;
        }

        private resetTimer timer;
        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public int NextFreeGoldTime
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("lottery.gold.next", out i);
                return i;
            }
        }

        public int ResetFreeGoldTime
        {
            get
            {
                int updataTime = 0;
                DataLookupsCache.Instance.SearchIntByID("lottery.gold.next_refresh_time", out updataTime);//reset
                return updataTime;
            }
        }

        public int hasGetFreeGoldTimes
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("lottery.gold.times", out i);
                return (LTDrawCardConfig.FreeTimes - i);
            }
        }

        public int NextFreeHcTime
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("lottery.hc.next", out i);
                return i;
            }
        }

        public int HasFreeGoldTenTimes
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchDataByID<int>("lottery.gold.ten_times", out i);
                return (LTDrawCardConfig.FreeTenTimes - i);
            }
        }


        public int SliverLotteryStone
        {
            get
            {
                int i = 0;
                i = GameItemUtil.GetInventoryItemNum(LTDrawCardConfig.LOTTERY_GOLD_ID);
                return i;
            }
        }

        public int GoldenLotteryStone
        {
            get
            {
                int i = 0;
                i = GameItemUtil.GetInventoryItemNum(LTDrawCardConfig.LOTTERY_HC_ID);
                return i;
            }
        }

        public int GetPlayerGold()
        {
            int temp;
            DataLookupsCache.Instance.SearchIntByID("res.gold.v", out temp);
            return temp;
        }

        public int GetPlayerHC()
        {
            int temp;
            DataLookupsCache.Instance.SearchIntByID("res.hc.v", out temp);
            return temp;
        }

        public int TotalHcTime
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("lottery.hc.total", out i);
                return i;
            }
        }
        
        public int NextSSRTime
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchDataByID<int>("lottery.hc.next_ssr", out i);
                return i;
            }
        }
    
        public int everytimeGift
        {
            get { return (int) NewGameConfigTemplateManager.Instance.GetGameConfigValue("LottetyHcTimesAward"); }
        }
        public int Currentime
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchDataByID<int>("lottery.hc.box_ticket", out i);
                return i;
            }
        }

        private List<LTDrawCardData> DrawCardList;
        public List<LTDrawCardData> GetDrawCardData()
        {
            return DrawCardList;
        }

        public void InitAllDrawPartner(Hashtable result, bool hasGiftTrigger = false)
        {
            DrawCardList = new List<LTDrawCardData>();
            ArrayList arrayList = Hotfix_LT.EBCore.Dot.Array("lottery.result", result, null);
            bool drawTrigger = false;
            if (arrayList != null)
            {
                foreach (object Obj in arrayList)
                {
                    LTDrawCardData data = new LTDrawCardData();
                    Hashtable obj = Obj as Hashtable;
                    data.data = EB.Dot.String("data", obj, data.data);
                    data.action = EB.Dot.String("action", obj, data.action);
                    string typeStr = EB.Dot.String("type", obj, null);
                    if (typeStr == LTShowItemType.TYPE_GAMINVENTORY && Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(data.data) != null)
                    {
                        if (Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(data.data).System == "HeroShard") typeStr = LTShowItemType.TYPE_HEROSHARD;
                    }
                    if (typeStr == LTShowItemType.TYPE_HERO)
                    {
                        var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfoByStatId(int.Parse(data.data));
                        if (temp.role_grade >= 4)
                        {
                            drawTrigger = true;

                            if (LTDrawCardDataManager.Instance.isHeroShard(data.data))
                            {
                                LTChargeManager.Instance.CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType.DrawSame, BalanceResourceUtil.GetUserLevel().ToString());
                            }
                            else
                            {
                                LTChargeManager.Instance.CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType.Draw, BalanceResourceUtil.GetUserLevel().ToString());
                            }
                        }
                    }
                    data.type = typeStr;
                    data.quantityNum = EB.Dot.Integer("quantity", obj, data.quantityNum);
                    DrawCardList.Add(data);
                }
            }

            if (hasGiftTrigger)
            {
                if (drawTrigger)
                {
                    DrawFailCount = 0;
                }
                else
                {
                    int triggerCount = Mathf.Max((int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("DrawCompensation"), 50);
                    if (DrawFailCount + arrayList.Count >= triggerCount)
                    {
                        DrawFailCount = 0;
                        LTChargeManager.Instance.CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType.DrawFail, BalanceResourceUtil.GetUserLevel().ToString());
                    }
                    else
                    {
                        DrawFailCount += arrayList.Count;
                    }
                }
            }
        }

        public void GetDrawCardRequireMsg(string type, string tag, int times, System.Action<bool> callback = null)
        {
            int CurGold = BalanceResourceUtil.GetUserGold();
            Api.RequestBuy(type, tag, times, delegate (Hashtable result)
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
                InitAllDrawPartner(result, type.Equals(BalanceResourceUtil.HcName));
                if (callback != null)
                {
                    callback(result != null);
                }
                if (CurGold - BalanceResourceUtil.GetUserGold() > 0)
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, BalanceResourceUtil.GetUserGold() - CurGold, "金币抽卡");
            });
        }
        
        public void GetDrawCardTimeGift(Action<bool> callback)
        {
            Api.GetDrawCardTimeGift(delegate(Hashtable result)
            {
                DataLookupsCache.Instance.CacheData(result);
                callback(result != null);
            });
        }

        public void GetDrawCardItemRequireMsg(int times, System.Action<bool> callback = null)
        {
            int CurHC = BalanceResourceUtil.GetUserDiamond();
            Api.RequestBuyLotteryItem(times, delegate (Hashtable result)
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
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, BalanceResourceUtil.GetUserDiamond() - CurHC, "购买抽奖券");
            });

        }

        public void GetLottyLoginData(System.Action<bool> callback = null)
        {
            Api.RequestLotteryLoginData(delegate (Hashtable result)
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

        public bool isHeroShard(string data)
        {
            return (PartnerTidList.Contains(data));
        }

        private List<string> PartnerTidList = new List<string>();
        public void GetPartnerTID()
        {
            PartnerTidList = new List<string>();
            List<string> temp = new List<string>();
            Hashtable ownPartnerCollection = Johny.HashtablePool.Claim();
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(LTPartnerConfig.DATA_PATH_ROOT, out ownPartnerCollection);
            foreach (DictionaryEntry data in ownPartnerCollection)
            {
                string Tid = EB.Dot.String("template_id", data.Value, null);
                if (Tid != null) temp.Add(Tid);
            }
            PartnerTidList = temp;
        }

        /*public EB.IAP.Item GetGiftItem()
        {
            EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(id, item);
        }*/

        List<LTShowItemData> showItemsList = new List<LTShowItemData>();
        public  List<LTShowItemData> GetShowItemsList()
        {
            if (showItemsList.Count>0)
            {
                return showItemsList;
            }
            else
            {
                ArrayList aList = EB.JSON.Parse(NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("LottetyHcTimesAward")) as ArrayList;
                if (aList == null)
                {
                    return showItemsList;
                }
                for (int i = 0; i < aList.Count; i++)
                {
                    string id = EB.Dot.String("data", aList[i], string.Empty);
                    int count = EB.Dot.Integer("quantity", aList[i], 0);
                    string type = EB.Dot.String("type", aList[i], string.Empty);
                    if (!string.IsNullOrEmpty(id))
                    {
                        LTShowItemData showItemData = new LTShowItemData(id, count, type);
                        showItemsList.Add(showItemData);
                    }
                }
                return showItemsList;
            }
        }
    }
}
