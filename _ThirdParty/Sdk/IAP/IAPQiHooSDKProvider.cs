#if UNITY_ANDROID && USE_QIHOOSDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class QiHooSDKProvider : Provider
	{
		private Config mConfig;
		private Transaction mCurTransaction;
		private Item mCurrentItem = null;

		public static QiHooSDKProvider Create(Config config)
		{
			return new QiHooSDKProvider(config);
		}

		public QiHooSDKProvider(Config config)
		{
			mConfig = config;
		}

		public string Name
		{
			get
			{
				return "qihoo";
			}
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("QiHooSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

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
                EB.Debug.LogWarning("QiHooSDKProvider.PurchaseItem: get transaction not end");
                if (mConfig != null && mConfig.Verify != null)
                {
                    OnPayResult(EB.Sparx.QiHooPayStatusCode.SUCCESS);
                }
            }

			/*if (mCurTransaction != null)
			{
				EB.Debug.LogWarning("QiHooSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurTransaction.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("QiHooSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}*/

			mCurrentItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("QiHooSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error);
					}
					mCurrentItem = null;
					return;
				}

				mCurTransaction = transaction;
				SparxHub.Instance.QiHooSDKManager.Pay(mCurrentItem, transaction, OnPayResult);
			});
		}

		private void OnPayResult(int code)
		{
			EB.Debug.LogWarning("OnPayResult: code={0}", code);
			if (code == EB.Sparx.QiHooPayStatusCode.SUCCESS)
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
			else if (code == EB.Sparx.QiHooPayStatusCode.FAIL)
			{
				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed("qihoo sdk pay failed");
				}
			}
			else if (code == EB.Sparx.QiHooPayStatusCode.CANCEL)
			{
				if (mConfig.OnPurchaseCanceled != null)
				{
					mConfig.OnPurchaseCanceled();
				}
			}
			else if (code == EB.Sparx.QiHooPayStatusCode.PAYING)
			{
				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed("transaction is paying");
				}
			}
			else if (code == EB.Sparx.QiHooPayStatusCode.ACCESS_TOKEN_INVAILD)
			{
				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed("access_token invaild");
				}
			}
			else if (code == EB.Sparx.QiHooPayStatusCode.QT_INVAILD)
			{
				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed("login state invaild");
				}
			}
			else
			{
				EB.Debug.LogError("OnPayResult: error code={0}", code);

				string err = "Error Code: " + code;
				if (code == EB.Sparx.QiHooPayStatusCode.CALLBACK_TIMEOUT)
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
            if (mCurTransaction != null && mCurrentItem != null)
            {
                if (mCurTransaction.transactionId == transactionId)
                {
                    EB.Debug.Log("QiHooSDKProvider.PurchaseItem: get pay callback from server");
                    if (mConfig != null && mConfig.Verify != null)
                    {
                        OnPayResult(EB.Sparx.QiHooPayStatusCode.SUCCESS);
                    }
                }
            }
        }
    }
}

#endif
