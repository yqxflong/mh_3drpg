using UnityEngine;

namespace Hotfix_LT.UI
{
    public class RewardCardItem : DynamicMonoHotfix
    {
        public TweenScale NormalScale, OpenScale;
        public LTShowItem UIItem;
        public System.Action OnAnimFinished;
        public GameObject PrivilegeObj;
        public GameObject FxObj;
        private bool mClickEnable = true;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            NormalScale = t.GetComponent<TweenScale>("CardNoOpen");
            OpenScale = t.GetComponent<TweenScale>("Open");
            UIItem = t.GetMonoILRComponent<LTShowItem>("Open/LTShowItem");
            PrivilegeObj = t.FindEx("Open/Border").gameObject;
            FxObj = t.FindEx("fx").gameObject;

            t.GetComponent<TweenScale>("CardNoOpen").onFinished.Add(new EventDelegate(OnNormalAnimPlayed));
            t.GetComponent<TweenScale>("Open").onFinished.Add(new EventDelegate(OnOpenAnimPlayed));
        }

        public void OpenCard()
        {
            NormalScale.gameObject.CustomSetActive(false);
            OpenScale.gameObject.CustomSetActive(true);
        }

        public void OpenCardWithAnim()
        {
            FusionAudio.PostEvent("UI/Card/Rotation", true);
            NormalScale.ResetToBeginning();
            NormalScale.PlayForward();

            OpenScale.ResetToBeginning();
            OpenScale.PlayForward();
        }

        public void DisableClick(bool isGoldVIP = false)
        {
            mClickEnable = false;
            PrivilegeObj.CustomSetActive(isGoldVIP);
        }

        public void OnCardClick()
        {
            if (mClickEnable)
                OpenCardWithAnim();
            FxObj.CustomSetActive(true);

        }

        public void OnNormalAnimPlayed()
        {
            NormalScale.gameObject.CustomSetActive(false);
            OpenScale.gameObject.CustomSetActive(true);
        }

        public void OnOpenAnimPlayed()
        {
            if (OnAnimFinished != null)
                OnAnimFinished();
        }
    }
}
