using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI {
    public enum LegionWarField
    {
        Water = 0,
        Wind = 1,
        Fire = 2,
        None = 3
    }

    public enum WarType
    {
        Qualify =0,
        Semifinal = 1,
        Final =2
    }
    
    public class LTLegionWarFinalController : UIControllerHotfix, IHotfixUpdate
    {
        public override void Awake() {
            base.Awake();
            _Instance = this;
            var t = controller.transform;
            PlayerLadderRanking = t.GetComponent<UILabel>("Edge/TopRight/MyRank");
            UsCell = t.GetMonoILRComponent<FinalRankCell>("Content/FinalView/NewRankList/LegionItem");
            EnemyCell = t.GetMonoILRComponent<FinalRankCell>("Content/FinalView/NewRankList/LegionItem (1)");

            BtnNameList = new List<Transform>();
            BtnNameList.Add(t.GetComponent<Transform>("Edge/Bottom/Battlefield/Water/EnchantTab1/Label"));
            BtnNameList.Add(t.GetComponent<Transform>("Edge/Bottom/Battlefield/Wind/EnchantTab1/Label"));
            BtnNameList.Add(t.GetComponent<Transform>("Edge/Bottom/Battlefield/Fire/EnchantTab1/Label"));


            NumLabel = new List<UILabel>();
            NumLabel.Add(t.GetComponent<UILabel>("Edge/Bottom/Battlefield/Water/EnchantTab1/Num"));
            NumLabel.Add(t.GetComponent<UILabel>("Edge/Bottom/Battlefield/Wind/EnchantTab1/Num"));
            NumLabel.Add(t.GetComponent<UILabel>("Edge/Bottom/Battlefield/Fire/EnchantTab1/Num"));


            TypeBtnList = new List<GameObject>();
            TypeBtnList.Add(t.GetComponent<Transform>("Edge/Bottom/Battlefield/Water/EnchantTab2").gameObject);
            TypeBtnList.Add(t.GetComponent<Transform>("Edge/Bottom/Battlefield/Wind/EnchantTab2").gameObject);
            TypeBtnList.Add(t.GetComponent<Transform>("Edge/Bottom/Battlefield/Fire/EnchantTab2").gameObject);

            UsFinalItems = new FinalPlayerItem[5];
            UsFinalItems[0] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Left/1/Container");
            UsFinalItems[1] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Left/1 (1)/Container");
            UsFinalItems[2] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Left/1 (2)/Container");
            UsFinalItems[3] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Left/1 (3)/Container");
            UsFinalItems[4] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Left/1 (4)/Container");

            EnemyFinalItems = new FinalPlayerItem[5];
            EnemyFinalItems[0] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Right/2/Container");
            EnemyFinalItems[1] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Right/2 (1)/Container");
            EnemyFinalItems[2] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Right/2 (2)/Container");
            EnemyFinalItems[3] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Right/2 (3)/Container");
            EnemyFinalItems[4] = t.GetMonoILRComponent<FinalPlayerItem>("Content/PlayerInfo/Right/2 (4)/Container");


            isWarOver = false;
            isWatchLog = false;
            StatuLabel = t.GetComponent<UILabel>("Edge/Top/StatuLabel");
            TimeLabel = t.GetComponent<UILabel>("Edge/Top/TimeLabel");

            controller.backButton = t.GetComponent<UIButton>("Edge/TopLeft/CancelBtn");

            t.GetComponent<UIButton>("Edge/TopLeft/Chat").onClick.Add(new EventDelegate(OnChatBtnClick));
            t.GetComponent<UIButton>("Edge/TopLeft/Friend").onClick.Add(new EventDelegate(OnFriendBtnClick));
            t.GetComponent<UIButton>("Edge/TopRight/RuleBtn").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Edge/Bottom/Battlefield/Water/EnchantTab1").onClick.Add(new EventDelegate(OnWaterTabClick));
            t.GetComponent<UIButton>("Edge/Bottom/Battlefield/Wind/EnchantTab1").onClick.Add(new EventDelegate(OnWindTabClick));
            t.GetComponent<UIButton>("Edge/Bottom/Battlefield/Fire/EnchantTab1").onClick.Add(new EventDelegate(OnFireTabClick));
        }


    
        public override bool IsFullscreen() { return true; }
    
        private static LTLegionWarFinalController _Instance;
        public static LTLegionWarFinalController Instance
        {
            get
            {
                return _Instance;
            }
        }
    
        private bool isOpen;
        private float updateTimer = 1;
    
        /// <summary>玩家列表</summary>
        public FinalPlayerItem[] UsFinalItems, EnemyFinalItems;
        /// <summary>玩家天梯排名label</summary>
        public UILabel PlayerLadderRanking;
        /// <summary>对战军团</summary>
        public FinalRankCell UsCell, EnemyCell;
        public List<Transform> BtnNameList;
        /// <summary>各个战场的玩家数量</summary>
        public List<UILabel> NumLabel;
        /// <summary>各个战场的按钮列表</summary>
        public List<GameObject> TypeBtnList;
        /// <summary>当前选择的战场类型</summary>
        public LegionWarField WarFiled;
        /// <summary>军团战是否已结束</summary>
        public bool isWarOver;
        /// <summary>是否为观看战报</summary>
        public bool isWatchLog;
        /// <summary>战斗状态或倒计时的Label</summary>
        public UILabel StatuLabel, TimeLabel;
        /// <summary>军团数据</summary>
        public static List<LegionRankData> Legions;
        /// <summary>战斗类型预赛半决还是决赛</summary>
        public static WarType _WarType;
        /// <summary>各战场状态情况</summary>
        public FinalStatusData ThisFinalStatus = new FinalStatusData();

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            // base.Update();
            //更新时间
            if (!isOpen) return;
            if (updateTimer < 0.5)
            {
                updateTimer += Time.deltaTime;
            }
            else
            {
                CheckTime();
                updateTimer = 0;
            }
        }
    
        public override void OnDestroy()
        {
            _Instance = null;
            base.OnDestroy();
        }
        
        public override void SetMenuData(object param)
        {
            LTLegionWarManager.Instance.FinalStatusList.CleanUp();
            isWatchLog = false;
            isWarOver = false;
            isOpen = false;
            updateTimer = 1;
    
            if (param != null)
            {
                isWatchLog = true;
                Legions = param as List<LegionRankData>;
                EndShowFunc();
            }
    
            for (int i = 0; i < NumLabel.Count; i++)
            {
                BtnNameList[i].localPosition = (param == null) ? new Vector3(0, 50, 0): new Vector3 (0,15,0);
                NumLabel[i].transform.gameObject.CustomSetActive(param == null);
            }
    
            for (int i = 0; i < UsFinalItems.Length; i++)
            {
                UsFinalItems[i].ResetFlash();
                EnemyFinalItems[i].ResetFlash();
            }
        }
    
        public override IEnumerator OnAddToStack() {
            FusionAudio.PostBGMEvent("BGM/JunTuanZhan", true);
            FusionAudio.StartBGM();
            yield return new WaitUntil(() => LTLegionWarJoinController.callBack);
            if (isWatchLog)
            {
                FinalStatusListener(null, null);
                FinalFieldListener(null, null);
            }
            else
            {
                Messenger.AddListener(Hotfix_LT.EventName.LegionWar_UpdataFinal, UpdataListener);
                if (LTLegionWarManager.Instance.serveCurState < 2)
                {
                    controller.Close();
                    yield break;
                }
                else
                {
                    int ts = 0;
                    if (LTLegionWarTimeLine.Instance != null)
                    {
                        ts = Convert.ToInt32(((LTLegionWarFinalController._WarType == WarType.Semifinal) ? LTLegionWarTimeLine.Instance.SemiFinalStopTime :
                            LTLegionWarTimeLine.Instance.FinalStopTime) - EB.Time.Now);
                    }                   
                    if (ts <= 0)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTLegionWarFinalController_4204"));
                        controller.Close();
                        yield break;
                    }
                }
                LTLegionWarManager.Instance.EnterSemiFinalFiled(LTLegionWarManager.Instance.SemiFinalField, 0);
            }
            yield return base.OnAddToStack();
            InitLegionData();
        }
    
        public override IEnumerator OnRemoveFromStack() {
            FusionAudio.StopBGM();
            if (isWatchLog)
            {
    
            }
            else
            {
                Messenger.RemoveListener(Hotfix_LT.EventName.LegionWar_UpdataFinal, UpdataListener);
                isOpen = false;
                if (GameDataSparxManager.Instance.HasListener(LTLegionWarManager.FinalPlayerDataListId)) GameDataSparxManager.Instance.UnRegisterListener(LTLegionWarManager.FinalPlayerDataListId, FinalFieldListener);
                if (GameDataSparxManager.Instance.HasListener(LTLegionWarManager.FinalStatusListId)) GameDataSparxManager.Instance.UnRegisterListener(LTLegionWarManager.FinalStatusListId, FinalStatusListener);
                LTLegionWarManager.Instance.LevelAllianceWar();
            }
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }
        
        private void CheckTime()
        {
            if (isWarOver)
            {
                GlobalMenuManager.Instance.ComebackToMianMenu();
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTLegionWarFinalController_4204"));
                return;
            }

            var dataList = LTLegionWarManager.Instance.FinalStatusList.AllFinalStatusDataList;

            for (var i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];
                if (data.FieldNumber == LTLegionWarManager.Instance.SemiFinalField && data.Type == WarFiled)
                {
                    ThisFinalStatus = data;
                    break;
                }
            }
    
            if (ThisFinalStatus.Status == 0)
            {
                StatuLabel.text = StatuLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_LTLegionWarFinalController_4204");
                PlayerLadderRanking.gameObject.CustomSetActive(false);
                TimeLabel.gameObject.CustomSetActive(true);
                SetAllReplaceButton(false);
                //SetAllEnemyInfo(false);
            }
            else
            {
                if (ThisFinalStatus.Status == 2)
                {
                    int ts = 0;
                    if (LTLegionWarTimeLine.Instance != null)
                    {
                        ts = Convert.ToInt32(((_WarType == WarType.Semifinal) ? LTLegionWarTimeLine.Instance.SemiFinalStopTime : LTLegionWarTimeLine.Instance.FinalStopTime) - EB.Time.Now);
                    }
                    if (ts > 0)
                    {
                        string StatuStr = string.Format(EB.Localizer.GetString("ID_LEGION_WAR_FINAL_LOG"), (_WarType == WarType.Semifinal) ? EB.Localizer.GetString("ID_HALF_BATTLE") : EB.Localizer.GetString("ID_ALLIANCE_FINAL"));
                        StatuLabel.text = StatuLabel.transform.GetChild(0).GetComponent<UILabel>().text = StatuStr;
                        PlayerLadderRanking.gameObject.CustomSetActive(true);
                        TimeLabel.gameObject.CustomSetActive(true);
                        SetAllReplaceButton(LTLegionWarManager.Instance.FinalPlayerDataList.MyWarField != WarFiled);
                        //SetAllEnemyInfo(true);
                    }
                    else
                    {
                        StatuLabel.text = StatuLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_LTLegionWarFinalController_4204");
                        PlayerLadderRanking.gameObject.CustomSetActive(false);
                        TimeLabel.gameObject.CustomSetActive(true);
                        SetAllReplaceButton(false);
                        //SetAllEnemyInfo(false);
                    }
                }
                else
                {
                    EndShowFunc();
                }
            }
    
            GetCountdown();
        }
    
        private void EndShowFunc()
        {
            StatuLabel.text = StatuLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_LTLegionWarFinalController_4850");
            PlayerLadderRanking.gameObject.CustomSetActive(false);
            TimeLabel.gameObject.CustomSetActive(false);
            SetAllReplaceButton(false);
        }
    
        private void GetCountdown()
        {
            LegionWarTimeLine time = LTLegionWarManager.GetLegionWarStatus();
    
            if (time == LegionWarTimeLine.SemiFinal || time == LegionWarTimeLine.Final)
            {
                int ts = 0;
                if (LTLegionWarTimeLine.Instance != null)
                {
                    ts = Convert.ToInt32(((_WarType == WarType.Semifinal) ? LTLegionWarTimeLine.Instance.SemiFinalStopTime : LTLegionWarTimeLine.Instance.FinalStopTime) - EB.Time.Now);
                }
                string timeStr = "";
                if (ts >= 0)
                {
                    System.DateTime dt = EB.Time.FromPosixTime(ts);
                    if (dt.Hour > 0) timeStr = dt.ToString("hh:mm:ss");
                    else timeStr = dt.ToString("mm:ss");
                }
                TimeLabel.text = TimeLabel.transform.GetChild(0).GetComponent<UILabel>().text = timeStr;
                TimeLabel.gameObject.CustomSetActive(true);
            }
            else
            {
                isWarOver = true;
                TimeLabel.gameObject.CustomSetActive(false);
            }
        }
    
        private void InitLegionData()
        {
            UsCell.inWarFill(Legions[0]);
            EnemyCell.inWarFill(Legions[1]);
            PlayerLadderRanking.text = PlayerLadderRanking.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString("ID_MY_LADDER_RANK") + ":[fff348]{0}", LadderManager.Instance.Info.Rank > 0 ? LadderManager.Instance.Info.Rank.ToString() : EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"));
        }
    
        private void UpdataListener()
        {
            isOpen = true;
            if (!GameDataSparxManager.Instance.HasListener(LTLegionWarManager.FinalPlayerDataListId)) 
                GameDataSparxManager.Instance.RegisterListener(LTLegionWarManager.FinalPlayerDataListId, FinalFieldListener);
            if (!GameDataSparxManager.Instance.HasListener(LTLegionWarManager.FinalStatusListId))
                GameDataSparxManager.Instance.RegisterListener(LTLegionWarManager.FinalStatusListId, FinalStatusListener);
        }
    
        private void FinalFieldListener(string path, INodeData value)
        {
            if (isWatchLog)
            {
                StopCoroutine(InitWatchLogPlayers());
                StartCoroutine(InitWatchLogPlayers());
            }
            else
            {
                StopCoroutine(InitPlayers());
                StartCoroutine(InitPlayers());
            }
        }
    
        private void FinalStatusListener(string path, INodeData value)
        {
            if (isWatchLog)
            {
                StopCoroutine(InitWatchLogFieldType());
                StartCoroutine(InitWatchLogFieldType());
            }
            else
            {
                StopCoroutine(InitFieldType());
                StartCoroutine(InitFieldType());
            }
        }
    
        private IEnumerator InitPlayers() {
            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 0.8f);
            SetAllEnemyInfo(true);
            int field = (int)LTLegionWarFinalController.Instance.WarFiled;
            if (TypeBtnList != null)
            {
                for (int i = 0; i < TypeBtnList.Count; i++)
                {
                    TypeBtnList[i].CustomSetActive(field == i);
                }
            }
            if (LTLegionWarManager.Instance.FinalPlayerDataList.AllHomeTeam[field].Count<5)
                for (int i = LTLegionWarManager.Instance.FinalPlayerDataList.AllHomeTeam[field].Count; i < UsFinalItems.Length; i++) {            
                    LTLegionWarManager.Instance.FinalPlayerDataList.AllHomeTeam[field].Add(new FinalPlayerData());           
                }
            if (LTLegionWarManager.Instance.FinalPlayerDataList.AllAwayTeam[field].Count<5)
                for (int i = LTLegionWarManager.Instance.FinalPlayerDataList.AllAwayTeam[field].Count; i < UsFinalItems.Length; i++) {
                    LTLegionWarManager.Instance.FinalPlayerDataList.AllAwayTeam[field].Add(new FinalPlayerData());
                }
    
            for (int i = 0; i < UsFinalItems.Length; i++) {
                if (Legions[0].id == LTLegionWarManager .Instance .HomeTeamAid) {
                    UsFinalItems[i].ishomeTeam = true;
                    UsFinalItems[i].SetData(LTLegionWarManager.Instance.FinalPlayerDataList.AllHomeTeam[field][i]);
                    EnemyFinalItems[i].ishomeTeam = false;
                    EnemyFinalItems[i].SetData(LTLegionWarManager.Instance.FinalPlayerDataList.AllAwayTeam[field][i]);
                }
                else {
                    EnemyFinalItems[i].ishomeTeam = true;
                    EnemyFinalItems[i].SetData(LTLegionWarManager.Instance.FinalPlayerDataList.AllHomeTeam[field][i]);
                    UsFinalItems[i].ishomeTeam = false;
                    UsFinalItems[i].SetData(LTLegionWarManager.Instance.FinalPlayerDataList.AllAwayTeam[field][i]);
                }
            }
            SetJoinPeopleCount(LTLegionWarManager.Instance.FinalPlayerDataList);
            SetAllReplaceButton(false);
    
            float timer = 0.10f;
            for (int i = 0; i < UsFinalItems.Length; i++) {
                UsFinalItems[i].StartBootFlash();
                EnemyFinalItems[i].StartBootFlash();
                yield return new WaitForSeconds(timer);
            }
        }
    
        private IEnumerator InitWatchLogPlayers()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 0.8f);
            SetAllReplaceButton(false);
            SetAllEnemyInfo(false);
            int field = (int)LTLegionWarFinalController.Instance.WarFiled;
            LTLegionWarManager.Instance.WatchLogList.OnUpdateTeamData(LTLegionWarManager.Instance.SemiFinalField,field);
            if (TypeBtnList != null)
            {
                for (int i = 0; i < TypeBtnList.Count; i++)
                {
                    TypeBtnList[i].CustomSetActive(field == i);
                }
            }
    
            if (LTLegionWarManager.Instance.WatchLogList.HomeTeam.Count < 5)
                for (int i = LTLegionWarManager.Instance.WatchLogList.HomeTeam.Count; i < UsFinalItems.Length; i++)
                {
                    LTLegionWarManager.Instance.WatchLogList.HomeTeam.Add(new FinalPlayerData());
                }
            if (LTLegionWarManager.Instance.WatchLogList.AwayTeam.Count < 5)
                for (int i = LTLegionWarManager.Instance.WatchLogList.AwayTeam.Count; i < UsFinalItems.Length; i++)
                {
                    LTLegionWarManager.Instance.WatchLogList.AwayTeam.Add(new FinalPlayerData());
                }
    
            for (int i = 0; i < UsFinalItems.Length; i++)
            {
                UsFinalItems[i].SetData(LTLegionWarManager.Instance.WatchLogList.HomeTeam[i]);
                EnemyFinalItems[i].SetData(LTLegionWarManager.Instance.WatchLogList.AwayTeam[i]);
            }
    
            var timer = new WaitForSeconds(0.1f);
            for (int i = 0; i < UsFinalItems.Length; i++)
            {
                UsFinalItems[i].StartBootFlash();
                EnemyFinalItems[i].StartBootFlash();
                yield return timer;
            }
    
            timer = new WaitForSeconds(0.6f);
            var fieldData = LTLegionWarManager.Instance.WatchLogList.FindFinalStatusData(LTLegionWarManager.Instance.SemiFinalField, LTLegionWarFinalController.Instance.WarFiled);
            if (fieldData!=null&& fieldData.SimpleLogDataList !=null)
            {
                for (int i = 0; i < fieldData.SimpleLogDataList.Count; i++)
                {
                    for (int j = 0; j < UsFinalItems.Length; j++)
                    {
                        UsFinalItems[j].ShowWatchLogInfo(fieldData.SimpleLogDataList[i].winUid, fieldData.SimpleLogDataList[i].loseUid );
                        EnemyFinalItems[j].ShowWatchLogInfo(fieldData.SimpleLogDataList[i].winUid , fieldData.SimpleLogDataList[i].loseUid);
                    }
                    yield return timer;
                }
            }
        }
    
        private IEnumerator InitFieldType()
        {
            ThisFinalStatus = new FinalStatusData();
            var dataList = LTLegionWarManager.Instance.FinalStatusList.AllFinalStatusDataList;
            for (var i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];
                if (data.FieldNumber == LTLegionWarManager.Instance.SemiFinalField && data.Type == WarFiled)
                {
                    ThisFinalStatus = data;
                    break;
                }
            }
            //0,1,2 开始，结束，准备中
            if (ThisFinalStatus.Status==2)//准备中
            {
    
                UsCell.SetCurStatu(false);
                EnemyCell.SetCurStatu(false);
            }
            else if (ThisFinalStatus.Status == 0)//开始
            {
                PlayerLadderRanking.gameObject.CustomSetActive(false);
                TimeLabel.gameObject.CustomSetActive(false);
                
                //SetAllEnemyInfo(false);
                UsCell.SetCurStatu(false);
                EnemyCell.SetCurStatu(false);
            }
            else//结束
            {
                PlayerLadderRanking.gameObject.CustomSetActive(false);
                TimeLabel.gameObject.CustomSetActive(false);
                
                //SetAllEnemyInfo(false);
                if (Legions[0] != null)UsCell.SetCurStatu(true, ThisFinalStatus.WinAid == Legions[0].id);
                if(Legions[1]!=null)EnemyCell.SetCurStatu(true, ThisFinalStatus.WinAid == Legions[1].id);
            }
            yield break;
        }
    
        private IEnumerator InitWatchLogFieldType()
        {
            ThisFinalStatus = new FinalStatusData();
            var dataList = LTLegionWarManager.Instance.WatchLogList.AllFinalStatusDataList;

            for (var i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];
                if (data.FieldNumber == LTLegionWarManager.Instance.SemiFinalField && data.Type == WarFiled)
                {
                    ThisFinalStatus = data;
                    break;
                }
            }
            if (Legions[0] != null) UsCell.SetCurStatu(true, ThisFinalStatus.WinAid == Legions[0].id);
            if (Legions[1] != null) EnemyCell.SetCurStatu(true, ThisFinalStatus.WinAid == Legions[1].id);
            yield break;
    
        }
    
        /// <summary>
        /// 显示各个战场参加人数
        /// </summary>
        /// <param name="pushList"></param>
        public void SetJoinPeopleCount(FinalPlayerList pushList)
        {
            int UsCount = 0;
            int EnemyCount = 0;
            for (int i = 0; i < 3; i++)
            {
                int num = 0;
                for (var j = 0; j < pushList.AllHomeTeam[i].Count; j++)
                {
                    var item = pushList.AllHomeTeam[i][j];
                    if (item != null && !string.IsNullOrEmpty(item.Name))
                    {
                        if (Legions[0].id == LTLegionWarManager.Instance.HomeTeamAid) { num++; UsCount++; }
                        else EnemyCount++;
                    }
                }
                for (var j = 0; j < pushList.AllAwayTeam[i].Count; j++)
                {
                    var item = pushList.AllAwayTeam[i][j];
                    if (item != null && !string.IsNullOrEmpty(item.Name))
                    {
                        if (!(Legions[0].id == LTLegionWarManager.Instance.HomeTeamAid)) { num++; UsCount++; }
                        else EnemyCount++;
                    }
                }
                NumLabel[i].text = NumLabel[i].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}/5", num);
            }
            UsCell.WaitState.GetComponent<UILabel>().text = UsCell.WaitState.transform.GetChild(0).GetComponent<UILabel>().text = UsCount.ToString();
            EnemyCell.WaitState.GetComponent<UILabel>().text = EnemyCell.WaitState.transform.GetChild(0).GetComponent<UILabel>().text = EnemyCount.ToString();
        }
        
        public void SetAllEnemyInfo(bool show)
        {
            for (int i = 0; i < EnemyFinalItems.Length; i++)
            {
                EnemyFinalItems[i].SetInfo(show);
            }
        }
    
        public void SetAllReplaceButton(bool show)
        {
            for (int i = 0; i < UsFinalItems.Length; i++)
            {
                UsFinalItems[i].SetReplaceButton(show);
            }
        }
      
        public void OnChatBtnClick()
        {
            GlobalMenuManager.Instance.Open("ChatHudView", null);
        }
    
        public void OnFriendBtnClick()
        {
            GlobalMenuManager.Instance.Open("FriendHud", null);
        }
    
        public void OnRuleBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            string rule = EB.Localizer.GetString("ID_RULE_ALLIANCEWAR");
            GlobalMenuManager.Instance.Open("LTRuleUIView", rule);
        }
    
        public void OnWatchBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
            LTLegionWarManager.Instance.FieldType = (int)LTLegionWarFinalController.Instance.WarFiled;
            LTLegionWarManager.Instance.WatchWar(ThisFinalStatus.CombatId);
        }
    
        public void OnWaterTabClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            WarFiled = LegionWarField.Water;
            FinalStatusListener(null, null);
            FinalFieldListener(null, null);
        }
    
        public void OnWindTabClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            WarFiled = LegionWarField.Wind;
            FinalStatusListener(null, null);
            FinalFieldListener(null, null);
        }
    
        public void OnFireTabClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            WarFiled = LegionWarField.Fire;
            FinalStatusListener(null, null);
            FinalFieldListener(null, null);
        }
    }
}
