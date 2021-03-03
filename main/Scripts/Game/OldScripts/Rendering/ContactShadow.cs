using UnityEngine;
using System.Collections;

public class ContactShadow : MonoBehaviour {

	//angle offset
	public float hipDegreeOffset = 0;
	public float leftFootDegree = 0;
	public float rightFootDegree = 0;

	//Hips
	public Texture _TextureHip;
	public float _minSizeHip = 2f;
	public float _maxSizeHip = 4f;
	public float _minYHip = 0f;
	public float _maxYHip = 2f;
	
	private GameObject _GameObjectHip;
	private Mesh _MeshHip;
	private Renderer _RendererHip;
	private MeshFilter _MestFilterHip;
	private Material _MaterialHip;

	//Feet
	public Texture _TextureFoot;
	public float _minSizeFoot = 1f;
	public float _maxSizeFoot = 1.5f;
	public float _minYFoot = 0f;
	public float _maxYFoot = 2f;

	private PerformanceInfo.eSHADOW_QUALITY _ShadowQuality = PerformanceInfo.eSHADOW_QUALITY.Off;

	private GameObject _GameObjectLeftFoot;
	private Mesh _MeshLeftFoot;
	private Renderer _RendererLeftFoot;
	private MeshFilter _MestFilterLeftFoot;
	private Material _MaterialLeftFoot;

	private GameObject _GameObjectRightFoot;
	private Mesh _MeshRightFoot;
	private Renderer _RendererRightFoot;
	private MeshFilter _MestFilterRightFoot;
	private Material _MaterialRightFoot;

	//shared
	private float _yOffset = 0.01f;

	private GameObject hip, leftFoot, rightFoot;

	void Awake () 
	{
		//_ShadowQuality = PerformanceManager.ShadowQuality;

		_ShadowQuality = PerformanceInfo.eSHADOW_QUALITY.On;
		//if(_ShadowQuality != PerformanceInfo.eSHADOW_QUALITY.Low)
		{
			hip = EB.Util.GetObjectExactMatch(gameObject, "Hips");
			DrawHipShadow();
		}
		if(_ShadowQuality == PerformanceInfo.eSHADOW_QUALITY.On)
		{
			leftFoot = EB.Util.GetObjectExactMatch(gameObject, "LeftFoot");
			rightFoot = EB.Util.GetObjectExactMatch(gameObject, "RightFoot");
			DrawRightFootShadow();
			DrawLeftFootShadow();
		}
	}

	void DrawHipShadow()
	{
		_GameObjectHip = new GameObject("HipShadow", typeof(MeshFilter), typeof(MeshRenderer));
		_GameObjectHip.transform.parent = transform;
		_MestFilterHip = _GameObjectHip.GetComponent<MeshFilter>();
		
		_MaterialHip = new Material(Shader.Find("EBG/Misc/ContactShadow"));
		if(_TextureHip) 
		{
			_MaterialHip.SetTexture("_MainTex", _TextureHip);
		}
		Vector4 scale = new Vector4(_minSizeHip,1,_minSizeHip,1);
		_MaterialHip.SetVector("_Scale", scale);
		_RendererHip = _MestFilterHip.GetComponent<Renderer>();
		_RendererHip.material = _MaterialHip;
		
		_MestFilterHip.sharedMesh = new Mesh();
		_MeshHip = _MestFilterHip.sharedMesh;
		
		UpdateBasicMesh(_MeshHip);
	}

	void DrawLeftFootShadow()
	{
		_GameObjectLeftFoot = new GameObject("LeftFootShadow", typeof(MeshFilter), typeof(MeshRenderer));
		_GameObjectLeftFoot.transform.parent = leftFoot.transform;
		_GameObjectLeftFoot.transform.localPosition = Vector3.zero;
		_MestFilterLeftFoot = _GameObjectLeftFoot.GetComponent<MeshFilter>();
		
		_MaterialLeftFoot = new Material(Shader.Find("EBG/Misc/ContactShadow"));
		if(_TextureFoot) 
		{
			_MaterialLeftFoot.SetTexture("_MainTex", _TextureFoot);
		}
		Vector4 scale = new Vector4(_minSizeFoot,1,_minSizeFoot,1);
		_MaterialLeftFoot.SetVector("_Scale", scale);
		_RendererLeftFoot = _MestFilterLeftFoot.GetComponent<Renderer>();
		_RendererLeftFoot.material = _MaterialLeftFoot;
		
		_MestFilterLeftFoot.sharedMesh = new Mesh();
		_MeshLeftFoot = _MestFilterLeftFoot.sharedMesh;
		UpdateBasicMesh(_MeshLeftFoot);

	}

	void DrawRightFootShadow()
	{
		_GameObjectRightFoot = new GameObject("RightFootShadow", typeof(MeshFilter), typeof(MeshRenderer));
		_GameObjectRightFoot.transform.parent = rightFoot.transform;
		_GameObjectRightFoot.transform.localPosition = Vector3.zero;
		_MestFilterRightFoot = _GameObjectRightFoot.GetComponent<MeshFilter>();
		
		_MaterialRightFoot = new Material(Shader.Find("EBG/Misc/ContactShadow"));
		if(_TextureFoot) 
		{
			_MaterialRightFoot.SetTexture("_MainTex", _TextureFoot);
		}
		Vector4 scale = new Vector4(_minSizeFoot,1,_minSizeFoot,1);
		_MaterialRightFoot.SetVector("_Scale", scale);
		_RendererRightFoot = _MestFilterRightFoot.GetComponent<Renderer>();
		_RendererRightFoot.material = _MaterialRightFoot;
		
		_MestFilterRightFoot.sharedMesh = new Mesh();
		_MeshRightFoot = _MestFilterRightFoot.sharedMesh;
		UpdateBasicMesh(_MeshRightFoot);
		
	}

	void UpdateBasicMesh(Mesh m) 
	{
		Vector3[] _vertices = new Vector3[4];
		_vertices[0] = new Vector3(-0.5f,_yOffset,-0.5f);
		_vertices[1] = new Vector3(-0.5f,_yOffset,0.5f);
		_vertices[2] = new Vector3(0.5f,_yOffset,-0.5f);
		_vertices[3] = new Vector3(0.5f,_yOffset,0.5f);
		
		Vector2[] _uvs = new Vector2[4];
		_uvs[0] = new Vector2(0, 0);
		_uvs[1] = new Vector2(0, 1);
		_uvs[2] = new Vector2(1, 0);
		_uvs[3] = new Vector2(1, 1);
		
		int[] _indices = new int[6];
		_indices[0] = 0;
		_indices[1] = 1;
		_indices[2] = 2;
		_indices[3] = 2;
		_indices[4] = 1;
		_indices[5] = 3;
		
		m.vertices = _vertices;
		m.uv = _uvs;
		m.triangles = _indices;
	}

	~ContactShadow()
	{
		_MeshHip = null;
		_MestFilterHip = null;
		_GameObjectHip = null;
		_MaterialHip = null;

		_MeshLeftFoot = null;
		_MestFilterLeftFoot = null;
		_GameObjectLeftFoot = null;
		_MaterialLeftFoot = null;

		_MeshRightFoot = null;
		_MestFilterRightFoot = null;
		_GameObjectRightFoot = null;
		_MaterialRightFoot = null;
	}
	
	void Update () 
	{
	//	if(_ShadowQuality != PerformanceInfo.eSHADOW_QUALITY.Off)
		{
			//HIP
			float rotY = hip.transform.localEulerAngles.y + hipDegreeOffset;
			float normY = Mathf.Clamp01((transform.position.y - _minYHip) / (_maxYHip - _minYHip));
			float fade = 1f - normY;
			float size = Mathf.Lerp(_minSizeHip, _maxSizeHip, normY);
			Vector4 scale = new Vector4(size,1,size,1);

			_MaterialHip.SetMatrix("_Obj2World", Matrix4x4.TRS(hip.transform.position, Quaternion.AngleAxis(rotY, Vector3.up), scale));
			_MaterialHip.SetFloat("_Alpha", fade);
		}

		if(_ShadowQuality == PerformanceInfo.eSHADOW_QUALITY.On)
		{
			//LEFT
			float rotY = leftFoot.transform.localEulerAngles.y + leftFootDegree;
			float normYLeft = Mathf.Clamp01((leftFoot.transform.position.y - _minYFoot) / (_maxYFoot - _minYFoot));
			float fadeLeft = 1f - normYLeft;
			float sizeLeft = Mathf.Lerp(_minSizeFoot, _maxSizeFoot, normYLeft);
			Vector4 scaleLeft = new Vector4(sizeLeft,1,sizeLeft,1);

			_MaterialLeftFoot.SetMatrix("_Obj2World", Matrix4x4.TRS(leftFoot.transform.position, Quaternion.AngleAxis(rotY, Vector3.up), scaleLeft));
			_MaterialLeftFoot.SetFloat("_Alpha", fadeLeft);

			//RIGHT
			rotY = rightFoot.transform.localEulerAngles.y + rightFootDegree;
			float normYRight = Mathf.Clamp01((rightFoot.transform.position.y - _minYFoot) / (_maxYFoot - _minYFoot));
			float fadeRight = 1f - normYRight;
			float sizeRight = Mathf.Lerp(_minSizeFoot, _maxSizeFoot, normYRight);
			Vector4 scaleRight = new Vector4(sizeRight,1,sizeRight,1);

			_MaterialRightFoot.SetMatrix("_Obj2World", Matrix4x4.TRS(rightFoot.transform.position, Quaternion.AngleAxis(rotY, Vector3.up), scaleRight));
			_MaterialRightFoot.SetFloat("_Alpha", fadeRight);
		}
	
	}
}
