//ShareToManager
//用于提供分享接口
//Johny

using UnityEngine;
using System.Runtime.InteropServices;

public static class ShareToManager
{
    public static void ShareTextBySystem(string text)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.android.ShareManager"))
        {
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if(unityPlayerClass != null)
            {
                AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                jc.CallStatic("ShareTextToSystem", currentActivity, text);
            }
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
        ShareTextToSystem(text);
#endif
    }

    public static void ShareImageBySystem(string uri)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.android.ShareManager"))
        {
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if(unityPlayerClass != null)
            {
                AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                jc.CallStatic("ShareImgToSystem", currentActivity, uri);
            }
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
        ShareImgToSystem(uri);
#endif
    }

    public static bool IsEmulator()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.android.ShareManager"))
        {
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if(unityPlayerClass != null)
            {
                AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                return jc.CallStatic<bool>("IsEmulator", currentActivity);
            }
            else
            {
                return false;
            }
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
        return false;
#endif

#if UNITY_EDITOR
        return false;
#endif
    }

    public static void CopyToClipboard(string copytext)
    {
        GUIUtility.systemCopyBuffer = copytext;        
    }


#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern void ShareTextToSystem(string text);
    [DllImport("__Internal")]
    public static extern void ShareImgToSystem(string uri);
#endif
}