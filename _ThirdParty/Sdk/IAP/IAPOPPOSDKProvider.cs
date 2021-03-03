#if UNITY_ANDROID && USE_OPPOSDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class OPPOSDKProvider : Provider
	{
		private Config mConfig;
		private Transaction mCurTransaction;
		private Item mCurrentItem = null;

		public static OPPOSDKProvider Create(Config config)
		{
			return new OPPOSDKProvider(config);
		}

		public OPPOSDKProvider(Config config)
		{
			mConfig = config;
		}

		public string Name
		{
			get
			{
				return "oppo";
			}
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("OPPOSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

			mCurTransaction = null;
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
            if (mCurrentItem != null && mCurTransaction != null)
            {
                EB.Debug.LogWarning("OPPOSDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    OnPurchaseItemCallback(EB.Sparx.OPPOStatusCode.SUCCESS);
                }
            }

            /*if (mCurTransaction != null)
			{
				EB.Debug.LogWarning("OPPOSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurTransaction.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("OPPOSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}*/

            mCurrentItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("OPPOSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error);
					}
					mCurrentItem = null;
					return;
				}

				mCurTransaction = transaction;
				SparxHub.Instance.OPPOSDKManager.Pay(mCurrentItem, transaction, OnPurchaseItemCallback);
			});
		}

		private void OnPurchaseItemCallback(int code)
		{
			EB.Debug.LogWarning("OnPayResult: code={0}", code);
			if (code == EB.Sparx.OPPOStatusCode.SUCCESS)
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
			else
			{
				EB.Debug.LogError("OnPayResult: error code={0}", code);

				string err = "Error Code: " + code;
				if (code == EB.Sparx.OPPOStatusCode.PAY_TIMEOUT)
				{
					err = "ID_SPARX_IAP_CALLBACK_TIMEOUT";
				}

				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed(err);
				}

				//if (mConfig.OnPurchaseFailed != null)
				//{
				//    mConfig.OnPurchaseFailed("oppo sdk pay failed");
				//}
			}

			mCurTransaction = null;
			mCurrentItem = null;
		}

        public void OnPayCallback(string transactionId)
        {
            if (mCurTransaction != null && mCurrentItem != null)
            {
                if (mCurTransaction.transactionId == transactionId)
                {
                    EB.Debug.Log("OPPOSDKProvider.PurchaseItem: get pay callback from server");
                    if (mConfig != null && mConfig.Verify != null)
                    {
                        OnPurchaseItemCallback(EB.Sparx.OPPOStatusCode.SUCCESS);
                    }
                }
            }
        }
    }
}
#endif