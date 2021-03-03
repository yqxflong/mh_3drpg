using UnityEngine;
using System.Collections;

public class FadeScreenUIController : MonoBehaviour {

	private UISprite fadeMaskSprite;
	static public bool sFadeOver=true;

    void Awake()
    {
        fadeMaskSprite = transform.Find("MaskSprite").GetComponent<UISprite>();
    }

    public IEnumerator FadeOut(bool needFadein=true)
	{
		sFadeOver = false;
		float time = 0.0f;
		while (time < GameVars.SceneFadeTime)
		{
			yield return null;

			time += Time.unscaledDeltaTime;
			fadeMaskSprite.alpha = (time / GameVars.SceneFadeTime);
		}
		fadeMaskSprite.alpha = 1;
		if (needFadein)
			StartCoroutine(FadeIn());
		else
		{
			sFadeOver = true;
			Destroy(gameObject);
		}
		yield break;
	}

	IEnumerator FadeIn()
	{
		float time = 0.0f;
		while (time < GameVars.SceneFadeTime)
		{
			yield return null;

			time += Time.unscaledDeltaTime;
			fadeMaskSprite.alpha = 1.0f - (time / GameVars.SceneFadeTime);
		}
		sFadeOver = true;
		Destroy(gameObject);
	}
}
