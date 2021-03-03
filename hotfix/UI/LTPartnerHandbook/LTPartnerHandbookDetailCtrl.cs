using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTPartnerHandbookDetailCtrl : UIControllerHotfix
    {
        public override bool IsFullscreen() { return true; }

        public CampaignTextureCmp BGTexture;

        public UISprite RoleIcon;
        public UILabel TitleLabel, CurLevelLabel, NextLevelLabe;

        public UILabel ATKTitleLabel, curAddATKLabel, nextAddATKLabel,
        DefTitleLabel, curAddDefLabel, nextAddDefLabel,
        MaxHPTitleLabel, curAddMAXHPLabel, nextAddMaxHPLabel;

        public LTShowItem BreakItem;
        public UILabel BreakItemLabel;

        public BoxCollider BreakBtnBox;
        public GameObject RedPoint,TonextObj;
        public UILabel BreakBtnLabel;

        public List<HandbookCardItem> CardsItem;
        public GameObject RightRedPoint;
        public GameObject UpLevelFx;

        private Hotfix_LT.Data.eRoleAttr Type;
        private float poswidth,posheigh;
        private int currentIndex,isAllHandBookOpen = 0;//用于标识图鉴页数 isAllHandBookOpen 0未初始化，1未达成条件 2达成全图鉴解锁 
        private bool iscurrentShowGrid = true;//两个Grid轮转，默认为0显示
        private TweenPosition GridTween, Grid1Tween;
        private Vector3 leftPos, midPos, rightPos;
        private List<HandbookCardData> cardData;
        private UILabel toNext,toLast,lockDes;
        UIButton HotfixBtn;
        ConsecutiveClickCoolTrigger HotfixCoolBtn0;
        public override void Awake()
        {
            base.Awake();

            BGTexture = controller.transform.Find("BG_Texture").GetComponent<CampaignTextureCmp>();
            BGTexture.spriteName = "Game_Background_14";

            RoleIcon = controller.transform.Find("Anchor_Mid/Info/Icon").GetComponent<UISprite>(); 
            TitleLabel = controller.transform.Find("Anchor_Mid/Info/TipRoot").GetComponent<UILabel>(); 
            CurLevelLabel = controller.transform.Find("Anchor_Mid/Info/TipRoot/LevelLabel").GetComponent<UILabel>(); 
            NextLevelLabe = controller.transform.Find("Anchor_Mid/Info/TipRoot/LevelLabel (1)").GetComponent<UILabel>(); 
            ATKTitleLabel = controller.transform.Find("Anchor_Mid/Info/AttrList/0/NameLabel").GetComponent<UILabel>(); 
            curAddATKLabel = controller.transform.Find("Anchor_Mid/Info/AttrList/0/NumLabel").GetComponent<UILabel>(); 
            nextAddATKLabel = controller.transform.Find("Anchor_Mid/Info/AttrList/0/EquipNumLabel").GetComponent<UILabel>(); 
            DefTitleLabel = controller.transform.Find("Anchor_Mid/Info/AttrList/1/NameLabel").GetComponent<UILabel>();
            curAddDefLabel = controller.transform.Find("Anchor_Mid/Info/AttrList/1/NumLabel").GetComponent<UILabel>();
            nextAddDefLabel = controller.transform.Find("Anchor_Mid/Info/AttrList/1/EquipNumLabel").GetComponent<UILabel>(); 
            MaxHPTitleLabel = controller.transform.Find("Anchor_Mid/Info/AttrList/2/NameLabel").GetComponent<UILabel>();
            curAddMAXHPLabel = controller.transform.Find("Anchor_Mid/Info/AttrList/2/NumLabel").GetComponent<UILabel>(); 
            nextAddMaxHPLabel = controller.transform.Find("Anchor_Mid/Info/AttrList/2/EquipNumLabel").GetComponent<UILabel>(); 
            BreakItem = controller.transform.Find("Anchor_Mid/Info/LTShowItem").GetMonoILRComponent<LTShowItem>(); 
            BreakItemLabel = controller.transform.Find("Anchor_Mid/Info/LTShowItem/CountLabel").GetComponent<UILabel>(); 
            RedPoint = controller.transform.Find("Anchor_Mid/Info/BreakBtn/RedPoint").gameObject; 
            BreakBtnLabel = controller.transform.Find("Anchor_Mid/Info/BreakBtn/Label").GetComponent<UILabel>(); 
            UpLevelFx = controller.transform.Find("BG_Texture/Uplevel").gameObject;
            controller.backButton = controller.transform.Find("Anchor_TopLeft/CancelBtn").GetComponent<UIButton>();
            GridTween = controller.transform.Find("Anchor_Mid/Grid").GetComponent<TweenPosition>();
            Grid1Tween = controller.transform.Find("Anchor_Mid/Grid (1)").GetComponent<TweenPosition>();
            HotfixBtn = controller.transform.Find("Anchor_TopRight/Rule").GetComponent<UIButton>();
            HotfixBtn.onClick.Add(new EventDelegate(OnTipButtonClick));
            HotfixCoolBtn0 = controller.transform.Find("Anchor_Mid/Info/BreakBtn").GetComponent<ConsecutiveClickCoolTrigger>();
            HotfixCoolBtn0.clickEvent.Add(new EventDelegate(OnBreakThrouthBtnClick));
            lockDes = controller.transform.Find("Anchor_BottomRight/LockDes").GetComponent<UILabel>();
            TonextObj = controller.transform.Find("Anchor_BottomRight").gameObject;
            toNext = controller.transform.Find("Anchor_BottomRight/Label").GetComponent<UILabel>();
            RightRedPoint = controller.transform.Find("Anchor_BottomRight/RightBtn/RedPoint").gameObject;
            controller.transform.Find("Anchor_BottomRight/RightBtn").GetComponent<ConsecutiveClickCoolTrigger>().clickEvent.Add(new EventDelegate(()=>OnClickTurnthepageBtn(true)));
            toLast = controller.transform.Find("Anchor_BottomLeft/Label").GetComponent<UILabel>();
            controller.transform.Find("Anchor_BottomLeft/Label/LeftBtn").GetComponent<ConsecutiveClickCoolTrigger>().clickEvent.Add(new EventDelegate(() => OnClickTurnthepageBtn(false)));
            BreakBtnBox = HotfixCoolBtn0.GetComponent<BoxCollider>();
            MonoILRFunc();

            Vector2 v2 = GameUtils.GetPosWithConstrained(2730, 50);
            leftPos = new Vector3(-v2.x, v2.y, 0);
            midPos = new Vector3(0, v2.y, 0);
            rightPos = new Vector3(v2.x, v2.y, 0);

        }

        public override void OnDestroy()
        {
            BGTexture.spriteName = string.Empty;
            base.OnDestroy();
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Type = (Hotfix_LT.Data.eRoleAttr)param;
            RoleIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[Type]; 
            TitleLabel.text = EB.Localizer.GetString(string.Format("ID_HANDBOOK_TITLE_{0}", (int)Type));
            ATKTitleLabel.text = EB.Localizer.GetString(string.Format("ID_HANDBOOK_ATK_{0}", (int)Type));
            DefTitleLabel.text = EB.Localizer.GetString(string.Format("ID_HANDBOOK_DEF_{0}", (int)Type));
            MaxHPTitleLabel.text = EB.Localizer.GetString(string.Format("ID_HANDBOOK_MAXHP_{0}", (int)Type));
            UpLevelFx.CustomSetActive(false);
            InitHandbookPageInfo();
        }

        public override IEnumerator OnAddToStack()
        {
            isCheckItem = false;
            yield return null;
            isFristRegister = true;
            GameDataSparxManager.Instance.RegisterListener(LTPartnerHandbookManager.ListDataId, OnDataListener);
            Messenger.AddListener<PartnerTakeFieldEvent>(Hotfix_LT.EventName.PartnerTakeFieldEvent,OnPartnerTakeField);
            yield return base.OnAddToStack();
        }

        private void MonoILRFunc()
        {
            CardsItem = new List<HandbookCardItem>();
            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid/0").GetMonoILRComponent<HandbookCardItem>());
            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid/0 (1)").GetMonoILRComponent<HandbookCardItem>());
            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid/0 (2)").GetMonoILRComponent<HandbookCardItem>());
            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid/0 (3)").GetMonoILRComponent<HandbookCardItem>());
            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid/0 (4)").GetMonoILRComponent<HandbookCardItem>());

            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid (1)/0").GetMonoILRComponent<HandbookCardItem>());
            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid (1)/0 (1)").GetMonoILRComponent<HandbookCardItem>());
            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid (1)/0 (2)").GetMonoILRComponent<HandbookCardItem>());
            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid (1)/0 (3)").GetMonoILRComponent<HandbookCardItem>());
            CardsItem.Add(controller.transform.Find("Anchor_Mid/Grid (1)/0 (4)").GetMonoILRComponent<HandbookCardItem>());

            for (int i = 0; i < CardsItem.Count; ++i)
            {
                CardsItem[i].SetType((IHandBookAddAttrType)(i%5));
            }
        }

        bool isCheckItem = false;
        public override void OnFocus()
        {
            base.OnFocus();
            if (isCheckItem)
            {
                isCheckItem = false;
                CheckBreakItem();
            }
        }

        public override void OnBlur()
        {
            base.OnBlur();
            isCheckItem = true;
        }
        bool isFristRegister;
        public override IEnumerator OnRemoveFromStack()
        {
            GameDataSparxManager.Instance.UnRegisterListener(LTPartnerHandbookManager.ListDataId, OnDataListener);
            Messenger.RemoveListener<PartnerTakeFieldEvent>(Hotfix_LT.EventName.PartnerTakeFieldEvent,OnPartnerTakeField);
            ResetCardParticle();
            DestroySelf();
            yield break;
        }

        public void OnPartnerTakeField(PartnerTakeFieldEvent e)
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format("{0}{1}",
                string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerHandbookDetailController_7755"), EB.Localizer.GetString(LTPartnerHandbookManager.Instance.GetType(Type))),
                LTPartnerHandbookManager.Instance.GetAttAddNum(e.Partner, e.TakeFieldCardPos)));
        }

        private void OnDataListener(string path, INodeData data)
        {
            UpdateInfo();
        }

        private void ResetCardParticle()
        {
            for (int i = 0; i < CardsItem.Count; i++)
            {
                CardsItem[i].ResetCardParticle();
            }
        }

        private int BreakLevel;
        private void UpdateInfo()
        {
            var data = LTPartnerHandbookManager.Instance.TheHandbookList.Find(Type);
            if(data == null)
            {
                EB.Debug.LogError("LTPartnerHandbookDetailCtrl.UpdateInfo data == null");
                return;
            }
            Hotfix_LT.Data.MannualBreakTemplate curThrough = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetBreakTemplateByLevel(data.HandbookId, data.BreakLevel);
            if (curThrough == null)
            {
                EB.Debug.LogError("LTPartnerHandbookDetailCtrl.UpdateInfo curThrough == null");
                return;
            }
            BreakLevel = data.BreakLevel;
            CurLevelLabel.text = data.BreakLevel.ToString();
            curAddATKLabel.text = string.Format("{0}%", Mathf.FloorToInt(curThrough.IncATK * 100));
            curAddDefLabel.text = string.Format("{0}%", Mathf.FloorToInt(curThrough.IncDEF * 100));
            curAddMAXHPLabel.text = string.Format("{0}%", Mathf.FloorToInt((curThrough.IncMaxHp * 100)));

            Hotfix_LT.Data.MannualBreakTemplate breakThrough = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetBreakTemplateByLevel(data.HandbookId, data.BreakLevel + 1);
            if (breakThrough != null)//可升级
            {
                NextLevelLabe.gameObject.CustomSetActive(true);
                NextLevelLabe.text = (data.BreakLevel + 1).ToString();

                nextAddATKLabel.text = string.Format("+{0}%", Mathf.RoundToInt((breakThrough.IncATK - curThrough.IncATK) * 100));
                nextAddDefLabel.text = string.Format("+{0}%", Mathf.RoundToInt((breakThrough.IncDEF - curThrough.IncDEF) * 100));
                nextAddMaxHPLabel.text = string.Format("+{0}%", Mathf.RoundToInt((breakThrough.IncMaxHp - curThrough.IncMaxHp) * 100));

                BreakItem.LTItemData = new LTShowItemData(breakThrough.material_1, breakThrough.quantity_1, LTShowItemType.TYPE_GAMINVENTORY, false);
                BreakItem.mDMono.gameObject.CustomSetActive(true);

                int curCount = GameItemUtil.GetInventoryItemNum(breakThrough.material_1);
                int nextCount = breakThrough.quantity_1;
                string color = curCount < nextCount ? LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal : LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                BreakItemLabel.text = string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat + "/{2}", color, curCount, nextCount);
                RedPoint.CustomSetActive(curCount >= nextCount);
                BreakBtnBox.enabled = true;
                BreakBtnLabel.text = EB.Localizer.GetString("ID_HANDBOOK_LEVEL_UP1");
            }
            else//不可升级
            {
                NextLevelLabe.gameObject.CustomSetActive(false);
                nextAddATKLabel.text = string.Empty;
                nextAddDefLabel.text = string.Empty;
                nextAddMaxHPLabel.text = string.Empty;

                BreakItem.mDMono.gameObject.CustomSetActive(false);
                RedPoint.CustomSetActive(false);
                BreakBtnBox.enabled = false;
                BreakBtnLabel.text = EB.Localizer.GetString("ID_HAS_MAX_LEVEL");         
            }
   
            cardData = data.CardsInfo;
            SetShowHandBookData();
            SetTurnPageText();
            if (isFristRegister)
            {
                isFristRegister = false;
                return;
            }
            LTPartnerDataManager.Instance.OnDestineTypePowerChanged(Type, (prm)=>
            {
                LTFormationDataManager.OnRefreshMainTeamPower(prm);
            }, true); 
        }

        private void CheckBreakItem()
        {
            var data = LTPartnerHandbookManager.Instance.TheHandbookList.Find(Type);
            Hotfix_LT.Data.MannualBreakTemplate breakThrough = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetBreakTemplateByLevel(data.HandbookId, data.BreakLevel + 1);
            if (breakThrough != null)
            {
                int curCount = GameItemUtil.GetInventoryItemNum(breakThrough.material_1);
                int nextCount = breakThrough.quantity_1;
                string color = curCount < nextCount ? LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal : LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                BreakItemLabel.text = string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat + "/{2}", color, curCount, nextCount);
                RedPoint.CustomSetActive(curCount >= nextCount);
            }
            else
            {
                RedPoint.CustomSetActive(false);
            }
        }

        public void OnTipButtonClick()
        {
            string text = EB.Localizer.GetString("ID_HANDBOOK_RULES");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        public void OnBreakThrouthBtnClick()
        {
            //判断上限，材料//ID_HANDBOOK_HAS_MAX_LEVEL
            int curLevel = LTPartnerHandbookManager.Instance.GetHandBookLevel();
            var curHandBookInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateById(curLevel);
            if (BreakLevel >= curHandBookInfo.levelLimit)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_HANDBOOK_HAS_MAX_LEVEL"));
                return;
            }

            var data = LTPartnerHandbookManager.Instance.TheHandbookList.Find(Type);
            Hotfix_LT.Data.MannualBreakTemplate breakThrough = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetBreakTemplateByLevel(data.HandbookId, data.BreakLevel + 1);
            int curCount = GameItemUtil.GetInventoryItemNum(breakThrough.material_1);
            int nextCount = breakThrough.quantity_1;
            if (curCount < nextCount)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_HANDBOOK_BTN_TIP2"));
                return;//材料不足
            }
            LTPartnerHandbookManager.Instance.BreakThrough((int)Type, delegate {
                UpLevelFx.CustomSetActive(false);
                UpLevelFx.CustomSetActive(true);
                curAddATKLabel.GetComponent<TweenScale>().ResetToBeginning();
                curAddATKLabel.GetComponent<TweenScale>().PlayForward();
                curAddDefLabel.GetComponent<TweenScale>().ResetToBeginning();
                curAddDefLabel.GetComponent<TweenScale>().PlayForward();
                curAddMAXHPLabel.GetComponent<TweenScale>().ResetToBeginning();
                curAddMAXHPLabel.GetComponent<TweenScale>().PlayForward();
            });
        }

        #region(图鉴页数扩充相关)
        /// <summary>
        /// 初始化图鉴页数信息
        /// </summary>
        private void InitHandbookPageInfo()
        {
            currentIndex = 0;
            iscurrentShowGrid = true;
            isAllHandBookOpen = 0;
            GridTween.transform.localPosition = midPos;
            Grid1Tween.transform.localPosition = rightPos;
            SetTurnPageText();
            SetTurnPageBtnActive();
        }


        private void SetTurnPageText()
        {
            if (isAllHandBookOpen == 1 && !LTPartnerHandbookManager.isAllHandBookOpen || isAllHandBookOpen == 2 && LTPartnerHandbookManager.isAllHandBookOpen) 
                return;
            toLast.text = toLast.transform.Find("Label (1)").GetComponent<UILabel>().text = EB.Localizer.GetString("ID_HANDBOOK_TOLAST");
            if (LTPartnerHandbookManager.isAllHandBookOpen)
            {
                toNext.text = toNext.transform.Find("Label (1)").GetComponent<UILabel>().text = EB.Localizer.GetString("ID_HANDBOOK_TONEXT");               
                lockDes.gameObject.CustomSetActive(false);
                toNext.gameObject.CustomSetActive(true);
                isAllHandBookOpen = 2;

            }
            else
            {
                lockDes.text = lockDes.transform.Find("Label (1)").GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString("ID_HANDBOOK_LOCKDES"),LTPartnerHandbookManager.GetAllHandboolUnlockLevel(currentIndex+2));
                lockDes.gameObject.CustomSetActive(true);
                toNext.gameObject.CustomSetActive(false);
                isAllHandBookOpen = 1;
            }
        }
        /// <summary>
        /// 图鉴翻页,向右true
        /// </summary>
        /// <param name="isAdd"></param>
        private void OnClickTurnthepageBtn(bool isAdd)
        {
            if (isAdd)
            {
                if (!LTPartnerHandbookManager.isAllHandBookOpen)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_HANDBOOK_LOCKDES"), LTPartnerHandbookManager.GetAllHandboolUnlockLevel(currentIndex + 2)));
                    return;
                }
                if (currentIndex >= LTPartnerConfig.MAX_HANDBOOKPAGE) return;
                currentIndex++;
            }
            else
            {
                if (currentIndex <= 0) return;
                currentIndex--;
            }
            iscurrentShowGrid = !iscurrentShowGrid;
            SetShowHandBookData();
            PlayCardGridTween(isAdd);
            if (currentIndex < LTPartnerConfig.MAX_HANDBOOKPAGE) SetTurnPageText();
            SetTurnPageBtnActive();
        }


        private void SetTurnPageBtnActive()
        {
            TonextObj.CustomSetActive(LTPartnerConfig.MAX_HANDBOOKPAGE != 0 && currentIndex != LTPartnerConfig.MAX_HANDBOOKPAGE);
            toLast.gameObject.CustomSetActive(LTPartnerConfig.MAX_HANDBOOKPAGE != 0 && currentIndex != 0);
        }

        private void SetShowHandBookData()
        {
            int g = iscurrentShowGrid ? 0 : 1;
            for (int i = g * 5; i < (g+1)*5; i++)
            {
                if (cardData.Count > i)
                {
                    CardsItem[i].SetData(cardData[i]);
                }
            }
            
            RightRedPoint.SetActive(GetRedPoint());
        }

        public bool GetRedPoint()
        {
            //设置红点
            for (int i = 5; i < 10; i++)
            {
                if (cardData.Count > i)
                {
                    if (cardData[i].State == HandbookCardState.Empty && LTPartnerHandbookManager.Instance
                        .TheHandbookList.Find(cardData[i].handbookId).HasAvailableCard)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        

        //图鉴页面动画
        public void PlayCardGridTween(bool isAdd)
        {
            if(isAdd && !iscurrentShowGrid)
            {
                GridTween.from = midPos;
                GridTween.to = leftPos;
                Grid1Tween.from = rightPos;
                Grid1Tween.to = midPos;
            }else if(isAdd && iscurrentShowGrid)
            {
                Grid1Tween.from = midPos;
                Grid1Tween.to = leftPos;
                GridTween.from = rightPos;
                GridTween.to = midPos;
            }else if(!isAdd && !iscurrentShowGrid)
            {
                GridTween.from = midPos;
                GridTween.to = rightPos;
                Grid1Tween.from = leftPos ;
                Grid1Tween.to = midPos;
            }
            else if(!isAdd && iscurrentShowGrid)
            {
                Grid1Tween.from = midPos;
                Grid1Tween.to = rightPos;
                GridTween.from = leftPos;
                GridTween.to = midPos;
            }
            GridTween.ResetToBeginning();
            Grid1Tween.ResetToBeginning();
            GridTween.PlayForward();
            Grid1Tween.PlayForward();
        }
        #endregion
    }

}