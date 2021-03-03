//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Alignment = NGUISymbolText.Alignment;

[ExecuteInEditMode]
public class UISymbolLabel : UIWidget
{
	public enum Effect
	{
		None,
		Shadow,
		Outline,
		Outline8,
	}

	public enum Overflow
	{
		ShrinkContent,
		ClampContent,
		ResizeFreely,
		ResizeHeight,
	}

	public enum Crispness
	{
		Never,
		OnDesktop,
		Always,
	}

	public enum Modifier
	{
		None,
		ToUppercase,
		ToLowercase,
		Custom = 255,
	}

	/// <summary>
	/// Whether the label will keep its content crisp even when shrunk.
	/// You may want to turn this off on mobile devices.
	/// </summary>

	public Crispness keepCrispWhenShrunk = Crispness.OnDesktop;

	[HideInInspector][SerializeField] Font mTrueTypeFont;
	[HideInInspector][SerializeField] UIFont mFont;
#if !UNITY_3_5
	[MultilineAttribute(6)]
#endif
	[HideInInspector][SerializeField] string mText = "";
	[HideInInspector][SerializeField] int mFontSize = 16;
	[HideInInspector][SerializeField] FontStyle mFontStyle = FontStyle.Normal;
	[HideInInspector][SerializeField] Alignment mAlignment = Alignment.Automatic;
	[HideInInspector][SerializeField] bool mEncoding = true;
	[HideInInspector][SerializeField] int mMaxLineCount = 0; // 0 denotes unlimited
	[HideInInspector][SerializeField] Effect mEffectStyle = Effect.None;
	[HideInInspector][SerializeField] Color mEffectColor = Color.black;
	[HideInInspector][SerializeField] NGUISymbolText.SymbolStyle mSymbols = NGUISymbolText.SymbolStyle.Normal;
	[HideInInspector][SerializeField] Vector2 mEffectDistance = Vector2.one;
	[HideInInspector][SerializeField] Overflow mOverflow = Overflow.ShrinkContent;
	[HideInInspector][SerializeField] Material mMaterial;
	[HideInInspector][SerializeField] bool mApplyGradient = false;
	[HideInInspector][SerializeField] Color mGradientTop = Color.white;
	[HideInInspector][SerializeField] Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);
	[HideInInspector][SerializeField] int mSpacingX = 0;
	[HideInInspector][SerializeField] int mSpacingY = 0;
	[HideInInspector][SerializeField] bool mUseFloatSpacing = false;
	[HideInInspector][SerializeField] float mFloatSpacingX = 0;
	[HideInInspector][SerializeField] float mFloatSpacingY = 0;
	[HideInInspector][SerializeField] bool mOverflowEllipsis = false;
	[HideInInspector][SerializeField] int mOverflowWidth = 0;
	[HideInInspector][SerializeField] Modifier mModifier = Modifier.None;

	// Obsolete values
	[HideInInspector][SerializeField] bool mShrinkToFit = false;
	[HideInInspector][SerializeField] int mMaxLineWidth = 0;
	[HideInInspector][SerializeField] int mMaxLineHeight = 0;
	[HideInInspector][SerializeField] float mLineWidth = 0;
	[HideInInspector][SerializeField] bool mMultiline = true;

	[System.NonSerialized] Font mActiveTTF = null;
	[System.NonSerialized] float mDensity = 1f;
	[System.NonSerialized] bool mShouldBeProcessed = true;
	[System.NonSerialized] string mProcessedText = null;
	[System.NonSerialized] bool mPremultiply = false;
	[System.NonSerialized] Vector2 mCalculatedSize = Vector2.zero;
	[System.NonSerialized] float mScale = 1f;
	[System.NonSerialized] int mFinalFontSize = 0;
	[System.NonSerialized] int mLastWidth = 0;
	[System.NonSerialized] int mLastHeight = 0;

	/// <summary>
	/// Font size after modifications got taken into consideration such as shrinking content.
	/// </summary>

	public int finalFontSize
	{
		get
		{
			if (trueTypeFont) return Mathf.RoundToInt(mScale * mFinalFontSize);
			return Mathf.RoundToInt(mFinalFontSize * mScale);
		}
	}

	/// <summary>
	/// Function used to determine if something has changed (and thus the geometry must be rebuilt)
	/// </summary>

	bool shouldBeProcessed
	{
		get
		{
			return mShouldBeProcessed;
		}
		set
		{
			if (value)
			{
				mChanged = true;
				mShouldBeProcessed = true;
			}
			else
			{
				mShouldBeProcessed = false;
			}
		}
	}

	/// <summary>
	/// Whether the rectangle is anchored horizontally.
	/// </summary>

	public override bool isAnchoredHorizontally { get { return base.isAnchoredHorizontally || mOverflow == Overflow.ResizeFreely; } }

	/// <summary>
	/// Whether the rectangle is anchored vertically.
	/// </summary>

	public override bool isAnchoredVertically
	{
		get
		{
			return base.isAnchoredVertically ||
				mOverflow == Overflow.ResizeFreely ||
				mOverflow == Overflow.ResizeHeight;
		}
	}

	/// <summary>
	/// Retrieve the material used by the font.
	/// </summary>

	public override Material material
	{
		get
		{
			if (mMaterial != null) return mMaterial;
			if (mFont != null) return mFont.material;
			if (mTrueTypeFont != null) return mTrueTypeFont.material;
			return null;
		}
		set
		{
			if (mMaterial != value)
			{
				RemoveFromPanel();
				mMaterial = value;
				MarkAsChanged();
			}
		}
	}

	[Obsolete("Use UILabel.bitmapFont instead")]
	public UIFont font { get { return bitmapFont; } set { bitmapFont = value; } }

	/// <summary>
	/// Set the font used by this label.
	/// </summary>

	public UIFont bitmapFont
	{
		get
		{
			return mFont;
		}
		set
		{
			if (mFont != value)
			{
				RemoveFromPanel();
				mFont = value;
				mTrueTypeFont = null;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Set the font used by this label.
	/// </summary>

	public Font trueTypeFont
	{
		get
		{
			if (mTrueTypeFont != null) return mTrueTypeFont;
			return (mFont != null ? mFont.dynamicFont : null);
		}
		set
		{
			if (mTrueTypeFont != value)
			{
				SetActiveFont(null);
				RemoveFromPanel();
				mTrueTypeFont = value;
				shouldBeProcessed = true;
				mFont = null;
				SetActiveFont(value);
				ProcessAndRequest();
				if (mActiveTTF != null)
					base.MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Ambiguous helper function.
	/// </summary>

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			return (UnityEngine.Object)mFont ?? (UnityEngine.Object)mTrueTypeFont;
		}
		set
		{
			UIFont bf = value as UIFont;
			if (bf != null) bitmapFont = bf;
			else trueTypeFont = value as Font;
		}
	}

	/// <summary>
	/// Text that's being displayed by the label.
	/// </summary>

	public string text
	{
		get
		{
			return mText;
		}
		set
		{
			if (mText == value) return;

			if (string.IsNullOrEmpty(value))
			{
				if (value == null)
				{
					CleanSymbol();
				}

				if (!string.IsNullOrEmpty(mText))
				{
					mText = "";
					MarkAsChanged();
					ProcessAndRequest();
				}
			}
			else if (mText != value)
			{
				mText = value;
				MarkAsChanged();
				ProcessAndRequest();
			}

			if (autoResizeBoxCollider) ResizeCollider();
		}
	}

	/// <summary>
	/// Default font size.
	/// </summary>

	public int defaultFontSize { get { return (trueTypeFont != null) ? mFontSize : (mFont != null ? mFont.defaultSize : 16); } }

	/// <summary>
	/// Active font size used by the label.
	/// </summary>

	public int fontSize
	{
		get
		{
			return mFontSize;
		}
		set
		{
			value = Mathf.Clamp(value, 0, 256);

			if (mFontSize != value)
			{
				mFontSize = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	/// <summary>
	/// Dynamic font style used by the label.
	/// </summary>

	public FontStyle fontStyle
	{
		get
		{
			return mFontStyle;
		}
		set
		{
			if (mFontStyle != value)
			{
				mFontStyle = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	/// <summary>
	/// Text alignment option.
	/// </summary>

	public Alignment alignment
	{
		get
		{
			return mAlignment;
		}
		set
		{
			if (mAlignment != value)
			{
				mAlignment = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	/// <summary>
	/// Whether a gradient will be applied.
	/// </summary>

	public bool applyGradient
	{
		get
		{
			return mApplyGradient;
		}
		set
		{
			if (mApplyGradient != value)
			{
				mApplyGradient = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Top gradient color.
	/// </summary>

	public Color gradientTop
	{
		get
		{
			return mGradientTop;
		}
		set
		{
			if (mGradientTop != value)
			{
				mGradientTop = value;
				if (mApplyGradient) MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Bottom gradient color.
	/// </summary>

	public Color gradientBottom
	{
		get
		{
			return mGradientBottom;
		}
		set
		{
			if (mGradientBottom != value)
			{
				mGradientBottom = value;
				if (mApplyGradient) MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Additional horizontal spacing between characters when printing text.
	/// </summary>

	public int spacingX
	{
		get
		{
			return mSpacingX;
		}
		set
		{
			if (mSpacingX != value)
			{
				mSpacingX = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Additional vertical spacing between lines when printing text.
	/// </summary>

	public int spacingY
	{
		get
		{
			return mSpacingY;
		}
		set
		{
			if (mSpacingY != value)
			{
				mSpacingY = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether this label will use float text spacing values, instead of integers.
	/// </summary>

	public bool useFloatSpacing
	{
		get
		{
			return mUseFloatSpacing;
		}
		set
		{
			if (mUseFloatSpacing != value)
			{
				mUseFloatSpacing = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Additional horizontal spacing between characters when printing text.
	/// For this to have any effect useFloatSpacing must be true.
	/// </summary>

	public float floatSpacingX
	{
		get
		{
			return mFloatSpacingX;
		}
		set
		{
			if (!Mathf.Approximately(mFloatSpacingX, value))
			{
				mFloatSpacingX = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Additional vertical spacing between lines when printing text.
	/// For this to have any effect useFloatSpacing must be true.
	/// </summary>

	public float floatSpacingY
	{
		get
		{
			return mFloatSpacingY;
		}
		set
		{
			if (!Mathf.Approximately(mFloatSpacingY, value))
			{
				mFloatSpacingY = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Convenience property to get the used y spacing.
	/// </summary>

	public float effectiveSpacingY
	{
		get
		{
			return mUseFloatSpacing ? mFloatSpacingY : mSpacingY;
		}
	}

	/// <summary>
	/// Convenience property to get the used x spacing.
	/// </summary>

	public float effectiveSpacingX
	{
		get
		{
			return mUseFloatSpacing ? mFloatSpacingX : mSpacingX;
		}
	}

	/// <summary>
	/// Whether to append "..." at the end of clamped text that didn't fit.
	/// </summary>

	public bool overflowEllipsis
	{
		get
		{
			return mOverflowEllipsis;
		}
		set
		{
			if (mOverflowEllipsis != value)
			{
				mOverflowEllipsis = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Maximum width used when Resize Freely overflow type is enabled.
	/// If the printed text exceeds this width, it will wrap onto the following line.
	/// </summary>

	public int overflowWidth
	{
		get
		{
			return mOverflowWidth;
		}
		set
		{
			if (mOverflowWidth != value)
			{
				mOverflowWidth = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether the label will use the printed size instead of font size when printing the label.
	/// It's a dynamic font feature that will ensure that the text is crisp when shrunk.
	/// </summary>

	bool keepCrisp
	{
		get
		{
			if (trueTypeFont != null && keepCrispWhenShrunk != Crispness.Never)
			{
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY
				return (keepCrispWhenShrunk == Crispness.Always);
#else
				return true;
#endif
			}
			return false;
		}
	}

	/// <summary>
	/// Whether this label will support color encoding in the format of [RRGGBB] and new line in the form of a "\\n" string.
	/// </summary>

	public bool supportEncoding
	{
		get
		{
			return mEncoding;
		}
		set
		{
			if (mEncoding != value)
			{
				mEncoding = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Style used for symbols.
	/// </summary>

	public NGUISymbolText.SymbolStyle symbolStyle
	{
		get
		{
			return mSymbols;
		}
		set
		{
			if (mSymbols != value)
			{
				mSymbols = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Overflow method controls the label's behaviour when its content doesn't fit the bounds.
	/// </summary>

	public Overflow overflowMethod
	{
		get
		{
			return mOverflow;
		}
		set
		{
			if (mOverflow != value)
			{
				mOverflow = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Maximum width of the label in pixels.
	/// </summary>

	[System.Obsolete("Use 'width' instead")]
	public int lineWidth
	{
		get { return width; }
		set { width = value; }
	}

	/// <summary>
	/// Maximum height of the label in pixels.
	/// </summary>

	[System.Obsolete("Use 'height' instead")]
	public int lineHeight
	{
		get { return height; }
		set { height = value; }
	}

	/// <summary>
	/// Whether the label supports multiple lines.
	/// </summary>
	
	public bool multiLine
	{
		get
		{
			return mMaxLineCount != 1;
		}
		set
		{
			if ((mMaxLineCount != 1) != value)
			{
				mMaxLineCount = (value ? 0 : 1);
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Process the label's text before returning its corners.
	/// </summary>

	public override Vector3[] localCorners
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return base.localCorners;
		}
	}

	/// <summary>
	/// Process the label's text before returning its corners.
	/// </summary>

	public override Vector3[] worldCorners
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return base.worldCorners;
		}
	}

	/// <summary>
	/// Process the label's text before returning its drawing dimensions.
	/// </summary>

	public override Vector4 drawingDimensions
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return base.drawingDimensions;
		}
	}

	/// <summary>
	/// The max number of lines to be displayed for the label
	/// </summary>

	public int maxLineCount
	{
		get
		{
			return mMaxLineCount;
		}
		set
		{
			if (mMaxLineCount != value)
			{
				mMaxLineCount = Mathf.Max(value, 0);
				shouldBeProcessed = true;
				if (overflowMethod == Overflow.ShrinkContent) MakePixelPerfect();
			}
		}
	}

	/// <summary>
	/// What effect is used by the label.
	/// </summary>

	public Effect effectStyle
	{
		get
		{
			return mEffectStyle;
		}
		set
		{
			if (mEffectStyle != value)
			{
				mEffectStyle = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Color used by the effect, if it's enabled.
	/// </summary>

	public Color effectColor
	{
		get
		{
			return mEffectColor;
		}
		set
		{
			if (mEffectColor != value)
			{
				mEffectColor = value;
				if (mEffectStyle != Effect.None) shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Effect distance in pixels.
	/// </summary>

	public Vector2 effectDistance
	{
		get
		{
			return mEffectDistance;
		}
		set
		{
			if (mEffectDistance != value)
			{
				mEffectDistance = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Whether the label will automatically shrink its size in order to fit the maximum line width.
	/// </summary>

	[System.Obsolete("Use 'overflowMethod == UILabel.Overflow.ShrinkContent' instead")]
	public bool shrinkToFit
	{
		get
		{
			return mOverflow == Overflow.ShrinkContent;
		}
		set
		{
			if (value)
			{
				overflowMethod = Overflow.ShrinkContent;
			}
		}
	}

	/// <summary>
	/// Returns the processed version of 'text', with new line characters, line wrapping, etc.
	/// </summary>

	public string processedText
	{
		get
		{
			if (mLastWidth != mWidth || mLastHeight != mHeight)
			{
				mLastWidth = mWidth;
				mLastHeight = mHeight;
				mShouldBeProcessed = true;
			}

			// Process the text if necessary
			if (shouldBeProcessed) ProcessText();
			return mProcessedText;
		}
	}

	/// <summary>
	/// Actual printed size of the text, in pixels.
	/// </summary>

	public Vector2 printedSize
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return mCalculatedSize;
		}
	}

	/// <summary>
	/// Local size of the widget, in pixels.
	/// </summary>

	public override Vector2 localSize
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return base.localSize;
		}
	}

	/// <summary>
	/// Whether the label has a valid font.
	/// </summary>

	bool isValid { get { return mFont != null || mTrueTypeFont != null; } }

	/// <summary>
	/// Custom text modifier that can transform the visible text when the label's text is assigned.
	/// </summary>

	public ModifierFunc customModifier;
	public delegate string ModifierFunc (string s);

	/// <summary>
	/// Text modifier can transform the text that's actually printed, without altering the label's text value.
	/// </summary>

	public Modifier modifier
	{
		get
		{
			return mModifier;
		}
		set
		{
			if (mModifier != value)
			{
				mModifier = value;
				MarkAsChanged();
				ProcessAndRequest();
			}
		}
	}

	static BetterList<UISymbolLabel> mList = new BetterList<UISymbolLabel>();
	static Dictionary<Font, int> mFontUsage = new Dictionary<Font, int>();

	/// <summary>
	/// Register the font texture change listener.
	/// </summary>

	protected override void OnInit ()
	{
		base.OnInit();
		mList.Add(this);
		SetActiveFont(trueTypeFont);
	}

	/// <summary>
	/// Remove the font texture change listener.
	/// </summary>

	protected override void OnDisable ()
	{
		SetActiveFont(null);
		mList.Remove(this);
		base.OnDisable();
	}

	/// <summary>
	/// Set the active font, correctly setting and clearing callbacks.
	/// </summary>

	protected void SetActiveFont (Font fnt)
	{
		if (mActiveTTF != fnt)
		{
			Font font = mActiveTTF;

			if (font != null)
			{
				int usage;

				if (mFontUsage.TryGetValue(font, out usage))
				{
					usage = Mathf.Max(0, --usage);

					if (usage == 0)
					{
#if UNITY_4_3 || UNITY_4_5
						font.textureRebuildCallback = null;
#endif
						mFontUsage.Remove(font);
					}
					else mFontUsage[font] = usage;
				}
#if UNITY_4_3 || UNITY_4_5
				else font.textureRebuildCallback = null;
#endif
			}

			mActiveTTF = fnt;
			font = fnt;

			if (font != null)
			{
				int usage = 0;

				// Font hasn't been used yet? Register a change delegate callback
#if UNITY_4_3 || UNITY_4_5
				if (!mFontUsage.TryGetValue(font, out usage))
					font.textureRebuildCallback = delegate() { OnFontChanged(font); };
#endif
#if UNITY_FLASH
				mFontUsage[font] = usage + 1;
#else
				mFontUsage[font] = ++usage;
#endif
			}
		}
	}

	/// <summary>
	/// Label's actual printed text may be modified before being drawn.
	/// </summary>

	public string printedText
	{
		get
		{
			if (!string.IsNullOrEmpty(mText))
			{
				if (mModifier == Modifier.None) return mText;
				if (mModifier == Modifier.ToLowercase) return mText.ToLower();
				if (mModifier == Modifier.ToUppercase) return mText.ToUpper();
				if (mModifier == Modifier.Custom && customModifier != null) return customModifier(mText);
			}
			return mText;
		}
	}

	/// <summary>
	/// Notification called when the Unity's font's texture gets rebuilt.
	/// Unity's font has a nice tendency to simply discard other characters when the texture's dimensions change.
	/// By requesting them inside the notification callback, we immediately force them back in.
	/// Originally I was subscribing each label to the font individually, but as it turned out
	/// mono's delegate system causes an insane amount of memory allocations when += or -= to a delegate.
	/// So... queue yet another work-around.
	/// </summary>

	static void OnFontChanged (Font font)
	{
		for (int i = 0; i < mList.size; ++i)
		{
			UISymbolLabel lbl = mList[i];

			if (lbl != null)
			{
				Font fnt = lbl.trueTypeFont;

				if (fnt == font)
				{
					fnt.RequestCharactersInTexture(lbl.printedText, lbl.mFinalFontSize, lbl.mFontStyle);
					lbl.MarkAsChanged();

					if (lbl.panel == null)
						lbl.CreatePanel();

					if (mTempDrawcalls == null)
						mTempDrawcalls = new List<UIDrawCall>();

					if (lbl.drawCall != null && !mTempDrawcalls.Contains(lbl.drawCall))
						mTempDrawcalls.Add(lbl.drawCall);
				}
			}
		}

		if (mTempDrawcalls != null)
		{
			for (int i = 0, imax = mTempDrawcalls.Count; i < imax; ++i)
			{
				UIDrawCall dc = mTempDrawcalls[i];
				if (dc.panel != null) dc.panel.FillDrawCall(dc);
			}
			mTempDrawcalls.Clear();
		}
	}

	static List<UIDrawCall> mTempDrawcalls;

	/// <summary>
	/// Get the sides of the rectangle relative to the specified transform.
	/// The order is left, top, right, bottom.
	/// </summary>

	public override Vector3[] GetSides (Transform relativeTo)
	{
		if (shouldBeProcessed) ProcessText();
		return base.GetSides(relativeTo);
	}

	/// <summary>
	/// Upgrading labels is a bit different.
	/// </summary>

	protected override void UpgradeFrom265 ()
	{
		ProcessText(true, true);

		if (mShrinkToFit)
		{
			overflowMethod = Overflow.ShrinkContent;
			mMaxLineCount = 0;
		}

		if (mMaxLineWidth != 0)
		{
			width = mMaxLineWidth;
			overflowMethod = mMaxLineCount > 0 ? Overflow.ResizeHeight : Overflow.ShrinkContent;
		}
		else overflowMethod = Overflow.ResizeFreely;

		if (mMaxLineHeight != 0)
			height = mMaxLineHeight;

		if (mFont != null)
		{
			int min = mFont.defaultSize;
			if (height < min) height = min;
			fontSize = min;
		}

		mMaxLineWidth = 0;
		mMaxLineHeight = 0;
		mShrinkToFit = false;

		NGUITools.UpdateWidgetCollider(gameObject, true);
	}

	/// <summary>
	/// If the label is anchored it should not auto-resize.
	/// </summary>

	protected override void OnAnchor ()
	{
		if (mOverflow == Overflow.ResizeFreely)
		{
			if (isFullyAnchored)
				mOverflow = Overflow.ShrinkContent;
		}
		else if (mOverflow == Overflow.ResizeHeight)
		{
			if (topAnchor.target != null && bottomAnchor.target != null)
				mOverflow = Overflow.ShrinkContent;
		}
		base.OnAnchor();
	}

	/// <summary>
	/// Request the needed characters in the texture.
	/// </summary>

	void ProcessAndRequest ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying && !NGUITools.GetActive(this)) return;
		if (!mAllowProcessing) return;
#endif
		if (ambigiousFont != null) ProcessText();
	}

#if UNITY_EDITOR
	// Used to ensure that we don't process font more than once inside OnValidate function below
	[System.NonSerialized] bool mAllowProcessing = true;
	[System.NonSerialized] bool mUsingTTF = true;

	/// <summary>
	/// Validate the properties.
	/// </summary>

	protected override void OnValidate ()
	{
		base.OnValidate();

		if (NGUITools.GetActive(this))
		{
			Font ttf = mTrueTypeFont;
			UIFont fnt = mFont;

			// If the true type font was not used before, but now it is, clear the font reference
			if (!mUsingTTF && ttf != null) fnt = null;
			else if (mUsingTTF && fnt != null) ttf = null;

			mFont = null;
			mTrueTypeFont = null;
			mAllowProcessing = false;
			SetActiveFont(null);

			if (fnt != null)
			{
				bitmapFont = fnt;
				mUsingTTF = false;
			}
			else if (ttf != null)
			{
				trueTypeFont = ttf;
				mUsingTTF = true;
			}

			shouldBeProcessed = true;
			mAllowProcessing = true;
			ProcessAndRequest();
			if (autoResizeBoxCollider) ResizeCollider();
		}
	}
#endif

#if !UNITY_4_3 && !UNITY_4_5
	static bool mTexRebuildAdded = false;

	protected override void OnEnable ()
	{
		base.OnEnable();
		if (!mTexRebuildAdded)
		{
			mTexRebuildAdded = true;
			Font.textureRebuilt += OnFontChanged;
		}
	}
#endif

	/// <summary>
	/// Determine start-up values.
	/// </summary>

	protected override void OnStart ()
	{
		base.OnStart();

		// Legacy support
		if (mLineWidth > 0f)
		{
			mMaxLineWidth = Mathf.RoundToInt(mLineWidth);
			mLineWidth = 0f;
		}

		if (!mMultiline)
		{
			mMaxLineCount = 1;
			mMultiline = true;
		}

		// Whether this is a premultiplied alpha shader
		mPremultiply = (material != null && material.shader != null && material.shader.name.Contains("Premultiplied"));

		// Request the text within the font
		ProcessAndRequest();
	}

	/// <summary>
	/// UILabel needs additional processing when something changes.
	/// </summary>

	public override void MarkAsChanged ()
	{
		shouldBeProcessed = true;
		base.MarkAsChanged();
	}

	/// <summary>
	/// Process the raw text, called when something changes.
	/// </summary>

	public void ProcessText () { ProcessText(false, true); }

	/// <summary>
	/// Process the raw text, called when something changes.
	/// </summary>

	void ProcessText (bool legacyMode, bool full)
	{
		if (!isValid) return;

		mChanged = true;
		shouldBeProcessed = false;

		float regionX = mDrawRegion.z - mDrawRegion.x;
		float regionY = mDrawRegion.w - mDrawRegion.y;

		NGUISymbolText.rectWidth = legacyMode ? (mMaxLineWidth != 0 ? mMaxLineWidth : 1000000) : width;
		NGUISymbolText.rectHeight = legacyMode ? (mMaxLineHeight != 0 ? mMaxLineHeight : 1000000) : height;
		NGUISymbolText.regionWidth = (regionX != 1f) ? Mathf.RoundToInt(NGUISymbolText.rectWidth * regionX) : NGUISymbolText.rectWidth;
		NGUISymbolText.regionHeight = (regionY != 1f) ? Mathf.RoundToInt(NGUISymbolText.rectHeight * regionY) : NGUISymbolText.rectHeight;

		mFinalFontSize = Mathf.Abs(legacyMode ? Mathf.RoundToInt(cachedTransform.localScale.x) : defaultFontSize);
		mScale = 1f;

		if (NGUISymbolText.regionWidth < 1 || NGUISymbolText.regionHeight < 0)
		{
			mProcessedText = "";
			return;
		}

		bool isDynamic = (trueTypeFont != null);

		if (isDynamic && keepCrisp)
		{
			UIRoot rt = root;
			if (rt != null) mDensity = (rt != null) ? rt.pixelSizeAdjustment : 1f;
		}
		else mDensity = 1f;

		if (full) UpdateNGUIText();

		if (mOverflow == Overflow.ResizeFreely)
		{
			NGUISymbolText.rectWidth = 1000000;
			NGUISymbolText.regionWidth = 1000000;
			if (mOverflowWidth > 0)
			{
				NGUISymbolText.rectWidth = Mathf.Min(NGUISymbolText.rectWidth, mOverflowWidth);
				NGUISymbolText.regionWidth = Mathf.Min(NGUISymbolText.regionWidth, mOverflowWidth);
			}
		}

		if (mOverflow == Overflow.ResizeFreely || mOverflow == Overflow.ResizeHeight)
		{
			NGUISymbolText.rectHeight = 1000000;
			NGUISymbolText.regionHeight = 1000000;
		}

		if (mFinalFontSize > 0)
		{
			bool adjustSize = keepCrisp;

			for (int ps = mFinalFontSize; ps > 0; --ps)
			{
				// Adjust either the size, or the scale
				if (adjustSize)
				{
					mFinalFontSize = ps;
					NGUISymbolText.fontSize = mFinalFontSize;
				}
				else
				{
					mScale = (float)ps / mFinalFontSize;
					NGUISymbolText.fontScale = isDynamic ? mScale : ((float)mFontSize / mFont.defaultSize) * mScale;
				}

				NGUISymbolText.Update(false);

				// Wrap the text
				bool fits = NGUISymbolText.WrapText(printedText, out mProcessedText, true, false,
					mOverflowEllipsis && mOverflow == Overflow.ClampContent);

				if (mOverflow == Overflow.ShrinkContent && !fits)
				{
					if (--ps > 1) continue;
					else break;
				}
				else if (mOverflow == Overflow.ResizeFreely)
				{
					mCalculatedSize = NGUISymbolText.CalculatePrintedSize(mProcessedText);

					int w = Mathf.Max(minWidth, Mathf.RoundToInt(mCalculatedSize.x));
					if (regionX != 1f) w = Mathf.RoundToInt(w / regionX);
					int h = Mathf.Max(minHeight, Mathf.RoundToInt(mCalculatedSize.y));
					if (regionY != 1f) h = Mathf.RoundToInt(h / regionY);

					if ((w & 1) == 1) ++w;
					if ((h & 1) == 1) ++h;

					if (mWidth != w || mHeight != h)
					{
						mWidth = w;
						mHeight = h;
						if (onChange != null) onChange();
					}
				}
				else if (mOverflow == Overflow.ResizeHeight)
				{
					mCalculatedSize = NGUISymbolText.CalculatePrintedSize(mProcessedText);
					int h = Mathf.Max(minHeight, Mathf.RoundToInt(mCalculatedSize.y));
					if (regionY != 1f) h = Mathf.RoundToInt(h / regionY);
					if ((h & 1) == 1) ++h;

					if (mHeight != h)
					{
						mHeight = h;
						if (onChange != null) onChange();
					}
				}
				else
				{
					mCalculatedSize = NGUISymbolText.CalculatePrintedSize(mProcessedText);
				}

				// Upgrade to the new system
				if (legacyMode)
				{
					width = Mathf.RoundToInt(mCalculatedSize.x);
					height = Mathf.RoundToInt(mCalculatedSize.y);
					cachedTransform.localScale = Vector3.one;
				}
				break;
			}
		}
		else
		{
			cachedTransform.localScale = Vector3.one;
			mProcessedText = "";
			mScale = 1f;
		}
		
		if (full)
		{
			if (supportEncoding)
			{
				UpdateSymbol(mProcessedText);
			}

			NGUISymbolText.bitmapFont = null;
			NGUISymbolText.dynamicFont = null;
			NGUISymbolText.fakeSymbolHandler = null;
		}
	}

	/// <summary>
	/// Text is pixel-perfect when its scale matches the size.
	/// </summary>

	public override void MakePixelPerfect ()
	{
		if (ambigiousFont != null)
		{
			Vector3 pos = cachedTransform.localPosition;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			pos.z = Mathf.RoundToInt(pos.z);

			cachedTransform.localPosition = pos;
			cachedTransform.localScale = Vector3.one;

			if (mOverflow == Overflow.ResizeFreely)
			{
				AssumeNaturalSize();
			}
			else
			{
				int w = width;
				int h = height;

				Overflow over = mOverflow;
				if (over != Overflow.ResizeHeight) mWidth = 100000;
				mHeight = 100000;

				mOverflow = Overflow.ShrinkContent;
				ProcessText(false, true);
				mOverflow = over;

				int minX = Mathf.RoundToInt(mCalculatedSize.x);
				int minY = Mathf.RoundToInt(mCalculatedSize.y);

				minX = Mathf.Max(minX, base.minWidth);
				minY = Mathf.Max(minY, base.minHeight);

				if ((minX & 1) == 1) ++minX;
				if ((minY & 1) == 1) ++minY;

				mWidth = Mathf.Max(w, minX);
				mHeight = Mathf.Max(h, minY);

				MarkAsChanged();
			}
		}
		else base.MakePixelPerfect();
	}

	/// <summary>
	/// Make the label assume its natural size.
	/// </summary>

	public void AssumeNaturalSize ()
	{
		if (ambigiousFont != null)
		{
			mWidth = 100000;
			mHeight = 100000;
			ProcessText(false, true);
			mWidth = Mathf.RoundToInt(mCalculatedSize.x);
			mHeight = Mathf.RoundToInt(mCalculatedSize.y);
			if ((mWidth & 1) == 1) ++mWidth;
			if ((mHeight & 1) == 1) ++mHeight;
			MarkAsChanged();
		}
	}

	[System.Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex (Vector3 worldPos) { return GetCharacterIndexAtPosition(worldPos, false); }

	[System.Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex (Vector2 localPos) { return GetCharacterIndexAtPosition(localPos, false); }

	static BetterList<Vector3> mTempVerts = new BetterList<Vector3>();
	static BetterList<int> mTempIndices = new BetterList<int>();

	/// <summary>
	/// Return the index of the character at the specified world position.
	/// </summary>

	public int GetCharacterIndexAtPosition (Vector3 worldPos, bool precise)
	{
		Vector2 localPos = cachedTransform.InverseTransformPoint(worldPos);
		return GetCharacterIndexAtPosition(localPos, precise);
	}

	/// <summary>
	/// Return the index of the character at the specified local position.
	/// </summary>

	public int GetCharacterIndexAtPosition (Vector2 localPos, bool precise)
	{
		if (isValid)
		{
			string text = processedText;
			if (string.IsNullOrEmpty(text)) return 0;

			UpdateNGUIText();

			if (precise) NGUISymbolText.PrintExactCharacterPositions(text, mTempVerts, mTempIndices);
			else NGUISymbolText.PrintApproximateCharacterPositions(text, mTempVerts, mTempIndices);

			if (mTempVerts.size > 0)
			{
				ApplyOffset(mTempVerts, 0);
				int retVal = precise ?
					NGUISymbolText.GetExactCharacterIndex(mTempVerts, mTempIndices, localPos) :
					NGUISymbolText.GetApproximateCharacterIndex(mTempVerts, mTempIndices, localPos);

				mTempVerts.Clear();
				mTempIndices.Clear();

				NGUISymbolText.bitmapFont = null;
				NGUISymbolText.dynamicFont = null;
				NGUISymbolText.fakeSymbolHandler = null;
				return retVal;
			}

			NGUISymbolText.bitmapFont = null;
			NGUISymbolText.dynamicFont = null;
			NGUISymbolText.fakeSymbolHandler = null;
		}
		return 0;
	}

	/// <summary>
	/// Retrieve the word directly below the specified world-space position.
	/// </summary>

	public string GetWordAtPosition (Vector3 worldPos)
	{
		int index = GetCharacterIndexAtPosition(worldPos, true);
		return GetWordAtCharacterIndex(index);
	}

	/// <summary>
	/// Retrieve the word directly below the specified relative-to-label position.
	/// </summary>

	public string GetWordAtPosition (Vector2 localPos)
	{
		int index = GetCharacterIndexAtPosition(localPos, true);
		return GetWordAtCharacterIndex(index);
	}

	/// <summary>
	/// Retrieve the word right under the specified character index.
	/// </summary>

	public string GetWordAtCharacterIndex (int characterIndex)
	{
		string s = printedText;

		if (characterIndex != -1 && characterIndex < s.Length)
		{
#if UNITY_FLASH
			int wordStart = LastIndexOfAny(s, new char[] { ' ', '\n' }, characterIndex) + 1;
			int wordEnd = IndexOfAny(s, new char[] { ' ', '\n', ',', '.' }, characterIndex);
#else
			int wordStart = s.LastIndexOfAny(new char[] {' ', '\n'}, characterIndex) + 1;
			int wordEnd = s.IndexOfAny(new char[] { ' ', '\n', ',', '.' }, characterIndex);
#endif
			if (wordEnd == -1) wordEnd = s.Length;

			if (wordStart != wordEnd)
			{
				int len = wordEnd - wordStart;

				if (len > 0)
				{
					string word = s.Substring(wordStart, len);
					return NGUISymbolText.StripSymbols(word);
				}
			}
		}
		return null;
	}

#if UNITY_FLASH
	/// <summary>
	/// Flash is fail IRL: http://www.tasharen.com/forum/index.php?topic=11390.0
	/// </summary>

	int LastIndexOfAny (string input, char[] any, int start)
	{
		if (start >= 0 && input.Length > 0 && any.Length > 0 && start < input.Length)
		{
			for (int w = start; w >= 0; w--)
			{
				for (int r = 0; r < any.Length; r++)
				{
					if (any[r] == input[w])
					{
						return w;
					}
				}
			}
		}
		return -1;
	}

	/// <summary>
	/// Flash is fail IRL: http://www.tasharen.com/forum/index.php?topic=11390.0
	/// </summary>

	int IndexOfAny (string input, char[] any, int start)
	{
		if (start >= 0 && input.Length > 0 && any.Length > 0 && start < input.Length)
		{
			for (int w = start; w < input.Length; w++)
			{
				for (int r = 0; r < any.Length; r++)
				{
					if (any[r] == input[w])
					{
						return w;
					}
				}
			}
		}
		return -1;
	}
#endif

	/// <summary>
	/// Retrieve the URL directly below the specified world-space position.
	/// </summary>

	public string GetUrlAtPosition (Vector3 worldPos) { return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(worldPos, true)); }

	/// <summary>
	/// Retrieve the URL directly below the specified relative-to-label position.
	/// </summary>

	public string GetUrlAtPosition (Vector2 localPos) { return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(localPos, true)); }

	/// <summary>
	/// Retrieve the URL right under the specified character index.
	/// </summary>

	public string GetUrlAtCharacterIndex (int characterIndex)
	{
		string s = printedText;

		if (characterIndex != -1 && characterIndex < s.Length - 6)
		{
			int linkStart;

			// LastIndexOf() fails if the string happens to begin with the expected text
			if (s[characterIndex] == '[' &&
				s[characterIndex + 1] == 'u' &&
				s[characterIndex + 2] == 'r' &&
				s[characterIndex + 3] == 'l' &&
				s[characterIndex + 4] == '=')
			{
				linkStart = characterIndex;
			}
			else linkStart = s.LastIndexOf("[url=", characterIndex);
			
			if (linkStart == -1) return null;

			linkStart += 5;
			int linkEnd = s.IndexOf("]", linkStart);
			if (linkEnd == -1) return null;

			int urlEnd = s.IndexOf("[/url]", linkEnd);
			if (urlEnd == -1 || characterIndex <= urlEnd)
				return s.Substring(linkStart, linkEnd - linkStart);
		}
		return null;
	}

	/// <summary>
	/// Get the index of the character on the line directly above or below the current index.
	/// </summary>

	public int GetCharacterIndex (int currentIndex, KeyCode key)
	{
		if (isValid)
		{
			string text = processedText;
			if (string.IsNullOrEmpty(text)) return 0;

			int def = defaultFontSize;
			UpdateNGUIText();

			NGUISymbolText.PrintApproximateCharacterPositions(text, mTempVerts, mTempIndices);

			if (mTempVerts.size > 0)
			{
				ApplyOffset(mTempVerts, 0);

				for (int i = 0; i < mTempIndices.size; ++i)
				{
					if (mTempIndices[i] == currentIndex)
					{
						// Determine position on the line above or below this character
						Vector2 localPos = mTempVerts[i];

						if (key == KeyCode.UpArrow) localPos.y += def + effectiveSpacingY;
						else if (key == KeyCode.DownArrow) localPos.y -= def + effectiveSpacingY;
						else if (key == KeyCode.Home) localPos.x -= 1000f;
						else if (key == KeyCode.End) localPos.x += 1000f;

						// Find the closest character to this position
						int retVal = NGUISymbolText.GetApproximateCharacterIndex(mTempVerts, mTempIndices, localPos);
						if (retVal == currentIndex) break;

						mTempVerts.Clear();
						mTempIndices.Clear();
						return retVal;
					}
				}
				mTempVerts.Clear();
				mTempIndices.Clear();
			}

			NGUISymbolText.bitmapFont = null;
			NGUISymbolText.dynamicFont = null;
			NGUISymbolText.fakeSymbolHandler = null;

			// If the selection doesn't move, then we're at the top or bottom-most line
			if (key == KeyCode.UpArrow || key == KeyCode.Home) return 0;
			if (key == KeyCode.DownArrow || key == KeyCode.End) return text.Length;
		}
		return currentIndex;
	}

	public int GetCorrectTagIndex(string text, int currentIndex, out int tail)
	{
		tail = currentIndex;

		if (isValid && supportEncoding)
		{
			if (string.IsNullOrEmpty(text)) return 0;

			UpdateNGUIText();

			currentIndex = NGUISymbolText.GetCorrectSymbolIndex(text, currentIndex, out tail);

			NGUISymbolText.bitmapFont = null;
			NGUISymbolText.dynamicFont = null;
			NGUISymbolText.fakeSymbolHandler = null;

			return currentIndex;
		}

		return currentIndex;
	}

	public int GetCorrectCharacterIndex(string text, int currentIndex, out int previousIndex, out int nextIndex)
	{
		previousIndex = currentIndex;
		nextIndex = currentIndex;

		if (isValid && supportEncoding)
		{
			if (string.IsNullOrEmpty(text)) return 0;

			UpdateNGUIText();

			NGUISymbolText.PrintApproximateCharacterPositions(text, mTempVerts, mTempIndices);

			if (mTempVerts.size > 0)
			{
				int minIndex = currentIndex;
				int minDiff = int.MaxValue;
				int i = 0;
				for (; i < mTempIndices.size; ++i)
				{
					int diff = Mathf.Abs(mTempIndices[i] - currentIndex);
					if (diff < minDiff)
					{
						minDiff = diff;
						minIndex = i;

						if (minDiff == 0)
						{
							break;
						}
					}
				}
				currentIndex = mTempIndices[minIndex];

				for (int j = minIndex - 1; j >= 0; --j)
				{
					if (mTempIndices[j] < currentIndex && (mTempVerts[j].y > mTempVerts[minIndex].y || mTempVerts[j].x < mTempVerts[minIndex].x))
					{
						previousIndex = mTempIndices[j];
						break;
					}
				}

				for (int j = minIndex + 1; j < mTempIndices.size; ++j)
				{
					if (mTempIndices[j] > currentIndex && (mTempVerts[j].y < mTempVerts[minIndex].y || mTempVerts[j].x > mTempVerts[minIndex].x))
					{
						nextIndex = mTempIndices[j];
						break;
					}
				}

				mTempVerts.Clear();
				mTempIndices.Clear();

				NGUISymbolText.bitmapFont = null;
				NGUISymbolText.dynamicFont = null;
				NGUISymbolText.fakeSymbolHandler = null;
				return currentIndex;
			}

			NGUISymbolText.bitmapFont = null;
			NGUISymbolText.dynamicFont = null;
			NGUISymbolText.fakeSymbolHandler = null;
		}
		return currentIndex;
	}

	public string ProfanityFilter(string text, Func<string, string> filterFunc)
	{
		if (isValid && supportEncoding)
		{
			if (string.IsNullOrEmpty(text)) return filterFunc(text);

			UpdateNGUIText();

			text = NGUISymbolText.ProfanityFilter(text, filterFunc);

			NGUISymbolText.bitmapFont = null;
			NGUISymbolText.dynamicFont = null;
			NGUISymbolText.fakeSymbolHandler = null;

			return text;
		}

		return filterFunc(text);
	}

	/// <summary>
	/// Fill the specified geometry buffer with vertices that would highlight the current selection.
	/// </summary>

	public void PrintOverlay (int start, int end, UIGeometry caret, UIGeometry highlight, Color caretColor, Color highlightColor)
	{
		if (caret != null) caret.Clear();
		if (highlight != null) highlight.Clear();
		if (!isValid) return;

		string text = processedText;
		UpdateNGUIText();

		int startingCaretVerts = caret.verts.size;
		Vector2 center = new Vector2(0.5f, 0.5f);
		float alpha = finalAlpha;

		// If we have a highlight to work with, fill the buffer
		if (highlight != null && start != end)
		{
			int startingVertices = highlight.verts.size;
			NGUISymbolText.PrintCaretAndSelection(text, start, end, caret.verts, highlight.verts);

			if (highlight.verts.size > startingVertices)
			{
				ApplyOffset(highlight.verts, startingVertices);

				Color c = new Color(highlightColor.r, highlightColor.g, highlightColor.b, highlightColor.a * alpha);

				for (int i = startingVertices; i < highlight.verts.size; ++i)
				{
					highlight.uvs.Add(center);
					highlight.cols.Add(c);
				}
			}
		}
		else NGUISymbolText.PrintCaretAndSelection(text, start, end, caret.verts, null);

		// Fill the caret UVs and colors
		ApplyOffset(caret.verts, startingCaretVerts);
		Color cc = new Color(caretColor.r, caretColor.g, caretColor.b, caretColor.a * alpha);

		for (int i = startingCaretVerts; i < caret.verts.size; ++i)
		{
			caret.uvs.Add(center);
			caret.cols.Add(cc);
		}

		NGUISymbolText.bitmapFont = null;
		NGUISymbolText.dynamicFont = null;
		NGUISymbolText.fakeSymbolHandler = null;
	}

	/// <summary>
	/// Draw the label.
	/// </summary>

	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color> cols)
	{
		if (!isValid) return;

		int offset = verts.size;
		Color col = color;
		col.a = finalAlpha;
		
		if (mFont != null && mFont.premultipliedAlphaShader) col = NGUITools.ApplyPMA(col);

		string text = processedText;
		int start = verts.size;

		UpdateNGUIText();

		NGUISymbolText.tint = col;
		NGUISymbolText.Print(text, verts, uvs, cols);
		NGUISymbolText.bitmapFont = null;
		NGUISymbolText.dynamicFont = null;
		NGUISymbolText.fakeSymbolHandler = null;

		// Center the content within the label vertically
		Vector2 pos = ApplyOffset(verts, start);

		// Effects don't work with packed fonts
		if (mFont != null && mFont.packedFontShader) return;

		// Apply an effect if one was requested
		if (effectStyle != Effect.None)
		{
			int end = verts.size;
			pos.x = mEffectDistance.x;
			pos.y = mEffectDistance.y;

			ApplyShadow(verts, uvs, cols, offset, end, pos.x, -pos.y);

			if ((effectStyle == Effect.Outline) || (effectStyle == Effect.Outline8))
			{
				offset = end;
				end = verts.size;

				ApplyShadow(verts, uvs, cols, offset, end, -pos.x, pos.y);

				offset = end;
				end = verts.size;

				ApplyShadow(verts, uvs, cols, offset, end, pos.x, pos.y);

				offset = end;
				end = verts.size;

				ApplyShadow(verts, uvs, cols, offset, end, -pos.x, -pos.y);

				if (effectStyle == Effect.Outline8)
				{
					offset = end;
					end = verts.size;

					ApplyShadow(verts, uvs, cols, offset, end, -pos.x, 0);

					offset = end;
					end = verts.size;

					ApplyShadow(verts, uvs, cols, offset, end, pos.x, 0);

					offset = end;
					end = verts.size;

					ApplyShadow(verts, uvs, cols, offset, end, 0, pos.y);

					offset = end;
					end = verts.size;

					ApplyShadow(verts, uvs, cols, offset, end, 0, -pos.y);
				}
			}
		}

		if (onPostFill != null)
			onPostFill(this, offset, verts, uvs, cols);
	}

	/// <summary>
	/// Align the vertices, making the label positioned correctly based on the pivot.
	/// Returns the offset that was applied.
	/// </summary>

	public Vector2 ApplyOffset (BetterList<Vector3> verts, int start)
	{
		Vector2 po = pivotOffset;

		float fx = Mathf.Lerp(0f, -mWidth, po.x);
		float fy = Mathf.Lerp(mHeight, 0f, po.y) + Mathf.Lerp((mCalculatedSize.y - mHeight), 0f, po.y);

		fx = Mathf.Round(fx);
		fy = Mathf.Round(fy);

#if UNITY_FLASH
		for (int i = start; i < verts.size; ++i)
		{
			Vector3 buff = verts.buffer[i];
			buff.x += fx;
			buff.y += fy;
			verts.buffer[i] = buff;
		}
#else
		for (int i = start; i < verts.size; ++i)
		{
			verts.buffer[i].x += fx;
			verts.buffer[i].y += fy;
		}
#endif
		return new Vector2(fx, fy);
	}

	/// <summary>
	/// Apply a shadow effect to the buffer.
	/// </summary>

	public void ApplyShadow (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color> cols, int start, int end, float x, float y)
	{
		Color c = mEffectColor;
		c.a *= finalAlpha;
		if (bitmapFont != null && bitmapFont.premultipliedAlphaShader) c = NGUITools.ApplyPMA(c);
		Color col = c;

		for (int i = start; i < end; ++i)
		{
			verts.Add(verts.buffer[i]);
			uvs.Add(uvs.buffer[i]);
			cols.Add(cols.buffer[i]);

			Vector3 v = verts.buffer[i];
			v.x += x;
			v.y += y;
			verts.buffer[i] = v;

			Color uc = cols.buffer[i];

			if (uc.a == 1f)
			{
				cols.buffer[i] = col;
			}
			else
			{
				Color fc = c;
				fc.a = uc.a * c.a;
				cols.buffer[i] = fc;
			}
		}
	}

	/// <summary>
	/// Calculate the character index offset necessary in order to print the end of the specified text.
	/// </summary>

	public int CalculateOffsetToFit (string text)
	{
		UpdateNGUIText();
		//NGUISymbolText.encoding = false;
		//NGUISymbolText.symbolStyle = NGUISymbolText.SymbolStyle.None;
		int offset = NGUISymbolText.CalculateOffsetToFit(text);
		NGUISymbolText.bitmapFont = null;
		NGUISymbolText.dynamicFont = null;
		NGUISymbolText.fakeSymbolHandler = null;
		return offset;
	}

	/// <summary>
	/// Convenience function, in case you wanted to associate progress bar, slider or scroll bar's
	/// OnValueChanged function in inspector with a label.
	/// </summary>

	public void SetCurrentProgress ()
	{
		if (UIProgressBar.current != null)
			text = UIProgressBar.current.value.ToString("F");
	}

	/// <summary>
	/// Convenience function, in case you wanted to associate progress bar, slider or scroll bar's
	/// OnValueChanged function in inspector with a label.
	/// </summary>

	public void SetCurrentPercent ()
	{
		if (UIProgressBar.current != null)
			text = Mathf.RoundToInt(UIProgressBar.current.value * 100f) + "%";
	}

	/// <summary>
	/// Convenience function, in case you wanted to automatically set some label's text
	/// by selecting a value in the UIPopupList.
	/// </summary>

	public void SetCurrentSelection ()
	{
		if (UIPopupList.current != null)
		{
			text = UIPopupList.current.isLocalized ?
				Localization.Get(UIPopupList.current.value) :
				UIPopupList.current.value;
		}
	}

	/// <summary>
	/// Convenience function -- wrap the current text given the label's settings and unlimited height.
	/// </summary>

	public bool Wrap (string text, out string final) { return Wrap(text, out final, 1000000); }

	/// <summary>
	/// Convenience function -- wrap the current text given the label's settings and the given height.
	/// </summary>

	public bool Wrap (string text, out string final, int height)
	{
		UpdateNGUIText();
		NGUISymbolText.rectHeight = height;
		NGUISymbolText.regionHeight = height;
		bool retVal = NGUISymbolText.WrapText(text, out final);
		NGUISymbolText.bitmapFont = null;
		NGUISymbolText.dynamicFont = null;
		NGUISymbolText.fakeSymbolHandler = null;
		return retVal;
	}

	/// <summary>
	/// Update NGUISymbolText.current with all the properties from this label.
	/// </summary>

	public void UpdateNGUIText ()
	{
		Font ttf = trueTypeFont;
		bool isDynamic = (ttf != null);

		NGUISymbolText.fontSize = mFinalFontSize;
		NGUISymbolText.fontStyle = mFontStyle;
		NGUISymbolText.rectWidth = mWidth;
		NGUISymbolText.rectHeight = mHeight;
		NGUISymbolText.regionWidth = Mathf.RoundToInt(mWidth * (mDrawRegion.z - mDrawRegion.x));
		NGUISymbolText.regionHeight = Mathf.RoundToInt(mHeight * (mDrawRegion.w - mDrawRegion.y));
		NGUISymbolText.gradient = mApplyGradient && (mFont == null || !mFont.packedFontShader);
		NGUISymbolText.gradientTop = mGradientTop;
		NGUISymbolText.gradientBottom = mGradientBottom;
		NGUISymbolText.encoding = mEncoding;
		NGUISymbolText.premultiply = mPremultiply;
		NGUISymbolText.symbolStyle = mSymbols;
		NGUISymbolText.maxLines = mMaxLineCount;
		NGUISymbolText.spacingX = effectiveSpacingX;
		NGUISymbolText.spacingY = effectiveSpacingY;
		NGUISymbolText.fontScale = isDynamic ? mScale : ((float)mFontSize / mFont.defaultSize) * mScale;

		InitSymbol();
		NGUISymbolText.fakeSymbolHandler = GetFakeSymbol;

		if (mFont != null)
		{
			NGUISymbolText.bitmapFont = mFont;

			for (;;)
			{
				UIFont fnt = NGUISymbolText.bitmapFont.replacement;
				if (fnt == null) break;
				NGUISymbolText.bitmapFont = fnt;
			}

			if (NGUISymbolText.bitmapFont.isDynamic)
			{
				NGUISymbolText.dynamicFont = NGUISymbolText.bitmapFont.dynamicFont;
				NGUISymbolText.bitmapFont = null;
			}
			else NGUISymbolText.dynamicFont = null;
		}
		else
		{
			NGUISymbolText.dynamicFont = ttf;
			NGUISymbolText.bitmapFont = null;
		}

		if (isDynamic && keepCrisp)
		{
			UIRoot rt = root;
			if (rt != null) NGUISymbolText.pixelDensity = (rt != null) ? rt.pixelSizeAdjustment : 1f;
		}
		else NGUISymbolText.pixelDensity = 1f;

		if (mDensity != NGUISymbolText.pixelDensity)
		{
			ProcessText(false, false);
			NGUISymbolText.rectWidth = mWidth;
			NGUISymbolText.rectHeight = mHeight;
			NGUISymbolText.regionWidth = Mathf.RoundToInt(mWidth * (mDrawRegion.z - mDrawRegion.x));
			NGUISymbolText.regionHeight = Mathf.RoundToInt(mHeight * (mDrawRegion.w - mDrawRegion.y));
		}

		{
			UIRoot rt = root;
			if (rt != null) NGUISymbolText.pixelSizeAdjustment = (rt != null) ? rt.pixelSizeAdjustment : NGUISymbolText.pixelDensity;
		}

		if (alignment == Alignment.Automatic)
		{
			Pivot p = pivot;

			if (p == Pivot.Left || p == Pivot.TopLeft || p == Pivot.BottomLeft)
			{
				NGUISymbolText.alignment = Alignment.Left;
			}
			else if (p == Pivot.Right || p == Pivot.TopRight || p == Pivot.BottomRight)
			{
				NGUISymbolText.alignment = Alignment.Right;
			}
			else NGUISymbolText.alignment = Alignment.Center;
		}
		else NGUISymbolText.alignment = alignment;

		NGUISymbolText.Update();
	}

	void OnApplicationPause(bool paused)
	{
		if (!paused && mTrueTypeFont != null) Invalidate(false);
	}

	#region symbol extend
	public abstract class Symbol
	{
		public abstract string Sequence();
	}

	public abstract class FakeSymbol : Symbol
	{
		public int Offset { get; set; }
		public int Index { get; set; }
		public BMSymbol Symbol { get; protected set; }
		public abstract void Create(BMSymbol symbol, Rect rect, GameObject parent, UIAtlas atlas);
		public abstract void Destroy();
		public abstract void Enable(bool enable);
	}

	public class UrlSymbol : Symbol
	{
		public Hashtable Args
		{
			get; private set;
		}

		public string Type
		{
			get; private set;
		}

		public UrlSymbol()
		{
			Args = Johny.HashtablePool.Claim();
		}

		public UrlSymbol(string name, string type, Hashtable args)
		{
			Args = args;
			Type = type;
			Args["name"] = Args["name"] ?? name;
		}

		public override string Sequence()
		{
			string query = EB.QueryString.Stringify(Args);

			EB.Uri uri = new EB.Uri();
			uri.SetComponent(EB.Uri.Component.Protocol, "chat");
			uri.SetComponent(EB.Uri.Component.Path, "/" + Type);
			uri.SetComponent(EB.Uri.Component.Query, query);

			return string.Format("[url=%s]%s[/u]", uri, Args["name"].ToString());
		}

		public static UrlSymbol Parse(string sequence)
		{
			EB.Uri uri = new EB.Uri(sequence);
			if (!uri.Scheme.StartsWith("chat"))
			{
				return null;
			}

			Hashtable args = EB.QueryString.Parse(uri.Query);
			return new UrlSymbol(args["name"].ToString(), uri.Path, args);
		}
	}

	public class SpriteSymbol : FakeSymbol
	{
		public string SpriteName
		{
			get; private set;
		}

		private UISprite fakeSprite;

		public SpriteSymbol(string spriteName)
		{
			SpriteName = spriteName;
		}

		public override string Sequence()
		{
			return string.Format("${{{0}}}", SpriteName);
		}

		public static SpriteSymbol Parse(string sequence)
		{
			Regex reg = new Regex(@"\$\{([-_ 0-9a-zA-Z]+)\}");
			Match mat = reg.Match(sequence);
			if (string.IsNullOrEmpty(mat.Value))
			{
				return null;
			}

			string spriteName = mat.Groups[1].Value;
			if (string.IsNullOrEmpty(spriteName))
			{
				return null;
			}

			return new SpriteSymbol(spriteName);
		}

		public override void Create(BMSymbol symbol, Rect rect, GameObject parent, UIAtlas atlas)
		{
			Symbol = symbol;

			if (fakeSprite == null)
			{
				fakeSprite = NGUITools.AddSprite(parent, atlas, Symbol.spriteName);
			}

			var spriteData = atlas.GetSprite(Symbol.spriteName);
			if (spriteData != null)
			{
				fakeSprite.keepAspectRatio = AspectRatioSource.BasedOnHeight;
				fakeSprite.aspectRatio = spriteData.width / (float)spriteData.height;
			}

			fakeSprite.SetRect(rect.x, rect.y, rect.width, rect.height);
			//fakeSprite.depth = 1;
		}

		public override void Destroy()
		{
			if (fakeSprite != null)
			{
				fakeSprite.atlas = null;
#if UNITY_EDITOR
				GameObject.DestroyImmediate(fakeSprite.gameObject);
#else
				GameObject.Destroy(fakeSprite.gameObject);
#endif
				fakeSprite = null;
			}

			if (Symbol != null)
			{
				Symbol = null;
			}
		}

		public override void Enable(bool enable)
		{
			if (!enable)
			{
				fakeSprite.spriteName = string.Empty;
			}
			else
			{
				fakeSprite.spriteName = Symbol.spriteName;
			}
		}
	}

	public class SpriteAnimationSymbol : FakeSymbol
	{
		public int Number
		{
			get; private set;
		}

		private UISprite fakeSprite;

		public SpriteAnimationSymbol(int num)
		{
			Number = num;
		}

		public override string Sequence()
		{
			return string.Format("#{0}", Number);
		}

		public static SpriteAnimationSymbol Parse(string sequence)
		{
			int num = 0;
			if (!sequence.StartsWith("#") || !int.TryParse(sequence.Substring(1), out num))
			{
				return null;
			}
			return new SpriteAnimationSymbol(num);
		}

		public override void Create(BMSymbol symbol, Rect rect, GameObject parent, UIAtlas atlas)
		{
			Symbol = symbol;

			if (fakeSprite == null)
			{
				fakeSprite = NGUITools.AddSprite(parent, atlas, Symbol.spriteName);
			}

			var spriteData = atlas.GetSprite(Symbol.spriteName);
			if (spriteData != null)
			{
				fakeSprite.keepAspectRatio = AspectRatioSource.BasedOnHeight;
				fakeSprite.aspectRatio = spriteData.width / (float)spriteData.height;
			}

			fakeSprite.SetRect(rect.x, rect.y, rect.width, rect.height);

			UISymbolAnimation animation = fakeSprite.gameObject.GetComponent<UISymbolAnimation>();
			if (animation == null)
			{
				animation = fakeSprite.gameObject.AddComponent<UISymbolAnimation>();
			}
			animation.framesPerSecond = 5;
			animation.loop = true;
			animation.Snap = false;
			animation.namePrefix = string.Format("{0}_", Number);
		}

		public override void Destroy()
		{
			if (fakeSprite != null)
			{
				fakeSprite.atlas = null;
#if UNITY_EDITOR
				GameObject.DestroyImmediate(fakeSprite.gameObject);
#else
				GameObject.Destroy(fakeSprite.gameObject);
#endif
				fakeSprite = null;
			}

			if (Symbol != null)
			{
				Symbol = null;
			}
		}

		public override void Enable(bool enable)
		{
			if (!enable)
			{
				fakeSprite.spriteName = string.Empty;
				fakeSprite.GetComponent<UISymbolAnimation>().enabled = false;
			}
			else
			{
				fakeSprite.spriteName = Symbol.spriteName;
				fakeSprite.GetComponent<UISymbolAnimation>().enabled = true;
			}
		}
	}

	public class BMSymbolComparer : IComparer<BMSymbol>
	{
		public int Compare(BMSymbol x, BMSymbol y)
		{
			return -string.CompareOrdinal(x.sequence, y.sequence);
		}
	}

	[HideInInspector][SerializeField]public UIAtlas mSymbolAtlas;

	private List<BMSymbol> mAtlasSymbols = null;
	private List<FakeSymbol> mFakeSymbols = new List<FakeSymbol>();
	private List<FakeSymbol> mClean = new List<FakeSymbol>();

	public string DropUrlAtCharacterIndex(int characterIndex, out int startIndex, out int endIndex)
	{
		startIndex = endIndex = 0;

		string s = printedText;

		if (characterIndex != -1 && characterIndex < s.Length - 6)
		{
			int linkStart;

			// LastIndexOf() fails if the string happens to begin with the expected text
			if (s[characterIndex] == '[' &&
				s[characterIndex + 1] == 'u' &&
				s[characterIndex + 2] == 'r' &&
				s[characterIndex + 3] == 'l' &&
				s[characterIndex + 4] == '=')
			{
				linkStart = characterIndex;
			}
			else linkStart = s.LastIndexOf("[url=", characterIndex);

			if (linkStart == -1) return null;

			linkStart += 5;
			int linkEnd = s.IndexOf("]", linkStart);
			if (linkEnd == -1) return null;

			int urlEnd = s.IndexOf("[/url]", linkEnd);
			if (urlEnd == -1 || characterIndex <= urlEnd)
			{
				startIndex = linkStart - 5;
				endIndex = urlEnd + 6;
				return s.Substring(linkStart, linkEnd - linkStart);
			}
		}
		return null;
	}

	public Symbol GetSymbolAtPosition(Vector3 worldPos)
	{
		string url = GetUrlAtPosition(worldPos);
		if (!string.IsNullOrEmpty(url))
		{
			UrlSymbol symbol = UrlSymbol.Parse(url);
			if (symbol != null)
			{
				return symbol;
			}
		}

		int characterIndex = GetCharacterIndexAtPosition(worldPos, true);
		if (characterIndex != -1 && characterIndex < mText.Length)
		{
			foreach (var fakeSymbol in mFakeSymbols)
			{
				if (fakeSymbol.Offset == characterIndex)
				{
					return fakeSymbol;
				}
			}
		}

		return null;
	}

	private void CleanSymbol()
	{
		if (mFakeSymbols != null)
		{
			for (int i = 0; i < mFakeSymbols.Count; ++i)
			{
				mFakeSymbols[i].Enable(false);
			}
			mClean.AddRange(mFakeSymbols);
			mFakeSymbols.Clear();
		}

		if (mClean.Count > 0)
		{
			foreach (var symbol in mClean)
			{
				symbol.Destroy();
			}
			mClean.Clear();
		}

		for (int i = 0; i < transform.childCount; ++i)
		{
			UISprite symbolSprite = transform.GetChild(i).GetComponent<UISprite>();
			if (symbolSprite != null && symbolSprite.atlas == mSymbolAtlas)
			{
				Destroy(symbolSprite.gameObject);
			}
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if (mClean.Count > 0)
		{
			foreach(var symbol in mClean)
			{
				symbol.Destroy();
			}
			mClean.Clear();
		}
	}

	static Dictionary<string, List<BMSymbol>> sSymbols = new Dictionary<string, List<BMSymbol>>();

	private void InitSymbol()
	{
		if (supportEncoding && symbolStyle == NGUISymbolText.SymbolStyle.None)
		{
			symbolStyle = NGUISymbolText.SymbolStyle.Normal;
		}

		if (mAtlasSymbols == null && mSymbolAtlas != null && !sSymbols.TryGetValue(mSymbolAtlas.name, out mAtlasSymbols))
		{
			mAtlasSymbols = new List<BMSymbol>();
			Dictionary<int, List<int>> anims = new Dictionary<int, List<int>>();
			foreach (var sprite in mSymbolAtlas.spriteList)
			{
				BMSymbol symbol = new BMSymbol();
				symbol.spriteName = sprite.name;
				symbol.sequence = string.Format("${{{0}}}", sprite.name);
				symbol.MarkAsChanged();
				if (symbol.Validate(mSymbolAtlas))
				{
					mAtlasSymbols.Add(symbol);
				}

				if (char.IsDigit(sprite.name[0]))
				{
					string[] splits = sprite.name.Split(new char[] {'_'});
					if (splits.Length == 2)
					{
						int anim = int.Parse(splits[0]);
						int frame = int.Parse(splits[1]);
						if (!anims.ContainsKey(anim))
						{
							anims.Add(anim, new List<int>());
						}
						anims[anim].Add(frame);
					}
				}
			}

			foreach (var entry in anims)
			{
				entry.Value.Sort();
				BMSymbol symbol = new BMSymbol();
				symbol.spriteName = string.Format("{0}_{1}", entry.Key, entry.Value[0]);
				symbol.sequence = string.Format("#{0}", entry.Key);
				symbol.MarkAsChanged();
				if (symbol.Validate(mSymbolAtlas))
				{
					mAtlasSymbols.Add(symbol);
				}
			}
			mAtlasSymbols.Sort(new BMSymbolComparer());
			sSymbols.Add(mSymbolAtlas.name, mAtlasSymbols);
		}
		else if (mAtlasSymbols == null)
		{
			mAtlasSymbols = new List<BMSymbol>();
		}
	}	

	private void UpdateSymbol(string processedText)
	{
		// delay destroy
		for (int i = 0; i < mFakeSymbols.Count; ++i)
		{
			mFakeSymbols[i].Enable(false);
		}
		mClean.AddRange(mFakeSymbols);
		mFakeSymbols.Clear();

		UpdateFakeSymbols(processedText);
	}

	private BMSymbol GetFakeSymbol(string text, int offset, int textLength)
	{
		if (!Application.isPlaying)
		{
			return null;
		}

		for (int i = 0; i < mAtlasSymbols.Count; ++i)
		{
			BMSymbol symbol = mAtlasSymbols[i];
			if (string.CompareOrdinal(text, offset, symbol.sequence, 0, symbol.sequence.Length) == 0)
			{
				return symbol;
			}
		}

		return null;
	}

	private FakeSymbol UseFakeSymbol(BMSymbol symbol, string text, int offset)
	{
		FakeSymbol fakeSymbol = mFakeSymbols.Find(s => s.Symbol == symbol && s.Index == offset && text.IndexOf(s.Sequence(), offset) >= 0);
		if (fakeSymbol != null)
		{
			return fakeSymbol;
		}

		int index = mClean.FindIndex(s => s.Symbol == symbol && s.Index == offset && text.IndexOf(s.Sequence(), offset) >= 0);
		if (index >= 0)
		{
			fakeSymbol = mClean[index];
			mClean.RemoveAt(index);
			fakeSymbol.Enable(true);
			mFakeSymbols.Add(fakeSymbol);
		}

		return fakeSymbol;
	}

	private void PostFakeSymbol(BMSymbol symbol, string text, int offset, int characterIndex, Rect rect)
	{
		FakeSymbol fakeSymbol = UseFakeSymbol(symbol, text, offset);
		if (fakeSymbol != null)
		{// reuse
			fakeSymbol.Create(symbol, rect, cachedGameObject, mSymbolAtlas);
			return;
		}

		// create new
		if (!TryParseFakeSymbol(symbol.sequence, out fakeSymbol))
		{
			EB.Debug.LogError("PostFakeSymbol: parse fake symbol failed");
			return;
		}
		fakeSymbol.Offset = offset;
		fakeSymbol.Index = characterIndex;
		fakeSymbol.Create(symbol, rect, cachedGameObject, mSymbolAtlas);
		mFakeSymbols.Add(fakeSymbol);
	}

	private bool TryParseFakeSymbol(string sequence, out FakeSymbol symbol)
	{
		symbol = SpriteAnimationSymbol.Parse(sequence);
		if (symbol != null)
		{
			return true;
		}

		symbol = SpriteSymbol.Parse(sequence);
		if (symbol != null)
		{
			return true;
		}

		return false;
	}

	List<BMSymbol> fakeSymbols = new List<BMSymbol>();
	List<int> fakeSymbolIndex = new List<int>();
	List<Rect> fakeSymbolRect = new List<Rect>();
	List<int> fakeSymbolElementIndex = new List<int>();
	List<int> fakeSymbolCharacterIndex = new List<int>();

	private Rect UpdateFakeSymbols(string text)
	{
		fakeSymbols.Clear();
		fakeSymbolIndex.Clear();
		fakeSymbolRect.Clear();
		fakeSymbolElementIndex.Clear();
		fakeSymbolCharacterIndex.Clear();
		int indexOffset = 0;

		Vector2 v = Vector2.zero;

		if (!string.IsNullOrEmpty(text))
		{
			// When calculating printed size, get rid of all symbols first since they are invisible anyway
			if (NGUISymbolText.encoding) text = NGUISymbolText.StripSymbols(text);

			// Ensure we have characters to work with
			NGUISymbolText.Prepare(text);

			float x = 0f, y = 0f, maxX = 0f;
			int textLength = text.Length, ch = 0, prev = 0;
			int elements = 0, totalElements = 0;
			float maxLineHeight = 0f, delta = 0f; int adjustIndexOffset = 0;

			for (int i = 0; i < textLength; ++i)
			{
				ch = text[i];

				// Start a new line
				if (ch == '\n')
				{
					if (x > maxX) maxX = x;

					if (alignment != NGUISymbolText.Alignment.Left)
					{
						Align(fakeSymbolRect, indexOffset, x - NGUISymbolText.finalSpacingX, fakeSymbolElementIndex, elements);
						indexOffset = fakeSymbolRect.Count;
					}
					elements = 0;

					x = 0f;
					y += NGUISymbolText.finalLineHeight;

					delta = Mathf.Max(maxLineHeight - NGUISymbolText.finalLineHeight, 0);
					{
						y += delta;
						maxLineHeight = 0;

						Adjust(fakeSymbolRect, adjustIndexOffset, -delta);
						adjustIndexOffset = fakeSymbolRect.Count;
					}

					continue;
				}

				// Skip invalid characters
				if (ch < ' ') continue;

				// See if there is a symbol matching this text
				BMSymbol symbol = NGUISymbolText.useSymbols ? NGUISymbolText.GetSymbol(text, i, textLength) : null;
				symbol = symbol == null && NGUISymbolText.useFakeSymbols ? NGUISymbolText.fakeSymbolHandler(text, i, textLength) : symbol;

				if (symbol == null)
				{
					float w = NGUISymbolText.GetGlyphWidth(ch, prev);

					if (w != 0f)
					{
						w += NGUISymbolText.finalSpacingX;

						if (Mathf.RoundToInt(x + w) > NGUISymbolText.regionWidth)
						{
							if (NGUISymbolText.alignment != NGUISymbolText.Alignment.Left && indexOffset < fakeSymbols.Count)
							{
								Align(fakeSymbolRect, indexOffset, x, fakeSymbolElementIndex, elements);
								indexOffset = fakeSymbolRect.Count;
							}
							elements = 0;
							delta = Mathf.Max(maxLineHeight - NGUISymbolText.finalLineHeight, 0);
							{
								y += delta;
								maxLineHeight = 0;

								Adjust(fakeSymbolRect, adjustIndexOffset, -delta);
								adjustIndexOffset = fakeSymbolRect.Count;
							}

							if (x > maxX) maxX = x - NGUISymbolText.finalSpacingX;
							x = w;
							y += NGUISymbolText.finalLineHeight;
							elements = 0;
						}
						else x += w;

						prev = ch;
					}
				}
				else
				{
					float w = NGUISymbolText.finalSpacingX + symbol.advance * NGUISymbolText.fontScale;

					if (Mathf.RoundToInt(x + w) > NGUISymbolText.regionWidth)
					{
						if (NGUISymbolText.alignment != NGUISymbolText.Alignment.Left && indexOffset < fakeSymbols.Count)
						{
							Align(fakeSymbolRect, indexOffset, x, fakeSymbolElementIndex, elements);
							indexOffset = fakeSymbolRect.Count;
						}
						elements = 0;

						delta = Mathf.Max(maxLineHeight - NGUISymbolText.finalLineHeight, 0);
						{
							y += delta;
							maxLineHeight = 0;

							Adjust(fakeSymbolRect, adjustIndexOffset, -delta);
							adjustIndexOffset = fakeSymbolRect.Count;
						}

						if (x > maxX) maxX = x - NGUISymbolText.finalSpacingX;
						x = w;
						y += NGUISymbolText.finalLineHeight;
					}
					else x += w;

					if (NGUISymbolText.useFakeSymbols)
					{
						fakeSymbols.Add(symbol);
						fakeSymbolIndex.Add(i);
						Rect rect = new Rect(
							x - symbol.advance * NGUISymbolText.fontScale,
							-y - (NGUISymbolText.fontSize * NGUISymbolText.fontScale + symbol.offsetY * NGUISymbolText.fontScale),
							symbol.advance * NGUISymbolText.fontScale,
							symbol.height * NGUISymbolText.fontScale);
						fakeSymbolRect.Add(rect);
						fakeSymbolElementIndex.Add(elements);
						fakeSymbolCharacterIndex.Add(totalElements);
					}

					i += symbol.sequence.Length - 1;
					prev = 0;

					maxLineHeight = Mathf.Max(maxLineHeight, symbol.height * NGUISymbolText.fontScale);
				}

				elements++;
				totalElements++;
			}

			if (NGUISymbolText.alignment != NGUISymbolText.Alignment.Left && indexOffset < fakeSymbols.Count)
			{
				Align(fakeSymbolRect, indexOffset, x, fakeSymbolElementIndex, elements);
				indexOffset = fakeSymbolRect.Count;
			}

			delta = Mathf.Max(maxLineHeight - NGUISymbolText.finalLineHeight, 0);
			{
				y += delta;
				maxLineHeight = 0;

				Adjust(fakeSymbolRect, adjustIndexOffset, -delta);
				adjustIndexOffset = fakeSymbolRect.Count;
			}

			v.x = ((x > maxX) ? x - NGUISymbolText.finalSpacingX : maxX);
			v.y = (y + NGUISymbolText.finalLineHeight);
		}

		Vector2 offset = ApplyOffset(fakeSymbolRect, v);
		for (int j = 0; j < fakeSymbols.Count; ++j)
		{
			PostFakeSymbol(fakeSymbols[j], text, fakeSymbolIndex[j], fakeSymbolCharacterIndex[j], fakeSymbolRect[j]);
		}

		return new Rect(offset, v);
	}

	private void Adjust(List<Rect> rects, int indexOffset, float yOffset)
	{
		if (yOffset != 0)
		{
			for (int i = indexOffset; i < rects.Count; ++i)
			{
				Rect rect = rects[i];
				rect.Set(rect.x, rect.y + yOffset, rect.width, rect.height);
				rects[i] = rect;
			}
		}
	}

	private Vector2 ApplyOffset(List<Rect> rects, Vector2 printSize)
	{
		Vector2 po = pivotOffset;

		float fx = Mathf.Lerp(0f, -mWidth, po.x);
		float fy = Mathf.Lerp(mHeight, 0f, po.y) + Mathf.Lerp((printSize.y - mHeight), 0f, po.y);

		fx = Mathf.Round(fx);
		fy = Mathf.Round(fy);

		for (int i = 0; i < rects.Count; ++i)
		{
			Rect rect = rects[i];
			rect.Set(rect.x + fx, rect.y + fy, rect.width, rect.height);
			rects[i] = rect;
		}

		return new Vector2(fx, fy);
	}

	private void Align(List<Rect> rects, int indexOffset, float printedWidth, List<int> elementIndex, int elements)
	{
		switch (NGUISymbolText.alignment)
		{
			case NGUISymbolText.Alignment.Right:
				{
					float padding = NGUISymbolText.rectWidth - printedWidth;
					if (padding < 0f) return;

					for (int i = indexOffset; i < rects.Count; ++i)
					{
						Rect rect = rects[i];
						rect.x += padding;
						rects[i] = rect;
					}

					break;
				}

			case NGUISymbolText.Alignment.Center:
				{
					float padding = (NGUISymbolText.rectWidth - printedWidth) * 0.5f;
					if (padding < 0f) return;

					// Keep it pixel-perfect
					int diff = Mathf.RoundToInt(NGUISymbolText.rectWidth - printedWidth);
					int intWidth = Mathf.RoundToInt(NGUISymbolText.rectWidth);

					bool oddDiff = (diff & 1) == 1;
					bool oddWidth = (intWidth & 1) == 1;
					if ((oddDiff && !oddWidth) || (!oddDiff && oddWidth))
						padding += 0.5f * NGUISymbolText.fontScale;

					for (int i = indexOffset; i < rects.Count; ++i)
					{
						Rect rect = rects[i];
						rect.x += padding;
						rects[i] = rect;
					}

					break;
				}

			case NGUISymbolText.Alignment.Justified:
				{
					// Printed text needs to reach at least 65% of the width in order to be justified
					if (printedWidth < NGUISymbolText.rectWidth * 0.65f) return;

					// There must be some padding involved
					float padding = (NGUISymbolText.rectWidth - printedWidth) * 0.5f;
					if (padding < 1f) return;

					if (elements < 2) return;

					float progressPerChar = 1f / (elements - 1);
					float scale = NGUISymbolText.rectWidth / printedWidth;
					for (int i = indexOffset; i < rects.Count; ++i)
					{
						Rect rect = rects[i];

						float x0 = rect.x;
						float x1 = rect.x + rect.width;
						float w = x1 - x0;
						float x0a = x0 * scale;
						float x1a = x0a + w;
						float x1b = x1 * scale;
						float x0b = x1b - w;
						float progress = elementIndex[i] * progressPerChar;

						x1 = Mathf.Lerp(x1a, x1b, progress);
						x0 = Mathf.Lerp(x0a, x0b, progress);
						x0 = Mathf.Round(x0);
						x1 = Mathf.Round(x1);

						rect.x = x0;
						rect.width = x1 - x0;
						rects[i] = rect;
					}

					break;
				}
		}
	}

	#endregion
}
