using UnityEngine;
using System.Collections;

public class TextureMove : MonoBehaviour
{
	public float speed = 0.05f;
	private UITexture uiTex = null;

	void Start()
	{
		uiTex = GetComponent<UITexture>();
	}

	void Update()
	{
		Rect r = uiTex.uvRect;
		r.x += Time.deltaTime * speed;
		if (r.x > 1)
		{
			r.x -= 1;
		}
		uiTex.uvRect = r;
	}
}
