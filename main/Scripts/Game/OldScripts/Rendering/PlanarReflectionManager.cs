using UnityEngine;
using System.Collections.Generic;

public class PlanarReflectionManager : MonoBehaviour
{
	public static string PlanarReflectionOnlyLayer { get; private set; }	

	static PlanarReflectionManager()
	{
		PlanarReflectionOnlyLayer = "PlanarReflectionOnly";
	}

	public enum 			eCLEAR_MODE
	{
		Color,
		Skybox
	};

	public LayerMask		m_layerMask = -1;
	private Color			m_backgroundColor = Color.black;
	private eCLEAR_MODE		m_clearMode = eCLEAR_MODE.Color;
	
	private string 			m_textureName = "_PlanarReflectionTex";

	private float			m_ClipPlaneOffset = 0.07f;
	public Camera			m_ReflectionCamera;
	private Shader			m_ReplacementShader;
	
	//reflection
	private int[] 			m_ReflectionWidth = {256, 512};
	private int[] 			m_ReflectionHeight = {128, 256};
	private RenderTexture	m_renderTexture = null;
	
	//env blur
	private int[] 			m_blurWidth = {128, 256};
	private int[] 			m_blurHeight = {64, 128};
	private int[] 			m_blurWidth2 = {128, 256};
	private int[] 			m_blurHeight2 = {64, 128};
	public RenderTexture 	m_blurTexture1 = null;
	public RenderTexture 	m_blurTexture2 = null;
	private Shader 			m_blurShader;
	private Material 		m_blurMaterial;
	
	private bool			m_paused = false;
	private bool 			m_setup = false;
	public PerformanceInfo.eREFLECTION_QUALITY Quality { get; private set; }
	
//#if UNITY_EDITOR
	public bool				UseSceneView = false;
	public bool 			UseReplacementShaders = true;
	public float            PlaneY = 0.0f;
//#endif
	
	static PlanarReflectionManager _this;
	public static PlanarReflectionManager Instance
	{ 
		get 
		{ 
			if (_this == null)
			{
				GameObject go = new GameObject("PlanarReflectionManager");
				_this = go.AddComponent<PlanarReflectionManager>();
				go.hideFlags = HideFlags.HideAndDontSave;
				if (Application.isPlaying)
				{
					DontDestroyOnLoad(go);
				}
			}
			
			return _this; 
		}
	}

#if UNITY_EDITOR
	public static void DestroyInstance()
	{
		if (_this != null)
		{
			_this.Release();
			DestroyImmediate(_this.gameObject);
			_this = null;
		}
	}
#endif

	public static bool Initilized()
	{
		return _this != null;
	}

	void Release()
	{
		if (m_ReflectionCamera != null)
		{
#if UNITY_EDITOR
			DestroyImmediate(m_ReflectionCamera.gameObject);
#else
			Destroy(m_ReflectionCamera.gameObject);
#endif
		}

		if (m_blurTexture1 != null)
		{
			m_blurTexture1.Release();
#if UNITY_EDITOR
			DestroyImmediate(m_blurTexture1);
#else
			Destroy(m_blurTexture1);
#endif
		}

		if (m_blurTexture2 != null)
		{
			m_blurTexture2.Release();
#if UNITY_EDITOR
			DestroyImmediate(m_blurTexture2);
#else
			Destroy(m_blurTexture2);
#endif
		}

		if (m_renderTexture != null)
		{
			m_renderTexture.Release();
#if UNITY_EDITOR
			DestroyImmediate(m_renderTexture);
#else
			Destroy(m_renderTexture);
#endif
		}
	}
	
	private void Setup()
	{
		//things that only have to happen once

		if (m_setup)
		{
			return;
		}

		Quality = PerformanceInfo.eREFLECTION_QUALITY.Off;

#if UNITY_EDITOR
		GameObject go = GameObject.Find("Mirror Reflection Camera") ?? new GameObject( "Mirror Reflection Camera", typeof(Camera), typeof(Skybox) );
#else
		GameObject go = new GameObject( "Mirror Reflection Camera", typeof(Camera), typeof(Skybox) );
#endif
		go.hideFlags = HideFlags.HideAndDontSave;
		if (Application.isPlaying)
		{
			DontDestroyOnLoad(go);
		}
		
		m_ReflectionCamera = go.GetComponent<Camera>();
		m_ReflectionCamera.enabled = false;
		m_ReflectionCamera.transform.position = Vector3.zero;
		m_ReflectionCamera.transform.rotation = Quaternion.identity;
		m_ReflectionCamera.backgroundColor = Color.black;
		m_ReflectionCamera.clearFlags = CameraClearFlags.SolidColor;

		m_blurShader = Shader.Find("EBG/Effects/ReflectionBlur");
		m_blurMaterial = new Material(m_blurShader);
		m_blurMaterial.hideFlags = HideFlags.HideAndDontSave;

		m_ReplacementShader = Shader.Find("EBG/Effects/ReflectionReplacement");
		m_ReplacementShader.hideFlags = HideFlags.HideAndDontSave;
		
		m_setup = true;
	}
	
	public void Init(PerformanceInfo.eREFLECTION_QUALITY quality, int reflectedLayerMask)
	{
		Setup();

		m_layerMask = reflectedLayerMask | (1 << LayerMask.NameToLayer(PlanarReflectionOnlyLayer));

		if (quality == Quality)
		{
			EB.Debug.Log("PlanarReflectionManager: No quality change {0}", Quality);
			return;
		}

		if (Quality != PerformanceInfo.eREFLECTION_QUALITY.Off)
		{
			//De-init if we weren't previously "off"
			EB.Debug.Log("PlanarReflectionManager: DeInit {0}", Quality);
			DeInit();
		}

		if ((quality != PerformanceInfo.eREFLECTION_QUALITY.Off) && (reflectedLayerMask != 0))
		{
			//init if we aren't off and have something to reflect
			EB.Debug.Log("PlanarReflectionManager: Init {0}", quality);
			InitQuality(quality);
			Quality = quality;
		}
		else
		{
			EB.Debug.Log("PlanarReflectionManager: Off");
			Quality = PerformanceInfo.eREFLECTION_QUALITY.Off;
		}
	}

	private void InitQuality(PerformanceInfo.eREFLECTION_QUALITY quality)
	{
		m_renderTexture = new RenderTexture(m_ReflectionWidth[(int)quality], m_ReflectionHeight[(int)quality], 24);
		m_renderTexture.isPowerOfTwo = true;
		m_renderTexture.hideFlags = HideFlags.HideAndDontSave;
		m_renderTexture.name = "PlanarReflectionTexture";
		m_renderTexture.Create();

		m_ReflectionCamera.targetTexture = m_renderTexture;

		m_blurTexture1 = new RenderTexture(m_blurWidth[(int)quality], m_blurHeight[(int)quality], 0);
		m_blurTexture1.hideFlags = HideFlags.HideAndDontSave;
		m_blurTexture1.Create();

		m_blurTexture2 = new RenderTexture(m_blurWidth2[(int)quality], m_blurHeight2[(int)quality], 0);
		m_blurTexture2.hideFlags = HideFlags.HideAndDontSave;
		m_blurTexture2.Create();
	}

	private void DeInit()
	{
		m_ReflectionCamera.targetTexture = null;
		EB.Coroutines.EndOfFrame(EB.SafeAction.Wrap(this, delegate() {
			m_renderTexture.Release();
			m_blurTexture1.Release();
			m_blurTexture2.Release();
		}));
	}
	
	public void Render(Camera sceneCamera)
	{
		if ((Quality == PerformanceInfo.eREFLECTION_QUALITY.Off) || m_paused || (m_layerMask == 0))
		{
			return;
		}

		int oldPixelLightCount = QualitySettings.pixelLightCount;
		QualitySettings.pixelLightCount = 0;

#if UNITY_EDITOR
		if ( UseSceneView )
		{
			sceneCamera = Camera.current;
		}
#endif
		
		if( !sceneCamera )
			return;
		
		// find out the reflection plane: position and normal in world space
		Vector3 pos = Vector3.zero;
		pos.y = 0;//PerformanceManager.Instance.CurrentRoadHeight;
#if UNITY_EDITOR
		pos.y = PlaneY;
#endif
		Vector3 normal = Vector3.up;
		UpdateCameraModes( sceneCamera, m_ReflectionCamera );

		// Reflect camera around reflection plane
		float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
		Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);
 
		Matrix4x4 reflection = CalculateReflectionMatrix(reflectionPlane);
		Vector3 oldpos = sceneCamera.transform.position;
		Vector3 newpos = reflection.MultiplyPoint( oldpos );
		m_ReflectionCamera.worldToCameraMatrix = sceneCamera.worldToCameraMatrix * reflection;
		// Setup oblique projection matrix so that near plane is our reflection
		// plane. This way we clip everything below/above it for free.
		Vector4 clipPlane = CameraSpacePlane(m_ReflectionCamera, pos, normal, 1.0f);
		Matrix4x4 projection = sceneCamera.projectionMatrix;
		CalculateObliqueMatrix(ref projection, clipPlane);
		m_ReflectionCamera.projectionMatrix = projection;
		m_ReflectionCamera.farClipPlane = m_ReflectionCamera.farClipPlane;

		// Render reflection
		//GL.SetRevertBackfacing(true);
		GL.invertCulling = true;
		m_ReflectionCamera.transform.position = newpos;
		Vector3 euler = sceneCamera.transform.eulerAngles;
		m_ReflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
		m_ReflectionCamera.cullingMask = m_layerMask.value;
		
#if UNITY_EDITOR
		if (UseReplacementShaders)
#endif
		{
			m_ReflectionCamera.RenderWithShader(m_ReplacementShader, "RenderType");
		}
#if UNITY_EDITOR
		else
		{
			m_ReflectionCamera.Render();
		}
#endif
		m_ReflectionCamera.transform.position = oldpos;
		//GL.SetRevertBackfacing(false);
		GL.invertCulling = false;
		QualitySettings.pixelLightCount = oldPixelLightCount;

		m_blurTexture2.DiscardContents();
		m_blurMaterial.SetVector("_HalfTexelOffset", new Vector2(0.5f / ((float)m_renderTexture.width), 0.0f));
		Graphics.Blit(m_renderTexture, m_blurTexture1, m_blurMaterial, 0);
		m_renderTexture.DiscardContents();
		m_blurMaterial.SetVector("_HalfTexelOffset", new Vector2(0.0f, 0.5f / ((float)m_blurTexture2.height)));
		Graphics.Blit(m_blurTexture1, m_blurTexture2, m_blurMaterial, 1);
		m_blurTexture1.DiscardContents();

		//Set reflection texture
		Shader.SetGlobalTexture(m_textureName, m_blurTexture2);
	}
	
	public void Pause()
	{
		m_paused = true;
	}
	
	public void Resume()
	{
		m_paused = false;
	}

	public void SetPlanarReflectionRamp(float ramp)
	{
		if(m_blurMaterial != null)
		{
			m_blurMaterial.SetFloat("_Ramp", ramp);
		}
	}

	public void SetBackgroundColor(Color color)
	{
		m_backgroundColor = color;
	}

	public void SetClearMode(eCLEAR_MODE clearMode)
	{
		m_clearMode = clearMode;
	}
	
	private void UpdateCameraModes( Camera src, Camera dest )
	{
		if( dest == null )
			return;
		// set camera to clear the same way as current camera
		dest.clearFlags = src.clearFlags;
		if (m_clearMode == eCLEAR_MODE.Color)
		{
			dest.clearFlags = CameraClearFlags.SolidColor;
			dest.backgroundColor = m_backgroundColor;        
		}
		else
		{
			dest.GetComponent<Skybox>().material = src.GetComponent<Skybox>().material;
			dest.clearFlags = CameraClearFlags.Skybox; 
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}
 
	// Extended sign: returns -1, 0 or 1 based on sign of a
	private static float sgn(float a)
	{
		if (a > 0.0f) return 1.0f;
		if (a < 0.0f) return -1.0f;
		return 0.0f;
	}
 
	// Given position/normal of the plane, calculates plane in camera space.
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint( offsetPos );
		Vector3 cnormal = m.MultiplyVector( normal ).normalized * sideSign;
		return new Vector4( cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos,cnormal) );
	}
 
	// Adjusts the given projection matrix so that near plane is the given clipPlane
	// clipPlane is given in camera space. See article in Game Programming Gems 5 and
	// http://aras-p.info/texts/obliqueortho.html
	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 q = projection.inverse * new Vector4(
			sgn(clipPlane.x),
			sgn(clipPlane.y),
			1.0f,
			1.0f
		);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot (clipPlane, q)));
		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
	}
 
	// Calculates reflection matrix around the given plane
	private static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
	{
		Matrix4x4 reflectionMat;
		
		reflectionMat.m00 = (1F - 2F*plane[0]*plane[0]);
		reflectionMat.m01 = (   - 2F*plane[0]*plane[1]);
		reflectionMat.m02 = (   - 2F*plane[0]*plane[2]);
		reflectionMat.m03 = (   - 2F*plane[3]*plane[0]);
 
		reflectionMat.m10 = (   - 2F*plane[1]*plane[0]);
		reflectionMat.m11 = (1F - 2F*plane[1]*plane[1]);
		reflectionMat.m12 = (   - 2F*plane[1]*plane[2]);
		reflectionMat.m13 = (   - 2F*plane[3]*plane[1]);
 
		reflectionMat.m20 = (   - 2F*plane[2]*plane[0]);
		reflectionMat.m21 = (   - 2F*plane[2]*plane[1]);
		reflectionMat.m22 = (1F - 2F*plane[2]*plane[2]);
		reflectionMat.m23 = (   - 2F*plane[3]*plane[2]);
 
		reflectionMat.m30 = 0F;
		reflectionMat.m31 = 0F;
		reflectionMat.m32 = 0F;
		reflectionMat.m33 = 1F;
		
		return reflectionMat;
	}
}
