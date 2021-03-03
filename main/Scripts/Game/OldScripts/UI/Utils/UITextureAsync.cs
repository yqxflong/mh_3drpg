using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Child version of UITexture, specifically created to load textures on-demand using the TexturePoolManager
/// Because of the TexturePoolManager's Asynchronous nature an optional UISprite can be added on the same
/// component to present a placeholder image while the main texture is being loaded
/// </summary>

public class UITextureAsync : UITexture
{
	public string toLoadOnAwake;

	public List<EventDelegate> onTextureLoaded = new List<EventDelegate>();
	public List<EventDelegate> onTextureFailedToLoad = new List<EventDelegate>();

	private string _cachedTexturePath;
	private UISprite _cachedSpite;

	private bool ShowPlaceholderSprite
	{
		set
		{
			if (_cachedSpite != null)
			{
				_cachedSpite.enabled = value;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		if (Application.isPlaying)
		{
			_cachedSpite = GetComponent<UISprite>();

			if (!string.IsNullOrEmpty(toLoadOnAwake))
			{
				LoadMainTexture(toLoadOnAwake);
			}
		}
	}
	
	public void OnDestroy()
	{
		// We are going away, remove the texture reference we have
		mainTexture = null;
	}

	public void LoadMainTexture(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			// Modify the path depending on what resolution we are using
			string realPath = FixPathForResolution(path);

			ShowPlaceholderSprite = true;

			// If we already have a main texture, release it
			if (!string.IsNullOrEmpty(_cachedTexturePath))
			{
				mainTexture = null;
				TexturePoolManager.Instance.ReleaseTexture(_cachedTexturePath);
			}

			// Save the texture path for future release
			_cachedTexturePath = realPath;

			// Load the texture asynchronously and set it on the mainTexture property
			TexturePoolManager.Instance.LoadTexture(_cachedTexturePath, this, EB.SafeAction.Wrap(this, delegate(bool success, Texture2D texture){

				if (success && texture != null)
				{
					ShowPlaceholderSprite = false;

					// Make sure we set the BASE texture, because the overloaded version releases the texture
					base.mainTexture = texture;

					if (onTextureLoaded != null)
					{
						EventDelegate.Execute(onTextureLoaded);
					}
				}
				else
				{
					enabled = false;
					ShowPlaceholderSprite = true;

					if (onTextureFailedToLoad != null)
					{
						EventDelegate.Execute(onTextureFailedToLoad);
					}
				}
			}));
		}
		else
		{
			DebugSystem.Log("Can't load a texture from an empty or invalid path", LogType.Warning);
		}
	}

	private static string FixPathForResolution(string path)
	{
		if (!GameEngine.UseHDAssets)
		{
			return path.Replace("/UI/", "/UI_SD/");
		}

		return path;
	}

	// Only allow setting in the editor, for preview purposes
	public override Texture mainTexture
	{
		get
		{
			return base.mainTexture;
		}

		set
		{
			base.mainTexture = value;

			CleanupTexture();
		}
	}

	public void CleanupTexture()
	{
		if (mainTexture == null)
		{
			ShowPlaceholderSprite = true;
		}
		
		// If we already have a main texture, release it
		if (!string.IsNullOrEmpty(_cachedTexturePath))
		{
			TexturePoolManager.Instance.ReleaseTexture(_cachedTexturePath);
			
			_cachedTexturePath = string.Empty;
		}
	}
}
