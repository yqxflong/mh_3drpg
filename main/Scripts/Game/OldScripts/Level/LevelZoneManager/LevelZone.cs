using System;
using System.Collections.Generic;
using UnityEngine;

namespace BE.Level
{
    /// <summary>
    /// 场景区域控件
    /// </summary>
    [ExecuteInEditMode]
    public class LevelZone : MonoBehaviour
    {
        /// <summary>
        /// 引用的光照贴图索引
        /// </summary>
        public int v_LightmapIndex;
        /// <summary>
        /// 当前的区域类型
        /// </summary>
        [SerializeField]
        public enZoneType v_ZoneType;
        /// <summary>
        /// 当前的所有区域坐标
        /// </summary>
        public Transform[] v_AllZonePos;
        /// <summary>
        /// 多边型坐标
        /// </summary>
        private Vector2[] m_AllPolygon;

#if UNITY_EDITOR
        private void Reset()
        {
            Awake();
        }
#endif
        private void Awake()
        {
            v_AllZonePos = new Transform[transform.childCount];
            for (int i=0;i<this.transform.childCount;i++)
            {
                v_AllZonePos[i] = this.transform.GetChild(i);
            }
            m_AllPolygon = new Vector2[v_AllZonePos.Length];
            for (int i = v_AllZonePos.Length - 1; i >=0; i--)
            {
                m_AllPolygon[i] = new Vector2(v_AllZonePos[i].position.x, v_AllZonePos[i].position.z);
            }
        }

        /// <summary>
        /// 是否属性这个坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool F_IsInRound(Vector3 pos)
        {
            return IsPointInPolygon(new Vector2(pos.x, pos.z), m_AllPolygon);
        }
        
        /// <summary>
        /// 是否在多边形内
        /// </summary>
        /// <param name="point">坐标点</param>
        /// <param name="polygon">多边形</param>
        /// <returns></returns>
        private bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
        {
            int polygonLength = polygon.Length, i = 0;
            bool inside = false;
            
            float pointX = point.x, pointY = point.y;
            
            float startX, startY, endX, endY;
            Vector2 endPoint = polygon[polygonLength - 1];
            endX = endPoint.x;
            endY = endPoint.y;

            while (i < polygonLength)
            {
                startX = endX;
                startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.x;
                endY = endPoint.y;
                inside ^= (endY > pointY ^ startY > pointY) && ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }
            return inside;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            for (int i=1;i< v_AllZonePos.Length;i++)
            {
                Gizmos.DrawLine(v_AllZonePos[i - 1].position, v_AllZonePos[i].position);
            }
        }
#endif
    }
}
