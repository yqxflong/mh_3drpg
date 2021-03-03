#if UNITY_ANDROID && USE_ALIPAYSDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.IAP.Internal
{
    public class AlipaySDKProvider : Provider
    {
        private Config mConfig = null;
        private Transaction mCurrent = null;
        private Item mCurrentItem = null;

        public static AlipaySDKProvider Create(Config config)
        {
            return new AlipaySDKProvider(config);
        }

        private AlipaySDKProvider(Config config)
        {
            mConfig = config;
        }


        public string Name
        {
            get
            {
                return "alipay";
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
                EB.Debug.LogWarning("AlipaySDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    OnPayResult(EB.Sparx.AlipaySDKResultCode.Succ);
                }
            }
            mCurrentItem = item;
            string ipv4 = EB.Net.IPManager.GetIP(Net.ADDRESSFAM.IPv4);
            EB.Debug.Log("----------ipv4 ={0}", ipv4);
            var extraInfo = new Hashtable
            {
                {"spbill_create_ip",ipv4}
            };
            EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    EB.Debug.LogWarning("AlipaySDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
                    if (mConfig != null && mConfig.OnPurchaseFailed != null)
                    {
                        mConfig.OnPurchaseFailed(error, null);
                    }
                    mCurrentItem = null;
                    return;
                }

                mCurrent = transaction;
                SparxHub.Instance.AlipaySDKManager.Pay(mCurrentItem, transaction, OnPayResult);
            }, extraInfo);
        }

        public void OnPayResult(int code)
        {
            if (code == EB.Sparx.AlipaySDKResultCode.Succ)
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
            else if (code == EB.Sparx.AlipaySDKResultCode.Failed)
            {
                mConfig.OnPurchaseFailed("Error Code From Alipay SDK——AlipaySDKResultCode.Failed", mCurrent);
                Complete(mCurrent);
            }
            else
            {
                mConfig.OnPurchaseFailed("Error Code From Alipay SDK", mCurrent);
                Complete(mCurrent);
            }
        }
    }
}
#endif