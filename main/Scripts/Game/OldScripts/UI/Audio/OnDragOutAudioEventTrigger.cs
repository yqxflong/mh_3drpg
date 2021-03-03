using UnityEngine;
using System.Collections;

public class OnDragOutAudioEventTrigger : MonoBehaviour 
{
	public string m_AudioEvent;
	
	void OnDragOut()
	{
		FusionAudio.PostEvent(m_AudioEvent, gameObject, true);
	}
}
