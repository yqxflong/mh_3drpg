using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.MainLand
{
    public class HeadBars2DMonitor : MonoBehaviour
    {
        public Dictionary<int, HeadBarHUDMonitor> Bars = new Dictionary<int, HeadBarHUDMonitor>();

        private Transform m_locator = null;
        private Camera camera_main;
        private float screenScale = 0.001f;
        private float screenHeight = 0f;
        private float screenWidth = 0f;
        private int manualHeight_2;
        private int manualWidth_2;
        private Vector3 locatorPos;
        private Vector3 cameraPos;
        private readonly Vector3 remote = new Vector3(10000, 10000, -1200);
        private bool isAlreadyInitPos = false;

        private void Awake()
        {
            screenHeight = (float)Screen.height;
            screenWidth = (float)Screen.width;
            if ((float)Screen.width / Screen.height > (float)UIRoot.list[0].manualWidth / UIRoot.list[0].manualHeight)
            {
                screenScale = UIRoot.list[0].manualHeight / screenHeight;
                manualHeight_2 = UIRoot.list[0].manualHeight / 2;
                manualWidth_2 = (int)(Screen.width * screenScale) / 2;
            }
            else
            {
                screenScale = UIRoot.list[0].manualWidth / screenWidth;
                manualHeight_2 = (int)(Screen.height * screenScale) / 2;
                manualWidth_2 = UIRoot.list[0].manualWidth / 2;
            }
        }

        private void OnDisable()
        {
            isAlreadyInitPos = false;
        }

        public void SetLocator(Transform tran)
        {
            m_locator = tran;
            camera_main = Camera.main;
        }

        public void LateUpdate()
        {
            UpdatePosition();
        }
        
        public void UpdatePosition()
        {
            if (!GameEngine.Instance.IsTimeToRootScene) return;

            if (isAlreadyInitPos && UIStack.Instance.IsUIFullScreenOpened) return;

            if (m_locator == null) return;

            if (Bars.Count == 0) return;

            if (camera_main == null)
            {
                camera_main = Camera.main;
                if (camera_main == null) return;
            }

            if (isAlreadyInitPos && locatorPos == m_locator.position && cameraPos == camera_main.transform.position) return;
            
            locatorPos = m_locator.position;
            cameraPos = camera_main.transform.position;

            Vector3 screen_point = camera_main.WorldToScreenPoint(locatorPos);

            screen_point.x -= (screenWidth * 0.5f);
            screen_point.y -= (screenHeight * 0.5f);
            screen_point *= screenScale;
            screen_point.z = 1200;
            
            if (!IsInScreen(screen_point))
            {
                Dictionary<int, HeadBarHUDMonitor>.Enumerator enumerator = Bars.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.CleanPosition(remote, Vector3.one);
                }
            }
            else
            {
                Dictionary < int, HeadBarHUDMonitor>.Enumerator enumerator = Bars.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.UpdatePosition(screen_point, camera_main.transform.position, locatorPos);
                }

                isAlreadyInitPos = true;
            }
        }
        
        private bool IsInScreen(Vector3 pos)
        {
            if (pos.x < -1 * manualWidth_2 || pos.x > manualWidth_2 || pos.y < -1 * manualHeight_2 || pos.y > manualHeight_2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
