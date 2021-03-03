using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class GameCenterAPI : SparxAPI
	{
		public GameCenterAPI( EndPoint ep) : base(ep)
		{
		}
		
		public void SetGameCenterId( string gameCenterId, string gameCenterName, System.Action<string> callback ) 
		{
			var request = endPoint.Post("/gamecenter/id");
			request.AddData("gcid", gameCenterId);
			request.AddData("gcname", gameCenterName);
			endPoint.Service(request, delegate(Response result){
				if (result.sucessful) {
					callback(null);
				}
				else {
					callback(result.localizedError);
				}
			});
		}
		
	}
	
}

