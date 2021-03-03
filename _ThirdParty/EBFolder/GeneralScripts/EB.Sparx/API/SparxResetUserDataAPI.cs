// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class ResetUserDataAPI : SparxAPI
	{
		//private Hashtable _deviceInfo;

		public ResetUserDataAPI (EndPoint endpoint) : base(endpoint)
		{
		}

		Request Post(string path) {
			var req = endPoint.Post(path);
			return req;
		}

		public void ResetUserData(System.Action<string,Hashtable> callback )
		{
			var req = Post ("/debugroute/debugCleanUserData");
			endPoint.Service(req, delegate (Response res) {
				if (res.sucessful) {
					callback(null,res.hashtable);
				}
				else {
					callback(res.localizedError,null);
				}
			});
		}
	}
}

