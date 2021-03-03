using UnityEngine;
using System.Collections;

public class AddEventListener : MonoBehaviour {
    private void OnDestroy() {
        Destroy(gameObject.GetComponent<UIEventListener>());
    }

    private void OnEnable() {
        gameObject.AddComponent<UIEventListener>();
    }
}
