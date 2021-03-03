using System;
using System.Net.Sockets;
using System.IO;
using System.Net.Security;

namespace HTTP
{
    /// <summary>
    /// http���ӿؼ�
    /// </summary>
	public class HttpConnection : IDisposable
	{
        /// <summary>
        /// д�볬ʱ����
        /// </summary>
		public static int WriteTimeout = 10;
        /// <summary>
        /// ��ȡ��ʱ����
        /// </summary>
		public static int ReadTimeout = 25;
        /// <summary>
        /// ���ӵĳ�ʱ����
        /// </summary>
		public static int ConnectTimeout = 10;


		public string host;
		public int port;

		public TcpClient client = null;

		public Stream stream = null;
        /// <summary>
        /// http���ӿؼ�
        /// </summary>
        public HttpConnection ()
		{
			
		}
		/// <summary>
        /// Э������
        /// </summary>
		public void Connect ()
		{
			client = new TcpClient ();
#if BNICKSON_UPDATED
			client.NoDelay = true;
			client.SendTimeout = WriteTimeout * 1000;
			client.ReceiveTimeout = ReadTimeout * 1000;
            //ip��ַ·��
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
                //�첽����ָ��������
				var result = client.BeginConnect(ip, port, null, null);
                //���ӽ����ͷ�ʱ
				var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(ConnectTimeout));
                //�ж����ӽ��
				if (!success || !result.IsCompleted)
				{
					UnityEngine.Debug.LogWarningFormat("HttpConnection.BeginConnect to {0}:{1} timeout", ip, port);
					continue;
				}
                //�ﶨ���
				client.EndConnect(result);
                //�洢�ϴεĶ˿���ip
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

