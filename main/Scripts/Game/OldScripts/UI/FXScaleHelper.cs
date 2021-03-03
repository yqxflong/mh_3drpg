using UnityEngine;
using System.Collections;

public class FXScaleHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float xScale =transform.localScale.x/transform.lossyScale.x;
		float yScale = transform.localScale.y / transform.lossyScale.y;
		float zScale = transform.localScale.z / transform.lossyScale.z;
		transform.localScale = new Vector3(xScale, yScale, zScale);
	}	
}
