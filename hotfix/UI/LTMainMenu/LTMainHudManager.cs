using EB.Sparx;
using Hotfix_LT.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTMainHudManager : ManagerUnit
    {
        private static LTMainHudManager instance = null;
        public static LTMainHudManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTMainHudManager>();
                }
                return instance;
            }
        }
        private LTMainHudAPI Api;
        public string UserHeadIcon
        {
            get
            {
                string icon = null;
                string Tid;
                DataLookupsCache.Instance.SearchDataByID<string>("user.leaderId", out Tid);
                Tid = (Tid == null) ? "10011" : Tid;
                int skin;
                DataLookupsCache.Instance.SearchIntByID("user.skin", out skin);
                string characterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacterid(Tid);
                var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterid, skin);
                icon = charTpl.icon;
                return icon;
            }
        }
        public int UserLeaderTID
        {
            get
            {
                int i = 0;
                string Tid;
                DataLookupsCache.Instance.SearchDataByID<string>("user.leaderId", out Tid);
                int.TryParse(Tid, out i);
                return i == 0 ? 10011 : i;
            }
        }
        public int UserLeaderSkin
        {
            get
            {
                int skin = 0;
                DataLookupsCache.Instance.SearchIntByID("user.skin", out skin);
                return skin;
            }
        }
        public string UserLeaderHeadFrameStr
        {
            get
            {
                string headFrameStr;
                DataLookupsCache.Instance.SearchDataByID<string>("user.headFrame", out headFrameStr);
                return headFrameStr;
            }
        }
        public HeadFrame UserLeaderHeadFrame
        {
            get
            {
                string headFrameStr = UserLeaderHeadFrameStr;
                if (string.IsNullOrEmpty(headFrameStr))
                {
                    return new HeadFrame();
                }
                else
                {
                    string[] split = headFrameStr.Split('_');
                    HeadFrame data = EconemyTemplateManager.Instance.GetHeadFrame(split[0], int.Parse(split[1]));
                    return data;
                }
            }
        }
        public static bool topListState;
        public static bool rightListState;
        public static bool isFirstInGame;
        public static bool isCheckLevelCompensated;//等级补偿礼包检测，每次登录后只检测一次
        /// <summary>
        /// 第一次检测 活动通知
        /// </summary>
        public static bool isFirstActivityNotice;
        private bool isFromFirstBattle;

        /// <summary>
        /// 防止新手引导穿帮设置
        /// </summary>
        /// <param name="bo"></param>
        public void SetsFromFirstBattleType(bool bo)
        {
            if (isFromFirstBattle != bo) isFromFirstBattle = bo;
        }
        /// <summary>
        /// 防止新手引导穿帮设置
        /// </summary>
        /// <param name="bo"></param>
        public bool GetFromFirstBattleType()
        {
            return isFromFirstBattle;
        }

        public override void Initialize(Config config)
        {
            Instance.Api = new LTMainHudAPI();
            Instance.Api.ErrorHandler += ErrorHandler;
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, Instance.DataRefresh);
        }

        bool isADD = false;
        public override void OnLoggedIn()
        {
            topListState = true;
            rightListState = true;
            isFirstInGame = true;
            isFirstActivityNotice = true;
            isCheckLevelCompensated = true;

            LTGameSettingManager.Instance.InitFrameData();
            UpdataItemsData();

            SetDataAction(1);//XinkuaiSDKConfig.ENTER_SERVER
            if (!isADD)
            {
                isADD = true;
                SparxHub.Instance.LevelRewardsManager.OnLevelChange += delegate (LevelRewardsStatus status)
                {
                    SetDataAction(2, status.Level);//XinkuaiSDKConfig.LEVEL_UP
                };
            }
            Messenger.AddListener<EB.IAP.Item, EB.IAP.Transaction>(EventName.OnOfferPurchaseSuceeded, SetDataAfterCharge);
        }

        public override void Disconnect(bool isLogout)
        {
            Messenger.RemoveListener<EB.IAP.Item, EB.IAP.Transaction>(EventName.OnOfferPurchaseSuceeded, SetDataAfterCharge);
        }

        private void DataRefresh()
        {
            UITaskSystemController.UpdateTaskType();
            Bag_HasNewItem();
        }
        public RoleData CreateRoleData(int code, int level)
        {
            RoleData roleData = new RoleData();
            roleData.code = code;
            roleData.roleGid = LoginManager.Instance.Account.AccountId.ToString();
            roleData.roleId = LoginManager.Instance.LocalUser.Id.ToString();
            roleData.roleName = LoginManager.Instance.LocalUser.Name;
            roleData.roleLevel = code == 2 ? level : BalanceResourceUtil.GetUserLevel();//升级需要传回调里的参数
            roleData.serverId = LoginManager.Instance.LocalUser.WorldId.ToString();
            roleData.serverName =
                LoginManager.Instance.GetDefaultGameWorld(LoginManager.Instance.LocalUser.WorldId).Name;
            roleData.coinNum = BalanceResourceUtil.GetUserDiamond();
            EB.Debug.Log(string.Format("OnReportInfo:code:{0},level{1},coinnum:{2}", code, roleData.roleLevel, roleData.coinNum));
            return roleData;
        }

        private void SetDataAction(int code, int level = 1)
        {
            EB.Sparx.Hub.Instance.mBaseSdkManager.SetRoleData(CreateRoleData(code, level));
        }

        private void SetDataAfterCharge(EB.IAP.Item item, EB.IAP.Transaction trans)
        {
            TimerManager.instance.AddTimer(3000, 1, SetChargeDataDelay);
        }

        private void SetChargeDataDelay(int seq)
        {
            EB.Sparx.Hub.Instance.mBaseSdkManager.SetRoleData(CreateRoleData(2, BalanceResourceUtil.GetUserLevel()));
        }



        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public List<Hotfix_LT.Data.FuncTemplate> OpenFuncList = new List<Hotfix_LT.Data.FuncTemplate>();

        public bool isCheckLevelUp
        {
            get
            {
                return (EB.Sparx.Hub.Instance.LevelRewardsManager.IsLevelUp);
            }
        }
        public void CheckLevelUp(System.Action callBack = null)
        {
            if (EB.Sparx.Hub.Instance.LevelRewardsManager.IsLevelUp)
            {
                GlobalMenuManager.Instance.Open("LTPlayerLevelUpUI", callBack);
                //升级时重置
                if (BalanceResourceUtil.GetUserLevel() > 7)
                {
                    PlayerVigourTip.isShow = true;
                }
            }
        }

        //极限试炼挑战按钮点击
        public bool isBattleBtnClick = false;
        public void OnBattleBtnClick(int level)
        {
            isBattleBtnClick = true;
            Api.RequestStartChallenge(level, delegate (Hashtable result)
            {
                DataLookupsCache.Instance.CacheData(result);
            });
        }

        //夺宝奇兵战斗后获取双倍奖励
        public void OnGhostDoubleAward(System.Action<bool> callback)
        {
            Api.RequestDoubleAward(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        public static void UpdateTLEventFromILR()
        {
            if (Instance != null)
            {
                Instance.UpdateTLEvent();
            }
        }

        public void UpdateTLEvent()
        {
            EB.Coroutines.Run(_DelayUpdateTLEvent());
        }

        public IEnumerator _DelayUpdateTLEvent()
        {
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(300.0f, 360.0f));

            var mng = SparxHub.Instance.GetManager<WalletManager>();
            mng.FetchPayouts(delegate (string err, Hashtable ignore)
            {
                UpdateEventsLoginData(delegate { if (LTMainMenuHudController.Instance != null) LTMainMenuHudController.Instance.RefreshEvent(); });
            });
        }

        public void UpdateEventsLoginData(System.Action callBack = null)
        {
            EB.Sparx.Request request = Api.Post("/events/getLoginData");
            Api.Service(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                if (callBack != null) callBack();
            });
        }

        public void UpdateActivityLoginData(System.Action callBack = null)
        {
            EB.Sparx.Request request = Api.Post("/specialactivity/getLoginData");
            Api.Service(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                if (callBack != null) callBack();
            });
        }

        public void SetDefeatSkipPanel()
        {
            if (!string.IsNullOrEmpty(BattleDefeatController.sDefeatSkipPanel))
            {
                GlobalMenuManager.Instance.Open(BattleDefeatController.sDefeatSkipPanel, BattleDefeatController.sDefeatSkipParam);
                BattleDefeatController.sDefeatSkipPanel = null;
                BattleDefeatController.sDefeatSkipParam = null;
            }
        }

        public HashSet<string> ItemsList = new HashSet<string>();
        public void UpdataItemsData()
        {
            //更新获取当前装备的id
            Hashtable itemsData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out itemsData);
            ItemsList.Clear();
            foreach (DictionaryEntry data in itemsData)
            {
                int id = EB.Dot.Integer("inventory_id", data.Value, 0);
                string idStr = id.ToString();
                if (!ItemsList.Contains(idStr) && idStr != "0")
                {
                    string location = EB.Dot.String("location", data.Value, "bag_items");
                    if (location == "equipment") continue;
                    string eid = EB.Dot.String("economy_id", data.Value, null);
                    if (eid != null)
                    {
                        var GeneralItem = EconemyTemplateManager.Instance.GetGeneral(eid);
                        if (GeneralItem != null && GeneralItem.CanUse)
                        {
                            ItemsList.Add(idStr);
                        }
                    }
                }
            }
        }

        public void Bag_HasNewItem()
        {
            bool hasNewItem = false;
            Hashtable itemsData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out itemsData);
            if (itemsData != null)
            {
                foreach (DictionaryEntry data in itemsData)
                {
                    int id = EB.Dot.Integer("inventory_id", data.Value, 0);
                    string idStr = id.ToString();
                    if (!ItemsList.Contains(idStr))
                    {
                        string location = EB.Dot.String("location", data.Value, "bag_items");
                        if (location == "equipment") continue;
                        string eid = EB.Dot.String("economy_id", data.Value, null);
                        if (eid != null)
                        {
                            var GeneralItem = EconemyTemplateManager.Instance.GetGeneral(eid);
                            if (GeneralItem != null && GeneralItem.CanUse)
                            {
                                hasNewItem = true;
                                break;
                            }
                        }
                    }
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.bag, hasNewItem ? 1 : 0);
        }

        public bool CheckEventRedPoint(object ActivityData)
        {
            int aid = EB.Dot.Integer("activity_id", ActivityData, 0);
            int disappear = EB.Dot.Integer("displayuntil", ActivityData, 0);
            int endTime = EB.Dot.Integer("end", ActivityData, 0);

            if (EB.Time.Now >= disappear)
            {
                return false;
            }

            string state = EB.Dot.String("state", ActivityData, "");

            if (state.Equals("pending"))
            {
                return false;
            }

            var stages = EventTemplateManager.Instance.GetTimeLimitActivityStages(aid);
            Hashtable activityData;
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + aid, out activityData);
            int score = 0;

            switch (aid)
            {
                case 6507:
                    score = BalanceResourceUtil.GetUserDiamond();//招财猫没分数，取钻石数
                    break;
                case 6518:
                    if (EB.Time.Now >= endTime) return false;
                    var equipId = EB.Dot.Integer("current", activityData, 0);
                    return equipId <= 0;
                case 6519:
                case 6520:
                case 6521:
                    int timeTemp = Mathf.Max(endTime - EB.Time.Now, 0);
                    TimeSpan ts = new TimeSpan(0, 0, timeTemp);
                    return PlayerPrefs.GetInt(LTActivityBodyItem_BossChallenge.HasEnterActivityKey, 0) != ts.Days 
						|| LTActivityBodyItem_BossChallenge.IsShowRedPointInShop()
						|| LTActivityBodyItem_BossChallenge.IsShowRedPointOnReward(aid);
                case 6701:
                    if (EB.Time.Now >= endTime) return false;
                    return LTInstanceMapModel.Instance.GetFreeDice()||LTInstanceMapModel.Instance.GetMonopolyDiceTotleCount() > 0;
                case 6801:
                    return LTActivityRacingManager.Instance.AnyGroupWithinBetDuring();
                default:
                    score = EB.Dot.Integer("current", activityData, 0);
                    break;
            }

            for (var i = 0; i < stages.Count; ++i)
            {
                var stage = stages[i];

                if (score < stage.stage)
                {
                    continue;
                }

                int rewardgot = EB.Dot.Integer(string.Format("stages.{0}", stage.id), activityData, 0);

                if (stage.realm_num > 0)
                {
                    int realmgot = EB.Dot.Integer(string.Format("realm_num.{0}", stage.id), activityData, 0);

                    if (realmgot < stage.realm_num && rewardgot <= 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (rewardgot < stage.num)
                    {
                        return true;
                    }
                }
            }
            switch (aid)//活动结束后红点立即消失，积分奖励需要存在
            {
                case 6511:
                case 6512:
                case 6513:
                    if (EB.Time.Now >= endTime)
                    {
                        return false;
                    }
                    bool isfree = false;
                    if (!DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("lottery.urbuddy.{0}.isFree", aid), out isfree))
                    {
                        isfree = true;
                    }
                    if (isfree)
                    {
                        return isfree;
                    }
                    int tenUrDrawCard = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("hcLotteryTenTimes");
                    if (BalanceResourceUtil.GetUserDiamond() >= tenUrDrawCard)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}