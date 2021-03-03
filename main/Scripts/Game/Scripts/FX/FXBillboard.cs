using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FXBillboard : MonoBehaviour
{
	[HideInInspector][SerializeField] UIAtlas			m_Atlas				= null;
	[HideInInspector][SerializeField] string			m_SpriteName		= null;
	[HideInInspector][SerializeField] eDynamicAtlasType	m_DynamicAtlasType	= eDynamicAtlasType.FX;

	[HideInInspector][SerializeField] public Color	FromColor		= Color.white;
	[HideInInspector][SerializeField] public Color	ToColor			= Color.white;
	[HideInInspector][SerializeField] public float	FromSize		= 1.0f;
	[HideInInspector][SerializeField] public float	ToSize			= 1.0f;
	
	[HideInInspector][SerializeField] public int	TileX			= 1;
	[HideInInspector][SerializeField] public int	TileY			= 1;
	[HideInInspector][SerializeField] public int	MaxFrames		= 1;
	[HideInInspector][SerializeField] public float	Duration		= 1.0f;

	//
	[HideInInspector][SerializeField] public FXAtlasType AtlasType = FXAtlasType.Static;

	MeshFilter			m_Filter		= null;
	MeshRenderer		m_Renderer		= null;
	
	UISpriteData		m_SpriteData	= null;
	Mesh				m_Mesh			= null;
	Rect				m_SpriteUV;
	
	Vector3[]			m_Vertices		= new Vector3[4];
	Vector2[]			m_UVs			= new Vector2[4];
	Color32[]			m_Colors		= new Color32[4];
	int[]				m_Indices		= new int[6];
	
	bool				m_Changed			= false;
	float				m_CurrentTime		= 0.0f;
	string				m_LastSpriteName	= "";

	bool				m_DynamicSpriteLoaded = false;
	bool				m_UVCallbackAdded = false;

	void Start ()
	{
		InitOnce();
		EnableRenderer(true);
		m_Changed = true;
	}
	
	void OnEnable()
	{
		InitOnce();
		EnableRenderer(true);
		m_Changed = true;
	}
	
	void OnDisable()
	{
		ShutDown();
	}
	
	void InitOnce()
	{
		m_Filter = gameObject.GetComponent<MeshFilter>();
		if(m_Filter == null)
		{
			m_Filter = gameObject.AddComponent<MeshFilter>();
		}
		
		m_Renderer = gameObject.GetComponent<MeshRenderer>();
		if(m_Renderer == null)
		{
			m_Renderer = gameObject.AddComponent<MeshRenderer>();
		}
		
		if(m_Mesh == null)
		{
			m_Mesh = new Mesh();
			m_Mesh.name = "Mesh";
		}
	}
	
	void ShutDown()
	{
		EnableRenderer(false);

		if(AtlasType == FXAtlasType.Dynamic)
		{
			if(DynamicAtlasManager.IsAvailable(m_DynamicAtlasType))
			{
				UnloadDynamicSprite();
				UnregisterUVCallback();
			}
		}
	}

	void TryRegisterUVCallback()
	{
		if(!m_UVCallbackAdded)
		{
			DynamicAtlasManager.GetInstance().AddSpriteUVUpdateCallback(m_DynamicAtlasType, MarkAsChanged);
		}
	}

	void TryLoadDynamicSprite()
	{
		if(!m_DynamicSpriteLoaded)
		{
			if(!string.IsNullOrEmpty(m_LastSpriteName))
			{
				DynamicAtlasManager.GetInstance().UnloadDynamicSprite(m_LastSpriteName, m_DynamicAtlasType);
			}
			DynamicAtlasManager.GetInstance().LoadDynamicSprite(m_SpriteName, m_DynamicAtlasType);
			m_DynamicSpriteLoaded = true;
		}

	}

	void UnloadDynamicSprite()
	{
		if(m_DynamicSpriteLoaded)
		{
			DynamicAtlasManager.GetInstance().UnloadDynamicSprite(m_SpriteName, m_DynamicAtlasType);
			m_DynamicSpriteLoaded = false;
		}
	}

	void UnregisterUVCallback()
	{
		if(m_UVCallbackAdded)
		{
			DynamicAtlasManager.GetInstance ().RemoveSpriteUVUpdateCallback(m_DynamicAtlasType, MarkAsChanged);
			m_UVCallbackAdded = false;
		}
	}
	
	void EnableRenderer(bool enable)
	{
		if(m_Renderer != null)
		{
			m_Renderer.enabled = enable;
		}
	}

	void Update()
	{
		m_CurrentTime += Time.deltaTime;
		if(m_CurrentTime >= Duration)
		{
			m_CurrentTime = 0;
		}

		if(AtlasType == FXAtlasType.Dynamic)
		{
			if(DynamicAtlasManager.IsAvailable(m_DynamicAtlasType))
			{
				TryRegisterUVCallback();
				TryLoadDynamicSprite();
			}
		}

		if(m_Changed)
		{
			UpdateSprite();
		}
	}
	
	void LateUpdate()
	{
		if(m_SpriteData != null)
		{
			m_Mesh.Clear();

			m_Filter.mesh = null;

			float factor = m_CurrentTime / Duration;
			
			//
			float size = Mathf.Lerp(FromSize, ToSize, factor);
	
			m_Vertices[0].Set(-0.5f * size, -0.5f * size, 0);
			m_Vertices[1].Set(-0.5f * size,  0.5f * size, 0);
			m_Vertices[2].Set( 0.5f * size,  0.5f * size, 0);
			m_Vertices[3].Set( 0.5f * size, -0.5f * size, 0);

			//
			Rect uv = m_SpriteUV;
			float frame = MaxFrames * factor;
			int frame_index = (int)frame;
			int frameX = frame_index % TileX;
			int frameY = TileY - frame_index / TileY - 1;
			float stepX = m_SpriteUV.width / (float)TileX;
			float stepY = m_SpriteUV.height / (float)TileY;
			uv.Set(stepX * frameX + m_SpriteUV.x, stepY * frameY + m_SpriteUV.y, stepX, stepY);
			
			m_UVs[0].Set(uv.xMin, uv.yMin);
			m_UVs[1].Set(uv.xMin, uv.yMax);
			m_UVs[2].Set(uv.xMax, uv.yMax);
			m_UVs[3].Set(uv.xMax, uv.yMin);
			
			//
			Color color = Color.Lerp(FromColor, ToColor, factor);
			m_Colors[0] = color;
			m_Colors[1] = color;
			m_Colors[2] = color;
			m_Colors[3] = color;
			
			//
			m_Indices[0] = 0;
			m_Indices[1] = 1;
			m_Indices[2] = 2;
			m_Indices[3] = 0;
			m_Indices[4] = 2;
			m_Indices[5] = 3;

			//
			m_Mesh.vertices = m_Vertices;
			m_Mesh.uv = m_UVs;
			m_Mesh.colors32 = m_Colors;
			m_Mesh.triangles = m_Indices;
			
			m_Filter.mesh = m_Mesh;
		}
	}

	void UpdateUV()
	{
		if(AtlasType == FXAtlasType.Static)
		{
			if(m_Atlas != null && m_Renderer != null)
			{
				m_Renderer.sharedMaterial = m_Atlas.spriteMaterial;
				if(m_Renderer.sharedMaterial != null)
				{
					Texture texture = m_Renderer.sharedMaterial.mainTexture;
					if(texture != null)
					{
						m_SpriteUV.Set(m_SpriteData.x, m_SpriteData.y, m_SpriteData.width, m_SpriteData.height);
						m_SpriteUV = NGUIMath.ConvertToTexCoords(m_SpriteUV, texture.width, texture.height);
					}
				}
			}
		}
		else if(AtlasType == FXAtlasType.Dynamic)
		{
			if(m_Renderer != null)
			{
				DynamicAtlasManager dynamic_atlas_manager = DynamicAtlasManager.GetInstance();
				if(dynamic_atlas_manager != null)
				{
					DynamicAtlas the_atlas = dynamic_atlas_manager.GetDynamicAtlas(m_DynamicAtlasType);
					if(the_atlas != null)
					{
						m_Renderer.sharedMaterial = the_atlas.m_Material;
						if(m_Renderer.sharedMaterial != null)
						{
							Texture texture = m_Renderer.sharedMaterial.mainTexture;
							if(texture != null && m_SpriteData != null)
							{
								m_SpriteUV.Set(m_SpriteData.x, m_SpriteData.y, m_SpriteData.width, m_SpriteData.height);
								m_SpriteUV = NGUIMath.ConvertToTexCoords(m_SpriteUV, texture.width, texture.height);
							}
						}
					}
				}

			}
		}
	}

	public eDynamicAtlasType dynamicAtlasType
	{
		get
		{
			return m_DynamicAtlasType;
		}
		set
		{
			if(AtlasType == FXAtlasType.Dynamic)
			{
				m_DynamicAtlasType = value;
				m_SpriteData = null;
				m_Changed = true;
			}
		}
	}

	public string spriteName
	{
		get
		{
			return m_SpriteName;
		}
		
		set
		{
			if(string.Compare(m_SpriteName, value) != 0)
			{
				m_LastSpriteName = m_SpriteName;
				m_SpriteName = (value != null) ? value : "";
				m_SpriteData = null;
				m_DynamicSpriteLoaded = false;
				m_Changed = true;
			}
		}
	}
	
	public UIAtlas atlas
	{
		get
		{
			if(AtlasType == FXAtlasType.Static)
			{
				return m_Atlas;
			}
			return null;
		}
		
		set
		{
			if(AtlasType == FXAtlasType.Static)
			{
				m_Atlas = value;
				m_SpriteData = null;
				m_Changed = true;
			}
		}
	}

	void UpdateSprite()
	{
		if(AtlasType == FXAtlasType.Static)
		{
			if(m_Atlas != null)
			{
				m_SpriteData = m_Atlas.GetSprite(m_SpriteName);
				
				// can not find the sprite with the given name
				if(m_SpriteData == null)
				{
					if(m_Atlas.spriteList.Count > 0)
					{
						m_SpriteData = m_Atlas.spriteList[0];
						m_SpriteName = m_SpriteData.name;
					}
				}
				UpdateUV();
			}
		}
		else if(AtlasType == FXAtlasType.Dynamic)
		{
			UpdateUV();
		}
	}

	public void MarkAsChanged()
	{
		m_Changed = true;
		if(DynamicAtlasManager.IsAvailable(m_DynamicAtlasType))
		{
			m_SpriteData = DynamicAtlasManager.GetInstance().GetAtlasSprite(spriteName, m_DynamicAtlasType);
		}
	}
}
