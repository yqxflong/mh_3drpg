using UnityEngine;
using System.Collections;

/// <summary>
/// Dynamic sprite isa a textured element in the UI hierarchy.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/GAM Dynamic Sprite")]
public class DynamicUISprite : UIBasicSprite 
{
	public enum eClipType
	{
		None,
		Horizontally,
		Vertically
	}

	[HideInInspector][SerializeField] eDynamicAtlasType mDynamicAtlasType;	
	[HideInInspector][SerializeField] string			mSpriteName;
	[HideInInspector][SerializeField] GameObject		mSpinningRender;
	[HideInInspector][SerializeField] GameObject		mDefaultRender;

	[HideInInspector][SerializeField] eClipType mClipType = eClipType.None;

	[HideInInspector][SerializeField] Material mSharedDynamicMaterial;

	[System.NonSerialized]
	protected UISpriteData mSprite;
	bool mSpriteSet = false;
	private bool mLoadFlag = false;
	private bool mDACallbackFlag = false;
	private bool mGrayFlag = false;
	private Color mOrigionColor = Color.white;
	private Material mDynamicMaterial = null;
	private UISpriteData mOriginSprite = new UISpriteData();
	private UISpriteData mSwapSprite = new UISpriteData();

    private UISprite mUISprite;
    private Vector3 mDefaultPos = Vector3.zero;

    protected override void Awake()
    {
        mUISprite = GetComponent<UISprite>();//拷贝出来的需要进行赋值，不然将出现图片混乱
    }

    public bool Gray
	{
		get
		{
			return mGrayFlag;
		}
		set
		{
			if (value != mGrayFlag)
			{
				mGrayFlag = value;

				// modify depth to reduce draw call
				if (mGrayFlag)
				{
					mOrigionColor = color;
					color = new Color(1.0f, 0.0f, 1.0f);
				}
				else
				{
					color = mOrigionColor;
					mOrigionColor = Color.white;
				}
			}
		}
	}

	public eDynamicAtlasType DynamicAtlasType
	{
		get
		{
			return mDynamicAtlasType;
		}
		set
		{
			mDynamicAtlasType = value;
		}
	}

	public override Material material
	{
		get
		{
			if(DynamicAtlasManager.IsAvailable(mDynamicAtlasType))
			{
				DynamicAtlas atlas = DynamicAtlasManager.GetInstance().GetDynamicAtlas(mDynamicAtlasType);
				if(atlas != null)
				{
					if (mDynamicMaterial != null)
					{
						mDynamicMaterial.mainTexture = atlas.m_Material.mainTexture;
						return mDynamicMaterial;
					}

					return atlas.m_Material;
				}
			}
			return null;
		}
	}

    private bool isLoadHeroHead = false;

    private void setSpriteName(string value)
    {
        mSpriteName = value;
        GameEngine.Instance.LTStaticAtlas.LoadStaticAtlas(mSpriteName, succ=>{
			SetStaticSpriteAtlas();
		});
    }

    private void SetUISpriteEnabled(bool isShow)
    {
        if (this.enabled == isShow)
        {
            this.enabled = !isShow;
        }
        if (mUISprite != null)
        {
            if (mUISprite.enabled != isShow)
            {
                mUISprite.enabled = isShow;
            }
        }
    }

    public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
            if (isLoadHeroHead || GameEngine.Instance.LTStaticAtlas.IsHeadAtlas(value))
            {
                if (!string.IsNullOrEmpty(mSpriteName) && DynamicAtlasManager.IsAvailable(mDynamicAtlasType))
                {
                    UnloadDynamicSprite();
                }

                isLoadHeroHead = true;
                setSpriteName(value);

                return;
            }

            if (mUISprite != null)
            {
                SetUISpriteEnabled(false);
            }
            isLoadHeroHead = false;
            if (string.IsNullOrEmpty(value))
            {
                //If the sprite name hasn't been set yet, no need to do anything.
                if (string.IsNullOrEmpty(mSpriteName)) return;

                if (DynamicAtlasManager.IsAvailable(mDynamicAtlasType))
                {
                    UnloadDynamicSprite();
                }
                //clear the sprite name
                mSpriteName = string.Empty;
                mSprite = null;
                mChanged = true;
                mSpriteSet = false;
                mLoadFlag = true;
            }
            else if (string.Compare(mSpriteName, value) != 0)
            {
                if (DynamicAtlasManager.IsAvailable(mDynamicAtlasType))
                {
                    UnloadDynamicSprite();
                }
                //If the sprite name changes, need to check & load new sprite.
                mSpriteName = value;
                mSprite = null;
                mChanged = true;
                mSpriteSet = false;
                mLoadFlag = false;
            }
        }
	}
    
    public override Vector4 border
	{
		get
		{
			UISpriteData sp = GetAtlasSprite();
			if(sp == null) return base.border;

			return new Vector4(sp.borderLeft, sp.borderBottom, sp.borderRight, sp.borderTop);
		}
	}

	public override float pixelSize 
	{
		get
		{
			return base.pixelSize;
		}
	}

    public override Color color
    {
        get
        {
            return mColor;
        }
        set
        {
            base.color = value;
            if (mUISprite != null)
            {
                mUISprite.color = value; 
            }
        }
    }

    public override int minWidth
	{
		get
		{
			if(type == Type.Sliced || type == Type.Advanced)
			{
				Vector4 b = border * pixelSize;
				int min = Mathf.RoundToInt(b.x + b.z);

				UISpriteData sp = GetAtlasSprite();
				if(sp != null)
				{
					min += sp.paddingLeft + sp.paddingRight;
				}

				return Mathf.Max(base.minWidth, ((min & 1) == 1) ? min + 1 : min);
			}
			return base.minWidth;;
		}
	}

	public override int minHeight
	{
		get
		{
			if(type == Type.Sliced || type == Type.Advanced)
			{
				Vector4 b = border * pixelSize;
				int min = Mathf.RoundToInt(b.y + b.w);

				UISpriteData sp = GetAtlasSprite();
				if(sp != null)
				{
					min += sp.paddingTop + sp.paddingBottom;
				}

				return Mathf.Max (base.minHeight, ((min & 1) == 1) ? min + 1 : min);
			}

			return base.minHeight;
		}
	}

	/// <summary>
	/// Sprite's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
	/// This function automatically adds 1 pixel on the edge if the sprite's dimensions are not even.
	/// It's used to achieve pixel-perfect sprites even when an odd dimension sprite happens to be centered.
	/// </summary>

	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 offset = pivotOffset;
			
			float x0 = -offset.x * mWidth;
			float y0 = -offset.y * mHeight;
			float x1 = x0 + mWidth;
			float y1 = y0 + mHeight;
			
			if (GetAtlasSprite() != null && mType != Type.Tiled)
			{
				int padLeft = mSprite.paddingLeft;
				int padBottom = mSprite.paddingBottom;
				int padRight = mSprite.paddingRight;
				int padTop = mSprite.paddingTop;
				
				int w = mSprite.width + padLeft + padRight;
				int h = mSprite.height + padBottom + padTop;
				float px = 1f;
				float py = 1f;
				
				if (w > 0 && h > 0 && (mType == Type.Simple || mType == Type.Filled))
				{
					if ((w & 1) != 0) ++padRight;
					if ((h & 1) != 0) ++padTop;
					
					px = (1f / w) * mWidth;
					py = (1f / h) * mHeight;
				}
				
				if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
				{
					x0 += padRight * px;
					x1 -= padLeft * px;
				}
				else
				{
					x0 += padLeft * px;
					x1 -= padRight * px;
				}
				
				if (mFlip == Flip.Vertically || mFlip == Flip.Both)
				{
					y0 += padTop * py;
					y1 -= padBottom * py;
				}
				else
				{
					y0 += padBottom * py;
					y1 -= padTop * py;
				}
			}
			
			Vector4 br = border * pixelSize;
			
			float fw = br.x + br.z;
			float fh = br.y + br.w;
			
			float vx = Mathf.Lerp(x0, x1 - fw, mDrawRegion.x);
			float vy = Mathf.Lerp(y0, y1 - fh, mDrawRegion.y);
			float vz = Mathf.Lerp(x0 + fw, x1, mDrawRegion.z);
			float vw = Mathf.Lerp(y0 + fh, y1, mDrawRegion.w);
			
			return new Vector4(vx, vy, vz, vw);
		}
	}

	/// <summary>
	/// Whether the texture is using a premultiplied alpha material.
	/// </summary>
	
	public override bool premultipliedAlpha { get { return base.premultipliedAlpha; } }
	
	/// <summary>
	/// Retrieve the atlas sprite referenced by the spriteName field.
	/// </summary>
	
	public UISpriteData GetAtlasSprite ()
    {
        if (isLoadHeroHead)
            return null;

        if (!mSpriteSet) mSprite = null;
		
		if (mSprite == null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UISpriteData sp = null; 
				// get sprite data from dynamic atlas
				if(DynamicAtlasManager.IsAvailable(mDynamicAtlasType))
				{
					sp = DynamicAtlasManager.GetInstance().GetAtlasSprite(mSpriteName, mDynamicAtlasType);
				}
				if (sp == null) return null;
				SetAtlasSprite(sp);
			}
		}
		return mSprite;
	}
    /// <summary>
    /// Set the atlas sprite directly.
    /// </summary>

    private void LateUpdate()
    {
        if (mUISprite != null)
        {
            SetUISpriteEnabled(GameEngine.Instance.LTStaticAtlas.IsHeadAtlas(mSpriteName));
        }
    }

    protected void SetAtlasSprite (UISpriteData sp)
    {
        if (isLoadHeroHead)
            return;

        mChanged = true;
		mSpriteSet = true;
		
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = (mSprite != null) ? mSprite.name : "";
			mSprite = sp;
		}

		if (mSprite == null)
		{
			return;
		}

		// copy
		mOriginSprite.CopyFrom(mSprite);
		// clip
		mSprite = ClipAtlasSprite(mOriginSprite);
	}

	private UISpriteData ClipAtlasSprite(UISpriteData originSprite)
    {
        if (isLoadHeroHead)
            return null;

        if (originSprite == null)
		{
			return null;
		}

		if (mClipType != eClipType.None && type == Type.Simple)
		{
			UISpriteData clip_sprite = mSwapSprite;
			clip_sprite.CopyFrom(originSprite);
			clip_sprite.SetBorder(0, 0, 0, 0);
			clip_sprite.SetPadding(0, 0, 0, 0);
			if (mClipType == eClipType.Horizontally)
			{
				int keep_aspect_width = Mathf.RoundToInt((float)width / height * clip_sprite.height);
				int w = clip_sprite.width - keep_aspect_width;
				if (w < 0)
				{
					clip_sprite.SetRect(clip_sprite.x, clip_sprite.y, clip_sprite.width, clip_sprite.height);
					int padding = -w / 2;
					clip_sprite.SetPadding(padding, 0, padding, 0);
				}
				else
				{
					clip_sprite.SetRect(clip_sprite.x + w / 2, clip_sprite.y, keep_aspect_width, clip_sprite.height);
				}
			}
			else if (mClipType == eClipType.Vertically)
			{
				int keep_aspect_height = Mathf.RoundToInt((float)height / width * clip_sprite.width);
				int h = clip_sprite.height - keep_aspect_height;
				if (h < 0)
				{
					clip_sprite.SetRect(clip_sprite.x, clip_sprite.y, clip_sprite.width, clip_sprite.height);
					int padding = -h / 2;
					clip_sprite.SetPadding(padding, 0, padding, 0);
				}
				else
				{
					clip_sprite.SetRect(clip_sprite.x, clip_sprite.y + h / 2, clip_sprite.width, keep_aspect_height);
				}
			}

			return clip_sprite;
		}
		else
		{
			UISpriteData clip_sprite = mSwapSprite;
			clip_sprite.CopyFrom(originSprite);
			return clip_sprite;
		}
	}

	public override void MakePixelPerfect()
    {
        if (isLoadHeroHead)
            return;

        if (GetAtlasSprite() == null) return;
		base.MakePixelPerfect();

		if(mType == Type.Tiled) return;

		UISpriteData sp = GetAtlasSprite();

		if(sp == null) return;

		Texture tex = mainTexture;
		if(tex == null) return;

		if(mType == Type.Simple || mType == Type.Filled || !sp.hasBorder)
		{
			if(tex != null)
			{
				int x = Mathf.RoundToInt(pixelSize * (sp.width + sp.paddingLeft + sp.paddingRight));
				int y = Mathf.RoundToInt(pixelSize * (sp.height + sp.paddingTop + sp.paddingBottom));

				if((x & 1) == 1) ++x;
				if((y & 1) == 1) ++y;

				width = x;
				height = y;
			}
		}

	}

    private UISprite GetUISprite()
    {
        if (mUISprite == null)
        {
            mUISprite = gameObject.GetComponent<UISprite>();
            if (mUISprite == null)
            {
                mUISprite = gameObject.AddComponent<UISprite>();
            }
        }

        return mUISprite;
    }

    private void SetStaticSpriteAtlas()
    {
        if (GameEngine.Instance == null)
        {
            return;
        }
        if (GameEngine.Instance.gameObject != null)
        {
            if (isLoadHeroHead)
            {
                GetUISprite();


                if (string.IsNullOrEmpty(mSpriteName))
                {
                    mUISprite.atlas = null;
                    return;
                }
                SetUISpriteEnabled(true);
                mUISprite.depth = this.depth;
                mUISprite.width = this.width;
                mUISprite.height = this.height;
                mUISprite.type = this.type;
                mUISprite.gradientTop = this.mGradientTop;
                mUISprite.gradientBottom = this.mGradientBottom;
                mUISprite.color = this.color;
                mUISprite.atlas = GameEngine.Instance.LTStaticAtlas.GetStaticAtlas(mSpriteName);
                mUISprite.spriteName = mSpriteName;
                mUISprite.onPostFill -= OnUiSpritePostFill;
                mUISprite.onPostFill += OnUiSpritePostFill;
                isLoadHeroHead = false;
            }
        }
    }
    private void OnUiSpritePostFill(UIWidget widget, int bufferOffset, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color> cols)
    {
        mUISprite.autoResizeBoxCollider = true;
        mUISprite.ResizeCollider();
        //SetUISpriteEnabled(true);
    }

    // Use this for initialization
    protected override void OnInit ()
	{
        if (transform != null)
        {
            mDefaultPos = transform.localPosition;
        }

        if (mSpinningRender != null)
		{
			mSpinningRender.gameObject.SetActive(false);
		}
		if(mDefaultRender != null)
		{
			mDefaultRender.SetActive(true);
		}

		if(DynamicAtlasManager.IsAvailable(mDynamicAtlasType) && !string.IsNullOrEmpty(spriteName))
		{
			TryRegisterDACallbacks();
			TryLoadDynamicSprite();
        }

        if (mSpriteName != null)
        {
            if (GameEngine.Instance != null)
            {
                GameEngine.Instance.LTStaticAtlas.LoadStaticAtlas(mSpriteName, succ=>{
					SetStaticSpriteAtlas();
				});
            }
        }

        base.OnInit();
	}

	private void OnUVUpdated()
	{
        if (isLoadHeroHead)
            return;

		mChanged = true;
		mSprite = null;
	}

	public override void MarkAsChanged()
    {
        if (isLoadHeroHead)
            return;

        base.MarkAsChanged();
	}

	void TryRegisterDACallbacks()
    {
        if (isLoadHeroHead)
            return;

        if (!mDACallbackFlag)
		{
			DynamicAtlasManager.GetInstance().AddSpriteUVUpdateCallback(mDynamicAtlasType, OnUVUpdated);
			DynamicAtlasManager.GetInstance().AddTextureAsyncCallback(mDynamicAtlasType, OnTextureAsyncSucceeded, OnTextureAsyncFailed);
			mDACallbackFlag = true;
		}
	}

	public void TryLoadDynamicSprite()
    {
        if (isLoadHeroHead || GameEngine.Instance.LTStaticAtlas.IsHeadAtlas(spriteName))
            return;

        if (!mLoadFlag)
		{
			if(!string.IsNullOrEmpty(spriteName))
			{
				if (mSpinningRender != null)
				{
					mSpinningRender.SetActive(true);
				}
				if (mDefaultRender != null)
				{
					mDefaultRender.SetActive(true);
				}

				UISpriteData sp = DynamicAtlasManager.GetInstance().GetAtlasSprite(spriteName, mDynamicAtlasType);
				if (sp != null)
				{
					// already loaded
					if (mSpinningRender != null)
					{
						mSpinningRender.SetActive(false);
					}
					if (mDefaultRender != null)
					{
						mDefaultRender.SetActive(false);
					}
				}

				// reload, add reference
				DynamicAtlasManager.GetInstance().LoadDynamicSprite(mSpriteName, mDynamicAtlasType);
			}

			mLoadFlag = true;
		}
	}

	void UnloadDynamicSprite()
    {
        if (isLoadHeroHead)
            return;

        if (mLoadFlag)
		{
			DynamicAtlasManager.GetInstance().UnloadDynamicSprite(mSpriteName, mDynamicAtlasType);
			mLoadFlag = false;
			mSprite = null;
		}
	}

	void UnregisterDACallbacks()
    {
        if (isLoadHeroHead)
            return;

        if (mDACallbackFlag)
		{
			DynamicAtlasManager.GetInstance().RemoveSpriteUVUpdateCallback(mDynamicAtlasType, OnUVUpdated);
			DynamicAtlasManager.GetInstance().RemoveTextureAsyncCallback(mDynamicAtlasType, OnTextureAsyncSucceeded, OnTextureAsyncFailed);
			mDACallbackFlag = false;
		}
	}

	void OnTextureAsyncSucceeded()
    {
        if (isLoadHeroHead)
            return;

        if (spriteName.Equals(DynamicAtlas.CurrentAsyncedTexture)/*string.Compare(spriteName, DynamicAtlas.CurrentAsyncedTexture) == 0*/)
		{
			if(mSpinningRender != null)
			{
				mSpinningRender.gameObject.SetActive(false);
			}
			if(mDefaultRender != null)
			{
				mDefaultRender.SetActive(false);
			}
		}
	}

	void OnTextureAsyncFailed()
    {
        if (isLoadHeroHead)
            return;

        if (spriteName.Equals(DynamicAtlas.CurrentAsyncedTexture))
		{
			if(mSpinningRender != null)
			{
				mSpinningRender.gameObject.SetActive(false);
			}
		}

		DynamicSpriteData sprite_data = DynamicAtlasManager.GetInstance().GetAtlasSprite(spriteName, mDynamicAtlasType) as DynamicSpriteData;
		if(sprite_data == null || sprite_data.TextureReference == null)
		{
			if(mDefaultRender != null)
			{
				mDefaultRender.SetActive(true);
			}
		}
	}

	//use this for disable notification
	protected override void OnDisable ()
    {
        if (isLoadHeroHead)
            return;

        base.OnDisable ();

		if(DynamicAtlasManager.IsAvailable(mDynamicAtlasType) && !string.IsNullOrEmpty(spriteName))
		{
			UnloadDynamicSprite();
			UnregisterDACallbacks();
		}
	}

	// Update is called once per frame
	protected override void OnUpdate ()
	{
		base.OnUpdate ();

		if(DynamicAtlasManager.IsAvailable(mDynamicAtlasType) && !string.IsNullOrEmpty(spriteName))
		{
			TryRegisterDACallbacks();
			TryLoadDynamicSprite();
		}

		if (mChanged || !mSpriteSet)
		{
			mSpriteSet = true;
			mChanged = true;

			if (mSprite != null)
			{
				mSprite = ClipAtlasSprite(mOriginSprite);
			}
		}
    }

	/// <summary>
	/// Virtual function called by the UIPanel that fills the buffers.
	/// </summary>
	
	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color> cols)
	{
        if (isLoadHeroHead)
        {
            return;
        }
        Texture tex = mainTexture;
		if (tex == null) return;

		//if (mSprite == null || !mSprite.name.Equals(spriteName)) GetAtlasSprite();
		if (mSprite == null) GetAtlasSprite();
		if (mSprite == null) return;
		
		Rect outer = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
		Rect inner = new Rect(mSprite.x + mSprite.borderLeft, mSprite.y + mSprite.borderTop,
							  mSprite.width - mSprite.borderLeft - mSprite.borderRight,
							  mSprite.height - mSprite.borderBottom - mSprite.borderTop);
		
		outer = NGUIMath.ConvertToTexCoords(outer, tex.width, tex.height);
		inner = NGUIMath.ConvertToTexCoords(inner, tex.width, tex.height);
		
		Fill(verts, uvs, cols, outer, inner);

		if (onPostFill != null)
			onPostFill(this, verts.size, verts, uvs, cols);
	}
}
