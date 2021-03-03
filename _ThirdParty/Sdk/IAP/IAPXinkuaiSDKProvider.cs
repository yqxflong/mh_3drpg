#if USE_XINKUAISDK
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using UnityEngine.Networking;

namespace EB.IAP.Internal
{
    public class XinkuaiSDKProvider : Provider
    {

        private Config mConfig = null;
        private Transaction mCurrent = null;
        private Item mCurrentItem = null;
        private RoleData m_RoleData = null;

        public static XinkuaiSDKProvider Create(Config config)
        {
            return new XinkuaiSDKProvider(config);
        }

        private XinkuaiSDKProvider(Config config)
        {
            mConfig = config;
        }


        public string Name
        {
            get
            {
#if USE_XINKUAIFX//标识字段
                return "xinkuaifx";
#else
                return "xinkuai";
#endif
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
                EB.Debug.LogWarning("XinkuaiSDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    OnPayResult(EB.Sparx.XinkuaiSDKResultCode.Succ);
                }
            }
            mCurrentItem = item;
            string ipv4 = EB.Net.IPManager.GetIP(Net.ADDRESSFAM.IPv4);
            EB.Debug.Log("----------ipv4 ={0}", ipv4);
            var extraInfo = new Hashtable
            {
                {"spbill_create_ip",ipv4}//是否有用需要测试
            };
            EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    EB.Debug.LogWarning("XinkuaiSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
                    if (mConfig != null && mConfig.OnPurchaseFailed != null)
                    {
                        mConfig.OnPurchaseFailed(error, null);
                    }
                    mCurrentItem = null;
                    return;
                }

                mCurrent = transaction;
                SparxHub.Instance.mBaseSdkManager.Pay(mCurrentItem, transaction, OnPayResult);
            }, extraInfo);
        }

        public void OnPayResult(int code)
        {
            if (code == EB.Sparx.XinkuaiSDKResultCode.Succ )
            {
                var responseData = new Hashtable();
                responseData["transactionId"] = mCurrent.transactionId;
                mCurrent.payload = EB.JSON.Stringify(responseData);
                mCurrent.platform = Name;
                if (mConfig.Verify != null)
                {
                    mConfig.Verify(mCurrent);
                }
                Complete(mCurrent);
            }
            //else if (code == EB.Sparx.XinkuaiSDKResultCode.Cancel)
            //{
            //    mConfig.OnPurchaseCanceled(mCurrent);
            //    Complete(mCurrent);
            //}
            else if (code == EB.Sparx.XinkuaiSDKResultCode.Failed|| code == EB.Sparx.XinkuaiSDKResultCode.Cancel)//取消也添加进订单校验中
            {
                mConfig.OnPurchaseFailed("Error Code From Xinkuai SDK——XinkuaiSDKResultCode.PayFailed", mCurrent);
                if (mConfig.VerifyAgain != null)
                {
                    mConfig.VerifyAgain(mCurrent);
                }
                Complete(mCurrent);
            }
            else if(code == EB.Sparx.XinkuaiSDKResultCode.Unknow)
            {
                mConfig.OnPurchaseFailed("Error Code From Xinkuai SDK——XinkuaiSDKResultCode.PayFailed:Unknow", mCurrent);
                Complete(mCurrent);
            }
            else
            {
                mConfig.OnPurchaseFailed("Error Code From Xinkuai SDK", mCurrent);
                Complete(mCurrent);
            }
        }
    }
}
#endif