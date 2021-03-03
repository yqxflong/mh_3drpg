using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceBagScroll : DynamicTableScroll<LTChallengeInstanceBagData, LTChallengeInstanceBagCell, LTChallengeInstanceBagRow>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            columns = 4;
            scrollView = t.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = t .parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            addition = 0;
            IsNoNeedForDelayFill = false;

            padding = 0f;

            // Set padding
            if (mDMono.FloatParamList != null)
            {
                var count = mDMono.FloatParamList.Count;

                if (count > 0)
                {
                    thresholdFactor = mDMono.FloatParamList[0];
                }
            }
        }

        private System.Action<LTChallengeInstanceBagCell> OnBtnClick;

        public void SetOnBtnClickAction(System.Action<LTChallengeInstanceBagCell> OnClick)
        {
            OnBtnClick = OnClick;
        }

        protected override void OnFilled(LTChallengeInstanceBagCell itemCtrl, LTChallengeInstanceBagData itemData)
        {
            base.OnFilled(itemCtrl, itemData);

            if(OnBtnClick!=null) itemCtrl.SetOnBtnClickAction(OnBtnClick);
        }

        protected override void SetColumns()
        {

        }
    }
}
