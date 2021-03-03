using UnityEngine;
using System.Collections;

public class ScreenTransitionMask : MonoBehaviour {
	private static ScreenTransitionMask instance;
	public static ScreenTransitionMask Instance {
		get { return instance; }
	}

	public TweenAlpha TweenAlpha;

	void Start () {
		instance = this;

		gameObject.SetActive(false);
		TweenAlpha.ResetToBeginning();
	}

	/// <summary>
	/// Shows the mask.
	/// Flow : mask anim fade-in -> triggers onShowMaskTransitionComplete() -> [if 'autoPlayHideMaskTranstion' == TRUE] mask anim fade-out.
	/// If you pass 'autoPlayHideMaskTranstion' as FALSE, you must manually call HideMask() when the screen is ready to be shown.
	/// </summary>
	/// <param name="onShowMaskTransitionComplete">Called when mask animation finished to fade-in (appear)</param>
	/// <param name="autoPlayHideMaskTranstion">If set to <c>true</c> Hide() function will be automatically triggered after mask fade-in anim complete and onShowMaskTransitionComplete has been called</param>
	public void ShowMask(System.Action onShowMaskTransitionComplete = null, bool autoPlayHideMaskTranstion = true) {
		if(TweenAlpha.enabled) return;

		gameObject.SetActive(true);
		StartCoroutine(Show_coroutine(onShowMaskTransitionComplete, autoPlayHideMaskTranstion));
	}

	private IEnumerator Show_coroutine(System.Action onShowMaskTransitionComplete, bool autoPlayHideMaskTranstion) {
		yield return null; // must wait one frame before playing tween, cause toggling the gameObject from un-active to active breaks the tween during the first frame

		InputBlockerManager.Instance.Block(InputBlockReason.SCREEN_TRANSITION_MASK, 1000);

		TweenAlpha.PlayForward();
		while(TweenAlpha.enabled) yield return null;

		if(onShowMaskTransitionComplete != null)
			onShowMaskTransitionComplete();

		hideFlag = autoPlayHideMaskTranstion;

		while(!hideFlag) yield return null;

		TweenAlpha.PlayReverse();
		while(TweenAlpha.enabled) yield return null;
		gameObject.SetActive(false);
		InputBlockerManager.Instance.UnBlock(InputBlockReason.SCREEN_TRANSITION_MASK);
	}

	private bool hideFlag;

	/// <summary>
	/// Manualy trigger mask anim fade-out (disappear)
	/// </summary>
	public void HideMask() {
		if(TweenAlpha.enabled) return;

		hideFlag = true;
	}
}
