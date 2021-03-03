using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

using System.IO;
using System.Net;

namespace HTTP
{
    /// <summary>
    /// http请求控件
    /// </summary>
	public class Request : BaseHTTP
	{
        /// <summary>
        /// 指定的uri文件路径
        /// </summary>
		public static Uri proxy = null;

#region public fields
        /// <summary>
        /// 是否请求完成
        /// </summary>
		public bool isDone = false;
        /// <summary>
        /// 请求错误结果
        /// </summary>
		public Exception exception = null;

		public Response response { get; set; }
        /// <summary>
        /// 最多重发次数
        /// </summary>
		public int maximumRedirects = 8;
        /// <summary>
        /// 是否为zip文件
        /// </summary>
		public bool acceptGzip = true;
        /// <summary>
        /// 是否使用缓存
        /// </summary>
		public bool useCache = false;
        /// <summary>
        /// http头部信息
        /// </summary>
		public readonly Headers headers = new Headers ();
		public bool enableCookies = true;
        /// <summary>
        /// 超时的时间
        /// </summary>
		public float timeout = 0;


		public static Dictionary<string, HttpConnection> connectionPool = new Dictionary<string, HttpConnection>();
		public readonly static CookieContainer cookies = new CookieContainer ();
#endregion
	
#region public properties
		public Uri uri { get; set; }

		public HttpConnection upgradedConnection { get; private set; }

		public float Progress {
			get { return response == null ? 0 : response.progress; }
		}

		public float UploadProgress {
			get {
				return uploadProgress;
			}
		}

		public string Text {
			set { bytes = value == null ? null : HTTPProtocol.enc.GetBytes (value); }
		}

		public byte[] Bytes {
			set { bytes = value; }
		}
#endregion

#region public interface
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="OnDone">请求完成回调</param>
        /// <returns></returns>
		public Coroutine Send (System.Action<Request> OnDone)
		{
			this.OnDone = OnDone;
			return Send ();
		}
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <returns></returns>
		public Coroutine Send ()
		{
			BeginSending ();
			return UniWeb.Instance.StartCoroutine (_Wait ());   
		}

#endregion
#region constructors
		public Request() {
			this.method = "GET";
		}
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="method">http的请求方法</param>
        /// <param name="uri">路径</param>
		public Request (string method, string uri)
		{
			this.method = method;
			this.uri = new Uri (uri);
		}

		public Request (string method, string uri, bool useCache)
		{
			this.method = method;
			this.uri = new Uri (uri);
			this.useCache = useCache;
		}

		public Request (string uri, WWWForm form)
		{
			this.method = "POST";
			this.uri = new Uri (uri);
			this.bytes = form.data;
			foreach (string k in form.headers.Keys) {
				headers.Set (k, (string)form.headers [k]);
			}
		}

		public Request (string method, string uri, byte[] bytes)
		{
			this.method = method;
			this.uri = new Uri (uri);
			this.bytes = bytes;
		}


		public static Request BuildFromStream(string host, NetworkStream stream) {
			var request = CreateFromTopLine (host, HTTPProtocol.ReadLine (stream));
			if (request == null) {
				return null;
			}
			HTTPProtocol.CollectHeaders (stream, request.headers);
			float progress = 0;
			using (var output = new System.IO.MemoryStream()) {
				if (request.headers.Get ("transfer-encoding").ToLower () == "chunked") {
					HTTPProtocol.ReadChunks (stream, output, ref progress);
					HTTPProtocol.CollectHeaders (stream, request.headers);
				} else {
					HTTPProtocol.ReadBody (stream, output, request.headers, true, ref progress);
				}
				request.Bytes = output.ToArray ();
			}
			return request;
		}

#endregion
#region implementation

		static Request CreateFromTopLine (string host, string top)
		{
			var parts = top.Split (' ');
			if (parts.Length != 3)
				return null;
			if (parts [2] != "HTTP/1.1")
				return null;
			var request = new HTTP.Request ();
			request.method = parts [0].ToUpper ();
			request.uri = new Uri (host + parts [1]);
			request.response = new Response(request);
			return request;
		}
		

		static bool ValidateServerCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			//This is where you implement logic to determine if you trust the certificate.
			//By default, we trust all certificates.
			return true;
		}

        /// <summary>
        /// 创建http连接
        /// </summary>
        /// <param name="host">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="useSsl">是否使用ssl安全证书</param>
        /// <returns></returns>
		HttpConnection CreateConnection(string host, int port, bool useSsl)
		{
			// UnityVS will cause async method fail.
			// See: http://answers.unity3d.com/questions/892371/tcp-socket-async-beginsend-never-happens.html
			HttpConnection connection;

			var key = string.Format("{0}:{1}", host, port);
			if (connectionPool.ContainsKey(key))
			{
				connection = connectionPool[key];
				connectionPool.Remove(key);
				if (connection.IsConnected())
				{
					//Debug.LogFormat("CreateConnection reuse connection for {0}:{1}", host, port);
					return connection;
				}
				connection.Dispose();
			}

			connection = new HttpConnection() { host = host, port = port };
			//Debug.Log(host);
			connection.Connect();
			//Debug.LogFormat("CreateConnection connected to {0}:{1}", host, port);

			if (useSsl)
			{
#if BNICKSON_UPDATED
				var net = connection.client.GetStream ();
				net.ReadTimeout = HttpConnection.ReadTimeout * 1000;
				net.WriteTimeout = HttpConnection.WriteTimeout * 1000;
				connection.stream = new SslStream(net, false, ValidateServerCertificate);
#else
				connection.stream = new SslStream(connection.client.GetStream (), false, ValidateServerCertificate);
#endif
				var ssl = connection.stream as SslStream;
#if BNICKSON_UPDATED
				var async = ssl.BeginAuthenticateAsClient(uri.Host, null, null);
				var result = async.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(HttpConnection.ReadTimeout));
				if (!result || !async.IsCompleted)
				{
					throw new HTTP.HTTPException("CreateConnection AuthenticateAsClient timeout");
				}
#else
				ssl.AuthenticateAsClient(uri.Host);		
#endif
				EB.Debug.Log("CreateConnection ssl authenticated {0}:{1}", host, port);
			}
			else
			{
				connection.stream = connection.client.GetStream();

#if BNICKSON_UPDATED
				connection.stream.ReadTimeout = HttpConnection.ReadTimeout * 1000;
				connection.stream.WriteTimeout = HttpConnection.WriteTimeout * 1000;
#endif
			}

			return connection;
		}
        /// <summary>
        /// 请求是否超时的判断协程
        /// </summary>
        /// <returns></returns>
		IEnumerator Timeout ()
		{
			yield return new WaitForSeconds (timeout);
			if (!isDone) {
				exception = new TimeoutException ("Web request timed out");
				isDone = true;
			}
		}
        /// <summary>
        /// 添加http头部信息
        /// </summary>
		void AddHeadersToRequest ()
		{
			if (useCache) {
				string etag = "";
				if (etags.TryGetValue (uri.AbsoluteUri, out etag)) {
					headers.Set ("If-None-Match", etag);
				}
			}
			var hostHeader = uri.Host;
			if (uri.Port != 80 && uri.Port != 443) {
				hostHeader += ":" + uri.Port.ToString ();
			}
			headers.Set ("Host", hostHeader);
			if(!headers.Contains("User-Agent")) {
				headers.Add("User-Agent", "UniWeb (http://www.differentmethods.com)");
			}
	 
			if (acceptGzip) {
				headers.Set ("Accept-Encoding", "gzip");
			}


			if (enableCookies && uri != null) {
				try {
					var c = cookies.GetCookieHeader (uri);
					if (c != null && c.Length > 0) {
						headers.Set ("Cookie", c);
					}
				} catch (NullReferenceException) {
					//Some cookies make the .NET cookie class barf. MEGH again.
				} catch (IndexOutOfRangeException) {
					//Another weird exception that comes through from the cookie class. 
				}
			}

		}
        /// <summary>
        /// 开始发送请求数据
        /// </summary>
		void BeginSending()
		{
			isDone = false;

			if (timeout > 0)
			{
                //开启超时协议检测
				UniWeb.Instance.StartCoroutine(Timeout());
			}
            //使用线程池请求数据
			System.Threading.ThreadPool.QueueUserWorkItem(delegate (object state)
			{
				try
				{
                    //当前尝试重度的次数
					var retryCount = 0;
					HttpConnection connection = null;

					while (retryCount < maximumRedirects)
					{
						exception = null;
						AddHeadersToRequest();
						Uri pUri;
						if (proxy != null)
							pUri = proxy;
						else {
							try
							{
								if (System.Net.WebRequest.DefaultWebProxy != null)
									pUri = System.Net.WebRequest.DefaultWebProxy.GetProxy(uri);
								else
									pUri = uri;
							}
							catch (TypeInitializationException)
							{
								//Could not get WebRequest type... default to no proxy.
								pUri = uri;
							}
						}

						response = new Response(this);

						try
						{
							connection = CreateConnection(pUri.Host, pUri.Port, pUri.Scheme.ToLower() == "https");
#if BNICKSON_UPDATED
							WriteToStreamAsync(connection.stream);
#else
							WriteToStream(connection.stream);
#endif
							response.ReadFromStream(connection.stream, bodyStream);
						}
						catch (HTTPException ex)
						{
							EB.Debug.LogWarning("HTTPException {0}, retry ...", ex.Message);
							exception = ex;
							retryCount++;
							continue;
						}
						catch (ObjectDisposedException ex)
						{
							EB.Debug.LogWarning("ObjectDisposedException {0}, retry ...", ex.Message);
							exception = ex;
							retryCount++;
							continue;
						}
						catch (SocketException ex)
						{
							EB.Debug.LogWarning("SocketException {0}, retry ...", ex.Message);
							exception = ex;
							retryCount++;
							continue;
						}

						string key = string.Format("{0}:{1}", pUri.Host, pUri.Port);
						connectionPool[key] = connection;

						if (enableCookies)
						{
							foreach (var i in response.headers.GetAll("Set-Cookie"))
							{
								try
								{
									cookies.SetCookies(uri, i);
								}
								catch (System.Net.CookieException)
								{
									//Some cookies make the .NET cookie class barf. MEGH.
								}
							}
						}
						switch (response.status)
						{
							case 101:
								upgradedConnection = connection;
								break;
							case 304:
								break;
							case 307:
								uri = new Uri(response.headers.Get("Location"));
								retryCount++;
								continue;
							case 302:
							case 301:
								method = "GET";
								var location = response.headers.Get ("Location");
								if (location.StartsWith("/"))
								{
									uri = new Uri(uri, location);
								}
								else {
									uri = new Uri(location);
								}

								retryCount++;
								continue;
							default:
								break;
						}

						if (upgradedConnection == null)
						{
							if (response.protocol.ToUpper() == "HTTP/1.0" || response.headers.Get("Connection").ToUpper() == "CLOSE")
							{
								if (connectionPool.ContainsKey(key))
								{
									connectionPool.Remove(key);
								}
								connection.Dispose();
							}
						}
						break;
					}

					if (useCache && response != null)
					{
						var etag = response.headers.Get ("etag");
						if (etag.Length > 0)
						{
							etags[uri.AbsoluteUri] = etag;
						}
					}
				}
				catch (Exception e)
				{
					exception = e;
					response = null;
				}

				isDone = true;
			});
		}

		void WriteToStream (Stream outputStream)
		{
			uploadProgress = 0f;
			var stream = new BinaryWriter (outputStream);
			bool hasBody = false;
			var pathComponent = proxy==null?uri.PathAndQuery:uri.AbsoluteUri;
			stream.Write (HTTPProtocol.enc.GetBytes (method.ToUpper () + " " + pathComponent + " " + protocol));
			stream.Write (HTTPProtocol.EOL);
			if (uri.UserInfo != null && uri.UserInfo != "") {
				if (!headers.Contains ("Authorization")) {
					headers.Set ("Authorization", "Basic " + System.Convert.ToBase64String (HTTPProtocol.enc.GetBytes (uri.UserInfo)));  
				}
			}
			if (!headers.Contains ("Accept")) {
				headers.Add("Accept", "*/*");
			}
			if (bytes != null && bytes.Length > 0) {
				headers.Set ("Content-Length", bytes.Length.ToString ());
				// Override any previous value
				hasBody = true;
			} else {
				headers.Pop ("Content-Length");
			}
			
			headers.Write (stream);
			
			stream.Write (HTTPProtocol.EOL);
			
			if (hasBody) {
				var totalBytes = bytes.Length;
				var reader = new MemoryStream(bytes);
				var index = 0;
				var a = new byte[1024];
				while(index < totalBytes) {
					var readCount = reader.Read (a, 0, 1024);
					stream.Write(a, 0, readCount);
					uploadProgress += (totalBytes / (float)readCount);
					index += readCount;
				}
				//stream.Write (bytes);
			}
			//Debug.Log("WriteToStream done");
			uploadProgress = 1f;			
		}

#if BNICKSON_UPDATED
		void WriteToStreamAsync(Stream outputStream)
		{
			uploadProgress = 0f;
			bool hasBody = false;
			var pathComponent = proxy==null?uri.PathAndQuery:uri.AbsoluteUri;

			List<byte> writeBytes = new List<byte>();
			writeBytes.AddRange(HTTPProtocol.enc.GetBytes(method.ToUpper() + " " + pathComponent + " " + protocol));
			writeBytes.AddRange(HTTPProtocol.EOL);
			if (uri.UserInfo != null && uri.UserInfo != "")
			{
				if (!headers.Contains("Authorization"))
				{
					headers.Set("Authorization", "Basic " + System.Convert.ToBase64String(HTTPProtocol.enc.GetBytes(uri.UserInfo)));
				}
			}
			if (!headers.Contains("Accept"))
			{
				headers.Add("Accept", "*/*");
			}
			if (bytes != null && bytes.Length > 0)
			{
				headers.Set("Content-Length", bytes.Length.ToString());
				// Override any previous value
				hasBody = true;
			}
			else {
				headers.Pop("Content-Length");
			}

			headers.Write(writeBytes);

			writeBytes.AddRange(HTTPProtocol.EOL);

			if (hasBody)
			{
				writeBytes.AddRange(bytes);
			}

			byte[] byteArray = writeBytes.ToArray();
			var async = outputStream.BeginWrite(byteArray, 0, byteArray.Length, null, null);
			if (!async.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(HttpConnection.WriteTimeout)))
			{
				throw new HTTPException("Request WriteToStreamAsync timeout");
			}

			outputStream.Flush();
			//Debug.Log("WriteToStream done");
			uploadProgress = 1f;
		}
#endif

		IEnumerator _Wait ()
		{
			while (!isDone) {
				yield return null; 
			}
			if (OnDone != null) {
				OnDone (this);
			}
		}		

		static void AOTStrippingReferences ()
		{
			new System.Security.Cryptography.RijndaelManaged ();
		}

		byte[] bytes;
		public string method;
		string protocol = "HTTP/1.1";
		static Dictionary<string, string> etags = new Dictionary<string, string> ();
        /// <summary>
        /// 请求完成回调事件
        /// </summary>
		System.Action<HTTP.Request> OnDone;
		float uploadProgress = 0;
		Stream bodyStream = null;
#endregion

	}


}
