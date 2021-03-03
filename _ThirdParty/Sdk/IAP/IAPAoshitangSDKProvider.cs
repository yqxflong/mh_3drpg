#if USE_AOSHITANGSDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EB.Sparx;

namespace EB.IAP.Internal
{
    public class IAPAoshitangSDKProvider : Provider
    {
        public string Name
        {
            get
            {
                return "aoshitang";
            }
        }
        private Config mConfig = null;
        private Transaction mCurrent = null;
        private Item mCurrentItem = null;
        private RoleData m_Roledata = null;
        private string t_classname = "IAPAoshitangSDKProvider";


        public static IAPAoshitangSDKProvider Create(Config _config)
        {
            return new IAPAoshitangSDKProvider(_config);
        }

        public IAPAoshitangSDKProvider(Config _config)
        {
            mConfig = _config;
        }

        public void Complete(Transaction transaction)
        {
            mCurrent = null;
            mCurrentItem = null;

        }

        public void Enumerate(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].valid = true;
            }
            if(mConfig.OnEnumerate != null)
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
            if(mCurrent != null && mCurrentItem != null)
            {
#if !UNITY_IOS
                EB.Debug.LogWarning("{0}.PurchaseItem: get transcation not end", t_classname);
                if(mConfig!=null && mConfig.Verify != null)
                {
                    OnPayResult(SparxAoshitangSDKManager.RESULT_SUCESS);
                }
#else
 Complete(mCurrent);
#endif
            }
            mCurrentItem = item;
            string ipv4 = EB.Net.IPManager.GetIP(Net.ADDRESSFAM.IPv4);
            var extraInfo = new Hashtable
            {
                { "spbill_creat_ip",ipv4}
            };
            EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error,IAP.Transaction transaction)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    EB.Debug.LogError("{0}.PurchaseItem Request Buypayout return error: {1}", error.ToString());
                    if (mConfig != null && mConfig.OnPurchaseFailed != null)
                    {
                        mConfig.OnPurchaseFailed(error, null);
                    }
                    mCurrentItem = null;
                    return;
                }
                mCurrent = transaction;
                Hub.Instance.mBaseSdkManager.Pay(mCurrentItem, transaction, OnPayResult);
            }, extraInfo);
        }

        public void OnPayResult(int code)
        {
#region 组装订单号
            var responseData = new Hashtable();
            responseData["transactionId"] = mCurrent.transactionId;
            mCurrent.payload = EB.JSON.Stringify(responseData);
            mCurrent.platform = Name;
#endregion
            
            if(code == SparxAoshitangSDKManager.RESULT_SUCESS)
            {
                if(mConfig.Verify != null)
                {
                    mConfig.Verify(mCurrent);
                }
                Complete(mCurrent);
            }
#if !UNITY_IOS
            else if(code == SparxAoshitangSDKManager.RESULT_FAILED)
            {
                mConfig.OnPurchaseFailed("Error Code From Aoshitang SDK Pay failed", mCurrent);
                if (mConfig.VerifyAgain != null)
                {
                    mConfig.VerifyAgain(mCurrent);
                }
                Complete(mCurrent);
            }
#endif
            else
            {
                mConfig.OnPurchaseFailed("Error Code From Aoshitang SDK Pay failed", mCurrent);
                Complete(mCurrent);
            }
        }
    }
}
#endif