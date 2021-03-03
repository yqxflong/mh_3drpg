using UnityEngine;

public class FontPrewarm : MonoBehaviour
{
	public Font dynamicFont;
	public int fontSize;
	public FontStyle fontStyle;
	public string text;
    private static bool hasInit = false;
	// Use this for initialization
	void Start()
	{
        if (!hasInit)
        {
            hasInit = true;
            dynamicFont.RequestCharactersInTexture(text, fontSize, fontStyle);
            Texture texture = dynamicFont.material.mainTexture;
            EB.Debug.Log("FontPrewarm: Font texture size: {0} x {1}", texture.width, texture.height);
            dynamicFont.characterInfo = null;
        }
		Destroy(gameObject);
	}
}
