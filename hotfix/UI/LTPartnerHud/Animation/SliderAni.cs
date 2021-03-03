using _HotfixScripts.Utils;
using UnityEngine;

//此动画脚本只适用于有UISlider组件的进度条
namespace Hotfix_LT.UI
{
    public class SliderAni : DynamicMonoHotfix, IHotfixUpdate
    {
        private UISlider slider;
        private bool isPlayAni;
        private float initialSliderValue;
        private float goalSliderValue;
        private float playTime;
        private float curPlayTime;
        private float value;

        public override void Start()
        {
            slider = mDMono.transform.GetComponent<UISlider>();
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            ErasureMonoUpdater();
            isPlayAni = false;
            Hotfix_LT.Messenger.Raise("OnPartnerRollAniBreak");
        }

        public void Update()
        {
            if (isPlayAni)
            {
                if (slider == null)
                {
                    isPlayAni = false;
                    return;
                }

                curPlayTime += Time.deltaTime;
                value = initialSliderValue + (goalSliderValue - initialSliderValue) * (curPlayTime / playTime);
                slider.value = value;
                if (curPlayTime >= playTime)
                {
                    isPlayAni = false;
                    slider.value = goalSliderValue;
                }
            }
        }

        public void SetAniData(float initialSliderValue, float goalSliderValue, float playTime)
        {
            this.initialSliderValue = initialSliderValue;
            this.goalSliderValue = goalSliderValue;
            this.playTime = playTime;
            curPlayTime = 0;
            isPlayAni = true;

            if (slider != null && !slider.gameObject.activeInHierarchy)
            {
                slider.gameObject.CustomSetActive(true);
            }
        }

        public void StopAni()
        {
            isPlayAni = false;
        }
    }
}
