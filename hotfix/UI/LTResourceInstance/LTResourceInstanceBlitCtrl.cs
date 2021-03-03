using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTResourceInstanceBlitCtrl : UIControllerHotfix
    {
        public LTResourceInstanceRatingGM com;
        public GameObject Tip;

        private bool isWon;
        private bool isConfirm;
        private eBattleType _battleType;
        private System.Action callback;

        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return !isConfirm;
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            com = t.GetMonoILRComponent<LTResourceInstanceRatingGM>("Content/ResInstanceRatingGM");
            Tip = t.FindEx("Content/Tip").gameObject;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null)
            {
                Hashtable table = param as Hashtable;
                callback = table["action"] as System.Action;
                isConfirm = (bool)table["isConfirm"];
                _battleType = (eBattleType)table["battleType"];
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            com.OnShownAnimCompleted = () =>
            {
                Tip.CustomSetActive(true);
            };
            com.BattleType = _battleType;
            com.mDMono.gameObject.CustomSetActive(true);
    
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            Tip.CustomSetActive(false);
            com.mDMono.gameObject.CustomSetActive(false);
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }
    
        public override void OnCancelButtonClick()
        {
            base.OnCancelButtonClick();
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
    }
}
