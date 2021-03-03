using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContinuePressTrigger : MonoBehaviour {

	public List<EventDelegate> m_CallBackPress;

    public List<EventDelegate> mFasterCallBackPress;

    public List<EventDelegate> mFastestCallBackPress;

    public float fasterTime = 5;

    public float fastestTime = 10;

    public System.Action eventList;

    private float totalPressTime = 0;

    public float deltTime = 0.2f;
	private float pressTimer = 0;
	protected bool isPress;
	void Start()
	{
		isPress = false;
		pressTimer = 0;
	}

	// Update is called once per frame
	void Update()
	{
		
        if (isPress)
		{
            //EB.Debug.LogError("Update: isPress = {0}", isPress);
            pressTimer += Time.deltaTime;
            totalPressTime += Time.deltaTime;
			if (pressTimer > deltTime)
			{
                List<EventDelegate> callback = new List<EventDelegate>();
                if (totalPressTime >= fastestTime)
                {
                    callback = mFastestCallBackPress;
                }
                else if (totalPressTime >= fasterTime)
                {
                    callback = mFasterCallBackPress;
                }

                if (callback.Count <= 0)
                {
                    callback = m_CallBackPress;
                }
				pressTimer = 0;
				EventDelegate.Execute(callback);
			}
		}
	}

	void OnPress(bool ispressed)
	{
        //EB.Debug.LogError("OnPress: ispressed = {0}", ispressed);
		isPress = ispressed;
        if (ispressed)
        {
            EventDelegate.Execute(m_CallBackPress);
        }
        pressTimer = 0;
        totalPressTime = 0;
    }
}
