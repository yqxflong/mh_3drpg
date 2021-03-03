using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	// abstraction for a service endpoint
	// handles queuing of requests, ordering and sessioning
	public class EndPointSimulator : HttpEndPoint
	{
		public class SimulatedServiceData
		{
			public SimulatedServiceData(Request rq, EB.Sparx.Response rs)
			{
				request = rq;
				response = rs;
			}
			public Request request;
			public EB.Sparx.Response response;
		}
		public class SimulatedRPCData
		{
			public SimulatedRPCData(ArrayList aa)
			{
				args = aa;
			}
			public ArrayList args;
			public string error;
			public object obj;
		}
		private GameObject m_listener;

		public EndPointSimulator( string endPoint, EndPointOptions options )  :
			base(endPoint,options)
		{
		}
		
		public void RegisterListener(GameObject listener)
		{
			m_listener = listener;
		}
		
		public override Request Get( string path )
		{
			return new Request( Url + path, false);
		}
		
		public override Request Post( string path )
		{
			return new Request( Url + path, true);
		}
		
		public override int Service( Request request, System.Action<Response> callback )
		{
			if(m_listener != null)
			{
				EB.Sparx.Response response = new Response(request);
				response.sucessful = false;
				SimulatedServiceData myData = new SimulatedServiceData(request, response);
				m_listener.SendMessage("HandleRequest", myData, SendMessageOptions.RequireReceiver);
				callback(myData.response);
			}
			// -1 is presumably a reasonable value
			return -1;
		}
		
		public override void RPC (string name, ArrayList args, System.Action<string, object> callback)
		{
			if(m_listener != null)
			{
				SimulatedRPCData rpc_data = new SimulatedRPCData(args);
				m_listener.SendMessage ("HandleRPC", rpc_data, SendMessageOptions.RequireReceiver);
				if(callback != null)
				{
					callback(rpc_data.error,rpc_data.obj);
				}
			}
		}
	}
}

