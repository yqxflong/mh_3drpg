using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTChargeAndGift : DynamicMonoHotfix

    {
        public UIScrollView ScrollView;

        public UIGrid Grid;

        public LTChargeStoreItem tempChargeItem;
        public GameObject tempChargeItemObj;
        public GameObject Arrow;

        private List<EB.IAP.Item> chargeDataList;

        private EB.IAP.Item curChargeData;

        private List<LTChargeStoreItem> chargeItemList;

        private LTChargeStoreController.EChargeType curType;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            ScrollView = t.Find("ScrollView").GetComponent<UIScrollView>();
            Grid = t.Find("ScrollView/Grid").GetComponent<UIGrid>();
            tempChargeItemObj = t.Find("ScrollView/Grid/Item").gameObject;
            tempChargeItem = t.Find("ScrollView/Grid/Item").GetMonoILRComponent<LTChargeStoreItem>();
            Arrow = t.Find("Arrow").gameObject;
            chargeItemList = new List<LTChargeStoreItem>();
            chargeItemList.Add(tempChargeItem);
        }

        public void SetType(LTChargeStoreController.EChargeType eType)
        {
            curType = eType;
        }

        public void ShowUI(bool isShow, List<EB.IAP.Item> chargeDatas = null)
        {
            if (!isShow)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }

            mDMono.gameObject.CustomSetActive(true);
            chargeDataList = chargeDatas;
            //排序
            if (curType == LTChargeStoreController.EChargeType.eCharge)
            {
                chargeDataList.Sort(delegate (EB.IAP.Item x, EB.IAP.Item y) { return (int)(x.cost - y.cost); });
            }
            else if (curType != LTChargeStoreController.EChargeType.ePrivilege)
            {
                chargeDataList.Sort(delegate (EB.IAP.Item x, EB.IAP.Item y) { return (int)(x.order - y.order); });
            }

            RefreshUI();
        }

        private bool isMore;
        private void RefreshUI()
        {
            ScrollView.ResetPosition();
            isMore = chargeDataList.Count > 6;
            Arrow.CustomSetActive(isMore);

            InitItem();

            for (int i = 0; i < chargeItemList.Count; i++)
            {
                if (i < chargeDataList.Count)
                {
                    chargeItemList[i].mDMono.gameObject.CustomSetActive(true);
                    chargeItemList[i].Show(chargeDataList[i]);
                    chargeItemList[i].SetClickEvent(OnClickItem);
                }
                else
                {
                    chargeItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
        }

        private void InitItem()
        {
            int count = chargeDataList.Count - chargeItemList.Count;
            if (count <= 0)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                LTChargeStoreItem item =UnityEngine.Object.Instantiate(tempChargeItemObj).GetMonoILRComponent<LTChargeStoreItem>();
                item.mDMono.transform.SetParent(Grid.transform);
                item.mDMono.transform.localPosition = Vector3.zero;
                item.mDMono.transform.localEulerAngles = Vector3.zero;
                item.mDMono.transform.localScale = Vector3.one;
                chargeItemList.Add(item);
            }

            Grid.Reposition();
        }

        private void OnClickItem(EB.IAP.Item data)
        {
            if (curType == LTChargeStoreController.EChargeType.eCharge)
            {
                LTChargeManager.Instance.PurchaseOfferExpand(data, LTChargeStoreController.EventTable);
            }
            else
            {
                GlobalMenuManager.Instance.Open("LTChargeStoreGiftUI", data);
            }
        }

        public void OnScrollChange()
        {
            if (isMore)
            {
                Arrow.CustomSetActive(isMore && ScrollView.verticalScrollBar.value < 0.98);
            }
        }
   

        UIButton HotfixBtn0;
        ConsecutiveClickCoolTrigger HotfixCoolBtn0;

    }

}