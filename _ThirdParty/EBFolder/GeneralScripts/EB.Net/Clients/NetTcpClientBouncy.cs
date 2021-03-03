#if !UNITY_IPHONE || UNITY_EDITOR
//#define BCWP71
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace EB.Net
{
	public class TcpClientBouncy : TcpClientEvent, ITcpClient
	{
		#region ITcpClient implementation
		System.Net.Sockets.TcpClient 		_client;

		System.Net.Sockets.NetworkStream 	_net;
		NetworkFailure						_error = NetworkFailure.None;

#if BCWP71
		TlsProtocolHandler                  _handler;
#else
		TlsClientProtocol                  _handler;
#endif
		MyTlsAuthentication _auth;
		MyTlsClient							_tlsClient;

		public NetworkFailure Error { get { return _error; } }


		class MyTlsAuthentication : TlsAuthentication
		{
			public void NotifyServerCertificate (Certificate serverCertificate)
			{
#if BCWP71
				var certs = serverCertificate.GetCerts();
#else
				var certs = serverCertificate.GetCertificateList();
#endif
				foreach( var cert in certs )
				{
					var encoding = cert.GetDerEncoded();
					var base64   = Encoding.ToBase64String(encoding);
					if (_validCerts.IndexOf(base64) >= 0)
					{
						return;
					}
				}

// #if !DEBUG
// 				throw new TlsFatalAlert(AlertDescription.bad_certificate);
// #else
// 				EB.Debug.LogWarning("NotifyServerCertificate: bad_certificate. it's harmless if proxy used");
// #endif
				EB.Debug.LogWarning("NotifyServerCertificate: bad_certificate. it's harmless if proxy used");
			}

			public TlsCredentials GetClientCredentials (CertificateRequest certificateRequest)
			{
				return null;
			}
		}


		class MyTlsClient : DefaultTlsClient
		{
			TlsAuthentication _auth;

			public MyTlsClient( TlsAuthentication auth )
			{
				_auth = auth;
			}

			public override TlsAuthentication GetAuthentication ()
			{
				return _auth;
			}
		}

		System.IO.Stream					_stream;
		bool								_secure;
		System.IAsyncResult					_async;
		byte[] 								_readBuffer;
		int									_readCount;
		int									_readOffset;

		static List<string> _validCerts = new List<string>();

		public static void AddCertificate( byte[] certData )
		{
			try
			{
				var parser = new Org.BouncyCastle.X509.X509CertificateParser();
				var cert = parser.ReadCertificate(certData);
				if (cert != null)
				{
					EB.Debug.Log("Adding cert " + cert.SubjectDN.ToString() );
					_validCerts.Add( Encoding.ToBase64String(cert.GetEncoded()));
				}
			}
			catch (System.Exception ex)
			{
				EB.Debug.LogError("Failed to load certificate! " + ex);
			}
		}

		private static int RefCount = 0;

		public int Available { get { return _readCount-_readOffset; } }

		public TcpClientBouncy(bool secure)
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
			_readBuffer = new byte[4*1024];
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

			var async   = _client.BeginConnect( ip, port, null, null);
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

			_net = _client.GetStream();			
			_stream = _net;

			OnConnected();

			if (_secure)
			{
				//EB.Debug.Log("Doing ssl connect {0}:{1}", ip, port);
				try {
					var random = new System.Random();
					var bytes = new byte[20];
					random.NextBytes(bytes);

#if BCWP71
					var secureRandom = new SecureRandom(bytes);
#else
					var secureRandom = SecureRandom.GetInstance("SHA1PRNG", false);
#endif
					secureRandom.SetSeed(bytes);

					 _auth = new MyTlsAuthentication();
					_tlsClient = new MyTlsClient(_auth);
#if BCWP71
					_handler = new TlsProtocolHandler(_net, secureRandom);
#else
					_handler = new TlsClientProtocol(_net, secureRandom);
#endif
					_handler.Connect(_tlsClient);
					_stream = _handler.Stream;
					if (_stream == null)
					{
						EB.Debug.LogError("stream is null");
						_error = NetworkFailure.SecureConnectionFailed;
						return false;
					}
				}
				catch (System.Exception ex)
				{
					EB.Debug.LogError("ssl connect failed {0}\n{1}", ex.Message, ex.StackTrace);
					_error = NetworkFailure.SecureConnectionFailed;
					return false;
				}
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
				EB.Debug.LogError("failed to lookup {0}", host);
				_error = NetworkFailure.DNSLookupFailed;
				return false;
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
				if (_stream != null)
				{
					_stream.Close();
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
			_stream = null;
			_async = null;
			_net = null;
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
			var async = _stream.BeginWrite(buffer, offset, count, null, null);
			_stream.EndWrite(async);
			async = null;
			//_stream.Write(buffer,offset,count);
		}

		public int ReadTimeout {
			get {
				return _client.ReceiveTimeout;
			}
			set {
				_client.ReceiveTimeout = value;
				_net.ReadTimeout = value;
			}
		}

		public int WriteTimeout {
			get {
				return _client.SendTimeout;
			}
			set {
				_client.SendTimeout = value;
				_net.WriteTimeout = value;
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

				if (_secure)
				{
					return _handler != null && _handler.Stream != null;
				}

				return true;
			}
		}

		public bool DataAvailable
		{
			get
			{
				//CheckRead();
				//return Available > 0;
				if (_net != null)
				{
					return _net.DataAvailable || Available > 0;
				}
				return false;
			}
		}

		public System.DateTime LastTime {get;private set;}
#endregion

	}
}
#endif