using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTUITrigger_Individual : MonoBehaviour
{
    public List<EventDelegate> clickEvent = new List<EventDelegate>();
    public float CooldownTime = 1f;
    private float timer;
    void Start()
    {
        timer = CooldownTime;
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    public void OnClick()
    {
        if (timer > CooldownTime)
        {
            timer = 0;
            EventDelegate.Execute(clickEvent);
        }
    }
}
