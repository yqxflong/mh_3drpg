using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public class LTNewHeroBattleHudController : UIControllerHotfix
    {
        public static int MaxLay = 10;
        public static Color[] colorList = {
            new Color(202/255f,155/255f,149/255f),
            new Color(155f/255f,188/255f,229/255f),
            new Color(255f/255f,234/255f,98/255f)
        };
        
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            ListCtrl = t.GetMonoILRComponent<LTHeroBattleListCtrl>("CenterBG");
            awardView = t.GetMonoILRComponent<LTHeroBattleAwardView>("LTHeroBattleAwardView");
            RuleBtn = t.GetComponent<UIButton>("LeftTopHold/RuleBtn");
            RewardBtn = t.GetComponent<UIButton>("RightTopHold/Reward_Button");
            ShopBtn = t.GetComponent<UIButton>("RightTopHold/Shop_Button");
            StarLabel = t.GetComponent<UILabel>("LeftTopHold/Cup/Star_Label");
            SuccProgress = t.GetComponent<UIProgressBar>("LeftTopHold/Cup/WinProgressBar");
            SuccLabel = t.GetComponent<UILabel>("LeftTopHold/Cup/WinProgressBar/Progress_Label");
            CostProgress = t.GetComponent<UIProgressBar>("LeftTopHold/Cup/CostProgressBar");
            CostLabel = t.GetComponent<UILabel>("LeftTopHold/Cup/CostProgressBar/Progress_Label");
            HasGetTip = t.FindEx("LeftTopHold/Cup/RewardRoot/HasGetTip").gameObject;
            ItemRoot = t.GetComponent<UIGrid>("LeftTopHold/Cup/RewardRoot/RewardGrid");
            ItemPrefab = t.GetMonoILRComponent<LTShowItem>("LeftTopHold/Cup/RewardRoot/RewardGrid/LTShowItem");
            UIButton backButton = t.GetComponent<UIButton>("UINormalFrameBG/CancelBtn");
            backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
            RuleBtn.onClick.Add(new EventDelegate(OnRuleBtnClick));
            RewardBtn.onClick.Add(new EventDelegate(OnRewardBtnClick));
            ShopBtn.onClick.Add(new EventDelegate(OnShopBtnClick));
            RewardItemList = new List<LTShowItem>();
            
            cupUITexture = t.GetComponent<UITexture>("LeftTopHold/Cup");
            if (controller.ObjectParamList!=null)
            {
                CupTextures = new Texture[controller.ObjectParamList.Count];
                for (int i = 0; i < controller.ObjectParamList.Count; i++)
                {
                    CupTextures[i] = (Texture)controller.ObjectParamList[i];
                }
            }

            TextPanel = t.FindEx("Text").gameObject;
            DescLabel = t.GetComponent<UILabel>("Text/TextLabel");
            t.GetComponent<UIEventTrigger>("Text/Bg").onClick.Add(new EventDelegate(() =>
            {
                TextPanel.CustomSetActive(false);
            }));
            TextPanel.gameObject.CustomSetActive(false);
            Messenger.AddListener<int>(EventName.HeroBattleUpdateUI,UpdateUI);
            Messenger.AddListener<string>(EventName.HeroBattleShowDesc,ShowDescPanel);
        }

        private void ShowDescPanel(string arg1)
        {
            Vector2 screenPos = UICamera.lastEventPosition;
            Vector3 worldPos = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(Mathf.Clamp(screenPos.x, DescLabel.width * ((float)Screen.width / (float)UIRoot.list[0].manualWidth) / 2, (Screen.width - DescLabel.width * ((float)Screen.width / (float)UIRoot.list[0].manualWidth) / 2)), screenPos.y));
            TextPanel.transform.position=new Vector3(TextPanel.transform.position.x,worldPos.y,0);
            DescLabel.text = arg1;
            TextPanel.gameObject.CustomSetActive(true);
        }


        public override bool IsFullscreen() { return true; }

        public LTHeroBattleListCtrl ListCtrl;
        public LTHeroBattleAwardView awardView;

        public UIButton RuleBtn;
        public UIButton RewardBtn;
        public UIButton ShopBtn;

        public UILabel StarLabel;

        public UIProgressBar SuccProgress;
        public UILabel SuccLabel;
        //public TweenAlpha SuccLight;

        public UIProgressBar CostProgress;
        public UILabel CostLabel;
        //public TweenAlpha CostLight;

        public GameObject HasGetTip;
        public UIGrid ItemRoot;
        public LTShowItem ItemPrefab;
        private List<LTShowItem> RewardItemList;
        
        public UITexture cupUITexture;
        public Texture[] CupTextures;

        public GameObject TextPanel;
        public UILabel DescLabel;
       

        //heroBattleMaxCost最大花费
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            UpdateUI((int)LTNewHeroBattleManager.GetInstance().CurrentType);
            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.clash_topic,
                FusionTelemetry.GamePlayData.clash_event_id,FusionTelemetry.GamePlayData.clash_umengId,"open");
        }


        public override void OnDestroy()
        {
            Messenger.RemoveListener<int>(EventName.HeroBattleUpdateUI,UpdateUI);
            Messenger.RemoveListener<string>(EventName.HeroBattleShowDesc,ShowDescPanel);
            RuleBtn.onClick.Clear();
            RewardBtn.onClick.Clear();
            ShopBtn.onClick.Clear();
            base.OnDestroy();
        }
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            ResetUI();
            DestroySelf();
            return base.OnRemoveFromStack();
        }


        #region 更新界面
        public void UpdateUI(int select)
        {
            ListCtrl.UpdateUI(select);
            cupUITexture.SetTexture(CupTextures[select]);
            RewardBtn.gameObject.CustomSetActive(select==2);
            
            int id=LTNewHeroBattleManager.GetInstance().GetCurrentFinishLayer();
            int type = (int)LTNewHeroBattleManager.GetInstance().CurrentType;
            StarLabel.gradientBottom = colorList[select];
            switch (select)
            {
                case 0:
                case 1:
                    CostProgress.gameObject.CustomSetActive(false);
                    if (type==select)
                    {
                        int num = EventTemplateManager.Instance.GetNextHeroBattleData(id).Stage-1;
                        StarLabel.text=num.ToString();
                        SuccProgress.value = (float)num / (float)MaxLay;
                        SuccLabel.text = string.Format("{0}/{1}", num, MaxLay);
                        InitNewBieReward(EventTemplateManager.Instance.GetNextHeroBattleData(id).Award);
                        HasGetTip.CustomSetActive(false);
                    }
                    else
                    {
                        StarLabel.text=MaxLay.ToString();
                        SuccProgress.value = 1;
                        SuccLabel.text = string.Format("{0}/{1}", MaxLay, MaxLay);
                        OnInitReward(null);
                        HasGetTip.CustomSetActive(true);
                    }
                    break;
                case 2:
                    CostProgress.gameObject.CustomSetActive(true);
                    LTNewHeroBattleManager.GetInstance().GetNewHeroBattleInfo(delegate (bool isSuccessful)
                    {
                        if (isSuccessful)
                        {
                            InitUI();
                        }
                    });
                    break;
            }
        }

        private void ResetUI()
        {
            StarLabel.text = SuccLabel.text = CostLabel.text = string.Empty;
            SuccProgress.value = CostProgress.value = 0;
            
            //TODO
            // for (int i = 0; i < BattleItemList.Count; i++)
            // {
            //     BattleItemList[i].Clear();
            // }
        }

        private void InitUI()
        {
            int MaxWinCount = EventTemplateManager.Instance.GetClashOfHeroesRewardTpls().Count;
            int CurWinCount = LTNewHeroBattleManager.GetInstance().NewHeroBattleCurWinCount;
            int curCost = LTNewHeroBattleManager.GetInstance().NewHeroBattleCurCost;
            int maxCost = LTNewHeroBattleManager.GetInstance().NewHeroBattleMaxCost;

            InitWinFaile(CurWinCount, MaxWinCount);
            InitReward(CurWinCount, MaxWinCount);
            InitCost(maxCost - curCost, maxCost);
            InitRewardView(CurWinCount);
        }

        private void InitWinFaile(int CurWinCount, int MaxWinCount)
        {
            StarLabel.text = CurWinCount.ToString();
            //SuccLight.ResetToBeginning();
            //SuccLight.PlayForward();
            SuccProgress.value = (float)CurWinCount / (float)MaxWinCount;
            SuccLabel.text = string.Format("{0}/{1}", CurWinCount, MaxWinCount);
        }

        private void InitNewBieReward(string args)
        {
            List<LTShowItemData> temp=  LTUIUtil.GetLTShowItemDataFromStr(args);
            OnInitReward(temp);
        }

        private void InitReward(int CurWinCount, int MaxWinCount)
        {
            List<Hotfix_LT.Data.ClashOfHeroesRewardTemplate> tpls = Hotfix_LT.Data.EventTemplateManager.Instance.GetClashOfHeroesRewardTpls();
            List<LTShowItemData> dataList = new List<LTShowItemData>();
            if (tpls != null)
            {
                for (int i = 0; i < tpls.Count; i++)
                {
                    if (tpls[i].id == Mathf.Min(CurWinCount + 1, MaxWinCount))
                    {
                        if (tpls[i].item_num1 > 0) dataList.Add(new LTShowItemData(tpls[i].item_id1, tpls[i].item_num1, tpls[i].item_type1, false));
                        if (tpls[i].item_num2 > 0) dataList.Add(new LTShowItemData(tpls[i].item_id2, tpls[i].item_num2, tpls[i].item_type2, false));
                        if (tpls[i].item_num3 > 0) dataList.Add(new LTShowItemData(tpls[i].item_id3, tpls[i].item_num3, tpls[i].item_type3, false));
                        if (tpls[i].item_num4 > 0) dataList.Add(new LTShowItemData(tpls[i].item_id4, tpls[i].item_num4, tpls[i].item_type4, false));
                    }
                }
            }

            OnInitReward(dataList);

            HasGetTip.CustomSetActive(CurWinCount == MaxWinCount);
        }

        private void OnInitReward(List<LTShowItemData> dataList)
        {
            for (int i = 0; i < RewardItemList.Count; i++)
            {
                RewardItemList[i].mDMono.gameObject.CustomSetActive(false);
            }

            if (dataList==null)
            {
                return;
            }
            
            for (int i = 0; i < dataList.Count; i++)
            {
                if (i < RewardItemList.Count)
                {
                    RewardItemList[i].LTItemData = dataList[i];
                    RewardItemList[i].mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    LTShowItem temp = GameObject.Instantiate(ItemPrefab.mDMono, ItemRoot.transform).transform.GetMonoILRComponent<LTShowItem>();
                    temp.LTItemData = dataList[i];
                    temp.mDMono.gameObject.CustomSetActive(true);
                    RewardItemList.Add(temp);
                }
            }
            ItemRoot.Reposition();
        }

        private void InitCost(int curCount, int maxCost)
        {
            //CostLight.ResetToBeginning();
            //CostLight.PlayForward();
            CostProgress.value = (float)curCount / (float)maxCost;
            CostLabel.text = string.Format("{0}/{1}", curCount, maxCost);
        }

        private List<HeroBattleLevelReward> heroBattleRewards;
        private void InitRewardView(int CurWinCount)
        {
            if (heroBattleRewards == null)
            {
                heroBattleRewards = new List<HeroBattleLevelReward>();
                List<Hotfix_LT.Data.ClashOfHeroesRewardTemplate> tpls = Hotfix_LT.Data.EventTemplateManager.Instance.GetClashOfHeroesRewardTpls();
                for (int i = 0; i < tpls.Count; i++)
                {
                    HeroBattleLevelReward reward = new HeroBattleLevelReward();
                    reward.id = tpls[i].id;
                    LTShowItemData data1 = new LTShowItemData(tpls[i].item_id1, tpls[i].item_num1, tpls[i].item_type1);
                    LTShowItemData data2 = new LTShowItemData(tpls[i].item_id2, tpls[i].item_num2, tpls[i].item_type2);
                    LTShowItemData data3 = new LTShowItemData(tpls[i].item_id3, tpls[i].item_num3, tpls[i].item_type3);
                    LTShowItemData data4 = new LTShowItemData(tpls[i].item_id4, tpls[i].item_num4, tpls[i].item_type4);
                    if (!string.IsNullOrEmpty(data1.id)) reward.listShowItemData.Add(data1);
                    if (!string.IsNullOrEmpty(data2.id)) reward.listShowItemData.Add(data2);
                    if (!string.IsNullOrEmpty(data3.id)) reward.listShowItemData.Add(data3);
                    if (!string.IsNullOrEmpty(data4.id)) reward.listShowItemData.Add(data4);
                    heroBattleRewards.Add(reward);
                }
            }
            awardView.SetData(heroBattleRewards, CurWinCount);
        }
        #endregion

        private void OnRuleBtnClick()
        {
            string text = EB.Localizer.GetString("ID_HEROBATTLE_RULE_TEXT");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        private void OnRewardBtnClick()
        {
            awardView.mDMono.gameObject.SetActive(true);
            UITweener tw = awardView.mDMono.gameObject.GetComponent<UITweener>();
            tw.ResetToBeginning();
            tw.PlayForward();
        }

        private void OnShopBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTStoreUI", "herobattle");
        }

    }
}
