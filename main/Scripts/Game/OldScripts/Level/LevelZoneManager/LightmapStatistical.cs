using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//namespace BE.Level
//{
    /// <summary>
    /// 光照贴图统计
    /// </summary>
    [ExecuteInEditMode]
    public class LightmapStatistical : MonoBehaviour
    {
        private LightMapDataComponent[] m_AllRenderer;
        private HashSet<int> m_Statistical;

        private void Reset()
        {
            m_AllRenderer = transform.GetComponentsInChildren<LightMapDataComponent>();
            m_Statistical = new HashSet<int>();
            StringBuilder str = new StringBuilder();
            for (int i=0;i<m_AllRenderer.Length;i++)
            {
                if ( !m_Statistical.Contains(m_AllRenderer[i].lightmapIndex))
                {
                    m_Statistical.Add(m_AllRenderer[i].lightmapIndex);
                    str.AppendLine("<color=#00ff00>["+ m_Statistical.Count + "]m_AllRenderer[i].lightmapIndex:"+ m_AllRenderer[i].lightmapIndex + "</color>");
                }
            }
            str.AppendLine(this.name);
            EB.Debug.Log(str);
        }
    }
//}
