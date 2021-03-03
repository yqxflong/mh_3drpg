using System.Collections;
using UnityEngine;

/// <summary>
/// 背包格子实现
/// </summary>
namespace Hotfix_LT.UI
{
    public class UIMHBindableItemDataLookUp : DynamicMonoHotfix
    {
    	public const string m_EconomyIDPropertyName = "economy_id";
    	public const string m_NumPropertyName= "num";
    	public const string m_QualityLevelPropertyName = "qualityLevel";
    	public const string m_TypeName = "system";
        public const string m_EquipLevelStr = "currentLevel";
    
        public DynamicUISprite m_Icon;
    	public UISprite m_LevelBorder;
        public UISprite m_FrameBG;
        public UILabel m_Count;
        public GameObject RedPoint;//红点
        public GameObject Border;
        
        public string m_Type;

        private ParticleSystemUIComponent mQualityFX;
        private EffectClip mEffectClip;

        #region 装备item使用，不是装备item需要隐藏
        public DynamicUISprite m_EquipSuitIcon;//装备类型图标（不是装备隐藏，附加套装箱子的物品也需要显示(by:2019/4/2)）
        public UISprite m_EquipLevelBG;
        public UILabel m_EquipLevel;
        public UILabel m_EuqipLevelShadow;
        #endregion

        #region 伙伴碎片item使用，不是伙伴碎片item需要隐藏
        public GameObject m_Flag;
        #endregion
    
        #region 材料箱阶级数，不是材料箱或阶级数为0需要要隐藏
        public UILabel m_boxGradeNumLab;
        #endregion

        public UIInventoryBagLogic m_OwnerList;
        private bool m_IsEmpty = true;

        public bool IsEmpty
        {
            get
            {
                return m_IsEmpty;
            }
        }

        private IDictionary m_ItemData;
        public IDictionary ItemData
        {
            set
            {
                m_ItemData = value;
                if (value == null) m_IsEmpty = true;
                else m_IsEmpty = false;
                UpdateUI();
            }
            get
            {
                return m_ItemData;
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_OwnerList = t.parent.parent.GetMonoILRComponentByClassPath<UIInventoryBagLogic>("Hotfix_LT.UI.UIInventoryBagLogic");
            m_Icon = t.GetComponent<DynamicUISprite>("IMG");
            m_LevelBorder = t.GetComponent<UISprite>("IMG/LvlBorder");
            m_FrameBG = t.GetComponent<UISprite>("IMG/LvlBorder/FrameBG");
            m_EquipSuitIcon = t.GetComponent<DynamicUISprite>("EquipSuitIcon");
            m_EquipLevelBG = t.GetComponent<UISprite>("EquipLevel/Sprite");
            m_EquipLevel = t.GetComponent<UILabel>("EquipLevel");
            m_EuqipLevelShadow = t.GetComponent<UILabel>("EquipLevel/LabelShadow");
            m_Count = t.GetComponent<UILabel>("Label");
            m_Flag = t.FindEx("Flag").gameObject;
            m_boxGradeNumLab = t.GetComponent<UILabel>("BoxGradeNum");
            RedPoint = t.FindEx("RedPoint").gameObject;
            Border = t.FindEx("Sprite").gameObject;
        }

        private  void UpdateUI() {
    
    		if (null != ItemData)
    		{
                mDMono.gameObject.GetComponent<BoxCollider>().enabled = true;
                string templateid = EB.Dot.String(m_EconomyIDPropertyName, ItemData,"");
    			int num= EB.Dot.Integer(m_NumPropertyName, ItemData, 0);
    			int quality_level= EB.Dot.Integer(m_QualityLevelPropertyName, ItemData, 0);
    
    			m_Icon.spriteName = Hotfix_LT.Data.EconemyTemplateManager.GetItemIcon(templateid);
    			m_Type = EB.Dot.String(m_TypeName, ItemData, "");
                m_LevelBorder.spriteName = UIItemLvlDataLookup.LvlToStr(quality_level + "");
                if (quality_level == 7) m_FrameBG.spriteName = "Ty_Quality_Xuancai_Di";
                else m_FrameBG.spriteName = "Ty_Di_2";
                m_FrameBG.color = UIItemLvlDataLookup.GetItemFrameBGColor(quality_level + "");
                m_LevelBorder.gameObject.CustomSetActive(true);

                HotfixCreateFX.ShowItemQualityFX(mQualityFX, mEffectClip, mDMono.transform, quality_level);
    
                m_Flag.CustomSetActive(m_Type == "HeroShard");
                m_EquipSuitIcon.gameObject.CustomSetActive(m_Type == "Equipment");
                m_EquipLevel.gameObject.CustomSetActive(m_Type == "Equipment");
    
                if (m_Type == "Equipment")
                {
                    
                    int equipLevel = EB.Dot.Integer(m_EquipLevelStr, ItemData, 1);
                    if (equipLevel <= 0)
                    {
                        m_EquipLevel.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        m_EquipLevel.gameObject.CustomSetActive(true);
                        m_EquipLevel.text = m_EuqipLevelShadow.text = "+" + equipLevel;
                        m_EquipLevelBG.spriteName = UIItemLvlDataLookup.GetEquipLevelBGStr(quality_level);
                    }
                    m_EquipSuitIcon.spriteName = Hotfix_LT.Data.EconemyTemplateManager.GetEquipSuitIcon(templateid);
                }
    
                int grade = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGoodsGradeNum(templateid);
                m_boxGradeNumLab.gameObject.CustomSetActive(grade != 0);
                if (grade != 0)
                {
                    m_boxGradeNumLab.text = string.Format("+{0}", grade);
                }
    
                string suitIcon = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipSuit(templateid);
                if (!string.IsNullOrEmpty(suitIcon))
                {
                    m_EquipSuitIcon.gameObject.CustomSetActive(true);
                    m_EquipSuitIcon.spriteName = suitIcon;
                }
    
    			if (m_Count != null)
    			{
    				if(num>1)
    				{
    					m_Count.text = num + "";
    					m_Count.gameObject.CustomSetActive(true);
    				}
    				else
    				{
    					m_Count.gameObject.CustomSetActive(false);
    				}                
    			}
                string InventoryId = EB.Dot.String("inventory_id", ItemData, "");
                if (LTInventoryAllController._CurrentSelectCellId != null && LTInventoryAllController._CurrentSelectCellId.Contains(InventoryId))
                {
                    LTInventoryAllController._CurrentSelectCell = mDMono.transform.GetMonoILRComponentByClassPath<UIInventoryBagCellController>("Hotfix_LT.UI.UIInventoryBagCellController");
                    Border.CustomSetActive(true);
                }
                else
                {
                    Border.CustomSetActive(false);
                }
                
                //红点判断
                string eid = EB.Dot.String("economy_id", ItemData, null);
                if (eid != null)
                {
                    var GeneralItem = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(eid);
                    RedPoint.CustomSetActive(GeneralItem != null && GeneralItem.CanUse && !LTMainHudManager.Instance.ItemsList.Contains(InventoryId));
                }
    
            }
    		else
    		{
    			m_Icon.spriteName = "";
    			m_LevelBorder.gameObject.CustomSetActive(false);
                m_Flag.CustomSetActive(false);
                m_EquipSuitIcon.gameObject.CustomSetActive(false);
                m_EquipLevel.gameObject.CustomSetActive(false);
                m_boxGradeNumLab.gameObject.CustomSetActive(false);
                mDMono.gameObject.GetComponent<BoxCollider>().enabled = false;
    			if (m_Count != null) m_Count.gameObject.CustomSetActive(false);
                RedPoint.CustomSetActive(false);
                Border.CustomSetActive(false);
                if (mQualityFX != null)
                {
                    mQualityFX.gameObject.CustomSetActive(false);
                }
                else
                {
                    if (mDMono.transform.Find("QualityFX") != null)
                    {
                        mQualityFX = mDMono.transform.Find("QualityFX").GetComponent<ParticleSystemUIComponent>();
                        mEffectClip = mDMono.transform.Find("QualityFX").GetComponent<EffectClip>();
                        mQualityFX.gameObject.CustomSetActive(false);
                    }
                }
            }
    	}
    
    }
}
