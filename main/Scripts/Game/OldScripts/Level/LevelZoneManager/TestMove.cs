using BE.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BE.Level
{
    /// <summary>
    /// 测试移动
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class TestMove : MonoBehaviour
    {
        private LevelZone[] m_AllZone;
        [EnumFlags]
        public enZoneType v_Type;

        void Awake()
        {
            m_AllZone = FindObjectsOfType<LevelZone>();
        }

        private void Update()
        {
            m_AllZone = FindObjectsOfType<LevelZone>();
            v_Type = (enZoneType)0;
            for (int i=0;i<m_AllZone.Length;i++)
            {
                if (m_AllZone[i].F_IsInRound(this.transform.position))
                {
                    v_Type |= m_AllZone[i].v_ZoneType;
                }
            }
        }
    }
}
