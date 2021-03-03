using UnityEngine;
using System.Collections;

public class WinnerIOSSDKController : MonoBehaviour
{
#if USE_WINNERIOSSDK
	private UniWebView _webView = null;
	private const string url = "http://sdk-h5.winnergame.tv/user/game/index/appid/{0}/versions/iosapp/screens/landscape";
	private const int app_id = 60209;
    // Use this for initialization
	void Awake()
    {
#if UNITY_IPHONE 
		if (_webView == null)
		{
			_webView = gameObject.AddComponent<UniWebView>();
			_webView.OnReceivedMessage += OnReceivedMessage;
			_webView.OnLoadComplete += OnLoadComplete;
			_webView.OnWebViewShouldClose += OnWebViewShouldClose;
			_webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
			_webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
			_webView.AddUrlScheme("http");
        }
#endif
	}

	public void LoginWinnerSDK()
	{
#if UNITY_IPHONE 
		_webView.url= string.Format(url, app_id);
		_webView.Load();
#endif
	}

#if UNITY_IPHONE 
	void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
	{
		EB.Debug.Log(message.rawMessage);
		if (message.path.Contains( "sdk-h5.winnergame.tv/User/Game/outparams"))
		{
			int userid = 0;
			string username = string.Empty;
			long logintime = 0;
			string token = string.Empty;
			string[] splits=message.path.Split('/');
			for(int i=0;i<splits.Length-1;i++)
			{
				if (splits[i]== "userId")
				{
					int.TryParse(splits[i+1],out userid);
                }
				else if(splits[i] == "username")
				{
					username = splits[i + 1];
				}
				else if (splits[i] == "logintime")
				{
					long.TryParse(splits[i + 1], out logintime);
				}
				else if (splits[i] == "token")
				{
					token = splits[i + 1];
				}
			}
			Hashtable data = new Hashtable() { { "mem_id", userid }, { "access_token", token }, {"username",username }, {"logintime",logintime } , { "type","ios"} };
			Hashtable result = new Hashtable() { { "callbackType", "Login" }, {"code",0 }, {"data",data } };
			Transform parent = transform.parent;
            if (parent!=null)
			{
				parent.gameObject.SendMessage("OnWinnerIOSSDKCallBack", result, SendMessageOptions.DontRequireReceiver);
            }
			else
			{
				EB.Debug.LogError("OnReceivedMessage transform.parent is null");
			}
			webView.Hide();
			Destroy(webView);
			_webView.OnReceivedMessage -= OnReceivedMessage;
			_webView.OnLoadComplete -= OnLoadComplete;
			_webView.OnWebViewShouldClose -= OnWebViewShouldClose;
			_webView.OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
			_webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
			_webView = null;
		}
}

//5. When the webView complete loading the url sucessfully, you can show it.
//   You can also set the autoShowWhenLoadComplete of UniWebView to show it automatically when it loads finished.
void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		EB.Debug.Log("OnLoadComplete: {0}  {1}" , success,errorMessage);
		if (success)
		{
			webView.Show();
		}
		else
		{
			EB.Debug.Log("Something wrong in webview loading: " + errorMessage);
		}
	}

	//In this demo, we set the text to the return value from js.
	void OnEvalJavaScriptFinished(UniWebView webView, string result)
	{
		EB.Debug.Log("js result: " + result);
	}

	//10. If the user close the webview by tap back button (Android) or toolbar Done button (iOS), 
	//    we should set your reference to null to release it. 
	//    Then we can return true here to tell the webview to dismiss.
	bool OnWebViewShouldClose(UniWebView webView)
	{
		EB.Debug.Log("OnWebViewShouldClose");
		if (webView == _webView)
		{
			_webView = null;
			return true;
		}
		EB.Debug.Log("OnWebViewShouldClose webView!= _webView");
		return false;
	}

	// This method will be called when the screen orientation changed. Here we returned UniWebViewEdgeInsets(5,5,bottomInset,5)
	// for both situation. Although they seem to be the same, screenHeight was changed, leading a difference between the result.
	// eg. on iPhone 5, bottomInset is 284 (568 * 0.5) in portrait mode while it is 160 (320 * 0.5) in landscape.
	UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
	{
		return new UniWebViewEdgeInsets(0, 0, 0, 0);
	}
#endif
#endif
}