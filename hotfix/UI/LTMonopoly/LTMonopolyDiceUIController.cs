using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTMonopolyDiceUIController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }

        public override void Awake()
        {
            base.Awake();
            Transform t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("LTFrame/Content/CloseBtn");

            controller.FindAndBindingBtnEvent(new List<string>(6) { "Content/Btns/1", "Content/Btns/2", "Content/Btns/3", "Content/Btns/4", "Content/Btns/5", "Content/Btns/6"},
                new List<EventDelegate>(6) { new EventDelegate(() => OnDiceBtnClick(1)), new EventDelegate(() => OnDiceBtnClick(2)), new EventDelegate(() => OnDiceBtnClick(3)), new EventDelegate(() => OnDiceBtnClick(4)), new EventDelegate(() => OnDiceBtnClick(5)), new EventDelegate(() => OnDiceBtnClick(6))});
        }
        
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }

        public void OnDiceBtnClick(int Num)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            if(LTGeneralInstanceHudController.Instance != null)
            {
                LTMonopolyInstanceHudController temp = LTGeneralInstanceHudController.Instance as LTMonopolyInstanceHudController;
                if (temp != null)
                {
                    temp.OnDiceRequest(Num);
                }
            }
            controller.Close();
        }
    }
}