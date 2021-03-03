using System;
using UnityEngine;

/**
 * Unity3D's TouchScreenKeyboard is flawed, here's our own implementation to make up its shortcomings while having a similar API.
 * 
 * @author SamuliPiela / One Digit Ltd
 */

public static class AndroidKeyboard
{
	static string _className = "org.manhuang.keyboard.AndroidKeyboard";
	static string _methodOpen = "open";
	static string _methodClose = "close";
	static string _methodIsDone = "isDone";
	static string _methodIsCanceled = "isCanceled";
	static string _methodIsVisible = "isVisible";
	static string _methodGetText = "getText";
	static string _methodGetHeight = "getHeight";

	/**
	 * true if the keyboard is not open, false if the keyboard is still open
	 */
	public static bool IsDone
	{
		get
		{
			return AndroidKeyboard._IsDone();
		}
	}

	/**
	 * true if the keyboard has gone away without the end-user hitting the action button first.
	 */
	public static bool IsCanceled
	{
		get
		{
			return AndroidKeyboard._IsCanceled();
		}
	}

	public static bool IsVisible
	{
		get
		{
			return AndroidKeyboard._IsVisible();
		}
	}

	/**
	 * The text that is currently being input or if the keyboard is not open, the last inputted text
	 */
	public static string Text
	{
		get
		{
			return AndroidKeyboard._GetText();
		}
	}

	public static int Height
	{
		get
		{
			return AndroidKeyboard._GetHeight();
		}
	}

	public static void Open()
	{
#if UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass(_className))
		{
			jc.CallStatic(_methodOpen);
		}
#endif
    }

	public static void Close()
	{
#if UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass(_className))
		{
			jc.CallStatic(_methodClose);
		}
#endif
    }

	private static bool _IsDone()
	{
#if UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass(_className))
		{
			return jc.CallStatic<bool>(_methodIsDone);
		}
#else
        return false;
#endif
    }

	private static bool _IsCanceled()
	{
#if UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass(_className))
		{
			return jc.CallStatic<bool>(_methodIsCanceled);
		}
#else
        return false;
#endif
    }

    private static bool _IsVisible()
	{
#if UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass(_className))
		{
			return jc.CallStatic<bool>(_methodIsVisible);
		}
#else
        return false;
#endif
    }

    private static string _GetText()
	{
#if UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass(_className))
		{
			return jc.CallStatic<string>(_methodGetText);
		}
#else
        return "";
#endif
    }

    private static int _GetHeight()
	{
#if UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass(_className))
		{
			return jc.CallStatic<int>(_methodGetHeight);
		}
#else
        return 0;
#endif
    }
}