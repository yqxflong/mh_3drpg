using UnityEngine;
using System.Collections;

public class OnDragEndAudioEventTrigger : MonoBehaviour 
{
	public string m_AudioEvent;
	
	void OnDragEnd()
	{
		FusionAudio.PostEvent(m_AudioEvent, gameObject, true);
	}
}
