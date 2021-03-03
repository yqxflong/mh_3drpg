using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class WalletAPI : SparxAPI
	{
		int		 _next = 0;
		int		 _done = 0;
		
		public WalletAPI( EndPoint api ) : base(api)
		{
		}

		public void Fetch(System.Action<string,Hashtable> callback ) 
		{
			Coroutines.Run(_Fetch(callback));
		}
		
		public int Credit( int value, string reason, System.Action<int,string,Hashtable> callback )
		{
			var id = _next;
			Coroutines.Run(_Call("credit",value,reason,callback));
			return id;
		}
		
		public int Debit( int value, string reason, System.Action<int,string,Hashtable> callback )
		{
			var id = _next;
			Coroutines.Run(_Call("debit",value,reason,callback));
			return id;
		}
		
		public void FetchPayouts( string platform, System.Action<string,Hashtable> callback )
		{
			var request = endPoint.Get("/store/payouts");
			request.AddData("platform", platform);
			request.AddData("version", EB.Version.GetVersion() );
			endPoint.Service(request, delegate(Response result){
				if (result.sucessful)
				{
					callback(null,result.hashtable);
				}
				else
				{
					callback(result.localizedError, null);
				}
			});
		}
		
		public void FetchOfferUrl( string platform, string offerName, System.Action<string,string> callback )
		{
			var request = endPoint.Get("/store/offerurl");
			request.AddData("offer", offerName);
			request.AddData("platform", platform );
			endPoint.Service(request, delegate(Response result){
				if (result.sucessful)
				{
					callback(null,result.str);
				}
				else
				{
					callback(result.localizedError, null);
				}
			});
		}

		public void VerifyPayout( string platform, Hashtable data, System.Action<string,Hashtable> callback )
		{
			var request = endPoint.Post("/store/verify-payout");
			request.AddData("data", data);
			request.AddData("platform", platform);
			endPoint.Service(request, delegate(Response result){
				if (result.sucessful)
				{
					callback(null, result.hashtable);
				}
				else
				{
					callback(result.localizedError, result.hashtable);
				}
			});
		}
        
        public void BuyPayout(string externalTrkid, int payoutid, string platform, int numOfIGC, ArrayList redeemers, System.Action<string, Hashtable> callback, object extraInfo)
		{
			var request = endPoint.Post("/store/buyPayouts");
			request.AddData("externalTrkid", externalTrkid);
			request.AddData("payoutid", payoutid);
			request.AddData("platform", platform);
			request.AddData("numOfIGC", numOfIGC);
			request.AddData("redeemers", redeemers);
			request.AddData("extraInfo", extraInfo);
			endPoint.Service(request, delegate (Response result) 
			{
				if (result.sucessful)
				{
					callback(null, result.hashtable);
				}
				else
				{
					callback(result.localizedError, null);
				}
			});
		}

		public void GetPayouts(string transactionid, int payoutid, string platform, ArrayList redeemers, System.Action<string, Hashtable> callback)
		{
			var request = endPoint.Post("/store/getPayouts");
			request.AddData("transactionid", transactionid);
			request.AddData("payoutid", payoutid);
			request.AddData("platform", platform);
			request.AddData("redeemers", redeemers);
			endPoint.Service(request, delegate(Response result) 
			{
				if (result.sucessful)
				{
					callback(null, result.hashtable);
				}
				else
				{
					callback(result.localizedError, null);
				}
			});
		}

		IEnumerator _Fetch(System.Action<string,Hashtable> callback )
		{
			var id = _next++;
			while (id != _done)
			{
				yield return null;
			}
			
			var request = endPoint.Get("/wallet/balance");
			endPoint.Service(request, delegate(Response result){
					_done++;
					if (result.sucessful){
						callback(null,result.hashtable);
					}
					else if (result.error != null && result.error.ToString() == "nsf" )
					{
						callback("nsf", null);
					}
					else {
						callback(result.localizedError, null);
					}
			});
		}
		
		IEnumerator _Call( string type, int value, string reason, System.Action<int, string,Hashtable> callback ) 
		{
			var id = _next++;
			while (id != _done)
			{
				yield return null;
			}
			
			var nonce = Nonce.Generate();
			
			var request = endPoint.Post("/wallet/"+type);
				
			if (value > 0)
			{
				request.AddData("value", value);
			}
			
			if (!string.IsNullOrEmpty(reason))
			{
				request.AddData("reason", reason);	
			}
			
			request.AddData("nonce", nonce);
			
			endPoint.Service(request, delegate(Response result){
				_done++;
				if (result.sucessful){
					callback(id, null,result.hashtable);
				}
				else if (result.error != null && result.error.ToString() == "nsf" )
				{
					callback(id, "nsf", null);
				}
				else {
					callback(id, result.localizedError, null);
				}
			});			
		}		
	}
}
