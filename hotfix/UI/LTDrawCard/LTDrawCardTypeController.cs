using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using _HotfixScripts.Utils;


namespace Hotfix_LT.UI
{
    public class DrawCardEvent
    {
        public static Action HideFxParent;
        public static Action<bool> LockBuyBtnAction;
    }

    public class LTDrawCardTypeController : UIControllerHotfix
    {

        public static bool m_Open;
        private bool isMainView = false;
        public List<UITweener> MainTweeners;
        //type
        private bool BtnClickLimit = false;
        public List<UILabel> LeftList;//0.time;1.free;2.coin;3.stone;
        public List<UILabel> RightList;
        public enum GoldIndex
        {
            None = 0,
            Resetting,//恢复倒计时
            Reset,//恢复
            Wait,//倒计时
            Draw,//抽
        }
        public enum HCIndex
        {
            None = 0,
            Wait,//倒计时
            Draw,//抽
        }
        private GoldIndex GOLD_LabelIndex;
        private GoldIndex GOLD_CurIndex = GoldIndex.None;
        private HCIndex HC_LabelIndex;
        private HCIndex HC_CurIndex = HCIndex.None;

        //main
        private DrawCardType m_type;
        public PartnerAwakenCusumeDataLookUp DrawCardNum;
        public List<UILabel> LeftMianList;//0.time;1.free;2.coin;3.stone;
        public List<UILabel> RightMianList;//0.coin;1.stone;
        public List<UISprite> CoinSprites;
        public List<UISprite> StoneSprites;
        //private bool isNeedRefreshActTime;

        public override bool IsFullscreen()
        {
            return true;
        }

        private static LTDrawCardTypeController _Instance;

        public static LTDrawCardTypeController Instance
        {
            get => _Instance;
        }

        public override void Awake()
        {
            base.Awake();
            _Instance = this;
            Transform t = controller.transform;

            MainTweeners = new List<UITweener>();
            MainTweeners.Add(t.GetComponent<TweenAlpha>("MianHud/BottomHud"));
            MainTweeners.Add(t.GetComponent<TweenScale>("MianHud/BG"));

            LeftList = controller.FetchComponentList<UILabel>(new string[4] {
	            "TypeHud/SliverObj/TimeLabel", "TypeHud/SliverObj/FreeLabel", "TypeHud/SliverObj/CountLabel", "TypeHud/SliverObj/NumLabel"
			});
            RightList = controller.FetchComponentList<UILabel>(new string[4] {
	            "TypeHud/GoldenObj/TimeLabel", "TypeHud/GoldenObj/FreeLabel", "TypeHud/GoldenObj/CountLabel", "TypeHud/GoldenObj/NumLabel"
			});


            LeftMianList = controller.FetchComponentList<UILabel>(new string[4] {
				"MianHud/BottomHud/Buttom/LeftTimeLabel", "MianHud/BottomHud/Buttom/Table/BuyOnceBtn/FreeLabel", "MianHud/BottomHud/Buttom/Table/BuyOnceBtn/StoneLabel",
				"MianHud/BottomHud/Buttom/Table/BuyOnceBtn/CoinLabel"
			});
            RightMianList = controller.FetchComponentList<UILabel>(new string[4] {
	            "MianHud/BottomHud/Buttom/RightTimeLabel", "MianHud/BottomHud/Buttom/Table/BuyMoreBtn/FreeLabel", "MianHud/BottomHud/Buttom/Table/BuyMoreBtn/StoneLabel",
				"MianHud/BottomHud/Buttom/Table/BuyMoreBtn/CoinLabel"
			});

            CoinSprites = controller.FetchComponentList<UISprite>(new string[2] {
				"MianHud/BottomHud/Buttom/Table/BuyOnceBtn/CoinLabel/Sprite", "MianHud/BottomHud/Buttom/Table/BuyMoreBtn/CoinLabel/Sprite"
			});
            StoneSprites = controller.FetchComponentList<UISprite>(new string[2] {
	            "MianHud/BottomHud/Buttom/Table/BuyOnceBtn/StoneLabel/Sprite", "MianHud/BottomHud/Buttom/Table/BuyMoreBtn/StoneLabel/Sprite"
			});

            DrawCardNum = t.GetDataLookupILRComponent<PartnerAwakenCusumeDataLookUp>("Hud/NewCurrency/Table");
            controller.backButton = controller.transform.Find("Hud/CancelBtn").GetComponent<UIButton>();

			controller.FindAndBindingBtnEvent(new List<string>(4) {
				"TypeHud/SliverObj/SliverCard", "TypeHud/GoldenObj/GoldCard", 
                "MianHud/BottomHud/Buttom/LookUpBtn", "MianHud/BG/FX/GoldFX/Gift/BuyBtn"
			}, new List<EventDelegate>(4) {
				new EventDelegate(OnDrawSliverCardBtnClick), new EventDelegate(OnDrawGoldCardBtnClick),
				new EventDelegate(OnLookUpPartnerBtnClick), new EventDelegate(ShowGiftItem)});
            controller.transform.GetComponent<ConsecutiveClickCoolTrigger>("MianHud/BG/FX/GoldFX/TimeGift/BG").clickEvent.Add(new EventDelegate(ShowTimeGift));
            controller.FindAndBindingCoolTriggerEvent(new List<string>(2){ "MianHud/BottomHud/Buttom/Table/BuyOnceBtn", "MianHud/BottomHud/Buttom/Table/BuyMoreBtn" }, 
	            new List<EventDelegate>(){ new EventDelegate(BuyOnceBtnClick), new EventDelegate(BuyMoreBtnClick) });

			//添加抽卡数据
			DrawCardNum.InitDataList(new Dictionary<string, int>() { { LTDrawCardConfig.LOTTERY_GOLD_ID, 0 }, { LTDrawCardConfig.LOTTERY_HC_ID, 0 } });
            Vector2 screenSize = NGUITools.screenSize;
            float screenScale = 1f;

            if ((screenSize.x / screenSize.y) < (16f / 9f))
            {
                screenScale = screenSize.y / screenSize.x * (16.0f / 9.0f); // base aspect is iphone 16 : 9
            }
            else if ((screenSize.x / screenSize.y) > 2)
            {
                screenScale = screenSize.x / screenSize.y * (9.0f / 16.0f); // base aspect is iphone 16 : 9
            }

            controller.Transforms["BG"].localScale = new Vector2(screenScale, screenScale);
        }

        private int m_TimeHandler;

		public override void Show (bool isShowing)
		{
			base.Show(isShowing);
			//RegisterMonoUpdater();
			if (isShowing)
			{
				UpdateTime();
				if (m_TimeHandler == 0)
				{
					m_TimeHandler = ILRTimerManager.instance.AddTimer(1000, int.MaxValue, UpdateTime);
				}
			}
		}

		public void UpdateTime(int seq = 0)
        {
            // base.Update();
            CheakTimeFunc();

            int now = EB.Time.Now;
            if (now > lastupdate)
            {
                lastupdate = now;
                for (int i = EventBtns.Count - 1; i >= 0; i--)
                {
                    var btn = EventBtns[i];
                    if (!UpdateCountDown(btn, now))
                    {
                        ClearTimeupEventBtn(i);
                    }
                }
            }
        }

		public override void OnDisable()
		{
			ILRTimerManager.instance.RemoveTimer(m_TimeHandler);
		}

		int lastupdate = 0;

        private int nextFreeGlod;
        private int nextFreeHc;
        private int now;
        private int goldFreeTimes;
        private int resetFreeTime;
        private int goldFreeTenTimes;
        private void CheakTimeFunc()
        {
            nextFreeGlod = LTDrawCardDataManager.Instance.NextFreeGoldTime;
            nextFreeHc = LTDrawCardDataManager.Instance.NextFreeHcTime;
            now = EB.Time.Now;
            goldFreeTimes = LTDrawCardDataManager.Instance.hasGetFreeGoldTimes;
            resetFreeTime = LTDrawCardDataManager.Instance.ResetFreeGoldTime;
            goldFreeTenTimes = LTDrawCardDataManager.Instance.HasFreeGoldTenTimes;

            #region 金币抽奖显示
            //单抽
            if (LTDrawCardConfig.FreeTimes <= 0)
            {

            }
            else
            {
                if (resetFreeTime - now < 0)
                {
                    //恢复(免费)
                    LeftList[0].text = LeftList[0].transform.GetChild(0).GetComponent<UILabel>().text = LTDrawCardConfig.GOLD_FreeTimeStr;
                    GOLD_LabelIndex = GoldIndex.Reset;
                }
                else if (goldFreeTimes <= 0)
                {
                    //恢复倒计时
                    int timer = resetFreeTime - now;
                    LeftList[0].text = LeftList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextResetStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                    GOLD_LabelIndex = GoldIndex.Resetting;
                }
                else if (nextFreeGlod - now > 0)//抽倒计时
                {
                    int timer = nextFreeGlod - now;
                    LeftList[0].text = LeftList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextFreeTimeStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                    GOLD_LabelIndex = GoldIndex.Wait;
                }
                else//抽
                {
                    LeftList[0].text = LeftList[0].transform.GetChild(0).GetComponent<UILabel>().text = LTDrawCardConfig.GOLD_FreeTimeStr;
                    GOLD_LabelIndex = GoldIndex.Draw;
                }
            }
            //十连
            if (LTDrawCardConfig.FreeTenTimes <= 0)
            {

            }
            else
            {
                if (goldFreeTenTimes > 0)
                {
                    //恢复(免费)
                    LeftList[0].text = LeftList[0].transform.GetChild(0).GetComponent<UILabel>().text = LTDrawCardConfig.GOLD_FreeTimeStr;
                    GOLD_LabelIndex = GoldIndex.Reset;
                }
                //else if (goldFreeTenTimes <= 0)
                //{
                //    //恢复倒计时
                //    int timer = resetFreeTime - now;
                //    LeftList[0].text = LeftList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextFreeTimeStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                //    GOLD_LabelIndex = GoldIndex.Resetting;
                //}
                else//抽倒计时
                {
                    int timer = resetFreeTime - now + 5;
                    if (timer < 0)
                    {
                        LTDrawCardDataManager.Instance.GetLottyLoginData(delegate (bool isSucceed)
                        {
                            if (isSucceed)
                            {
                                timer = resetFreeTime - now + 5;
                            }
                        });
                    }
                    LeftList[0].text = LeftList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextFreeTimeStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                    GOLD_LabelIndex = GoldIndex.Resetting;
                }
            }


            #endregion

            #region 钻石抽奖显示
            if (nextFreeHc - now > 0)//倒计时
            {
                int timer = nextFreeHc - now;
                RightList[0].text = RightList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextFreeTimeStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                HC_LabelIndex = HCIndex.Wait;
            }
            else//抽
            {
                RightList[0].text = RightList[0].transform.GetChild(0).GetComponent<UILabel>().text = LTDrawCardConfig.HC_FreeTimeStr;
                HC_LabelIndex = HCIndex.Draw;
            }
            #endregion

            if (m_type == DrawCardType.gold)
            {
                if (LTDrawCardConfig.FreeTimes <= 0)
                {

                }
                else
                {
                    LeftMianList[0].gameObject.CustomSetActive(true);
                    RightMianList[0].gameObject.CustomSetActive(true);
                    if (resetFreeTime - now < 0)
                    {
                        //恢复(免费)
                        LeftMianList[0].text = LeftMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = LTDrawCardConfig.GOLD_FreeTimeStr;
                    }
                    else if (goldFreeTimes <= 0)
                    {
                        //恢复倒计时
                        int timer = resetFreeTime - now;
                        LeftMianList[0].text = LeftMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextResetStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                    }
                    else if (nextFreeGlod - now > 0)//抽倒计时
                    {
                        int timer = nextFreeGlod - now;
                        LeftMianList[0].text = LeftMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextFreeTimeStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                    }
                    else//抽
                    {
                        LeftMianList[0].text = LeftMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = LTDrawCardConfig.GOLD_FreeTimeStr;
                    }
                }
                if (LTDrawCardConfig.FreeTenTimes <= 0)
                {

                }
                else
                {
                    LeftMianList[0].gameObject.CustomSetActive(false);
                    RightMianList[0].gameObject.CustomSetActive(true);
                    if (goldFreeTenTimes > 0)
                    {
                        //恢复(免费)
                        RightMianList[0].text = RightMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = LTDrawCardConfig.GOLD_FreeTimeStr;
                    }
                    else
                    {
                        //恢复倒计时
                        int timer = resetFreeTime - EB.Time.Now +5;
                        if (timer < 0)
                        {
                            LTDrawCardDataManager.Instance.GetLottyLoginData(delegate (bool isSucceed)
                            {
                                if (isSucceed)
                                {
                                    timer = resetFreeTime - now + 5;
                                }
                            });
                        }
                        RightMianList[0].text = RightMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextFreeTimeStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                    }
                    //else if (nextFreeGlod - now > 0)//抽倒计时
                    //{
                    //    int timer = nextFreeGlod - now;
                    //    RightMianList[0].text = RightMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextFreeTimeStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                    //}
                    //else//抽
                    //{
                    //    RightMianList[0].text = RightMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = LTDrawCardConfig.GOLD_FreeTimeStr;
                    //}
                }

            }
            else if (m_type == DrawCardType.hc)
            {
                LeftMianList[0].gameObject.CustomSetActive(true);
                RightMianList[0].gameObject.CustomSetActive(true);
                RightMianList[0].text = RightMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_uifont_in_LTDrawCardTypeHud_RightTimeLabel_8");
                if (nextFreeHc - now > 0)//倒计时
                {
                    int timer = nextFreeHc - now;
                    LeftMianList[0].text = LeftMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString(LTDrawCardConfig.NextFreeTimeStr), timer / 3600, (timer % 3600) / 60, (timer % 60));
                }
                else//抽
                {
                    LeftMianList[0].text = LeftMianList[0].transform.GetChild(0).GetComponent<UILabel>().text = LTDrawCardConfig.HC_FreeTimeStr;
                }
            }



            if (isMainView) { UpdateMainUI(); } else { UpdateTypeUI(); }
        }

        private void UpdateTypeUI()
        {
            if (GOLD_LabelIndex == GOLD_CurIndex && HC_CurIndex == HC_LabelIndex) return;
            if (GOLD_LabelIndex != GOLD_CurIndex)
            {
                if (GOLD_LabelIndex == GoldIndex.Draw)
                {
                    LeftList[1].text = LeftList[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LTDrawCardConfig.HC_FreeLabelStr, LTDrawCardDataManager.Instance.hasGetFreeGoldTimes);
                    for (int i = 1; i <= 3; i++)
                    {
                        LeftList[i].gameObject.CustomSetActive(i == 1);
                    }
                }
                else if (GOLD_LabelIndex == GoldIndex.Reset)
                {
                    LeftList[1].text = LeftList[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LTDrawCardConfig.HC_FreeLabelStr, LTDrawCardConfig.FreeTimes);
                    for (int i = 1; i <= 3; i++)
                    {
                        LeftList[i].gameObject.CustomSetActive(i == 1);//feeeIndex
                    }
                }
                else
                {
                    LeftList[1].gameObject.CustomSetActive(false);
                    LeftList[2].text = LeftList[2].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}/1", LTDrawCardDataManager.Instance.SliverLotteryStone);
                    string colorStr = (LTDrawCardDataManager.Instance.GetPlayerGold() < LTDrawCardConfig.Once_GoldCost) ? LTDrawCardConfig.RedStrColor : LTDrawCardConfig.GreedStrColor;
                    LeftList[3].text = LeftList[3].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}", colorStr + LTDrawCardConfig.Once_GoldCost);
                    bool hasSliverStone = LTDrawCardDataManager.Instance.SliverLotteryStone >= 1;
                    LeftList[2].gameObject.CustomSetActive(hasSliverStone);
                    LeftList[3].gameObject.CustomSetActive(!hasSliverStone);
                }
                GOLD_CurIndex = GOLD_LabelIndex;
            }

            if (HC_CurIndex != HC_LabelIndex)
            {
                if (HC_LabelIndex == HCIndex.Draw)
                {
                    RightList[1].text = RightList[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LTDrawCardConfig.HC_FreeLabelStr);
                    for (int i = 1; i <= 3; i++)
                    {
                        RightList[i].gameObject.CustomSetActive(i == 1);//feeeIndex
                    }
                }
                else
                {
                    RightList[1].gameObject.CustomSetActive(false);
                    string colorStr = (LTDrawCardDataManager.Instance.GoldenLotteryStone == 0) ? LTDrawCardConfig.RedStrColor : LTDrawCardConfig.GreedStrColor;
                    RightList[2].text = RightList[2].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}/1", colorStr, LTDrawCardDataManager.Instance.GoldenLotteryStone);
                    RightList[2].gameObject.CustomSetActive(true);
                    RightList[3].gameObject.CustomSetActive(false);//钻石抽奖全转换成券购买
                }
                HC_CurIndex = HC_LabelIndex;
            }
        }

        public void UpdateMainUI()
        {
            if (m_type == DrawCardType.gold)
            {
                if (LTDrawCardConfig.FreeTenTimes <= 0 && LTDrawCardConfig.FreeTimes > 0)
                {
                    if (GOLD_LabelIndex == GOLD_CurIndex) return;
                    RightMianList[1].gameObject.CustomSetActive(false);
                    RightMianList[2].text = RightMianList[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}/9", LTDrawCardDataManager.Instance.SliverLotteryStone);
                    string colorStr1 = (LTDrawCardDataManager.Instance.GetPlayerGold() < LTDrawCardConfig.More_GoldCost) ? LTDrawCardConfig.RedStrColor : "";
                    RightMianList[3].text = RightMianList[2].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}", colorStr1 + LTDrawCardConfig.More_GoldCost);
                    bool hasMore = LTDrawCardDataManager.Instance.SliverLotteryStone >= 9;
                    RightMianList[2].gameObject.CustomSetActive(hasMore);
                    RightMianList[3].gameObject.CustomSetActive(!hasMore);

                    if (GOLD_LabelIndex == GoldIndex.Draw)
                    {
                        LeftMianList[1].text = LeftMianList[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LTDrawCardConfig.GOLD_FreeLabelStr, LTDrawCardDataManager.Instance.hasGetFreeGoldTimes);
                        for (int i = 1; i <= 3; i++)
                        {
                            LeftMianList[i].gameObject.CustomSetActive(i == 1);
                        }
                    }
                    else if (GOLD_LabelIndex == GoldIndex.Reset)
                    {
                        LeftMianList[1].text = LeftMianList[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LTDrawCardConfig.GOLD_FreeLabelStr, LTDrawCardConfig.FreeTimes);
                        for (int i = 1; i <= 3; i++)
                        {
                            LeftMianList[i].gameObject.CustomSetActive(i == 1);//feeeIndex
                        }
                    }
                    else
                    {
                        LeftMianList[1].gameObject.CustomSetActive(false);
                        LeftMianList[2].text = LeftMianList[2].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}/1", LTDrawCardDataManager.Instance.SliverLotteryStone);
                        string colorStr2 = (LTDrawCardDataManager.Instance.GetPlayerGold() < LTDrawCardConfig.Once_GoldCost) ? LTDrawCardConfig.RedStrColor : "";// LTDrawCardConfig.GreedStrColor;
                        LeftMianList[3].text = LeftMianList[3].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}", colorStr2 + LTDrawCardConfig.Once_GoldCost);
                        bool hasOne = LTDrawCardDataManager.Instance.SliverLotteryStone >= 1;
                        LeftMianList[2].gameObject.CustomSetActive(hasOne);
                        LeftMianList[3].gameObject.CustomSetActive(!hasOne);
                    }
                }
                else if (LTDrawCardConfig.FreeTimes <= 0 && LTDrawCardConfig.FreeTenTimes > 0)
                {
                    if (GOLD_LabelIndex == GOLD_CurIndex) return;
                    LeftMianList[1].gameObject.CustomSetActive(false);
                    LeftMianList[2].text = LeftMianList[2].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}/1", LTDrawCardDataManager.Instance.SliverLotteryStone);
                    string colorStr2 = (LTDrawCardDataManager.Instance.GetPlayerGold() < LTDrawCardConfig.Once_GoldCost) ? LTDrawCardConfig.RedStrColor : "";// LTDrawCardConfig.GreedStrColor;
                    LeftMianList[3].text = LeftMianList[3].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}", colorStr2 + LTDrawCardConfig.Once_GoldCost);
                    bool hasOne = LTDrawCardDataManager.Instance.SliverLotteryStone >= 1;
                    LeftMianList[2].gameObject.CustomSetActive(hasOne);
                    LeftMianList[3].gameObject.CustomSetActive(!hasOne);


                    if (GOLD_LabelIndex == GoldIndex.Draw)
                    {
                        RightMianList[1].text = RightMianList[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LTDrawCardConfig.HC_FreeLabelStr);
                        for (int i = 1; i <= 3; i++)
                        {
                            RightMianList[i].gameObject.CustomSetActive(i == 1);
                        }
                    }
                    else if (GOLD_LabelIndex == GoldIndex.Reset)
                    {
                        RightMianList[1].text = RightMianList[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LTDrawCardConfig.HC_FreeLabelStr);
                        for (int i = 1; i <= 3; i++)
                        {
                            RightMianList[i].gameObject.CustomSetActive(i == 1);//feeeIndex
                        }
                    }
                    else
                    {
                        RightMianList[1].gameObject.CustomSetActive(false);
                        RightMianList[2].gameObject.CustomSetActive(false);
                        RightMianList[2].text = RightMianList[2].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}/9", LTDrawCardDataManager.Instance.SliverLotteryStone);
                        string colorStr1 = (LTDrawCardDataManager.Instance.GetPlayerGold() < LTDrawCardConfig.More_GoldCost) ? LTDrawCardConfig.RedStrColor : "";
                        RightMianList[3].text = RightMianList[3].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}", colorStr1 + LTDrawCardConfig.More_GoldCost);
                        bool hasMore = LTDrawCardDataManager.Instance.SliverLotteryStone >= 9;
                        RightMianList[2].gameObject.CustomSetActive(hasMore);
                        RightMianList[3].gameObject.CustomSetActive(!hasMore);

                    }
                }

            }
            else
            {
                if (HC_CurIndex == HC_LabelIndex) return;
                string colorStr1 = (LTDrawCardDataManager.Instance.GoldenLotteryStone < 9) ? LTDrawCardConfig.RedStrColor : "";
                RightMianList[2].text = RightMianList[2].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}/9", colorStr1, LTDrawCardDataManager.Instance.GoldenLotteryStone);
                RightMianList[1].gameObject.CustomSetActive(false);
                RightMianList[2].gameObject.CustomSetActive(true);
                RightMianList[3].gameObject.CustomSetActive(false);

                if (HC_LabelIndex == HCIndex.Draw)
                {
                    LeftMianList[1].text = LeftMianList[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LTDrawCardConfig.HC_FreeLabelStr);
                    for (int i = 1; i <= 3; i++)
                    {
                        LeftMianList[i].gameObject.CustomSetActive(i == 1);
                    }
                }
                else
                {
                    LeftMianList[1].gameObject.CustomSetActive(false);
                    string colorStr2 = (LTDrawCardDataManager.Instance.GoldenLotteryStone < 1) ? LTDrawCardConfig.RedStrColor : "";
                    LeftMianList[2].text = LeftMianList[2].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}/1", colorStr2, LTDrawCardDataManager.Instance.GoldenLotteryStone);

                    LeftMianList[2].gameObject.CustomSetActive(true);
                    LeftMianList[3].gameObject.CustomSetActive(false);
                }
            }
        }


        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            if (param == null)
            {
                return;
            }

            if (param.GetType() == typeof(string))
            {
                m_type = DrawCardType.hc;
                currentEventTag = (string)param;
                return;
            }
            currentEventTag = string.Empty;

            if ((DrawCardType)param == DrawCardType.gold)
            {
                m_type = DrawCardType.gold;
            }
            else if ((DrawCardType)param == DrawCardType.hc)
            {
                m_type = DrawCardType.hc;
            }
        }

        public override IEnumerator OnAddToStack()
        {
            m_Open = true;
            //isNeedRefreshActTime = false;
            UIBroadCastMessageController.Instance.mDMono.transform.GetComponentEx<UIWidget>().alpha = 0;
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 1f);
            yield return base.OnAddToStack();
            yield return null;
            if (m_type != DrawCardType.none)
            {
                ShowMainHudFunc();
            }
            CheakTimeFunc();
            yield return null;
            DrawCardEvent.HideFxParent += HideFxParent;
            DrawCardEvent.LockBuyBtnAction += LockBuyBtnAction;

            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.drawcardgold, SetGoldRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.drawcardhc, SetHCRP);
        }

        public void SetGoldRP(RedPointNode node)
        {
            controller.GObjects["SliverRP"] .CustomSetActive(node.num > 0);
        }
        public void SetHCRP(RedPointNode node)
        {
            controller.GObjects["GoldenRP"].CustomSetActive(node.num > 0);
        }

        public override IEnumerator OnRemoveFromStack()
        {
            m_Open = false;
            //if (isNeedRefreshActTime)
            //{
            //    ActivityUtil.ResetRankRefreshRecord((int)ActivityUtil.ActivityRankType.URPartnerRank);
            //}
            UIBroadCastMessageController.Instance.mDMono.transform.GetComponentEx<UIWidget>().alpha = 1;
            DrawCardEvent.HideFxParent -= HideFxParent;
            DrawCardEvent.LockBuyBtnAction -= LockBuyBtnAction;

            StopCoroutine(SetBtnClickLimit(1));

            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.drawcardgold, SetGoldRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.drawcardhc, SetHCRP);

            DestroySelf();
            yield break;
        }

        public override void OnFocus()
        {
            base.OnFocus();
            GOLD_CurIndex = GoldIndex.None;
            HC_CurIndex = HCIndex.None;
            controller.GObjects["FxParent"].CustomSetActive(true);
            if (m_type != DrawCardType.none)
            {
                controller.Transforms["MainHudTran"].GetComponent<UIPanel>().alpha = 1;
                controller.UiPanels["HudPanel"].alpha = 1;
                ShowTopInfo(m_type, true);
            }
            controller.GObjects["SSRWish"].CustomSetActive(IsSSRActivityOpen() && string.IsNullOrEmpty(currentEventTag));
        }


        /// <summary>
        /// 隐藏特效，抽奖时获得物品表现需要隐藏
        /// </summary>
        private void HideFxParent()
        {
            controller.GObjects["FxParent"].CustomSetActive(false);
            controller.Transforms["MainHudTran"].GetComponent<UIPanel>().alpha = 0;
            controller.UiPanels["HudPanel"].alpha = 0;
        }

        public override void OnCancelButtonClick()
        {
            if (lockBuyBtn)
            {
                return;
            }

            if (isMainView)
            {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                StopAllCoroutines();
                StartCoroutine(HideMainHud());
            }
            else
            {
                if (m_TimeHandler > 0)
                {
                    ILRTimerManager.instance.RemoveTimer(m_TimeHandler);
                    m_TimeHandler = 0;
                }
                base.OnCancelButtonClick();
            }
        }

        /// <summary>
        ////钻石或金币主抽奖界面显示的协程
        /// </summary>
        private WaitForSeconds mWait = new WaitForSeconds(0.2f);
        IEnumerator ShowMainHud()
        {
            GOLD_CurIndex = GoldIndex.None;
            HC_CurIndex = HCIndex.None;
            isMainView = true;

            CheakTimeFunc();

            controller.TweenAlphas["TypeHudTA"].PlayForward();
            controller.GObjects["TypeFx"].CustomSetActive(false);
            yield return mWait;
            ShowTopInfo(m_type);

            for (int i = 0; i < MainTweeners.Count; i++) MainTweeners[i].PlayForward();
            yield return new WaitForSeconds(0.2f);
            controller.GObjects["BtnsObject"].CustomSetActive(true);
        }

        //钻石或金币主抽奖界面隐藏的协程
        IEnumerator HideMainHud()
        {
            StopCoroutine(ShowMainHud());
            controller.GObjects["BtnsObject"].CustomSetActive(false);
            GOLD_CurIndex = GoldIndex.None;
            HC_CurIndex = HCIndex.None;

            //if (m_type == DrawCardType.gold)
            //{
            //    RedPointManager.Instance.SetDrawCardEventsStr();
            //}
            m_type = DrawCardType.none;
            isMainView = false;
            if (MainTweeners != null) for (int i = 0; i < MainTweeners.Count; i++) MainTweeners[i].PlayReverse();
            yield return mWait;
            controller.TweenAlphas["TypeHudTA"].PlayReverse();
            controller.GObjects["TypeFx"].CustomSetActive(true);

            ShowTopInfo(m_type);
        }

        /// <summary>
        /// 顶上道具显隐
        /// </summary>
        /// <param name="type"></param>
        private void ShowTopInfo(DrawCardType type, bool isFocus = false)
        {
            controller.GObjects["SliverFx"].CustomSetActive(type == DrawCardType.gold);
            controller.GObjects["GoldFx"].CustomSetActive(type == DrawCardType.hc);
            controller.GObjects["TopSliverStoneObj"].CustomSetActive(type != DrawCardType.hc);
            controller.GObjects["TopGoldenStoneObj"].CustomSetActive(type != DrawCardType.gold);
            StoneSprites[0].spriteName = StoneSprites[1].spriteName = type == DrawCardType.gold ? LTDrawCardConfig.GOLDDRAWICON : LTDrawCardConfig.HCDRAWICON;
            CoinSprites[0].spriteName = CoinSprites[1].spriteName = type == DrawCardType.gold ? LTDrawCardConfig.GOLDICON : LTDrawCardConfig.HCICON;
            controller.GObjects["TopGoldenStoneObj"].transform.parent.GetComponent<UIGrid>().Reposition();

            if (type == DrawCardType.hc)
            {
                if (!isFocus)
                {
                    EventBtns.ForEach(b => GameObject.DestroyImmediate(b.Key.gameObject));
                    EventBtns.Clear();

                    Transform t = controller.transform;
                    UIEventTrigger template = t.Find("MianHud/BG/FX/GoldFX/Anchor/ScrollView/Placeholder/ButtonList/template").GetComponent<UIEventTrigger>();
                    UIGrid grid = t.Find("MianHud/BG/FX/GoldFX/Anchor/ScrollView/Placeholder/ButtonList").GetComponent<UIGrid>();
                    ArrayList eventlist;
                    DataLookupsCache.Instance.SearchDataByID("events.events", out eventlist);

                    if (eventlist != null)
                    {
                        for (var k = 0; k < eventlist.Count; k++)
                        {
                            var e = eventlist[k];
                            string tag = EB.Dot.String("tag", e, "");
                            string desc = EB.Dot.String("desc", e, "");
                            string reward = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue(string.Format("hcSpecialBoxes-{0}", tag));
                            if (string.IsNullOrEmpty(reward)) continue;
                            int start = EB.Dot.Integer("start", e, 0);
                            int fin = EB.Dot.Integer("end", e, 0);
                            string state = EB.Dot.String("state", e, "");
                            if (fin <= EB.Time.Now)
                            {
                                continue;
                            }
                            if (state.Equals("pending"))
                            {
                                if (EB.Time.Now > start)
                                {
                                    LTMainHudManager.Instance.UpdateEventsLoginData(delegate { ShowTopInfo(m_type); });
                                }
                                continue;
                            }

                            UIEventTrigger eventBtn = GameUtils.InstantiateEx<UIEventTrigger>(template, grid.transform, tag);

                            var pair = new KeyValuePair<UIEventTrigger, int>(eventBtn, fin);
                            EventBtns.Add(pair);

                            UILabel label = eventBtn.transform.Find("BG/Label").GetComponent<UILabel>();
                            label.text = EB.Dot.String("title", e, "");

                            UITexture titlebg = eventBtn.transform.Find("BG").GetComponent<UITexture>();
                            GlobalMenuManager.Instance.LoadRemoteUITexture(EB.Dot.String("titlebg", e, ""), titlebg);

                            Transform PressObj = eventBtn.transform.Find("Pressed");
                            PressObj.gameObject.CustomSetActive(false);

                            string[] cmd = EB.Dot.String("nav_string", e, "").Split('-');
                            Transform HeroInfoRoot = PressObj.Find("HeroInfoList");
                            if (cmd != null && cmd.Length == 3)
                            {
                                List<DrawInfoBtn> ItemList = new List<DrawInfoBtn>();
                                DrawInfoBtn Info1 = new DrawInfoBtn(HeroInfoRoot.Find("InfoItem"));
                                DrawInfoBtn Info2 = new DrawInfoBtn(HeroInfoRoot.Find("InfoItem (1)"));
                                DrawInfoBtn Info3 = new DrawInfoBtn(HeroInfoRoot.Find("InfoItem (2)"));
                                ItemList.Add(Info1);
                                ItemList.Add(Info2);
                                ItemList.Add(Info3);

                                string[] temp = cmd[2].Split(',');
                                for (int i = 0; i < ItemList.Count; ++i)
                                {
                                    if (i < temp.Length)
                                    {
                                        ItemList[i].FillData(temp[i]);
                                    }
                                    else
                                    {
                                        ItemList[i].FillData(string.Empty);
                                    }
                                }
                                HeroInfoRoot.gameObject.CustomSetActive(true);
                            }
                            else
                            {
                                HeroInfoRoot.gameObject.CustomSetActive(false);
                            }

                            var ed = new EventDelegate(SelectEventTag(tag, eventBtn, desc));
                            eventBtn.onClick.Add(ed);
                            UpdateCountDown(pair, EB.Time.Now);

                            if (tag == currentEventTag)
                            {
                                ed.Execute();
                            }
                        }
                    }

                    if (template.onClick.Count <= 0)
                    {
                        UILabel templateLabel = template.transform.Find("BG/Label").GetComponent<UILabel>(); ;
                        templateLabel.text = EB.Localizer.GetString("ID_ACTIVITY_LOTTERY");
                        var ed = new EventDelegate(SelectEventTag(string.Empty, template, EB.Localizer.GetString("ID_uifont_in_LTDrawCardTypeHud_InfoLabel_11")));
                        template.onClick.Add(ed);
                    
                    }
                    if (string.IsNullOrEmpty(currentEventTag))
                    {
                        template.onClick[0].Execute();
                    }

                    grid.Reposition();
                }

                UpdateTopInfo();

                if (!lockBuyBtn) LTChargeManager.Instance.UpdateLimitedTimeGiftNotice();
            }

        }

        void ClearTimeupEventBtn(int idx)
        {
            var btn = EventBtns[idx];

            if (btn.Key != null)
            {
                UnityEngine.Object.DestroyImmediate(btn.Key.gameObject);
            }

            EventBtns.RemoveAt(idx);
            UIEventTrigger template = controller.transform.GetComponent<UIEventTrigger>("MianHud/BG/FX/GoldFX/ScrollView/Placeholder/ButtonList/template");
           
            if (template != null && template.onClick.Count > 0)
            {
                template.onClick[0].Execute();
            }
            else
            {
                currentEventTag = string.Empty;
            }
        }

        bool UpdateCountDown(KeyValuePair<UIEventTrigger, int> btndata, int now)
        {
            var eventBtn = btndata.Key;
            int fin = btndata.Value;
            if (fin <= now)
            {
                return false;
            }
            UILabel label = eventBtn.transform.Find("BG/Countdown").GetComponent<UILabel>();
            System.TimeSpan timeleft = new System.TimeSpan(0, 0, Mathf.Max(fin - now, 0));
            label.text = string.Format("{0:D2}: {1:D2}: {2:D2}", (int)Math.Floor(timeleft.TotalHours), timeleft.Minutes, timeleft.Seconds);
            return true;
        }

        public class DrawInfoBtn
        {
            public UIButton btn;

            public UISprite icon;
            public UISprite roleIcon;
            public UISprite gradeIcon;

            public string heroInfoId;


            private ParticleSystemUIComponent charFx;
            private EffectClip efClip;

            public void FillData(string Id)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    LTShowItemData itemData = new LTShowItemData(Id, 0, LTShowItemType.TYPE_HERO);
                    heroInfoId = itemData.id;
                    int cid = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(itemData.id).character_id;
                    Hotfix_LT.Data.HeroInfoTemplate data = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(cid);
                    icon.spriteName = data.icon;
                    roleIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[data.char_type]; 
                    gradeIcon.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)data.role_grade];

                    HotfixCreateFX.ShowCharTypeFX(charFx, efClip, roleIcon.transform, (PartnerGrade)data.role_grade, data.char_type);
                    btn.gameObject.CustomSetActive(true);
                }
                else
                {
                    btn.gameObject.CustomSetActive(false);
                }
            }

            public DrawInfoBtn(Transform Root)
            {
                btn = Root.GetComponent<UIButton>();
                btn.onClick.Add(new EventDelegate(OnFinalAwardClick));
                icon = Root.Find("Icon").GetComponent<UISprite>();
                roleIcon = Root.Find("Role").GetComponent<UISprite>();
                gradeIcon = Root.Find("Grade").GetComponent<UISprite>();
            }

            private void OnFinalAwardClick()
            {
                Vector2 screenPos = UICamera.lastEventPosition;
                var ht = Johny.HashtablePool.Claim();
                ht.Add("id", heroInfoId);
                ht.Add("screenPos", screenPos);
                GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
            }
        }

        private UILabel info;
        private TweenAlpha infoTS;
        EventDelegate.Callback SelectEventTag(string tag, UIEventTrigger btn, string desc)
        {
            return delegate
            {
                currentEventTag = tag;
                controller.GObjects["SSRWish"].CustomSetActive(IsSSRActivityOpen() && string.IsNullOrEmpty(currentEventTag));
                if (currentEventBtn != null)
                {
                    currentEventBtn.transform.Find("Pressed").gameObject.CustomSetActive(false);
                }

                currentEventBtn = btn;
                currentEventBtn.transform.Find("Pressed").gameObject.CustomSetActive(true);
                TweenPosition HeroInfoRoot = currentEventBtn.transform.Find("Pressed/HeroInfoList").GetComponent<TweenPosition>();
                if (HeroInfoRoot.gameObject.activeSelf)
                {
                    HeroInfoRoot.ResetToBeginning();
                    HeroInfoRoot.PlayForward();
                }

                if (info == null)
                {
                    info = controller.transform.Find("MianHud/BG/FX/GoldFX/InfoLabel").GetComponent<UILabel>();
                }
                if (info.text != desc)
                {
                    info.text = desc;
                    if (infoTS == null)
                    {
                        infoTS = info.GetComponent<TweenAlpha>();
                    }
                    infoTS.ResetToBeginning();
                    infoTS.PlayForward();
                }
            };
        }

        public bool IsSSRActivityOpen()
        {
            // Hashtable hashtable;
            // DataLookupsCache.Instance.SearchDataByID("tl_acs." + SSRWishItem.ssrWishActivityId, out hashtable);
            // if (hashtable!=null)
            // {
            //     int fin = EB.Dot.Integer("e", hashtable, 0);
            //     if (fin > EB.Time.Now)
            //     {
            //         return true;
            //     }
            // }

            // return false;
            
            ArrayList eventlist;
            DataLookupsCache.Instance.SearchDataByID("events.events", out eventlist);
            for (int i = 0; i < eventlist.Count; ++i)
            {
                int aid = EB.Dot.Integer("activity_id", eventlist[i], 0);
                if (aid == SSRWishItem.ssrWishActivityId)
                {
                    int start = EB.Dot.Integer("start", eventlist[i], 0);
                    int fin = EB.Dot.Integer("end", eventlist[i], 0);
                    string state = EB.Dot.String("state", eventlist[i], "");
                    if (start < EB.Time.Now  && fin > EB.Time.Now)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        List<KeyValuePair<UIEventTrigger, int>> EventBtns = new List<KeyValuePair<UIEventTrigger, int>>();
        public string currentEventTag = string.Empty;
        UIEventTrigger currentEventBtn = null;
        
        private IEnumerator SetBtnClickLimit(float delay)
        {
            yield return new WaitForSeconds(delay);
            BtnClickLimit = false;
        }

        /// <summary>
        /// 按钮事件，点击金币抽奖
        /// </summary>
        public void OnDrawSliverCardBtnClick()
        {
            if (BtnClickLimit)
            {
                return;
            }

            BtnClickLimit = true;
            m_type = DrawCardType.gold;
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 1f);
            ShowMainHudFunc();
            StartCoroutine(SetBtnClickLimit(1));
        }

        /// <summary>
        /// 按钮事件，点击钻石抽奖
        /// </summary>
        public void OnDrawGoldCardBtnClick()
        {
            if (BtnClickLimit)
            {
                return;
            }

            BtnClickLimit = true;
            m_type = DrawCardType.hc;
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 1f);
            ShowMainHudFunc();
            StartCoroutine(SetBtnClickLimit(1));
        }
        /// <summary>
        /// 显示当前类型抽奖界面
        /// </summary>
        private void ShowMainHudFunc()
        {
            StopAllCoroutines();
            StartCoroutine(ShowMainHud());
        }

        /// <summary>
        /// 抽奖锁状态防止多点触碰跟连点问题
        /// </summary>
        private bool lockBuyBtn = false;
        private void LockBuyBtnAction(bool value)
        {
            lockBuyBtn = value;
        }

        /// <summary>
        /// 购买一次按钮点击
        /// </summary>
        public void BuyOnceBtnClick()
        {
            if (lockBuyBtn || m_type == DrawCardType.none) return;
            lockBuyBtn = true;

            if (HCDarwCardJudgeFun(1)) return;

            LTDrawCardDataManager.Instance.GetPartnerTID();
            LTDrawCardDataManager.Instance.GetDrawCardRequireMsg((m_type == DrawCardType.gold) ? BalanceResourceUtil.GoldName : BalanceResourceUtil.HcName, currentEventTag, 1, delegate (bool success)
            {
                InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 1f);
                if (success)
                {
                    object[] i = { (int)m_type, 1, currentEventTag };
                    GlobalMenuManager.Instance.Open("LTGetItemUI", i);
                    //if (m_type == DrawCardType.hc)
                    //{
                    //    //钻石抽奖添加UR活动标志
                    //    isNeedRefreshActTime = true;
                    //}
                }
                lockBuyBtn = false;
            });
        }
        /// <summary>
        /// 购买多次按钮点击
        /// </summary>
        public void BuyMoreBtnClick()
        {
            if (lockBuyBtn || m_type == DrawCardType.none) return;
            lockBuyBtn = true;

            if (HCDarwCardJudgeFun(9)) return;

            LTDrawCardDataManager.Instance.GetPartnerTID();
            LTDrawCardDataManager.Instance.GetDrawCardRequireMsg((m_type == DrawCardType.gold) ? BalanceResourceUtil.GoldName : BalanceResourceUtil.HcName, currentEventTag, 10, delegate (bool success)
            {
                InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 1f);
                if (success)
                {
                    object[] i = { (int)m_type, 10, currentEventTag };
                    GlobalMenuManager.Instance.Open("LTGetItemUI", i);
                    //if(m_type == DrawCardType.hc)
                    //{
                    //    //钻石抽奖添加UR活动标志
                    //    isNeedRefreshActTime = true;                                             
                    //}
                }
                lockBuyBtn = false;
            });
        }

        /// <summary>
        /// 查看抽奖的信息
        /// </summary>
        public void OnLookUpPartnerBtnClick()
        {
            Hashtable data = new Hashtable();
            data.Add("type", m_type);
            GlobalMenuManager.Instance.Open("LTDrawCardLookupUI", data);
        }

        /// <summary>
        /// 钻石抽奖都需要转换成抽奖券再进行抽奖
        /// </summary>
        /// <returns></returns>
        private bool HCDarwCardJudgeFun(int count)
        {
            if (m_type == DrawCardType.hc && (count == 9 || HC_LabelIndex != HCIndex.Draw))
            {
                if (LTDrawCardDataManager.Instance.GoldenLotteryStone < count)
                {
                    int costCount = LTDrawCardConfig.Once_HCCost * (count - LTDrawCardDataManager.Instance.GoldenLotteryStone);
                    if (costCount <= BalanceResourceUtil.GetUserDiamond())
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, string.Format(EB.Localizer.GetString("ID_DRAWCARD_COST_HC"), "[42fe79]" + costCount.ToString() + "[-]"), delegate (int result)
                        {
                            if (result == 0)
                            {
                                InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 1f);
                                LTDrawCardDataManager.Instance.GetDrawCardItemRequireMsg(count - LTDrawCardDataManager.Instance.GoldenLotteryStone, delegate (bool success) {
                                    lockBuyBtn = false;
                                    if (success)
                                    {
                                        if (count == 1) BuyOnceBtnClick();
                                        else BuyMoreBtnClick();
                                    }
                                });
                            }
                            else lockBuyBtn = false;
                        });
                        return true;
                    }
                    else
                    {
                        EB.IAP.Item GiftItem = null;
                        string GiftList = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("DrawCardGiftItemLift");
                        string[] GiftStr = GiftList.Split(',');
                        bool hasGift = false;
                        for (int i = 0; i < GiftStr.Length; ++i)
                        {
                            if (!string.IsNullOrEmpty(GiftStr[i]))
                            {
                                int giftId = int.Parse(GiftStr[i]);
                                if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(giftId, out GiftItem))
                                {
                                    hasGift = true;
                                    GlobalMenuManager.Instance.Open("LTChargeStoreGiftUI", GiftItem);
                                    break;
                                }
                            }
                        }

                        if (!hasGift)
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_DRAWCARD_BUY_HC"), delegate (int result)
                            {
                                if (result == 0)
                                {
                                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                    GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
                                }
                            });
                        }
                        lockBuyBtn = false;
                        return true;
                    }
                }
            }
            return false;
        }

        
        private LTDrawTimesGiftController TimesGiftController
        {
            get { return controller.transform.Find("MianHud/BG/FX/GoldFX/TimeGift").GetMonoILRComponent<LTDrawTimesGiftController>(); }
        }

        private void UpdateTopInfo()
        {
            string str = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("hcLotteryFloors");
            if (!string.IsNullOrEmpty(str))
            {
                int show = 0;
                int floors = 0;
                string[] strs = str.Split(',');
                if (strs.Length == 2)
                {
                    int.TryParse(strs[0], out show);
                    int.TryParse(strs[1], out floors);
                    int totleTime = LTDrawCardDataManager.Instance.TotalHcTime;
                    //需要一个数据 获取SSP
                    if (totleTime >= floors )
                    {
                        ShowGiftInfoLabel(true,LTDrawCardDataManager.Instance.NextSSRTime);
                        ShowRecommendGift();
                    }
                    else if (totleTime >= show )
                    {
                        ShowGiftInfoLabel(true, floors - totleTime);
                        ShowRecommendGift();
                    }
                    else
                    {
                        ShowGiftInfoLabel(false);
                        ShowRecommendGift(false);
                    }
                }
                TimesGiftController.SetState();
            }
        }

        private void ShowRecommendGift(bool isShow = true)
        {
            if (isShow)
            {
                EB.IAP.Item GiftItem = null;
                if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(3007, out GiftItem))
                {
                    ShowGiftInfo(GiftItem);
                    controller.TweenScales["GiftRootTween"].gameObject.CustomSetActive(true);
                    controller.TweenScales["GiftRootTween"].PlayForward();
                }
                else
                {
                    controller.TweenScales["GiftRootTween"].gameObject.CustomSetActive(false);
                }
            }
            else
            {
                controller.TweenScales["GiftRootTween"].gameObject.CustomSetActive(false);
            }
        }

        private void ShowGiftInfo(EB.IAP.Item GiftItem)
        {
            controller.UiSprites["GiftIcon"].spriteName = GiftItem.icon;
            controller.UiLabels["NameLabel"].text = GiftItem.longName;
            int count = 0;
            for (int i = 0; i < GiftItem.redeemers.Count; i++)
            {
                if (GiftItem.redeemers[i].Data.Equals(LTDrawCardConfig.LOTTERY_HC_ID))
                {
                    count = GiftItem.redeemers[i].Quantity;
                    break;
                }
            }
            controller.UiLabels["ContainNumLabel"].text = string.Format("x{0}", count);
            controller.UiLabels["CostLabel"].text = GiftItem.localizedCost;
        }

        private void ShowGiftInfoLabel(bool isShow = true, int timer = 0)
        {
            if (info == null)
            {
                info = controller.transform.Find("MianHud/BG/FX/GoldFX/InfoLabel").GetComponent<UILabel>();
            }
            if (isShow)
            {
                if (controller.UiLabels["GiftInfoLabel"] != null && timer > 0)
                {
                    controller.UiLabels["GiftInfoLabel"].text = string.Format(EB.Localizer.GetString("ID_DRAWCARD_GIFT_TIP"), timer);
                    controller.UiLabels["GiftInfoLabel"].gameObject.CustomSetActive(true);
                    info.gameObject.CustomSetActive(false);
                }
                else
                {
                    info.gameObject.CustomSetActive(true);
                    controller.UiLabels["GiftInfoLabel"].gameObject.CustomSetActive(false);
                }
            }
            else
            {
                info.gameObject.CustomSetActive(true);
                controller.UiLabels["GiftInfoLabel"].gameObject.CustomSetActive(false);
            }
        }

        public void ShowGiftItem()
        {
            EB.IAP.Item GiftItem = null;
            if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(3007, out GiftItem))
            {
                GlobalMenuManager.Instance.Open("LTChargeStoreGiftUI", GiftItem);

            }
            controller.TweenScales["GiftRootTween"].ResetToBeginning();
            ShowRecommendGift(false);
        }
        
        public void ShowTimeGift()
        {
            List<LTShowItemData> showItemsList = LTDrawCardDataManager.Instance.GetShowItemsList();
            if (LTDrawCardDataManager.Instance.Currentime>=LTDrawCardDataManager.Instance.everytimeGift)
            {
                //请求奖励
                LTDrawCardDataManager.Instance.GetDrawCardTimeGift((b) =>
                {
                    if (b)
                    {
                        GlobalMenuManager.Instance.Open("LTShowRewardView", showItemsList);
                    }
                });
            }
            else
            {
                Hashtable data = new Hashtable() { { "data", showItemsList}, { "tip", EB.Localizer.GetString("ID_SUMMON_NODE_PACK") } };
                GlobalMenuManager.Instance.Open("LTRewardShowUI",data);
            }
        }
    }
}
