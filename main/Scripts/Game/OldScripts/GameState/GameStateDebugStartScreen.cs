//#define DEBUG
//#define SIMPLE_DEBUG_SCREEN

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerConfiguration
{
	public string m_name;
	public string m_address;

	public ServerConfiguration(string name, string address)
	{
		m_name = name;
		m_address = address;
	}
}

[GameState(eGameState.DebugStartScreen)]
public class GameStateDebugStartScreen : GameState
#if DEBUG
, IDebuggable
{
    public override IEnumerator Start(GameState oldState)
    {
        EB.Debug.Log("DebugStartScreen Start");
        DebugSystem.RegisterSystem("DebugStartScreen", this);

        m_api_selected_server_id = PlayerPrefs.GetInt("m_api_selected_server_id");
        m_ota_selected_server_id = PlayerPrefs.GetInt("m_ota_selected_server_id");
        m_api_other_server_address = PlayerPrefs.GetString("m_api_other_server_address");
        m_ota_other_server_address = PlayerPrefs.GetString("m_ota_other_server_address");

        authAPIAddress = "https://10.0.1.202:33333";

        if (m_ota_servers.Length > 0)
        {
            m_ota_selected_server_id = Mathf.Clamp(m_ota_selected_server_id, 0, m_ota_servers.Length - 1);
        }
        else
        {
            m_ota_selected_server_id = -1;
        }

        if (m_api_servers.Length > 0)
        {
            m_api_selected_server_id = Mathf.Clamp(m_api_selected_server_id, 0, m_api_servers.Length - 1);
        }
        else
        {
            m_api_selected_server_id = -1;
        }

        if (m_languages.Length > 0)
        {
            m_language_id = Mathf.Clamp(m_language_id, 0, m_languages.Length - 1);
        }
        else
        {
            m_language_id = -1;
        }

#if SIMPLE_DEBUG_SCREEN
		m_fte = true;
		m_api_check = true;
#if UNITY_EDITOR
		m_run_after_lost_focus = true;
#endif
#endif

        m_has_start_butten_been_pressed = false;

        m_has_reset_user_data_butten_been_pressed = false;

        m_api_server_names = new string[m_api_servers.Length];
        for (int i = 0; i < m_api_servers.Length; ++i)
        {
            m_api_server_names[i] = m_api_servers[i].m_name;
        }
        m_ota_server_names = new string[m_ota_servers.Length];
        for (int i = 0; i < m_ota_servers.Length; ++i)
        {
            m_ota_server_names[i] = m_ota_servers[i].m_name;
        }
        m_language_names = new string[m_languages.Length];
        for (int i = 0; i < m_languages.Length; ++i)
        {
            m_language_names[i] = EB.Localizer.GetSparxLanguageCode(m_languages[i]);
        }

        yield break;
    }

    public override void Update()
    {
        base.Update();
        if (m_has_start_butten_been_pressed)
        {
            GameEngine.Instance.IsResetUserData = false;
            DebugSystem.UnregisterSystem(this);

            GameEngine.Instance.ApiServerAddress = GetApiServerAddress();
            GameEngine.Instance.OtaServerAddress = GetOtaServerAddress();
            GameEngine.Instance.IsFTE = m_fte;
            GameEngine.Instance.ApiCheckCompleted = !m_api_check;
            UserData.Locale = GetLocale();

#if UNITY_EDITOR
            Application.runInBackground = m_run_after_lost_focus;
#endif

            Mgr.SetGameState<GameStateDownload>();
        }

        if (m_has_reset_user_data_butten_been_pressed)
        {
            GameEngine.Instance.IsResetUserData = true;
            DebugSystem.UnregisterSystem(this);

            GameEngine.Instance.ApiServerAddress = GetApiServerAddress();
            GameEngine.Instance.OtaServerAddress = GetOtaServerAddress();
            GameEngine.Instance.IsFTE = m_fte;
            GameEngine.Instance.ApiCheckCompleted = !m_api_check;
            UserData.Locale = GetLocale();

#if UNITY_EDITOR
            Application.runInBackground = m_run_after_lost_focus;
#endif

            Mgr.SetGameState<GameStateDownload>();
        }
    }
    
    bool m_has_start_butten_been_pressed;
	bool m_has_reset_user_data_butten_been_pressed;

	public ServerConfiguration[] m_api_servers =
	{
#if !SIMPLE_DEBUG_SCREEN
#if UNITY_EDITOR
			new ServerConfiguration ("LocalHost", "http://localhost"),
			new ServerConfiguration ("Ubuntu.VM", "http://ubuntu.vm"),
#endif
			new ServerConfiguration ("local-test-lt1", "http://10.0.1.203"),
			new ServerConfiguration ("local-test-lt2", "http://10.0.1.204"),
			new ServerConfiguration ("API", "https://api.manhuang.org"),
			new ServerConfiguration ("TestFlight API", "https://testflight.api.manhuang.org"),
			new ServerConfiguration ("api.test", "https://api.test.manhuang.org"),
			new ServerConfiguration ("api.mhj", "https://api.mhj.manhuang.org"),
			new ServerConfiguration ("Lxy Server","http://10.1.0.230"),
            new ServerConfiguration ("hzh Server","http://10.0.0.11"),
			new ServerConfiguration ("xd Server","http://10.1.0.60"),
			new ServerConfiguration ("ljl Server","http://10.1.0.59"),
            new ServerConfiguration ("Canada Server","https://s3.ca-central-1.amazonaws.com/api.manhuanggame.com"),
#endif
	};

	public ServerConfiguration[] m_ota_servers =
	{
#if !SIMPLE_DEBUG_SCREEN
#if !UNITY_EDITOR
			new ServerConfiguration ("StreamingFolder", UserData.localAssetAddress),
#else
			new ServerConfiguration ("OutputFolder", "file:///" + BMUtility.InterpretPath("$(Personal)/LTSites", GetRuntimeBuildPlatform()).Replace('\\', '/')),
			new ServerConfiguration ("BundleServer", "http://localhost:9091"),
			new ServerConfiguration ("LocalHost", "http://localhost:9090"),
			new ServerConfiguration ("Ubuntu.VM", "http://ubuntu.vm:9090"),
#endif
			new ServerConfiguration ("OTA", "http://ota.manhuang.org:9090"),
			new ServerConfiguration ("OTA Secure", "https://ota.manhuang.org:9442"),
			new ServerConfiguration ("Amazon S3", "http://s3.amazonaws.com/mhjbundles"),
			new ServerConfiguration ("Amazon S3 Secure", "https://s3.amazonaws.com/mhjbundles"),
			new ServerConfiguration ("local-test-1", "http://10.0.1.201:9090"),
			new ServerConfiguration ("local-test-qa", "http://10.0.1.202:9090"),
			new ServerConfiguration ("api.mhj1-ios", "http://manhuangoss.oss-cn-hangzhou.aliyuncs.com/mhj1/ios/bundle/"),
			new ServerConfiguration ("api.mhj1-android", "http://manhuangoss.oss-cn-hangzhou.aliyuncs.com/mhj1/android/bundle/"),
			new ServerConfiguration ("api.mhj-ios", "http://manhuangoss.oss-cn-hangzhou.aliyuncs.com/mhj/ios/bundle/"),
			new ServerConfiguration ("api.mhj-android", "http://manhuangoss.oss-cn-hangzhou.aliyuncs.com/mhj/android/bundle/"),
			new ServerConfiguration ("macbook", "http://10.0.0.209:9090"), // fix in 76 dhcp
            new ServerConfiguration ("Canada IOS","https://dungeonchessoss.s3.ca-central-1.amazonaws.com/lt/IOS/bundle"),
			new ServerConfiguration ("InernetOTA","http://lostoss.oss-cn-shenzhen.aliyuncs.com/lt/android/bundle"),
#endif
	};

	public EB.Language[] m_languages =
	{
		EB.Language.Unknown,
		EB.Language.English,
		EB.Language.ChineseSimplified,
		EB.Language.ChineseTraditional,
	};

	public static string m_appVersion = "App Version: " + GetVersion();
	string[] m_api_server_names = null;
	string[] m_ota_server_names = null;
	string m_api_other_server_address = string.Empty;
	string m_ota_other_server_address = string.Empty;
	int m_api_selected_server_id = 0;
	int m_ota_selected_server_id = 0;
	string[] m_language_names = null;
	int m_language_id = 0;

	bool m_fte = false;
	bool m_api_check = false;
	Vector2 m_scroll_pos = new Vector2(0, 0);

	bool m_show_fps = false;
	bool m_show_memory_usage = false;
	bool m_use_proxy = false;
	string m_proxy = "127.0.0.1:8888";
#if UNITY_EDITOR
	bool m_run_after_lost_focus = true;
    public static bool HideJoy = false;
#endif

	private static string GetVersion()
	{
		TextAsset versionTxt = (TextAsset)Resources.Load("version");
		string version = versionTxt.text;
		version = version.Replace("\n", "");
		return version;
	}

	void Resize(float width, float height)
	{
		if (Screen.width > width || Screen.height > height)
		{
			Vector3 ratio = new Vector3((float)Screen.width / width, (float)Screen.height / height, 1.0f);
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, ratio);
		}
		else
		{
			GUI.matrix = Matrix4x4.identity;
		}
	}
    
	string GetApiServerAddress()
	{
		if (string.IsNullOrEmpty(m_api_other_server_address))
		{
			if (m_api_selected_server_id >= 0 & m_api_selected_server_id < m_api_servers.Length)
			{
				return m_api_servers[m_api_selected_server_id].m_address;
			}
			else
			{
				return m_api_servers.Length > 0 ? m_api_servers[0].m_address : UserData.defaultApiServerAddress;
			}
		}
		else
		{
			return m_api_other_server_address;
		}
	}

	string GetOtaServerAddress()
	{
		if (string.IsNullOrEmpty(m_ota_other_server_address))
		{
			if (m_ota_selected_server_id >= 0 & m_ota_selected_server_id < m_ota_servers.Length)
			{
				return m_ota_servers[m_ota_selected_server_id].m_address;
			}
			else
			{
				return m_ota_servers.Length > 0 ? m_ota_servers[0].m_address : UserData.localAssetAddress;
			}
		}
		else
		{
			return m_ota_other_server_address;
		}
	}

	EB.Language GetLocale()
	{
		if (m_language_id >= 0 && m_language_id < m_languages.Length)
		{
			return m_languages[m_language_id];
		}
		else
		{
			return EB.Language.Unknown;
		}
	}

	void SetupProxy(string proxy)
	{
		if (string.IsNullOrEmpty(proxy))
		{
			System.Net.IWebProxy sysProxy = null;
			if (System.Net.WebRequest.DefaultWebProxy != null)
			{
				sysProxy = System.Net.WebRequest.DefaultWebProxy;
			}
			else
			{
				sysProxy = System.Net.WebRequest.GetSystemWebProxy();
			}

			if (sysProxy != null)
			{
				System.Uri testUri = new System.Uri("http://localhost/");
				System.Uri pUri = sysProxy.GetProxy(testUri);
				if (pUri.ToString() == testUri.ToString())
				{
					return;
				}
				proxy = pUri.Host + ":" + pUri.Port;
			}
		}

		if (!string.IsNullOrEmpty(proxy))
		{
			if (!proxy.StartsWith("http"))
			{
				proxy = "http://" + proxy;
			}
			EB.Net.WebSocket.Proxy = EB.Net.WebRequest.Proxy = new EB.Uri(proxy);
			HTTP.Request.proxy = new System.Uri(proxy);

			// fixme: xxxxxxxxxxxxxxxxx
			// Network.proxyIP = EB.Net.WebSocket.Proxy.Host;
			// Network.proxyPort = EB.Net.WebSocket.Proxy.Port;
		}
	}

	void OnStartButtonPressed()
	{
		EB.Debug.Log("Start Button Pressed");
		m_has_start_butten_been_pressed = true;

		PlayerPrefs.SetInt("m_api_selected_server_id", m_api_selected_server_id);
		PlayerPrefs.SetInt("m_ota_selected_server_id", m_ota_selected_server_id);
		PlayerPrefs.SetString("m_api_other_server_address", m_api_other_server_address);
		PlayerPrefs.SetString("m_ota_other_server_address", m_ota_other_server_address);
		PlayerPrefs.Save();

		if (m_show_fps || m_show_memory_usage)
		{
			GameObject debugObj = new GameObject("DebugPerformance");
			DebugPerformance debugPerformance = debugObj.AddComponent<DebugPerformance>();
			debugPerformance.displayFps = m_show_fps;
			debugPerformance.displayMemoryUsage = m_show_memory_usage;
		}

		if (m_use_proxy && !string.IsNullOrEmpty(m_proxy))
		{
			SetupProxy(m_proxy);
		}
	}

	void OnResetUserDataButtonPressed()
	{
		EB.Debug.Log("Reset User Data Button Pressed");
		m_has_reset_user_data_butten_been_pressed = true;

		string cachePath = System.IO.Path.Combine(Application.persistentDataPath, "Caches");
		if (System.IO.Directory.Exists(cachePath))
		{
			System.IO.Directory.Delete(cachePath, true);
		}
		PlayerPrefs.DeleteAll();

		PlayerPrefs.SetInt("m_api_selected_server_id", m_api_selected_server_id);
		PlayerPrefs.SetInt("m_ota_selected_server_id", m_ota_selected_server_id);
		PlayerPrefs.SetString("m_api_other_server_address", m_api_other_server_address);
		PlayerPrefs.SetString("m_ota_other_server_address", m_ota_other_server_address);
		PlayerPrefs.Save();

		if (m_show_fps || m_show_memory_usage)
		{
			GameObject debugObj = new GameObject("DebugPerformance");
			DebugPerformance debugPerformance = debugObj.AddComponent<DebugPerformance>();
			debugPerformance.displayFps = m_show_fps;
			debugPerformance.displayMemoryUsage = m_show_memory_usage;
		}

		if (m_use_proxy)
		{
			SetupProxy("127.0.0.1:8888");
		}
	}

	void OnClearAssetsCacheButtonPressed()
	{
		EB.Debug.Log("Clear Assets Cache Button Pressed");
		GM.AssetUtils.ClearAssetsBundlesCache();
	}

    void OnClearDataCacheButtonPressed()
    {
        EB.Debug.Log("Clear DataCache Button Pressed");
        EB.Sparx.DataCacheUtil.ClearDataCache();
    }

	public void OnDrawDebug()
	{

	}

    public static string authAPIAddress;
    public void OnDebugGUI()
	{
		Resize(800, 500);
		m_scroll_pos = GUILayout.BeginScrollView(m_scroll_pos, false, false);

		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

#if !SIMPLE_DEBUG_SCREEN
		GUILayout.BeginHorizontal();
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 12;
        GUILayout.Label("API Server:", guiStyle);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		m_api_selected_server_id = GUILayout.SelectionGrid(m_api_selected_server_id, m_api_server_names, 6, GUILayout.Height(30 * Mathf.Max(1, m_api_server_names.Length / 6 + (m_api_server_names.Length % 6 > 0 ? 1 : 0))));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Other:", guiStyle, GUILayout.Width(90.0f));
		m_api_other_server_address = GUILayout.TextField(m_api_other_server_address, 64, GUILayout.Width(150.0f));
        GUILayout.Label("AuthAPI:", guiStyle, GUILayout.Width(90.0f));
        authAPIAddress = GUILayout.TextField(authAPIAddress, 32, GUILayout.Width(150.0f));
        GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Language:", guiStyle, GUILayout.Width(90.0f));
		m_language_id = GUILayout.SelectionGrid(m_language_id, m_language_names, m_language_names.Length, GUILayout.Width(70.0f * m_language_names.Length));
		GUILayout.EndHorizontal();

		GUILayout.Space(1.0f);

		GUILayout.BeginHorizontal();
		m_api_check = GUILayout.Toggle(m_api_check, new GUIContent("Api version check?", "Get Api server address and Ota server address from public server."), GUILayout.Width(150.0f));
		GUI.color = Color.green;
		GUILayout.Label(m_appVersion, guiStyle);
		GUI.color = Color.white;
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		m_show_fps = GUILayout.Toggle(m_show_fps, new GUIContent("Show FPS?", "Enable to turn the FPS Profiler on."));		
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		m_show_memory_usage = GUILayout.Toggle(m_show_memory_usage, new GUIContent("Show Memory Usage?", "Enable to display Heap & Mono Size."));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		m_use_proxy = GUILayout.Toggle(m_use_proxy, new GUIContent("Use Proxy?", "Use Fiddler to Capture Request & Response"), GUILayout.Width(150.0f));
		if (m_use_proxy)
		{
			m_proxy = GUILayout.TextField(m_proxy, 32, GUILayout.Width(150.0f));
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		m_fte = GUILayout.Toggle(m_fte, "Start with FTE?");
		GUILayout.EndHorizontal();

#if UNITY_EDITOR
		GUILayout.BeginHorizontal();
		m_run_after_lost_focus = GUILayout.Toggle(m_run_after_lost_focus, "Run after Lost Focus?");

        HideJoy = GUILayout.Toggle(HideJoy, "Hide Joy?");
        GUILayout.EndHorizontal();
#endif

#endif

#if !SIMPLE_DEBUG_SCREEN
        GUILayout.Space(1.0f);
        
		GUILayout.BeginHorizontal();
		GUILayout.Label("OTA Server:", guiStyle);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		m_ota_selected_server_id = GUILayout.SelectionGrid(m_ota_selected_server_id, m_ota_server_names, 6, GUILayout.Height(30 * Mathf.Max(1, m_ota_server_names.Length / 6 + (m_ota_server_names.Length % 6 > 0 ? 1 : 0))));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Other:", guiStyle, GUILayout.Width(90.0f));
		m_ota_other_server_address = GUILayout.TextField(m_ota_other_server_address, 32, GUILayout.Width(150.0f));
		GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
		if (GUILayout.Button("Clear AssetsCache", GUILayout.Height(32.0f), GUILayout.Width(130.0f)))
		{
			OnClearAssetsCacheButtonPressed();
		}

        if (GUILayout.Button("Clear DataCache", GUILayout.Height(32.0f), GUILayout.Width(130.0f)))
        {
            OnClearDataCacheButtonPressed();
        }
        GUILayout.EndHorizontal();
#endif
        GUILayout.Space(1.0f);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Start Game", GUILayout.Height(40.0f)))
		{
			OnStartButtonPressed();
		}

		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Reset Account", GUILayout.Height(40.0f)))
		{
			OnResetUserDataButtonPressed();
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
	}

	public void OnDebugPanelGUI()
	{
	}
#else
{
#endif
}