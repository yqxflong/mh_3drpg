//HealthBarHUDMonitor
//血条监控器,用于监控位置等信息
//Johny


using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

namespace Main.Combat.UI{
    public class HealthBarHUDMonitor : MonoBehaviour
    {
        private Camera m_camera = null;
        private Transform m_cameraTransform = null;
        private Transform m_locator = null;
        private Vector3 m_screenOffset = new Vector3(0, 20f, 0);
        private UIWidget[] healthBarWidgets = null;
        private string[] healthBarWidgetsName = null;
        private int healthBarLen = 0;

        private JobHandle mJobHandleUpdateHealthBarPosition;
        private JobUpdateHealthBarPosition mJobUpdateHealthBarPosition;

        public void SetLocator(Transform tran){
            m_locator = tran;
            m_camera = Camera.main;
            m_cameraTransform = m_camera != null ? m_camera.transform : null;
        }

        void Update()
        {
           UpdatePosition();
        }
    
        private void UpdatePosition()
        {
            if (m_locator == null)
            {
                return;
            }

            if (m_camera == null || !m_camera.enabled)
            {
                return;
            }

            Vector3 position = m_locator.position;
            Vector3 screen_point = m_camera.WorldToScreenPoint(position);
            Vector3 cameraTransformPosition = m_cameraTransform.position;

            mJobUpdateHealthBarPosition = new JobUpdateHealthBarPosition()
            {
                screen_point = screen_point,
                m_screenOffset = m_screenOffset
            };

            Transform[] transforms = new[] { this.transform };
            TransformAccessArray accessArray = new TransformAccessArray(transforms);
            mJobHandleUpdateHealthBarPosition = mJobUpdateHealthBarPosition.Schedule(accessArray);
            mJobHandleUpdateHealthBarPosition.Complete();

            JobHandle.ScheduleBatchedJobs();
            accessArray.Dispose();

            float dis = Vector3.Distance(position, cameraTransformPosition);

            if (healthBarWidgets == null)
            {
                healthBarWidgets = this.GetComponentsInChildren<UIWidget>(true);
                healthBarLen = healthBarWidgets.Length;
                healthBarWidgetsName = new string[healthBarLen];
                for (int i = 0; i < healthBarLen; i++)
                {
                    healthBarWidgetsName[i] = healthBarWidgets[i].name;
                }
            }
            this.transform.localScale = Vector3.one;
            dis = dis * 5;
            int realdis = Mathf.RoundToInt(dis);
            for (int i = 0; i < healthBarLen; ++i)
            {
                UIWidget healthUIWidget = healthBarWidgets[i];
                string healthUIWidgetName = healthBarWidgetsName[i];
                if(healthUIWidgetName.IndexOf("_Depth") == -1){
                    int depth = -realdis * 5;
                    if (depth != healthUIWidget.depth)
                        healthUIWidget.depth = depth;
                }
                else{
                    if (healthUIWidgetName.IndexOf("1") != -1)
                    {
                        int depth = -realdis * 5 + 1;
                        if (depth != healthUIWidget.depth)
                            healthUIWidget.depth = depth;
                    }
                    else if (healthUIWidgetName.IndexOf("2") != -1)
                    {
                        int depth = -realdis * 5 + 2;
                        if (depth != healthUIWidget.depth)
                            healthUIWidget.depth = depth;
                    }
                    else if (healthUIWidgetName.IndexOf("3") != -1)
                    {
                        int depth = -realdis * 5 + 3;
                        if (depth != healthUIWidget.depth)
                            healthUIWidget.depth = depth;
                    }
                    else if (healthUIWidgetName.IndexOf("4") != -1)
                    {
                        int depth = -realdis * 5 + 4;
                        if (depth != healthUIWidget.depth)
                            healthUIWidget.depth = depth;
                    }
                }
            }
        }
    }
}

