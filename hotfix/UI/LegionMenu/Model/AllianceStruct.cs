using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hotfix_LT.UI
{
    public enum eAllianceState
    {
        None = 1,
        Joined = 2,
        Leaved = 3,
    }

    public enum eAllianceMemberRole
    {
        Member = 1,
        Admin = 2,
        ExtraOwner = 3,
        Owner = 4,
    }

    public enum eAllianceUserRace
    {
        Jin = 1,
        Mu = 2,
        Shui = 3,
        Huo = 4,
        Tu = 5,
    }

    public enum eAllianceDartState
    {
        None = 0,
        Transfered = 1,
        Robed = 2,
    }

    public enum eAllianceDartCurrentState
    {
        None = 0,
        Transfer = 1,
        Transfering = 2,
        Robing = 3,
        Rob = 4
    }

    public enum eAllianceTransferState
    {
        None = 0,
        Received = 1,
        Finished = 2,
        Fail = 3
    }

    public enum eAllianceApplyHelpState
    {
        None = 0,
        Applyed = 1,
        Agreed = 2
    }

    public enum eAllianceCopyState
    {
        Lock = 0,
        Unlock,
        Pass
    }

    public enum eAllianceCopyGiftAllocationState
    {
        Not = 0,
        Have
    }

    public enum eBattleResult
    {
        unknown = 0,
        win = 1,
        fail = 2,
        draw = 3
    }

    public class AllianceDataMeta
    {
        public string DataPath
        {
            get; set;
        }

        public float LifeTime
        {
            get; set;
        }

        public float LifeDuration
        {
            get; set;
        }

        public bool Dirty
        {
            get; set;
        }

        public System.Action Updater
        {
            get; set;
        }

        public AllianceDataMeta()
        {

        }

        public AllianceDataMeta(string path)
        {
            DataPath = path;
        }

        public AllianceDataMeta(string path, System.Action updater)
        {
            DataPath = path;
            Updater = updater;
        }

        public AllianceDataMeta(string path, System.Action updater, float lifeDuration)
        {
            DataPath = path;
            Updater = updater;
            LifeDuration = lifeDuration;
        }
    }

    public class AllianceDonate
    {
        public int Gold
        {
            get; set;
        }

        public int HC
        {
            get; set;
        }

        /// <summary>
        /// 捐献的次数
        /// </summary>
        public int Times
        { get; set; }

        public int GoldTimes
        { get; set; }

        public int HcTimes
        { get; set; }

        public int LuxuryTimes
        { get; set; }


        public AllianceDonate(int gold, int hc)
        {
            Gold = gold;
            HC = hc;
        }

        public AllianceDonate()
        {
            Gold = 0;
            HC = 0;
        }
    }

    public class AllianceItem
    {
        public int Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Owner
        {
            get; set;
        }

        public int Level
        {
            get; set;
        }

        public int MemberCount
        {
            get; set;
        }

        public int MaxMemberCount
        {
            get; set;
        }

        /// <summary>
        /// 限制等级 必须高于此等级才能申请加入
        /// </summary>
        public int LimitLevel
        {
            get; set;
        }

        /// <summary>
        /// 是否要审批
        /// </summary>
        public bool IsReview
        {
            get; set;
        }

        /// <summary>
        /// 总经验
        /// </summary>
        public int TotalExp
        {
            get; set;
        }

        /// <summary>
        /// 军团图标id
        /// </summary>
        public int IconID
        {
            get; set;
        }
    }

    public class AllianceDetail : INodeData
    {
        public int Id
        {
            get; set;
        }

        public int CreateTime
        {
            get; set;
        }

        public long CreateUid
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int Level
        {
            get; set;
        }

        public int TechnologyLevel
        {
            get; set;
        }

        public long OwnerUid
        {
            get; set;
        }

        public string OwnerName
        {
            get; set;
        }

        public string Notice
        {
            get; set;
        }

        public int Liveness
        {
            get; set;
        }

        public int Balance
        {
            get; set;
        }

        public int ApplyCount
        {
            get; set;
        }

        public int MemberCount
        {
            get; set;
        }

        public int MaxMemberCount
        {
            get; set;
        }

        public int ActivityCount
        {
            get; set;
        }

        public int AbilityCount
        {
            get; set;
        }

        public int CopySceneCount
        {
            get; set;
        }

        public int CurrentExp
        {
            get; set;
        }
        public int TodayExp
        {
            get; set;
        }
        public int TodayTotalExp
        {
            get; set;
        }

        /// <summary> 帮会图标 </summary>
        public int IconID
        {
            get; set;
        }

        /// <summary> 限制入会等级 </summary>
        public int LimitLevel
        {
            get; set;
        }
        /// <summary> 是否审批 </summary>
        public bool IsReview
        {
            get; set;
        }

        public int MailTimes
        {
            get; set;
        }

        public AllianceDetail()
        {

        }

        public void OnUpdate(object obj)
        {
            Id = EB.Dot.Integer("aid", obj, Id);
            CreateTime = EB.Dot.Integer("createTime", obj, CreateTime);
            CreateUid = EB.Dot.Long("createUid", obj, CreateUid);
            Name = EB.Dot.String("name", obj, Name);
            Level = EB.Dot.Integer("level", obj, Level);
            TechnologyLevel = EB.Dot.Integer("technologyLevel", obj, TechnologyLevel);
            OwnerUid = EB.Dot.Long("ownerUid", obj, OwnerUid);
            OwnerName = EB.Dot.String("ownerName", obj, OwnerName);
            Notice = EB.Dot.String("notice", obj, String.Empty);
            Liveness = EB.Dot.Integer("liveness", obj, Liveness);
            Balance = EB.Dot.Integer("balance", obj, Balance);
            CurrentExp = EB.Dot.Integer("exp", obj, CurrentExp);
            ApplyCount = EB.Dot.Integer("applyCount", obj, ApplyCount);
            MemberCount = EB.Dot.Integer("memberCount", obj, MemberCount);
            MaxMemberCount = EB.Dot.Integer("maxMemberCount", obj, MaxMemberCount);
            ActivityCount = EB.Dot.Integer("activityCount", obj, ActivityCount);
            AbilityCount = EB.Dot.Integer("abilityCount", obj, AbilityCount);
            CopySceneCount = EB.Dot.Integer("copySceneCount", obj, CopySceneCount);
            IconID = EB.Dot.Integer("iconID", obj, IconID);
            LimitLevel = EB.Dot.Integer("limitLevel", obj, LimitLevel);
            IsReview = EB.Dot.Bool("isReview", obj, IsReview);
            MailTimes = EB.Dot.Integer("mailTimes", obj, MailTimes);
            TodayExp = EB.Dot.Integer("boss_exp", obj, TodayExp);
            TodayTotalExp = EB.Dot.Integer("today_exp", obj, TodayTotalExp);
            LegionModel.GetInstance().SetAllianceDetail(this);
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public void CleanUp()
        {

        }

        public object Clone()
        {
            return new AllianceDetail();
        }
    }

    public class AllianceApply
    {
        public long Uid
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int AllianceId
        {
            get; set;
        }

        public double ApplyTime
        {
            get; set;
        }

        public eAllianceUserRace Race
        {
            get; set;
        }

        public int DonateDegree
        {
            get; set;
        }

        public int Level
        {
            get; set;
        }

        public string Portrait
        {
            get; set;
        }

        public string Frame
        {
            get; set;
        }

        public int Skin
        {
            get; set;
        }

    }

    public class AllianceMember
    {
        public long Uid
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int JoinTime
        {
            get; set;
        }

        public int ApplyTime
        {
            get; set;
        }

        public eAllianceMemberRole Role
        {
            get; set;
        }

        public eAllianceUserRace Race
        {
            get; set;
        }

        public int Level
        {
            get; set;
        }

        public int BattleRating
        {
            get; set;
        }

        public int DonateDegree
        {
            get; set;
        }

        public long DonateTime
        {
            get; set;
        }

        public double OfflineTime
        {
            get; set;
        }

        public AllianceDonate Donated
        {
            get; set;
        }

        public double Today
        {
            get; set;
        }

        public int TodayDonateDegree
        {
            get; set;
        }

        public AllianceDonate TodayDonated
        {
            get; set;
        }

        public string Portrait
        {
            get; set;
        }

        public int Skin
        {
            get; set;
        }

        public string HeadFrame
        {
            get; set;
        }

        public int templateId
        {
            get; set;
        }

        public int VipLevel
        {
            get; set;
        }

        public AllianceMember()
        {
            Donated = new AllianceDonate();
            TodayDonated = new AllianceDonate();
        }
    }

    public class AllianceBattleMember
    {
        public long Uid
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public eAllianceUserRace Race
        {
            get; set;
        }

        public int Level
        {
            get; set;
        }

        public int BattleRating
        {
            get; set;
        }

        public string Portrait
        {
            get; set;
        }

        public bool Failed
        {
            get; set;
        }
    }

    public class AllianceBattleAwardMember
    {
        public long Uid
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public eAllianceMemberRole Role
        {
            get; set;
        }

        public int BattleTimes
        {
            get; set;
        }

        public int KillTimes
        {
            get; set;
        }
    }

    public class AllianceForwardSelectionMember : AllianceItem
    {
        public int Score
        {
            get; set;
        }
    }

    public class FinalsMember
    {
        public string Name
        {
            get; set;
        }

        public eBattleResult State
        {
            get; set;
        }
    }

    public class AllianceCopyGiftMember
    {
        public long Uid { get; set; }
        public int Rank { get; set; }
        public string MemberName { get; set; }
        public int DamagePercent { get; set; }
        public int DamageTotal { get; set; }
        public eAllianceCopyGiftAllocationState State { get; set; }
    }

    public class AllianceAccount : INodeData
    {
        public eAllianceState State
        {
            get; set;
        }

        public int AllianceId
        {
            get; set;
        }

        public double JoinTime
        {
            get; set;
        }

        public double ApplyTime
        {
            get; set;
        }

        public double LeaveTime
        {
            get; set;
        }

        public AllianceDonate TotalDonated
        {
            get; set;
        }

        public int DonateDegree
        {
            get; set;
        }

        public int[] History
        {
            get; set;
        }
        public LegionTechInfo legionTechInfo;

        public AllianceAttriAddtion allianceAttriAddtion;
        public AllianceAccount()
        {
            TotalDonated = new AllianceDonate();
            legionTechInfo = new LegionTechInfo();
            allianceAttriAddtion = new AllianceAttriAddtion();
            History = new int[0];
        }

        public void OnUpdate(object obj)
        {
            State = (eAllianceState)EB.Dot.Integer("state", obj, (int)State);
            AllianceId = EB.Dot.Integer("aid", obj, AllianceId);
            JoinTime = EB.Dot.Double("joinTime", obj, JoinTime);
            ApplyTime = EB.Dot.Double("applyTime", obj, ApplyTime);
            LeaveTime = EB.Dot.Double("leaveTime", obj, LeaveTime);
            TotalDonated.Gold = EB.Dot.Integer("totalDonated.gold", obj, TotalDonated.Gold);
            TotalDonated.HC = EB.Dot.Integer("totalDonated.hc", obj, TotalDonated.HC);
            TotalDonated.Times = EB.Dot.Integer("totalDonated.times", obj, TotalDonated.Times);
            DonateDegree = EB.Dot.Integer("donateDegree", obj, DonateDegree);
            History = AllianceUtil.ParseIntergerArray("history", obj, History);
            legionTechInfo.UpdataTechInfo(obj);
            LegionModel.GetInstance().SetAllianceAccount(this);
        }



        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public void CleanUp()
        {
            State = eAllianceState.None;
            AllianceId = 0;
            JoinTime = 0;
            ApplyTime = 0;
            LeaveTime = 0;
            TotalDonated = new AllianceDonate();
            DonateDegree = 0;
            History = new int[0];
            legionTechInfo = new LegionTechInfo();
            legionTechInfo.RemoveTimer();
        }

        public object Clone()
        {
            return new AllianceAccount();
        }
    }


    public class AllianceTotalMessage : INodeData
    {
        public List<AllianceMessage> Members
        {
            get; set;
        }

        public Dictionary<long, string> HistoryMembers;

        public void CleanUp()
        {
            Members.Clear();
        }

        public object Clone()
        {
            return new AllianceTotalMessage();
        }

        public void OnUpdate(object obj)
        {
            Members = Hotfix_LT.EBCore.Dot.List<AllianceMessage, long>(string.Empty, obj, Members, ParseMember);

            LegionModel.GetInstance().SetLegionMessages(Members);
        }

        public void OnMerge(object obj)
        {
            List<AllianceMessage> buffs = Hotfix_LT.EBCore.Dot.List<AllianceMessage, long>(string.Empty, obj, Members, ParseMember);
            if (Members == null)
            {
                Members = buffs;
            }
            else
            {
                Members.AddRange(buffs);
            }
            LegionModel.GetInstance().SetLegionMessages(Members);
        }

        private AllianceMessage ParseMember(object value, long uid)
        {
            if (value == null)
            {
                return null;
            }
            AllianceMessage member = new AllianceMessage();

            member.Uid = EB.Dot.Long("uid", value, member.Uid);
            member.Name = EB.Dot.String("name", value, member.Name);
            member.Time = EB.Dot.Double("time", value, member.Time);
            member.Type = EB.Dot.String("type", value, member.Type);
            return member;
        }

        public AllianceTotalMessage()
        {
            Members = new List<AllianceMessage>();
        }

        //public AllianceMessage Find(long uid)
        //{
        //    AllianceMessage member = Members.Where(m => m.Uid == uid).FirstOrDefault();
        //    return member;
        //}

        //public void Remove(long uid)
        //{
        //    Members.RemoveAll(m => m.Uid == uid);
        //}
    }

    public class AllianceMessage
    {
        public long Uid;
        public double Time;
        public string Type;
        public string Name;
    }

    public class AllianceDetailMembers : INodeData
    {
        public List<AllianceMember> Members
        {
            get; set;
        }

        public Dictionary<long, string> HistoryMembers;

        public void CleanUp()
        {
            Members.Clear();
        }

        public object Clone()
        {
            return new AllianceDetailMembers();
        }

        public void OnUpdate(object obj)
        {
            Members = Hotfix_LT.EBCore.Dot.List<AllianceMember, long>(string.Empty, obj, Members, ParseMember);

            LegionModel.GetInstance().SetLegionMembers(Members);
        }

        public void OnMerge(object obj)
        {
            Members = Hotfix_LT.EBCore.Dot.List<AllianceMember, long>(string.Empty, obj, Members, ParseMember, (member, uid) => member.Uid == uid);
            LegionModel.GetInstance().MergeLegionMembers(Members);
        }

        private AllianceMember ParseMember(object value, long uid)
        {
            if (value == null)
            {
                return null;
            }

            AllianceMember member = Find(uid) ?? new AllianceMember();
            member.Uid = EB.Dot.Long("uid", value, member.Uid);
            member.Name = EB.Dot.String("name", value, member.Name);
            member.JoinTime = EB.Dot.Integer("joinTime", value, member.JoinTime);
            member.ApplyTime = EB.Dot.Integer("applyTime", value, member.ApplyTime);
            member.Role = (eAllianceMemberRole)EB.Dot.Integer("role", value, (int)member.Role);
            member.Race = (eAllianceUserRace)EB.Dot.Integer("race", value, (int)member.Race);
            member.Level = EB.Dot.Integer("level", value, member.Level);
            member.BattleRating = EB.Dot.Integer("battleRating", value, member.BattleRating);
            member.DonateDegree = EB.Dot.Integer("donateDegree", value, member.DonateDegree);
            member.DonateTime = EB.Dot.Long("donateTime", value, member.DonateTime);
            member.OfflineTime = EB.Dot.Double("offlineTime", value, member.OfflineTime);
            member.Donated.Gold = EB.Dot.Integer("donated.gold", value, member.Donated.Gold);
            member.Donated.Times = EB.Dot.Integer("donated.times", value, member.Donated.Times);
            member.Donated.HC = EB.Dot.Integer("donated.hc", value, member.Donated.HC);
            member.Today = EB.Dot.Double("today", value, member.Today);
            member.TodayDonateDegree = EB.Dot.Integer("todayDonateDegree", value, member.TodayDonateDegree);
            member.TodayDonated.Gold = EB.Dot.Integer("todayDonated.gold", value, member.TodayDonated.Gold);
            member.TodayDonated.HC = EB.Dot.Integer("todayDonated.hc", value, member.TodayDonated.HC);
            member.TodayDonated.GoldTimes = EB.Dot.Integer("todayDonated.goldTimes", value, member.TodayDonated.GoldTimes);
            member.TodayDonated.HcTimes = EB.Dot.Integer("todayDonated.hcTimes", value, member.TodayDonated.HcTimes);
            member.TodayDonated.LuxuryTimes = EB.Dot.Integer("todayDonated.luxuryTimes", value, member.TodayDonated.LuxuryTimes);
            member.TodayDonated.Times = EB.Dot.Integer("todayDonated.times", value, member.TodayDonated.Times);
            member.Portrait = EB.Dot.String("portrait", value, member.Portrait);
            member.Skin = EB.Dot.Integer("skin", value, member.Skin);
            member.HeadFrame = EB.Dot.String("headFrame", value, member.HeadFrame);
            member.templateId = EB.Dot.Integer("tplId", value, member.templateId);
            member.VipLevel = EB.Dot.Integer("vipLevel", value, member.VipLevel);
            return member;
        }

        public AllianceDetailMembers()
        {
            Members = new List<AllianceMember>();
        }

        public AllianceMember Find(long uid)
        {
            AllianceMember member = Members.Where(m => m.Uid == uid).FirstOrDefault();
            return member;
        }

        public void Remove(long uid)
        {
            Members.RemoveAll(m => m.Uid == uid);
        }
    }

    //五族大战的人员
    public class AllianceBattleInfo : INodeData
    {
        public bool IsAtRankList;
        public bool IsHaveApply;
        public bool IsPass;
        public bool IsHaveGift;
        public bool IsFieldFull;
        public bool IsHaveCancelApply;
        public bool IsNeedApplyCondition = true;
        public bool IsHaveEnterField;
        public string OtherAllianceName;
        public eBattleResult BattleResult;
        public bool IsEnterSuccess;

        public void CleanUp()
        {
            IsAtRankList = false;
            IsHaveApply = false;
            IsPass = false;
            IsHaveGift = false;
            IsFieldFull = false;
            IsHaveCancelApply = false;
            IsNeedApplyCondition = true;
            IsHaveEnterField = false;
            OtherAllianceName = "";
            BattleResult = eBattleResult.unknown;
            IsEnterSuccess = false;
        }

        public object Clone()
        {
            return new AllianceBattleInfo();
        }

        public void OnUpdate(object obj)
        {
            Hashtable hs = obj as Hashtable;
            OtherAllianceName = EB.Dot.String("otherAllianceName", hs, OtherAllianceName);
            BattleResult = Hotfix_LT.EBCore.Dot.Enum<eBattleResult>("battleResult", hs, eBattleResult.unknown);
            IsEnterSuccess = EB.Dot.Bool("isEnterSuccess", hs, false);
            IsAtRankList = EB.Dot.Bool("isAtRankList", obj, IsAtRankList);
            IsHaveApply = EB.Dot.Bool("isApply", obj, IsHaveApply);
            IsPass = EB.Dot.Bool("isPass", obj, IsPass);
            IsHaveGift = EB.Dot.Bool("isHaveGift", obj, IsHaveGift);
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }
    }

    public class AllianceBattleMembers : INodeData
    {
        public enum FieldType
        {
            jin, mu, shui, huo, tu, eastsea
        }

        public class AllianceField
        {
            public bool IsUpdate;
            public string Key;
            public string Name;
            public bool IsBattleOver;
            public eBattleResult Result;
            public int Point;  //战绩、得分
            public AllianceBattleMember[] Members;

            public AllianceField()
            {
                Members = new AllianceBattleMember[5];
            }

            public void Clear()
            {
                Members = new AllianceBattleMember[5];
            }
        }

        public bool IsHaveUpdateData;
        public bool IsBattleOver;
        public bool IsResultEmpty;
        public Dictionary<string, AllianceField> FieldDic;

        public void CleanUp()
        {
            IsHaveUpdateData = false;
            IsBattleOver = false;
            IsResultEmpty = false;
            FieldDic.Clear();
        }

        public object Clone()
        {
            return new AllianceBattleMembers();
        }

        public AllianceBattleMembers()
        {
            FieldDic = new Dictionary<string, AllianceField>();
        }

        public void OnUpdate(object obj)
        {
            IsHaveUpdateData = true;
            if (obj is Hashtable)
            {
                Hashtable h = obj as Hashtable;
                IsResultEmpty = h.Count == 0;

                //OtherAllianceName = EB.Dot.String("otherAllianceName", h, "");
                //Debug.Log("OtherAllianceName="+ OtherAllianceName);
                foreach (DictionaryEntry kv in h)
                {
                    string fieldKey = kv.Key as string;

                    AllianceField allianceField = null;
                    if (FieldDic.ContainsKey(fieldKey))
                        allianceField = FieldDic[fieldKey];
                    else
                        allianceField = new AllianceField();
                    if (allianceField == null)
                        EB.Debug.LogError("allianceField == null");

                    Hashtable info = kv.Value as Hashtable;
                    if (info["point"] != null)
                    {
                        allianceField.Name = EB.Localizer.GetString(GameStringValue.FieldNameDic[fieldKey]);
                        allianceField.Key = fieldKey;

                        int point = EB.Dot.Integer("point", info, 0);
                        allianceField.Point = point;
                    }

                    if (info["isWon"] != null)
                    {
                        allianceField.Result = Hotfix_LT.EBCore.Dot.Enum<eBattleResult>("isWon", info, eBattleResult.unknown);
                        allianceField.IsBattleOver = true;
                        IsBattleOver = true;
                    }
                    else
                    {
                        allianceField.IsBattleOver = false;
                    }
                    Hashtable members = info["playerList"] as Hashtable;
                    ParseMembers(members, allianceField);
                    //allianceField.AddRange(ParseMembers(members), allianceField);
                    allianceField.IsUpdate = true;

                    if (!FieldDic.ContainsKey(fieldKey))
                        FieldDic.Add(fieldKey, allianceField);
                }
            }
            else
                EB.Debug.LogError("obj not is Hashtable obj={0}", obj);

        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        private void ParseMembers(Hashtable playList, AllianceField field)
        {
            if (playList == null)
            {
                return;
            }

            foreach (string memberIndexKey in playList.Keys)
            {
                //string k=i.ToString();
                object v = playList[memberIndexKey];
                int memberIndex = 0;
                if (!int.TryParse(memberIndexKey, out memberIndex))
                {
                    EB.Debug.LogError("memberIndexKey parse int fail memberIndexKey={0}", memberIndexKey);
                    memberIndex = 0;
                }
                if (v != null)
                {
                    AllianceBattleMember member = null;
                    if (field.Members[memberIndex] != null)
                        member = field.Members[memberIndex];
                    else
                        member = new AllianceBattleMember();
                    member.Uid = EB.Dot.Long("uid", v, member.Uid);
                    member.Name = EB.Dot.String("name", v, member.Name);
                    member.Race = (eAllianceUserRace)EB.Dot.Integer("race", v, (int)member.Race);
                    member.Level = EB.Dot.Integer("level", v, member.Level);
                    member.BattleRating = EB.Dot.Integer("battleRating", v, member.BattleRating);
                    member.Portrait = EB.Dot.String("portrait", v, member.Portrait);
                    if (EB.Dot.Bool("failed", v, member.Failed))
                    {
                        EB.Debug.LogError("battle member word failed is null");
                        member.Failed = EB.Dot.Bool("failed", v, member.Failed);
                    }
                    else
                        member.Failed = false;

                    field.Members[memberIndex] = member;
                }
                else
                    field.Members[memberIndex] = null;
            }
        }
    }

    public class AllianceBattleAwards : INodeData
    {
        public List<AllianceBattleAwardMember> AwardMembers
        {
            get; set;
        }
        public ArrayList GotAwardUids
        {
            get; set;
        }

        public Dictionary<string, LTShowItemData> BoxsDic;

        public AllianceBattleAwards()
        {
            AwardMembers = new List<AllianceBattleAwardMember>();
            GotAwardUids = Johny.ArrayListPool.Claim();
            BoxsDic = new Dictionary<string, LTShowItemData>();
        }

        public void CleanUp()
        {
            AwardMembers.Clear();
            GotAwardUids.Clear();
            BoxsDic.Clear();
        }

        public object Clone()
        {
            return new AllianceBattleAwards();
        }

        public void OnUpdate(object obj)
        {
            GotAwardUids = Hotfix_LT.EBCore.Dot.Array("gotAwardUids", obj, GotAwardUids);

            Hashtable membersHs = (obj as Hashtable)["memberStats"] as Hashtable;
            if (membersHs != null)
            {
                AwardMembers = Hotfix_LT.EBCore.Dot.List<AllianceBattleAwardMember, long>(null, membersHs, AwardMembers, ParseMember);  //, (member, uid) => member.Uid == uid
                MergeAllMember(AwardMembers);

                for (var i = 0; i < GotAwardUids.Count; i++)
                {
                    object uid = GotAwardUids[i];
                    AwardMembers.RemoveAll(m => m.Uid == long.Parse(uid.ToString()));
                }
            }
            //else
            //{
            //	EBCore.Debug.LogError("allianceBattleAward membersData = null");
            //}

            Hashtable awardsHs = (obj as Hashtable)["awards"] as Hashtable;
            if (awardsHs != null)
            {
                BoxsDic.Clear();
                foreach (DictionaryEntry kv in awardsHs)
                {
                    string type = EB.Dot.String("type", kv.Value, "");
                    string id = EB.Dot.String("data", kv.Value, "");
                    int quality = EB.Dot.Integer("quantity", kv.Value, 0);

                    LTShowItemData itemData = new LTShowItemData(id, quality, type);
                    string boxName = kv.Key as string;
                    if (!BoxsDic.ContainsKey(boxName))
                        BoxsDic.Add(boxName, itemData);
                    else
                        BoxsDic[boxName] = itemData;
                }
            }
            else
                EB.Debug.LogError("allianceBattleAward = null");

            //AwardMembers.Sort(new AllianceGiftAllocationUI.AllianceBattleAwardMemberComparer());
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
            //AwardMembers = Hotfix_LT.EBCore.Dot.List<AllianceBattleAwardMember, int>(null, obj, AwardMembers, ParseApply, (apply, aid) => apply.AllianceId == aid);
        }

        private AllianceBattleAwardMember ParseMember(object value, long uid)
        {
            if (value == null)
            {
                return null;
            }

            AllianceBattleAwardMember member = Find(uid) ?? new AllianceBattleAwardMember();
            member.Uid = uid; //EB.Dot.Long("uid", value, member.Uid);
            member.BattleTimes = EB.Dot.Integer("battleTimes", value, member.BattleTimes);
            member.KillTimes = EB.Dot.Integer("killTimes", value, member.KillTimes);

            AllianceMember m = AlliancesManager.Instance.DetailMembers.Find(member.Uid);
            if (m != null)
            {
                member.Name = m.Name;
                member.Role = m.Role;
            }
            else
                EB.Debug.LogError("DetailMember=null uid= {0}", member.Uid);

            return member;
        }

        private void MergeAllMember(List<AllianceBattleAwardMember> awardMembers)
        {
            List<AllianceMember> members = AlliancesManager.Instance.DetailMembers.Members;
            int count = members.Count;
            for (int i = 0; i < count; ++i)
            {
                AllianceMember v = members[i];
                if (!awardMembers.Exists(m => m.Uid == v.Uid))
                {
                    AllianceBattleAwardMember m = new AllianceBattleAwardMember();
                    m.Uid = v.Uid;
                    m.Name = v.Name;
                    m.Role = v.Role;
                    m.BattleTimes = 0;
                    m.KillTimes = 0;
                    awardMembers.Add(m);
                }
            }
        }

        public AllianceBattleAwardMember Find(long uid)
        {
            return AwardMembers.Find(m => m.Uid == uid);
        }

        public void Remove(long uid)
        {
            AwardMembers.RemoveAll(m => m.Uid == uid);
        }

        public int GetResidueAwardMemberCount()
        {
            return AwardMembers.Count;
        }
    }

    public class AllianceForwardSelectionMembers : INodeData
    {
        public List<AllianceForwardSelectionMember> Members
        {
            get; private set;
        }

        public AllianceForwardSelectionMembers()
        {
            Members = new List<AllianceForwardSelectionMember>();
        }

        public void CleanUp()
        {
            Members.Clear();
        }

        public object Clone()
        {
            return new AllianceForwardSelectionMembers();
        }

        public void OnUpdate(object obj)
        {
            Members = Hotfix_LT.EBCore.Dot.List<AllianceForwardSelectionMember, int>(string.Empty, obj, Members, ParseMember);
        }

        public void OnMerge(object obj)
        {
            Members = Hotfix_LT.EBCore.Dot.List<AllianceForwardSelectionMember, int>(string.Empty, obj, Members, ParseMember, (member, aid) => member.Id == aid);
        }

        private AllianceForwardSelectionMember ParseMember(object value, int aid)
        {
            if (value == null)
            {
                return null;
            }

            AllianceForwardSelectionMember member = Find(aid) ?? new AllianceForwardSelectionMember();
            member.Id = EB.Dot.Integer("aid", value, member.Id);
            member.Name = EB.Dot.String("name", value, member.Name);
            member.Owner = EB.Dot.String("owner", value, member.Owner);
            member.Level = EB.Dot.Integer("level", value, member.Level);
            member.MemberCount = EB.Dot.Integer("memberCount", value, member.MemberCount);
            member.MaxMemberCount = EB.Dot.Integer("maxMemberCount", value, member.MaxMemberCount);
            member.Score = EB.Dot.Integer("score", value, member.Score);
            return member;
        }

        public AllianceForwardSelectionMember Find(int aid)
        {
            AllianceForwardSelectionMember member = Members.Where(m => m.Id == aid).FirstOrDefault();
            return member;
        }

        public void Remove(int aid)
        {
            Members.RemoveAll(m => m.Id == aid);
        }
    }

    public class AllianceFinalsMembers : INodeData
    {
        public List<FinalsMember> QuarterMembers;
        public List<FinalsMember> HalfMembers;
        public List<FinalsMember> FinalsMembers;
        public List<FinalsMember> ChampionMembers;

        public void CleanUp()
        {
            QuarterMembers.Clear();
            HalfMembers.Clear();
            FinalsMembers.Clear();
            ChampionMembers.Clear();
        }

        public object Clone()
        {
            return new AllianceFinalsMembers();
        }

        public void OnUpdate(object obj)
        {
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("quarter", obj, null);
            ParseMembers(list, QuarterMembers);
            list = Hotfix_LT.EBCore.Dot.Array("half", obj, null);
            ParseMembers(list, HalfMembers);
            list = Hotfix_LT.EBCore.Dot.Array("finals", obj, null);
            ParseMembers(list, FinalsMembers);
            list = Hotfix_LT.EBCore.Dot.Array("champion", obj, null);
            ParseMembers(list, ChampionMembers);
        }

        private void ParseMembers(ArrayList objList, List<FinalsMember> targetList)
        {
            targetList.Clear();
            if (objList != null)
            {
                for (var i = 0; i < objList.Count; i++)
                {
                    IDictionary member = (IDictionary)objList[i];
                    FinalsMember fm = new FinalsMember();
                    fm.Name = EB.Dot.String("name", member, string.Empty);
                    fm.State = Hotfix_LT.EBCore.Dot.Enum<eBattleResult>("isWon", member, eBattleResult.unknown);
                    targetList.Add(fm);
                }
            }
            else
            {
                EB.Debug.LogError("finals members = null");
            }
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public AllianceFinalsMembers()
        {
            QuarterMembers = new List<FinalsMember>();
            HalfMembers = new List<FinalsMember>();
            FinalsMembers = new List<FinalsMember>();
            ChampionMembers = new List<FinalsMember>();
        }
    }

    public class TransferDartMember
    {
        public string Id { get; set; }
        public string DartName { get; set; }
        public string TargetScene { get; set; }
        public string TargetNpc { get; set; }
        public LTShowItemData[] Award { get; set; }
        public eAllianceTransferState State { get; set; }
    }

    public class TransferDartMemberComparer : IComparer<TransferDartMember>
    {
        public int Compare(TransferDartMember x, TransferDartMember y)
        {
            int xDartIndex = GameStringValue.DartIndexDic[x.DartName];
            int yDartIndex = GameStringValue.DartIndexDic[y.DartName];
            if (xDartIndex < 0 || yDartIndex < 0)
                EB.Debug.LogError("sort DartIndex<0");

            return yDartIndex - xDartIndex;
        }
    }

    public class RobDartMemberComparer : IComparer<RobDartMember>
    {
        public int Compare(RobDartMember x, RobDartMember y)
        {
            int xDartIndex = GameStringValue.DartIndexDic[x.DartName];
            int yDartIndex = GameStringValue.DartIndexDic[y.DartName];
            if (xDartIndex < 0 || yDartIndex < 0)
                EB.Debug.LogError("sort DartIndex<0");

            return xDartIndex - yDartIndex;
        }
    }

    public class RobDartMember
    {
        public long Uid { get; set; }
        public string DartId { get; set; }
        public string DartName { get; set; }
        public string TargetScene { get; set; }
        public string Portrait { get; set; }
        public int PlayerSkin { get; set; }
        public string HeadFrame { get; set; }
        public int QualityLevel { get; set; }
        public int PlayerLevel { get; set; }
        public string PlayerName { get; set; }
        public List<LTShowItemData> Award { get; set; }
    }

    public class ApplyHelpNode
    {
        public long Uid { get; set; }
        public string InviteId { get; set; }
        public int PlayerLevel { get; set; }
        public string PlayerName { get; set; }
        public string Portrait { get; set; }
        public int PlayerSkin { get; set; }
        public string HeadFrame { get; set; }
        public int QualityLevel { get; set; }
        public eAllianceApplyHelpState State { get; set; }
        //public ArrayList BuddyPortraits { get; set; }
    }

    public class HelpApplyNode
    {
        public long Uid { get; set; }
        public string InviteId { get; set; }
        public string DartName { get; set; }
        public int PlayerLevel { get; set; }
        public string PlayerName { get; set; }
        public string Portrait { get; set; }
        public string HeadFrame { get; set; }
        public int QualityLevel { get; set; }
        public LTShowItemData Award { get; set; }
    }

    //劫镖运镖
    #region AllianceDart

    public class AllianceDartConfig : INodeData
    {
        public int TransferDartTimeLimit; // need 30 minute finish    unit:seconds
        public int AttackRedNameLevelLimit;
        public int RobArenaRankLimit;

        public void CleanUp()
        {
            TransferDartTimeLimit = 0;
            AttackRedNameLevelLimit = 0;
            RobArenaRankLimit = 0;
        }

        public object Clone()
        {
            return new AllianceDartConfig();
        }

        public void OnUpdate(object obj)
        {
            TransferDartTimeLimit = EB.Dot.Integer("transferDartTimeLimit", obj, TransferDartTimeLimit);
            AttackRedNameLevelLimit = EB.Dot.Integer("attackRedNameLevelLimit", obj, AttackRedNameLevelLimit);
            RobArenaRankLimit = EB.Dot.Integer("robArenaRankLimit", obj, RobArenaRankLimit);
            //State = Hotfix_LT.EBCore.Dot.Enum<eAllianceDartState>("info.state", obj, State);
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public AllianceDartConfig()
        {
            CleanUp();
        }
    }

    public class AllianceDartData : INodeData
    {
        public eAllianceDartState DartState;
        public eAllianceDartCurrentState State;
        public string CurrentDartId;
        public int HaveEscortNum;
        public int HaveRobNum;
        public int HaveEscortAndRobTimes;

        public void CleanUp()
        {
            DartState = eAllianceDartState.None;
            State = eAllianceDartCurrentState.None;
            CurrentDartId = string.Empty;
            HaveEscortNum = 0;
            HaveRobNum = 0;
            HaveEscortAndRobTimes = 0;
        }

        public object Clone()
        {
            return new AllianceDartData();
        }

        public void OnUpdate(object obj)
        {
            //TransferDartTimeLimit = EB.Dot.Integer("config.transferDartTimeLimit", obj, TransferDartTimeLimit);
            DartState = (eAllianceDartState)EB.Dot.Integer("dartState", obj, (int)DartState);
            State = (eAllianceDartCurrentState)EB.Dot.Integer("state", obj, (int)State);
            int forwardNum = HaveEscortNum;
            HaveEscortNum = EB.Dot.Integer("escort_times", obj, HaveEscortNum);
            HaveRobNum = EB.Dot.Integer("rob_times", obj, HaveRobNum);
            HaveEscortAndRobTimes = EB.Dot.Integer("haveEscortAndRobTimes", obj, HaveEscortAndRobTimes);
            if (forwardNum != HaveEscortNum) LTDailyDataManager.Instance.SetDailyDataRefreshState();
            LegionLogic.GetInstance().IsOpenConvoy();//调用红点刷新
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public AllianceDartData()
        {
            CleanUp();
        }
    }

    public class AllianceTransferDartInfo : INodeData
    {
        //public int HaveTransferNum;
        public int TransferEndTs; //timeSpan
        public int NextRefreshTs; //timeSpan
        public int HaveFreeRefreshNum;
        public float AdditionRate;
        public string CurrentTaskID;
        public int NextTransferPoint;
        public bool DataUpdated;
        public List<TransferDartMember> Members;

        public void OnUpdate(object obj)
        {
            DataUpdated = true;
            //HaveTransferNum = EB.Dot.Integer("info.haveTransferNum", obj, HaveTransferNum);
            TransferEndTs = EB.Dot.Integer("info.transferEndTs", obj, TransferEndTs);
            NextRefreshTs = EB.Dot.Integer("info.nextRefreshTs", obj, NextRefreshTs);
            HaveFreeRefreshNum = EB.Dot.Integer("info.haveFreeRefreshNum", obj, HaveFreeRefreshNum);
            AdditionRate = EB.Dot.Single("info.additionRate", obj, 0);
            CurrentTaskID = EB.Dot.String("info.current_task_id", obj, "");
            NextTransferPoint = EB.Dot.Integer("info.nextPoint", obj, NextTransferPoint);

            Hashtable hash = EB.Dot.Object("list", obj, null);
            if (hash != null)
            {
                if (hash.Count < 4)
                    Members = Hotfix_LT.EBCore.Dot.List<TransferDartMember, string>(null, hash, Members, ParseMember, (member, id) => member.Id == id);
                else
                {
                    Members = Hotfix_LT.EBCore.Dot.List<TransferDartMember, string>(null, hash, Members, ParseMember);
                    Members.Sort(new TransferDartMemberComparer());
                }

                if (Members.Count > 4)
                    EB.Debug.LogError("tranferDart Members.count={0}", Members.Count);
            }
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public AllianceTransferDartInfo()
        {
            Members = new List<TransferDartMember>();
        }

        public void CleanUp()
        {
            //HaveTransferNum = 0;
            TransferEndTs = 0;
            HaveFreeRefreshNum = 0;
            NextRefreshTs = 0;
            AdditionRate = 0f;
            NextTransferPoint = 0;
            Members.Clear();
        }

        public object Clone()
        {
            return new AllianceTransferDartInfo();
        }

        private TransferDartMember ParseMember(object value, string uid)
        {
            if (value == null)
            {
                return null;
            }

            TransferDartMember member = Find(uid) ?? new TransferDartMember();
            member.Id = EB.Dot.String("id", value, member.Id);
            member.DartName = EB.Dot.String("dartName", value, member.DartName);
            member.TargetScene = EB.Dot.String("targetScene", value, member.TargetScene);
            member.TargetNpc = EB.Dot.String("npcName", value, member.TargetNpc);
            ArrayList awardObj = Hotfix_LT.EBCore.Dot.Array("award", value, null);
            if (awardObj != null)
            {
                member.Award = new LTShowItemData[awardObj.Count];
                for (int i = 0; i < awardObj.Count; ++i)
                {
                    string type = EB.Dot.String("t", awardObj[i], "");
                    string id = EB.Dot.String("n", awardObj[i], "");
                    int quality = EB.Dot.Integer("q", awardObj[i], 0);
                    member.Award[i] = new LTShowItemData(id, quality, type, true);
                    //member.Award[i].SetData(type,id,quality);
                }
            }
            try
            {
                int state = EB.Dot.Integer("transferState", value, 0);
                member.State = (eAllianceTransferState)state;
            }
            catch
            {
                member.State = member.State;
            }
            member.State = (eAllianceTransferState)EB.Dot.Integer("transferState", value, (int)member.State);
            return member;
        }

        public TransferDartMember Find(string id)
        {
            return Members.Find(m => m.Id == id);
        }

        public void Remove(string id)
        {
            Members.RemoveAll(m => m.Id == id);
        }

        public TransferDartMember GetCurrentSelectDart()
        {
            for (int i = 0; i < Members.Count; ++i)
            {
                if (Members[i].Id == CurrentTaskID)
                {
                    return Members[i];
                }
            }

            return null;
        }

        public int GetDartIndex(TransferDartMember dart)
        {
            for (int i = 0; i < Members.Count; ++i)
            {
                if (Members[i] == dart)
                {
                    return i;
                }
            }

            return 0;
        }


        public TransferDartMember GetCurrentDart()
        {
            for (int i = 0; i < Members.Count; ++i)
            {
                if (Members[i].State == eAllianceTransferState.Received)
                {
                    return Members[i];
                }
            }

            return null;
        }

        public string GetCurrentDartId()
        {
            var current = GetCurrentDart();
            if (current != null)
                return current.Id;

            return "";
        }

        public int GetCurrentDartIndex()
        {
            for (int i = 0; i < Members.Count; ++i)
            {
                if (Members[i].State == eAllianceTransferState.Received)
                {
                    return i;
                }
            }

            return 0;
        }

        public void SetCurrentDartState(eAllianceTransferState state)
        {
            var current = GetCurrentDart();
            if (current != null)
                current.State = state;
            else
                EB.Debug.LogError("SetCurrentDartState fail");
        }
    }

    public class AllianceRobDartInfo : INodeData
    {
        //public int HaveRobNum;
        public List<RobDartMember> Members;
        public List<LTShowItemData> RobAwards;

        public void OnUpdate(object obj)
        {
            //HaveRobNum = EB.Dot.Integer("info.haveRobNum", obj, HaveRobNum);
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("list", obj, null);
            if (list != null)
            {
                Members = Hotfix_LT.EBCore.Dot.List<RobDartMember, int>(null, list, Members, ParseMember);  //(member,uid)=>member.Uid==uid
                Members.Sort(new RobDartMemberComparer());  //need sort by receive time
            }
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public AllianceRobDartInfo()
        {
            Members = new List<RobDartMember>();
        }

        public void CleanUp()
        {
            //HaveRobNum = 0;
            Members.Clear();
        }

        public object Clone()
        {
            return new AllianceTransferDartInfo();
        }

        private RobDartMember ParseMember(object value, int uid)
        {
            if (value == null)
            {
                return null;
            }

            RobDartMember member = Find(uid) ?? new RobDartMember();
            member.Uid = EB.Dot.Long("uid", value, member.Uid);
            member.DartId = EB.Dot.String("dartId", value, member.DartId);
            member.DartName = EB.Dot.String("dartName", value, member.DartName);
            member.TargetScene = EB.Dot.String("targetScene", value, member.TargetScene);
            member.Portrait = EB.Dot.String("portrait", value, member.Portrait);
            member.PlayerSkin = EB.Dot.Integer("playerSkin", value, member.PlayerLevel);
            member.HeadFrame = EB.Dot.String("playerHeadFrame", value, member.HeadFrame);
            member.QualityLevel = EB.Dot.Integer("qualityLevel", value, member.QualityLevel);
            member.PlayerLevel = EB.Dot.Integer("playerLevel", value, member.PlayerLevel);
            member.PlayerName = EB.Dot.String("playerName", value, member.PlayerName);
            ArrayList awardObj = Hotfix_LT.EBCore.Dot.Array("award", value, null);
            if (awardObj != null)
            {
                member.Award = new List<LTShowItemData>();
                for (int i = 0; i < awardObj.Count; ++i)
                {
                    string type = EB.Dot.String("t", awardObj[i], "");
                    string id = EB.Dot.String("n", awardObj[i], "");
                    int quality = EB.Dot.Integer("q", awardObj[i], 0);
                    member.Award.Add(new LTShowItemData(id, quality, type, false));
                }
            }
            return member;
        }

        public RobDartMember Find(int uid)
        {
            return Members.Find(m => m.Uid == uid);
        }
    }

    public class AllianceApplyHelpInfo : INodeData
    {
        //public int TransferDartRefreshInterval; // 30 minute
        public int HaveApplyNum;
        //public int ResidueTransferTime;
        public List<ApplyHelpNode> ApplyHelpList;

        public void CleanUp()
        {
            HaveApplyNum = 0;
            //ResidueTransferTime = 0;
            ApplyHelpList.Clear();
        }

        public object Clone()
        {
            return new AllianceApplyHelpInfo();
        }

        public void OnUpdate(object obj)
        {
            HaveApplyNum = EB.Dot.Integer("info.haveApplyNum", obj, HaveApplyNum);
            //ResidueTransferTime = EB.Dot.Integer("info.residueTransferTime", obj, ResidueTransferTime);

            Hashtable list = EB.Dot.Object("list", obj, null);
            if (list != null)
            {
                //if (list.Count <= 1)
                //{
                //	ApplyHelpList = Hotfix_LT.EBCore.Dot.List<ApplyHelpNode, int>(null, list, ApplyHelpList, Parse, (member, uid) => member.Uid == uid);
                //}
                //else
                {
                    ApplyHelpList = Hotfix_LT.EBCore.Dot.List<ApplyHelpNode, int>(null, list, ApplyHelpList, Parse);
                }
            }
            else
            {
                EB.Debug.LogError("find alliance.applyHelp.list fail");
            }
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public AllianceApplyHelpInfo()
        {
            ApplyHelpList = new List<ApplyHelpNode>();
        }

        private ApplyHelpNode Parse(object value, int uid)
        {
            if (value == null)
            {
                return null;
            }

            ApplyHelpNode member = Find(uid) ?? new ApplyHelpNode();
            member.Uid = EB.Dot.Long("uid", value, member.Uid);
            member.InviteId = EB.Dot.String("inviteId", value, member.InviteId);
            member.PlayerLevel = EB.Dot.Integer("playerLevel", value, member.PlayerLevel);
            member.PlayerName = EB.Dot.String("playerName", value, member.PlayerName);
            member.Portrait = EB.Dot.String("portrait", value, member.Portrait);
            member.PlayerSkin = EB.Dot.Integer("playerSkin", value, member.PlayerSkin);
            member.HeadFrame = EB.Dot.String("playerHeadFrame", value, member.HeadFrame);
            member.QualityLevel = EB.Dot.Integer("qualityLevel", value, member.QualityLevel);
            member.State = (eAllianceApplyHelpState)EB.Dot.Integer("state", value, (int)member.State);
            return member;
        }

        public ApplyHelpNode Find(int uid)
        {
            return ApplyHelpList.Find(m => m.Uid == uid);
        }

        public void Remove(long uid)
        {
            ApplyHelpList.RemoveAll(m => m.Uid == uid);
        }

        public void Remove(string inviteId)
        {
            ApplyHelpList.RemoveAll(m => m.InviteId == inviteId);
        }
    }

    public class AllianceHelpApplyInfo : INodeData
    {
        //public int TransferDartRefreshInterval; // 30 minute
        public int HaveHelpNum;
        public List<HelpApplyNode> HelpApplyList;

        public void CleanUp()
        {
            HaveHelpNum = 0;
            HelpApplyList.Clear();
        }

        public object Clone()
        {
            return new AllianceHelpApplyInfo();
        }

        public void OnUpdate(object obj)
        {
            HaveHelpNum = EB.Dot.Integer("info.haveHelpNum", obj, HaveHelpNum);

            Hashtable list = EB.Dot.Object("list", obj, null);
            if (list != null)
            {
                HelpApplyList = Hotfix_LT.EBCore.Dot.List<HelpApplyNode, long>(null, list, HelpApplyList, Parse);
            }
        }

        public void OnMerge(object obj)
        {
            HaveHelpNum = EB.Dot.Integer("info.haveHelpNum", obj, HaveHelpNum);

            Hashtable list = EB.Dot.Object("list", obj, null);
            if (list != null)
            {
                HelpApplyList = Hotfix_LT.EBCore.Dot.List<HelpApplyNode, long>(null, list, HelpApplyList, Parse, (member, uid) => member.Uid == uid);
            }
        }

        public AllianceHelpApplyInfo()
        {
            HelpApplyList = new List<HelpApplyNode>();
        }

        private HelpApplyNode Parse(object value, long uid)
        {
            if (value == null)
            {
                return null;
            }

            HelpApplyNode member = Find(uid) ?? new HelpApplyNode();
            member.Uid = EB.Dot.Long("uid", value, member.Uid);
            member.InviteId = EB.Dot.String("inviteId", value, member.InviteId);
            member.DartName = EB.Dot.String("dartName", value, member.DartName);
            member.PlayerLevel = EB.Dot.Integer("playerLevel", value, member.PlayerLevel);
            member.PlayerName = EB.Dot.String("playerName", value, member.PlayerName);
            member.Portrait = EB.Dot.String("portrait", value, member.Portrait);
            member.HeadFrame = EB.Dot.String("playerHeadFrame", value, member.HeadFrame);
            member.QualityLevel = EB.Dot.Integer("qualityLevel", value, member.QualityLevel);
            //ArrayList awardObj=Hotfix_LT.EBCore.Dot.Array("award", value, null);
            //object awardObj = EB.Dot.Object("award", value, null);
            //if (awardObj != null)
            //{
            //    string type = EB.Dot.String("t", awardObj, "");
            //    string id = EB.Dot.String("n", awardObj, "");
            //    int quality = EB.Dot.Integer("q", awardObj, 0);
            //    member.Award = new ShowItemData(id, quality, type);
            //}
            //else
            //{
            //    EBCore.Debug.LogError("helpApply awardObj==null");
            //}
            return member;
        }

        public HelpApplyNode Find(long uid)
        {
            return HelpApplyList.Find(m => m.Uid == uid);
        }

        public void Remove(long uid)
        {
            HelpApplyList.RemoveAll(m => m.Uid == uid);
        }

        public void Remove(string inviteId)
        {
            HelpApplyList.RemoveAll(m => m.InviteId == inviteId);
        }
    }
    #endregion

    #region AllianceCopy
    public class AllianceCopyInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public eAllianceCopyState State { get; set; }
        public int OpenTime { get; set; }

        public int Locator { get { return CampaignTemplate.locator; } }
        public string SceneName { get { return CampaignTemplate.display_name; } }
        public string ModelName { get { return CampaignTemplate.show_monster; } }
        public string Desc { get { return CampaignTemplate.boss_desc; } }
        public string Feature { get { return CampaignTemplate.boss_factor; } }
        public List<LTShowItemData> DropItems { get { return CampaignTemplate.award_items; } }
        public int UnlockCost { get { return CampaignTemplate.unlock_alli_balance; } }
        public int UnlockLevelLimit { get { return CampaignTemplate.alli_level_limit; } }
        public string BossIcon { get { return CampaignTemplate.boss_icon; } }

        public AllianceCampaignTemplate CampaignTemplate { get { return Hotfix_LT.Data.SceneTemplateManager.Instance.GetAllianceCampaign(Name); } }
    }

    public class AllianceCopyBaseInfo : INodeData
    {
        public int HaveBattleNum { get; set; }  //2 times battle chance
        public int NextResetTime { get; set; }
        public List<AllianceCopyInfo> Infos { get; set; }
        public AllianceCopyInfo CurrentCopyInfo;

        public void CleanUp()
        {
            Infos = new List<AllianceCopyInfo>();
        }

        public object Clone()
        {
            return new AllianceCopyBaseInfo();
        }

        public void InitData()
        {
            Infos = new List<AllianceCopyInfo>();
            Dictionary<string, AllianceCampaignTemplate> campaignDic = Hotfix_LT.Data.SceneTemplateManager.Instance.GetAllianceCampaigns();
            if (campaignDic != null)
            {
                foreach (var cam in campaignDic)
                {
                    AllianceCopyInfo info = new AllianceCopyInfo();
                    info.Name = cam.Key;
                    info.Id = 0;
                    info.State = eAllianceCopyState.Lock;
                    info.OpenTime = 0;
                    Infos.Add(info);
                }
            }
        }

        public void OnUpdate(object obj)
        {
            HaveBattleNum = EB.Dot.Integer("haveBattleNum", obj, HaveBattleNum);
            NextResetTime = EB.Dot.Integer("nextResetTime", obj, NextResetTime);
            object members = EB.Dot.Object("list", obj, null);
            Infos = Hotfix_LT.EBCore.Dot.List<AllianceCopyInfo, string>(null, members, Infos, Parse, (copy, name) => copy.Name == name);
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        private AllianceCopyInfo Parse(object value, string name)
        {
            if (value == null)
            {
                return null;
            }

            AllianceCopyInfo copy = Find(name) ?? new AllianceCopyInfo();
            copy.Id = EB.Dot.Integer("id", value, copy.Id);
            copy.State = (eAllianceCopyState)EB.Dot.Integer("state", value, (int)copy.State);
            copy.OpenTime = EB.Dot.Integer("openTime", value, copy.OpenTime);
            return copy;
        }

        public AllianceCopyBaseInfo()
        {
            //InitData();
        }

        public AllianceCopyInfo Find(string name)
        {
            return Infos.Find(info => info.Name == name);
        }
    }

    public class AllianceCopyRankMembers : INodeData
    {
        public List<AllianceCopyRankItemData> Members
        {
            get; set;
        }
        public int MyRank;

        public void CleanUp()
        {
            Members.Clear();
        }

        public object Clone()
        {
            return new AllianceCopyRankMembers();
        }

        public void OnUpdate(object obj)
        {
            Members = Hotfix_LT.EBCore.Dot.List<AllianceCopyRankItemData, long>(string.Empty, obj, Members, ParseMember);
            MyRank = Members[0].m_Rank;
            Members.RemoveAt(0);
        }

        public void OnMerge(object obj)
        {
            Members = Hotfix_LT.EBCore.Dot.List<AllianceCopyRankItemData, long>(string.Empty, obj, Members, ParseMember, (member, uid) => member.m_Uid == uid);
            MyRank = Members[0].m_Rank;
            Members.RemoveAt(0);
        }

        private AllianceCopyRankItemData ParseMember(object value, long uid)
        {
            if (value == null)
            {
                return null;
            }
            Hashtable hs = value as Hashtable;
            AllianceCopyRankItemData member = new AllianceCopyRankItemData(hs, 0);
            return member;
        }

        public AllianceCopyRankMembers()
        {
            Members = new List<AllianceCopyRankItemData>();
        }

        public AllianceCopyRankItemData Find(long uid)
        {
            AllianceCopyRankItemData member = Members.Where(m => m.m_Uid == uid).FirstOrDefault();
            return member;
        }

        public void Remove(long uid)
        {
            Members.RemoveAll(m => m.m_Uid == uid);
        }
    }

    public class AllianceCopyGiftInfo : INodeData
    {
        public int HaveAllocationNum { get; set; }
        public List<AllianceCopyGiftMember> Gifts { get; set; }

        public void CleanUp()
        {
            Gifts = new List<AllianceCopyGiftMember>();
        }

        public object Clone()
        {
            return new AllianceCopyGiftInfo();
        }

        public void OnUpdate(object obj)
        {
            HaveAllocationNum = EB.Dot.Integer("haveAllocationNum", obj, HaveAllocationNum);
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("list", obj, null);
            Gifts = Hotfix_LT.EBCore.Dot.List<AllianceCopyGiftMember, int>(null, list, Gifts, Parse);
        }

        public void OnMerge(object obj)
        {
            HaveAllocationNum = EB.Dot.Integer("haveAllocationNum", obj, HaveAllocationNum);
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("list", obj, null);
            Gifts = Hotfix_LT.EBCore.Dot.List<AllianceCopyGiftMember, int>(null, list, Gifts, Parse, (copy, uid) => copy.Uid == uid);
        }

        private AllianceCopyGiftMember Parse(object value, int uid)
        {
            if (value == null)
            {
                return null;
            }
            AllianceCopyGiftMember gift = Find(uid) ?? new AllianceCopyGiftMember();
            gift.Uid = EB.Dot.Long("uid", value, gift.Uid);
            gift.Rank = EB.Dot.Integer("rank", value, gift.Rank);
            gift.MemberName = EB.Dot.String("memberName", value, gift.MemberName);
            gift.DamagePercent = EB.Dot.Integer("damagePercent", value, gift.DamagePercent);
            gift.DamageTotal = EB.Dot.Integer("damageTotal", value, gift.DamageTotal);
            gift.State = Hotfix_LT.EBCore.Dot.Enum<eAllianceCopyGiftAllocationState>("state", value, gift.State);
            return gift;
        }

        public AllianceCopyGiftInfo()
        {
            Gifts = new List<AllianceCopyGiftMember>();
        }

        public AllianceCopyGiftMember Find(long uid)
        {
            return Gifts.Find(info => info.Uid == uid);
        }
    }

    public class AllianceInventoryInfo : INodeData
    {
        public int NextFreeTs { get; set; }
        public int Cost { get; set; }
        public List<LTShowItemData> ItemDatas { get; set; }

        public void CleanUp()
        {
            ItemDatas = new List<LTShowItemData>();
        }

        public object Clone()
        {
            return new AllianceInventoryInfo();
        }

        public void OnUpdate(object obj)
        {
            NextFreeTs = EB.Dot.Integer("nextFreeTs", obj, NextFreeTs);
            Cost = EB.Dot.Integer("cost", obj, Cost);
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("items", obj, null);
            ItemDatas = Hotfix_LT.EBCore.Dot.List<LTShowItemData, string>(null, list, ItemDatas, Parse);
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        private LTShowItemData Parse(object value, string id)
        {
            if (value == null)
            {
                return null;
            }

            string type = EB.Dot.String("type", value, "");
            string dataId = EB.Dot.String("data", value, "");
            int count = EB.Dot.Integer("quantity", value, 0);
            LTShowItemData item = new LTShowItemData(dataId, count, type);
            return item;
        }

        public AllianceInventoryInfo()
        {
            ItemDatas = new List<LTShowItemData>();
        }

        public LTShowItemData Find(string id)
        {
            return ItemDatas.Find(item => item.id == id);
        }
    }

    #endregion

    /// <summary>
    /// 账号自己申请过的入会列表 会随着发送搜索公会列表带来此数据
    /// </summary>
    public class AllianceAccountApplies : INodeData
    {
        public List<AllianceApply> AccountApplies
        {
            get; set;
        }

        public void CleanUp()
        {
            AccountApplies.Clear();
        }

        public object Clone()
        {
            return new AllianceAccountApplies();
        }

        public void OnUpdate(object obj)
        {
            AccountApplies = Hotfix_LT.EBCore.Dot.List<AllianceApply, int>(null, obj, AccountApplies, ParseApply);
            LegionModel.GetInstance().SetApplyItemDatas(AccountApplies);
        }

        public void OnMerge(object obj)
        {
            AccountApplies = Hotfix_LT.EBCore.Dot.List<AllianceApply, int>(null, obj, AccountApplies, ParseApply, (apply, aid) => apply.AllianceId == aid);
            LegionModel.GetInstance().SetApplyItemDatas(AccountApplies);
        }

        private AllianceApply ParseApply(object value, int aid)
        {
            if (value == null)
            {
                return null;
            }

            AllianceApply apply = Find(aid) ?? new AllianceApply();
            apply.AllianceId = EB.Dot.Integer("aid", value, apply.AllianceId);
            apply.Uid = EB.Dot.Long("uid", value, apply.Uid);
            apply.Name = EB.Dot.String("name", value, apply.Name);
            apply.ApplyTime = EB.Dot.Double("applyTime", value, apply.ApplyTime);
            return apply;
        }

        public AllianceAccountApplies()
        {
            AccountApplies = new List<AllianceApply>();
        }

        public AllianceApply Find(int aid)
        {
            return AccountApplies.Find(apply => apply.AllianceId == aid);
        }

        public void Add(AllianceApply apply)
        {
            AccountApplies.Add(apply);
        }

        public void Remove(int aid)
        {
            AccountApplies.RemoveAll(apply => apply.AllianceId == aid);
            LegionModel.GetInstance().SetApplyItemDatas(AccountApplies);
        }
    }

    /// <summary>
    /// 军团长管理入会申请列表
    /// </summary>
    public class AllianceDetailApplies : INodeData
    {
        public List<AllianceApply> DetailApplies
        {
            get; set;
        }

        public void CleanUp()
        {
            DetailApplies.Clear();
        }

        public object Clone()
        {
            return new AllianceDetailApplies();
        }

        public void OnUpdate(object obj)
        {
            DetailApplies = Hotfix_LT.EBCore.Dot.List<AllianceApply, long>(string.Empty, obj, DetailApplies, ParseApply);

            LegionModel.GetInstance().SetAllianceApplys(DetailApplies);
        }

        public void OnMerge(object obj)
        {
            DetailApplies = Hotfix_LT.EBCore.Dot.List<AllianceApply, long>(string.Empty, obj, DetailApplies, ParseApply, (apply, uid) => apply.Uid == uid);
            LegionModel.GetInstance().SetAllianceApplys(DetailApplies);
        }

        private AllianceApply ParseApply(object value, long uid)
        {
            if (value == null)
            {
                return null;
            }

            AllianceApply apply = Find(uid) ?? new AllianceApply();
            apply.AllianceId = EB.Dot.Integer("aid", value, apply.AllianceId);
            apply.Uid = EB.Dot.Long("uid", value, apply.Uid);
            apply.Name = EB.Dot.String("name", value, apply.Name);
            apply.ApplyTime = EB.Dot.Double("applyTime", value, apply.ApplyTime);
            apply.Race = (eAllianceUserRace)EB.Dot.Integer("race", value, (int)apply.Race);
            apply.DonateDegree = EB.Dot.Integer("donateDegree", value, apply.DonateDegree);
            apply.Level = EB.Dot.Integer("level", value, apply.Level);
            apply.Portrait = EB.Dot.String("portrait", value, apply.Portrait);
            apply.Frame = EB.Dot.String("headFrame", value, "0_0");
            apply.Skin = EB.Dot.Integer("skin", value, 0);
            return apply;
        }

        public void Remove(long uid)
        {
            DetailApplies.RemoveAll(a => a.Uid == uid);
            LegionModel.GetInstance().SetAllianceApplys(DetailApplies);
        }

        public AllianceApply Find(long uid)
        {
            if (DetailApplies.Count == 0)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < DetailApplies.Count; i++)
                {
                    if (DetailApplies[i] == null)
                    {
                        DetailApplies.RemoveAt(i);
                        i--;
                    }
                }
            }
            return DetailApplies.Find(a => a.Uid == uid);
        }

        public AllianceDetailApplies()
        {
            DetailApplies = new List<AllianceApply>();
        }
    }

    public class AllianceItems : INodeData
    {
        public AllianceItem[] List
        {
            get; set;
        }

        public void CleanUp()
        {
            List = new AllianceItem[0];
        }

        public object Clone()
        {
            return new AllianceItems();
        }

        public void OnUpdate(object obj)
        {
            List = Hotfix_LT.EBCore.Dot.Array(string.Empty, obj, List, delegate (object value)
            {
                AllianceItem item = new AllianceItem();
                item.Id = EB.Dot.Integer("aid", value, item.Id);
                item.Name = EB.Dot.String("name", value, item.Name) ?? EB.Dot.String("title", value, item.Name);
                item.Owner = EB.Dot.String("owner", value, item.Owner);
                item.Level = EB.Dot.Integer("level", value, item.Level);
                item.MemberCount = EB.Dot.Integer("memberCount", value, item.MemberCount);
                item.MaxMemberCount = EB.Dot.Integer("maxMemberCount", value, item.MaxMemberCount);
                item.LimitLevel = EB.Dot.Integer("limitLevel", value, item.LimitLevel);
                item.IsReview = EB.Dot.Bool("isReview", value, item.IsReview);
                item.TotalExp = EB.Dot.Integer("exp", value, item.TotalExp);
                item.IconID = EB.Dot.Integer("iconID", value, item.IconID);
                return item;
            });

            //需要依据数据初始化 并排序
            LegionModel.GetInstance().SetSearchItemData(List);


        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public AllianceItems()
        {
            CleanUp();
        }

        public AllianceItem[] Slice(int start, int limit)
        {
            AllianceItem[] result = new AllianceItem[limit];
            for (int i = 0; i < limit; ++i)
            {
                result[i] = (start + i) < List.Length ? List[start + i] : null;
            }
            return result;
        }
    }

    public class AllianceLevelConfig
    {
        public int Level { get; set; }
        public int MaxMemberCount { get; set; }
        public int LevelUpCost { get; set; }
        public int TodayDonateGoldLimit { get; set; }
        public int TodayDonateHCLimit { get; set; }
        public int LevelUpExp { get; set; }
        public float ATKAdd = 0;
        public float DEFAdd = 0;
        public float MaxHPAdd = 0;
    }

    public class AllianceTechnologyConfig
    {
        public int TechLevel;
        public int CostBalance;
        public int AllianceLevelLimit;
        public int MaxSkillLevel;
    }

    public class AllianceCopyBuffConfig
    {
        public int Order;
        public int Id;
        public string Icon;
        public string Desc;
    }

    //public class AllianceCopySceneInfoConfig
    //{
    //	public int Id { get; set; }
    //	public int PreId { get; set; }
    //	public string MonsterName { get; set; }
    //	public string Model { get; set; }
    //	public string BossDesc { get; set; }
    //	public string BossFeature { get; set; }
    //	public int UnlockLevel { get; set; }
    //	public int OpenCost { get; set; }
    //	public int UpdateCost { get; set; }
    //	public int DropAllianceCoin { get; set; }
    //	public string DropItem { get; set; }
    //}

    //public class AllianceCopyLevelConfig
    //{
    //	public int AllianceLevel;
    //	public int CopyMaxLevel;
    //	public int ResetCopyCount;
    //	public int OpenCopyMaxCount;
    //	public int OwnerAllocationLimit;
    //	public int ExtraAllocationLimit;
    //}

    public class AllianceSkillConfig
    {
        //public int SkillLevel;          //技能等级
        //public int CostDonate;          //消耗贡献
        //public int MaxHP;               //生命
        //public int PATK;                //攻击
        //public int PDEF;                //防御
        //public int MATK;                //魔法
        //public int MDEF;                //抗性
        //public int Penetration;         //攻击穿透
        //public int SpellPenetration;    //魔法穿透
        //public int DamageReduction;     //伤害减免
        //public int HealRecover;         //生命回复

        public int SkillLevel;          //技能等级
        public int CostDonate;          //消耗贡献
        public Dictionary<string, int> SkillDic = new Dictionary<string, int>();
    }

    public class AllianceSkillList : INodeData
    {
        public Dictionary<string, int> SkillDic;

        public void CleanUp()
        {
            SkillDic.Clear();
            //Infos = new AllianceSkillInfo[0];
        }

        public object Clone()
        {
            return new AllianceSkillList();
        }

        public void OnUpdate(object obj)
        {
            //Infos = Hotfix_LT.EBCore.Dot.Array(string.Empty, obj, Infos, delegate (object value)
            //{
            //	AllianceSkillInfo item = new AllianceSkillInfo();
            //	item.SkillName = EB.Dot.String("skillName", value, item.SkillName);
            //	item.Level = EB.Dot.Integer("level", value, item.Level);
            //	return item;
            //});

            ArrayList arr = obj as ArrayList;
            for (var i = 0; i < arr.Count; i++)
            {
                var a = arr[i];
                string skillName = EB.Dot.String("skillName", a, string.Empty);
                int level = EB.Dot.Integer("level", a, SkillDic.ContainsKey(skillName) ? SkillDic[skillName] : 0);
                if (SkillDic.ContainsKey(skillName))
                {
                    SkillDic[skillName] = level;
                }
                else
                {
                    SkillDic.Add(skillName, level);
                }
            }
            if (SkillDic.Count != 9)
            {
                EB.Debug.LogError("skillDic.count!={0}", SkillDic.Count);
            }
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public AllianceSkillList()
        {
            SkillDic = new Dictionary<string, int>();
        }
    }

    public class AllianceConfig : INodeData
    {
        public int CostAmount { get; set; }
        public int GoldDonateRate { get; set; }
        public int GoldDonateDegreeRate { get; set; }
        public int GoldBalanceRate { get; set; }
        public int HCDonateRate { get; set; }
        public int HCDonateDegreeRate { get; set; }
        public int HCBalanceRate { get; set; }
        public int ApplyMaxCount { get; private set; }
        public int ReapplyInterval { get; private set; }
        public int MaxExtraOwnerNum { get; private set; }
        public int AllianceBattleApplyNeedMemberCount { get; private set; }
        public int MaxCopyChallengeNum { get; private set; }
        public int LotteryRefreshTime { get; private set; }
        // 军团最大的等级上限
        public int MaxAllianceLevel { get; private set; }
        // 军团一天最大捐献次数
        //public int DonateMaxTimes { get; private set; }
        public int TotalDonateMaxTimes { get; private set; }
        public int GoldDonateMaxTimes { get; private set; }
        public int DiamondDonateMaxTimes { get; private set; }
        public int LuxuryDonateMaxTimes { get; private set; }
        public int CreateCost { get; private set; }
        public int RenameCost { get; private set; }
        public int MaxMailTimes { get; private set; }
        public int MaxAlliCampaignTimes { get; private set; }
        public int DonateGoldSpend { get; private set; }
        public int DonateGoldRedeem { get; private set; }
        public int DonateHcSpend { get; private set; }
        public int DonateHcRedeem { get; private set; }
        public int DonateLuxurySpend { get; private set; }
        public int DonateLuxuryRedeem { get; private set; }
        public int MonthCardReward { get; private set; }


        public AllianceLevelConfig[] Levels { get; private set; }
        public AllianceTechnologyConfig[] Technologys { get; private set; }
        public AllianceSkillConfig[] Skills { get; private set; }
        public Dictionary<int, AllianceCopyBuffConfig> CopyBuffs { get; private set; }

        public void CleanUp()
        {
            GoldDonateRate = 3000;
            GoldDonateDegreeRate = 1;
            GoldBalanceRate = 1;
            HCDonateRate = 1;
            HCDonateDegreeRate = 1;
            HCBalanceRate = 1;
            ApplyMaxCount = 10;
            MaxExtraOwnerNum = 2;
            AllianceBattleApplyNeedMemberCount = 0;
            ReapplyInterval = 8 * 3600;
            MaxAllianceLevel = 15;
            //DonateMaxTimes = 5;
            TotalDonateMaxTimes = 35;
            GoldDonateMaxTimes = 20;
            DiamondDonateMaxTimes = 10;
            LuxuryDonateMaxTimes = 5;
            Levels = null;
            Technologys = null;
            Skills = null;
            CopyBuffs = null;
        }

        public object Clone()
        {
            return new AllianceConfig();
        }

        public void OnUpdateFunc(GM.DataCache.Alliance obj)
        {
            if (obj == null)
            {
                EB.Debug.LogWarning("OnUpdate: obj is null");
                return;
            }

            GM.DataCache.ConditionAlliance conditionSet = obj.GetArray(0);

            Levels = new AllianceLevelConfig[conditionSet.LevelLength + 1];
            Levels[0] = new AllianceLevelConfig();
            for (int i = 0; i < conditionSet.LevelLength; ++i)
            {
                var level = conditionSet.GetLevel(i);
                AllianceLevelConfig lc = new AllianceLevelConfig();
                lc.Level = level.Level;
                //lc.LevelUpCost = level.LevelupBalance;

                lc.ATKAdd = level.ATKAdd;
                lc.DEFAdd = level.DEFAdd;
                lc.MaxHPAdd = level.MaxHPAdd;
                lc.MaxMemberCount = level.MaxMember;
                lc.LevelUpExp = level.LevelupExp;
                //lc.TodayDonateGoldLimit = level.DonateGoldDayLimit;
                //lc.TodayDonateHCLimit = level.DonateHcDayLimit;
                Levels[lc.Level] = lc;

                //UnityEngine.Debug.LogWarning("Level" + lc.Level.ToString() + " MaxMemberCount" + lc.MaxMemberCount + " " + lc.LevelUpExp);
            }
            MaxAllianceLevel = Levels[Levels.Length - 1].Level;

            Technologys = new AllianceTechnologyConfig[conditionSet.TechnologyLength + 1];
            Technologys[0] = new AllianceTechnologyConfig();
            for (int i = 0; i < conditionSet.TechnologyLength; ++i)
            {
                var t = conditionSet.GetTechnology(i);
                AllianceTechnologyConfig tc = new AllianceTechnologyConfig();
                int level = t.TechLevel;
                if (level != i + 1)
                {
                    EB.Debug.LogError("OnUpdate: level != i + 1 level={0}", level);
                }
                tc.TechLevel = i + 1;
                tc.CostBalance = t.CostBalance;
                tc.AllianceLevelLimit = t.AllianceLevelLimit;
                tc.MaxSkillLevel = t.MaxSkillLevel;
                Technologys[tc.TechLevel] = tc;
            }

            Skills = new AllianceSkillConfig[conditionSet.SkillLength + 1];
            Skills[0] = new AllianceSkillConfig();
            for (int i = 0; i < conditionSet.SkillLength; ++i)
            {
                var s = conditionSet.GetSkill(i);
                AllianceSkillConfig sc = new AllianceSkillConfig();
                int level = s.SkillLevel;
                if (level != i + 1)
                {
                    EB.Debug.LogError("OnUpdate: skill level != i + 1 level={0}", level);
                }
                sc.SkillLevel = i + 1;
                sc.CostDonate = s.Cost;
                sc.SkillDic.Add("MaxHP", s.MaxHP);
                sc.SkillDic.Add("PATK", s.PATK);
                sc.SkillDic.Add("PDEF", s.PDEF);
                sc.SkillDic.Add("MATK", s.MATK);
                sc.SkillDic.Add("MDEF", s.MDEF);
                sc.SkillDic.Add("penetration", s.Penetration);
                sc.SkillDic.Add("spell_penetration", s.SpellPenetration);
                sc.SkillDic.Add("damage_reduction", s.DamageReduction);
                sc.SkillDic.Add("heal_recover", s.HealRecover);
                if (sc.SkillDic.Count != 9)
                {
                    EB.Debug.LogError("OnUpdate: alliance SkillDic.Count != 9");
                }
                Skills[sc.SkillLevel] = sc;
            }

            CopyBuffs = new Dictionary<int, AllianceCopyBuffConfig>();
            for (int i = 0; i < conditionSet.CampaignBuffLength; ++i)
            {
                var buff = conditionSet.GetCampaignBuff(i);
                AllianceCopyBuffConfig bf = new AllianceCopyBuffConfig();
                bf.Order = buff.Id;
                bf.Id = buff.BuffId;
                bf.Icon = buff.BuffIcon;
                bf.Desc = buff.BuffDesc;
                CopyBuffs[bf.Id] = bf;
            }
        }

        public void OnUpdate(object obj)
        {
            //CreateCost = ResourceContainer.Parse(Hotfix_LT.EBCore.Dot.Array("createCost", obj, CreateCost.ToJson()));
            GoldDonateRate = EB.Dot.Integer("donate.gold.base", obj, GoldDonateRate);
            GoldDonateDegreeRate = EB.Dot.Integer("donate.gold.addDonate", obj, GoldDonateDegreeRate);
            GoldBalanceRate = EB.Dot.Integer("donate.gold.addBalance", obj, GoldBalanceRate);
            HCDonateRate = EB.Dot.Integer("donate.hc.base", obj, HCDonateRate);
            HCDonateDegreeRate = EB.Dot.Integer("donate.hc.addDonate", obj, HCDonateDegreeRate);
            HCBalanceRate = EB.Dot.Integer("donate.hc.addBalance", obj, HCBalanceRate);
            MaxExtraOwnerNum = EB.Dot.Integer("maxExtraOwnerNum", obj, MaxExtraOwnerNum);
            AllianceBattleApplyNeedMemberCount = EB.Dot.Integer("battleApplyNeedMemberCount", obj, AllianceBattleApplyNeedMemberCount);
            //MaxCopyResetNum = EB.Dot.Integer("maxCopyResetNum", obj, MaxCopyResetNum);
            MaxCopyChallengeNum = EB.Dot.Integer("maxCampaignChallengeNum", obj, MaxCopyChallengeNum);
            //MaxCopyAllocationNum= EB.Dot.Integer("maxCopyAllocationNum", obj, MaxCopyAllocationNum);
            ApplyMaxCount = EB.Dot.Integer("applyMaxCount", obj, ApplyMaxCount);
            ReapplyInterval = EB.Dot.Integer("reapplyInterval", obj, ReapplyInterval);
            //RenameCost = ResourceContainer.Parse(Hotfix_LT.EBCore.Dot.Array("renameCost", obj, RenameCost.ToJson()));


            CreateCost = EB.Dot.Integer("createCost", obj, CreateCost);
            RenameCost = EB.Dot.Integer("renameCost", obj, RenameCost);
            GoldDonateMaxTimes = EB.Dot.Integer("donateGoldTimes", obj, GoldDonateMaxTimes);
            DiamondDonateMaxTimes = EB.Dot.Integer("donateHcTimes", obj, DiamondDonateMaxTimes);
            LuxuryDonateMaxTimes = EB.Dot.Integer("donateLuxuryTimes", obj, LuxuryDonateMaxTimes);
            TotalDonateMaxTimes = GoldDonateMaxTimes + DiamondDonateMaxTimes + LuxuryDonateMaxTimes;
            //DonateMaxTimes = EB.Dot.Integer("donateMaxTimes", obj, DonateMaxTimes);
            MaxMailTimes = EB.Dot.Integer("maxMailTimes", obj, MaxMailTimes);
            MaxAlliCampaignTimes = EB.Dot.Integer("maxAlliCampaignTimes", obj, MaxAlliCampaignTimes);
            DonateGoldSpend = EB.Dot.Integer("donateGoldSpend", obj, DonateGoldSpend);
            DonateGoldRedeem = EB.Dot.Integer("donateGoldRedeem", obj, DonateGoldRedeem);
            DonateHcSpend = EB.Dot.Integer("donateHcSpend", obj, DonateHcSpend);
            DonateHcRedeem = EB.Dot.Integer("donateHcRedeem", obj, DonateHcRedeem);
            DonateLuxurySpend = EB.Dot.Integer("donateLuxurySpend", obj, DonateLuxurySpend);
            DonateLuxuryRedeem = EB.Dot.Integer("donateLuxuryRedeem", obj, DonateLuxuryRedeem);
            MonthCardReward = EB.Dot.Integer("monthCardReward", obj, MonthCardReward);
            //AlliancesManager.Instance.OnUpdatedConfig(this);
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public AllianceConfig()
        {
            CleanUp();
        }

        public int GetSkillAttrValue(string skillName)
        {
            if (!AllianceUtil.IsJoinedAlliance)
                return 0;

            int skillLevel = AllianceUtil.GetSkillLevel(skillName);
            if (skillLevel <= 0)
                return 0;

            if (Skills[skillLevel] != null)
            {
                if (Skills[skillLevel].SkillDic.ContainsKey(skillName))
                    return Skills[skillLevel].SkillDic[skillName];
                else
                    EB.Debug.LogError("Skills[level].SkillDic not ContainsKey(skillName) skillName={0}", skillName);
            }
            else
                EB.Debug.LogError("AllianceSkills[level]==null level={0}", skillLevel);
            return 0;
        }

        public AllianceCopyBuffConfig GetBuff(int id)
        {
            if (CopyBuffs.ContainsKey(id))
                return CopyBuffs[id];

            EB.Debug.LogError("not found buff id={0}", id);
            return null;
        }

        public Dictionary<int, AllianceCopyBuffConfig> GetAllBuff()
        {
            return CopyBuffs;
        }
    }

    public class DonateChestInfo
    {
        public int id
        {
            private set; get;
        }
        public int score
        {
            private set; get;
        }
        public int hasRecieve
        {
            private set; get;
        }
        public DonateChestInfo(int chestid, int chestscore)
        {
            id = chestid;
            score = chestscore;
            hasRecieve = 0;
        }

        public void SetRecieveState(int isRecieve)
        {
            hasRecieve = isRecieve;
        }
    }

    public class LegionDonateInfo : INodeData
    {

        public List<DonateChestInfo> chsetinfolist
        {
            private set; get;
        }
        private bool isChestInit;
        public LegionDonateInfo()
        {
            recievedChestDic = new Dictionary<int, int>();
            chsetinfolist = new List<DonateChestInfo>();
            isChestInit = false;
            chestLevel = 0;
        }
        public int luxuryDonateTimes
        {
            set; get;
        }

        public int hcDonateTimes
        {
            set; get;
        }

        public int goldDonateTimes
        {
            set; get;
        }

        public bool get_hire_reward
        {
            set; get;
        }

        public int today_hire_times
        {
            set; get;
        }
        public int TotalTimes
        {
            set; get;
        }
        private int chestLevel;
        public Dictionary<int, int> recievedChestDic
        {
            set; get;
        }
        private Hashtable chestdata;
        public void CleanUp()
        {
            recievedChestDic = new Dictionary<int, int>();
            chsetinfolist = new List<DonateChestInfo>();
            isChestInit = false;
            chestLevel = 0;
            luxuryDonateTimes = 0;
            hcDonateTimes = 0;
            goldDonateTimes = 0;
            TotalTimes = 0;
        }

        public object Clone()
        {
            return new LegionDonateInfo();
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public void OnUpdate(object obj)
        {
            InitChestList();
            int preDonatimes = TotalTimes;
            luxuryDonateTimes = EB.Dot.Integer("today_luxury_donated_times", obj, luxuryDonateTimes);
            hcDonateTimes = EB.Dot.Integer("today_hc_donated_times", obj, hcDonateTimes);
            goldDonateTimes = EB.Dot.Integer("today_gold_donated_times", obj, goldDonateTimes);
            TotalTimes = luxuryDonateTimes + hcDonateTimes + goldDonateTimes;
            if (preDonatimes != TotalTimes)
            {
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnLegionDonateTimesChaged);
                LegionLogic.GetInstance().HaveDonateRP();
                LegionLogic.GetInstance().IsHaveCouldLevelupTechSkill();
            }
            today_hire_times = EB.Dot.Integer("today_hire_times", obj, goldDonateTimes);
            get_hire_reward = EB.Dot.Bool("get_hire_reward", obj, get_hire_reward);
            chestdata = EB.Dot.Object("donate_chest", obj, null);
            ParseToDonateChest(chestdata);
        }

        private void InitChestList()
        {
            if (!isChestInit)
            {
                var data = Data.AllianceTemplateManager.Instance.mDonateChestList;
                for (int i = 0; i < data.Count; i++)
                {
                    var tempchest = new DonateChestInfo(data[i].id, data[i].score);
                    chsetinfolist.Add(tempchest);
                }
                isChestInit = true;
            }
        }

        private void RefreshChestList()
        {
            for (int i = 0; i < chsetinfolist.Count; i++)
            {
                var chestdata = chsetinfolist[i];
                if (recievedChestDic.TryGetValue(chestdata.id, out int chestlevel))
                {
                    if (chestdata.hasRecieve != chestLevel)
                    {
                        chestdata.SetRecieveState(chestLevel);
                    }
                }
            }
        }

        private void ParseToDonateChest(Hashtable data)
        {
            if (data != null)
            {
                int perLevel = chestLevel;
                chestLevel = 0;
                recievedChestDic.Clear();
                foreach (DictionaryEntry ad in data)
                {
                    int skillId = EB.Dot.Integer("", ad.Key, 0);
                    int level = EB.Dot.Integer("", ad.Value, 0);
                    if (skillId != 0)
                    {
                        recievedChestDic.Add(skillId, level);
                        chestLevel += level;
                    }
                }
                if (perLevel != chestLevel)
                {
                    RefreshChestList();
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnLegionDonateChestInfoChange);
                    LegionLogic.GetInstance().IshaveCouldReciveDonateChest();
                }
            }
        }

        public void ClearDonateChestState()
        {
            DataLookupsCache.Instance.SearchDataByID("alliance.todayDonateTimes.donate_chest", out Hashtable data);
            if (data == null)
            {
                chestLevel = 0;
                recievedChestDic.Clear();
                RefreshChestList();
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnLegionDonateChestInfoChange);
                LegionLogic.GetInstance().IshaveCouldReciveDonateChest();
            }
        }

    }


    public class PlayerFormation : INodeData
    {
        public List<PlayerFormationData> playerForData
        {
            set; get;
        }

        public void CleanUp()
        {
            playerForData.Clear();
        }

        public object Clone()
        {
            return new PlayerFormation();
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public void OnUpdate(object obj)
        {
            playerForData = Hotfix_LT.EBCore.Dot.List<PlayerFormationData, long>(string.Empty, obj, playerForData, ParseFormationData);
        }

        public PlayerFormation()
        {
            playerForData = new List<PlayerFormationData>();
        }

        private PlayerFormationData ParseFormationData(object value, long uid)
        {
            if (value == null)
            {
                return null;
            }

            PlayerFormationData formationData = new PlayerFormationData();
            formationData.templateId = EB.Dot.Integer("templateId", value, formationData.templateId);
            formationData.star = EB.Dot.Integer("star", value, formationData.star);
            formationData.level = EB.Dot.Integer("level", value, formationData.level);
            formationData.awakeLevel = EB.Dot.Integer("awakeLevel", value, formationData.awakeLevel);
            formationData.charType = EB.Dot.Integer("charType", value, formationData.charType);
            formationData.isAwaken = EB.Dot.Integer("stat.awaken.level", value, formationData.isAwaken);
            formationData.skin = EB.Dot.Integer("skin", value, formationData.skin);
            return formationData;
        }
    }
    public class AllianceAttriAddtion
    {
        public float FengAtk;
        public float FengDef;
        public float FengMhp;
        public float ShuiAtk;
        public float ShuiDef;
        public float ShuiMhp;
        public float HuoAtk;
        public float HuoDef;
        public float HuoMhp;
        public void Clear()
        {
            FengAtk = 0f;
            FengDef = 0f;
            FengMhp = 0f;
            ShuiAtk = 0f;
            ShuiDef = 0f;
            ShuiMhp = 0f;
            HuoAtk = 0f;
            HuoDef = 0f;
            HuoMhp = 0f;
        }

        public AllianceAttriAddtion()
        {
            Clear();
        }
        public void GetAttrAddtionByCharType(Data.eRoleAttr type, out float Atk, out float Def, out float Mhp)
        {
            Atk = 0;
            Def = 0;
            Mhp = 0;
            switch (type)
            {
                case Data.eRoleAttr.None:
                    return;
                case Data.eRoleAttr.Feng:
                    Atk = FengAtk;
                    Def = FengDef;
                    Mhp = FengMhp;
                    return;
                case Data.eRoleAttr.Shui:
                    Atk = ShuiAtk;
                    Def = ShuiDef;
                    Mhp = ShuiMhp;
                    return;
                case Data.eRoleAttr.Huo:
                    Atk = HuoAtk;
                    Def = HuoDef;
                    Mhp = HuoMhp;
                    return;
                default:
                    return;
            }
        }
    }
}
