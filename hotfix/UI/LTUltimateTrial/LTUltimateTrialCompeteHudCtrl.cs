using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    //
    public class LTUltimateTrialCompeteHudCtrl : UIControllerHotfix
    {
        public UILabel VigorLabel,SelfLabel, TheFastLabel,TimeLabel;

        public LTUltimateTrialCompeteScroll Scroll;

        public LTPassTeamInfoItem Item;

        private GameObject BtnObj, BtnLockObj;

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            controller .backButton = t.GetComponent<UIButton>("Edge/TopLeft/CancelBtn");

            VigorLabel = t.GetComponent<UILabel>("Edge/BtnRoot/BattleBtn/VigorLabel");
            EnterVigor = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("InfiniteCompeteCost");
            VigorLabel.text = EnterVigor.ToString();
            VigorLabel.color = EnterVigor > BalanceResourceUtil.GetUserVigor() ? LT.Hotfix.Utility.ColorUtility.RedColor : LT.Hotfix.Utility.ColorUtility.GreenColor;

            SelfLabel = t.GetComponent<UILabel>("Edge/RankInfoRoot/SelfLabel");
            TheFastLabel = t.GetComponent<UILabel>("Edge/RankInfoRoot/TheFastLabel");
            TimeLabel = t.GetComponent<UILabel>("Edge/BtnRoot/TimeLabel");

            Scroll = t.GetMonoILRComponent<LTUltimateTrialCompeteScroll>("Edge/Content/ScrollView/Placehodler/Grid");

            Item = t.GetMonoILRComponent<LTPassTeamInfoItem>("Edge/TheFast");

            t.GetComponent<UIButton>("Edge/TopRight/Notice").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Edge/BtnRoot/RankBtn").onClick.Add(new EventDelegate(OnRankBtnClick));
            t.GetComponent<UIButton>("Edge/BtnRoot/RewardBtn").onClick.Add(new EventDelegate(OnRewardBtnClick));

            BtnObj = t.Find("Edge/BtnRoot/BattleBtn").gameObject;
            BtnObj.transform.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnChallengeBtnClick));
            BtnLockObj = t.Find("Edge/BtnRoot/LockLable").gameObject;
        }

        public override IEnumerator OnAddToStack()
        {
            UpdateTime();
            mTimer = ILRTimerManager.instance.AddTimer(30000, int.MaxValue, UpdateTime);
            isClickBtn = true;
            yield return base.OnAddToStack();
            DataLookupsCache.Instance.CacheData("speedinfiniteChallenge", null);
            LTUltimateTrialDataManager.Instance.RequestGetCompeteInfo(delegate
            {
                isClickBtn = false;

                LTUltimateTrialDataManager.Instance.curCompeteLevel = LTUltimateTrialDataManager.Instance.GetCurCompeteLevel();
                int maxIndex = Data.EventTemplateManager.Instance.GetAllInfiniteCompete().Count;
                OnSelectFunc();

                if (LTUltimateTrialDataManager.Instance.curCompeteLevel > 3)
                {
                    int moveto = Mathf.Clamp(LTUltimateTrialDataManager.Instance.curCompeteLevel - 2, 0, maxIndex - 4);//滚动区域内显示的最大个数4
                    Scroll.MoveTo(moveto);
                }

                SelfLabel.text = TheFastLabel.text = EB.Localizer.GetString("ID_LEGION_MEDAL_NOT");
                if (maxIndex == LTUltimateTrialDataManager.Instance.curCompeteLevel)
                {
                    if (LTUltimateTrialDataManager.Instance.GetCurCompeteTime(maxIndex) > 0)
                    {
                        int time=LTUltimateTrialDataManager.Instance.GetCurCompeteTotleTime();
                        int hour = time / 3600;
                        if (hour > 99)
                        {
                            SelfLabel.text = "99:59:60";
                        }
                        else
                        {
                            SelfLabel.text = string.Format("{0}:{1}:{2}", hour.ToString("00"), (time / 60 % 60).ToString("00"), (time % 60).ToString("00"));
                        }
                    }

                }

                int rtime = LTUltimateTrialDataManager.Instance.GetCurCompeteRealmTotleTime();
                if (rtime > 0)
                {
                    int hour = rtime / 3600;
                    if (hour > 99)
                    {
                        TheFastLabel.text = "99:59:60";
                    }
                    else
                    {
                        TheFastLabel.text = string.Format("{0}:{1}:{2}", hour.ToString("00"), (rtime / 60 % 60).ToString("00"), (rtime % 60).ToString("00"));
                    }
                }

                GlobalMenuManager.Instance.PushCache("LTUltimateTrialCompeteHud");
                LTUltimateTrialDataManager.Instance.OnCompeteSelect += OnSelectFunc;
            });
        }

        public override void OnCancelButtonClick()
        {
            if (isClickBtn) return;
            GlobalMenuManager.Instance.RemoveCache("LTUltimateTrialCompeteHud");
            base.OnCancelButtonClick();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            LTUltimateTrialDataManager.Instance.OnCompeteSelect -= OnSelectFunc;
            if (mTimer > 0)
            {
                ILRTimerManager.instance.RemoveTimer(mTimer);
                mTimer = 0;
            }
            DestroySelf();
            yield break;
        }

        public override void OnFocus()
        {
            base.OnFocus();
            VigorLabel.color = EnterVigor>BalanceResourceUtil.GetUserVigor() ? LT.Hotfix.Utility.ColorUtility.RedColor: LT.Hotfix.Utility.ColorUtility.GreenColor;
        }

        public void OnSelectFunc(int level=-1)
        {
            if (level == -1)//初始化
            {
                level = LTUltimateTrialDataManager.Instance.curCompeteLevel;
                UpdateTheFast();//更新最快队伍数据
            }
            else
            {
                Hashtable data = LTUltimateTrialDataManager.Instance.GetCurCompeteRealmInfoTeam();
                if(data!=null && data.Count > 0)
                {
                    UpdateTheFast();//更新最快队伍数据
                }
                else
                {
                    LTUltimateTrialDataManager.Instance.RequestGetCompeteOtherInfo(level,delegate {
                        UpdateTheFast();//更新最快队伍数据
                    });
                }
            }

            var datas = Data.EventTemplateManager.Instance.GetAllInfiniteCompete();
            Scroll.SetItemDatas(datas.ToArray());
            UpdateBtn();
        }

        private int mTimer = 0;
        private void UpdateTime(int timer = 0)
        {
            TimeLabel.text = LTUltimateTrialDataManager.Instance.GetTimeTip(true);
            if (TimeLabel.text.Equals("close"))
            {
                isClickBtn = false;
                controller.OnCancelButtonClick();
            }
        }
        
        private void UpdateTheFast()
        {
            Item.OnFill(null, null);
            int time = LTUltimateTrialDataManager.Instance.GetCurCompeteRealmInfo();
            if (time > 0)
            {
                Hashtable data = LTUltimateTrialDataManager.Instance.GetCurCompeteRealmInfoTeam();
                int hour = time / 60;
                string timeStr;
                if (hour > 99)
                {
                    timeStr = "99:60";
                }
                else
                {
                    timeStr = string.Format("{0}:{1}", hour.ToString("00"), (time % 60).ToString("00"));
                }
                Item.OnFill(data, timeStr);
            }
        }

        private void UpdateBtn()
        {
            if (LTUltimateTrialDataManager.Instance.curCompeteLevel > LTUltimateTrialDataManager.Instance.GetCurCompeteLevel())
            {
                BtnObj.CustomSetActive(false);
                BtnLockObj.CustomSetActive(true);
            }
            else
            {
                BtnLockObj.CustomSetActive(false );
                BtnObj.CustomSetActive(true);
            }
        }

        public void OnRuleBtnClick()
        {
            string text = EB.Localizer.GetString("ID_ULTIMATE_TRIAL_COMPETE_RULES");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        public void OnRankBtnClick()
        {
            if (isClickBtn) return;
            GlobalMenuManager.Instance.Open("LTUltimateTrialCompeteRankListHud");
        }

        public void OnRewardBtnClick()
        {
            if (isClickBtn) return;
            var temp = Data.EventTemplateManager.Instance.GetAllInfiniteCompeteReward();
            List<LTRankAwardData> data = new List<LTRankAwardData>();
            for (int i = 0; i < temp.Count; ++i)
            {
                string[] splits= temp[i].ranks.Split(';');

                if(splits.Length >=2) data.Add(new LTRankAwardData(int.Parse(splits[0]), int.Parse(splits[1]), temp[i].reward));
            }
            string tip = EB.Localizer.GetString("ID_ULTIMATE_COMPETE_TIP2");

            var ht = Johny.HashtablePool.Claim();
            ht.Add("itemDatas", data);
            ht.Add("tip", tip);

            GlobalMenuManager.Instance.Open("LTGeneralRankAwardUI", ht);
        }

        private int EnterVigor;
        private bool isClickBtn;
        public void OnChallengeBtnClick()
        {
            if (isClickBtn) return;
            FusionAudio.PostEvent("UI/General/ButtonClick");

            //判断体力是否足够
            if (BalanceResourceUtil.EnterVigorCheck(EnterVigor))
            {
                return;
            }

            if (AllianceUtil.GetIsInTransferDart(""))
            {
                return;
            }

            if (LTUltimateTrialDataManager.Instance.curCompeteLevel > LTUltimateTrialDataManager.Instance.GetCurCompeteLevel())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer .GetString("ID_codefont_in_LTResourceInstanceHudController_7254"));
                return;
            }

            if (!LTUltimateTrialDataManager.Instance.GetTimeTip(false, true).Equals("open"))
            {
                //赛季结束,不在活动时间
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_62898"));
                return;
            }

            int level = LTUltimateTrialDataManager.Instance.curCompeteLevel;
            string enemyLayout = LTUltimateTrialDataManager.Instance.GetCurCompeteLayout(level);
            if (string.IsNullOrEmpty(enemyLayout))
            {
                //服务器未开始这个活动，没初始化怪物数据
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_NOT_OPEN"));
                return;
            }
            isClickBtn = true;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            
            System.Action startCombatCallback = delegate { LTUltimateTrialDataManager.Instance.RequestStarttCompete(level); };

            BattleReadyHudController.Open(eBattleType.InfiniteCompete, startCombatCallback, enemyLayout);
            isClickBtn = false;
        }
    }
}
