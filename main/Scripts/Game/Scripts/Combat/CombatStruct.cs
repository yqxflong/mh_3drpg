using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Hotfix_LT.UI;
using ILRuntime.Runtime;
using Debug = EB.Debug;

namespace Hotfix_LT.Combat
{
    public enum eSpecialBuffState
    {
        None = 0,
        Frozen = 1,
        Stone = 2
    }

    public class CombatantIndex
    {
        public int TeamIndex { get; set; }
        public int IndexOnTeam { get; set; }  //不是九宫格站位标号，而是实际排序

        public CombatantIndex()
        {
            TeamIndex = -1;
            IndexOnTeam = -1;
        }

        public CombatantIndex(int team_index, int index_on_team)
        {
            TeamIndex = team_index;
            IndexOnTeam = index_on_team;
        }

        public int EnemyTeamIndex
        {
            get { return 1 - TeamIndex; }
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            if (obj is CombatantIndex == false)
            {
                return false;
            }

            CombatantIndex cmp_idx = obj as CombatantIndex;

            return (TeamIndex == cmp_idx.TeamIndex) && (IndexOnTeam == cmp_idx.IndexOnTeam);
        }

        public bool Equals(CombatantIndex cmp_idx)
        {
            if (cmp_idx == null)
            {
                return false;
            }

            if (cmp_idx == this)
            {
                return true;
            }

            return (TeamIndex == cmp_idx.TeamIndex) && (IndexOnTeam == cmp_idx.IndexOnTeam);
        }

        public override int GetHashCode()
        {
            return TeamIndex << 16 | IndexOnTeam;
        }

        public static CombatantIndex Parse(Hashtable info)
        {
            int team_index = EB.Dot.Integer("team", info, -1);
            int index_on_team = EB.Dot.Integer("combatant", info, -1);
            if (team_index >= 0 || index_on_team >= 0)
            {
                return new CombatantIndex(team_index, index_on_team);
            }

            return null;
        }

        public static CombatantIndex Parse(IDictionary info)
        {
            if (!info.Contains("team") || !info.Contains("combatant"))
            {
                return null;
            }

            int team_index = int.Parse(info["team"].ToString());
            int index_on_team = int.Parse(info["combatant"].ToString());
            return new CombatantIndex(team_index, index_on_team);
        }

        public int CompareTo(CombatantIndex other)
        {
            if (other == null)
            {
                return -1;
            }

            if (Equals(other))
            {
                return 0;
            }

            if (TeamIndex != other.TeamIndex)
            {
                return TeamIndex - other.TeamIndex;
            }

            return IndexOnTeam - other.IndexOnTeam;
        }

        public override string ToString()
        {
            //return base.ToString() + "{team: " + TeamIndex.ToString() + ", indexInTeam: " + IndexOnTeam.ToString() + "}";
            return "CombatantIndex" + "{team: " + TeamIndex.ToString() + ", indexInTeam: " + IndexOnTeam.ToString() + "}";
        }

        public Hashtable ToHash()
        {
            Hashtable ht = Johny.HashtablePool.Claim();
            ht["team"] = TeamIndex;
            ht["combatant"] = IndexOnTeam;
            return ht;
        }
    }

    public class TeamIndex
    {
        public int Team
        {
            get;
            set;
        }

        public int Batch
        {
            get;
            set;
        }

        public TeamIndex()
        {
            Team = -1;
            Batch = -1;
        }

        public TeamIndex(int team_index, int batch_index)
        {
            Team = team_index;
            Batch = batch_index;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            if (obj is TeamIndex == false)
            {
                return false;
            }

            TeamIndex cmp_idx = obj as TeamIndex;
            return (Team == cmp_idx.Team) && (Batch == cmp_idx.Batch);
        }

        public bool Equals(TeamIndex cmp_idx)
        {
            if (cmp_idx == null)
            {
                return false;
            }

            if (cmp_idx == this)
            {
                return true;
            }

            return (Team == cmp_idx.Team) && (Batch == cmp_idx.Batch);
        }

        public override int GetHashCode()
        {
            return Team << 16 | Batch;
        }

        public static TeamIndex Parse(Hashtable info)
        {
            int team_index = EB.Dot.Integer("team", info, -1);
            int batch_index = EB.Dot.Integer("batch", info, -1);
            if (team_index >= 0 && batch_index >= 0)
            {
                return new TeamIndex(team_index, batch_index);
            }

            return null;
        }

        public static TeamIndex Parse(IDictionary info)
        {
            if (!info.Contains("team") || !info.Contains("batch"))
            {
                return null;
            }

            int team_index = int.Parse(info["team"].ToString());
            int batch_index = int.Parse(info["batch"].ToString());
            return new TeamIndex(team_index, batch_index);
        }
    }

    class TeamIndexComparer : IEqualityComparer<TeamIndex>
    {
        public bool Equals(TeamIndex x, TeamIndex y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(TeamIndex obj)
        {
            return obj.GetHashCode();
        }
    }

    class CombatantIndexComparer : IEqualityComparer<CombatantIndex>
    {
        public bool Equals(CombatantIndex x, CombatantIndex y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(CombatantIndex obj)
        {
            return obj.GetHashCode();
        }

        public static CombatantIndexComparer Default = new CombatantIndexComparer();
    }

    public class TeamData
    {
        public TeamIndex Index { get; set; }
        public string Layout { get; set; }
        public int LeaderIndex { get; set; }
        public string TeamId { get; set; }
        public List<CombatantData> Team { get; set; }
        public CombatantData LeaderData
        {
            get { return Team[LeaderIndex]; }
        }
        public int SPoint { get; set; }
    }

    public class CombatantData
    {
        public CombatantIndex Index { get; set; }
        public string Model { get; set; }
        public string Portrait { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        string _position;
        public string Position
        {
            get
            {
                return (Index.IndexOnTeam + 1).ToString();
            }
            set { _position = value; }
        }
        public bool IsPlayer { get; set; }
        public bool IsPlayerTroop { get; set; }
        public bool IsEnemy { get; set; }
        public bool IsEnemyTroop { get; set; }
        public bool IsPlayerMirror { get; set; }
        public bool IsPlayerTroopMirror { get; set; }
        public bool Threaten { get; set; }
        public long PlayerId { get; set; }
        public int TroopId { get; set; }
        public int EnemyId { get; set; }
        public int TplId { get; set; }
        public int CharacterId { get; set; }
        public Dictionary<string, string> Equipments { get; set; }
        private int mRow = -1;
        private int mCol = -1;
        private int GetRow(string position)
        {
            if (position.StartsWith("f"))
            {
                return 0;
            }

            if (position.StartsWith("m"))
            {
                return 1;
            }

            if (position.StartsWith("b"))
            {
                return 2;
            }

            return -1;
        }
        private int GetCol(string position)
        {
            if (position.EndsWith("1"))
            {
                return 0;
            }

            if (position.EndsWith("2"))
            {
                return 1;
            }

            if (position.EndsWith("3"))
            {
                return 2;
            }

            return -1;
        }

        public int Row
        {
            get { return mRow = mRow == -1 ? GetRow(_position) : mRow; }
        }

        public int Col
        {
            get { return mCol = mCol == -1 ? GetCol(_position) : mCol; }
        }

        public CombatantData()
        {
            Equipments = new Dictionary<string, string>();
        }
    }

    public class CombatantAttributes
    {
        public enum eAttribute
        {
            MaxHp,
            Hp,
            MaxMp,
            Mp,
            Speed,
            Shield,
            Miss,
            Critical,
            CriticalFactor,
            PhysicAttack,
            MagicAttack,
            PhysicDefend,
            MagicDefend,
            Penetration,
            SpellPenetration,
            DamageReduction,
            Stun,
            HpRecover,
            CanRevive,
            CanUseManualSkill,
            CanNormalAttack,
            CanUseComboSkill,
            Slow,
            FreezeHitReaction,
            CriticalFactorAdd,
        }

        public class AttributeComparer : IEqualityComparer<eAttribute>
        {
            public bool Equals(eAttribute x, eAttribute y)
            {
                return x == y;
            }

            public int GetHashCode(eAttribute obj)
            {
                return (int)obj;
            }

            public static AttributeComparer Default = new AttributeComparer();
        }

        private Dictionary<eAttribute, long> m_intAttrs = new Dictionary<eAttribute, long>(AttributeComparer.Default);
        private Dictionary<eAttribute, bool> m_boolAttrs = new Dictionary<eAttribute, bool>(AttributeComparer.Default);

        private static int AttributeCount = 0;

        private static int GetAttributeCount()
        {
            if (AttributeCount == 0)
            {
                AttributeCount = System.Enum.GetValues(typeof(eAttribute)).Length;
            }

            return AttributeCount;
        }

        public Dictionary<eAttribute, long> IntAttrs
        {
            get { return m_intAttrs; }
        }

        public void Clear()
        {
            m_intAttrs.Clear();
            m_boolAttrs.Clear();
        }

        public void SetIntAttr(eAttribute attr, long value)
        {
            m_intAttrs[attr] = value;
        }

        public long GetIntAttr(eAttribute attr)
        {
            if (!HasIntAttr(attr))
            {
                return 0;
            }

            return m_intAttrs[attr];
        }

        public bool HasIntAttr(eAttribute attr)
        {
            return m_intAttrs.ContainsKey(attr);
        }

        public void SetBoolAttr(eAttribute attr, bool value)
        {
            m_boolAttrs[attr] = value;
        }

        public bool IsInit()
        {
            return m_intAttrs.Count + m_boolAttrs.Count == GetAttributeCount();
        }
    }

    #region newcombat struct

    public enum eCombatSyncAction { None, FirstCombat, WaitAction, Finish, Start, HonorStart }

    abstract public class CombatSyncEventBase
    {
        int subEid;
        public int SubEid { get { return subEid; } }

        protected CombatSyncEventBase()
        {
        }
        protected CombatSyncEventBase(object eventData)
        {
            subEid = EB.Dot.Integer("eid", eventData, 0);
        }
        public abstract void DealEvent();
        public abstract string GenerateLog();

        public static CombatSyncEventBase BuildEvent(object eventData)
        {
            string eventTypeStr = EB.Dot.String("type", eventData, string.Empty);
            switch (eventTypeStr)
            {
                case "wd":
                    return new CombatWaveSyncData(eventData);
                case "td":
                    return new CombatTeamSyncData(eventData);
                case "cd":
                    return new CombatCharacterSyncData(eventData);
                case "md":
                    return new CombatMoveSlotData(eventData);
                case "sd":
                    return new CombatSkillSyncData(eventData);
                case "dmg":
                    return new CombatDamageSyncData(eventData);
                case "buff":
                    return new CombatBuffSyncData(eventData);
                case "sync":
                    return new CombatSyncActionData(eventData);
                case "ss":
                    return new CombatSkillStartEvent(eventData);
                case "time":
                    return new CombatCountdownEvent(eventData);
                case "death":
                    return new CombatDeathEventData(eventData);
                case "flash":
                    return new CombatFlashEvent(eventData);
                case "summon":
                    return new CombatSummonEvent(eventData);
                case "remove":
                    return new CombatRemoveEvent(eventData);
                case "err":
                    {
                        EB.Debug.LogError("{0},----------CombatStruct CombatSyncEventBase BuildEvent Error---------", EB.Debug.ACCIDENTAL);
                        string msgStr = EB.Dot.String("msg", eventData, "");
                        string msgDetail = EB.Dot.String("detail", eventData, "");
                        CombatInfoData.GetInstance().LogError(msgStr, msgDetail);

                        long loginID = (long)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LoginManager", "Instance", "GetLoginID");
                        PlayerPrefs.SetInt(loginID.ToString() + "CombatAuto", 1);
                        Hotfix_LT.UI.LTCombatEventReceiver.Instance.Ready = false;
                        GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "ClientCombatReadyFromCombatStruct", Hotfix_LT.Combat.CombatSyncData.Instance.CombatId);
                        //throw new Exception();
                        break;
                    }
                case "log":
                    {
                        string msgStr = EB.Dot.String("msg", eventData, "");
                        string msgDetail = EB.Dot.String("detail", eventData, "");
                        CombatInfoData.GetInstance().LogString(msgStr + msgDetail + "\n");
                    }
                    break;
                default:
                    {
                        EB.Debug.LogError("parse eventType fail type={0}",eventTypeStr);
                    }
                    break;
            }
            return null;
        }

    protected static void ParseBuffData(object eventData, CombatCharacterSyncData characterData)
    {
        ArrayList buffInfos = EB.Dot.Array("buffs", eventData, null);
        if (buffInfos == null)
            return;

        if (buffInfos.Count > 0)
        {
            foreach (var buffInfo in buffInfos)
            {
                int guid = EB.Dot.Integer("guid", buffInfo, 0);
                int id = EB.Dot.Integer("id", buffInfo, -1);
                if (guid <= 0)
                {
                    EB.Debug.LogError("buff guid <=0 characterData={0}", characterData.ToString());
                    continue;
                }
                CombatCharacterSyncData.BuffData newBuffData = new CombatCharacterSyncData.BuffData();
                characterData.BuffDatas.Add(guid, newBuffData);
                ArrayList leftTurns = EB.Dot.Array("leftturn", buffInfo, null);
                if (leftTurns == null)
                {
                    EB.Debug.LogError("leftTurns == null");
                    return;
                }

                int[] leftTurnArray = new int[leftTurns.Count];
                int maxLeftTurn = 0;
                for (int idx = 0; idx < leftTurns.Count; ++idx)
                {
                    leftTurnArray[idx] = int.Parse(leftTurns[idx].ToString());
                    if (leftTurnArray[idx] == -1)
                    {
                        maxLeftTurn = -1;
                    }
                }
                if(id != -1) newBuffData.Id = id;
                newBuffData.LeftTurnArray = leftTurnArray;
                newBuffData.MaxTurn = maxLeftTurn;
                newBuffData.Overlying = leftTurnArray.Length;
                if (maxLeftTurn != -1 && leftTurnArray.Length > 0)
                    newBuffData.MaxTurn = leftTurnArray.Max();
            }
        }
    }

        protected static eCombatSyncAction ParseActionType(string actionStr)
        {
            switch (actionStr)
            {
                case "FirstCombat":
                    return eCombatSyncAction.FirstCombat;
                case "WaitAction":
                    return eCombatSyncAction.WaitAction;
                case "Finish":
                    return eCombatSyncAction.Finish;
                case "Start":
                    return eCombatSyncAction.Start;
                case "HonorStart":
                    return eCombatSyncAction.HonorStart;
                default:
                    EB.Debug.LogError("ParseActionType Err actionStr={0}" , actionStr);
                    return eCombatSyncAction.None;
            }
        }

        protected static string GetCharNameByTplID(int TplId)
        {
            string characterName = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.CharacterTemplateManager", "Instance", "GetCharacterName", TplId);
            return ColoredString(characterName, Color.green);
        }

        protected static string GetSkillName(int id)
        {
            string skillName = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.SkillTemplateManager", "Instance", "GetSkillName", id);
            return ColoredString(skillName, Color.green);
        }

        protected static string GetBuffName(int id)
        {
            string skillName = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.BuffTemplateManager", "Instance", "GetBufferName", id);
            return ColoredString(skillName, Color.green);
        }

        protected static string GetCharNameByIngameID(int id)
        {
            var data = CombatSyncData.Instance.GetCharacterData(id);
            return data != null ? GetCharNameByTplID(data.TplId) : ColoredString("UnknownCharName", Color.green);
        }

        protected static string ColoredString(string str, Color c)
        {
            return string.Format("<color={1}>{0}</color>", str, GameUtils.ColorToWebColor(c));
        }
    }

    public class CombatWaveSyncData : CombatSyncEventBase
    {
        public int Team;

        public CombatWaveSyncData(object eventData) : base(eventData)
        {
            Team = EB.Dot.Integer("team", eventData, 0);

            ArrayList waveData = EB.Dot.Array("data", eventData, null);

            if (waveData != null)
            {
                for (int index = 0; index < waveData.Count; ++index)
                {
                    chars.Add(new CombatCharacterSyncData(waveData[index]));
                }
            }
        }

        public List<CombatCharacterSyncData> chars = new List<CombatCharacterSyncData>();


        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                string ret = string.Format("队伍{0}出生({1}方队伍)\n", Team, CombatLogic.Instance.IsPlayerOrChallengerSide(Team) ? "我" : "敌");

                int len = chars.Count;
                for (int i = 0; i < len; i++)
                {
                    ret += chars[i].GenerateLog();
                }
                return ret;
            }
            else
            {
                return "DISABLE_LOG";
            }
        }

        public override void DealEvent()
        {
            var charDic = CombatSyncData.Instance.CharacterDataDic;
            var charList = CombatSyncData.Instance.TeamDataDic[Team].CharList;
            charList.Clear();
            for(int i = 0; i< chars.Count; i++)
            {
                var c = chars[i];
                charDic[c.IngameId] = c;
                charList.Add(c);
                c.PrepareCharData(i);
            }

            if (Team == CombatLogic.Instance.LocalPlayerTeamIndex)
            {
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "RefreshAutoButton");
            }
            Hotfix_LT.UI.LTCombatEventReceiver.Instance.StartLoadNewWave(Team);
        }
    }

    public class CombatTeamSyncData : CombatSyncEventBase
    {
        #region From GameUtils
        public class LTShowItemData
        {
            public string type;
            public string id;
            public int count;
            public bool coloring;
            public float multiple;

            public LTShowItemData(string id, int count, string type, bool coloring = false, float multiple = 1)
            {
                this.id = id;
                this.count = count;
                this.type = type;
                this.coloring = coloring;
                this.multiple = multiple;
            }

            public LTShowItemData(object obj)
            {
                type = EB.Dot.String("type", obj, string.Empty);
                id = EB.Dot.String("data", obj, string.Empty);
                count = EB.Dot.Integer("quantity", obj, 0);
                coloring = false;
            }
        }

        static public List<LTShowItemData> ParseAwardArr(ArrayList arr)
        {
            if (arr == null)
            {
                return new List<LTShowItemData>();
            }

            List<LTShowItemData> items = new List<LTShowItemData>();
            for (var i = 0; i < arr.Count; i++)
            {
                var a = arr[i];
                string type = EB.Dot.String("type", a, string.Empty);
                if (string.IsNullOrEmpty(type))
                    type = EB.Dot.String("t", a, string.Empty);

                string id = EB.Dot.String("data", a, string.Empty);
                if (string.IsNullOrEmpty(id))
                    id = EB.Dot.String("n", a, string.Empty);

                int count = EB.Dot.Integer("quantity", a, 0);
                if (count <= 0)
                    count = EB.Dot.Integer("q", a, 0);
                items.Add(new LTShowItemData(id, count, type, false));
            }

            List<LTShowItemData> itemsnew = new List<LTShowItemData>();
            for (var i = 0; i < items.Count; i++)
            {
                MergeAdd(itemsnew, items[i]);
            }
            return itemsnew;
        }

        private static bool MergeAdd(List<LTShowItemData> itemsnew, LTShowItemData itemData)
        {
            for (var i = 0; i < itemsnew.Count; i++)
            {
                var v = itemsnew[i];

                if (v.id == itemData.id)
                {
                    v.count += itemData.count;
                    return true;
                }
            }
            itemsnew.Add(itemData);
            return false;
        }
        #endregion

        public int TeamId;
        public int CurWave;
        public int MaxWave;
        public int ComboCD;
        public int SPoint;
        /// <summary>
        /// 创成总的伤害值（军团副本使用）
        /// </summary>
        public long Hurt;
        /// <summary>
        /// 受到的伤害值
        /// </summary>
        public long Hurted;
        public List<LTShowItemData> ScrollList;
        public List<CombatCharacterSyncData> CharList = new List<CombatCharacterSyncData>();

        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                CombatTeamSyncData olddata;
                if (CombatSyncData.Instance.TeamDataDic.TryGetValue(TeamId, out olddata))
                {
                    string ret = string.Format("{0}队更新: \n", TeamId);

                    if (CurWave != -1) ret += string.Format("当前波次{0} -> {1}\n", olddata.CurWave, CurWave);
                    if (MaxWave != -1) ret += string.Format("最大波次{0} -> {1}\n", olddata.MaxWave, MaxWave);
                    if (SPoint != -1) ret += string.Format("魔力点数{0} -> {1}\n", olddata.CurWave, CurWave);

                    int len = ScrollList.Count;
                    for (int i = 0; i < len; i++)
                    {
                        var s = ScrollList[i];
                        var oldscroll = olddata.ScrollList.Find(p => p.id == s.id);
                        if (oldscroll != null)
                        {
                            ret += string.Format("卷轴({0}) {1} -> {2}\n", GetSkillName(int.Parse(s.id)), oldscroll.count,
                                s.count);
                        }
                    }

                    return ret;
                }
                else
                {
                    string ret = string.Format("{0}队初始化: 波次: {1}/{2}, 法力: {3}\n卷轴: ", TeamId, CurWave, MaxWave, SPoint);
                    int len = ScrollList.Count;
                    for (int i = 0; i < len; i++)
                    {
                        var s = ScrollList[i];
                        ret += string.Format("{0} x {1}, ", GetSkillName(int.Parse(s.id)), s.count);
                    }

                    ret += '\n';
                    return ret;
                }
            }
            else
            {
                return "DISABLE_LOG";
            }
        }

        public CombatTeamSyncData(object eventData) : base(eventData)
        {
            TeamId = EB.Dot.Integer("teamId", eventData, 0);
            CurWave = EB.Dot.Integer("curWave", eventData, -1);
            MaxWave = EB.Dot.Integer("maxWave", eventData, -1);
            ComboCD = EB.Dot.Integer("chainCD", eventData, -1);
            SPoint = EB.Dot.Integer("spoint", eventData, -1);
            Hurt = EB.Dot.Long("hurt", eventData, 0);
            Hurted = EB.Dot.Long("hurted", eventData, 0);
            ScrollList = ParseAwardArr(EB.Dot.Array("scrolls", eventData, null));
        }

        public void UpdatedData(CombatTeamSyncData update)
        {
            if (update.CurWave != -1) CurWave = update.CurWave;
            if (update.MaxWave != -1) MaxWave = update.MaxWave;
            if (update.ComboCD != -1) ComboCD = update.ComboCD;
            if (update.SPoint != -1) SPoint = update.SPoint;

            int len = update.ScrollList.Count;
            for (int i = 0; i < len; i++)
            {
                var s = update.ScrollList[i];
                var localscroll = ScrollList.Find(p => p.id == s.id);
                if (localscroll != null)
                {
                    localscroll.count = s.count;
                    if (s.count == 0)
                    {
                        ScrollList.Remove(localscroll);
                    }
                }
                else
                {
                    ScrollList.Add(s);
                }
            }
        }

        public override void DealEvent()
        {
            CombatTeamSyncData data;
            if (CombatSyncData.Instance.TeamDataDic.TryGetValue(TeamId, out data))
            {
                data.UpdatedData(this);
            }
            else
            {
                CombatSyncData.Instance.TeamDataDic.Add(TeamId, this);
                data = this;
            }
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "UpdateTeamInfo", data);

            //判断当前是否为对方受到的伤害
            if (CombatLogic.Instance.IsOpponentSide(data.TeamId))
            {
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "ResetCurrentHurt", data.Hurted);
            }
        }
    }

    public class CombatMoveSlotData : CombatSyncEventBase
    {
        public int[] movequeue;

        public CombatMoveSlotData(object eventData) : base(eventData)
        {
            movequeue = EB.Dot.Array("moveQueue", eventData, null, delegate (object val) { return int.Parse(val.ToString()); });
            if (movequeue.Length < 3)
            {
                EB.Debug.LogWarning("MoveQueue.Length < 3");
            }
        }

        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                string ret = "行动序列变更: ";

                int len = movequeue.Length;
                for (int i = 0; i < len; i++)
                {
                    var id = movequeue[i];
                    ret += string.Format("-> {0}({1})", id, GetCharNameByIngameID(id));
                }
                ret += "\n";
                return ret;
            }
            else
            {
                return "DISABLE_LOG";
            }
        }


        public override void DealEvent()
        {
            //Hotfix_LT.UI.LTCombatHudController.Instance.UpdateActionQueue(movequeue);
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "UpdateActionQueue", movequeue);
        }
    }

    public class CombatCharacterSyncData : CombatSyncEventBase
    {
        public class SkillData
        {
            public int ID;
            public int Level;
            public int CD;
            public int Index;
            private int skillType = -1;
            public int SkillType   //TODOX 待优化了
            {
                get
                {
                    if (skillType == -1)
                    {
                        skillType = (int)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.SkillTemplateManager", "Instance", "GetSkillType", ID);
                    }

                    return skillType;
                }
            }
            private string icon = string.Empty;
            public string Icon
            {
                get
                {
                    if (string.IsNullOrEmpty(icon))
                    {
                        icon = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.SkillTemplateManager", "Instance", "GetSkillIcon", ID);
                    }

                    return icon;
                }
            }

            private int maxCooldown = -1;
            public int MaxCooldown
            {
                get
                {
                    if (maxCooldown == -1)
                    {
                        maxCooldown = (int)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.SkillTemplateManager", "Instance", "GetMaxCooldown", ID);
                    }

                    return maxCooldown;
                }
            }


            public string TypeName
            {
                get
                {
                    if (SkillType == 0)
                        return EB.Localizer.GetString("ID_STORE_NAME_COMMON");
                    else if (SkillType == 2)
                        return EB.Localizer.GetString("ID_PASSIVE");
                    else if (SkillType == 1)
                        return EB.Localizer.GetString("ID_AOYI");
                    else if (SkillType == 5)
                        return EB.Localizer.GetString("ID_SCROLL");
                    else
                        EB.Debug.LogError("SkillType Error for type={0}", SkillType);
                    return "";
                }
            }
        }

        public class BuffData
        {
            public int Id;
            public int[] LeftTurnArray;
            public int MaxTurn;
            public int Overlying;
            public bool hasPlayedNotice;

            public string GetMaxTurnStr()
            {
                if (MaxTurn == -1)
                    return "";
                return MaxTurn.ToString();
            }

            public string GetOverlying(int SackNum)
            {
                if (Overlying <=0 || SackNum <= 1)
                {
                    return "";
                }

                return Overlying.ToString();
            }

        }

        public long Uid;//用于组队时判断
        public int ID;
        public int TplId;
        public int Skin;
        public int TeamId;
        public int IngameId;
        public string Model;
        public string Portrait;
        //public Hotfix_LT.Data.eRoleAttr Attr = Hotfix_LT.Data.eRoleAttr.None;
        public int Attr = -1;
        public Dictionary<string, string> Equipments;

        public long Hp;
        public bool Dead = false;
        public long MaxHp;
        public float MoveBarValue
        {
            get { return MoveSlot / MaxMoveSlot; }
        }
        public float MoveSlot;
        public float MaxMoveSlot;
        public int NextSkillID;
        public SortedDictionary<int, BuffData> BuffDatas = new SortedDictionary<int, BuffData>();
        public ArrayList Limits;
        public int ScrollTimes;
        public Dictionary<int, SkillData> SkillDataList = new Dictionary<int, SkillData>();
        public SkillData NormalSkillData;
        public List<SkillData> SpecialSkillDataList = new List<SkillData>();
        public SkillData OpponentSpecialSkill;

        public CombatantIndex Index;
        public int TeamIndex { get { return Index.TeamIndex; } }
        public int IndexOnTeam { get { return Index.IndexOnTeam; } }

        public bool IsBoss
        {
            get
            {
                return (bool)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.CharacterTemplateManager", "Instance", "IsBoss", TplId);
            }
        }

        public bool IsSkillCanUse(SkillData skilldata)
        {
            if (skilldata.SkillType == 0 && !Limits.Contains("disableAttack"))
            {
                return true;
            }
            else if (skilldata.SkillType == 1 && !Limits.Contains("disableCast") && skilldata.CD <= 0)
            {
                return true;
            }
            return false;
        }

        public int GetCanUseSkill(bool defaultNormal)
        {
            if (defaultNormal && IsSkillCanUse(NormalSkillData))
            {
                return NormalSkillData.ID;
            }
            else
            {
                int len = SpecialSkillDataList.Count;
                for (int i = 0; i < len; i++)
                {
                    var specialSkillData = SpecialSkillDataList[i];
                    if (IsSkillCanUse(specialSkillData))
                    {
                        return specialSkillData.ID;
                    }
                }
            }
            return 0;
        }

        public CombatCharacterSyncData() : base()
        {
        }

        public CombatCharacterSyncData(object eventData) : base(eventData)
        {
            Uid = EB.Dot.Long("uid", eventData, 0);
            TeamId = EB.Dot.Integer("team", eventData, 0);
            IngameId = EB.Dot.Integer("ingameid", eventData, 0);
            ID = EB.Dot.Integer("charid", eventData, -1);
            Skin = EB.Dot.Integer("skin", eventData, 0);
            TplId = EB.Dot.Integer("roleid", eventData, -1);
            MoveSlot = EB.Dot.Single("moveSlot", eventData, float.NaN);
            MaxMoveSlot = EB.Dot.Single("maxMoveSlot", eventData, float.NaN);
            Limits = EB.Dot.Array("limits", eventData, null);
            ScrollTimes = EB.Dot.Integer("scrollTimes", eventData, CombatSyncData.Instance.CurScrollTimes);
            CombatSyncData.Instance.CurScrollTimes = ScrollTimes;

            float hp = EB.Dot.Single("HP", eventData, float.NaN);
            if (float.IsNaN(hp))
            {
                Hp = -1;
            }
            else
            {
                Hp = (long)hp;
                if (Hp < 0) Hp = 0;
            }
            float mhp = EB.Dot.Single("MaxHP", eventData, float.NaN);
            if (float.IsNaN(mhp))
            {
                MaxHp = -1;
            }
            else
            {
                MaxHp = (long)mhp;
            }

            ArrayList skillObjs = EB.Dot.Array("skills", eventData, null);

            if (skillObjs != null)
            {
                foreach (var skillObj in skillObjs)
                {
                    CombatCharacterSyncData.SkillData skillData = new CombatCharacterSyncData.SkillData();
                    skillData.ID = EB.Dot.Integer("id", skillObj, -1);
                    skillData.CD = EB.Dot.Integer("cd", skillObj, -1);
                    skillData.Level = EB.Dot.Integer("lv", skillObj, -1);
                    skillData.Index = EB.Dot.Integer("index", skillObj, -1);
                    if (skillData.ID == -1 || skillData.CD == -1)
                    {
                        EB.Debug.LogError("invalid skill info, ID or CD must exist");
                    }
                    else
                    {
                        SkillDataList[skillData.ID] = skillData;
                    }
                }
            }

            NextSkillID = EB.Dot.Integer("nextSkill", eventData, -1);
            ParseBuffData(eventData, this);
        }

        public override string GenerateLog()
        {
            if (!ILRDefine.ENABLE_LOGGING){
                return "DISABLE_LOG";
            }

            CombatCharacterSyncData oldData;
            if (CombatSyncData.Instance.CharacterDataDic.TryGetValue(IngameId, out oldData))
            {
                string ret = string.Format("P{0}({1})更新:\n", oldData.IngameId, GetCharNameByTplID(oldData.TplId));

                if (!float.IsNaN(MoveSlot)) ret += string.Format("行动条: {0} -> {1}\n", oldData.MoveSlot, MoveSlot);
                if (!float.IsNaN(MaxMoveSlot)) ret += string.Format("最大行动条: {0} -> {1}\n", oldData.MaxMoveSlot, MaxMoveSlot);

                if (ID != -1) ret += string.Format("ID: {0} -> {1}\n", oldData.ID, ID);
                if (TplId != -1) ret += string.Format("TplId: {0} -> {1}\n", oldData.TplId, TplId);
                if (Limits != null) ret += string.Format("Limits: {0} -> {1}\n", LitJson.JsonMapper.ToJson(oldData.Limits), LitJson.JsonMapper.ToJson(Limits));
                if (Hp != -1) ret += string.Format("Hp: {0} -> {1}\n", oldData.Hp, Hp);
                if (MaxHp != -1) ret += string.Format("MaxHp: {0} -> {1}\n", oldData.MaxHp, MaxHp);
                if (NextSkillID != -1) ret += string.Format("NextSkillID: {0} -> {1}({2})\n", oldData.NextSkillID, NextSkillID, GetSkillName(NextSkillID));

                if (SkillDataList.Count > 0)
                {
                    ret += "技能CD更新:\n";

                    var enumerator = SkillDataList.Values.GetEnumerator();
                    while(enumerator.MoveNext())
                    {
                        var skill = enumerator.Current;
                        var oldCD = oldData.SkillDataList.ContainsKey(skill.ID) ? oldData.SkillDataList[skill.ID].CD : 0;
                        ret += string.Format("{0} - {1}({2}) - CD: {3} -> {4}\n", skill.Index, skill.ID, GetSkillName(skill.ID), oldCD, skill.CD);
                    }
                }

                if (BuffDatas.Count > 0)
                {
                    ret += "BUFF更新:\n";

                    var enumurator = BuffDatas.GetEnumerator();
                    while(enumurator.MoveNext())
                    {
                        var buff = enumurator.Current;
                        var oldtime = oldData.BuffDatas.ContainsKey(buff.Key) ? oldData.BuffDatas[buff.Key].LeftTurnArray : new int[1];
                        ret += string.Format("{0} - {1}({2}) - 时间: {3} -> {4}\n", buff.Key, buff.Value.Id, GetBuffName(buff.Value.Id), LitJson.JsonMapper.ToJson(oldtime), LitJson.JsonMapper.ToJson(buff.Value.LeftTurnArray));
                    }
                }

                return ret;
            }
            else
            {
                string ret = string.Format("P{0}({1}-{2})初始化:\n", IngameId, TplId, GetCharNameByTplID(TplId));
                ret += string.Format("HP: {0}/{1} 行动条: {2}/{3}\n", Hp, MaxHp, MoveSlot, MaxMoveSlot);
                if (Limits != null) ret += string.Format("Limits: {0}\n", LitJson.JsonMapper.ToJson(Limits));
                if (NextSkillID != -1) ret += string.Format("NextSkillID: {0}({1})\n", NextSkillID, GetSkillName(NextSkillID));

                ret += "技能列表:\n";

                var enumerator = SkillDataList.Values.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var skill = enumerator.Current;
                    ret += string.Format("{0} - {1}({2}) - CD: {3}\n", skill.Index, skill.ID, GetSkillName(skill.ID), skill.CD);
                }

                if (BuffDatas.Count > 0)
                {
                    ret += "BUFF列表:\n";
                    var enumurator = BuffDatas.GetEnumerator();
                    while (enumurator.MoveNext())
                    {
                        var buff = enumurator.Current;
                        ret += string.Format("{0} - {1}({2}) - 时间: {3}\n", buff.Key, buff.Value.Id, GetBuffName(buff.Value.Id), LitJson.JsonMapper.ToJson(buff.Value.LeftTurnArray));
                    }
                }

                return ret;
            }
        }

        public void PrepareCharData(int index_in_team)
        {
            Hashtable heroInfo = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.CharacterTemplateManager", "Instance", "GetHeroInfoEx", ID, Skin);
            if (heroInfo == null)
            {
                EB.Debug.LogWarning("newCharacterData:heroInfo = null id={0}", ID);
                return;
            }
            Model = heroInfo["model_name"] != null ? heroInfo["model_name"].ToString() : string.Empty;//需添加皮肤
            Attr = heroInfo["char_type"] != null ? heroInfo["char_type"].ToInt32() : 0;
            Portrait = heroInfo["icon"] != null ? heroInfo["icon"].ToString() : string.Empty;

            SpecialSkillDataList.Clear();
            var it = SkillDataList.GetEnumerator();
            while (it.MoveNext())
            {
                var tp = it.Current.Value.SkillType;
                switch (tp)
                {
                    case 0:
                        NormalSkillData = it.Current.Value;
                        break;
                    case 1:
                        SpecialSkillDataList.Add(it.Current.Value);
                        break;
                }
            }

            CombatantIndex ci = new CombatantIndex();
            ci.TeamIndex = TeamId;
            ci.IndexOnTeam = index_in_team;
            Index = ci;
        }

        void UpdateCharData(CombatCharacterSyncData other)
        {
            if (!float.IsNaN(other.MoveSlot)) MoveSlot = other.MoveSlot;
            if (!float.IsNaN(other.MaxMoveSlot)) MaxMoveSlot = other.MaxMoveSlot;

            if (other.ID != -1) ID = other.ID;
            if (other.TplId != -1) TplId = other.TplId;
            if (other.Limits != null) Limits = other.Limits;
            if (other.ScrollTimes != -1) ScrollTimes = other.ScrollTimes;
            if (other.Hp != -1) Hp = other.Hp;
            if (other.MaxHp != -1) MaxHp = other.MaxHp;
            if (other.NextSkillID != -1) NextSkillID = other.NextSkillID;

            var enumerator0 = other.SkillDataList.GetEnumerator();
            while(enumerator0.MoveNext())
            {
                var skill = enumerator0.Current;
                SkillData localskill;// = SkillDataList[skill.Key];
                if (SkillDataList.TryGetValue(skill.Key, out localskill))
                {
                    if (skill.Value.CD != -1) localskill.CD = skill.Value.CD;
                    if (skill.Value.Level != -1) localskill.Level = skill.Value.Level;
                    if (skill.Value.Index != -1) localskill.Index = skill.Value.Index;
                }
                else
                {
                    SkillDataList.Add(skill.Key, skill.Value);
                }
            }

            var enumerator = other.BuffDatas.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var buff = enumerator.Current;
                BuffData localbuff;
                if (BuffDatas.TryGetValue(buff.Key, out localbuff))
                {
                    //buff turn列表暂时没有增量更新
                    if (buff.Value.LeftTurnArray.All(p => p == 0))
                    {
                        BuffDatas.Remove(buff.Key);
                    }
                    else
                    {
                        localbuff.LeftTurnArray = buff.Value.LeftTurnArray;
                    }
                    localbuff.MaxTurn = buff.Value.MaxTurn;
                    localbuff.Overlying = buff.Value.Overlying;
                }
                else
                {
                    BuffDatas.Add(buff.Key, buff.Value);
                }
            }

            if (IsBoss)
            {
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "SetNextSkill", NextSkillID);
            }
        }

        public override void DealEvent()
        {
            CombatCharacterSyncData data;
            if (CombatSyncData.Instance.CharacterDataDic.TryGetValue(IngameId, out data))
            {
                data.UpdateCharData(this);

                //if (Hotfix_LT.UI.LTCombatHudController.Instance != null)
                //{
                //    Hotfix_LT.UI.LTCombatHudController.Instance.CombatSkillCtrl.UpdateSkillList(data);
                //}
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "UpdateSkillList", data);
            }
            else
            {
                EB.Debug.LogError("HandleEventQueue:Character: Need wave data to init");
                return;
            }

            Combatant combatant = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(data.Index);
            if (combatant == null)
            {
                EB.Debug.LogWarning("HandleEventQueue:Character: combatant = null");
                return;
            }
            combatant.Data = data;

            //long startHP = combatant.HealthBar.Hp;
            //if (startHP != data.Hp)
            //{
            //    combatant.HealthBar.Hp = data.Hp;
            //    combatant.HealthBar.MaxHp = data.MaxHp;

            //    if (combatant.Data.IsBoss)
            //    {
            //        Hotfix_LT.UI.LTCombatHudController.Instance.BossHealthBarCtrl.UpdateHp(data.Hp);
            //    }
            //}
            combatant.HealthBar.OnHandleMessage("OnHpChange", data);

            if (combatant.IsAlive())
            {
                combatant.UpdateMoveBar(data.MoveBarValue);
                combatant.UpdateBuff(data.BuffDatas.Values);
            }
        }

        public override string ToString()
        {
            //base.ToString() 在IL卡死报错
            //return string.Format("{0}[TeamId:{1},ActionId:{2},Model:{3}]", base.ToString(), TeamId.ToString(), IngameId.ToString(), Model);
            return string.Format("{0}[TeamId:{1},ActionId:{2},Model:{3}]", "CombatCharacterSyncData", TeamId.ToString(), IngameId.ToString(), Model);
        }

        public class CombatCharacterComparer : IComparer<CombatCharacterSyncData>
        {
            public int Compare(CombatCharacterSyncData x, CombatCharacterSyncData y)
            {
                return x.IngameId - y.IngameId;
            }
        }

        public static CombatCharacterComparer Default = new CombatCharacterComparer();
    }

    public class CombatSkillSyncData : CombatSyncEventBase
    {
        public int SkillID;
        public bool IsCombo;
        public int Source;
        public int[] Target;

    public CombatSkillSyncData(object eventData) : base(eventData)
    {
        SkillID = EB.Dot.Integer("skillid", eventData, 0);
        Source = EB.Dot.Integer("source", eventData, 0);
        ArrayList tars = EB.Dot.Array("target", eventData, null);

        Target = new int[tars.Count];
        for (int idx = 0; idx < tars.Count; ++idx)
        {
            Target[idx] = int.Parse(tars[idx].ToString());
        }
    }

        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                string ret = string.Format("P{0}({1}) 对 [ ", Source, GetCharNameByIngameID(Source));

                int len = Target.Length;
                for (int i = 0; i < len; i++)
                {
                    var t = Target[i];
                    ret += string.Format("P{0}({1}, ", t, GetCharNameByIngameID(t));
                }
                ret += string.Format(" ] 施放了技能{0}({1})\n", SkillID, GetSkillName(SkillID));
                return ret;
            }
            else
            {
                return "DISABLE_LOG";
            }
        }
        public override void DealEvent()
        {
            CombatSyncData.Instance.ProcessingSkill = false;
            Hotfix_LT.UI.LTCombatEventReceiver.Instance.PlaySkill(this);
        }

        public void LogSkillStart()
        {
            if(ILRDefine.ENABLE_LOGGING){
                CombatInfoData.GetInstance().LogString(string.Format("{0}: skill anim start: {1}, source = {2}, target = {3}\n"
                    , SubEid, SkillID, Source, LitJson.JsonMapper.ToJson(Target)));
            }
        }
    }

    public abstract class CombatHitEventBase : CombatSyncEventBase
    {
        public int Order;
        public bool IsDirect;
        public int Target;
        bool _noTarget;
        public CombatHitEventBase(object eventData, bool NoTarget = false) : base(eventData)
        {
            Order = EB.Dot.Integer("order", eventData, -1);
            Target = EB.Dot.Integer("target", eventData, -1);
            _noTarget = NoTarget;
            if (!_noTarget && Target == -1)
                EB.Debug.LogError("buff target=-1");
        }

        public void OnHit()
        {
            if (_noTarget)
            {
                OnHit(null);
                return;
            }
            var target = CombatSyncData.Instance.GetCharacterData(Target);
            if (target == null) return;
            Combatant target_combatant = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(target.Index);
            if (target_combatant == null) return;
            if (this is CombatDeathEventData)
            {
                if (this.Target == CombatSyncData.Instance.CurActionId)
                {
                    if (ILRDefine.ENABLE_LOGGING)
                    {
                        CombatInfoData.GetInstance().LogString("将CombatDeathEventData.OnHit(target)滞后处理，防止放技能中途卡死情况\n");
                    }
                    CombatSyncData.Instance.OverDeathEvenAction += delegate { OnHit(target_combatant); };
                    return;
                }
            }
            OnHit(target_combatant);
        }
        public abstract void OnHit(Combatant target);
        public override void DealEvent()
        {
            if (CombatSyncData.Instance.ProcessingSkill)
            {
                Queue<CombatHitEventBase> dmgqueue;
                CombatSyncData.Instance.DamageDatas.TryGetValue(this.Target, out dmgqueue);
                if (dmgqueue == null)
                {
                    dmgqueue = new Queue<CombatHitEventBase>();
                    CombatSyncData.Instance.DamageDatas.Add(this.Target, dmgqueue);
                }
                dmgqueue.Enqueue(this);
            }
            else
            {
                OnHit();
                if (this.IsDirect) EB.Debug.LogError("direct damage outside skill process");
            }
        }
    }

    public class CombatDamageSyncData : CombatHitEventBase
    {
        public int Damage;
        public int Absorb;
        public bool IsCrit;
        public int Source;

        public CombatDamageSyncData(object eventData) : base(eventData)
        {
            float buffDamage = EB.Dot.Single("damage", eventData, 0);
            Damage = -Mathf.RoundToInt(buffDamage);
            float absorbDamage = EB.Dot.Single("absorb", eventData, 0);
            Absorb = Mathf.RoundToInt(absorbDamage);
            IsCrit = EB.Dot.Bool("iscrit", eventData, false);
            Source = EB.Dot.Integer("source", eventData, 0);
            IsDirect = EB.Dot.Bool("isDirect", eventData, false);
        }

        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                string ret = string.Format("{0}({1}) 对 {2}({3}) 造成了 {4}点{5}伤害(order:{6},{7})",
                    Source, GetCharNameByIngameID(Source), Target, GetCharNameByIngameID(Target), Damage, IsCrit ? "暴击" : "", Order, IsDirect ? "Direct" : "NonDirect");
                if (Absorb > 0)
                {
                    ret += string.Format("({0}点伤害被吸收)", Absorb);
                }
                ret += '\n';
                return ret;
            }
            else
            {
                return "DISABLE_LOG";
            }
        }
        public override void OnHit(Combatant target)
        {
            target.ApplyDamageData(this);
        }
    }

    public class CombatBuffSyncData : CombatSyncEventBase
    {
        public int ID;
        public int Source;
        public int Target;

        public CombatBuffSyncData(object eventData) : base(eventData)
        {
            ID = EB.Dot.Integer("buffid", eventData, 0);
            Source = EB.Dot.Integer("source", eventData, 0);
            Target = EB.Dot.Integer("target", eventData, -1);
            if (Target == -1)
                EB.Debug.LogError("buff target=-1");
        }

        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                return string.Format("P{0}({1}) 对 P{2}({3}) 添加了BUFF {4}({5})\n",
                    Source, GetCharNameByIngameID(Source), Target, GetCharNameByIngameID(Target), ID, GetBuffName(ID));
            }
            else
            {
                return "DISABLE_LOG";
            }
        }
        public override void DealEvent()
        {
            Combatant combatant = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatantByIngameId(Target);
            combatant.BuffFloatFont(this);
            if (!CombatSyncData.Instance.ProcessingSkill)
            {
                combatant.ShowBuffFloatFont();
            }
        }
    }

    public class CombatDeathEventData : CombatHitEventBase
    {
        public CombatDeathEventData(object eventData) : base(eventData)
        {
            IsDirect = false;
        }
        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                return string.Format("P{0}({1}) 死了\n",
                    Target, GetCharNameByIngameID(Target));
            }
            else
            {
                return "DISABLE_LOG";
            }
        }
        public override void OnHit(Combatant target)
        {
            if (target != null && target.Data != null)
            {
                if (!target.Data.Dead)
                {
                    target.Data.Dead = true;
                    target.CallDeath();
                }
            }
        }
    }

    public class CombatSyncActionData : CombatSyncEventBase
    {
        public class CombatTeamInfo
        {
            public int teamIndex;
            public string teamName;
            public int worldId;
            public int teamWin;
        }

        public eCombatSyncAction ActionType;
        public int Actor;
        public int ActionCount;
        public long Uid;
        private int Round;
        private List<CombatTeamInfo> TeamInfo = new List<CombatTeamInfo>();

        public CombatSyncActionData(object eventData) : base(eventData)
        {
            string actionStr = EB.Dot.String("state", eventData, "");
            ActionType = ParseActionType(actionStr);
            if (ActionType== eCombatSyncAction.Start)
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "SetStartEvent", eventData);
            }
            Actor = EB.Dot.Integer("actor", eventData, -1);
            ActionCount = EB.Dot.Integer("actionCount", eventData, 0);
            Uid = EB.Dot.Long("uid", eventData, 0);
            Round = EB.Dot.Integer("round", eventData, 0);

            ArrayList teamInfo = EB.Dot.Array("teamInfo", eventData, null);
            if (teamInfo != null)
            {
                foreach (var team in teamInfo)
                {
                    int index = EB.Dot.Integer("teamIndex", team, 0);
                    string name = EB.Dot.String("teamName", team, "");
                    int worldId = EB.Dot.Integer("worldId", team, 0);
                    int win = EB.Dot.Integer("teamWin", team, 0);

                    TeamInfo.Add(new CombatTeamInfo() { teamIndex = index, teamName = name, worldId = worldId, teamWin = win });
                }
            }
        }

        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                switch (ActionType)
                {
                    case eCombatSyncAction.WaitAction:
                        return ColoredString(string.Format("轮到P{0}({1})行动\n", Actor, GetCharNameByIngameID(Actor)), Color.red);
                    case eCombatSyncAction.FirstCombat:
                        return ColoredString("开场大战剧情开始\n", Color.red);
                    case eCombatSyncAction.Finish:
                        return ColoredString("战斗结束\n", Color.red);
                    case eCombatSyncAction.Start:
                        return ColoredString("战斗开始\n", Color.red);
                    default:
                        return ColoredString("错误的Action类型:" + ActionType + "\n", Color.red);
                }
            }
            else
            {
                return "DISABLE_LOG";
            }
        }
        public override void DealEvent()
        {
            switch (ActionType)
            {
                case eCombatSyncAction.WaitAction:
                    {
                        if (!CombatSyncData.Instance.CharacterDataDic.ContainsKey(Actor))
                        {
                            EB.Debug.LogError("CharacterDataDic Not ContainsKey for:{0}" , Actor.ToString());
                            return;
                        }
                        Hashtable combatData = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "GetBasicInfo");
                        bool testBool = (bool)combatData["testBool"];
                        int actorNum = combatData["actorNum"].ToInt32();
                        bool autoMode = (bool)combatData["autoMode"];

                        long loginId = (long)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LoginManager", "Instance", "GetLoginID");

                        var actor = CombatSyncData.Instance.CharacterDataDic[testBool ? actorNum : Actor];
                        if (Uid == loginId && actor != null && CombatLogic.Instance.IsPlayerOrChallengerSide(actor.TeamId))
                        {
                            CombatSyncData.Instance.NeedSetSkill = true;

                            if (autoMode)
                            {
                                Hotfix_LT.UI.LTCombatEventReceiver.Instance.AutoSkillSelect(actor);
                            }
                            else
                            {
                                Hotfix_LT.UI.LTCombatEventReceiver.Instance.ShowSkillPanel(actor);
                            }
                        }
                    }
                    break;
                case eCombatSyncAction.FirstCombat:
                    {
                        Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnStartBattle(true);
                    }
                    break;
                case eCombatSyncAction.Finish:
                    {
                        Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnFinishBattle();
                    }
                    break;
                case eCombatSyncAction.Start:  //涉及后台断线重连逻辑下发消息，荣耀竞技场时过滤
                    {
                        Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnCombatStart();
                    }
                    break;
                case eCombatSyncAction.HonorStart:
                    {
                        Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnHonorCombatStart(Round, TeamInfo);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class CombatSkillStartEvent : CombatSyncEventBase
    {
        int actor;
        public CombatSkillStartEvent(object eventData) : base(eventData)
        {
            actor = EB.Dot.Integer("actor", eventData, -1);
        }

        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                return string.Format("P{0}({1})技能过程开始\n", actor, GetCharNameByIngameID(actor));
            }
            else
            {
                return "DISABLE_LOG";
            }
        }
        public override void DealEvent()
        {
            if (CombatSyncData.Instance.DamageDatas.Count > 0)
            {
                EB.Debug.LogError( "{0}: damage or buff data not clear when skill start dmgcount = {1}", SubEid.ToString() , CombatSyncData.Instance.DamageDatas.Count.ToString());
                CombatSyncData.Instance.DamageDatas.Clear();
            }
            CombatSyncData.Instance.ProcessingSkill = true;
            CombatSyncData.Instance.CurActionId = actor;
        }
    }

    public class CombatCountdownEvent : CombatSyncEventBase
    {
        int actor;
        int endtime;

        public CombatCountdownEvent(object eventData) : base(eventData)
        {
            actor = EB.Dot.Integer("actor", eventData, -1);
            endtime = EB.Dot.Integer("endtime", eventData, -1);
        }
        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                return string.Format("倒计时更新: {0}({1}): 到期时间: {2}", actor, GetCharNameByIngameID(actor), EB.Time.FromPosixTime(endtime).ToShortTimeString());

            }
            else
            {
                return "DISABLE_LOG";

            }
        }
        
        public override void DealEvent()
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "LaunchActionCountdown", actor, endtime);
        }
    }

    public class CombatResumeEvent : CombatSyncEventBase
    {
        int EID;
        int SubEID;

        public CombatResumeEvent(int eid, int subeid)
        {
            EID = eid;
            SubEID = subeid;
        }
        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                return string.Format("战斗重连 在 {0}, {1}\n", EID, SubEID);
            }
            else
            {
                return "DISABLE_LOG";
            }
        }
        public override void DealEvent()
        {
            CombatSyncData.Instance.CleanUp();
            CombatSyncData.Instance.CombatResume(EID, SubEID);
        }
    }

    public class CombatFlashEvent : CombatSyncEventBase
    {
        int target;
        string key;
        public CombatFlashEvent(object eventData) : base(eventData)
        {
            target = EB.Dot.Integer("target", eventData, -1);
            key = EB.Dot.String("key", eventData, "");
        }
        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                return string.Format("CombatFlashEvent at {0}, {1}\n", target, key);
            }
            else
            {
                return "DISABLE_LOG";
            }
        }
        public override void DealEvent()
        {
            Combatant combatant = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatantByIngameId(target);
            if (combatant != null)
            {
                combatant.FlashEventFont(key);
                if (!CombatSyncData.Instance.ProcessingSkill)
                {
                    combatant.ShowBuffFloatFont();
                }
            }
        }
    }

    public class CombatSummonEvent : CombatHitEventBase
    {
        CombatCharacterSyncData data;
        int pos;
        public CombatSummonEvent(object eventData) : base(eventData, true)
        {
            data = new CombatCharacterSyncData(EB.Dot.Object("data", eventData, null));
            pos = EB.Dot.Integer("pos", eventData, -1);
        }

        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                return string.Format("召唤: {0}\n", data.GenerateLog());
            }
            else
            {
                return "DISABLE_LOG";
            }
        }

        public override void OnHit(Combatant target)
        {
            if (CombatSyncData.Instance.CharacterDataDic.ContainsKey(data.IngameId))
            {
                EB.Debug.LogError("repeat ingame id: {0}" , data.IngameId.ToString());
                return;
            }
            CombatSyncData.Instance.CharacterDataDic[data.IngameId] = data;
            CombatSyncData.Instance.TeamDataDic[data.TeamId].CharList.Add(data);
            data.PrepareCharData(pos);

            var list = new List<CombatCharacterSyncData>();
            list.Add(data);
            Hotfix_LT.UI.LTCombatEventReceiver.Instance.SummonCharacters(list);
        }
    }
    public class CombatRemoveEvent : CombatSyncEventBase
    {
        int target;
        public CombatRemoveEvent(object eventData) : base(eventData)
        {
            target = EB.Dot.Integer("target", eventData, -1);
        }
        public override string GenerateLog()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                return string.Format("Unsummoning at {0}\n", target);
            }
            else
            {
                return "DISABLE_LOG";
            }
        }

        public override void DealEvent()
        {
            Hotfix_LT.UI.LTCombatEventReceiver.Instance.RemoveCombatant(target);
        }
    }

    public class CombatSyncData
    {
        private static CombatSyncData s_instance;
        public static CombatSyncData Instance
        {
            get { return s_instance = s_instance ?? new CombatSyncData(); }
        }

        public int CombatId { get { return CombatLogic.Instance.CombatId; } }
        private int currentEid = 1;
        public int getCurrentSubEid() { return currentSubEid; }
        private int currentSubEid = 1;
        public int WaitEIDTimes { get; private set; }
        public int Turn { get; private set; }
        public int ActionId { get; private set; }
        public Dictionary<int, CombatTeamSyncData> TeamDataDic { get; private set; }
        public Dictionary<int, CombatCharacterSyncData> CharacterDataDic { get; private set; }
        public EB.Collections.Queue<CombatSyncEventBase> EventQueue { get; private set; }
        public Dictionary<int, Queue<CombatHitEventBase>> DamageDatas = new Dictionary<int, Queue<CombatHitEventBase>>();
        public bool CharacterDataLoadOK { get; private set; }
        public bool ProcessingSkill = false;
        /// <summary>记录当前施放技能者的id</summary>
        public int CurActionId = -1;
        public int CurScrollTimes = 0;
        public bool NeedSetSkill;
        public IDictionary<int, Hashtable> PendingEvents = new SortedDictionary<int, Hashtable>();
        public bool isResume;
        public bool isResumePvp;
        public Dictionary<int, int[]> MoveQueueDic;
        Dictionary<int, List<CombatCharacterSyncData.BuffData>> BuffDataMirror;

        public CombatCharacterSyncData WaitCharacterData;
        /// <summary>
        /// 技能过程中死亡时事件调整至技能结束后再处理
        /// </summary>
        public Action OverDeathEvenAction = null;


        CombatSyncData()
        {
            TeamDataDic = new Dictionary<int, CombatTeamSyncData>();
            CharacterDataDic = new Dictionary<int, CombatCharacterSyncData>();
            EventQueue = new EB.Collections.Queue<CombatSyncEventBase>();
            MoveQueueDic = new Dictionary<int, int[]>();
            BuffDataMirror = new Dictionary<int, List<CombatCharacterSyncData.BuffData>>();
        }
        public void CleanUp()
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "ClearPingTime");
            isResume = false;
            isResumePvp = false;
            Turn = 0;
            ActionId = 0;
            currentEid = 1;
            currentSubEid = 1;
            CharacterDataLoadOK = false;
            NeedSetSkill = false;
            WaitCharacterData = null;
            PendingEvents.Clear();
            TeamDataDic.Clear();
            CharacterDataDic.Clear();
            MoveQueueDic.Clear();
            DamageDatas.Clear();
            BuffDataMirror.Clear();
            CurActionId = -1;
        }

        /// <summary>
        /// 战斗重连改变Eid
        /// </summary>
        /// <param name="eid"></param>
        public void CombatResume(int eid, int subEid)
        {
            currentEid = eid + 1;
            currentSubEid = subEid + 1;
            isResume = true;
            isResumePvp = true;
        }

        public bool ParseAll()
        {
            while (PendingEvents.Count > 0)
            {
                var first = PendingEvents.First();

                if (first.Key < currentEid)
                {
                    PendingEvents.Remove(first.Key);
                }
                else if (first.Key == currentEid)
                {
                    Parse(first.Value, false);
                    currentEid++;
                    PendingEvents.Remove(first.Key);
                }
                else
                {
                    WaitEIDTimes++;

                    if (WaitEIDTimes < 50) //超时大于50次 设计上因当问服务器重新获取 现在暂时调整为放过并提示 避免卡死
                    {
                        return false;
                    }
                    else
                    {
                        WaitEIDTimes = 0;
                        GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "PingImmediately");
                        return true;
                    }
                }
            }

            if (LTCombatEventReceiver.Instance != null)
            {
                LTCombatEventReceiver.Instance.HandleEventQueue();
            }
            return true;
        }

        public void Parse(Hashtable data, bool directDeal)
        {
            string[] ints_names = {"combat.actorId", "combat.turnId"};
            int[] ints_dv = {ActionId, Turn};
            int[] ints = EB.Dot.Integers(data, ints_names, ints_dv);
            ActionId = ints[0];
            Turn = ints[1];

            var events = EB.Dot.Array("combat.events", data, null);
            if (events != null)
            {
                int count =  events.Count;
                for(int i = 0; i < count; i++)
                {
                    var eventData = events[i];
                    var e = CombatSyncEventBase.BuildEvent(eventData);
                    if (e == null) continue;
                    if (directDeal)
                    {
                        AddQueue(e);
                        if (Hotfix_LT.UI.LTCombatEventReceiver.Instance != null)
                        {
                            Hotfix_LT.UI.LTCombatEventReceiver.Instance.HandleEventQueue();
                        }
                    }
                    else if (e.SubEid < currentSubEid)
                    {
                        continue;
                    }
                    else
                    {
                        AddQueue(e);
                        currentSubEid++;
                    }
                }
            }
            else
            {
                EB.Debug.LogError("combat events == null ");
            }

            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "UpdateUI");
        }

        public void AddQueue(CombatSyncEventBase eventBase)
        {
            EventQueue.Add(eventBase);
        }

        public CombatCharacterSyncData GetCharacterData(int actor_id)
        {
            CombatCharacterSyncData ret = null;
            CharacterDataDic.TryGetValue(actor_id, out ret);
            return ret;
        }

        public CombatantIndex GetCombatantIndex(int actor_id)
        {
            var ret = GetCharacterData(actor_id);
            if (ret != null) return ret.Index;
            else return null;
        }

        public int GetTeamCount(int teamId)
        {
            if (TeamDataDic != null && TeamDataDic.ContainsKey(teamId) && TeamDataDic[teamId].CharList != null)
            {
                return TeamDataDic[teamId].CharList.Count;
            }

            return 0;
        }

        public CombatTeamSyncData GetMyTeamData()
        {
            try
            {
                return TeamDataDic[CombatLogic.Instance.LocalPlayerTeamIndex];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<CombatCharacterSyncData> GetCharacterList(int teamID)
        {
            try
            {
                if (TeamDataDic != null && TeamDataDic.ContainsKey(teamID))
                {
                    return TeamDataDic[teamID].CharList;
                }

                return new List<CombatCharacterSyncData>(0);
            }
            catch (Exception e)
            {
                string keys = "";

                var enumerator = TeamDataDic.Keys.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    keys += enumerator.Current;
                    keys += ", ";
                }
                EB.Debug.LogError("try to get invalid teamId: {0} when TeamData has {1}", teamID, keys);
                throw e;
            }
        }

        public List<CombatCharacterSyncData> GetAliveCharacterList()
        {
            List<CombatCharacterSyncData> list = new List<CombatCharacterSyncData>();
            var it = CharacterDataDic.GetEnumerator();
            while(it.MoveNext())
            {
                if (it.Current.Value.Hp > 0)
                    list.Add(it.Current.Value);
            }
            return list;
        }

        public bool GetDiedCharacterList()
        {
            bool hasDied = false;
            List<CombatCharacterSyncData> list = new List<CombatCharacterSyncData>();
            var it = CharacterDataDic.GetEnumerator();
            while(it.MoveNext())
            {
                if (it.Current.Value.Hp <= 0 && CombatLogic.Instance != null 
                    && it.Current.Value.TeamId == CombatLogic.Instance.LocalPlayerTeamIndex)
                {
                    hasDied = true;
                    break;
                }
            }
            return hasDied;
        }

        public bool GetIsExistSpecialBuffState(CombatCharacterSyncData characterData, eSpecialBuffState buffState)
        {
            var enumerator = characterData.BuffDatas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var buffdata = enumerator.Current;
                int state = (int)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.BuffTemplateManager", "Instance", "GetSpecialState", buffdata.Value.Id);

                if (buffState == eSpecialBuffState.Frozen && state == (int)eSpecialBuffState.Frozen)
                {
                    return true;
                }
                else if (buffState == eSpecialBuffState.Stone && state == (int)eSpecialBuffState.Stone)
                {
                    return true;
                }
            }
            return false;
        }

        public eSpecialBuffState GetCurrentBuffState(CombatCharacterSyncData characterData)
        {
            int maxPriorityState = 0;
            var enumerator = characterData.BuffDatas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var buffdata = enumerator.Current;

                int SpecialState = (int)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.BuffTemplateManager", "Instance", "GetBufferState", buffdata.Value.Id);

                if (SpecialState > maxPriorityState)
                    maxPriorityState = SpecialState;
            }
            return (eSpecialBuffState)maxPriorityState;
        }
    }

    #endregion
}