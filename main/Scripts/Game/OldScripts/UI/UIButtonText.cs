using UnityEngine;
using System.Collections;

public class UIButtonText : UIButton {

	public UILabel buttonLabel;

	protected Vector3 m_normal_text_pos;
	protected Vector3 m_pressed_text_pos;
	protected override void OnInit ()
	{
		base.OnInit ();
		if(buttonLabel != null)
		{
			m_normal_text_pos = buttonLabel.transform.localPosition;
			m_pressed_text_pos = new Vector3(m_normal_text_pos.x - 2.0f, m_normal_text_pos.y - 5.0f, 0.0f);
		}
	}

	protected override void OnPress (bool isPressed)
	{
		base.OnPress (isPressed);
		if(isEnabled)
		{
			if(buttonLabel != null)
			{
				if(isPressed)
				{
					buttonLabel.transform.localPosition = m_pressed_text_pos;
				}
				else
				{
					buttonLabel.transform.localPosition = m_normal_text_pos;
				}
			}
		}
	}
}
