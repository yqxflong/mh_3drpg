using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class LTSuitSourceInfoController : DynamicMonoHotfix, IHotfixUpdate
    {
        public DynamicUISprite MainIcon;
        public UILabel NameLabel;
        public UILabel SuitAttr2Label;
        public UILabel SuitAttr4Label;
        public UISprite BGSprite;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            MainIcon = t.GetComponent<DynamicUISprite>("EquipmentInfo/Icon/IMG");
            NameLabel = t.GetComponent<UILabel>("EquipmentInfo/TitleName");
            SuitAttr2Label = t.GetComponent<UILabel>("EquipmentInfo/Infor/Effect_2");
            SuitAttr4Label = t.GetComponent<UILabel>("EquipmentInfo/Infor/Effect_4");
            BGSprite = t.GetComponent<UISprite>("EquipmentInfo");

            m_SourceLabelList = new List<UILabel>();
            m_SourceLabelList.Add(t.GetComponent<UILabel>("EquipmentInfo/Items/0/0"));
            m_SourceLabelList.Add(t.GetComponent<UILabel>("EquipmentInfo/Items/1/1"));
            m_SourceLabelList.Add(t.GetComponent<UILabel>("EquipmentInfo/Items/2/2"));

            m_SourceSpriteList = new List<UISprite>();
            m_SourceSpriteList.Add(t.GetComponent<UISprite>("EquipmentInfo/Items/0/BG"));
            m_SourceSpriteList.Add(t.GetComponent<UISprite>("EquipmentInfo/Items/1/BG"));
            m_SourceSpriteList.Add(t.GetComponent<UISprite>("EquipmentInfo/Items/2/BG"));

            GetLabelObj = t.FindEx("EquipmentInfo/NumLabel").gameObject;
            DropGrid = t.GetComponent<UIGrid>("EquipmentInfo/Items");

            var t0 = t.GetComponent<Transform>("EquipmentInfo/Items/0/0");
            t.GetComponent<UIButton>("EquipmentInfo/Items/0").onClick.Add(new EventDelegate(() => OnGoto(t0)));

            var t1 = t.GetComponent<Transform>("EquipmentInfo/Items/1/1");
            t.GetComponent<UIButton>("EquipmentInfo/Items/1").onClick.Add(new EventDelegate(() => OnGoto(t1)));

            var t2 = t.GetComponent<Transform>("EquipmentInfo/Items/2/2");
            t.GetComponent<UIButton>("EquipmentInfo/Items/2").onClick.Add(new EventDelegate(() => OnGoto(t2)));
        }
    
        public void Show(Hotfix_LT.Data.SuitTypeInfo type,Vector2 Pos)
        {
            MainIcon.spriteName=type.SuitIcon; //装备角标LTPartnerEquipConfig.SuitIconDic[type.SuitType];
            UpdateDrop(type.DropDatas);
            NameLabel.text = NameLabel.transform.GetChild (0).GetComponent <UILabel>().text =string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentSuitInfoItem_3047"), type.TypeName);
            List<SuitAttrsSuitTypeAndCount> SuitList = LTPartnerEquipDataManager.Instance.CurrentPartnerData.EquipmentTotleAttr.SuitList;
            int Count = 0;
            for (int i = 0; i < SuitList.Count; i++)
            {
                if (type.SuitType == SuitList[i].SuitType)
                {
                    Count = SuitList[i].count;
                }
            }
    
            string str = null;
            int suitNeed = 6;
            if (type.SuitAttr2 != 0)
            {
                Hotfix_LT.Data.SkillTemplate suitAttr = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(type.SuitAttr2);//套装2
                str = suitAttr.Description;
                suitNeed = 2;
                SuitAttr2Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_1924"), LTPartnerEquipConfig.HasEffectStrDic[Count >= suitNeed], str);
            }
            else if(type.SuitAttr4 != 0)
            {
                Hotfix_LT.Data.SkillTemplate suitAttr = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(type.SuitAttr4);//套装4
                str = suitAttr.Description;
                suitNeed = 4;
                SuitAttr2Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_2136"), LTPartnerEquipConfig.HasEffectStrDic[Count >= suitNeed], str);
            }
    
            if(Count>= suitNeed)
            {
                SuitAttr2Label.effectStyle = UILabel.Effect.Outline8;
                SuitAttr2Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.74f, 1f, 0.85f);
            }
            else
            {
                SuitAttr2Label.effectStyle = UILabel.Effect.None;
                SuitAttr2Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.75f, 0.75f, 0.75f);
            }
            /*Hotfix_LT.Data.SuitAttribute suitAttr = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitAttrByID(type.SuitAttr2);//套装2
            string str = string.Format(EB.Localizer.GetString("ID_codefont_in_LTSuitSourceInfoController_2338"), suitAttr.desc, (int)(suitAttr.value * 100f));
            Hotfix_LT.Data.SkillTemplate suitAttr2 = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(type.SuitAttr4);//套装4
            string str2 = suitAttr2.Description;
    
            bool hasSuit2_Effect = Count >= 2;//未做判断//BCFFD8
            SuitAttr2Label.text =string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_1924"), LTPartnerEquipConfig.HasEffectStrDic[hasSuit2_Effect], str);
    
            if (hasSuit2_Effect)
            {
                SuitAttr2Label.effectStyle = UILabel.Effect.Outline8;
                SuitAttr2Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.74f,1f,0.85f);
            }
            else
            {
                SuitAttr2Label.effectStyle = UILabel.Effect.None ;
                SuitAttr2Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.75f, 0.75f, 0.75f);
            }
            bool hasSuit4_Effect = Count >= 4;//C0C0C0
            SuitAttr4Label.text =string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_2136"), LTPartnerEquipConfig.HasEffectStrDic[hasSuit4_Effect], str2);
    
            if (hasSuit4_Effect)
            {
                SuitAttr4Label.effectStyle = UILabel.Effect.Outline8;
                SuitAttr4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.74f, 1f, 0.85f);
            }
            else
            {
                SuitAttr4Label.effectStyle = UILabel.Effect.None;
                SuitAttr4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.75f, 0.75f, 0.75f);
            }*/
            SuitAttr4Label.gameObject.SetActive(false);
            //删除了一个套装的效果，不知道会不会改回来1094
            mDMono.gameObject.CustomSetActive(true);
            BGSprite.height = 334+Mathf.Clamp(type.DropDatas .Count,1,3)*242 + ((SuitAttr2Label.gameObject .activeSelf)?SuitAttr2Label.height:0) + ((SuitAttr4Label.gameObject.activeSelf)?SuitAttr4Label.height:0);
            //SetAnchor(Pos);
    
        }
    
        void SetAnchor(Vector2 screenPos)
        {
            Vector3 worldPos = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y));
            mDMono.transform.position = worldPos;
        }
    
        private bool CheckMouseClick = false;
        public override void OnEnable()
        {
			RegisterMonoUpdater();
		
            CheckMouseClick = true;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public void Update()
        {
            if (CheckMouseClick && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
            {
                if (!mDMono.transform.GetChild (0). GetComponent<BoxCollider>().bounds.Contains(UICamera.lastWorldPosition) )
                {
                    CheckMouseClick = false;
                    mDMono.gameObject.SetActive(false);
                }
            }
        }
    
        public List<UILabel> m_SourceLabelList;
        public List<UISprite> m_SourceSpriteList;
        public GameObject GetLabelObj;
        public UIGrid DropGrid;
        private int[] bigBGHighValue = new int[4] { 576, 664, 908, 1152 };
        private int[] smallBGHighValue = new int[4] { 234, 334, 578, 822 };
        private IList DropDatas;
        public void OnGoto(Transform t)
        {
            if (DropDatas == null)
                EB.Debug.LogError("dropDatas = null");
            if (DropDatas.Count == 0)
                return;
            int index = int.Parse(t.gameObject.name);
            if (index > DropDatas.Count)
            {
                EB.Debug.LogError("index > DropDatas.Count");
                return;
            }
            Hotfix_LT.Data.DropDataBase data = (DropDatas[index] as Hotfix_LT.Data.DropDataBase);
            if (mDMono.transform.GetUIControllerILRComponent<UIToolTipPanelController>(false)==null)
            {
                data.GotoDrop();
            }
            else
            {
                data.GotoDrop(mDMono.transform.GetUIControllerILRComponent<UIToolTipPanelController>().controller);
            }
        }    
    
        private void UpdateDrop(List<Hotfix_LT.Data.DropDataBase> dropDatas)
        {
            DropDatas = dropDatas;
            int count = dropDatas.Count;
            if (count > 0)
            {
                GetLabelObj.SetActive(false);
                for (int i = 0; i < 3; ++i)
                {
                    if (i < count)
                    {
                        m_SourceLabelList[i].transform.parent.gameObject.CustomSetActive(true);
                        dropDatas[i].ShowName(m_SourceLabelList[i]);
                        dropDatas[i].ShowBG(m_SourceSpriteList[i]);
                        m_SourceLabelList[i].transform.parent.SetSiblingIndex(i);
                    }
                    else
                    {
                        m_SourceLabelList[i].transform.parent.gameObject.CustomSetActive(false);
                    }
                }
    
                for (int i = 0; i < count; i++)
                {
                    if (!dropDatas[i].IsOpen)
                    {
                        m_SourceLabelList[i].transform.parent.SetSiblingIndex(count - 1);
                    }
                }
            }
            else
            {
                GetLabelObj.SetActive(true);
                m_SourceLabelList[1].transform.parent.gameObject.CustomSetActive(false);
                m_SourceLabelList[2].transform.parent.gameObject.CustomSetActive(false);
                m_SourceLabelList[0].transform.parent.gameObject.CustomSetActive(false);
            }
            DropGrid.enabled=true; 
        }
    }
}
