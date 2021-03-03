using UnityEngine;
using System.Collections;

/// <summary>
/// 所有的界面的接口
/// </summary>
public interface IStackableUI 
{
	/// <summary>
	/// if root hud ui Enstack when it create
	/// </summary>
	bool EnstackOnCreate { get; }

	/// <summary>
	/// show blocker or not
	/// </summary>
	bool ShowUIBlocker { get; }

	/// <summary>
	/// Background Panel Fade Time, Background Panel will hide when full screen ui opened
	/// </summary>
	float BackgroundUIFadeTime { get; }

	/// <summary>
	/// ui visibility, true is visible, false is hidden
	/// </summary>
	bool Visibility { get; }

    IEnumerator OnPrepareAddToStack();

    /// <summary>
    /// add to stack, usually show ui
    /// </summary>
    /// <returns></returns>
    IEnumerator OnAddToStack();

	/// <summary>
	/// remove from stack, usually hide ui
	/// </summary>
	/// <returns></returns>
	IEnumerator OnRemoveFromStack();

	/// <summary>
	/// set Visibility
	/// </summary>
	/// <param name="isShowing"></param>
	void Show(bool isShowing);

	/// <summary>
	/// called when become top ui
	/// </summary>
	void OnFocus();

	/// <summary>
	/// called when lost focus
	/// </summary>
	void OnBlur();

	/// <summary>
	/// return ui could be auto back stack
	/// </summary>
	/// <returns></returns>
	bool CanAutoBackstack();

	/// <summary>
	/// return is full screen ui
	/// </summary>
	/// <returns></returns>
	bool IsFullscreen();

	/// <summary>
	/// if (true == IsFullscreen()), do we want the world to render
	/// </summary>
	/// <returns></returns>
	bool IsRenderingWorldWhileFullscreen();

    /// <summary>
    /// clear all data by destory
    /// </summary>
    void ClearData();
}
