using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTBattleAnalyseItem: DynamicMonoHotfix
    {
        public class BattleAnalyseItem
        {
            public UIProgressBar AnalyseBar;
            public UILabel AnalyseNumLabel;
            public UILabel AnalyseRatioLabel;

            public BattleAnalyseItem(Transform t)
            {
                AnalyseBar = t.GetComponent<UIProgressBar>();
                AnalyseNumLabel=t.Find("TotleLabel").GetComponent <UILabel>();
                AnalyseRatioLabel = t.Find("ThumbLabel").GetComponent<UILabel>();
            }

            public void Fill(int  cur, int totle)
            {
                float value=(totle > 0) ? (float)cur / (float)totle : 0;
                AnalyseBar.value = value;
                AnalyseNumLabel.text = cur.ToString();
                AnalyseRatioLabel.text = string.Format("{0}%", (value*100).ToString("f0"));
            }

            public void Clean()
            {
                AnalyseBar.value = 0;
                AnalyseNumLabel.text = 0.ToString();
                AnalyseRatioLabel.text = string.Format("{0}%", 0);
            }
        }

        private UIBuddyShowItem FillItem;
        private GameObject EmptyItem;
        private BattleAnalyseItem A_Item, D_Item, H_Item;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            FillItem = t.Find("FillItem").GetMonoILRComponent <UIBuddyShowItem>();
            EmptyItem = t.Find("EmptyItem").gameObject;
            A_Item = new BattleAnalyseItem(t.Find("Bar/ProgressBar"));
            D_Item = new BattleAnalyseItem(t.Find("Bar/ProgressBar (1)"));
            H_Item = new BattleAnalyseItem(t.Find("Bar/ProgressBar (2)"));
        }

        public void InitData(BattleAnalyseData heroData, BattleAnalyseData TotleData)
        {
            if (heroData==null)
            {
                FillItem.mDMono.gameObject.CustomSetActive(false);
                EmptyItem.CustomSetActive(true);
                A_Item.Clean();
                D_Item.Clean();
                H_Item.Clean();
            }
            else
            {
                FillItem.ShowUI(heroData.heroID);
                EmptyItem.CustomSetActive(false);
                FillItem.mDMono.gameObject.CustomSetActive(true);
                A_Item.Fill(heroData.totalDamage, TotleData.totalDamage);
                D_Item.Fill(heroData.totalDamaged, TotleData.totalDamaged);
                H_Item.Fill(heroData.totalHealth , TotleData.totalHealth);
            }
        }

    }

    public class BattleAnalyseData
    {
        public string heroID;
        public int totalDamage;
        public int totalDamaged;
        public int totalHealth;
    }

    public class LTBattleAnalyseController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen() { return false; }

        private LTBattleAnalyseItem ItemTemplate;
        private List<LTBattleAnalyseItem> ItemList;

        private UIGrid mGrid;

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            controller.backButton = t.Find("BG/Top/CloseBtn").GetComponent<UIButton>();
            mGrid = t.Find("Content/Formation").GetComponent <UIGrid>();
        }
        
        public override IEnumerator OnAddToStack()
        {
            ItemTemplate = controller.transform.Find("Content/Item").GetMonoILRComponent<LTBattleAnalyseItem>();
            ItemTemplate.mDMono.transform.gameObject.CustomSetActive(false);

            yield return base .OnAddToStack();
            InitItems();
        }

        public void InitItems()
        {
            List<BattleAnalyseData> heroIDs = new List<BattleAnalyseData>();

            string teamName = FormationUtil.GetCurrentTeamName(SceneLogic.BattleType);
            string teamDataPath = SmallPartnerPacketRule.USER_TEAM + "." + teamName + ".team_info";
            ArrayList teamDatas;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(teamDataPath, out teamDatas);
            ArrayList metricList;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("battlerMetric", out metricList);

            BattleAnalyseData TotleData = new BattleAnalyseData();
            Dictionary<string, BattleAnalyseData> MLDic = new Dictionary<string, BattleAnalyseData>();

            if (metricList != null)
            {
                for (int i = 0; i < metricList.Count; ++i)// var ml in metricList)
                {
                    BattleAnalyseData data = new BattleAnalyseData();
                    data.heroID = EB.Dot.String("heroId", metricList[i], string.Empty);
                    data.totalDamage = EB.Dot.Integer("totalDamage", metricList[i], 0);
                    data.totalDamaged = EB.Dot.Integer("totalDamaged", metricList[i], 0);
                    data.totalHealth = EB.Dot.Integer("totalHealth", metricList[i], 0);

                    if (!string.IsNullOrEmpty(data.heroID))
                    {
                        MLDic.Add(data.heroID, data);
                    }
                }
            }


            if (teamDatas != null)
            {
                for (int i = 0; i < teamDatas.Count; ++i)
                {
                    string heroid = EB.Dot.String("hero_id", teamDatas[i], "");

                    if (!string.IsNullOrEmpty(heroid))
                    {
                        BattleAnalyseData data;

                        if (MLDic.ContainsKey(heroid))
                        {
                            data = MLDic[heroid];
                            TotleData.totalDamage += data.totalDamage;
                            TotleData.totalDamaged += data.totalDamaged;
                            TotleData.totalHealth += data.totalHealth;
                            heroIDs.Add(data);
                        }
                    }
                }
            }

            if (ItemList == null)
            {
                ItemList = new List<LTBattleAnalyseItem>();

                for (int i = 0; i < 6; ++i)
                {
                    LTBattleAnalyseItem bodyItem = InstantiateEx<LTBattleAnalyseItem>(ItemTemplate, mGrid != null ? mGrid.transform : null, i.ToString());
                    ItemList.Add(bodyItem);
                }

                if (mGrid != null)
                {
                    mGrid.Reposition();
                }
            }
            
            for (int i = 0; i < heroIDs.Count; ++i)
            {
                ItemList[i].InitData(heroIDs[i], TotleData);
            }

            for(int i = heroIDs.Count; i < ItemList.Count; ++i)
            {
                ItemList[i].InitData(null,null);
            }
        }
    }
}
