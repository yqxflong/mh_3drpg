using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class DataCacheAPI : SparxAPI
	{
		public DataCacheAPI(EndPoint endpoint) : base(endpoint)
		{
		}

		#region 17s
		public void GetConfig(Hashtable data, System.Action<string, byte[]> callback)
		{
			var req = endPoint.Post("/config/fetch");
			req.numRetries = 1;
			req.suspendMethod = Request.eSuspendMethod.Break;
			req.AddData(data);
			endPoint.Service(req, delegate (Response res)
			{
				if (res.sucessful)
				{
					callback(null, res.bytes);
				}
				else
				{
					callback(res.localizedError, null);
				}
			});
		}
		#endregion
	}
}