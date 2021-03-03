using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 创建通用特效（物品品质特效、SSR属性特效）
    /// </summary>
    public static class HotfixCreateFX
    {
        private static Dictionary<string, ParticleSystem> mQualityFXDic;

        public static void ShowItemQualityFX(ParticleSystemUIComponent qualityFX, EffectClip effectClip, Transform parentTf, int qualityLevel)
        {
            if (qualityLevel < 5)
            {
                if (qualityFX != null)
                {
                    qualityFX.gameObject.CustomSetActive(false);
                }
                else
                {
                    if (parentTf.Find("QualityFX") != null)
                    {
                        parentTf.Find("QualityFX").gameObject.CustomSetActive(false);
                    }
                }
                return;
            }

            if (qualityFX == null)
            {
                if (parentTf.Find("QualityFX") == null)
                {
                    GameObject obj = new GameObject("QualityFX");
                    obj.transform.SetParent(parentTf);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localEulerAngles = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    effectClip = obj.AddComponent<EffectClip>();
                    qualityFX = obj.AddComponent<ParticleSystemUIComponent>();

                    UIPanel panel = parentTf.GetComponentInParentEx<UIPanel>();

                    if (panel != null)
                    {
                        qualityFX.panel = panel;
                    }

                    qualityFX.sortingOrderOffset = 1;
                    NGUITools.SetLayer(qualityFX.gameObject, parentTf.gameObject.layer);
                }
                else
                {
                    qualityFX = parentTf.Find("QualityFX").GetComponent<ParticleSystemUIComponent>();
                    effectClip = parentTf.Find("QualityFX").GetComponent<EffectClip>();
                }
            }

            qualityFX.gameObject.CustomSetActive(true);
            string fxName = qualityLevel == 5 ? "DJ_pingjie5_jin_FX" : "DJ_pingjie6_red_FX";
            qualityFX.fx = GetQualityFx(parentTf, fxName);
            qualityFX.Play();
            effectClip.Init();
        }

        public static ParticleSystemUIComponent ShowCharTypeFX(ParticleSystemUIComponent typeFX, EffectClip effectClip, Transform parentTf, PartnerGrade grade, Hotfix_LT.Data.eRoleAttr charType, int waitFrame = 0)
        {
            if ((int)grade < (int)PartnerGrade.SSR)
            {
                if (typeFX != null)
                {
                    typeFX.gameObject.CustomSetActive(false);
                }
                else
                {
                    if (parentTf.Find("TypeFX") != null)
                    {
                        parentTf.Find("TypeFX").gameObject.CustomSetActive(false);
                    }
                }
                return typeFX;
            }

            if (typeFX == null)
            {
                if (parentTf.Find("TypeFX") == null)
                {
                    GameObject obj = new GameObject("TypeFX");
                    obj.transform.SetParent(parentTf);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localEulerAngles = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    effectClip = obj.AddComponent<EffectClip>();
                    typeFX = obj.AddComponent<ParticleSystemUIComponent>();

                    UIPanel panel = parentTf.GetComponentInParentEx<UIPanel>();

                    if (panel != null)
                    {
                        typeFX.panel = panel;
                    }

                    typeFX.sortingOrderOffset = 1;
                    NGUITools.SetLayer(typeFX.gameObject, parentTf.gameObject.layer);
                }
                else
                {
                    typeFX = parentTf.Find("TypeFX").GetComponent<ParticleSystemUIComponent>();
                    effectClip = parentTf.Find("TypeFX").GetComponent<EffectClip>();
                }
            }

            typeFX.gameObject.CustomSetActive(false);
            string fxName = null;
            switch (charType)
            {
                case Hotfix_LT.Data.eRoleAttr.Shui:
                    {
                        fxName = "fx_ui_touxiang_ShuiFX";
                    }; break;
                case Hotfix_LT.Data.eRoleAttr.Huo:
                    {
                        fxName = "fx_ui_touxiang_HuoFX";
                    }; break;
                case Hotfix_LT.Data.eRoleAttr.Feng:
                    {
                        fxName = "fx_ui_touxiang_FengFX";
                    }; break;
            }
            typeFX.needFXScaleMode = true;
            typeFX.fx = GetQualityFx(parentTf, fxName);
            typeFX.WaitFrame = waitFrame;
            typeFX.playOnEnable = true;
            typeFX.stopOnDisable = true;
            typeFX.gameObject.CustomSetActive(true);

            return typeFX;
        }

        public static ParticleSystemUIComponent ShowUpgradeQualityFX(ParticleSystemUIComponent Fx,Transform parentTf, int Quality, EffectClip effectClip = null,int waitFrame = 0)
        {
            if (Quality < 6)
            {
                if (Fx != null)
                {
                    Fx.gameObject.CustomSetActive(false);
                }
                else
                {
                    if (parentTf.Find("Fx") != null)
                    {
                        parentTf.Find("Fx").gameObject.CustomSetActive(false);
                    }
                }
                return Fx;
            }
            if (Fx == null)
            {
                if (parentTf.Find("Fx") == null)
                {
                    GameObject obj = new GameObject("Fx");
                    obj.transform.SetParent(parentTf);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localEulerAngles = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    effectClip = obj.AddComponent<EffectClip>();
                    Fx = obj.AddComponent<ParticleSystemUIComponent>();

                    UIPanel panel = parentTf.GetComponentInParentEx<UIPanel>();

                    if (panel != null)
                    {
                        Fx.panel = panel;
                    }

                    Fx.sortingOrderOffset = 1;
                    NGUITools.SetLayer(Fx.gameObject, parentTf.gameObject.layer);
                }
                else
                {
                    Fx = parentTf.Find("Fx").GetComponent<ParticleSystemUIComponent>();
                    effectClip = parentTf.Find("Fx").GetComponent<EffectClip>();
                }
            }

            Fx.gameObject.CustomSetActive(false);
            string fxName;
            switch (Quality)
            {
                case 6://炫彩
                        fxName = "fx_t_hb_xuancai_uv";
                   break;
                default:
                    fxName = "fx_t_hb_xuancai_uv";
                    break;
            }
            Fx.needFXScaleMode = false;
            Fx.fx = GetQualityFx(parentTf, fxName);
            Fx.WaitFrame = waitFrame;
            Fx.playOnEnable = true;
            Fx.stopOnDisable = true;
            Fx.gameObject.CustomSetActive(true);
            //Fx.Play();
            return Fx;
        }

        private static ParticleSystem GetQualityFx(Object m_Obj, string fxName)
        {
            if (mQualityFXDic == null)
            {
                mQualityFXDic = new Dictionary<string, ParticleSystem>();
            }

            if (mQualityFXDic.ContainsKey(fxName))
            {
                if (mQualityFXDic[fxName] == null)
                {
                    ParticleSystem ps = PSPoolManager.Instance.Use(m_Obj, fxName);
                    mQualityFXDic[fxName] = ps;
                }
            }
            else
            {
                ParticleSystem ps = PSPoolManager.Instance.Use(m_Obj, fxName);
                mQualityFXDic.Add(fxName, ps);
            }

            return mQualityFXDic[fxName];
        }
    }

    public static class HotfixIlrExtension
    {
        public static T GetILRComponent<T>(this UIControllerILR ilr) where T : Component
        {
            T ilinstance = ilr.ilinstance as T;
            if (ilinstance == null)
            {
                ilr.ILRObjInit();
                ilinstance = ilr.ilinstance as T;
            }
            return ilinstance;
        }

        public static T GetILRComponent<T>(this DynamicMonoILR ilr) where T : Component
        {
            T ilinstance = ilr._ilrObject as T;
            if (ilinstance == null)
            {
                ilr.ILRObjInit();
                ilinstance = ilr._ilrObject as T;
            }
            return ilinstance;
        }
    }
}