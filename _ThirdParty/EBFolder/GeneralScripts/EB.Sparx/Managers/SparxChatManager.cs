using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;

namespace EB.Sparx
{
	public class ChatMessage
	{
		public string channel;
		public string id;
		public System.Net.IPAddress ip;
		public long uid;
		public string name;
		public int level;
		public EB.Language language;
		public EB.Country country;
		public string text;
		public string lower;
		public long privateUid;
		public string privateName;
		public long ts;
        public int monthVipType;//1:���� 2���ƽ� 3��˫��
		public string achievementType;

		public bool isAudio;
		public UnityEngine.AudioClip audioClip;
		public bool isRead;

		public string channelType;
		public int vipLevel;
		public string icon;
        public string frame;
        public int quality;
		public int battleRating;
		public string allianceName;
		public object json;

		public ChatMessage()
		{
			language = Language.English;
			country = Country.USA;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}

			if (obj == null)
			{
				return false;
			}

			if (obj is ChatMessage == false)
			{
				return false;
			}

			ChatMessage cmp = obj as ChatMessage;
			return this.id == cmp.id;
		}

		public override int GetHashCode()
		{
			return EB.Hash.StringHash(this.id);
		}

		// avoid gc
		static short[] wavBuffer = new short[10 * 8000 * 1];
		static int wavBufferLength = 0;
		static float[] samplesBuffer = new float[10 * 8000 * 1];
		static int samplesBufferLength = 0;
		static short[] wavFrameBuffer = new short[AudioWraper.PCM_FRAME_SIZE];
		static byte[] amrFrameBuffer = new byte[500];

		public static ChatMessage Parse(object json)
		{
			try
			{
				ChatMessage msg = new ChatMessage();

				msg.json = json;
				msg.channel = EB.Dot.String("channel", json, msg.channel);
				msg.id = EB.Dot.String("id", json, msg.id);
				System.Net.IPAddress.TryParse(EB.Dot.String("ip", json, string.Empty), out msg.ip);
				msg.text = EB.Dot.String("text", json, string.Empty);
				msg.text = msg.text.Replace("\n", "");
				msg.lower = EB.Dot.String("lower", json, msg.lower);
				msg.uid = EB.Dot.Long("attributes.uid", json, EB.Dot.Long("uid", json, msg.uid));
				msg.name = EB.Dot.String("attributes.name", json, EB.Dot.String("name", json, msg.name));
				msg.level = EB.Dot.Integer("attributes.level", json, msg.level);
				msg.channelType = EB.Dot.String("attributes.channel_type", json, msg.channelType);
				msg.vipLevel = EB.Dot.Integer("attributes.vip_level", json, msg.vipLevel);
				msg.icon = EB.Dot.String("attributes.head_icon", json, msg.icon);
				msg.frame = EB.Dot.String("attributes.head_frame", json, msg.frame);
				msg.quality = EB.Dot.Integer("attributes.quality", json, msg.quality);
				msg.battleRating = EB.Dot.Integer("attributes.battle_rating", json, msg.battleRating);
				msg.allianceName = EB.Dot.String("attributes.alliance_name", json, msg.allianceName);
				msg.privateUid = EB.Dot.Long("attributes.private_uid", json, msg.privateUid);
				msg.privateName = EB.Dot.String("attributes.private_name", json, msg.privateName);
				msg.monthVipType = EB.Dot.Integer("attributes.month_vip_type", json, msg.monthVipType);
				msg.achievementType = EB.Dot.String("attributes.achievement_type", json, msg.achievementType);
				msg.ts = EB.Dot.Long("ts", json, EB.Dot.Long("_ts", json, msg.ts));

				msg.isAudio = EB.Dot.Bool("attributes.is_audio", json, msg.isAudio);
				if (msg.isAudio)
				{
					string audioName = EB.Dot.String("attributes.audio.name", json, string.Empty);
					string audioFormat = EB.Dot.String("attributes.audio.format", json, string.Empty);

					// decode audio
					if (audioFormat == "amr")
					{
						System.IntPtr amr = AudioWraper.Decoder_Interface_init();
						try
						{
							// amr to wav
							string audioDataEncoded = EB.Dot.String("attributes.audio.data", json, EB.Encoding.ToBase64String(new byte[0]));
							byte[] amrData = EB.Encoding.FromBase64String(audioDataEncoded);

							wavBufferLength = 0;

							int amrDataIndex = 0;
							while (amrDataIndex < amrData.Length)
							{
								int amrBufferLength = 0;

								/* Read the mode byte */
								amrFrameBuffer[amrBufferLength++] = amrData[amrDataIndex++];
								/* Find the packet size */
								int size = AudioWraper.SIZES[(amrFrameBuffer[0] >> 3) & 0x0f];
								//EB.Debug.Log("DecodeAudioFrame: mode = {0}, size = {1}", amrBuffer[0], size);
								System.Array.Copy(amrData, amrDataIndex, amrFrameBuffer, amrBufferLength, size);
								/* Decode the packet */
								AudioWraper.Decoder_Interface_Decode(amr, amrFrameBuffer, wavFrameBuffer, 0);

								/* Append to Buffer */
								if (wavBufferLength + wavFrameBuffer.Length > wavBuffer.Length)
								{
									EB.Debug.LogWarning("ParseAudio: wavBuffer overflow {0} > {1}", wavBufferLength + wavFrameBuffer.Length, wavBuffer.Length);
									break;
								}
								System.Array.Copy(wavFrameBuffer, 0, wavBuffer, wavBufferLength, wavFrameBuffer.Length);
								wavBufferLength += wavFrameBuffer.Length;

								amrDataIndex += size;
							}

							samplesBufferLength = wavBufferLength;
							if (samplesBufferLength > samplesBuffer.Length)
							{
								EB.Debug.LogWarning("ParseAudio: samplesBuffer overflow {0} > {1}", samplesBufferLength, samplesBuffer.Length);
								return null;
							}
							AudioWraper.ConvertShortToFloat(wavBuffer, wavBufferLength, samplesBuffer, 0);

							msg.audioClip = UnityEngine.AudioClip.Create(audioName, samplesBufferLength / AudioWraper.CHANNELS, AudioWraper.CHANNELS, AudioWraper.FREQUENCY, false);
							if (!msg.audioClip.SetData(samplesBuffer, 0))
							{
								EB.Debug.LogWarning("ParseAudio: SetData failed");
								return null;
							}
						}
						catch (System.Exception ex)
						{
							EB.Debug.LogError("ChatMessage.Parse: error, json = {0}", EB.JSON.Stringify(json));
							UnityEngine.Debug.LogException(ex);
							return null;
						}
						finally
						{
							AudioWraper.Decoder_Interface_exit(amr);
						}
					}
					else
					{
						int audioLengthSamples = EB.Dot.Integer("attributes.audio.lengthSamples", json, 0);
						int audioFrequency = EB.Dot.Integer("attributes.audio.frequency", json, 0);
						int audioChannels = EB.Dot.Integer("attributes.audio.channels", json, 0);
						string audioDataEncoded = EB.Dot.String("attributes.audio.data", json, EB.Encoding.ToBase64String(AudioWraper.ConvertFloatToByteArray(new float[0])));
						float[] audioData = AudioWraper.ConvertByteArrayToFloat(EB.Encoding.FromBase64String(audioDataEncoded));

						msg.audioClip = UnityEngine.AudioClip.Create(audioName, audioLengthSamples, audioChannels, audioFrequency, false);
						msg.audioClip.SetData(audioData, 0);
					}

					msg.isRead = false;
				}

				string locale = EB.Dot.String("locale", json, string.Format("{0}-{1}", EB.Localizer.GetLanguageCode(msg.language), EB.Localizer.GetCountryCode(msg.country)));
				string[] splits = locale.Split(new char[] {'-', '_'}, System.StringSplitOptions.RemoveEmptyEntries);
				if (splits.Length > 0 && EB.Symbols.LanguageCode.ContainsKey(splits[0].ToLower()))
				{
					msg.language = EB.Symbols.LanguageCode[splits[0].ToLower()];
					msg.country = EB.Localizer.GetDefaultCountry(msg.language);
				}
				if (splits.Length > 1 && EB.Symbols.CountryCode.ContainsKey(splits[1].ToUpper()))
				{
					msg.country = EB.Symbols.CountryCode[splits[1].ToUpper()];
				}

				if (string.IsNullOrEmpty(msg.channelType))
				{
					msg.channelType = msg.channel.Split(new char[] { '_' })[0];
				}

				return msg;
			}
			catch(System.NullReferenceException e)
			{
				EB.Debug.LogError(e.ToString());
				return null;
			}
		}
	}

	public class ChatManager : SubSystem, Updatable
	{
		public const string Protocol = "io.sparx.chat";
		public const int HistoryLimit = 50;

		public System.Action OnDisconnected;
		public System.Action OnConnected;
		public System.Action<ChatMessage[]> OnMessages;

		PushAPI _api;
		Net.TalkWebSocket _socket;
		Deferred _deffered;
		EB.Uri _url;

		EB.CircularBuffer<ChatMessage> _msgs = new CircularBuffer<ChatMessage>(50);
		Dictionary<string, float> _lastSendTime = new Dictionary<string, float>();
		List<string> _joinedChannels = new List<string>();

		System.Action<string> _recognizeCallback = null;

		public ChatMessage[] Messages
		{
			get { return _msgs.ToArray(); }
		}

#region implemented abstract members of EB.Sparx.SubSystem
		public override void Initialize(Config config)
		{
			_api = new PushAPI(Hub.ApiEndPoint);
			_deffered = new Deferred(4);
			_url = null;

			new GameObject("recognize_callback", typeof(AudioRecognizeReceiver));
			AudioWraper.InitRecognizer();
		}

		public bool UpdateOffline { get { return false; } }

		public override void Connect()
		{
			State = SubSystemState.Connecting;

			var push = Dot.Object("chat", Hub.DataStore.LoginDataStore.LoginData, null);
			if (push != null)
			{
				OnGetPushToken(null, push);
			}
			else
			{
				_api.GetPushToken(OnGetPushToken);
			}
		}

		public void Update()
		{
			try{
				_deffered.Dispatch();
			}
			catch(System.NullReferenceException e)
			{
				EB.Debug.LogError(e.ToString());
			}
		}

		public override void Disconnect(bool isLogout)
		{
			State = SubSystemState.Disconnected;

			if (_socket != null)
			{
				_socket.Dispose();
				_socket = null;
			}

			_msgs.Reset();
			_joinedChannels.Clear();

			_recognizeCallback = null;
			AudioWraper.CancelRecognize();
		}

		public override void Dispose()
		{
			if (_socket != null)
			{
				_socket.Dispose();
				_socket = null;
			}
		}

		public override void OnEnteredBackground()
		{
			if (_socket != null)
			{
				_socket.Dispose();
				_socket = null;
			}

			_recognizeCallback = null;
			AudioWraper.CancelRecognize();
		}

		public override void OnEnteredForeground()
		{
			if (_socket != null)
			{
				_socket.Dispose();
				_socket = null;
			}

			_recognizeCallback = null;
			AudioWraper.CancelRecognize();

			ConnectWebsocket();
		}
#endregion

		private void SimpleRPC(string name, ArrayList args, System.Action<string, object> callback)
		{
			// EB.Debug.Log("->SimpleN " + str);
			if (_socket != null && _socket.State == EB.Net.WebSocketState.Open)
			{
				_socket.RPC(name, args, delegate (string err, object result)
				{
					_deffered.Defer(callback, err, result);
				});
			}
			else if (callback != null)
			{
				callback("ID_SPARX_NETWORK_ERROR", null);
			}
		}

		/// <summary>
		/// join channel
		/// </summary>
		/// <param name="channel">world_{worldId} | alliance_{allianceId} | private_{uid}</param>
		/// <param name="options">wid: int worldId, history: boolean fetch history</param>
		/// <param name="callback"></param>
		public void Join(string channel, Hashtable options, System.Action<string, object> callback)
		{
			string channelType = channel.Split(new char[] {'_'})[0];
			if (_joinedChannels.Contains(channelType))
			{
				EB.Debug.LogWarning("Join: channel {0} joined", channelType);
				callback("ID_CHAT_JOINED", null);
				return;
			}
			_joinedChannels.Add(channelType);

			options = options ?? Johny.HashtablePool.Claim();

			ArrayList args = new ArrayList();
			args.Add(channel);
			args.Add(options);
			SimpleRPC("join", args, callback);
		}

		/// <summary>
		/// leave channel
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="options"></param>
		/// <param name="callback"></param>
		public void Leave(string channel, Hashtable options, System.Action<string, object> callback)
		{
			string channelType = channel.Split(new char[] {'_'})[0];
			if (!_joinedChannels.Contains(channelType))
			{
				EB.Debug.Log("Leave: channel {0} not joined", channelType);
				callback(null, null);
				return;
			}
			_joinedChannels.Remove(channelType);

			options = options ?? Johny.HashtablePool.Claim();

			ArrayList args = new ArrayList();
			args.Add(channel);
			args.Add(options);
			SimpleRPC("leave", args, callback);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="text"></param>
		/// <param name="attributes">channel_type: string world | alliance | private</param>
		/// <param name="callback"></param>
		public void SendText(string channel, string text, Hashtable attributes, System.Action<string, object> callback)
		{
			attributes = attributes ?? Johny.HashtablePool.Claim();
			attributes["is_audio"] = false;
			attributes["audio"] = null;
			attributes["channel_type"] = attributes["channel_type"] ?? channel.Split('_')[0];

			if (!IsJoined(attributes["channel_type"].ToString()))
			{
				EB.Debug.LogWarning("SendText: channel {0} not joined", attributes["channel_type"]);
				callback("ID_CHAT_NOT_JOINED", null);
				return;
			}

			ArrayList args = new ArrayList();
			args.Add(channel);
			args.Add(text);
			args.Add(attributes);
			SimpleRPC("send", args, callback);

			SetSendTime(attributes["channel_type"].ToString());
		}

		short[] wavFrameBuffer = new short[AudioWraper.PCM_FRAME_SIZE];
		byte[] amrBuffer = new byte[500];
		MemoryStream amrStream = new MemoryStream();
		float[] samplesBuffer = new float[10 * 8000 * 1]; // max = 10s * 8000hz/s * 1channel
		int samplesBufferLength = 0;
		short[] wavBuffer = new short[10 * 8000 * 1];
		int wavBufferLength = 0;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="audio"></param>
		/// <param name="attributes">channel_type: string world | alliance | private</param>
		/// <param name="callback"></param>
		public void SendAudio(string channel, UnityEngine.AudioClip audio, int samplePos, Hashtable attributes, System.Action<string, object> callback)
		{
			samplesBufferLength = samplePos * audio.channels;
			if (samplesBufferLength > samplesBuffer.Length)
			{
				EB.Debug.LogError("SendAudio: samplesBuffer overflow {0} > {1}", samplesBufferLength, samplesBuffer.Length);
				callback(null, null);
				return;
			}
			if (!audio.GetData(samplesBuffer, 0))
			{
				EB.Debug.LogError("SendAudio: read samplesBuffer failed");
				callback(null, null);
				return;
			}

			// wav to amr
			if (audio.channels != AudioWraper.CHANNELS)
			{
				EB.Debug.LogWarning("Warning, only compressing one audio channel");

				AudioWraper.FetchFirstChannel(samplesBuffer, ref samplesBufferLength, audio.channels);
			}
			if (audio.frequency != AudioWraper.FREQUENCY)
			{
				EB.Debug.LogWarning("Warning, AMR-NB uses 8000 Hz sample rate (WAV file has {0} Hz)", audio.frequency);
			}

			wavBufferLength = samplesBufferLength;
			if (wavBufferLength > wavBuffer.Length)
			{
				EB.Debug.LogError("SendAudio: wavBuffer overflow {0} > {1}", wavBufferLength, wavBuffer.Length);
				callback(null, null);
				return;
			}
			AudioWraper.ConvertFloatToShort(samplesBuffer, samplesBufferLength, wavBuffer);

			// audio filter
			EB.Coroutines.Run(StartRecognize(wavBuffer, wavBufferLength, delegate(string result)
			{
				if (!string.IsNullOrEmpty(result) && !EB.ProfanityFilter.Test(result))
				{
					EB.Debug.LogWarning("SendAudio: audio contains profanity words, {0}", result);
					SendText(channel, EB.ProfanityFilter.Filter(result), attributes, callback);
					return;
				}

				System.IntPtr amr = AudioWraper.Encoder_Interface_init(0);

				// reset memory stream buffer
				amrStream.Position = 0;
				amrStream.SetLength(0);

				int wavBufferOffset = 0;
				while (wavBufferOffset < wavBufferLength)
				{
					if (wavBufferOffset + AudioWraper.PCM_FRAME_SIZE > wavBufferLength)
					{
						break;
					}

					System.Array.Copy(wavBuffer, wavBufferOffset, wavFrameBuffer, 0, AudioWraper.PCM_FRAME_SIZE);
					int length = AudioWraper.Encoder_Interface_Encode(amr, AudioWraper.Mode.MR122, wavFrameBuffer, amrBuffer, 0);
					//EB.Debug.Log("EncodeAudioFrame: mode = {0}, length = {1}", amrBuffer[0], length);
					amrStream.Write(amrBuffer, 0, length);
					wavBufferOffset += AudioWraper.PCM_FRAME_SIZE;
				}
				AudioWraper.Encoder_Interface_exit(amr);
				string data = EB.Encoding.ToBase64String(amrStream.GetBuffer(), 0, (int)amrStream.Position);

				Hashtable audioHash = new Hashtable()
				{
					{ "name", audio.name },
					{ "data",  data },
					{ "format", "amr" },
				};

				EB.Debug.Log("SendAudio: name = {0}, length = {1}, samples = {2}, channels = {3}, frequency = {4}, data.Length = {5}",
					audio.name, audio.length, audio.samples, audio.channels, audio.frequency, data.Length);

				attributes = attributes ?? Johny.HashtablePool.Claim();
				attributes["is_audio"] = true;
				attributes["audio"] = audioHash;
				attributes["channel_type"] = attributes["channel_type"] ?? channel.Split('_')[0];

				if (!IsJoined(attributes["channel_type"].ToString()))
				{
					EB.Debug.LogWarning("SendAudio: channel {0} not joined", attributes["channel_type"]);
					callback("ID_CHAT_NOT_JOINED", null);
					return;
				}

				ArrayList args = new ArrayList();
				args.Add(channel);
				args.Add("default");
				args.Add(attributes);
				SimpleRPC("send", args, callback);

				SetSendTime(attributes["channel_type"].ToString());
			}));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="options">last: string last message id, limit: int max message count</param>
		/// <param name="callback"></param>
		public void History(string channel, Hashtable options, System.Action<string, object> callback)
		{
			string channelType = channel.Split(new char[] {'_'})[0];
			if (!IsJoined(channelType))
			{
				EB.Debug.LogWarning("History: channel {0} not joined", channelType);
				callback("ID_CHAT_NOT_JOINED", null);
				return;
			}

			options = options ?? Johny.HashtablePool.Claim();
			options["limit"] = options["limit"] ?? HistoryLimit;
			//加上这个 服务器不会把历史记录广播给其他人
			options["uid"] = Hub.Instance.DataStore.LoginDataStore.LocalUserId.ToString();
			ArrayList args = new ArrayList();
			args.Add(channel);
			args.Add(options);
			SimpleRPC("history", args, callback);
		}

		public ChatMessage[] GetMessages(string channel)
		{
			var msgs = Messages;
			return System.Array.FindAll(msgs, m => m.channelType == channel);
		}

		public float GetLastSendTime(string channel)
		{
			if (_lastSendTime.ContainsKey(channel))
			{
				return _lastSendTime[channel];
			}

			return 0.0f;
		}

		public bool IsJoined(string channel)
		{
			return _joinedChannels.Contains(channel);
		}

		public void SetSendTime(string channel)
		{
			_lastSendTime[channel] = Time.realtimeSinceStartup;
		}

		public void HandlePublicMessage(string text)
		{
			ChatMessage msg = new ChatMessage();
			msg.channel = "world";
			msg.channelType = "world";
			msg.name = Localizer.GetString("ID_CHAT_SYSTEM_NAME");
			msg.vipLevel = -1;
			msg.icon = "Header_Xitong";
			msg.uid = 0;
			msg.quality = 0;
			msg.text = text;
			msg.ts = EB.Time.Now;

			while (_msgs.Count >= _msgs.bufferSize)
			{
				_msgs.Dequeue();
			}
			_msgs.Enqueue(msg);

			if (OnMessages != null)
			{
				OnMessages(new ChatMessage[] { msg });
			}
			
		}

		public void HandleAllianceMessage(string text)
		{
			ChatMessage msg = new ChatMessage();

			msg.channel = "alliance";
			msg.channelType = "alliance";
			msg.name = Localizer.GetString("ID_CHAT_SYSTEM_NAME");
			msg.vipLevel = -1;
			msg.icon = "Header_Xitong";
			msg.uid = 0;
			msg.quality = 0;
			msg.text = text;
			msg.ts = EB.Time.Now;

			while (_msgs.Count >= _msgs.bufferSize)
			{
				_msgs.Dequeue();
			}
			_msgs.Enqueue(msg);

			if (OnMessages != null)
			{
				OnMessages(new ChatMessage[] { msg });
			}
		}

		public void HandleTeamMessage(string text)
		{
			ChatMessage msg = new ChatMessage();

			msg.channel = "team";
			msg.channelType = "team";
			msg.name = Localizer.GetString("ID_CHAT_SYSTEM_NAME");
			msg.vipLevel = -1;
			msg.icon = "Header_Xitong";
			msg.uid = 0;
			msg.quality = 0;
			msg.text = text;
			msg.ts = EB.Time.Now;

			while (_msgs.Count >= _msgs.bufferSize)
			{
				_msgs.Dequeue();
			}
			_msgs.Enqueue(msg);

			if (OnMessages != null)
			{
				OnMessages(new ChatMessage[] { msg });
			}
		}

		public void HandleSystemMessage(string text)
		{
			ChatMessage msg = new ChatMessage();

			msg.channel = "system";
			msg.channelType = "system";
			msg.name = string.Empty;
			msg.vipLevel = -1;
			msg.icon = "Header_Xitong";
			msg.uid = 0;
			msg.quality = 0;
			msg.text = text;
			msg.ts = EB.Time.Now;

			while (_msgs.Count >= _msgs.bufferSize)
			{
				_msgs.Dequeue();
			}
			_msgs.Enqueue(msg);

			if (OnMessages != null)
			{
				OnMessages(new ChatMessage[] { msg });
			}
		}

		public void HandleMessage(ChatMessage[] msgs)
		{
			List<ChatMessage> dist = new List<ChatMessage>();

			for (int i = 0; i < msgs.Length; ++i)
			{
				var msg = msgs[i];
				if (msg != null && !_msgs.Contains(msg))
				{
					dist.Add(msg);

					while (_msgs.Count >= _msgs.bufferSize)
					{
						_msgs.Dequeue();
					}
					_msgs.Enqueue(msg);
				}
			}
			
			if (OnMessages != null)
			{
				OnMessages(dist.ToArray());
			}
		}

		void HandleMessage(string message, object payload)
		{
			EB.Debug.Log("Got chat async message: {0}, {1}", message, payload);

			List<ChatMessage> dist = new List<ChatMessage>();
			ArrayList msgs = payload as ArrayList;
			for (int i = msgs.Count - 1; i >= 0; --i)
			{
				
				var msg = ChatMessage.Parse(msgs[i]);
				if (msg != null && !_msgs.Contains(msg))
				{
					dist.Add(msg);

					while (_msgs.Count >= _msgs.bufferSize)
					{
						_msgs.Dequeue();
					}
					_msgs.Enqueue(msg);
				}
			}

			if (OnMessages != null && dist.Count > 0)
			{
				OnMessages(dist.ToArray());
			}
		}

		void SetupSocket()
		{
			if (_socket != null)
			{
				_socket.Dispose();
			}

			_socket = new Net.TalkWebSocket();
			_socket.OnConnect += OnConnect;
			_socket.OnError += OnError;
			_socket.OnAsync += OnAsync;
		}

		void OnConnect()
		{
			_deffered.Defer((System.Action)delegate ()
			{
				//_msgs.Reset();

				if (OnConnected != null)
				{
					OnConnected();
				}
			});
			//Debug.Log("Connected to push server");
		}

		void OnAsync(string message, object payload)
		{
			_deffered.Defer((System.Action)delegate ()
			{
				HandleMessage(message, payload);
			});
		}

		void OnError(string error)
		{
			_deffered.Defer((System.Action)delegate ()
			{
				EB.Debug.Log("Lost connect to chat server: " + error);

				_joinedChannels.Clear();

				if (OnDisconnected != null)
				{
					OnDisconnected();
				}

				Coroutines.SetTimeout(delegate ()
				{
					ConnectWebsocket();
				}, 1000);
			});
		}

		void ConnectWebsocket()
		{
			if (State != SubSystemState.Connected)
			{
				return;
			}

			if (_socket == null)
			{
				SetupSocket();
			}

			if (_socket.State < Net.WebSocketState.Connecting)
			{
				_socket.ConnectAsync(_url, Protocol, null);
			}
		}

		void OnGetPushToken(string error, Hashtable result)
		{
			var websocket = Dot.String("url", result, string.Empty);
			if (!string.IsNullOrEmpty(websocket))
			{
				_url = new EB.Uri(websocket);
				State = SubSystemState.Connected;
				ConnectWebsocket();
			}
			else
			{
				State = SubSystemState.Error;
			}
		}

		IEnumerator StartRecognize(short[] wavBuffer, int wavBufferLength, System.Action<string> callback)
		{
			while (AudioWraper.IsRecognizing())
			{
				yield return null;
			}

			while (_recognizeCallback != null)
			{
				yield return null;
			}

			_recognizeCallback += callback;
			if (!AudioWraper.StartRecognize(AudioWraper.ConvertShortToByteArray(wavBuffer, wavBufferLength)))
			{
				OnRecognizeResult(string.Empty);
			}
		}

		public void OnRecognizeError(string msg)
		{
			EB.Debug.LogError("OnRecognizeError: {0}", msg);
			if (_recognizeCallback != null)
			{
				_recognizeCallback(string.Empty);
				_recognizeCallback = null;
			}
		}

		public void OnRecognizeResult(string msg)
		{
			EB.Debug.Log("OnRecognizeResult: {0}", msg);
			if (_recognizeCallback != null)
			{
				_recognizeCallback(msg);
				_recognizeCallback = null;
			}
		}
	}
}

public static class AudioWraper
{
	public enum Mode
	{
		MR475,      /* 4.75 kbps */
		MR515,      /* 5.15 kbps */
		MR59,       /* 5.90 kbps */
		MR67,       /* 6.70 kbps */
		MR74,       /* 7.40 kbps */
		MR795,      /* 7.95 kbps */
		MR102,      /* 10.2 kbps */
		MR122,      /* 12.2 kbps */
		MRDTX,      /* DTX       */
		N_MODES     /* Not Used  */
	}

	public const int PCM_FRAME_SIZE = 160; // 8khz 8000 * 0.02 = 160
	public const int MAX_AMR_FRAME_SIZE = 32;
	public const int AMR_FRAME_COUNT_PER_SECOND = 50;
	public const int FREQUENCY = 8000;
	public const int CHANNELS = 1;
	public const int BITSPERSAMPLE = 16;
	public const int BYTESPERSAMPLE = BITSPERSAMPLE / 8;
	public const float MAXLENGTH = 10;
	public const float MINLENGTH = 0.5f;
	/* From WmfDecBytesPerFrame in dec_input_format_tab.cpp */
	public static readonly int[] SIZES = new int[] { 12, 13, 15, 17, 19, 20, 26, 31, 5, 6, 5, 5, 0, 0, 0, 0 };

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
	[DllImport("libopencore-amrnb-0")]
	public static extern System.IntPtr Decoder_Interface_init();

	[DllImport("libopencore-amrnb-0")]
	public static extern void Decoder_Interface_exit(System.IntPtr state);

	[DllImport("libopencore-amrnb-0")]
	public static extern void Decoder_Interface_Decode(System.IntPtr state, byte[] inBuffer, short[] outBuffer, int bfi);

	[DllImport("libopencore-amrnb-0")]
	public static extern System.IntPtr Encoder_Interface_init(int dtx);

	[DllImport("libopencore-amrnb-0")]
	public static extern System.IntPtr Encoder_Interface_exit(System.IntPtr s);

	[DllImport("libopencore-amrnb-0")]
	public static extern int Encoder_Interface_Encode(System.IntPtr s, Mode mode, short[] inBuffer, byte[] outBuffer, int forceSpeech);
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
	[DllImport("libopencore-amrnb.0")]
	public static extern System.IntPtr Decoder_Interface_init();

	[DllImport("libopencore-amrnb.0")]
	public static extern void Decoder_Interface_exit(System.IntPtr state);

	[DllImport("libopencore-amrnb.0")]
	public static extern void Decoder_Interface_Decode(System.IntPtr state, byte[] inBuffer, short[] outBuffer, int bfi);

	[DllImport("libopencore-amrnb.0")]
	public static extern System.IntPtr Encoder_Interface_init(int dtx);

	[DllImport("libopencore-amrnb.0")]
	public static extern System.IntPtr Encoder_Interface_exit(System.IntPtr s);

	[DllImport("libopencore-amrnb.0")]
	public static extern int Encoder_Interface_Encode(System.IntPtr s, Mode mode, short[] inBuffer, byte[] outBuffer, int forceSpeech);
#elif !UNITY_EDITOR && UNITY_IPHONE
	[DllImport("__Internal")]
	public static extern System.IntPtr Decoder_Interface_init();

	[DllImport("__Internal")]
	public static extern void Decoder_Interface_exit(System.IntPtr state);

	[DllImport("__Internal")]
	public static extern void Decoder_Interface_Decode(System.IntPtr state, byte[] inBuffer, short[] outBuffer, int bfi);

	[DllImport("__Internal")]
	public static extern System.IntPtr Encoder_Interface_init(int dtx);

	[DllImport("__Internal")]
	public static extern System.IntPtr Encoder_Interface_exit(System.IntPtr s);

	[DllImport("__Internal")]
	public static extern int Encoder_Interface_Encode(System.IntPtr s, Mode mode, short[] inBuffer, byte[] outBuffer, int forceSpeech);
#elif !UNITY_EDITOR && UNITY_ANDROID
	[DllImport("opencore-amrnb")]
	public static extern System.IntPtr Decoder_Interface_init();

	[DllImport("opencore-amrnb")]
	public static extern void Decoder_Interface_exit(System.IntPtr state);

	[DllImport("opencore-amrnb")]
	public static extern void Decoder_Interface_Decode(System.IntPtr state, byte[] inBuffer, short[] outBuffer, int bfi);

	[DllImport("opencore-amrnb")]
	public static extern System.IntPtr Encoder_Interface_init(int dtx);

	[DllImport("opencore-amrnb")]
	public static extern System.IntPtr Encoder_Interface_exit(System.IntPtr s);

	[DllImport("opencore-amrnb")]
	public static extern int Encoder_Interface_Encode(System.IntPtr s, Mode mode, short[] inBuffer, byte[] outBuffer, int forceSpeech);
#endif

#if USE_IFLYSDK && UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void _InitRecognizer();
	[DllImport("__Internal")]
	private static extern int _StartRecognize(byte[] pcmStream, long length);
	[DllImport("__Internal")]
	private static extern void _CancelRecognize();
	[DllImport("__Internal")]
	private static extern int _IsRecognizing();
#endif

	public static byte[] ConvertFloatToByteArray(float[] floats)
	{
		byte[] ret = new byte[floats.Length * 4];// a single float is 4 bytes/32 bits

		System.Buffer.BlockCopy(floats, 0, ret, 0, ret.Length);
		return ret;
	}

	public static float[] ConvertByteArrayToFloat(byte[] bytes)
	{
		float[] ret = new float[bytes.Length / 4];

		System.Buffer.BlockCopy(bytes, 0, ret, 0, ret.Length * 4);
		return ret;
	}

	public static float[] FetchFirstChannel(float[] samples, int channels)
	{
		float[] firstSamples = new float[samples.Length / channels];
		for (int i = 0; i < firstSamples.Length; ++i)
		{
			firstSamples[i] = samples[channels * i];
		}
		return firstSamples;
	}

	public static float[] FetchFirstChannel(float[] samplesBuffer, ref int samplesBufferLength, int channels)
	{
		samplesBufferLength = samplesBufferLength / channels;
		for (int i = 0; i < samplesBufferLength; ++i)
		{
			samplesBuffer[i] = samplesBuffer[channels * i];
		}
		return samplesBuffer;
	}

	public static short[] ConvertFloatToShort(float[] samples)
	{
		short[] inBuffer = new short[samples.Length];
		int rescaleFactor = 32767; // to convert float to Int16
		for (int i = 0; i < samples.Length; i++)
		{
			short value = (short)(samples[i] * rescaleFactor);
			inBuffer[i] = value;
		}

		return inBuffer;
	}

	public static short[] ConvertFloatToShort(float[] samplesBuffer, int samplesBufferLength, short[] wavBuffer)
	{
		short[] inBuffer = wavBuffer;
		int rescaleFactor = 32767; // to convert float to Int16
		for (int i = 0; i < samplesBufferLength; i++)
		{
			short value = (short)(samplesBuffer[i] * rescaleFactor);
			inBuffer[i] = value;
		}

		return inBuffer;
	}

	public static float[] ConvertShortToFloat(short[] inBuffer)
	{
		float[] samples = new float[inBuffer.Length];
		float rescaleFactor = 32767.0f;
		for (int i = 0; i < inBuffer.Length; i++)
		{
			samples[i] = inBuffer[i] / rescaleFactor;
		}
		return samples;
	}

	public static float[] ConvertShortToFloat(short[] wavBuffer, int wavBufferLength, float[] samplesBuffer, int samplesOffset)
	{
		float rescaleFactor = 32767.0f;
		for (int i = 0; i < wavBufferLength; i++)
		{
			samplesBuffer[samplesOffset + i] = wavBuffer[i] / rescaleFactor;
		}
		return samplesBuffer;
	}

	public static byte[] ConvertShortToByteArray(short[] shortBuffer, int shortBufferLength)
	{
		byte[] bytes = new byte[shortBufferLength * 2];// a single short is 2 bytes/16 bits

		for (int i = 0; i < shortBufferLength; ++i)
		{
			short value = shortBuffer[i];
			System.BitConverter.GetBytes(value).CopyTo(bytes, i * 2);
		}

		return bytes;
	}

	public static short[] ConvertByteArrayToShort(byte[] byteBuffer, int byteBufferLength)
	{
		short[] shorts = new short[byteBufferLength / 2];

		for (int i = 0, len = shorts.Length; i < len; ++i)
		{
			int offset = i * 2;
			ushort low = byteBuffer[offset];
			ushort high = (ushort)(byteBuffer[offset + 1] << 8);
			short value = (short)(low | high);
			shorts[i] = value;
		}

		return shorts;
	}

	public static byte[] EncodeToWAVFile(this AudioClip clip)
	{
		byte[] bytes = null;

		using (var memoryStream = new MemoryStream())
		{
			memoryStream.Write(new byte[44], 0, 44);// reserve 44 bytes header

			byte[] bytesData = clip.EncodeToWAV();

			memoryStream.Write(bytesData, 0, bytesData.Length);

			memoryStream.Seek(0, SeekOrigin.Begin);

			byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
			memoryStream.Write(riff, 0, 4);

			byte[] chunkSize = System.BitConverter.GetBytes(memoryStream.Length - 8);
			memoryStream.Write(chunkSize, 0, 4);

			byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
			memoryStream.Write(wave, 0, 4);

			byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
			memoryStream.Write(fmt, 0, 4);

			byte[] subChunk1 = System.BitConverter.GetBytes(16);
			memoryStream.Write(subChunk1, 0, 4);

			//UInt16 two = 2;
			System.UInt16 one = 1;

			byte[] audioFormat = System.BitConverter.GetBytes(one);
			memoryStream.Write(audioFormat, 0, 2);

			byte[] numChannels = System.BitConverter.GetBytes(clip.channels);
			memoryStream.Write(numChannels, 0, 2);

			byte[] sampleRate = System.BitConverter.GetBytes(clip.frequency);
			memoryStream.Write(sampleRate, 0, 4);

			byte[] byteRate = System.BitConverter.GetBytes(clip.frequency * clip.channels * 2); // sampleRate * bytesPerSample * number of channels
			memoryStream.Write(byteRate, 0, 4);

			System.UInt16 blockAlign = (ushort)(clip.channels * 2);
			memoryStream.Write(System.BitConverter.GetBytes(blockAlign), 0, 2);

			System.UInt16 bps = 16;
			byte[] bitsPerSample = System.BitConverter.GetBytes(bps);
			memoryStream.Write(bitsPerSample, 0, 2);

			byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
			memoryStream.Write(datastring, 0, 4);

			byte[] subChunk2 = System.BitConverter.GetBytes(clip.samples * clip.channels * 2);
			memoryStream.Write(subChunk2, 0, 4);

			bytes = memoryStream.ToArray();
		}

		return bytes;
	}

	public static byte[] EncodeToWAV(this AudioClip clip)
	{
		var data = new float[clip.samples * clip.channels];

		clip.GetData(data, 0);

		byte[] bytes = new byte[data.Length * 2];

		int rescaleFactor = 32767;

		for (int i = 0; i < data.Length; i++)
		{
			short value = (short)(data[i] * rescaleFactor);
			System.BitConverter.GetBytes(value).CopyTo(bytes, i * 2);
		}

		return bytes;
	}

	public static byte[] EncodeToAMRFile(this AudioClip clip, Mode mode, int dtx)
	{
		byte[] amrData = null;

		using (MemoryStream amrStream = new MemoryStream())
		{
			byte[] byteHead = System.Text.Encoding.UTF8.GetBytes("#!AMR\n");
			amrStream.Write(byteHead, 0, byteHead.Length);

			byte[] amrBuffer = clip.EncodeToAMR(mode, dtx);
			amrStream.Write(amrBuffer, 0, amrBuffer.Length);

			amrData = amrStream.ToArray();
		}

		return amrData;
	}

	public static byte[] EncodeToAMR(this AudioClip clip, Mode mode, int dtx)
	{
		byte[] amrData = null;

		using (MemoryStream amrStream = new MemoryStream())
		{
			float[] samples = new float[clip.samples * clip.channels];
			clip.GetData(samples, 0);

			if (clip.channels != CHANNELS)
			{
				EB.Debug.LogWarning("Warning, only compressing one audio channel");

				samples = FetchFirstChannel(samples, clip.channels);
			}
			if (clip.frequency != FREQUENCY)
			{
				EB.Debug.LogWarning("Warning, AMR-NB uses 8000 Hz sample rate (WAV file has {0} Hz)", clip.frequency);
			}
			short[] wavData = ConvertFloatToShort(samples);

			System.IntPtr amr = Encoder_Interface_init(dtx);
			short[] wavFrameBuffer = new short[PCM_FRAME_SIZE];
			byte[] amrBuffer = new byte[500];

			int wavDataIndex = 0;
			while (wavDataIndex < wavData.Length)
			{
				if (wavDataIndex + PCM_FRAME_SIZE > wavData.Length)
				{
					break;
				}

				System.Array.Copy(wavData, wavDataIndex, wavFrameBuffer, 0, PCM_FRAME_SIZE);
				int length = Encoder_Interface_Encode(amr, mode, wavFrameBuffer, amrBuffer, 0);
				EB.Debug.Log("DecodeAudioFrame: mode = {0}, length = {1}", amrBuffer[0], length);
				amrStream.Write(amrBuffer, 0, length);
				wavDataIndex += PCM_FRAME_SIZE;
			}
			Encoder_Interface_exit(amr);
			amrData = amrStream.ToArray();
		}

		return amrData;
	}

	public static void DecodeFromAmrFile(this AudioClip clip, byte[] amrStream)
	{
		List<short> wavBuffer = new List<short>(10240);
		short[] wavFrameBuffer = new short[PCM_FRAME_SIZE];
		System.IntPtr amr = Decoder_Interface_init();
		byte[] amrBuffer = new byte[500];
		int amrDataIndex = 6; // #!AMR\n
		while (amrDataIndex < amrStream.Length)
		{
			int amrBufferLength = 0;

			/* Read the mode byte */
			amrBuffer[amrBufferLength++] = amrStream[amrDataIndex++];
			/* Find the packet size */
			int size = SIZES[(amrBuffer[0] >> 3) & 0x0f];
			EB.Debug.Log("DecodeAudioFrame: mode = {0}, size = {1}", amrBuffer[0], size);
			System.Array.Copy(amrStream, amrDataIndex, amrBuffer, amrBufferLength, size);
			/* Decode the packet */
			Decoder_Interface_Decode(amr, amrBuffer, wavFrameBuffer, 0);

			/* Append to Buffer */
			wavBuffer.AddRange(wavFrameBuffer);
			amrDataIndex += amrBufferLength;
		}
		Decoder_Interface_exit(amr);

		float[] audioData = ConvertShortToFloat(wavBuffer.ToArray());
		clip.SetData(audioData, 0);
	}

	public static void DecodeFromAmr(this AudioClip clip, byte[] amrData)
	{
		List<short> wavBuffer = new List<short>(10240);
		short[] wavFrameBuffer = new short[PCM_FRAME_SIZE];
		System.IntPtr amr = Decoder_Interface_init();
		byte[] amrBuffer = new byte[500];
		int amrDataIndex = 0;
		while (amrDataIndex < amrData.Length)
		{
			int amrBufferLength = 0;

			/* Read the mode byte */
			amrBuffer[amrBufferLength++] = amrData[amrDataIndex++];
			/* Find the packet size */
			int size = SIZES[(amrBuffer[0] >> 3) & 0x0f];
			EB.Debug.Log("DecodeAudioFrame: mode = {0}, size = {1}", amrBuffer[0], size);
			System.Array.Copy(amrData, amrDataIndex, amrBuffer, amrBufferLength, size);
			/* Decode the packet */
			Decoder_Interface_Decode(amr, amrBuffer, wavFrameBuffer, 0);

			/* Append to Buffer */
			wavBuffer.AddRange(wavFrameBuffer);
			amrDataIndex += amrBufferLength;
		}
		Decoder_Interface_exit(amr);

		float[] audioData = ConvertShortToFloat(wavBuffer.ToArray());
		clip.SetData(audioData, 0);
	}

	public static void DecodeFromWavFile(this AudioClip clip, byte[] wavStream)
	{
		using (MemoryStream ms = new MemoryStream(wavStream))
		{
			ms.Seek(44, SeekOrigin.Begin);
			// read header

			int audioBufferLength = (wavStream.Length - 44) / clip.channels;
			short[] audioBuffer = new short[audioBufferLength];
			for (int i = 0; i < audioBufferLength; ++i)
			{
				int offset = BYTESPERSAMPLE * clip.channels * i + 44;
				ushort low = wavStream[offset];
				ushort high = (ushort)(wavStream[offset + 1] << 8);
				audioBuffer[i] = (short)(low | high);
			}
			float[] audioData = ConvertShortToFloat(audioBuffer);
			clip.SetData(audioData, 0);
		}
	}

	public static void DecodeFromWav(this AudioClip clip, byte[] wavData)
	{
		int audioBufferLength = wavData.Length / clip.channels;
		short[] audioBuffer = new short[audioBufferLength];
		for (int i = 0; i < audioBufferLength; ++i)
		{
			int offset = BYTESPERSAMPLE * clip.channels * i;
			ushort low = wavData[offset];
			ushort high = (ushort)(wavData[offset + 1] << 8);
			audioBuffer[i] = (short)(low | high);
		}
		float[] audioData = ConvertShortToFloat(audioBuffer);
		clip.SetData(audioData, 0);
	}

	public static void InitRecognizer()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
#if USE_IFLYSDK
			_InitRecognizer();
#endif
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
#if USE_IFLYSDK
			using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.ifly.IATManager"))
			{
				jc.CallStatic("initRecognizer");

			}
#endif
		}
	}

	public static bool StartRecognize(byte[] pcmStream)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
#if USE_IFLYSDK
			return _StartRecognize(pcmStream, pcmStream.Length) == 0;
#endif
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
#if USE_IFLYSDK
			using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.ifly.IATManager"))
			{
				return jc.CallStatic<bool>("startRecognize", pcmStream, pcmStream.Length);
			}
#endif
		}

		return false;
	}

	public static void CancelRecognize()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
#if USE_IFLYSDK
			_CancelRecognize();
#endif
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
#if USE_IFLYSDK
			using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.ifly.IATManager"))
			{
				jc.CallStatic("cancelRecognize");
			}
#endif
		}
	}

	public static bool IsRecognizing()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
#if USE_IFLYSDK
			return _IsRecognizing() == 0;
#endif
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
#if USE_IFLYSDK
			using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.ifly.IATManager"))
			{
				return jc.CallStatic<bool>("isListening");
			}
#endif
		}

		return false;
	}
}

public class AudioRecognizeReceiver : MonoBehaviour
{
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void OnError(string msg)
	{
		EB.Debug.LogError("OnError: {0}", msg);
		SparxHub.Instance.ChatManager.OnRecognizeError(msg);
	}

	public void OnResult(string msg)
	{
		EB.Debug.Log("OnResult: {0}", msg);
		SparxHub.Instance.ChatManager.OnRecognizeResult(msg);
	}
}

