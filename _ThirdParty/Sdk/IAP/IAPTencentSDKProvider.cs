#if UNITY_ANDROID && USE_TENCENTSDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
    public class TencentSDKProvider : Provider
    {
        private Config mConfig = null;
        private Transaction mCurTransaction = null;
        private Item mCurItem = null;

        public TencentSDKProvider(Config config)
        {
            mConfig = config;
        }

        public static TencentSDKProvider Create(Config config)
        {
            return new TencentSDKProvider(config);
        }

        public string Name
        {
            get
            {
                return "tencent";
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

        public void PurchaseItem(Item item)
        {
            if (mCurTransaction != null && mCurItem != null)
            {
                EB.Debug.LogWarning("TencentSDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    RequestVerifyPayout();
                }
            }

            /*if (mCurTransaction != null)
            {
                EB.Debug.LogWarning("TencentSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurTransaction.transactionId);
                return;
            }

            if (mCurItem != null)
            {
                EB.Debug.LogWarning("TencentSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurItem.productId);

                return;
            }*/

            mCurItem = item;
            GetRecordData(delegate (object data)
            {
                EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurItem, delegate (string error, IAP.Transaction transaction)
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        EB.Debug.LogWarning("TencentSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
                        if (mConfig != null && mConfig.OnPurchaseFailed != null)
                        {
                            mConfig.OnPurchaseFailed(error);                            
                        }
                        mCurItem = null;
                        return;
                    }

                    mCurTransaction = transaction;
                    SparxHub.Instance.TencentSDKManager.Pay(mCurItem, transaction, OnPayResult);
                }, data);
            });
        }

        public void OnPayResult(int code, object data)
        {
            if (code == EB.Sparx.TencentPayRet.RET_SUCC)
            {
                int payState = Dot.Integer("payState", data, -1000);
                switch (payState)
                {
                    case EB.Sparx.TencentPayState.PAYSTATE_PAYSUCC:

                        RequestVerifyPayout();
                        return;
                        //break;

                    case EB.Sparx.TencentPayState.PAYSTATE_PAYCANCEL:

                        if (mConfig.OnPurchaseCanceled != null)
                        {
                            mConfig.OnPurchaseCanceled();
                        }
                        break;

                    case EB.Sparx.TencentPayState.PAYSTATE_PAYUNKOWN:
                        //用户支付结果未知，建议查询余额
                        if (mConfig.OnPurchaseFailed != null)
                        {
                            mConfig.OnPurchaseFailed("tencent sdk pay failed");
                        }
                        break;

                    case EB.Sparx.TencentPayState.PAYSTATE_PAYERROR:

                        if (mConfig.OnPurchaseFailed != null)
                        {
                            mConfig.OnPurchaseFailed("tencent sdk pay error");
                        }
                        break;

                    default:
                        if (mConfig.OnPurchaseFailed != null)
                        {
                            mConfig.OnPurchaseFailed("unknown paystate:" + payState);
                        }
                        break;
                }
            }
            else
            {
                int flag = Dot.Integer("flag", data, -1000);
                switch (flag)
                {
                    case EB.Sparx.TencentEFlag.Login_TokenInvalid:
                        //token 非法建议引导用户重登
                        if (mConfig.OnPurchaseFailed != null)
                        {
                            mConfig.OnPurchaseFailed("login token invalid");
                        }
                        break;

                    case EB.Sparx.TencentEFlag.Pay_User_Cancle:

                        if (mConfig.OnPurchaseCanceled != null)
                        {
                            mConfig.OnPurchaseCanceled();
                        }
                        break;

                    case EB.Sparx.TencentEFlag.Pay_Param_Error:

                        if (mConfig.OnPurchaseFailed != null)
                        {
                            mConfig.OnPurchaseFailed("tencent sdk pay param error");
                        }
                        break;

                    case EB.Sparx.TencentEFlag.Error:

                        if (mConfig.OnPurchaseFailed != null)
                        {
                            mConfig.OnPurchaseFailed("tencent sdk pay pay error");
                        }
                        break;

                    case EB.Sparx.TencentEFlag.Pay_Time_Out:
                        if (mConfig.OnPurchaseFailed != null)
                        {
                            mConfig.OnPurchaseFailed("ID_SPARX_IAP_CALLBACK_TIMEOUT");
                        }
                        break;

                    default:

                        if (mConfig.OnPurchaseFailed != null)
                        {
                            mConfig.OnPurchaseFailed("unknown flag:" + flag);
                        }
                        break;
                }
            }
            mCurTransaction = null;
            mCurItem = null;
        }

        private void RequestVerifyPayout()
        {
            GetRecordData(delegate (object data)
            {
                var responseData = data as Hashtable;
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
                mCurTransaction = null;
                mCurItem = null;
            });
        }

        private void GetRecordData(Action<object> callback)
        {
            var loginRecordData = Sparx.Hub.Instance.TencentSDKManager.GetLoginRecordWithoutPlatform();
            string platfrom = SparxHub.Instance.TencentSDKManager.Platform;
            string accessToken = Dot.String("accessToken", loginRecordData, string.Empty);
            string payToken = Dot.String("payToken", loginRecordData, string.Empty);
            string openId = Dot.String("openId", loginRecordData, string.Empty);
            string pf = Dot.String("pf", loginRecordData, string.Empty);
            string pfKey = Dot.String("pf_key", loginRecordData, string.Empty);
            var responseData = new Hashtable();
            responseData["platform"] = platfrom;
            responseData["pf"] = pf;
            responseData["pfkey"] = pfKey;
            responseData["openid"] = openId;
            if (platfrom == EB.Sparx.TencentSDKManager.QQPlatform)
            {
                responseData["openkey"] = payToken;
            }
            else if (platfrom == EB.Sparx.TencentSDKManager.WXPlatform)
            {
                responseData["openkey"] = accessToken;
            }
            callback(responseData);
        }

        public void OnPayCallback(string transactionId)
        {
            if (mCurTransaction != null && mCurItem != null)
            {
                if (mCurTransaction.transactionId == transactionId)
                {
                    EB.Debug.Log("TencentSDKProvider.PurchaseItem: get pay callback from server");
                    if (mConfig != null && mConfig.Verify != null)
                    {
                        RequestVerifyPayout();
                    }
                }
            }
        }
    }
}
#endif
