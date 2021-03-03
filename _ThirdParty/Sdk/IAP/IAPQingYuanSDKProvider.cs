#if USE_QINGYUANSDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class QingYuanSDKProvider : Provider
	{
		private Config mConfig;
		private Transaction mCurTransaction;
		private Item mCurrentItem = null;

		public static QingYuanSDKProvider Create(Config config)
		{
			return new QingYuanSDKProvider(config);
		}

		public QingYuanSDKProvider(Config config)
		{
			mConfig = config;
		}

		public string Name
		{
			get
			{
				return "qingyuan";
			}
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("QingYuanSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

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
				EB.Debug.LogWarning("QingYuanSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurTransaction.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("QingYuanSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}
			mCurrentItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("QingYuanSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error);
					}
					mCurrentItem = null;
					return;
				}

				mCurTransaction = transaction;
				SparxHub.Instance.QingYuanSDKManager.Pay(mCurrentItem, transaction.transactionId, OnPayResult);
			});
		}

		private void OnPayResult(int code)
		{
			EB.Debug.LogWarning("OnPayResult: code={0}", code);
			if (code == EB.Sparx.QingYuanResultCode.RESULT_OK)
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
			else if (code == EB.Sparx.QingYuanResultCode.RESULT_FAIL)
			{
				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed("QingYuan sdk pay failed");
				}
			}
			else
			{
				EB.Debug.LogError("OnPayResult: error code={0}", code);

				string err = "Error Code: " + code;
				if (code == EB.Sparx.QingYuanResultCode.RESULT_TIMEOUT)
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