using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class PushAPI : SparxAPI
	{
		public PushAPI( EndPoint api ) : base(api)
		{
		}
		
		public void GetPushToken(System.Action<string,Hashtable> callback )
		{
			var request = endPoint.Get("/push/token");
			endPoint.Service( request, delegate( Response result ){
				if ( result.sucessful)
				{
					callback(null, result.hashtable);
				}
				else
				{
					callback(result.localizedError,null);
				}
			});
		}
		
		public void SetApplePushToken( string token, System.Action<string> callback )
		{
			var request = endPoint.Post("/push/apple");
			request.AddData("token", token);
			endPoint.Service( request, delegate( Response result ){
				if ( result.sucessful)
				{
					callback(null);
				}
				else
				{
					callback(result.localizedError);
				}
			});
		}
		
	}
}
