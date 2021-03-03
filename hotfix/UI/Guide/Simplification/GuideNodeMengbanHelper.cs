using UnityEngine;
using System.Collections;
using System;


namespace Hotfix_LT.UI
{
    public class GuideNodeMengbanHelper : DynamicMonoHotfix
    {
        public Material mengbanMat;
        RenderTexture rtTempA;
        RenderTexture rtTempB;

        public override void Awake()
        {
            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                mengbanMat = (Material)mDMono.ObjectParamList[0];
            }           
        }
        private float x, y, w, h;
        public void Set(float uirootW, float uirootH, float x, float y, float width, float height)
        {
            this.x = x / Screen.width;
            this.y = y / Screen.height;
            this.w = width / uirootW;
            this.h = height / uirootH;
        }
        public void Mix(Action<RenderTexture> callback)
        {
            if (rtTempA == null)
            {
                rtTempA = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
            }
            if (rtTempB == null)
            {
                rtTempB = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
            }
            rtTempA.filterMode = FilterMode.Bilinear;
            rtTempB.filterMode = FilterMode.Bilinear;
            Vector4 v = new Vector4(x - w / 2, y + h / 2, w, h);
            if (mengbanMat != null)
            {
                mengbanMat.SetVector("_MengBanTransparent", v);
                mengbanMat.SetFloat("_MengBanScreenHWRatio", ((float)Screen.height) / Screen.width);
                mengbanMat.SetFloat("_MengBanRadius", 0.02f);
                mengbanMat.SetFloat("_MengBanAlphaMax", 0.6f);
                Graphics.Blit(rtTempA, rtTempB, mengbanMat);
            }
            callback(rtTempB);
        }

        public override void OnDestroy()
        {
            if (rtTempA != null)
            {
                UnityEngine.Object.DestroyImmediate(rtTempA);
            }
            if (rtTempB != null)
            {
                UnityEngine.Object.DestroyImmediate(rtTempB);
            }
        }
    }
}