using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Utilities;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceManager : ManagerUnit
    {
        public Dictionary<int, int> dicts = new Dictionary<int, int>();
        private static LTAwakeningInstanceManager instance = null;

        public static LTAwakeningInstanceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTAwakeningInstanceManager>();
                }
                return instance;
            }
        }

        private LTAwakeningInstanceAPI Api;

        public override void Initialize(Config config)
        {
            Instance.Api = new LTAwakeningInstanceAPI();
            Instance.Api.ErrorHandler += ErrorHandler;

            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, DataRefresh);
        }

        private void DataRefresh()
        {
            if (Expedition_Awaken()) { }
        }

        public bool Expedition_Awaken()
        {
            if (FuncTemplateManager.Instance.GetFunc(10083).IsConditionOK())
            {
                bool HaveDis= LTAwakeningInstanceConfig.GetHaveDisCount();
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.awaken, HaveDis ? 1 : 0);
                return HaveDis;
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.awaken,0);
            return false;
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public Hotfix_LT.Data.eRoleAttr JudgeInstanceOpenTime(Hotfix_LT.Data.eRoleAttr type)
        {
            return Hotfix_LT.Data.eRoleAttr.None;
        }


        public bool JudgeFinish(int ID, int Stage)
        {
            bool isFinish = false;
            IDictionary temp;
            if (DataLookupsCache.Instance.SearchDataByID("userAwakenCampaign.campaigns", out temp))
            {
                if (DataLookupsCache.Instance.SearchDataByID
                    (string.Format("userAwakenCampaign.campaigns.{0}.finish", ID), out isFinish))
                    return true;
            }
            return isFinish;
        }

        public bool JudgeIsLock(int ID, int Stage)
        {
            if (Stage == 1) return false;
            bool isLock = true;
            IDictionary temp;
            if (DataLookupsCache.Instance.SearchDataByID("userAwakenCampaign.campaigns", out temp))
            {
                if (DataLookupsCache.Instance.SearchDataByID
                    (string.Format("userAwakenCampaign.campaigns.{0}.finish", ID - 1), out isLock))
                    return false;
            }
            return true;
        }

        public bool JudgeIsFirst(int ID, int Stage)
        {
            bool First = false;
            IDictionary temp;
            if (DataLookupsCache.Instance.SearchDataByID("userAwakenCampaign.campaigns", out temp))
            {
                if (DataLookupsCache.Instance.SearchDataByID
                    (string.Format("userAwakenCampaign.campaigns.{0}.finish", ID), out First))
                {
                    return false;
                }

            }
            return true;
        }

        public int GetMaxLayer(Hotfix_LT.Data.eRoleAttr eRoleAttr)
        {
            int id = (int)eRoleAttr;
            int maxLayer = 0;
            IDictionary temp;
            if (DataLookupsCache.Instance.SearchDataByID("userAwakenCampaign.campaigns", out temp))
            {
                foreach (DictionaryEntry entry in temp)
                {
                    string key = (string)entry.Key;
                    if (key.StartsWith(id.ToString()))
                    {
                        if (int.Parse(key) > maxLayer)
                        {
                            maxLayer = int.Parse(key);
                        }
                    }
                }
            }

            return Reverse(maxLayer);
        }

        private int Reverse(int x)
        {
            int pop = x % 10;
            int ans = (x / 10) % 10;
            return ans * 10 + pop;
        }

        public int BattleType
        {
            get; private set;
        }

        public void StartBattle(int uid, int battleType)
        {
            Api.StartBattle(uid, battleType, delegate (Hashtable data)
            {
                if (data != null)
                {
                    BattleType = battleType;
                    DataLookupsCache.Instance.CacheData(data);
                }
            });
        }

        public void Blitz(int uid, int battleType, int times, System.Action callback)
        {
            Api.blockErrorFunc = (response) =>
            {
                if (response.error!=null)
                {
                    string strObjects = (string)response.error;
                    switch (strObjects)
                    {
                        case "insufficient vigor":
                        {
                                BalanceResourceUtil.TurnToVigorGotView();
                                return true;
                        }
                    }
                }
                return false;
            };
            
            Api.RequestBlitzInstance(uid, battleType, times, delegate (Hashtable data)
            {
                if (data != null)
                {
                    DataLookupsCache.Instance.CacheData(data);
                    if (callback != null)
                    {
                        callback();
                    }
                }
            });
        }


        //试炼石每天回复5个  服务器不会主动推送
        public void RequestTrialStone(int uid, System.Action callback)
        {
            Api.RequestTrialStone(uid, delegate (Hashtable data)
            {
                if (data != null)
                {
                    DataLookupsCache.Instance.CacheData(data);
                }

                if (callback != null)
                {
                    callback();
                }
            });
        }

        //
        public void RequestCompoundItem(int id, int num, System.Action callback)
        {
            Api.CompoundItem(id.ToString(), num, delegate (Hashtable data)
            {
                if (data != null)
                {
                    DataLookupsCache.Instance.CacheData(data);
                }

                if (callback != null)
                {
                    callback();
                }
            });
        }
    }
}
