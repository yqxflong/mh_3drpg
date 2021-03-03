using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class WorldSceneStruct
    {
        public int index;
        public List<TriggerEntry> m_Triggers;
    }

    public class WorldPathNode
    {
        public string scene;
        public string locator;
        public WorldPathNode()
        {
            scene = null;
            locator = null;
        }
    }

    public class WorldMapPathFinder
    {
        private int[][] t;
        private WorldPathNode[][] map;
        private Dictionary<string, WorldSceneStruct> indexmap;

        public void Test()
        {
            List<WorldPathNode> path;
            if (GetPath("QingDiYuYuan", "HuoShan", out path))
            {
                string pathstr = "";
                for (int i = 0; i < path.Count; i++)
                {
                    pathstr += path[i].scene + ";";
                }
                EB.Debug.LogError(pathstr);
            }

            if (GetPath("BeiHuang", "QingCheng", out path))
            {
                string pathstr = "";
                for (int i = 0; i < path.Count; i++)
                {
                    pathstr += path[i].scene + ";";
                }
                EB.Debug.LogError(pathstr);
            }
        }

        public bool GetPath(string s, string e, out List<WorldPathNode> path)
        {
            path = null;
            if (!indexmap.ContainsKey(s) || !indexmap.ContainsKey(e))
            {
                return false;
            }

            int si = indexmap[s].index;
            int ei = indexmap[e].index;
            List<int> index_path;
            if (!shortpan(t, si, ei, out index_path))
            {
                return false;
            }
            else
            {
                path = new List<WorldPathNode>(index_path.Count - 1);
                for (int i = 1; i < index_path.Count; i++)
                {
                    path.Add(map[index_path[i - 1]][index_path[i]]);
                }
                return true;
            }
        }

        public void InitMap(Dictionary<string, MainLandTemplate> MainLandTbl)
        {
            t = new int[MainLandTbl.Count][];
            for(int i = 0; i < t.Length; i++){
                t[i] = new int[MainLandTbl.Count];
            }

            map = new WorldPathNode[MainLandTbl.Count][];
            for(int i = 0; i < map.Length; i++){
                map[i] = new WorldPathNode[MainLandTbl.Count];
            }

            indexmap=new Dictionary<string, WorldSceneStruct>();
            List<MainLandTemplate> mainlands = new List<MainLandTemplate>(MainLandTbl.Values);
            for(int i=0; i<mainlands.Count;i++)
            {
                WorldSceneStruct wss = new WorldSceneStruct();
        wss.index = i;
                wss.m_Triggers= SceneTemplateManager.GetSceneTransferPoints(mainlands[i].mainland_name, "mainlands");
                indexmap.Add(mainlands[i].mainland_name, wss);
            }
            for (int i = 0; i<mainlands.Count; i++)
            {
                List<TriggerEntry> triggers = indexmap[mainlands[i].mainland_name].m_Triggers;
                if (triggers == null)
                {
                    EB.Debug.LogWarning("WorldMapPathFinder.InitMap: triggers = null!");
                    continue;
                }
                for(int j=0;j<triggers.Count; j++)
                {
                    TransferTriggerEntry tte = triggers[j] as TransferTriggerEntry;
                    int index = indexmap[tte.name].index;
                    t[i][index] = 1;
                    WorldPathNode wpn = new WorldPathNode();
                    wpn.scene = mainlands[i].mainland_name;
                    wpn.locator = tte.locator;
                    map[i][index] = wpn;
                }
            }
        }

        /// <summary>
        /// 最小值
        /// </summary>
        /// <param name="Q"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private int MIN(int[] Q, out int j)
        {
            int a = 10000;
            j = 0;
            for (int i = 0; i < Q.Length; i++)
            {
                if (a >= Q[i])
                {
                    a = Q[i];
                    j = i;
                }
            }
            return a;
        }

        /// <summary>
        /// 最短路径
        /// </summary>
        /// <param name="t"></param>      
        /// <param name="v"></param>
        /// <param name="d"></param>
        private bool shortpan(int[][] t, int v, int e, out List<int> path)
        {
            int[,] h;
            int[] Q;
            int[] p;
            int[] d;
            h = new int[t.GetLength(0), t.GetLength(0)];
            d = new int[t.GetLength(0)];
            Q = new int[t.GetLength(0)];
            p = new int[t.GetLength(0)];
            d[v] = 0;//源点到源点为0;
            int u = v;//记录移除的节点                    
            int max = 10000;
            //判断是否存在边，初始化Q
            for (int i = 0; i < t.GetLength(0); i++)
            {
                for (int j = 0; j < t.GetLength(0); j++)
                {
                    if (t[i][j] < max && t[i][j] != 0)
                    {
                        h[i, j] = 1;
                    }
                }
                Q[i] = max;
            }
            //更新最短路径
            for (int j = 1; j < d.Length; j++)
            {
                for (int i = 0; i < Q.Length; i++)
                {
                    if (h[u, i] == 1)
                    {
                        if (t[u][i] + d[u] <= Q[i])
                        {
                            Q[i] = t[u][i] + d[u];
                            p[i] = u;
                        }
                        else
                        {

                        }
                        h[i, u] = 2;
                    }
                }
                int r = MIN(Q, out u);
                d[u] = r;
                Q[u] = max;
            }
            int temp = e;
            if (p[temp] == 0)
            {
                EB.Debug.Log("No Path");
                path = null;
                return false;
            }
            path = new List<int>();
            path.Add(temp);
            while (p[temp] != v)
            {
                path.Add(p[temp]);
                temp = p[temp];
            }
            path.Add(v);
            path.Reverse();
            return true;
        }

        void TestData()
        {
            int[][] t = new int[5][];
            t[0][1] = 10;
            t[0][2] = 20;
            t[0][3] = 30;
            t[0][4] = 10000;
            t[1][2] = 5;
            t[1][3] = 10;
            t[1][4] = 10000;
            t[2][3] = 10000;
            t[2][4] = 30;
            t[3][4] = 20;
            for (int i = 0; i < 5; i++)
            {
                //t[i][i] = 0;                      
                for (int j = 0; j < 5; j++)
                {
                    t[j][i] = t[i][j];
                }
            }
            List<int> path;
            string p = "";
            if (shortpan(t, 2, 3, out path))
            {
                for (int i = 0; i < path.Count; i++)
                {
                    p += path[i] + ",";
                }
                EB.Debug.Log(p);
            }
        }
    }
}
