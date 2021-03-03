using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LadderController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            StageLeftSprite = t.GetComponent<DynamicUISprite>("Content/FxInfo/StageLeft/Icon");
            StageLeftNameLabel = t.GetComponent<UILabel>("Content/FxInfo/StageLeft/Name");
            ProgressBarLabel = t.GetComponent<UILabel>("Content/FxInfo/ProgressBar/Label");
            LeftEndTimeLabel = t.GetComponent<UILabel>("Content/FxInfo/LeftEndTime/Time");
            RankLabel = t.GetComponent<UILabel>("Content/BG/Rank/Num");
            HasMatchNumLabel = t.GetComponent<UILabel>("Content/RightStage/Box/HasNum");
            NeedMatchNumLabel = t.GetComponent<UILabel>("Content/RightStage/Box/NeedNum");
            WinNumLabel = t.GetComponent<UILabel>("Content/BG/Grid/WinNum/Num");
            FailNumLabel = t.GetComponent<UILabel>("Content/BG/Grid/FailNum/Num");
            TotalNumLabel = t.GetComponent<UILabel>("Content/BG/Grid/TotalNum/Num");
            WinRateLabel = t.GetComponent<UILabel>("Content/BG/Grid/WinRate/Num");
            MatchCountdownLabel = t.GetComponent<UILabel>("Content/RightStage/MatchingBtn/Countdown");
            MatchButton = t.GetComponent<UIButton>("Content/RightStage/MatchBtn");
            MatchingBtn = t.GetComponent<UIButton>("Content/RightStage/MatchingBtn");
            ProgressBar = t.GetComponent<UIProgressBar>("Content/FxInfo/ProgressBar");

            SeasonAwardItemList = new List<LTShowItem>();
            SeasonAwardItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/BG/AwardItem/Grid/LTShowItem"));
            SeasonAwardItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/BG/AwardItem/Grid/LTShowItem (1)"));
            SeasonAwardItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/BG/AwardItem/Grid/LTShowItem (2)"));

            FxList = new List<GameObject>();
            FxList.Add(t.Find("Content/FxInfo/StageLeft/Icon/fx").gameObject);
            FxList.Add(t.Find("Content/FxInfo/StageLeft/Icon/fx (1)").gameObject);
            FxList.Add(t.Find("Content/FxInfo/StageLeft/Icon/fx (2)").gameObject);
            FxList.Add(t.Find("Content/FxInfo/StageLeft/Icon/fx (3)").gameObject);
            FxList.Add(t.Find("Content/FxInfo/StageLeft/Icon/fx (4)").gameObject);
            FxList.Add(t.Find("Content/FxInfo/StageLeft/Icon/fx (5)").gameObject);
            FxList.Add(t.Find("Content/FxInfo/StageLeft/Icon/fx (6)").gameObject);

            UIButton backButton = t.GetComponent<UIButton>("UINormalFrame/CancelBtn");
            backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));

            t.GetComponent<UIButton>("UINormalFrame/NewCurrency/RankBtn").onClick.Add(new EventDelegate(OnRankBtnClick));
            t.GetComponent<UIButton>("UINormalFrame/NewCurrency/StoreBtn").onClick.Add(new EventDelegate(OnStoreBtnClick));
            t.GetComponent<UIButton>("UINormalFrame/NewCurrency/RankAwardBtn").onClick.Add(new EventDelegate(OnRankAwardBtnClick));
            t.GetComponent<UIButton>("UINormalFrame/NewCurrency/LogBtn").onClick.Add(new EventDelegate(OnLogdBtnClick));
            t.GetComponent<UIButton>("Content/BG/UpBar/RuleBtn").onClick.Add(new EventDelegate(OnRuleButtonClick));
            t.GetComponent<UIButton>("Content/RightStage/MatchBtn").onClick.Add(new EventDelegate(OnMatchBtnClick));
            t.GetComponent<UIButton>("Content/RightStage/MatchingBtn").onClick.Add(new EventDelegate(OnMatchBtnClick));

            t.GetComponent<UIEventTrigger>("Content/RightStage/Box/Box").onClick.Add(new EventDelegate(OnBoxBtnClick));

            t.GetComponent<UIButton>("Content/RightStage/TrusteeshipBtn").onClick.Add(new EventDelegate(OnTrusteeshipBtnClick));
            TrusteeshipOpenSprite = t.Find("Content/RightStage/TrusteeshipBtn/Open").gameObject;

            Instance = this;
        }

        private void OnLogdBtnClick()
        {
             GlobalMenuManager.Instance.Open("ArenaLogUI",LadderManager.LadderBattleLogsDataId);
        }

        public static LadderController Instance;

        public override bool IsFullscreen() { return true; }

        public DynamicUISprite StageLeftSprite;
        public UILabel StageLeftNameLabel, ProgressBarLabel;
        public UILabel LeftEndTimeLabel, RankLabel, HasMatchNumLabel, NeedMatchNumLabel;
        public UILabel WinNumLabel, FailNumLabel, TotalNumLabel, WinRateLabel;
        public UILabel MatchCountdownLabel;
        public UIButton MatchButton;
        public UIButton MatchingBtn;
        public UIProgressBar ProgressBar;
        public List<LTShowItem> SeasonAwardItemList;
        private bool _hasMatch;
        private System.DateTime _startMatchTime;
        public List<GameObject> FxList;

        public GameObject TrusteeshipOpenSprite;

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.ladder_topic,
                FusionTelemetry.GamePlayData.ladder_event_id,FusionTelemetry.GamePlayData.ladder_umengId,"open");
        }

        public override void OnCancelButtonClick()
        {
            if (LadderManager.Instance.IsTrusteeshiping())
            {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                return;
            }
            if (_hasMatch)
            {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleMatchHudController_13754"),
                    (int reseult) =>
                    {
                        switch (reseult)
                        {
                            case 0:
                                {
                                    _hasMatch = false;
                                    OnCancelMatch();
                                    controller.Close();
                                }; break;
                            case 1:
                            case 2:
                                break;
                        }
                    });
            }
            else
            {
                GlobalMenuManager.Instance.RemoveCache("LadderUI");
                base.OnCancelButtonClick();
            }


        }


        public override void OnDestroy()
        {
            Instance = null;
        }

        public override IEnumerator OnAddToStack()
        {
            LadderManager.Instance.ResetBattleTimerData();
            Coroutine coroutine = EB.Coroutines.Run(base.OnAddToStack());

            GlobalMenuManager.Instance.PushCache("LadderUI");

            LadderManager.Instance.GetInfo(delegate {
                GameDataSparxManager.Instance.RegisterListener(LadderManager.LadderInfoDataId, OnLadderInfoListener);
            });

            yield return coroutine;
            yield return null;
            if (LadderManager.Instance.IsTrusteeship)
            {
                yield return new WaitForSeconds(1);
                if(LadderManager.Instance.IsTrusteeship) OpenMatch();
            }
        }

        public override void OnFocus()
        {
            base.OnFocus();
            if (isFocusSetTrusteeship)
            {
                isFocusSetTrusteeship = false;
                if (LadderManager.Instance.IsTrusteeship) OpenMatch();
            }
        }

        public override IEnumerator OnRemoveFromStack()
        {
            StageLeftSprite.spriteName = null;
        GameDataSparxManager.Instance.UnRegisterListener(LadderManager.LadderInfoDataId, OnLadderInfoListener);
            if (_hasMatch)
            {
                LadderManager.Instance.CancelMatch();
                if (MatchingCoroutine != null)
                {
                    StopCoroutine(MatchingCoroutine);
                    MatchingCoroutine = null;
                }
            }
            DestroySelf();
            yield break;
        }

        public override void StartBootFlash()
        {
            UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }

        private void OnLadderInfoListener(string path, INodeData data)
        {
            LadderInfo info = data as LadderInfo;

            if (string.IsNullOrEmpty(info.Stage)) return;

            StageLeftSprite.spriteName = GetStageSpriteName(info.Stage);
            var ts = StageLeftSprite.GetComponent<TweenScale>();
            ts.ResetToBeginning();
            ts.PlayForward();

            for (int i = 1; i < GameStringValue.Ladder_Stage_Names.Length; i++)//青铜无特效因此i初始为0
            {
                FxList[i - 1].CustomSetActive(info.Stage == GameStringValue.Ladder_Stage_Names[i]);
            }

            LTUIUtil.SetText(StageLeftNameLabel, GetStageCharacterName(info.Stage));

            if (!info.Stage.Equals(GameStringValue.Ladder_Stage_Names[GameStringValue.Ladder_Stage_Names.Length - 1])) //不是最后一个段位
            {
                int currentNeed = LadderManager.Instance.Config.GetStageNeedScore(info.Stage);
                int nextNeed = LadderManager.Instance.Config.GetStageNeedScore(info.NextStage);
                float progressValue = 0;

                if ((nextNeed - currentNeed == 0))
                {
                    EB.Debug.LogError("ladder nextNeed - currentNeed = 0");
                    progressValue = 1;
                }
                else
                {
                    progressValue = (info.Point - currentNeed) / (float)(nextNeed - currentNeed);
                }

                ProgressBar.value = progressValue;
                ProgressBarLabel.text = string.Format("{0}/{1}", info.Point, nextNeed);
            }
            else
            {
                ProgressBar.value = 1;
                int need = LadderManager.Instance.Config.GetStageNeedScore(info.Stage);
                ProgressBarLabel.text = string.Format("{0}/{1}", info.Point, need);
            }

            string rankStr;
            if (info.Rank <= 0 || info.Rank > 10000)
                rankStr = "10000+";
            else
                rankStr = info.Rank.ToString();
            LTUIUtil.SetText(RankLabel, rankStr);

            //battle record
            WinNumLabel.text = WinNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString("ID_CHANG"), info.WinNum);
            FailNumLabel.text = FailNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString("ID_CHANG"), info.FailNum);
            TotalNumLabel.text = TotalNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString("ID_CHANG"), info.TotalNum);
            if (info.TotalNum > 0)
                WinRateLabel.text = (int)((info.WinNum / (float)info.TotalNum) * 100) + " %";
            else
                WinRateLabel.text = "0 %";
            WinRateLabel.transform.GetChild(0).GetComponent<UILabel>().text = WinRateLabel.text;
            NeedMatchNumLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LadderController_4223"), LadderManager.Instance.Config.ReceiveAwardNeedMatchNum);
            NeedMatchNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = NeedMatchNumLabel.text;
            if (info.FightNum >= LadderManager.Instance.Config.ReceiveAwardNeedMatchNum)
            {
                HasMatchNumLabel.text = string.Format("{0}/{1}", LadderManager.Instance.Config.ReceiveAwardNeedMatchNum, LadderManager.Instance.Config.ReceiveAwardNeedMatchNum);
                HasMatchNumLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
            }
            else
            {
                HasMatchNumLabel.text = string.Format("{0}/{1}", info.FightNum, LadderManager.Instance.Config.ReceiveAwardNeedMatchNum);
                HasMatchNumLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
            }
            HasMatchNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = HasMatchNumLabel.text;
            ShowSeasonAward(info.Rank);

            if (CountdownCoroutine != null)
                StopCoroutine(CountdownCoroutine);
            CountdownCoroutine = StartCoroutine(CountdownEndTime());
        }

        private void ShowSeasonAward(int rank)
        {
            Hotfix_LT.Data.LadderSeasonAwardTemplate tpl = Hotfix_LT.Data.EventTemplateManager.Instance.GetLadderSeasonAwardTemplate(rank);
            if (tpl != null)
            {
                int award_count = 0;
                if (tpl.item != null)
                {
                    SeasonAwardItemList[award_count].LTItemData = new LTShowItemData(tpl.item.id, tpl.item.count, tpl.item.type);
                    award_count++;
                }
                if (tpl.diamond != 0)
                {
                    SeasonAwardItemList[award_count].LTItemData = new LTShowItemData("hc", tpl.diamond, "res", false);
                    award_count++;
                }
                if (tpl.gold != 0)
                {
                    SeasonAwardItemList[award_count].LTItemData = new LTShowItemData("gold", tpl.gold, "res", false);
                    award_count++;
                }
                if (tpl.ladder_gold != 0)
                {
                    SeasonAwardItemList[award_count].LTItemData = new LTShowItemData("ladder-gold", tpl.ladder_gold, "res", false);
                    award_count++;
                }
                for (int j = 0; j < award_count; ++j)
                {
                    SeasonAwardItemList[j].mDMono.gameObject.CustomSetActive(true);
                }
                for (int j = award_count; j < SeasonAwardItemList.Count; ++j)
                {
                    SeasonAwardItemList[j].mDMono.gameObject.CustomSetActive(false);
                }
                SeasonAwardItemList[0].mDMono.GetComponentInParent<UIGrid>().Reposition();
            }
            else
            {
                SeasonAwardItemList.ForEach(item => item.mDMono.gameObject.CustomSetActive(false));
            }
        }

        private WaitForSeconds wait1 = new WaitForSeconds(1f);
        Coroutine CountdownCoroutine;
        IEnumerator CountdownEndTime()
        {
            while (true)
            {
                yield return wait1;
                int ts = LadderManager.Instance.Info.EndTs - EB.Time.Now;
                System.TimeSpan timespan = System.TimeSpan.FromSeconds(ts);
                string lefttime = string.Format(EB.Localizer.GetString("ID_codefont_in_LadderController_7647"), timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds);
                LTUIUtil.SetText(LeftEndTimeLabel, lefttime);
                if (ts <= 0)
                {
                   EB.Debug.Log("ladder CountdownEndTime getinfo");
                    LadderManager.Instance.GetInfo();
                    yield break;
                }
            }
        }

        static public string GetStageCharacterName(string stageName)
        {
            if (string.IsNullOrEmpty(stageName))
            {
               EB.Debug.LogError("stage_name is null");
                return "";
            }
            return EB.Localizer.GetString("ID_LADDER_STAGE_" + stageName.ToUpper()); ;
        }

        static public string GetStageSpriteName(string stageName)
        {
            return "Ladder_Icon_" + stageName;
        }

        #region eventHandler
        public void OnRankBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTRankListHud", "Ladder");
        }
        
        public void OnStoreBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTStoreUI", "ladder");
        }

        public void OnRankAwardBtnClick()
        {
            var temp =Data.EventTemplateManager.Instance.GetLadderSeasonAwardTemplates();
            List<LTRankAwardData> data = new List<LTRankAwardData>();
            for (int i = 0; i < temp.Count; ++i)
            {
                data.Add(new LTRankAwardData(temp[i].min_rank, temp[i].max_rank, temp[i].gold, temp[i].diamond, new LTShowItemData("ladder-gold", temp[i].ladder_gold, "res"), temp[i].item));
            }
            string tip = EB.Localizer.GetString("ID_LADDER_RANK_AWARD_TIP");
            var ht = Johny.HashtablePool.Claim();
            ht.Add("itemDatas", data);
            ht.Add("tip", tip);

            GlobalMenuManager.Instance.Open("LTGeneralRankAwardUI", ht);
        }

        public void OnTrusteeshipBtnClick()
        {
            LadderManager.Instance.OpenOrCloseTrusteeship(!LadderManager.Instance.IsTrusteeship);
            if(LadderManager.Instance.IsTrusteeship)
            {
                OpenMatch();
            }
            else
            {
                TrusteeshipOpenSprite.CustomSetActive(false);
            }
        }

        public void OpenMatch()
        {
            if (LTPartnerDataManager.Instance.GetOwnPartnerList().Count < 6)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LADDER_PARTNER_NUM_LIMIT"));
                return;
            }
            TrusteeshipOpenSprite.CustomSetActive(true);
            if (!_hasMatch)
            {
                _hasMatch = !_hasMatch;
                LadderManager.Instance.StartMatch(r =>
                {
                    if (r)
                    {
                        _startMatchTime = System.DateTime.Now;
                        MatchingBtn.gameObject.CustomSetActive(true);
                        MatchButton.gameObject.CustomSetActive(false);
                        MatchingCoroutine = StartCoroutine(MatchTimer());
                    }
                    else
                    {
                        _hasMatch = false;
                        if (LadderManager.Instance.IsTrusteeship)
                        {
                            OnTrusteeshipBtnClick();
                        }
                    }
                });
            }
        }

        public void OnMatchBtnClick()
        {
            if (LTPartnerDataManager.Instance.GetOwnPartnerList().Count < 6)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer .GetString("ID_LADDER_PARTNER_NUM_LIMIT"));
                return;
            }
            if (LadderManager.Instance.IsTrusteeshiping()) return;
            _hasMatch = !_hasMatch;
            if (_hasMatch)
            {
                LadderManager.Instance.StartMatch(r =>
                {
                    if (r)
                    {
                        _startMatchTime = System.DateTime.Now;
                        MatchingBtn.gameObject.CustomSetActive(true);
                        MatchButton.gameObject.CustomSetActive(false);
                        MatchingCoroutine = StartCoroutine(MatchTimer());
                    }
                    else
                    {
                        _hasMatch = false;
                        if (LadderManager.Instance.IsTrusteeship)
                        {
                            OnTrusteeshipBtnClick();
                        }
                    }
                });
            }
            else
            {
                OnCancelMatch();
            }
        }

        private void OnCancelMatch()
        {
            if (LadderManager.Instance.IsTrusteeshiping()) return;
            LadderManager.Instance.CancelMatch();
            if (MatchingCoroutine != null)
            {
                StopCoroutine(MatchingCoroutine);
                MatchingCoroutine = null;
            }
            MatchingBtn.gameObject.CustomSetActive(false);
            MatchButton.gameObject.CustomSetActive(true);
        }

        bool isFocusSetTrusteeship = false;
        public void StopMatch(bool isGiveUp = false)
        {
            if (MatchingCoroutine != null)
            {
                StopCoroutine(MatchingCoroutine);
                MatchingCoroutine = null;
                _hasMatch = false;
                MatchButton.gameObject.CustomSetActive(true);
                MatchingBtn.gameObject.CustomSetActive(false);
                if (isGiveUp)
                {
                    LadderManager.Instance.GetInfo();
                    TrusteeshipOpenSprite.CustomSetActive(false);
                    isFocusSetTrusteeship = true;
                }
                else
                {
                    MessageTemplateManager.ShowMessage(902266);
                    if (LadderManager.Instance.IsTrusteeship)
                    {
                        OnTrusteeshipBtnClick();
                    }
                    else
                    {
                        TrusteeshipOpenSprite.CustomSetActive(false);
                    }
                }
                //if (LadderManager.Instance.IsTrusteeship)
                //{
                //    OnTrusteeshipBtnClick();
                //}
                //else
                //{
                //    TrusteeshipOpenSprite.CustomSetActive(false);
                //}
            }
            SceneLogic.SceneState = SceneLogic.eSceneState.SceneLoop;

        }

        Coroutine MatchingCoroutine;
        IEnumerator MatchTimer()
        {
            while (true)
            {
                System.TimeSpan ts = System.DateTime.Now - _startMatchTime;

                LTUIUtil.SetText(MatchCountdownLabel, ((int)ts.TotalSeconds) + EB.Localizer.GetString("ID_SECOND"));
                yield return null;
            }
        }

        public void OnBoxBtnClick()
        {
            //读取当前天梯奖励
            ArrayList aList = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("LadderReward")) as ArrayList;

            List<LTShowItemData> showItemsList = new List<LTShowItemData>();
            for (int i = 0; i < aList.Count; i++)
            {
                string id = EB.Dot.String("data", aList[i], string.Empty);
                int count = EB.Dot.Integer("quantity", aList[i], 0);
                string type = EB.Dot.String("type", aList[i], string.Empty);
                if (!string.IsNullOrEmpty(id))
                {
                    LTShowItemData showItemData = new LTShowItemData(id, count, type);
                    showItemsList.Add(showItemData);
                }
            }
            LadderManager.Instance.Info.EverydayAward = showItemsList;
            //
            if (LadderManager.Instance.Info.FightNum >= LadderManager.Instance.Config.ReceiveAwardNeedMatchNum && !LadderManager.Instance.Info.HasReceiveEverydayAward)
            {
                LadderManager.Instance.ReceiveEveryAward(delegate (bool successful)
                {
                    if (successful)
                    {
                        LadderManager.Instance.Info.HasReceiveEverydayAward = true;
                        GlobalMenuManager.Instance.Open("LTShowRewardView", LadderManager.Instance.Info.EverydayAward);
                    }
                });
                return;
            }

            string tip = EB.Localizer.GetString("ID_codefont_in_LadderController_11750");
            if (!LadderManager.Instance.Info.HasReceiveEverydayAward)
            {
                int leftMatchNum = LadderManager.Instance.Config.ReceiveAwardNeedMatchNum - LadderManager.Instance.Info.FightNum;
                if (leftMatchNum <= 0)
                {
                   EB.Debug.LogError("leftMatchNum<=0");
                    leftMatchNum = 0;
                }
                tip = string.Format(EB.Localizer.GetString("ID_codefont_in_LadderController_12072"), leftMatchNum);
            }
            var ht = Johny.HashtablePool.Claim();
            ht.Add("data", LadderManager.Instance.Info.EverydayAward);
            ht.Add("tip", tip);
            GlobalMenuManager.Instance.Open("LTRewardShowUI", ht);
        }

        public void OnRuleButtonClick()
        {
            string text = EB.Localizer.GetString("ID_LADDER_RULES");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }
        #endregion
    }
}
