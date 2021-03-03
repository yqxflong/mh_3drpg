#if USE_UCSDK && UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class UCSDKProvider : Provider
	{
		private Config mConfig = null;
		private Transaction mCurrent = null;
		private Item mCurrentItem = null;

		public static UCSDKProvider Create(Config config)
		{
			return new UCSDKProvider(config);
		}

		private UCSDKProvider(Config config)
		{
			mConfig = config;
		}

		public string Name
		{
			get { return "uc"; }
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("UCSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

			mCurrent = null;
			mCurrentItem = null;
		}

        public string GetPayload(Transaction transaction)
        {
            return null;
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
            if (mCurrent != null && mCurrentItem != null)
            {
                EB.Debug.LogWarning("UCSDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    OnPayResult(EB.Sparx.UCStatusCode.SUCCESS, string.Empty);
                }
            }

			/*if (mCurrent != null)
			{
				EB.Debug.LogWarning("UCSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurrent.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("UCSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}*/

			mCurrentItem = item;
            /*var extraInfo = new Hashtable
            {
                {"appId", SparxHub.Instance.UCSDKManager.AppId},
                {"userId",LoginManager.Instance.LocalUser.Id.Value}
            };*/
            EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("UCSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error,null);
					}
					mCurrentItem = null;
					return;
				}

				mCurrent = transaction;
				SparxHub.Instance.UCSDKManager.Pay(mCurrentItem, transaction, OnPayResult);
			},null);
		}

		private void OnPayResult(int code, object jsonOrder)
		{
			if (code == EB.Sparx.UCStatusCode.SUCCESS)
			{
				string orderId = EB.Dot.String("orderId", jsonOrder, string.Empty);
				float orderAmount = EB.Dot.Single("orderAmount", jsonOrder, 0.0f);
				int payWayId = EB.Dot.Integer("payWayId", jsonOrder, -1);
				string payWayName = EB.Dot.String("payWayName", jsonOrder, string.Empty);

				EB.Debug.Log("OnPayCallback: received order info: code={0}, orderId={1}, orderAmount={2:0.00}, payWayId={3}, payWayName={4}",
						code, orderId, orderAmount, payWayId, payWayName);
 
				var responseData = new Hashtable();
				responseData["transactionId"] = mCurrent.transactionId;
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
			else if (code == EB.Sparx.UCStatusCode.PAY_USER_EXIT)
			{
				EB.Debug.LogWarning("OnPayResult: canceled code={0}", code);

				if (mConfig.OnPurchaseCanceled != null)
				{
					mConfig.OnPurchaseCanceled(null);
				}
			}
			else
			{
				EB.Debug.LogError("OnPayResult: error code={0}", code);

				string err = "Error Code: " + code;
				if (code == EB.Sparx.UCStatusCode.CALLBACK_TIMEOUT)
				{
					err = "ID_SPARX_IAP_CALLBACK_TIMEOUT";
				}

				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed(err,null);
				}
			}

			mCurrent = null;
			mCurrentItem = null;
		}

        public void OnPayCallback(string transactionId)
        {
            if (mCurrent != null && mCurrentItem != null)
            {
                if (mCurrent.transactionId == transactionId)
                {
                    EB.Debug.Log("UCSDKProvider.PurchaseItem: get pay callback from server");
                    if (mConfig != null && mConfig.Verify != null)
                    {
                        OnPayResult(EB.Sparx.UCStatusCode.SUCCESS, string.Empty);
                    }
                }
            }
        }

        public void OnPayCallbackFromServer(string transactionId)
        {

        }
    }
}
#endif