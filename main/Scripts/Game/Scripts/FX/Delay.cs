using UnityEngine;
using System.Collections;

public class Delay : MonoBehaviour {
	
	public float delayTime = 1.0f;
	
	void Awake(){
		gameObject.SetActive(false);
		Invoke("DelayFunc", delayTime);
	}
	
	void DelayFunc()
	{
		gameObject.SetActive(true);
	}
	
}
