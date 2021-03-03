#if USE_AMAZON && UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.amazon.device.iap.cpt;
using System;

namespace EB.IAP.Internal
{
	public class AmazonProvider : MonoBehaviour, Internal.Provider
	{
		private static Config _config;

		private IAmazonIapV2 _iapService;
		private List<Item> 	_items;
		private string 		_userId = null;

		private const int kMaxRetryCount = 3;

		public static AmazonProvider Create( Config config)
		{
			_config 	= config;
			var go 		= new GameObject("iap_plugin_kindle" );
			return go.AddComponent<IAPAmazonProvider>();
		}

		void Awake()
		{
			EB.Debug.Log ("AmazonProvider.Awake : DontDestroyOnLoad");
			DontDestroyOnLoad(this);

			_iapService = AmazonIapV2Impl.Instance;
		}

		void OnEnable()
		{
			EB.Debug.Log("AmazonIAPOnEnable");
			// Listen to all events for illustration purposes
			_iapService.AddGetUserDataResponseListener(GetUserDataResponseCallback);
			_iapService.AddPurchaseResponseListener(PurchaseResponseCallback);
			_iapService.AddGetProductDataResponseListener(GetProductDataResponseCallback);
			_iapService.AddGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseCallback);
		}

		void OnDisable()
		{
			EB.Debug.Log("AmazonIAPOnDisable");
			// Remove all event handlers
			_iapService.RemoveGetUserDataResponseListener(GetUserDataResponseCallback);
			_iapService.RemovePurchaseResponseListener(PurchaseResponseCallback);
			_iapService.RemoveGetProductDataResponseListener(GetProductDataResponseCallback);
			_iapService.RemoveGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseCallback);
		}

		private void GetUserDataResponseCallback(GetUserDataResponse eventName)
		{
			if (eventName.Status == "NOT_SUPPORTED")
			{
				EB.Debug.LogError("GetUserDataResponseCallback: NOT_SUPPORTED");
				return;
			}

			if (eventName.Status == "FAILED")
			{
				EB.Debug.LogError("GetUserDataResponseCallback: FAILED");
				return;
			}

			EB.Debug.Log("GetUserDataResponseCallback: user = {0}", eventName.ToJson());
			_userId = eventName.AmazonUserData.UserId;
		}

		private void PurchaseResponseCallback(PurchaseResponse eventName)
		{
			if (eventName.Status == "NOT_SUPPORTED")
			{
				EB.Debug.LogError("PurchaseResponseCallback: NOT_SUPPORTED");

				if (_config.OnPurchaseFailed != null)
				{
					_config.OnPurchaseFailed(eventName.Status);
				}
				return;
			}

			if (eventName.Status == "FAILED")
			{
				EB.Debug.LogError("PurchaseResponseCallback: FAILED");

				if (_config.OnPurchaseFailed != null)
				{
					_config.OnPurchaseFailed(eventName.Status);
				}

				return;
			}

			if (eventName.PurchaseReceipt.CancelDate > 0)
			{
				if (_config.OnPurchaseCanceled != null)
				{
					_config.OnPurchaseCanceled();
				}

				return;
			}

			//Note: sounds like this could be called as soon as the customer connects if they didn't receive their success response last time they bought something (e.g. connectivity loss or device shuts down) 
			var receipt = eventName.PurchaseReceipt;
			EB.Debug.Log("purchaseSuccessfulEvent: " + receipt.ToJson());

			Transaction transaction = new Transaction();
			transaction.transactionId = receipt.ReceiptId;
			transaction.signature = "";

			// doing this here so we don't have to modify the Amazon plugin code.
			Hashtable payload = new Hashtable();
			payload["sku"] = receipt.Sku;
			payload["userId"] = eventName.AmazonUserData.UserId;

			transaction.payload = EB.JSON.Stringify(payload);
			transaction.productId = receipt.Sku;

			if (_config.ReceiptPersistance != null)
			{
				_config.ReceiptPersistance.AddPendingPurchaseReceipt(eventName.ToJson(), string.Empty);   // save the token in case it doesn't get verified correctly, so we can retry later.
			}
			else
			{
				EB.Debug.Log("++++++++ NO RECEIPT PERSISTANCE!!");
			}

			if (_config.Verify != null)
			{
				_config.Verify(transaction);
			}
			else
			{
				Complete(transaction);
			}
		}

		private void GetProductDataResponseCallback(GetProductDataResponse eventName)
		{
			if (eventName.Status == "NOT_SUPPORTED")
			{
				EB.Debug.LogError("GetProductDataResponseCallback: NOT_SUPPORTED");

				if (_items != null)
				{
					foreach (Item item in _items)
					{
						item.valid = false;
					}

					if (_config.OnEnumerate != null)
					{
						_config.OnEnumerate();
					}
				}

				return;
			}

			if (eventName.Status == "FAILED")
			{
				EB.Debug.LogError("GetProductDataResponseCallback: FAILED");

				if (_items != null)
				{
					foreach (Item item in _items)
					{
						item.valid = false;
					}

					if (_config.OnEnumerate != null)
					{
						_config.OnEnumerate();
					}
				}

				return;
			}

			if (_items != null && eventName.ProductDataMap != null)
			{
				EB.Debug.Log("GetProductDataResponseCallback: products = {0}", eventName.ToJson());
				foreach(Item item in _items)
				{
					if (eventName.ProductDataMap.ContainsKey(item.productId))
					{
						ProductData productData = eventName.ProductDataMap[item.productId];
						item.valid = true;
						item.localizedTitle = productData.Title;
						item.localizedDesc = productData.Description;
						item.localizedCost = productData.Price;

						//TODO: amazon doesn't seem to provide a currency code, or cents value
						//item.currencyCode = "";

						EB.Debug.Log("parsing cents: {0}", productData.Price);
						float costValue = 0.0f;
						string price = productData.Price;

						//TODO: need to make sure this works for all currency
						if (float.TryParse(price, out costValue))
						{
							item.cents = (int)(costValue * 100);
							EB.Debug.Log("cents: {0}", item.cents);
							break;
						}
						else
						{
							item.cents = (int)item.cost * 100;
						}

						EB.Debug.Log("Cost {0}", item.cost);
						EB.Debug.Log("item currency code {0}", item.currencyCode);

						//TODO: Are there other item members that need filling here?
					}
				}

				if (_config.OnEnumerate != null)
				{
					_config.OnEnumerate();
				}

				// now that we've successfully called Amazon, AmazonIAP.initiateItemDataRequest we are allowed to start other AmazonIAP operations.
				Coroutines.Run(RequestAmazonUserId());
			}
		}

		private void GetPurchaseUpdatesResponseCallback(GetPurchaseUpdatesResponse eventName)
		{
			if (eventName.Status == "NOT_SUPPORTED")
			{
				EB.Debug.LogError("GetPurchaseUpdatesResponseCallback: NOT_SUPPORTED");
				return;
			}

			if (eventName.Status == "FAILED")
			{
				EB.Debug.LogError("GetPurchaseUpdatesResponseCallback: FAILED");
				return;
			}

			EB.Debug.Log("GetPurchaseUpdatesResponseCallback. receipts count: {0}", eventName.Receipts.Count);
			foreach (var receipt in eventName.Receipts)
			{
				EB.Debug.Log("Purchase Update Receipt: {0}", receipt.ToJson());
				PurchaseResponseCallback(new PurchaseResponse()
				{
					AmazonUserData = eventName.AmazonUserData,
					Status = eventName.Status,
					PurchaseReceipt = receipt,
				});
			}

			if (eventName.HasMore)
			{
				ResetInput input = new ResetInput() { Reset = false };
				_iapService.GetPurchaseUpdates(input);
			}
		}

		private IEnumerator RetryPendingPurchases()
		{
			EB.Debug.Log("Retrying Amazon pending PURCHASES");
			int retryCount = 0;
			float waitSeconds = 1.0f;
			while (retryCount < kMaxRetryCount)
			{
				if (_config.ReceiptPersistance == null)
				{
					EB.Debug.Log("NO RECEIPT PERSISTANCE");
					break;
				}
				Hashtable pendingPurchases = _config.ReceiptPersistance.GetPendingPurchaseReceipts();
				if (pendingPurchases.Count <= 0)
				{
					EB.Debug.Log("No Pending Purchases! Done!");
					break;
				}
				Hashtable pendingPurchasesCopy = new Hashtable(pendingPurchases);
				for (IDictionaryEnumerator iter = pendingPurchasesCopy.GetEnumerator(); iter.MoveNext();)
				{
					string receiptString = (string)iter.Key;
					string sku = (string)iter.Value;

					EB.Debug.Log("Retrying Amazon pending purchase token "+receiptString+" - "+sku);

					PurchaseResponseCallback(PurchaseResponse.CreateFromJson(receiptString));
				}
				yield return new WaitForSeconds(waitSeconds);
				waitSeconds *= 2.0f;
				retryCount++;
			}
			EB.Debug.Log("Retrying Amazon pending PURCHASES DONE");
		}

		private IEnumerator RequestAmazonUserId()
		{
			EB.Debug.Log("Getting Amazon User ID");
			int retryCount = 0;
			float waitSeconds = 1.0f;
			while (retryCount < kMaxRetryCount && string.IsNullOrEmpty(_userId))
			{
				_iapService.GetUserData();
				yield return new WaitForSeconds(waitSeconds);
				waitSeconds *= 2.0f;
				retryCount++;
			}
			if (!string.IsNullOrEmpty(_userId))
			{
				Coroutines.Run(RetryPendingPurchases());

				ResetInput input = new ResetInput() { Reset = false };
				_iapService.GetPurchaseUpdates(input);
			}
			EB.Debug.Log("RequestAmazonUserId Finished - Result: {0}", (string.IsNullOrEmpty(_userId) ? "failed." : _userId));
		}

		#region Provider implementation
		public void PurchaseItem( Item item )
		{
			EB.Debug.Log("PurchaseItem: " + item.productId);

			//TODO: if user doesn't have userid yet warning / retry

			SkuInput sku = new SkuInput();
			sku.Sku = item.productId;
			_iapService.Purchase(sku);
		}

		public void Enumerate (List<Item> items)
		{
			_items = items;

			List<string> productIds = ArrayUtils.Map<Item, string>( items, delegate(Item item)
			{
				return item.productId;
			});

			SkusInput skus = new SkusInput();
			skus.Skus = productIds;
			_iapService.GetProductData(skus);
		}

		public void Complete (Transaction transaction)
		{
			EB.Debug.Log ("AmazonProvider.Complete.");

			if (_config.ReceiptPersistance != null)
			{
				_config.ReceiptPersistance.RemovePendingPurchaseReceipt(transaction.transactionId);	// save the token in case it doesn't get verified correctly, so we can retry later.
				EB.Debug.Log("+++++++++++++ REMOVAL SUCCESSFUL");
			}
			else
			{
				EB.Debug.Log("++++++++ NO RECEIPT PERSISTANCE!!");
			}
		}

        public void OnPayCallback(string transactionId)
        {

        }

        public string Name
		{
			get
			{
				return "amazonapp";
			}
		}
		#endregion
	}
}

class IAPAmazonProvider : EB.IAP.Internal.AmazonProvider
{

}
#endif