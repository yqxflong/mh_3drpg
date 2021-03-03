using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTDrawCardLookupController : UIControllerHotfix
    {
        public static bool m_Open;
        private LTpartnerInfoDynamicScroll URScroll;
        public LTpartnerInfoDynamicScroll SSRScroll;
        public LTpartnerInfoDynamicScroll SRScroll;
        public LTpartnerInfoDynamicScroll RScroll;
        public LTpartnerInfoDynamicScroll NScroll;
        private UIScrollView scrollview;
        private UIWidget URWidget;
        public UIWidget SSRWidget;
        public UIWidget SRWidget;
        public UIWidget RWidget;
        public UIWidget NWidget;
        public List<UILabel> ProbabilityLables;
        public UILabel TitleLabel;
        private LTShowItem[] SpeacialItemArray;
        private string[] URChipId;
        private List<Hotfix_LT.Data.HeroInfoTemplate> URList = new List<Hotfix_LT.Data.HeroInfoTemplate>();
        private List<Hotfix_LT.Data.HeroInfoTemplate> SSRList = new List<Hotfix_LT.Data.HeroInfoTemplate>();
        private List<Hotfix_LT.Data.HeroInfoTemplate> SRList = new List<Hotfix_LT.Data.HeroInfoTemplate>();
        private List<Hotfix_LT.Data.HeroInfoTemplate> RList = new List<Hotfix_LT.Data.HeroInfoTemplate>();
        private List<Hotfix_LT.Data.HeroInfoTemplate> NList = new List<Hotfix_LT.Data.HeroInfoTemplate>();
        private Hotfix_LT.Data.HeroInfoTemplate[] AllInfo;
        private List<Hotfix_LT.Data.HeroInfoTemplate> GoldDraw = new List<Hotfix_LT.Data.HeroInfoTemplate>();
        private List<Hotfix_LT.Data.HeroInfoTemplate> HcDraw = new List<Hotfix_LT.Data.HeroInfoTemplate>();
        private List<Hotfix_LT.Data.HeroInfoTemplate> WikiDraw = new List<Hotfix_LT.Data.HeroInfoTemplate>();
        private GameObject urchipSp, urchipGrid;
        private UILabel urchipprobability;

        private static DrawCardType preType = DrawCardType.none;
        private static DrawCardType m_type;

        public static DrawCardType DrawType
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
                preType = DrawCardType.none;
            }
        }

        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("Bg/Bg1/Top/CloseBtn");
            URScroll = t.GetMonoILRComponent<LTpartnerInfoDynamicScroll>("LookupScrollView/Placeholder/URGrid");
            SSRScroll = t.GetMonoILRComponent<LTpartnerInfoDynamicScroll>("LookupScrollView/Placeholder/SSRGrid");
            SRScroll = t.GetMonoILRComponent<LTpartnerInfoDynamicScroll>("LookupScrollView/Placeholder/SRGrid");
            RScroll = t.GetMonoILRComponent<LTpartnerInfoDynamicScroll>("LookupScrollView/Placeholder/RGrid");
            NScroll = t.GetMonoILRComponent<LTpartnerInfoDynamicScroll>("LookupScrollView/Placeholder/NGrid");
            URWidget = t.GetComponent<UIWidget>("LookupScrollView/Placeholder/URGradeSprite/UR_TittleBg");
            SSRWidget = t.GetComponent<UIWidget>("LookupScrollView/Placeholder/SSRGradeSprite/SSR_TittleBg");
            SRWidget = t.GetComponent<UIWidget>("LookupScrollView/Placeholder/SRGradeSprite/SR_TittleBg");
            RWidget = t.GetComponent<UIWidget>("LookupScrollView/Placeholder/RGradeSprite/R_TittleBg");
            NWidget = t.GetComponent<UIWidget>("LookupScrollView/Placeholder/NGradeSprite/N_TittleBg");
            scrollview = t.GetComponent<UIScrollView>("LookupScrollView");
            ProbabilityLables = new List<UILabel>();
            ProbabilityLables.Add(t.GetComponent<UILabel>("LookupScrollView/Placeholder/URGradeSprite/UR_TittleBg/DropRate/Label"));
            ProbabilityLables.Add(t.GetComponent<UILabel>("LookupScrollView/Placeholder/SSRGradeSprite/SSR_TittleBg/DropRate/Label"));
            ProbabilityLables.Add(t.GetComponent<UILabel>("LookupScrollView/Placeholder/SRGradeSprite/SR_TittleBg/DropRate/Label"));
            ProbabilityLables.Add(t.GetComponent<UILabel>("LookupScrollView/Placeholder/RGradeSprite/R_TittleBg/DropRate/Label"));
            ProbabilityLables.Add(t.GetComponent<UILabel>("LookupScrollView/Placeholder/NGradeSprite/N_TittleBg/DropRate/Label"));
            TitleLabel = t.GetComponent<UILabel>("Bg/Bg1/Top/Label");
            SpeacialItemArray = new LTShowItem[4];
            SpeacialItemArray[0] = t.GetMonoILRComponent<LTShowItem>("LookupScrollView/Placeholder/URChipGrid/Item/LTShowItem");
            SpeacialItemArray[1] = t.GetMonoILRComponent<LTShowItem>("LookupScrollView/Placeholder/URChipGrid/Item/LTShowItem (1)");
            SpeacialItemArray[2] = t.GetMonoILRComponent<LTShowItem>("LookupScrollView/Placeholder/URChipGrid/Item/LTShowItem (2)");
            SpeacialItemArray[3] = t.GetMonoILRComponent<LTShowItem>("LookupScrollView/Placeholder/URChipGrid/Item/LTShowItem (3)");
            urchipSp = t.GetComponent<Transform>("LookupScrollView/Placeholder/URChipGrid").gameObject;
            urchipGrid = t.GetComponent<Transform>("LookupScrollView/Placeholder/URChip").gameObject;
            urchipprobability = t.GetComponent<UILabel>("LookupScrollView/Placeholder/URChip/UR_TittleBg/DropRate/Label");
        }

        public override void OnDestroy()
        {
            preType = DrawCardType.none;
            base.OnDestroy();
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null)
            {
                Hashtable data = param as Hashtable;
                m_type = (DrawCardType)data["type"];
                if(m_type == DrawCardType.ur)
                {
                    URChipId = data["chipdata"] as string[];
                }
            }
        }

        public override IEnumerator OnAddToStack()
        {
            m_Open = true;
            //if (!LTDrawCardTypeController.m_Open) Close();
            ///百科处理
            if (m_type == DrawCardType.wiki)
            {
                TitleLabel.text = TitleLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_WIKI");
                SetDrawPartner();
            }
            else if (m_type == DrawCardType.gold)
            {
                TitleLabel.text = TitleLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_uifont_in_LTLookUpPartnerUI_Label_0");
                SetDrawPartner();
            }
            else if (m_type == DrawCardType.hc)
            {
                TitleLabel.text = TitleLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_uifont_in_LTLookUpPartnerUI_Label_0");
                SetDrawPartner();

                string mLotteryProbability = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("LotteryProbability");
                string[] mLotteryProbabilitys = mLotteryProbability.Split(',');
                if (mLotteryProbabilitys != null)
                {
                    for (int index = 0; index < mLotteryProbabilitys.Length; index++)
                    {
                        float f = 0;
                        float.TryParse(mLotteryProbabilitys[index], out f);
                        ProbabilityLables[index].text = string.Format("{0}%", f * 100);
                    };
                }
            }else if(m_type == DrawCardType.ur)
            {
                TitleLabel.text = TitleLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_DRAWCAR_URTIP");
                SetDrawPartner();
                string mUrLotteryProbability = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("UrLotteryProbability");
                string[] mUrLotteryProbabilitys = mUrLotteryProbability.Split(',');
                int index = 0; 
                if (mUrLotteryProbabilitys != null && mUrLotteryProbabilitys.Length >= ProbabilityLables.Count+1)
                {
                    for (index = 0; index < ProbabilityLables.Count; index++)
                    {
                        float f = 0;
                        float.TryParse(mUrLotteryProbabilitys[index], out f);
                        ProbabilityLables[index].text = string.Format("{0}%", f * 100);
                    };
                    float g = 0;
                    float.TryParse(mUrLotteryProbabilitys[index], out g);
                    urchipprobability.text = string.Format("{0}%", g * 100);
                }
                
            }
            //只在钻石抽奖时显示概率
            for (int index = 0; index < ProbabilityLables.Count; index++)
            {
                ProbabilityLables[index].transform.parent.gameObject.CustomSetActive(m_type == DrawCardType.hc||m_type == DrawCardType.ur);
            };

            yield return base.OnAddToStack();
        }

        private void SetDrawPartner()
        {
            if (preType != m_type)
            {
                preType = m_type;
            }
            else
            {
                return;
            }
            if(m_type == DrawCardType.ur)
            {
                int count = URChipId.Length;
                for (int u = 0; u < SpeacialItemArray.Length; u++)
                {
                    if (u < count)
                    {
                        SpeacialItemArray[u].LTItemData = new LTShowItemData(URChipId[u], 0, LTShowItemType.TYPE_HEROSHARD);
                    }
                    else
                    {
                        SpeacialItemArray[u].mDMono.gameObject.CustomSetActive(false);
                    }
                }
                urchipGrid.CustomSetActive(true);
                urchipSp.CustomSetActive(true);
            }
            else
            {
                urchipGrid.CustomSetActive(false);
                urchipSp.CustomSetActive(false);
            }
            GetShowList(DrawType);
            URScroll.SetItemDatas(URList);
            int s = URList.Count;
            if(s == 0)
            {
                URWidget.transform.parent.localScale = new Vector3(1, 0, 1);
            }
            else
            {
                URWidget.transform.parent.localScale = Vector3.one;
                URWidget.height = 260 * (s / 4 + ((s % 4 != 0) ? 1 : 0)) + 80;
            }
            SSRScroll.SetItemDatas(SSRList);
            int i = SSRList.Count;

            if (i == 0)
            {
                SSRWidget.transform.parent.localScale = new Vector3(1, 0, 1);
            }
            else
            {
                SSRWidget.transform.parent.localScale = Vector3.one;
                SSRWidget.height = 260 * (i / 4 + ((i % 4 != 0) ? 1 : 0)) + 80;
            }

            SRScroll.SetItemDatas(SRList);
            int j = SRList.Count;

            if (j == 0)
            {
                SRWidget.transform.parent.localScale = new Vector3(1, 0, 1);
            }
            else
            {
                SRWidget.transform.parent.localScale = Vector3.one;
                SRWidget.height = 260 * (j / 4 + ((j % 4 != 0) ? 1 : 0)) + 80;
            }

            RScroll.SetItemDatas(RList);
            int k = RList.Count;

            if (k == 0)
            {
                RWidget.transform.parent.localScale = new Vector3(1, 0, 1);
            }
            else
            {
                RWidget.transform.parent.localScale = Vector3.one;
                RWidget.height = 260 * (k / 4 + ((k % 4 != 0) ? 1 : 0)) + 80;
            }

            NScroll.SetItemDatas(NList);
            int m = NList.Count;

            if (m == 0)
            {
                NWidget.transform.parent.localScale = new Vector3(1, 0, 1);
            }
            else
            {
                NWidget.transform.parent.localScale = Vector3.one;
                NWidget.height = 260 * (m / 4 + ((m % 4 != 0) ? 1 : 0)) + 80;
            }
            scrollview.ResetPosition();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            m_Open = false;
            DestroySelf();
            yield break;
        }

        public override void StartBootFlash()
        {
			SetCurrentPanelAlpha(1);
            UITweener[] tweeners = controller.transform.GetComponents<UITweener>();

            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }

        private void InitBaseList()
        {
            AllInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfos();

            for (int i = 0; i < AllInfo.Length; i++)
            {
                var tempdata = AllInfo[i];
                switch (tempdata.draw)
                {
                    case 1:
                        GoldDraw.Add(tempdata);
                        break;
                    case 2:
                        HcDraw.Add(tempdata);
                        break;
                    case 3:
                        GoldDraw.Add(tempdata);
                        HcDraw.Add(tempdata);
                        break;
                    default:
                        break;
                }
                if(tempdata.isShowInWiki)
                {
                    WikiDraw.Add(tempdata);
                }
            }

        }

        private void GetShowList(DrawCardType type)
        {
            switch (type)
            {
                case DrawCardType.none:
                    break;
                case DrawCardType.gold:
                    InitDetailInfo(GoldDraw);
                    break;
                case DrawCardType.hc:
                case DrawCardType.ur:
                    InitDetailInfo(HcDraw);
                    break;
                case DrawCardType.wiki:
                    InitDetailInfo(WikiDraw);
                    break;
                default:
                    break;
            }
        }

        private void InitDetailInfo(List<Hotfix_LT.Data.HeroInfoTemplate> list)
        {
            NList.Clear();
            RList.Clear();
            SRList.Clear();
            SSRList.Clear();
            URList.Clear();
            if (list.Count <= 0)
            {
                InitBaseList();
            }

            for (int i = 0; i < list.Count; i++)
            {
                switch (list[i].role_grade)
                {
                    case 1:
                        NList.Add(list[i]);
                        break;
                    case 2:
                        RList.Add(list[i]);
                        break;
                    case 3:
                        SRList.Add(list[i]);
                        break;
                    case 4:
                        SSRList.Add(list[i]);
                        break;
                    case 5:
                        URList.Add(list[i]);
                        break;
                }
            }
        }
    }
}
