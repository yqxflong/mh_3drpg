using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeSPCtrl : DataLookupHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDL.transform;
            NumLabel = t.GetComponent<UILabel>();
            Hotfix_LT.Messenger.AddListener<object>(EventName .ChallengeInstanceBuySucc, OnChallengeShopBuySucc);
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener<object>(EventName .ChallengeInstanceBuySucc, OnChallengeShopBuySucc);
            base.OnDestroy();
        }

        public enum SPEventType
        {
            INIT,
            ADD,
            REMOVE,
            DOUBLE,
        }
    
        private int pastSP;
    
        public UILabel NumLabel;
    
        public override void OnLookupUpdate(string dataID, object value)
        {
            if(value != null)
            {
                LTUIUtil.SetText(NumLabel, value.ToString());
            }
        }
    
        public void UpdateSp(SPEventType type)
        {
            int curSP = 0;
            DataLookupsCache.Instance.SearchIntByID(mDL.DataIDList[0], out curSP);
    
            if (type == SPEventType.REMOVE)
            {
                if (pastSP > curSP || (pastSP == 0 && curSP == 0))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeSPCtrl_1368"), pastSP - curSP));
                }
            }
            else if (type == SPEventType.DOUBLE)
            {
                if (pastSP * 2 == curSP)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeSPCtrl_1634")));
                }
            }
            else if (type == SPEventType.ADD)
            {
                if (pastSP < curSP)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeSPCtrl_1870"), curSP - pastSP));
                }
            }
    
            LTUIUtil.SetText(NumLabel, curSP.ToString());
            pastSP = curSP;
        }
    
        private void OnChallengeShopBuySucc(object data)
        {
            if (data != null)
            {
                UpdateSp(SPEventType.INIT);
            }
        }
    }
}
