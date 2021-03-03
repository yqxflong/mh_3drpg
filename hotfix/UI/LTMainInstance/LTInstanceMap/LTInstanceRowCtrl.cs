//优化ByJohny
//预加载Wall和Floor

using GM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    /// <summary>
    /// 地图单行控制(包含大地图和小地图)
    /// </summary>
    public class LTInstanceRowCtrl : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();
            Floor = (GameObject)mDMono.ObjectParamList[0];
            Wall = (GameObject)mDMono.ObjectParamList[1];
            IsSmallMap = mDMono.BoolParamList[0];
        }
        
        #region For Preload
        //private const int CACHE_FLOOR_CNT = 0;
        //private const int CACHE_WALL_CNT = 0;
        //private static Stack<GameObject> st_CacheWall = new Stack<GameObject>(CACHE_WALL_CNT);
        //private static Dictionary<GameObject, LTInstanceNodeTemp> st_CacheWall_Comp = new Dictionary<GameObject, LTInstanceNodeTemp>(CACHE_WALL_CNT);
        //private static Stack<GameObject> st_CacheFloor = new Stack<GameObject>(CACHE_FLOOR_CNT);
        //private static Dictionary<GameObject, LTInstanceNodeTemp> st_CacheFloor_Comp = new Dictionary<GameObject, LTInstanceNodeTemp>(CACHE_FLOOR_CNT);

        //public void DoPreload(){
        //    if (IsSmallMap)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        if (st_CacheWall.Count > 0|| st_CacheFloor.Count > 0)
        //        {
        //            EB.Debug.LogError("Cache没有回收！！！！非常严重！！！！");
        //            return;
        //        }

        //        for (int i = 0; i < CACHE_WALL_CNT; i++)
        //        {
        //            var go = GameObject.Instantiate(Wall, mDMono.transform);
        //            st_CacheWall.Push(go);
        //            go.CustomSetActive(true);
        //            st_CacheWall_Comp.Add(go, go.GetMonoILRComponent<LTInstanceNodeTemp>());
        //            go.CustomSetActive(false);
        //        }

        //        for (int i = 0; i < CACHE_FLOOR_CNT; i++)
        //        {
        //            var go = GameObject.Instantiate(Floor, mDMono.transform);
        //            st_CacheFloor.Push(go);
        //            go.CustomSetActive(true);
        //            st_CacheFloor_Comp.Add(go, go.GetMonoILRComponent<LTInstanceNodeTemp>());
        //            go.CustomSetActive(false);
        //        }
        //    }


        //}

        //public static void DoClearPool(){
        //    #region clear Wall
        //    {
        //        var it = st_CacheWall.GetEnumerator();
        //        while(it.MoveNext()){
        //            GameObject.Destroy(it.Current);
        //        }
        //        st_CacheWall.Clear();
        //        st_CacheWall_Comp.Clear();
        //    }
        //    #endregion

        //    #region clear Floor
        //    {
        //        var it = st_CacheFloor.GetEnumerator();
        //        while(it.MoveNext()){
        //            GameObject.Destroy(it.Current);
        //        }
        //        st_CacheFloor.Clear();
        //        st_CacheFloor_Comp.Clear();
        //    }
        //    #endregion
        //}

        //public void GetOneWall(Stack<GameObject> wallStack,  Dictionary<GameObject, LTInstanceNodeTemp> compDic, out GameObject go, out LTInstanceNodeTemp comp)
        //{
        //    if (wallStack.Count > 0)
        //    {
        //        go = wallStack.Pop();
        //        go.transform.parent = mDMono.transform;
        //        comp = compDic[go];
        //    }
        //    else
        //    {
        //        go = GameObject.Instantiate(Wall, mDMono.transform);
        //        go.CustomSetActive(true);
        //        comp = go.GetMonoILRComponent<LTInstanceNodeTemp>();
        //    }
        //}

        //public void GetOneFloor(Stack<GameObject> FloorStack,  Dictionary<GameObject, LTInstanceNodeTemp> compDic, out GameObject go, out LTInstanceNodeTemp comp)
        //{
        //    if (FloorStack.Count > 0)
        //    {
        //        go = FloorStack.Pop();
        //        go.transform.parent = mDMono.transform;
        //        comp = compDic[go];
        //    }
        //    else
        //    {
        //        go = GameObject.Instantiate(Floor, mDMono.transform);
        //        go.CustomSetActive(true);
        //        comp = go.GetMonoILRComponent<LTInstanceNodeTemp>();
        //    }
        //}

        //public void BackToPool(GameObject go){
        //    go.CustomSetActive(false);
        //    if (IsSmallMap)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        if (st_CacheWall_Comp.ContainsKey(go))
        //        {
        //            st_CacheWall.Push(go);
        //        }
        //        else if (st_CacheFloor_Comp.ContainsKey(go))
        //        {
        //            st_CacheFloor.Push(go);
        //        }
        //    }
        //}

        //public void AllBackToPool()
        //{
        //    foreach (var item in itemObjDic)
        //    {
        //        GameObject go = item.Value.mDMono.gameObject;
        //        go.CustomSetActive(false);
        //        if (IsSmallMap)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            if (st_CacheWall_Comp.ContainsKey(go))
        //            {
        //                st_CacheWall.Push(go);
        //            }
        //            else if (st_CacheFloor_Comp.ContainsKey(go))
        //            {
        //                st_CacheFloor.Push(go);
        //            }
        //        }
        //    }
        //    itemObjDic.Clear();
        //}
        #endregion

        public GameObject Floor;
    
        public GameObject Wall;
    
        public int Num;
    
        public Dictionary<int, LTInstanceNodeTemp> itemObjDic = new Dictionary<int, LTInstanceNodeTemp>();
    
        public LTInstanceMapCtrl MapCtrl;
    
        public bool IsSmallMap = false;
    
        public void CreateNodeFromCache(LTInstanceNode data)
        {
            if(data == null){
                EB.Debug.LogError("CreateNodeFromCache===>data is null!");
                return;
            }

            GameObject obj = null;
            LTInstanceNodeTemp temp = null;
            if(IsSmallMap){
                if (data.Type == LTInstanceNode.NodeType.WALL)
                {
                    obj = GameObject.Instantiate(Wall, mDMono.transform);
                }
                else if (data.Type == LTInstanceNode.NodeType.Floor)
                {
                    obj = GameObject.Instantiate(Floor, mDMono.transform);
                }
                temp = obj.GetMonoILRComponent<LTInstanceNodeTemp>();
            }
            else
            {
                if (data.Type == LTInstanceNode.NodeType.WALL)
                {
                    //GetOneWall(st_CacheWall, st_CacheWall_Comp, out obj, out temp);
                    obj = GameObject.Instantiate(Wall, mDMono.transform);
                    obj.CustomSetActive(true);
                    temp = obj.GetMonoILRComponent<LTInstanceNodeTemp>();
                }
                else if (data.Type == LTInstanceNode.NodeType.Floor)
                {
                    //GetOneFloor(st_CacheFloor, st_CacheFloor_Comp, out obj, out temp);
                    obj = GameObject.Instantiate(Floor, mDMono.transform);
                    obj.CustomSetActive(true);
                    temp = obj.GetMonoILRComponent<LTInstanceNodeTemp>();
                }
            }

            obj.CustomSetActive(true);
            if (IsSmallMap)
            {
                obj.transform.localPosition = new Vector3(data.x * LTInstanceConfig.SMALL_MAP_SCALE_X, data.x * LTInstanceConfig.SMALL_MAP_SCALE_Y, 0);
            }
            else
            {
                obj.transform.localPosition = new Vector3(data.x * LTInstanceConfig.MAP_X, data.x * LTInstanceConfig.MAP_Y, data.x * LTInstanceConfig.MAP_XZ);
            }
            obj.transform.localScale = Vector3.one;
            obj.name = data.x.ToString();
            //----
            temp.MapCtrl = MapCtrl;
            temp.SetData(data.x, data);
            itemObjDic[data.x] = temp;
        }
    }
}
