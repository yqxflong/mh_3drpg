#if UNITY_ANDROID && USE_VIVOSDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class VivoSDKProvider : Provider
	{
		private Config mConfig;
		private Transaction mCurTransaction;
		private Item mCurrentItem = null;

		public static VivoSDKProvider Create(Config config)
		{
			return new VivoSDKProvider(config);
		}

		public VivoSDKProvider(Config config)
		{
			mConfig = config;
		}

		public string Name
		{
			get
			{
				return "vivo";
			}
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("VivoSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

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
				EB.Debug.LogWarning("VivoSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurTransaction.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("VivoSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}

			mCurrentItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(item, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("VivoSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error);
					}
					mCurrentItem = null;
					return;
				}

				mCurTransaction = transaction;				
				SparxHub.Instance.VivoSDKManager.Pay(mCurrentItem, transaction, OnPurchaseItemCallback);
			});
		}

		private void OnPurchaseItemCallback(int code,object msg)
		{
			EB.Debug.LogWarning("OnPayResult: code={0}", code);
			if (code == EB.Sparx.VivoStatusCode.PAY_SUCCESS_CODE)
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
			else if (code == EB.Sparx.VivoStatusCode.PAY_CANCEL_CODE)
			{
				EB.Debug.LogWarning("OnPayResult: canceled msg={0}", msg);

				if (mConfig.OnPurchaseCanceled != null)
				{
					mConfig.OnPurchaseCanceled();
				}

			}
			else if (code == EB.Sparx.VivoStatusCode.PAY_FAIL_CODE)
			{
				EB.Debug.LogError("OnPayResult: PAY_FAIL msg={0}", msg);

				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed("vivo sdk pay failed");
				}
			}
			else
			{
				EB.Debug.LogError("OnPayResult: Pay Exception msg={0}", msg);

				string err = "Error Code: " + code;
				if (code == EB.Sparx.VivoStatusCode.CALLBACK_TIMEOUT)
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
	}
}
#endif