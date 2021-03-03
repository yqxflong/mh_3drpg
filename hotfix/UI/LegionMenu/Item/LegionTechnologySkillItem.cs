using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;


 namespace Hotfix_LT.UI
{
    public class LegionTechnologySkillItem : DynamicMonoHotfix
    {
        private AllianceTechSkill SelfItemdata;
        private UILabel levelLabel,levelLabelShadow;
        private DynamicUISprite skillIcon;
        private GameObject redPoint;
        private string redpointkey;
        private int AllianceLevel;

        public override void Awake()
        {
            var t = mDMono.transform;
            t.GetComponent<ConsecutiveClickCoolTrigger>().clickEvent.Add(new EventDelegate(OnClickSkill));
            redPoint = t.GetComponent<Transform>("rp").gameObject;
            levelLabel = t.GetComponent<UILabel>("level");
            levelLabelShadow = t.GetComponent<UILabel>("level/level1");
            skillIcon = t.GetComponent<DynamicUISprite>("skilltex");

        }

        public override void OnDestroy()
        {
            if(redpointkey != null)
            {
                LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(redpointkey, SetRP);
            }
            if (SelfItemdata != null)
            {
                Messenger.RemoveListener(EventName.LegionTechSkillLevelUp, OnLevelChange);
            }
            base.OnDestroy();
        }


        public void Fill(int alliancelevel, AllianceTechSkill data,string rpkey)
        {
            if(data != null)
            {
                SelfItemdata = data;
                int curLevel = 0;
                AllianceLevel = alliancelevel;
                AlliancesManager.Instance.Account.legionTechInfo.TechlevelDic.TryGetValue(data.skillid, out curLevel);
                levelLabel.text = levelLabelShadow.text = string.Format("{0}/{1}", curLevel, alliancelevel);              
                redPoint.CustomSetActive(true);
                skillIcon.spriteName = data.skillIcon;
                redpointkey = rpkey;
                LTRedPointSystem.Instance.AddRedPointNodeCallBack(redpointkey, SetRP);
                Messenger.AddListener(EventName.LegionTechSkillLevelUp, OnLevelChange);
            }
            
        }


        public void OnAllianceLevelValueChanged(int alliancelevel)
        {
            if (AllianceLevel != alliancelevel)
            {
                AllianceLevel = alliancelevel;
                OnLevelChange();
            }
        }
        private void OnLevelChange()
        {
            if (SelfItemdata != null)
            {
                int curLevel = 0;
                AlliancesManager.Instance.Account.legionTechInfo.TechlevelDic.TryGetValue(SelfItemdata.skillid, out curLevel);
                levelLabel.text = levelLabelShadow.text = string.Format("{0}/{1}", curLevel, AllianceLevel);
            }
        }

        private void OnClickSkill()
        {
            if (SelfItemdata != null)
            {
                GlobalMenuManager.Instance.Open("LegionTechSkillUpView", SelfItemdata);
            }
        }
        private void SetRP(RedPointNode node)
        {
            redPoint.CustomSetActive(node.num > 0);
        }


    }
}
