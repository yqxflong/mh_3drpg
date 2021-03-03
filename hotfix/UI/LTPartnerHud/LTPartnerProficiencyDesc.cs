using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerProficiencyDesc : DynamicMonoHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            MainSprite = t.GetComponent<UISprite>("BG/Proficiency/Icon");
            NameLabel = t.GetComponent<UILabel>("BG/Proficiency/NameLabel");
            LevelLabel = t.GetComponent<UILabel>("BG/Proficiency/LevelLabel");
            CurIncomeLabel = t.GetComponent<UILabel>("BG/Grid/Attr/NumLabel");
            NextIncomeLabel = t.GetComponent<UILabel>("BG/Grid/Attr (1)/NumLabel");
            box = t.GetComponent<BoxCollider>();
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public UISprite MainSprite;
        public UILabel NameLabel,LevelLabel,CurIncomeLabel,NextIncomeLabel;
    
        private BoxCollider box;
    
       
    
        public void Update()
        {
            if ((Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
            {
                if (!box.bounds.Contains(UICamera.lastWorldPosition))
                {
                    CloseUI();
                }
            }
        }
    
        public void ShowUI(Hotfix_LT.Data.ProficiencyUpTemplate cur, Hotfix_LT.Data.ProficiencyUpTemplate next)
        {
            LevelLabel.text = "Lv.0";
            Hotfix_LT.Data.ProficiencyUpTemplate topData=null;
            if (cur != null)
            {
                var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetProficiencyDescByType(cur .type);
                if (temp != null)
                {
                    MainSprite.spriteName = temp.icon;
                    NameLabel.text = temp.name;
                    LevelLabel.text =string .Format ("Lv.{0}", cur.level);
                }
                topData = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetTheTopProficiencyType(cur.type);
            }
            else if (next!=null)
            {
                var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetProficiencyDescByType(next.type);
                if (temp != null)
                {
                    MainSprite.spriteName = temp.icon;
                    NameLabel.text = temp.name;
                    LevelLabel.text = "Lv.0";
                }
                topData = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetTheTopProficiencyType(next.type);
            }
            else
            {
                MainSprite.spriteName = NameLabel.text = LevelLabel.text = string.Empty;
            }
            CurIncomeLabel.text = GetDesc(cur, topData);
            NextIncomeLabel.text = GetDesc(next, topData);
            OpenUI();
        }
    
        public static string GetDesc(Hotfix_LT.Data.ProficiencyUpTemplate data, Hotfix_LT.Data.ProficiencyUpTemplate topData=null)
        {
            string temp = string.Empty;
            if (data != null)
            {
                if (data.ATK > 0 || topData != null && topData.ATK > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_ATK"), Mathf.RoundToInt(data.ATK*100));
                if (data.maxHP > 0 || topData != null && topData.maxHP > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_MaxHP"), Mathf.RoundToInt(data.maxHP * 100));
                if (data.DEF > 0 || topData != null && topData.DEF > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_DEF"), Mathf.RoundToInt(data.DEF * 100));
                if (data.CritV > 0 || topData != null && topData.CritV > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_CritV"), Mathf.RoundToInt(data.CritV * 100));
                if (data.speed > 0 || topData != null && topData.speed > 0) temp += string.Format("  {0}+{1}", EB.Localizer.GetString("ID_ATTR_Speed"), Mathf.RoundToInt(data.speed));
                if (data.CritP > 0 || topData != null && topData.CritP > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_CritP"), Mathf.RoundToInt(data.CritP * 100));
                if (data.AntiCritP > 0 || topData != null && topData.AntiCritP > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_CRIresist"), Mathf.RoundToInt(data.AntiCritP * 100));
                if (data.SpExtra > 0 || topData != null && topData.SpExtra > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_SpExtra"), Mathf.RoundToInt(data.SpExtra * 100));
                if (data.SpRes > 0 || topData != null && topData.SpRes > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_SpRes"), Mathf.RoundToInt(data.SpRes * 100));
                if (data.DmgMulti > 0 || topData != null && topData.DmgMulti > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_DMGincrease"), Mathf.RoundToInt(data.DmgMulti * 100));
                if (data.DmgRes > 0 || topData != null && topData.DmgRes > 0) temp += string.Format("  {0}+{1}%", EB.Localizer.GetString("ID_ATTR_DMGreduction"), Mathf.RoundToInt(data.DmgRes * 100));
            }
            else if(topData!=null)
            {
                if (topData.ATK > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_ATK"));
                if (topData.maxHP > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_MaxHP"));
                if (topData.DEF > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_DEF"));
                if (topData.CritV > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_CritV"));
                if (topData.speed > 0) temp += string.Format("  {0}+0", EB.Localizer.GetString("ID_ATTR_Speed"));
                if (topData.CritP > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_CritP"));
                if (topData.AntiCritP > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_CRIresist"));
                if (topData.SpExtra > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_SpExtra"));
                if (topData.SpRes > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_SpRes"));
                if (topData.DmgMulti > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_DMGincrease"));
                if (topData.DmgRes > 0) temp += string.Format("  {0}+0%", EB.Localizer.GetString("ID_ATTR_DMGreduction"));
            }
            temp=temp.Trim();
            if (string.IsNullOrEmpty(temp)) temp = EB.Localizer.GetString("ID_LEGION_MEDAL_NOT");
            return temp;
        }
    
        private void OpenUI()
        {
           mDMono.transform.GetComponent<UIPanel>().sortingOrder = mDMono.transform.parent.GetComponent<UIPanel>().sortingOrder + 1;
           mDMono.transform.GetComponent<UIPanel>().depth = mDMono.transform.parent.GetComponent<UIPanel>().depth + 1;
           mDMono.gameObject.CustomSetActive(true);
        }
    
        private void CloseUI()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    }
}
