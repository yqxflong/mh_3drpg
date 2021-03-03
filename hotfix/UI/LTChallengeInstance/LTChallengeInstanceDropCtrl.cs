using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceDropCtrl : UIControllerHotfix
    {
        public override bool IsFullscreen() { return false; }
        public override bool ShowUIBlocker { get { return true; } }
        private List<LTChallengeInstanceDropItem> ItemList;
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("BG/Top/CancelBtn");
            ItemList = new List<LTChallengeInstanceDropItem>();
            ItemList.Add(t.GetMonoILRComponent<LTChallengeInstanceDropItem>("Scroll/PlaceHolder/Grid/Item"));
            ItemList.Add(t.GetMonoILRComponent<LTChallengeInstanceDropItem>("Scroll/PlaceHolder/Grid/Item (1)"));
            ItemList.Add(t.GetMonoILRComponent<LTChallengeInstanceDropItem>("Scroll/PlaceHolder/Grid/Item (2)"));
            ItemList.Add(t.GetMonoILRComponent<LTChallengeInstanceDropItem>("Scroll/PlaceHolder/Grid/Item (3)"));
            ItemList.Add(t.GetMonoILRComponent<LTChallengeInstanceDropItem>("Scroll/PlaceHolder/Grid/Item (4)"));
            ItemList.Add(t.GetMonoILRComponent<LTChallengeInstanceDropItem>("Scroll/PlaceHolder/Grid/Item (5)"));
            ItemList.Add(t.GetMonoILRComponent<LTChallengeInstanceDropItem>("Scroll/PlaceHolder/Grid/Item (6)"));
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            for(int i=0;i<ItemList.Count; ++i)
            {
                yield return null;
                ItemList[i].SetDrop();
            }
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    }
}
