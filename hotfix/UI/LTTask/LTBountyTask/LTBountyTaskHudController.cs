using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Hotfix_LT.Data;
using DG.Tweening;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 悬赏目标item数据
    /// </summary>
    [System.Serializable]
    public class MonsterShowItem
    {
        public DynamicUISprite Icon;
        public UISprite AttrLogo;
        public UILabel NameLabel;
        public UISprite GradeSprite;
    
        public Transform Trans;
    
        public GameObject InfoObj;
        public GameObject NullObj;
    
        public Transform[] AttrBG;
    }
    /// <summary>
    /// 最终奖励数据
    /// </summary>
    [System.Serializable]
    public class RewardItem
    {
        public LTShowItem ShowItem;
        public GameObject EmptyItem;
        public GameObject DoubleTip;
    }
    
    
    public class LTBountyTaskHudController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            Btn = t.GetComponent<UIButton>("Content/BottomAnchor/Btn");
            BtnLabel = t.GetComponent<UILabel>("Content/BottomAnchor/Btn/Label");
            ResidueOfferNumberLabel = t.GetComponent<UILabel>("Content/BottomAnchor/NumberLabel");
            FreeRefreshNumLabel = t.GetComponent<UILabel>("Content/FramePanel/Joystick/Tips/FreeTimes");
            RefreshCostHCLabel = t.GetComponent<UILabel>("Content/FramePanel/Joystick/Tips/CostTips/CostLabel");
            RefreshResLabel = t.GetComponent<UILabel>("Content/FramePanel/Joystick/Tips/CostResTips");
            FxLabel = t.FindEx("Content/ClipPanel/FXObj").gameObject;
            HaveReceiveTipsGO = t.FindEx("Content/FramePanel/Joystick/Tips/HaveReveive").gameObject;
            JoystickGO = t.FindEx("Content/FramePanel/Joystick/Ctrl").gameObject;
            JoystickAfterGO = t.FindEx("Content/FramePanel/Joystick/DrawJoystick").gameObject;
            SelectBtn = t.FindEx("Content/FramePanel/SelectBtnAnchors/SelectBtn").gameObject;
            
            MainTargetMonsterItems = new MonsterShowItem[3];
            MainTargetMonsterItems[0] = new MonsterShowItem();
            MainTargetMonsterItems[0].Icon = t.GetComponent<DynamicUISprite>("Content/ClipPanel/RandomGrid/1/Item/LTPlayerPortrait/Info/Icon");
            MainTargetMonsterItems[0].AttrLogo = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/1/Item/LTPlayerPortrait/Info/AttrFlag");
            MainTargetMonsterItems[0].NameLabel = t.GetComponent<UILabel>("Content/ClipPanel/RandomGrid/1/Item/LTPlayerPortrait/Info/Name");
            MainTargetMonsterItems[0].GradeSprite = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/1/Item/LTPlayerPortrait/Info/SSR");
            MainTargetMonsterItems[0].Trans =  t.FindEx("Content/ClipPanel/RandomGrid/1/Item");
            MainTargetMonsterItems[0].InfoObj = t.FindEx("Content/ClipPanel/RandomGrid/1/Item/LTPlayerPortrait/Info").gameObject;
            MainTargetMonsterItems[0].NullObj = t.FindEx("Content/ClipPanel/RandomGrid/1/Item/LTPlayerPortrait/?").gameObject;
            MainTargetMonsterItems[0].AttrBG = new Transform[3];
            MainTargetMonsterItems[0].AttrBG[0] = t.FindEx("Content/ClipPanel/RandomGrid/1/Item/LTPlayerPortrait/BG/BG_Shui");
            MainTargetMonsterItems[0].AttrBG[1] = t.FindEx("Content/ClipPanel/RandomGrid/1/Item/LTPlayerPortrait/BG/BG_Feng");
            MainTargetMonsterItems[0].AttrBG[2] = t.FindEx("Content/ClipPanel/RandomGrid/1/Item/LTPlayerPortrait/BG/BG_Huo");
            
            MainTargetMonsterItems[1] = new MonsterShowItem();
            MainTargetMonsterItems[1].Icon = t.GetComponent<DynamicUISprite>("Content/ClipPanel/RandomGrid/2/Item/LTPlayerPortrait/Info/Icon");
            MainTargetMonsterItems[1].AttrLogo = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/2/Item/LTPlayerPortrait/Info/AttrFlag");
            MainTargetMonsterItems[1].NameLabel = t.GetComponent<UILabel>("Content/ClipPanel/RandomGrid/2/Item/LTPlayerPortrait/Info/Name");
            MainTargetMonsterItems[1].GradeSprite = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/2/Item/LTPlayerPortrait/Info/SSR");
            MainTargetMonsterItems[1].Trans =  t.FindEx("Content/ClipPanel/RandomGrid/2/Item");
            MainTargetMonsterItems[1].InfoObj = t.FindEx("Content/ClipPanel/RandomGrid/2/Item/LTPlayerPortrait/Info").gameObject;
            MainTargetMonsterItems[1].NullObj = t.FindEx("Content/ClipPanel/RandomGrid/2/Item/LTPlayerPortrait/?").gameObject;
            MainTargetMonsterItems[1].AttrBG = new Transform[3];
            MainTargetMonsterItems[1].AttrBG[0] = t.FindEx("Content/ClipPanel/RandomGrid/2/Item/LTPlayerPortrait/BG/BG_Shui");
            MainTargetMonsterItems[1].AttrBG[1] = t.FindEx("Content/ClipPanel/RandomGrid/2/Item/LTPlayerPortrait/BG/BG_Feng");
            MainTargetMonsterItems[1].AttrBG[2] = t.FindEx("Content/ClipPanel/RandomGrid/2/Item/LTPlayerPortrait/BG/BG_Huo");

            MainTargetMonsterItems[2] = new MonsterShowItem();
            MainTargetMonsterItems[2].Icon = t.GetComponent<DynamicUISprite>("Content/ClipPanel/RandomGrid/3/Item/LTPlayerPortrait/Info/Icon");
            MainTargetMonsterItems[2].AttrLogo = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/3/Item/LTPlayerPortrait/Info/AttrFlag");
            MainTargetMonsterItems[2].NameLabel = t.GetComponent<UILabel>("Content/ClipPanel/RandomGrid/3/Item/LTPlayerPortrait/Info/Name");
            MainTargetMonsterItems[2].GradeSprite = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/3/Item/LTPlayerPortrait/Info/SSR");
            MainTargetMonsterItems[2].Trans =  t.FindEx("Content/ClipPanel/RandomGrid/3/Item");
            MainTargetMonsterItems[2].InfoObj = t.FindEx("Content/ClipPanel/RandomGrid/3/Item/LTPlayerPortrait/Info").gameObject;
            MainTargetMonsterItems[2].NullObj = t.FindEx("Content/ClipPanel/RandomGrid/3/Item/LTPlayerPortrait/?").gameObject;
            MainTargetMonsterItems[2].AttrBG = new Transform[3];
            MainTargetMonsterItems[2].AttrBG[0] = t.FindEx("Content/ClipPanel/RandomGrid/3/Item/LTPlayerPortrait/BG/BG_Shui");
            MainTargetMonsterItems[2].AttrBG[1] = t.FindEx("Content/ClipPanel/RandomGrid/3/Item/LTPlayerPortrait/BG/BG_Feng");
            MainTargetMonsterItems[2].AttrBG[2] = t.FindEx("Content/ClipPanel/RandomGrid/3/Item/LTPlayerPortrait/BG/BG_Huo");

            TempTargetMonsterItems = new MonsterShowItem[3];
            TempTargetMonsterItems[0] = new MonsterShowItem();
            TempTargetMonsterItems[0].Icon = t.GetComponent<DynamicUISprite>("Content/ClipPanel/RandomGrid/1/Item (1)/LTPlayerPortrait/Info/Icon");
            TempTargetMonsterItems[0].AttrLogo = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/1/Item (1)/LTPlayerPortrait/Info/AttrFlag");
            TempTargetMonsterItems[0].NameLabel = t.GetComponent<UILabel>("Content/ClipPanel/RandomGrid/1/Item (1)/LTPlayerPortrait/Info/Name");
            TempTargetMonsterItems[0].GradeSprite = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/1/Item (1)/LTPlayerPortrait/Info/SSR");
            TempTargetMonsterItems[0].Trans =  t.FindEx("Content/ClipPanel/RandomGrid/1/Item (1)");
            TempTargetMonsterItems[0].InfoObj = t.FindEx("Content/ClipPanel/RandomGrid/1/Item (1)/LTPlayerPortrait/Info").gameObject;
            TempTargetMonsterItems[0].NullObj = t.FindEx("Content/ClipPanel/RandomGrid/1/Item (1)/LTPlayerPortrait/?").gameObject;
            TempTargetMonsterItems[0].AttrBG = new Transform[3];
            TempTargetMonsterItems[0].AttrBG[0] = t.FindEx("Content/ClipPanel/RandomGrid/1/Item (1)/LTPlayerPortrait/BG/BG_Shui");
            TempTargetMonsterItems[0].AttrBG[1] = t.FindEx("Content/ClipPanel/RandomGrid/1/Item (1)/LTPlayerPortrait/BG/BG_Feng");
            TempTargetMonsterItems[0].AttrBG[2] = t.FindEx("Content/ClipPanel/RandomGrid/1/Item (1)/LTPlayerPortrait/BG/BG_Huo");
            
            TempTargetMonsterItems[1] = new MonsterShowItem();
            TempTargetMonsterItems[1].Icon = t.GetComponent<DynamicUISprite>("Content/ClipPanel/RandomGrid/2/Item (1)/LTPlayerPortrait/Info/Icon");
            TempTargetMonsterItems[1].AttrLogo = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/2/Item (1)/LTPlayerPortrait/Info/AttrFlag");
            TempTargetMonsterItems[1].NameLabel = t.GetComponent<UILabel>("Content/ClipPanel/RandomGrid/2/Item (1)/LTPlayerPortrait/Info/Name");
            TempTargetMonsterItems[1].GradeSprite = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/2/Item (1)/LTPlayerPortrait/Info/SSR");
            TempTargetMonsterItems[1].Trans =  t.FindEx("Content/ClipPanel/RandomGrid/2/Item (1)");
            TempTargetMonsterItems[1].InfoObj = t.FindEx("Content/ClipPanel/RandomGrid/2/Item (1)/LTPlayerPortrait/Info").gameObject;
            TempTargetMonsterItems[1].NullObj = t.FindEx("Content/ClipPanel/RandomGrid/2/Item (1)/LTPlayerPortrait/?").gameObject;
            TempTargetMonsterItems[1].AttrBG = new Transform[3];
            TempTargetMonsterItems[1].AttrBG[0] = t.FindEx("Content/ClipPanel/RandomGrid/2/Item (1)/LTPlayerPortrait/BG/BG_Shui");
            TempTargetMonsterItems[1].AttrBG[1] = t.FindEx("Content/ClipPanel/RandomGrid/2/Item (1)/LTPlayerPortrait/BG/BG_Feng");
            TempTargetMonsterItems[1].AttrBG[2] = t.FindEx("Content/ClipPanel/RandomGrid/2/Item (1)/LTPlayerPortrait/BG/BG_Huo");

            TempTargetMonsterItems[2] = new MonsterShowItem();
            TempTargetMonsterItems[2].Icon = t.GetComponent<DynamicUISprite>("Content/ClipPanel/RandomGrid/3/Item (1)/LTPlayerPortrait/Info/Icon");
            TempTargetMonsterItems[2].AttrLogo = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/3/Item (1)/LTPlayerPortrait/Info/AttrFlag");
            TempTargetMonsterItems[2].NameLabel = t.GetComponent<UILabel>("Content/ClipPanel/RandomGrid/3/Item (1)/LTPlayerPortrait/Info/Name");
            TempTargetMonsterItems[2].GradeSprite = t.GetComponent<UISprite>("Content/ClipPanel/RandomGrid/3/Item (1)/LTPlayerPortrait/Info/SSR");
            TempTargetMonsterItems[2].Trans =  t.FindEx("Content/ClipPanel/RandomGrid/3/Item (1)");
            TempTargetMonsterItems[2].InfoObj = t.FindEx("Content/ClipPanel/RandomGrid/3/Item (1)/LTPlayerPortrait/Info").gameObject;
            TempTargetMonsterItems[2].NullObj = t.FindEx("Content/ClipPanel/RandomGrid/3/Item (1)/LTPlayerPortrait/?").gameObject;
            TempTargetMonsterItems[2].AttrBG = new Transform[3];
            TempTargetMonsterItems[2].AttrBG[0] = t.FindEx("Content/ClipPanel/RandomGrid/3/Item (1)/LTPlayerPortrait/BG/BG_Shui");
            TempTargetMonsterItems[2].AttrBG[1] = t.FindEx("Content/ClipPanel/RandomGrid/3/Item (1)/LTPlayerPortrait/BG/BG_Feng");
            TempTargetMonsterItems[2].AttrBG[2] = t.FindEx("Content/ClipPanel/RandomGrid/3/Item (1)/LTPlayerPortrait/BG/BG_Huo");

            RewardItems = new [] {new RewardItem(), new RewardItem(), new RewardItem(), new RewardItem()};
            RewardItems[0].ShowItem = t.GetMonoILRComponent<LTShowItem>("Content/BottomAnchor/Award/Grid/LTShowItem");
            RewardItems[0].EmptyItem = t.FindEx("Content/BottomAnchor/Award/Grid/LTShowItem/EmptyItem").gameObject;
            RewardItems[0].DoubleTip = t.FindEx("Content/BottomAnchor/Award/Grid/LTShowItem/DoubleTip").gameObject;
            
            RewardItems[1].ShowItem = t.GetMonoILRComponent<LTShowItem>("Content/BottomAnchor/Award/Grid/LTShowItem (1)");
            RewardItems[1].EmptyItem = t.FindEx("Content/BottomAnchor/Award/Grid/LTShowItem (1)/EmptyItem").gameObject;
            RewardItems[1].DoubleTip = t.FindEx("Content/BottomAnchor/Award/Grid/LTShowItem (1)/DoubleTip").gameObject;
            
            RewardItems[2].ShowItem = t.GetMonoILRComponent<LTShowItem>("Content/BottomAnchor/Award/Grid/LTShowItem (2)");
            RewardItems[2].EmptyItem = t.FindEx("Content/BottomAnchor/Award/Grid/LTShowItem (2)/EmptyItem").gameObject;
            RewardItems[2].DoubleTip = t.FindEx("Content/BottomAnchor/Award/Grid/LTShowItem (2)/DoubleTip").gameObject;
            
            RewardItems[3].ShowItem = t.GetMonoILRComponent<LTShowItem>("Content/BottomAnchor/Award/Grid/LTShowItem (3)");
            RewardItems[3].EmptyItem = t.FindEx("Content/BottomAnchor/Award/Grid/LTShowItem (3)/EmptyItem").gameObject;
            RewardItems[3].DoubleTip = t.FindEx("Content/BottomAnchor/Award/Grid/LTShowItem (3)/DoubleTip").gameObject;
            
            RefreshReq = t.GetComponent<UIServerRequest>("ReqServer/Refresh");
            UIServerRequestHotFix RefreshReqHotFix=RefreshReq.transform.GetMonoILRComponent<UIServerRequestHotFix>();
            RefreshReqHotFix.response = OnRefreshResponse;
            RefreshReq.onResponse.Add(new EventDelegate(RefreshReqHotFix.mDMono,"OnFetchData"));
            
            ReceiveReq = t.GetComponent<UIServerRequest>("ReqServer/ReceiveTask");
            UIServerRequestHotFix ReceiveReqHotFix= ReceiveReq.transform.GetMonoILRComponent<UIServerRequestHotFix>();
            ReceiveReqHotFix.response = OnAcceptBountyTaskResponse;
            ReceiveReq.onResponse.Add(new EventDelegate(ReceiveReqHotFix.mDMono,"OnFetchData"));
            
            StartBattleReq = t.GetComponent<UIServerRequest>("ReqServer/StartBattle");
            GiveItemReq = t.GetComponent<UIServerRequest>("ReqServer/GiveItem");
            AngleRange = 45;
            perTagIntervalDistance = 8;
            resCountLabel = t.GetComponent<UILabel>("UIFrameBG/TopRightAnchor/NewCurrency/Table/1_Exp/Bg/Label");
            ResetJoystickDelayTime = 0.7f;
            TogetherRollTime = 1;
            InitRollSpeed = 3000;
            DecelerateSpeedCostTime = 0.7f;
            MinDecelerateSpeedLoopNum = 1;
            MaxDecelerateSpeedLoopNum = 1;
            SimulateDecelerateMoveObj = t.FindEx("Content/SimulateDecelerateMoveObj").gameObject;
           
            //CurrenBar
            t.GetComponent<UIButton>("UIFrameBG/TopLeftAnchor/CancelBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("Content/TipsLabel/RuleBtn").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Content/FramePanel/SelectBtnAnchors/SelectBtn").onClick.Add(new EventDelegate(OnSelectBtnClick));
            t.GetComponent<UIButton>("Content/BottomAnchor/Btn").onClick.Add(new EventDelegate(OnDoBtnClick));

            t.GetComponent<UIEventTrigger>("Content/FramePanel/Joystick/Ctrl/Qiu").onDragStart.Add(new EventDelegate(OnJoystickDragStart));
            //ShowItem
        }
        public override bool IsFullscreen()
        {
            return true;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.bounty_topic,
                FusionTelemetry.GamePlayData.bounty_event_id,FusionTelemetry.GamePlayData.bounty_umengId,"open");
        }

        public UIButton Btn;
        public UILabel BtnLabel;
        public UILabel ResidueOfferNumberLabel;
        public UILabel FreeRefreshNumLabel;
        public UILabel RefreshCostHCLabel;
        public UILabel RefreshResLabel;
    
        public GameObject FxLabel;
    
        public GameObject HaveReceiveTipsGO;
        public GameObject JoystickGO;
        public GameObject JoystickAfterGO;
    
        public GameObject SelectBtn;
    
        public MonsterShowItem[] MainTargetMonsterItems;
        public MonsterShowItem[] TempTargetMonsterItems;
        public RewardItem[] RewardItems;
        public UIServerRequest RefreshReq, ReceiveReq;
        public UIServerRequest StartBattleReq, GiveItemReq;
        public int AngleRange = 45;
    
        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;
    
        //static int _TaskID;
        public static int TaskID()
        {
                //if (_TaskID > 0)
                //	return _TaskID;
                Hashtable tasksData;
                DataLookupsCache.Instance.SearchDataByID<Hashtable>("tasks", out tasksData);
                var iter = tasksData.GetEnumerator();
                while (iter.MoveNext())
                {
                    if (iter.Value != null)
                    {
                        eTaskType task_type =(eTaskType) System.Enum.Parse(typeof(eTaskType), ((Hashtable) iter.Value)?["task_type"].ToString());
                        if (task_type == eTaskType.Hant)
                        {
                            return int.Parse(iter.Key.ToString());
                        }
                    }
                }
                EB.Debug.LogWarning("Not Found hant taskid");
                return 0;
        }
        
        public static eTaskType DotEnum(string name, object obj, eTaskType defaultValue) 
        {
            Hashtable value =obj as Hashtable;
            if (value[name] != null)
            {
                try
                {
                    return (eTaskType)System.Enum.Parse(typeof(eTaskType), value[name].ToString(), true);
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
        private static int curStartTime = 0;
        public static int perTagIntervalDistance = 8;
        bool mIsReceiveTask;
        int mFreeResetTimes;
        int mNextRequestTime;
    
        public override IEnumerator OnAddToStack()
        {
            JoystickDrag = false;
            mNextRequestTime = AutoRefreshingManager.Instance.GetCronRefreshExcuter("refreshTaskState").NextRequestTime;
            UpdateUI();
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.BountyTask_Select,ShowSelect);
            return base.OnAddToStack();
        }
    
        void UpdateUI()
        {
            if (TaskID() != 0)
            {
                InitData();
                ShowTargetMonster();
                ShowReward(GetRewardDatas());
                UpdateUIInfo();
                if (!mIsReceiveTask)
                {
                    TheSameFx();
                }
            }
            else
            {
                HideTargetMonster();
                ShowReward(null);
                UpdateUIInfo();
            }
        }
    
        /// <summary>
        /// 判断是否需要进入到此界面显示双倍特效
        /// </summary>
        /// <returns></returns>
        private bool FxState()
        {
            int startTime = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("tasks.{0}.start_time", TaskID()), out startTime);
            if (startTime != curStartTime)
            {
                curStartTime = startTime;
                return true;
            }
            else
            {
                return false;
            }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            FusionAudio.PostEvent("UI/New/ZhonJiang", false);
            FusionAudio.PostEvent("UI/New/LaoHuJi", false);
            // EventManager.instance.RemoveListener(eSimpleEventID.BountyTask_Select, ShowSelect);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.BountyTask_Select,ShowSelect);
            DestroySelf();
            yield break;
        }
    
        void InitData()
        {
            mMonsterItemOriginYPos = MainTargetMonsterItems[0].Trans.localPosition.y;
            mMonsterItemHight = MainTargetMonsterItems[0].Trans.parent.Find("BG").GetComponent<UIWidget>().height * 2;
            Btn.hover = Btn.pressed = Btn.GetComponent<UISprite>().color = new Color(1, 1, 1);
            Btn.isEnabled = true;
        }
    
        /// <summary>
        /// 显示悬赏目标
        /// </summary>
    	void ShowTargetMonster()
        {
            //FusionAudio.PostEvent("UI/New/ZhonJiang", true);
            mTargetMonsterArray = GetTargetMonsterIdArray();
            for (int index = 0; index < mTargetMonsterArray.Length; ++index)
            {
                if (index < MainTargetMonsterItems.Length)
                    FillTargetMonsterUIInfo(MainTargetMonsterItems[index], mTargetMonsterArray[index]);
            }
        }
        /// <summary>
        /// 隐藏悬赏目标
        /// </summary>
        void HideTargetMonster()
        {
            for (int i = 0; i < MainTargetMonsterItems.Length; i++)
            {
                MainTargetMonsterItems[i].Trans.gameObject.CustomSetActive(false);
            }
        }
    
        /// <summary>
        /// Fill悬赏目标item的相关显示
        /// </summary>
        /// <param name="item">item</param>
        /// <param name="tplId">目标的伙伴tpid</param>
        void FillTargetMonsterUIInfo(MonsterShowItem item, int tplId)
        {
            string char_id = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacteridEX(tplId.ToString());
            Hotfix_LT.Data.HeroInfoTemplate infoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(char_id);
            if (infoTpl != null)
            {
                item.Icon.spriteName = infoTpl.icon;
                item.AttrLogo.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[infoTpl.char_type]; 
                HotfixCreateFX.ShowCharTypeFX(charFx, efClip, item.AttrLogo.transform, (PartnerGrade)infoTpl.role_grade, infoTpl.char_type);
                for(int i=0;i< item.AttrBG.Length; i++)
                {
                    item.AttrBG[i].gameObject.CustomSetActive(UIBuddyShowItem.AttrToBGNum(infoTpl.char_type)==i);
                }
                item.NameLabel.text = item.NameLabel.transform.GetChild(0).GetComponent<UILabel>().text = infoTpl.name;
                item.GradeSprite.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)infoTpl.role_grade];
                item.NullObj.CustomSetActive(false);
                item.InfoObj.CustomSetActive(true);
                item.Trans.gameObject.CustomSetActive(true);
    
    
            }
            else
            {
                EB.Debug.LogError("heroInfoTpl is null for tpl_id:{0}", tplId);
            }
        }
    
        /// <summary>
        /// 奖励展示
        /// </summary>
        /// <param name="itemDatas"></param>
    	void ShowReward(List<LTShowItemData> itemDatas)
        {
            if (itemDatas != null)
            {
    
                bool isSame = IsSame();
                for (int viewIndex = 0; viewIndex < RewardItems.Length; ++viewIndex)
                {
                    if (viewIndex < itemDatas.Count)
                    {
                        RewardItems[viewIndex].EmptyItem.gameObject.CustomSetActive(false);
                        if (RewardItems[viewIndex].ShowItem.mDMono.transform.GetComponent<BoxCollider>() != null) RewardItems[viewIndex].ShowItem.mDMono.transform.GetComponent<BoxCollider>().enabled = true;
                        RewardItems[viewIndex].ShowItem.mDMono.gameObject.CustomSetActive(true);
                        RewardItems[viewIndex].ShowItem.Icon.gameObject.CustomSetActive(true);
                        RewardItems[viewIndex].ShowItem.LTItemData = itemDatas[viewIndex];
                        RewardItems[viewIndex].DoubleTip.CustomSetActive(isSame);
                    }
                    else
                    {
                        RewardItems[viewIndex].EmptyItem.gameObject.CustomSetActive(false);
                        RewardItems[viewIndex].ShowItem.mDMono.gameObject.CustomSetActive(false);
                        RewardItems[viewIndex].DoubleTip.CustomSetActive(false);
                    }
                }
            }
            else
            {
                for (int viewIndex = 0; viewIndex < RewardItems.Length; ++viewIndex)
                {
                    RewardItems[viewIndex].EmptyItem.gameObject.CustomSetActive(true);
                    RewardItems[viewIndex].ShowItem.ToolTipEnabled = false;
                    RewardItems[viewIndex].DoubleTip.CustomSetActive(false);
                    RewardItems[viewIndex].ShowItem.HideQualityFx();
                }
            }
        }
    
        /// <summary>
        /// 判断悬赏目标是否都相同
        /// </summary>
        /// <returns></returns>
        private bool IsSame()
        {
            Hotfix_LT.Data.eRoleAttr roleAttr = Hotfix_LT.Data.eRoleAttr.Feng;
            bool isFirst = true;
            bool isSame = true;
            for (int i = 0; i < mTargetMonsterArray.Length; i++)
            {
                string char_id = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacteridEX(mTargetMonsterArray[i].ToString());
                Hotfix_LT.Data.HeroInfoTemplate infoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(char_id);
                if (infoTpl != null)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        roleAttr = infoTpl.char_type;
                    }
                    else
                    {
                        isSame = roleAttr == infoTpl.char_type;
                        if (!isSame)
                        {
                            break;
                        }
                    }
                }
            }
            return isSame;
        }
    
        /// <summary>
        /// 获取悬赏目标tpid数组
        /// </summary>
        /// <returns></returns>
    	static int[] GetTargetMonsterIdArray()
        {
            if (TaskID() == 0)
            {
                return null;
            }
            string monsterIdStr;
            DataLookupsCache.Instance.SearchDataByID<string>(string.Format("tasks.{0}.event_count.kill_monster.monster_id", TaskID()), out monsterIdStr);
            string[] idArray = monsterIdStr.Split(',');
            if (idArray.Length != 3)
                EB.Debug.LogError("monster id err:{0}", monsterIdStr);
    
            int[] int_id_array = new int[idArray.Length];
            for (int index = 0; index < idArray.Length; ++index)
            {
                int int_id;
                int.TryParse(idArray[index], out int_id);
                int_id_array[index] = int_id;
            }
            return int_id_array;
        }
    
        /// <summary>
        /// 获取一个随机可悬赏目标
        /// </summary>
    	int RandomTplId
        {
            get
            {
                return Hotfix_LT.Data.TaskTemplateManager.Instance.GetMonsterPool()[Random.Range(0, Hotfix_LT.Data.TaskTemplateManager.Instance.GetMonsterPool().Length - 1)];
            }
        }
    
        /// <summary>
        /// 获取悬赏最终奖励信息
        /// </summary>
        /// <returns></returns>
    	public static List<LTShowItemData> GetRewardDatas()
        {
            ArrayList rewardDataArray;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(string.Format("tasks.{0}.rewards.", TaskID()), out rewardDataArray);
            return GameUtils.ParseAwardArr(rewardDataArray);
        }
    
        /// <summary>
        /// 相关UI显示，按钮状态，次数等
        /// </summary>
    	void UpdateUIInfo()
        {
            mIsReceiveTask = TaskSystem.GetState(TaskID().ToString()) == TaskSystem.RUNNING;
            string btnText = mIsReceiveTask ? EB.Localizer.GetString("ID_codefont_in_LTBountyTaskHudController_8566_1") : EB.Localizer.GetString("ID_codefont_in_LTBountyTaskHudController_8566_2");
            Btn.GetComponent<UISprite>().spriteName = Btn.hoverSprite = Btn.pressedSprite = Btn.disabledSprite = mIsReceiveTask ? "Ty_Button_3" : "Ty_Button_1";
            LTUIUtil.SetText(BtnLabel, btnText);
            if (!mIsReceiveTask && HantTimes >= TotalHantTimes)
            {
                Btn.hover = Btn.pressed = Btn.GetComponent<UISprite>().color = new Color(1, 0, 1);
                Btn.isEnabled = false;
            }
            LTUIUtil.SetText(ResidueOfferNumberLabel, CurHantTimes.ToString());
            HaveReceiveTipsGO.CustomSetActive(mIsReceiveTask);
            SelectBtn.CustomSetActive(!mIsReceiveTask);
            UpdataRefreshRes();
            UpdateJoystickUIInfo();
        }
        public UILabel resCountLabel;
        void UpdataRefreshRes()
        {
            resCountLabel.text = GetBountyTaskRefreshThing.ToString();
        }
        void UpdateJoystickUIInfo()
        {
            mIsReceiveTask = TaskSystem.GetState(TaskID().ToString()) == TaskSystem.RUNNING;
            if (!mIsReceiveTask)
            {
                mFreeResetTimes = FreeResetTimes;
                int leftFreeTimes = TotalFreeResetTimes - mFreeResetTimes;
                leftFreeTimes = leftFreeTimes < 0 ? 0 : leftFreeTimes;
                if (leftFreeTimes > 0)
                {
                    LTUIUtil.SetText(FreeRefreshNumLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LTBountyTaskHudController_9725"), (TotalFreeResetTimes - FreeResetTimes)));
                    RefreshCostHCLabel.transform.parent.gameObject.CustomSetActive(false);
                    FreeRefreshNumLabel.gameObject.CustomSetActive(true);
                    RefreshResLabel.gameObject.CustomSetActive(false);
                }
                else if (GetBountyTaskRefreshThing > 0)
                {
                    RefreshCostHCLabel.transform.parent.gameObject.CustomSetActive(false);
                    FreeRefreshNumLabel.gameObject.CustomSetActive(false);
                    RefreshResLabel.gameObject.CustomSetActive(true);
                }
                else
                {
                    LTUIUtil.SetText(RefreshCostHCLabel, RefreshCostHc.ToString());
                    if (BalanceResourceUtil.GetUserDiamond() < RefreshCostHc)
                        RefreshCostHCLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
                    else
                        RefreshCostHCLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
                    RefreshCostHCLabel.transform.parent.gameObject.CustomSetActive(true);
                    FreeRefreshNumLabel.gameObject.CustomSetActive(false);
                    RefreshResLabel.gameObject.CustomSetActive(false);
                }
            }
            else
            {
                FreeRefreshNumLabel.gameObject.CustomSetActive(false);
                RefreshCostHCLabel.transform.parent.gameObject.CustomSetActive(false);
                RefreshResLabel.gameObject.CustomSetActive(false);
            }
        }
    
        /// <summary>
        /// 老虎机向下遥感表现协程
        /// </summary>
        /// <returns></returns>
    	IEnumerator DelayResetJoystickState()
        {
            TweenPosition TP = JoystickAfterGO.transform.GetChild(0).GetComponent<TweenPosition>();
            TweenScale TS = JoystickAfterGO.transform.GetChild(0).GetComponent<TweenScale>();
            TP.PlayForward();
            TS.PlayForward();
            yield return new WaitForSeconds(TP.duration / 2f);
            JoystickAfterGO.transform.GetChild(1).gameObject.CustomSetActive(false);
            JoystickAfterGO.transform.GetChild(2).gameObject.CustomSetActive(true);
            yield return new WaitForSeconds(ResetJoystickDelayTime);
            TP.PlayReverse();
            TS.PlayReverse();
            yield return new WaitForSeconds(TP.duration / 2f);
            JoystickAfterGO.transform.GetChild(1).gameObject.CustomSetActive(true);
            JoystickAfterGO.transform.GetChild(2).gameObject.CustomSetActive(false);
            yield return new WaitForSeconds(TP.duration / 2f);
            JoystickGO.gameObject.CustomSetActive(true);
            JoystickAfterGO.gameObject.CustomSetActive(false);
        }
    
        public float ResetJoystickDelayTime = 0.7f;
        public float TogetherRollTime = 1;
        public float InitRollSpeed = 3000;
        public float DecelerateSpeedCostTime = 0.7f;
        public int MinDecelerateSpeedLoopNum = 1;
        public int MaxDecelerateSpeedLoopNum = 1;
        public GameObject SimulateDecelerateMoveObj;
		private Ease m_DecelerateTweenType;
        int[] mTargetMonsterArray;
        float mMonsterItemOriginYPos;
        int mMonsterItemHight;
        bool mRollAniming;
        int mDecelerateRollIndex = 0;
        Coroutine[] mConstantSpeedRollCoroutines = new Coroutine[3];
        Coroutine mCurrentDecelerateCoroutine;
        Vector3 mSimulaterOriginPosition;
        Vector3 mDecelerateObjOriginPosition;
        Vector3 mDecelerateTransitionObjOriginPosition;
        float mPreSimulatePosYValue;
        float mPreTransitionSimulatePosYValue;
        float mCurrentDecelerateTotalDistance;
    
        /// <summary>
        /// 开始摇老虎机
        /// </summary>
        /// <returns></returns>
    	IEnumerator StartRollAnimationCourtine()
        {
            SelectBtn.CustomSetActive(false);
            FillTargetMonsterUIInfo(MainTargetMonsterItems[0], mTargetMonsterArray[0]);
            for (int i = 1; i < 3; ++i)
            {
                FillTargetMonsterUIInfo(MainTargetMonsterItems[i], RandomTplId);
                mConstantSpeedRollCoroutines[i] = StartCoroutine(SetupConstantSpeedRollCoroutine(i));
            }
            yield return new WaitForSeconds(TogetherRollTime);
            SetupDecelerateSimulater();
        }
        IEnumerator SetupConstantSpeedRollCoroutine(int itemIndex)
        {
            while (true)
            {
                MainTargetMonsterItems[itemIndex].Trans.localPosition -= new Vector3(0f, InitRollSpeed * Time.deltaTime);
                float offsetY = MainTargetMonsterItems[itemIndex].Trans.localPosition.y - mMonsterItemOriginYPos;
                if (offsetY < -mMonsterItemHight)
                {
                    float yposValue = mMonsterItemOriginYPos + mMonsterItemHight + (mMonsterItemHight + offsetY);
                    MainTargetMonsterItems[itemIndex].Trans.localPosition = new Vector3(0f, yposValue);
                    FillTargetMonsterUIInfo(MainTargetMonsterItems[itemIndex], RandomTplId);
                }
    
                TempTargetMonsterItems[itemIndex].Trans.localPosition -= new Vector3(0f, InitRollSpeed * Time.deltaTime);
                offsetY = TempTargetMonsterItems[itemIndex].Trans.localPosition.y - mMonsterItemOriginYPos;
                if (offsetY < -mMonsterItemHight)
                {
                    float yposValue = mMonsterItemOriginYPos + mMonsterItemHight + (mMonsterItemHight + offsetY);
                    TempTargetMonsterItems[itemIndex].Trans.localPosition = new Vector3(0f, yposValue);
                    FillTargetMonsterUIInfo(TempTargetMonsterItems[itemIndex], RandomTplId);
                }
                yield return null;
            }
        }
        void SetupDecelerateSimulater(bool isFirst = false)
        {
            if (isFirst) mDecelerateRollIndex = 0;
            StopCoroutine(mConstantSpeedRollCoroutines[mDecelerateRollIndex]);
            float distanceDiff;
            if (MainTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition.y < 0)
                distanceDiff = 2 * mMonsterItemHight + MainTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition.y;
            else
                distanceDiff = MainTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition.y;
    
            mSimulaterOriginPosition = SimulateDecelerateMoveObj.transform.localPosition;
            mDecelerateObjOriginPosition = MainTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition;
            mDecelerateTransitionObjOriginPosition = TempTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition;
            mPreSimulatePosYValue = mDecelerateObjOriginPosition.y;
            mPreTransitionSimulatePosYValue = mDecelerateTransitionObjOriginPosition.y;
            mCurrentDecelerateTotalDistance = Random.Range(MinDecelerateSpeedLoopNum, MaxDecelerateSpeedLoopNum + 1) * mMonsterItemHight * 2 + distanceDiff;

			var option = SimulateDecelerateMoveObj.transform.DOLocalMoveY(mCurrentDecelerateTotalDistance + mSimulaterOriginPosition.y, DecelerateSpeedCostTime);
			option.SetEase(m_DecelerateTweenType);
			option.SetUpdate(true);
			option.OnComplete(delegate()
			{
				if (Mathf.Abs(MainTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition.y - mMonsterItemOriginYPos) >= 1f)
				{
					//Debug.LogError("correct position mDecelerateRollIndex="+ mDecelerateRollIndex);
					MainTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition = new Vector3(MainTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition.x, mMonsterItemOriginYPos, MainTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition.z);
				}
				if (Mathf.Abs(TempTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition.y + (mMonsterItemOriginYPos + mMonsterItemHight)) >= 1f)
				{
					//Debug.LogError("correct position mDecelerateRollIndex=" + mDecelerateRollIndex);
					TempTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition = new Vector3(TempTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition.x, -(mMonsterItemOriginYPos + mMonsterItemHight), TempTargetMonsterItems[mDecelerateRollIndex].Trans.localPosition.z);
				}
				FusionAudio.PostEvent("UI/New/ZhonJiang", true);
				mDecelerateRollIndex++;
				if (!isFirst && mDecelerateRollIndex < 3)
				{
					SetupDecelerateSimulater();
				}
				else  //all roll over
				{
					mRollAniming = false;
					SelectBtn.CustomSetActive(true);
					FusionAudio.PostEvent("UI/New/LaoHuJi", false);
					if (mCurrentDecelerateCoroutine != null)
					{
						StopCoroutine(mCurrentDecelerateCoroutine);
					}
					if (HantTimes < TotalHantTimes)
					{
						Btn.hover = Btn.pressed = Btn.GetComponent<UISprite>().color = new Color(1, 1, 1);
						Btn.isEnabled = true;
					}
					ShowReward(GetRewardDatas());
					TheSameFx();
				}
			});

            if (mCurrentDecelerateCoroutine != null)
            {
                StopCoroutine(mCurrentDecelerateCoroutine);
            }
            mCurrentDecelerateCoroutine = StartCoroutine(SetupDecelerateRollCourtine(mDecelerateRollIndex));
        }
    
        private void ShowSelect()
        {
            //判断有没修改第一个伙伴  如果重复收到事件就有可能执行两次导致无限转动
            if (!mRollAniming)
            {
                StartCoroutine(StartFirstRollAnimationCourtine());
            }
        }
    
        /// <summary>
        /// 第一个物品变化时的动画
        /// </summary>
        /// <returns></returns>
        IEnumerator StartFirstRollAnimationCourtine()
        {
            FusionAudio.PostEvent("UI/New/LaoHuJi", true);
            mRollAniming = true;
            SelectBtn.CustomSetActive(false);
            mTargetMonsterArray = GetTargetMonsterIdArray();
            //FillTargetMonsterUIInfo(MainTargetMonsterItems[0], mTargetMonsterArray[0]);
            mConstantSpeedRollCoroutines[0] = StartCoroutine(SetupConstantSpeedRollCoroutine(0));
            yield return new WaitForSeconds(TogetherRollTime);
            SetupDecelerateSimulater(true);
        }
    
        /// <summary>
        /// 双倍奖励特效的实现
        /// </summary>
        private void TheSameFx()
        {
            if (IsSame() && FxState())
            {
                FusionAudio.PostEvent("UI/New/ZhonDaJiang", true);
                StartCoroutine(TheSamePlayFx());
            }
        }
        private void CloseTheSameFx()
        {
            StopCoroutine(TheSamePlayFx());
        }
        IEnumerator TheSamePlayFx()
        {
            FxLabel.CustomSetActive(false);
            FxLabel.CustomSetActive(true);
            yield break;
        }
    
        IEnumerator SetupDecelerateRollCourtine(int itemIndex)
        {
            while (true)
            {
                yield return null;
    
                MainTargetMonsterItems[itemIndex].Trans.localPosition = GetSimulatePos(mDecelerateObjOriginPosition);
                if (MainTargetMonsterItems[itemIndex].Trans.localPosition.y > mPreSimulatePosYValue)
                {
                    Vector3 simulaterMoveDistance = SimulateDecelerateMoveObj.transform.localPosition - mSimulaterOriginPosition;
                    if (simulaterMoveDistance.y + mMonsterItemHight + 20 >= mCurrentDecelerateTotalDistance)
                    {
                        FillTargetMonsterUIInfo(MainTargetMonsterItems[itemIndex], mTargetMonsterArray[itemIndex]);
                    }
                    else
                        FillTargetMonsterUIInfo(MainTargetMonsterItems[itemIndex], RandomTplId);
                }
                mPreSimulatePosYValue = MainTargetMonsterItems[itemIndex].Trans.localPosition.y;
    
                TempTargetMonsterItems[itemIndex].Trans.localPosition = GetSimulatePos(mDecelerateTransitionObjOriginPosition);
                if (TempTargetMonsterItems[itemIndex].Trans.localPosition.y > mPreTransitionSimulatePosYValue)
                {
                    FillTargetMonsterUIInfo(TempTargetMonsterItems[itemIndex], RandomTplId);
                }
                mPreTransitionSimulatePosYValue = TempTargetMonsterItems[itemIndex].Trans.localPosition.y;
            }
        }
    
        Vector3 GetSimulatePos(Vector3 decelerateObjOriginPosition)
        {
            Vector3 simulaterMoveDistance = SimulateDecelerateMoveObj.transform.localPosition - mSimulaterOriginPosition;
    
            float posY = decelerateObjOriginPosition.y - simulaterMoveDistance.y;
            if (Mathf.Abs(posY) >= mMonsterItemHight)
            {
                if (((int)(posY / mMonsterItemHight)) % 2 == -1)
                {
                    posY = mMonsterItemHight + posY % mMonsterItemHight;
                }
                else
                {
                    posY = posY % mMonsterItemHight;
                }
            }
            return new Vector3(0f, posY);
        }
    
        private bool JoystickDrag = false;
        public void OnJoystickDragStart()
        {
            if (EB.Time.Now > mNextRequestTime)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LTBountyTaskConversationController_6504"), delegate (int result) {
                    if (result == 0)
                    {
                        controller.Close();
                    }
                });
                return;
            }
    
            if (TaskID() == 0)
            {
                return;
            }
    
            if (mIsReceiveTask)
            {
                return;
            }
    
            if (mRollAniming)
            {
                return;
            }
            Hotfix_LT.Data.eRoleAttr roleAttr = Hotfix_LT.Data.eRoleAttr.Feng;
            bool isFirst = true;
            bool isSame = true;
            for (int i = 0; i < mTargetMonsterArray.Length; i++)
            {
                string char_id = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacteridEX(mTargetMonsterArray[i].ToString());
                Hotfix_LT.Data.HeroInfoTemplate infoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(char_id);
                if (infoTpl != null)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        roleAttr = infoTpl.char_type;
                    }
                    else
                    {
                        isSame = roleAttr == infoTpl.char_type;
                        if (!isSame)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    EB.Debug.LogError("heroInfoTpl is null for tpl_id:{0}", mTargetMonsterArray[i]);
                }
            }
    
            if (isSame)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_LTBountyTaskHudController_20980"), delegate (int result)
                {
                    if (result == 0)
                        ConditionRefresh();
                });
            }
            else
            {
                ConditionRefresh();
            }
        }
    
        void ConditionRefresh()
        {
            if (mFreeResetTimes >= TotalFreeResetTimes)
            {
                if (GetBountyTaskRefreshThing > 0 || BalanceResourceUtil.GetUserDiamond() >= RefreshCostHc)
                {
                    FusionAudio.PostEvent("UI/New/HuaGan", true);
                    LaunchRefreshReq();
                }
                else
                {
                    BalanceResourceUtil.HcLessMessage();
                }
            }
            else
            {
                FusionAudio.PostEvent("UI/New/HuaGan", true);
                LaunchRefreshReq();
            }
        }
    
        int CurHC = 0;
        void LaunchRefreshReq()
        {
            FusionAudio.PostEvent("UI/New/LaoHuJi", true);
            CurHC = BalanceResourceUtil.GetUserDiamond();
            JoystickDrag = true;
            mRollAniming = true;
            mDecelerateRollIndex = 1;
            RefreshReq.parameters[0].parameter = TaskID().ToString();
            RefreshReq.SendRequest();
    
            JoystickGO.gameObject.CustomSetActive(false);
            JoystickAfterGO.gameObject.CustomSetActive(true);
            Btn.hover = Btn.pressed = Btn.GetComponent<UISprite>().color = new Color(1, 0, 1);
            Btn.isEnabled = false;
            StartCoroutine(DelayResetJoystickState());
            CloseTheSameFx();
        }
    
        public void OnJoystickDragEnd()
        {
        }
    
        public void OnDoBtnClick()
        {
            if (TaskID() == 0) return;//当悬赏任务不存在时
    
            if (EB.Time.Now > mNextRequestTime)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LTBountyTaskConversationController_6504"), delegate (int result) {
                    if (result == 0)
                    {
                        controller.Close();
                    }
                });
                return;
            }
    
            if (!mIsReceiveTask)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_LTBountyTaskHudController_22700"), delegate (int result)
                {
                    if (result == 0)
                    {
                        ReceiveReq.parameters[0].parameter = TaskID().ToString();
                        ReceiveReq.SendRequest();
                    }
                });
            }
            else
            {
                controller.Close();
                string npc_locator;
                DataLookupsCache.Instance.SearchDataByID<string>(string.Format("tasks.{0}.event_count.locator", TaskID()), out npc_locator);
                WorldMapPathManager.Instance.StartPathFindToNpc(MainLandLogic.GetInstance().CurrentSceneName, MainLandLogic.GetInstance().CurrentSceneName, string.Format("EnemySpawns_{0}", LocatorManager.Instance.GetLocator(npc_locator).name));//MainLandLogic.GetInstance().LocatorPathFinding(MainLandLogic.GetInstance().CurrentSceneName,string.Format ("EnemySpawns_{0}", LocatorManager.Instance.GetLocator(npc_locator).name));
            }
        }
    
        public void OnRefreshResponse(EB.Sparx.Response res)
        {
            if (res.sucessful)
            {
                if (CurHC - BalanceResourceUtil.GetUserDiamond() > 0) FusionTelemetry.PostBuy(((int)FusionTelemetry.UseHC.hc_bountytask).ToString(), 1, CurHC - BalanceResourceUtil.GetUserDiamond());
                mFreeResetTimes++;
                FreeResetTimes = mFreeResetTimes;
                mTargetMonsterArray = GetTargetMonsterIdArray();
                ShowReward(null);
                UpdateJoystickUIInfo();
                StartCoroutine(StartRollAnimationCourtine());
            }
            else
            {
                mRollAniming = false;
                res.CheckAndShowModal();
            }
            UpdataRefreshRes();
            JoystickDrag = false;
        }
    
        public void OnAcceptBountyTaskResponse(EB.Sparx.Response res)
        {
            if (res.sucessful)
            {
                UpdateUIInfo();
    
                SetupHantMonster();
            }
            else
            {
                res.CheckAndShowModal();
            }
        }
    
        public static Hotfix_LT.Data.HeroInfoTemplate GetFirstMonsterInfoTpl()
        {
            int[] monsterIDs = GetTargetMonsterIdArray();
            if (monsterIDs.Length > 0)
            {
                string char_id = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacteridEX(monsterIDs[0].ToString());
                Hotfix_LT.Data.HeroInfoTemplate infoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(char_id);
                return infoTpl;
            }
            return null;
        }
    
        private static Queue<Vector3> st_LoadAsync_posQueue = new Queue<Vector3>();
        private static Queue<Vector3> st_LoadAsync_lookAtQueue = new Queue<Vector3>();

        private static void OnTimerSetupHantMonster(int seq)
        {
            string scene_id;
            DataLookupsCache.Instance.SearchDataByID<string>(string.Format("tasks.{0}.event_count.scene_id", TaskID()), out scene_id);
            string npc_locator;
            DataLookupsCache.Instance.SearchDataByID<string>(string.Format("tasks.{0}.event_count.locator", TaskID()), out npc_locator);
            string firstMonsterModelName = "P501-Variant";
            if (GetFirstMonsterInfoTpl() != null)
                firstMonsterModelName = GetFirstMonsterInfoTpl().model_name;
            string func_npc_locator = "NPCSpawns_A";
            Hotfix_LT.Data.MainLandEncounterTemplate funcNpcEncounter = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandEncounter(10068);
            if (funcNpcEncounter != null)
                func_npc_locator = funcNpcEncounter.locator;
            else
                EB.Debug.LogError("funcNpcEncounter not found");
                MainLandLogic.GetInstance().PlaceOneTaskMonster(scene_id, npc_locator, firstMonsterModelName, delegate () {
                EnemyController ec = MainLandLogic.GetInstance().EnemyControllers.ContainsKey(func_npc_locator) ? MainLandLogic.GetInstance().EnemyControllers[func_npc_locator] : null;
                if (ec != null)
                {
                    SceneRootEntry sceneRoot = SceneLoadManager.GetSceneRoot(SceneLoadManager.CurrentSceneName);
                    Transform hantMonsterRoot = sceneRoot.m_SceneRoot.transform.Find("hantMonsterRoot");
                    if (hantMonsterRoot != null)
                    {
                        GameObject.Destroy(hantMonsterRoot.gameObject);
                    }
                    GameObject func_npc_locatorGO = LocatorManager.Instance.GetLocator(func_npc_locator);
                    GameObject target_locatorGO = LocatorManager.Instance.GetLocator(npc_locator);
                    Seeker seeker = ec.GetComponent<Seeker>();
                    Path p = seeker.StartPath(ec.transform.localPosition, target_locatorGO.transform.localPosition);
                    AstarPath.WaitForPath(p);
    
                    Transform navArrrowParent = null;
                    if (sceneRoot != null)
                    {
                        GameObject monsterRoot = new GameObject("hantMonsterRoot");
                        monsterRoot.transform.SetParent(sceneRoot.m_SceneRoot.transform, false);
                        monsterRoot.transform.position = new Vector3(monsterRoot.transform.position.x, monsterRoot.transform.position.y - 0.3f, monsterRoot.transform.position.z);
                        navArrrowParent = monsterRoot.transform;
                    }
                    else
                    {
                        EB.Debug.LogError("sceneRoot is null");
                    }
    
                    int currentIndex = 0;
                    float missDistance = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("BountyTaskDistance");//距离悬赏怪 X m后隐藏引导
                    p.vectorPath = GetMiddlePath(p.vectorPath);

                    #region 加载一堆怪
                    st_LoadAsync_posQueue.Clear();
                    st_LoadAsync_lookAtQueue.Clear();
                    for (int pathPointIndex = 1; pathPointIndex < p.vectorPath.Count; pathPointIndex++)
                    {
                        float distanceWithTwoPathPoint = (p.vectorPath[pathPointIndex] - p.vectorPath[currentIndex]).magnitude;
                        if (distanceWithTwoPathPoint > perTagIntervalDistance)
                        {
                            Vector3 directionNorVect = (p.vectorPath[pathPointIndex] - p.vectorPath[currentIndex]).normalized;
                            Vector3 nextPos = p.vectorPath[currentIndex];
                            float addLength = 0;
                            while (addLength <= (distanceWithTwoPathPoint - perTagIntervalDistance) 
                                   && Vector3.Distance(nextPos, p.vectorPath[p.vectorPath.Count - 1]) > missDistance)
                            {
                                st_LoadAsync_posQueue.Enqueue(nextPos);
                                st_LoadAsync_lookAtQueue.Enqueue(p.vectorPath[pathPointIndex]);
                                EB.Assets.LoadAsync("Bundles/VFX/ReticleFX/Reticle_TargetDirection", typeof(GameObject), o =>
                                {
                                    if(!o){
                                        return;
                                    }
                                    GameObject go = GameObject.Instantiate(o) as GameObject;
                                    go.transform.SetParent(navArrrowParent);
                                    go.name = "hantMonsterNavTag";
                                    go.transform.localPosition = st_LoadAsync_posQueue.Dequeue();
                                    go.transform.LookAt(st_LoadAsync_lookAtQueue.Dequeue());
                                });
                                addLength += perTagIntervalDistance;
                                nextPos += directionNorVect * perTagIntervalDistance;
                            }
                            currentIndex = pathPointIndex;
                        }
                    }
                    #endregion
                }
                else
                {
                    EB.Debug.LogError("EnemyController is null");
                }
            });
        }
    
        public static void SetupHantMonster()
        {
            GlobalMenuManager.Instance.ShowMainLandHandler(true);
            ILRTimerManager.instance.AddTimer(200, 1, OnTimerSetupHantMonster);
        }
    
        public static void DeleteMonster(string npcLocator = null)
        {
            if (npcLocator != null) MainLandLogic.GetInstance().DeleteNpc(npcLocator);
    
            SceneRootEntry sceneRoot = SceneLoadManager.GetSceneRoot(SceneLoadManager.CurrentSceneName);
            if (sceneRoot == null)
            {
                EB.Debug.LogError("sceneRoot is null");
                return;
            }
            Transform hantMonsterRoot = sceneRoot.m_SceneRoot.transform.Find("hantMonsterRoot");
            if (hantMonsterRoot != null)
            {
                GameObject.Destroy(hantMonsterRoot.gameObject);
            }
        }
    
        public void OnRuleBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_RULE_BOUNTY_TASK"));
        }
    
        public void OnSelectBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            string char_id = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacteridEX(mTargetMonsterArray[0].ToString());
            GlobalMenuManager.Instance.Open("LTBountyTaskSelectUI", int.Parse(char_id)+1);
        }
    
    
    
        #region//获取相关信息
    
        public static int RefreshCostHc
        {
            get
            {
                int cost = 10;
                if (!DataLookupsCache.Instance.SearchIntByID("userTaskConfig.refresh_cost_hc.value", out cost))
                {
                    return 10;
                }
                return cost;
            }
        }
    
        public static int TotalFreeResetTimes
        {
            get
            {
                int times = 5;
                if (!DataLookupsCache.Instance.SearchIntByID("userTaskConfig.total_free_reset_times.value", out times))
                {
                    return 5;
                }
                return times;
            }
        }
    
        public static int FreeResetTimes
        {
            get
            {
                int times = 0;
                DataLookupsCache.Instance.SearchIntByID("userTaskStatus.free_reset_times", out times);
                return times;
            }
            set
            {
                DataLookupsCache.Instance.CacheData("userTaskStatus.free_reset_times", value);
            }
        }
    
        public static int TotalHantTimes
        {
            get
            {
                int times = 5;
                if (!DataLookupsCache.Instance.SearchIntByID("userTaskStatus.day_bounty_times", out times))
                {
                    times = 5;
                }
                return times + VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.InfiniteChallengeTimes);
            }
        }
    
        public static int HantTimes
        {
            get
            {
                int times = 0;
                DataLookupsCache.Instance.SearchIntByID("userTaskStatus.bounty_times", out times);
                return times;
            }
        }
    
        public static int CurHantTimes
        {
            get
            {
                return Mathf.Max(0, TotalHantTimes - HantTimes);
            }
        }
    
        public override void OnCancelButtonClick()
        {
            if (JoystickDrag  || mRollAniming) return;
            base.OnCancelButtonClick();
        }
    
        public static int GetBountyTaskRefreshThing
        {
            get
            {
                int i = 0;
                IDictionary economys;
                if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("inventory", out economys)) return 0;
                else
                {
                    var iter = economys.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        string template_id = EB.Dot.String("economy_id", iter.Value, "");
                        if (template_id.CompareTo("2010") == 0) i = EB.Dot.Integer("num", iter.Value, 0);
                    }
                }
                return i;
            }
        }
    
        /// <summary>
        /// 悬赏取寻路点路径优化，改为取中点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static List<Vector3> GetMiddlePath(List<Vector3> path)
        {
            if (path == null || path.Count == 0)
            {
                EB.Debug.LogError("LTBountyTask Path is Null!");
                return path;
            }
            List<Vector3> newPath = new List<Vector3>();
            newPath.Add(path[0]);
            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector3 newNode = (path[i] + path[i + 1]) / 2;
                newPath.Add(newNode);
            }
            newPath.Add(path[path.Count - 1]);
            return newPath;
        }
        #endregion
    }
}
