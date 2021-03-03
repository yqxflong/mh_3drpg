using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;

namespace Hotfix_LT.UI
{
    public class PathProcessNode
    {
        public string scene;
        public string target_scene;
        public PathProcessNode(string scene, string target_scene)
        {
            this.scene = scene;
            this.target_scene = target_scene;
        }

        public virtual void ProcessNode()
        {
        }

        public void Process()
        {
            ProcessNode();
        }
    }

    public class PathProcessTransferNode : PathProcessNode
    {
        public string transfer_scene;

        public PathProcessTransferNode(string scene, string target_scene, string transfer_scene) : base(scene, target_scene)
        {
            this.transfer_scene = transfer_scene;
        }

        public override void ProcessNode()
        {
            UIStack.Instance.ExitStack(true);
            //传送到目标地址
            Vector3 from_pos = new Vector3(25.0f, 0.0f, 45.0f);
            Vector3 to_pos = new Vector3(0.0f, 0.0f, 0.0f);//0，0，0 目标场景会自动设置为初始位置
            SceneLogic.Transfer(scene, from_pos, 90.0f, transfer_scene, to_pos, 90.0f);
        }
    }

    public class PathProcessToTransferPointNode : PathProcessNode
    {
        public string locator;
        public PathProcessToTransferPointNode(string scene, string target_scene, string locator) : base(scene, target_scene)
        {
            this.locator = locator;
        }

        public override void ProcessNode()
        {

            MainLandLogic.GetInstance().LocatorPathFinding(scene, locator);
        }
    }

    public class PathProcessToNpcNode : PathProcessNode
    {
        public string locator;
        public PathProcessToNpcNode(string scene, string target_scene, string locator) : base(scene, target_scene)
        {
            this.locator = locator;
        }

        public override void ProcessNode()
        {
           MainLandLogic.GetInstance().NpcPathFinding(scene,locator);
        }
    }

    public class WorldMapPathManager
    {
        private EB.Collections.Queue<PathProcessNode> m_Queue;
        private WorldMapPathFinder m_PathFinder;
        private PathProcessNode m_CurrentNode;
        public static WorldMapPathManager m_Instance;
        public static WorldMapPathManager Instance
        {
            get
            {
                if (m_Instance == null) m_Instance = new WorldMapPathManager();
                return m_Instance;
            }
        }

        private WorldMapPathManager()
        {
            m_Queue = new EB.Collections.Queue<PathProcessNode>();
            m_PathFinder = new WorldMapPathFinder();
            m_CurrentNode = null;
        }

        // public void InitPathFinder(Dictionary<string, Data.MainLandTemplate> MainLandTbl)
        // {
        //     m_PathFinder.InitMap(MainLandTbl);
        // }

        public void StartPathFindToNpc(string scene, string target_scene, string npcid)
        {
            ClearPath();
            if (!scene.Equals(target_scene))
            {
                List<WorldPathNode> path;
                if (m_PathFinder.GetPath(scene, target_scene, out path))
                {
                    for (int i = 0; i < path.Count; i++)
                    {
                        PathProcessToTransferPointNode node_tpn = new PathProcessToTransferPointNode(path[i].scene, target_scene, path[i].locator);
                        m_Queue.Enqueue(node_tpn);
                    }
                }
            }

            PathProcessToNpcNode node_p = new PathProcessToNpcNode(target_scene, target_scene, npcid);
            m_Queue.Enqueue(node_p);
            Process();
        }

        public void StartPathFindToNpcFly(string scene, string target_scene, string npcid)
        {
            ClearPath();
            if (!scene.Equals(target_scene))
            {
                PathProcessTransferNode node_t = new PathProcessTransferNode(scene, target_scene, target_scene);
                m_Queue.Enqueue(node_t);
            }

            PathProcessToNpcNode node_p = new PathProcessToNpcNode(target_scene, target_scene, npcid);
            m_Queue.Enqueue(node_p);
            Process();
        }

        public void Process()
        {
            if (m_Queue.Count > 0)
            {
                PathProcessNode ppn = m_Queue.Dequeue();
                m_CurrentNode = ppn;
                ppn.Process();
            }
        }

        public void StopPath()
        {
            ClearPath();
        }

        public void ClearPath()
        {
            if (m_Queue.Count > 0 || m_CurrentNode != null)
            {
                m_Queue.Clear();
                m_CurrentNode = null;
            }
        }

        /// <summary>
        /// 获取当前寻路节点的目标npc 如果是npc寻路节点（一般在传送完了调用）
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="npc"></param>
        /// <returns></returns>
        // public bool GetCurrentNodeTargetNPC(out string scene, out string npc)
        // {
        //     scene = null;
        //     npc = null;
        //     if (m_Queue.Count > 0)
        //     {
        //         PathProcessNode ppn = m_Queue.Peek();
        //         if (ppn is PathProcessToNpcNode)
        //         {
        //             PathProcessToNpcNode pptnn = ppn as PathProcessToNpcNode;
        //             scene = pptnn.scene;
        //             npc = pptnn.locator;
        //             return true;
        //         }
        //     }
        //     return false;
        // }

        /// <summary>
        /// 中途被拉入战斗 做保存
        /// </summary>
        public void SaveBeforePullIntoCombat()
        {
            if (m_CurrentNode != null) m_Queue.Insert(0, m_CurrentNode);
        }
    }
}