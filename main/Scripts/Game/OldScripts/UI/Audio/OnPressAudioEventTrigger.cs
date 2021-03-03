using UnityEngine;
using System.Collections;

public class OnPressAudioEventTrigger : MonoBehaviour 
{
	public string m_AudioEvent;
	
	void OnPress()
	{
		FusionAudio.PostEvent(m_AudioEvent, gameObject, true);
	}
}
