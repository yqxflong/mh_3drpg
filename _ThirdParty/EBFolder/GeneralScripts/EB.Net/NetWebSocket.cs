//#define WS_DEBUG
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace EB.Net
{
    public enum WebSocketCloseStatus
    {
        NormalClosure = 1000,
        EndpointUnavailable = 1001,
        ProtocolError = 1002,
        InvalidMessageType = 1003,
        Empty = 0,
        InvalidPayloadData = 1007,
        PolicyViolation = 1008,
        MessageTooBig = 1004,
        MandatoryExtension = 1010,
        InternalServerError,
    }

    public enum WebSocketState
    {
        None,
        Connecting,
        Open,
    }

    public enum WebSocketOpCode
    {
        Continuation = 0x0,
        Text = 0x1,
        Binary = 0x2,
        Close = 0x8,
        Ping = 0x9,
        Pong = 0xA,

        Control = 0x8, // control mask
    }

    public class WebSocket : System.IDisposable
    {
        public WebSocketState State { get { return _impl != null ? _impl._state : WebSocketState.None; } }
        public int PingTimeout { get; set; } // no ping and lost connection
        public int ActivityTimeout { get; set; } // if set, then we will close the connection if nothing is being sent.
        public int Ping { get { return _impl != null ? _impl._ping : 0; } }

        public static Uri Proxy { get; set; }

        // events
        public event System.Action OnConnect;
        public event System.Action<string> OnError;
        public event System.Action<string> OnMessage;
        public event System.Action<byte[]> OnData;

        public class WebSocketFrame
        {
            public bool fin;
            public bool rsv1;
            public bool rsv2;
            public bool rsv3;
            public int opcode;
            public int mask;
            public byte[] payload;
        }

        // data
        enum CompressType
        {
            none = 0,
            permessageDeflate = 1,
            deflate = 2,
        }
        protected class Impl
        {
            public WebSocket _parent;
            public ITcpClient _client;
            public Uri _uri;
            public bool _running;
            public Thread _thread;
            public System.Random _random = new System.Random();
            public WebSocketState _state;
            public string _protocol;
            public int _ping;
            private CompressType compressType = CompressType.none;

            public System.DateTime _lastPing;
            public System.DateTime _lastRead;
            public System.DateTime _lastActivity;

            public string _key;
            public string _accept;
            public Dictionary<string, string> _responseHeaders = new Dictionary<string, string>();
            public EB.Collections.Queue<WebSocketFrame> _writeQueue = new EB.Collections.Queue<WebSocketFrame>();

            public Impl(WebSocket parent)
            {
                _parent = parent;
            }

            public void Connect(Uri uri, string protocol, byte[] key)
            {
                GenerateKeyAndAccept(key);

                _uri = uri;
                _protocol = protocol;

                _state = WebSocketState.Connecting;

                _running = true;
                _thread = new Thread(DoConnectSafe, 256 * 1024);
                _thread.Start();
            }

            public void QueueFrame(WebSocketFrame frame)
            {
                lock (_writeQueue)
                {
                    _writeQueue.Enqueue(frame);
                }
            }

            void Error(string err)
            {
                if (IsActive())
                {
                    _parent.Error(err);
                }
            }

            bool IsActive()
            {
                return _parent._impl == this;
            }

            void OnConnect()
            {
                if (IsActive())
                {
                    if (_parent.OnConnect != null)
                    {
                        _parent.OnConnect();
                    }
                }
            }

            void OnMessage(string message)
            {
                if (IsActive())
                {
                    if (_parent.OnMessage != null)
                    {
                        _parent.OnMessage(message);
                    }
                }
            }

            void OnData(byte[] message)
            {
                if (IsActive())
                {
                    if (_parent.OnData != null)
                    {
                        _parent.OnData(message);
                    }
                }
            }

            void DoConnectSafe(object state)
            {
                try
                {
                    Thread.CurrentThread.Name = "websocket";
                }
                catch { }

                try
                {
                    if (DoConnect())
                    {
                        if (SendUpgradeRequest())
                        {
                            MainLoop();
                        }

                    }
                }
                catch (System.Threading.ThreadAbortException)
                {
                    // don't care
                }
                catch (System.Exception e)
                {
                    Error("exception " + e.ToString());
                }

                _state = WebSocketState.None;

                EB.Debug.Log("Disconnecting from socket " + _uri);
                try
                {
                    if (_client != null)
                    {
                        _client.Close();
                    }
                }
                catch
                {

                }
                _client = null;

                if (_parent != null)
                {
                    _parent.DidClose(this);
                }
            }

            bool DoConnect()
            {
                if (Proxy == null)
                {
                    _client = TcpClientFactory.Create(_uri.Scheme == "wss");

                    EB.Debug.Log("connect " + _uri.Host + " on port " + _uri.Port);
                    if (!_client.Connect(_uri.Host, _uri.Port, 5 * 1000))
                    {
                        _client.Close();
                        _client = null;
                        _state = WebSocketState.None;
                        Error("failed to connect");
                        return false;
                    }
                }
                else
                { // use proxy
                    bool secure = _uri.Scheme == "wss";

                    _client = TcpClientFactory.Create(secure);

                    if (secure)
                    {
                        (_client as TcpClientEvent).ConnectedEvent += delegate (object sender)
                        {
                            const string CRLF = "\r\n";

                            string request = "CONNECT " + _uri.Host + ":" + _uri.Port + " HTTP/1.1" + CRLF
                                + "Host: " + _uri.HostAndPort + CRLF
                                + CRLF;

                            var bytes = Encoding.GetBytes(request);
                            _client.Write(bytes, 0, bytes.Length);

                            string header = ReadLine(_client).ToLower();
                            if (!header.StartsWith("http/1.1 200 connection established") && !header.StartsWith("http/1.0 200 connection established"))
                            {
                                _client.Close();
                                _state = WebSocketState.None;
                                throw new System.Net.WebException("failed to connect to proxy");
                            }

                            // read the headers
                            while (true)
                            {
                                var line = ReadLine(_client);
                                //Debug.Log("line: " + line + " " + line.Length);
                                if (line.Length == 0)
                                {
                                    break;
                                }
                            }

                        };
                    }

                    EB.Debug.Log("connect proxy " + Proxy.Host + " on port " + Proxy.Port);
                    if (!_client.Connect(Proxy.Host, Proxy.Port, 5 * 1000))
                    {
                        _client.Close();
                        _client = null;
                        _state = WebSocketState.None;
                        Error("failed to connect proxy");
                        return false;
                    }
                }

                _client.ReadTimeout = 5 * 1000;
                _client.WriteTimeout = 5 * 1000;

                if (!_running)
                {
                    return false;
                }

                return true;
            }

            void AddRange(List<byte> dst, byte[] src)
            {
                foreach (var b in src)
                {
                    dst.Add(b);
                }
            }

            void GenerateKeyAndAccept(byte[] key)
            {
                if (key == null)
                {
                    key = Crypto.RandomBytes(64);
                }

                _key = System.Convert.ToBase64String(key);

                // generate the accept			
                List<byte> data = new List<byte>();
                AddRange(data, Encoding.GetBytes(_key));
                AddRange(data, Encoding.GetBytes("258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));

                var hash = EB.Digest.Sha1().Hash(data.ToArray());
                _accept = System.Convert.ToBase64String(hash);
            }

            private string ReadLine(ITcpClient s)
            {
                var data = new List<byte>(64);
                var buffer = new byte[1];
                while (true)
                {
                    int r = s.Read(buffer, 0, 1);
                    if (r == 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    byte b = buffer[0];
                    if (b == 10) // LF
                    {
                        break;
                    }
                    else if (b != 13) // CR
                    {
                        data.Add(b);
                    }
                }
                return Encoding.GetString(data.ToArray());
            }

            bool ParseHeader(string header)
            {
                //Debug.Log("Header: " + header);
                var split = header.IndexOf(':');
                if (split < 0)
                {
                    EB.Debug.LogError("Failed to parse header: " + header);
                    return false;
                }

                var key = header.Substring(0, split).Trim().ToLower();
                var value = header.Substring(split + 1).Trim();
                _responseHeaders[key] = value;

                //Debug.Log("Header " + key + " " + value);

                return true;
            }

            string GetResponseHeader(string key)
            {
                string value;
                if (_responseHeaders.TryGetValue(key.ToLower(), out value))
                {
                    return value;
                }
                return string.Empty;
            }

            byte[] Read(int count)
            {
                var buffer = new byte[count];
                int read = 0;

                while (read < count)
                {
                    if (!_running)
                    {
                        throw new System.Exception("Read aborted");
                    }

                    var n = _client.Read(buffer, read, count - read);
                    if (n < 0)
                    {
                        throw new System.Exception("Read Failed");
                    }
                    read += n;
                    if (n == 0)
                    {
                        Thread.Sleep(10);
                    }
                }
                return buffer;
            }

            System.Int16 ReadInt16()
            {
                var buffer = Read(2);
                System.Array.Reverse(buffer);
                return System.BitConverter.ToInt16(buffer, 0);
            }

            System.UInt16 ReadUInt16()
            {
                var buffer = Read(2);
                System.Array.Reverse(buffer);
                return System.BitConverter.ToUInt16(buffer, 0);
            }

            System.Int32 ReadInt32()
            {
                var buffer = Read(4);
                System.Array.Reverse(buffer);
                return System.BitConverter.ToInt32(buffer, 0);
            }

            System.Int64 ReadInt64()
            {
                var buffer = Read(8);
                System.Array.Reverse(buffer);
                return System.BitConverter.ToInt64(buffer, 0);
            }

            System.UInt64 ReadUInt64()
            {
                var buffer = Read(8);
                System.Array.Reverse(buffer);
                return System.BitConverter.ToUInt64(buffer, 0);
            }

            byte[] WriteInt16(System.Int16 value)
            {
                var buffer = System.BitConverter.GetBytes(value);
                System.Array.Reverse(buffer);
                return buffer;
            }

            byte[] WriteInt32(System.Int32 value)
            {
                var buffer = System.BitConverter.GetBytes(value);
                System.Array.Reverse(buffer);
                return buffer;
            }

            byte[] WriteInt64(System.Int64 value)
            {
                var buffer = System.BitConverter.GetBytes(value);
                System.Array.Reverse(buffer);
                return buffer;
            }

            WebSocketFrame ReadFrame()
            {
                WebSocketFrame frame = new WebSocketFrame();

                var header = Read(2);

                var first = header[0];
                var second = header[1];

                frame.fin = (first & 0x80) != 0;
                frame.rsv1 = (first & 0x40) != 0;
                frame.rsv2 = (first & 0x20) != 0; ;
                frame.rsv3 = (first & 0x10) != 0; ;
                frame.opcode = (first & 0x0F);

                var mask = (second & 0x80) != 0; ;


                var length = (second & 0x7F);

                if ((frame.opcode & (int)WebSocketOpCode.Control) != 0)
                {
                    // make sure size < 125
                    if (length > 125)
                    {
                        throw new System.Exception("Protocol error, control frame not allow to have payload > 125");
                    }
                }

                // check the length
                if (length == 126)
                {
                    // 16-bit length				
                    length = ReadUInt16();
                }
                else if (length == 127)
                {
                    // 64-bit length, but we don't support it length >2^31			
                    System.UInt64 l = ReadUInt64();
                    EB.Debug.LogWarning("NetWebSocket.ReadFrame: 64 bit length {0}, cast to Int32", l);
                    length = (int)l;
                }

                if (mask)
                {
                    // mask
                    frame.mask = ReadInt32();
                }

                if (_payloadCacheWriter == null)
                {
                    _payloadCache = new MemoryStream();
                    _payloadCacheWriter = new BinaryWriter(_payloadCache);
                    if (compressType == CompressType.permessageDeflate && frame.rsv1)
                    {
                        _permessagedeflate = true;
                    }
                    lastopcode = frame.opcode;
                }
                _payloadCacheWriter.Write(Read(length));
                if (frame.fin)
                {
                    if (_permessagedeflate)
                    {
                        _payloadCacheWriter.Write(trailer);
                    }

                    if (compressType == CompressType.none || (!_permessagedeflate && compressType == CompressType.permessageDeflate))
                    {
                        frame.payload = _payloadCache.GetBuffer();
                    }
					else
					{
                        frame.payload = Compress(_payloadCache.GetBuffer(), HTTP.Zlib.CompressionMode.Decompress);
					}

                    _permessagedeflate = false;
					_payloadCacheWriter = null;
                    frame.opcode = lastopcode;
                    return frame;
                }
                return null;

            }
            MemoryStream _payloadCache;
            BinaryWriter _payloadCacheWriter;
            bool _permessagedeflate;
            int lastopcode;
            static byte[] trailer = { 0x00, 0x00, 0xff, 0xff, 0x03, 0x00 };

            private byte[] Compress(byte[] src, HTTP.Zlib.CompressionMode mode)
            {
                var input = new MemoryStream(src);
                //move Plugins/DotNetZip to Plugins/Editor/DotNetZip to reduce package size
                //var inflater = new Ionic.Zlib.GZipStream(input, Ionic.Zlib.CompressionMode.Decompress);
                var inflater = new HTTP.Zlib.DeflateStream(input, mode);
                var output = new MemoryStream(src.Length * 5);
                var buffer = new byte[4096];
                while (true)
                {
                    int read = inflater.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        break;
                    }
                    output.Write(buffer, 0, read);
                }
                return output.ToArray();
            }

            void WriteFrame(WebSocketFrame frame)
            {
                List<byte> buffer = new List<byte>();

                // send fin and opcode
                byte tmp = (byte)frame.opcode;
                if (frame.fin)
                {
                    tmp |= 0x80;
                }
                buffer.Add(tmp);

                var len = frame.payload != null ? frame.payload.Length : 0;
                if (len <= 125)
                {
                    buffer.Add((byte)(0x80 | len));

                }
                else if (len < System.Int16.MaxValue)
                {
                    buffer.Add(0x80 | 126);
                    AddRange(buffer, WriteInt16((System.Int16)len));
                }
                else
                {
                    buffer.Add(0x80 | 127);
                    AddRange(buffer, WriteInt64(len));
                }

                // generate a mask
                frame.mask = _random.Next();

                var maskBytes = WriteInt32(frame.mask);
                AddRange(buffer, maskBytes);

                if (frame.payload != null)
                {
                    if (compressType == CompressType.deflate)
                    {
                        frame.payload = Compress(frame.payload, HTTP.Zlib.CompressionMode.Compress);
                    }

                    for (int i = 0; i < frame.payload.Length; ++i)
                    {
                        buffer.Add((byte)(frame.payload[i] ^ maskBytes[i % 4]));
                    }
                }

                _client.Write(buffer.ToArray(), 0, buffer.Count);

#if WS_DEBUG
				EB.Debug.Log("Frame: " + Encoding.ToHexString(buffer.ToArray()));
#endif
            }


            bool SendUpgradeRequest()
            {
                var host = _uri.HostAndPort;
                var origin = "http://" + host;
                //EB.Debug.LogError("tony debug log : " + _uri.PathAndQuery + "host:"  + host +"protocle" + _protocol);

                string request = "GET " + _uri.PathAndQuery + " HTTP/1.1\r\n" +
                        "Upgrade: WebSocket\r\n" +
                        "Connection: Upgrade\r\n" +
                        "Host: " + host + "\r\n" +
                        "Origin: " + origin + "\r\n" +
                        "Sec-WebSocket-Key: " + _key + "\r\n" +
                        "Sec-WebSocket-Protocol: " + _protocol + "\r\n" +
                        "Sec-WebSocket-Version: 13\r\n" +
                        "Sec-WebSocket-Extensions: permessage-deflate, deflate\r\n" +
                        "\r\n";

                var bytes = Encoding.GetBytes(request);
                _client.Write(bytes, 0, bytes.Length);

                string header = ReadLine(_client);
                if (header != "HTTP/1.1 101 Switching Protocols")
                    throw new IOException("Invalid handshake response: " + header);

                // read the headers
                while (true)
                {
                    if (!_running)
                    {
                        return false;
                    }

                    var line = ReadLine(_client);
                    //Debug.Log("line: " + line + " " + line.Length);
                    if (line.Length == 0)
                    {
                        break;
                    }

                    if (!ParseHeader(line))
                    {
                        EB.Debug.LogError("Failed to parse header: " + line);
                        break;
                    }
                }

                if (GetResponseHeader("connection").ToLower() != "upgrade")
                {
                    throw new IOException("Unknow connection header: " + GetResponseHeader("connection"));
                }

                if (GetResponseHeader("upgrade").ToLower() != "websocket")
                {
                    throw new IOException("Unknow upgrade header: " + GetResponseHeader("upgrade"));
                }

                // check key
                if (GetResponseHeader("Sec-WebSocket-Accept") != _accept)
                {
                    throw new IOException("Unknow connection accpet key " + GetResponseHeader("Sec-WebSocket-Accept"));
                }

				string exts = GetResponseHeader("Sec-WebSocket-Extensions");
				string[] extparam = exts.Split(';');
                if(extparam.Length < 1)
				{
                    compressType = CompressType.none;
				}
				else if (extparam[0] == "deflate")
                {
                    compressType = CompressType.deflate;
                }
                else if (extparam[0] == "permessage-deflate")
                {
                    compressType = CompressType.permessageDeflate;
                }
                else
                {
                    compressType = CompressType.none;
                }

                // connected!
                _state = WebSocketState.Open;

                OnConnect();

                return true;
            }

            int Since(System.DateTime dt)
            {
                return (int)((System.DateTime.Now - dt).TotalMilliseconds);
            }

            bool NeedPing()
            {
                if (_parent.PingTimeout == 0)
                {
                    return false;
                }

                var ms = Since(_lastPing);
                var ht = _parent.PingTimeout / 2;
                if (ms > ht)
                {
                    return true;
                }
                return false;
            }

            void CheckTimeout()
            {
                if (_parent.PingTimeout > 0)
                {
                    if (Since(_lastRead) > _parent.PingTimeout)
                    {
                        throw new IOException("connection timed out");
                    }
                }

                if (_parent.ActivityTimeout > 0)
                {
                    if (Since(_lastActivity) > _parent.ActivityTimeout)
                    {
                        EB.Debug.Log("Closing idle connection");
                        _running = false;
                    }
                }
            }

            void SendPing()
            {
#if WS_DEBUG
				EB.Debug.Log("sending ping");
#endif
                _lastPing = System.DateTime.Now;
                var frame = new WebSocketFrame();
                frame.fin = true;
                frame.opcode = (int)WebSocketOpCode.Ping;
                WriteFrame(frame);
            }

            void MainLoop()
            {
                _lastPing = System.DateTime.Now;
                _lastActivity = System.DateTime.Now;
                _lastRead = System.DateTime.Now;

                while (_running)
                {
#if WS_DEBUG
					EB.Debug.Log("main loop");
#endif
                    WebSocketFrame frame = null;
                    lock (_writeQueue)
                    {
                        if (_writeQueue.Count > 0)
                        {
                            frame = _writeQueue.Dequeue();
                        }
                    }

                    if (!_client.Connected)
                    {
#if WS_DEBUG
						EB.Debug.Log("lost connection");
#endif
                        Error("Lost connection");
                        return;
                    }
                    else if (frame != null)
                    {
                        _lastActivity = System.DateTime.Now;
                        WriteFrame(frame);
                    }
                    else if (_client.DataAvailable)
                    {
                        frame = ReadFrame();
                        if (frame != null)
                        {
                            switch (frame.opcode)
                            {
                                case (int)WebSocketOpCode.Text:
                                    {
                                        _lastActivity = System.DateTime.Now;
                                        OnMessage(Encoding.GetString(frame.payload));
                                    }
                                    break;
                                case (int)WebSocketOpCode.Binary:
                                    {
                                        _lastActivity = System.DateTime.Now;
                                        OnData(frame.payload);
                                    }
                                    break;
                                case (int)WebSocketOpCode.Ping:
                                    {
#if WS_DEBUG
									EB.Debug.Log("Got ping!");
#endif
                                        var reply = new WebSocketFrame();
                                        reply.fin = true;
                                        reply.opcode = (int)WebSocketOpCode.Pong;
                                        WriteFrame(reply);
                                    }
                                    break;
                                case (int)WebSocketOpCode.Pong:
                                    {
                                        _ping = Since(_lastPing);
#if WS_DEBUG
									EB.Debug.Log("Got Pong : " + Ping);
#endif
                                    }
                                    break;
                            }
                            _lastPing = System.DateTime.Now;
                            _lastRead = System.DateTime.Now;
                        }
                    }
                    else if (NeedPing())
                    {
                        SendPing();
                    }
                    else
                    {
                        CheckTimeout();
                        Thread.Sleep(10);
                    }
                }
            }
        }

        Impl _impl;

        public WebSocket()
        {
            PingTimeout = 10 * 1000; // 
#if DEBUG
            PingTimeout = 300 * 1000; // 
#endif
        }

        void DisposeImpl()
        {
            if (_impl != null)
            {
                _impl._running = false;
                _impl = null;
            }
        }

        void QueueFrame(WebSocketFrame frame)
        {
            if (_impl != null)
            {
                _impl.QueueFrame(frame);
            }
            else
            {
                Error("sending without connecting");
            }
        }

        public virtual void Dispose()
        {
            OnError = null;
            OnMessage = null;
            OnData = null;

            DisposeImpl();
        }

        public void ConnectAsync(Uri uri, string protocol, byte[] key)
        {
            DisposeImpl();

            _impl = new Impl(this);
            _impl.Connect(uri, protocol, key);
        }

        public void SendUTF8(string message)
        {
#if WS_DEBUG
			EB.Debug.Log("Send: " + message);
#endif
            var frame = new WebSocketFrame();
            frame.fin = true;
            frame.opcode = (int)WebSocketOpCode.Text;
            frame.payload = Encoding.GetBytes(message);
            QueueFrame(frame);
        }

        public void SendBinary(byte[] binary)
        {
#if WS_DEBUG
			EB.Debug.Log("SendBinary: " + binary.Length);
#endif
            //Debug.Log("Send: " + message);
            var frame = new WebSocketFrame();
            frame.fin = true;
            frame.opcode = (int)WebSocketOpCode.Binary;
            frame.payload = binary;
            QueueFrame(frame);
        }

        public void SendBinary(System.ArraySegment<byte> segment)
        {
#if WS_DEBUG
			EB.Debug.Log("SendBinary: " + segment.Count);
#endif
            //Debug.Log("Send: " + message);
            var frame = new WebSocketFrame();
            frame.fin = true;
            frame.opcode = (int)WebSocketOpCode.Binary;
            frame.payload = new byte[segment.Count];
            System.Array.Copy(segment.Array, segment.Offset, frame.payload, 0, segment.Count);
            QueueFrame(frame);
        }

        protected virtual void Error(string err)
        {
            EB.Debug.LogError("WebSocketError: " + err);

            DisposeImpl();

            if (OnError != null)
            {
                OnError(err);
            }
        }

        public void SendBinary(Buffer buffer)
        {
            SendBinary(buffer.ToArraySegment(false));
        }

        protected virtual void DidClose(Impl impl)
        {

        }
    }
}

