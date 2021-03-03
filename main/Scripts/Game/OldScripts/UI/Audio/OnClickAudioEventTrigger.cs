using UnityEngine;
using System.Collections;

public class OnClickAudioEventTrigger : MonoBehaviour 
{
	public string m_AudioEvent;
	
	void OnClick()
	{
		FusionAudio.PostEvent(m_AudioEvent, gameObject, true);
	}
}
