using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PressOrClick : MonoBehaviour {

    public float deltTime = 0.3f;
    public List<EventDelegate> m_CallBackClick;
    public List<EventDelegate> m_CallBackPress;
    public List<EventDelegate> m_CallBackRelease;

    protected List<EventDelegate> m_CallBackOnPress;
    protected bool ispress;
	private float pressTimer = 0;
	// Use this for initialization
	void Start()
    {
        ispress = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ispress)
        {
            pressTimer += Time.deltaTime;
            //if (pressTimer > 0.3f)
            if (pressTimer > deltTime)
            {
				ispress = false;
				EventDelegate.Execute(m_CallBackPress);
            }
        }
    }

    void OnPress(bool ispressed)
    {
		pressTimer = 0;
		if (ispressed)
        {
            ispress = true;
            EventDelegate.Execute(m_CallBackOnPress);
        }
        else
        {
            ispress = false;
            //判定如果 时间小于多少0.3f 则为click
            if(pressTimer <= deltTime)
            {
                EventDelegate.Execute(m_CallBackClick);
            }
            else
            {
                EventDelegate.Execute(m_CallBackRelease);
            }
        }
        //DisableEnchantButton();
        //requestObj.SendRequest();
        //Invoke("EnableEnchantButton", 1.2f);
    }
}
