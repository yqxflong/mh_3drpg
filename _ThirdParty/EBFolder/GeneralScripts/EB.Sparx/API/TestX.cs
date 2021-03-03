using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestX : MonoBehaviour {

    private static TestX ins;

    public string login = "";

    private string loginUid = "015fe7e4ae77a846c62cc7fed601d070ec917263-EDITOR";

    public static TestX Ins
    {
        get
        {
            if (ins == null && GameObject.Find("LoginEnter") != null)
            {
                ins = GameObject.Find("LoginEnter").GetComponent<TestX>();
            }
            return ins;
        }
    }

    public string GetLoginUid(string loginUid)
    {
        return loginUid + login;
    }
}
