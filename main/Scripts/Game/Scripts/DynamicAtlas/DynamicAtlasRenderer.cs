using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicAtlasRenderer : MonoBehaviour 
{
	public Camera						mRenderCamera;
	public Material						mDynamicMaterial;
	public Material						mDefaultSpriteMaterial;
	public GameObject					mMeshRendererTemplate;
	public string						mDefaultIconPath = "DynamicAtlases/Icons/";

	private List<MeshRenderer>			mMeshRenderers = new List<MeshRenderer>();
	private List<DynamicSpriteData>		mDynamicSpriteDatas = new List<DynamicSpriteData>();
	private List<DynamicUISprite>		mDynamicSprites = new List<DynamicUISprite>();
	private List<string>				mDirtySpriteNames = new List<string>();
	private bool						mIsInitialized = false;					
	// Use this for initialization
	void Start () 
	{
		int mat_width = mDynamicMaterial.mainTexture.width;
		int mat_height = mDynamicMaterial.mainTexture.height;

		int sprite_width = mDefaultSpriteMaterial.mainTexture.width;
		int sprite_height = mDefaultSpriteMaterial.mainTexture.height;
		int column = mat_width / sprite_width;
		int row = mat_height / sprite_height;
	
		float camera_size = mRenderCamera.orthographicSize;
		float plane_width = 2.0f * camera_size / (float)column;
		float plane_height = 2.0f * camera_size / (float)row;

		int total_count = column * row;
		Vector3 local_pos = Vector3.zero;
		Vector3 local_scale = new Vector3(plane_width, 0.0f, plane_height);
		for(int i = 0; i < total_count; i++)
		{
			MeshRenderer renderer = InstantiateMeshRenderer();
			local_pos.x = -camera_size + plane_width / 2.0f + i % column * plane_width;
			local_pos.y = camera_size - plane_height / 2.0f - i / column * plane_height;
			renderer.transform.localPosition = local_pos;
			renderer.transform.localScale = local_scale;

			mMeshRenderers.Add (renderer);

			DynamicSpriteData sprite_data = new DynamicSpriteData();
			sprite_data.width = sprite_width;
			sprite_data.height = sprite_height;
			sprite_data.x = (i % column) * sprite_width;
			sprite_data.y = (i / column) * sprite_height;
			mDynamicSpriteDatas.Add (sprite_data);
		}

		mIsInitialized = true;
	}

	void OnDestroy()
	{
		mMeshRenderers.Clear();
		mMeshRenderers = null;

		mDynamicSpriteDatas.Clear();
		mDynamicSpriteDatas = null;

		mDirtySpriteNames.Clear();
		mDirtySpriteNames = null;
	}

	public bool IsInitialized()
	{
		return mIsInitialized;
	}

	protected MeshRenderer InstantiateMeshRenderer()
	{	
		GameObject plane = GameObject.Instantiate(mMeshRendererTemplate) as GameObject;
		if(plane == null)
		{
			EB.Debug.LogWarning("Failed to instantiate plane");
			return null;
		}
		plane.transform.parent = mRenderCamera.transform;
		MeshRenderer mesh_renderer = plane.GetComponent<MeshRenderer>();
		if(mesh_renderer == null)
		{
			EB.Debug.LogWarning("Failed to get mesh renderer");
			return null;
		}
		mesh_renderer.material = mDefaultSpriteMaterial;
		return mesh_renderer;
	}

	public void ActiveRender()
	{
		if(mRenderCamera != null)
		{
			mRenderCamera.gameObject.SetActive(true);
		}
	}

	public void OnPostRender()
	{
		if(mRenderCamera != null)
		{
			mRenderCamera.gameObject.SetActive(false);
		}

		int dirty_count = mDirtySpriteNames.Count;
		int sprite_count = mDynamicSprites.Count;
		for(int i = 0; i < dirty_count; i++)
		{
			for(int j = 0; j < sprite_count; j++)
			{
				if(string.Compare(mDirtySpriteNames[i], mDynamicSprites[j].spriteName) == 0)
				{
					mDynamicSprites[j].MarkAsChanged();
				}
			}
		}
		
		mDirtySpriteNames.Clear();
	}
	

	protected void LoadTexture(string textureName, bool need_update = true)
	{
		if(!mIsInitialized)
		{
			return;
		}
		
		int index = -1;
		if(IsTextureLoaded(textureName, ref index))
		{
			mDynamicSpriteDatas[index].AddReference();
		}
		else
		{
			if(ReplaceTexture(textureName) && need_update)
			{
				ActiveRender();
			}
		}
	}


	public bool IsTextureLoaded(string textureName, ref int index)
	{
		if(!mIsInitialized)
		{
			return false;
		}

		int count = mDynamicSpriteDatas.Count;
		//index 0 is always usage for default icon.
		for(int i = 1; i < count; i++)
		{
			if(mDynamicSpriteDatas[i] != null && 
			   mDynamicSpriteDatas[i].IsInUsage() && 
			   string.Compare(textureName, mDynamicSpriteDatas[i].name) == 0
			   )
			{
				index = i;
				return true;
			}
		}
		
		return false;
	}

	bool ReplaceTexture(string textureName)
	{
		int unuse_index = -1;
		int noref_index = -1;
		int count = mDynamicSpriteDatas.Count;
		for(int i = 1; i < count; i++)
		{
			if(!mDynamicSpriteDatas[i].IsInUsage())
			{
				unuse_index = i;
				break;
			}
			else
			{
				if(mDynamicSpriteDatas[i].UsageReference == 0 && noref_index == -1)
				{
					noref_index = i;
				}
			}
		}
		bool need_load = false;
		if(unuse_index != -1)
		{
			Texture2D texture = Resources.Load(string.Format("{0}/{1}", mDefaultIconPath, textureName)) as Texture2D;
			if(texture != null)
			{
				Shader shader = mDynamicMaterial.shader;
				Material mat = new Material(shader);
				mat.mainTexture = texture;
				mMeshRenderers[unuse_index].material = mat;

				mDynamicSpriteDatas[unuse_index].name = textureName;
				mDynamicSpriteDatas[unuse_index].UsageReference = 1;
				mDynamicSpriteDatas[unuse_index].TextureReference = texture;
				mDynamicSpriteDatas[unuse_index].SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Local_Resource;
				need_load = true;
			}
			else
			{
				//just mark name & ref_count
				mDynamicSpriteDatas[unuse_index].name = textureName;
				mDynamicSpriteDatas[unuse_index].UsageReference = 1;
				mDynamicSpriteDatas[unuse_index].SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Local_Resource;
				need_load = false;
			}

			//check if there is new icon in AssetBundle
			//TextureManager.GetTexture2DAsync(textureName, LoadTextureAsyncCallback, this.gameObject);

			return need_load;
		}
		else
		{
			if(noref_index != -1)
			{
				Texture2D texture = Resources.Load(string.Format("{0}/{1}", mDefaultIconPath, textureName)) as Texture2D;
				if(texture != null)
				{
					mMeshRenderers[noref_index].material.mainTexture = texture;
					
					//Unload current replaced textures
					mDynamicSpriteDatas[noref_index].UnloadTexture();
					mDynamicSpriteDatas[noref_index].UsageReference = 1;
					mDynamicSpriteDatas[noref_index].name = textureName;
					mDynamicSpriteDatas[noref_index].TextureReference = texture;
					mDynamicSpriteDatas[noref_index].SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Local_Resource;
					need_load = true;;
				}
				else
				{
					//use defualt 
					mMeshRenderers[noref_index].material.mainTexture = mDefaultSpriteMaterial.mainTexture;
					
					//mDynamicSpriteDatas[noref_index].UnloadTexture();
					mDynamicSpriteDatas[noref_index].UsageReference = 1;
					mDynamicSpriteDatas[noref_index].name = textureName;
					//mDynamicSpriteDatas[noref_index].RefTexture = null;
					mDynamicSpriteDatas[noref_index].SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Local_Resource;
					need_load = true;;

				}

				//check if there is new icon in AssetBundle
				//TextureManager.GetTexture2DAsync(textureName, LoadTextureAsyncCallback, this.gameObject);
				return need_load;
			}
			else
			{
				EB.Debug.LogWarning("Try to render too many textures in dynamic atlases");
			}
		}
		
		return false;
	
	}

	void LoadTextureAsyncCallback(Texture2D texture, bool success)
	{
		if(success)
		{
			int count = mDynamicSpriteDatas.Count;
			for(int i = 1; i < count; i++)
			{
				if(mDynamicSpriteDatas[i] != null && 
				   string.Compare(mDynamicSpriteDatas[i].name, texture.name) == 0)
				{
					if(mMeshRenderers[i].material != mDefaultSpriteMaterial)
					{
						mMeshRenderers[i].material.mainTexture = texture;
					}
					else
					{
						Shader shader = Shader.Find("Unlit/Transparent Colored");
						Material mat = new Material(shader);
						mat.mainTexture = texture;
						mMeshRenderers[i].material = mat;
					}
					ActiveRender();

					mDynamicSpriteDatas[i].UnloadTexture();
					mDynamicSpriteDatas[i].TextureReference = texture;
					mDynamicSpriteDatas[i].SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Asset_Bundle;
					//int sprite_count = mDynamicSprites.Count;

					EB.Debug.Log(string.Format("Update dynamic sprite with texture [{0}] via loading texture form assetbundle!", texture.name));
					mDirtySpriteNames.Add(texture.name);
					break;
				}
			}
		}
		else
		{
			//Do nothing.
		}
	}
	

	public UISpriteData GetSprite(string spriteName)
	{
		int count = mDynamicSpriteDatas.Count;
		for(int i = 0; i < count; i++)
		{
			if(mDynamicSpriteDatas[i] != null && string.Compare(spriteName, mDynamicSpriteDatas[i].name) == 0)
			{
				return mDynamicSpriteDatas[i];
			}
		}
		
		return null;
	}

	public UISpriteData GetDefaultSprite()
	{
		if(mDynamicSpriteDatas.Count > 0)
		{
			return mDynamicSpriteDatas[0];
		}
		
		return null;
	}

	public void RemoveReference(string textureName)
	{
		int count = mDynamicSpriteDatas.Count;
		for(int i = 0; i < count; i++)
		{
			if(string.Compare(mDynamicSpriteDatas[i].name, textureName) == 0)
			{
				mDynamicSpriteDatas[i].RemoveReference();
				break;
			}
		}
	}

	public void LoadDynamicSprites(DynamicUISprite sprite, bool need_update = true)
	{
		if(!mDynamicSprites.Contains(sprite))
		{
			mDynamicSprites.Add(sprite);
		}
		
		LoadTexture(sprite.spriteName, need_update);
	}

	public void UnloadDynamicSprites(DynamicUISprite sprite)
	{
		mDynamicSprites.Remove(sprite);
		
		RemoveReference(sprite.spriteName);
	}

	#region Test_Functions
	//function for testcase.
	public void ResetTextureUsage()
	{
		int count = mDynamicSpriteDatas.Count;
		for(int i = 0; i < count; i++)
		{
			mDynamicSpriteDatas[i].UsageReference = 0;
		}
	}

	public void LoadTextures(string[] textureNames)
	{
		if(!mIsInitialized)
		{
			return;
		}
		
		int count = textureNames.Length;
		bool need_update = false;
		for(int i = 0; i < count; i++)
		{
			int index = -1;
			if(IsTextureLoaded(textureNames[i], ref index))
			{
				mDynamicSpriteDatas[index].AddReference();
			}
			else
			{
				if(ReplaceTexture(textureNames[i]))
				{
					need_update = true;
				}
			}
		}
		
		if(need_update)
		{
			ActiveRender();
		}
	}
	#endregion
}
