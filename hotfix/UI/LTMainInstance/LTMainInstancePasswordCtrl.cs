using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTMainInstancePasswordCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            PasswordLabel = t.GetComponent<UILabel>("Label");

        }
        
        public UILabel PasswordLabel;
    
        private string password;
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override void SetMenuData(object param)
        {
            password = param as string;
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            PasswordLabel.text = password;
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    }
}
