using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class InventoryAPI : SparxAPI
	{
		int		 _next = 0;
		int		 _done = 0;
		
		public InventoryAPI( EndPoint api ) : base(api)
		{
		}
		
		public int Sync(System.Action<int, string,Hashtable> callback )
		{
			var id = _next;
			Coroutines.Run(_Sync(callback));
			return id;
		}
		
		public int Add( Hashtable items, System.Action<int, string,Hashtable> callback )
		{
			var id = _next;
			Coroutines.Run(_Call("add", items, -1, callback));
			return id;
		}
		
		public int Use( Hashtable items, System.Action<int, string,Hashtable> callback )
		{
			var id = _next;
			Coroutines.Run(_Call("use", items, -1, callback));
			return id;
		}
		
		public int Purchase( Hashtable items, int cost, System.Action<int, string,Hashtable> callback )
		{
			var id = _next;
			Coroutines.Run(_Call("purchase", items, cost, callback));
			return id;
		}
		
		public bool IsDone( int id )
		{
			return id < _done;
		}
		
		public Coroutine Wait( int id )
		{
			return Coroutines.Run(_Wait(id));
		}
		
		IEnumerator _Wait(int id)
		{
			while(IsDone(id)==false)	
					yield return null;
		}
		
		IEnumerator _Sync(System.Action<int, string,Hashtable> callback )
		{
			var id = _next++;
			while (id != _done)
			{
				yield return null;
			}			
			
			var request = endPoint.Get("/inventory");
			endPoint.Service( request, delegate(Response result){
				_done++;
				
				if (result.sucessful)
				{
					callback(result.id, null, result.hashtable);
				}
				else
				{
					callback(result.id, result.localizedError, null);
				}
			});
		}
		
		IEnumerator _Call( string type, Hashtable items, int cost, System.Action<int, string,Hashtable> callback ) 
		{
			var id = _next++;
			while (id != _done)
			{
				yield return null;
			}
			
			var nonce = Nonce.Generate();
			
			var request = endPoint.Post("/inventory/"+type);
				
			if (cost >= 0)
			{
				request.AddData("cost", cost);
			}
			
			if (items != null)
			{
				request.AddData("items", items);	
			}
			
			request.AddData("nonce", nonce);
			
			endPoint.Service(request, delegate(Response result){
				_done++;
				if (result.sucessful){
					callback(id, null,result.hashtable);
				}
				else if (result.error != null && result.error.ToString() == "nsf" )
				{
					callback(id, "nsf", null);
				}
				else {
					callback(id, result.localizedError, null);
				}
			});
			
		}
	}
}
