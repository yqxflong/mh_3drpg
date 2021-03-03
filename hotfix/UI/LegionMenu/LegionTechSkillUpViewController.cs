using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LegionTechSkillUpViewController : UIControllerHotfix
    {

        private AllianceTechSkill SelfItemdata;
        private int curSkillLevel, nextSkillLevel,Legionlevel,costMedal, totalMedal,aid;
        private ConsecutiveClickCoolTrigger breakBtn;
        private UILabel curLevel, curlevelShadow, nextLevel, nextLevelShadow, skillName, skillNameShadow, medalNum, medalNumShadow;
        private string skilldex, colorstr, medalnumstr;
        private bool isCouldClick,isFristEnter;
        private GameObject medalspobj;
        private Vector3 maxlevellabelpos , normallabelpos;
        private DynamicUISprite skillSp;
        private ParticleSystemUIComponent fx;
        public override void Awake()
        {
            base.Awake();
            Transform t = controller.transform;
            curLevel = t.GetComponent<UILabel>("Content/Bg/Label");
            curlevelShadow = t.GetComponent<UILabel>("Content/Bg/Label/Label");
            nextLevel = t.GetComponent<UILabel>("Content/Bg (1)/Label");
            nextLevelShadow = t.GetComponent<UILabel>("Content/Bg (1)/Label/Label");
            skillName = t.GetComponent<UILabel>("Content/legionskill/name");
            skillNameShadow = t.GetComponent<UILabel>("Content/legionskill/name/name");
            medalNum = t.GetComponent<UILabel>("Content/LegionMedal/Label");
            medalNumShadow = t.GetComponent<UILabel>("Content/LegionMedal/Label/Label (1)");
            medalspobj = t.GetComponent<Transform>("Content/LegionMedal/Sprite").gameObject;
            breakBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("Content/BreakBtn");
            skillSp = t.GetComponent<DynamicUISprite>("Content/legionskill/skilltex");
            fx = t.GetComponent<ParticleSystemUIComponent>("Content/Bg (1)/fx");
            breakBtn.clickEvent.Add(new EventDelegate(OnClickBreakBtn));
            normallabelpos = new Vector3(36,-93,0);
            maxlevellabelpos = new Vector3(0, -93, 0);
        }

        public override void SetMenuData(object param)
        {
            if (param != null)
            {
                SelfItemdata = param as AllianceTechSkill;
                aid = LegionModel.GetInstance().legionData.legionID;
                skillName.text = skillNameShadow.text = SelfItemdata.skillName;
                skillSp.spriteName = SelfItemdata.skillIcon;
            }
        }

        public override bool IsFullscreen()
        {
            return false;
        }

        public override bool ShowUIBlocker => true;

        public override IEnumerator OnAddToStack()
        {
            isFristEnter = true;
            UpdateUI();
            Messenger.AddListener(EventName.LegionTechSkillLevelUp, UpdateUI);
            return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            Messenger.RemoveListener(EventName.LegionTechSkillLevelUp, UpdateUI);
            return base.OnRemoveFromStack();
        }

        private void UpdateUI()
        {
            if (SelfItemdata == null)
            {
                controller.Close();
                return;
            }
            AlliancesManager.Instance.Account.legionTechInfo.TechlevelDic.TryGetValue(SelfItemdata.skillid, out curSkillLevel);
            nextSkillLevel = curSkillLevel >= SelfItemdata.maxLevel ? -1 : curSkillLevel + 1;
            Legionlevel = LegionModel.GetInstance().legionData.legionLevel;
            if (SelfItemdata.levelinfo.Count > curSkillLevel)
            {
                string skilldex;
                skilldex = string.Format(SelfItemdata.skilldesTemplate, curSkillLevel, FloatToPercent(SelfItemdata.levelinfo[curSkillLevel].addition));
                curLevel.text = curlevelShadow.text = skilldex;
                if (nextSkillLevel != -1 && SelfItemdata.levelinfo.Count > nextSkillLevel)
                {
                    skilldex = string.Format(SelfItemdata.skilldesTemplate, nextSkillLevel, FloatToPercent(SelfItemdata.levelinfo[nextSkillLevel].addition));
                    totalMedal = BalanceResourceUtil.GetUserAllianceDonate();
                    costMedal = SelfItemdata.levelinfo[curSkillLevel].cost;
                    colorstr = costMedal > totalMedal ? "[ff6699]" : "[42fe79]";
                    medalnumstr = string.Format("{0}{1}/{2}[-]", colorstr, totalMedal, costMedal);
                    medalspobj.CustomSetActive(true);
                    medalspobj.GetComponent<UIWidget>().ResetAnchors();
                    medalNum.transform.localPosition = normallabelpos;
                    LTUIUtil.SetGreyButtonEnable(breakBtn, true);
                }
                else if(nextSkillLevel == -1)
                {                
                    medalnumstr = EB.Localizer.GetString("ID_HAS_MAX_LEVEL");
                    medalNum.transform.localPosition = maxlevellabelpos;
                    medalspobj.CustomSetActive(false);
                    LTUIUtil.SetGreyButtonEnable(breakBtn, false);
                }
                medalNum.text = medalNumShadow.text = medalnumstr;
                nextLevel.text = nextLevelShadow.text = skilldex;
                if (!isFristEnter)
                {
                    //if (!fx.IsPlaying())
                    //{
                        fx.Play(true);
                    //}

                }
                isFristEnter = false;
            }
            isCouldClick = true;
        }

        private void SetParticleHide()
        {

        }

        private string FloatToPercent(float data)
        {
            return string.Format("{0}%", data * 100); 
        }

        private void OnClickBreakBtn()
        {
            if (!isCouldClick) return;
            if(curSkillLevel >= Legionlevel)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer.GetString("ID_LEGION_TECH_LEVELLIMIT"));//技能等级不能超过军团等级
                return;
            }
            if (costMedal > totalMedal)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LEGION_TECH_COSTLIMIT"));//技能升级所需奖章不足
                return;
            }
            if(curSkillLevel >= SelfItemdata.maxLevel)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LEGION_TECH_MAXLEVEL"));//技能已达到最大等级
                return;
            }
            LegionLogic.GetInstance().LegionUplevelTechLevel(aid, curSkillLevel, SelfItemdata.skillid);
            isCouldClick = false;
        
        }

    }

}