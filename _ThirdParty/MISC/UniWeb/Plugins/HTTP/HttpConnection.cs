using System;
using System.Net.Sockets;
using System.IO;
using System.Net.Security;

namespace HTTP
{
    /// <summary>
    /// http连接控件
    /// </summary>
	public class HttpConnection : IDisposable
	{
        /// <summary>
        /// 写入超时设置
        /// </summary>
		public static int WriteTimeout = 10;
        /// <summary>
        /// 读取超时设置
        /// </summary>
		public static int ReadTimeout = 25;
        /// <summary>
        /// 连接的超时设置
        /// </summary>
		public static int ConnectTimeout = 10;


		public string host;
		public int port;

		public TcpClient client = null;

		public Stream stream = null;
        /// <summary>
        /// http连接控件
        /// </summary>
        public HttpConnection ()
		{
			
		}
		/// <summary>
        /// 协议连接
        /// </summary>
		public void Connect ()
		{
			client = new TcpClient ();
#if BNICKSON_UPDATED
			client.NoDelay = true;
			client.SendTimeout = WriteTimeout * 1000;
			client.ReceiveTimeout = ReadTimeout * 1000;
            //ip地址路径
			var addreses = EB.Net.DNS.Lookup(host);
			if (addreses.Length == 0)
			{
				// retry
				throw new HTTPException(string.Format("HttpConnection.Connect failed to lookup {0}", host));
			}

			for (int i = 0; i < addreses.Length; ++i)
			{
				var ip = addreses[i];

				if (client.Client.AddressFamily != ip.AddressFamily)
				{
					client.Close();
					client = new TcpClient(ip.AddressFamily);
					client.NoDelay = true;
					client.SendTimeout = WriteTimeout * 1000;
					client.ReceiveTimeout = ReadTimeout * 1000;
				}
                //异步连接指定服务器
				var result = client.BeginConnect(ip, port, null, null);
                //连接结束就否超时
				var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(ConnectTimeout));
                //判断连接结果
				if (!success || !result.IsCompleted)
				{
					UnityEngine.Debug.LogWarningFormat("HttpConnection.BeginConnect to {0}:{1} timeout", ip, port);
					continue;
				}
                //帮定结果
				client.EndConnect(result);
                //存储上次的端口与ip
				if (i > 0) EB.Net.DNS.StoreLast(host, ip);
				break;
			}

			if (!client.Connected)
			{
				EB.Net.DNS.Clear(host);

				// retry
				string errMsg = string.Format("HttpConnection.Connect to {0}:{1} timeout.", host, port);
#if UNITY_EDITOR
				errMsg += " Notice: if UnityVS was installed this could be caused by UnityVS, Use 'Attach to Unity' or uninstall UnityVS to check";
#endif
				throw new HTTPException(errMsg);
			}
#else
			client.Connect (host, port);
#endif
			}

		public bool IsConnected()
		{
			return client != null && client.Connected && stream != null && (stream is SslStream == false || (stream as SslStream).IsAuthenticated);
		}

		public void Dispose ()
		{
#if BNICKSON_UPDATED
			client.Close();
#endif
			stream.Dispose ();
		}

	}
}

