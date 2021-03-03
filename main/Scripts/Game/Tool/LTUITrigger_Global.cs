using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTUITrigger_Global : MonoBehaviour
{
    public List<EventDelegate> clickEvent = new List<EventDelegate>();
    public int CooldownTime = 1;
    private static int timer;

    public void OnClick()
    {
        if (timer <= EB.Time.Now)
        {
            timer = EB.Time.Now + CooldownTime;
            EventDelegate.Execute(clickEvent);
        }
    }
}
