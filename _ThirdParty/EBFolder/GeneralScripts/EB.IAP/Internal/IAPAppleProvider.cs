using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if UNITY_IPHONE
namespace EB.IAP.Internal
{
	class AppleProvider :  MonoBehaviour, Provider
	{
		private static Config _config;
		private static bool _init = false;

		[DllImport("__Internal")]
		static extern void _IAPInitialize();

		[DllImport("__Internal")]
		static extern void _IAPEnumerate(string offerString);

		[DllImport("__Internal")]
		static extern void _IAPPurchase(string offerId);

		[DllImport("__Internal")]
		static extern void _IAPComplete(string identifier);

		public static AppleProvider Create( Config config )
		{
			_config 	= config;
			var go 		= new GameObject("iap_plugin_apple" );
			DontDestroyOnLoad(go);
			return go.AddComponent<IAPAppleProvider>();
		}
		
		event System.Action<string> _onEnumerate;
		
		void Awake()
		{
			if (!_init)
			{
				_init = true;
				_IAPInitialize();
			}
		}

        #region Provider implementation
        public void PurchaseItem (Item item)
		{
            EB.Debug.Log("---Apple---PurchaseItem():调用苹果的支付SDK,productId:{0}", item.productId);
            _IAPPurchase(item.productId);
		}

        private bool hasEnumrate = false;
        public void Enumerate (List<Item> items)
		{
            if (hasEnumrate)
            {
                EB.Debug.Log("---Apple---Enumerate()");
                foreach (var item in items)
                {
                    item.valid = true;
                }

                if (_config.OnEnumerate != null)
                {
                    _config.OnEnumerate();
                }
            }
            else
            {
                var productIds = ArrayUtils.Map<Item, string>(items, delegate (Item item)
                {
                    return item.productId;
                });
                var offersString = ArrayUtils.Join(productIds, ',');

                _onEnumerate += delegate (string json)
                {
                    var objects = JSON.Parse(json) as ArrayList;

                    foreach (var item in items)
                    {
                        item.valid = false;
                        foreach (var obj in objects)
                        {
                            if (Dot.String("productIdentifier", obj, string.Empty) == item.productId)
                            {
                                item.valid = true;
                                string localeIdentifier = Dot.String("localeIdentifier", obj, string.Empty);
                                string[] localeIdentifierData = localeIdentifier.Split('=');
                                if (localeIdentifierData.Length > 1)
                                {
                                    item.currencyCode = localeIdentifierData[1];
                                }
                                else
                                {
                                    item.currencyCode = string.Empty;
                                }
                                //item.localizedTitle = Dot.String("localizedTitle", obj, string.Empty);
                                //item.localizedDesc = Dot.String("localizedDescription", obj, string.Empty);
                                item.localizedCost = Dot.String("localizedPrice", obj, string.Empty);
                                item.cost = Dot.Single("price", obj, item.cost);
                                item.cents = Dot.Integer("cents", obj, (int)item.cost * 100);
                                break;
                            }
                        }
                    }

                    if (_config.OnEnumerate != null)
                    {
                        _config.OnEnumerate();
                    }

                    hasEnumrate = true;
                };
                EB.Debug.Log("---Apple---Enumerate():items.Count:{0},Enumerating IAP: {1}", items.Count, offersString);
                _IAPEnumerate(offersString);
            }
		}
		
		public string Name
		{
			get { return "itunes"; }
		}
#endregion

		void OnIAPEnumerate( string json )
		{
            EB.Debug.Log("---Apple---OnIAPEnumerate(): {0}", json);
			if (_onEnumerate != null)
			{
				_onEnumerate(json);
				_onEnumerate = null;
			}
		}
		
		void OnIAPPurchaseCanceled(string ignore)
		{
            EB.Debug.Log("---Apple---OnIAPPurchaseCanceled()");
            if (_config.OnPurchaseCanceled != null)
			{
				_config.OnPurchaseCanceled(null);
			}
		}
		
		void OnIAPPurchaseFailed(string localizedError)
		{
            EB.Debug.Log("---Apple---OnIAPPurchaseFailed(): " ,localizedError );
			if (_config.OnPurchaseFailed != null)
			{
				_config.OnPurchaseFailed("Not supported", null);
			}
		}
		
		void OnIAPComplete(string data)
		{
            EB.Debug.Log("---Apple---OnIAPComplete():苹果SDK告诉我已经支付成功{0}", data);
            object json = JSON.Parse(data);
			var transaction = new Transaction();
			transaction.transactionId = EB.Dot.String("transactionIdentifier", json, transaction.transactionId);
			transaction.payload = EB.Dot.String("receipt", json, transaction.payload);
			transaction.signature = string.Empty;
			transaction.productId = EB.Dot.String("productIdentifier", json, transaction.productId);
			transaction.platform = Name;

			// see if we need to verify
			if ( _config.Verify != null )
			{
				_config.Verify(transaction);
			}
			else
			{
				Complete(transaction);
			}
		}

		public void Complete (Transaction transaction)
		{
            EB.Debug.Log("---Apple---Complete():服务器已经下发商品相应的东西transaction:{0}", transaction.productId);
            _IAPComplete(transaction.transactionId);
		}
		
		public void OnIAPFinalize(string transactionId)
		{
            EB.Debug.Log("---Apple---OnIAPFinalize(): transactionId = {0}", transactionId);
		}

        public void OnPayCallback(string transactionId)
        {
            
        }

        public void OnPayCallbackFromServer(string transactionId)
        {
            EB.Debug.Log("---Apple---OnPayCallbackFromServer(): transactionId = {0}", transactionId);
        }

        public string GetPayload(Transaction transaction)
        {
            var data = new Hashtable();
            data["orderId"] = EB.Encoding.ToHexString(EB.Crypto.RandomBytes(32));
            data["productId"] = transaction.productId;
            data["algorithm"] = "sha1";

            var bytes = Encoding.GetBytes(JSON.Stringify(data));
            var hmac = Hmac.Sha1(Encoding.GetBytes(_config.PublicKey));
            var payload = SignedRequest.Stringify(bytes, hmac);
            EB.Debug.Log("---Apple---GetPayload():这里是苹果版本的支付信息，但是不知道是否有地方调用");
            return payload;
        }
    }
}

class IAPAppleProvider : EB.IAP.Internal.AppleProvider 
{
	
}
#endif
