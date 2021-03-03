#if USE_K7KSDK
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public class K7KSDKProvider : Provider
	{
		private Config mConfig;
		private Transaction mCurTransaction;
		private Item mCurrentItem = null;

		public static K7KSDKProvider Create(Config config)
		{
			return new K7KSDKProvider(config);
		}

		public K7KSDKProvider(Config config)
		{
			mConfig = config;
		}

		public string Name
		{
			get
			{
				return "k7k";
			}
		}

		public void Complete(Transaction transaction)
		{
			EB.Debug.Log("K7KSDKProvider.Complete: transaction {0} completed", transaction.transactionId);

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
				EB.Debug.LogWarning("K7KSDKProvider.PurchaseItem: current is purchasing transactionId = {0}", mCurTransaction.transactionId);
				return;
			}

			if (mCurrentItem != null)
			{
				EB.Debug.LogWarning("K7KSDKProvider.PurchaseItem: current is purchasing productId = {0}", mCurrentItem.productId);
				return;
			}
			mCurrentItem = item;
			EB.Sparx.Hub.Instance.WalletManager.BuyPayout(mCurrentItem, delegate (string error, IAP.Transaction transaction)
			{
				if (!string.IsNullOrEmpty(error))
				{
					EB.Debug.LogWarning("K7KSDKProvider.PurchaseItem RequestBuyPayout GetCallback error = {0}", error);
					if (mConfig != null && mConfig.OnPurchaseFailed != null)
					{
						mConfig.OnPurchaseFailed(error);
					}
					mCurrentItem = null;
					return;
				}

				mCurTransaction = transaction;
				SparxHub.Instance.K7KSDKManager.Pay(mCurrentItem, transaction.transactionId, OnPayResult);
			});
		}

		private void OnPayResult(int code)
		{
			EB.Debug.LogWarning("OnPayResult: code={0}", code);
			if (code == EB.Sparx.K7KResultCode.RESULT_OK)
			{
				OnPurchaseItemSucc();
            }
			else if (code == EB.Sparx.K7KResultCode.RESULT_FAIL)
			{
				OnPurchaseItemFail();
            }
			else if(code == EB.Sparx.K7KResultCode.RESULT_CANCEL)
			{
				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed("k7k sdk pay cancel");
				}
				CloseTransation();
			}
			else if (code == EB.Sparx.K7KResultCode.PAY_NO_RESULT)
			{
				OnPurchaseItemNoResult();
            }
            else
			{
				EB.Debug.LogError("OnPayResult: error code={0}", code);

				string err = "Error Code: " + code;
				if (mConfig.OnPurchaseFailed != null)
				{
					mConfig.OnPurchaseFailed(err);
				}
			}
		}


		private void OnPurchaseItemSucc()
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
			CloseTransation();
		}

		private void OnPurchaseItemFail()
		{
			if (mConfig.OnPurchaseFailed != null)
			{
				mConfig.OnPurchaseFailed("get error code from k7k sdk");
				CloseTransation();
			}
		}

		private void OnPurchaseItemNoResult()
		{
			if (mConfig.OnPurchaseNoResult != null)
			{
				mConfig.OnPurchaseNoResult(delegate
				{
					OnPurchaseItemSucc();
				});
			}
		}

		private void CloseTransation()
		{
			mCurrentItem = null;
			mCurTransaction = null;
		}

        public void OnPayCallback(string transactionId)
        {
            
        }
	}
}

#endif