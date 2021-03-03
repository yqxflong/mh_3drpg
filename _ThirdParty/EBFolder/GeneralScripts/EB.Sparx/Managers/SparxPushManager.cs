using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class PushManagerConfig
	{
		public string Protocol = "io.sparx.push";
	}

	public class PushManager : SubSystem, Updatable
	{
		public System.Action OnDisconnected;
		public System.Action OnConnected;
		public System.Action<string, string, object> OnHandleMessage;
		public System.Action OnScheduleLocalNotification;

		PushAPI _api;
		Net.WebSocket _socket;
		Deferred _deffered;
		EB.Uri _url;
		bool _gotToken;

		#region implemented abstract members of EB.Sparx.SubSystem
		public override void Initialize(Config config)
		{
			_api = new PushAPI(Hub.ApiEndPoint);
			_deffered = new Deferred(4);
			_url = null;

#if UNITY_IPHONE
			UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
			UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();

			EB.Debug.Log("PushManager: registering for notification services");
			UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
#elif UNITY_ANDROID
#endif
		}

		public bool UpdateOffline { get { return false; } }

		public override void Connect()
		{
			State = SubSystemState.Connecting;

			var push = Dot.Object("push", Hub.DataStore.LoginDataStore.LoginData, null);
			if (push != null)
			{
				OnGetPushToken(null, push);
			}
			else
			{
				_api.GetPushToken(OnGetPushToken);
			}

#if UNITY_IPHONE
			// get the apple push token 
			_gotToken = false;
#elif UNITY_ANDROID
			_gotToken = false;
#endif
		}

		public void Update()
		{
			_deffered.Dispatch();

#if UNITY_IPHONE
			if (!_gotToken)
			{
				var token = UnityEngine.iOS.NotificationServices.deviceToken;
				if (token != null)
				{
					_gotToken = true;
					var tokenHex = Encoding.ToHexString(token);
					EB.Debug.Log("Got push token " + tokenHex);
					_api.SetApplePushToken(tokenHex, OnSentApplePushToken);
				}
				else if (!string.IsNullOrEmpty(UnityEngine.iOS.NotificationServices.registrationError))
				{
					_gotToken = true;
					EB.Debug.LogWarning("failed to register for push token : " + UnityEngine.iOS.NotificationServices.registrationError);
				}
			}
#elif UNITY_ANDROID
			if (!_gotToken)
			{
			//	string androidID = OtherLevelsSDK.androidToken;
			//	if(!string.IsNullOrEmpty(androidID))
			//	{
			//		if (OtherLevelsPlugin.Instance != null)
			//		{
			//			OtherLevelsPlugin.Instance.deviceToken = androidID;
			//		}
			//		_gotToken = true;
			//	}
			}
#endif
		}

		void OnSentApplePushToken(string error)
		{
			if (!string.IsNullOrEmpty(error))
			{
				EB.Debug.Log("failed to send push token  " + error);
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
		}

		public override void Dispose()
		{
			if (_socket != null)
			{
				_socket.Dispose();
			}
		}

		public override void OnEnteredBackground()
		{			
			if (_socket != null)
			{
				_socket.Dispose();
				_socket = null;
			}

			_deffered.Dispatch();

			if (OnScheduleLocalNotification != null)
			{
				OnScheduleLocalNotification();
			}
		}

		public override void OnEnteredForeground()
		{
			if (_socket != null)
			{
				_socket.Dispose();
				_socket = null;
			}

			_deffered.Dispose();

			ConnectWebsocket();

			CleanNotification();
		}
		#endregion

		void SetupSocket()
		{
			if (_socket != null)
			{
				_socket.Dispose();
			}

			_socket = new Net.WebSocket();
			_socket.OnConnect += OnConnect;
			_socket.OnError += OnError;
			_socket.OnMessage += OnMessage;
		}

		void OnConnect()
		{
			_deffered.Defer((System.Action)delegate ()
			{
				if (OnConnected != null)
				{
					OnConnected();
				}
			});
			//Debug.Log("Connected to push server");
		}

		public bool IsConnected()
		{
			return _socket != null && _socket.State == Net.WebSocketState.Open;
		}

		public void SimpleRPC(string type, Hashtable args)
		{
			ArrayList data = new ArrayList();

			data.Add(type);

			if (null != args)
			{
				data.Add(args);
			}

			string str = JSON.Stringify(data);

			// EB.Debug.Log("->SimpleN " + str);
			if (_socket != null && _socket.State == EB.Net.WebSocketState.Connecting)
			{
				_socket.SendUTF8(str);
			}
		}

		public void HandleResult(object result)
		{
			if(result is ArrayList){
				var arr = result as ArrayList;
				for (int i = 0; i < arr.Count; i++)
				{
					HandleMessage(arr[i]);
				}
			}
			else
			{
				HandleMessage(result);
			}
		}

		#region CPU耗时6ms
		public void HandleMessage(object data)
		{
			if (data == null)
			{
				EB.Debug.LogError("HandleMessage: data is null");
				return;
			}

			var component = Dot.String("component", data, string.Empty);
			var message = Dot.String("message", data, string.Empty);
			var payload = Dot.Find<object>("payload", data);

			if (OnHandleMessage != null)
			{
				OnHandleMessage(component, message, payload);
			}

			var manager = Hub.GetManager(component);
			if (manager != null)
			{
				manager.Async(message, payload);
			}
			else
			{
				EB.Debug.Log("Failed to find manager: " + component+ ".So send message to LTHotfixManagerLogic");
                manager = Hub.GetManager("LTHotfixManagerLogic");
                manager.Async(string.Format ("{0}|{1}", component, message), payload);
            }
		}
		#endregion

		public void HandleMessage(string data){
			EB.Debug.Log("Got async message: " + data);
#if DEBUG
			int length = Encoding.GetBytes(data).Length;
			if (length > 1024)
			{
				EB.Debug.LogWarning("HandleMessage: message size {0} great than 1k", length);
			}
#endif
			var obj = JSON.Parse(data);
			HandleMessage(obj);
		}

		void OnMessage(string data)
		{
			_deffered.Defer((System.Action)delegate ()
			{
				HandleMessage(data);
			});
		}

		void OnError(string error)
		{
			_deffered.Defer((System.Action)delegate ()
			{
				EB.Debug.Log("Lost connect to push server: " + error);

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
				_socket.ConnectAsync(_url, Hub.Instance.Config.PushManagerConfig.Protocol, null);
			}
		}

		void OnGetPushToken(string error, Hashtable result)
		{
			var websocket = Dot.String("websocket", result, string.Empty);
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

		public void GlobalApiResultSyncHandler(EB.Sparx.Response response)
		{
			if (response.async != null)
			{
				HandleResult(response.async);
				response.async = null;
			}
		}

		public void EndPointPostAsyncHandler(EB.Sparx.Response response)
		{
			if (response.async != null)
			{
				HandleResult(response.async);
				response.async = null;
			}
		}

		public void ScheduleLocalNotification(string message, int hour, int minute, bool repeatPerDay)
		{
			var now = System.DateTime.Now;
			System.DateTime date = new System.DateTime(now.Year, now.Month, now.Day, hour, minute, 0);

			if (repeatPerDay)
			{
				ScheduleDailyLocalNotification(EB.Localizer.GetString("ID_SPARX_NOTIFICATION_TITLE"), message, date);
			}
			else
			{
				ScheduleOnceLocalNotification(EB.Localizer.GetString("ID_SPARX_NOTIFICATION_TITLE"), message, date);
			}
		}

		public void ScheduleDailyLocalNotification(string title, string message, System.DateTime date)
		{
			for (int i = 0; i < 7; ++i)
			{
				ScheduleLocalNotification(title, message, date + System.TimeSpan.FromDays(i), false);
			}

			ScheduleLocalNotification(title, message, date + System.TimeSpan.FromDays(7), true);
		}

		public void ScheduleOnceLocalNotification(string title, string message, System.DateTime date)
		{
			ScheduleLocalNotification(title, message, date, false);
		}

		private void ScheduleLocalNotification(string title, string message, System.DateTime date, bool repeatPerDay)
		{
			if (date < System.DateTime.Now)
			{
				EB.Debug.LogWarning("ScheduleLocalNotification: too late to schedule notification");
				return;
			}

			//EB.Debug.Log("NotificationMessage: start NotificationMessage message = {0} date = {1} repeat = {2}", message, date, repeatPerDay);

#if UNITY_EDITOR
			//EB.Debug.Log("ScheduleLocalNotification: not support in editor");
#elif UNITY_IPHONE
			UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
			localNotification.fireDate = date;
			localNotification.alertBody = message;
			localNotification.applicationIconBadgeNumber = 1;
			localNotification.hasAction = false;
			if (repeatPerDay)
			{
				localNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.ChineseCalendar;
				localNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
			}
			localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
#elif UNITY_ANDROID
			using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.android.NotificationManager"))
			{
				var utcDate = date.ToUniversalTime();
				System.DateTime Jan1St1970 = new System.DateTime (1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
				long millis = (long)((utcDate - Jan1St1970).TotalMilliseconds);
				jc.CallStatic("scheduleLocalNotification", title, message, millis, repeatPerDay ? 1 : 0);
			}
#endif
		}

		public void CleanNotification()
		{
#if UNITY_EDITOR
			EB.Debug.Log("CleanNotification: not support in editor");
#elif UNITY_IPHONE
			UnityEngine.iOS.LocalNotification badge = new UnityEngine.iOS.LocalNotification();
			badge.applicationIconBadgeNumber = -1;
			UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow(badge);

			UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
			UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
			UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
#elif UNITY_ANDROID
			using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.android.NotificationManager"))
			{
				jc.CallStatic("clearLocalNotifications");
			}
#endif
		}
	}
}

