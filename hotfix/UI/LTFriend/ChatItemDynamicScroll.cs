namespace Hotfix_LT.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ChatItemDynamicScroll : DynamicUITableScroll<ChatUIMessage, ChatItem>
    {
        public override void Awake()
        {
            base.Awake();
            if (mDMono.FloatParamList!=null)
            {
                thresholdFactor = mDMono.FloatParamList[0];
                padding = mDMono.FloatParamList[1];
                mOffsetValue = mDMono.FloatParamList[2];
            }

            CellLimit = 50;
            IsNoNeedForDelayFill = true;
        }
    }

}