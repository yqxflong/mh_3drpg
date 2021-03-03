using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTCloudFXUIController : UIControllerHotfix
    {
        public GameObject CloudHoldCom;
        public GameObject CloudOpenCom;
        public GameObject CloudCloseCom;

        private System.Action CloudCallback;

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            CloudHoldCom = t.Find("CloudLoadingScreen/CloudHold").gameObject;
            CloudOpenCom = t.Find("CloudLoadingScreen/CloudOpen").gameObject;
            CloudCloseCom = t.Find("CloudLoadingScreen/CloudClose").gameObject;
        }

        public override bool IsFullscreen() { return false; }
        public override bool ShowUIBlocker { get { return false; } }
        
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null)
            {
                CloudCallback = param as System.Action;
            }
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            ShowCloudScreen();
            yield return new WaitForSeconds(1.0f);
            HoldCloudScreen();
            yield return new WaitForSeconds(0.3f);
            CloseCloudScreen();
            yield return new WaitForSeconds(1.0f);
            Hotfix_LT.Messenger.Raise(EventName.OnShowDoorEvent);
            controller.Close();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
        
        private void ShowCloudScreen()
        {
            CloudCloseCom.CustomSetActive(true);
            CloudOpenCom.CustomSetActive(false);
            CloudHoldCom.CustomSetActive(false);
        }

        private void HoldCloudScreen()
        {
            CloudHoldCom.CustomSetActive(true);
            CloudCloseCom.CustomSetActive(false);
            CloudOpenCom.CustomSetActive(false);
            if (CloudCallback != null)
            {
                CloudCallback();
                CloudCallback = null;
            }
        }

        public void CloseCloudScreen()
        {
            CloudOpenCom.CustomSetActive(true);
            CloudCloseCom.CustomSetActive(false);
            CloudHoldCom.CustomSetActive(false);
        }
    }
}
