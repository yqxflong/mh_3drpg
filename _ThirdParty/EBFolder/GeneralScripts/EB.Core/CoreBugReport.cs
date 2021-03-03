//#define ENABLE_ALERTVIEW
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

namespace EB
{
	public static class BugReport
	{
		private static string 		_url;
		private static Hashtable	_data = Johny.HashtablePool.Claim();
		private static bool 		_sent;
		public static bool DidCrash {get { return _sent; }}
		
		public static event System.Action OnBugReport;

#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		static extern string _initBugReport(string crashFile, string reportUrl,bool showAlertView);

		[DllImport("__Internal")]
		static extern void _bugAlert(string msg);

		[DllImport("__Internal")]
		static extern void _createCrash();
#endif

		public static void Init( string url)
		{
			if (!string.IsNullOrEmpty(_url))
			{
				// inited
				EB.Debug.Log("BugReport.Init: inited, do nothing");
				return;
			}

			_sent = false;
			_url = url;

			string lastCrash = NativeSetup();
			if (!string.IsNullOrEmpty(lastCrash))
			{
				EB.Debug.LogWarning("BugReport.Init: last crash log was found: {0}", lastCrash);
				EB.Coroutines.Run(SendBugReport(lastCrash));
			}
			
			// Application.logMessageReceived += LogCallback;
			System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionCallback;

			AddData("os", SystemInfo.operatingSystem );	
			AddData("sl", SystemInfo.graphicsShaderLevel );
			AddData("graphics", SystemInfo.graphicsDeviceName );
			AddData("unity", Application.unityVersion );
			AddData("version", Version.GetVersion() );
			AddData("changelist", Version.GetChangeList() );
			AddData("locale", Version.GetLocale() );
			
#if UNITY_IPHONE
			AddData("device", UnityEngine.iOS.Device.generation.ToString() );
#elif UNITY_ANDROID
			AddData("device", SystemInfo.deviceModel );
			AddData("processor", SystemInfo.processorType );
#endif
		}

		public static void AddData( string key, object value )
		{
			_data[key] = value;
		}
		
		static IEnumerator SendBugReport()
		{
			// add log
			Debug.Dump(_data);
			
			// screen shot
			yield return new WaitForEndOfFrame();
			TakeScreenShot();
			
			// call this after the screenshot so we don't see any error dialogs			
			try
			{
				if (OnBugReport != null)
				{
					OnBugReport();
				}
			}
			catch
			{

			}

			if (Application.isEditor)
			{
				yield break;
			}
			
			var json = JSON.Stringify(_data);
			yield return EB.Coroutines.Run(SendBugReport(json));

			NativeAlert("bugAlert", (_data["condition"] ?? _data["message"]).ToString());
		}

		static IEnumerator SendBugReport(string crash)
		{
			if (Application.isEditor)
			{
				yield break;
			}

			var bytes = Encoding.GetBytes(crash);

			if (_url.StartsWith("http://"))
			{
				//Dictionary<string, string> headers = new Dictionary<string, string>();
				//headers.Add("Content-Type", "application/json");
                

                UnityWebRequest uwr = new UnityWebRequest(_url, UnityWebRequest.kHttpVerbPOST);
                uwr.uploadHandler = new UploadHandlerRaw(bytes);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");

                //var www = new WWW( _url, bytes, headers );
				yield return uwr.SendWebRequest(); 
				if (uwr.isHttpError || uwr.isNetworkError)
				{
					UnityEngine.Debug.LogWarning(uwr.error);
				}
			}
			else
			{
				HTTP.Request r = new HTTP.Request("POST", _url, bytes);
				r.headers.Add("Content-Type", "application/json");
				yield return r.Send();
				if (r.exception != null)
				{
					UnityEngine.Debug.LogWarning(r.exception.Message);
				}
			}
		}

		static void TakeScreenShot()
		{
			try
			{
				if ( Camera.main != null )
				{				
					var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
					tex.ReadPixels( new Rect(0,0,Screen.width,Screen.height), 0, 0);
					tex.Apply();
					
					var scale = 360.0f / (float)Screen.width;
					var encoder = new JPGEncoder(tex, 25, scale); 
					var bytes = encoder.GetBytes();
					
					Texture2D.Destroy(tex);
					AddData("screen", Encoding.ToBase64String(bytes) );
				}
			}
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogWarning("Failed to create screenshot " + ex);
			}
		}
		
		static void LogCallback (string condition, string stackTrace, LogType type)
		{
			if (_sent)
			{
				return;
			}
			
			if ( type == LogType.Exception )
			{
				_sent = true;
				AddData("type", "exception");
				AddData("condition", condition);
				AddData("stack", stackTrace);
				Coroutines.Run(SendBugReport());
			}
		}

		static void UnhandledExceptionCallback(object sender, System.UnhandledExceptionEventArgs args)
		{
			if (_sent)
			{
				return;
			}

			if (args == null || args.ExceptionObject == null)
			{
				return;
			}

			System.Exception e = args.ExceptionObject as System.Exception;
			if (e != null)
			{
				_sent = true;
				AddData("type", "crash");
				AddData("name", e.GetType().ToString());
				AddData("message", e.Message);
				AddData("sender", "UnhandledExceptionCallback" + sender.ToString());
				AddData("stack", StackTrace(e));
				Coroutines.Run(SendBugReport());
			}
		}

		static string StackTrace(System.Exception e)
		{
			string answer = e.StackTrace;
			// Using seen for cycle detection to break cycling.
			List<System.Exception> seen = new List<System.Exception> ();
			seen.Add(e);
			if (answer != null)
			{
				// There has to be some way of telling where InnerException ie stacktrace
				// ends and main Exception e stacktrace begins.  This is it.
				answer = ((e.GetType().FullName + " : " + e.Message + "\r\n")
					+ answer);
				System.Exception ie = e.InnerException;
				while ((ie != null) && (seen.IndexOf(ie) < 0))
				{
					seen.Add(ie);
					answer = ((ie.GetType().FullName + " : " + ie.Message + "\r\n")
						+ (ie.StackTrace + "\r\n")
						+ answer);
					ie = ie.InnerException;
				}
			}
			else {
				answer = "";
			}
			return answer;
		}

#if DEBUG
		public static void CreateCrash()
		{
#if !UNITY_EDITOR && UNITY_IPHONE
			_createCrash();
#elif !UNITY_EDITOR && UNITY_ANDROID
			using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.app.CrashCatchHandler"))
			{
				jc.CallStatic("createCrash");
			}
#endif
		}

		public static void MakeBug()
		{
			try
			{
				throw new System.Exception("Make Bug Test Exception");
			}
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogException(ex);
			}
		}
#endif

		private static void NativeAlert(string title, string message)
		{
#if UNITY_IPHONE && !UNITY_EDITOR
			_bugAlert((_data["condition"] ?? _data["message"]).ToString());
#elif UNITY_ANDROID && !UNITY_EDITOR
			using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.app.CrashCatchHandler"))
			{
				jc.CallStatic("bugAlert", (_data["condition"] ?? _data["message"]).ToString());
			}
#endif
		}

		private static string NativeSetup()
		{
#if UNITY_IPHONE && !UNITY_EDITOR
#if ENABLE_ALERTVIEW
			return _initBugReport("error.log", _url,true);
#else
			return _initBugReport("error.log", _url,false);
#endif
#elif UNITY_ANDROID && !UNITY_EDITOR
			using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.app.CrashCatchHandler"))
			{
#if ENABLE_ALERTVIEW
				return jc.CallStatic<string>("initBugReport", "error.log", _url,true);
#else
				return jc.CallStatic<string>("initBugReport", "error.log", _url,false);
#endif
			}
#else
			return string.Empty;
#endif
		}
	}	
}
