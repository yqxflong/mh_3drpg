using System.Collections;
using EB.Sparx;
/// <summary>
/// 军团副本UI界面
/// 对应挂载的预置体：LTLegionFBUI
/// </summary>
namespace Hotfix_LT.UI
{

    public class LTLegionFBHudController : UIControllerHotfix
    {

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            v_TLegionFBBossInfo = t.GetMonoILRComponent<LTLegionFBBossInfo>("ContentView/Views/BossPanel");
            HuDState.IsLTLegionFBHudOpen = false;
            controller.backButton = t.GetComponent<UIButton>("ContentView/Views/BossPanel/RewardPanel/CancelBtn");

        }


        /// <summary>
        /// 军团副本界面的BOSS信息界面
        /// </summary>
        public LTLegionFBBossInfo v_TLegionFBBossInfo;
        /// <summary>
        /// 协议
        /// </summary>
        public LTLegionFBAPI v_Api
        {
            get; private set;
        }

        public int BossIndex;
        /// <summary>
        /// 设置当前界面
        /// </summary>
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null)
            {
                BossIndex = (int)param;
            }
            controller.gameObject.SetActive(true);
        }

        public override IEnumerator OnAddToStack()
        {
            var coroutine = EB.Coroutines.Run(base.OnAddToStack());
            HuDState.IsLTLegionFBHudOpen = true;
            v_TLegionFBBossInfo.F_SetMenuData(BossIndex);
            v_Api = new LTLegionFBAPI();
            yield return coroutine;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            base.OnRemoveFromStack();
            DestroySelf();
            BossIndex = 0;
            v_TLegionFBBossInfo.v_RankController.CleanRank();
            yield break;
        }

        public override void OnCancelButtonClick()
        {
            if (v_TLegionFBBossInfo.IsLoadModel) return;

            HuDState.IsLTLegionFBHudOpen = false;
            GlobalMenuManager.Instance.RemoveCache("LTLegionFBUI");
            base.OnCancelButtonClick();
        }
    }
}



