#if UNITY_ANDROID &&USE_WECHATSDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.IAP.Internal
{
    public class WeChatSDKProvider : Provider
    {
        private Config mConfig = null;
        private Transaction mCurrent = null;
        private Item mCurrentItem = null;

        public static WeChatSDKProvider Create(Config config)
        {
            return new WeChatSDKProvider(config);
        }

        private WeChatSDKProvider(Config config)
        {
            mConfig = config;
        }


        public string Name
        {
            get
            {
                return "wechat";
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
                EB.Debug.LogWarning("WeChatSDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    OnPayResult(EB.Sparx.WeChatSDKResultCode.Succ);
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
                    EB.Debug.LogWarning("WeChatSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
                    if (mConfig != null && mConfig.OnPurchaseFailed != null)
                    {
                        mConfig.OnPurchaseFailed(error, null);
                    }
                    mCurrentItem = null;
                    return;
                }

                mCurrent = transaction;
                SparxHub.Instance.WeChatSDKManager.Pay(mCurrentItem, transaction, OnPayResult);
            },extraInfo);
        }

        public void OnPayResult(int code)
        {
            if (code == EB.Sparx.WeChatSDKResultCode.Succ)
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
            else if (code == EB.Sparx.WeChatSDKResultCode.Cancel)
            {
                mConfig.OnPurchaseCanceled(mCurrent);
                Complete(mCurrent);
            }
            else if (code == EB.Sparx.WeChatSDKResultCode.PayFailed)
            {
                mConfig.OnPurchaseFailed("Error Code From Wechat SDK——WeChatSDKResultCode.PayFailed", mCurrent);
                Complete(mCurrent);
            }
            else if (code == EB.Sparx.WeChatSDKResultCode.WechatInstalledFailed)
            {
                SparxHub.Instance.WeChatSDKManager.OnShowTipCall("ID_WECHAT_NOT_FIND");
                mConfig.OnPurchaseFailed("Error Code From Wechat SDK——WeChatSDKResultCode.WechatInstalledFailed", mCurrent);
                Complete(mCurrent);
            }
            else
            {
                mConfig.OnPurchaseFailed("Error Code From Wechat SDK", mCurrent);
                Complete(mCurrent);
            }
        }
    }
}
#endif