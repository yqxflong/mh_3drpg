using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// This controller will play popup animation when Gameobject get activated.
/// </summary>
public class PopupAnimController : MonoBehaviour 
{
	//public LeanTweenType animTweenType = LeanTweenType.easeSpring;
	public float timer = 0.3f;

	void OnEnable()
	{
		StartAnim();
	}
	
	public void StartAnim()
	{
		transform.localScale = new Vector3(0.01f, 0.01f, 1.0f);
		//LeanTween.scale(gameObject, Vector3.one, timer).tweenType = animTweenType;
		transform.DOScale(Vector3.one, timer).SetEase( Ease.OutSine );
	}
}
