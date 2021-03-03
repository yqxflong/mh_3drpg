using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using _HotfixScripts.Utils;
using Hotfix_LT.Combat;
using ILRuntime.Runtime;
using EB.Director.Runtime;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 战斗事件接收器
    /// </summary>
    public class LTCombatEventReceiver : MonoBehaviour
    {
        static private LTCombatEventReceiver s_instance;
        /// <summary>
        /// 自行判空
        /// </summary>
        /// <value></value>
        static public LTCombatEventReceiver Instance { get { return s_instance; } }

        public void Awake()
        {
            s_instance = this;
            SparxHub.Instance.GetManager<EB.Sparx.PushManager>().OnConnected += OnPushConnected;
            LT.MainMessenger.AddListenerEx<int, int, int>("GetIngameId", OnGetIngameId);
            LT.MainMessenger.AddListenerEx<int, int, bool>("IsAlive", IsAlive);
            LT.MainMessenger.AddListenerEx<int, int, string>("GetmyName", GetmyName);
            LT.MainMessenger.AddListenerEx<int, int, int>("GetID", OnGetId);
            LT.MainMessenger.AddListenerEx<bool>("IsCombatInit", CombatInit);
            LT.MainMessenger.AddListenerEx<bool>("IsBattleOver", CombatOver);
        }

        public void OnDestroy()
        {
            TimerManager.instance.RemoveTimerSafely(ref _StartCombat_Seq);
        }

        public void OnExitCombat()
        {
            if (LTCombatEventReceiver.Instance != null)
            {
                LTCombatEventReceiver.Instance.ExitCombat();
            }
        }
        public void CancelBtnClick()
        {
            for (int i = 0; i < m_teams.Length; ++i)
            {
                for (int j = 0; j < m_combatants[i].Length; ++j)
                {
                    var combatant = m_combatants[i][j];
                    if (combatant != null)
                    {
                        combatant.PreExit();
                    }
                }
            }
        }
        private int OnGetIngameId(int teamIndex, int index)
        {
            Hotfix_LT.Combat.Combatant combatant = GetCombatant(teamIndex, index);
            if (combatant != null)
            {
                return combatant.Data.IngameId;
            }

            return -1;
        }

        private bool IsAlive(int teamIndex, int index)
        {
            Hotfix_LT.Combat.Combatant combatant = GetCombatant(teamIndex, index);
            if (combatant != null)
            {
                return combatant.IsAlive();
            }

            return false;
        }
        private string GetmyName(int teamIndex, int index)
        {
            Hotfix_LT.Combat.Combatant combatant = GetCombatant(teamIndex, index);
            if (combatant != null)
            {
                return combatant.myName;
            }

            return string.Empty;
        }
        private int OnGetId(int teamIndex, int index)
        {
            Hotfix_LT.Combat.Combatant combatant = GetCombatant(teamIndex, index);
            if (combatant != null)
            {
                return combatant.Data.ID;
            }

            return -1;
        }
        private bool CombatInit()
        {
            return IsCombatInit();
        }
        private bool CombatOver()
        {
            return this.IsBattleOver;
        }

        void OnPushConnected()
        {
            if (Ready)
            {
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "PingImmediately");
            }
        }

        public void OnDisable()
        {
            s_instance = null;
            m_combatInit = false;
            m_pendingEvents.Clear();
            SparxHub.Instance.GetManager<EB.Sparx.PushManager>().OnConnected -= OnPushConnected;
            LT.MainMessenger.RemoveListenerEx<int, int, int>("GetIngameId", OnGetIngameId);
            LT.MainMessenger.RemoveListenerEx<int, int, bool>("IsAlive", IsAlive);
            LT.MainMessenger.RemoveListenerEx<int, int, string>("GetmyName", GetmyName);
            LT.MainMessenger.RemoveListenerEx<int, int, int>("GetID", OnGetId);
            LT.MainMessenger.RemoveListenerEx<bool>("IsCombatInit", CombatInit);
            LT.MainMessenger.RemoveListenerEx<bool>("IsBattleOver", CombatOver);
        }

        public void Update()
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "Ping");
        }

        private bool m_combatInit = false;
        private bool[] m_teamReady = new bool[2] { false, false };
        private bool m_firstPlaybackStartAudioEventTriggered = false;

        private GameObject[][] m_teams = null;
        private Hotfix_LT.Combat.Combatant[][] m_combatants = null;
        private Dictionary<int, Hotfix_LT.Combat.Combatant> m_combatantsByIngameId = null;
        private List<object> m_pendingEvents = new List<object>();

        private Hotfix_LT.Combat.CombatEvent m_battleStateEvent = null;

        private float m_timeScale = 1.0f;
        private float m_timeScaleVelocity = 0.0f;
        private float m_timeScaleTime = 0.0f;

        private List<string> m_toRemoveResources = new List<string>();

        public int Turn
        {
            get
            {
                return Hotfix_LT.Combat.CombatSyncData.Instance.Turn;
            }
        }

        public bool TeamReady
        {
            get
            {
                for (int i = 0; i < m_teamReady.Length; i++)
                {
                    if (!m_teamReady[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void SetTeamReady(int teamID, bool value)
        {
            m_teamReady[teamID] = value;
        }

        public enum AnimState
        {
            /// <summary>待机</summary>
            Idle,
            /// <summary>处理消息</summary>
            Processing,
            LoadWave,
            /// <summary>释放技能</summary>
            CastingSkill,
            /// <summary>死亡</summary>
            DeathAnim,
            Preparing,
            /// <summary>无战斗</summary>
            NotInBattle,
            /// <summary>召唤</summary>
            Summoning,
            /// <summary>过场</summary>
            Spawning
        }

        /// <summary>
        /// 用于存放目前该标记的次数
        /// </summary>
        Dictionary<AnimState, int> _animStateFlags = new Dictionary<AnimState, int>();
        UnityEngine.Object _animStateLock = new UnityEngine.Object();
        /// <summary>
        /// 设置标记
        /// </summary>
        /// <param name="state"></param>
        public void SetAnimState(AnimState state)
        {
            //lock (_animStateLock)
            {
                int flag;
                if (_animStateFlags.TryGetValue(state, out flag))
                {
                    _animStateFlags[state] += 1;
                }
                else
                {
                    _animStateFlags[state] = 1;
                }

                if (ILRDefine.ENABLE_LOGGING)
                {
                    EB.Debug.Log(GameUtils.ColoredString("anim state change: add {0}", Color.cyan), state);
                    LogFullAnimState();
                }
            }
        }

        /// <summary>
        /// 打印当前设置的标记
        /// </summary>
        public void LogFullAnimState()
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                string ret = "CurrentState: ";
                foreach (var pair in _animStateFlags)
                {
                    ret += (pair.Key + "*" + pair.Value + " ");
                }
                EB.Debug.Log(GameUtils.ColoredString(ret, Color.cyan));
            }
        }

        /// <summary>
        /// 移除标记
        /// </summary>
        /// <param name="state"></param>
        public void UnsetAnimState(AnimState state)
        {
            int count = 0;
            if (_animStateFlags.TryGetValue(state, out count))
            {
                if (count <= 1)
                {
                    _animStateFlags.Remove(state);
                }
                else
                {
                    _animStateFlags[state]--;
                }
            }
            HandleEventQueue();
        }

        public bool IsAnimStateIdle() { return _animStateFlags.Count == 0; }
        public bool IsAnimStateProcessing() { return _animStateFlags.Count == 1 && _animStateFlags.ContainsKey(AnimState.Processing); }
        public bool IsAnimStateNotInBattle() { return _animStateFlags.ContainsKey(AnimState.NotInBattle); }
        public void ClearAnimState()
        {
            _animStateFlags.Clear();
            EB.Debug.Log(GameUtils.ColoredString("anim state cleared to Idle", Color.cyan));
        }

        public void OnBattleResultScreen()
        {
            SetAnimState(AnimState.NotInBattle);
        }

        public void OnPlayerDeathAnimStart(int ActionId)
        {
            SetAnimState(AnimState.DeathAnim);
        }

        public void OnPlayerDeathAnimEnd(int ActionId)
        {
            UnsetAnimState(AnimState.DeathAnim);
        }
        private HashSet<int> deathAnimPlayingId = new HashSet<int>();

        public bool Ready { get; set; }

        public bool IsBattleOver
        {
            get { return IsAnimStateNotInBattle(); }
        }

        public float TimeScale
        {
            get { return m_timeScale; }
            set { m_timeScale = value; }
        }

        public Hotfix_LT.Combat.CombatEvent StateEvent
        {
            get { return m_battleStateEvent; }
        }

        public void OnCombatViewLoaded()
        {
            if (_StartCombat_Seq != 0)
            {
                EB.Debug.LogError("OnCombatViewLoaded: m_startCoroutine is not null");
                TimerManager.instance.RemoveTimerSafely(ref _StartCombat_Seq);
            }

            int battleType = (int)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.SceneLogic", "GetBattleType");
            if (battleType == 21)
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTStoryController", "OpenMovieEx");
            }
            else
            {
                _StartCombat_Seq = TimerManager.instance.AddTimer(1, 0, StartCombat);     
            }
        }

        public void StoryOver(string pausedMusic)
        {
            _StartCombat_Seq = TimerManager.instance.AddTimer(1, 0, StartCombat);
            FusionAudio.ResumeMusic(pausedMusic);
        }

        #region  Coroutine -> timer
        private int _StartCombat_Seq = 0;
        private void StartCombat(int seq)
        {
            if (!CombatLogic.Instance.Ready)
            {
                return;
            }
            bool visible = (bool)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "SetupBaseUI");
            if (!visible)
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.SceneLogic", "StartCombatFromMain");
                return;
            }
            TimerManager.instance.RemoveTimerSafely(ref _StartCombat_Seq);

            //感觉没啥用，by zj 2020/8/28
            //CombatManager.Instance.RegisterListener(gameObject, false, Hotfix_LT.Combat.CombatSyncData.Instance.CombatId);
            //GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "RegisterListener", gameObject, false, CombatSyncData.Instance.CombatId);

            InitCombat();

            if (CombatLogic.Instance.LocalPlayerIsObserver)
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "ClientWatchReady", Hotfix_LT.Combat.CombatSyncData.Instance.CombatId);
            else
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "ClientCombatReadyFromLTCombatEventReceiver", Hotfix_LT.Combat.CombatSyncData.Instance.CombatId);

            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "InitCombatSkillCtrl");

            Ready = true;
            HandleEventQueue();//开始处理事件列表

            //这里和线上版本不一致，待观察 by zj 2020/8/28
            int battleType = (int)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.SceneLogic", "GetBattleType");
            if (battleType == 20 || battleType == 28)
            {
                SetupScrollSkillCaster("Common-Variant");
            }
        }
        #endregion

        public void OnCombatStart()
        {
            StartCoroutine(_OnCombatStart());
        }

        private IEnumerator _OnCombatStart()
        {
            SetAnimState(AnimState.Preparing);

            bool isEntryActionAllPlayOver = false;
            while (!isEntryActionAllPlayOver)
            {
                ForEach(combatant => isEntryActionAllPlayOver = combatant.IsEntryActionPlayOver());
                yield return null;
            }

            HandleStartConversationEvent();
            //等对话结束
            while (_conversationing)
            {
                yield return null;
            }

            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "DoReadyAction");
        }
        public void OnHonorCombatStart(int round, List<CombatSyncActionData.CombatTeamInfo> teamInfo)
        {
            StartCoroutine(_OnHonorCombatStart(round, teamInfo));
        }

        private IEnumerator _OnHonorCombatStart(int round, List<CombatSyncActionData.CombatTeamInfo> teamInfo)
        {
            SetAnimState(AnimState.Preparing);

            bool isEntryActionAllPlayOver = false;
            while (!isEntryActionAllPlayOver)
            {
                ForEach(combatant => isEntryActionAllPlayOver = combatant.IsEntryActionPlayOver());
                yield return null;
            }

            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "DoHonorReadyAction", round, teamInfo);
        }

        public void DoReadyActionOver()
        {
            MyFollowCamera.Instance?.Init();

            UnsetAnimState(AnimState.Preparing);
        }

        private void InitCombat()
        {
            m_teams = new GameObject[2][];
            m_teams[CombatLogic.Instance.ChallengerTeamIndex] = new GameObject[2 * 3];
            m_teams[CombatLogic.Instance.DefenderTeamIndex] = new GameObject[2 * 3];

            m_combatants = new Hotfix_LT.Combat.Combatant[2][];
            m_combatants[CombatLogic.Instance.ChallengerTeamIndex] = new Hotfix_LT.Combat.Combatant[2 * 3];
            m_combatants[CombatLogic.Instance.DefenderTeamIndex] = new Hotfix_LT.Combat.Combatant[2 * 3];

            m_combatantsByIngameId = new Dictionary<int, Hotfix_LT.Combat.Combatant>();

            // audio listener follow scene center
            FusionAudio.InitCombat(gameObject);

            ResetAudioEventFlags();

            m_combatInit = true;

            ClearAnimState();

            TrailRendererManager.Instance.FakeInit();//fake init here
        }

        public static bool IsCombatInit()
        {
            if (Instance == null)
            {
                return false;
            }

            return Instance.m_combatInit;
        }

        private void ResetAudioEventFlags()
        {
            m_firstPlaybackStartAudioEventTriggered = false;
        }

        public Hotfix_LT.Combat.Combatant GetCombatantByIngameId(int idx)
        {
            try
            {
                return m_combatantsByIngameId[idx];
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public Hotfix_LT.Combat.Combatant GetCombatant(Hotfix_LT.Combat.CombatantIndex idx)
        {
            if (idx == null)
                return null;

            if (m_combatants == null)
            {
                return null;
            }

            return m_combatants[idx.TeamIndex][idx.IndexOnTeam];
        }

        public Hotfix_LT.Combat.Combatant GetCombatant(int team_index, int index_on_team)
        {
            if (m_combatants == null)
            {
                return null;
            }

            if (m_combatants.Length <= team_index)
            {
                return null;
            }

            if (m_combatants[team_index] == null || m_combatants[team_index].Length <= index_on_team) //by pj
            {
                return null;
            }

            return m_combatants[team_index][index_on_team];
        }

        public int GetTeamLength(int team_index)
        {
            if (m_teams == null)
            {
                return 0;
            }

            return m_teams[team_index].Length;
        }

        public int GetTeamCount(int team_index)
        {
            if (m_teams == null)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < m_teams[team_index].Length; ++i)
            {
                count += m_teams[team_index][i] == null ? 0 : 1;
            }

            return count;
        }

        public delegate bool CombatantCallbackBool(Hotfix_LT.Combat.Combatant combatant);
        public delegate void CombatantCallbackVoid(Hotfix_LT.Combat.Combatant combatant);
        public void ForEach(CombatantCallbackBool callback)
        {
            if (callback == null || m_combatants == null)
            {
                return;
            }

            for (int i = 0; i < m_teams.Length; ++i)
            {
                for (int j = 0; j < m_combatants[i].Length; ++j)
                {
                    if (m_combatants[i][j] != null && !callback(m_combatants[i][j]))
                    {
                        return;
                    }
                }
            }
        }
        public void ForEach(CombatantCallbackVoid callback)
        {
            if (callback == null || m_combatants == null)
            {
                return;
            }

            for (int i = 0; i < m_teams.Length; ++i)
            {
                for (int j = 0; j < m_combatants[i].Length; ++j)
                {
                    if (m_combatants[i][j] != null)
                    {
                        callback(m_combatants[i][j]);
                    }
                }
            }
        }

        public int ForeachBuff()
        {
            int buff = 0;
            int type = 0;
            for (int i = 0; i < m_teams.Length; ++i)
            {
                for (int j = 0; j < m_combatants[i].Length; ++j)
                {
                    if (m_combatants[i][j] != null && m_combatants[i][j].IsAlive() && m_combatants[i][j].HavaAwakeningBuff())
                    {
                        type = m_combatants[i][j].Data.Attr;
                        buff++;
                    }
                }
            }
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "DamgeBuff", buff, type);
            return buff;
        }

        //event received
        public void OnCombatEventListReceived(object event_obj)
        {
            if (!m_combatInit)
            {
                EB.Debug.LogError("Client has not init combat data yet. Put combat event message to pending List!");
                m_pendingEvents.Add(event_obj);
                return;
            }

            // Trigger an audio event
            if (!m_firstPlaybackStartAudioEventTriggered)
            {
                m_firstPlaybackStartAudioEventTriggered = true;
            }

            // refresh launch animation
            ForEach(combatant => { if (combatant.IsLaunch()) combatant.StopLaunch(); });
        }

        public void StartLoadNewWave(int team)
        {
            StartCoroutine(LoadNewWave(team));
        }

        IEnumerator LoadNewWave(int team_index)
        {
            SetAnimState(AnimState.LoadWave);
            //判断当前状态当中是否有死亡状态，有的话就创成卡死，无法再处理战斗事件队列,所以移除死亡动作事件，by:wwh 20191118
            TryRemoveDeathAniState();

            // pre clean
            for (int i = 0; i < m_teams[team_index].Length; ++i)
            {
                if (m_teams[team_index][i] != null)
                {
                    m_combatants[team_index][i].FXHelper.StopAll(true);
                }
            }

            // waiting for a few frames
            //yield return new WaitForSeconds(0.1f);

            CleanupTeam(team_index);

            //yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(SetupTeamsCoroutine(team_index));

            bool isEntryActionAllPlayOver = false;
            while (!isEntryActionAllPlayOver)
            {
                ForEach(combatant => { if (combatant.Data.TeamId == team_index) { isEntryActionAllPlayOver = combatant.IsEntryActionPlayOver(); } });
                yield return null;
            }
            ForEach(combatant =>
            {
                if (combatant.Data.TeamId == team_index)
                {
                    combatant.OnReady();
                }
            });
            UnsetAnimState(AnimState.LoadWave);
        }


        /// <summary>
        /// 主要是给加载新波次的时候，做处理，因为有一情况，就是BUFF创成的死亡会卡死战斗
        /// </summary>
        private void TryRemoveDeathAniState()
        {
            if (_animStateFlags.ContainsKey(AnimState.DeathAnim))
            {
                EB.Debug.Log("{0},很少会出现入场的时候，有人阵亡情况，所以移除,Remove AnimState.DeathAnim", EB.Debug.ACCIDENTAL);
                _animStateFlags.Remove(AnimState.DeathAnim);
            }
        }


        /// <summary>
        /// 召唤角色
        /// </summary>
        /// <param name="chars"></param>
        public void SummonCharacters(List<Hotfix_LT.Combat.CombatCharacterSyncData> chars)
        {
            SetAnimState(AnimState.Summoning);
            StartCoroutine(_LoadCharacters(chars));
        }

        IEnumerator _LoadCharacters(List<CombatCharacterSyncData> chars)
        {
            EB.Debug.LogTs("LoadCombatCharacter Start");
            var startTime = System.DateTime.UtcNow;
            yield return StartCoroutine(LoadCharactersCoroutine(chars, "", ""));
            yield return StartCoroutine(SpawnCharactersCoroutine(chars));
            chars.ForEach((CombatCharacterSyncData data) =>
            {
                Combatant cbt = GetCombatantByIngameId(data.IngameId);
                if (cbt == null)
                {
                    EB.Debug.LogError("LoadCombatCharacter Failed, combatant not exist");
                }
                cbt.OnReady();
            });
            EB.Debug.LogTs("LoadCombatCharacter End");
            EB.Debug.Log("Team Load Time:" + (System.DateTime.UtcNow - startTime).TotalSeconds);
            UnsetAnimState(AnimState.Summoning);
        }
        IEnumerator SetupTeamsCoroutine(int teamID)
        {
            List<CombatCharacterSyncData> characterDataList = CombatSyncData.Instance.GetCharacterList(teamID);
            string specialSpawnModelName = "";
            string spawnCamera = "";
            if (!CombatSyncData.Instance.isResume) GetSpawnCamera(characterDataList, out specialSpawnModelName, out spawnCamera);
            EB.Debug.LogTs("LoadCombatCharacter Start");
            var startTime = System.DateTime.UtcNow;
            yield return StartCoroutine(LoadCharactersCoroutine(characterDataList, specialSpawnModelName, spawnCamera));
            yield return StartCoroutine(SpawnCharactersCoroutine(characterDataList));
            EB.Debug.LogTs("LoadCombatCharacter End");
            EB.Debug.Log("Team Load Time:" + (System.DateTime.UtcNow - startTime).TotalSeconds);
        }

        //是否是boss的波次
        private bool isBossWave(CombatCharacterSyncData characterData)
        {
            if (characterData.IsBoss)
            {
                return true;
            }
            return false;
        }

        #region About Skill Play
        private bool _firstBattle = false;
        private bool _conversationing = false;
        private bool _isCombo;
        private int _comboNum;
        public bool Conversationing { get { return _conversationing; } }
        int a = 0;
        /// <summary>
        /// 释放技能,业务太多，分帧处理
        /// </summary>
        /// <param name="skillData"></param>
        public void PlaySkill(Hotfix_LT.Combat.CombatSkillSyncData skillData)
        {
            StartCoroutine(DoPlaySkill(skillData));
        }

        /// <summary>
        /// 分帧处理释放技能逻辑
        /// </summary>
        /// <param name="skillData"></param>
        /// <returns></returns>
        public IEnumerator DoPlaySkill(Hotfix_LT.Combat.CombatSkillSyncData skillData)
        {
            #region Split Frame 1
            SpecialSet(skillData);
            skillData.LogSkillStart();
            SetAnimState(AnimState.CastingSkill);

            var source = Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterData(skillData.Source);
            if (source == null)
            {
                EB.Debug.LogError("PlaySkill===>source == null!!");
                yield break;
            }
            Hotfix_LT.Combat.Combatant source_cbt = GetCombatant(source.Index);
            var target = skillData.Target.Length > 0 ? Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterData(skillData.Target[0]) : null;

            Hashtable skillTemplateData = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.SkillTemplateManager", "Instance", "GetSkillInfo", skillData.SkillID);

            if (skillTemplateData["skillType"].ToInt32() == 5)
            {
                ScrollSkillCaster.Data.Index = source_cbt.Data.Index;
                source_cbt = ScrollSkillCaster;
            }

            if (source_cbt == null)
            {
                EB.Debug.LogError("StartSkill:Not Found Source Hotfix_LT.Combat.Combatant");
                UnsetAnimState(AnimState.CastingSkill);
                yield break;
            }

            Hashtable combatInfo = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "GetBasicInfo");
            if (CombatLogic.Instance.IsPlayerOrChallengerSide(source_cbt.Data.TeamId)
                && (bool)combatInfo["autoMode"]
                && !CombatLogic.Instance.LocalPlayerIsObserver)
            {
                TransformAutoActionSequence(source_cbt.Data.IngameId);
            }
            #endregion

            yield return null;

            #region Split Frame 2
            Hotfix_LT.Combat.CombatSkillEvent skillEvent = new Hotfix_LT.Combat.CombatSkillEvent();
            skillEvent.SkillId = skillData.SkillID;
            skillEvent.Target = (target != null) ? target.Index : null;
            skillEvent.IsCombo = skillData.IsCombo;
            _isCombo = skillData.IsCombo;

            source_cbt.LTTargets = skillEvent.Targets = new HashSet<Hotfix_LT.Combat.CombatantIndex>();
            for (var i = 0; i < skillData.Target.Length; i++)
            {
                int id = skillData.Target[i];
                var index = Hotfix_LT.Combat.CombatSyncData.Instance.GetCombatantIndex(id);
                skillEvent.Targets.Add(index);
            }

            if (skillEvent.Target != null)
            {
                skillEvent.Targets.Add(skillEvent.Target);
            }
            #endregion

            yield return null;

            #region Split Frame 3
            if (string.Equals(skillTemplateData["MoveName"], "Revive"))
            {
                Hotfix_LT.Combat.DeathActionState deathAction = source_cbt.GetActionState<Hotfix_LT.Combat.DeathActionState>();
                deathAction.Revive = true;
                if (source_cbt.GetMoveState() != MoveController.CombatantMoveState.kDeath)
                {
                    source_cbt.SetActionState(deathAction);
                    EB.Debug.LogWarning("Revive: moveState != MoveController.CombatantMoveState.kDeath source_cbt={0}", source_cbt);
                }
            }
            else
            {
                source_cbt.StartSkill(skillEvent);
                if (!skillData.IsCombo)
                    OnSomeOneActionEvent();
            }

            if (!skillData.IsCombo)
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "SetCurrentActionSkill", skillData.SkillID);
            #endregion
        }

        private static List<int> st_SpecialSet_tempList = new List<int>();
        /// <summary>
        /// 特殊设置
        /// </summary>
        /// <param name="skillData"></param>
        private void SpecialSet(Hotfix_LT.Combat.CombatSkillSyncData skillData)
        {
            st_SpecialSet_tempList.Clear();
            //该技能为攻击标记目标的技能，因为服务器没办法传过来具体的攻击对象，只能把全体传过来，所以客户端要做筛选
            if (skillData.SkillID == 95117)
            {
                for (int i = 0; i < skillData.Target.Length; i++)
                {
                    var tempSource = Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterData(skillData.Target[i]);
                    Hotfix_LT.Combat.Combatant cbt = GetCombatant(tempSource.Index);
                    if (cbt.IsMarkForBoss)
                    {
                        st_SpecialSet_tempList.Add(skillData.Target[i]);
                    }
                }
                skillData.Target = st_SpecialSet_tempList.ToArray();
            }
        }
        #endregion

        public void OnStartBattle(bool firstBattle)
        {
            _firstBattle = firstBattle;
        }
        public void OnFinishBattle()
        {
            if (CombatLogic.Instance.LocalPlayerIsObserver)
            {
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "ExitWatchAsk");
            }
            if (_firstBattle)
            {
                ConversationStart();
                StartFirstStory.Instance.Exit(delegate ()
                {
                    ConversationEnd();
                    GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "LastEventPlayed", Hotfix_LT.Combat.CombatSyncData.Instance.CombatId, 0);
                });
            }
            else
            {
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "LastEventPlayed", Hotfix_LT.Combat.CombatSyncData.Instance.CombatId, 0);
            }
        }

        /// <summary>
        /// 战斗事件队列处理入口
        /// </summary>
        public void HandleEventQueue()
        {
            //EB.Debug.LogError("HandleEventQueue!EventQueue.Count——{0};_animStateFlags.Count——{1}", Hotfix_LT.Combat.CombatSyncData.Instance.EventQueue.Count,_animStateFlags.Count);
            if (!Ready || !IsAnimStateIdle() || Hotfix_LT.Combat.CombatSyncData.Instance.EventQueue.Count == 0)
            {
                return;
            }

            //if (_eventQueueTimer >0)
            //{
            //    TimerManager.instance.RemoveTimerSafely(ref _eventQueueTimer);
            //}

           // EB.Debug.LogError("HandleEventQueue——Start");
            SetAnimState(AnimState.Processing);
            while (Hotfix_LT.Combat.CombatSyncData.Instance.EventQueue.Count > 0)
            {
                if (!IsAnimStateProcessing())
                {
                    break;
                }
                Hotfix_LT.Combat.CombatSyncEventBase baseData = Hotfix_LT.Combat.CombatSyncData.Instance.EventQueue.Dequeue();
                Hotfix_LT.Combat.CombatInfoData.GetInstance().LogEvent(baseData);

                baseData.DealEvent();
                ForeachBuff();

                Hotfix_LT.Combat.CombatInfoData.GetInstance().LogSeparator(baseData);
            }

            UnsetAnimState(AnimState.Processing);
        }

        private int _eventQueueTimer = 0;
        private void HandleEventQueueTimer(int timer)
        {
            _eventQueueTimer = 0;
            HandleEventQueue();
        }

        void HandleStartConversationEvent()
        {
            if (_firstBattle)
            {
                ConversationStart();
                StartFirstStory.Instance.Enter(delegate ()
                {
                    ConversationEnd();
                });
            }
        }

        private void ConversationStart()
        {
            _conversationing = true;
            MyFollowCamera.Instance.isActive = false;
        }

        private void ConversationEnd()
        {
            _conversationing = false;
            MyFollowCamera.Instance.isActive = true;
        }

        public void AutoSkillSelect(Hotfix_LT.Combat.CombatCharacterSyncData ch_data)
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "ShowAutoSkillSelect", ch_data);
        }

        public void ShowSkillPanel(Hotfix_LT.Combat.CombatCharacterSyncData ch_data)
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "ShowSkillPanel", ch_data);
        }

        public void EndSkill(Hotfix_LT.Combat.CombatCharacterSyncData data)
        {
            foreach (var dmgqueue in Hotfix_LT.Combat.CombatSyncData.Instance.DamageDatas)
            {
                while (dmgqueue.Value.Count > 0)
                {
                    var dmg = dmgqueue.Value.Dequeue();
                    if (dmg.IsDirect)
                    {
                        Hotfix_LT.Combat.CombatInfoData.GetInstance().LogError(EB.Localizer.GetString("ID_codefont_in_LTCombatEventReceiver_24056"), dmg.GenerateLog());
                        dmg.IsDirect = false;
                    }
                    dmg.OnHit();
                }
            }
            Hotfix_LT.Combat.CombatSyncData.Instance.DamageDatas.Clear();
            LTCombatEventReceiver.Instance.ForEach((Hotfix_LT.Combat.Combatant c) =>
            {
                c.ShowBuffFloatFont();
            });

            if (IsAnimStateNotInBattle()) return;

            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "SetCurrentActionSkill", 0);

            UnsetAnimState(AnimState.CastingSkill);

            if (Hotfix_LT.Combat.CombatSyncData.Instance.OverDeathEvenAction != null)
            {
                Hotfix_LT.Combat.CombatInfoData.GetInstance().LogString("滞后的CombatDeathEventData.OnHit(target)处理\n");
                Hotfix_LT.Combat.CombatSyncData.Instance.OverDeathEvenAction();
                Hotfix_LT.Combat.CombatSyncData.Instance.OverDeathEvenAction = null;
            }
        }

        private void TransformAutoActionSequence(int actionId)
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "TransformAutoActionSequence", actionId);
        }

        public void SetDeath(Hotfix_LT.Combat.Combatant combatant, bool isTrue)
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "SetDeath", combatant.Data.TeamId, combatant.Data.IngameId, isTrue);
        }

        private void OnSomeOneActionEvent()
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "OnSomeOneActionEvent");
        }

        public bool StartEffect(Hotfix_LT.Combat.CombatDamageSyncData dmg_data)
        {
            var SourceIndex = Hotfix_LT.Combat.CombatSyncData.Instance.GetCombatantIndex(dmg_data.Source);
            var TargetIndex = Hotfix_LT.Combat.CombatSyncData.Instance.GetCombatantIndex(dmg_data.Target);
            var combatant = GetCombatant(TargetIndex);
            if (combatant == null)
            {
                EB.Debug.LogError("StartEffect: target not found, waiting for create {0}", TargetIndex.ToString());
                return false;
            }

            if (dmg_data.Source == dmg_data.Target)
            {
                if (dmg_data.Damage >= 0)
                {
                    Hotfix_LT.Combat.DamageEffectEvent effect_event = new Hotfix_LT.Combat.DamageEffectEvent();
                    effect_event.EffectType = Hotfix_LT.Combat.eCombatEffectType.DAMAGE;
                    effect_event.Sender = SourceIndex;
                    effect_event.Target = TargetIndex;
                    effect_event.Damage = dmg_data.Damage;
                    effect_event.Show = dmg_data.Damage;
                    effect_event.Critical = dmg_data.IsCrit;
                    combatant.StartEffect(effect_event);
                }
                else
                {
                    Hotfix_LT.Combat.HealEffectEvent effect_event = new Hotfix_LT.Combat.HealEffectEvent();
                    effect_event.EffectType = Hotfix_LT.Combat.eCombatEffectType.HEAL;
                    effect_event.Sender = SourceIndex;
                    effect_event.Target = TargetIndex;
                    effect_event.Heal = dmg_data.Damage;
                    effect_event.Show = dmg_data.Damage;
                    combatant.StartEffect(effect_event);
                }

            }
            else
            {
                Hotfix_LT.Combat.ReactionEffectEvent effect_event = new Hotfix_LT.Combat.ReactionEffectEvent();
                effect_event.EffectType = Hotfix_LT.Combat.eCombatEffectType.REACTION;
                effect_event.Reaction = MoveEditor.PlayHitReactionProperties.eReactionType.Hit;
                effect_event.Sender = SourceIndex;
                effect_event.Target = TargetIndex;
                effect_event.Damage = dmg_data.Damage;
                effect_event.Show = dmg_data.Damage;
                effect_event.IsCritical = dmg_data.IsCrit;
                combatant.StartEffect(effect_event);
            }
            return true;
        }

        public void SetWinningTeam(bool isVictory)
        {
            //clean state
            ForEach(combatant =>
            {
                if (combatant.EventState != null)
                {
                    combatant.EventState.Stop();
                    combatant.EndExile();
                    combatant.EndRevive();
                    //Debug.Assert(combatant.EventState == null, combatant.myName + " Stop failed ?");
                }

                combatant.RemoveLoopImpactFX();
                combatant.StopAllCoroutines();
            });

            // victory action
            ForEach(combatant =>
            {
                if (combatant.ActionState != null)
                {
                    combatant.ActionState.Stop();
                }
                combatant.FXHelper.StopAll(true);

                if (isVictory && !CombatLogic.Instance.IsPlayerOrChallengerSide(combatant.Data.TeamId) && combatant.GetMoveState() != MoveController.CombatantMoveState.kDeath)
                {
                    combatant.transform.localScale = Vector3.zero;
                }
            });
            StopAllCoroutines();
        }

        public void PlayVictoryDance()
        {
            // victory action
            ForEach(combatant =>
            {
                if (CombatLogic.Instance.IsPlayerOrChallengerSide(combatant.Data.TeamId))
                {
                    combatant.TryVictoryDance();
                }
            });
        }

        public void ExitCombat()
        {
            EB.Debug.Log("CombatEventReceiver.ExitCombat: exit combat");

            // clean combat status
            Hotfix_LT.Combat.CombatSyncData.Instance.CleanUp();
            Hotfix_LT.Combat.CombatSyncData.Instance.EventQueue.Clear();
            _firstBattle = false;
            _conversationing = false;

            _comboNum = 1;

            // clean state
            ForEach(combatant =>
            {
                if (combatant.EventState != null)
                {
                    combatant.EventState.Stop();
                    combatant.EndRevive();
                    combatant.EndExile();
                    //Debug.Assert(combatant.EventState == null, combatant.myName + " Stop failed ?");
                }

                combatant.RemoveSelectionFX(true);
                combatant.RemoveLoopImpactFX();
                combatant.StopAllCoroutines();
            });

            // stop action
            ForEach(combatant =>
            {
                if (combatant.ActionState != null)
                {
                    combatant.ActionState.Stop();
                }
            });

            // clean fx
            ForEach(combatant => combatant.FXHelper.StopAll(true));

            StopAllCoroutines();

            //CombatManager.Instance.UnRegisterListener();
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatManager", "Instance", "UnRegisterListener");

            CleanupTeams();
            StopRadialBlurEffect();

            BOSSMode = false;
            Ready = false;
            m_battleStateEvent = null;
            m_pendingEvents.Clear();
            m_timeScale = 1.0f;
            m_timeScaleVelocity = 0.0f;
            m_timeScaleTime = 0.0f;
            TimerManager.instance.RemoveTimerSafely(ref _StartCombat_Seq);
            m_firstPlaybackStartAudioEventTriggered = false;
            deathAnimPlayingId.Clear();
            SetAnimState(AnimState.NotInBattle);
        }

        public void CleanupTeam(int team_index)
        {
            if (m_teams == null || m_teams[team_index] == null)
            {
                return;
            }

            for (int i = 0; i < m_teams[team_index].Length; ++i)
            {
                if (m_teams[team_index][i] != null)
                {
                    RemoveCombatant(m_combatants[team_index][i]);
                }
            }
        }

        public float spawnFxStayTime = 0.2f;
        public bool spawnCameraing = false;
        private void GetSpawnCamera(ICollection<Hotfix_LT.Combat.CombatCharacterSyncData> list, out string modelName, out string spawnCamera)
        {
            //要改成读表
            modelName = "";
            spawnCamera = "";
            foreach (var i in list)
            {
                Hashtable info = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.CharacterTemplateManager", "Instance", "GetMonsterInfoFromMain", i.TplId);
                if (info == null || info["monster_type"].ToInt32() != 3) continue;
                modelName = i.Model;
                if (info["spawn_camera"] != null)
                {
                    spawnCamera = GetStrToolFunc(info["spawn_camera"]);
                    if (!string.IsNullOrEmpty(spawnCamera)) spawnCameraing = true;
                }
                return;
            }
        }

        /// <summary>
        /// Boss出场动画镜头控制
        /// </summary>
        /// <param name="spawnModelGo"></param>
        /// <param name="specialCameraName"></param>
        /// <returns></returns>
        private IEnumerator SpawnCamera(GameObject spawnModelGo, string specialCameraName)
        {
            if (string.IsNullOrEmpty(specialCameraName))
            {
                spawnCameraing = false;
                yield break;
            }

            if (specialCameraName.Contains("M001"))
            {
                //世界boss一场活动里面只需要播放一次特殊出场镜头，不用每次进战斗都播放，所以这里做特殊处理
                bool isPlay = (bool)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTWorldBossDataManager", "Instance", "IsPlayWorldBossSpecialCam", false);
                
                if (!isPlay)
                {
                    spawnCameraing = false;
                    yield break;
                }
            }

            // 首领挑战（火），首领挑战（风），首领挑战（水）
            if (specialCameraName.Contains("M552") || specialCameraName.Contains("M561") || specialCameraName.Contains("M573"))
            {
                // 一场活动里面只需要播放一次特殊出场镜头，不用每次进战斗都播放，所以这里做特殊处理
                bool isPlay = (bool)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTActivityBodyItem_BossChallenge", "IsPlayBossChallengeSpecialCamera");
                
                if (!isPlay)
                {
                    spawnCameraing = false;

                    if (MyFollowCamera.Instance != null) {
                        MyFollowCamera.Instance.OnCombatBossCameraListenerEx(specialCameraName.Split('_')[0]);
                    }

                    yield break;
                }
            }

            while (!FadeScreenUIController.sFadeOver)
            {
                yield return null;
            }
            if (spawnModelGo != null)
            {
                Vector3 scale = spawnModelGo.transform.lossyScale;
                Quaternion rotation = spawnModelGo.transform.parent.rotation;
                Vector3 position = spawnModelGo.transform.parent.position;
                spawnModelGo.transform.localPosition = new Vector3(0, 10000, 0);

                yield return null;
                var fxLib = spawnModelGo.GetComponent<FXLib>();
                if (fxLib != null)
                {
                    //等待特效加载完毕
                    yield return new WaitUntil(() => fxLib.NotLoadFX());
                }

                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "BossCameraHandle");
                GM.AssetLoader<GameObject> mload = new GM.AssetLoader<GameObject>(specialCameraName, gameObject);
                yield return mload;
                if (mload.Success)
                {
                    
                    mload.Instance.transform.position = position;
                    mload.Instance.transform.localScale = scale;
                    if (specialCameraName.Contains("M001"))
                    {
                        mload.Instance.transform.localScale = 1.75f * Vector3.one;//世界boss需要特殊调整
                    }
                    mload.Instance.transform.rotation = rotation;
                    Animation a = mload.Instance.GetComponentInChildren<Animation>();
                    GameObject c = a.transform.GetChild(0).gameObject;
                    c.gameObject.CustomSetActive(true);
                    yield return null;
                    spawnModelGo.transform.localPosition = Vector3.zero;
                    spawnModelGo.GetComponent<Hotfix_LT.Combat.Combatant>().TrySpecialAcition("SKILL_specialappear");
                    a.Play();
                    yield return new WaitForSeconds(a.clip.length);
                    GameObject.Destroy(mload.Instance);
                    
                    if (MyFollowCamera.Instance != null)
                    {
                        MyFollowCamera.Instance.OnCombatBossCameraListenerEx(specialCameraName.Split('_')[0]);
                    }
                }
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "HideUIHud", false);
            }
            spawnCameraing = false;
        }

        //接收到boss的出场特效加载完成的消息
        public void OnBoss_ParticleInSceneComplete()
        {
            if (!mBossHasInit)
            {
                CharacterInit(mBosscharacter, mBossstate, mBossspawnModel, mBossspawnCameraName);
            }
        }

        private GameObject mBosscharacter;
        private object mBossstate;
        private string mBossspawnModel;
        private string mBossspawnCameraName;
        private bool mBossHasInit = false;

        //超时未获取到出场动画的处理
        private void onTimerUpHandler(int time)
        {
            TimerManager.instance.RemoveTimer(onTimerUpHandler);
            if (!mBossHasInit)
            {
                CharacterInit(mBosscharacter, mBossstate, mBossspawnModel, mBossspawnCameraName);
            }
        }


        private void CharacterInit(GameObject character, object state, string spawnModel, string spawnCameraName)
        {
            mBossHasInit = true;
            CombatCharacterSyncData member = (CombatCharacterSyncData)state;
            int team_index = member.TeamIndex;
            int index_on_team = member.IndexOnTeam;
            if (m_teams[team_index][index_on_team] != null)
            {
                CharacterVariant variant = character.transform.parent.GetComponentInChildren<CharacterVariant>();
                variant.Recycle();
                PoolModel.DestroyModel(variant.gameObject);
                return;
            }

            m_teams[team_index][index_on_team] = character;
            Combatant combatant = m_combatants[team_index][index_on_team] = m_combatantsByIngameId[member.IngameId] = character.GetComponent<Combatant>();
            combatant.SetupCombat(member);
            //需要修正
            combatant.transform.localScale = Vector3.one;

            //scale size
            bool needScale = (bool)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.SceneLogic", "NeedScale");
            if (needScale)
            {
                if (!CombatLogic.Instance.IsPlayerOrChallengerSide(combatant.Data.TeamId))
                {
                    Hashtable monsterTpl = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.CharacterTemplateManager", "Instance", "GetMonsterInfoFromMain", combatant.Data.TplId);
                    if (monsterTpl != null)
                    {
                        float mul = monsterTpl["scale_mul"].ToFloat();
                        combatant.transform.localScale = new Vector3(mul, mul, mul);
                    }
                    else
                    {
                        EB.Debug.LogError("GetMonsterInfo: MonsterInfo not found, id = {0}", combatant.Data.TplId);
                    }
                }
            }

            if (!string.IsNullOrEmpty(spawnModel) && combatant.Data.Model == spawnModel)
            {
                if (!string.IsNullOrEmpty(spawnCameraName))
                {
                    Hashtable monsterInfo = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.CharacterTemplateManager", "Instance", "GetMonsterInfoFromMain", combatant.Data.TplId);

                    if (!string.IsNullOrEmpty(GetStrToolFunc(monsterInfo["spawn_camera"])))
                    {
                        spawnCameraing = true;
                        StartCoroutine(SpawnCamera(combatant.gameObject, spawnCameraName));
                    }
                }
            }
            else 
            {
                //不是boss出场直接关闭加载界面，避免挡住双方伙伴生成过程
                UIStack.Instance.HideLoadingScreenImmediately(false, false);
            }

            combatant.Spawn();

        }
        private string GetStrToolFunc(object fx)
        {
            string result = string.Empty;
            if (fx != null)
            {
                result = fx.ToString();
            }

            return result;
        }
        private WaitForSeconds wait02 = new WaitForSeconds(0.2f);
        private bool BOSSMode = false;

        IEnumerator LoadCharactersCoroutine(ICollection<CombatCharacterSyncData> current, string spawnModel, string spawnCameraName)
        {
            List<Coroutine> coroutines = new List<Coroutine>();

            if (current.Count > 0)
            {
                coroutines.Clear();
                var BossTeam = false;
                foreach (var pair in current)
                {
                    if (pair.IsBoss)
                    {
                        BOSSMode = true;
                        BossTeam = true;
                        mBossHasInit = false;
                        //
                        List<CombatCharacterSyncData> dataList = new List<CombatCharacterSyncData>(current);
                        if (!dataList[0].IsBoss)
                        {
                            EB.Debug.LogError("配置表_BOSS阵营里,BOSS不在第一个就会卡住的");
                        }
                        break;
                    }
                }

                foreach (var pair in current)
                {
                    if (!pair.IsBoss)
                    {
                        while (BossTeam && (!mBossHasInit || spawnCameraing))
                        {
                            yield return null;
                        }
                    }
                    if (m_teams[pair.TeamId][pair.IndexOnTeam] != null)
                    {
                        EB.Debug.Log("SetupTeamsCoroutine: position not empty {0}", pair.ToString());
                        m_teams[pair.TeamId][pair.IndexOnTeam].transform.localPosition = Vector3.zero;
                        continue;
                    }

                    CombatLogic.FormationSide side = CombatLogic.Instance.GetSide(pair.TeamId);
                    string formationName = (side == CombatLogic.FormationSide.Opponent && BOSSMode) ? "Boss" : CombatSyncData.Instance.GetTeamCount(pair.TeamId).ToString();
                    string positionName = (pair.IndexOnTeam + 1).ToString();

                    Transform parentTrans = null;
                    try
                    {
                        parentTrans = Formations.Instance.GetPositionTransform(side, formationName, positionName);
                    }
                    catch(System.NullReferenceException e)
                    {
                        EB.Debug.LogError(e.ToString());
                        continue;
                    }

                    {
                        var self = this;
                        var coroutine = StartCoroutine(GetHeroGameObjectCoroutine(parentTrans.gameObject, pair.Model, pair.Equipments, pair, delegate (GameObject character, object state)
                        {
                            if (self == null)
                            {
                                EB.Debug.LogError("SetupTeamsCoroutine Callback: self is destroyed");
                                return;
                            }

                            if (isBossWave(pair) && !string.IsNullOrEmpty(spawnCameraName))
                            {
                                mBosscharacter = character;
                                mBossstate = state;
                                mBossspawnModel = spawnModel;
                                mBossspawnCameraName = spawnCameraName;
                                mBossHasInit = false;
                                character.transform.localPosition = new Vector3(1000, 0, 0);
                                //超时处理(之前设置2000看不出为什么要2秒的原因,现在设置0)
                                //TimerManager.instance.AddTimer(0, 1, onTimerUpHandler);
                                //if (!mBossHasInit)
                                //{
                                CharacterInit(mBosscharacter, mBossstate, mBossspawnModel, mBossspawnCameraName);
                                //}
                            }
                            else
                            {
                                //character.CustomSetActive(true);
                                CharacterInit(character, state, spawnModel, spawnCameraName);
                            }
                        }));

                        coroutines.Add(coroutine);
                        if (coroutines.Count >= 2)
                        {
                            for (int i = 0; i < coroutines.Count; ++i)
                            {
                                yield return coroutines[i];
                            }
                            coroutines.Clear();
                            yield return wait02;
                        }

                        while (spawnCameraing)
                        {
                            yield return null;
                        }

                        //yield return coroutine; //如果要改成等待单个加载就取消注释
                        //coroutines.Add(coroutine);
                        //yield return wait02;
                    }
                }

                //// wait load coroutine
                for (int i = 0; i < coroutines.Count; ++i)
                {
                    yield return coroutines[i];
                }
            }
        }


        private IEnumerator SpawnCharactersCoroutine(ICollection<Hotfix_LT.Combat.CombatCharacterSyncData> current)
        {
            if (current.Count > 0)
            {
                foreach (var pair in current)
                {
                    while (m_combatants[pair.TeamIndex][pair.IndexOnTeam] == null)
                    {
                        yield return null;
                    }
                }

                foreach (var pair in current)
                {
                    while (m_combatants[pair.TeamIndex][pair.IndexOnTeam] != null && !m_combatants[pair.TeamIndex][pair.IndexOnTeam].Ready)
                    {
                        yield return null;
                    }
                }
            }
        }

        Combatant ScrollSkillCaster;

        private void SetupScrollSkillCaster(string prefab_name)
        {
            string prefab_path = "Bundles/Player/Variants/" + prefab_name;
            var self = this;
            Transform parent_node = transform;
            PoolModel.GetModelAsync(prefab_path, parent_node.transform.position, parent_node.transform.rotation, delegate (UnityEngine.Object obj, object param)
            {
                //We should remove character when exit combat & always instantiate character for each battle.
                GameObject variantObj = obj as GameObject;

                if (variantObj == null)
                {
                    EB.Debug.LogError("Failed to create hero game object");
                    return;
                }

                if (self == null || parent_node == null)
                {
                    EB.Debug.LogError("LoadAsync Callback: self is destroyed");
                    PoolModel.DestroyModel(obj);
                    return;
                }

                variantObj.transform.parent = parent_node;
                CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();

                if (variant == null)
                {
                    EB.Debug.LogError("Failed to create hero game object");
                    PoolModel.DestroyModel(obj);
                    return;
                }

                variant.SyncLoad = false;
                variant.InstantiateCharacter();
                GameObject hero = variant.CharacterInstance;

                if (hero == null)
                {
                    EB.Debug.LogError("Failed to create hero game object");
                    PoolModel.DestroyModel(obj);
                    return;
                }

                hero.SetActive(true);
                hero.transform.parent = parent_node;
                hero.transform.localPosition = Vector3.zero;
                ScrollSkillCaster = hero.GetComponent<Combatant>();
                CombatCharacterSyncData char_data = new CombatCharacterSyncData();
                char_data.Index = new CombatantIndex(CombatLogic.Instance.LocalPlayerTeamIndex, -1);
                char_data.Hp = long.MaxValue;
                char_data.MaxHp = long.MaxValue;

                if (ScrollSkillCaster != null)
                {
                    ScrollSkillCaster.SetupCombat(char_data);
                }
            }, null);
        }

        public void CleanupTeams()
        {
            CleanupTeam(CombatLogic.Instance.ChallengerTeamIndex);
            CleanupTeam(CombatLogic.Instance.DefenderTeamIndex);
            RemoveScrollCaster();
        }

        public void RemoveCombatant(Hotfix_LT.Combat.CombatantIndex idx)
        {
            Hotfix_LT.Combat.Combatant combatant = m_combatants[idx.TeamIndex][idx.IndexOnTeam];
            RemoveCombatant(combatant);
        }

        public void RemoveCombatant(int id)
        {
            Hotfix_LT.Combat.Combatant combatant = m_combatantsByIngameId[id];
            RemoveCombatant(combatant);
        }
        public void RemoveCombatant(Hotfix_LT.Combat.Combatant combatant)
        {
            if (combatant == null || combatant.Data == null)
            {
                EB.Debug.LogError("LTComabatEventReceiver RemoveCombatant combatant is Null !!!");
                return;
            }

            m_teams[combatant.Data.TeamIndex][combatant.Data.IndexOnTeam] = null;
            m_combatants[combatant.Data.TeamIndex][combatant.Data.IndexOnTeam] = null;
            m_combatantsByIngameId[combatant.Data.IngameId] = null;
            combatant.CleanCombat();

            CharacterVariant variant = combatant.transform.parent.GetComponentInChildren<CharacterVariant>();
            variant.Recycle();
            PoolModel.DestroyModel(variant.gameObject);
        }

        public void RemoveScrollCaster()
        {
            if (ScrollSkillCaster != null)
            {
                ScrollSkillCaster.CleanCombat();

                CharacterVariant variant = ScrollSkillCaster.transform.parent.GetComponentInChildren<CharacterVariant>();
                variant.Recycle();
                PoolModel.DestroyModel(variant.gameObject);
            }
        }

        // private GameObject GetHeroGameObject(GameObject parent_node, string prefab_name, IDictionary partitions)
        // {
        //     string prefab_path = "Bundles/Player/Variants/" + prefab_name;
        //     m_toRemoveResources.Add(prefab_path);

        //     //We should remove character when exit combat & always instantiate character for each battle.
        //     GameObject variantObj = PoolModel.GetNext(prefab_path, parent_node.transform.position, parent_node.transform.rotation) as GameObject;
        //     if (variantObj == null)
        //     {
        //         EB.Debug.LogError("Failed to create hero game object");
        //         return null;
        //     }

        //     variantObj.transform.parent = parent_node.transform;
        //     CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
        //     if (variant == null)
        //     {
        //         EB.Debug.LogError("Failed to create hero game object");
        //         PoolModel.DestroyManagedObject(variantObj);
        //         return null;
        //     }

        //     variant.SyncLoad = false;
        //     variant.InstantiateCharacter(partitions);
        //     GameObject hero = variant.CharacterInstance;
        //     if (hero == null)
        //     {
        //         EB.Debug.LogError("Failed to create hero game object");
        //         PoolModel.DestroyManagedObject(variantObj);
        //         return null;
        //     }
        //     hero.SetActive(true);
        //     hero.transform.parent = parent_node.transform;
        //     return hero;
        // }
        private IEnumerator GetHeroGameObjectCoroutine(GameObject parent_node, string prefab_name, IDictionary partitions, object state, System.Action<GameObject, object> callback)
        {
            string prefab_path = "Bundles/Player/Variants/" + prefab_name;
            m_toRemoveResources.Add(prefab_path);
            var self = this;
            yield return PoolModel.GetModelAsync(prefab_path, parent_node.transform.position, parent_node.transform.rotation,
            delegate (UnityEngine.Object obj, System.Object param)
            {
                //We should remove character when exit combat & always instantiate character for each battle.
                GameObject variantObj = obj as GameObject;
                if (variantObj == null)
                {
                    EB.Debug.LogError(EB.Debug.ACCIDENTAL + "{0}Load Model 1, Failed to create hero game object");
                    callback(null, state);
                    return;
                }

                if (self == null || parent_node == null)
                {
                    EB.Debug.LogError("{0}Load Model 2, LoadAsync Callback: self is destroyed", EB.Debug.ACCIDENTAL);
                    PoolModel.DestroyModel(obj);
                    callback(null, state);
                    return;
                }

                variantObj.transform.SetParent(parent_node.transform);
                CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
                if (variant == null)
                {
                    EB.Debug.LogError("{0}Load Model 3, variantObj.GetComponent<CharacterVariant>()=null", EB.Debug.ACCIDENTAL);
                    PoolModel.DestroyModel(obj);
                    callback(null, state);
                    return;
                }

                variant.SyncLoad = false;
                variant.InstantiateCharacter(partitions);
                GameObject hero = variant.CharacterInstance;
                if (hero == null)
                {
                    EB.Debug.LogError("{0}Load Model 4, variant.CharacterInstance=null", EB.Debug.ACCIDENTAL);
                    PoolModel.DestroyModel(obj);
                    callback(null, state);
                    return;
                }
                //因为M532模型存在在飞出镜头之外的情况，所以在这里强行调它的动画渲染模式 by:wwh 20190809
                if (prefab_name.Contains("532"))
                {
                    Animator animator = hero.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    }
                }

                hero.CustomSetActive(true);
                hero.transform.SetParent(parent_node.transform);
                callback(hero, state);
            }, state);
        }

        public void PlayRadialBlurEffect(Hotfix_LT.Combat.Combatant target)
        {
            //bool allDie = (WonTeam >= 0 && CombatLogic.Instance.TotalBatch[1 - WonTeam] == Batches[1 - WonTeam] + 1);
            //ForEach(combatant => allDie = allDie && (combatant.Index.TeamIndex == WonTeam || !combatant.IsAlive()));
            //if (allDie && Time.timeScale == m_timeScale)
            //{
            //	PostFXManager.Instance.SetRadialTarget(Camera.main, target.FXHelper.ChestNubTransform);
            //	Time.timeScale = 0.5f * m_timeScale;
            //	m_timeScaleTime = 1.0f;
            //	RenderGlobals.SetRadialBlur(0.0f, 0.0f);
            //}
        }

        public void StopRadialBlurEffect()
        {
            Time.timeScale = 1.0f;
            m_timeScaleTime = 0.0f;
            if (PostFXManager.Instance != null)
            {
                PostFXManager.Instance.SetRadialTarget(null, null);
            }
            RenderGlobals.SetRadialBlur(0.0f, 0.0f);
        }

        public void UpdateRadialBlurEffect()
        {
            // update time scale
            if (m_timeScaleTime > 0.0f)
            {
                float deltaTime = Time.deltaTime / Time.timeScale;
                m_timeScaleTime -= deltaTime;
                m_timeScaleTime = m_timeScaleTime >= 0.0f ? m_timeScaleTime : 0.0f;
                Time.timeScale = Mathf.SmoothDamp(Time.timeScale, m_timeScale, ref m_timeScaleVelocity, m_timeScaleTime, float.PositiveInfinity, deltaTime);
                if (m_timeScaleTime > 0.0f)
                {
                    float startScale = 0.5f * m_timeScale;
                    float mix = (Time.timeScale - startScale) / (m_timeScale - startScale);
                    RenderGlobals.SetRadialBlur(mix, 0.0f);
                }
                else
                {
                    StopRadialBlurEffect();
                }
            }
        }

        public bool IsRadialBlurEffect()
        {
            return m_timeScaleTime > 0.0f;
        }
    }
}
