using UnityEngine;
using System.Collections;

public class OnDragStartAudioEventTrigger : MonoBehaviour 
{
	public string m_AudioEvent;
	
	void OnDragStart()
	{
		FusionAudio.PostEvent(m_AudioEvent, gameObject, true);
	}
}
