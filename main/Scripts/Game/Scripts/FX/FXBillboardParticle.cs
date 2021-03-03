using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FXBillboardParticle : MonoBehaviour
{
	public struct ParticleMesh
	{
		public GameObject	m_Object;
		public Mesh			m_Mesh;
		public Vector3[]	m_Vertices;
		public Vector2[]	m_UVs;
		public Color32[]	m_Colors;
		public int[]		m_Indices;
		public MeshFilter	m_Filter;
		public MeshRenderer	m_Renderer;

		bool isInited;

		void Start()
		{
			isInited = false;
		}

		public void InitOnce(Transform parent)
		{
			if(isInited)
			{
				return;
			}

			if(m_Object == null)
			{
				m_Object = new GameObject("Particle");
				m_Object.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
				m_Object.transform.parent = parent;
				m_Object.transform.localRotation = Quaternion.identity;
				m_Filter = m_Object.AddComponent<MeshFilter>();
				m_Renderer = m_Object.AddComponent<MeshRenderer>();
			}

			if(m_Mesh == null)
			{
				m_Mesh = new Mesh();
				m_Mesh.name = "Mesh";
			}

			if(m_Vertices == null)
			{
				m_Vertices = new Vector3[4];
			}

			if(m_UVs == null)
			{
				m_UVs = new Vector2[4];
			}

			if(m_Colors == null)
			{
				m_Colors = new Color32[4];
			}

			if(m_Indices == null)
			{
				m_Indices = new int[6];
			}

			isInited = true;
		}

		public void Reset()
		{
			m_Object = null;
			m_Renderer = null;
			m_Filter = null;
			m_Mesh = null;
			m_Vertices = null;
			m_UVs = null;
			m_Colors = null;
			m_Indices = null;
			isInited = false;
		}

		public void Clear()
		{
			if(m_Mesh != null)
			{
				m_Mesh.Clear();
			}
		}

		public void PreUpdate(Transform parent)
		{
			if(!isInited)
			{
				InitOnce(parent);
			}

			Clear();
		}

		public void Update()
		{
			/*Vector3		vecUp = Camera.main.transform.rotation * Vector3.up;
			
			if (true)
				m_Object.transform.LookAt(Camera.main.transform,	vecUp);
			else m_Object.transform.LookAt(m_Object.transform.position + Camera.main.transform.rotation * Vector3.back, vecUp);*/

			if(!isInited) return;

			if(m_Mesh != null)
			{
				m_Mesh.vertices = m_Vertices;
				m_Mesh.uv = m_UVs;
				m_Mesh.colors32 = m_Colors;
				m_Mesh.triangles = m_Indices;

				if(m_Filter != null)
				{
					m_Filter.mesh = m_Mesh;
				}
			}
		}
	}


	[HideInInspector][SerializeField] UIAtlas			m_Atlas				= null;
	[HideInInspector][SerializeField] string			m_SpriteName		= null;
	[HideInInspector][SerializeField] eDynamicAtlasType	m_DynamicAtlasType	= eDynamicAtlasType.FX;

	[HideInInspector][SerializeField] public Color	FromColor	= Color.white;
	[HideInInspector][SerializeField] public Color	ToColor		= Color.white;
	[HideInInspector][SerializeField] public float	FromSize	= 1.0f;
	[HideInInspector][SerializeField] public float	ToSize		= 1.0f;

	[HideInInspector][SerializeField] public int	TileX		= 1;
	[HideInInspector][SerializeField] public int	TileY		= 1;

	//
	[HideInInspector][SerializeField] public FXAtlasType AtlasType = FXAtlasType.Static;
	
	UISpriteData		m_Sprite	= null;
	Rect				m_SpriteUV;
	
	BetterBuffer<ParticleMesh>				m_Meshes		= new BetterBuffer<ParticleMesh>();
	BetterBuffer<ParticleSystem.Particle>	m_Particles 	= new BetterBuffer<ParticleSystem.Particle>();

	bool	m_Changed			= false;
	string	m_LastSpriteName	= "";
	
	bool				m_DynamicSpriteLoaded = false;
	bool				m_UVCallbackAdded = false;

	void OnEnable()
	{
		RemoveParticleObjects();
		m_Changed = true;
	}

	void OnDisable()
	{
		if(AtlasType == FXAtlasType.Dynamic)
		{
			if(DynamicAtlasManager.IsAvailable(m_DynamicAtlasType))
			{
				UnloadDynamicSprite();
				UnregisterUVCallback();
			}
		}

		//RemoveParticleObjects();
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
	
	void Update()
	{
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
			m_Changed = false;
		}

#if UNITY_EDITOR
		UpdateGeometry();
#endif
	}

	void RemoveParticleObjects()
	{
		for(int i = 0; i < m_Meshes.Length; i++)
		{
			m_Meshes[i].Reset();
		}

		foreach(Transform child in transform)
		{
#if UNITY_EDITOR
			DestroyImmediate(child.gameObject);
#else
			Destroy(child.gameObject);
#endif
        }
	}


	void LateUpdate()
	{
#if !UNITY_EDITOR
		UpdateGeometry();
#endif
	}

	int mUpdateFrame = -1;

	void UpdateGeometry()
	{
#if UNITY_EDITOR
		if (mUpdateFrame != Time.frameCount || !Application.isPlaying)
#else
		if (mUpdateFrame != Time.frameCount)
#endif
		{
			mUpdateFrame = Time.frameCount;

			if(GetComponent<ParticleSystem>() != null)
			{
				//
				int max_particles = GetComponent<ParticleSystem>().maxParticles;
				m_Particles.Adjust(max_particles);
				bool isBufferChanged = m_Meshes.Adjust(max_particles, true);

				ParticleSystem.Particle[] particles = m_Particles.GetBuffer();
				GetComponent<ParticleSystem>().GetParticles(particles);

				//
				ParticleMesh[] meshes = m_Meshes.GetBuffer();
				int mesh_count = meshes.Length;
				int particle_count = GetComponent<ParticleSystem>().particleCount;

				if(isBufferChanged)
				{
					UnityEngine.Profiling.Profiler.BeginSample("FXBillboardParticle 001");
					for(int i = particle_count; i < mesh_count; i++)
					{
						if(meshes[i].m_Renderer != null)
						{
							meshes[i].m_Renderer.enabled = false;
						}
					}
					UnityEngine.Profiling.Profiler.EndSample();
				}

				UnityEngine.Profiling.Profiler.BeginSample("FXBillboardParticle 002");
				for(int i = 0; i < particle_count; i++)
				{
					ParticleSystem.Particle particle = particles[i];

					float factor = (particle.startLifetime - particle.remainingLifetime) / particle.startLifetime;
					float size = particle.GetCurrentSize(GetComponent<ParticleSystem>()) * Mathf.Lerp(FromSize, ToSize, factor);
					Matrix4x4 transform_matrix = Matrix4x4.TRS(particle.position,
															   Quaternion.AngleAxis(particle.rotation, -Camera.main.transform.forward),
															   new Vector3(size, size, size));

					meshes[i].PreUpdate(transform);

					if(AtlasType == FXAtlasType.Static)
					{
						if(meshes[i].m_Renderer != null && m_Atlas != null)
						{
							meshes[i].m_Renderer.sharedMaterial = m_Atlas.spriteMaterial;
							meshes[i].m_Renderer.enabled = true;
						}
					}
					else if(AtlasType == FXAtlasType.Dynamic)
					{
						DynamicAtlasManager dynamic_atlas_manager = DynamicAtlasManager.GetInstance();
						if(meshes[i].m_Renderer != null && dynamic_atlas_manager != null)
						{
							DynamicAtlas dynamic_atlas = dynamic_atlas_manager.GetDynamicAtlas(m_DynamicAtlasType);
							if(dynamic_atlas != null)
							{
								meshes[i].m_Renderer.sharedMaterial = dynamic_atlas.m_Material;
								meshes[i].m_Renderer.enabled = true;
							}
						}
					}

					if(GetComponent<ParticleSystem>().simulationSpace == ParticleSystemSimulationSpace.Local)
					{
						meshes[i].m_Vertices[0].Set(-0.5f, -0.5f, 0);
						meshes[i].m_Vertices[0] = transform_matrix.MultiplyPoint(meshes[i].m_Vertices[0]);

						meshes[i].m_Vertices[1].Set(-0.5f,  0.5f, 0);
						meshes[i].m_Vertices[1] = transform_matrix.MultiplyPoint(meshes[i].m_Vertices[1]);

						meshes[i].m_Vertices[2].Set( 0.5f,  0.5f, 0);
						meshes[i].m_Vertices[2] = transform_matrix.MultiplyPoint(meshes[i].m_Vertices[2]);

						meshes[i].m_Vertices[3].Set( 0.5f, -0.5f, 0);
						meshes[i].m_Vertices[3] = transform_matrix.MultiplyPoint(meshes[i].m_Vertices[3]);
					}
					else
					{
						Matrix4x4 world2local_matrix = transform.worldToLocalMatrix;
						
						meshes[i].m_Vertices[0].Set(-0.5f, -0.5f, 0);
						meshes[i].m_Vertices[0] = transform_matrix.MultiplyPoint(meshes[i].m_Vertices[0]);
						meshes[i].m_Vertices[0] = world2local_matrix.MultiplyPoint(meshes[i].m_Vertices[0]);

						meshes[i].m_Vertices[1].Set(-0.5f,  0.5f, 0);
						meshes[i].m_Vertices[1] = transform_matrix.MultiplyPoint(meshes[i].m_Vertices[1]);
						meshes[i].m_Vertices[1] = world2local_matrix.MultiplyPoint(meshes[i].m_Vertices[1]);

						meshes[i].m_Vertices[2].Set( 0.5f,  0.5f, 0);
						meshes[i].m_Vertices[2] = transform_matrix.MultiplyPoint(meshes[i].m_Vertices[2]);
						meshes[i].m_Vertices[2] = world2local_matrix.MultiplyPoint(meshes[i].m_Vertices[2]);
		
						meshes[i].m_Vertices[3].Set( 0.5f, -0.5f, 0);
						meshes[i].m_Vertices[3] = transform_matrix.MultiplyPoint(meshes[i].m_Vertices[3]);
						meshes[i].m_Vertices[3] = world2local_matrix.MultiplyPoint(meshes[i].m_Vertices[3]);
					}
					
					//
					Rect uv = m_SpriteUV;
					float frame = TileX * TileY * factor;
					int frame_index = (int)frame;
					int frameX = frame_index % TileX;
					int frameY = TileY - frame_index / TileY - 1;
					float stepX = m_SpriteUV.width / (float)TileX;
					float stepY = m_SpriteUV.height / (float)TileY;
					uv.Set(stepX * frameX + m_SpriteUV.x, stepY * frameY + m_SpriteUV.y, stepX, stepY);
					
					meshes[i].m_UVs[0].Set(uv.xMin, uv.yMin);
					meshes[i].m_UVs[1].Set(uv.xMin, uv.yMax);
					meshes[i].m_UVs[2].Set(uv.xMax, uv.yMax);
					meshes[i].m_UVs[3].Set(uv.xMax, uv.yMin);
					
					//
					Color color = particle.GetCurrentColor(GetComponent<ParticleSystem>()) * Color.Lerp(FromColor, ToColor, factor);
					meshes[i].m_Colors[0] = color;
					meshes[i].m_Colors[1] = color;
					meshes[i].m_Colors[2] = color;
					meshes[i].m_Colors[3] = color;

					meshes[i].m_Indices[0] = 0;
					meshes[i].m_Indices[1] = 1;
					meshes[i].m_Indices[2] = 2;
					meshes[i].m_Indices[3] = 0;
					meshes[i].m_Indices[4] = 2;
					meshes[i].m_Indices[5] = 3;

					meshes[i].Update();
				}
				UnityEngine.Profiling.Profiler.EndSample();
			}
		}
	}
	
	void UpdateUV()
	{
		if(AtlasType == FXAtlasType.Static)
		{
			if(m_Atlas != null)
			{
				Texture texture = m_Atlas.spriteMaterial.mainTexture;
				if(texture != null)
				{
					m_SpriteUV.Set(m_Sprite.x, m_Sprite.y, m_Sprite.width, m_Sprite.height);
					m_SpriteUV = NGUIMath.ConvertToTexCoords(m_SpriteUV, texture.width, texture.height);
				}
			}
		}
		else if(AtlasType == FXAtlasType.Dynamic)
		{
			DynamicAtlasManager dynamic_atlas_manager = DynamicAtlasManager.GetInstance();
			if(dynamic_atlas_manager != null)
			{
				DynamicAtlas the_atlas = dynamic_atlas_manager.GetDynamicAtlas(m_DynamicAtlasType);
				if(the_atlas != null)
				{
					Texture texture = the_atlas.m_Material.mainTexture;
					if(texture != null && m_Sprite != null)
					{
						m_SpriteUV.Set(m_Sprite.x, m_Sprite.y, m_Sprite.width, m_Sprite.height);
						m_SpriteUV = NGUIMath.ConvertToTexCoords(m_SpriteUV, texture.width, texture.height);
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
				m_Sprite = null;
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
			m_SpriteName = (value != null) ? value : "";
			m_Sprite = null;
			m_Changed = true;
			m_DynamicSpriteLoaded = false;
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
				m_Sprite = null;
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
				m_Sprite = m_Atlas.GetSprite(m_SpriteName);
				
				// can not find the sprite with the given name
				if(m_Sprite == null)
				{
					if(m_Atlas.spriteList.Count > 0)
					{
						m_Sprite = m_Atlas.spriteList[0];
						m_SpriteName = m_Sprite.name;
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
			m_Sprite = DynamicAtlasManager.GetInstance().GetAtlasSprite(m_SpriteName, m_DynamicAtlasType);
		}
	}
}
