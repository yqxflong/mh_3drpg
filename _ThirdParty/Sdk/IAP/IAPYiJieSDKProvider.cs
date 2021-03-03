#if USE_YIJIESDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
    public class YiJieSDKProvider : Provider
    {
        private Config mConfig;

        private Transaction mCurTransation = null;

        private Item mCurItem = null;

        public string Name
        {
            get
            {
                return "yijie";
            }
        }

        public static YiJieSDKProvider Create(Config config)
        {
            YiJieSDKProvider provider = new YiJieSDKProvider();
            provider.mConfig = config;
            return provider;
        }

        public void Complete(Transaction transaction)
        {
            EB.Debug.Log("YiJieSDKProvider.Complete: transaction {0} completed", transaction.transactionId);
            mCurTransation = null;
            mCurItem = null;
        }

        public void Enumerate(List<Item> items)
        {
            foreach (var item in items)
            {
                item.valid = true;
            }

            if (mConfig.OnEnumerate != null)
            {
                mConfig.OnEnumerate();
            }
        }

        public void PurchaseItem(Item item)
        {
            if (mCurItem != null && mCurTransation != null)
            {
                EB.Debug.LogWarning("YiJieSDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    OnPurchaseItemCallback(EB.Sparx.YiJieSDKConfig.PAY_SUCCESS);
                }
            }
            /*if (mCurTransation != null)
            {
                EB.Debug.LogWarning("YiJieSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurTransation.transactionId);
                return;
            }

            if (mCurItem != null)
            {
                EB.Debug.LogWarning("YiJieSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurItem.productId);
                return;
            }*/

            mCurItem = item;
            EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurItem, delegate (string error, IAP.Transaction transaction)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    EB.Debug.LogWarning("YiJieSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
                    if (mConfig != null && mConfig.OnPurchaseFailed != null)
                    {
                        mConfig.OnPurchaseFailed(error);
                    }
                    mCurItem = null;
                    return;
                }

                mCurTransation = transaction;
                SparxHub.Instance.YiJieSDKManager.Pay(mCurItem, transaction, OnPurchaseItemCallback);
            });
        }

        private void OnPurchaseItemCallback(int resultCode)
        {
            if (EB.Sparx.YiJieSDKConfig.PAY_SUCCESS == resultCode)
            {
                OnPurchaseItemSucc();
            }
            else
            {
#if UNITY_ANDROID
                OnPurchaseItemFail();
#elif UNITY_IPHONE
                OnPurchaseItemSucc();
#endif
            }
            mCurItem = null;
            mCurTransation = null;
        }

        private void OnPurchaseItemSucc()
        {
            var responseData = new Hashtable();
            responseData["transactionId"] = mCurTransation.transactionId;
            mCurTransation.payload = EB.JSON.Stringify(responseData);
            mCurTransation.platform = Name;
            if (mConfig.Verify != null)
            {
                mConfig.Verify(mCurTransation);
            }
            else
            {
                Complete(mCurTransation);
            }
        }

        private void OnPurchaseItemFail()
        {
            if (mConfig.OnPurchaseFailed != null)
            {
                mConfig.OnPurchaseFailed("get error code from yijie sdk");
            }
        }

        public void OnPayCallback(string transactionId)
        {
            if (mCurItem != null && mCurTransation != null)
            {
                if (mCurTransation.transactionId == transactionId)
                {
                    EB.Debug.Log("YiJieSDKProvider.PurchaseItem: get pay callback from server");
                    if (mConfig != null && mConfig.Verify != null)
                    {
                        OnPurchaseItemCallback(EB.Sparx.YiJieSDKConfig.PAY_SUCCESS);
                    }
                }
            }
        }
    }
}
#endif