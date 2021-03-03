using UnityEngine;
using System.Collections;

public class FusionUIPlaySound : MonoBehaviour
{
    public enum Trigger
    {
        OnClick,
        OnMouseOver,
        OnMouseOut,
        OnPress,
        OnRelease,
        Custom,
        OnEnable,
        OnDisable,
    }

    public string fabricEvent = defaultSoundEvent;
    public Trigger trigger = Trigger.OnClick;

    bool mIsOver = false;

    const string defaultSoundEvent = "UI/General/ButtonClick";

    bool canPlay
    {
        get
        {
            if (!enabled) return false;
            UIButton btn = GetComponent<UIButton>();
            return (btn == null || btn.isEnabled);
        }
    }

    void PlaySound(string fabricEvent)
    {
        FusionAudio.PostEvent(fabricEvent, true);
    }

    void PlaySound()
    {
        PlaySound(fabricEvent);
    }

    void OnEnable()
    {
        if (trigger == Trigger.OnEnable)
            PlaySound();
    }

    void OnDisable()
    {
        if (trigger == Trigger.OnDisable)
            PlaySound();
    }

    void OnHover(bool isOver)
    {
        if (trigger == Trigger.OnMouseOver)
        {
            if (mIsOver == isOver) return;
            mIsOver = isOver;
        }

        if (canPlay && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
            PlaySound();
    }

    void OnPress(bool isPressed)
    {
        if (trigger == Trigger.OnPress)
        {
            if (mIsOver == isPressed) return;
            mIsOver = isPressed;
        }

        if (canPlay && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
            PlaySound();
    }

    void OnClick()
    {
        if (canPlay && trigger == Trigger.OnClick)
            PlaySound();
    }

    void OnSelect(bool isSelected)
    {
        if (canPlay && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
            OnHover(isSelected);
    }
}
