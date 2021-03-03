using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class Config
	{
		public string               ApiEndpoint         = string.Empty;
		public Key                  ApiKey              = null;
		public int                  ApiKeepAlive        = 45;
		public Language             Locale              = Language.English;
		public PushManagerConfig    PushManagerConfig   = new PushManagerConfig();
		public LoginConfig          LoginConfig         = new LoginConfig();
		public GameManagerConfig    GameManagerConfig   = new GameManagerConfig();
		public WalletConfig         WalletConfig        = new WalletConfig();
		public GameCenterConfig     GameCenterConfig    = new GameCenterConfig();
		public GachaConfig          GachaConfig         = new GachaConfig();
		public PerformanceConfig    PerformanceConfig   = new PerformanceConfig();
		public System.Type          HubType             = typeof(SparxHub);
		public List<System.Type>    GameComponents      = new List<System.Type>();
		public string               DataCacheDirectory  = "Caches";
	}

    public class DataStore
    {
        public LoginDataStore LoginDataStore = new LoginDataStore();
    }

	public enum HubState
	{
		Idle = 0,
		LogginIn,
		Connecting,
		Connected,
	}

	public class Hub : MonoBehaviour
	{
		public static Hub Instance { get; private set; }

		public Config Config { get; private set; }
        public DataStore DataStore { get; private set; }
        public HubState State { get; private set; }

		public EndPoint ApiEndPoint { get; private set; }
		public UserManager UserManager { get; private set; }
		public GameManager GameManager { get; private set; }
		public DataManager DataManager { get; private set; }
		public PushManager PushManager { get; private set; }
		public FetchPushMsgManager FetchPushMsgManager { get; private set; }
		public WalletManager WalletManager { get; private set; }
		public GameCenterManager GameCenterManager { get; private set; }
		public TelemetryManager TelemetryManager { get; private set; }
		public PerformanceManager PerformanceManager { get; private set; }
		public LevelRewardsManager LevelRewardsManager { get; private set; }
		public DataCacheManager DataCacheManager { get; private set; }
		public ChatManager ChatManager { get; private set; }

#if USE_ASSDK
		public AsSDKManager AsSDKManager { get; private set; }
#endif

#if USE_XYSDK
		public XYSDKManager XYSDKManager { get; private set; }
#endif

#if USE_KUAIYONGSDK
		public KuaiYongSDKManager KuaiYongSDKManager { get; private set; }
#endif

#if USE_UCSDK
		public UCSDKManager UCSDKManager { get; private set; }
#endif

#if USE_GOOGLE
	    public GoogleSDKManager GoogleSDKManager { get; private set; }
#endif

#if USE_QIHOOSDK
		public QiHooSDKManager QiHooSDKManager { get; private set; }
#endif

#if USE_XIAOMISDK
		public XiaoMiSDKManager XiaoMiSDKManager { get; private set; }
#endif

#if USE_OPPOSDK
		public OPPOSDKManager OPPOSDKManager { get; private set; }
#endif

#if USE_VIVOSDK
		public VivoSDKManager VivoSDKManager { get; private set; }
#endif

#if USE_TENCENTSDK
		public TencentSDKManager TencentSDKManager { get; private set; }
#endif

#if USE_WINNERSDK
		public WinnerSDKManager WinnerSDKManager { get; private set; }
#endif

#if USE_HUAWEISDK
		public HuaweiSDKManager HuaweiSDKManager { get; private set; }
#endif

#if USE_WINNERIOSSDK
		public WinnerIOSSDKManager WinnerIOSSDKManager { get; private set; }
#endif

#if USE_YIJIESDK
		public YiJieSDKManager YiJieSDKManager { get; private set; }
#endif

#if USE_EWANSDK
		public EWanSDKManager EWanSDKManager { get; private set; }
#endif

#if USE_LBSDK
		public LBSDKManager LBSDKManager { get; private set; }
#endif

#if USE_K7KSDK
		public K7KSDKManager K7KSDKManager { get; private set; }
#endif

#if USE_QINGYUANSDK
		public QingYuanSDKManager QingYuanSDKManager { get; private set; }
#endif

#if USE_AIBEISDK
        public AibeiSDKManager AibeiSDKManager { get; private set; }
#endif

#if USE_ASDK
        public ASDKManager ASDKManager { get; private set; }
#endif

#if USE_M4399SDK
        public M4399SDKManager M4399SDKManager { get; private set; }
#endif

 // #if USE_WECHATSDK
        public WeChatSDKManager WeChatSDKManager { get; private set; }
 // #endif

// #if USE_ALIPAYSDK
        public AlipaySDKManager AlipaySDKManager { get; private set; }
// #endif

#if USE_VFPKSDK
        public VFPKSDKManager VFPKSDKManager { get; private set; }
#endif

// #if USE_XINKUAISDK
        // public XinkuaiSDKManager XinkuaiSDKManager { get; private set; }

        public BaseSDKManager mBaseSdkManager { get; private set; }

        public event System.Action OnUpdate;
        public System.Action<string> TipCallBack = null;
        private List<Manager> _managers = new List<Manager>();
		private List<SubSystem> _subsystems = new List<SubSystem>();
		private List<Updatable> _update = new List<Updatable>();

		private bool _wasLoggedIn;
		private bool _wasServiceActive;
		private int  _enteredBackground;
		public bool RunInBackground { get; set; }

        public static Hub Create(Config config, System.Action <string> tipCall)
		{
			if (Instance != null)
			{
				EB.Debug.LogWarning("Sparx is already initialized");
				return Instance;
			}

			GameObject hud = new GameObject("SparxHub", config.HubType);
            if (hud.GetComponent(config.HubType)==null)
            {
                hud.AddComponent<SparxHub>();
            }
            DontDestroyOnLoad(hud);
			Instance = hud.GetComponent(config.HubType) as SparxHub;

			Instance.Initialize(config);

            if (Instance.TipCallBack == null)
            {
                Instance.TipCallBack += tipCall;
            }

			return Instance;
		}

		void Initialize(Config config)
		{
			Config = config;
            DataStore = new DataStore();

            BugReport.Init(config.ApiEndpoint + "/bugs");
            
			Assets.LoadAsync("profanity", typeof(TextAsset), o =>
			{
				TextAsset profanity = o as TextAsset;
				if (profanity != null)
				{
					ProfanityFilter.Init(profanity.text);
					Assets.Unload("profanity");
				}
			});
            
			var options = new EndPointOptions{ Key = config.ApiKey.Value };
			if (config.ApiKeepAlive > 0)
			{
				options.KeepAlive = true;
				options.KeepAliveUrl = "/util/ping";
				options.KeepAliveInterval = config.ApiKeepAlive;
			}
			ApiEndPoint = EndPointFactory.Create(config.ApiEndpoint, options);

			EB.Memory.OnBreach += delegate ()
			{
				FatalError("ID_SPARX_ERROR_UNKNOWN");
			};

			InitializeComponents();
		}

		protected virtual void InitializeComponents()
		{
			TelemetryManager = AddManager<TelemetryManager>();
			UserManager = AddManager<UserManager>();
			DataManager = AddManager<DataManager>();
			WalletManager = AddManager<WalletManager>();
			PerformanceManager = AddManager<PerformanceManager>();
			LevelRewardsManager = AddManager<LevelRewardsManager>();
			DataCacheManager = AddManager<DataCacheManager>();
			PushManager = AddManager<PushManager>();
			FetchPushMsgManager = AddManager<FetchPushMsgManager>();
			ChatManager = AddManager<ChatManager>();
            
			if (Config.GameCenterConfig.Enabled)
			{
				GameCenterManager = AddManager<GameCenterManager>();
			}
			if (Config.GameManagerConfig.Enabled)
			{
				GameManager = AddManager<GameManager>();
			}
			
#if USE_ASSDK
			AsSDKManager = AddManager<AsSDKManager>();
#endif
#if USE_XYSDK
			XYSDKManager = AddManager<XYSDKManager>();
#endif
#if USE_KUAIYONGSDK
			KuaiYongSDKManager = AddManager<KuaiYongSDKManager>();
#endif
#if USE_UCSDK
			UCSDKManager = AddManager<UCSDKManager>();
#endif
#if USE_QIHOOSDK
			QiHooSDKManager = AddManager<QiHooSDKManager>();
#endif
#if USE_XIAOMISDK
			XiaoMiSDKManager = AddManager<XiaoMiSDKManager>();
#endif
#if USE_OPPOSDK
			OPPOSDKManager = AddManager<OPPOSDKManager>();
#endif
#if USE_VIVOSDK
			VivoSDKManager = AddManager<VivoSDKManager>();
#endif
#if USE_TENCENTSDK
			TencentSDKManager = AddManager<TencentSDKManager>();
#endif
#if USE_WINNERSDK
			WinnerSDKManager = AddManager<WinnerSDKManager>();
#endif
#if USE_HUAWEISDK
			HuaweiSDKManager = AddManager<HuaweiSDKManager>();
#endif
#if USE_WINNERIOSSDK
			WinnerIOSSDKManager =AddManager<WinnerIOSSDKManager>();
#endif
#if USE_YIJIESDK
			YiJieSDKManager = AddManager<YiJieSDKManager>();
#endif
#if USE_EWANSDK
			EWanSDKManager = AddManager<EWanSDKManager>();
#endif
#if USE_LBSDK
			LBSDKManager = AddManager<LBSDKManager>();
#endif
#if USE_K7KSDK
			K7KSDKManager = AddManager<K7KSDKManager>();
#endif
#if USE_QINGYUANSDK
			QingYuanSDKManager = AddManager<QingYuanSDKManager>();
#endif
#if USE_AIBEISDK
            AibeiSDKManager = AddManager<AibeiSDKManager>();
#endif
#if USE_ASDK
            ASDKManager = AddManager<ASDKManager>();
#endif
#if USE_GOOGLE
		    GoogleSDKManager = AddManager<GoogleSDKManager>();
#endif
#if USE_M4399SDK
            M4399SDKManager = AddManager<M4399SDKManager>();
#endif
// #if USE_WECHATSDK
            WeChatSDKManager = AddManager<WeChatSDKManager>();
// #endif
// #if USE_ALIPAYSDK
            AlipaySDKManager = AddManager<AlipaySDKManager>();
			// #endif
#if USE_VFPKSDK
             VFPKSDKManager = AddManager<VFPKSDKManager>();
#endif

#if USE_XINKUAISDK
			mBaseSdkManager= XinkuaiSDKManager.getInstance();
#endif

#if USE_AOSHITANGSDK
			mBaseSdkManager= AoshitangSDKManager.getInstance();
#endif
			if (mBaseSdkManager==null)
			{
				mBaseSdkManager =new DefaultEmptySDKManager();
			}
			
            for (int i = 0, cnt = Config.GameComponents.Count; i < cnt; ++i)
			{
				var type = Config.GameComponents[i];
				AddManager<Manager>(type);
			}
		}

		protected T AddManager<T>() where T : Manager, new()
		{
			return AddManager<T>(typeof(T));
		}

		protected T AddManager<T>(System.Type type) where T : Manager
		{
			T manager = (T)System.Activator.CreateInstance(type);
			manager.Initialize(Config);
			AddManager(manager);
			return manager;
		}

		public Manager GetManager(string name)
		{
			var ebname = "EB.Sparx."+name;
			for (int i = 0, cnt = _managers.Count; i < cnt; ++i)
			{
				var manager = _managers[i];
				var n = manager.GetType().Name;
				//Equals使用StringComparison.Ordinal接口比"=="比较快10倍
				if (n.Equals(name) || n.Equals(ebname))
				{
					return manager;
				}
			}
			return null;
		}

		public Manager GetManager(System.Type type)
		{
			for (int i = 0, cnt = _managers.Count; i < cnt; ++i)
			{
				Manager manager = _managers[i];
				if (manager.GetType() == type || manager.GetType().IsSubclassOf(type))
				{
					return manager;
				}
			}
			return null;
		}

		public T GetManager<T>() where T : Manager
		{
			return (T)GetManager(typeof(T));
		}

		public void AddManager(Manager manager)
		{
			_managers.Add(manager);
			if (manager is SubSystem)
			{
				_subsystems.Add((SubSystem)manager);
			}

			if (manager is Updatable)
			{
				_update.Add((Updatable)manager);
			}

		}

		void OnDestroy()
		{
			ApiEndPoint.Dispose();
			for (int i = 0, cnt = _managers.Count; i < cnt; ++i)
			{
				var manager = _managers[i];
				manager.Dispose();
			}
		}

		void OnLoginStateChanged(LoginState state)
		{
			switch (state)
			{
				case LoginState.LoggedIn:
					{
						State = HubState.LogginIn;
						SubSystemConnect();
					}
					break;
			}
		}

		public void FatalError(string error)
		{
			EB.Debug.LogWarning(EB.Debug.ACCIDENTAL + ",发生致命错误，踢回登入界面:" + error);

			var wasConnected = State == HubState.Connected;

			State = HubState.Idle;

			if (Config.LoginConfig.Listener != null)
			{
				if (wasConnected)
				{
                    
					Config.LoginConfig.Listener.OnDisconnected(error);
				}
				else
				{
					Config.LoginConfig.Listener.OnLoginFailed(error);
				}
			}
            
			SubSystemDisconnect(false);
		}

		public void Disconnect(bool isLogout)
		{
            EB.Debug.LogWarning(EB.Debug.ACCIDENTAL + ",网络断开，踢回登入界面:" + isLogout);

            State = HubState.Idle;

			if (Config.LoginConfig.Listener != null)
			{
				Config.LoginConfig.Listener.OnDisconnected(string.Empty);
			}
            
			SubSystemDisconnect(isLogout);
		}

		void SubSystemConnect()
		{
			EB.Debug.Log("SparxHub: SubSystemConnect");

			State = HubState.Connecting;
			for (int i = 0, cnt = _subsystems.Count; i < cnt; ++i)
			{
				SubSystem system = _subsystems[i];
				system.State = SubSystemState.Connecting;
				system.Connect();
			}
		}

		void SubSystemConnecting()
		{
			var allConnected = true;
			for (int i = 0, cnt = _subsystems.Count; i < cnt; ++i)
			{
				SubSystem system = _subsystems[i];
				switch (system.State)
				{
					case SubSystemState.Error:
						{
							FatalError(Localizer.GetString("ID_SPARX_ERROR_UNKNOWN"));
							return;
						}
					case SubSystemState.Connecting:
					case SubSystemState.Disconnected:
						{
							allConnected = false;
                            return;
                        }
					case SubSystemState.Connected:
						{
						}
						break;
				}

			}

			if (allConnected)
			{
				EB.Debug.Log("SparxHub: SubSystemConnected");
				State = HubState.Connected;
                
				ApiEndPoint.StartKeepAlive();
                
				if (Config.LoginConfig.Listener != null)
				{
					Config.LoginConfig.Listener.OnLoggedIn();
				}

				for (int i = 0, cnt = _managers.Count; i < cnt; ++i)
				{
					var manager = _managers[i];
					EB.Debug.Log("manager.OnLoggedIn()===>{0}", manager.GetType().FullName);
					manager.OnLoggedIn();
				}
			}
		}

		void SubSystemUpdate()
		{
			bool online = State != HubState.Idle;
			for (int i = 0, cnt = _update.Count; i < cnt; ++i)
			{
				Updatable system = _update[i];

				if (system.UpdateOffline || online)
				{
					system.Update();
				}
			}
		}

		void SubSystemDisconnect(bool isLogout)
		{
			EB.Debug.Log("SparxHub: SubSystemDisconnect");
            
			for (int i = _subsystems.Count - 1; i >= 0; --i)
			{
				SubSystem system = _subsystems[i];
				system.Disconnect(isLogout);
				system.State = SubSystemState.Disconnected;
			}

			ApiEndPoint.StopKeepAlive();
		}
        
        void Update()
        {
            SubSystemUpdate();

			switch (State)
			{
				case HubState.Connecting:
					{
						SubSystemConnecting();
					}
					break;
			}

			if (OnUpdate != null)
			{
				OnUpdate();
			}
		}

		public void OnPause(bool pauseStatus)
		{
			if (pauseStatus && _enteredBackground == 0 && !RunInBackground)
			{
				OnEnteredBackground();
			}
			else if (!pauseStatus && _enteredBackground > 0 && !RunInBackground)
			{
				OnEnteredForeground();
			}
		}

		public void OnFocus(bool focusStatus)
		{
			//EB.Debug.Log("SparxHub.OnFocus: status = {0}", focusStatus);
		}

		void OnEnteredBackground()
		{
			_wasLoggedIn = State == HubState.Connected;
			_wasServiceActive = (ApiEndPoint is HttpEndPoint) && (ApiEndPoint as HttpEndPoint).ServiceActive;
			EB.Debug.Log("OnEnteredBackground: _wasLoggedIn: {0}, _wasServiceActive: {1}", _wasLoggedIn, _wasServiceActive);

			if (_wasLoggedIn)
			{
				for (int i = _managers.Count - 1; i >= 0; --i)
				{
					Manager manager = _managers[i];
					manager.OnEnteredBackground();
				}

				ApiEndPoint.StopKeepAlive();
			}

			_enteredBackground = Time.Now;
		}

		void OnEnteredForeground()
		{
			var since = Time.Since(_enteredBackground);
			EB.Debug.Log("OnEnteredForeground: since: {0}", since);
			EB.Debug.Log("OnEnteredForeground: reachability: {0}", Application.internetReachability);
			EB.Debug.Log("OnEnteredForeground: _wasLoggedIn: {0}, _wasServiceActive: {1}", _wasLoggedIn, _wasServiceActive);

			if (_wasLoggedIn)
			{
				var ping = ApiEndPoint.Post("/util/ping");
				ping.suspendMethod = Request.eSuspendMethod.Break;
				ApiEndPoint.Service(ping, delegate (Response r)
				{
					if (r.sucessful)
					{
						EB.Debug.Log("OnEnteredForeground: ping is ok!");
						ApiEndPoint.StartKeepAlive();
					}
					else
					{
						EB.Debug.Log("OnEnteredForeground: sesstion timedout, re-login");
						Disconnect(false);
					}
				});

				for (int i = 0, cnt = _managers.Count; i < cnt; ++i)
				{
					var manager = _managers[i];
					manager.OnEnteredForeground();
				}
			}

			_enteredBackground = 0;
		}

		public void Logout()
		{
#if USE_XINKUAISDK || USE_AOSHITANGSDK
			SparxHub.Instance.mBaseSdkManager.Logout();
#endif
		}

	}
}

public class SparxHub : EB.Sparx.Hub
{
	
}
