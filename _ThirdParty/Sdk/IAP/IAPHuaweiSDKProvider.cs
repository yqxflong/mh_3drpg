#if UNITY_ANDROID && USE_HUAWEISDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class HuaweiSDKProvider : Provider
	{
		private Config mConfig;
		private Transaction mCurTransaction;
		private Item mCurItem;

		public HuaweiSDKProvider(Config config)
		{
			mConfig = config;
		}

		public static HuaweiSDKProvider Create(Config config)
		{
			return new HuaweiSDKProvider(config);
		}

		public string Name
		{
			get
			{
				return "huawei";
			}
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("HuaweiSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

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
            if (mCurItem != null && mCurTransaction != null)
            {
                EB.Debug.LogWarning("HuaweiSDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    Hashtable data = new Hashtable();
                    data.Add("returnCode", "0");
                    data.Add("returnCode", "0");
                    OnPayResult(null, data);
                }
            }

            /*if (mCurTransaction != null)
			{
				EB.Debug.LogWarning("HuaweiSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurTransaction.transactionId);
				return;
			}

			if (mCurItem != null)
			{
				EB.Debug.LogWarning("HuaweiSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurItem.productId);
				return;
			}*/

            mCurItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("HuaweiSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error, mCurTransaction);
					}
					mCurItem = null;
					return;
				}

				mCurTransaction = transaction;
				SparxHub.Instance.HuaweiSDKManager.Pay(mCurItem, transaction, OnPayResult);
			});
		}

		private void OnPayResult(string err, object data)
		{
			if (err != null)
			{
				mConfig.OnPurchaseFailed(err, mCurTransaction);

				mCurTransaction = null;
				mCurItem = null;

				return;
			}

			string returnCode = EB.Dot.String("returnCode", data, string.Empty);
			string errMsg = EB.Dot.String("errMsg", data, string.Empty);
			if (returnCode == "0")
			{
				if (errMsg == "success")
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
			}
			else if (returnCode == EB.Sparx.HuaweiPayReturnCode.USER_CANCEL)
			{
				mConfig.OnPurchaseCanceled(mCurTransaction);

				mCurTransaction = null;
				mCurItem = null;

				return;
			}
			else
			{
				EB.Debug.LogError("HuaweiSDKProvider.OnPayResult  unknown code ,data = ", data);

				string errNotice = "pay failed sdk return data = " + data;

				if (returnCode == EB.Sparx.HuaweiPayReturnCode.CALLBACK_TIMEOUT)
				{
					errNotice = "ID_SPARX_IAP_CALLBACK_TIMEOUT";
				}
				mConfig.OnPurchaseFailed(errNotice, mCurTransaction);
			}

			mCurTransaction = null;
			mCurItem = null;
		}

        public void OnPayCallback(string transactionId)
        {
            if (mCurTransaction != null && mCurItem != null)
            {
                if (mCurTransaction.transactionId == transactionId)
                {
                    EB.Debug.Log("HuaweiSDKProvider.PurchaseItem: get pay callback from server");
                    if (mConfig != null && mConfig.Verify != null)
                    {
                        Hashtable data = new Hashtable();
                        data.Add("returnCode", "0");
                        data.Add("returnCode", "0");
                        OnPayResult(null, data);
                    }
                }
            }
        }

        public void OnPayCallbackFromServer(string transactionId)
        {
            throw new NotImplementedException();
        }

        public string GetPayload(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
