//#define EBG_ADJUSTMENT_DEBUG
#define EBG_POSTFX_BLOOM
#define EBG_POSTFX_VIGNETTE
#define EBG_POSTFX_VIGNETTE2
#define EBG_POSTFX_WARP
#define EBG_POSTFX_COLOR_GRADE
#define EBG_POSTFX_RADIAL_BLUR

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RenderSettings : RenderSettingsBase
{
    /// <summary>
    /// 全局角色描边增幅
    /// </summary>
    public static float GlobalCharacterOutlineScale = 1;

    //WHEN ADDING OBJECTS, BE SURE TO UPDATE SHADERGLOBALSINSPECTOR AND CLONE() FUNCTION

    //Global Ambient light
    public Color GlobalLightAmbient = new Color (0.3f, 0.3f, 0.3f, 1.0f);

    //Global Character light
    public Color GlobalLightDiffuse = new Color (0.3f, 0.3f, 0.3f, 1.0f);
    public Color GlobalLightDiffuse1 = new Color (0.3f, 0.3f, 0.3f, 1.0f);
    public Color GlobalLightSpecular = new Color (0.3f, 0.3f, 0.3f, 1.0f);
    public Color GlobalCharColorScale = new Color (1.0f, 1.0f, 1.0f, 1.0f);
    public Vector3 GlobalLightRotation = new Vector3 (1.0f, 1.0f, 1.0f);
    public Vector3 GlobalLightRotation1 = new Vector3 (1.0f, 1.0f, 1.0f);
    public float CharactorOutlineScale = 1.0f;

    //Global Environment light
    public Color EnvGlobalLightDiffuse = new Color (0.3f, 0.3f, 0.3f, 1.0f);
    public Color EnvGlobalLightSpecular = new Color (0.3f, 0.3f, 0.3f, 1.0f);
    public Color EnvGlobalLightScale = new Color (1.0f, 1.0f, 1.0f, 1.0f);
    public Vector3 EnvGlobalLightRotation = new Vector3 (1.0f, 1.0f, 1.0f);

    public float EnvGlobalScale = 1.0f;

    public float CombatantScale = 1.0f;
    public float NonCombatantScale = 1.0f;
    public float IrrelevanceCombatantScale = 0.5f;


    //color filter
    public float CharactorContrast = 0.5f;
    public float CharactorBrightness = 0.0f;
    public float CharactorGrayScale = 1.0f;

    // color shift
    public Color CharactorFinalColor = new Color (1.0f, 1.0f, 1.0f, 0.0f);

    // specular
    public float CharactorSpecularIntensity = 1.0f;
    public float CharactorSpecularGloss = 2.0f;

    //Reflections
    public Cubemap EnvironmentCubemap = null;
    public Cubemap EnvironmentBlurryCubemap = null;

    //Planar reflections
    public PlanarReflectionManager.eCLEAR_MODE PlanarReflectionClearMode = PlanarReflectionManager.eCLEAR_MODE.Color;
    public Color PlanarReflectionCameraClearColor = new Color (0.0f, 0.0f, 0.0f, 0.0f);
    public float PlanarReflectionRamp = 3.0f;

    //Fog
    public Color FogColor = new Color (0.3f, 0.3f, 0.3f, 0.0f);
    public float FogDistStart = 5.0f;
    public float FogDistEnd = 50.0f;
    public float FogHeightStart = -1.0f;
    public float FogHeightEnd = 20.0f;

    //postfx
    public float PostFXBloomBlur = 2.0f;
    public float PostFXBloomRamp = 2.0f;
    public Color PostFXBloomColor = Color.black;
    public float PostFXBloomIntensity = 1.0f;
    public float PostFXVignetteIntensity = 0.0f;
    public Color PostFXVignetteColor = Color.white;
    public Vector2 PostFXWarpIntensity = Vector2.zero;
    public Texture3D PostFXColourGradeTexture = null;
    public Texture2D PostFXVignette2Mask = null;
    [Range(0.0f, 5.0f)]
    public float PostFXVignette2Intensity = 1f;
    public Color PostFXVignette2Color = Color.white;
    [Range(0.0f, 5.0f)]
    public float PostFXVignette2DarkIntensity = 1f;
    public Color PostFXVignette2MaskColor = Color.black;
    [Range(0.0f, 1.0f)]
    public float PostFXRadialBlurSampleDist = 0.17f;
    [Range(1.0f, 5.0f)]
    public float PostFXRadialBlurSampleStrength = 2.09f;

    public Color EnvGlobalSkyBoxColor = new Color(0,0,0,1);
    public Color EnvGlobalGlassColor = new Color(0,0,0,1);
    public Color EnvGlobalTColor = new Color(255, 255, 255, 1);

    //Reflection/Specular Adjustment
#if EBG_ADJUSTMENT_DEBUG
    public Color AdjustmentScale = Color.gray;
    public Color AdjustmentOffset = Color.black;
#endif
    public Color AdjustmentNoRelfScale = Color.gray;
    public Color AdjustmentNoRelfOffset = Color.black;
    public Color AdjustmentNoRelfSpecScale = Color.gray;
    public Color AdjustmentNoRelfSpecOffset = Color.black;

    // SR Hack for now, need to Speak to Russell about this for an editor flow
    // The problem is the ExecuteInEditMode..ideally we shouldn't have this attribute and the
    // move editor or whatever forces a refresh?
    public static bool forceUpdate = false;

    [System.NonSerialized]
    public static bool dontAdjust = false;

    [SerializeField]
    public GameObject[] particles;

    [SerializeField]
    public GameObject[] OtherShaderRenderers;

    public void Clone(RenderSettings toClone)
    {
        base.Clone(toClone);

        BlendInTime = toClone.BlendInTime;

        particles = toClone.particles;
        OtherShaderRenderers = toClone.OtherShaderRenderers;

        GlobalLightAmbient = toClone.GlobalLightAmbient;
        GlobalLightDiffuse = toClone.GlobalLightDiffuse;
        GlobalLightDiffuse1 = toClone.GlobalLightDiffuse1;
        GlobalLightSpecular = toClone.GlobalLightSpecular;
        GlobalLightRotation = toClone.GlobalLightRotation;
        GlobalLightRotation1 = toClone.GlobalLightRotation1;
        GlobalCharColorScale = toClone.GlobalCharColorScale;
        CharactorOutlineScale = toClone.CharactorOutlineScale;

        EnvGlobalLightDiffuse = toClone.EnvGlobalLightDiffuse;
        EnvGlobalLightSpecular = toClone.EnvGlobalLightSpecular;
        EnvGlobalLightRotation = toClone.EnvGlobalLightRotation;
        EnvGlobalLightScale = toClone.EnvGlobalLightScale;
        EnvGlobalGlassColor = toClone.EnvGlobalGlassColor;
        EnvGlobalSkyBoxColor = toClone.EnvGlobalSkyBoxColor;
        EnvGlobalTColor = toClone.EnvGlobalTColor;

        CharactorGrayScale = toClone.CharactorGrayScale;
        CharactorBrightness = toClone.CharactorBrightness;
        CharactorContrast = toClone.CharactorContrast;

        CharactorFinalColor = toClone.CharactorFinalColor;

        CharactorSpecularIntensity = toClone.CharactorSpecularIntensity;
        CharactorSpecularGloss = toClone.CharactorSpecularIntensity;

        EnvironmentCubemap = toClone.EnvironmentCubemap;
        EnvironmentBlurryCubemap = toClone.EnvironmentBlurryCubemap;

        LightDir = toClone.LightDir;

        FogColor = toClone.FogColor;
        FogDistStart = toClone.FogDistStart;
        FogDistEnd = toClone.FogDistEnd;
        FogHeightStart = toClone.FogHeightStart;
        FogHeightEnd = toClone.FogHeightEnd;

        PlanarReflectionClearMode = toClone.PlanarReflectionClearMode;
        PlanarReflectionCameraClearColor = toClone.PlanarReflectionCameraClearColor;
        PlanarReflectionRamp = toClone.PlanarReflectionRamp;

        PostFXBloomBlur = toClone.PostFXBloomBlur;
        PostFXBloomRamp = toClone.PostFXBloomRamp;
        PostFXBloomColor = toClone.PostFXBloomColor;
        PostFXBloomIntensity = toClone.PostFXBloomIntensity;
        PostFXVignetteIntensity = toClone.PostFXVignetteIntensity;
        PostFXVignetteColor = toClone.PostFXVignetteColor;
        PostFXWarpIntensity = toClone.PostFXWarpIntensity;
        PostFXColourGradeTexture = toClone.PostFXColourGradeTexture;
        PostFXVignette2Mask = toClone.PostFXVignette2Mask;
        PostFXVignette2Intensity = toClone.PostFXVignette2Intensity;
        PostFXVignette2Color = toClone.PostFXVignette2Color;
        PostFXVignette2DarkIntensity = toClone.PostFXVignette2DarkIntensity;
        PostFXVignette2MaskColor = toClone.PostFXVignette2MaskColor;
        PostFXRadialBlurSampleDist = toClone.PostFXRadialBlurSampleDist;
        PostFXRadialBlurSampleStrength = toClone.PostFXRadialBlurSampleStrength;

        AdjustmentNoRelfScale = toClone.AdjustmentNoRelfScale;
        AdjustmentNoRelfOffset = toClone.AdjustmentNoRelfOffset;
        AdjustmentNoRelfSpecScale = toClone.AdjustmentNoRelfSpecScale;
        AdjustmentNoRelfSpecOffset = toClone.AdjustmentNoRelfSpecOffset;

        EnvGlobalScale = toClone.EnvGlobalScale;
        CombatantScale = toClone.CombatantScale;
        NonCombatantScale = toClone.NonCombatantScale;
        IrrelevanceCombatantScale = toClone.IrrelevanceCombatantScale;
    }

    public override void ApplyAtSceneLoad()
    {
        //Global Character light
        UnityEngine.RenderSettings.ambientLight = GlobalLightAmbient;
        SetWaterRenderTranscy(WaterTranscy);
        //Global Character light
        Shader.SetGlobalColor("_EBGCharLightDiffuseColor0", GlobalLightDiffuse);
        Shader.SetGlobalColor("_EBGCharLightDiffuseColor1", GlobalLightDiffuse1 * 2.0f);
        Shader.SetGlobalColor("_EBGCharLightSpecularColor0", GlobalLightSpecular);
        Shader.SetGlobalColor("_EBGCharLightScale", GlobalCharColorScale);
        Vector3 charLightDirection = Quaternion.Euler (GlobalLightRotation) * Vector3.forward;
        Vector3 charLightDirection1 = Quaternion.Euler (GlobalLightRotation1) * Vector3.forward;
        Shader.SetGlobalVector("_EBGCharLightDirection0", charLightDirection.normalized);
        Shader.SetGlobalVector("_EBGCharDirectionToLight0", Vector3.zero - charLightDirection.normalized);
        Shader.SetGlobalVector("_EBGCharLightDirection1", charLightDirection1.normalized);
        Shader.SetGlobalVector("_EBGCharDirectionToLight1", Vector3.zero - charLightDirection1.normalized);
        Shader.SetGlobalFloat("_EBGCharLightProbeScale", LightProbeScale);
        Shader.SetGlobalFloat("_EBGCharOutlineScale", CharactorOutlineScale * GlobalCharacterOutlineScale);

        //Global Environment light
        Shader.SetGlobalColor("_EBGEnvLightDiffuseColor0", EnvGlobalLightDiffuse);
        Shader.SetGlobalColor("_EBGEnvLightSpecularColor0", EnvGlobalLightSpecular);
        Shader.SetGlobalColor("_EBGEnvLightColorScale", EnvGlobalLightScale);
        Vector3 envLightDirection = Quaternion.Euler (EnvGlobalLightRotation) * Vector3.forward;
        Shader.SetGlobalVector("_EBGEnvLightDirection0", envLightDirection.normalized);
        Shader.SetGlobalVector("_EBGEnvDirectionToLight0", Vector3.zero - envLightDirection.normalized);
        Shader.SetGlobalFloat("_EBGEnvScale", EnvGlobalScale);

        Shader.SetGlobalColor("_EBGTint", EnvGlobalSkyBoxColor);
        Shader.SetGlobalColor("_EBGGlassColor", EnvGlobalGlassColor);
        Shader.SetGlobalColor("_EBGTColor", EnvGlobalTColor);
        //Reflections
        if (EnvironmentCubemap != null)
        {
            Shader.SetGlobalTexture("_EBGCubemap", EnvironmentCubemap);
        }
        if (EnvironmentBlurryCubemap != null)
        {
            Shader.SetGlobalTexture("_EBGCubemapBlurry", EnvironmentBlurryCubemap);
        }

        //Fog
        Shader.SetGlobalColor("_EBGFogColor", FogColor);
        Vector4 FogParams;
        FogParams.x = FogDistStart;
        FogParams.y = 1.0f / (FogDistEnd - FogDistStart);
        FogParams.z = FogHeightEnd;
        FogParams.w = 1.0f / (FogHeightEnd - FogHeightStart);
        Shader.SetGlobalVector("_EBGFogParams", FogParams);

        //Planar
        if (PlanarReflectionManager.Initilized())
        {
            PlanarReflectionManager.Instance.SetPlanarReflectionRamp(PlanarReflectionRamp);
            PlanarReflectionManager.Instance.SetBackgroundColor(PlanarReflectionCameraClearColor);
            PlanarReflectionManager.Instance.SetClearMode(PlanarReflectionClearMode);
        }

        //DynamicPointLight
        if (!Application.isPlaying && !forceUpdate)
        {
            Shader.SetGlobalMatrix("_EBGPointLightColor", Matrix4x4.zero);
            Shader.SetGlobalMatrix("_EBGPointLightPosition", Matrix4x4.zero);
            Shader.SetGlobalVector("_EBGPointLightMultiplier", Vector4.zero);
            Shader.SetGlobalVector("EBGPointLightIntensity", Vector4.one);
        }

        if ((Application.isPlaying || forceUpdate) && PostFXManager.Instance != null)
        {
            var postfx = PostFXManager.Instance;
#if EBG_POSTFX_VIGNETTE2
            postfx.SetVignette2(PostFXVignette2Mask, PostFXVignette2MaskColor, PostFXVignette2Color, PostFXVignette2Intensity, PostFXVignette2DarkIntensity);
#endif
#if EBG_POSTFX_COLOR_GRADE
            postfx.SetColorGradeTexture(PostFXColourGradeTexture);
#endif
        }
    }

    public override void ApplyEveryFrame()
    {
        //Adjustments
        RenderGlobals.eADJUSTMENT_MODE adjustmentMode = RenderGlobals.EnvironmentAdjustmentMode;
        Color adjustmentScale = RenderGlobals.AdjustmentScale;
        Color adjustmentOffset = RenderGlobals.AdjustmentOffset;

#if EBG_ADJUSTMENT_DEBUG
        if (adjustmentMode == RenderGlobals.eADJUSTMENT_MODE.eADJUSTMENT_MODE.NONE)
        {
            adjustmentScale.r *= AdjustmentScale.r * 2.0f;
            adjustmentScale.g *= AdjustmentScale.g * 2.0f;
            adjustmentScale.b *= AdjustmentScale.b * 2.0f;
            adjustmentOffset += AdjustmentOffset;
        }
#endif
        if (adjustmentMode == RenderGlobals.eADJUSTMENT_MODE.NO_REFLECTIONS)
        {
            adjustmentScale.r *= AdjustmentNoRelfScale.r * 2.0f;
            adjustmentScale.g *= AdjustmentNoRelfScale.g * 2.0f;
            adjustmentScale.b *= AdjustmentNoRelfScale.b * 2.0f;
            adjustmentOffset += AdjustmentNoRelfOffset;
        }
        else if (adjustmentMode == RenderGlobals.eADJUSTMENT_MODE.NO_REFLECTIONS_OR_SPECULAR)
        {
            adjustmentScale.r *= AdjustmentNoRelfSpecScale.r * 2.0f;
            adjustmentScale.g *= AdjustmentNoRelfSpecScale.g * 2.0f;
            adjustmentScale.b *= AdjustmentNoRelfSpecScale.b * 2.0f;
            adjustmentOffset += AdjustmentNoRelfSpecOffset;
        }

        if (dontAdjust)
        {
            Shader.SetGlobalVector("_EBGEnvAdjustScale", new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            Shader.SetGlobalVector("_EBGEnvAdjustOffset", Vector4.zero);
        }
        else
        {
            Shader.SetGlobalVector("_EBGEnvAdjustScale", new Vector4(adjustmentScale.r * 2.0f, adjustmentScale.g * 2.0f, adjustmentScale.b * 2.0f, 1.0f));
            Shader.SetGlobalVector("_EBGEnvAdjustOffset", new Vector4(adjustmentOffset.r, adjustmentOffset.g, adjustmentOffset.b, 0.0f));
        }

        Shader.SetGlobalVector("_Gyroscope", Input.gyro.gravity);

        if ((Application.isPlaying || forceUpdate) && PostFXManager.Instance != null)
        {
            var postfx = PostFXManager.Instance;
#if EBG_POSTFX_BLOOM
            postfx.SetBloomBlur(Mathf.Lerp(PostFXBloomBlur, RenderGlobals.PostFXBloomBlur, RenderGlobals.PostFXBloomMix));
            postfx.SetBloomRamp(Mathf.Lerp(PostFXBloomRamp, RenderGlobals.PostFXBloomRamp, RenderGlobals.PostFXBloomMix));
            postfx.SetBloomColors(Color.Lerp(PostFXBloomColor, RenderGlobals.PostFXBloomColor, RenderGlobals.PostFXBloomMix), Color.black);
            postfx.SetBloomIntensity(Mathf.Lerp(PostFXBloomIntensity, RenderGlobals.PostFXBloomIntensity, RenderGlobals.PostFXBloomMix));
#endif
#if EBG_POSTFX_VIGNETTE
            postfx.SetVignetteIntensity(Mathf.Lerp(PostFXVignetteIntensity, RenderGlobals.PostFXVignetteIntensity, RenderGlobals.PostFXVignetteMix));
            postfx.SetVignetteColor(Color.Lerp(PostFXVignetteColor, RenderGlobals.PostFXVignetteColor, RenderGlobals.PostFXVignetteMix));
#endif
#if EBG_POSTFX_WARP
            postfx.SetWarpIntensity(Vector2.Lerp(PostFXWarpIntensity, RenderGlobals.PostFXWarpIntensity, RenderGlobals.PostFXWarpMix));
#endif
#if EBG_POSTFX_RADIAL_BLUR
            postfx.SetRadialBlur(Mathf.Lerp(PostFXRadialBlurSampleDist, RenderGlobals.PostFXRadialBlurSampleDist, RenderGlobals.PostFXRadialBlurMix), PostFXRadialBlurSampleStrength);
#endif
        }
    }

    public void Awake()
    {
        if (RenderSettingsManager.Instance != null)
        {
            RenderSettingsManager.Instance.Register(this);
        }
    }

    public void OnDestroy()
    {
        if (!RenderSettingsManager.Ignore(this))
        {
            RenderSettingsManager.Instance.UnRegister(this);
        }
    }

    public override void Start()
    {
        base.Start();
        SetCameraBackground();
        GetWaterRenderSetting();
        GetSceneFX();
        GetSceneRenderInLegacyShader();
#if UNITY_EDITOR
        SetupBlackTexture();
#endif
        if (StartActive)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                RenderSettingsManager.Instance.SetActiveRenderSettings(name);
            }
            else
            {
                RenderSettingsManager.Instance.SetActiveRenderSettings(this);
            }
#else
            RenderSettingsManager.Instance.SetActiveRenderSettings(name);
#endif
        }
    }

    public StylizedWater.StylizedWater Water;
    [Range(0,1)]
    public float WaterTranscy = 0f;
    private void GetWaterRenderSetting()
    {
        Transform parent = transform.parent;
        if (parent)
        {
            Water = parent.GetComponentInChildren<StylizedWater.StylizedWater>();
            if (Water)
            {
                WaterTranscy = Water.transparency;
            }
        }
    }

    /// <summary>
    /// 改变相机背景色为黑色
    /// </summary>
    private void SetCameraBackground()
    {
        if (Camera.main)
        {
            Camera.main.backgroundColor = Color.black;
        }
    }

    /// <summary>
    /// 显示/隐藏场景中的其他非透明物体
    /// </summary>
    /// <param name="needShow"></param>
    public void ShowOrHideOtherShaderRenderers(bool needShow)
    {
        if (OtherShaderRenderers != null && OtherShaderRenderers.Length != 0)
        {
            for (int i = 0; i < OtherShaderRenderers.Length; i++)
            {
                if (OtherShaderRenderers[i])
                {
                    OtherShaderRenderers[i].SetActive(needShow);
                    Animator anim = OtherShaderRenderers[i].GetComponent<Animator>();
                    if (anim)
                    {
                        anim.enabled = needShow;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 激活/关闭场景特效
    /// </summary>
    /// <param name="needPlayFX"></param>
    public void StartOrStopSceneFX(bool needPlayFX)
    {
        if (particles != null && particles.Length != 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem fx = particles[i].GetComponent<ParticleSystem>();
                if (fx)
                {
                    fx.gameObject.SetActive(needPlayFX);
                    if (needPlayFX)
                    {
                        fx.Play();
                    }
                    else
                    {
                        fx.Stop();
                    }
                }
            }
        }
    }


    /// <summary>
    /// 获取场景特效资源
    /// </summary>
    private void GetSceneFX()
    {
        particles = GameObject.FindGameObjectsWithTag("CombatSceneFX");
    }

    /// <summary>
    /// 获取场景使用其他非透明shader物体
    /// </summary>
    public void GetSceneRenderInLegacyShader()
    {
        OtherShaderRenderers = GameObject.FindGameObjectsWithTag("CombatSceneOtherShader");
    }

    /// <summary>
    /// 设置水流shader的透明度
    /// </summary>
    /// <param name="transparent"></param>
    public void SetWaterRenderTranscy(float transparent)
    {
        if (Water)
        {
            Water.transparency = transparent;
            Water.SetProperties();
        }
    }

#if UNITY_EDITOR
    Texture2D blackTexture;
    void SetupBlackTexture()
    {
        blackTexture = new Texture2D(1, 1);
        blackTexture.SetPixels(new Color[] { Color.black });
        blackTexture.Apply();
    }

    void ApplyBlackTexture()
    {
        Shader.SetGlobalTexture("_PlanarReflectionTex", blackTexture);
    }
#endif

    public override void ApplyBlend(RenderSettingsBase source, RenderSettingsBase dest, float destFactor)
    {
        //EB.Debug.Log ("BLEND: " + destFactor);

        RenderSettings RSdest = (RenderSettings)dest;
        RenderSettings RSsource = (RenderSettings)source;

        LightProbeScale = Mathf.Lerp(RSsource.LightProbeScale, RSdest.LightProbeScale, destFactor);
        LightProbeOffset = Mathf.Lerp(RSsource.LightProbeOffset, RSdest.LightProbeOffset, destFactor);
        LightProbeMax = Mathf.Lerp(RSsource.LightProbeMax, RSdest.LightProbeMax, destFactor);

        GlobalLightAmbient = Color.Lerp(RSsource.GlobalLightAmbient, RSdest.GlobalLightAmbient, destFactor);
        GlobalLightDiffuse = Color.Lerp(RSsource.GlobalLightDiffuse, RSdest.GlobalLightDiffuse, destFactor);
        GlobalLightDiffuse1 = Color.Lerp(RSsource.GlobalLightDiffuse1, RSdest.GlobalLightDiffuse1, destFactor);
        GlobalLightSpecular = Color.Lerp(RSsource.GlobalLightSpecular, RSdest.GlobalLightSpecular, destFactor);
        GlobalCharColorScale = Color.Lerp(RSsource.GlobalCharColorScale, RSdest.GlobalCharColorScale, destFactor);
        GlobalLightRotation = Vector3.Lerp(RSsource.GlobalLightRotation, RSdest.GlobalLightRotation, destFactor);
        GlobalLightRotation1 = Vector3.Lerp(RSsource.GlobalLightRotation1, RSdest.GlobalLightRotation1, destFactor);
        CharactorOutlineScale = Mathf.Lerp(RSsource.CharactorOutlineScale, RSdest.CharactorOutlineScale, destFactor);

        LightDir = RSdest.LightDir;

        CharactorGrayScale = Mathf.Lerp(RSsource.CharactorGrayScale, RSdest.CharactorGrayScale, destFactor);
        CharactorBrightness = Mathf.Lerp(RSsource.CharactorBrightness, RSdest.CharactorBrightness, destFactor);
        CharactorContrast = Mathf.Lerp(RSsource.CharactorContrast, RSdest.CharactorContrast, destFactor);

        CharactorFinalColor = Color.Lerp(RSsource.CharactorFinalColor, RSdest.CharactorFinalColor, destFactor);

        CharactorSpecularIntensity = Mathf.Lerp(RSsource.CharactorSpecularIntensity, RSdest.CharactorSpecularIntensity, destFactor);
        CharactorSpecularGloss = Mathf.Lerp(RSsource.CharactorSpecularGloss, RSdest.CharactorSpecularGloss, destFactor);


        EnvGlobalLightDiffuse = Color.Lerp(RSsource.EnvGlobalLightDiffuse, RSdest.EnvGlobalLightDiffuse, destFactor);
        EnvGlobalLightSpecular = Color.Lerp(RSsource.EnvGlobalLightSpecular, RSdest.EnvGlobalLightSpecular, destFactor); ;
        EnvGlobalLightScale = Color.Lerp(RSsource.EnvGlobalLightScale, RSdest.EnvGlobalLightScale, destFactor); ;
        EnvGlobalLightRotation = Vector3.Lerp(RSsource.EnvGlobalLightRotation, RSdest.EnvGlobalLightRotation, destFactor);

        EnvGlobalSkyBoxColor = Color.Lerp(RSsource.EnvGlobalSkyBoxColor, RSdest.EnvGlobalSkyBoxColor, destFactor);
        EnvGlobalGlassColor = Color.Lerp(RSsource.EnvGlobalGlassColor, RSdest.EnvGlobalGlassColor, destFactor);
        EnvGlobalTColor = Color.Lerp(RSsource.EnvGlobalTColor, RSdest.EnvGlobalTColor, destFactor);

        EnvironmentCubemap = RSdest.EnvironmentCubemap;
        EnvironmentBlurryCubemap = RSdest.EnvironmentBlurryCubemap;

        FogColor = Color.Lerp(RSsource.FogColor, RSdest.FogColor, destFactor);
        FogDistStart = Mathf.Lerp(RSsource.FogDistStart, RSdest.FogDistStart, destFactor);
        FogDistEnd = Mathf.Lerp(RSsource.FogDistEnd, RSdest.FogDistEnd, destFactor);
        FogHeightStart = Mathf.Lerp(RSsource.FogHeightStart, RSdest.FogHeightStart, destFactor);
        FogHeightEnd = Mathf.Lerp(RSsource.FogHeightEnd, RSdest.FogHeightEnd, destFactor);

        PlanarReflectionClearMode = RSdest.PlanarReflectionClearMode;
        PlanarReflectionCameraClearColor = RSdest.PlanarReflectionCameraClearColor;
        PlanarReflectionRamp = RSdest.PlanarReflectionRamp;

        PostFXBloomBlur = Mathf.Lerp(RSsource.PostFXBloomBlur, RSdest.PostFXBloomBlur, destFactor);
        PostFXBloomRamp = Mathf.Lerp(RSsource.PostFXBloomRamp, RSdest.PostFXBloomRamp, destFactor);
        PostFXBloomColor = Color.Lerp(RSsource.PostFXBloomColor, RSdest.PostFXBloomColor, destFactor);
        PostFXBloomIntensity = Mathf.Lerp(RSsource.PostFXBloomIntensity, RSdest.PostFXBloomIntensity, destFactor);
        PostFXVignetteIntensity = Mathf.Lerp(RSsource.PostFXVignetteIntensity, RSdest.PostFXVignetteIntensity, destFactor);
        //EB.Debug.Log (RSsource.PostFXVignetteColor);
        PostFXVignetteColor = Color.Lerp(RSsource.PostFXVignetteColor, RSdest.PostFXVignetteColor, destFactor);
        PostFXWarpIntensity = Vector2.Lerp(RSsource.PostFXWarpIntensity, RSdest.PostFXWarpIntensity, destFactor);
        PostFXColourGradeTexture = RSdest.PostFXColourGradeTexture;
        if (RSdest.PostFXVignette2Mask != null || RSdest.PostFXVignette2Intensity == 0.0f)
        {
            PostFXVignette2MaskColor = Color.Lerp(RSsource.PostFXVignette2MaskColor, RSdest.PostFXVignette2MaskColor, destFactor);
            PostFXVignette2Intensity = Mathf.Lerp(RSsource.PostFXVignette2Intensity, RSdest.PostFXVignette2Intensity, destFactor);
            PostFXVignette2DarkIntensity = Mathf.Lerp(RSsource.PostFXVignette2DarkIntensity, RSdest.PostFXVignette2DarkIntensity, destFactor);
            PostFXVignette2Color = Color.Lerp(RSsource.PostFXVignette2Color, RSdest.PostFXVignette2Color, destFactor);
            PostFXVignette2Mask = RSdest.PostFXVignette2Mask;
        }
        else
        {
            PostFXVignette2Mask = null;
            PostFXVignette2Color = Color.white;
            PostFXVignette2DarkIntensity = 1.0f;
            PostFXVignette2Intensity = 1.0f;
            PostFXVignette2MaskColor = Color.black;
        }

        AdjustmentNoRelfScale = RSdest.AdjustmentNoRelfScale;
        AdjustmentNoRelfOffset = RSdest.AdjustmentNoRelfOffset;
        AdjustmentNoRelfSpecScale = RSdest.AdjustmentNoRelfSpecScale;
        AdjustmentNoRelfSpecOffset = RSdest.AdjustmentNoRelfSpecOffset;

        EnvGlobalScale = Mathf.Lerp(RSsource.EnvGlobalScale, RSdest.EnvGlobalScale, destFactor);
        CombatantScale = Mathf.Lerp(RSsource.CombatantScale, RSdest.CombatantScale, destFactor);
        NonCombatantScale = Mathf.Lerp(RSsource.NonCombatantScale, RSdest.NonCombatantScale, destFactor);
        IrrelevanceCombatantScale = Mathf.Lerp(RSsource.IrrelevanceCombatantScale, RSdest.IrrelevanceCombatantScale, destFactor);
        WaterTranscy = Mathf.Lerp(RSsource.WaterTranscy, RSdest.WaterTranscy, destFactor);
    }
}
