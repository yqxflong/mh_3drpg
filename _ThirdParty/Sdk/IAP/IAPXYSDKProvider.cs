#if USE_XYSDK && UNITY_IPHONE
using System.Collections;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class XYSDKProvider : Provider
	{
		private Config config;

		private Item curItem;
		private Transaction curTransaction;

		public static XYSDKProvider Create(Config config)
		{
			XYSDKProvider provider = new XYSDKProvider();
			provider.config = config;
			return provider;
		}

		public void Enumerate(List<Item> items)
		{
			EB.Debug.Log("XYSDKProvider.Enumerate!");

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
			EB.Debug.Log("XYSDKProvider.PurchaseItem");
			if (curTransaction != null)
			{
				EB.Debug.LogWarning("XYSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", curTransaction.transactionId);
				return;
			}

			if (curItem != null)
			{
				EB.Debug.LogWarning("XYSDKProvider.PurchaseItem: current is purchasing productId = {0}", curItem.productId);
				return;
			}

			curItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(curItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("XYSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (config != null && config.OnPurchaseFailed != null)
					{
						config.OnPurchaseFailed(error);
					}
					curItem = null;
					return;
				}
				curTransaction = transaction;
				SparxHub.Instance.XYSDKManager.Pay(curItem, transaction.transactionId, OnPayResult);
			});
		}


		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("XYSDKProvider.Complete: transaction {0} completed", transaction.transactionId);
			curTransaction = null;
			curItem = null;
		}

		public string Name
		{
			get
			{
				return "xy";
			}
		}

		private void OnPayResult(string resultCode, string orderId)
		{
			switch (resultCode)
			{
				case "successed":
					OnItemPurchased(orderId);
					break;
				case "failed":
					OnItemPurchaseFailed();
					break;
				case "canceled":
					OnItemPurchaseCanceled();
					break;
			}
			curTransaction = null;
			curItem = null;
		}

		private void OnItemPurchased(string orderId)
		{
			EB.Debug.Log("XYSDKProvider.OnItemPurchased: orderId = {0}", orderId);
			curTransaction.platform = Name;
			var responseData = new Hashtable();
			responseData["orderId"] = orderId;
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
			EB.Debug.Log("XYSDKProvider.OnItemPurchaseFailed");
			if (config.OnPurchaseFailed != null)
			{
				config.OnPurchaseFailed("Get Failed Code from XYSDK");
			}
		}

		private void OnItemPurchaseCanceled()
		{
			EB.Debug.Log("XYSDKProvider.OnItemPurchaseCanceled");
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
