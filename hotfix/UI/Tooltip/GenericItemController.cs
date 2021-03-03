using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class GenericItemInfo
    {
        public string InventoryId;
        public string EconomyId;
        public int Num;
        public int Price;
        public string DropDesc;
    }

    public class GenericItemController : DynamicMonoHotfix
    {
        static private GenericItemController _Instance;
        static public GenericItemController Instance { get { return _Instance; } }
        public GenericItemInfo ItemInfo = new GenericItemInfo();
        public DynamicUISprite m_Icon;
        public GameObject IconParent;
        public UISprite m_Frame, m_FrameBG;
        public UILabel m_GoodsName;
        public UILabel m_NumLabel;
        public Transform DropItemsRoot;
        public UIGrid DropItemsGrid;
        public List<UILabel> m_SourceLabelList;
        public List<UISprite> m_SourceSpriteList;
        public UIPanel m_ThisPanel;
        public GameObject m_ClipFlag;
        public DynamicUISprite m_EquipSuitIcon;
        private ParticleSystemUIComponent mQualityFX;
        private EffectClip mEffectClip;

        #region 材料箱阶级数，不是材料箱或阶级数为0需要要隐藏
        public UILabel m_boxGradeNumLab;
        #endregion

        private string m_TipTargetStr = "tooltip.target";
        public string m_InventoryId;
        private bool m_IsSell = false;
        private int m_QualityLevel;

        private UIToolTipPanelController _uiToolTipController;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Icon = t.GetComponent<DynamicUISprite>("DataPanel/Item/Icon");
            IconParent = t.FindEx("DataPanel/Item").gameObject;
            m_Frame = t.GetComponent<UISprite>("DataPanel/Item/Icon/LvlBorder");
            m_FrameBG = t.GetComponent<UISprite>("DataPanel/Item/Icon/FrameBG");
            m_GoodsName = t.GetComponent<UILabel>("DataPanel/Item/NameLabel");
            m_NumLabel = t.GetComponent<UILabel>("DataPanel/Item/NumLabel");
            DropItemsRoot = t.GetComponent<Transform>("DataPanel/Sources/Items");
            DropItemsGrid = t.GetComponent<UIGrid>("DataPanel/Sources/Items");

            m_SourceLabelList = new List<UILabel>();
            m_SourceLabelList.Add(t.GetComponent<UILabel>("DataPanel/Sources/Items/0/0"));
            m_SourceLabelList.Add(t.GetComponent<UILabel>("DataPanel/Sources/Items/1/1"));
            m_SourceLabelList.Add(t.GetComponent<UILabel>("DataPanel/Sources/Items/2/2"));


            m_SourceSpriteList = new List<UISprite>();
            m_SourceSpriteList.Add(t.GetComponent<UISprite>("DataPanel/Sources/Items/0/BG"));
            m_SourceSpriteList.Add(t.GetComponent<UISprite>("DataPanel/Sources/Items/1/BG"));
            m_SourceSpriteList.Add(t.GetComponent<UISprite>("DataPanel/Sources/Items/2/BG"));

            m_ThisPanel = t.GetComponentEx<UIPanel>();
            m_ClipFlag = t.FindEx("DataPanel/Item/Icon/Flag").gameObject;
            m_EquipSuitIcon = t.GetComponent<DynamicUISprite>("DataPanel/Item/Icon/EquipSuitIcon");
            m_boxGradeNumLab = t.GetComponent<UILabel>("DataPanel/Item/Icon/BoxGradeNum");
            BigBG = t.GetComponent<UIWidget>("DynamicBG/BG");
            SmallBG = t.GetComponent<UIWidget>("DynamicBG/BG4");
            GetLabelObj = t.FindEx("DataPanel/Sources/NullLabel").gameObject;

            t.GetComponent<UIButton>("DataPanel/Sources/Items/0").onClick.Add(new EventDelegate(() => OnGoto(t.GetComponent<Transform>("DataPanel/Sources/Items/0/0"))));
            t.GetComponent<UIButton>("DataPanel/Sources/Items/1").onClick.Add(new EventDelegate(() => OnGoto(t.GetComponent<Transform>("DataPanel/Sources/Items/1/1"))));
            t.GetComponent<UIButton>("DataPanel/Sources/Items/2").onClick.Add(new EventDelegate(() => OnGoto(t.GetComponent<Transform>("DataPanel/Sources/Items/2/2"))));

            if (_Instance == null)
            {
                _Instance = this;
            }

            _uiToolTipController = t.GetUIControllerILRComponent<UIToolTipPanelController>();
        }

        public override void OnDestroy()
        {
            if (_Instance == this)
            {
                _Instance = null;
            }
        }

        public void Show()
        {
            string economyId = "";
            string unhave;
            int num;
            if (!DataLookupsCache.Instance.SearchDataByID<string>("tooltip.unhave", out unhave, null))
            {
                if (!DataLookupsCache.Instance.SearchDataByID<string>("{" + m_TipTargetStr + "}.economy_id", out economyId, null))
                {
                    EB.Debug.LogError("SearchDataByID {0}.economy_id is null", m_TipTargetStr);
                    return;
                }
                m_InventoryId = InventoryId;
                if (!DataLookupsCache.Instance.SearchIntByID(m_InventoryId + ".num", out num, null))
                    EB.Debug.LogError("SearchDataByID inventory_id==null");
            }
            else
            {
                if (!DataLookupsCache.Instance.SearchDataByID<string>(m_TipTargetStr, out economyId, null))
                {
                    EB.Debug.LogError("SearchDataByID {0} is null",m_TipTargetStr);
                    return;
                }
                num = GameItemUtil.GetInventoryItemNum(economyId);
            }

            LTIconNameQuality inl = LTItemInfoTool.GetInfo(economyId, LTShowItemType.TYPE_GAMINVENTORY);
            m_Icon.spriteName = inl.icon;
            m_Frame.spriteName = UIItemLvlDataLookup.LvlToStr(inl.quality);
            m_FrameBG.spriteName = UIItemLvlDataLookup.GetItemFrameBGSprite(inl.quality);
            m_FrameBG.color = UIItemLvlDataLookup.GetItemFrameBGColor(inl.quality);
            m_QualityLevel = int.Parse(inl.quality);
            LTUIUtil.SetText(m_GoodsName, inl.name);
            var itemInfo = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(economyId);
            if (itemInfo != null)
            {
                m_ClipFlag.CustomSetActive(itemInfo.System == "HeroShard");
            }
            else
            {
                m_ClipFlag.CustomSetActive(false);
            }

            string equipSuitIconStr = Hotfix_LT.Data.EconemyTemplateManager.GetEquipSuitIcon(economyId);
            if (string.IsNullOrEmpty(equipSuitIconStr))
            {
                m_EquipSuitIcon.gameObject.CustomSetActive(false);

                string suitIcon = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipSuit(economyId);
                if (!string.IsNullOrEmpty(suitIcon))
                {
                    m_EquipSuitIcon.gameObject.CustomSetActive(true);
                    m_EquipSuitIcon.spriteName = suitIcon;
                }
            }
            else
            {
                m_EquipSuitIcon.gameObject.CustomSetActive(true);
                m_EquipSuitIcon.spriteName = equipSuitIconStr;
            }

            int grade = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGoodsGradeNum(economyId);
            m_boxGradeNumLab.gameObject.CustomSetActive(grade != 0);
            if (grade != 0)
            {
                m_boxGradeNumLab.text = string.Format("+{0}", grade);
            }

            var item = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(economyId);
            if (item.DropDatas.Count <= 0)
            {
                DropItemsRoot.gameObject.CustomSetActive(true);
                UpdateDrop(item);
            }
            else
            {
                DropItemsRoot.gameObject.CustomSetActive(true);
                UpdateDrop(item);
            }
            ItemInfo.InventoryId = m_InventoryId;
            ItemInfo.EconomyId = economyId;
            ItemInfo.Num = num;
            LTUIUtil.SetText(m_NumLabel, EB.Localizer.GetString("ID_LABEL_NAME_HADE") + num);

            //从伙伴进阶界面打开跳转界面需要做刷新伙伴进阶界面操作
            if (!string.IsNullOrEmpty(LTPartnerDataManager.Instance.itemID))
            {
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerUIRefresh, CultivateType.UpGrade);
            }

            IconParent.CustomSetActive(true);

        }

        public void UpdateNum(int num)
        {
            m_IsSell = true;
            ItemInfo.Num = num;
            LTUIUtil.SetText(m_NumLabel, EB.Localizer.GetString("ID_LABEL_NAME_HADE") + num);
        }

        public void Close()
        {
            HideUI();
            IconParent.CustomSetActive(false);
            if (ShowBagContent.Instance != null)
            {
                UIInventoryBagLogic.Instance.RefreshBagItem();
            }
            if (!m_IsSell)
                return;

            m_IsSell = false;

            if (ItemInfo.Num <= 0)
                DataLookupsCache.Instance.CacheData(m_InventoryId, null);
            UIInventoryBagLogic.Instance.RefeshBag();
        }

        private void ShowUI()
        {
            StartCoroutine(ShowUIIE());
        }

        private IEnumerator ShowUIIE()
        {
            // 等待0.1s是为了防止闪屏，在长界面切到短界面或短界面切到长界面的时候，高度值虽然改变了，但是锚点的对其需要下一帧才会刷新
            yield return null;
            m_ThisPanel.alpha = 1;

            HotfixCreateFX.ShowItemQualityFX(mQualityFX, mEffectClip, m_Icon.transform, m_QualityLevel);
        }

        private void HideUI()
        {
            m_ThisPanel.alpha = 0;
        }

        //  无：576, 234  一个：664,334  两个：908, 578  三个：1152, 822  单个间隔：244
        public UIWidget BigBG;
        public UIWidget SmallBG;
        public GameObject GetLabelObj;
        private int[] bigBGHighValue = new int[4] { 520, 590, 840, 1090 };
        private int[] smallBGHighValue = new int[4] { 210, 310, 560, 810 };
        private IList DropDatas;

        private void UpdateDrop(Hotfix_LT.Data.EconemyItemTemplate item)
        {
            DropDatas = item.DropDatas;
            int count = DropDatas.Count;
            BigBG.height = bigBGHighValue[count];
            SmallBG.height = smallBGHighValue[count];
            if (count > 0)
            {
                GetLabelObj.CustomSetActive(false);
                for (int i = 0; i < 3; ++i)
                {
                    if (i < count)
                    {
                        m_SourceLabelList[i].transform.parent.gameObject.CustomSetActive(true);
                        item.DropDatas[i].ShowName(m_SourceLabelList[i]);
                        item.DropDatas[i].ShowBG(m_SourceSpriteList[i]);
                        m_SourceLabelList[i].transform.parent.SetSiblingIndex(i);
                    }
                    else
                    {
                        m_SourceLabelList[i].transform.parent.gameObject.CustomSetActive(false);
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    if (!item.DropDatas[i].IsOpen)
                    {
                        m_SourceLabelList[i].transform.parent.SetSiblingIndex(count - 1);
                    }
                }
                DropItemsGrid.Reposition();
            }
            else
            {
                GetLabelObj.CustomSetActive(true);
                m_SourceLabelList[1].transform.parent.gameObject.CustomSetActive(false);
                m_SourceLabelList[2].transform.parent.gameObject.CustomSetActive(false);
                m_SourceLabelList[0].transform.parent.gameObject.CustomSetActive(false);
            }
            ShowUI();
        }
        
        public static void SetButtonEnable(UILabel label, string text, bool enable)
        {
            var btn = label.GetComponent<UIButton>();
            if (!enable)
                btn.tweenTarget = null;
            btn.isEnabled = enable;
            label.text = label.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal, text);

            label.transform.parent.GetChild(3).gameObject.CustomSetActive(!enable);
        }

        public string InventoryId
        {
            get
            {
                string inventory_id;
                if (!DataLookupsCache.Instance.SearchDataByID<string>(m_TipTargetStr, out inventory_id, null))
                {
                    EB.Debug.LogError("SearchDataByID inventory_id==null");
                    return "";
                }
                return inventory_id;
            }
        }

        public void OnSell()
        {
            _uiToolTipController.controller.Close();
            GlobalMenuManager.Instance.Open("GenericSellView", ItemInfo);
        }

        public void OnUse()
        {
            _uiToolTipController.controller.Close();
            GlobalMenuManager.Instance.Open("GenericUseView", ItemInfo);
        }

        public void OnGoto(Transform t)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
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
            data.GotoDrop(_uiToolTipController.controller);

        }

        private static int IsVigorEnough(int times, int CampaignId)
        {
            int curVigor = 0;
            DataLookupsCache.Instance.SearchIntByID("res.vigor.v", out curVigor);
            Hotfix_LT.Data.LostMainCampaignsTemplate mainTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(CampaignId.ToString());
            if (curVigor >= (mainTpl.CostVigor * times))
            {
                return times;
            }
            else
            {
                return curVigor / mainTpl.CostVigor;
            }
        }
    }
}
