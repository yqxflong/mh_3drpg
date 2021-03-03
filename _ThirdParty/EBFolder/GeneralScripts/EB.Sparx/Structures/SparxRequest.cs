using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class Request
	{
		public enum eSuspendMethod
		{
			Retry,
			Finish,
			Break
		}

		private Hashtable   _data;
		private Hashtable   _query;
		private EB.Uri      _url;
		private bool        _isPost;

		public object userData { get; set; }
		public int id { get; set; }

		public System.Action<byte[]> dataCallback { get; set; }
		public System.Action<Net.WebRequest> headersCallback { get; set; }

		public Hashtable query { get { return _query; } }
		public Hashtable data { get { return _data; } }
		public bool isPost { get { return _isPost; } }
		public EB.Uri uri { get { return _url; } }

		public int numRetries { get; set; }
		public eSuspendMethod suspendMethod { get; set; }
		public bool suspend { get; set; }

		public string url
		{
			get
			{
				var url = _url.Scheme + "://" + _url.HostAndPort + _url.Path + "?" + QueryString.Stringify(_query);
				return url;
			}
		}

		public void Destroy(){
			Johny.HashtablePool.Release(_data);
			_data = null;
			Johny.HashtablePool.Release(_query);
			_query = null;	
		}

		public Request(string url, bool isPost)
		{
			_url = new EB.Uri(url);
			_isPost = isPost;
			numRetries = 0;
			suspendMethod = eSuspendMethod.Retry;

			_query = QueryString.Parse(_url.Query);
			if (isPost)
			{
				_data = Johny.HashtablePool.Claim();
			}
			else
			{
				_data = _query;
			}
		}

		public void AddQuery(string key, object value)
		{
			_query[key] = value;
		}

		public void AddData(string key, object value)
		{
			_data[key] = value;
		}

		public void AddData(Hashtable data)
		{
			foreach (DictionaryEntry entry in data)
			{
				AddData(entry.Key.ToString(), entry.Value);
			}
		}
	}

	public class Response
	{
		public Request request { get; set; }
		public bool sucessful { get; set; }
		public int timeTaken { get; set; } // time in milliseconds
		public string text { get; set; }
		public object result { get; set; }
        public string msg { get; set; }
        public byte[] bytes { get; set; }
		public string error { get; set; }
		public object async { get; set; }
		public bool fatal { get; set; }
		public bool sessionError { get; set; }
		public Hashtable hashtable { get; set; }
		public ArrayList arrayList { get; set; }
		public double number { get; set; }
		public string str { get; set; }

		private static System.Func<string, string> errorTranslateHandler;
		public static void SetErrorTranslatorHandler(System.Func<string, string> handler)
		{
			errorTranslateHandler = handler;
		}

		public bool empty
		{
			get
			{
				if (result == null)
				{
					return true;
				}

				if (hashtable != null && hashtable.Count == 0)
				{
					return true;
				}

				if (arrayList != null && arrayList.Count == 0)
				{
					return true;
				}

				if (str != null && str == string.Empty)
				{
					return true;
				}

				return false;
			}
		}

		public string localizedError
		{
			get
			{
				if (error != null)
				{
					var tmp = error.ToString();
					var localized = string.Empty;

					// strip "Error: " leaving the actual error tag for localization
					tmp = tmp.Replace("Error: ", "");

					if (Localizer.GetString(tmp, out localized))
					{
						ArrayList errArgs = EB.Dot.Array("errArgs", hashtable, null);
						if (localized != null && errArgs != null)
						{
							localized = string.Format(localized, errArgs.ToArray());
						}
						return localized;
					}
					else if (tmp.StartsWith("^"))
					{
						// pre localized
						return tmp.Substring(1);
					}
					else if (errorTranslateHandler != null)
					{
						string trans = errorTranslateHandler(tmp);
						if (!string.IsNullOrEmpty(trans) && trans != tmp)
						{
							return trans;
						}
					}

					// fall back
#if DEBUG
					return Localizer.GetString("ID_SPARX_ERROR_UNKNOWN") + "\nError: " + tmp;
#else
					return Localizer.GetString("ID_SPARX_ERROR_UNKNOWN");
#endif
				}
				return string.Empty;
			}
		}

		public int id
		{
			get { return request.id; }
		}

		public string url
		{
			get { return request.url; }
		}

		public void Destroy(){
			//clear hashtable
			this.hashtable = null;
			//clear jsonarray
			this.arrayList = null;
			//clear result
			if(this.result is Hashtable){
				Johny.HashtablePool.Release(this.result as Hashtable);
			}
			else if(this.result is ArrayList){
				Johny.ArrayListPool.Release(this.result as ArrayList);
			}
			this.result = null;
			//clear async
			if(this.async is Hashtable){
				Johny.HashtablePool.Release(this.async as Hashtable);
			}
			else if(this.async is ArrayList){
				Johny.ArrayListPool.Release(this.async as ArrayList);
			}
			this.async = null;
		}

		public Response(Request r)
		{
			request = r;
		}
	}
}


