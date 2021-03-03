using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class LevelRewardsAPI : SparxAPI
	{
		private readonly int LevelRewardsAPIVersion = 1;
		
		public LevelRewardsAPI( EndPoint api ) : base(api)
		{
		}
		
		public void FetchStatus(System.Action<string,Hashtable> cb )
		{
			EB.Sparx.Request request = this.endPoint.Post("/levelrewards/fetch");
			request.AddData("api", LevelRewardsAPIVersion );
			
			this.endPoint.Service( request, delegate( Response result ) {
				if (result.sucessful)
				{
					cb(null, result.hashtable);
				}
				else
				{
					EB.Debug.Log( "ExecuteLevelRewardsFetchStatus Error: {0}", result.localizedError );
					cb(result.localizedError, null);
				}
			});
		}
	}
}

