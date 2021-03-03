using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GM;

public class DynamicAtlas : MonoBehaviour 
{
	public const int DA_PADDING = 2;
	public enum eDynamicAtlasPackType
	{
		FIXED_SIZE,
		POWER2_SIZE,
		MULTI_SIZE
	}
	public eDynamicAtlasPackType				m_AtlasType;
	public Camera								m_RenderCamera;
	public Material								m_Material;
	public int									m_HDAtlasSize = 1024;
	public int 									m_SDAtlasSize = 512;
	public int									m_HDSpriteFixedSize = 128;
	public int									m_SDSpriteFixedSize = 64;
	public string								m_LocalHDTexturePath = "DynamicAtlases/Icons/";
	public string								m_LocalSDTexturePath = "DynamicAtlases/Icons/";

	public GameObject							m_MeshRenderTemplate;
	[HideInInspector]
	public List<EventDelegate> 					onSpriteUVsUpdate = new List<EventDelegate>();
	[HideInInspector]
	public List<EventDelegate>					onTextureAsyncSucceeded = new List<EventDelegate>();
	[HideInInspector]
	public List<EventDelegate>					onTextureAsyncFailed = new List<EventDelegate>();
	[HideInInspector]
	public static string						CurrentAsyncedTexture = string.Empty;

	private BetterList<MeshRenderer>			m_MeshRenderers = new BetterList<MeshRenderer>();

	private bool								m_IsInitialized = false;
	private bool								m_UVUpdateFlag = false;
	private DynamicSpriteManagerBase			m_SpriteManager;

	private MaterialPropertyBlock _matPropBlock;
	private int _matTextureID;

	void Start()
	{
		_matPropBlock = new MaterialPropertyBlock();
		_matTextureID = Shader.PropertyToID("_MainTex");

		InitRenderTexture();
		//m_UVCacluator = new DynamicSpriteCalculatorPacker(m_Material.mainTexture.width, m_Material.mainTexture.height);
		m_IsInitialized = true;

		if(m_AtlasType == eDynamicAtlasPackType.FIXED_SIZE)
		{
			//TO DO: each sprite size is the same and fixed,  and sprite uvs can be pre-calculated.
			bool is_hd = Misc.HDAtlas();
			int sprite_size = is_hd ? m_HDSpriteFixedSize : m_SDSpriteFixedSize;
			m_SpriteManager = new DynamicSpriteManagerFixedSize(m_Material.mainTexture.width, m_Material.mainTexture.height,
																sprite_size, sprite_size);
		}
		else if(m_AtlasType == eDynamicAtlasPackType.MULTI_SIZE)
		{
			//m_UVCacluator = new DynamicSpriteCalculatorPacker(m_Material.mainTexture.width, m_Material.mainTexture.height);
			m_SpriteManager = new DynamicSpriteManagerPacker(m_Material.mainTexture.width, m_Material.mainTexture.height);
		}
		else if(m_AtlasType == eDynamicAtlasPackType.POWER2_SIZE)
		{
			//TO DO: Use SquareAllocator to calculate Sprite UVs.
			m_SpriteManager = new DynamicSpriteManagerSquare(m_Material.mainTexture.width, m_Material.mainTexture.height);
		}
	}

	void InitRenderTexture()
	{
		bool is_hd = Misc.HDAtlas();
		int target_size = is_hd ? m_HDAtlasSize : m_SDAtlasSize;

		EB.Debug.Log(string.Format("=============>Create {0} atlas {1}x{2}", is_hd ? "HD" : "SD", target_size, target_size));
#if UNITY_EDITOR
        RenderTexture render_texture = new RenderTexture(target_size, target_size, 0, RenderTextureFormat.ARGB32);
#else
        RenderTexture render_texture = new RenderTexture(target_size, target_size, 0, RenderTextureFormat.ARGB1555);
#endif
        render_texture.Create();
		render_texture.name = "DynamicAtlas_" + name;

		m_RenderCamera.targetTexture = render_texture;
		m_Material.mainTexture = render_texture;

		ActiveRenderer ();
	}

	void OnDestroy()
	{
		if (m_MeshRenderers != null)
		{
			for (var i = m_MeshRenderers.size - 1; i >= 0; i--)
			{
				Destroy(m_MeshRenderers[i].material);
			}
		}

		m_MeshRenderers.Release();
		m_MeshRenderers = null;

		if(m_SpriteManager != null)
		{
			m_SpriteManager.Release();
		}
	}

	public bool IsInitialized()
	{
		return m_IsInitialized;
	}

	public void ActiveRenderer()
	{
		if(m_RenderCamera != null)
		{
			m_RenderCamera.gameObject.SetActive(true);
		}
	}

	public void OnPostRender()
	{
		if(m_RenderCamera != null)
		{
			m_RenderCamera.gameObject.SetActive(false);
			// moved to Update
			//EventDelegate.Execute(onSpriteUVsUpdate);
		}
	}

	protected MeshRenderer InstantiateMeshRenderer()
	{	
		GameObject plane = GameObject.Instantiate(m_MeshRenderTemplate) as GameObject;

		if(plane == null)
		{
			EB.Debug.LogWarning("Failed to instantiate plane");
			return null;
		}

		plane.transform.parent = m_RenderCamera.transform;
		MeshRenderer mesh_renderer = plane.GetComponent<MeshRenderer>();

		if(mesh_renderer == null)
		{
			EB.Debug.LogWarning("Failed to get mesh renderer");
			return null;
		}

		Shader shader = Shader.Find("Unlit/Transparent Colored");
		Material mat = new Material(shader);
		mesh_renderer.material = mat;

		return mesh_renderer;
	}

	void Update()
	{
		if(!m_IsInitialized)
		{
			return;
		}
		if(m_UVUpdateFlag)
		{
			DoMeshRenderer();
			m_UVUpdateFlag = false;

			// Atlas Camera will render in this frame, but OnPostRender called next frame, so notify UI to update uv here.
			EventDelegate.Execute(onSpriteUVsUpdate);
		}
	}

	public void DoMeshRenderer()
	{
		if(m_SpriteManager == null)
		{
			return;
		}

		m_SpriteManager.PackSprites();

		BetterList<DynamicSpriteData> sprite_datas = m_SpriteManager.GetDynamicSpriteDatas();
		//Debug.LogWarning ("Do Mesh Renderer for dynamic atlas!");

		int size = sprite_datas.size;
		for(int i = m_MeshRenderers.size; i < size; i++)
		{
			MeshRenderer mesh_renderer = InstantiateMeshRenderer();
			m_MeshRenderers.Add(mesh_renderer);
		}

		int rt_width = m_Material.mainTexture.width;
		int rt_height = m_Material.mainTexture.height;

		float x_ratio = 2.0f / rt_width;
		float y_ratio = 2.0f / rt_height;
		Vector3 loc_pos = Vector3.zero;
		Vector3 loc_scale = Vector3.one;

		int counter = 0;
		for(int i = 0; i < size; i++)
		{
			++counter;
			MeshRenderer mr = m_MeshRenderers[i];

			if (sprite_datas[i] == null)
			{
				mr.enabled = false;
				continue;
			}

			loc_pos.x = (sprite_datas[i].x + (sprite_datas[i].width) / 2.0f) * x_ratio - 1.0f;
			loc_pos.y = (rt_height - sprite_datas[i].y - (sprite_datas[i].height) / 2.0f) * y_ratio - 1.0f;
			loc_scale.x = x_ratio * sprite_datas[i].width;
			loc_scale.z = x_ratio * sprite_datas[i].height;

			//不推荐的写法，因为对材质属性的修改会造成额外的材质创建，请使用材质属性块（MaterialPropertyBlock）来修改材质属性
			//mr.material.mainTexture = sprite_datas[i].TextureReference;  

			mr.GetPropertyBlock(_matPropBlock);
			_matPropBlock.SetTexture(_matTextureID, sprite_datas[i].TextureReference);
			mr.SetPropertyBlock(_matPropBlock);

			mr.transform.localPosition = loc_pos;
			mr.transform.localScale = loc_scale;

			mr.enabled = true;
		}

		for(int i = counter; i < m_MeshRenderers.size; ++i)
		{
			m_MeshRenderers[i].enabled = false;
		}

		ActiveRenderer();
	}

	public void LoadDynamicSprite(string spriteName, bool is_runtime = true)
	{
		if(m_SpriteManager == null)
		{
			EB.Debug.LogError("DynamicAtlas[LoadDynamicSprite]: Sprite Manager does not exist!");
			return;
		}

		DynamicSpriteData sprite_data = null;

		if(is_runtime)
		{
			sprite_data = m_SpriteManager.TryAddSpriteReference(spriteName);
		}
		else
		{
			sprite_data = m_SpriteManager.GetSpriteData(spriteName);
		}

		if(sprite_data == null)
		{
			//bool is_hd = Misc.HDAtlas();
			//Texture2D texture = Resources.Load(string.Format("{0}/{1}", is_hd ? m_LocalHDTexturePath : m_LocalSDTexturePath, spriteName)) as Texture2D;

			//m_SpriteManager.AddDynamicSpriteData(spriteName, texture, DynamicSpriteData.eSpriteResTypes.Local_Resource, is_runtime);

			//check if there is new icon in AssetBundle
			LoadTextureAsync(spriteName);
		}
		else
		{
			if(sprite_data.UsageReference == 1)
			{
				// no need to update uv
				//m_UVUpdateFlag = true;
			}

			CurrentAsyncedTexture = spriteName;
			EventDelegate.Execute(onTextureAsyncSucceeded);
		}
	}

	public void RemoveSpriteReference(string spriteName)
	{
		if(m_SpriteManager == null)
		{
			return;
		}

		if (m_SpriteManager.GetSpriteData(spriteName) != null)
		{
			m_SpriteManager.RemoveSpriteReference(spriteName);
		}
		else
		{// for loading textures
			TextureManager.RemoveReference(spriteName, gameObject);
		}
	}

	public DynamicSpriteData GetSpriteData(string spriteName)
	{
		if(m_SpriteManager == null)
		{
			return null;
		}

		var spriteData = m_SpriteManager.GetSpriteData(spriteName);
		if (spriteData == null)
		{
			return null;
		}

		if (spriteData.RuntimeFlag && spriteData.UsageReference <= 0)
		{
			return null;
		}

		return spriteData;
	}

	void LoadTextureAsync(string spriteName)
	{
		TextureManager.GetTexture2DAsync(spriteName, LoadTextureAsyncCallback, this.gameObject);
	}

	void LoadTextureAsyncCallback(string texName, Texture2D texture, bool success)
	{
		if(m_SpriteManager == null)
		{
			EB.Debug.LogError("Dynamic Sprite Manager is null for DynamicAtlas: {0}", gameObject.name);
			return;
		}
		if(success && texture != null)
		{
			DynamicSpriteData data = m_SpriteManager.GetSpriteData(texName);
			if (data != null)
			{
				data.AddReference();
			}
			else
			{
				m_SpriteManager.AddDynamicSpriteData(texName, texture, DynamicSpriteData.eSpriteResTypes.Asset_Bundle);
				m_UVUpdateFlag = true;

				CurrentAsyncedTexture = texture.name;
				EventDelegate.Execute(onTextureAsyncSucceeded);
			}
		}
		else
		{
			//EB.Debug.LogWarning(string.Format("Load texture [{0}] from assetbundle failed.", texName));
			CurrentAsyncedTexture = texName;
			EventDelegate.Execute(onTextureAsyncFailed);
		}
	}

	public UISpriteData GetSpirteData(string spriteName)
	{
		if(m_SpriteManager == null)
		{
			return null;
		}

		return m_SpriteManager.GetSpriteData(spriteName);
	}

}
