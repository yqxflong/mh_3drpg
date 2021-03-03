#if USE_ASSDK && UNITY_IPHONE
using System.Collections;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class AsSDKProvider : Provider
	{
		private Config mConfig;

		private Transaction mTransaction;
		private Item mCurrentItem = null;

		public static AsSDKProvider Create(Config config)
		{
			AsSDKProvider provider = new AsSDKProvider();
			provider.mConfig = config;
			return provider;
		}

#region Provider implementation

		public void Enumerate(List<Item> items)
		{
			EB.Debug.Log("AsSDKProvider.Enumerate!");

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
			if (mTransaction != null)
			{
				EB.Debug.LogWarning("AsSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mTransaction.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("AsSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}

			mCurrentItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("AsSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error);
					}
					mCurrentItem = null;
					return;
				}

				mTransaction = transaction;
				SparxHub.Instance.AsSDKManager.Pay(mCurrentItem, transaction, OnPayResult);
			});
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("AsSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

			mTransaction = null;
			mCurrentItem = null;
		}

		public string Name
		{
			get { return "as"; }
		}

#endregion

		private void OnPayResult(string resultCode)
		{
			EB.Debug.Log("AsSDKProvider.OnPayResult resultCode = {0}", resultCode);
			switch (resultCode)
			{
				case "0":
					EB.Debug.Log("AsSDKProvider.OnPayResult resultCode = AsPaySucceedResult");
					OnItemPurchased();
					break;
				case "1":
					EB.Debug.Log("AsSDKProvider.OnPayResult resultCode = AsPayFailedResult");
					OnItemPurchaseFailed("Get Failed Result from AsSDK");
					break;
				case "6":
					EB.Debug.Log("AsSDKProvider.OnPayResult resultCode = AsPayUnknownResult");
					OnItemPurchased();
					break;
				default:
					OnItemPurchaseFailed("Get Undefined Code from AsSDK");
					break;
			}
		}

		private void OnItemPurchased()
		{
			EB.Debug.Log("OnIAPComplete:" + mTransaction.transactionId);

			var responseData = new Hashtable();
			responseData["transactionId"] = mTransaction.transactionId;
			mTransaction.payload = EB.JSON.Stringify(responseData);
			mTransaction.platform = Name;
			if (mConfig.Verify != null)
			{
				mConfig.Verify(mTransaction);
			}
			else
			{
				Complete(mTransaction);
			}

			mTransaction = null;
			mCurrentItem = null;
		}

		private void OnItemPurchaseFailed(string localizedError)
		{
			EB.Debug.LogError("Purchase failed: " + localizedError);

			if (mConfig.OnPurchaseFailed != null)
			{
				mConfig.OnPurchaseFailed(localizedError);
			}

			mTransaction = null;
			mCurrentItem = null;
		}

		private void OnItemPurchaseCanceled()
		{
			EB.Debug.Log("User canceled purchase");

			if (mConfig.OnPurchaseCanceled != null)
			{
				mConfig.OnPurchaseCanceled();
			}

			mTransaction = null;
			mCurrentItem = null;
		}

        public void OnPayCallback(string transactionId)
        {
            
        }
	}
}
#endif