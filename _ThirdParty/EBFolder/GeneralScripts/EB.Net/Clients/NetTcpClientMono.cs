#if !UNITY_IPHONE || UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace EB.Net
{
	public class TcpClientMono : TcpClientEvent, ITcpClient
	{
		#region ITcpClient implementation
		System.Net.Sockets.TcpClient 		_client;

		System.Net.Sockets.NetworkStream 	_net;
		NetworkFailure						_error = NetworkFailure.None;

		System.Net.Security.SslStream 		_ssl;

		public NetworkFailure Error { get { return _error; } }

		System.IO.Stream					_stream;
		bool								_secure;
		System.IAsyncResult					_async;
		byte[] 								_readBuffer;
		int									_readCount;
		int									_readOffset;

		private static int RefCount = 0;

		public int Available { get { return _readCount-_readOffset; } }

		static List<string> _validCerts = new List<string>();

		public static void AddCertificate( byte[] certData )
		{
			try
			{
				var cert = new X509Certificate2(certData);
				var hash = Encoding.ToHexString(cert.GetCertHash());
				_validCerts.Add( hash );
				EB.Debug.Log("Adding cert hash: " + hash);
			}
			catch (System.Exception ex)
			{
				EB.Debug.LogError("Failed to load certificate! " + ex);
			}
		}

		public TcpClientMono(bool secure)
		{
			if (TcpClientUtil.OSSupportsIPv4())
			{
				_client = new System.Net.Sockets.TcpClient(System.Net.Sockets.AddressFamily.InterNetwork);
			}
			else if (TcpClientUtil.OSSupportsIPv6())
			{
				_client = new System.Net.Sockets.TcpClient(System.Net.Sockets.AddressFamily.InterNetworkV6);
			}

			_client.NoDelay = true;
			_secure = secure;
			_readBuffer = new byte[4096];
			_readOffset = 0;
			_readCount = 0;

			++RefCount;
			//Debug.Log("TcpClients: Construct " + RefCount);
		}

		bool TryConnect(string hostname, System.Net.IPAddress ip, int port, int connectTimeout)
		{
			//EB.Debug.Log("Try connect {0}:{1}", ip, port);

			if (_client.Client.AddressFamily != ip.AddressFamily)
			{
				_client.Close();
				_client = new System.Net.Sockets.TcpClient(ip.AddressFamily);
				_client.NoDelay = true;
			}

			var async 	= _client.BeginConnect( ip, port, null, null);
			if (!async.AsyncWaitHandle.WaitOne(System.TimeSpan.FromMilliseconds(connectTimeout)))
			{
				_error = NetworkFailure.TimedOut;
				return false;
			}
			if (!async.IsCompleted)
			{
				_error = NetworkFailure.TimedOut;
				return false;
			}
			_client.EndConnect(async);

			if (_client.Connected == false )
			{
				EB.Debug.LogError("Failed to connect to {0}:{1}", ip, port);
				_error = NetworkFailure.CannotConnectToHost;
				return false;
			}

			OnConnected();

			_net = _client.GetStream();			
			_stream = _net;

			if (_secure)
			{
				//EB.Debug.Log("Doing ssl connect {0}:{1}", ip, port);
				_ssl = new System.Net.Security.SslStream( _stream, true, RemoteCertificateValidationCallback, null);
				try
				{
					_ssl.AuthenticateAsClient( hostname );
				}
				catch (System.Exception e)
				{
					EB.Debug.LogError("Failed to authenticate: " + e);
					return false;
				}
				_stream = _ssl;
			}

			//EB.Debug.Log("Connected to {0}:{1}", ip, port);

			LastTime = System.DateTime.Now;

			return true;
		}

		public bool Connect (string host, int port, int connectTimeout)
		{
			var addreses = DNS.Lookup(host);
			if (addreses.Length == 0 )
			{
				EB.Debug.LogError("Failed to lookup {0}", host);
			}

			for (int i = 0; i < addreses.Length; ++i)
			{
				var ip = addreses[i];
				if (TryConnect(host, ip, port, connectTimeout))
				{
					if (i > 0) DNS.StoreLast(host, ip);
					return true;
				}
			}
			DNS.Clear(host);

			return false;
		}

		public void Close ()
		{
			--RefCount;
			//Debug.Log("TcpClients: Destroy " + RefCount);
			try
			{
				if (_ssl != null)
				{
					_ssl.Close();
				}

				if (_net != null)
				{
					_net.Close();
				}

				if (_client != null)
				{
					_client.Close();
				}

				if (_async != null)
				{
					_async = null;
				}
			}
			catch
			{

			}
			_client = null;
			_ssl = null;
			_stream = null;
			_async = null;
			_net = null;			
		}

		bool RemoteCertificateValidationCallback( System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			var hash = Encoding.ToHexString(certificate.GetCertHash());
			EB.Debug.Log("checking cert hash: " + hash);
			foreach( var cert in _validCerts )
			{
				if (hash == cert)
				{
					return true;
				}
			}
#if !DEBUG
			EB.Debug.LogError("Cant find certificate in pinned certificate!");
#else
			EB.Debug.LogWarning("RemoteCertificateValidationCallback: bad_certificate. it's harmless if proxy used");
#endif

			return false;
		}

		private void CheckRead()
		{
			if (_stream == null)
			{
				return;
			}

			if (_async != null)
			{
				if (_async.IsCompleted)
				{
					_readCount = _stream.EndRead(_async);
					_readOffset = 0;
					//Debug.Log("EndRead {0}", _readCount);
					_async = null;

					if (_readCount > 0)
					{
						LastTime = System.DateTime.Now;
					}
				}
			}
			else if (Available == 0)
			{
				//Debug.Log("BeginRead");
				_async = _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, null, null);
			}
		}

		public int Read (byte[] buffer, int offset, int count)
		{
			CheckRead();

			int read = 0;
			if ( Available > 0 )
			{
				read = Mathf.Min( count, Available );
				System.Array.Copy(_readBuffer, _readOffset, buffer, offset, read);
				_readOffset += read;
			}

			return read;
		}

		public void Write (byte[] buffer, int offset, int count)
		{
			_stream.Write(buffer,offset,count);
		}

		public int ReadTimeout {
			get {
				return _client.ReceiveTimeout;
			}
			set {
				_client.ReceiveTimeout = value;
				_net.ReadTimeout = ReadTimeout;
			}
		}

		public int WriteTimeout {
			get {
				return _client.SendTimeout;
			}
			set {
				_client.SendTimeout = value;
				_net.WriteTimeout = WriteTimeout;
			}
		}

		public bool Connected
		{
			get
			{
				if (_client.Connected==false)
				{
					return false;
				}

				if (_ssl != null)
				{
					return _ssl.IsAuthenticated;
				}

				return true;
			}
		}

		public bool DataAvailable
		{
			get
			{
				CheckRead();
				return Available > 0;
			}
		}

		public System.DateTime LastTime {get;private set;}
#endregion

	}
}
#endif