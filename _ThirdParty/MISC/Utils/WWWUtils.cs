using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Networking;

public static class WWWUtils
{
    public enum Environment
    {
        LocalHost,
        UbuntuVM,
        LocalTest,
        LocalTestQA,
        API,
        TestFlightAPI,
        Custom
    };

    public static string[] ServerConfig =
    {
        "localhost",
        "ubuntu.vm",
        "10.0.1.203:8090",
        "10.0.1.202:8090",
        "api.manhuang.org",
        "testflight.api.manhuang.org",
        ""
    };

    public static Environment Environ = Environment.LocalHost;
    public static string Email = string.Empty;
    public static string Password = string.Empty;

    public static string GetAdminUrl( Environment _env )
    {
        return "https://" + ServerConfig[(int)_env];
    }

    public static string AdminUrl( string uri )
    {
        return GetAdminUrl(Environ) + uri;
    }

    public static string AdminLogin()
    {
        return AdminLogin(Environ, Email, Password);
    }

    public static string AdminLogin(Environment e)
    {
        return AdminLogin(e, "admin@sparx.io", "qwER!@34");
    }

    public static string AdminLogin(Environment e, string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            Debug.LogWarning("AdminLogin: email is empty");
            return null;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("AdminLogin: password is empty");
            return null;
        }

        WWWForm form = new WWWForm();
        form.AddField("format", "plain");
        form.AddField("email", email);
        form.AddField("password", password);
        return Post(GetAdminUrl(e) + "/session/dologin", form);
    }

    public static void WaitOnWWW(UnityWebRequest www, bool throwOnError )
    {
        www.SendWebRequest();
        while (!www.isDone)
        {
            System.Threading.Thread.Sleep(10);
            //yield return null;
        }

        if ((www.isHttpError || www.isNetworkError) && throwOnError)
        {
           EB.Debug.Log("Error: " + www.error);
            Debug.LogFormat("Headers: {0}", www.responseCode);// responseHeaders );
        }

        //Debug.Log(www.text);
    }

    public static void WaitOnHttpRequest(HTTP.Request req, bool throwOnError)
    {
        while (!req.isDone)
        {
            System.Threading.Thread.Sleep(10);
            //yield return null;
        }

        if (req.exception != null && throwOnError)
        {
           EB.Debug.Log("Exception: " + req.exception.Message);
            Debug.LogFormat("Headers: {0}", req.headers);
        }
    }

    public static string HandleRequest(string url, WWWForm form, bool throwOnError)
    {
       EB.Debug.Log(url);
        string lastError = string.Empty;
        for (int i = 0; i < 1; ++i)
        {
            //var www = form != null ? new WWW(url, form) : new WWW(url);
            UnityWebRequest request = form != null ? UnityWebRequest.Post(url, form) : UnityWebRequest.Get(url);
            WaitOnWWW(request, false);

            /*var status = "";
            if (!www.responseHeaders.TryGetValue("STATUS", out status))
            {
                status = string.Empty;
            }

            if (string.IsNullOrEmpty(www.error))
            {
                return www.text;
            }
            else if (status.Contains("303 See Other"))
            {
                return string.Empty;
            }
            else if (status.Contains("404 Not Found"))
            {
                lastError = www.error;
                break;
            }*/

            long code = request.responseCode;
            if (string.IsNullOrEmpty(request.error))
            {
                return request.downloadHandler.text;
            }
            else if (code == 303)
            {
                return string.Empty;
            }
            else if (code == 404)
            {
                lastError = request.error;
                break;
            }


            lastError = request.error;
            Debug.LogError("Request " + url + " Failed: " + lastError + " Status: " + code);
            Debug.LogErrorFormat("Headers: {0}", request.responseCode); //.responseHeaders);
            System.Threading.Thread.Sleep(5 * 1000);

            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        if (!string.IsNullOrEmpty(lastError) && throwOnError)
        {
            throw new System.Exception("Build Failed: " + lastError);
        }
        return string.Empty;
    }

    public static string HandleHttpRequest(string url, WWWForm form, bool throwOnError)
    {
       EB.Debug.Log(url);
        string lastError = string.Empty;
        for (int i = 0; i < 1; ++i)
        {
            var www = form != null ? new HTTP.Request(url, form) : new HTTP.Request("GET", url);
            www.acceptGzip = false;
            www.useCache = false;
            www.Send();
            WaitOnHttpRequest(www, false);

            var status = www.headers.Get("STATUS");
            if (www.exception == null)
            {
                return www.response.Text;
            }
            else if (status.Contains("303 See Other"))
            {
                return string.Empty;
            }
            else if (status.Contains("404 Not Found"))
            {
                lastError = www.exception.Message;
                break;
            }

            lastError = www.exception.Message;
            Debug.LogError("Request " + url + " Failed: " + lastError + " Status: " + status);
            Debug.LogErrorFormat("Headers: {0}", www.headers);
            System.Threading.Thread.Sleep(5 * 1000);

            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        if (!string.IsNullOrEmpty(lastError) && throwOnError)
        {
            throw new System.Exception("Build Failed: " + lastError);
        }
        return string.Empty;
    }

    public static string Get(string url)
    {
        return HandleHttpRequest(url, null, true);
    }

    public static object GetJson(string url)
    {
        return EB.JSON.Parse(Get(url));
    }

    public static string Post(string url, WWWForm form)
    {
        return HandleHttpRequest(url, form, true);
    }

    public static object PostJson(string url, WWWForm form)
    {
        return EB.JSON.Parse(Post(url,form));
    }
}
