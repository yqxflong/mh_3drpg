using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EB.IAP.Internal
{
	internal class DebugProvider : Provider
	{
		public static DebugProvider Create( Config config )
		{
			return new DebugProvider(config);
		}
		
		Config _config;
		
		DebugProvider(Config config)
		{
			_config = config;
		}
		
		public void PurchaseItem( Item item) 
		{
			var data = Johny.HashtablePool.Claim();
			data["orderId"] = EB.Encoding.ToHexString( EB.Crypto.RandomBytes(32) );
			data["productId"] = item.productId;
			data["algorithm"] = "sha1";

            if(!string.IsNullOrEmpty(item.LimitedTimeGiftId))data["limitedTimeGiftId"] = item.LimitedTimeGiftId;

			var bytes = Encoding.GetBytes( JSON.Stringify(data) ); 
			var hmac = Hmac.Sha1(  Encoding.GetBytes( _config.PublicKey) );
			var payload = SignedRequest.Stringify(bytes, hmac);


			Transaction transaction = new Transaction();
			transaction.transactionId = data["orderId"].ToString();
			transaction.payload = payload;
			transaction.productId = item.productId;
            if (!string.IsNullOrEmpty(item.LimitedTimeGiftId)) transaction.limitedTimeGiftId = item.LimitedTimeGiftId;

            if ( _config.Verify != null )
			{
				_config.Verify(transaction);
			}
			else
			{
				Complete(transaction);
			}

		}

		#region Provider implementation
		public void Enumerate (List<Item> items)
		{
			foreach( var item in items )
			{
				item.valid = true;
			}
			
			if (_config.OnEnumerate!=null)
			{
				_config.OnEnumerate();
			}
		}
		#endregion

		#region Provider implementation
		public void Complete (Transaction transaction)
		{
			
		}

        public void OnPayCallbackFromServer(string transactionId)
        {

        }

        public string GetPayload(Transaction transaction)
        {
            var data = Johny.HashtablePool.Claim();
            data["orderId"] = EB.Encoding.ToHexString(EB.Crypto.RandomBytes(32));
            data["productId"] = transaction.productId;
            data["algorithm"] = "sha1";

            var bytes = Encoding.GetBytes(JSON.Stringify(data));
            var hmac = Hmac.Sha1(Encoding.GetBytes(_config.PublicKey));
            var payload = SignedRequest.Stringify(bytes, hmac);

            return payload;
        }
        #endregion

        #region Provider implementation
        public string Name 
		{
			get 
			{
				return "debug";
			}
		}
		#endregion
	}
}