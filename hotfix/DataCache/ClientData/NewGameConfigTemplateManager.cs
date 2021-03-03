using System;
using System.Collections;
using System.Collections.Generic;
using EB;
using Hotfix_LT.UI;
using UnityEngine;
namespace Hotfix_LT.Data
{
    public class GameConfigTemplate
    {
        public string Key;
        public float Value;
        public string strValue;
        //public string Comment;
    }

    public class NewGameConfigTemplateManager
    {
        private static NewGameConfigTemplateManager sInstance = null;

        public Dictionary<string, GameConfigTemplate> mGameConfigTpl = new Dictionary<string, GameConfigTemplate>();

        public static NewGameConfigTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new NewGameConfigTemplateManager(); }
        }

        public static void ClearUp()
        {
            if (sInstance != null)
            {
                //Hotfix_LT.Messenger.RemoveListenerEx<string, float>(Hotfix_LT.EventName.NewGameConfigGetValue, Instance.GetGameConfigValue);
                sInstance.mGameConfigTpl.Clear();
            }
        }

        public bool InitFromDataCache(GM.DataCache.NewGameConfig tbls)
        {
            if (tbls == null)
            {
                EB.Debug.LogError("InitFromDataCache: tbls is null");
                return false;
            }

            var conditionSet = tbls.GetArray(0);

            if (!InitGameConfig(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init gameconfig table failed");
                return false;
            }
            InitAttrPowerPer();
            // Hotfix_LT.Messenger.AddListenerEx<string, float>(Hotfix_LT.EventName.NewGameConfigGetValue, GetGameConfigValue);
            return true;
        }

        private bool InitGameConfig(GM.DataCache.ConditionNewGameConfig tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitGameConfig: gameconfig tbl is null");
                return false;
            }

            mGameConfigTpl = new Dictionary<string, GameConfigTemplate>();
            for (int i = 0; i < tbl.GameConfigLength; i++)
            {
                var tpl = ParsGameConfig(tbl.GetGameConfig(i));
                if (mGameConfigTpl.ContainsKey(tpl.Key))
                {
                    EB.Debug.LogError("InitGameConfig: {0} exits", tpl.Key);
                    mGameConfigTpl.Remove(tpl.Key);
                }
                mGameConfigTpl.Add(tpl.Key, tpl);
            }

            return true;
        }

        private GameConfigTemplate ParsGameConfig(GM.DataCache.GameConfig obj)
        {
            GameConfigTemplate tpl = new GameConfigTemplate();
            tpl.Key = obj.Key;
            tpl.Value = obj.Value;
            tpl.strValue = obj.StrValue;
            //tpl.Comment = obj.Comment;
            return tpl;
        }

        public float GetGameConfigValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return 0;
            }

            if (mGameConfigTpl.ContainsKey(key))
            {
                return mGameConfigTpl[key].Value;
            }
            return 0;
        }

        public string GetGameConfigStrValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            if (mGameConfigTpl.ContainsKey(key))
            {
                return mGameConfigTpl[key].strValue;
            }
            return string.Empty;
        }

        public Dictionary<string, int> GetEquipUpItemDic()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            var enumerator = mGameConfigTpl.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var tpl = enumerator.Current;
                if (tpl.Key.Contains("EquipUpItem_"))
                {
                    string[] key = tpl.Key.Split('_');
                    dic.Add(key[1], (int)tpl.Value.Value);
                }
            }
            return dic;
        }
        /// <summary>
        /// 根据查表id获取货币图标
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public string GetCurrencyIconById(string templateId)
        {
            Hashtable Icon = null;
            if (mGameConfigTpl.ContainsKey("CurrencyIconGet"))
            {
                Icon = EB.JSON.Parse(mGameConfigTpl["CurrencyIconGet"].strValue) as Hashtable;
            }
            else
            {
                EB.Debug.LogError("Can not Fins CurrencyIconGet");
                return string.Empty;
            }
            string iconstr = Icon[templateId.ToString()] as string;
            if (string.IsNullOrEmpty(iconstr))
            {
                EB.Debug.LogError("Icon Is Null, id:{0}", templateId);
            }
            return iconstr;
        }

        public int GetAwakenCurrencyByTypeConfig(int type)
        {
            Hashtable awakenCurrencyByType = null;
            if (mGameConfigTpl.ContainsKey("AwakenCurrencyByType"))
            {
                awakenCurrencyByType = EB.JSON.Parse(mGameConfigTpl["AwakenCurrencyByType"].strValue) as Hashtable;
            }
            else
            {
                EB.Debug.LogError("Can not Fins AwakenCurrencyByType");
                return 0;
            }
            int id = Dot.Integer(type.ToString(), awakenCurrencyByType, 0);
            return id;
        }

        //获取属性加成
        public void InitAttrPowerPer()
        {
            GameConfigTemplate template;
            if (mGameConfigTpl.TryGetValue("ZhanDouLiParam", out template))
            {
                    Hashtable table = Johny.HashtablePool.Claim();
                    table = EB.JSON.Parse(template.strValue) as Hashtable;
                    LTPartnerConfig.Atkper = float.Parse(Dot.String("PowerATK", table, "0"));
                    LTPartnerConfig.MaxHpper = float.Parse(Dot.String("PowerMaxHP", table, "0"));
                    LTPartnerConfig.Defper = float.Parse(Dot.String("PowerDEF", table, "0"));
                    LTPartnerConfig.ChainATKper = 100*float.Parse(Dot.String("PowerChainATK", table, "0"));
                    LTPartnerConfig.CritPper = 100*float.Parse(Dot.String("PowerCritP", table, "0"));
                    LTPartnerConfig.CritVper = 100*float.Parse(Dot.String("PowerCritV", table, "0"));
                    LTPartnerConfig.CritDper = 100*float.Parse(Dot.String("PowerCritDef", table, "0"));
                    LTPartnerConfig.SpExtraper = 100*float.Parse(Dot.String("PowerSpExtra", table, "0"));
                    LTPartnerConfig.SpResper = 100*float.Parse(Dot.String("PowerSpRes", table, "0"));
                    LTPartnerConfig.Speedper = float.Parse(Dot.String("PowerSpeed", table, "0"));
                    LTPartnerConfig.DmgReper = 100*float.Parse(Dot.String("PowerDamageReduce", table, "0"));
                    LTPartnerConfig.DmgAddper = 100*float.Parse(Dot.String("PowerDamageAdd", table, "0"));
                    Johny.HashtablePool.Release(table);
                return;
            }
            EB.Debug.LogError("Have not import Power param");
        }

        public void GetEnterVigor(eBattleType battleType, out int discountTime, out int discountVigor, out int norVigor)
        {
            string goodsStr = "";
            switch (battleType)
            {
                // //金币副本
                // case eBattleType.TreasuryBattle:
                //     goodsStr = GetGameConfigStrValue("TreasureCost");
                //     break;
                // //经验副本
                // case eBattleType.ExpSpringBattle:
                //     goodsStr = GetGameConfigStrValue("ExpSpringCost");
                //     break;
                //极限试炼
                case eBattleType.InfiniteChallenge:
                    goodsStr = GetGameConfigStrValue("InfiniteChallengeCost");
                    break;
                //觉醒副本
                case eBattleType.AwakeningBattle:
                    goodsStr = GetGameConfigStrValue("AwakenDungeonCost");
                    break;
            }
            string[] tempGoodsStrs = goodsStr.Split(',');
            if (tempGoodsStrs.Length == 3)
            {
                int.TryParse(tempGoodsStrs[0], out discountTime);
                int.TryParse(tempGoodsStrs[1], out discountVigor);
                int.TryParse(tempGoodsStrs[2], out norVigor);
                if (battleType == eBattleType.InfiniteChallenge )
                {
                    discountTime += VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.InfiDiscountTimes);
                }
                
                if (battleType == eBattleType.AwakeningBattle )
                {
                    discountTime += VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.AwakenCampDiscountTimes);
                }
                
            }
            else
            {
                discountTime = 0;
                discountVigor = 0;
                norVigor = 0;
            }
        }

        public int GetResourceBattleEnterVigor(ResourceInstanceType type)
        {
            switch (type)
            {
                //金币副本
                case ResourceInstanceType.Gold:
                    return (int)GetGameConfigValue("TreasureCost");
                //经验副本
                case ResourceInstanceType.Exp:
                    return (int)GetGameConfigValue("ExpSpringCost");
            }

            return 0;
        }
    }
}