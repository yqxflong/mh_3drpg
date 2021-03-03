#if USE_XIAOMISDK && UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class XiaoMiSDKProvider : Provider
	{
		private Config mConfig = null;
		private Transaction mCurrent = null;
		private Item mCurrentItem = null;

		public static XiaoMiSDKProvider Create(Config config)
		{
			return new XiaoMiSDKProvider(config);
		}

		private XiaoMiSDKProvider(Config config)
		{
			mConfig = config;
		}

		public string Name
		{
			get { return "xiaomi"; }
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("XiaoMiSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

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

		public void PurchaseItem(Item item)
		{
			if (mCurrent != null)
			{
				EB.Debug.LogWarning("XiaoMiSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurrent.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("XiaoMiSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}

			mCurrentItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("XiaoMiSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error);
					}
					mCurrentItem = null;
					return;
				}

				mCurrent = transaction;
				SparxHub.Instance.XiaoMiSDKManager.Pay(mCurrentItem, transaction, OnPayResult);
			});
		}

		private void OnPayResult(int code, object jsonOrder)
		{
			if (code == EB.Sparx.XiaoMiStatusCode.MI_XIAOMI_GAMECENTER_SUCCESS)
			{
				var responseData = new Hashtable();
				responseData["transactionId"] = mCurrent.transactionId;
				responseData["uid"] = SparxHub.Instance.XiaoMiSDKManager.Uid;
				mCurrent.payload = EB.JSON.Stringify(responseData);
				mCurrent.platform = Name;
				if (mConfig.Verify != null)
				{
					mConfig.Verify(mCurrent);
				}
				else
				{
					Complete(mCurrent);
				}
			}
			else if (code == EB.Sparx.XiaoMiStatusCode.MI_XIAOMI_GAMECENTER_ERROR_PAY_CANCEL)
			{
				EB.Debug.LogWarning("OnPayResult: canceled code={0}", code);

				if (mConfig.OnPurchaseCanceled != null)
				{
					mConfig.OnPurchaseCanceled();
				}
			}
			else
			{
				EB.Debug.LogError("OnPayResult: error code={0}", code);

				string err = "Error Code: " + code;
				if (code == EB.Sparx.XiaoMiStatusCode.CALLBACK_TIMEOUT)
				{
					err = "ID_SPARX_IAP_CALLBACK_TIMEOUT";
				}

				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed(err);
				}
			}

			mCurrent = null;
			mCurrentItem = null;
		}
	}
}
#endif