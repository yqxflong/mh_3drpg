//LTActivityBodyItem_Racing
//赛跑活动
//Johny

using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTActivityBodyItem_Racing:LTActivityBodyItem
    {
        private enum GroupStatus
        {
            None,
            Preview,
            Beting,
            Racing,
            Finished
        }

        private class GroupQuickData
        {
            public int Group;
            public System.TimeSpan BetCountDown; //下注倒计时
            public System.TimeSpan RunCountDown; //赛跑倒计时
            public GroupStatus Status; //当前活动状态
            public List<int> PlayerIds = new List<int>();

            private System.TimeSpan _ts_1sec = new System.TimeSpan(0,0,1);

            #region display
            //场次
            public UILabel NumLabel;
            //倒计时label
            public UILabel BetCountDownLabel;
            //未开启标识
            public UISprite UnopenSprite;
            //0：bet  1： watch  2：result
            public List<ConsecutiveClickCoolTrigger> Buttons = new List<ConsecutiveClickCoolTrigger>();
            public List<UISprite> Heads = new List<UISprite>();
            //0: during_bet  1: during_bet_outline 2:during_race  3: during_race_outline
            public List<UILabel> Labels = new List<UILabel>();
            #endregion

            public GroupQuickData()
            {
                Status = GroupStatus.None;
            }

            public void BetTickOnce()
            {
                BetCountDown -= _ts_1sec;
            }

            public void RunTickOnce()
            {
                RunCountDown -= _ts_1sec;
            }
        }


        #region private mem
        //倒计时句柄
        private int countDownSeq = -1;
        private List<GroupQuickData> groupDatas;
        #endregion

        public override void Awake()
        {
            base.Awake();
            //刷新选手数据
            LTActivityRacingManager.Instance.RefreshPlayerData();
            //初始化控件
            InitControls();
        }

        public override void OnEnable()
        {
            countDownSeq = ILRTimerManager.instance.AddTimer(1000, 0, OnTimer_CountDown);
        }

        public override void OnDisable()
        {
            ILRTimerManager.instance.RemoveTimerSafely(ref countDownSeq);
        }

        public override void SetData(object data)
        {
            base.SetData(data);
            
            var mgr = LTActivityRacingManager.Instance;

            for(int i = 1; i <= 2; i++)
            {
                //准备快捷数据
                var ts_betleft = mgr.GetGuessLeftTime(i);
                var ts_runleft = mgr.GetRunLeftTime(i);
                var gd = groupDatas[i - 1];
                gd.Group = i;
                gd.BetCountDown = ts_betleft;
                gd.RunCountDown = ts_runleft;
                mgr.GetPlayerIds(i, gd.PlayerIds);

                //刷新场次信息
                RefreshGroupInfoDisplay(i);
            }

            //手动刷一次计时器
            OnTimer_CountDown(countDownSeq);
        }

        private void InitControls()
        {
            var t = mDMono.transform;
            var node_first = t.Find("CONTENT/node_first");
            var node_second = t.Find("CONTENT/node_second");

            groupDatas = new List<GroupQuickData>{
                new GroupQuickData(),
                new GroupQuickData()
            };
            var gd1 = groupDatas[0];
            var gd2 = groupDatas[1];

            #region group num
            gd1.NumLabel = node_first.GetComponent<UILabel>("lb_num");
            gd1.NumLabel.text = EB.Localizer.Format("ID_RACING_GROUP_NUM", 1);
            gd2.NumLabel = node_second.GetComponent<UILabel>("lb_num");
            gd2.NumLabel.text = EB.Localizer.Format("ID_RACING_GROUP_NUM", 2);
            #endregion

            #region group during label
            {
                var lb_betduring = node_first.GetComponent<UILabel>("lb_betduring");
                gd1.Labels.Add(lb_betduring);
                var lb_raceduring = node_first.GetComponent<UILabel>("lb_raceduring");
                gd1.Labels.Add(lb_raceduring);
            }
            {
                var lb_betduring = node_second.GetComponent<UILabel>("lb_betduring");
                gd2.Labels.Add(lb_betduring);
                var lb_raceduring = node_second.GetComponent<UILabel>("lb_raceduring");
                gd2.Labels.Add(lb_raceduring);
            }
            #endregion

            #region group heads
            for(int i = 1; i <= 3; i++)
            {
                gd1.Heads.Add(node_first.GetComponent<UISprite>($"sp_player{i}"));
            }
            for(int i = 1; i <= 3; i++)
            {
                gd2.Heads.Add(node_second.GetComponent<UISprite>($"sp_player{i}"));
            }
            #endregion

            #region group unopen sp
            gd1.UnopenSprite = node_first.GetComponent<UISprite>("node_button/sp_unopen");
            gd2.UnopenSprite = node_second.GetComponent<UISprite>("node_button/sp_unopen");
            #endregion

            #region group coundown label
            gd1.BetCountDownLabel = node_first.GetComponent<UILabel>($"node_button/lb_countdown");
            gd2.BetCountDownLabel = node_second.GetComponent<UILabel>($"node_button/lb_countdown");
            #endregion

            #region group buttons
            for(int i = 1; i <= 3; i++)
            {
                var btn = node_first.GetComponent<ConsecutiveClickCoolTrigger>($"node_button/btn_1_{i}");
                btn.clickEvent.Add(new EventDelegate(()=> OnButtonClicked(btn)));
                gd1.Buttons.Add(btn);
            }
            for(int i = 1; i <= 3; i++)
            {
                var btn = node_second.GetComponent<ConsecutiveClickCoolTrigger>($"node_button/btn_2_{i}");
                btn.clickEvent.Add(new EventDelegate(()=> OnButtonClicked(btn)));
                gd2.Buttons.Add(btn);
            }
            #endregion
        }

        private void RefreshGroupInfoDisplay(int group)
        {
            var mgr = LTActivityRacingManager.Instance;
            var gd = groupDatas[group - 1];

            //时间
            string betBegin = mgr.GetGuessStartTime(group);
            string raceBegin = mgr.GetRunStartTime(group);
            string raceEnd = mgr.GetRunEndTime(group);
            betBegin = betBegin.Remove(betBegin.LastIndexOf(':'));
            raceBegin = raceBegin.Remove(raceBegin.LastIndexOf(':'));
            raceEnd = raceEnd.Remove(raceEnd.LastIndexOf(':'));
            gd.Labels[0].text = $"{betBegin}-{raceBegin}";
            gd.Labels[1].text = $"{raceBegin}-{raceEnd}";

            //选手
            for(int index = 0; index < 3; index++)
            {
                int pid = gd.PlayerIds[index];
                var info = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(pid);
                var head = gd.Heads[index];
                head.spriteName = info.icon;
            }
        }

        private void RefreshGroupButtonStatus(int group, GroupStatus status)
        {
            var gd = groupDatas[group - 1];
            var sp_unopen = gd.UnopenSprite;
            var btn_bet = gd.Buttons[0];
            var btn_racing = gd.Buttons[1];
            var btn_result = gd.Buttons[2];

            switch(status)
            {
                case GroupStatus.Preview:
                    {
                        sp_unopen.gameObject.CustomSetActive(true);
                        btn_bet.gameObject.CustomSetActive(false);
                        btn_racing.gameObject.CustomSetActive(false);
                        btn_result.gameObject.CustomSetActive(false);
                    }
                    break;
                case GroupStatus.Beting:
                    {
                        sp_unopen.gameObject.CustomSetActive(false);
                        btn_bet.gameObject.CustomSetActive(true);
                        btn_racing.gameObject.CustomSetActive(false);
                        btn_result.gameObject.CustomSetActive(false);
                    }
                    break;
                case GroupStatus.Racing:
                    {
                        sp_unopen.gameObject.CustomSetActive(false);
                        btn_bet.gameObject.CustomSetActive(false);
                        btn_racing.gameObject.CustomSetActive(true);
                        btn_result.gameObject.CustomSetActive(false);
                    }
                    break;
                case GroupStatus.Finished:   
                    {
                        sp_unopen.gameObject.CustomSetActive(false);
                        btn_bet.gameObject.CustomSetActive(false);
                        btn_racing.gameObject.CustomSetActive(false);
                        btn_result.gameObject.CustomSetActive(true);
                    }
                    break;
                default:
                    break;
            }
        }

        private void RefreshBetCountDownDisplay(int group)
        {
            var gd = groupDatas[group - 1];
            var ts_bet_left = gd.BetCountDown;
            var lb_countdown = gd.BetCountDownLabel;
            if(ts_bet_left.TotalSeconds <= 0)
            {
                lb_countdown.gameObject.CustomSetActive(false);
            }
            else
            {
                string hr = ts_bet_left.Hours < 10 ? $"0{ts_bet_left.Hours}" : ts_bet_left.Hours.ToString();
                string min = ts_bet_left.Minutes < 10 ? $"0{ts_bet_left.Minutes}" : ts_bet_left.Minutes.ToString();
                string sec = ts_bet_left.Seconds < 10 ? $"0{ts_bet_left.Seconds}" : ts_bet_left.Seconds.ToString();
                string strTime = $"{hr}:{min}:{sec}";
                lb_countdown.text = EB.Localizer.Format("ID_RACING_START_COUNTDOWN", strTime);
                lb_countdown.gameObject.CustomSetActive(true);
            }
        }

        //进入比赛
        private void EnterMatch(int group)
        {
            var mgr = LTActivityRacingManager.Instance;
            var gd = mgr.GetGroupData(group);
            gd.CurIsReplay = false;
            mgr.RequestAllResult(group, OnRacingDataResponse);
        }

        #region About Button 回调事件
        private void OnTimer_CountDown(int seq)
        {
            for(int i = 1; i <= 2; i++)
            {
                var gd = groupDatas[i - 1];

                #region 倒计时刷新
                gd.BetTickOnce();
                gd.RunTickOnce();
                #endregion

                if(!LTActivityRacingManager.Instance.IsActivityOpen(gd.Group))
                {
                    if(gd.Status != GroupStatus.Preview)
                    {
                        gd.Status = GroupStatus.Preview;
                        RefreshGroupButtonStatus(i, GroupStatus.Preview);
                    }
                    continue;
                }

                if(gd.BetCountDown.TotalSeconds > 0)//竞猜中
                {
                    RefreshBetCountDownDisplay(i);
                    if(gd.Status != GroupStatus.Beting)
                    {
                        gd.Status = GroupStatus.Beting;
                        RefreshGroupButtonStatus(i, GroupStatus.Beting);
                    }
                }
                else if(gd.RunCountDown.TotalSeconds > 0)//比赛中
                {
                    
                    if(gd.Status != GroupStatus.Racing)
                    {
                        gd.Status = GroupStatus.Racing;
                        RefreshGroupButtonStatus(i, GroupStatus.Racing);
                    }
                }
                else//比赛结束
                {
                    if(gd.Status != GroupStatus.Finished)
                    {
                        gd.Status = GroupStatus.Finished;
                        RefreshGroupButtonStatus(i, GroupStatus.Finished);
                    }
                }
            }
        }

        private void OnButtonClicked(ConsecutiveClickCoolTrigger btn)
        {
            string btnName = btn.name;
            string[] btnNameSep = btnName.Split('_');
            int group = int.Parse(btnNameSep[1]);
            int btnType = int.Parse(btnNameSep[2]);
            switch(btnType)
            {
                case 1:
                    LTActivityRacingManager.Instance.RequestFinalResult(group, "bet", OnFinalResultResponse);
                    break;
                case 2:
                    EnterMatch(group);
                    break;
                case 3:
                    LTActivityRacingManager.Instance.RequestFinalResult(group, "result", OnFinalResultResponse);
                    break;
                default:
                    break;
            }
        }

        private void OnRacingDataResponse(int group, bool succ)
        {
            if(succ)
            {
                var mgr = LTActivityRacingManager.Instance;
                if(mgr.RacingPlayersJump2Since(group)) //已结束，请求结算
                {
                    LTActivityRacingManager.Instance.RequestFinalResult(group, "result", OnFinalResultResponse);
                }
                else
                {
                    //数据准备完备，打开赛跑界面
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("group", group);
                    GlobalMenuManager.Instance.Open("LTActivityRacingHud", ht);
                }
            }
            else
            {
                EB.Debug.LogError("[Racing]没有比赛数据!!");
            }
        }

        private void OnFinalResultResponse(int group, bool succ, string view)
        {
            if(view.Equals("bet"))
            {
                if(succ)
                {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("view", "bet");
                    ht.Add("group", group);
                    GlobalMenuManager.Instance.Open("LTActivityRacingBetHud", ht);
                }
                else
                {
                    EB.Debug.LogError("[Racing]没有合适的数据!!");
                }
            }
            else 
            {
                if(succ)
                {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("view", "result");
                    ht.Add("group", group);
                    GlobalMenuManager.Instance.Open("LTActivityRacingBetHud", ht);
                }
                else
                {
                    EB.Debug.LogError("[Racing]没有结算数据!!");
                }
            }
        }
        #endregion
    }
}
