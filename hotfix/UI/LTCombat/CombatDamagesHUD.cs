using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class CombatDamagesHUD : DynamicMonoHotfix
    {
        public enum eDamageTextType
        {
            Attack,
            Crit,
            Block,
            Heal,
            Miss,
            Poisoning,
            Bleeding,
            Fire,
            Absorb,
        }
    
        [System.Serializable]
        public class DamageTextColor
        {
            public eDamageTextType textType;
            public Color topColor;
            public Color bottomColor;
            public Color outlineColor;
            public string icon;
            public string tip;
            public string tipSprite;
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
    
        public UILabel DamagesLabel;
        public UISprite TipSprite;
        public List<UIWidget> DamageMotions;
    
        public ClampRectMarginValues ClampRectMargin;
        private List<DamageTextColor> DamageText = new List<DamageTextColor>();
    
        [HideInInspector]
        public Transform target;
        private Vector3 offset;
        private System.Action onShowCompleteCallback;
    
        private float screenScale;
        private int screenHeight = Screen.height;
        private int screenWidth = Screen.width;
        private eDamageTextType m_damageType;
    
        private Rect clampRect;
    
        private UITweener[] m_tweeners;
    
        private Bounds bounds;
    
        private Camera mMainCamera = null;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            DamagesLabel = t.GetComponent<UILabel>("DamageMotion/DamageNum");
            TipSprite = t.GetComponent<UISprite>("DamageMotion/TipSprite");
            DamageMotions = new List<UIWidget>
            {
                t.GetChild(0).GetComponent<UIWidget>()
            };
            ClampRectMargin = new ClampRectMarginValues() { Left = 0, Right = 0, Top = 100, Bottom = 0 };

            SetDamageText(eDamageTextType.Attack, Color.white, Color.white, Color.black, "Y", "", "", 86);
            SetDamageText(eDamageTextType.Crit, new Color(1f, 0.99f, 0.9f), new Color(1f, 0.68f, 0f), Color.black, "R", "", "Combat_Fluttering_Baoji", 86);
            SetDamageText(eDamageTextType.Block, Color.white, new Color(0.41f, 0.41f, 0.41f), Color.black, "B", "", "", 80);
            SetDamageText(eDamageTextType.Heal, new Color(0.13f, 0.93f, 0.26f), new Color(0.13f, 0.93f, 0.26f), new Color(0.02f, 0.51f, 0f), "G", "", "", 86);
            SetDamageText(eDamageTextType.Miss, Color.white, new Color(0.41f, 0.41f, 0.41f), Color.black, "B", "", "", 80);
            SetDamageText(eDamageTextType.Poisoning, new Color(0f, 0.45f, 1f), new Color(0.67f, 0.1f, 0.66f), Color.black, "Y", "", "", 80);
            SetDamageText(eDamageTextType.Bleeding, Color.red, Color.red, Color.black, "Y", "", "", 80);
            SetDamageText(eDamageTextType.Fire, new Color(1f, 0.87f, 0f), new Color(1f, 0.18f, 0.01f), Color.black, "Y", "", "", 80);
            SetDamageText(eDamageTextType.Absorb, new Color(0.82f, 0.96f, 1f), new Color(0.11f, 0.67f, 1f), Color.black, "Y", "", "", 80);

            CacheSettings();
            if (DamageMotions.Count == 0)
            {
                EB.Debug.LogError("DamageMotions is Empty!!!");
                m_tweeners = new UITweener[0];
                return;
            }
            m_tweeners = DamageMotions[0].transform.parent.GetComponentsInChildren<UITweener>();
        }

		public override void OnEnable()
		{
			//base.OnEnable();
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        private void SetDamageText(eDamageTextType ty, Color topC, Color bottomC, Color outlineC, string ic, string tp, string tpSprite, int ftSize)
        {
            DamageText.Add(new DamageTextColor
            {
                textType = ty,
                topColor = new Color(topC.r, topC.g, topC.b),
                bottomColor = new Color(bottomC.r, bottomC.g, bottomC.b),
                outlineColor = new Color(outlineC.r, outlineC.g, outlineC.b),
                icon = ic,
                tip = tp,
                tipSprite = tpSprite,
                fontSize = ftSize
            });
        }
        private void CacheSettings()
        {
            float manualHeight = UIRoot.list[0].manualHeight;
            float manualWidth = UIRoot.list[0].manualWidth;
            
    
            if ((float)screenWidth / (float)screenHeight > manualWidth / manualHeight)
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
    
            clampRect.yMin = -(screenHeight * screenScale - ClampRectMargin.Bottom);
            clampRect.yMax = -ClampRectMargin.Top;
        }
    
        private const int SHOW_DURATION = 1500;
    
        public void ShowDamage(int numDamages, Transform spawn_point, Vector3 offset, eDamageTextType text_type, System.Action onComplete = null)
        {
            StartCoroutine(ShowDamage_coroutine(spawn_point, offset, numDamages, text_type, onComplete));
        }
    
      //  public void ShowMiss(Transform spawn_point, Vector3 offset, System.Action onComplete = null)
      //  {
      //      StartCoroutine(ShowMiss_coroutine(spawn_point, offset, onComplete));
      //  }

      //  private int _ShowMiss_Seq = 0;

      //  private IEnumerator ShowMiss_coroutine(Transform spawn_point, Vector3 offset, System.Action onComplete = null)
      //  {
      //      this.offset = offset;
      //      this.target = spawn_point;
      //      this.onShowCompleteCallback = onComplete;
    
      //      DamagesLabel.text = "";
      //      setTipSpritePos();
      //      SetFontColor(eDamageTextType.Miss);
    		//for(int i=0;i<DamageMotions.Count;i++)
    		//{
    		//	DamageMotions[i].alpha = 0;
    		//	DamageMotions[i].transform.localPosition = Vector3.zero;
    		//}
    
      //      #region PsdLayerSpriteFont FIX
      //      // ********
      //      // PsdLayerSpriteFont refresh its text content only on its Update(). 
      //      // This is problematic cause it results in seeing previous text during the first frame (looks "buggy")
      //      // and also prevent us to calculate the bounds correctly
      //      //
      //      // WORK-AROUND : we hide the label during next frame
      //      // ********
    
      //      // wait for 1 frame
      //      yield return null;
    		//#endregion
    
    		//for (int i = 0; i < DamageMotions.Count; i++)
    		//{
    		//	DamageMotions[i].alpha = 1;
    		//}
    
      //      float duration = 0.0f;
      //      for (int i = 0; i < m_tweeners.Length; ++i)
      //      {
      //          m_tweeners[i].tweenFactor = 0;
      //          m_tweeners[i].PlayForward();
    
      //          duration = Mathf.Max(duration, m_tweeners[i].delay + m_tweeners[i].duration);
      //      }
            
      //      bounds = NGUIMath.CalculateRelativeWidgetBounds(mDMono.transform);
      //      UpdatePosition(true);
    		//for (int i = 0; i < DamageMotions.Count; i++)
    		//{
    		//	DamageMotions[i].width = (int)bounds.size.x;
    		//	DamageMotions[i].height = (int)bounds.size.y;
    		//}
      //      int timer = (int)((duration + 0.3f) * 1000);
      //      _ShowMiss_Seq = ILRTimerManager.instance.AddTimer(timer, int.MaxValue, delegate { OnShowComplete(); });
      //  }

        private int _ShowDamage_Seq = 0;

        private IEnumerator ShowDamage_coroutine(Transform spawn_point, Vector3 offset, int numDamages, eDamageTextType text_type, System.Action onComplete = null)
        {
            this.offset = offset;
            this.target = spawn_point;
            this.onShowCompleteCallback = onComplete;
    
            if (numDamages < 0)
            {// heal
                DamagesLabel.text = (-numDamages).ToString();
                setTipSpritePos();
            }
            else if (numDamages > 0)
            {// damage
                DamagesLabel.text = numDamages.ToString();
                setTipSpritePos();
            }
            else
            {// miss/block
                switch (text_type)
                {
                    case eDamageTextType.Miss:
                    case eDamageTextType.Block:
                        DamagesLabel.text = "0";
                        setTipSpritePos();
                        break;
                    default:
                        DamagesLabel.text = string.Empty;
                        setTipSpritePos();
                        break;
                }
            }
    
            SetFontColor(text_type);
    		for (int i = 0; i < DamageMotions.Count; i++)
    		{
    			DamageMotions[i].alpha = 0;
    			DamageMotions[i].transform.localPosition = Vector3.zero;
    		}
    		#region PsdLayerSpriteFont FIX
    		// ********
    		// PsdLayerSpriteFont refresh its text content only on its Update(). 
    		// This is problematic cause it results in seeing previous text during the first frame (looks "buggy")
    		// and also prevent us to calculate the bounds correctly
    		//
    		// WORK-AROUND : we hide the label during next frame
    		// ********
    
    		// wait for 1 frame
    		yield return null;
    		#endregion
    
    		for (int i = 0; i < DamageMotions.Count; i++)
    		{
    			DamageMotions[i].alpha = 1;
    		}
    
            for (int i = 0; i < m_tweeners.Length; ++i)
            {
                m_tweeners[i].tweenFactor = 0;
                m_tweeners[i].PlayForward();
            }
            
            bounds = NGUIMath.CalculateRelativeWidgetBounds(mDMono.transform);
            UpdatePosition(true);
            _ShowDamage_Seq = ILRTimerManager.instance.AddTimer(SHOW_DURATION , int.MaxValue, delegate { OnShowComplete(); });
        }
    
        private void SetFontColor(eDamageTextType type)
        {
            for (var i = 0; i < DamageText.Count; i++)
            {
                DamageTextColor color = DamageText[i];
                if (color.textType == type)
                {
                    DamagesLabel.gradientTop = color.topColor;
                    DamagesLabel.gradientBottom = color.bottomColor;
                    DamagesLabel.effectColor = color.outlineColor;
                    DamagesLabel.fontSize = color.fontSize;
    
                    TipSprite.spriteName = color.tipSprite;
    
                    break;
                }
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
            //ILRTimerManager.instance.RemoveTimerSafely(ref _ShowMiss_Seq);
            ILRTimerManager.instance.RemoveTimerSafely(ref _ShowDamage_Seq);
            StopAllCoroutines();
            target = null;
            onShowCompleteCallback = null;
        }
    
        private void UpdatePosition(bool isInit)
        {
            if (target == null)
            {
                return;
            }

            Vector3 position = target.TransformPoint(offset);
    
            if (mMainCamera == null)
            {
                mMainCamera = Camera.main;
            }

            if (mMainCamera == null)
            {
                return;
            }

            position = mMainCamera.WorldToScreenPoint(position);
            position.y = -screenHeight + position.y;
            position *= screenScale;
    
            if (isInit)
            {
                position.x = Mathf.Clamp(position.x, clampRect.min.x + bounds.extents.x, clampRect.max.x - bounds.extents.x);
                position.y = Mathf.Clamp(position.y, clampRect.min.y, clampRect.max.y - bounds.size.y);
            }

            try
            {
                if (target.transform.position.x < 8000 && target.transform.position.y < 8000)//DeathActionState .HideCharacterPos
                {
                    mDMono.transform.localPosition = position;
                }
                else//隐藏防止死亡后显示buff位置错误
                {
                    mDMono.transform.localPosition = Hotfix_LT.Combat.DeathActionState.HideCharacterPos; 
                }
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
        }
    
        void setTipSpritePos()
        {
            TipSprite.transform.localPosition = new Vector2(-(DamagesLabel.width/2+10), TipSprite.transform.localPosition.y);
        }
    }
}
