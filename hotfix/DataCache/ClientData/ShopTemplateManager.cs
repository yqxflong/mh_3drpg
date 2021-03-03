using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Data
{
    public class ShopTemplate
    {
        public int id;
        public string shop_type;
        public string shop_balance_type;
        //public string cost;
        public string name;
        public int level_limit;
        public int refresh_time_1;
        public int refresh_time_2;
        public int refresh_time_3;
        public int refresh_time_4;
        public string refresh_balance_type;
    }

    public class BossChallengeTemplate
    {
        public int id;
        public int item_num;
        public int balance_num;
        public int weight;
        public int is_must_in;
        public int position;
        public int buy_num;
        public string item_type;
        public string item_id;
        public string item_name;
        public string balance_name;
    }

    public class ShopTemplateManager
    {

        private static ShopTemplateManager sInstance = null;
        private Dictionary<int, ShopTemplate> mShopTbl = new Dictionary<int, ShopTemplate>();
        private Dictionary<int, BossChallengeTemplate> mBossChallenge1Tbl = new Dictionary<int, BossChallengeTemplate>();
        private Dictionary<int, BossChallengeTemplate> mBossChallenge2Tbl = new Dictionary<int, BossChallengeTemplate>();
        private Dictionary<int, BossChallengeTemplate> mBossChallenge3Tbl = new Dictionary<int, BossChallengeTemplate>();

        public static ShopTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new ShopTemplateManager(); }
        }

        public static void ClearUp()
        {
            if (sInstance != null)
            {
                sInstance.mShopTbl.Clear();
            }
        }

        public bool InitFromDataCache(GM.DataCache.Shop shop)
        {
            if (shop == null)
            {
                EB.Debug.LogError("InitFromDataCache: tbls is null");
                return false;
            }

            var conditionSet = shop.GetArray(0);

            if (!InitShopTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init Shop table failed");
                return false;
            }

            if (!InitBossChallenge1Tbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init BossChallenge1 table failed");
                return false;
            }

            if (!InitBossChallenge2Tbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init BossChallenge2 table failed");
                return false;
            }

            if (!InitBossChallenge3Tbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init BossChallenge3 table failed");
                return false;
            }

            return true;
        }

        private bool InitShopTbl(GM.DataCache.ConditionShop tbl)
        {
            mShopTbl = new Dictionary<int, ShopTemplate>(tbl.ShopLength);
            for (int i = 0; i < tbl.ShopLength; ++i)
            {
                var tpl = ParseShop(tbl.GetShop(i));
                if (mShopTbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitShopTbl: {0} exists", tpl.id);
                    mShopTbl.Remove(tpl.id);
                }
                mShopTbl.Add(tpl.id, tpl);
            }

            return true;
        }

        private ShopTemplate ParseShop(GM.DataCache.ShopInfo obj)
        {
            ShopTemplate tpl = new ShopTemplate();

            tpl.id = obj.Id;
            tpl.shop_type = obj.ShopType;
            tpl.shop_balance_type = obj.ShopBalanceType;
            //tpl.cost = string.Empty;
            tpl.name = EB.Localizer.GetTableString(string.Format("ID_shops_shop_{0}_name", tpl.id), obj.Name);//obj.Name;
            tpl.level_limit = obj.LevelLimit;
            tpl.refresh_time_1 = obj.RefreshTime1;
            tpl.refresh_time_2 = obj.RefreshTime2;
            tpl.refresh_time_3 = obj.RefreshTime3;
            tpl.refresh_time_4 = obj.RefreshTime4;
            tpl.refresh_balance_type = obj.RefreshBalanceType;
            return tpl;
        }

        public ShopTemplate GetShop(int id)
        {
            ShopTemplate result = null;
            if (!mShopTbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetShop: shop not found, id = {0}", id);
            }
            return result;
        }

        public ShopTemplate GetShopByShopType(string shopType)
        {
            ShopTemplate result = null;

            var enumerator = mShopTbl.Values.GetEnumerator();
            while(enumerator.MoveNext())
            {
                ShopTemplate item = enumerator.Current;
                if (item.shop_type == shopType)
                {
                    result = item;
                    break;
                }
            }
            return result;
        }

        private bool InitBossChallenge1Tbl(GM.DataCache.ConditionShop tbl)
        {
            mBossChallenge1Tbl = new Dictionary<int, BossChallengeTemplate>(tbl.Bosschallenge1Length);

            for (int i = 0; i < tbl.Bosschallenge1Length; ++i)
            {
                var tpl = ParseBossChallenge1Tbl(tbl.GetBosschallenge1(i));

                if (mBossChallenge1Tbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitBossChallenge1Tbl: {0} exists", tpl.id);
                    mBossChallenge1Tbl.Remove(tpl.id);
                }

                mBossChallenge1Tbl.Add(tpl.id, tpl);
            }

            return true;
        }

        private BossChallengeTemplate ParseBossChallenge1Tbl(GM.DataCache.BossChallenge1ShopInfo obj)
        {
            BossChallengeTemplate tpl = new BossChallengeTemplate();
            tpl.id = obj.Id;
            tpl.item_num = obj.ItemNum;
            tpl.balance_num = obj.BalanceNum;
            tpl.weight = obj.Weight;
            tpl.is_must_in = obj.IsMustIn;
            tpl.position = obj.Position;
            tpl.buy_num = obj.BuyNum;
            tpl.item_type = obj.ItemType;
            tpl.item_id = obj.ItemId;
            tpl.item_name = obj.ItemName;
            tpl.balance_name = obj.BalanceName;
            return tpl;
        }

        public BossChallengeTemplate GetBossChallenge1Template(int id)
        {
            BossChallengeTemplate result;

            if (!mBossChallenge1Tbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetBossChallenge1Template: BossChallenge1Template not found, id = {0}", id);
            }

            return result;
        }

        public Dictionary<int, BossChallengeTemplate> GetAllBossChallenge1Template()
        {
            return mBossChallenge1Tbl;
        }

        private bool InitBossChallenge2Tbl(GM.DataCache.ConditionShop tbl)
        {
            mBossChallenge2Tbl = new Dictionary<int, BossChallengeTemplate>(tbl.Bosschallenge2Length);

            for (int i = 0; i < tbl.Bosschallenge2Length; ++i)
            {
                var tpl = ParseBossChallenge2Tbl(tbl.GetBosschallenge2(i));

                if (mBossChallenge2Tbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitBossChallenge2Tbl: {0} exists", tpl.id);
                    mBossChallenge2Tbl.Remove(tpl.id);
                }

                mBossChallenge2Tbl.Add(tpl.id, tpl);
            }

            return true;
        }

        private BossChallengeTemplate ParseBossChallenge2Tbl(GM.DataCache.BossChallenge2ShopInfo obj)
        {
            BossChallengeTemplate tpl = new BossChallengeTemplate();
            tpl.id = obj.Id;
            tpl.item_num = obj.ItemNum;
            tpl.balance_num = obj.BalanceNum;
            tpl.weight = obj.Weight;
            tpl.is_must_in = obj.IsMustIn;
            tpl.position = obj.Position;
            tpl.buy_num = obj.BuyNum;
            tpl.item_type = obj.ItemType;
            tpl.item_id = obj.ItemId;
            tpl.item_name = obj.ItemName;
            tpl.balance_name = obj.BalanceName;
            return tpl;
        }

        public BossChallengeTemplate GetBossChallenge2Template(int id)
        {
            BossChallengeTemplate result;

            if (!mBossChallenge2Tbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetBossChallenge2Template: BossChallenge2Template not found, id = {0}", id);
            }

            return result;
        }

        public Dictionary<int, BossChallengeTemplate> GetAllBossChallenge2Template()
        {
            return mBossChallenge2Tbl;
        }

        private bool InitBossChallenge3Tbl(GM.DataCache.ConditionShop tbl)
        {
            mBossChallenge3Tbl = new Dictionary<int, BossChallengeTemplate>(tbl.Bosschallenge3Length);

            for (int i = 0; i < tbl.Bosschallenge3Length; ++i)
            {
                var tpl = ParseBossChallenge3Tbl(tbl.GetBosschallenge3(i));

                if (mBossChallenge3Tbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitBossChallenge3Tbl: {0} exists", tpl.id);
                    mBossChallenge3Tbl.Remove(tpl.id);
                }

                mBossChallenge3Tbl.Add(tpl.id, tpl);
            }

            return true;
        }

        private BossChallengeTemplate ParseBossChallenge3Tbl(GM.DataCache.BossChallenge3ShopInfo obj)
        {
            BossChallengeTemplate tpl = new BossChallengeTemplate();
            tpl.id = obj.Id;
            tpl.item_num = obj.ItemNum;
            tpl.balance_num = obj.BalanceNum;
            tpl.weight = obj.Weight;
            tpl.is_must_in = obj.IsMustIn;
            tpl.position = obj.Position;
            tpl.buy_num = obj.BuyNum;
            tpl.item_type = obj.ItemType;
            tpl.item_id = obj.ItemId;
            tpl.item_name = obj.ItemName;
            tpl.balance_name = obj.BalanceName;
            return tpl;
        }

        public BossChallengeTemplate GetBossChallenge3Template(int id)
        {
            BossChallengeTemplate result;

            if (!mBossChallenge3Tbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetBossChallenge3Template: BossChallenge3Template not found, id = {0}", id);
            }

            return result;
        }

        public Dictionary<int, BossChallengeTemplate> GetAllBossChallenge3Template()
        {
            return mBossChallenge3Tbl;
        }
    }
}