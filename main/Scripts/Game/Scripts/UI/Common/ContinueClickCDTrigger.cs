using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContinueClickCDTrigger : MonoBehaviour {

	public List<EventDelegate> m_CallBackPress;
    public List<EventDelegate> CD_CallBackPress;
	public float intervalTime = 0.2f;
	private float pressTimer = 0;
	protected bool isPress;
	void Start()
	{
		isPress = false;
		pressTimer = intervalTime;
	}

	// Update is called once per frame
	void Update()
	{
		if (isPress)
		{
			//EB.Debug.LogError("Update: isPress = {0}", isPress);
			pressTimer += Time.deltaTime;
			if (pressTimer > intervalTime)
			{
				isPress = false;
			}
		}
	}

	public void OnPress(bool ispressed)
	{
		//EB.Debug.LogError("OnPress: ispressed = {0}", ispressed);
		if(ispressed)
			isPress = true;
		if (ispressed)
		{
		    if (pressTimer >= intervalTime)
		    {
		        pressTimer = 0;
		        EventDelegate.Execute(m_CallBackPress);
            }
		    else
		    {
		        EventDelegate.Execute(CD_CallBackPress);
            }
			
		}
	}
}
