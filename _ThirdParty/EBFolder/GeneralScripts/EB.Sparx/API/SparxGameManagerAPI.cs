using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class GameManagerAPI : SparxAPI, System.IDisposable
	{
		Dictionary<string,EndPoint> _masterServerEndpoints = new Dictionary<string, EndPoint>();
		
		public GameManagerAPI( EndPoint api ) : base(api)
		{
		}
		
		public void DisconnectEndPoints()
		{
			Coroutines.NextFrame(delegate(){
				foreach(var kvp in _masterServerEndpoints)
				{
					kvp.Value.Dispose();
				}
				_masterServerEndpoints.Clear();
			});
		}
		
		private EndPoint GetEndPoint( MasterServer server )
		{
			EndPoint ep;
			if (_masterServerEndpoints.TryGetValue(server.EndPoint, out ep))
			{
				return ep;
			}
			ep = server.CreateEndPoint();
			_masterServerEndpoints.Add(server.EndPoint,ep);
			return ep;
		}
		
		public MasterServer[] ParseMasterServers(ArrayList arrayList ) 
		{
			List<MasterServer> list = new List<MasterServer>();
			if ( arrayList != null)
			{
				for(int i = 0; i < arrayList.Count; i++)
				{
					list.Add( new MasterServer(arrayList[i]) );
				}
			}
			return list.ToArray();
		}
		
		public void GetMasterServers(System.Action<string,MasterServer[]> callback)
		{
			var request = endPoint.Get("/master/list");
			endPoint.Service(request, delegate(Response r){
			
				if (r.sucessful)
				{
					callback(null, ParseMasterServers(r.arrayList) );
				}
				else
				{
					callback(r.localizedError,null);
				}
			});
		}
		
		public void PingMasterServer( MasterServer server, System.Action<string,MasterServer> callback)
		{
			var qosProbe = new QosProbe(4, 2*1000, 128, server.Hostname, server.PingPort);
			qosProbe.OnComplete += delegate(){
				EB.Debug.Log("Probes complete " + qosProbe.AvgPing);	
				server.Ping = qosProbe.AvgPing;
				
				if (qosProbe.Data != null && qosProbe.Data.Length > 0 )
				{
					try {
						var json = Encoding.GetString(qosProbe.Data);
						var obj = JSON.Parse(json);
						server.GameCount = Dot.Integer("g", obj, 0);
						server.PlayerCount = Dot.Integer("p", obj, 0);
					}
					catch {}
				}
				
				callback(null, server);
				
				qosProbe.Dispose();
			};
			
			qosProbe.OnError += delegate(string obj) {
				callback(obj,server);
				
				qosProbe.Dispose();
			};
		}
		
		public void JoinGame( MasterServer server, long gameId, Hashtable playerAttributes, System.Action<string,object> callback)  
		{
			var ep = GetEndPoint(server);
			
			var args = new ArrayList();
			args.Add( gameId );
			args.Add( playerAttributes );
			
			ep.RPC("joinGame", args, delegate(string err, object obj){
			
				if (!string.IsNullOrEmpty(err))
				{
					callback(err,null);
					return;
				}
				
				callback(null,obj);
				
			});
		}
		
		public void FindPrivateGame( string name, System.Action<string,Hashtable> callback )
		{
			var request = endPoint.Post("/push/key-get");
			request.AddData("key", name);
			endPoint.Service(request, delegate(Response result){
				if (result.sucessful)
				{
					callback(null, result.hashtable);
				}
				else 
				{
					callback(result.localizedError,null);	
				}
			});
		}
		
		public void ResetServer( MasterServer server, int maxPlayers, Hashtable attributes, Hashtable playerAttributes, string privateKey, System.Action<string,object> callback)  
		{
			var ep = GetEndPoint(server);
			
			var args = new ArrayList();
			args.Add( new Hashtable(){ {"max_players", maxPlayers}, {"attributes", attributes}, {"private", privateKey} } );
			args.Add( playerAttributes );
			
			ep.RPC("resetServer", args, delegate(string err, object obj){
			
				if (!string.IsNullOrEmpty(err))
				{
					callback(err,null);
					return;
				}
				
				callback(null,obj);
				
			});
		}
		
		public void ListGames( MasterServer server, Hashtable attributes, System.Action<string,MasterServerGame[]> callback )
		{
			var ep = GetEndPoint(server);
			var args = new ArrayList();
			args.Add(attributes);
			
			ep.RPC("listGames", args, delegate(string err,object obj){
				
				if (!string.IsNullOrEmpty(err))
				{
					EB.Debug.LogError("List Games Error:" + err);
					callback(err,null);
					return;
				}
				
				List<MasterServerGame> games = new List<MasterServerGame>();
				var list = (ArrayList)obj;
				foreach( var g in list )
				{
					games.Add( new MasterServerGame(g) );
				}
				
				callback(null,games.ToArray());
			});
		}
		
		public void FindGame( MasterServer server, Hashtable queryAttributes, Hashtable playerAttributes, System.Action<string,object> callback )
		{
			var ep = GetEndPoint(server);
			var args = new ArrayList();
			args.Add(queryAttributes);
			args.Add(playerAttributes);
			
			ep.RPC("find", args, delegate(string err,object obj){
				if (!string.IsNullOrEmpty(err))
				{
					callback(err,null);
					return;
				}
				callback(null,obj);
			});
		}
		
		public void FindOrCreateGame( MasterServer server, Hashtable queryAttributes, int maxPlayers, Hashtable resetAttributes, Hashtable playerAttributes, System.Action<string,object> callback )
		{
			var ep = GetEndPoint(server);
			var args = new ArrayList();
			args.Add(queryAttributes);
			args.Add( new Hashtable(){ {"max_players", maxPlayers}, {"attributes", resetAttributes} } );
			args.Add( playerAttributes );
			
			ep.RPC("findOrCreate", args, delegate(string err,object obj){
				if (!string.IsNullOrEmpty(err))
				{
					callback(err,null);
					return;
				}
				callback(null,obj);
			});
		}
		
		#region IDisposable implementation
		public void Dispose ()
		{
			foreach( var endpoint in _masterServerEndpoints.Values) 
			{
				endpoint.Dispose();
			}
			_masterServerEndpoints.Clear();
		}
		#endregion
	}
}


