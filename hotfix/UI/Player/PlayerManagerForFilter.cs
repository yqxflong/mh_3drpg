using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;


namespace Hotfix_LT.UI
{
    public class PlayerDelegate
    {
        public long id;
        public Vector3 pos;
        public Quaternion quater;
        public string templateid;
        public long aid;
        public string sceneid;
        public bool created;
        public bool preloaded;

        public PlayerDelegate(long id, object data, string scenetype)
        {
            string pos_str = EB.Dot.String("pos", data, null);
            if (pos_str == null)
            {
                this.pos = Vector3.zero;
            }
            else
            {
                this.pos = GM.LitJsonExtension.ImportVector3(pos_str);
            }
            this.id = id;
            this.quater = Quaternion.identity;
            this.templateid = EB.Dot.String("tid", data, "");
            this.aid = EB.Dot.Long("aid", data, 0);
            this.sceneid = scenetype + EB.Dot.Long("sid", data, 0);
            this.created = false;
            this.preloaded = false;
        }

        public bool IsSpecial()
        {
            return (aid > 0 && aid == PlayerManagerForFilter.UserAid);
        }
    }

    public class PlayerManagerForFilter
    {

        //protected static bool m_Inited = false;
        public static long UserAid;
        private static PlayerManagerForFilter m_Instance;
        private const int HIGHT_PERFORMANCE_PLAYRES = 10;
        private const int MEDIUM_PERFORMANCE_PLAYRES = 2;
        private const int LOW_PERFORMANCE_PLAYRES = 2;

        public static PlayerManagerForFilter Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new PlayerManagerForFilter();
                    m_Instance.Initialize();
                }
                return m_Instance;
            }
        }

        public static void Dispose()
        {
            if (m_Instance != null) m_Instance.StopShowPlayer();
            m_Instance = null;
        }

        private void Initialize()
        {
            m_RegionPlayers = new Dictionary<long, PlayerDelegate>();
            m_SpcialPlayers = new Dictionary<long, PlayerDelegate>();
            m_ManagedPlayers = new Dictionary<long, PlayerDelegate>();
            m_PlayerQueue = new EB.Collections.Queue<PlayerDelegate>();
            int SettingPlayerNum = global::UserData.PlayerNum;
            m_MaxManagedPlayer = SettingPlayerNum;
            m_IsCanShow = false;
        }

        private bool CalcMaxPlayer()
        {
            int SettingPlayerNum = global::UserData.PlayerNum;
            if (SettingPlayerNum >= 0)
            {
                m_MaxManagedPlayer = SettingPlayerNum;
                return true;
            }
            else
            {
                m_MaxManagedPlayer = CalcMaxPlayerByPerformance();
                if (m_MaxManagedPlayer >= 0)
                {
                    global::UserData.PlayerNum = m_MaxManagedPlayer;
                    return true;
                }
            }
            return false;
        }

        private int CalcMaxPlayerByPerformance()
        {
            if (PerformanceManager.Instance.PerformanceInfo == null)
            {
                return -1;
            }

            int persormancePlayerNum = -1;
            switch (PerformanceManager.Instance.PerformanceInfo.CpuProfileName)
            {
                case "High":
                    persormancePlayerNum = HIGHT_PERFORMANCE_PLAYRES;
                    break;
                case "Medium":
                    persormancePlayerNum = MEDIUM_PERFORMANCE_PLAYRES;
                    break;
                case "Low":
                    persormancePlayerNum = LOW_PERFORMANCE_PLAYRES;
                    break;
            }
            return persormancePlayerNum;
        }

        private Dictionary<long, PlayerDelegate> m_RegionPlayers;
        private Dictionary<long, PlayerDelegate> m_SpcialPlayers;
        private Dictionary<long, PlayerDelegate> m_ManagedPlayers;
        private EB.Collections.Queue<PlayerDelegate> m_PlayerQueue;
        private string m_CurrentSceneID;
        private SceneLogic m_CurrentSceneLogic = null;
        private int m_MaxManagedPlayer;
        public int MaxManagedPlayer
        {
            get
            {
                return m_MaxManagedPlayer < 0 ? LOW_PERFORMANCE_PLAYRES : m_MaxManagedPlayer;
            }
            set
            {
                m_MaxManagedPlayer = value;
                //SparxHub.Instance.GetManager<EB.Sparx.SceneManager>().SetSceneViewPlayerNum(m_MaxManagedPlayer);
                MatchPlayersToMaxManagerPlayer();
            }
        }
        private bool m_IsCanShow;

        public bool GetTest1()
        {
            return !m_IsCanShow;
        }
        public bool GetTest2()
        {
            return !GM.AssetManager.IsIdle;
        }
        public bool GetTest3()
        {
            return m_PlayerQueue.Count <= 0;
        }

        //发起切换场景时锁住防止创建玩家
        public void StopShowPlayer()
        {
            m_IsCanShow = false;
            NpcManager.Instance.StopShowNpc();
            m_CurrentSceneLogic = null;
        }

        /// <summary>
        /// 解决 多人场景进战斗 后出来  前后两次的sceneid一样导致旧的managedplayers没有清理干净
        /// </summary>
        public void StopShowPlayerForGoToCombat()
        {
            m_IsCanShow = false;
            Clear();
            NpcManager.Instance.StopShowNpcForGoToCombat();
            m_CurrentSceneLogic = null;
        }

        public void BeginShowPlayer()
        {
            m_IsCanShow = true;
            m_CurrentSceneID = GetSceneId();
            ClearNotCurrentScenePlayer();
            NpcManager.Instance.BeginShowNpc();
            m_CurrentSceneLogic = MainLandLogic.GetInstance();
        }

        public void ProcessPlayerLeaveSync(object incomingData)
        {
            string scenetype = EB.Dot.String("scenetype", incomingData, "");
            IDictionary players = EB.Dot.Object(scenetype + ".pl", incomingData, null);
            if (players == null) return;
            foreach (DictionaryEntry entry in players)
            {
                long userid = System.Convert.ToInt64(entry.Key);
                PlayerOut(userid);
            }
        }

        public void ProcessPlayerCommInSync(object incomingData)
        {
            UserAid = GetLocalPlayerAid();
            string scenetype = EB.Dot.String("scenetype", incomingData, "");
            IDictionary players = EB.Dot.Object(scenetype + ".pl", incomingData, null);
            if (players == null) return;
            int num = 0;
            foreach (DictionaryEntry entry in players)
            {
                long userid = System.Convert.ToInt64(entry.Key);
                if (entry.Value == null)//Player leave
                {
                    continue;
                }
                if (userid == LoginManager.Instance.LocalUserId.Value) continue;
                if (m_ManagedPlayers.ContainsKey(userid)) continue;

                IDictionary datatmp = entry.Value as IDictionary;
                if (datatmp["pos"] != null)
                {
                    PlayerIn(userid, datatmp, scenetype);
                    num++;
                }
            }
            if (num == 0) return;
            int less = MaxManagedPlayer - m_ManagedPlayers.Count;
            if (less > 0)
            {
                for (int i = 0; i < less; i++)
                {
                    PlayerDelegate player = GetPlayerToShow();
                    if (player != null) ShowPlayer(player);
                    else break;
                }
            }
            PrintStats();
        }

        /// <summary>
        /// 玩家进入region
        /// </summary>
        /// <param name=""></param>
        public void PlayerIn(long userid, IDictionary data, string scenetype)
        {
            PlayerDelegate player = new PlayerDelegate(userid, data, scenetype);
            if (player.IsSpecial())
            {
                if (!m_SpcialPlayers.ContainsKey(player.id)) m_SpcialPlayers.Add(player.id, player);
            }
            else
            {
                if (!m_RegionPlayers.ContainsKey(player.id)) m_RegionPlayers.Add(player.id, player);
            }
        }

        /// <summary>
        /// 玩家离开 
        /// </summary>
        /// <param name="uid"></param>
        public void PlayerOut(long uid)
        {
            //从ManagedPlayers中删除
            //获取一个GetPlayerToShow
            //显示 ShowPlayer
            if (m_ManagedPlayers.ContainsKey(uid))
            {
                m_ManagedPlayers.Remove(uid);
                if (m_ManagedPlayers.Count < MaxManagedPlayer)
                {
                    PlayerDelegate player = GetPlayerToShow();
                    if (player != null) ShowPlayer(player);
                }
                PrintStats();
//#if PRINT_PLAYER_SYNC_DEBUG
//			EB.Debug.LogWarning("Player leave uid=" + uid);
//#endif
            }
            else
            {
                if (m_RegionPlayers.ContainsKey(uid)) m_RegionPlayers.Remove(uid);
                else if (m_SpcialPlayers.ContainsKey(uid)) m_SpcialPlayers.Remove(uid);
            }
//#if PRINT_PLAYER_SYNC_DEBUG
//			CheckConsistent();
//#endif

            PlayerController pc = PlayerManager.GetPlayerController(uid);
            if (pc == null) return;
            if (pc.IsLocal) return;
            PlayerManager.UnregisterPlayerController(pc);
            pc.Destroy();
        }

        /// <summary>
        /// 玩家隐藏 
        /// </summary>
        /// <param name="uid"></param>
        public void PlayerHide(long uid)
        {
            if (m_ManagedPlayers.ContainsKey(uid))
            {
                PlayerDelegate player = m_ManagedPlayers[uid];
                if (player == null) return;
                player.preloaded = false;
                player.created = false;
                m_ManagedPlayers.Remove(uid);
                if (player.IsSpecial())
                {
                    m_SpcialPlayers.Add(uid, player);
                }
                else
                {
                    m_RegionPlayers.Add(uid, player);
                }
            }
            PlayerController pc = PlayerManager.GetPlayerController(uid);
            if (pc == null) return;
            if (pc.IsLocal) return;
            PlayerManager.UnregisterPlayerController(pc);
            pc.Destroy();
        }

        public void Clear()
        {
            m_RegionPlayers.Clear();
            m_SpcialPlayers.Clear();
            m_ManagedPlayers.Clear();
            m_PlayerQueue.Clear();
        }

        /// <summary>
        /// is slow function  use less
        /// </summary>
        public void ClearNotCurrentScenePlayer()
        {
            List<long> player_to_remove = new List<long>();
            foreach (KeyValuePair<long, PlayerDelegate> entry in m_RegionPlayers)
            {
                PlayerDelegate player = entry.Value;
                if (!player.sceneid.Equals(m_CurrentSceneID)) player_to_remove.Add(player.id);
            }
            for (int i = 0; i < player_to_remove.Count; i++)
            {
                if (m_RegionPlayers.ContainsKey(player_to_remove[i])) m_RegionPlayers.Remove(player_to_remove[i]);
            }

            player_to_remove.Clear();
            foreach (KeyValuePair<long, PlayerDelegate> entry in m_SpcialPlayers)
            {
                PlayerDelegate player = entry.Value;
                if (!player.sceneid.Equals(m_CurrentSceneID)) player_to_remove.Add(player.id);
            }
            for (int i = 0; i < player_to_remove.Count; i++)
            {
                if (m_SpcialPlayers.ContainsKey(player_to_remove[i])) m_SpcialPlayers.Remove(player_to_remove[i]);
            }
            player_to_remove.Clear();

            foreach (KeyValuePair<long, PlayerDelegate> entry in m_ManagedPlayers)
            {
                PlayerDelegate player = entry.Value;
                if (!player.sceneid.Equals(m_CurrentSceneID)) player_to_remove.Add(player.id);
            }
            for (int i = 0; i < player_to_remove.Count; i++)
            {
                if (m_ManagedPlayers.ContainsKey(player_to_remove[i])) m_ManagedPlayers.Remove(player_to_remove[i]);
            }
        }

        /// <summary>
        /// 获取一个可以显示的Player
        /// </summary>
        /// <returns></returns>
        public PlayerDelegate GetPlayerToShow()
        {
            if (m_SpcialPlayers.Count > 0)
            {
                Dictionary<long, PlayerDelegate>.Enumerator enu = m_SpcialPlayers.GetEnumerator();
                enu.MoveNext();
                PlayerDelegate player = enu.Current.Value;
                m_SpcialPlayers.Remove(player.id);
//#if PRINT_PLAYER_SYNC_DEBUG
//			EB.Debug.LogWarning("Player add uid=" + player.id);
//#endif
                if (!m_ManagedPlayers.ContainsKey(player.id)) m_ManagedPlayers.Add(player.id, player);
                else
                {
                    EB.Debug.LogWarning("Already Managed this Player {0}",player.id);
                }
                return player;
            }
            else if (m_RegionPlayers.Count > 0)
            {
                Dictionary<long, PlayerDelegate>.Enumerator enu = m_RegionPlayers.GetEnumerator();
                enu.MoveNext();
                PlayerDelegate player = enu.Current.Value;
                m_RegionPlayers.Remove(player.id);
//#if PRINT_PLAYER_SYNC_DEBUG
//			EB.Debug.LogWarning("Player add uid=" + player.id);
//#endif
                if (!m_ManagedPlayers.ContainsKey(player.id)) m_ManagedPlayers.Add(player.id, player);
                else
                {
                    EB.Debug.LogWarning("Already Managed this Player {0}", player.id);
                }
                return player;
            }
            return null;
        }

        /// <summary>
        /// 显示一个玩家
        /// </summary>
        public void ShowPlayer(PlayerDelegate player)
        {
            m_PlayerQueue.Enqueue(player);
        }

        void MatchPlayersToMaxManagerPlayer()
        {
            if (m_ManagedPlayers.Count == MaxManagedPlayer)
            {
                return;
            }
            else if (m_ManagedPlayers.Count > MaxManagedPlayer)
            {
                int delta = m_ManagedPlayers.Count - MaxManagedPlayer;
                List<long> toclear = new List<long>(delta);
                foreach (KeyValuePair<long, PlayerDelegate> kv in m_ManagedPlayers)
                {
                    delta--;
                    toclear.Add(kv.Value.id);
                    if (delta == 0) break;
                }

                for (int i = 0; i < toclear.Count; i++)
                {
                    PlayerHide(toclear[i]);
                }
            }
            else
            {
                int less = MaxManagedPlayer - m_ManagedPlayers.Count;
                if (less > 0)
                {
                    for (int i = 0; i < less; i++)
                    {
                        PlayerDelegate player = GetPlayerToShow();
                        if (player != null) ShowPlayer(player);
                        else break;
                    }
                }
            }
        }

        private bool needSyncNumToServer = true;
        void Update()
        {
            if (m_MaxManagedPlayer < 0)
            {
                CalcMaxPlayer();
            }
            else
            {
                if (needSyncNumToServer)
                {
                    needSyncNumToServer = false;
                }
            }

            FreshQueue();
            if (!m_IsCanShow) return;
            if (!GM.AssetManager.IsIdle) return;
            if (m_PlayerQueue.Count > 0)
            {
                PlayerDelegate playerdata = m_PlayerQueue.Dequeue();
                if (playerdata != null)
                {
                    if (playerdata.sceneid != m_CurrentSceneID)
                    {
//#if PRINT_PLAYER_SYNC_DEBUG
//					EB.Debug.LogWarning("Player not this scene uid=" + playerdata.id);
//#endif
                        m_ManagedPlayers.Remove(playerdata.id);
                    }
                    else if (m_ManagedPlayers.ContainsKey(playerdata.id))
                    {
                        if (!playerdata.preloaded)
                        {
                            if (m_CurrentSceneLogic != null) m_CurrentSceneLogic.PreloadAsync(playerdata.id, (success) => { playerdata.preloaded = success; m_PlayerQueue.Enqueue(playerdata); });
                            else
                            {
                                EB.Debug.LogError("CurrentSceneLogic is Null");
                            }
                        }
                        else if (!m_ManagedPlayers[playerdata.id].created)
                        {
                            m_ManagedPlayers[playerdata.id].created = true;
                            if (m_CurrentSceneLogic != null) m_CurrentSceneLogic.CreateOtherPlayer(playerdata.pos, playerdata.quater, playerdata.id);
                            else
                            {
                                EB.Debug.LogError("CurrentSceneLogic is Null");
                            }
                        }
                    }
//#if PRINT_PLAYER_SYNC_DEBUG
//				CheckConsistent();
//#endif
                }
            }
            else
            {
                MatchPlayersToMaxManagerPlayer();
            }
        }

        /// <summary>
        /// 清理queue 以免占用太多内存 在战斗回多人场景时
        /// </summary>
        void FreshQueue()
        {
            if (m_PlayerQueue.Count <= 200) return;
            List<PlayerDelegate> list = new List<PlayerDelegate>(128);
            while (m_PlayerQueue.Count > 0)
            {
                PlayerDelegate playerdata = m_PlayerQueue.Dequeue();
                if (m_ManagedPlayers.ContainsKey(playerdata.id))
                {
                    list.Add(playerdata);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                m_PlayerQueue.Enqueue(list[i]);
            }
        }


        public static void Process()
        {
            if (m_Instance != null) m_Instance.Update();
        }

        string GetSceneId()
        {
            long id = 0;
            string scenetype = SceneLogicManager.getSceneType();
            DataLookupsCache.Instance.SearchDataByID<long>(scenetype + ".id", out id);
            return scenetype + id;
        }

        long GetLocalPlayerAid()
        {
            long aid = 0;
            DataLookupsCache.Instance.SearchDataByID<long>("alliance.account.aid", out aid);
            return aid;
        }

        //private Dictionary<long, PlayerDelegate> m_RegionPlayers;
        //private Dictionary<long, PlayerDelegate> m_SpcialPlayers;
        //private Dictionary<long, PlayerDelegate> m_ManagedPlayers;
        public void MergePlayer(string scenetype, IDictionary scenedata)
        {
            if (scenedata == null)
            {
                EB.Debug.LogWarning("MergePlayer scenedata is null");
                return;
            }
            //long sceneid = EB.Dot.Long("id", scenedata, 0);
            IDictionary playerdata = EB.Dot.Object("pl", scenedata, null);
            if (playerdata == null)
            {
                EB.Debug.LogWarning("MergePlayer pl is null");
                return;
            }
            List<long> player_to_remove = new List<long>();
            //Manager have scenedata didnot must clear
            foreach (KeyValuePair<long, PlayerDelegate> kv in m_RegionPlayers)
            {
                if (!playerdata.Contains(kv.Key.ToString()))
                {
                    player_to_remove.Add(kv.Key);
                }
            }
            for (int i = 0; i < player_to_remove.Count; i++)
            {
                m_RegionPlayers.Remove(player_to_remove[i]);
            }

            player_to_remove.Clear();

            foreach (KeyValuePair<long, PlayerDelegate> kv in m_SpcialPlayers)
            {
                if (!playerdata.Contains(kv.Key.ToString()))
                {
                    player_to_remove.Add(kv.Key);
                }
            }
            for (int i = 0; i < player_to_remove.Count; i++)
            {
                m_RegionPlayers.Remove(player_to_remove[i]);
            }

            player_to_remove.Clear();

            foreach (KeyValuePair<long, PlayerDelegate> kv in m_ManagedPlayers)
            {
                if (!playerdata.Contains(kv.Key.ToString()))
                {
                    player_to_remove.Add(kv.Key);
                }
            }
            for (int i = 0; i < player_to_remove.Count; i++)
            {
                PlayerOut(player_to_remove[i]);
            }

            //scenedata have  Manager didnot must add
            int num = 0;
            foreach (DictionaryEntry de in playerdata)
            {
                long userid = System.Convert.ToInt64(de.Key);
                if (de.Value == null)//Player leave
                {
                    continue;
                }
                if (userid == LoginManager.Instance.LocalUserId.Value) continue;
                if (m_ManagedPlayers.ContainsKey(userid)) continue;

                IDictionary datatmp = de.Value as IDictionary;
                if (datatmp["pos"] != null)
                {
                    PlayerIn(userid, datatmp, scenetype);
                    num++;
                }
            }

            if (num == 0) return;
            int less = MaxManagedPlayer - m_ManagedPlayers.Count;
            if (less > 0)
            {
                for (int i = 0; i < less; i++)
                {
                    PlayerDelegate player = GetPlayerToShow();
                    if (player != null) ShowPlayer(player);
                    else break;
                }
            }
            PrintStats();

        }

        bool HasPlayer(long uid)
        {
            if (m_ManagedPlayers.ContainsKey(uid) || m_RegionPlayers.ContainsKey(uid) || m_SpcialPlayers.ContainsKey(uid))
            {
                return true;
            }
            return false;
        }

        public PlayerDelegate GetPlayerDelegate(long uid)
        {
            if (m_SpcialPlayers.ContainsKey(uid))
            {
                return m_SpcialPlayers[uid];
            }
            else if (m_ManagedPlayers.ContainsKey(uid))
            {
                return m_ManagedPlayers[uid];
            }
            else if (m_RegionPlayers.ContainsKey(uid))
            {
                return m_RegionPlayers[uid];
            }
            else
            {
                return null;
            }
        }

        public string GetPlayerTempalteid(long uid)
        {
            PlayerDelegate player = GetPlayerDelegate(uid);
            if (player == null) return null;
            else return player.templateid;
        }

        void PrintStats()
        {
//#if PRINT_PLAYER_SYNC_DEBUG
//		EB.Debug.LogWarning("m_RegionPlayers.Count="+ m_RegionPlayers.Count
//			+ "-------m_SpcialPlayers.Count=" + m_SpcialPlayers.Count
//			+ "-------m_ManagedPlayers.Count=" + m_ManagedPlayers.Count
//			+ "-------m_PlayerQueue.Count=" + m_PlayerQueue.Count);
//#endif
        }

        void CheckConsistent()
        {
            int managedcount = m_ManagedPlayers.Count;
            int showcount = PlayerManager.sPlayerControllers.Count - 1;
            if (managedcount != showcount)
            {
                EB.Debug.LogError("Managed count={0}---- ShowCount={1}" ,managedcount ,showcount);
            }
        }
    }

    //public class PlayerMoveSyncManager
    //{
    //    private static PlayerMoveSyncManager m_Instance;
    //    public static PlayerMoveSyncManager Instance
    //    {
    //        get
    //        {
    //            if (m_Instance == null)
    //            {
    //                m_Instance = new PlayerMoveSyncManager();
    //                m_Instance.Initialize();
    //            }
    //            return m_Instance;
    //        }
    //    }

    //    void Initialize()
    //    {
    //        //m_LastSendTime = 0;
    //        //m_Queue = new EB.Collections.Queue<MoveSyncStruct>();            
    //        //Hotfix_LT.Messenger.AddListener<Vector3>(Hotfix_LT.EventName.PlayerMoveSyncManagerMove, Move);
    //    }

    //    private void OnDisable()
    //    {           
    //        //Hotfix_LT.Messenger.RemoveListener<Vector3>(Hotfix_LT.EventName.PlayerMoveSyncManagerMove, Move);
    //    }
    //    public static void Dispose()
    //    {
    //        Instance.OnDisable();
    //        m_Instance = null;            
    //    }

    //    //public class MoveSyncStruct
    //    //{
    //    //    public Vector3 pos;
    //    //    public int sceneid;

    //    //    public MoveSyncStruct(Vector3 pos, int sceneid)
    //    //    {
    //    //        this.pos = pos;
    //    //        this.sceneid = sceneid;
    //    //    }
    //    //}

    //    //public const double kMovementUpdateInterval = 2.0;
    //    //private EB.Collections.Queue<MoveSyncStruct> m_Queue;
    //    //private float m_LastSendTime;

    //    //public void Move(Vector3 pos)
    //    //{
    //    //    //EB.Debug.LogError("Move——({0},{1},{2})",pos.x, pos.y, pos.z);
    //    //    int scene_id = MainLandLogic.GetInstance().SceneId;
    //    //    if (m_Queue.Count == 0)
    //    //    {
    //    //        if (Time.realtimeSinceStartup - m_LastSendTime > kMovementUpdateInterval)
    //    //        {
    //    //            m_LastSendTime = Time.realtimeSinceStartup;
    //    //            SendToRemote(scene_id, pos);
    //    //            return;
    //    //        }
    //    //    }

    //    //    MoveSyncStruct mss = new MoveSyncStruct(pos, scene_id);
    //    //    m_Queue.Enqueue(mss);
    //    //}

    //    //public void Clear()
    //    //{
    //    //    m_Queue.Clear();
    //    //}

    //    //public void SendToRemote(int scene_id, Vector3 pos)
    //    //{
    //    //    //EB.Debug.Log("SendPosToRemote  ts="+ Time.realtimeSinceStartup);
    //    //    LTHotfixManager.GetManager<SceneManager>().UpdateMoveDestination(scene_id, pos, null);
    //    //}

    //    //public void OnLateUpdate()
    //    //{
    //    //    LTHotfixManager.GetManager<SceneManager>().OnLateUpdate();

    //    //    if (m_Queue.Count == 0) return;
    //    //    if (Time.realtimeSinceStartup - m_LastSendTime > kMovementUpdateInterval)
    //    //    {
    //    //        m_LastSendTime = Time.realtimeSinceStartup;
    //    //        MoveSyncStruct mss = null;
    //    //        while (m_Queue.Count > 0)//暴力合并
    //    //        {
    //    //            mss = m_Queue.Dequeue();
    //    //        }
    //    //        if (mss == null) return;
    //    //        SendToRemote(mss.sceneid, mss.pos);
    //    //        return;
    //    //    }
    //    //}
    //}
}
