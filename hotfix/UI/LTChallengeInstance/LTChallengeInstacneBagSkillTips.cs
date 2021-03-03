using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstacneBagSkillTips : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            item = t.GetMonoILRComponent<LTShowItem>("UpGroup/LTShowItem");
            //NameLab = t.GetComponent<UILabel>("UpGroup/Label");
            MagicPointLab = t.GetComponent<UILabel>("UpGroup/Magic/Label");
            DescLab = t.GetComponent<UILabel>("NormalContent/Desc");
            MagicObj = t.FindEx("UpGroup/Magic").gameObject;
            EquipContent = t.FindEx("EquipContent").gameObject;
            NormalContent = t.FindEx("NormalContent").gameObject;
            OtherAttrLab = t.GetComponent<UILabel>("EquipContent/AttrList/AttachAttr/Num");
            SuitAttrDescLab = t.GetComponent<UILabel>("EquipContent/AttrList/FourSuitAttr/Desc");
        }
        
        public LTShowItem item;
        //public UILabel NameLab;
        public UILabel MagicPointLab;
        public UILabel DescLab;
        public GameObject MagicObj;
    
        public GameObject EquipContent;
    
        public GameObject NormalContent;
    
        private LTShowItemData curData;
    
        #region 装备
    
        public UILabel OtherAttrLab;
        public UILabel SuitAttrDescLab;
    
        #endregion
    
        public void Init(LTShowItemData data)
        {
            HideUI();
            curData = data;
            item.LTItemData = data;

            EquipContent.CustomSetActive(false);
            NormalContent.CustomSetActive(true);

            if (data.type == LTShowItemType.TYPE_SCROLL)
            {
                Hotfix_LT.Data.SkillTemplate skillTpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(int.Parse(data.id));
                DescLab.text = skillTpl.Description;
    
                int magicPoint = BalanceResourceUtil.GetResValue("mana");
                MagicPointLab.text = skillTpl.SPCost.ToString();
    
                MagicObj.CustomSetActive(true);
                DescLab.gameObject.CustomSetActive(true);
            }
            else
            {
                string id = data.id;
                if (data.type == LTShowItemType.TYPE_RES)
                {
                    id =BalanceResourceUtil.GetResID(data.id).ToString();
                }

                Hotfix_LT.Data.EconemyItemTemplate itemTpl = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(id);
                MagicObj.CustomSetActive(false);
    
                if (itemTpl != null)
                {
                    DescLab.gameObject.CustomSetActive(true);
                    if (itemTpl is Hotfix_LT.Data.EquipmentItemTemplate)
                    {
                        NormalContent.CustomSetActive(false);
                        EquipContent.CustomSetActive(true);
                        InitEquipContent(itemTpl as Hotfix_LT.Data.EquipmentItemTemplate);
                    }
                    else
                    {
                        DescLab.text = itemTpl.Desc;
                    }
                }
                else
                {
                    DescLab.gameObject.CustomSetActive(false);
                }
            }

            ShowUI();
        }
    
        private void InitEquipContent(Hotfix_LT.Data.EquipmentItemTemplate data)
        {
            InitEquipOtherAttr(data.QualityLevel);
    
            int index = data.SuitAttrId_1 > 0 ? 1 : 2;
            int suitSkillID = data.SuitAttrId_1 > 0 ? data.SuitAttrId_1 : data.SuitAttrId_2;
            InitEquipSuitDesc(index, suitSkillID);
        }
    
        private void InitEquipOtherAttr(int star)
        {
            int low = 1;
            int high = 5;
    
            Hotfix_LT.Data.EquipAttributeRate rate = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipAttributeRate(star);
            if (rate != null)
            {
                for (int i = 0; i < rate.rating.Count; i++)
                {
                    if (rate.rating[i] > 0)
                    {
                        low = i;
                        break;
                    }
                }
    
                for (int i = rate.rating.Count - 1; i >= 0; i--)
                {
                    if (rate.rating[i] > 0)
                    {
                        high = i;
                        break;
                    }
                }
            }
    
            OtherAttrLab.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeInstacneBagSkillTips_2765"), low, high);
        }
    
        private void InitEquipSuitDesc(int index, int suitSkillID)
        {
            string str1 = index == 1 ?EB.Localizer.GetString("ID_TWO_SUIT") : index == 2 ? EB.Localizer.GetString("ID_FOUR_SUIT") : "";
            Hotfix_LT.Data.SkillTemplate suitAttr = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(suitSkillID);
            if (suitAttr != null)
            {
                SuitAttrDescLab.text = string.Format("{0}: {1}", str1, suitAttr.Description);
            }
            else
            {
                SuitAttrDescLab.text = string.Empty;
            }
        }
    
        public void ShowUI()
        {
            mDMono.gameObject.CustomSetActive(true);
        }
    
        public void HideUI()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    }
}
