using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Hotfix_LT.UI
{
    public class NpcDelegate
    {
        public string locator;
        public Vector3 pos;
        public Quaternion quater;
        public string sceneid;
        public string modelname;
        public bool isfighting;
        public string role;
        public GroupCellKey key;
        public bool created;
        public bool preloaded;
        public int attr;

        public NpcDelegate(string locator, Vector3 pos, Quaternion quater, string sceneid, string modelname, string role, bool isfighting, int attr = 0)
        {
            this.locator = locator;
            this.pos = pos;
            this.quater = quater;
            this.sceneid = sceneid;
            this.modelname = modelname;
            this.role = role;
            this.isfighting = isfighting;
            key = null;
            this.created = false;
            this.preloaded = false;
            this.attr = attr;
        }

        public NpcDelegate(DungeonPlacement placement, string sceneid)
        {
            this.locator = placement.Locator;
            if (string.IsNullOrEmpty(placement.Pos)) this.pos = Vector3.zero;
            else this.pos = GM.LitJsonExtension.ImportVector3(placement.Pos.ToString());
            Quaternion rot = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
            this.quater = rot;
            this.sceneid = sceneid;
            this.modelname = placement.Encounter;
            this.isfighting = placement.IsFighting;
            this.role = placement.Role;
            key = null;
            this.created = false;
            this.preloaded = false;
            if (!placement.Attr.Equals(string.Empty))
            {
                attr = int.Parse(placement.Attr);
            }
        }
    }

    public class NpcManager
    {

        private static NpcManager m_Instance;

        public static NpcManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new NpcManager();
                    m_Instance.Initialize();
                }
                return m_Instance;
            }
        }

        public static void Dispose()
        {
            if (m_Instance != null) m_Instance.StopShowNpc();
            m_Instance = null;
        }

        private Dictionary<string, NpcDelegate> m_ManagedNpcs;
        private EB.Collections.Queue<NpcDelegate> m_NpcQueue;
        private GroupCellManager m_GroupCellManager;
        private string m_CurrentSceneID;
        private SceneLogic m_CurrentSceneLogic = null;
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
            return m_NpcQueue.Count <= 0;
        }

        private void Initialize()
        {
            m_ManagedNpcs = new Dictionary<string, NpcDelegate>();
            m_NpcQueue = new EB.Collections.Queue<NpcDelegate>();
            m_IsCanShow = false;
            m_GroupCellManager = new GroupCellManager();
        }

        public void Clear()
        {
            m_ManagedNpcs.Clear();
            m_NpcQueue.Clear();
            m_CurrentSceneLogic = null;
        }

        public void StopShowNpc()
        {
            m_IsCanShow = false;
            m_CurrentSceneLogic = null;
        }

        /// <summary>
        /// 解决 多人场景进战斗 后出来  前后两次的sceneid一样导致旧的managedplayers没有清理干净
        /// </summary>
        public void StopShowNpcForGoToCombat()
        {
            m_IsCanShow = false;
            Clear();
        }

        public void BeginShowNpc()
        {
            m_IsCanShow = true;
            m_CurrentSceneID = GetSceneId();
            ClearNotCurrentSceneNpc();
            m_CurrentSceneLogic = MainLandLogic.GetInstance();
        }

        public void RegisterNpc(string id, NpcDelegate npc)
        {
            if (m_ManagedNpcs.ContainsKey(id))
            {
                EB.Debug.LogError("Npc already have " + id);
                return;
            }
            else
            {
                npc.preloaded = true;
                npc.created = true;
                m_ManagedNpcs.Add(id, npc);
            }
        }

        public bool IsNpcManaged(string id)
        {
            if (m_ManagedNpcs.ContainsKey(id)) return true;
            else return false;
        }

        public void ProcessNpcUpdateSync(object incomingData)
        {
            string scenetype = EB.Dot.String("scenetype", incomingData, "");
            IDictionary npcs = EB.Dot.Object(scenetype + ".npc_list", incomingData, null);
            if (npcs == null) return;
            SceneLogic scenelogic = MainLandLogic.GetInstance();
            if (scenelogic == null) return;
            foreach (DictionaryEntry entry in npcs)
            {
                bool fighting = EB.Dot.Bool("busy", entry.Value, false);
                EnemyController ec = scenelogic.GetEnemyController(entry.Key.ToString());
                if (ec != null)
                {
                    //if(!( ec is BoxController))
                    //{
                    //    ec.SetBarHudState(eHeadBarHud.FightStateHud, null, fighting);
                    //}
                }
                else
                {
                    NpcDelegate npc = null;
                    m_ManagedNpcs.TryGetValue(entry.Key.ToString(), out npc);
                    if (npc != null) npc.isfighting = fighting;
                }
            }
        }

        public void ProcessNpcLeaveSync(object incomingData)
        {
            string scenetype = EB.Dot.String("scenetype", incomingData, "");
            IDictionary npcs = EB.Dot.Object(scenetype + ".npc_list", incomingData, null);
            if (npcs == null) return;
            SceneLogic scenelogic = MainLandLogic.GetInstance();
            if (scenelogic == null) return;
            foreach (DictionaryEntry entry in npcs)
            {
                NpcLeave(entry.Key.ToString(), scenelogic);
                m_GroupCellManager.ObjectLeave(entry.Key.ToString());
            }
        }

        public void NpcLeave(string locator, SceneLogic currentlogic)
        {
            m_ManagedNpcs.Remove(locator);
            if (currentlogic != null) currentlogic.DeleteNpc(locator);
            m_GroupCellManager.ObjectLeave(locator);

            //如果是世界boss，删除之后需要立马生成下一个
            if (locator.Equals("EnemySpawns_11"))
            {
                long id = 0;
                string scenetype = SceneLogicManager.getSceneType();
                DataLookupsCache.Instance.SearchDataByID(scenetype + ".id", out id);

                string curLayout = "Layout50102";
                DataLookupsCache.Instance.SearchDataByID("mainlands.lastWeekBossLayoutId", out curLayout);

                Hashtable data = Johny.HashtablePool.Claim();
                data["role"] = "w_boss";
                data["sid"] = id;
                data["bossLayoutId"] = curLayout;

                NpcCommIn("EnemySpawns_11", data, MainLandLogic.GetInstance().CurrentSceneName, "mainlands");
            }
        }

        public void ProcessNpcCommInSync(object incomingData)
        {
            string scenetype = EB.Dot.String("scenetype", incomingData, "");
            IDictionary npcs = EB.Dot.Object(scenetype + ".npc_list", incomingData, null);
            SceneLogic scenelogic = MainLandLogic.GetInstance();
            if (scenelogic == null) return;
            if (npcs == null) return;
            foreach (DictionaryEntry entry in npcs)
            {
                if (entry.Value == null)//Player leave
                {
                    continue;
                }
                string locator = entry.Key.ToString();
                if (m_ManagedNpcs.ContainsKey(locator))
                {

                    continue;
                }
                IDictionary datatmp = entry.Value as IDictionary;
                NpcCommIn(locator, datatmp, scenelogic.CurrentSceneName, scenetype);
            }
        }

        void NpcCommIn(string locator, IDictionary data, string scenename, string scenetype)
        {
            Vector3 v3 = Vector3.zero;
            if (data["pos"] != null)
            {
                v3 = GM.LitJsonExtension.ImportVector3(data["pos"].ToString());
            }
            Quaternion rot = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
            long sceneid = EB.Dot.Long("sid", data, 0);
            string role = EB.Dot.String("role", data, "");

            string curLayout = EB.Dot.String("bossLayoutId", data, string.Empty);
            if (string.IsNullOrEmpty(curLayout))
            {
                if (!DataLookupsCache.Instance.SearchDataByID("mainlands.lastWeekBossLayoutId", out curLayout))
                {
                    curLayout = "Layout50103";
                }
            }

            string modelname = Hotfix_LT.Data.SceneTemplateManager.GetNPCModel(scenename, role, locator, curLayout);

            bool fighting = EB.Dot.Bool("busy", data, false);
            string strAttr = EB.Dot.String("tag_attribute", data, string.Empty);
            int attr = 0;
            if (!strAttr.Equals(string.Empty))
            {
                attr = int.Parse(strAttr);
            }
            NpcDelegate npc = new NpcDelegate(locator, v3, rot, scenetype + sceneid, modelname, role, fighting, attr);
            if (role != "func")
            {
                NpcCommIn(npc);
            }
        }

        public void NpcCommIn(NpcDelegate npc)
        {
            if (!m_ManagedNpcs.ContainsKey(npc.locator))
            {
                m_ManagedNpcs.Add(npc.locator, npc);
                m_NpcQueue.Enqueue(npc);
            }
        }

        public void ShowForGroupCell(string locator, bool state)
        {
            if (state)
            {
                if (m_ManagedNpcs.ContainsKey(locator))
                {
                    m_NpcQueue.Enqueue(m_ManagedNpcs[locator]);
                }
            }
            else
            {
                if (m_ManagedNpcs.ContainsKey(locator))
                {
                    m_ManagedNpcs[locator].preloaded = false;
                    m_ManagedNpcs[locator].created = false;
                    if (m_CurrentSceneLogic != null) m_CurrentSceneLogic.DeleteNpc(locator);
                }
            }
        }

        void Update()
        {
            FreshQueue();
            if (!m_IsCanShow) return;
            if (!GM.AssetManager.IsIdle) return;
            if (m_NpcQueue != null && m_NpcQueue.Count > 0)
            {
                NpcDelegate npcdata = m_NpcQueue.Dequeue();
                if (npcdata != null)
                {
                    if (npcdata.sceneid.Equals(m_CurrentSceneID) && m_ManagedNpcs != null && m_ManagedNpcs.ContainsKey(npcdata.locator))
                    {
                        Vector3 v3;
                        Quaternion rot;
                        switch (npcdata.role)
                        {
                            case NPC_ROLE.GHOST:
                                v3 = npcdata.pos;
                                rot = npcdata.quater;
                                break;
                            default:
                                GameObject locator_obj = LocatorManager.Instance.GetLocator(npcdata.locator);
                                if (locator_obj == null)
                                {
                                    EB.Debug.LogWarning(string.Format("No available locator data for npc  at locator [{0}]", npcdata.locator));
                                    return;
                                }
                                v3 = locator_obj.transform.position;
                                rot = locator_obj.transform.rotation;
                                break;
                        }

                        if (npcdata.key != null)
                        {
                            //夺宝奇兵等级未开放或该区服未开放判断
                            if (npcdata.role == NPC_ROLE.GHOST &&
                                (
                                !Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10073).IsConditionOK() ||
                                !Hotfix_LT.Data.EventTemplateManager.Instance.GetRealmIsOpen("main_land_ghost_start")))
                            {
                                EB.Debug.LogWarning("NPC_ROLE.GHOST Return! (10073).IsConditionOK()——{0}", Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10073).IsConditionOK() + ";GetRealmIsOpen——" + Hotfix_LT.Data.EventTemplateManager.Instance.GetRealmIsOpen("main_land_ghost_start"));
                                return;
                            }
                            else if (m_GroupCellManager != null && m_GroupCellManager.IsCellShowing(npcdata.key))
                            {
                                if (!npcdata.preloaded)
                                {
                                    if (m_CurrentSceneLogic != null)
                                    {
                                        m_CurrentSceneLogic.PreloadAsync(npcdata.modelname, (success) =>
                                        {
                                            npcdata.preloaded = success;
                                            m_NpcQueue.Enqueue(npcdata);
                                        });
                                    }
                                    else
                                    {
                                        EB.Debug.LogError("CurrentSceneLogic is Null");
                                    }
                                }
                                else if (m_ManagedNpcs != null && !m_ManagedNpcs[npcdata.locator].created)
                                {
                                    m_ManagedNpcs[npcdata.locator].created = true;
                                    if (m_CurrentSceneLogic != null)
                                    {
                                        #region 异步加载一个NPC
                                        m_CurrentSceneLogic.SpawnNpcAsync(npcdata.locator, npcdata.modelname, npcdata.role, v3, rot, ec =>
                                        {
                                            if (ec != null)
                                            {
                                                Player.EnemyHotfixController ehc = ec.transform.GetMonoILRComponent<Player.EnemyHotfixController>();
                                                ehc.SetBarHudState(eHeadBarHud.FightStateHud, null, m_ManagedNpcs[npcdata.locator].isfighting);
                                                ehc.Role = npcdata.role;
                                                ehc.Attr = npcdata.attr;
                                                ehc.SetNpcName(m_CurrentSceneLogic.CurrentSceneName);
                                            }
                                        });
                                        #endregion
                                    }
                                    else
                                    {
                                        EB.Debug.LogError("CurrentSceneLogic is Null");
                                    }
                                }
                            }
                        }
                        else if (m_GroupCellManager != null)
                        {
                            GroupCellKey key = m_GroupCellManager.ObjectIn(npcdata.locator, v3);
                            npcdata.key = key;
                            if (m_GroupCellManager.IsCellShowing(npcdata.key))
                            {
                                m_NpcQueue.Enqueue(npcdata);
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }

            if (m_GroupCellManager != null)
            {
                var go = PlayerManager.LocalPlayerGameObject();
                m_GroupCellManager.Update(go != null ? go.transform : null);
            }
        }

        /// <summary>
        /// 清理queue 以免占用太多内存 在战斗回多人场景时
        /// </summary>
        void FreshQueue()
        {
            if (m_NpcQueue.Count <= 100) return;
            List<NpcDelegate> list = new List<NpcDelegate>(64);
            while (m_NpcQueue.Count > 0)
            {
                NpcDelegate npcdata = m_NpcQueue.Dequeue();
                if (m_ManagedNpcs.ContainsKey(npcdata.locator))
                {
                    list.Add(npcdata);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                m_NpcQueue.Enqueue(list[i]);
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

        /// <summary>
        /// is slow function  use less
        /// </summary>
        public void ClearNotCurrentSceneNpc()
        {
            List<string> npc_to_remove = new List<string>();
            foreach (KeyValuePair<string, NpcDelegate> entry in m_ManagedNpcs)
            {
                NpcDelegate npc = entry.Value;
                if (!npc.sceneid.Equals(m_CurrentSceneID)) npc_to_remove.Add(npc.locator);
            }
            for (int i = 0; i < npc_to_remove.Count; i++)
            {
                if (m_ManagedNpcs.ContainsKey(npc_to_remove[i])) m_ManagedNpcs.Remove(npc_to_remove[i]);
            }

            m_GroupCellManager.Clear();
        }

        public void MergeNpc(string scenetype, IDictionary scenedata)
        {
            if (scenedata == null)
            {
                EB.Debug.LogWarning("MergeNpc SceneData is null");
                return;
            }
            IDictionary npcdata = EB.Dot.Object("npc_list", scenedata, null);
            if (npcdata == null)
            {
                EB.Debug.LogWarning("MergeNpc npc_list is null");
                return;
            }
            if (m_ManagedNpcs == null) return;
            //m_ManagedNpcs have scenedata didnt   must be clear
            SceneLogic scenelogic = MainLandLogic.GetInstance();
            if (scenelogic == null) return;
            List<string> npc_to_remove = new List<string>();
            foreach (KeyValuePair<string, NpcDelegate> kv in m_ManagedNpcs)
            {
                if (!npcdata.Contains(kv.Key))
                {
                    npc_to_remove.Add(kv.Key.ToString());
                }
            }
            for (int i = 0; i < npc_to_remove.Count; i++)
            {
                NpcLeave(npc_to_remove[i], scenelogic);
            }
            //scenedata have m_ManagedNpcs didnt   must be clear
            long sceneid = EB.Dot.Long("id", scenedata, 0);
            foreach (DictionaryEntry de in npcdata)
            {
                string locator = de.Key.ToString();
                if (!m_ManagedNpcs.ContainsKey(locator))
                {
                    if (!(de.Value is IDictionary)) continue;
                    IDictionary datatmp = de.Value as IDictionary;
                    datatmp.Add("sid", sceneid);
                    NpcCommIn(locator, datatmp, scenelogic.CurrentSceneName, scenetype);
                }
            }
        }
    }

    public class GroupCellKey
    {
        private int m_X;
        private int m_Z;
        public GroupCellKey(int x, int z)
        {
            m_X = x;
            m_Z = z;
        }

        public int X
        {
            get { return m_X; }
        }

        public int Z
        {
            get { return m_Z; }
        }


        public string FormatCellKey()
        {
            return string.Format("{0},{1}", m_X, m_Z);
        }

        public bool IsShowing(GroupCellKey Center)
        {
            //return true;
            if (Center == null) return false;
            if (m_X <= Center.X + 1 && m_X >= Center.X - 1 && m_Z <= Center.Z + 1 && m_Z >= Center.Z - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    public class GroupCell
    {
        private List<string> m_Objects;
        private GroupCellKey m_Key;
        public GroupCellKey Key
        {
            get { return m_Key; }
        }

        private bool m_Showing;
        public bool Showing
        {
            get { return m_Showing; }
            set { m_Showing = value; }
        }

        public GroupCell(GroupCellKey Key)
        {
            m_Key = Key;
            m_Showing = false;
            m_Objects = new List<string>();
        }

        public void ObjectIn(string id)
        {
            if (!m_Objects.Contains(id))
            {
                m_Objects.Add(id);
            }
        }

        public bool ObjectLeave(string id)
        {
            if (m_Objects.Remove(id))
            {
                return true;
            }
            return false;
        }

        public void Show(bool state)
        {
            if (m_Showing == state)
            {
                return;
            }
            else
            {
                for (int i = 0; i < m_Objects.Count; i++)
                {
                    NpcManager.Instance.ShowForGroupCell(m_Objects[i], state);
                }
                m_Showing = state;
            }
        }
    }

    public class GroupCellManager
    {
        private Dictionary<string, GroupCell> m_Cells;
        private float m_CellSize;
        private GroupCellKey m_ShowingCellKey;
        public GroupCellManager()
        {
            m_CellSize = 10;
            m_Cells = new Dictionary<string, GroupCell>();
            m_ShowingCellKey = null;
        }

        string FormatCellId(int x, int z)
        {
            return string.Format("{0},{1}", x, z);
        }

        /// <summary>
        /// 切换场景时清理
        /// </summary>
        public void Clear()
        {
            m_Cells.Clear();
            m_ShowingCellKey = null;
        }

        public GroupCellKey ObjectIn(string id, Vector3 pos)
        {
            int x = (int)(pos.x / m_CellSize);
            int z = (int)(pos.z / m_CellSize);
            GroupCellKey key = new GroupCellKey(x, z);
            string tmp = key.FormatCellKey();
            if (m_Cells.ContainsKey(tmp))
            {
                m_Cells[tmp].ObjectIn(id);
            }
            else
            {
                GroupCell cell = new GroupCell(key);
                m_Cells.Add(tmp, cell);
                cell.ObjectIn(id);
            }
            return key;
        }

        public GroupCellKey ObjectLeave(string id)
        {
            foreach (KeyValuePair<string, GroupCell> kv in m_Cells)
            {
                if (kv.Value.ObjectLeave(id))
                {
                    return kv.Value.Key;
                }
            }
            return null;
        }

        public bool IsCellShowing(GroupCellKey key)
        {
            if (key == null) return false;
            if (m_ShowingCellKey == null)
            {
                return false;
            }
            else
            {
                return key.IsShowing(m_ShowingCellKey);
            }
        }

        public void Update(Transform player)
        {
            if (!GameEngine.Instance.IsTimeToRootScene)
            {
                return;
            }
            if (player == null) return;
            Vector3 pos = player.position;
            int x = (int)(pos.x / m_CellSize);
            int z = (int)(pos.z / m_CellSize);

            if (m_ShowingCellKey == null)//首次
            {
                m_ShowingCellKey = new GroupCellKey(x, z);
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = z - 1; j <= z + 1; j++)
                    {
                        ShowCell(string.Format("{0},{1}", i, j), true);
                    }
                }
            }
            else
            {
                if (m_ShowingCellKey.X != x || m_ShowingCellKey.Z != z)//变化
                {
                    int oldx = m_ShowingCellKey.X;
                    int oldz = m_ShowingCellKey.Z;
                    m_ShowingCellKey = new GroupCellKey(x, z);
                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        for (int j = z - 1; j <= z + 1; j++)
                        {
                            ShowCell(string.Format("{0},{1}", i, j), true);
                        }
                    }

                    for (int i = oldx - 1; i <= oldx + 1; i++)
                    {
                        for (int j = oldz - 1; j <= oldz + 1; j++)
                        {
                            if (i > x + 1 || i < x - 1 || j > z + 1 || j < z - 1)
                            {
                                ShowCell(string.Format("{0},{1}", i, j), false);
                            }
                        }
                    }
                }
            }
        }

        void ShowCell(string key, bool state)
        {
            if (m_Cells.ContainsKey(key))
            {
                m_Cells[key].Show(state);
            }
        }
    }
}