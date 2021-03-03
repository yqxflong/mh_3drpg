using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectClip : MonoBehaviour
{
    UIPanel mPanel;//遮挡容器，即ScrollView
    Transform mRoot;//UI的根
    float mHalfWidth, mHalfHeight, mRootScale;

    [HideInInspector] public bool HasInitialized;

    private MaterialPropertyBlock _matPropBlock;
    private int _matVectorID;

    public void Init()
    {
        _matPropBlock = new MaterialPropertyBlock();
        _matVectorID = Shader.PropertyToID("_Area");

        //return;
        mPanel = gameObject.GetComponentInParent<UIPanel>();
        mRoot = GameObject.Find("MainHudUI").transform;

        if (mPanel == null || mPanel.clipping != UIDrawCall.Clipping.SoftClip)
        {
            //不是滚动的item不需要做特效剪裁
            return;
        }

        //获取UI的scale，容器的宽高的一半的值
        mRootScale = mRoot.localScale.x;
        mHalfWidth = mPanel.GetViewSize().x * 0.5f * mRootScale;
        mHalfHeight = mPanel.GetViewSize().y * 0.5f * mRootScale;

        //给shader的容器坐标变量_Area赋值
        Vector4 area = CalculateArea(GetPostion());

        //获取所有需要修改shader的material，并替换shader
        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        var len = particleSystems.Length;

        for (int i = 0; i < len; i++)
        {
            var render = particleSystems[i].GetComponent<Renderer>();
            var mat = render.material;

            //Debug.LogErrorFormat("EffectClip Material Name = {0}", mat.name);
            if (mat == null)
            {
                continue;
            }

            if (mat.shader.name.Contains("Scroll2LayersSineAlphaAdd"))//特写流光uv特效，有更好方法后再替换
            {
                mat.shader = Shader.Find("Custom/Scaler/Particles/Scroll2LayersSineAlphaAdd1");
            }
            else
            {
                mat.shader = Shader.Find("EBG/Particle/Add 1");
            }

            //mat.SetVector("_Area", area);
            render.GetPropertyBlock(_matPropBlock);
            _matPropBlock.SetVector(_matVectorID, area);
            render.SetPropertyBlock(_matPropBlock);
            HasInitialized = true;
        }
    }

    //计算容器在世界坐标的Vector4，xz为左右边界的值，yw为下上边界值
    Vector4 CalculateArea(Vector3 position)
    {
        return new Vector4()
        {
            x = position.x - mHalfWidth,
            y = position.y - mHalfHeight,
            z = position.x + mHalfWidth,
            w = position.y + mHalfHeight
        };
    }

    Vector3 GetPostion()
    {
        var tempPos = mPanel.transform.localPosition;
        tempPos.x += mPanel.clipOffset.x + mPanel.baseClipRegion.x;
        tempPos.y += mPanel.clipOffset.y + mPanel.baseClipRegion.y;
        GameObject tempObj = new GameObject();
        tempObj.transform.SetParent(mPanel.transform.parent);
        tempObj.transform.localPosition = tempPos;
        tempObj.transform.localEulerAngles = Vector3.zero;
        tempObj.transform.localScale = Vector3.one;
        var pos = tempObj.transform.position;
        Destroy(tempObj);

        return pos;
    }

}
