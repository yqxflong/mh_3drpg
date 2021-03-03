using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTChargePrivilege : DynamicMonoHotfix
    {
        public enum EMonthCardType
        {
            eSilver = 0,
            eGold = 1
        }

        public LTChargePrivilegeItem[] ChargePriItemList;
        private List<EB.IAP.Item> chargeDataList;
        private EB.IAP.Item curChargeData;
        ConsecutiveClickCoolTrigger HotfixCoolBtn0;
        ConsecutiveClickCoolTrigger HotfixCoolBtn1;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            ChargePriItemList = new LTChargePrivilegeItem[2];
            ChargePriItemList[0] = t.Find("Item2").GetMonoILRComponent<LTChargePrivilegeItem>();
            ChargePriItemList[1] = t.Find("Item1").GetMonoILRComponent<LTChargePrivilegeItem>();
        }

        public void ShowUI(bool isShow, List<EB.IAP.Item> chargeDatas = null)
        {
            if (!isShow || chargeDatas == null || chargeDatas.Count <= 0)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }

            mDMono.gameObject.CustomSetActive(true);
            chargeDataList = chargeDatas;

            RefreshUI();
        }

        private void RefreshUI()
        {
            if (chargeDataList.Count < 2)
            {
                EB.Debug.LogError("LTChargePrivilege RefreshUI is Error,chargeDataList Less Than 2, chargeDataList.Count : {0}", chargeDataList.Count);
                return;
            }

            for (int i = 0; i < ChargePriItemList.Length; i++)
            {
                ChargePriItemList[i].SetMonthCardType((EMonthCardType)i);
                ChargePriItemList[i].ShowUI(chargeDataList[i]);
            }
        }
    }
}
