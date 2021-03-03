using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogGroupData : ScriptableObject
{
    public List<string> LogUsers = new List<string>();
    public int logLevel = 1;
}
