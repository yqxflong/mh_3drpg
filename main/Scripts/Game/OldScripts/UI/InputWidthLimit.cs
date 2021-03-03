using UnityEngine;
using System.Collections;

public class InputWidthLimit : MonoBehaviour
{
	public int inputWidthLimit;

	private UIInput mInput;

	// Use this for initialization
	void Start()
	{
		mInput = GetComponent<UIInput>();
		SetupChangeDelegate();
	}

	private void OnInputChange()
	{
		if (inputWidthLimit > 0)
		{
			string str = mInput.value;

			while (GetStringWidth(str) > inputWidthLimit)
			{
				str = str.Substring(0, str.Length - 1);
			}

			if (mInput.value != str)
			{
				mInput.value = str;
			}
		}
	}

	public void SetupChangeDelegate()
	{
		if (mInput != null)
		{
			mInput.onChange.Add(new EventDelegate(OnInputChange));
		}
	}

	public int GetStringWidth(string str)
	{
		char[] chars = str.ToCharArray();
		int width = 0;
		foreach (var ch in chars)
		{
			// see: http://php.net/manual/zh/function.mb-strwidth.php
			if (ch >= '\u0000' && ch <= '\u0019')
			{
				width += 0;
			}
			else if (ch >= '\u0020' && ch <= '\u1fff')
			{
				width += 1;
			}
			else if (ch >= '\u2000' && ch <= '\uff60')
			{
				width += 2;
			}
			else if (ch >= '\uff61' && ch <= '\uff9f')
			{
				width += 1;
			}
			else
			{
				width += 2;
			}
		}

		return width;
	}

    public UILabel Label;
    public UIInput Input;

    public void Clear() {
        Label.text = "";
        Input.value = "";
    }
}
