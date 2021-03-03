using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTWorldBossRewardPreviewCtrl : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
    
        public LTWorldBossRankRewardDynamicScroll DynamicScroll;
    
        public override bool IsFullscreen()
        {
            return false;
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            DynamicScroll = t.GetMonoILRComponent<LTWorldBossRankRewardDynamicScroll>("Content/Scroll/PlaceHolder/Grid");
            controller.backButton = t.GetComponent<UIButton>("BG/Top/CancelBtn");
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
    
            DynamicScroll.SetItemDatas(Hotfix_LT.Data.EventTemplateManager.Instance.GetBossRewardList().ToArray());
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield return base.OnRemoveFromStack();
        }
    }
}
