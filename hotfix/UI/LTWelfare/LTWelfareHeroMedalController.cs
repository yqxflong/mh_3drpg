using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    public class LTWelfareHeroMedalController : DynamicMonoHotfix
    {
        public UILabel CurStageLabel;
        public UIButton BuyHeroMedalBtn;
        public UIButton RuleBtn;

        public LTWelfareHeroMedalScroll Scroll;
        
        private UILabel DayLabel;
        private UILabel TimeLabel;
        private long timeFinal;
        private bool ActivityOver;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            CurStageLabel = t.Find("BG/Title").GetComponent<UILabel >();
            BuyHeroMedalBtn = t.Find("BG/ReceiveButton").GetComponent<UIButton>();
            BuyHeroMedalBtn.onClick.Add(new EventDelegate (OnBuyHeroMedalBtnClick));
            RuleBtn = t.Find("BG/RuleBtn").GetComponent<UIButton>();
            RuleBtn.onClick.Add(new EventDelegate(OnRuleBtnClick));

            Scroll = t.GetMonoILRComponent<LTWelfareHeroMedalScroll>("ScrollView/Placeholder/Grid");
            
            DayLabel = t.Find("BG/TimeLabel/DayLabel").GetComponent<UILabel>();
            TimeLabel = t.Find("BG/TimeLabel/TimeLabel").GetComponent<UILabel>();
            
            updataUI = true;
        }

        int mTimer = 0;
        public override void Start()
        {
            Hotfix_LT.Messenger.AddListener(EventName.LTWelfareHudOpen, ChangeInitUI);
            LTWelfareEvent.WelfareOnfocus += ChangeUpdataUI;

            if (timeFinal == 0)
            {
                timeFinal = LTWelfareModel.Instance.GetHeroMedalActivityTime();
            }

            mTimer = ILRTimerManager.instance.AddTimer(1000,int.MaxValue, ShowTimeLabel);
        }

        public override void OnDestroy()
        {
            RemoveTimer();
            Hotfix_LT.Messenger.RemoveListener(EventName.LTWelfareHudOpen, ChangeInitUI);
            LTWelfareEvent.WelfareOnfocus -= ChangeUpdataUI;
            base.OnDestroy();
        }

        void RemoveTimer()
        {
            if (mTimer > 0)
            {
                mTimer = 0;
                ILRTimerManager.instance.RemoveTimer(mTimer);
            }
        }

        void ChangeInitUI()
        {
            ActivityOver = false;
            timeFinal = LTWelfareModel.Instance.GetHeroMedalActivityTime();
        }

        bool updataUI = false;
        void ChangeUpdataUI()
        {
            if (mDMono.gameObject.activeSelf)
            {
                EB.Coroutines.Run(UpdateUI());
            }
            else
            {
                updataUI = true;
            }
        }

        public override void OnEnable()
        {
            //base.OnEnable();
            if (updataUI)
            {
                updataUI = false;
                EB.Coroutines.Run(UpdateUI());
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        void  ShowTimeLabel(int timer)
        {
            if(!ActivityOver)
            {
                UpdataTimeFun();
            }
        }

        public void UpdataTimeFun()
        {
            long ts = timeFinal - EB.Time.Now;
            string colorStr = "";
            if (ts < 0)
            {
                ActivityOver = true;
                ts = 0;
                colorStr = "[ff6699]";
                RemoveTimer();

            }
            int day = (int)(ts / (24 * 60 * 60));
            DayLabel.text = string.Format("{0}{1}", colorStr, day);

            string timeStr = "";
            timeStr = (ts > 0) ? (string.Format("{0:D2}:{1:D2}:{2:D2}", (ts % (24 * 60 * 60)) / (60 * 60), (ts % (60 * 60)) / 60, ts % 60)) : "00:00:00";
            TimeLabel.text = string.Format(EB.Localizer.GetString("ID_DAY_FORMAT"), colorStr, timeStr);
        }
        
        //LTWelfareHeroMedalItem
        IEnumerator UpdateUI()
        {
            yield return null;
            //分值
            CurStageLabel.text = string.Format(EB.Localizer.GetString("ID_WELFARE_YINXIONG_SCORE"), LTWelfareModel.Instance.HeroMedalStage); 
            //购买按钮
            ResetBuyBtn();

            yield return null;
            //物品显示
            var datas = Hotfix_LT.Data.EventTemplateManager.Instance.GetAllHeroMedalTp1();
            Scroll.SetItemDatas(datas.ToArray());
        }
        
        private void ResetBuyBtn()
        {
            if (BuyHeroMedalBtn == null)
            {
                return;
            }

            BuyHeroMedalBtn.isEnabled = !LTWelfareModel.Instance.HasHeroMedal;
            var sprite = BuyHeroMedalBtn.transform.GetComponent<UISprite>();

            if (sprite != null)
            {
                sprite.color = LTWelfareModel.Instance.HasHeroMedal ? new Color(1, 0, 1) : new Color(1, 1, 1);
                sprite.spriteName = LTWelfareModel.Instance.HasHeroMedal ? "Ty_Button_2" : "Ty_Button_3";
            }
        }

        private void OnBuyHeroMedalBtnClick()
        {
            if (ActivityOver)
            {
                return;
            }
            if (!LTWelfareModel.Instance.HasHeroMedal)
            {
                EB.IAP.Item GiftItem = null;
                EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(6002, out GiftItem);
                GlobalMenuManager.Instance.Open("LTChargeStoreGiftUI", GiftItem);
            }
        }

        private void OnRuleBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_RULE_WELFARE_HEROMEDAL"));
        }
    }
}
