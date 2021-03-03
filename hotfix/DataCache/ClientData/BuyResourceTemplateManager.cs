using System.Collections.Generic;
namespace Hotfix_LT.Data
{
    public class BuyVigorTemplate
    {
        public int id;
        public int price_hc;
        public int vigor;

        public static BuyVigorTemplate Search = new BuyVigorTemplate();
        public static BuyVigorTemplateComparer Comparer = new BuyVigorTemplateComparer();
    }

    public class BuyVigorTemplateComparer : IComparer<BuyVigorTemplate>
    {
        public int Compare(BuyVigorTemplate x, BuyVigorTemplate y)
        {
            return x.id - y.id;
        }
    }

    public class BuyGoldTemplate
    {
        public int id;
        public int price_hc;
        public int base_gold;
        public int level_inc_gold;
        public int rate_2;
        public int rate_3;
        public int rate_10;

        public static BuyGoldTemplate Search = new BuyGoldTemplate();
        public static BuyGoldTemplateComparer Comparer = new BuyGoldTemplateComparer();
    }

    public class BuyGoldTemplateComparer : IComparer<BuyGoldTemplate>
    {
        public int Compare(BuyGoldTemplate x, BuyGoldTemplate y)
        {
            return x.id - y.id;
        }
    }

    public class BuyActionPowerTemplate
    {
        public int id;
        public int price_hc;
        public int base_power;
        public int rate_2;
        public int rate_3;
        public int rate_10;
    }

    public class BuyButtyExpTemplate
    {
        public int id;
        public int price_hc;
        public int base_exp;
        public int level_inc_exp;
        public int rate_2;
        public int rate_3;
        public int rate_10;
    }

    public class BuyActicketTemplate
    {
        public int id;
        public int price_hc;
        public int base_acticket;
        public int rate_2;
        public int rate_3;
        public int rate_10;
    }



    public class BuyResourceTemplateManager
    {
        private static BuyResourceTemplateManager sInstance = null;

        private BuyVigorTemplate[] mVigorTbl = null;
        public int GetVigorTblCount()
        {
            return mVigorTbl.Length;
        }
        private BuyGoldTemplate[] mGoldTbl = null;
        public int GetGoldTblCount()
        {
            return mGoldTbl.Length;
        }
        private BuyActicketTemplate[] mActicketTbl = null;
        public int GetActicketCount()
        {
            return mActicketTbl.Length;
        }
        private List<BuyActionPowerTemplate> mActionPowerTbl = null;
        public int GetPowerTblCount()
        {
            return mActionPowerTbl.Count;
        }
        private List<BuyButtyExpTemplate> mButtyExpTbl = null;
        public int GetExpTblCount()
        {
            return mButtyExpTbl.Count;
        }

        public static BuyResourceTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new BuyResourceTemplateManager(); }
        }

        private BuyResourceTemplateManager()
        {

        }

        public static void ClearUp()
        {
            if (sInstance != null)
            {
                sInstance.mVigorTbl = null;
                sInstance.mGoldTbl = null;
                sInstance.mActionPowerTbl = null;
                sInstance.mButtyExpTbl = null;
                sInstance.mActicketTbl = null;
            }
        }

        public bool InitFromDataCache(GM.DataCache.Resource tbls)
        {
            if (tbls == null)
            {
                EB.Debug.LogError("InitFromDataCache: tbls is null");
                return false;
            }

            var conditionSet = tbls.GetArray(0);
            if (!InitVigorTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init vigor table failed");
                return false;
            }

            if (!InitGoldTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init gold table failed");
                return false;
            }

            if (!InitActionPowerTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init actionPower table failed");
                return false;
            }

            if (!InitBuddyExpTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init buddyExp table failed");
                return false;
            }
            if (!InitActicketTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init acticket table failed");
                return false;
            }

            return true;
        }

        private bool InitVigorTbl(GM.DataCache.ConditionResource vigors)
        {
            if (vigors == null)
            {
                EB.Debug.LogError("InitVigorTbl: vigors is null");
                return false;
            }

            mVigorTbl = new BuyVigorTemplate[vigors.VigorLength];
            for (int i = 0; i < mVigorTbl.Length; ++i)
            {
                mVigorTbl[i] = ParseVigorTemplate(vigors.GetVigor(i));
            }

            System.Array.Sort(mVigorTbl, BuyVigorTemplate.Comparer);
            return true;
        }

        private bool InitGoldTbl(GM.DataCache.ConditionResource golds)
        {
            if (golds == null)
            {
                EB.Debug.LogError("InitGoldTbl: golds is null");
                return false;
            }

            mGoldTbl = new BuyGoldTemplate[golds.GoldLength];
            for (int i = 0; i < mGoldTbl.Length; ++i)
            {
                mGoldTbl[i] = ParseGoldTemplate(golds.GetGold(i));
            }

            System.Array.Sort(mGoldTbl, BuyGoldTemplate.Comparer);
            return true;
        }

        private bool InitActionPowerTbl(GM.DataCache.ConditionResource actionPowers)
        {
            if (actionPowers == null)
            {
                EB.Debug.LogError("InitActionPowerTbl: actionPowers is null");
                return false;
            }

            mActionPowerTbl = new List<BuyActionPowerTemplate>();
            for (int i = 0; i < actionPowers.ActionPowerLength; ++i)
            {
                mActionPowerTbl.Add(ParseActionPowerTemplate(actionPowers.GetActionPower(i)));
            }
            return true;
        }

        private bool InitBuddyExpTbl(GM.DataCache.ConditionResource buddyExps)
        {
            if (buddyExps == null)
            {
                EB.Debug.LogError("InitBuddyExpTbl: buddyExps is null");
                return false;
            }

            mButtyExpTbl = new List<BuyButtyExpTemplate>();
            for (int i = 0; i < buddyExps.BuddyExpLength; ++i)
            {
                mButtyExpTbl.Add(ParseBuddyExpTemplate(buddyExps.GetBuddyExp(i)));
            }
            return true;
        }
        private bool InitActicketTbl(GM.DataCache.ConditionResource actickets)
        {
            if (actickets == null)
            {
                EB.Debug.LogError("InitActicketTbl: actickets is null");
            }
            mActicketTbl = new BuyActicketTemplate[actickets.ActicketLength];
            for (int i = 0; i < mActicketTbl.Length; i++)
            {
                mActicketTbl[i] = ParseActicketTemplate(actickets.GetActicket(i));
            }
            return true;
        }
        private BuyVigorTemplate ParseVigorTemplate(GM.DataCache.VigorInfo obj)
        {
            BuyVigorTemplate tpl = new BuyVigorTemplate();
            tpl.id = obj.Id;
            tpl.price_hc = obj.PriceHc;
            tpl.vigor = obj.Vigor;
            return tpl;
        }

        private BuyGoldTemplate ParseGoldTemplate(GM.DataCache.GoldInfo obj)
        {
            BuyGoldTemplate tpl = new BuyGoldTemplate();
            tpl.id = obj.Id;
            tpl.price_hc = obj.PriceHc;
            tpl.base_gold = obj.BaseGold;
            tpl.level_inc_gold = obj.LevelIncGold;
            tpl.rate_2 = obj.Rate2;
            tpl.rate_3 = obj.Rate3;
            tpl.rate_10 = obj.Rate10;
            return tpl;
        }

        private BuyActionPowerTemplate ParseActionPowerTemplate(GM.DataCache.ActionPowerInfo obj)
        {
            BuyActionPowerTemplate tpl = new BuyActionPowerTemplate();
            tpl.id = obj.Id;
            tpl.price_hc = obj.PriceHc;
            tpl.base_power = obj.BaseActionPower;
            tpl.rate_2 = obj.Rate2;
            tpl.rate_3 = obj.Rate3;
            tpl.rate_10 = obj.Rate10;
            return tpl;
        }

        private BuyButtyExpTemplate ParseBuddyExpTemplate(GM.DataCache.BuddyExpInfo obj)
        {
            BuyButtyExpTemplate tpl = new BuyButtyExpTemplate();
            tpl.id = obj.Id;
            tpl.price_hc = obj.PriceHc;
            tpl.base_exp = obj.BaseBuddyExp;
            tpl.level_inc_exp = obj.LevelIncBuddyExp;
            tpl.rate_2 = obj.Rate2;
            tpl.rate_3 = obj.Rate3;
            tpl.rate_10 = obj.Rate10;
            return tpl;
        }

        public BuyActicketTemplate ParseActicketTemplate(GM.DataCache.ActicketInfo obj)
        {
            BuyActicketTemplate tpl = new BuyActicketTemplate();
            tpl.id = obj.Id;
            tpl.price_hc = obj.PriceHc;
            tpl.rate_2 = obj.Rate2;
            tpl.base_acticket = obj.Acticket;
            tpl.rate_3 = obj.Rate3;
            tpl.rate_10 = obj.Rate10;
            return tpl;
        }

        public BuyVigorTemplate GetBuyVigor(int id)
        {
            BuyVigorTemplate.Search.id = id;
            int index = System.Array.BinarySearch<BuyVigorTemplate>(mVigorTbl, BuyVigorTemplate.Search, BuyVigorTemplate.Comparer);
            if (index >= 0)
            {
                return mVigorTbl[index];
            }
            else
            {
                EB.Debug.LogWarning("GetBuyVigor: vigor info not found, id = {0}", id);
                return null;
            }
        }

        public BuyGoldTemplate GetBuyGold(int id)
        {
            BuyGoldTemplate.Search.id = id;
            int index = System.Array.BinarySearch<BuyGoldTemplate>(mGoldTbl, BuyGoldTemplate.Search, BuyGoldTemplate.Comparer);
            if (index >= 0)
            {
                return mGoldTbl[index];
            }
            else
            {
                EB.Debug.LogWarning("GetBuyGold: gold info not found, id = {0}", id);
                return null;
            }
        }

        public BuyActionPowerTemplate GetBuyPower(int id)
        {
            for (int i = 0; i < mActionPowerTbl.Count; i++)
            {
                if (mActionPowerTbl[i].id == id)
                {
                    return mActionPowerTbl[i];
                }
            }
            return null;
        }

        public BuyButtyExpTemplate GetBuyExp(int id)
        {
            for (int i = 0; i < mButtyExpTbl.Count; i++)
            {
                if (mButtyExpTbl[i].id == id)
                {
                    return mButtyExpTbl[i];
                }
            }
            return null;
        }

        public BuyActicketTemplate GetBuyActicket(int id)
        {
            for (int i = 0; i < mActicketTbl.Length; i++)
            {
                if (mActicketTbl[i].id == id)
                {
                    return mActicketTbl[i];
                }
            }
            return null;
        }

    }
}