using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class SliderAniData
    {
        public float CurValue;   //当前值
        public float NextValue;  //到达值
        public float Timer;       //用时

        public SliderAniData(float curValue, float nextValue, float timer)
        {
            CurValue = curValue;
            NextValue = nextValue;
            Timer = timer;
        }
    }

    public class LTEquipSliderAni : DynamicMonoHotfix, IHotfixUpdate
    {
        public Queue<SliderAniData> TimesList = new Queue<SliderAniData>();
        public Transform UpLabelObj;
        private bool isShow = false;
        private UISlider m_slider;
        private SliderAniData curData;
        private float curTimer;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            UpLabelObj = t.GetComponent<Transform>("StarUpTitle");
        }	

		public override void Start()
        {
            m_slider = mDMono.transform.GetComponent<UISlider>();
            curTimer = 0;
        }

        private void OnAnimFinish()
        {
            FusionAudio.PostEvent("UI/Partner/ExpSlider", false);
        }

        public override void OnEnable()
        {
            isShow = false;
			RegisterMonoUpdater();

			UITweener[] tweeners = UpLabelObj.GetComponents<UITweener>();

            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].ResetToBeginning();
                tweeners[0].AddOnFinished(OnAnimFinish);
                tweeners[j].enabled = false;
            }

            TimesList = new Queue<SliderAniData>();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }

        public void ResetList()
        {
            TimesList.Clear();
            isShow = false;
        }

        public void Update()
        {
            if (TimesList.Count > 0 && !isShow)
            {
                curTimer = 0;
                curData = TimesList.Dequeue();
                isShow = true;
            }
            if (isShow)
            {
                if (curData.NextValue == -1)//到达最大值
                {
                    m_slider.value = 1;
                    isShow = false;
                }
                else
                {
                    curTimer += Time.deltaTime;
                    m_slider.value = curData.CurValue + (curData.NextValue - curData.CurValue) * (curTimer / curData.Timer);

                    if (curTimer >= curData.Timer)
                    {
                        isShow = false;
                        if (TimesList.Count == 0)
                        {

                        }

                        if (m_slider.value == 1 && UpLabelObj != null)
                        {
                            UITweener[] tweeners = UpLabelObj.GetComponents<UITweener>();

                            for (int j = 0; j < tweeners.Length; ++j)
                            {
                                tweeners[j].ResetToBeginning();
                                tweeners[j].PlayForward();

                            }
                        }
                    }
                }
            }
        }
    }
}
