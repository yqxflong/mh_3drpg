using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.MainLand
{
    public class HeadBarHUDMonitor : MonoBehaviour
    {
        public Vector2 m_Offset;
        public bool m_Limit;
        public bool m_Scale;
        public virtual void CleanPosition(Vector3 pos, Vector3 scale)
        {
            if (transform == null || gameObject == null || !gameObject.activeSelf)
            {
                return;
            }
            Vector3 pos_target = pos;
            pos_target.x += m_Offset.x;
            pos_target.y += m_Offset.y;
            transform.localPosition = pos_target;
            transform.localScale = scale;
        }

        public void UpdatePosition(Vector3 screen_pos, Vector3 camera_pos, Vector3 pos)
        {
            if (m_Limit || gameObject == null || !gameObject.activeSelf)
            {
                return;
            }
            Vector3 pos_target = screen_pos;
            pos_target.x += m_Offset.x;
            pos_target.y += m_Offset.y;
            transform.localPosition = pos_target;

            if (m_Scale)
            {
                float dis = Vector3.Distance(pos, camera_pos);

                Vector3 scale = Vector3.one * (15 / dis);
                transform.localScale = scale;
            }
        }
    }
}
