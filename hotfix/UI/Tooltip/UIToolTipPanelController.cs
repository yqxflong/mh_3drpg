using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class UIToolTipPanelController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }

        public override void OnFocus()
        {
            base.OnFocus();

            var ctrl = controller.transform.GetMonoILRComponentByClassPath<GenericItemController>("Hotfix_LT.UI.GenericItemController", false);
            
            if (ctrl != null)
            {
                ctrl.Show();
            }
        }

        public override void OnBlur()
        {
            var ctrl = controller.transform.GetMonoILRComponentByClassPath<GenericItemController>("Hotfix_LT.UI.GenericItemController", false);
            
            if (ctrl != null && !GuideNodeManager.IsGuide)
            {
                ctrl.m_ThisPanel.alpha = 0;
            }

            base.OnBlur();
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            BootFlash();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            var ctrl = controller.transform.GetMonoILRComponentByClassPath<GenericItemController>("Hotfix_LT.UI.GenericItemController", false);
            
            if (ctrl != null)
            {
                ctrl.Close();
            }
            Coroutine coroutine = EB.Coroutines.Run(base.OnRemoveFromStack());
            ClearCacheData();
            UITooltipManager.Instance.HideTooltip();

            //发送这个消息 是为了删除被点击物体上的事件监听
            Messenger.Raise(Hotfix_LT.EventName.OnLoadClick, eLoadType.Default);
            yield return coroutine;
        }

        public void CloseFromLoadClick()
        {
            controller.Close();
        }
        private void ClearCacheData()
        {
            DataLookupsCache.Instance.CacheData("next_upgrade_id", null);
        }

        private void BootFlash()
        {
            UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }

        public override void OnCancelButtonClick()
        {
            base.OnCancelButtonClick();
            UITooltipManager.Instance.CleanToolTip();
        }
    }
}
