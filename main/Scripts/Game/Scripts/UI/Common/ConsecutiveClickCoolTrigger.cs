using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsecutiveClickCoolTrigger : MonoBehaviour
{
    public List<EventDelegate> clickEvent = new List<EventDelegate>();

    public float coolTime=2;
    private float timer;
    

    void Start()
    {
        timer = coolTime;
    }

    void Update()
    {
		timer += Time.deltaTime;
    }

    public void OnClick()
    {
		if (timer > coolTime)
        {
            EventDelegate.Execute(clickEvent);
            timer = 0;
        }
    }
}
