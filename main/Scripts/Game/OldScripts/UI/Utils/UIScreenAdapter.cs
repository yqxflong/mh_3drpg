using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenAdapter : MonoBehaviour
{

    public GameObject Container;

    //public UIRoot Root;

    [HideInInspector]
    public float Scaler;

    public Vector2 BaseScreen = new Vector2(2960,1440);

    public bool BaseOnScreenWidth;

    public bool BaseOnScreenHeight;
	// Use this for initialization
	void Start () {

	    if (BaseOnScreenWidth)
	    {
	        Scaler = Screen.width / BaseScreen.x;
	        Scaler = Mathf.Clamp(Scaler, 0.95f, 1.2f);
            Container.transform.localScale = new Vector3(Scaler,Scaler,1);
            return;
	    }


	    if (BaseOnScreenHeight)
	    {
	        Scaler = Screen.height / BaseScreen.y;
	        Scaler = Mathf.Clamp(Scaler, 0.95f, 1.2f);
            Container.transform.localScale = new Vector3(Scaler, Scaler, 1);
	        return;
        }
	}

    void Update()
    {
#if UNITY_EDITOR
        if (BaseOnScreenHeight)
        {
            Scaler = Screen.height / BaseScreen.y;
            Scaler = Mathf.Clamp(Scaler, 0.95f, 1.2f);
            Container.transform.localScale = new Vector3(Scaler, Scaler, 1);
            return;
        }
#endif
    }
}
