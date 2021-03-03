#if UNITY_ANDROID && USE_LBSDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class LBSDKProvider : Provider
	{
		private Config mConfig;
		private Transaction mCurTransaction;
		private Item mCurrentItem = null;

		public static LBSDKProvider Create(Config config)
		{
			return new LBSDKProvider(config);
		}

		public LBSDKProvider(Config config)
		{
			mConfig = config;
		}

		public string Name
		{
			get
			{
				return "lb";
			}
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("LBSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

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
			if (mCurTransaction != null)
			{
				EB.Debug.LogWarning("LBSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurTransaction.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("LBSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}

			mCurrentItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(item, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("LBSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error);
					}
					mCurrentItem = null;
					return;
				}

				mCurTransaction = transaction;				
				SparxHub.Instance.LBSDKManager.Pay(mCurrentItem, transaction, OnPurchaseItemCallback);
			});
		}

		private void OnPurchaseItemCallback(int code,object data)
		{
			EB.Debug.LogWarning("OnPayResult: code={0}", code);
			if (code == EB.Sparx.LBStatusCode.SUCCESS)
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
			else if (code == EB.Sparx.LBStatusCode.Cancel)
			{
				EB.Debug.LogWarning("OnPayResult: canceled msg={0}", data);

				if (mConfig.OnPurchaseCanceled != null)
				{
					mConfig.OnPurchaseCanceled();
				}

			}
			else if (code == EB.Sparx.LBStatusCode.FAIL)
			{
				EB.Debug.LogError("OnPayResult: PAY_FAIL msg={0}", data);

				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed("LB sdk pay failed");
				}
			}
			else
			{
				EB.Debug.LogError("OnPayResult: Pay Exception msg={0}", data);

				string err = "Error Code: " + code;
				if (code == EB.Sparx.LBStatusCode.CALLBACK_TIMEOUT)
				{
					err = "ID_SPARX_IAP_CALLBACK_TIMEOUT";
				}

				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed(err);
				}
			}

			mCurTransaction = null;
			mCurrentItem = null;
		}

        public void OnPayCallback(string transactionId)
        {
            
        }
	}
}
#endif