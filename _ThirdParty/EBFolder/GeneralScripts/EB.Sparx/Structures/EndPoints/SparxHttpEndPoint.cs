using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	// abstraction for a service endpoint
	// handles queuing of requests, ordering and sessioning
	public class HttpEndPoint : EndPoint
	{
		private EB.Collections.Queue<Request> _queue = new EB.Collections.Queue<Request>();
		private EB.Collections.Queue<Request> _backup = new Collections.Queue<Request>();
		private Hashtable _data = Johny.HashtablePool.Claim();


		private static Dictionary<string, System.Action> _handlers = new Dictionary<string, System.Action>();

		public static void RegisterHandler(string key, System.Action act)
		{
			System.Action fn;
			if(_handlers.TryGetValue(key, out fn))
			{
				fn += act;
				_handlers[key] = fn;
			}
			else
			{
				_handlers[key] = act;
			}
		}

		public static void UnRegisterHandler(string key, System.Action act)
		{
			System.Action fn;
			if(_handlers.TryGetValue(key, out fn))
			{
				fn -= act;
				_handlers[key] = fn;
			}
		}

        private string _cookie = "";
        private Request _current = null;
		private int _nextId = 1;
		private int _doneId = 0;

		private object _interval;
		private int _lastActivity = -1;

		private Hmac _hmac;

		public bool ServiceActive
		{
			get { return _current != null || _queue.Count > 0; }
		}

		static System.DateTime _lastMemoryCheck = System.DateTime.Now;

		public HttpEndPoint( string endPoint, EndPointOptions options ) : base(endPoint,options)
		{
			if (options.Key != null)
			{
				_hmac = Hmac.Sha1(options.Key);
			}
		}

		public override void Dispose ()
		{
			StopKeepAlive();
			if(_current != null){
				_current.Destroy();
				_current = null;
			}
			ClearQueueAndBackUpDeep();

			base.Dispose ();
		}

		public override void StartKeepAlive ()
		{
			if (_interval == null)
			{
				if (Options.KeepAlive && Options.KeepAliveInterval > 0 && !string.IsNullOrEmpty(Options.KeepAliveUrl))
				{
					_interval = Coroutines.SetInterval(OnInterval, (Options.KeepAliveInterval * 1000) / 4);
				}
			}

			if (_backup.Count > 0)
			{
				EB.Debug.Log("StartKeepAlive: restore {0} requests", _backup.Count);
				_queue.InsertRange(0, _backup);
				_backup.Clear();
			}
		}

		public override void StopKeepAlive ()
		{
			if (_interval != null)
			{
				Coroutines.ClearInterval(_interval);
				_interval = null;
				_lastActivity = -1;
			}

			List<Request> backup = new List<Request>();
			if (_current != null)
			{
				// mark as suspend & try re-queue
				EB.Debug.LogWarning("StopKeepAlive: Suspend current request {0}", _current.url);
				_current.suspend = true;
				if (_current.suspendMethod != Request.eSuspendMethod.Retry)
				{
					backup.Add(_current);
				}
			}

			if (_queue.Count > 0)
			{
				backup.AddRange(_queue);
				_queue.Clear();
			}

			for (int i = 0; i < backup.Count; ++i)
			{
				if (backup[i].suspendMethod == Request.eSuspendMethod.Retry)
				{
					EB.Debug.LogWarning("StopKeepAlive: Backup request {0}", backup[i].url);
					_backup.Enqueue(backup[i]);
				}
				else if (backup[i].suspendMethod == Request.eSuspendMethod.Finish)
				{
					EB.Debug.LogWarning("StopKeepAlive: Finish request {0}", backup[i].url);
					backup[i].suspend = true;
					if (SuspendHandler != null)
					{
						Response res = new Response(backup[i]);
						SuspendHandler(res);
					}
				}
				else if (backup[i].suspendMethod == Request.eSuspendMethod.Break)
				{
					EB.Debug.LogWarning("StopKeepAlive: Break request {0}", backup[i].url);
				}
			}

			EB.Net.WebRequest.CleanConnections();
		}

		private void OnInterval()
		{
			//EB.Debug.LogError("OnInterval " + _lastActivity + " " + (Time.Now-_lastActivity));
			if ( _lastActivity > 0 && (Time.Now-_lastActivity) > Options.KeepAliveInterval )
			{
				var keepAlive = Post(Options.KeepAliveUrl);
				Service(keepAlive, delegate(Response r){
					if (!r.sucessful && r.fatal)
					{
						ClearQueueAndBackUpDeep();
						Hub.Instance.FatalError(r.localizedError);
					}
				});
			}
		}

		public override void AddData( string name, string value )
		{
			_data[name] = value;
		}

		public override string GetData (string key)
		{
			var d = _data[key];
			if (d != null)
			{
				return d.ToString();
			}
			return string.Empty;
		}

		Request Prepare( Request r )
		{
			foreach( DictionaryEntry e in _data )
			{
				r.AddQuery(e.Key.ToString(), e.Value);
			}
			return r;
		}

		public override int Service( Request request, System.Action<Response> callback )
		{
			// store the callback
			request.userData = callback;
			request.id = _nextId++;
			_queue.Enqueue(request);
			ServiceNext();
			return request.id;
		}

		public override bool IsDone( int requestId )
		{
			return _doneId >= requestId;
		}

		private void ServiceNext()
		{
			if ( _current != null)
			{
				return;
			}

			if ( _queue.Count > 0 )
			{
				_current = _queue.Dequeue();
				Prepare(_current);
				EB.Coroutines.Run(Fetch(_current));
			}
		}

		string Sign( Request r, string method, string data )
		{
			if (Options.Key != null)
			{
				var sb = new System.Text.StringBuilder(2048);
				sb.Append(method);
				sb.Append("\n");
				sb.Append(r.uri.Host);
				sb.Append("\n");
				sb.Append(r.uri.Path);
				sb.Append("\n");
				sb.Append(data);
				sb.Append("\n");
				//EB.Debug.Log("String to sign: " + sb);
				var digest = _hmac.Hash(Encoding.GetBytes(sb.ToString()));
				return Encoding.ToBase64String(digest);
			}
			return string.Empty;
		}

		private uint GenLogId(Request r, int retry)
		{
			var buffer = new Buffer(4);
			buffer.WriteInt32LE(Time.Now ^ Hash.FNV32(Device.UniqueIdentifier) ^ retry);
			buffer.Reset();
			return buffer.ReadUInt32LE();
		}

		private Net.WebRequest Generate( Request r, int retry )
		{
			// need to sign the request
			// we use a method similar to AWS signature version 2 (with a few changes to better support JSON and not sort the qs)
			Net.WebRequest request = null;

			// log id
			r.AddQuery("logid", GenLogId(r, retry));

			// timestamp
			if ( Time.Valid )
			{
				r.AddQuery("ts", Time.Now);
			}

			if ( retry > 0 )
			{
				r.AddQuery("retry", retry);
			}

			if ( r.isPost )
			{
				if( r.data.ContainsKey( "nonce" ) == false )
				{
					r.AddData( "nonce", EB.Sparx.Nonce.Generate() );
				}
				// post as json
				string json = JSON.Stringify(r.data);
				byte[] body = Encoding.GetBytes(json);

				string sig	= Sign(r, "POST", json);
				r.AddQuery("sig", sig);

				Hashtable headers = Johny.HashtablePool.Claim();
				headers["Content-Type"] = "application/json";
                if (!string.IsNullOrEmpty(_cookie))
                {
                    headers["Cookie"] = _cookie;
                }

                EB.Debug.Log("<color=#42fe79>[Post]</color>: " + r.url + "\n" + json);

				request = new Net.WebRequest(r.url, body, headers);
			}
			else
			{
				r.query.Remove("sig");
				string sig	= Sign(r, "GET", QueryString.StringifySorted(r.query) );
				r.AddQuery("sig", sig);

				EB.Debug.Log("<color=#42fe79>[Get]</color>: " + r.url);

                if (!string.IsNullOrEmpty(_cookie))
                {
                    Hashtable headers = Johny.HashtablePool.Claim();
                    headers["Cookie"] = _cookie;
                    request = new Net.WebRequest(r.url, headers);
                }
                else
                {
                    request = new Net.WebRequest(r.url);
                }
            }

			request.OnDataCallback = r.dataCallback;
			request.OnHeaders = r.headersCallback;

			request.Start();
			return request;
		}

		IEnumerator Fetch( Request req)
		{
			var res = new Response(req);

			// check memory
			if ( (System.DateTime.Now -_lastMemoryCheck).TotalSeconds> 2)
			{
				EB.Memory.Update(10000);
				_lastMemoryCheck = System.DateTime.Now;
			}

			int kNumRetries = Mathf.Max(req.numRetries, 0);
            for (int i = 0; i <= kNumRetries; ++i)
			{
				// wait and try again
				if (i > 0)
				{
					yield return new WaitForSeconds(Mathf.Pow(2.0f, i));
				}

				if (HasInternetConnectivity == false)
				{
					EB.Debug.Log("No internet connectivity");
					res.error = "ID_SPARX_ERROR_NOT_CONNECTED";
					res.fatal = true;
					break;
				}

				// check for hacks
				if (SafeValue.Breach || Memory.Breach)
				{
					res.fatal = true;
					res.error = "ID_SPARX_ERROR_UNKNOWN";
					break;
				}

				//EB.Debug.Log("Fetch: start {0}", req.url);
				Net.WebRequest www = Generate(req, i);
				//EB.Debug.Log("Fetch: generate {0}", req.url);
				while (!www.isDone)
				{
					//EB.Debug.Log("Fetch: wait {0}", req.url);
					yield return null;
				}


				System.DateTime parseStartTime = System.DateTime.Now;

				res.timeTaken = www.responseTime;
				string error = www.error;

				// suspend request
				if (req.suspend)
				{
					req.suspend = false;

					if (req.suspendMethod == Request.eSuspendMethod.Retry)
					{
						if (!string.IsNullOrEmpty(error))
						{
							EB.Debug.LogWarning("Fetch: retry suspend request {0}, error = {1}", req.url, error);
							++kNumRetries;
							continue;
						}
						else
						{
							EB.Debug.LogWarning("Fetch: suspend request was done {0}, (ziped)length = {1}", req.url, www.bytesRecieved);
						}
					}
					else
					{
						EB.Debug.LogWarning("Fetch: ignore suspend request {0}", req.url);
						_doneId = req.id;
						_current = null;
						ServiceNext();
						yield break;
					}
				}
				
				if ( string.IsNullOrEmpty(error) && req.dataCallback != null )
				{
					res.sucessful = true;
					res.fatal = false;
					res.sessionError = false;
					break;
				}
				else if ( string.IsNullOrEmpty(error) && www.bytesRecieved > 0)
				{
					res.result = null;
					res.error = null;

                    string cookie = www.GetResponseHeader("set-cookie");
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        _cookie = cookie;
                    }

                    string contentType = www.GetResponseHeader("Content-Type");
					if (contentType.IndexOf("application/json") != -1)
					{
                        yield return null;
						string text = www.text;
						res.text = text ?? string.Empty;
						if (res.text.Length > 1024 * 100)
						{
							EB.Debug.LogWarning("Package size is bigger than 100K!");
						}
						else
						{
                            EB.Debug.Log("<color=#ff6699>[Result]</color>: {0}", res.text);
						}

						try
						{
							object obj = JSON.Parse(res.text);
							if (obj == null)
							{
								res.error = "Failed to decode json: " + res.text;
							}
							else
							{
								if (obj is Hashtable)
								{
									Hashtable response =  obj as Hashtable;
									res.result = response["result"];
									res.error = response["err"] as string;
									res.async = response["async"];
                                    res.msg = response["msg"] as string;
                                    // update time
                                    Time.Now = Dot.Integer("ts", response, Time.Now);

									_lastActivity = Time.Now;
								}
								else
								{
									res.error = "Obj is " + obj.ToString();
								}
							}
						}
						catch (System.Exception ex)
						{
							EB.Debug.LogWarning("failed to decode www " + ex);
							res.error = ex.ToString();
						}

						if (res.result != null)
						{
							if(res.result is Hashtable)
							{
								res.hashtable = res.result as Hashtable;
							}
							else if(res.result is ArrayList)
							{
								res.arrayList = res.result as ArrayList;
							}
							else if(res.result is double){
								res.number = (double)res.result;
							}
							else if(res.result is string){
								res.str = res.result as string;
							}
						}
					}
					else if (contentType.IndexOf("application/octet-stream") != -1)
					{
						res.bytes = www.bytes;
					}


					var parseDelta = System.DateTime.Now - parseStartTime;
					EB.Debug.LogIf(www.overallElapsed > 100 || parseDelta.TotalMilliseconds > 30,
						"Fetch: request {0} responseTime = {1} overallElapsed = {2} parse = {3} milliseconds",
						req.url,
						www.responseTime,
						www.overallElapsed,
						parseDelta.TotalMilliseconds);


					if ( res.error == null )
					{
						res.sucessful = true;
						res.fatal = false;
						res.sessionError = false;
						break;
					}

					res.fatal 		= Dot.Bool("fatal", res.hashtable, true);
					res.sessionError= Dot.Bool("invalid_session", res.hashtable, false);
					bool retry 		= Dot.Bool("retry",res.hashtable, true);
					if (!retry)
					{
						// EB.Debug.LogWarning("skipping retry");
						break;
					}
				}
				// JEH - moved what was here, up there
				else
				{
					if (www.statusCode != 0)
					{
						PrintError(req, www);
					}

					if (string.IsNullOrEmpty(error))
					{
						res.error = "unknown";
					}
					else
					{
#if DEBUG
						res.error = string.Format("^{0}\nError: {1}", Localizer.GetString("ID_SPARX_NETWORK_ERROR"), error);
#else
						res.error = "ID_SPARX_NETWORK_ERROR";
#endif
					}
					res.fatal = true;

					// if this is a client error, then don't retry
					if (www.statusCode >= 400 && www.statusCode <= 499|| www.error.Equals("EndWrite failure"))
					{
						break;
					}
				}
			}

			OnComplete(res);
		}

		private void PrintError( Request r, Net.WebRequest www )
		{
            EB.Debug.LogError("Url: " + r.url + " Error: " + (www.error ?? "null") + " Status: " + www.statusCode );
			if ( www.responseHeaders != null && www.responseHeaders.Count > 0 )
			{
				foreach( var header in www.responseHeaders )
				{
                    EB.Debug.LogError("Error Header: {0}:{1}", header.Key ?? "" , header.Value ?? "" );
				}
			}
			else
			{
                EB.Debug.LogError("No response headers");
			}
		}

		private void OnComplete( Response result )
		{
			var request = result.request;
			if (_current != result.request)
			{
                EB.Debug.LogError("Something went seriously wrong in the service queue");
			}
			_current.Destroy();
			_current = null;

			bool hasCallback = request.userData != null && request.userData is System.Action<Response>;
			if (hasCallback)
			{
				var cb = (System.Action<Response>)request.userData;
				if (System.Object.ReferenceEquals(cb.Target, null) || !cb.Target.Equals(null))
				{
					cb(result);
				}
			}

			if (PostHandler != null)
			{
				PostHandler(result);
			}

			_doneId  = request.id;

			if (!result.sucessful && result.fatal && hasCallback)
			{
				ClearQueueAndBackUpDeep();
			}
			else
			{
				ServiceNext();
			}
		}

		/// <summary>
		/// Queue与Backup必须同时Clear
		/// </summary>
		private void ClearQueueAndBackUpDeep()
		{
			#region 通知清除了消息队列
			System.Action act;
			if(_handlers.TryGetValue("ClearQueueAndBackUpDeep", out act))
			{
				act?.Invoke();
			}
			#endregion

			#region 清除中...
			{
				var it = _queue.GetEnumerator();
				while(it.MoveNext()){
					it.Current.Destroy();
				}
				_queue.Clear();
			}
			{
				var it = _backup.GetEnumerator();
				while(it.MoveNext()){
					it.Current.Destroy();
				}
				_backup.Clear();
			}
			#endregion
		}
	}
}

