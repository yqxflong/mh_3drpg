#if USE_VFPKSDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.IAP.Internal
{
    public class VFPKSDKProvider : Provider
    {

        private Config mConfig = null;
        private Transaction mCurrent = null;
        private Item mCurrentItem = null;

        public static VFPKSDKProvider Create(Config config)
        {
            return new VFPKSDKProvider(config);
        }

        private VFPKSDKProvider(Config config)
        {
            mConfig = config;
        }


        public string Name
        {
            get
            {
                return "vfpk";
            }
        }

        public void Complete(Transaction transaction)
        {
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

        public string GetPayload(Transaction transaction)
        {
            return string.Empty;
        }

        public void OnPayCallbackFromServer(string transactionId)
        {

        }

        public void PurchaseItem(Item item)
        {
            if (mCurrent != null && mCurrentItem != null)
            {
                EB.Debug.LogWarning("VFPKSDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    OnPayResult(EB.Sparx.VFPKSDKResultCode.Succ);
                }
            }
            mCurrentItem = item;
            var extraInfo = new Hashtable
            {
                {"spbill_create_ip",Network.player.ipAddress}
            };
            EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    EB.Debug.LogWarning("VFPKSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
                    if (mConfig != null && mConfig.OnPurchaseFailed != null)
                    {
                        mConfig.OnPurchaseFailed(error, null);
                    }
                    mCurrentItem = null;
                    return;
                }

                mCurrent = transaction;
                SparxHub.Instance.VFPKSDKManager.Pay(mCurrentItem, transaction, OnPayResult);
            }, extraInfo);
        }

        public void OnPayResult(int code)
        {
            if (code == EB.Sparx.VFPKSDKResultCode.Succ)
            {
                var responseData = new Hashtable();
                responseData["transactionId"] = mCurrent.transactionId;
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
            }
            else if (code == EB.Sparx.VFPKSDKResultCode.Cancel)
            {
                mConfig.OnPurchaseCanceled(mCurrent);
                Complete(mCurrent);
            }
            else if (code == EB.Sparx.VFPKSDKResultCode.Failed)
            {
                mConfig.OnPurchaseFailed("Error Code From VFPK SDK——VFPKSDKResultCode.PayFailed", mCurrent);
                Complete(mCurrent);
            }
            else
            {
                mConfig.OnPurchaseFailed("Error Code From VFPK SDK", mCurrent);
                Complete(mCurrent);
            }
        }
    }
}
#endif
