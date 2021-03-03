//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY || UNITY_WINRT)
#define MOBILE
#endif

using UnityEngine;

/// <summary>
/// This class is added by UIInput when it gets selected in order to be able to receive input events properly.
/// The reason it's not a part of UIInput is because it allocates 336 bytes of GC every update because of OnGUI.
/// It's best to only keep it active when it's actually needed.
/// </summary>

[RequireComponent(typeof(UISymbolInput))]
public class UISymbolInputOnGUI : MonoBehaviour
{
#if !MOBILE
	[System.NonSerialized] UISymbolInput mInput;

	void Awake() { mInput = GetComponent<UISymbolInput>(); }

	/// <summary>
	/// Unfortunately Unity 4.3 and earlier doesn't offer a way to properly process events outside of OnGUI.
	/// </summary>

	void OnGUI ()
	{
		if (Event.current.rawType == EventType.KeyDown)
			mInput.ProcessEvent(Event.current);
	}
#endif
}
