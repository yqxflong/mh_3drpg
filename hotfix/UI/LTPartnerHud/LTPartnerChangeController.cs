using System;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerChangeController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            hud = t.parent.parent.GetUIControllerILRComponent<LTPartnerHudController>();
            EquipTitle = t.GetMonoILRComponent<LTPartnerTitleStateController>("EquipTitle");
            SkillTitle = t.GetMonoILRComponent<LTPartnerTitleStateController>("SkillTitle");
            EquipContent = t.FindEx("Equip").gameObject;
            EquipContent2 = t.FindEx("UnEquipAll").gameObject;
            SkillContent = t.FindEx("Skill").gameObject;
            commonsprite = t.GetComponent<UISprite>("Skill/SkillItem0");
            passivesprite = t.GetComponent<UISprite>("Skill/SkillItem2");
            activesprite = t.GetComponent<UISprite>("Skill/SkillItem1");
            CommonSkillBreakSprite = t.GetComponent<DynamicUISprite>("Skill/SkillItem0/Icon");
            CommonSkillLevel = t.GetComponent<UILabel>("Skill/SkillItem0/Sprite/Level");
            PassiveSkillBreakSprite = t.GetComponent<DynamicUISprite>("Skill/SkillItem2/Icon");
            PassiveSkillLevel = t.GetComponent<UILabel>("Skill/SkillItem2/Sprite/Level");
            ActiveSkillBreakSprite = t.GetComponent<DynamicUISprite>("Skill/SkillItem1/Icon");
            ActiveSkillLevel = t.GetComponent<UILabel>("Skill/SkillItem1/Sprite/Level");

            t.GetComponent<UIButton>("EquipTitle").onClick.Add(new EventDelegate(OnClickEquipTitle));
            t.GetComponent<UIButton>("SkillTitle").onClick.Add(new EventDelegate(OnClickSkillTile));

            t.GetComponent<ConsecutiveClickCoolTrigger>("Skill/SkillItem0").clickEvent.Add(new EventDelegate(OnCommonSkillClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Skill/SkillItem1").clickEvent.Add(new EventDelegate(OnActiveSkillClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Skill/SkillItem2").clickEvent.Add(new EventDelegate(OnPassiveSkillClick));
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnParnerSkillChange, OnPartnerSkillChangeFunc);
            // if (m_Instance == null)
            // {
            //     m_Instance = this;
            // }
        }

        private Vector3 defaultVector3 = new Vector3(473,0,0);
        public LTPartnerHudController hud;
        public LTPartnerTitleStateController EquipTitle;
        public LTPartnerTitleStateController SkillTitle;
        public GameObject EquipContent;
        public GameObject EquipContent2;
        public GameObject SkillContent;
    
        public UISprite commonsprite ;
        public UISprite passivesprite;
        public UISprite activesprite ;
    
        public DynamicUISprite CommonSkillBreakSprite;
        public UILabel CommonSkillLevel;
        
        public DynamicUISprite PassiveSkillBreakSprite;
        public UILabel PassiveSkillLevel;
       
    
        public DynamicUISprite ActiveSkillBreakSprite;
        public UILabel ActiveSkillLevel;
    
        // private static LTPartnerChangeController m_Instance;
        // public static LTPartnerChangeController MInstance
        // {
        //     get { return m_Instance; }
        // }
    
        private LTPartnerData partnerData;
        
        private Hotfix_LT.Data.HeroAwakeInfoTemplate skillLevelUptpl = new Hotfix_LT.Data.HeroAwakeInfoTemplate();
    
    
        private void OnPartnerSkillChangeFunc()
        {
            InitSkillInfo(hud.CurSelectPartner);
        }
    
        public override void OnDestroy()
        {
            // m_Instance = null;
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnParnerSkillChange, OnPartnerSkillChangeFunc);
        }
    
        public void InitSkillInfo(LTPartnerData itemData)
        {
            if (itemData == null )
            {
                return;
            }
           
            partnerData = itemData;
            SkillSetTool.SkillFrameStateSet(commonsprite, false);
            SkillSetTool.SkillFrameStateSet(passivesprite, false);
            SkillSetTool.SkillFrameStateSet(activesprite, false);
            skillLevelUptpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(partnerData.InfoId);
            Data.SkillTemplate commonSkillTem = Data.SkillTemplateManager.Instance.GetTemplateWithAwake(partnerData,
                partnerData.HeroStat.common_skill, () => { SkillSetTool.SkillFrameStateSet(commonsprite, true); });
            Data.SkillTemplate passiveSkillTem = Data.SkillTemplateManager.Instance.GetTemplateWithAwake(partnerData,
                partnerData.HeroStat.passive_skill, () => { SkillSetTool.SkillFrameStateSet(passivesprite, true); });
            Data.SkillTemplate activeSkillTem = Data.SkillTemplateManager.Instance.GetTemplateWithAwake(partnerData,
                partnerData.HeroStat.active_skill, () => { SkillSetTool.SkillFrameStateSet(activesprite, true); });

            CommonSkillBreakSprite.spriteName = commonSkillTem.Icon;
            CommonSkillLevel.text = partnerData.CommonSkillLevel.ToString();
            PassiveSkillBreakSprite.spriteName = passiveSkillTem.Icon;
            PassiveSkillLevel.text = partnerData.PassiveSkillLevel.ToString();
            ActiveSkillBreakSprite.spriteName = activeSkillTem.Icon;
            ActiveSkillLevel.text = partnerData.ActiveSkillLevel.ToString();
        }
        
        public void OnClickEquipTitle()
        {
            EquipContent.gameObject.CustomSetActive(true);
            EquipContent2.gameObject.CustomSetActive(hud.CurSelectPartner.HeroId > 0);
            
            SkillContent.gameObject.CustomSetActive(false);
    
            hud.IsSelectSkill = false;
            //标题
            EquipTitle.SetSelect();
            SkillTitle.SetUnSelect();
        }
    
        public void OnClickSkillTile()
        {
            EquipContent.gameObject.CustomSetActive(false);
            EquipContent2.gameObject.CustomSetActive(false);
            SkillContent.gameObject.CustomSetActive(true);
            
            hud.IsSelectSkill = true;
            //标题
            EquipTitle.SetUnSelect();
            SkillTitle.SetSelect();
        }
        
        
         /// <summary>
        /// 点击第一个技能图标
        /// </summary>
        public void OnActiveSkillClick()
        {
            if (partnerData==null ||partnerData.HeroStat.active_skill <= 0)
            {
                return;
            }
            int ShowskillId = partnerData.HeroStat.active_skill;
            if (partnerData.IsAwaken > 0)
            {
                if (ShowskillId == skillLevelUptpl.beforeSkill) ShowskillId = skillLevelUptpl.laterSkill;
            }
            UITooltipManager.Instance.DisplayTooltipForPress(ShowskillId.ToString() + "," + partnerData.ActiveSkillLevel.ToString(), "Skill", "default", defaultVector3, ePressTipAnchorType.LeftDown, false, false, false, delegate () { });//-374
        }
    
        /// <summary>
        /// 点击第二个技能图标
        /// </summary>
        public void OnCommonSkillClick()
        {
            if (partnerData==null ||partnerData.HeroStat.common_skill <= 0)
            {
                return;
            }
            int ShowskillId = partnerData.HeroStat.common_skill;
            if (partnerData.IsAwaken > 0)
            {          
                if (ShowskillId == skillLevelUptpl.beforeSkill) ShowskillId = skillLevelUptpl.laterSkill;
            }
    
            UITooltipManager.Instance.DisplayTooltipForPress(ShowskillId.ToString() + "," + partnerData.CommonSkillLevel.ToString(), "Skill", "default", defaultVector3, ePressTipAnchorType.LeftDown, false, false, false, delegate () { });//-24
        }
    
        /// <summary>
        /// 点击第三个技能图标
        /// </summary>
        public void OnPassiveSkillClick()
        {
            if (partnerData==null ||partnerData.HeroStat.passive_skill <= 0)
            {
                return;
            }
            int ShowskillId = partnerData.HeroStat.passive_skill;
            if (partnerData.IsAwaken > 0)
            {
                if (ShowskillId == skillLevelUptpl.beforeSkill) ShowskillId = skillLevelUptpl.laterSkill;
            }
            //18:9的分辨率技能描述ui会超出屏幕外，在这里做处理
            int width = Screen.width;
            int height = Screen.height;
            Vector3 pos = new Vector3(0, -72400, 0);
            //1.9为容错
            if ((float)width / height >= 1.9)
            {
                pos = new Vector3(0, -65000, 0);
            }
            UITooltipManager.Instance.DisplayTooltipForPress(ShowskillId.ToString() + "," + partnerData.PassiveSkillLevel.ToString(), "Skill", "default", defaultVector3, ePressTipAnchorType.LeftDown, false, false, false, delegate () { });
        }
        
        
        public void DragEvent()
        {
            if (hud.IsSelectSkill)
            {
               OnClickEquipTitle();
            }
            else
            {
                OnClickSkillTile();
            }
        }
    }
}
