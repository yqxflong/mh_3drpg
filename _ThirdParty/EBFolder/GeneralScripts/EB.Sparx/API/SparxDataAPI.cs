using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class DataAPI : SparxAPI
	{
		public DataAPI(EndPoint endpoint) : base(endpoint)
		{
		}

		public void Load(Id userId, string key, System.Action<string, Hashtable> callback)
		{
			var request = endPoint.Get( string.Format("/ds/{0}/{1}", userId, key) );
			endPoint.Service(request, delegate (Response result)
			{
				if (result.sucessful)
				{
					callback(null, result.hashtable);
					return;
				}
				callback(result.localizedError, null);
			});
		}

		public void Save(Id userId, Hashtable data, System.Action<string> callback)
		{
			var request = endPoint.Post( string.Format("/ds/{0}", userId) );
			request.AddData("data", data);
			endPoint.Service(request, delegate (Response result)
			{
				if (result.sucessful)
				{
					callback(null);
					return;
				}
				callback(result.localizedError);
			});
		}

	}
}

