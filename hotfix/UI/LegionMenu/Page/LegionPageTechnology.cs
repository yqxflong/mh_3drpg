using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{

    public class LegionPageTechnology : DynamicMonoHotfix
    {


        private UILabel chestName, chestNameShadow, GoldPerHour, TotalGoldNum, TotalGoldNumShadow,
            ExpPerHour, TotalExpNum, TotalExpNumShadow, VigorPerHour, TotalVigor, TotalVigorShadow;
        private LegionTechnologySkillItem[] Fenskills, Huoskills, Shuiskills;
        private UIGrid grid;
        private UISprite goldbar, expbar, vigorbar;
        private GameObject goldsp, exsp, vigorsp;
        //private GameObject goldfx, exfx, vigorfx;
        private int maxbarWidth;
        private AllianceTechChest techChest;
        private string formatstring;
        private List<LTShowItemData> rewardList;
        private float MaxFxScale;
        int aid;
        public override void Awake()
        {
            base.Awake();
            maxbarWidth = 415;
            Transform t = mDMono.transform;
            chestName = t.GetComponent<UILabel>("Left/name");
            chestNameShadow = t.GetComponent<UILabel>("Left/name/Label");
            GoldPerHour = t.GetComponent<UILabel>("Left/Grid/gold/Label");
            ExpPerHour = t.GetComponent<UILabel>("Left/Grid/exp/Label");
            VigorPerHour = t.GetComponent<UILabel>("Left/Grid/vigor/Label");
            TotalGoldNum = t.GetComponent<UILabel>("Left/Grid/gold/progressbar/Label");
            TotalGoldNumShadow = t.GetComponent<UILabel>("Left/Grid/gold/progressbar/Label/Label");
            goldbar = t.GetComponent<UISprite>("Left/Grid/gold/progressbar/Sprite");
            //goldfx = t.GetComponent<Transform>("Left/Grid/gold/progressbar/fx").gameObject;
            goldsp = t.GetComponent<Transform>("Left/Grid/gold/progressbar/Sprite").gameObject;
            exsp = t.GetComponent<Transform>("Left/Grid/exp/progressbar/Sprite").gameObject;
            vigorsp = t.GetComponent<Transform>("Left/Grid/vigor/progressbar/Sprite").gameObject;
            TotalExpNum = t.GetComponent<UILabel>("Left/Grid/exp/progressbar/Label");
            TotalExpNumShadow = t.GetComponent<UILabel>("Left/Grid/exp/progressbar/Label/Label");
            expbar = t.GetComponent<UISprite>("Left/Grid/exp/progressbar/Sprite");
            //exfx = t.GetComponent<Transform>("Left/Grid/exp/progressbar/fx").gameObject;
            TotalVigor = t.GetComponent<UILabel>("Left/Grid/vigor/progressbar/Label");
            TotalVigorShadow = t.GetComponent<UILabel>("Left/Grid/vigor/progressbar/Label/Label");
            vigorbar = t.GetComponent<UISprite>("Left/Grid/vigor/progressbar/Sprite");
            //vigorfx = t.GetComponent<Transform>("Left/Grid/vigor/progressbar/fx").gameObject;
            Fenskills = new LegionTechnologySkillItem[3];
            Fenskills[0] = t.GetMonoILRComponent<LegionTechnologySkillItem>("Grid/Feng/Grid/legionskill");
            Fenskills[1] = t.GetMonoILRComponent<LegionTechnologySkillItem>("Grid/Feng/Grid/legionskill (1)");
            Fenskills[2] = t.GetMonoILRComponent<LegionTechnologySkillItem>("Grid/Feng/Grid/legionskill (2)");
            Huoskills = new LegionTechnologySkillItem[3];
            Huoskills[0] = t.GetMonoILRComponent<LegionTechnologySkillItem>("Grid/Huo/Grid/legionskill");
            Huoskills[1] = t.GetMonoILRComponent<LegionTechnologySkillItem>("Grid/Huo/Grid/legionskill (1)");
            Huoskills[2] = t.GetMonoILRComponent<LegionTechnologySkillItem>("Grid/Huo/Grid/legionskill (2)");
            Shuiskills = new LegionTechnologySkillItem[3];
            Shuiskills[0] = t.GetMonoILRComponent<LegionTechnologySkillItem>("Grid/Shui/Grid/legionskill");
            Shuiskills[1] = t.GetMonoILRComponent<LegionTechnologySkillItem>("Grid/Shui/Grid/legionskill (1)");
            Shuiskills[2] = t.GetMonoILRComponent<LegionTechnologySkillItem>("Grid/Shui/Grid/legionskill (2)");

            t.GetComponent<ConsecutiveClickCoolTrigger>("Left/Rule").clickEvent.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Left/RecieveBtn").clickEvent.Add(new EventDelegate(OnClickRecieveChestBtn));
            formatstring = EB.Localizer.GetString("ID_LEGION_TECH_REWARDRATE");
            rewardList = new List<LTShowItemData> { new LTShowItemData("gold",0, LTShowItemType.TYPE_RES)
                ,new LTShowItemData("vigor",0,LTShowItemType.TYPE_RES),new LTShowItemData("buddy-exp",0,LTShowItemType.TYPE_RES) };
            InitSkillInfo();
            isCouldclick = true;
            MaxFxScale = 18;
        }
        public void SetTechnologyInfo(LegionData legionData)
        {
            SetTechChestInfo();
            aid = legionData.legionID;
            UpdataAllianceLevel(legionData.legionLevel);
            //SetTechSkillInfo();
        }

        private void SetTechChestInfo()
        {

            int chestLevel = AlliancesManager.Instance.Account.legionTechInfo.techchestLevel;
            chestName.text = chestNameShadow.text = string.Format(EB.Localizer.GetString("ID_LEGION_TECH_CHESTNAME"), chestLevel);
            techChest = AllianceTemplateManager.Instance.GetAllianceTechChestInfoByLevel(chestLevel);
            LegionTechInfo techinfo = AlliancesManager.Instance.Account.legionTechInfo;
            if (techChest != null)
            {
                Vector3 curScale = new Vector3(1, 1, 1);
                GoldPerHour.text = string.Format(formatstring, techChest.goldrate);
                ExpPerHour.text = string.Format(formatstring, techChest.exprate);
                VigorPerHour.text = string.Format(formatstring, techChest.vigorrate);
                TotalGoldNum.text = TotalGoldNumShadow.text = string.Format("{0}/{1}", techinfo.curGold, techChest.goldmax);
                TotalExpNum.text = TotalExpNumShadow.text = string.Format("{0}/{1}", techinfo.curExp, techChest.expmax);
                TotalVigor.text = TotalVigorShadow.text = string.Format("{0}/{1}", techinfo.curVigor, techChest.vigormax);
                if (techinfo.curGold == 0)
                {
                    goldsp.CustomSetActive(false);
                    //goldfx.CustomSetActive(false);
                }
                else
                {
                    goldbar.width = techinfo.curGold * maxbarWidth / techChest.goldmax;
                    //goldfx.CustomSetActive(true);
                    goldsp.CustomSetActive(true);
                    //if (goldfx.transform?.childCount > 0)
                    //{
                    //    curScale.x = techinfo.curGold * MaxFxScale / techChest.goldmax;
                    //    goldfx.transform.GetChild(0).localScale = curScale;
                    //}
                }
                if (techinfo.curExp == 0)
                {
                    exsp.CustomSetActive(false);
                    //exfx.CustomSetActive(false);
                }
                else
                {
                    expbar.width = techinfo.curExp * maxbarWidth / techChest.expmax;
                    exsp.CustomSetActive(true);
                    //exfx.CustomSetActive(true);
                    //if (exfx.transform?.childCount > 0)
                    //{
                    //    curScale.x = techinfo.curExp * MaxFxScale / techChest.expmax;
                    //    exfx.transform.GetChild(0).localScale = curScale;
                    //}
                }
                if (techinfo.curVigor == 0)
                {
                    vigorsp.CustomSetActive(false);
                    //vigorfx.CustomSetActive(false);
                }
                else
                {
                    vigorbar.width = techinfo.curVigor * maxbarWidth / techChest.vigormax;
                    //vigorfx.CustomSetActive(true);
                    vigorsp.CustomSetActive(true);
                    //if (vigorfx.transform?.childCount > 0)
                    //{
                    //    curScale.x = techinfo.curVigor * MaxFxScale / techChest.vigormax;
                    //    vigorfx.transform.GetChild(0).localScale = curScale;
                    //}

                }
                //goldbar.value = (float)techinfo.curGold / techChest.goldmax;
                //expbar.value = (float)techinfo.curExp / techChest.expmax;
                //vigorbar.value = (float)techinfo.curVigour / techChest.vigormax;
            }
        }

        //private void SetTechSkillInfo()
        //{

        //}

        private void InitSkillInfo()
        {
            string repskeystr = "Main.Legion.Technology.techskill{0}";
            int alliancelevel = 0;
            LegionData data = LegionModel.GetInstance().legionData;
            if (data != null)
            {
                alliancelevel = data.legionLevel;
            }
            int f = 0, h = 0, s = 0;
            for (int i = 0; i < AllianceTemplateManager.Instance.mTechSkillList.Count; i++)
            {
                var temp = AllianceTemplateManager.Instance.mTechSkillList[i];
                switch (temp.charType)
                {
                    case 1:
                        if (f < Fenskills.Length)
                        {
                            Fenskills[f++].Fill(alliancelevel, temp, string.Format(repskeystr, i + 1));
                        }
                        break;
                    case 2:
                        if (h < Shuiskills.Length)
                        {
                            Shuiskills[h++].Fill(alliancelevel, temp, string.Format(repskeystr, i + 1));
                        }
                        break;
                    case 3:
                        if (s < Huoskills.Length)
                        {
                            Huoskills[s++].Fill(alliancelevel, temp, string.Format(repskeystr, i + 1));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdataAllianceLevel(int allianceLevel)
        {
            for (int i = 0; i < 3; i++)
            {
                Fenskills[i].OnAllianceLevelValueChanged(allianceLevel);
                Shuiskills[i].OnAllianceLevelValueChanged(allianceLevel);
                Huoskills[i].OnAllianceLevelValueChanged(allianceLevel);
            }
        }

        public override void OnEnable()
        {
            Messenger.AddListener(EventName.LegionTechChestUpdate, SetTechChestInfo);
            Messenger.AddListener(EventName.LegionTechSkillLevelUp, SetTechChestInfo);
        }

        public override void OnDestroy()
        {
            Messenger.RemoveListener(EventName.LegionTechChestUpdate, SetTechChestInfo);
            Messenger.RemoveListener(EventName.LegionTechSkillLevelUp, SetTechChestInfo);
        }
        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.CustomSetActive(isShow);
        }

        private void OnRuleBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_LEGION_TECH_RULE"));
        }
        bool isCouldclick;
        private void OnClickRecieveChestBtn()
        {
            if (!isCouldclick) return;
            if (AlliancesManager.Instance.Account.legionTechInfo.curGold <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LEGION_TECH_NORES"));//暂无可领取资源
                return;
            }
            isCouldclick = false;
            LegionTechInfo techinfo = AlliancesManager.Instance.Account.legionTechInfo;
            for (int i = 0; i < rewardList.Count; i++)
            {
                var data = rewardList[i];
                switch (data.id)
                {
                    case "gold":
                        data.count = techinfo.curGold;
                        break;
                    case "vigor":
                        data.count = techinfo.curVigor;
                        break;
                    case "buddy-exp":
                        data.count = techinfo.curExp;
                        break;
                    default:
                        break;
                }
            }
            LegionLogic.GetInstance().OnRequestTechChest(aid, delegate
            {
                GlobalMenuManager.Instance.Open("LTShowRewardView", rewardList);
                isCouldclick = true;
            });
        }
    }

}