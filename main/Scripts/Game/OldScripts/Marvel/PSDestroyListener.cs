using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSDestroyListener : MonoBehaviour
{
    private void OnDestroy()
    {
        EB.Debug.LogPSPoolAsset(string.Format("<color=#FF0000>这个特效寄主</color>{0}被清除", this.name));
    }

}
