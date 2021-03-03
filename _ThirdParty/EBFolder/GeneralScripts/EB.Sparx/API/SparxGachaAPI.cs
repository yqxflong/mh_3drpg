using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class GachaAPI : SparxAPI
	{
		private readonly int GachaAPIVersion = 5;
	
		public GachaAPI( EndPoint endpoint ) : base(endpoint)
		{
		}
		
		public void FetchSets( string[] groups, System.Action<string,Hashtable> callback )
		{
			EB.Sparx.Request setRequest = this.endPoint.Get("/gacha/getSet");
			setRequest.numRetries = 3;
			setRequest.AddData( "groups", string.Join( ",", groups ) );
			setRequest.AddData( "api", GachaAPIVersion );
			this.endPoint.Service( setRequest, delegate( Response result ){
				if( result.sucessful == true )
				{
					callback( null, result.hashtable );
				}
				else
				{
					callback( result.localizedError, null );
				}
			});
		}
		
		public void PickFromBox( string group, string version, string set, string box, string payment, System.Action<string,Hashtable> callback )
		{
			EB.Sparx.Request request = this.endPoint.Post("/gacha/pick");
			request.AddData("group", group );
			request.AddData("api", GachaAPIVersion );
			
			request.AddData("version", version );
			request.AddData("set", set );
			request.AddData("box", box);
			request.AddData("payment", payment);
			request.AddData("nonce", EB.Sparx.Nonce.Generate() );
			this.endPoint.Service( request, delegate( Response result ) {
				if (result.sucessful)
				{
					callback(null, result.hashtable);
				}
				else
				{
					EB.Debug.Log( "PickFromBox Error: {0}", result.localizedError );
					callback(result.localizedError, null);
				}
			});
		}
		
		public void SyncTokens(System.Action<string, ArrayList> callback )
		{
			EB.Sparx.Request request = this.endPoint.Get("/gacha/getTokens");
			request.AddData("api", GachaAPIVersion );
			
			this.endPoint.Service( request, delegate( Response result ) {
				if (result.sucessful)
				{
					callback(null, result.arrayList);
				}
				else
				{
					EB.Debug.Log( "SyncTokens Error: {0}", result.localizedError );
					callback(result.localizedError, null);
				}
			});
		}

		public void GetLiveHash(System.Action<string, Hashtable> callback)
		{
			EB.Sparx.Request request = this.endPoint.Get("/gacha/getTimestamp");
			request.AddData("api", GachaAPIVersion );			
			request.numRetries = 3;
			this.endPoint.Service(request, delegate(Response result) 
				{
					if (result.sucessful)
					{
						callback(null, result.hashtable);
					}
					else
					{
						EB.Debug.Log("Timestamp Error: {0}", result.localizedError);
						callback(result.localizedError, null);
					}
				}
			);
		}
	}
}
