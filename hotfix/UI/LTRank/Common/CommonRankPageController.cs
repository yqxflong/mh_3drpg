using System.Collections.Generic;
using EB;

namespace Hotfix_LT.UI
{
    public class CommonRankPageController<T> : RankPageController where T : CommonRankItemData
    {
        public UILabel m_localPlayerRankLabel;
        public UILabel MiddleText;
        public UILabel RightText;

        protected CommonRankGridScroll _gridScroll;
        public virtual CommonRankGridScroll m_gridScroll
        {
            get
            {
                if (_gridScroll == null)
                    _gridScroll= mDMono.transform.parent.parent.parent.parent.parent.Find("ContentView/Views/Scroll View/Placeholder/Grid").GetMonoILRComponent<CommonRankGridScroll>();
                return _gridScroll;
            }
            set { _gridScroll = value; }
        }
        public UILabel TheTopOneLevelLabel;
        public UILabel TheTopOneNameLabel;

        protected T m_localPlayerRankData;
        protected T m_firstPlayerRankData;
        protected List<T> rankdatas;
        public int m_FreshDataDeltaTime = 2;
        protected bool isDataReady;
        private static int _LastGetDataTime = 0;
        protected string defaultocalPlayerRankLabelData = "0";

        private LTRankListCtrl _lTRankListCtrl;
        public LTRankListCtrl lTRankListCtrl
        {
            get
            {
                if(_lTRankListCtrl==null)
                    _lTRankListCtrl=(mDMono.GetComponentInParent<UIControllerILR>().ilinstance as LTRankListCtrl);
                return _lTRankListCtrl;
            }
        }
        protected override bool NeedRefreshData()
        {
            if (_LastGetDataTime == 0)
            {
                _LastGetDataTime = EB.Time.Now;
                return true;
            }
            else
            {
                if (EB.Time.Now - _LastGetDataTime >= m_FreshDataDeltaTime)
                {
                    _LastGetDataTime = EB.Time.Now;
                    return true;
                }
            }
            return false;
        }


        public override void OnEnable()
        {
            InitText();
            if (!isDataReady)return;
            UpdateInfo();
        }

        protected virtual void InitText()
        {
            //玩家名称
            LTUIUtil.SetText(MiddleText, Localizer.GetString("ID_uifont_in_LTRankListHud_Name_0"));
            //玩家等级
            LTUIUtil.SetText(RightText, Localizer.GetString("ID_uifont_in_LTRankListHud_Level_1"));
        }

        public virtual void ClickTitleRefreshGrid()
        {
            
        }

        protected virtual void UpdateInfo()
        {
            if (m_firstPlayerRankData != null)
            {
                LTUIUtil.SetText(TheTopOneNameLabel, m_firstPlayerRankData.m_Name);
                LTUIUtil.SetText(TheTopOneLevelLabel, m_firstPlayerRankData.m_DrawLevel);
                TheTopOneLevelLabel.gameObject.CustomSetActive(true);
				TheTopOneLevelLabel.UpdateAnchors();
                lTRankListCtrl.OnSwithModel(m_firstPlayerRankData.m_Model);
            }
            else
            {
                LTUIUtil.SetText(TheTopOneNameLabel, EB.Localizer.GetString("ID_EMPTY_PLACE"));
                TheTopOneLevelLabel.gameObject.CustomSetActive(false);
                lTRankListCtrl.OnSwithModel("");
            }

            if (m_localPlayerRankData != null && m_localPlayerRankData.m_Rank < 100)
            {
                LTUIUtil.SetText(m_localPlayerRankLabel, string.Format("{0}.   {1}   {2}", m_localPlayerRankData.m_Rank + 1, m_localPlayerRankData.m_Name, m_localPlayerRankData.m_Parm));
            }
            else if (m_localPlayerRankData != null && m_localPlayerRankData.m_Rank >= 100)
            {
                LTUIUtil.SetText(m_localPlayerRankLabel, string.Format("{0}   {1}   {2}", EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"), m_localPlayerRankData.m_Name, m_localPlayerRankData.m_Parm));
            }
            else
            {
                LTUIUtil.SetText(m_localPlayerRankLabel, string.Format("{0}   {1}   {2}", EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"), BalanceResourceUtil.GetUserName(), defaultocalPlayerRankLabelData));
            }
        }

        public override void Awake()
        {
            base.Awake();
            m_localPlayerRankLabel = mDMono.transform.parent.parent.parent.parent.parent.Find("UINormalFrameBG/BGs/BG (1)/Rank").GetComponent<UILabel>(); 
            MiddleText = mDMono.transform.parent.parent.parent.parent.parent.Find("ContentView/Views/TitleBG/TitleText/Name").GetComponent<UILabel>(); 
            RightText = mDMono.transform.parent.parent.parent.parent.parent.Find("ContentView/Views/TitleBG/TitleText/Level").GetComponent<UILabel>(); 
            m_gridScroll = mDMono.transform.parent.parent.parent.parent.parent.Find("ContentView/Views/Scroll View/Placeholder/Grid").GetComponent<DynamicMonoILR>()._ilrObject as CommonRankGridScroll; 
            TheTopOneLevelLabel = mDMono.transform.parent.parent.parent.parent.parent.Find("ContentView/Views/TheTopOne/Panel/TheTopOne/Level").GetComponent<UILabel>(); 
            TheTopOneNameLabel = mDMono.transform.parent.parent.parent.parent.parent.Find("ContentView/Views/TheTopOne/Panel/TheTopOne/Name").GetComponent<UILabel>(); 
            m_FreshDataDeltaTime = 5;
        }
    }
}