using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;
using System;

namespace Hotfix_LT.UI
{
    public class PlayerInviteManager : ManagerUnit
    {

        private static PlayerInviteManager instance;
        public static PlayerInviteManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<PlayerInviteManager>();
                }
                return instance;
            }
        }
        private PlayerInviteApi api;
        private string inviteCode;
        public string InviteCode
        {
            get
            {
                if (string.IsNullOrEmpty(inviteCode))
                {
                    DataLookupsCache.Instance.SearchDataByID<string>("inviteFriends.invite_code", out inviteCode);
                }
                return inviteCode;
            }
        }
        private int dailyShareReward;
        public int DailyShareReward
        {
            get
            {
                if (dailyShareReward == -1)
                {
                    dailyShareReward = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("DailyInviteReward");
                }
                return dailyShareReward;
            }
        }
        public bool CouldRecieveDailyShare 
        {
            get
            {
                DataLookupsCache.Instance.SearchDataByID<bool>("inviteFriends.got_share_award", out bool ShareTimes);
                return  !ShareTimes;
            }
        }
        public static bool isFirstShare;
        public static long sharetime;
        //private const string InviteDataId = "inviteFriends.task";
        //public PlayerInviteTaskData InviteTaskData { get; private set; }
        public bool HasBindInviteCode
        {
            get
            {
                DataLookupsCache.Instance.SearchDataByID<string>("inviteFriends.used_code", out string usecode);
                return !string.IsNullOrEmpty(usecode);
            }
        }

        public int InvitePlayersNum {
            get
            {
                DataLookupsCache.Instance.SearchDataByID<int>("inviteFriends.friendNum", out int num);
                return num;
            }
        }
        public int InviteHcGotNum 
        {
            get
            {
                DataLookupsCache.Instance.SearchDataByID<int>("inviteFriends.friendHc", out int hccount);
                return hccount;
            }
        }
        public override void Initialize(Config config)
        {
            Instance.api = new PlayerInviteApi();
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, instance.ReflashRedPoint);
        }

        public override void OnLoggedIn()
        {
            inviteCode = null;
            isFirstShare = false;
            dailyShareReward = -1;
            sharetime = 0;
        }

        public override void OnEnteredBackground()
        {
            base.OnEnteredBackground();
        }
        public override void OnEnteredForeground()
        {
            base.OnEnteredForeground();
            long timenow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (CouldRecieveDailyShare && isFirstShare && timenow - sharetime>=3)
            {              
                api.GotDailyShareReward(delegate (Hashtable data)
                {
                    DataLookupsCache.Instance.CacheData(data);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INVITE_26"));
                    List<LTShowItemData> showItemData = new List<LTShowItemData>();
                    showItemData.Add(new LTShowItemData("hc", DailyShareReward, LTShowItemType.TYPE_RES));
                    GlobalMenuManager.Instance.Open("LTShowRewardView", showItemData);
                    Messenger.Raise(EventName.OnInviteShareSucceed);
                    isFirstShare = false;
                });
            }

        }

        public int inviteloginnum;
        public int inviteuplevelnum;
        public int invitechargenum;
        public int invitedloginnum;
        public int inviteduplevelnum;
        public int invitedchargenum;
        public int invitenum;
        public int invitednum;

        public int inviteloginrednum;
        public int inviteuplevelrednum;
        public int invitechargerednum;
        public int invitedloginrednum;
        public int inviteduplevelrednum;
        public int invitedchargerednum;
        /// <summary>
        /// 红点相关
        /// </summary>
        public void ReflashRedPoint()
        {
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("tasks",out Hashtable task);
            if (task != null)
            {
                inviteloginnum = 0;
                inviteuplevelnum = 0;
                invitechargenum = 0;
                invitedloginnum = 0;
                inviteduplevelnum = 0;
                invitedchargenum = 0;
                inviteloginrednum = 0;
                inviteuplevelrednum = 0;
                invitechargerednum = 0;
                invitedloginrednum = 0;
                inviteduplevelrednum = 0;
                invitedchargerednum = 0;
                var enumerator = task.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DictionaryEntry v = enumerator.Entry;
                    IDictionary dic = v.Value as IDictionary;
                    int tasktype = EB.Dot.Integer("task_type", dic, 0);
                    if(tasktype == (int)eTaskType.Invite)
                    {
                        int taskid = EB.Dot.Integer("task_id", dic, 0);
                        TaskTemplate tasktpl = TaskTemplateManager.Instance.GetTask(taskid);
                        if (tasktpl == null) continue;
                        eTaskTabType tab = GetInviteTaskTab(tasktpl.target_parameter_1);
                        eInvitePageType page = GetTaskPage(taskid);
                        string state = EB.Dot.String("state", dic, string.Empty);
                        if (page == eInvitePageType.inviteOther && tab == eTaskTabType.logion)
                        {
                            if (state.Equals(TaskSystem.FINISHED))
                            {
                                inviteloginrednum += 1;
                                inviteloginnum += 1;
                            }
                            else if (state.Equals(TaskSystem.RUNNING))
                            {
                                inviteloginnum += 1;
                            }
                        }
                        else if (page == eInvitePageType.inviteOther && tab == eTaskTabType.levelup)
                        {
                            if (state.Equals(TaskSystem.FINISHED))
                            {
                                inviteuplevelrednum += 1;
                                inviteuplevelnum += 1;
                            }
                            else if (state.Equals(TaskSystem.RUNNING))
                            {
                                inviteuplevelnum += 1;
                            }

                        }
                        else if(page == eInvitePageType.inviteOther && tab == eTaskTabType.charge)
                        {
                            if (state.Equals(TaskSystem.FINISHED))
                            {
                                invitechargerednum += 1;
                                invitechargenum += 1;
                            }
                            else if (state.Equals(TaskSystem.RUNNING))
                            {
                                invitechargenum += 1;
                            }
                        }
                        else if(page == eInvitePageType.invited && tab == eTaskTabType.logion)
                        {
                            if (state.Equals(TaskSystem.FINISHED))
                            {
                                invitedloginrednum += 1;
                                invitedloginnum += 1;
                            }
                            else if (state.Equals(TaskSystem.RUNNING))
                            {
                                invitedloginnum += 1;
                            }

                        }
                        else if(page == eInvitePageType.invited && tab == eTaskTabType.levelup)
                        {
                            if (state.Equals(TaskSystem.FINISHED))
                            {
                                inviteduplevelrednum += 1;
                                inviteduplevelnum += 1;
                            }
                            else if (state.Equals(TaskSystem.RUNNING))
                            {
                                inviteduplevelnum += 1;
                            }
                        }
                        else if (page == eInvitePageType.invited && tab == eTaskTabType.charge)
                        {
                            if (state.Equals(TaskSystem.FINISHED))
                            {
                                invitedchargerednum += 1;
                                invitedchargenum += 1;
                            }
                            else if (state.Equals(TaskSystem.RUNNING))
                            {
                                invitedchargenum += 1;
                            }
                        }
                    }
                }
                invitenum = inviteloginnum + inviteuplevelnum + invitechargenum;
                invitednum = invitedloginnum + inviteduplevelnum + invitedchargenum;
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.invitelogin, inviteloginrednum);
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.invitelevelup, inviteuplevelrednum);
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.invitecharge, invitechargerednum);
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.invitedlogin, invitedloginrednum);
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.invitedlevelup, inviteduplevelrednum);
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.invitedcharge, invitedchargerednum);

            }
        }


        public static eInvitePageType GetTaskPage(int taskId)
        {
            int pagenum = taskId % 1000 / 100;
            return (eInvitePageType)(pagenum + 1);
        }
        public static eTaskTabType GetInviteTaskTab(string param)
        {
            if (int.TryParse(param, out int tab))
            {
                tab = tab % 3;
            }
            return (eTaskTabType)tab;
        }

        public void BindInvitePlayer(string code,System.Action callback)
        {
            api.BindInvitePlayer(code, delegate (Hashtable data)
             {
                 DataLookupsCache.Instance.CacheData(data);
                 callback();
             });
        }

    }

    //public class PlayerInviteTaskData : INodeData
    //{
    //    public List<int> inviteOtherKeyList { get; private set; }
    //    public List<int> invitedKeyList { get; private set; }
    //    private List<InviteTaskItemData> inviteOtherTaskValueList;
    //    private List<InviteTaskItemData> invitedTaskValueList;
    //    private InviteTaskItemData[] AllInviteTaskArray;
    //    private bool isdataInit;

    //    public PlayerInviteTaskData()
    //    {
    //        InitList();
    //    }

    //    private void InitList()
    //    {
    //        if (inviteOtherKeyList == null)
    //        {
    //            inviteOtherKeyList = new List<int>();
    //        }
    //        else
    //        {
    //            inviteOtherKeyList.Clear();
    //        }
    //        if (invitedKeyList == null)
    //        {
    //            invitedKeyList = new List<int>();
    //        }
    //        else
    //        {
    //            invitedKeyList.Clear();
    //        }
    //        if (invitedTaskValueList == null)
    //        {
    //            invitedTaskValueList = new List<InviteTaskItemData>();
    //        }
    //        else
    //        {
    //            invitedTaskValueList.Clear();
    //        }
    //        if (inviteOtherTaskValueList == null)
    //        {
    //            inviteOtherTaskValueList = new List<InviteTaskItemData>();
    //        }
    //        else
    //        {
    //            inviteOtherTaskValueList.Clear();
    //        }
    //    }

    //    public List<InviteTaskItemData> GetShowInviteOtherTaskList()
    //    {
    //        InitTaskData();
    //        return inviteOtherTaskValueList;
    //    }

    //    public List<InviteTaskItemData> GetShowInvitedTaskList()
    //    {
    //        InitTaskData();
    //        return invitedTaskValueList;
    //    }

    //    private void InitTaskData()
    //    {
    //        if (!isdataInit)
    //        {
    //            var tasklist = EventTemplateManager.Instance.GetInviteTaskList();
    //            if (tasklist != null)
    //            {
    //                if (AllInviteTaskArray == null) AllInviteTaskArray = new InviteTaskItemData[tasklist.Count];
    //                for (int i = 0; i < tasklist.Count; i++)
    //                {
    //                    var temptask = tasklist[i];
    //                    int showdatakey = temptask.groupid;
    //                    InviteTaskItemData tempitemdata = new InviteTaskItemData(temptask.taskId,
    //                        temptask.pretask, temptask.param1, temptask.param2, temptask.groupid, temptask.tasktype,temptask.showpage, temptask.reward);
    //                    AllInviteTaskArray[i] = tempitemdata;
    //                    if (temptask.showpage == 1)
    //                    {
    //                        if (inviteOtherKeyList.Contains(showdatakey))
    //                        {
    //                            continue;
    //                        }
    //                        inviteOtherKeyList.Add(showdatakey);
    //                        inviteOtherTaskValueList.Add(tempitemdata);
    //                    }
    //                    else
    //                    {
    //                        if (invitedKeyList.Contains(showdatakey))
    //                        {
    //                            continue;
    //                        }
    //                        invitedKeyList.Add(showdatakey);
    //                        invitedTaskValueList.Add(tempitemdata);
    //                    }
    //                }
    //            }
    //            isdataInit = true;
    //        }
    //    }
    //    private void RefreshTaskData()
    //    {
    //        for (int i = 0; i < AllInviteTaskArray.Length; i++)
    //        {
    //            var taskdata = AllInviteTaskArray[i];
    //            if (taskdata.state != TaskState.finished)
    //            {
    //                continue;
    //            }
    //            if (taskdata.pagetype == 1)
    //            {
    //                int index = inviteOtherKeyList.FindIndex(m => m == taskdata.groupid);
    //                var tempshowdata = inviteOtherTaskValueList[index];
    //                if (taskdata.taskid < tempshowdata.taskid || tempshowdata.state == TaskState.recievereward)
    //                {
    //                    inviteOtherTaskValueList[index] = taskdata;
    //                    Messenger.Raise(EventName.OnInvitePlayerTaskStateChanged);
    //                }
    //            }
    //            else
    //            {
    //                int index = invitedKeyList.FindIndex(m => m == taskdata.groupid);
    //                var tempshowdata = invitedTaskValueList[index];
    //                if (taskdata.taskid < tempshowdata.taskid || tempshowdata.state == TaskState.recievereward)
    //                {
    //                    invitedTaskValueList[index] = taskdata;
    //                    Messenger.Raise(EventName.OnInvitePlayerTaskStateChanged);
    //                }
    //            }
    //        }
    //    }
    //    public void CleanUp()
    //    {
    //        InitList();
    //        isdataInit = false;
    //    }

    //    public object Clone()
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public void OnMerge(object obj)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public void OnUpdate(object obj)
    //    {
    //        if (!isdataInit)
    //        {
    //            InitTaskData();
    //        }
    //        for (int i = 0; i < AllInviteTaskArray.Length; i++)
    //        {
    //            var taskData = EB.Dot.Object(AllInviteTaskArray[i].taskid.ToString(), obj, null);
    //            if (taskData != null)
    //            {
    //                TaskState state = (TaskState)EB.Dot.Integer("state", taskData, 1);
    //                int curTimes = EB.Dot.Integer("times", taskData, 0);
    //                int curPlayers = EB.Dot.Integer("players", taskData, 0);
    //                AllInviteTaskArray[i].ReflashTaskState(state, curTimes, curPlayers);
    //            }
    //        }
    //        RefreshTaskData();
    //    }
    //}

    public enum eTaskTabType
    {
        none = -1,
        charge = 0,
        logion = 1,
        levelup = 2,
        
    }
    public enum eInvitePageType
    {
        none = 0,
        inviteOther = 1,
        invited = 2,
    }
}