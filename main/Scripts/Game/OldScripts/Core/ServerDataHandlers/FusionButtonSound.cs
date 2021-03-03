using UnityEngine;
using System.Collections;

public class FusionButtonSound : MonoBehaviour 
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
	}
	
	public Trigger trigger = Trigger.OnClick;
	
	void OnClick()
	{
		FusionAudio.PostEvent(FusionAudio.eEvent.SFX_UI_ButtonClick, gameObject, true);
	}
}
