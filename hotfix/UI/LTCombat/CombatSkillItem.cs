using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class CombatSkillItem : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Icon = t.GetComponent<DynamicUISprite>("Icon");
            NameLabel = t.GetComponent<UILabel>("Panel/Name");
            CDMaskSprite = t.GetComponent<UISprite>("CDMask");
            FrameSprite = t.GetComponent<UISprite>("BG");
            CDTurnLabel = t.GetComponent<UILabel>("CDValue");
            SelectedObj = t.FindEx("Selected").gameObject;
            PassiveFX = t.FindEx("FX").gameObject;

            t.GetComponent<UIEventTrigger>("Icon").onClick.Add(new EventDelegate(OnSelectClick));
        }

        public override void Start()
        {
            // 移除复制过来的技能选中特效对象
            if (SelectedObj != null && !SelectedObj.activeInHierarchy)
            {
                var fx = SelectedObj.transform.GetChild(0);

                if (fx.childCount > 0)
                {
                    GameObject.Destroy(fx.GetChild(0).gameObject);
                }
            }
        }

        public DynamicUISprite Icon;
    	public UILabel NameLabel;
    	public UISprite CDMaskSprite;
    	public UISprite FrameSprite;
    	public UILabel CDTurnLabel;
    	public GameObject SelectedObj;
        public GameObject PassiveFX;
    
    	public Hotfix_LT.Combat.CombatCharacterSyncData.SkillData Data { get; private set; }
    
    	public void Fill(Hotfix_LT.Combat.CombatCharacterSyncData.SkillData skillData,bool isSelect=false,bool onlyFill=false)
    	{
            this.Data = skillData;
            Icon.spriteName = skillData.Icon;
            LTUIUtil.SetText(NameLabel, skillData.TypeName);
            SkillSetTool.SkillFrameStateSet(FrameSprite, Hotfix_LT.Data.CharacterTemplateManager.Instance.IsAwakenSkill(skillData.ID));
            if (skillData.SkillType == (int)Hotfix_LT.Data.eSkillType.ACTIVE)
            {
                CDMaskSprite.fillAmount = (float)skillData.CD / skillData.MaxCooldown;
                CDMaskSprite.gameObject.SetActive(true);
                if (skillData.CD > 0)
                {
                    Icon.color = new Color(255 / 255f, 0, 255 / 255f, 1);
                    LTUIUtil.SetText(CDTurnLabel, skillData.CD.ToString());
                }
                else
                {
                    Icon.color = Color.white;
                    LTUIUtil.SetText(CDTurnLabel, "");
                }
            }
            else
            {
                if (skillData.CD > 0)
                    Debug.LogError("Not SpecialSkill But skillCD>0");
                LTUIUtil.SetText(CDTurnLabel, "");
                CDMaskSprite.gameObject.SetActive(false);
                Icon.color = Color.white;
            }

            if (!onlyFill)
            {
                if (skillData.SkillType == (int)Hotfix_LT.Data.eSkillType.PASSIVE)
                {
                    PassiveFX.gameObject.SetActive(true);
                }
                else
                {
                    PassiveFX.gameObject.SetActive(false);
                }
                SelectedObj.gameObject.SetActive(isSelect);
            }

            mDMono.gameObject.CustomSetActive(true);
    	}
    
    	public void OnSelectClick()
    	{
            OnSkillClick();

            if (Data.SkillType == (int)Hotfix_LT.Data.eSkillType.PASSIVE)
            {
                return;
            }
            else if (Data.SkillType == (int)Hotfix_LT.Data.eSkillType.ACTIVE && Data.CD > 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_CombatSkillItem_2075"));
                return;
            }

            if (Data.SkillType == (int)Hotfix_LT.Data.eSkillType.NORMAL && !mDMono.transform.parent.GetMonoILRComponent<CombatSkillController>().CharacterData.IsSkillCanUse(Data))
            {
                MessageTemplateManager.ShowMessage(902273);
                return;
            }

            if (Data.SkillType == (int)Hotfix_LT.Data.eSkillType.ACTIVE && !mDMono.transform.parent.GetMonoILRComponent<CombatSkillController>().CharacterData.IsSkillCanUse(Data))
            {
                MessageTemplateManager.ShowMessage(902273);
                return;
            }

            SelectedObj.gameObject.SetActive(true);
            mDMono.transform.parent.GetMonoILRComponent<CombatSkillController>().OnSkillSelectEvent(Data.ID, (Hotfix_LT.Data.eSkillType)Data.SkillType);
        }
    
    	public void UnSelect()
    	{
    		SelectedObj.gameObject.SetActive(false);
    	}
    
    	public void OnSkillClick()
    	{
            mDMono.transform.parent.GetMonoILRComponent<CombatSkillController>().OnSkillClickEvent(Data.ID,Data.Level);
    	}
    }
}
