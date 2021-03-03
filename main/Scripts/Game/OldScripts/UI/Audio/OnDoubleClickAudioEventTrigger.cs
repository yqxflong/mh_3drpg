using UnityEngine;
using System.Collections;

public class OnDoubleClickAudioEventTrigger : MonoBehaviour 
{
	public string m_AudioEvent;
	
	void OnDoubleClick()
	{
		FusionAudio.PostEvent(m_AudioEvent, gameObject, true);
	}
}
