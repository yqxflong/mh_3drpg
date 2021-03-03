using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class UILazyLabel : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            if (mDMono.FloatParamList != null)
            {
                var count = mDMono.FloatParamList.Count;

                if (count > 0)
                {
                    delt = mDMono.FloatParamList[0];
                }
            }

            if (mDMono.IntParamList != null)
            {
                var count = mDMono.IntParamList.Count;

                if (count > 0)
                {
                    step = mDMono.IntParamList[0];
                }
            }

            m_Label = mDMono.transform.GetComponentEx<UILabel>();
        }


        public float delt = 0.005f;
        public int step = 1;
        public UILabel m_Label;
        private string text;
        Coroutine SeedLabelCourtine;
        [HideInInspector]
        public bool isPlaying = false;

        public string Text
        {
            set
            {
                text = value;

                if (mDMono.gameObject.activeInHierarchy)
                {
                    if (SeedLabelCourtine != null)
                    {
                        StopCoroutine(SeedLabelCourtine);
                        SeedLabelCourtine = null;
                    }
                    SeedLabelCourtine = StartCoroutine(SeedLabel());
                }
            }
        }

        IEnumerator SeedLabel()
        {
            yield return new WaitForEndOfFrame();
            if (string.IsNullOrEmpty(text)) yield break;
            isPlaying = true;
            m_Label.text = "";
            int steps = text.Length / step;
            int left = text.Length % step;
            for (int i = 1; i <= steps; i++)
            {
                if (i > text.Length)
                {
                    //在某些时候已经出现了下一句了，text已经被重新赋值了，但是这里还有可能再被调用一次，导致如果上一句比下一句上，会出现越界的问题
                    break;
                }
                m_Label.text = text.Substring(0, i * step);
                yield return new WaitForSeconds(delt);
            }
            if (left != 0) m_Label.text = text;
            isPlaying = false;
            yield break;
        }

        public void StopSeedLabel()
        {
            StopCurCoroutine();
            m_Label.text = text;
        }

        private void StopCurCoroutine()
        {
            if (SeedLabelCourtine != null)
            {
                StopCoroutine(SeedLabelCourtine);
                SeedLabelCourtine = null;
            }
            isPlaying = false;
        }

        public override void OnEnable()
        {
            //StopCoroutine(SeedLabel());
            //StartCoroutine(SeedLabel());
        }

        public override void OnDisable()
        {
            //StopCoroutine(SeedLabel());
            StopCurCoroutine();
        }
    }
}
