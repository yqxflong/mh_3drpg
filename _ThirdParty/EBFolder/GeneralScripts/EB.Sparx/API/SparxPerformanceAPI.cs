using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class PerformanceAPI : SparxAPI
	{
		//private readonly int PerformanceAPIVersion = 1;
	
		public PerformanceAPI( EndPoint api ) : base(api)
		{
		}
		
		public void Fetch( string device, string CPU, string GPU, int platform, int memorySize, System.Action<string,object> callback,string profileName)
		{
			EB.Sparx.Request request = this.endPoint.Get("/performance/profile");
			request.AddData("device", device);
			request.AddData("cpu", CPU);
			request.AddData("gpu", GPU);
			request.AddData("platform", platform);
			request.AddData("memory", memorySize);
            //request.AddData("profileName", "High");//≤‚ ‘
            if (profileName!=null) request.AddData("profileName", profileName);//Low Medium High

            this.endPoint.Service( request, delegate( Response res ){
				if( res.sucessful == true )
				{
					callback( null, res.result );
				}
				else
				{
					callback( res.localizedError, null );
				}
			});
		}
	}
}
