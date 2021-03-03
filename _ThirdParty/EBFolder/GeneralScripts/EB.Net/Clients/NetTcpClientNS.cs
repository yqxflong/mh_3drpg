using UnityEngine;
using System.Runtime.InteropServices;

namespace EB.Net
{
#if UNITY_IPHONE
	public class TcpClientNS : TcpClientEvent, ITcpClient
	{
		private System.IntPtr _handle;
		private bool _secure = false;

		NetworkFailure						_error = NetworkFailure.None;

		#region P/Invoke
		const string DLL_NAME = "__Internal";

		[DllImport(DLL_NAME)]
		static extern System.IntPtr 	_NSTcpClientCreate(bool secure);

		[DllImport(DLL_NAME)]
		static extern void 		_NSTcpClientDestory(System.IntPtr client);

		[DllImport(DLL_NAME)]
		static extern void 		_NSTcpClientConnect(System.IntPtr client, string host, int port);

		[DllImport(DLL_NAME)]
		static extern bool		_NSTcpClientConnected(System.IntPtr client);

		[DllImport(DLL_NAME)]
		static extern bool		_NSTcpClientError(System.IntPtr client);

		[DllImport(DLL_NAME)]
		static extern bool		_NSTcpClientDataAvailable(System.IntPtr client);

		[DllImport(DLL_NAME)]
		static extern int 		_NSTcpClientRead(System.IntPtr client, System.IntPtr buffer, int offset, int count);

		[DllImport(DLL_NAME)]
		static extern int 		_NSTcpClientWrite(System.IntPtr client, byte[] buffer, int offset, int count);

		[DllImport(DLL_NAME)]
		static extern System.IntPtr	_NSCreatePool();

		[DllImport(DLL_NAME)]
		static extern System.IntPtr 	_NSDestroyPool(System.IntPtr pool );

		[DllImport(DLL_NAME)]
		static extern void 		_NSImportCertificate(System.IntPtr data, int dataLength);

		#endregion

		public NetworkFailure Error { get { return _error; } }

		public static void AddCertificate( byte[] certData )
		{
			var ptr = Marshal.AllocHGlobal(certData.Length);
			Marshal.Copy(certData, 0, ptr, certData.Length);
			_NSImportCertificate(ptr, certData.Length);
			Marshal.FreeHGlobal(ptr);
			EB.Debug.Log("Adding cert: " + System.BitConverter.ToString(certData));
		}

		System.IntPtr _buffer;
		const int kBufferSize = 2048;

		#region ITcpClient implementation
		public TcpClientNS(bool secure)
		{
			_secure = secure;

			_buffer = Marshal.AllocHGlobal(kBufferSize);
		}

		bool TryConnect (string host, System.Net.IPAddress ip, int port, int connectTimeout)
		{
			//EB.Debug.Log("Try connect {0}:{1}", ip, port);

			DestroyClient();
			_handle = _NSTcpClientCreate(_secure);

			var timeout = System.DateTime.Now + System.TimeSpan.FromMilliseconds(connectTimeout);
			_NSTcpClientConnect(_handle,ip.ToString(),port);

			while( !Connected && System.DateTime.Now < timeout )
			{
				System.Threading.Thread.Sleep(100);
			}

			if (!Connected)
			{
				EB.Debug.LogError("Failed to connect to {0}:{1}", ip, port);
				_error = NetworkFailure.CannotConnectToHost;
				return false;
			}

			//OnConnected();
			//EB.Debug.Log("Connected to {0}:{1}", ip, port);

			return Connected;
		}

		// can only call connect once per tcp client
		public bool Connect (string host, int port, int connectTimeout)
		{
			var addreses = DNS.Lookup(host);
			if (addreses.Length == 0 )
			{
				EB.Debug.LogError("failed to lookup " + host);
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

		void DestroyClient()
		{
			if (_handle != System.IntPtr.Zero)
			{
				_NSTcpClientDestory(_handle);
				_handle = System.IntPtr.Zero;
			}
		}

		public void Close ()
		{
			DestroyClient();

			if (_buffer != System.IntPtr.Zero)
			{
				Marshal.FreeHGlobal(_buffer);
				_buffer = System.IntPtr.Zero;
			}
		}

		public int Read (byte[] buffer, int index, int count)
		{
			count = Mathf.Min(count,kBufferSize);
			var read = _NSTcpClientRead(_handle,_buffer,0,count);
			if ( read > 0 )
			{
				Marshal.Copy(_buffer, buffer, index, count);
				LastTime = System.DateTime.Now;
				//Debug.Log("NSRead: " + Encoding.ToHexString(buffer, index, count, 1) );
			}
			else if ( read < 0 )
			{
				// read failure
				throw new System.IO.IOException("Read Failed!");
			}
			return read;
		}

		public void Write (byte[] buffer, int index, int count)
		{
			while( count > 0 )
			{
				int w = _NSTcpClientWrite(_handle,buffer,index,count);
				if ( w < 0 )
				{
					throw new System.IO.IOException("Write Failed!");
				}
				else if (w == 0)
				{
					// wait
					System.Threading.Thread.Sleep(100);
				}
				else
				{
					count -= w;
					index += w;
				}
			}
		}

		public int ReadTimeout {
			get {
				return 0;
			}
			set {

			}
		}

		public int WriteTimeout {
			get {
				return 0;
			}
			set {

			}
		}

		public System.DateTime LastTime {
			get; private set;
		}

		public bool Connected {
			get
			{
				if (_NSTcpClientError(_handle))
				{
					return false;
				}

				return _NSTcpClientConnected(_handle);
			}
		}

		public bool DataAvailable {
			get {
				return _NSTcpClientDataAvailable(_handle);
			}
		}
		#endregion
	}
#endif
}

