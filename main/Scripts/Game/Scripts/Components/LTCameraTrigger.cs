using UnityEngine;
using System.Collections;

public class LTCameraTrigger : MonoBehaviour
{
    public string triggerGameCameraParamName;
	public float stopDist=5;

    public void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactive");
		AreaTriggersManager.Instance.Register(this.name, transform);
    }

    public void OnDestroy()
    {
		AreaTriggersManager.Instance.UnRegister(this.name, transform);
	}
}
