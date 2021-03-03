using System;
using System.Collections.Generic;
using UnityEngine;

namespace BE.Level
{
    /// <summary>
    /// 场景区域管理器(它挂载主城s001a的预设上)
    /// IdleFlowAction.OnEnter时加载
    /// </summary>
    public class LevelZoneManager : MonoBehaviour
    {
        /// <summary>
        /// 所有的区域控件
        /// </summary>
        private LevelZone[] m_AllZone;

        /// <summary>
        /// 当前玩家所属区域
        /// </summary>
        private enZoneType m_CurrentZone;

        /// <summary>
        /// 当前显示的区域信息
        /// </summary>
        private List<LevelInfo> m_StackObj;

        /// <summary>
        /// 所有的当时贴图数据
        /// </summary>
        private Dictionary<enZoneType, LightmapData> m_LightmapDataDic;

        /// <summary>
        /// 临时协程
        /// </summary>
        private Coroutine m_Coroutine;

        /// <summary>
        /// 场景信息
        /// </summary>
        private class LevelInfo
        {
            /// <summary>
            /// 区域预置体
            /// </summary>
            public GameObject v_LevelObj;
            /// <summary>
            /// 预置体资源
            /// </summary>
            public UnityEngine.Object v_LevelAsset;
            /// <summary>
            /// 区域类型
            /// </summary>
            public enZoneType v_ZoneType;
            /// <summary>
            /// 引用的光照索引
            /// </summary>
            public int v_LightmapIndex;
        }

        /// <summary>
        /// 当时贴图相应对应的过些
        /// </summary>
        private Dictionary<enZoneType, string> m_LightmapStr;

        private bool _isEnterMainView = true;

        private void Awake()
        {
            m_AllZone = this.GetComponentsInChildren<LevelZone>(true);
            m_StackObj = new List<LevelInfo>();
            m_LightmapDataDic = new Dictionary<enZoneType, LightmapData>();
            m_CurrentZone = (enZoneType)0;

            m_LightmapStr = new Dictionary<enZoneType, string>();
            m_LightmapStr.Add(enZoneType.Zone_0, "ZC");
            m_LightmapStr.Add(enZoneType.Zone_1, "ZC_1");
            m_LightmapStr.Add(enZoneType.Zone_2, "ZC_2");
            m_LightmapStr.Add(enZoneType.Zone_3, "ZC_3");
            m_LightmapStr.Add(enZoneType.Zone_4, "ZC_4");
            m_LightmapStr.Add(enZoneType.Zone_5, "ZC_a");
            m_LightmapStr.Add(enZoneType.Zone_6, "ZC_b");
            m_LightmapStr.Add(enZoneType.Zone_7, "ZC_c");
            m_LightmapStr.Add(enZoneType.Zone_8, "ZC_d");
            m_LightmapStr.Add(enZoneType.Zone_9, "ZC_e");
            m_LightmapStr.Add(enZoneType.Zone_10, "ZC_f");
            m_LightmapStr.Add(enZoneType.Zone_11, "ZC_g");
            m_LightmapStr.Add(enZoneType.Zone_12, "diaoxiang");
        }

        /// <summary>
        /// 通过坐标获取当前所属的区域类型
        /// </summary>
        /// <param name="pos">坐标</param>
        /// <returns></returns>
        private enZoneType GetCurrentTypeByPos(Vector3 pos)
        {
            enZoneType type = (enZoneType)0;

            for (int i = 0; i < m_AllZone.Length; i++)
            {
                if (m_AllZone[i].F_IsInRound(pos))
                {
                    type |= m_AllZone[i].v_ZoneType;
                }
            }

            return type;
        }

        private void Update()
        {
            UpdateLevel(false);
        }

        private void OnEnable()
        {
            _isEnterMainView = true;

            //解决从战斗回主城光照丢失造成变黑问题
            if (m_StackObj != null && m_StackObj.Count > 0)
            {
                SetLightMap(m_StackObj);
            }
        }

        /// <summary>
        /// 刷新区域
        /// </summary>
        /// <param name="isForceUpdate"></param>
        private void UpdateLevel(bool isForceUpdate)
        {
            if (PlayerManager.LocalPlayerController() != null)
            {
                enZoneType zoneType = GetCurrentTypeByPos(PlayerManager.LocalPlayerController().transform.position);

                if (m_CurrentZone != zoneType || isForceUpdate)
                {
                    LoadLevelZone(zoneType);
                    m_CurrentZone = zoneType;

                    if (zoneType == (enZoneType)0)
                    {
                        EB.Debug.LogError("+++++++++为什么会存在0的区域情况呢?+++++++++++++");
                    }
                }              
            }
        }

        List<Coroutine> _coroutines = new List<Coroutine>();

        /// <summary>
        /// 加载相应的区域
        /// </summary>
        /// <param name="zone">区域类型</param>
        private void LoadLevelZone(enZoneType zone)
        {
            LoadingLogic.AddCustomProgress(10);
            for (var i = 0; i < _coroutines.Count; i++)
            {
                if (_coroutines[i] != null)
                {
                    StopCoroutine(_coroutines[i]);
                }
            }
            _coroutines.Clear();

            LevelInfo levelInfo = null;
            enZoneType tempType = (enZoneType)0;
            int lightmapIndex = 0;
            int count = 0;
            Dictionary<string, enZoneType> dict = new Dictionary<string,enZoneType>();

            for (int i = 0; i <= m_LightmapStr.Count; i++)
            {
                tempType = (enZoneType)(1 << i);

                if ((zone & tempType) > 0)
                {
                    levelInfo = m_StackObj.Find(p => p.v_ZoneType == tempType);

                    if (levelInfo == null)
                    {
                        dict.Add(m_LightmapStr[tempType], tempType);

                        var c1 = EB.Assets.LoadAsync(string.Format("_GameAssets/Res/Environment/s001a/MainLevelZone/Lightmap_{0}", m_LightmapStr[tempType]), typeof(Texture2D), (obj) => {
                            var lightmapData = new LightmapData();
                            lightmapData.lightmapColor = obj as Texture2D;
                            var key = dict[obj.name.Replace("Lightmap_", "")];

                            if (!m_LightmapDataDic.ContainsKey(key))
                            {
                                m_LightmapDataDic.Add(key, lightmapData);
                            }
                            else
                            {
                                m_LightmapDataDic[key] = lightmapData;
                            }

                            var c2 = EB.Assets.LoadAsync(string.Format("_GameAssets/Res/Environment/s001a/MainLevelZone/{0}", m_LightmapStr[key]), typeof(GameObject), (o) => {
                                var li = new LevelInfo();
                                li.v_LevelObj = Instantiate<GameObject>((GameObject)o);
                                li.v_ZoneType = dict[o.name];
                                li.v_LightmapIndex = lightmapIndex;
                                m_StackObj.Add(li);
                                count++;
                                lightmapIndex++;

                                if (count > m_LightmapStr.Count)
                                {
                                    ClearZone(zone);
                                    dict.Clear();
                                }
                            });
                            _coroutines.Add(c2);
                        });
                        _coroutines.Add(c1);
                    }
                    else
                    {
                        levelInfo.v_LightmapIndex = lightmapIndex;
                        count++;
                        lightmapIndex++;

                        if (count > m_LightmapStr.Count)
                        {
                            ClearZone(zone);
                            dict.Clear();
                        }
                    }
                }
                else
                {
                    count++;

                    if (count > m_LightmapStr.Count)
                    {
                        ClearZone(zone);
                        dict.Clear();
                    }
                }
            }  
        }

        private void ClearZone(enZoneType zone)
        {
            // 清除不在这个区域的数据与资源
            for (int i = m_StackObj.Count - 1; i >= 0; i--)
            {
                if ((m_StackObj[i].v_ZoneType & zone) == 0)
                {
                    Destroy(m_StackObj[i].v_LevelObj);
                    m_StackObj.Remove(m_StackObj[i]);
                }
            }

            SetLightMap(m_StackObj);
        }

        /// <summary>
        /// 设置相应的场景区域预置体
        /// </summary>
        /// <param name="levelInfo">区域信息</param>
        private void SetLevelZoneObj(LevelInfo levelInfo)
        {
            GameObject tempObj = levelInfo.v_LevelObj;
            tempObj.transform.parent = this.transform;
            tempObj.transform.localPosition = Vector3.zero;
            tempObj.transform.localScale = Vector3.one;
            tempObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            tempObj.SetActive(true);

            //设置的光照贴图索引
            LightMapDataComponent[] allDataComponenes = tempObj.GetComponentsInChildren<LightMapDataComponent>(true);

            for (int i = 0; i < allDataComponenes.Length; i++)
            {
                allDataComponenes[i].lightmapIndex = levelInfo.v_LightmapIndex;
                allDataComponenes[i].RestoreLightMapData();
            }
        }

        /// <summary>
        /// 设置相应区域的光照贴图
        /// </summary>
        /// <param name="zoneType">区域类型</param>
        private void SetLightMap(List<LevelInfo> levelInfos)
        {
            //排序
            levelInfos.Sort((x, y) => x.v_LightmapIndex.CompareTo(y.v_LightmapIndex));

            //设置当前需要的区域
            var len = levelInfos.Count;
            LightmapData[] lightmapDatas = new LightmapData[len];

            for (int i = 0; i < len; i++)
            {
                var levelInfo = levelInfos[i];

                SetLevelZoneObj(levelInfo);

                if (m_LightmapDataDic.ContainsKey(levelInfo.v_ZoneType))
                {
                    lightmapDatas[levelInfo.v_LightmapIndex] = m_LightmapDataDic[levelInfo.v_ZoneType];
                }
            }

            LightmapSettings.lightmaps = lightmapDatas;
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;

            ClearLightMap(levelInfos);
        }

        private void ClearLightMap(List<LevelInfo> levelInfos)
        {
            LoadingLogic.AddCustomProgress(10);
            //将之前还在内存里的释放掉
            List<enZoneType> clearList = new List<enZoneType>();

            foreach (enZoneType zoneType in m_LightmapDataDic.Keys)
            {
                LevelInfo info = levelInfos.Find(p => p.v_ZoneType == zoneType);

                if (info == null)
                {
                    clearList.Add(zoneType);
                }
            }

            for (int i = 0; i < clearList.Count; i++)
            {
                Resources.UnloadAsset(m_LightmapDataDic[clearList[i]].lightmapColor);
                m_LightmapDataDic.Remove(clearList[i]);
            }

            #region 登陆游戏在主城时，此处结束loading
            if (_isEnterMainView)
            {
                _isEnterMainView = false;
                UIStack.Instance.HideLoadingScreen();
            }
            #endregion
        }
    }
}
