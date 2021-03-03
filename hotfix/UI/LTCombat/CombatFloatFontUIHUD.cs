using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class CombatFloatFontUIHUD : DynamicMonoHotfix
    {
    	public static string[] FontMap = new string[9] { "ID_ATTR_CritP", "ID_ATTR_CritV", "ID_ATTR_HP", "ID_ATTR_ATK", "ID_ATTR_DEF", "ID_ATTR_Chain", "ID_ATTR_SpExtra", "ID_ATTR_SpRes", "ID_ATTR_Speed" };
    
    	public enum eFloatFontType
    	{
    		Gain,
    		Debuff,
    		OnlyFont,
            StarSkill,
    	}
    
    	[System.Serializable]
    	public class FontColor
    	{
    		public eFloatFontType textType;
    		public Color topColor;
    		public Color bottomColor;
    		public Color outlineColor;
    		public int fontSize;
    	}
    
    	[System.Serializable]
    	public struct ClampRectMarginValues
    	{
    		public float Left;
    		public float Right;
    		public float Top;
    		public float Bottom;
    	}
    
    	public UILabel FontLabel;
    	public UISprite FontSprite;
    	public UIWidget MotionRoot;
    
    	public ClampRectMarginValues ClampRectMargin;
    	public List<FontColor>FontColorConfig = new List<FontColor>();
    
    	[HideInInspector]
    	public Transform target;
    	private Vector3 offset;
    
    	private System.Action onShowCompleteCallback;
    
    	private float screenScale;
        private int screenHeight = Screen.height;
        private int screenWidth = Screen.width;
    
        private Rect clampRect;
    
    	private UITweener[] m_tweeners;
    
    	private Bounds bounds;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            FontLabel = t.GetComponent<UILabel>("Motion/Font");

            FontSprite = t.GetComponent<UISprite>("Motion/Sprite");
            MotionRoot = t.GetComponent<UIWidget>("Motion");

            ClampRectMargin = new ClampRectMarginValues() { Left = 0, Right = 0, Top = 100, Bottom = 0 };

            SetFontColor(eFloatFontType.Gain, new Color(1f, 0.98f, 0.31f), new Color(1f, 0.8f, 0f), Color.black, 70);
            SetFontColor(eFloatFontType.Debuff, new Color(1f, 0.29f, 0.28f), new Color(1f, 0f, 0f), Color.black, 70);
            SetFontColor(eFloatFontType.OnlyFont, new Color(0.71f, 1f, 0.98f), new Color(0.33f, 0.63f, 0.67f), Color.black, 70);
            SetFontColor(eFloatFontType.StarSkill, new Color(1f, 0f, 78/255f), new Color(1f, 81/255f, 219/255f), Color.black, 70);

            CacheSettings(); // this must happen during Awake, not Start(), since Show() is sometime called before Start()
            m_tweeners = MotionRoot.GetComponentsInChildren<UITweener>();
        }
    
        private void SetFontColor(eFloatFontType ty, Color tC, Color bottomC, Color outlineC, int ftSize)
        {
            FontColorConfig.Add(new FontColor
            {
                textType = ty,
                topColor = tC,
                bottomColor = bottomC,
                outlineColor = outlineC,
                fontSize = ftSize
            });
        }

        private void CacheSettings()
        {
            float manualHeight = UIRoot.list[0].manualHeight;
            float manualWidth = UIRoot.list[0].manualWidth;
    
    
            if (screenWidth / screenHeight > manualWidth / manualHeight)
            {
                screenScale = manualHeight / screenHeight;
            }
            else // fitWidth
            {
                screenScale = manualWidth / screenWidth;
            }
    
            clampRect = new Rect();
    		clampRect.xMin = ClampRectMargin.Left;
    		clampRect.xMax = Screen.width * screenScale - ClampRectMargin.Right;
    
    		clampRect.yMin = -(Screen.height * screenScale - ClampRectMargin.Bottom);
    		clampRect.yMax = -ClampRectMargin.Top;
    	}
    
    	private const int SHOW_DURATION = 1500;
    
    	public void ShowBuffEffect(eFloatFontType floatFontType,string font, Transform spawn_point,Vector3 offset, System.Action onComplete = null)
    	{
    		StartCoroutine(ShowBuffEffect_coroutine(spawn_point,offset, floatFontType, font, onComplete));
    	}

        int sequence = 0;
        private IEnumerator ShowBuffEffect_coroutine(Transform target,Vector3 offset, eFloatFontType floatFontType, string font, System.Action onComplete = null)
    	{
    		this.target = target;
    		this.offset = offset;
    		this.onShowCompleteCallback = onComplete;
    
    		LTUIUtil.SetText(FontLabel,font);
    		SetFontColor(floatFontType);
    		SetFontSprite(floatFontType);
    		MotionRoot.alpha = 0;
    		MotionRoot.transform.localPosition = Vector3.zero;
    
    		// wait for 1 frame
    		yield return null;
    
    		MotionRoot.alpha = 1;
    
    		for (int i = 0; i < m_tweeners.Length; ++i)
    		{
    			m_tweeners[i].tweenFactor = 0;
    			m_tweeners[i].PlayForward();
    		}
    
    		bounds = NGUIMath.CalculateRelativeWidgetBounds(mDMono.transform);
    		UpdatePosition();
            sequence = ILRTimerManager.instance.AddTimer(SHOW_DURATION, int.MaxValue, delegate { OnShowComplete(); });
        }
    
    	private void SetFontColor(eFloatFontType type)
    	{
            for (var i = 0; i < FontColorConfig.Count; i++)
    		{
                FontColor color = FontColorConfig[i];
                if (color.textType == type)
    			{
                    FontLabel.gradientTop = color.topColor;
    				FontLabel.gradientBottom = color.bottomColor;
    				FontLabel.effectColor = color.outlineColor;
    				FontLabel.fontSize = color.fontSize;
    
    				break;
    			}
    		}
    	}
    
    	private void SetFontSprite(eFloatFontType type)
    	{
    		if (type == eFloatFontType.Gain)
    		{
    			FontSprite.spriteName = "Combat_Fluttering_Zengyi";
    			FontSprite.gameObject.SetActive(true);
    		}
    		else if (type == eFloatFontType.Debuff)
    		{
    			FontSprite.spriteName = "Combat_Fluttering_Jianyi";
    			FontSprite.gameObject.SetActive(true);
    		}
    		else if (type == eFloatFontType.OnlyFont)
    		{
    			FontSprite.gameObject.SetActive(false);
    		}
            else if (type == eFloatFontType.StarSkill)
            {
                FontSprite.gameObject.SetActive(false);
            }
        }
    
    	public void OnShowComplete()
    	{
    		target = null;
    		if (onShowCompleteCallback != null)
    		{
    			onShowCompleteCallback();
    		}
    	}
    
    	public void Clean()
    	{
    		target = null;
    		onShowCompleteCallback = null;
    		StopAllCoroutines();
            if (sequence != 0)
            {
                ILRTimerManager.instance.RemoveTimer(sequence);
                sequence = 0;
            }

        }
    
    	private void UpdatePosition()
    	{
            if (target == null)
            {
                return;
            }
    
    		Vector3 position = target.TransformPoint(offset);
    
            if (Camera.main == null) return; 
            position = Camera.main.WorldToScreenPoint(position);
    		position.y = -screenHeight + position.y;
    		position *= screenScale;
    
            // when clamping, we assume that the label is aligned on center-bottom            
            position.x = Mathf.Clamp(position.x, clampRect.min.x + bounds.extents.x, clampRect.max.x - bounds.extents.x);
            position.y = Mathf.Clamp(position.y, clampRect.min.y, clampRect.max.y - bounds.size.y);
    
            if (target.transform.position.x < 8000 && target.transform.position.y < 8000)//DeathActionState .HideCharacterPos
            {
                mDMono.transform.localPosition = position;
            }
            else//���ط�ֹ��������ʾbuffλ�ô���
            {
                //transform.localPosition = DeathActionState.HideCharacterPos;
            }
        }
    }
}
