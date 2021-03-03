using System;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceConfig
    {
        //index=>1低级，2中级，3高级
        public static int GetAwakeningItemID(Hotfix_LT.Data.eRoleAttr type, int index)
        {
            int baseNum = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetAwakenCurrencyByTypeConfig((int)type);
            int itemID = baseNum + index;
            return itemID;
        }

        //获取最大恢复数量
        public static int GetMaxRecover()
        {
            int AwakenCampaingMaxTicket = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("AwakenCampaingMaxTicket");
            return AwakenCampaingMaxTicket;
        }

        //觉醒每次进入消耗门票数量
        public static int GetCost()
        {
            int AwakenCampaingCostTicket = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("AwakenCampaingCostTicket");
            return AwakenCampaingCostTicket;
        }

        //觉醒每天回复门票数量
        public static int GetRecover()
        {
            int AwakenCampaingRecoverTicket = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("AwakenCampaingRecoverTicket");
            return AwakenCampaingRecoverTicket;
        }

        public static void OpenCompound(int SourId)
        {
            var item = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(SourId.ToString());
            if ((item is Hotfix_LT.Data.GeneralItemTemplate))
            {
                GlobalMenuManager.Instance.Open("LTAwakeningGenericTrans", SourId.ToString());
            }
        }

        public static bool AwakeningIsLock(Hotfix_LT.Data.eRoleAttr type)
        {
            //判断当日是否开启
            DateTime datetime = TaskSystem.TimeSpanToDateTime(EB.Time.Now);
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            bool isLock = true;

            switch (type)
            {
                case Hotfix_LT.Data.eRoleAttr.Feng:
                    string goodsStr = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("WindOpenWeek");
                    string[] tempGoodsStrs = goodsStr.Split(',');
                    isLock = !ContainWeek(tempGoodsStrs, weeknow);
                    break;
                case Hotfix_LT.Data.eRoleAttr.Huo:
                    string goodsStrH = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("FireOpenWeek");
                    string[] tempGoodsStrsH = goodsStrH.Split(',');
                    isLock = !ContainWeek(tempGoodsStrsH, weeknow);
                    break;
                case Hotfix_LT.Data.eRoleAttr.Shui:
                    string goodsStrS = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("WaterOpenWeek");
                    string[] tempGoodsStrsS = goodsStrS.Split(',');
                    isLock = !ContainWeek(tempGoodsStrsS, weeknow);
                    break;
            }
            return isLock;
        }

        public static bool ContainWeek(string[] arrays, int day)
        {
            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i] == day.ToString())
                {
                    return true;
                }
            }
            return false;
        }
        
        public static int GetCurrencyTimes()
        {
            int times=0;
            DataLookupsCache.Instance.SearchIntByID("userAwakenCampaign.currentTimes", out times);
            return times;
        }
        
        public static int GetNeedEnterVigor()
        {
            int dayDisCountTime=0;
            int oldVigor=0;
            int NewVigor=0;
            NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.AwakeningBattle,out dayDisCountTime,out NewVigor,out oldVigor);
            int disCountTime =dayDisCountTime-LTAwakeningInstanceConfig.GetCurrencyTimes();
            int EnterVigor = disCountTime > 0 ? NewVigor : oldVigor;
            return EnterVigor;
        }
        
        public static bool GetHaveDisCount()
        {
            int dayDisCountTime=0;
            int oldVigor=0;
            int NewVigor=0;
            NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.AwakeningBattle,out dayDisCountTime,out NewVigor,out oldVigor);
            int disCountTime =dayDisCountTime-LTAwakeningInstanceConfig.GetCurrencyTimes();
            return disCountTime > 0;
        }
    }
}
