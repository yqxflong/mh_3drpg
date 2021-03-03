using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ToggleChangeStyle
{
    TextColor = 0,
    SpriteChange = 1,
}

public class ToggleGroupState : MonoBehaviour
{
    public ToggleChangeStyle style = ToggleChangeStyle.TextColor;
    public Color m_PressColor = GameColorValue.Yellow_Light_Color;
    public Color m_UnPressColor = GameColorValue.Gray_Color;
    public bool m_ChangeSpriteColor = false;

    public List<UIToggle> m_Toggles;

    void Awake()
    {
        foreach (var t in m_Toggles)
        {
            t.onChange.Add(new EventDelegate(OnToggleChange));
        }
    }

    public void OnToggleChange()
    {
        switch (style)
        {
            case ToggleChangeStyle.TextColor :
                OnTextColor();
                break;
            case ToggleChangeStyle.SpriteChange:
                OnChangeSprite();
                break;
             default:
                EB.Debug.LogError("no this style for toggle");
                 break;
        }
    }

    public void OnChangeSprite()
    {
        foreach (var t in m_Toggles)
        {
            if (t.value)
            {
               t.activeSprite.gameObject.SetActive(true);
            }
            else
            {
                t.activeSprite.gameObject.SetActive(false);
            }
        }
    }

    public void OnTextColor()
    {
        foreach (var t in m_Toggles)
        {
            if (t.value)
            {
                if (m_ChangeSpriteColor)
                {
                    t.GetComponent<UISprite>().color = m_PressColor;
                }
                else
                {
                    UILabel label = t.GetComponentInChildren<UILabel>();
                    if (label == null)
                        label = t.transform.parent.GetComponentInChildren<UILabel>();
                    if (label != null)
                        label.color = m_PressColor;
                }
            }
            else
            {
                if (m_ChangeSpriteColor)
                {
                    t.GetComponent<UISprite>().color = m_UnPressColor;
                }
                else
                {
                    UILabel label = t.GetComponentInChildren<UILabel>();
                    if (label == null)
                        label = t.transform.parent.GetComponentInChildren<UILabel>();
                    if (label != null)
                        label.color = m_UnPressColor;
                }
            }
        }
    }

    [ContextMenu("AddToggles")]
    public void AddToggles()
    {
        m_Toggles = new List<UIToggle>();
        m_Toggles.AddRange(transform.GetComponentsInChildren<UIToggle>());
    }
}
