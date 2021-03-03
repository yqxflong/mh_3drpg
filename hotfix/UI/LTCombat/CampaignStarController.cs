using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class CampaignStarController : DynamicMonoHotfix
    {
        public StarItem m_1;
        public StarItem m_2;
        public StarItem m_3;
        public Vector3[] m_RankShakeOffsets = new Vector3[] { new Vector3(2.0f, 2.0f, 0.0f), new Vector3(-2.0f, -2.0f, 0.0f) };
        public float PlayInterval = 0.5f;
        public bool isTest;
        public UICamera m_TestCamera;
        private UIPanel panel;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_1 = t.GetMonoILRComponent<StarItem>("1");
            m_2 = t.GetMonoILRComponent<StarItem>("2");
            m_3 = t.GetMonoILRComponent<StarItem>("3");
            panel = t.GetComponent<UIPanel>();
            PlayInterval = 0f;
            isTest = false;
        }

        public void LightStarsAnimation(int num)
        {
            StartCoroutine(DoLightAnimation(num));
        }

        public IEnumerator DoLightAnimation(int num)
        {
            Light(0);
            yield return new WaitForEndOfFrame();
            if (m_1.SSO != null) m_1.SSO.SortingOrder = panel.sortingOrder + 10;
            if (m_2.SSO != null) m_2.SSO.SortingOrder = panel.sortingOrder + 10;
            if (m_3.SSO != null) m_3.SSO.SortingOrder = panel.sortingOrder + 10;

            //第一个亮
            if (num >= 1)
            {
                DoCameraShake();
                yield return m_1.DynamicLight();
            }
            //第二个亮
            if (num >= 2)
            {
                yield return new WaitForSeconds(PlayInterval);
                DoCameraShake();
                yield return m_2.DynamicLight();
            }
            //第三个亮
            if (num >= 3)
            {
                yield return new WaitForSeconds(PlayInterval);
                DoCameraShake();
                yield return m_3.DynamicLight();
            }
            yield break;
        }

        public void DoCameraShake()
        {
            UICamera ui_camera = null;
            //ThinksquirrelSoftware.Utilities.CameraShake camera_shake = null;
            if (UIHierarchyHelper.Instance != null)
            {
                ui_camera = UIHierarchyHelper.Instance.MainUICamera;
            }

            if (isTest) ui_camera = m_TestCamera;
            StartCoroutine(CampaignRatingDialogMH.DoUICameraShake(ui_camera, m_RankShakeOffsets, 0.2f));
        }

        public void Light(int num)
        {
            switch (num)
            {
                case 0:
                    m_1.Reset();
                    m_2.Reset();
                    m_3.Reset();
                    break;
                case 1:
                    m_1.Light();
                    m_2.Reset();
                    m_3.Reset();
                    break;
                case 2:
                    m_1.Light();
                    m_2.Light();
                    m_3.Reset();
                    break;
                case 3:
                    m_1.Light();
                    m_2.Light();
                    m_3.Light();
                    break;
                default:
                    m_1.Reset();
                    m_2.Reset();
                    m_3.Reset();
                    break;
            }
        }

        [ContextMenu("TestPlay")]
        public void TestPlay()
        {
            StartCoroutine(DoLightAnimation(3));
        }
    }
}
