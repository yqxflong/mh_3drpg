using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ToVigorRestoreTimeScreen : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            mDMono.transform.GetComponentEx<UIButton>().onClick.Add(new EventDelegate(() =>
            {
                Vector2 screenPos = UICamera.lastEventPosition;
                GlobalMenuManager.Instance.Open("LTVigorToolTipUI", screenPos);
                FusionAudio.PostEvent("UI/General/ButtonClick");
            }));
        }
    }
}
