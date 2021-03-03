using UnityEngine;
using System.Collections;

public class UIConditionTabAdapt : MonoBehaviour {
    public virtual bool IsConditionOk()
    {
        return true;
    }

    public virtual bool ShowConditionMessage()
    {
        return true;
    }
}
