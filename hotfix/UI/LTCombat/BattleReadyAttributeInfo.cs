using System.Collections;

namespace Hotfix_LT.UI
{
    public class BattleReadyAttributeInfo : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("BG/BG2/CloseBtn");
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    }
}
