using System;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionPageMedal : DynamicMonoHotfix, IHotfixUpdate
    {
        UIButton HotfixBtn0;
        UIButton HotfixBtn1;
        UIButton HotfixBtn2;
        public override void Awake()
        {
            base.Awake();
            HeadIconSp = mDMono.transform.Find("Right/Item/Member/Border/Icon").GetComponent<UISprite>();
            FrameIconSp = mDMono.transform.Find("Right/Item/Member/Border/Icon/Frame").GetComponent<UISprite>();
            LevelLab = mDMono.transform.Find("Right/Item/Member/LevelSprite/LevelLabel").GetComponent<UILabel>();
            NameLab = mDMono.transform.Find("Right/Item/Member/NameLabel").GetComponent<UILabel>();
            StateLab = mDMono.transform.Find("Right/Item/Member/StateLabel").GetComponent<UILabel>();
            CountDownLab = mDMono.transform.Find("Lift/CountDown/CountDownLabl").GetComponent<UILabel>();
            BuyMedalCoin = mDMono.transform.Find("Lift/BuyBtn/Sprite/CoinNum").GetComponent<UILabel>();
            BuyBtnObj = mDMono.transform.Find("Lift/BuyBtn").gameObject;
            GiveBtnObj = mDMono.transform.Find("Lift/GiveBtn").gameObject;
            CountDownObj = mDMono.transform.Find("Lift/CountDown").gameObject;
            MemberItemObj = mDMono.transform.Find("Right/Item/Member").gameObject;
            NoneObj = mDMono.transform.Find("Right/Item/None").gameObject;
            MedalMemberView = mDMono.transform.parent.parent.Find("LTLegionMedalMemberView").GetMonoILRComponent<LegionMedalMemberView>();
            HotfixBtn0 = mDMono.transform.Find("Lift/BuyBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnClickBuyBtn));
            HotfixBtn1 = mDMono.transform.Find("Lift/GiveBtn").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(OnClickGiveBtn));
            HotfixBtn2 = mDMono.transform.Find("Right/Item/Member/RelieveBtn").GetComponent<UIButton>();
            HotfixBtn2.onClick.Add(new EventDelegate(OnClickRelieve));
        }


        /// </summary>
        public enum EMedalState
        {
            /// <summary>
            /// 购买状态
            /// </summary>
            mBuy,

            /// <summary>
            /// 赠送状态
            /// </summary>
            mGive,

            /// <summary>
            /// 倒计时状态
            /// </summary>
            mCountDown
        }

        /// <summary>
        /// 赠与者头像
        /// </summary>
        public UISprite HeadIconSp;

        /// <summary>
        /// 赠与者头像框
        /// </summary>
        public UISprite FrameIconSp;

        /// <summary>
        /// 赠与者等级
        /// </summary>
        public UILabel LevelLab;

        /// <summary>
        /// 赠与者名字
        /// </summary>
        public UILabel NameLab;

        /// <summary>
        /// 赠与者状态
        /// </summary>
        public UILabel StateLab;

        /// <summary>
        /// 军团勋章倒计时
        /// </summary>
        public UILabel CountDownLab;

        /// <summary>
        /// 购买勋章的花费
        /// </summary>
        public UILabel BuyMedalCoin;

        /// <summary>
        /// 购买按钮Obj
        /// </summary>
        public GameObject BuyBtnObj;

        /// <summary>
        /// 赠与按钮Obj
        /// </summary>
        public GameObject GiveBtnObj;

        /// <summary>
        /// 倒计时Obj
        /// </summary>
        public GameObject CountDownObj;

        /// <summary>
        /// 成员item
        /// </summary>
        public GameObject MemberItemObj;

        /// <summary>
        /// 没有成员的item
        /// </summary>
        public GameObject NoneObj;

        /// <summary>
        /// 可赠与勋章成员界面
        /// </summary>
        public LegionMedalMemberView MedalMemberView;

        /// <summary>
        /// 所有公会成员数据
        /// </summary>
        private List<LegionMemberData> mLegionMemberDataList;

        /// <summary>
        /// 当前的勋章状态
        /// </summary>
        private EMedalState mCurMedalState;

        /// <summary>
        /// 勋章结束时间
        /// </summary>
        private int mEndTime;

        /// <summary>
        /// 花费钻石的数量
        /// </summary>
        private float mCoinNum;

        /// <summary>
        /// update执行倒计时
        /// </summary>
        private float mCountDownTime;

        /// <summary>
        /// 是否可以买军团勋章（已经有军团勋章了就不能买了）
        /// </summary>
        private bool mIsCouldBuyMedal;

        /// <summary>
        /// 是否开启倒计时
        /// </summary>
        private bool mIsOpenCountDown;

        /// <summary>
        /// 倒计时文本
        /// </summary>
        private string mCountDownStr;

        /// <summary>
        /// 当前的勋章成员
        /// </summary>
        private LegionMemberData mCurGiveMember;

        /// <summary>
        /// 倒计时update执行的频率
        /// </summary>
        private const int mCountDownTimeCon = 10;

        public void Update()
        {
            if (mIsOpenCountDown)
            {
                mCountDownTime -= Time.deltaTime;
                if (mCountDownTime <= 0)
                {
                    SetCountDown();
                    mCountDownTime = mCountDownTimeCon;
                }
            }
        }

        /// <summary>
        /// 显示勋章界面
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.CustomSetActive(isShow);

            if (isShow)
            {
                Init();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            InitData();

            SetMedalState();

            SetMedalCoin();

            SetMedalMember();

            InitCountDown();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            mCurMedalState = EMedalState.mBuy;

            int num = 0;
            DataLookupsCache.Instance.SearchDataByID("alliance.account.medal.num", out num);


            IDictionary medalMember;
            DataLookupsCache.Instance.SearchDataByID("alliance.account.medal.send", out medalMember);

            if (medalMember == null)
            {
                if (num > 0)
                {
                    mCurMedalState = EMedalState.mGive;
                }
                return;
            }

            long uid = 0;
            int timer = 0;
            mEndTime = 0;
            foreach (DictionaryEntry item in medalMember)
            {
                timer = EB.Dot.Integer("expiry", item.Value, 0);
                if (long.Parse(item.Key.ToString()) == 0 || EB.Time.Now >= timer)
                {
                    continue;
                }
                uid = long.Parse(item.Key.ToString());
                mEndTime = timer;
            }
            if (uid == 0 || EB.Time.Now >= mEndTime)
            {
                if (num > 0)
                {
                    mCurMedalState = EMedalState.mGive;
                }
                return;
            }

            mCurGiveMember = null;
            for (int i = 0; i < mLegionMemberDataList.Count; i++)
            {
                if (mLegionMemberDataList[i].uid == uid)
                {
                    mCurGiveMember = mLegionMemberDataList[i];
                    break;
                }
            }

            if (mCurGiveMember == null)
            {
                if (num > 0)
                {
                    mCurMedalState = EMedalState.mGive;
                }
                return;
            }

            mCurMedalState = EMedalState.mCountDown;
        }

        /// <summary>
        /// 设置勋章状态
        /// </summary>
        private void SetMedalState()
        {
            BuyBtnObj.CustomSetActive(mCurMedalState == EMedalState.mBuy);
            GiveBtnObj.CustomSetActive(mCurMedalState == EMedalState.mGive);
            CountDownObj.CustomSetActive(mCurMedalState == EMedalState.mCountDown);

            MemberItemObj.CustomSetActive(mCurMedalState == EMedalState.mCountDown);
            NoneObj.CustomSetActive(mCurMedalState == EMedalState.mBuy || mCurMedalState == EMedalState.mGive);
            hasBuy = false;
        }

        /// <summary>
        /// 设置价格
        /// </summary>
        public void SetMedalCoin()
        {
            mCoinNum = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("AllianceMedalCost");
            string colorStr = BalanceResourceUtil.GetUserDiamond() >= mCoinNum ? LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
            BuyMedalCoin.text = string.Format("[{0}]{1}[-]", colorStr, mCoinNum);
        }

        /// <summary>
        /// 设置勋章成员
        /// </summary>
        private void SetMedalMember()
        {
            if (mCurGiveMember != null)
            {
                HeadIconSp.spriteName = Data.CharacterTemplateManager.Instance.GetHeroInfo(mCurGiveMember.templateId, mCurGiveMember.skin).icon;
                FrameIconSp.spriteName = Data.EconemyTemplateManager.Instance.GetHeadFrame(mCurGiveMember.headFrame).iconId;
                NameLab.text = mCurGiveMember.memberName;
                LevelLab.text = mCurGiveMember.level.ToString();

                if (mCurGiveMember.offlineTime == 0)
                {
                    StateLab.text = EB.Localizer.GetString("ID_ON_LINE");
                }
                else
                {
                    if (mCurGiveMember.offlineHour < 24)
                    {
                        StateLab.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LegionMemberItem_1531"), LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal);
                    }
                    else if (mCurGiveMember.offlineHour < 168)
                    {
                        StateLab.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LegionMemberItem_1689"), LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal, (mCurGiveMember.offlineHour / 24).ToString());
                    }
                    else
                    {
                        StateLab.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LegionMemberItem_1856"), LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal);
                    }
                }
            }
        }

		private int m_TimerIndex;

        /// <summary>
        /// 初始化倒计时
        /// </summary>
        private void InitCountDown()
        {
            mCountDownStr = EB.Localizer.GetString("ID_LEGION_MEDAL_COUNTDOWN_VALUE");

            if (mCurMedalState == EMedalState.mCountDown)
            {
                mIsOpenCountDown = true;
                mCountDownTime = (mEndTime - EB.Time.Now) % 10;
                SetCountDown();

				m_TimerIndex = ILRTimerManager.instance.AddTimer(1000, Mathf.RoundToInt(mCountDownTime), (tIndex) => { Update(); });
            }
        }

        /// <summary>
        /// 设置倒计时
        /// </summary>
        /// <returns></returns>
        private void SetCountDown()
        {
            int remainTime = mEndTime - EB.Time.Now - 1;
            if (remainTime < 0)
            {
                mIsOpenCountDown = false;
				ILRTimerManager.instance.RemoveTimer(m_TimerIndex);
                Init();
            }

            int day = remainTime / 86400;
            int hour = (remainTime / 3600) % 24;
            int minute = (remainTime % 3600) / 60;

            CountDownLab.text = string.Format(mCountDownStr, day, hour, minute);
        }

        /// <summary>
        /// 该成员是否还在公会里面
        /// </summary>
        /// <param name="uid"></param>
        private bool IsInLegion(long uid)
        {
            for (int i = 0; i < mLegionMemberDataList.Count; i++)
            {
                if (mLegionMemberDataList[i].uid == uid)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 设置所有公会成员数据
        /// </summary>
        /// <param name="legionMemberDataList"></param>
        public void SetLegionMemberData(List<LegionMemberData> legionMemberDataList)
        {
            mLegionMemberDataList = legionMemberDataList;

            if (MedalMemberView.mDMono.gameObject.activeInHierarchy)
            {
                MedalMemberView.SetLegionMemberData(mLegionMemberDataList);
            }
        }

        private bool hasBuy = false;
        /// <summary>
        /// 点击购买按钮
        /// </summary>
        public void OnClickBuyBtn()
        {
            if (BalanceResourceUtil.GetUserDiamond() < mCoinNum)
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }

            if (hasBuy) return;
            hasBuy = true;
            LegionLogic.GetInstance().BuyLegionMedal(delegate
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceShopCtrl_4116"));

                mCurMedalState = EMedalState.mGive;
                SetMedalState();
            });
        }

        /// <summary>
        /// 点击赠与按钮
        /// </summary>
        public void OnClickGiveBtn()
        {
            MedalMemberView.SetLegionMemberData(mLegionMemberDataList);
            MedalMemberView.SetAction(delegate { Init(); });
            MedalMemberView.ShowUI(true);
        }

        /// <summary>
        /// 点击解除按钮
        /// </summary>
        public void OnClickRelieve()
        {
            if (mCurGiveMember != null)
            {
                if (!IsInLegion(mCurGiveMember.uid))
                {
                    Init();
                    return;
                }

                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, string.Format(EB.Localizer.GetString("ID_LEGION_MEDAL_RELIEVE_CONFIRM"), mCurGiveMember.memberName), delegate (int result)
                {
                    if (result == 0)
                    {
                        LegionLogic.GetInstance().UnlinkMedalPair(mCurGiveMember.uid, delegate
                        {
                            mCurGiveMember = null;
                            DataLookupsCache.Instance.CacheData("alliance.account.medal.send", null);
                            Init();
                        });
                    }
                });
            }
        }
    }
}
