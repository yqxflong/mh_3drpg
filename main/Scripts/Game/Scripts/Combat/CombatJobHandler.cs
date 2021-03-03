

using UnityEngine;
using UnityEngine.Jobs;

public struct JobUpdateHealthBarPosition : IJobParallelForTransform
{
    public Vector3 screen_point;
    public Vector3 m_screenOffset;

    public void Execute(int index, TransformAccess transform)
    {
        UIRoot uiroot = UIRoot.list[0];
        bool fitHeight = uiroot.fitHeight;
        int manualHeight = uiroot.manualHeight;
        int manualWidth = uiroot.manualWidth;

        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        screen_point += m_screenOffset;

        float screenScale = 1.0f;

        if ((float)Screen.width / Screen.height > (float)manualWidth / manualHeight)
        {
            screenScale = manualHeight / screenHeight;
        }
        else
        {
            screenScale = manualWidth / screenWidth;
        }

        screen_point.x -= screenWidth / 2;
        screen_point.y -= screenHeight / 2;
        screen_point *= screenScale;

        transform.localPosition = screen_point;

    }
}


public struct JobUpdateRotation : IJobParallelForTransform
{
    public float lerp_time;
    public Quaternion m_fromRotation;
    public Quaternion m_toRotation;

    public void Execute(int index, TransformAccess transform)
    {
        lerp_time = Mathf.Clamp(lerp_time, 0.0f, 1.0f);
        Quaternion local_rotation = Quaternion.Slerp(m_fromRotation, m_toRotation, lerp_time);
        if (!float.IsNaN(local_rotation.x))
        {
            transform.localRotation = local_rotation;
        }
    }
}



public struct JobUpdateShadowPlane : IJobParallelForTransform
{
    public float m_shadowPlaneY;

    public void Execute(int index, TransformAccess transform)
    {
        Vector3 position = transform.position;
        position.y = m_shadowPlaneY;
        transform.position = position;
    }
}

public struct JobUpdateSetHealthBar : IJobParallelForTransform
{
    public int m_currentHP;
    public int m_maxHP;

    public float m_targetHpValue;

    public void Execute(int index, TransformAccess transform)
    {
        float hpPercentageValue = Mathf.Clamp01((float)m_currentHP / m_maxHP);
        // anything under this minimum is not visible in our health bar
        const float hpMin = 0.0175f;
        hpPercentageValue = hpMin + hpPercentageValue * (1.0f - hpMin);
        //EB.Debug.Log("hpPercentageValue: " +hpPercentageValue);
        m_targetHpValue = 1 - hpPercentageValue;
    }
}