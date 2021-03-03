//#define USE_GET_TEMPORARY
#define EBG_POSTFX_BLOOM
#define EBG_POSTFX_VIGNETTE
#define EBG_POSTFX_VIGNETTE2
#define EBG_POSTFX_WARP
#define EBG_POSTFX_TONE_MAP
#define EBG_POSTFX_COLOR_GRADE
#define EBG_POSTFX_RADIAL_BLUR

using UnityEngine;
using System.Collections.Generic;
using System;

public class PostFXManager : MonoBehaviour
{
	private enum POSTFX_KEYWORDS
	{

	}

	private class PostFXComparer : IEqualityComparer<PerformanceInfo.ePOSTFX>
	{
		public bool Equals(PerformanceInfo.ePOSTFX x, PerformanceInfo.ePOSTFX y)
		{
			return x == y;
		}

		public int GetHashCode(PerformanceInfo.ePOSTFX obj)
		{
			return (int)obj;
		}

		public static PostFXComparer Default = new PostFXComparer();
	}

	private bool			_paused = false;
	private bool			_inited = false;
	public HashSet<PerformanceInfo.ePOSTFX> CurrentPostFX { get; private set; }
	public PerformanceInfo.ePOSTFX_QUALITY Quality { get; private set; }

	//Composite
	private Shader			_CompositeShader;
	private Material		_CompositeMaterial;

#if EBG_POSTFX_BLOOM
	//Blur
	private Shader			_GaussianBlurShader;
	private Material		_GaussianBlurMaterial;

	//Bloom
	private const string 	_BloomTextureName = "_BloomTex";
	private const string	_BloomColorName = "_PostFXBloomColor";
	private const string    _BloomIntensityName = "_PostFXBloomIntensity";
	private const string    _BloomOverbrightColorName = "_OrignScreenColor";

	private Color			_BloomColor = Color.black;
	private Color			_BloomOverbrightColor = Color.black;
	private float			_BloomIntensity = 1.0f;
	private float			_BloomRamp = 1.0f;
	private int[][] 		_BloomBlurWidth = new int[2][];
	private int[][]			_BloomBlurHeight = new int[2][];
	private RenderTexture[] _BloomBlurTextures;
	private float 			_BloomBlurSigma = 1.0f;
	public float 			_GBlurModifier = 1.0f;

#if EBG_POSTFX_TONE_MAP
	//Tone Mapping
	private int[][] 		_ToneMappingWidth = new int[2][];
	private int[][]			_ToneMappingHeight = new int[2][];
	private RenderTexture[] _ToneMappingTextures;
	private const string	_ToneMappingName = "_PostFXToneMapping";
	private float			_ToneMappingTargetBrightness = 0.4f;
	private float			_ToneMappingCurrentBrightness = 0.4f;
	private float			_ToneMappingResponsiveness = 0.1f;
#endif

#endif

#if EBG_POSTFX_VIGNETTE
	//Vignette
	private const string 	_VignetteTextureName = "_VignetteTex";
	private const string 	_VignetteIntensityName = "_PostFXVignetteIntensity";
	private const string	_VignetteColorName = "_PostFXVignetteColor";
	private Texture2D		_VignetteTexture = null;
	private float			_VignetteIntensity = 1.0f;
	private Color           _VignetteColor = Color.white;
#endif

#if EBG_POSTFX_VIGNETTE2
	private const string    _Vignette2MaskTexName = "_Vignette2MaskTex";
	private const string    _Vignette2DarkIntensityName = "_Vignette2DarkIntensity";
	private const string    _Vignette2BrightIntensityName = "_Vignette2BrightIntensity";
	private const string    _Vignette2MaskColorName = "_Vignette2MaskColor";
	private const string    _Vignette2BrightColorName = "_Vignette2BrightColor";

	private Texture2D       _Vignette2MaskTex = null;
	private float           _Vignette2DarkIntensity = 1.0f;
	private float           _Vignette2BrightIntensity = 1.0f;
	private Color           _Vignette2MaskColor = Color.black;
	private Color           _Vignette2BrightColor = Color.white;
#endif

#if EBG_POSTFX_WARP
	//Warp
	private const string	_WarpTextureName = "_WarpTex";
	private const string	_WarpIntensityName = "_PostFXWarpIntensity";
	private RenderTextureFormat _WarpRenderTextureFormat;
	private RenderTexture	_WarpRenderTexture;
	public Camera			_WarpCamera;
	private int[] 			_WarpRenderTextureWidth = new int[2] { 512, 256 };
	private int[] 			_WarpRenderTextureHeight = new int[2] { 256, 128 };
	private const string	_WarpLayerName = "Warp";
	private Vector2			_WarpIntensity = Vector2.zero;
	private Shader			_WarpReplacementShader;
#endif

#if EBG_POSTFX_COLOR_GRADE
	//ColorGrading
	private Texture3D		_ColorGradeTexture;
	private const string	_ColorGradeTextureName = "_ColorGradeTex";
#endif

#if EBG_POSTFX_RADIAL_BLUR
	private int[]           _RadialBlurRenderTextureWidth = new int[2] { 512, 256 };
	private int[]           _RadialBlurRenderTextureHeight = new int[2] { 256, 128 };
	private RenderTexture   _RadialBlurTexture;
	private RenderTexture   _RadialBlurBaseTexture;
	private Vector2         _RadialCenter = new Vector2(0.5f, 0.5f);
	private float           _RadialSampleDist = 0;
	private float           _RadialSampleStrength = 0;
	private Material        _RadialBlurMaterial;
	private Camera          _RadialBlurCamera;
	private Transform       _RadialBlurTarget;
	private const string    _RadialCenterName = "_RadialCenter";
	private const string    _RadialSampleDistName = "_RadialSampleDist";
	private const string    _RadialSampleStrengthName = "_RadialSampleStrength";
	private const string    _RadialBlurTextureName = "_RadialBlurTex";
	private const string    _RadialBlurShaderName = "EBG/Effects/RadialBlur";
#endif

	static PostFXManager _this;
	public static PostFXManager Instance
	{
		get
		{
			if (_this == null)
			{
				GameObject go = new GameObject("~POSTFXMANAGER");
				go.hideFlags = HideFlags.HideAndDontSave;
				if (Application.isPlaying)
				{					
					DontDestroyOnLoad(go);
				}
				_this = go.AddComponent<PostFXManager>();
				_this.InitBase();
			}

			return _this;
		}
	}

	public void Init(Camera camera, PerformanceInfo.ePOSTFX_QUALITY quality, PerformanceInfo.ePOSTFX[] newPostFX)
	{
		if (camera == null)
		{
			EB.Debug.LogWarning("Couldn't init PostFX as we don't have a camera");
			return;
		}

		if (quality != Quality)
		{
			//quality mode has changed, destroy any previously active postfx
			foreach (PerformanceInfo.ePOSTFX postfx in System.Enum.GetValues(typeof(PerformanceInfo.ePOSTFX)))
			{
				if (IsActive(postfx))
				{
					DestroyEffect(postfx);
				}
			}
		}

		Quality = quality;

		var trigger = camera.gameObject.GetComponent<PostFXManagerTrigger>();
		if ((Quality == PerformanceInfo.ePOSTFX_QUALITY.Off) || (newPostFX.Length == 0))
		{
			if (trigger != null)
			{
#if EBG_POSTFX_BLOOM
				DestroyEntity(trigger);
#endif
			}

			foreach (PerformanceInfo.ePOSTFX postfx in System.Enum.GetValues(typeof(PerformanceInfo.ePOSTFX)))
			{
				//destroy any previously active postfx that are no longer active
				if (IsActive(postfx))
				{
					DestroyEffect(postfx);
				}
			}
			return;
		}
		else if (trigger == null)
		{
			//create the trigger if it doesn't exist
			camera.gameObject.AddComponent<PostFXManagerTrigger>();
		}

		foreach (PerformanceInfo.ePOSTFX postfx in System.Enum.GetValues(typeof(PerformanceInfo.ePOSTFX)))
		{
			//destroy any previously active postfx that are no longer active
			if (IsActive(postfx) && (System.Array.IndexOf(newPostFX, postfx) == -1))
			{
				DestroyEffect(postfx);
			}
			//init any active post fx that weren't previously active
			if (!IsActive(postfx) && (System.Array.IndexOf(newPostFX, postfx) != -1))
			{
				InitEffect(postfx, camera);
			}
		}

		_inited = true;
	}

	private bool DoPostFX()
	{
		return !_paused && _inited && (CurrentPostFX.Count > 0);
	}


	public void PostRender(Camera camera, RenderTexture src, RenderTexture dst)
	{
		if (!DoPostFX())
			return;

#if EBG_POSTFX_BLOOM
		if (IsActive(PerformanceInfo.ePOSTFX.Bloom))
		{
			Bloom(src);

#if EBG_POSTFX_TONE_MAP
			if (IsActive(PerformanceInfo.ePOSTFX.ToneMap))
			{
				ToneMap(src);
			}
#endif
		}
#endif

#if EBG_POSTFX_VIGNETTE
		if (IsActive(PerformanceInfo.ePOSTFX.Vignette))
		{
			Vignette(src);
		}
#endif

#if EBG_POSTFX_VIGNETTE2
		if (IsActive(PerformanceInfo.ePOSTFX.Vignette2))
		{
			Vignette2(src);
		}
#endif

#if EBG_POSTFX_WARP
		if (IsActive(PerformanceInfo.ePOSTFX.Warp))
		{
			Warp(src, camera);
		}
#endif

#if EBG_POSTFX_COLOR_GRADE
		if (IsActive(PerformanceInfo.ePOSTFX.ColorGrade))
		{
			ColorGrade(src);
		}
#endif

#if EBG_POSTFX_RADIAL_BLUR
		if (IsActive(PerformanceInfo.ePOSTFX.RadialBlur))
		{
			RadialBlur(src);
		}
#endif

		Graphics.Blit(src, dst, _CompositeMaterial);
	}

	//Things that use little-to-no memory, and we always keep around
	public void InitBase()
	{
		CurrentPostFX = new HashSet<PerformanceInfo.ePOSTFX>(PostFXComparer.Default);

#if EBG_POSTFX_BLOOM
		_BloomBlurWidth[(int)PerformanceInfo.ePOSTFX_QUALITY.Medium] = new int[] { 256, 256 };
		_BloomBlurHeight[(int)PerformanceInfo.ePOSTFX_QUALITY.Medium] = new int[] { 128, 128 };

		_BloomBlurWidth[(int)PerformanceInfo.ePOSTFX_QUALITY.High] = new int[] { 1024, 512, 256, 256, 256, 256 };
		_BloomBlurHeight[(int)PerformanceInfo.ePOSTFX_QUALITY.High] = new int[] { 512, 256, 128, 128, 128, 128 };

		_GaussianBlurShader = Shader.Find("EBG/Effects/GaussianBlur");
		_GaussianBlurMaterial = new Material(_GaussianBlurShader);
		_GaussianBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
#endif

#if EBG_POSTFX_TONE_MAP
		_ToneMappingWidth[(int)PerformanceInfo.ePOSTFX_QUALITY.Medium]  = new int[] {256, 64, 16};
		_ToneMappingHeight[(int)PerformanceInfo.ePOSTFX_QUALITY.Medium] = new int[] {128, 32,  8};
		
		_ToneMappingWidth[(int)PerformanceInfo.ePOSTFX_QUALITY.High]  = new int[] {512, 128, 32, 16};
		_ToneMappingHeight[(int)PerformanceInfo.ePOSTFX_QUALITY.High] = new int[] {256,  64, 16, 8};
#endif

		//final composite material
		_CompositeShader = Shader.Find("EBG/Effects/PostFXComposite");
		EB.Debug.Log("PostFXManager: FOUND SHADER in INITBASE!!!");
		_CompositeMaterial = new Material(_CompositeShader);
		_CompositeMaterial.hideFlags = HideFlags.HideAndDontSave;

		foreach (PerformanceInfo.ePOSTFX postfx in System.Enum.GetValues(typeof(PerformanceInfo.ePOSTFX)))
		{
			EnableMaterialKeyword(postfx, false);
		}
	}

	private void Release()
	{
		if (_BloomBlurTextures != null && _BloomBlurTextures.Length > 0)
		{
			foreach (var tex in _BloomBlurTextures)
			{
				tex.Release();
				DestroyEntity(tex);
			}
			_BloomBlurTextures = null;
		}
		if (_ToneMappingTextures != null && _ToneMappingTextures.Length > 0)
		{
			foreach (var tex in _ToneMappingTextures)
			{
				tex.Release();
				DestroyEntity(tex);
			}
			_ToneMappingTextures = null;
		}
		if (_WarpRenderTexture != null)
		{
			_WarpRenderTexture.Release();
			DestroyEntity(_WarpRenderTexture);
			_WarpRenderTexture = null;
		}
		if (_RadialBlurTexture != null)
		{
			_RadialBlurTexture.Release();
			DestroyEntity(_RadialBlurTexture);
			_RadialBlurTexture = null;
		}
		if (_RadialBlurBaseTexture != null)
		{
			_RadialBlurBaseTexture.Release();
			DestroyEntity(_RadialBlurBaseTexture);
			_RadialBlurBaseTexture = null;
		}
	}

#if UNITY_EDITOR
	public static void DestroyInstance()
	{
		if (_this != null)
		{
			_this.Release();
			UnityEngine.Object.DestroyImmediate(_this);
			_this = null;
		}
	}
#endif

	private void InitEffect(PerformanceInfo.ePOSTFX postfx, Camera camera)
	{
		EnablePostFX(postfx, true);

        switch (postfx)
        {
#if EBG_POSTFX_BLOOM
            case (PerformanceInfo.ePOSTFX.Bloom):
                EB.Debug.Log("PostFXManager: Initing Bloom");
                InitBloom();
                break;
#endif
#if EBG_POSTFX_VIGNETTE
            case (PerformanceInfo.ePOSTFX.Vignette):
                EB.Debug.Log("PostFXManager: Initing Vignette");
                InitVignette();
                break;
#endif
#if EBG_POSTFX_VIGNETTE2
            case (PerformanceInfo.ePOSTFX.Vignette2):
                EB.Debug.Log("PostFXManager: Initing Vignette2");
                InitVignette2();
                break;
#endif
#if EBG_POSTFX_WARP
            case (PerformanceInfo.ePOSTFX.Warp):
                EB.Debug.Log("PostFXManager: Initing Warp");
                InitWarp(camera);
                break;
#endif
#if EBG_POSTFX_TONE_MAP
            case (PerformanceInfo.ePOSTFX.ToneMap):
                EB.Debug.Log("PostFXManager: Initing Tone Mapping");
                InitToneMapping();
                break;
#endif
#if EBG_POSTFX_COLOR_GRADE
            case (PerformanceInfo.ePOSTFX.ColorGrade):
                EB.Debug.Log("PostFXManager: Initing Color Grading");
                InitColorGrade();
                break;
#endif
#if EBG_POSTFX_RADIAL_BLUR
            case PerformanceInfo.ePOSTFX.RadialBlur:
                EB.Debug.Log("PostFXManager: Initing Radial Blur");
                InitRadialBlur();
                break;
#endif
            case PerformanceInfo.ePOSTFX.None1:
            case PerformanceInfo.ePOSTFX.None2:
                EB.Debug.Log("PostFXManager: Initing None1 or None2"); 
                break;
            default:
                EB.Debug.LogError("Don't know how to init postfx {0}", postfx.ToString());
                break;
        }
	}

	private void DestroyEffect(PerformanceInfo.ePOSTFX postfx)
	{
		EnablePostFX(postfx, false);

        switch (postfx)
        {
#if EBG_POSTFX_BLOOM
            case (PerformanceInfo.ePOSTFX.Bloom):
                EB.Debug.Log("PostFXManager: Destroying Bloom");
                DestroyBloom();
                break;
#endif
#if EBG_POSTFX_VIGNETTE
            case (PerformanceInfo.ePOSTFX.Vignette):
                EB.Debug.Log("PostFXManager: Destroying Vignette");
                DestroyVignette();
                break;
#endif
#if EBG_POSTFX_VIGNETTE2
            case (PerformanceInfo.ePOSTFX.Vignette2):
                EB.Debug.Log("PostFXManager: Destroying Vignette2");
                DestroyVignette2();
                break;
#endif
#if EBG_POSTFX_WARP
            case (PerformanceInfo.ePOSTFX.Warp):
                EB.Debug.Log("PostFXManager: Destroying Warp");
                DestroyWarp();
                break;
#endif
#if EBG_POSTFX_TONE_MAP
            case (PerformanceInfo.ePOSTFX.ToneMap):
                EB.Debug.Log("PostFXManager: Destroying Tone Mapping");
                DestroyToneMapping();
                break;
#endif
#if EBG_POSTFX_COLOR_GRADE
            case (PerformanceInfo.ePOSTFX.ColorGrade):
                EB.Debug.Log("PostFXManager: Destroying Color Grading");
                DestroyColorGrade();
                break;
#endif
#if EBG_POSTFX_RADIAL_BLUR
            case (PerformanceInfo.ePOSTFX.RadialBlur):
                EB.Debug.Log("PostFXManager: Destroying Radial Blur");
                DestroyRadialBlur();
                break;
#endif
            case PerformanceInfo.ePOSTFX.None1:
            case PerformanceInfo.ePOSTFX.None2:
                EB.Debug.Log("PostFXManager: Destroying None1 or None2");
                break;
            default:
                EB.Debug.LogError("Don't know how to destroy postfx {0}", postfx.ToString());
                break;
        }
	}

#if EBG_POSTFX_BLOOM

	private void InitBloom()
	{
		//EnableShaderKeyword(POSTFX_KEYWORDS.POSTFX_COMPOSITE_BLOOM, true);

		int bloomBlurTexturesCount = _BloomBlurWidth[(int)Quality].Length;

		_BloomBlurTextures = new RenderTexture[bloomBlurTexturesCount];

		for (int i = 0; i < bloomBlurTexturesCount; ++i)
		{
#if USE_GET_TEMPORARY
			_BloomBlurTextures[i] = null;
#else
			_BloomBlurTextures[i] = new RenderTexture(_BloomBlurWidth[(int)Quality][i], _BloomBlurHeight[(int)Quality][i], 0, RenderTextureFormat.ARGB32);
			//DontDestroyOnLoad(_BloomBlurTextures[i]);
			_BloomBlurTextures[i].Create();
#endif
		}
	}

	void DestroyEntity(UnityEngine.Object g)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			DestroyImmediate(g);
		else
			Destroy(g);
#else
		Destroy(g);
#endif
	}
	private void DestroyBloom()
	{
		//EnableShaderKeyword(POSTFX_KEYWORDS.POSTFX_COMPOSITE_BLOOM, false);

		int bloomBlurTextureCount = _BloomBlurWidth[(int)Quality].Length;

		for (int i = 0; i < bloomBlurTextureCount; ++i)
		{
			if (_BloomBlurTextures[i] != null)
			{
#if USE_GET_TEMPORARY
				RenderTexture.ReleaseTemporary(_BloomBlurTextures[i]);
#else
				DestroyEntity(_BloomBlurTextures[i]);
#endif
			}
		}
		_BloomBlurTextures = null;
	}

	public void SetBloomColors(Color bloomColor, Color bloomOverbrightColor)
	{
		_BloomColor = bloomColor;
		_BloomOverbrightColor = bloomOverbrightColor;
	}

	public void SetBloomRamp(float bloomRamp)
	{
		_BloomRamp = bloomRamp;
	}

	public void SetBloomBlur(float bloomBlur)
	{
		_BloomBlurSigma = bloomBlur;
	}

	public void SetBloomIntensity(float intensity)
	{
		_BloomIntensity = intensity;
	}

	private static Vector4 GenerateGaussianBlurKernel(POSTFX_GUASSIAN_BLUR_MODE mode, float sigma)
	{
		float lowEndFake = 1.0f;
		if (GameFlowControlManager.Instance == null)
		{

		}
		else
		{
			PerformanceInfo.ePOSTFX_QUALITY fxquality = PerformanceManager.Instance.PerformanceInfo.EnvironmentInfoForScene(GameFlowControlManager.Instance.ActiveStateName).postFXQuality;
			if (fxquality == PerformanceInfo.ePOSTFX_QUALITY.Medium)
			{
				lowEndFake *= 1.8f;
			}
		}

		sigma *= lowEndFake;
		// any questions about the fake values in rendersettings,find hank.



		Vector4 res = Vector4.zero;
		if (mode == POSTFX_GUASSIAN_BLUR_MODE.TAP_3 || mode == POSTFX_GUASSIAN_BLUR_MODE.TAP_3_RAMP)
		{
			res.x = Mathf.Exp(-0.0f / (2.0f * sigma * sigma)) / (2.0f * Mathf.PI * sigma * sigma);
			res.y = Mathf.Exp(-1.0f / (2.0f * sigma * sigma)) / (2.0f * Mathf.PI * sigma * sigma);
			float t = res.y + res.x + res.y;
			res /= t;
		}
		else
		{
			res.x = Mathf.Exp(-0.0f / (2.0f * sigma * sigma)) / (2.0f * Mathf.PI * sigma * sigma);
			res.y = Mathf.Exp(-1.0f / (2.0f * sigma * sigma)) / (2.0f * Mathf.PI * sigma * sigma);
			res.z = Mathf.Exp(-4.0f / (2.0f * sigma * sigma)) / (2.0f * Mathf.PI * sigma * sigma);
			res.w = Mathf.Exp(-9.0f / (2.0f * sigma * sigma)) / (2.0f * Mathf.PI * sigma * sigma);
			float t = res.w + res.z + res.y + res.x + res.y + res.z + res.w;
			res /= t;
		}
		return res;
	}

	enum POSTFX_GAUSSIAN_BLUR_DIRECTION
	{
		HORIZONTAL,
		VERTICAL
	}

	enum POSTFX_GUASSIAN_BLUR_MODE
	{
		//maps to the pass ordering in the gaussian blur shader
		TAP_3 = 0,
		TAP_3_RAMP = 1,
		TAP_7 = 2,
		TAP_7_RAMP = 3
	}

	private void GaussianBlur(POSTFX_GAUSSIAN_BLUR_DIRECTION dir, POSTFX_GUASSIAN_BLUR_MODE mode, RenderTexture src, RenderTexture dst, float sigma, float ramp = 1.0f)
	{
		_GaussianBlurMaterial.SetVector("_BlurKernel", GenerateGaussianBlurKernel(mode, sigma));
		if (dir == POSTFX_GAUSSIAN_BLUR_DIRECTION.HORIZONTAL)
		{
			_GaussianBlurMaterial.SetVector("_StepSize", new Vector2(_GBlurModifier / src.width, 0.0f));
		}
		else
		{
			_GaussianBlurMaterial.SetVector("_StepSize", new Vector2(0.0f, _GBlurModifier / src.height));
		}
		if (mode == POSTFX_GUASSIAN_BLUR_MODE.TAP_7_RAMP || mode == POSTFX_GUASSIAN_BLUR_MODE.TAP_3_RAMP)
		{
			_GaussianBlurMaterial.SetFloat("_Ramp", ramp);
		}
		Graphics.Blit(src, dst, _GaussianBlurMaterial, (int)mode);
	}

	private void Bloom(RenderTexture baseTexture)
	{
		RenderTexture src = baseTexture;

		int bloomBlurTextureCount = _BloomBlurWidth[(int)Quality].Length;

#if USE_GET_TEMPORARY
		for (int i = 0; i < bloomBlurTextureCount; ++i)
		{
			if (_BloomBlurTextures[i] != null)
			{
				RenderTexture.ReleaseTemporary(_BloomBlurTextures[i]);
			}
			_BloomBlurTextures[i] = RenderTexture.GetTemporary(_BloomBlurWidth[(int)_quality][i], _BloomBlurHeight[(int)_quality][i], 0, RenderTextureFormat.ARGB32);
		}
#else
		//discard last frames texture
		_BloomBlurTextures[bloomBlurTextureCount - 1].DiscardContents();
#endif

		//first two passes are cheap blurs
		GaussianBlur(POSTFX_GAUSSIAN_BLUR_DIRECTION.VERTICAL, POSTFX_GUASSIAN_BLUR_MODE.TAP_7_RAMP, src, _BloomBlurTextures[0], _BloomBlurSigma, _BloomRamp);
		GaussianBlur(POSTFX_GAUSSIAN_BLUR_DIRECTION.HORIZONTAL, POSTFX_GUASSIAN_BLUR_MODE.TAP_7, _BloomBlurTextures[0], _BloomBlurTextures[1], _BloomBlurSigma);
		_BloomBlurTextures[0].DiscardContents();

		//blur
		for (int i = 2; i < bloomBlurTextureCount; i += 2)
		{
			GaussianBlur(POSTFX_GAUSSIAN_BLUR_DIRECTION.VERTICAL, POSTFX_GUASSIAN_BLUR_MODE.TAP_7, _BloomBlurTextures[i - 1], _BloomBlurTextures[i], _BloomBlurSigma);

#if !USE_GET_TEMPORARY
			_BloomBlurTextures[i - 1].DiscardContents();
#endif

			GaussianBlur(POSTFX_GAUSSIAN_BLUR_DIRECTION.HORIZONTAL, POSTFX_GUASSIAN_BLUR_MODE.TAP_7, _BloomBlurTextures[i], _BloomBlurTextures[i + 1], _BloomBlurSigma);

#if !USE_GET_TEMPORARY
			_BloomBlurTextures[i].DiscardContents();
#endif
		}

		_CompositeMaterial.SetTexture(_BloomTextureName, _BloomBlurTextures[bloomBlurTextureCount - 1]);
		_CompositeMaterial.SetColor(_BloomColorName, _BloomColor);
		_CompositeMaterial.SetFloat(_BloomIntensityName, _BloomIntensity);
		_CompositeMaterial.SetColor(_BloomOverbrightColorName, _BloomOverbrightColor);
		_CompositeMaterial.SetColor(_VignetteColorName, _VignetteColor);
	}

#endif

#if EBG_POSTFX_VIGNETTE

	private void InitVignette()
	{
		EB.Debug.Log("PostFXManager: INIT VIGNETTE");
		//EnableShaderKeyword(POSTFX_KEYWORDS.POSTFX_COMPOSITE_VIGNETTE, true);
		_VignetteTexture = (Texture2D)Resources.Load("Rendering/Textures/VignetteMask", typeof(Texture2D));
		EB.Debug.Log(_VignetteTexture.name);
		EB.Debug.Log(_CompositeMaterial.name);
		_CompositeMaterial.SetTexture(_VignetteTextureName, _VignetteTexture);

		_VignetteIntensity = 0.0f;
	}

	private void DestroyVignette()
	{
		EB.Debug.Log("PostFXManager: Destroy VIGNETTE");
		//EnableShaderKeyword(POSTFX_KEYWORDS.POSTFX_COMPOSITE_VIGNETTE, false);
		Resources.UnloadAsset(_VignetteTexture);
		_VignetteTexture = null;
	}

	private void Vignette(RenderTexture baseTexture)
	{
		_CompositeMaterial.SetFloat(_VignetteIntensityName, _VignetteIntensity);
		_CompositeMaterial.SetColor(_VignetteColorName, _VignetteColor);
	}

	public void SetVignetteIntensity(float intensity)
	{
		_VignetteIntensity = intensity;
	}

	public void SetVignetteColor(Color color)
	{
		_VignetteColor = color;
	}

#endif

#if EBG_POSTFX_VIGNETTE2
	public void SetVignette2(Texture2D mask, Color maskColor, Color brightColor, float brightIntensity, float darkIntensity)
	{
		_Vignette2MaskTex = mask;
		_Vignette2MaskColor = maskColor;
		_Vignette2BrightColor = brightColor;
		_Vignette2DarkIntensity = darkIntensity;
		_Vignette2BrightIntensity = brightIntensity;
	}

	private void Vignette2(RenderTexture baseTexture)
	{
		_CompositeMaterial.SetTexture(_Vignette2MaskTexName, _Vignette2MaskTex);
		_CompositeMaterial.SetFloat(_Vignette2DarkIntensityName, _Vignette2DarkIntensity);
		_CompositeMaterial.SetFloat(_Vignette2BrightIntensityName, _Vignette2BrightIntensity);
		_CompositeMaterial.SetColor(_Vignette2MaskColorName, _Vignette2MaskColor);
		_CompositeMaterial.SetColor(_Vignette2BrightColorName, _Vignette2BrightColor);
	}

	private void InitVignette2()
	{
		EB.Debug.Log("PostFXManager: INIT VIGNETTE2");
		//EnableShaderKeyword(POSTFX_KEYWORDS.POSTFX_COMPOSITE_VIGNETTE2, true);
	}

	private void DestroyVignette2()
	{
		EB.Debug.Log("PostFXManager: Destroy VIGNETTE2");
		//EnableShaderKeyword(POSTFX_KEYWORDS.POSTFX_COMPOSITE_VIGNETTE2, false);
		_Vignette2MaskTex = null;
		_Vignette2BrightColor = Color.white;
		_Vignette2BrightIntensity = 1;
		_Vignette2DarkIntensity = 1;
		_Vignette2MaskColor = Color.black;
	}
#endif

#if EBG_POSTFX_WARP
	
	private void InitWarp(Camera camera)
	{
		RenderTextureFormat[] desiredFormats = { RenderTextureFormat.RGHalf, RenderTextureFormat.RGFloat, RenderTextureFormat.ARGB32 };

		_WarpRenderTextureFormat = RenderTextureFormat.Default;
		foreach (RenderTextureFormat format in desiredFormats)
		{
			if (SystemInfo.SupportsRenderTextureFormat(format))
			{
				_WarpRenderTextureFormat = format;
				break;
			}
		}

#if USE_GET_TEMPORARY
		_WarpRenderTexture = null;
#else
		_WarpRenderTexture = new RenderTexture(_WarpRenderTextureWidth[(int)Quality], _WarpRenderTextureHeight[(int)Quality], 24, _WarpRenderTextureFormat);
		_WarpRenderTexture.isPowerOfTwo = true;
		_WarpRenderTexture.hideFlags = HideFlags.HideAndDontSave;
		_WarpRenderTexture.name = "WarpRenderTexture";
		_WarpRenderTexture.Create();
#endif

		GameObject go = new GameObject( "Warp Camera", typeof(Camera), typeof(Skybox) );
		if (Application.isPlaying)
		{
			DontDestroyOnLoad(go);
		}
		go.hideFlags = HideFlags.HideAndDontSave;
		_WarpCamera = go.GetComponent<Camera>();
		_WarpCamera.enabled = false;
		_WarpCamera.orthographic = false;
		_WarpCamera.targetTexture = _WarpRenderTexture;
		_WarpCamera.backgroundColor = Color.gray;
		_WarpCamera.clearFlags = CameraClearFlags.SolidColor;

		_WarpReplacementShader = Shader.Find("EBG/Effects/WarpReplacement");
	}
	
	private void DestroyWarp()
	{
#if USE_GET_TEMPORARY
		RenderTexture.ReleaseTemporary(_WarpRenderTexture);
		_WarpRenderTexture = null;
#else
		_WarpRenderTexture.Release();
		_WarpRenderTexture = null;
#endif

		DestroyEntity(_WarpCamera.gameObject);
		_WarpCamera = null;
	}

	private void Warp(RenderTexture src, Camera camera)
	{
		_WarpCamera.transform.position = camera.transform.position;
		_WarpCamera.transform.rotation = camera.transform.rotation;
		_WarpCamera.aspect = camera.aspect;
		_WarpCamera.fieldOfView = camera.fieldOfView;
		_WarpCamera.farClipPlane = camera.farClipPlane;
		_WarpCamera.nearClipPlane = camera.nearClipPlane;
#if USE_GET_TEMPORARY
		if (_WarpRenderTexture != null)
		{
			RenderTexture.ReleaseTemporary(_WarpRenderTexture);
			_WarpRenderTexture = null;
		}
		_WarpRenderTexture = RenderTexture.GetTemporary(_WarpRenderTextureWidth[(int)_quality], _WarpRenderTextureHeight[(int)_quality], 24, _WarpRenderTextureFormat);
#endif

		//render depth pre-pass
		_WarpCamera.cullingMask = camera.cullingMask;
		_WarpCamera.clearFlags = CameraClearFlags.SolidColor;
		_WarpCamera.SetReplacementShader(_WarpReplacementShader, "RenderType");
		_WarpCamera.Render();
		_WarpCamera.ResetReplacementShader();

		//render warp stuff
		_WarpCamera.clearFlags = CameraClearFlags.Nothing;
		_WarpCamera.cullingMask = (1 << LayerMask.NameToLayer(_WarpLayerName));
		_WarpCamera.Render();			

		_CompositeMaterial.SetTexture(_WarpTextureName, _WarpRenderTexture);
		_CompositeMaterial.SetVector(_WarpIntensityName, _WarpIntensity);
	}

	public void SetWarpIntensity(Vector2 intensity)
	{
		_WarpIntensity = intensity;
	}
	
#endif

#if EBG_POSTFX_TONE_MAP
	
	private void InitToneMapping()
	{
		int toneMapTexturesCount = _ToneMappingWidth[(int)Quality].Length;
		
		_ToneMappingTextures = new RenderTexture[toneMapTexturesCount];
		
		for (int i = 0; i < toneMapTexturesCount; ++i)
		{
#if USE_GET_TEMPORARY
			_ToneMappingTextures[i] = null;
#else
			_ToneMappingTextures[i] = new RenderTexture(_ToneMappingWidth[(int)Quality][i], _ToneMappingHeight[(int)Quality][i], 0, RenderTextureFormat.ARGB32);
			if (Application.isPlaying)
			{
				//DontDestroyOnLoad(_ToneMappingTextures[i]);
			}
			_ToneMappingTextures[i].Create();
#endif
		}
	}
	
	private void DestroyToneMapping()
	{
		int toneMapTexturesCount = _ToneMappingWidth[(int)Quality].Length;

		for (int i = 0; i < toneMapTexturesCount; ++i)
		{
			if (_ToneMappingTextures[i] != null)
			{
#if USE_GET_TEMPORARY
				RenderTexture.ReleaseTemporary(_ToneMappingTextures[i]);
#else
				DestroyEntity(_ToneMappingTextures[i]);
#endif
			}
		}
		_ToneMappingTextures = null;
	}
	
	private void ToneMap(RenderTexture baseTexture)
	{
		//RenderTexture src = baseTexture;
		
		int toneMapTexturesCount = _ToneMappingWidth[(int)Quality].Length;
		
#if USE_GET_TEMPORARY
		for (int i = 0; i < toneMapTexturesCount; ++i)
		{
			if (_ToneMappingTextures[i] != null)
			{
				RenderTexture.ReleaseTemporary(_ToneMappingTextures[i]);
			}
			_ToneMappingTextures[i] = RenderTexture.GetTemporary(_ToneMappingWidth[(int)_quality][i], _ToneMappingHeight[(int)_quality][i], 0, RenderTextureFormat.ARGB32);
		}
#endif

		Graphics.Blit(baseTexture, _ToneMappingTextures[0]);

		for(int i = 1; i < toneMapTexturesCount - 1; ++i)
		{
			Graphics.Blit(_ToneMappingTextures[i-1], _ToneMappingTextures[i]);
#if !USE_GET_TEMPORARY
			_ToneMappingTextures[i - 1].DiscardContents();
#endif
		}
		
		_GaussianBlurMaterial.SetTexture(_BloomTextureName, _BloomBlurTextures[_BloomBlurTextures.Length-1]);
		_GaussianBlurMaterial.SetColor(_BloomColorName, _BloomColor);
		Graphics.Blit(_ToneMappingTextures[toneMapTexturesCount - 2], _ToneMappingTextures[toneMapTexturesCount - 1], _GaussianBlurMaterial, 4);
#if !USE_GET_TEMPORARY
		_ToneMappingTextures[toneMapTexturesCount-2].DiscardContents();
#endif
		
		var tex = _ToneMappingTextures[toneMapTexturesCount - 1];
		
		RenderTexture.active = tex;

		Texture2D dest = new Texture2D(tex.width, tex.height);
		dest.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
		dest.Apply();

#if !USE_GET_TEMPORARY
		_ToneMappingTextures[toneMapTexturesCount - 1].DiscardContents();
#endif

		Color averageColor = Color.black;
		for (int x = 0; x < tex.width; ++x)
		{
			for (int y = 0; y < tex.height; ++y)
			{
				averageColor += dest.GetPixel(x,y);
			}
		}

		averageColor /= tex.width * tex.height;

		float brightness = (averageColor.r * 0.299f) + (averageColor.g * 0.587f) + (averageColor.b * 0.114f);

		float multiplier = _ToneMappingTargetBrightness / brightness;

		_ToneMappingCurrentBrightness = Mathf.Lerp(_ToneMappingCurrentBrightness, multiplier, _ToneMappingResponsiveness);

		_CompositeMaterial.SetFloat(_ToneMappingName, _ToneMappingCurrentBrightness);
	}

	public void SetToneMappingTargetBrightness(float target)
	{
		_ToneMappingTargetBrightness = target;
	}

	public void SetToneMappingResponsiveness(float target)
	{
		_ToneMappingResponsiveness = target;
	}
	
#endif

#if EBG_POSTFX_COLOR_GRADE
	
	private void InitColorGrade()
	{

	}
	
	private void DestroyColorGrade()
	{
		_ColorGradeTexture = null;
	}
	
	private void ColorGrade(RenderTexture baseTexture)
	{
		_CompositeMaterial.SetTexture(_ColorGradeTextureName, _ColorGradeTexture);
	}

	public void SetColorGradeTexture(Texture3D colourGradeTexture)
	{
		_ColorGradeTexture = colourGradeTexture;
	}

#endif

#if EBG_POSTFX_RADIAL_BLUR
	private void InitRadialBlur()
	{
		if (_RadialBlurMaterial == null)
		{
			Shader shader = Shader.Find(_RadialBlurShaderName);
			_RadialBlurMaterial = new Material(shader);
			_RadialBlurMaterial.hideFlags = HideFlags.NotEditable;
		}

		if (_RadialBlurTexture == null)
		{
			_RadialBlurTexture = new RenderTexture(_RadialBlurRenderTextureWidth[(int)Quality], _RadialBlurRenderTextureHeight[(int)Quality], 0, RenderTextureFormat.ARGB32);
			_RadialBlurTexture.filterMode = FilterMode.Bilinear;
			_RadialBlurTexture.hideFlags = HideFlags.HideAndDontSave;
			_RadialBlurTexture.Create();
		}

		if (_RadialBlurBaseTexture == null)
		{
			_RadialBlurBaseTexture = new RenderTexture(_RadialBlurRenderTextureWidth[(int)Quality], _RadialBlurRenderTextureHeight[(int)Quality], 0, RenderTextureFormat.ARGB32);
			_RadialBlurBaseTexture.filterMode = FilterMode.Bilinear;
			_RadialBlurBaseTexture.hideFlags = HideFlags.HideAndDontSave;
			_RadialBlurBaseTexture.Create();
		}

		SetRadialTarget(null, null);
	}

	private void DestroyRadialBlur()
	{
		if (_RadialBlurMaterial != null)
		{
			DestroyEntity(_RadialBlurMaterial);
			_RadialBlurMaterial = null;
		}

		if (_RadialBlurTexture != null)
		{
			_RadialBlurTexture.Release();
			DestroyEntity(_RadialBlurTexture);
			_RadialBlurTexture = null;
		}

		if (_RadialBlurBaseTexture != null)
		{
			_RadialBlurBaseTexture.Release();
			DestroyEntity(_RadialBlurBaseTexture);
			_RadialBlurBaseTexture = null;
		}

		_RadialSampleDist = 0;
		_RadialSampleStrength = 0;
		_RadialCenter = new Vector2(0.5f, 0.5f);
		_RadialBlurCamera = null;
		_RadialBlurTarget = null;
	}

	private void RadialBlur(RenderTexture baseTexture)
	{
		if (Application.isPlaying)
		{
			if (_RadialBlurCamera == null || _RadialBlurTarget == null)
			{
				_CompositeMaterial.SetFloat(_RadialSampleStrengthName, 0);
				return;
			}

			//var worldPos = _RadialBlurTarget.position;
			//var screenPos = _RadialBlurCamera.WorldToScreenPoint(worldPos);
			//_RadialCenter = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
		}

		if (_RadialSampleDist > 0 && _RadialSampleStrength > 0)
		{
			_RadialBlurMaterial.SetFloat(_RadialSampleDistName, _RadialSampleDist);
			_RadialBlurMaterial.SetVector(_RadialCenterName, _RadialCenter);
			
			_RadialBlurBaseTexture.DiscardContents();
			_RadialBlurTexture.DiscardContents();
			// large to small
			Graphics.Blit(baseTexture, _RadialBlurBaseTexture);
			// blur small
			Graphics.Blit(_RadialBlurBaseTexture, _RadialBlurTexture, _RadialBlurMaterial);

			_CompositeMaterial.SetFloat(_RadialSampleStrengthName, _RadialSampleStrength);
			_CompositeMaterial.SetVector(_RadialCenterName, _RadialCenter);
			_CompositeMaterial.SetTexture(_RadialBlurTextureName, _RadialBlurTexture);
		}
		else
		{
			_CompositeMaterial.SetFloat(_RadialSampleStrengthName, 0);
			_CompositeMaterial.SetVector(_RadialCenterName, _RadialCenter);
			_CompositeMaterial.SetTexture(_RadialBlurTextureName, _RadialBlurTexture);
		}
	}

	public void SetRadialBlur(float sampleDist, float sampleStrength)
	{
		_RadialSampleDist = sampleDist;
		_RadialSampleStrength = sampleStrength;
	}

	public void SetRadialTarget(Camera camera, Transform target)
	{
		_RadialBlurCamera = camera;
		_RadialBlurTarget = target;

		if (_RadialBlurCamera != null && _RadialBlurTarget != null)
		{
			var worldPos = _RadialBlurTarget.position;
			var screenPos = _RadialBlurCamera.WorldToScreenPoint(worldPos);
			_RadialCenter = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
		}

		if (Application.isPlaying)
		{
			EnableMaterialKeyword(PerformanceInfo.ePOSTFX.RadialBlur, camera != null && target != null);
		}
		else
		{
			EnableMaterialKeyword(PerformanceInfo.ePOSTFX.RadialBlur, true);
		}
	}
#endif

			//UTILITY

	public void Pause()
	{
		_paused = true;
	}

	public void Resume()
	{
		_paused = false;
	}

	private void EnableShaderKeyword(POSTFX_KEYWORDS keyword, bool enabled)
	{
		string shaderKeywordPrefix = keyword.ToString();
		if (enabled)
		{
			Shader.DisableKeyword("EBG_" + shaderKeywordPrefix + "_OFF");
			Shader.EnableKeyword("EBG_" + shaderKeywordPrefix + "_ON");
		}
		else
		{
			Shader.DisableKeyword("EBG_" + shaderKeywordPrefix + "_ON");
			Shader.EnableKeyword("EBG_" + shaderKeywordPrefix + "_OFF");
		}
	}

	private void EnablePostFX(PerformanceInfo.ePOSTFX keyword, bool enabled)
	{
		//string shaderKeywordPrefix = "EBG_POSTFX_COMPOSITE_" + keyword.ToString().ToUpper();
		if (enabled)
		{
			//_CompositeMaterial.DisableKeyword(shaderKeywordPrefix + "_OFF");
			//_CompositeMaterial.EnableKeyword(shaderKeywordPrefix + "_ON");
#if UNITY_EDITOR
			if (CurrentPostFX.Contains(keyword))
			{
				EB.Debug.LogError("Enabling already enabled PostFX: {0}", keyword);
			}
#endif
			CurrentPostFX.Add(keyword);
		}
		else
		{
			//_CompositeMaterial.DisableKeyword(shaderKeywordPrefix + "_ON");
			//_CompositeMaterial.EnableKeyword(shaderKeywordPrefix + "_OFF");
#if UNITY_EDITOR
			if (!CurrentPostFX.Contains(keyword))
			{
				EB.Debug.LogError("Disabling already disabled PostFX: {0}", keyword);
			}
#endif
			CurrentPostFX.Remove(keyword);
		}

		EnableMaterialKeyword(keyword, enabled);
	}

	private void EnableMaterialKeyword(PerformanceInfo.ePOSTFX keyword, bool enabled)
	{
		string shaderKeywordPrefix = "EBG_POSTFX_COMPOSITE_" + keyword.ToString().ToUpper();
		if (enabled)
		{
			_CompositeMaterial.DisableKeyword(shaderKeywordPrefix + "_OFF");
			_CompositeMaterial.EnableKeyword(shaderKeywordPrefix + "_ON");
		}
		else
		{
			_CompositeMaterial.DisableKeyword(shaderKeywordPrefix + "_ON");
			_CompositeMaterial.EnableKeyword(shaderKeywordPrefix + "_OFF");
		}
	}

	public bool IsActive(PerformanceInfo.ePOSTFX postfx)
	{
		return CurrentPostFX.Contains(postfx);
	}

}
