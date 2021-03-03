//LTActivityRacingManager
//赛跑活动管理器
//Johny

using System.Collections.Generic;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class RacingPlayerData
    {
        public int Group;
        public int Num;
        public int Pid;
        public string Name;
        public string ModelName;
        public int Rank; //from server
        public int Support; //from server

        public RacingPlayerData(int group, int num, int pid, string name, string modelname)
        {
            Group = group;
            Num = num;
            Pid = pid;
            Name = name;
            ModelName = modelname;
        }
    }

    public class RacingGroupData
    {
        public int Group;

        #region from 表
        public string GuessStartTime, RunStartTime, RunEndTime;
        public string BetType;  //下注货币类型
        public int OnceBetAmount;  //一次下注额
        public float PrizePercent; //返奖倍数
        public int BetMaxTime; //最大追加次数
        #endregion

        #region from server
        public int CurBetPlayerNum = -1;  //当前下注选手号码
        public int CurBetAmount; //当前下注数量
        public int CurAddBetCount;  //当前追注次数
        public long CurRacingStartTime; //此组比赛开始时间
        public bool CurIsReplay; //当前是否是回放
        #endregion

        //key: pid
        public Dictionary<int, RacingPlayerData> Players;
        //key: num
        public List<int> OrderedPids;
        //key: pid
        public Dictionary<int, ActivityRacingPlayer> RacingPlayers = new Dictionary<int, ActivityRacingPlayer>();

        public RacingGroupData(int group, int p1, int p2, int p3, string tp, int amt)
        {
            Group = group;
            BetType = tp;
            OnceBetAmount = amt;
            OrderedPids = new List<int>{
                p1, p2, p3
            };
        } 

        public void Refresh()
        {
            if(Players != null)
            {
                return;
            }
            Players = new Dictionary<int, RacingPlayerData>();
            for (int i = 0; i < OrderedPids.Count; i++)
            {
                int pid = OrderedPids[i];
                var info = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(pid);
                Players[pid] = new RacingPlayerData(Group, i + 1, pid, info.name, info.model_name);
            }
        }

        public RacingPlayerData GetPlayerData(int pid)
        {
            return Players[pid];
        }

        public RacingPlayerData GetPlayerDataByNum(int num)
        {
            int pid = OrderedPids[num - 1];
            return GetPlayerData(pid);
        }

        public RacingPlayerData GetCurBetPlayerData()
        {
            return GetPlayerDataByNum(CurBetPlayerNum);
        }

        public int RankByNum(int num)
        {
            if(num > 0)
            {
                int pid = OrderedPids[num - 1];
                var pd = Players[pid];
                return pd.Rank;
            }
            else
            {
                return -1;
            }
        }
    }

    public class LTActivityRacingManager
    {

        #region  instance
        private static LTActivityRacingManager instance = null;
        public static LTActivityRacingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LTActivityRacingManager();
                }
                return instance;
            }
        }
        #endregion

        private List<RacingGroupData> _groupDatas = new List<RacingGroupData>();

        public void InitConfigs(GM.DataCache.ConditionEvent data)
        {
            _groupDatas.Clear();
            for(int i = 0; i < data.BuddyRunLength; i++)
            {
                //存入缓存
                var cd = data.GetBuddyRun(i);

                //准备快捷数据
                string[] ss = cd.BetAmount.Split(',');
                if(ss.Length != 3)
                {
                    throw new System.Exception("表的BetAmount参数与约定的不同，请校验!");
                }
                if(int.TryParse(ss[1], out int amt))
                {
                    var qd = new RacingGroupData(i + 1, cd.Player1, cd.Player2, cd.Player3, ss[0], amt);
                    _groupDatas.Add(qd);
                    qd.PrizePercent = cd.PrizePercent;
                    qd.BetMaxTime = cd.BetMaxTime;

                    //读取during
                    qd.GuessStartTime = GetTimeFromDuring($"buddyrunsupport{i + 1}");
                    qd.RunStartTime = GetTimeFromDuring($"buddyruncalcresult{i + 1}");
                    qd.RunEndTime = GetTimeFromDuring($"buddyrunsendreward{i + 1}");
                }
                else
                {
                    throw new System.Exception("表的BetAmount参数与约定的不同，请校验!");
                }
            }
        }

        private string GetTimeFromDuring(string key)
        {
            Hotfix_LT.Data.CronJobs cj = Hotfix_LT.Data.EventTemplateManager.Instance.GetCronJobsByName(key);
            string[] ss = cj.interval.Split(' ');    
            string hr = ss[2].Equals("0") ? "00" : ss[2];
            string min = ss[1].Equals("0") ? "00" : ss[1];
            string sec = ss[0].Equals("0") ? "00" : ss[0];
            return $"{hr}:{min}:{sec}";
        }

        public void RefreshPlayerData()
        {
            for(int i = 0; i < _groupDatas.Count; i++)
            {
                var rpd = _groupDatas[i];
                rpd.Refresh();
            }
        }

        #region during
        //活动是否开启
        public bool IsActivityOpen(int group)
        {
            string strGuessBegin = GetGuessStartTime(group);
            string[] ss_begin = strGuessBegin.Split(':');
            System.TimeSpan ts_begin = new System.TimeSpan(int.Parse(ss_begin[0]), int.Parse(ss_begin[1]), 0);
            return EB.Time.After(ts_begin);
        }

        //竞猜开始时间
        public string GetGuessStartTime(int group)
        {
            var gd = _groupDatas[group - 1];
            return gd.GuessStartTime;
        }

        //竞猜结束时间
        public string GetGuessEndTime(int group)
        {
            var gd = _groupDatas[group - 1];
            return gd.RunStartTime;
        }

        //现在距离竞猜结束时间(无论当前是否在竞猜阶段)
        public System.TimeSpan GetGuessLeftTime(int group)
        {
            string strGuessBegin = GetGuessStartTime(group);
            string strGuessEnd = GetGuessEndTime(group);
            string[] ss_begin = strGuessBegin.Split(':');
            string[] ss_end = strGuessEnd.Split(':');
            System.TimeSpan ts_begin = new System.TimeSpan(int.Parse(ss_begin[0]), int.Parse(ss_begin[1]), 0);
            System.TimeSpan ts_end = new System.TimeSpan(int.Parse(ss_end[0]), int.Parse(ss_end[1]), 0);
            System.TimeSpan now = EB.Time.LocalTimeOfDay;
            return ts_end - now;
        }

        //是否在竞猜期间
        public bool WithinGuessDuring(int group)
        {
            string strGuessBegin = GetGuessStartTime(group);
            string strGuessEnd = GetGuessEndTime(group);
            string[] ss_begin = strGuessBegin.Split(':');
            string[] ss_end = strGuessEnd.Split(':');
            System.TimeSpan ts_begin = new System.TimeSpan(int.Parse(ss_begin[0]), int.Parse(ss_begin[1]), 0);
            System.TimeSpan ts_end = new System.TimeSpan(int.Parse(ss_end[0]), int.Parse(ss_end[1]), 0);
            return EB.Time.LocalInRange(ts_begin, ts_end);
        }

        //是否有任一祖在竞赛期间
        public bool AnyGroupWithinBetDuring()
        {
            for(int gg = 1; gg <= 2; gg++)
            {
                if(WithinGuessDuring(gg))
                {
                    return true;
                }
            }

            return false;
        }

        //赛跑开始时间
        public string GetRunStartTime(int group)
        {
            var gd = _groupDatas[group -1];
            return gd.RunStartTime;
        }

        //赛跑结束时间
        public string GetRunEndTime(int group)
        {
            var gd = _groupDatas[group -1];
            return gd.RunEndTime;
        }

        //现在距离赛跑结束的时间(无论当前是否在赛跑阶段)
        public System.TimeSpan GetRunLeftTime(int group)
        {
            string strBegin = GetRunStartTime(group);
            string strEnd = GetRunEndTime(group);
            string[] ss_begin = strBegin.Split(':');
            string[] ss_end = strEnd.Split(':');
            System.TimeSpan ts_begin = new System.TimeSpan(int.Parse(ss_begin[0]), int.Parse(ss_begin[1]), 0);
            System.TimeSpan ts_end = new System.TimeSpan(int.Parse(ss_end[0]), int.Parse(ss_end[1]), 0);
            System.TimeSpan now = EB.Time.LocalTimeOfDay;
            return ts_end - now;
        }

        //赛跑开始多少时间
        public int GetRacingSinceStart(int group)
        {
            var gd = GetGroupData(group);
            int sinceStart = EB.Time.Since(gd.CurRacingStartTime);
            return sinceStart;
        }

        //是否已有到达终点的选手
        public bool RacingPlayersJump2Since(int group)
        {
            bool hasAnyOneReached = false;
            int since = GetRacingSinceStart(group);
            var gd = _groupDatas[group - 1];
            foreach(var it in gd.RacingPlayers)
            {
                hasAnyOneReached = it.Value.HasReachedEnd(since);
            }

            return hasAnyOneReached;
        }
        #endregion

        #region bet
        //获取货币类型
        public string GetBetCurrencyType(int group)
        {
            var gd = _groupDatas[group - 1];
            return gd.BetType;
        }

        //获取货币spriteName
        public string GetBetCurrencyTypeSpriteName(int group)
        {
            string tp = GetBetCurrencyType(group);
            if(tp.Equals("hc"))
            {
                return "Ty_Icon_Jewel";
            }
            else if(tp.Equals("gold"))
            {
                return "Ty_Icon_Gold";
            }

            EB.Debug.LogError("没有对应的货币类型: {0}", tp);
            return "";
        }

        //获取一次下注数量
        public int GetOnceBetAmount(int group)
        {
            var gd = _groupDatas[group - 1];
            return gd.OnceBetAmount;
        }

        //最大追加次数
        public int GetAddBetMaxTime(int group)
        {
            var gd = _groupDatas[group - 1];
            return gd.BetMaxTime;
        }

        //获取当前下注次数显示
        public string GetCurrentAddBetTimeDisplay(int group)
        {
            var gd = _groupDatas[group - 1];
            int maxAddBetTime = GetAddBetMaxTime(group);
            return $"{maxAddBetTime - gd.CurAddBetCount}/{maxAddBetTime}";
        }
        #endregion

        #region Has Something
        //当前是否有下注选手
        public bool HasBetPlayer(int group)
        {
            var gd = _groupDatas[group - 1];
            return gd.CurBetPlayerNum > 0;
        }

        //是否有足够货币
        public bool HasEnoughCurrency(int group)
        {
            var gd = GetGroupData(group);
            if(gd.BetType.Equals("hc"))
            {
                return BalanceResourceUtil.GetUserDiamond() >= GetOnceBetAmount(group);
            }
            else if(gd.BetType.Equals("gold"))
            {
                return BalanceResourceUtil.GetUserDiamond() >= GetOnceBetAmount(group);
            }

            EB.Debug.LogError("无效货币: {0}", gd.BetType);
            return false;   
        }

        //是否有足够追加次数
        public bool HasEnoughAddBetTime(int group)
        {
            var gd = _groupDatas[group - 1];
            return GetAddBetMaxTime(group) - gd.CurAddBetCount > 0;
        }
        #endregion

        #region payout
        //返奖倍数
        public float GetPrizePercent(int group)
        {
            var gd = _groupDatas[group - 1];
            return gd.PrizePercent;
        }

        //当前返奖数量
        public int GetCurrentPayOut(int group)
        {
            var gd = _groupDatas[group - 1];
            return (int)(GetPrizePercent(group) * gd.CurBetAmount);
        }
        #endregion

        #region getter
        //获取当前选手id
        public void GetPlayerIds(int group, List<int> ll)
        {
            var qd = _groupDatas[group - 1];
            var orderedPids = qd.OrderedPids;
            for(int i = 0; i < orderedPids.Count; i++)
            {
                ll.Add(orderedPids[i]);
            }
        }

        //返回的是引用，不可擅自修改
        public RacingGroupData GetGroupData(int group)
        {
            return _groupDatas[group - 1];
        }
        #endregion

        #region Debug下使用
        //请求进入下注
        public void DebugRequestEnterBet(int group)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/buddyrun/startSupport");
            request.AddData("groupId", group);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                var strJson = EB.JSON.Stringify(data);
                EB.Debug.LogError(strJson);
            });
        }

        //请求计算结果
        public void DebugRequestCalcResult(int group)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/buddyrun/startCalcResult");
            request.AddData("groupId", group);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                var strJson = EB.JSON.Stringify(data);
                EB.Debug.LogError(strJson);
            });
        }

        //请求发奖
        public void DebugRequestSendReward(int group)
        {
            #region Send
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/buddyrun/startSendReward");
            request.AddData("groupId", group);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                var strJson = EB.JSON.Stringify(data);
                EB.Debug.LogError(strJson);
            });
            #endregion
        }
        
        public void DebugRequestCleanStatus(int group)
        {
           EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/buddyrun/clean"); 
            request.AddData("groupId", group);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                var strJson = EB.JSON.Stringify(data);
                EB.Debug.LogError(strJson);
            });
        }
        #endregion

        #region request
        //下注
        public void RequestBet(int group, int num, System.Action<int, bool> act)
        {
            #region local
            var gd = _groupDatas[group - 1];
            gd.CurBetPlayerNum = num;
            gd.CurBetAmount += GetOnceBetAmount(group);
            #endregion

            #region Send
            int pid = gd.OrderedPids[num - 1];
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/buddyrun/supportBuddy");
            request.AddData("groupId", group);
            request.AddData("playerId", pid);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                var strJson = EB.JSON.Stringify(data);
                EB.Debug.LogError(strJson);
                if(data.ContainsKey("buddyRun"))
                {
                    HandleBetResponse(group, data, act);
                }
                else
                {
                    act?.Invoke(group, false);
                }
            });
            #endregion
        }

        //追加下注
        public void RequestAddBet(int group, System.Action<int, bool> act)
        {
            #region local
            var gd = _groupDatas[group - 1];
            gd.CurAddBetCount++;
            gd.CurBetAmount += GetOnceBetAmount(group);
            #endregion

            #region Send
            int pid = gd.OrderedPids[gd.CurBetPlayerNum - 1];
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/buddyrun/supportBuddy");
            request.AddData("groupId", group);
            request.AddData("playerId", pid);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                var strJson = EB.JSON.Stringify(data);
                EB.Debug.LogError(strJson);
                if(data.ContainsKey("buddyRun"))
                {
                    HandleBetResponse(group, data, act);
                }
                else
                {
                    act?.Invoke(group, false);
                }
            });
            #endregion
        }

        //请求最终结果(会返回下注信息)
        public void RequestFinalResult(int group, string view, System.Action<int, bool, string> act)
        {
            #region Send
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/buddyrun/getFinalResult");
            request.AddData("groupId", group);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                var strJson = EB.JSON.Stringify(data);
                EB.Debug.LogError(strJson);
                if(data.ContainsKey("buddyRun"))
                {
                    HandleFinalResultResponse(data, view, act);
                }
                else
                {
                    act?.Invoke(group, false, view);
                }
            });
            #endregion
        }

        //请求所有结果
        public void RequestAllResult(int group, System.Action<int, bool> act)
        {
            #region Send
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/buddyrun/getAllResult");
            request.AddData("groupId", group);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                var strJson = EB.JSON.Stringify(data);
                EB.Debug.LogError(strJson);
                if(data.ContainsKey("buddyRun"))
                {
                    HandleRacingResponse(data, act);
                }
                else
                {
                    act.Invoke(group, false);
                }
            });
            #endregion
        }

        //处理人气值读取
        private void HandleReadSupport(RacingGroupData gd, Hashtable data)
        {
            var supportResult = EB.Dot.Object("supportResult", data, null);
            if(supportResult != null)
            {
                foreach(DictionaryEntry it in supportResult)
                {
                    int pid = int.Parse(it.Key as string);
                    var pd = gd.GetPlayerData(pid);
                    pd.Support = EB.Dot.Integer("times", it.Value, 0);
                }
            }
        }

        //处理下注数据
        private void HandleBetResponse(int group, Hashtable data, System.Action<int, bool> act)
        {
            var gd = _groupDatas[group - 1];
            var ht_buddyRun = EB.Dot.Object("buddyRun", data, null);
            HandleReadSupport(gd, ht_buddyRun);
            act?.Invoke(group, true);
        }

        //处理下注及结果信息
        private void HandleFinalResultResponse(Hashtable data, string view, System.Action<int, bool, string> act)
        {
            var ht_buddyRun = EB.Dot.Object("buddyRun", data, null);
            var al_finalresult = EB.Dot.Array("finalResult", ht_buddyRun, null);
            int group = EB.Dot.Integer("group", ht_buddyRun, 0);
            var gd = _groupDatas[group - 1];
            var info = EB.Dot.Object("info", ht_buddyRun, null);
            
            if(info != null)
            {
                var support = EB.Dot.Object("support", info, null);
                foreach(DictionaryEntry it in support)
                {
                    if(it.Key.ToString().Equals(group.ToString()))
                    {
                        int pid = EB.Dot.Integer("player_id", it.Value, 0);
                        var pd = gd.GetPlayerData(pid);
                        gd.CurBetPlayerNum = pd.Num;
                        gd.CurAddBetCount = EB.Dot.Integer("times", it.Value, 0) - 1;
                        gd.CurBetAmount = EB.Dot.Integer("hc", it.Value, 0);
                    }
                }
            }

            HandleReadSupport(gd, ht_buddyRun);

            if (al_finalresult != null)
            {
                for(int i = 0; i < al_finalresult.Count; i++)
                {
                    int nPid = int.Parse(al_finalresult[i].ToString());
                    var pd = gd.Players[nPid];
                    pd.Rank = i + 1;
                }
                act?.Invoke(group, true, view);
            }
            else if(view.Equals("result"))
            {
                act?.Invoke(group, false, view);
            }
            else
            {
                act?.Invoke(group, true, view);
            }
        }

        //处理赛跑中数据
        private void HandleRacingResponse(Hashtable data, System.Action<int, bool> act)
        {
            //parse data
            var ht_buddyRun = EB.Dot.Object("buddyRun", data, null);
            int group = EB.Dot.Integer("group", ht_buddyRun, 0);
            var ht_runResult = EB.Dot.Object("runResult", ht_buddyRun, null);
            if(ht_runResult == null)
            {
                act.Invoke(group, false);
            }
            else
            {
                var gd = _groupDatas[group - 1];
                
                //读取info里的下注数据
                var info = EB.Dot.Object("info", ht_buddyRun, null);
                if(info != null)
                {
                    var support = EB.Dot.Object("support", info, null);
                    foreach(DictionaryEntry it in support)
                    {
                        int pid = EB.Dot.Integer("player_id", it.Value, 0);
                        var pd = gd.GetPlayerData(pid);
                        gd.CurBetPlayerNum = pd.Num;
                        gd.CurAddBetCount = EB.Dot.Integer("times", it.Value, 0) - 1;
                        gd.CurBetAmount = EB.Dot.Integer("hc", it.Value, 0);
                        break;
                    }
                }

                //读取赛跑数据
                var al_finalresult = EB.Dot.Array("finalResult", ht_buddyRun, null);
                int winnerPid = int.Parse(al_finalresult[0].ToString());
                gd.RacingPlayers.Clear();
                var ht_result = EB.Dot.Object("result", ht_runResult, null);
                gd.CurRacingStartTime = EB.Dot.Long("startTime", ht_runResult, 0);
                for(int i = 0; i < al_finalresult.Count; i++)
                {
                    int pid = int.Parse(al_finalresult[i].ToString());
                    var pd = gd.GetPlayerData(pid);
                    pd.Rank = i + 1;
                }
                foreach (DictionaryEntry it in ht_result)
                {
                    int pid = int.Parse(it.Key as string);
                    var pd =  gd.GetPlayerData(pid);
                    var rpd = new ActivityRacingPlayer(pd);
                    rpd.PrepareBuffData(it.Value as ArrayList, pid == winnerPid);
                    gd.RacingPlayers[pid] = rpd;
                }
                act.Invoke(group, true);
            }

        }
        #endregion
    
        #region BroadCast
        private System.Action<string> _broadcastAct = null;
        public void RegisterBroadcastReciever(System.Action<string> act)
        {
            _broadcastAct += act;
        }

        public void UnRegisterBroadcastReciever(System.Action<string> act)
        {
            _broadcastAct -= act;
        }

        public void BroadCastInRacing(string text)
        {
            _broadcastAct?.Invoke(text);
        }
        #endregion
    }
}