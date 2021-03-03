using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTResourceInstanceRatingGM : DynamicMonoHotfix
    {
        public eBattleType BattleType;
        public GameObject VictoryAnimationObj;
        public UILabel NameLabel;
        public UILabel DamageLabel;
        public UILabel DamageDescLabel;
        public UILabel RewardLabel;
        public UILabel PrivilegeAdditionLabel;
        public UISprite GoldSprite;
        public UISprite ExpSprite;
        public List<UIBuddyShowItem> BuddyItemList;
        public UIGrid Grid;

        public System.Action OnShownAnimCompleted;

        public override void Awake()
        {
            base.Awake();

            BattleType = eBattleType.TreasuryBattle;

            var t = mDMono.transform;
            VictoryAnimationObj = t.FindEx("Content/Title").gameObject;
            NameLabel = t.GetComponent<UILabel>("Content/Text/Name/Num");
            DamageLabel = t.GetComponent<UILabel>("Content/Text/Damage/Num");
            DamageDescLabel = t.GetComponent<UILabel>("Content/Text/Damage/Desc");
            RewardLabel = t.GetComponent<UILabel>("Content/Text/Reward/Num");
            PrivilegeAdditionLabel = t.GetComponent<UILabel>("Content/Privilege");
            GoldSprite = t.GetComponent<UISprite>("Content/Text/Reward/Gold");
            ExpSprite = t.GetComponent<UISprite>("Content/Text/Reward/Exp");

            BuddyItemList = new List<UIBuddyShowItem>();
            BuddyItemList.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Content/Heros/Heros_Grid/0_Pos"));
            BuddyItemList.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Content/Heros/Heros_Grid/0_Pos (1)"));
            BuddyItemList.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Content/Heros/Heros_Grid/0_Pos (2)"));
            BuddyItemList.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Content/Heros/Heros_Grid/0_Pos (3)"));
            BuddyItemList.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Content/Heros/Heros_Grid/0_Pos (4)"));
            BuddyItemList.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Content/Heros/Heros_Grid/0_Pos (5)"));

            Grid = t.GetComponent<UIGrid>("Content/Heros/Heros_Grid");

            for (var i = 0; i < Grid.transform.childCount; i++)
            {
                var ts = Grid.transform.GetChild(i).GetComponent<TweenScale>();

                if (ts != null)
                {
                    var buddyBaseStats = ts.transform.GetMonoILRComponent<CamapignVictoryExpDataSet>("BuddyBaseStats");
                    ts.onFinished.Add(new EventDelegate(buddyBaseStats.OnTSFinished));
                }
            }
        }

        public override void OnEnable()
        {
            RestState();
    
            VictoryAnimationObj.CustomSetActive(true);
            SetHeroInfo();
            SetTextInfo();
    
            if (OnShownAnimCompleted != null)
            {
                OnShownAnimCompleted();
            }
        }
    
        private void Reset()
        {
            RestState();
        }
    
        private void SetHeroInfo()
        {
            try
            {
                string teamName = FormationUtil.GetCurrentTeamName(SceneLogic.BattleType);
        
                string teamDataPath = SmallPartnerPacketRule.USER_TEAM + "." + teamName + ".team_info";
                ArrayList teamDatas;
                List<string> heroIDs = new List<string>();
                DataLookupsCache.Instance.SearchDataByID<ArrayList>(teamDataPath, out teamDatas);
                
                for (var i = 0; i < teamDatas.Count; i++)
                {
                    string heroid = EB.Dot.String("hero_id", teamDatas[i], "");
                    if (!string.IsNullOrEmpty(heroid))
                    {
                        heroIDs.Add(heroid);
                    }
                }
        
                for (int i = 0; i < BuddyItemList.Count; i++)
                {
                    if (i < heroIDs.Count)
                    {
                        string heroid = heroIDs[i];
                        BuddyItemList[i].mDMono.gameObject.CustomSetActive(true);
                        BuddyItemList[i].ShowUI(heroid, false);
                    }
                    else
                    {
                        BuddyItemList[i].mDMono.gameObject.CustomSetActive(false);
                    }
                }
        
                Grid.Reposition();
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
           
        }
    
        private void SetTextInfo()
        {
            ArrayList rewardData;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("combat.rewards", out rewardData);
            Hashtable rewardDataTable;
            if (rewardData != null)
            {
                rewardDataTable = rewardData[0] as Hashtable;
            }else{
                rewardDataTable = Johny.HashtablePool.Claim();
            }
    
            float percentValue = 0;
    
            if (BattleType == eBattleType.TreasuryBattle)
            {
                rewardDataTable = EB.Dot.Object("gold", rewardDataTable, null);
                int num = (int)EB.Dot.Single("quantity", rewardDataTable, 0);
                float percent = EB.Dot.Single("percent", rewardDataTable, 0);
                int damage = (int)EB.Dot.Single("damage", rewardDataTable, 0);
                int id = (int)EB.Dot.Single("currentDifficulty", rewardDataTable, 0);
                float mul = EB.Dot.Single("mul", rewardDataTable, 1);
                Hotfix_LT.Data.SpecialActivityLevelTemplate temp = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivityLevel(id);
                NameLabel.text = temp != null ? temp.name : string.Empty;
                DamageLabel.text = string.Format("{0}({1})", damage, ((int)(percent * 100f)).ToString() + "%");
                DamageDescLabel.text = EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceRatingGM_3232");
    
                string colorStr = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                float resultValue = num * mul;
                RewardLabel.text = string.Format("[{0}]+{1}", colorStr, (int)resultValue);
    
                GoldSprite.gameObject.CustomSetActive(true);
                ExpSprite.gameObject.CustomSetActive(false);
    
                percentValue = VIPTemplateManager.Instance.GetVIPPercent(VIPPrivilegeKey.TreasureRating, false);
            }
            else if (BattleType == eBattleType.ExpSpringBattle)
            {
                int num = (int)EB.Dot.Single("exp", rewardDataTable, 0);
                int killNum = (int)EB.Dot.Single("defeatMonsterNum", rewardDataTable, 0);
                int fullNum = (int)EB.Dot.Single("totalMonsterNum", rewardDataTable, 0);
                int id = (int)EB.Dot.Single("currentDifficulty", rewardDataTable, 0);
                float mul = EB.Dot.Single("mul", rewardDataTable, 1);
                Hotfix_LT.Data.SpecialActivityLevelTemplate temp = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivityLevel(id);
                NameLabel.text = temp != null ? temp.name : string.Empty;
                DamageLabel.text = string.Format("{0}/{1}", killNum, fullNum);
                DamageDescLabel.text = EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceRatingGM_4117");
    
                string colorStr = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                float resultValue = num * mul;
                RewardLabel.text = string.Format("[{0}]+{1}", colorStr, (int)resultValue);
    
                GoldSprite.gameObject.CustomSetActive(false);
                ExpSprite.gameObject.CustomSetActive(true);
    
                percentValue = VIPTemplateManager.Instance.GetVIPPercent(VIPPrivilegeKey.ExpSpringRating,false);
            }

            PrivilegeAdditionLabel.gameObject.CustomSetActive(percentValue > 0);
            PrivilegeAdditionLabel.text = string.Format("{0} +{1}%", EB.Localizer.GetString("ID_CHARGE_VIP_WORD"), percentValue * 100);
        }
    
        private void RestState()
        {
            mDMono.transform.localScale = Vector3.one;
            VictoryAnimationObj.CustomSetActive(false);
        }
    }
}
