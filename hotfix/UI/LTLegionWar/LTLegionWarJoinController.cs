using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    public enum LTLegionWarTabValue
    {
        Qualify = 0, Semifinal = 1, Final = 2
    }

    public class LTLegionWarJoinController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TabController = t.GetComponent<UITabController>("Content/BG/Top/LeftList");
            ViewTween = t.GetComponent<TweenAlpha>("Content");
            JoinButton = t.FindEx("Content/BG/Bottom/JoinButton").gameObject;
            EndButton = t.FindEx("Content/BG/Bottom/EndButton").gameObject;
            TimeLabel = t.GetComponent<UILabel>("Content/BG/Bottom/Time");

            QualifyCellList = new List<QualifyRankCell>();
            QualifyCellList.Add(t.GetMonoILRComponent<QualifyRankCell>("Content/QualifyView/NewRankList/LegionItem"));
            QualifyCellList.Add(t.GetMonoILRComponent<QualifyRankCell>("Content/QualifyView/NewRankList/LegionItem (1)"));
            QualifyCellList.Add(t.GetMonoILRComponent<QualifyRankCell>("Content/QualifyView/NewRankList/LegionItem (2)"));
            QualifyCellList.Add(t.GetMonoILRComponent<QualifyRankCell>("Content/QualifyView/NewRankList/LegionItem (3)"));

            SemiFinalCellList = new List<FinalRankCell>();
            SemiFinalCellList.Add(t.GetMonoILRComponent<FinalRankCell>("Content/SemiFinalView/NewRankList/0"));
            SemiFinalCellList.Add(t.GetMonoILRComponent<FinalRankCell>("Content/SemiFinalView/NewRankList/1"));
            SemiFinalCellList.Add(t.GetMonoILRComponent<FinalRankCell>("Content/SemiFinalView/NewRankList/2"));
            SemiFinalCellList.Add(t.GetMonoILRComponent<FinalRankCell>("Content/SemiFinalView/NewRankList/3"));

            FinalCellList = new List<FinalRankCell>();
            FinalCellList.Add(t.GetMonoILRComponent<FinalRankCell>("Content/FinalView/NewRankList/0"));
            FinalCellList.Add(t.GetMonoILRComponent<FinalRankCell>("Content/FinalView/NewRankList/1"));

            SemiFinalCellCheckBtns = new List<GameObject>();
            SemiFinalCellCheckBtns.Add(t.FindEx("Content/SemiFinalView/NewRankList/VS (0)/CheckButton").gameObject);
            SemiFinalCellCheckBtns.Add(t.FindEx("Content/SemiFinalView/NewRankList/VS (1)/CheckButton").gameObject);

            FinalCellCheckBtn = t.FindEx("Content/FinalView/NewRankList/VS/CheckButton").gameObject;
            callBack = false;

            AwardList = new List<LTShowItem>();
            AwardList.Add(t.GetMonoILRComponent<LTShowItem>("Content/BG/Bottom/Award/Grid/0"));
            AwardList.Add(t.GetMonoILRComponent<LTShowItem>("Content/BG/Bottom/Award/Grid/1"));
            AwardList.Add(t.GetMonoILRComponent<LTShowItem>("Content/BG/Bottom/Award/Grid/2"));
            AwardList.Add(t.GetMonoILRComponent<LTShowItem>("Content/BG/Bottom/Award/Grid/3"));

            LockObj = new List<GameObject>();
            LockObj.Add(t.FindEx("Content/BG/Top/LeftList/LeftButtonGrid/1_QualifyGame/EnchantTab1/Label/Lock").gameObject);
            LockObj.Add(t.FindEx("Content/BG/Top/LeftList/LeftButtonGrid/2_Semifinal/EnchantTab1/Label/Lock").gameObject);
            LockObj.Add(t.FindEx("Content/BG/Top/LeftList/LeftButtonGrid/3_Final/EnchantTab1/Label/Lock").gameObject);

            controller.backButton = t.GetComponent<UIButton>("BG/CancelBtn");
            t.GetComponent<UIButton>("Content/BG/Top/Info").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Content/BG/Bottom/JoinButton").onClick.Add(new EventDelegate(OnEnterBtnClick));
            t.GetComponent<UIButton>("Content/SemiFinalView/NewRankList/VS (0)/CheckButton").onClick.Add(new EventDelegate(() => OnCheckButtonClick(SemiFinalCellList[0], SemiFinalCellList[1])));
            t.GetComponent<UIButton>("Content/SemiFinalView/NewRankList/VS (1)/CheckButton").onClick.Add(new EventDelegate(() => OnCheckButtonClick(SemiFinalCellList[2], SemiFinalCellList[3])));
            t.GetComponent<UIButton>("Content/FinalView/NewRankList/VS/CheckButton").onClick.Add(new EventDelegate(() => OnCheckButtonClick(FinalCellList[0], FinalCellList[1])));
            t.GetComponent<UIEventTrigger>("Content/BG/Top/LeftList/LeftButtonGrid/1_QualifyGame/EnchantTab1").onPress.Add(new EventDelegate(OnClickQualifyGame));
            t.GetComponent<UIEventTrigger>("Content/BG/Top/LeftList/LeftButtonGrid/2_Semifinal/EnchantTab1").onPress.Add(new EventDelegate(OnClickSemifinal));
            t.GetComponent<UIEventTrigger>("Content/BG/Top/LeftList/LeftButtonGrid/3_Final/EnchantTab1").onPress.Add(new EventDelegate(OnClickFinal));
        }

        public override bool IsFullscreen() { return true; }
        public UITabController TabController;
        public TweenAlpha ViewTween;
        public GameObject JoinButton, EndButton;
        private bool[] TabLocks = new bool[3];
        public UILabel TimeLabel;
        public List<QualifyRankCell> QualifyCellList;
        public List<FinalRankCell> SemiFinalCellList, FinalCellList;

        public List<GameObject> SemiFinalCellCheckBtns;
        public GameObject FinalCellCheckBtn;

        private bool isOpen = false;
        private bool isTimeLimit = false;

        public List<LTShowItem> AwardList;
        private void SetAwardList(LTLegionWarTabValue Channel)//根据界面类型设置奖励
        {
            Hotfix_LT.Data.AllianceWarReward winLists = Hotfix_LT.Data.AllianceTemplateManager.Instance.GetWarReward((int)Channel + 1, 1);
            for (int i = 0; i < AwardList.Count; i++)
            {
                if (winLists != null && i < winLists.Rewards.Count)
                {
                    AwardList[i].LTItemData = winLists.Rewards[i];
                    AwardList[i].mDMono.gameObject.CustomSetActive(true);
                }
                else { AwardList[i].mDMono.gameObject.CustomSetActive(false); }
            }
        }
        private void SetBtn(int index = 0)//根据界面类型设置按钮 0未开始，1可参加，2已结束
        {
            switch (index)
            {
                case 0:
                    {
                        string str = string.Empty;
                        if (isTimeLimit) str = string.Format("[FD3A41FF]{0} {1}[-]", EB.Localizer.GetString("ID_NEXT_WEEK"), EB.Localizer.GetString("ID_uifont_in_LTMainMenu_Label_13"));
                        else string.Format("[FD3A41FF]{0}[-]", GetTimeStr());
                        TimeLabel.text = TimeLabel.transform.GetChild(0).GetComponent<UILabel>().text = str;
                        JoinButton.CustomSetActive(false);
                        EndButton.transform.GetChild(0).GetComponent<UILabel>().text = EndButton.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_LTLegionWarJoinController_1816");
                        EndButton.CustomSetActive(true);
                    }
                    break;
                case 1:
                    {
                        TimeLabel.text = TimeLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("[3FFCC0FF]{0}[-]", GetTimeStr());
                        int ts = 0;
                        if (LTLegionWarTimeLine.Instance != null)
                        {
                            ts = (int)((_LTLegionWarTabValue == LTLegionWarTabValue.Semifinal) ? LTLegionWarTimeLine.Instance.SemiFinalStopTime : LTLegionWarTimeLine.Instance.FinalStopTime) - EB.Time.Now;
                        }
                        bool btnType = ts >= 0;
                        JoinButton.CustomSetActive(btnType);
                        if (!btnType) EndButton.transform.GetChild(0).GetComponent<UILabel>().text = EndButton.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_LTLegionWarFinalController_4204");
                        EndButton.CustomSetActive(!btnType);
                    }
                    break;
                case 2:
                    {
                        TimeLabel.text = TimeLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("[FD3A41FF]{0}[-]", GetTimeStr());
                        JoinButton.CustomSetActive(false);
                        EndButton.transform.GetChild(0).GetComponent<UILabel>().text = EndButton.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_LTLegionWarFinalController_4850");
                        EndButton.CustomSetActive(true);
                    }
                    break;
                default:
                    {
                        JoinButton.CustomSetActive(false);
                        EndButton.CustomSetActive(false);
                        EB.Debug.LogError("SetBtn is Error!");
                    }
                    break;
            }
        }

        private LTLegionWarTabValue _LTLegionWarTabValue;//选择界面类型 预赛/半决/决赛
        public LTLegionWarTabValue CurrentTabValue
        {
            set
            {
                _LTLegionWarTabValue = value;
                switch (_LTLegionWarTabValue)
                {
                    case LTLegionWarTabValue.Qualify:
                        SetQualifyRank(); break;
                    case LTLegionWarTabValue.Semifinal:
                        SetSemifinalRank(); break;
                    case LTLegionWarTabValue.Final:
                        SetFinalRank(); break;
                }
            }
            get
            {
                return _LTLegionWarTabValue;
            }
        }

        public void OnClickQualifyGame()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick", !isFirstUpdate);
            if (TabLocks[0]) return;
            TabController.SelectTab(0);
            CurrentTabValue = LTLegionWarTabValue.Qualify;
        }
        public void OnClickSemifinal()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick", !isFirstUpdate);
            if (TabLocks[1]) return;
            TabController.SelectTab(1);
            CurrentTabValue = LTLegionWarTabValue.Semifinal;
        }
        public void OnClickFinal()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick", !isFirstUpdate);
            if (TabLocks[2]) return;
            TabController.SelectTab(2);
            CurrentTabValue = LTLegionWarTabValue.Final;
        }


        public override void OnDestroy()
        {
            callBack = false;
            base.OnDestroy();
        }

        public static bool callBack = false;
        public void ShowUI()
        {
            if (!LTLegionWarManager.Instance.IsOpenWarTime())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleMatchHudController_10973"));
                isTimeLimit = true;
                UpdateView();
                return;
            }
            else
            {
                isTimeLimit = false;
            }

            LTLegionWarManager.Instance.CheckState(delegate (bool success)
            {
                if (LTLegionWarTimeLine.Instance != null)
                {
                    LTLegionWarTimeLine.Instance.StartGetTimeNow();
                }
                if (success)
                {
                    switch (LTLegionWarManager.Instance.serveCurState)
                    {
                        case 1:
                            {
                                LTLegionWarManager.Instance.GetLegionRank(UpdateView);
                            }
                            break;
                        default:
                            {
                                LTLegionWarManager.Instance.InitSemiFinal(UpdateView);
                            }
                            break;
                    }
                }
                else
                {
                    LTLegionWarManager.Instance.InitSemiFinal(UpdateView);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_NOT_IN_ACTIVITY_TIME"));
                }
            });
        }

        private void OnLegionRankList()
        {
            if (LTLegionWarManager.Instance.serveCurState == 1)
            {
                List<LegionRankData> temp = LTLegionWarManager.Instance.LegionRankList.LegionRankList;
                for (int i = 0; i < QualifyCellList.Count; i++)
                {
                    if (i < temp.Count) QualifyCellList[i].Fill(temp[i]);
                }
            }
            else
            {
                List<LegionRankData> temp = LTLegionWarManager.Instance.SemiOrFinalLegionList.QualifyLegionRankList;
                for (int i = 0; i < QualifyCellList.Count; i++)
                {
                    if (i < temp.Count) QualifyCellList[i].Fill(temp[i]);
                }
            }
        }

        private void OnSemiFinalInit()
        {
            List<LegionRankData> rankList = LTLegionWarManager.Instance.SemiOrFinalLegionList.SemiLegionRankList;
            if (rankList.Count < 4)
                for (int i = rankList.Count; i < 4; i++) rankList.Add(new LegionRankData());
            else if (rankList.Count > 4)
                EB.Debug.LogError("LTLegionWarManager.Instance.SemiFinalLegionList.SemiLegionRankList.Count > 4");

            SemiFinalCellList[0].item = rankList[0];
            SemiFinalCellList[1].item = rankList[3];
            SemiFinalCellList[2].item = rankList[1];
            SemiFinalCellList[3].item = rankList[2];

            for (var i = 0; i < SemiFinalCellList.Count; i++)
            {
                SemiFinalCellList[i].Fill();
            }
        }

        private void OnFinalInit()
        {
            List<LegionRankData> rankList = new List<LegionRankData>();
            var dataList = LTLegionWarManager.Instance.SemiOrFinalLegionList.FinalLegionRankList;
            for (var i = 0; i < dataList.Count; i++)
            {
                var Data = dataList[i];
                if (Data != null)
                {
                    rankList.Add(Data);
                };
            }
            if (rankList.Count < 2)
                for (int i = rankList.Count; i < 2; i++) rankList.Add(new LegionRankData());
            else if (rankList.Count > 2)
                EB.Debug.LogError("LTLegionWarManager.Instance.SemiFinalLegionList.FinalLegionRankList.Count > 2");

            for (int i = 0; i < rankList.Count; i++)
            {
                if (i < FinalCellList.Count)
                {
                    FinalCellList[i].item = rankList[i];
                }
            }
            for (int i = 0; i < rankList.Count; i++)
            {
                if (i < FinalCellList.Count)
                {
                    FinalCellList[i].Fill();
                }
            }
        }


        private void SetQualifyRank()
        {
            SetAwardList(LTLegionWarTabValue.Qualify);
            OnLegionRankList();
            if (LTLegionWarManager.Instance.serveCurState != 0)
            {
                switch (LTLegionWarManager.Instance.serveCurState)
                {
                    case 1: SetBtn(1); break;
                    case 2: SetBtn(2); break;
                    case 3: SetBtn(2); break;
                    default: { EB.Debug.LogError("LTLegionWarManager.Instance.serveCurState is Error!"); } break;
                }
            }
            else
            {
                if ((int)LTLegionWarTimeLine.TimeNow >=1 || isTimeLimit)
                {
                    SetBtn(0);
                }
                else
                {
                    SetBtn(2);
                }
            }
            LTLegionWarFinalController._WarType = WarType.Qualify;
        }

        private void SetSemifinalRank()
        {
            SetAwardList(LTLegionWarTabValue.Semifinal);
            OnSemiFinalInit();
            if (LTLegionWarManager.Instance.serveCurState != 0)
            {
                switch (LTLegionWarManager.Instance.serveCurState)
                {
                    case 1: SetBtn(0); break;
                    case 2: SetBtn(1); break;
                    case 3: SetBtn(2); break;
                    default: { EB.Debug.LogError("LTLegionWarManager.Instance.serveCurState is Error!"); } break;
                }

            }
            else
            {
                if ((int)LTLegionWarTimeLine.TimeNow <= 3) SetBtn(0);
                else SetBtn(2);
            }

            for (int i = 0; i < SemiFinalCellCheckBtns.Count; i++)
            {
                SemiFinalCellCheckBtns[i].CustomSetActive((int)LTLegionWarTimeLine.TimeNow >= 4);//LTLegionWarManager.Instance.serveCurState == 0);战报
            }
            LTLegionWarFinalController._WarType = WarType.Semifinal;
        }

        private void SetFinalRank()
        {
            SetAwardList(LTLegionWarTabValue.Final);
            OnFinalInit();
            if (LTLegionWarManager.Instance.serveCurState != 0)
            {
                switch (LTLegionWarManager.Instance.serveCurState)
                {
                    case 1: SetBtn(0); break;
                    case 2: SetBtn(0); break;
                    case 3: SetBtn(1); break;
                    default: { EB.Debug.LogError("LTLegionWarManager.Instance.serveCurState is Error!"); } break;
                }
            }
            else
            {
                if ((int)LTLegionWarTimeLine.TimeNow <= 5) SetBtn(0);
                else SetBtn(2);
            }

            FinalCellCheckBtn.CustomSetActive((int)LTLegionWarTimeLine.TimeNow >= 6);//LTLegionWarManager.Instance.serveCurState == 0);战报
            LTLegionWarFinalController._WarType = WarType.Final;
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            ShowUI();
            if (!isTimeLimit)
            {               
                string savestring = PlayerPrefs.GetString(LTLegionWarManager.Instance.LocalSavekey);
                if (string.IsNullOrEmpty(savestring) || !int.TryParse(savestring, out int time) || (EB.Time.Now - time) > 86400)
                {
                    PlayerPrefs.SetString(LTLegionWarManager.Instance.LocalSavekey, EB.Time.Now.ToString());
                    LegionLogic.GetInstance().IsOpenLegionBattle();
                }
            }
            GlobalMenuManager.Instance.PushCache("LTLegionWarJoinView");
            isOpen = true;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            if (LTLegionWarTimeLine.Instance != null)
            {
                LTLegionWarTimeLine.Instance.StopGetTimeNow();
            }
            isOpen = false;
            isFirstUpdate = true;
            ViewTween.value = 0;
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
        }

        public override void OnFocus()
        {
            base.OnFocus();
            if (!isTimeLimit) CurrentTabValue = CurrentTabValue;
        }

        private bool isFirstUpdate = true;

        public void UpdateView()
        {
            callBack = true;
            if (!isOpen && !isTimeLimit) return;
            switch (LTLegionWarManager.Instance.serveCurState)
            {
                case 1://预赛
                    {
                        LockUpdate(1);
                        OnClickQualifyGame();
                    }
                    break;
                case 2://半决
                    {
                        LockUpdate(2);
                        OnClickSemifinal();
                    }
                    break;
                case 3://决赛
                    {
                        LockUpdate(3);
                        OnClickFinal();
                    }
                    break;
                default://根据存在的列表获得比赛状态
                    {
                        if (LTLegionWarManager.Instance.SemiOrFinalLegionList.FinalLegionRankList.Count != 0)
                        {
                            LockUpdate(3);
                            OnClickFinal();
                        }
                        else if (LTLegionWarManager.Instance.SemiOrFinalLegionList.SemiLegionRankList.Count != 0)
                        {
                            LockUpdate(2);
                            OnClickSemifinal();
                        }
                        else
                        {
                            LockUpdate(1);
                            OnClickQualifyGame();
                        }
                    }
                    break;
            }
            isFirstUpdate = false;
            ViewTween.ResetToBeginning();
            ViewTween.PlayForward();
        }

        public List<GameObject> LockObj;
        private Vector3 lockVec3 = new Vector3(30, 3, 0);
        private Vector3 noralVec3 = new Vector3(0, 3, 0);
        private void LockUpdate(int index = 0)
        {
            for (int i = 0; i < TabLocks.Length; i++)
            {
                TabLocks[i] = (i >= index);
                LockObj[i].CustomSetActive(TabLocks[i]);
                TabController.TabLibPrefabs[i].TabTitle.transform.localPosition = (TabLocks[i]) ? lockVec3 : noralVec3;

            }
        }

        public void OnEnterBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!(LTLegionWarManager.Instance.serveCurState > 0 && LTLegionWarManager.Instance.serveCurState < 4))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_NOT_IN_ACTIVITY_TIME"));
                return;
            }
            if (!LegionModel.GetInstance().isJoinedLegion)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTLegionWarJoinController_13125"));
                return;
            }

            LTLegionWarFinalController.Legions = new List<LegionRankData> {
                new LegionRankData(),new LegionRankData()
            };
            int qualify = Qualify();
            if (qualify < 9 && LadderManager.Instance.Info.Rank == 0) LadderManager.Instance.GetInfo();
            switch (qualify)
            {
                case 0:
                    {//0-3，玩家有半决赛资格
                        LTLegionWarFinalController.Legions[0] = SemiFinalCellList[0].item;
                        LTLegionWarFinalController.Legions[1] = SemiFinalCellList[1].item;
                        LTLegionWarManager.Instance.SemiFinalField = 0;
                        GlobalMenuManager.Instance.Open("LTLegionWarFinalView");
                    }; break;
                case 1:
                    {
                        LTLegionWarFinalController.Legions[0] = SemiFinalCellList[2].item;
                        LTLegionWarFinalController.Legions[1] = SemiFinalCellList[3].item;
                        LTLegionWarManager.Instance.SemiFinalField = 1;
                        GlobalMenuManager.Instance.Open("LTLegionWarFinalView");
                    }; break;
                case 2:
                    {
                        LTLegionWarFinalController.Legions[0] = SemiFinalCellList[3].item;
                        LTLegionWarFinalController.Legions[1] = SemiFinalCellList[2].item;
                        LTLegionWarManager.Instance.SemiFinalField = 1;
                        GlobalMenuManager.Instance.Open("LTLegionWarFinalView");
                    }; break;
                case 3:
                    {
                        LTLegionWarFinalController.Legions[0] = SemiFinalCellList[1].item;
                        LTLegionWarFinalController.Legions[1] = SemiFinalCellList[0].item;
                        LTLegionWarManager.Instance.SemiFinalField = 0;
                        GlobalMenuManager.Instance.Open("LTLegionWarFinalView");
                    }; break;
                case 4:
                    {//4-5,玩家有决赛资格
                        LTLegionWarFinalController.Legions[0] = FinalCellList[0].item;
                        LTLegionWarFinalController.Legions[1] = FinalCellList[1].item;
                        LTLegionWarManager.Instance.SemiFinalField = 0;
                        GlobalMenuManager.Instance.Open("LTLegionWarFinalView");
                    }; break;
                case 5:
                    {
                        LTLegionWarFinalController.Legions[0] = FinalCellList[1].item;
                        LTLegionWarFinalController.Legions[1] = FinalCellList[0].item;
                        LTLegionWarManager.Instance.SemiFinalField = 0;
                        GlobalMenuManager.Instance.Open("LTLegionWarFinalView");
                    }; break;
                case 9:
                    {//预赛资格
                        GlobalMenuManager.Instance.Open("LTLegionWarQualifyView");
                    }; break;
                case 99:
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTLegionWarJoinController_17668"));
                        EB.Debug.Log(EB.Localizer.GetString("ID_codefont_in_LTLegionWarJoinController_17722"));
                    }; break;
                default:
                    {
                        EB.Debug.LogError(EB.Localizer.GetString("ID_codefont_in_LTLegionWarJoinController_17821"));
                    }; break;
            };
        }

        private int Qualify()
        {
            if (CurrentTabValue == LTLegionWarTabValue.Qualify)
            {
                return 9;
            }
            else if (CurrentTabValue == LTLegionWarTabValue.Semifinal)
            {
                List<LegionRankData> SemiFinalLegions = LTLegionWarManager.Instance.SemiOrFinalLegionList.SemiLegionRankList;
                for (int i = 0; i < SemiFinalLegions.Count; i++)
                {
                    if (SemiFinalLegions[i] != null && LegionModel.GetInstance().legionData.legionID == SemiFinalLegions[i].id)
                    {
                        return i;
                    }
                }
            }
            else if (CurrentTabValue == LTLegionWarTabValue.Final)
            {
                List<LegionRankData> FinalLegions = new List<LegionRankData>();
                var dataList = LTLegionWarManager.Instance.SemiOrFinalLegionList.SemiLegionRankList;
                for (var i = 0; i < dataList.Count; i++)
                {
                    var Data = dataList[i];
                    if (Data != null && Data.enter)
                    {
                        FinalLegions.Add(LTLegionWarManager.Instance.SemiOrFinalLegionList.Find(Data.id));
                    };
                }
                for (int i = 0; i < FinalLegions.Count; i++)
                {
                    if (LegionModel.GetInstance().legionData.legionID == FinalLegions[i].id)
                    {
                        return i + 4;
                    }
                }
            }
            return 99;
        }

        public void OnCheckButtonClick(FinalRankCell item1, FinalRankCell item2)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");

            if (item1.item == null ||
                item2.item == null ||
                !LTLegionWarManager.Instance.WatchLogList.CanShowFieldWatchLog(item1.item.id, item2.item.id))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LEGION_WAR_WATCHLOG_FAIL"));
                return;
            }

            List<LegionRankData> CheckLegion = new List<LegionRankData>();

            if (item1.mDMono.transform.name.Equals("0"))
                LTLegionWarManager.Instance.SemiFinalField = 0;
            else if (item1.mDMono.transform.name == "2")
                LTLegionWarManager.Instance.SemiFinalField = 1;
            else
                LTLegionWarManager.Instance.SemiFinalField = -1;

            CheckLegion.Add(item1.item);
            CheckLegion.Add(item2.item);

            GlobalMenuManager.Instance.Open("LTLegionWarFinalView", CheckLegion);
        }

        public override void OnCancelButtonClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTLegionWarJoinView");
            base.OnCancelButtonClick();
        }

        public void OnRuleBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            string rule = EB.Localizer.GetString("ID_RULE_ALLIANCEWAR");
            GlobalMenuManager.Instance.Open("LTRuleUIView", rule);
        }

        #region GM

        private string GetTimeStr()
        {
            LTLegionWarTime[] data;
            switch (_LTLegionWarTabValue)
            {
                case LTLegionWarTabValue.Qualify: data = LTLegionWarManager.Instance.WarOpenTime.QualifyOpenTime; break;
                case LTLegionWarTabValue.Semifinal: data = LTLegionWarManager.Instance.WarOpenTime.SemiOpenTime; break;
                case LTLegionWarTabValue.Final: data = LTLegionWarManager.Instance.WarOpenTime.FinalOpenTime; break;
                default: { data = new LTLegionWarTime[2];EB.Debug.LogError("_LTLegionWarTabValue not find!"); } break;
            }

            System.DateTime startD = Hotfix_LT.Data.ZoneTimeDiff.GetDateTime(data[0].hour, data[0].minute);
            System.DateTime stopD = Hotfix_LT.Data.ZoneTimeDiff.GetDateTime(data[1].hour, data[1].minute);

            string day = getDay(data[0].day);
            string str = string.Format("{0} {1:t}-{2:t}", day, startD, stopD);
            return str;
        }
        private string getDay(int i)
        {
            return LegionPageActivity.getDay(i);
        }

        public void OnOpenFinalWar()
        {
            LTLegionWarManager.Instance.startSemiOrFinalBattle();
        }

        public void OpenServe(Transform btn)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            int i = -1;
            int.TryParse(btn.name, out i);
            if (i != -1) LTLegionWarManager.Instance.OnOpenAllianceWar(i, delegate { ShowUI(); });
        }
        public void CloseServe(Transform btn)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            int i = -1;
            int.TryParse(btn.name, out i);
            if (i != -1) LTLegionWarManager.Instance.OnCloseAllianceWar(i, delegate { ShowUI(); });
        }

        public void debugAutoCombat()
        {
            LTLegionWarManager.Instance.debugAutoCombat();
        }

        public void GMResetWar()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            LTLegionWarManager.Instance.GetResetWar(delegate { ShowUI(); });
        }
        public void ChangeTeam(Transform btn)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            LTLegionWarManager.Instance.ChangeTeams(int.Parse(btn.name), delegate { ShowUI(); });
        }

        #endregion
    }
}
