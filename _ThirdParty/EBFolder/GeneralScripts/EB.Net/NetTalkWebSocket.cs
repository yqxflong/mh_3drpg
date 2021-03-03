using UnityEngine;
using System.Collections;
using EB.Collections;

namespace EB.Net
{
	/// <summary>
	/// A web socket that conforms to the sparx WSTalk json protocol
	/// </summary>
	public class TalkWebSocket : WebSocket
	{
		// constants
		const int TYPE_CALL 	= 0;
		const int TYPE_RESULT 	= 1;
		const int TYPE_ASYNC    = 2;

		public event System.Action<string,ArrayList,System.Action<string,object>> OnRPC;
		public event System.Action<string, object> OnAsync;

		int _nextId;
		Hashtable _callbacks = Johny.HashtablePool.Claim();
		
		public TalkWebSocket() : base()
		{
			_nextId = 1;
			
			// attach to the message handler
			this.OnMessage += this.MessageHandler;
		}
		
		public override void Dispose()
		{
			base.Dispose();
			
			// clear any callbacks
			_callbacks.Clear();
		}
		
		// make a remote procedure call
		public void RPC( string name, ArrayList args, System.Action<string,object> callback )
		{			
			int id = 0;
			lock(_callbacks)
			{
				id = _nextId++;
							
				// save the callback
				_callbacks[id] = callback;
			}
			
			// send
			SendMsg( new object[]{ TYPE_CALL, id, name, args } );
		}
		
		protected override void Error( string error )
		{
			Hashtable tmp;
			lock(_callbacks)
			{
				tmp = _callbacks;
				_callbacks = Johny.HashtablePool.Claim();
			}
			
			foreach( DictionaryEntry kvp in tmp)
			{
				EB.Debug.Log("Doing Error Callbacks");
				var cb = (System.Action<string,object>)kvp.Value;
				cb(error,null);
			}
			base.Error(error);
		}
		
		void MessageHandler( string message )
		{
			try
			{
				var data = JSON.Parse(message);
				if ( data is ArrayList )
				{
					var list = data as ArrayList;
					if (list.Count != 4)
					{
						throw new System.IO.IOException("Invalid message: " + message);
					}
					//��ע����־�������־�ܳ���
					//EB.Debug.Log("<-N " + message);
					
					// stupid json casting...
					int type = (int)(double)list[0];
					int id = (int)(double)list[1];
					
					if (type == TYPE_CALL)
					{
						// call here
						var rpc = list[2] as string;
						var args = list[3] as ArrayList;
						
						var callback = (System.Action<string,object>)delegate(string error,object result){
							SendMsg(new object[]{ TYPE_RESULT, id, error, result });
						};
						
						if (OnRPC!=null)
						{
							OnRPC(rpc,args,callback); 
						}
						else
						{
							callback("no handler for rpc",null);
						}						
					}
					else if (type == TYPE_RESULT)
					{
						
						// got back a result
						var error = list[2];
						var errStr= error != null ? error.ToString() : null;
						var result= list[3];
						
						object cbObj = null;
						lock(_callbacks)
						{
							cbObj = _callbacks[id];
							_callbacks.Remove(id);
						}
						
						if ( cbObj != null )
						{
							// make the callback
							((System.Action<string,object>)cbObj)(errStr,result);
						}
						else
						{
							EB.Debug.LogError("Callback missing for id " + id); 
						}
					}
					else if (type == TYPE_ASYNC)
					{
						if (OnAsync != null)
						{
							OnAsync(list[2] as string, list[3]);
						}
					}
					else
					{
						throw new System.IO.IOException("Unknown message type " + type);
					}
				}
			}
			catch (System.Exception e)
			{
				Error("wstalk exception: " + e);
			}
		}
		
		void SendMsg( object data )
		{
			var str = JSON.Stringify(data);
			EB.Debug.Log("->N " + str);
			SendUTF8(str);
		}
		
		protected override void DidClose (Impl impl)
		{
			base.DidClose (impl);
			
			ArrayList cbs = new ArrayList();
			lock(_callbacks)
			{
				foreach( var cb in _callbacks.Values )
				{
					cbs.Add(cb);
				}
				_callbacks.Clear();
			}
			
			foreach( var cb in cbs )
			{
				var action = (System.Action<string,object>)cb;
				if (action != null)
				{
					action("Lost connection",null);
				}
			}
		}
	}
}
