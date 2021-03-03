using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceBossCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;

        }

        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return false;
            }
        }

        private int timer = 0;
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            timer= ILRTimerManager.instance.AddTimer(3000, 1, OnCloseFunc);
        }
    

        public override IEnumerator OnRemoveFromStack()
        {
            FusionAudio.PostEvent("UI/New/Warn", false);
            if (timer > 0)
            {
                ILRTimerManager.instance.RemoveTimer(timer);
                timer = 0;
            }
            DestroySelf();
            yield break;
        }

        private void OnCloseFunc(int timer)
        {
            timer = 0;
            controller.Close();
        }
    }
}
