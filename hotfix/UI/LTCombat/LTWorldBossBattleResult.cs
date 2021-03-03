using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTWorldBossBattleResult : DynamicMonoHotfix
    {
        public UILabel curHitLab;
        public UILabel totalHitLab;
        public UILabel partnerExpLab;
        public UIGrid grid;
        public GameObject needHideObj;
        public UILabel[] chestLabel;
        public PlayerFormationInfoItem[] items;
    
        public System.Action onShownAnimCompleted;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            curHitLab = t.GetComponent<UILabel>("Info/CurHit/CurHitValue");
            totalHitLab = t.GetComponent<UILabel>("Info/TotalHit/TotalHitValue");
            partnerExpLab = t.GetComponent<UILabel>("Info/PartnerExp/PartnerExpValue");
            grid = t.GetComponent<UIGrid>("Info/GridTeam");
            needHideObj = t.FindEx("Info").gameObject;
            chestLabel = new UILabel[2];
            chestLabel[0] = t.GetComponent<UILabel>("Info/TotalHit/Label");
            chestLabel[1] = t.GetComponent<UILabel>("Info/TotalHit/Label/Label (1)");
            var childCount = grid.transform.childCount;
            items = new PlayerFormationInfoItem[childCount];

            for (var i = 0; i < childCount; i++)
            {
                items[i] = grid.transform.GetChild(i).GetMonoILRComponent<PlayerFormationInfoItem>();
            }
        }

        public override void OnEnable()
        {
            needHideObj.CustomSetActive(false);
            OnBattleResultAniEnd();
        }

        public override void OnDisable()
        {
        }
    
        private void InitFormation()
        {
            string teamName = FormationUtil.GetCurrentTeamName(SceneLogic.BattleType);
    
            string teamDataPath = SmallPartnerPacketRule.USER_TEAM + "." + teamName + ".team_info";
            ArrayList teamDatas;
            List<PlayerFormationData> heroData = new List<PlayerFormationData>();
            DataLookupsCache.Instance.SearchDataByID(teamDataPath, out teamDatas);

            if (teamDatas != null)
            {
                for (var i = 0; i < teamDatas.Count; i++)
                {
                    var td = teamDatas[i];
                    string heroid = EB.Dot.String("hero_id", td, "");
                    if (!string.IsNullOrEmpty(heroid))
                    {
                        LTPartnerData data = LTPartnerDataManager.Instance.GetPartnerByHeroId(int.Parse(heroid));
                        if (data != null)
                        {
                            PlayerFormationData forData = new PlayerFormationData();
                            forData.templateId = data.StatId;
                            forData.star = data.Star;
                            forData.level = data.Level;
                            forData.peak = data.AllRoundLevel;
                            forData.awakeLevel = data.UpGradeId;
                            forData.charType = (int)data.HeroInfo.char_type;
                            forData.isAwaken = data.IsAwaken;
                            forData.skin = data.CurSkin;
                            heroData.Add(forData);
                        }
                    }
                }
            }
    
            for (int i = 0; i < items.Length; i++)
            {
                if (i < heroData.Count)
                {
                    items[i].ShowUI(heroData[i]);
                }
                else
                {
                    items[i].HideUI();
                }
            }
            grid.Reposition();
        }
    
        private void InitHitValue()
        {
            partnerExpLab.transform.parent.gameObject.CustomSetActive(SceneLogic.BattleType != eBattleType.AllianceCampaignBattle);
            if (SceneLogic.BattleType == eBattleType.AllianceCampaignBattle)
            {
                LTUIUtil.SetText(chestLabel, EB.Localizer.GetString("ID_GET_CHEST_TIPS"));
                int curHitValue = 0;
                DataLookupsCache.Instance.SearchDataByID("combat.totalDamage", out curHitValue);
                curHitLab.text = curHitValue.ToString();
                int totalHitValue = 0;
                DataLookupsCache.Instance.SearchDataByID("combat.boxCount", out totalHitValue);
                totalHitLab.text = "x"+ totalHitValue;
                if (totalHitValue > 0)
                {
                    FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.alliance_camp_topic,FusionTelemetry.GamePlayData.alliance_camp_event_id, FusionTelemetry.GamePlayData.alliance_camp_umengId, "reward");
                }
            }
            else
            {
                LTUIUtil.SetText(chestLabel, EB.Localizer.GetString("ID_uifont_in_LTBattleResultScreen_Label_7"));
    
                float bossMaxHp = LTWorldBossDataManager.Instance.GetMaxBossHp();

                float curHitValue = 0;
                DataLookupsCache.Instance.SearchDataByID("world_boss.thisTimeHit", out curHitValue);
                curHitLab.text = string.Format("{0}({1}%)", curHitValue.ToString("F2"), ((float)curHitValue / bossMaxHp * 100).ToString("F2"));

                float totalHitValue = 0;
                DataLookupsCache.Instance.SearchDataByID("world_boss.totalScore", out totalHitValue);
                totalHitLab.text = string.Format("{0}({1}%)", totalHitValue.ToString("F2"), ((float)totalHitValue / bossMaxHp * 100).ToString("F2"));
    
                int curPartnerExp = 0;
                int totalPartnerExp = 0;
                DataLookupsCache.Instance.SearchDataByID("world_boss.gotBuddyXp", out curPartnerExp);
                DataLookupsCache.Instance.SearchDataByID("world_boss.xpPool", out totalPartnerExp);
                partnerExpLab.text = string.Format("{0}/{1}", curPartnerExp, totalPartnerExp);
                if(SceneLogic.BattleType == eBattleType.BossBattle && totalHitValue > 0)
                {
                    FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.worldboss_topic, FusionTelemetry.GamePlayData.worldboss_event_id, FusionTelemetry.GamePlayData.worldboss_umengId, "reward");
                }
            }
        }
    
        private void OnBattleResultAniEnd()
        {
            needHideObj.CustomSetActive(true);
            InitFormation();
            InitHitValue();
            if (onShownAnimCompleted != null)
            {
                onShownAnimCompleted();
            }
        }
    }
}
