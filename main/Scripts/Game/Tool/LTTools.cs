using System.Text.RegularExpressions;
using UnityEngine;

public static class LTTools
{
	public static string TrimAll(this string text, params char[] chars)
	{
		int len = text.Length;
		char[] s2 = new char[len];
		int i2 = 0;
		for (int i = 0; i < len; i++)
		{
			char c = text[i];
			bool add = true;
			for (int k = 0; k < chars.Length; k++)
			{
				char target = chars[k];
				if (c == target)
				{
					add = false;
					break;
				}
			}
			if(add)s2[i2++] = c;
		}
		return new string(s2, 0, i2);
	}

	public static string ReplaceWholeWord(this string original, string wordToFind, string replacement, RegexOptions regexOptions = RegexOptions.None)
	{
		string pattern = string.Format(@"\b{0}\b", wordToFind);

		string ret = Regex.Replace(original, pattern, replacement, regexOptions);

		return ret;
	}

	public static void SwitchToPlayerCamera(bool isFull)
	{
#if UNITY_ANDROID
		//Debug.LogFormat("SwitchToPlayerCamera({0})", isFull);
		if (!isFull) return;
		PlayerCameraComponent playerCamera = CameraBase.GetInstance() as PlayerCameraComponent;
		if (playerCamera != null) playerCamera.GetComponent<Camera>().enabled = true;

		//if (UICamera.currentCamera)
		//	UICamera.currentCamera.enabled = false;
		//else
		//{
		//	if (UICamera.current)
		//		UICamera.current.cachedCamera.enabled = false;
		//}
#endif
	}

	public static void SwitchToUICamera(bool isFull)
	{
#if UNITY_ANDROID
		//Debug.LogFormat("SwitchToUICamera({0})", isFull);
		if (!isFull) return;
		// PlayerCameraComponent playerCamera = CameraBase.GetInstance() as PlayerCameraComponent;
		// if (playerCamera != null) playerCamera.GetComponent<Camera>().enabled = false;

		if (UICamera.currentCamera)
			UICamera.currentCamera.enabled = true;
		else
		{
			if (UICamera.current)
				UICamera.current.cachedCamera.enabled = true;
		}
#endif
	}

	public static void OpenSceneCamera()
	{
#if UNITY_ANDROID
		//Debug.Log("OpenSceneCamera");
		PlayerCameraComponent playerCamera = CameraBase.GetInstance() as PlayerCameraComponent;
		if (playerCamera != null) playerCamera.GetComponent<Camera>().enabled = true;
#endif
	}
}