using Hotfix_LT.Data;
using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTShowItemData
    {
        public string type;
        public string id;
        public int count;
        public bool coloring;
        public bool isFromWish;
        public float multiple;

        public LTShowItemData(string id, int count, string type, bool coloring = false, float multiple = 1, bool isFromWish = false)
        {
            this.id = id;
            this.count = count;
            this.type = type;
            this.coloring = coloring;
            this.multiple = multiple;
            this.isFromWish = isFromWish;
        }

        public LTShowItemData(object obj)
        {
            type = EB.Dot.String("type", obj, string.Empty);
            id = EB.Dot.String("data", obj, string.Empty);
            count = EB.Dot.Integer("quantity", obj, 0);
            coloring = false;
            isFromWish = false;
        }
    }

    [System.Serializable]
    public class LTIconNameQuality
    {
        public string type;
        public string icon;
        public string name;
        public string quality;

        public LTIconNameQuality(string type, string icon, string name, string quality)
        {
            this.type = type;
            this.icon = icon;
            this.name = name;
            this.quality = quality;
        }
    }

    // BoolParam[0]为是否开启tip,BoolParam[1]是则描述否则来源
    public class LTShowItem : DynamicMonoHotfix
    {
        public static string MULTIPLE_FORMAT = "[00F93C]x{0}[-]";

        public DynamicUISprite Icon { get; protected set; }
        public UILabel Name { get; protected set; }
        public UILabel Count { get; protected set; }
        public UISprite Frame { get; protected set; }
        public UISprite FrameBG { get; protected set; }
        public UISprite Corner { get; protected set; }
        public UILabel GradeNum { get; protected set; }
        public DynamicUISprite EquipType { get; protected set; }
        protected UILabel DropRate { get; set; }
        protected UISprite DropRateBg { get; set; }

        //伙伴
        public GameObject HeroRoot { get; private set; }
        public UISprite CharType { get; private set; }
        public UIGrid StarGrid { get; private set; }
        public UISprite HeroRole { get; private set; }

        //特效
        private ParticleSystemUIComponent m_QualityFX;
        private EffectClip m_EffectClip;

        private ParticleSystemUIComponent m_CharFX;
        private EffectClip m_CharEffectClip;

        public int QualityLevel
        {
            get; private set;
        }

        protected LTShowItemData mLTItemData;

        public LTShowItemData LTItemData
        {
            set
            {
                mLTItemData = value;

                if (mLTItemData != null)
                {
                    LTIconNameQuality itemInfo = LTItemInfoTool.GetInfo(mLTItemData.id, mLTItemData.type, mLTItemData.coloring);
                    string quality = itemInfo.quality;
                    QualityLevel = int.Parse(quality);

                    if (Icon != null)
                    {
                        Icon.spriteName = itemInfo.icon;
                    }

                    if (Name != null)
                    {
                        Name.text = itemInfo.name;
                        if(itemInfo.type.Equals(LTShowItemType.TYPE_GAMINVENTORY) && itemInfo.quality.Equals("7"))
                        {
                            Name.gradientTop = new Color(255 / 255.0f, 255 / 255.0f, 88 / 255.0f);
                            Name.gradientBottom = new Color(255 / 255.0f, 64 / 255.0f, 255 / 255.0f);
                            Name.applyGradient = true;
                            Name.text = Name.text.Replace("[FF1C54]", "[FFFFFF]");
                        }
                        else
                        {
                            Name.applyGradient = false;
                        }
                    }

                    if (Count != null)
                    {
                        if (mLTItemData.count > 0 && mLTItemData.type != LTShowItemType.TYPE_HERO)
                        {
                            Count.text = ApplyNumFormat(mLTItemData.count);
                            Count.gameObject.CustomSetActive(true);
                        }
                        else
                        {
                            Count.gameObject.CustomSetActive(false);
                        }
                    }

                    if (Corner != null)
                    {
                        Corner.gameObject.CustomSetActive(false);
                    }

                    if (GradeNum != null)
                    {
                        GradeNum.gameObject.CustomSetActive(false);
                    }

                    if (EquipType != null)
                    {
                        EquipType.spriteName = string.Empty;
                    }

                    if (Frame != null)
                    {
                        Frame.spriteName = UIItemLvlDataLookup.LvlToStr(quality);
                    }

                    if (FrameBG != null)
                    {
                        FrameBG.spriteName = UIItemLvlDataLookup.GetItemFrameBGSprite(quality);
                        FrameBG.color = UIItemLvlDataLookup.GetItemFrameBGColor(quality);
                    }

                    if (HeroRoot != null)
                    {
                        HeroRoot.CustomSetActive(mLTItemData.type.Equals(LTShowItemType.TYPE_HERO));
                    }

                    if (DropRate != null)
                    {
                        if (mLTItemData.isFromWish)
                        {
                            DropRate.color = Color.black;
                            DropRate.text = EB.Localizer.GetString("ID_WISH");
                        }
                        else
                        {
                            DropRate.color = Color.white;

                            if (mLTItemData.multiple == 1)
                            {
                                DropRate.text = string.Empty;
                            }
                            else
                            {
                                DropRate.text = string.Format(LTShowItem.MULTIPLE_FORMAT, mLTItemData.multiple);
                            }
                        }
                    }

                    if (DropRateBg != null)
                    {
                        DropRateBg.spriteName = mLTItemData.isFromWish ? "Ty_Di_10" : "Ty_Di_9";

                        if (mLTItemData.isFromWish)
                        {
                            DropRateBg.gameObject.SetActive(true);
                        }

                        if (DropRate != null && string.IsNullOrEmpty(DropRate.text))
                        {
                            DropRateBg.gameObject.SetActive(false);
                        }
                    }

                    if (Frame != null)
                    {
                        HotfixCreateFX.ShowItemQualityFX(m_QualityFX, m_EffectClip, Frame.transform, int.Parse(quality));
                        var t_particle = Frame.transform.GetComponentInChildren<ParticleSystemUIComponent>();
                        if (t_particle!= null) t_particle.transform.localScale = Vector3.one * (Frame.width / 186f);
                    }

                    if (mLTItemData.type.Equals(LTShowItemType.TYPE_HEROSHARD))
                    {
                        if (Corner != null)
                        {
                            Corner.gameObject.CustomSetActive(true);
                        }
                    }
                    else if (mLTItemData.type.Equals(LTShowItemType.TYPE_HERO))
                    {
                        Hotfix_LT.Data.HeroInfoTemplate item = null;
                        int init_star = 1;
                        bool isCharacterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.HasHeroInfo(int.Parse(mLTItemData.id));

                        if (isCharacterid)
                        {
                            item = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(mLTItemData.id);

                            if (item != null)
                            {
                                if (CharType != null)
                                {
                                    CharType.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[item.char_type];
                                    HotfixCreateFX.ShowCharTypeFX(m_CharFX, m_CharEffectClip, CharType.transform, (PartnerGrade)item.role_grade, item.char_type);
                                }

                                init_star = item.init_star;
                            }
                        }
                        else
                        {  //这里做兼容，之前小伙伴没有品级，所有的id 都是character_id 现在id有可能有template_id
                            var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(mLTItemData.id);

                            if (temp != null)
                            {
                                item = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(temp.character_id);
                            }
                        }

                        if (item != null && CharType != null)
                        {
                            CharType.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[item.char_type];
                            HotfixCreateFX.ShowCharTypeFX(m_CharFX, m_CharEffectClip, CharType.transform, (PartnerGrade)item.role_grade, item.char_type);

                            if (StarGrid != null)
                            {
                                StarGrid.gameObject.CustomSetActive(true);

                                for (int i = 0; i < StarGrid.transform.childCount; i++)
                                {
                                    StarGrid.transform.GetChild(i).gameObject.CustomSetActive(i < item.init_star);
                                }
                                StarGrid.repositionNow = true;
                            }
                            else
                            {
                                if (HeroRole != null)
                                {
                                    HeroRole.gameObject.CustomSetActive(true);
                                    HeroRole.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)item.role_grade];
                                }
                            }
                        }
                    }
                    else if (mLTItemData.type == LTShowItemType.TYPE_PAD || mLTItemData.type == LTShowItemType.TYPE_PDB|| mLTItemData.type == LTShowItemType.TYPE_BPT || mLTItemData.type == LTShowItemType.TYPE_MCARD)
                    {
                        mDMono.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        if (mLTItemData.type.Equals(LTShowItemType.TYPE_GAMINVENTORY))
                        {
                            var General = EconemyTemplateManager.Instance.GetGeneral(mLTItemData.id);
                            if (General != null && General.System == "HeroShard")
                            {
                                if (Corner != null) { Corner.gameObject.CustomSetActive(true); }
                            }

                            if (EquipType != null)
                            {
                                Data.EconemyItemTemplate item = Data.EconemyTemplateManager.Instance.GetItem(mLTItemData.id);

                                if (item != null && (item is Data.EquipmentItemTemplate))
                                {
                                    Data.EquipmentItemTemplate temp = item as Data.EquipmentItemTemplate;
                                    EquipType.spriteName = temp.SuitIcon;
                                    EquipType.gameObject.CustomSetActive(true);
                                    if(Count!=null) Count.gameObject.CustomSetActive(false);//装备不需要显示数量
                                }

                                string suitIcon = Data.EconemyTemplateManager.Instance.GetEquipSuit(mLTItemData.id);

                                if (!string.IsNullOrEmpty(suitIcon))
                                {
                                    EquipType.spriteName = suitIcon;
                                    EquipType.gameObject.CustomSetActive(true);
                                }
                            }

                            if (GradeNum != null)
                            {
                                int grade = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGoodsGradeNum(mLTItemData.id);

                                if (grade != 0)
                                {
                                    GradeNum.text =string.Format ("+{0}", grade);
                                    GradeNum.gameObject.CustomSetActive(true);
                                }
                                else
                                {
                                    GradeNum.gameObject.CustomSetActive(false);
                                }
                            }
                        }
                    }
                }
            }

            get { return mLTItemData; }
        }

        public override void Awake()
        {
            base.Awake();
            InitToolTipEvent();
            BindingComponent();
        }

        public override void OnEnable()
        {
            //base.OnEnable();

            if (mLTItemData != null && Frame != null)
            {
                LTIconNameQuality itemInfo = LTItemInfoTool.GetInfo(mLTItemData.id, mLTItemData.type, mLTItemData.coloring);
                HotfixCreateFX.ShowItemQualityFX(m_QualityFX, m_EffectClip, Frame.transform, int.Parse(itemInfo.quality));
                var t_particle = Frame.transform.GetComponentInChildren<ParticleSystemUIComponent>();
                if (t_particle != null) t_particle.transform.localScale = Vector3.one * (Frame.width / 186f);
            }
        }

        protected virtual void BindingComponent()
        {
            var t = mDMono.transform;
            Icon = t.GetComponent<DynamicUISprite>("Icon", false);
            Name = t.GetComponent<UILabel>("Name", false);
            Count = t.GetComponent<UILabel>("Count", false);
            Frame = t.GetComponent<UISprite>("Frame", false);
            FrameBG = t.GetComponent<UISprite>("Frame/BG", false);
            Corner = t.GetComponent<UISprite>("Corner", false);
            GradeNum = t.GetComponent<UILabel>("GradeNum", false);
            EquipType = t.GetComponent<DynamicUISprite>("EquipType", false);
            DropRate = t.GetComponent<UILabel>("DropRate/Label", false);
            DropRateBg = t.GetComponent<UISprite>("DropRate", false);

            var root = t.FindEx("HeroRoot", false);

            if (root != null)
            {
                HeroRoot = root.gameObject;
                CharType = root.GetComponent<UISprite>("CharType", false);
                StarGrid = root.GetComponent<UIGrid>("StarList", false);
                HeroRole = root.GetComponent<UISprite>("HeroRole", false);
            }
        }
        
        public bool ToolTipEnabled = false;
        public bool ItemToolTipIsDesc = true;

        protected virtual void InitToolTipEvent()
        {
            var BoolParam = mDMono.BoolParamList;

            if (BoolParam != null && BoolParam.Count > 0)
            {
                ToolTipEnabled = BoolParam[0];
            }

            if (!ToolTipEnabled)
            {
                return;
            }

            if (BoolParam.Count > 1)
            {
                ItemToolTipIsDesc = BoolParam[1];
            }

            if (mDMono.gameObject.GetComponent<Collider>() == null)
            {
                NGUITools.AddWidgetCollider(mDMono.gameObject);
                Collider col = mDMono.gameObject.GetComponent<Collider>();
                if (col != null)
                {
                    BoxCollider box = col as BoxCollider;
                    if (box.size.x==0|| box.size.y == 0)
                    {
                        box.size = new Vector3(Mathf.Max(box.size.x, 185), Mathf.Max(box.size.y, 195), 0);
                    }
                }
            }

            UIEventTrigger trigger = mDMono.gameObject.GetComponent<UIEventTrigger>();

            if (trigger == null)
            {
                trigger = mDMono.gameObject.AddComponent<UIEventTrigger>();
            }

            trigger.onClick.Clear();
            trigger.onClick.Add(new EventDelegate(() =>
            {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                PopToolTip();
            }));
        }

        public void PopToolTip()
        {
            PopToolTip(mLTItemData, ItemToolTipIsDesc);
        }

        static public void PopToolTip(LTShowItemData itemData, bool isDescription = false)
        {
            if (itemData.type == LTShowItemType.TYPE_GAMINVENTORY)
            {
                Hotfix_LT.Data.EconemyItemTemplate eit = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(itemData.id);

                if (eit == null)
                {
                    return;
                }

                string system = "";

                if (eit is Hotfix_LT.Data.EquipmentItemTemplate)
                {
                    system = "Equipment";
                }
                else
                {
                    system = "Generic";
                }

                //以下判断显示物品来源Tip还是描述Tip，由物品bool ToolTipIsDesc字段决定
                if (!isDescription)
                {
                    UITooltipManager.Instance.DisplayTooltipSrc(itemData.id, system, "default");//来源
                }
                else
                {
                    LTResToolTipController.Show(itemData.type, itemData.id);//描述
                }
            }
            else if (itemData.type == LTShowItemType.TYPE_HERO)
            {
                Vector2 screenPos = UICamera.lastEventPosition;
                var ht = Johny.HashtablePool.Claim();
                ht.Add("id", itemData.id);
                ht.Add("screenPos", screenPos);
                GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
            }
            else
            {
                LTResToolTipController.Show(itemData.type, itemData.id);
            }
        }

        public void PlayFX()
        {
            if (m_QualityFX != null && m_EffectClip != null)
            {
                m_QualityFX.Play();
                m_EffectClip.Init();
            }
        }

        public void HideQualityFx()
        {
            if (m_QualityFX != null)
            {
                m_QualityFX.gameObject.CustomSetActive(false);
            }
            else
            {
                Transform trans = Frame.transform.Find("QualityFX");

                if (trans != null)
                {
                    trans.gameObject.CustomSetActive(false);
                }
            }
        }

        protected string ApplyNumFormat(int num)
        {
            if (num > 1000000000)
            {
                string str = string.Format("{0}.{1}G", num / 1000000000, ((num % 1000000000) / 100000000));
                return str;
            }
            else if (num > 1000000)
            {
                string str = string.Format("{0}.{1}M", num / 1000000, ((num % 1000000) / 100000));
                return str;
            }
            else if (num >= 1000)
            {
                string str = string.Format("{0}.{1}K", num / 1000, ((num % 1000) / 100));
                return str;
            }
            else
            {
                return num.ToString();
            }
        }

        public void SetDropRateText(string text)
        {
            if (DropRate != null)
            {
                DropRate.text = text;
            }

            if (DropRateBg != null)
            {
                DropRateBg.gameObject.SetActive(!string.IsNullOrEmpty(text));
            }
        }

		public void DisappearName()
		{
			if (Name) Name.alpha = 0;
		}
    }
}