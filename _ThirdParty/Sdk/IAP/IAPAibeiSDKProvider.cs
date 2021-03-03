#if USE_AIBEISDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
    public class AibeiSDKProvider : Provider
    {
        private Config mConfig = null;
        private Transaction mCurrent = null;
        private Item mCurrentItem = null;

        public static AibeiSDKProvider Create(Config config)
        {
            return new AibeiSDKProvider(config);
        }

        private AibeiSDKProvider(Config config)
        {
            mConfig = config;
        }

        public string Name
        {
            get
            {
                return "aibei";
            }
        }

        public void Complete(Transaction transaction)
        {
            EB.Debug.Log("AibeiSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

            mCurrent = null;
            mCurrentItem = null;
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
            if (mCurrent != null)
            {
                EB.Debug.LogWarning("AibeiSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurrent.transactionId);
                return;
            }

            if (mCurrentItem != null)
            {
                EB.Debug.LogWarning("AibeiSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
                return;
            }

            mCurrentItem = item;
            var extraInfo = new Hashtable
            {
                {"appId", SparxHub.Instance.AibeiSDKManager.AppId},
                {"userId",LoginManager.Instance.LocalUser.Id.Value}
            };
            Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    EB.Debug.LogWarning("AibeiSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
                    if (mConfig != null && mConfig.OnPurchaseFailed != null)
                    {
                        mConfig.OnPurchaseFailed(error, null);
                    }
                    mCurrentItem = null;
                    return;
                }
                mCurrent = transaction;
                Sparx.Hub.Instance.AibeiSDKManager.Pay(mCurrentItem, transaction, OnPayResult);
            }, extraInfo);
        }

        private void OnPayResult(int code)
        {
            EB.Debug.Log("OnPayCallback: received order info: code = {0}", code);

            if (code == EB.Sparx.AibeiConfig.PAY_SUCCESS)
            {
                OnPurchaseItemSucc();
            }
            else if (code == EB.Sparx.AibeiConfig.PAY_CANCEL)
            {
                OnPurchaseItemCancel();
            }
            else if (code == EB.Sparx.AibeiConfig.PAY_ERROR)
            {
                OnPurchaseItemFail();
            }
            else
            {
                OnPurchaseItemNoResult();
            }
        }
        public string GetPayload(Transaction transaction)
        {
            var responseData = new Hashtable();
            responseData["transactionId"] = transaction.transactionId;
            responseData["appid"] = SparxHub.Instance.AibeiSDKManager.AppId;

            return JSON.Stringify(responseData);
        }

        private void OnPurchaseItemSucc()
        {
            var responseData = new Hashtable();
            responseData["transactionId"] = mCurrent.transactionId;
            responseData["appid"] = SparxHub.Instance.AibeiSDKManager.AppId;
            mCurrent.payload = EB.JSON.Stringify(responseData);
            mCurrent.platform = Name;
            if (mConfig.Verify != null)
            {
                mConfig.Verify(mCurrent);
            }
            else
            {
                Complete(mCurrent);
            }
            CloseTran();
        }

        private void OnPurchaseItemCancel()
        {
            if (mConfig.OnPurchaseCanceled != null)
            {
                mConfig.OnPurchaseCanceled(mCurrent);
                CloseTran();
            }
        }

        private void OnPurchaseItemFail()
        {
            if (mConfig.OnPurchaseFailed != null)
            {
                mConfig.OnPurchaseFailed("get error code from aibei sdk", mCurrent);
                CloseTran();
            }
        }

        private void OnPurchaseItemNoResult()
        {
            if (mConfig.OnPurchaseNoResult != null)
            {
                mConfig.OnPurchaseNoResult(delegate
                {
                    OnPurchaseItemSucc();
                });
            }
        }

        private void CloseTran()
        {
            mCurrent = null;
            mCurrentItem = null;
        }
        
        public void OnPayCallbackFromServer(string transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
#endif