#if USE_KUAIYONGSDK && UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class KuaiYongSDKProvider : Provider
	{
		private Config config;

		private Transaction curTransaction;
		private Item mCurrentItem = null;

		public static KuaiYongSDKProvider Create(Config config)
		{
			KuaiYongSDKProvider provider = new KuaiYongSDKProvider();
			provider.config = config;
			return provider;
		}

		public void Enumerate(List<Item> items)
		{
			EB.Debug.Log("KuaiYongSDKProvider.Enumerate!");

			foreach (var item in items)
			{
				item.valid = true;
			}

			if (config.OnEnumerate != null)
			{
				config.OnEnumerate();
			}
		}

		public void PurchaseItem(Item item)
		{
			EB.Debug.Log("KuaiYongSDKProvider.PurchaseItem");
			if (curTransaction != null)
			{
				EB.Debug.LogWarning("KuaiYongSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", curTransaction.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("KuaiYongSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}

			mCurrentItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("KuaiYongSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (config != null && config.OnPurchaseFailed != null)
					{
						config.OnPurchaseFailed(error);
					}
					mCurrentItem = null;
					return;
				}
				curTransaction = transaction;
				SparxHub.Instance.KuaiYongSDKManager.Pay(mCurrentItem, transaction.transactionId, OnPayResult);
			});
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("KuaiYongSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

			curTransaction = null;
			mCurrentItem = null;
		}

		public string Name
		{
			get
			{
				return "kuaiyong";
			}
		}

		private void OnPayResult(string resultCode)
		{
			switch (resultCode)
			{
				case "successed":
					OnItemPurchased();
					break;
				case "failed":
					OnItemPurchaseFailed();
					break;
				case "canceled":
					OnItemPurchaseCanceled();
					break;
			}
			curTransaction = null;
			mCurrentItem = null;
		}

		private void OnItemPurchased()
		{
			EB.Debug.Log("KuaiYongSDKProvider.OnItemPurchased");
			curTransaction.platform = Name;
			var responseData = new Hashtable();
			responseData["transactionId"] = curTransaction.transactionId;
			curTransaction.payload = EB.JSON.Stringify(responseData);
			if (config.Verify != null)
			{
				config.Verify(curTransaction);
			}
			else
			{
				Complete(curTransaction);
			}
		}

		private void OnItemPurchaseFailed()
		{
			EB.Debug.Log("KuaiYongSDKProvider.OnItemPurchaseFailed");
			if (config.OnPurchaseFailed != null)
			{
				config.OnPurchaseFailed("Get Failed Code from KuaiYongSDK");
			}
		}

		private void OnItemPurchaseCanceled()
		{
			EB.Debug.Log("KuaiYongSDKProvider.OnItemPurchaseCanceled");
			if (config.OnPurchaseCanceled != null)
			{
				config.OnPurchaseCanceled();
			}
		}

        public void OnPayCallback(string transactionId)
        {
            
        }
	}
}
#endif
