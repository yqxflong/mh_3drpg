#if UNITY_ANDROID && USE_M4399SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.IAP.Internal
{
    public class M4399SDKProvider : Provider
    {
        private Config mConfig = null;

        private Transaction mCurTransaction = null;

        private Item mCurItem = null;

        public M4399SDKProvider(Config config)
        {
            mConfig = config;
        }

        public static M4399SDKProvider Create(Config config)
        {
            return new M4399SDKProvider(config);
        }

        public string Name
        {
            get
            {
                return "m4399";
            }
        }

        public void Complete(Transaction transaction)
        {
            mCurTransaction = null;
            mCurItem = null;
        }

        public void Enumerate(List<Item> items)
        {
            foreach (var item in items)
            {
                item.valid = true;
            }
            if (mConfig != null)
            {
                mConfig.OnEnumerate();
            }
        }

        public string GetPayload(Transaction transaction)
        {
            return string.Empty;
        }

        public void OnPayCallbackFromServer(string transactionId)
        {
            if (mCurItem != null && mCurTransaction != null)
            {
                if (mCurTransaction.transactionId == transactionId)
                {
                    EB.Debug.Log("M4399SDKProvider.PurchaseItem: get pay callback from server");
                    if (mConfig != null && mConfig.Verify != null)
                    {
                        OnPurchaseItemCallback(EB.Sparx.M4399ResultCode.Succ);
                    }
                }
            }
        }

        public void PurchaseItem(Item item)
        {
            if (mCurItem != null && mCurTransaction != null)
            {
                EB.Debug.LogWarning("M4399SDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    OnPurchaseItemCallback(EB.Sparx.M4399ResultCode.Succ);
                }
            }

            mCurItem = item;
            EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurItem, delegate (string error, IAP.Transaction transaction)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    EB.Debug.LogWarning("M4399SDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
                    if (mConfig != null && mConfig.OnPurchaseFailed != null)
                    {
                        mConfig.OnPurchaseFailed(error, mCurTransaction);
                    }
                    mCurItem = null;
                    return;
                }

                mCurTransaction = transaction;
                SparxHub.Instance.M4399SDKManager.Pay(mCurItem, transaction, OnPurchaseItemCallback);
            });
        }

        public void OnPurchaseItemCallback(int code)
        {
            if (code == EB.Sparx.M4399ResultCode.Succ)
            {
                var responseData = new Hashtable();
                responseData["transactionId"] = mCurTransaction.transactionId;
                mCurTransaction.payload = EB.JSON.Stringify(responseData);
                mCurTransaction.platform = Name;
                if (mConfig.Verify != null)
                {
                    mConfig.Verify(mCurTransaction);
                }
                else
                {
                    Complete(mCurTransaction);
                }
            }
            else if (code == EB.Sparx.M4399ResultCode.Cancel)
            {
                mConfig.OnPurchaseCanceled(mCurTransaction);
            }
            else
            {
                mConfig.OnPurchaseFailed("get error code from yijie sdk", mCurTransaction);

            }
        }
    }
}
#endif