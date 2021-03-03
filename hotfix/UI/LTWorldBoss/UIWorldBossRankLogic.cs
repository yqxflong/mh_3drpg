using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class UIWorldBossRankLogic : DataLookupHotfix
    {
        public GameObject RankObj;
        public List<LTWorldBossRankItemTemplate> ItemList;
        public UILabel SelfRank;
        public UILabel SelfDamage;
        public UILabel SelfName;
        public UILabel SelfPercent;

        public override void Awake()
        {
            base.Awake();

            var t = mDL.transform;
            RankObj = t.FindEx("Rank").gameObject;

            ItemList = new List<LTWorldBossRankItemTemplate>();
            ItemList.Add(t.GetMonoILRComponent<LTWorldBossRankItemTemplate>("Rank/Top5/Item"));
            ItemList.Add(t.GetMonoILRComponent<LTWorldBossRankItemTemplate>("Rank/Top5/Item (1)"));
            ItemList.Add(t.GetMonoILRComponent<LTWorldBossRankItemTemplate>("Rank/Top5/Item (2)"));
            ItemList.Add(t.GetMonoILRComponent<LTWorldBossRankItemTemplate>("Rank/Top5/Item (3)"));
            ItemList.Add(t.GetMonoILRComponent<LTWorldBossRankItemTemplate>("Rank/Top5/Item (4)"));

            SelfRank = t.GetComponent<UILabel>("Rank/Self/RankLabel");
            SelfDamage = t.GetComponent<UILabel>("Rank/Self/DamageLabel");
            SelfName = t.GetComponent<UILabel>("Rank/Self/NameLabel");
            SelfPercent = t.GetComponent<UILabel>("Rank/Self/PercentLabel");

            t.GetComponent<UIButton>("Rank/CancelBtn").onClick.Add(new EventDelegate(OnRankCancelBtnClick));
        }
    
        public void OnRewardBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTWorldBossRewardPreviewUI");
        }
    
        public void OnBattleReadyBtnClick()
        {
            BattleReadyHudController.Open(eBattleType.MainCampaignBattle, null);
        }
    
        public void OnRankBtnClick()
        {
            RankObj.CustomSetActive(true);
        }
    
        public void OnRankCancelBtnClick()
        {
            RankObj.CustomSetActive(false);
        }
        
        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
            if (value != null)
            {
                if(value is IDictionary){
                    RefreshData(value as IDictionary);
                }
            }
        }

        public void RefreshData(IDictionary rankdata)
        {
            if (rankdata == null)
            {
                return;
            }
    
            ArrayList ranksList = Hotfix_LT.EBCore.Dot.Array("ranks", rankdata, null);
            Hashtable rankTable = EB.Dot.Object("ranks", rankdata, null);
            List<UIWorldBossRankScrollItemData> datas = new List<UIWorldBossRankScrollItemData>();
            if (ranksList != null)
            {
                for (int i = 0; i < ranksList.Count; i++)
                {
                    int rank = EB.Dot.Integer("r", ranksList[i], 0);
                    string name = EB.Dot.String("n", ranksList[i], "");
                    int damage = EB.Dot.Integer("d", ranksList[i], 0);
                    UIWorldBossRankScrollItemData data = new UIWorldBossRankScrollItemData(rank, damage, name);
                    datas.Add(data);
                }
                datas.Sort(delegate (UIWorldBossRankScrollItemData x, UIWorldBossRankScrollItemData y)
                {
                    int result = 0;
                    result = x.rank - y.rank;
                    return result;
                });
                InitTopFive(datas);
            }
            else if (rankTable != null)//当只有一组数据时，得到的是hashtable
            {
                int rank = EB.Dot.Integer("0.r", rankTable, 0);
                string name = EB.Dot.String("0.n", rankTable, "");
                int damage = EB.Dot.Integer("0.d", rankTable, 0);
                UIWorldBossRankScrollItemData data = new UIWorldBossRankScrollItemData(rank, damage, name);
                datas.Add(data);
                InitTopFive(datas);
            }
    
            LTUIUtil.SetText(SelfRank, EB.Dot.Integer("r", rankdata, 0) > 0 ? EB.Dot.Integer("r", rankdata, 0).ToString() : EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"));
            LTUIUtil.SetText(SelfName, LoginManager.Instance.LocalUser.Name);
            LTUIUtil.SetText(SelfDamage, GetDamageStr(EB.Dot.Integer("d", rankdata, 0)));
            LTUIUtil.SetText(SelfPercent, string.Format("({0}%)", (EB.Dot.Integer("d", rankdata, 0) / (float)LTWorldBossDataManager.Instance.GetMaxBossHp() * 100).ToString("#0.00")));
        }
    
        private string GetDamageStr(int damageValue)
        {
            if (damageValue >= 100000000)
            {
                damageValue /= 100000000;
                return damageValue + EB.Localizer.GetString("ID_codefont_in_LTWorldBossRankItemTemplate_1082");
            }
    
            if (damageValue >= 10000)
            {
                damageValue /= 10000;
                return damageValue + EB.Localizer.GetString("ID_codefont_in_LTWorldBossRankItemTemplate_1215");
            }
            return damageValue.ToString();
        }
    
        private void InitTopFive(List<UIWorldBossRankScrollItemData> list)
        {
            list.Sort((a, b) => {
                if (a.damage > b.damage)
                {
                    return -1;
                }
                else if (a.damage < b.damage)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (i < list.Count)
                {
                    ItemList[i].SetData(list[i]);
                }
            }
        }
    }
}
