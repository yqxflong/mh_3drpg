namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections;

    public class ArenaRankAwardController : UIControllerHotfix {

        public override bool ShowUIBlocker { get { return true;	} }

        public ArenaRankAwardDynamicScroll DynamicScroll;
        public UIButton closeBtn;

        public override void Awake()
        {
            base.Awake();
            DynamicScroll =
                controller.transform.Find("Content/ScrollView/Placehodler/Grid").GetComponent<DynamicMonoILR>()
                    ._ilrObject as ArenaRankAwardDynamicScroll;
            closeBtn = controller.transform.Find("Frame/BG/Top/CloseBtn").GetComponent<UIButton>();
            closeBtn.onClick.Add(new EventDelegate(base.OnCancelButtonClick));
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();

            DynamicScroll.SetItemDatas(Hotfix_LT.Data.EventTemplateManager.Instance.GetArenaAwardTemplates().ToArray());
        }
    }

}