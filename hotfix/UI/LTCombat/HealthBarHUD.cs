using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    [System.Serializable]
    public class BuffIconItem
    {
    	public UISprite IconSprite;
    	public UILabel LeftTurnLabel;
    	public UILabel OverlyingLabel;
    	public GameObject RootNode { get { return IconSprite.gameObject; } }
    }
    
    public class HealthBarHUD : HealthBarBase, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;

            BuffIconItems = new BuffIconItem[4];
            BuffIconItems[0] = new BuffIconItem()
            {
                IconSprite = t.GetComponent<UISprite>("Container/BuffList/0"),
                LeftTurnLabel = t.GetComponent<UILabel>("Container/BuffList/0/Label_Depth4"),
                OverlyingLabel = t.GetComponent<UILabel>("Container/BuffList/0/Label_Depth3")
            };
            BuffIconItems[1] = new BuffIconItem()
            {
                IconSprite = t.GetComponent<UISprite>("Container/BuffList/1"),
                LeftTurnLabel = t.GetComponent<UILabel>("Container/BuffList/1/Label_Depth4"),
                OverlyingLabel = t.GetComponent<UILabel>("Container/BuffList/1/Label_Depth3")
            };
            BuffIconItems[2] = new BuffIconItem()
            {
                IconSprite = t.GetComponent<UISprite>("Container/BuffList/2"),
                LeftTurnLabel = t.GetComponent<UILabel>("Container/BuffList/2/Label_Depth4"),
                OverlyingLabel = t.GetComponent<UILabel>("Container/BuffList/2/Label_Depth3")
            };
            BuffIconItems[3] = new BuffIconItem()
            {
                IconSprite = t.GetComponent<UISprite>("Container/BuffList/3"),
                LeftTurnLabel = t.GetComponent<UILabel>("Container/BuffList/3/Label_Depth4"),
                OverlyingLabel = t.GetComponent<UILabel>("Container/BuffList/3/Label_Depth3")
            };

            container = t.FindEx("Container").gameObject;
            healthProgressBarRenderer = t.GetComponent<UIProgressBar>("Container/HealthBarRenderer");
            healthBarHpColorRenderer = t.GetComponent<UISprite>("Container/HealthBarRenderer/Foreground_Depth1");
            moveBarProgressRenderer = t.GetComponent<UIProgressBar>("Container/MoveBarRenderer");
            flagObj = t.FindEx("Container/Flag").gameObject;
            AttrRenderer = t.GetComponent<UISprite>("Container/Flag/AttrFlag_Depth3");
            hpRenderer = t.GetComponent<UILabel>("Container/HealthBarRenderer/HpRenderer_Depth4");
            convergeFlag = t.FindEx("Container/Flag/ConvergeFlag_Depth1").gameObject;
            isConverge = false;
            restrainFlag = t.GetComponent<UISprite>("Container/Flag/RestrainFlag_Depth2");
            buffAnchor = t.GetComponent<Transform>("Container/BuffAnchor");

        }

    	public GameObject container;
    	public UIProgressBar healthProgressBarRenderer;
    	public UISprite healthBarHpColorRenderer;
    	public UIProgressBar moveBarProgressRenderer;
    
        public GameObject flagObj;
    
    	public UISprite AttrRenderer;
    	public UILabel hpRenderer;
    	public GameObject convergeFlag;
        public bool isConverge;
    	public UISprite restrainFlag;
    	public Transform buffAnchor;
    	private IEnumerator m_hpCoroutine = null;
        private bool m_visible = true;
    
    	public Hotfix_LT.Combat.CombatCharacterSyncData Data;//其实有了这个就不在需要每次都写一堆hp, maxhp等同步了，可以直接从此对象链接中访问
        public long MaxHp
        {
            get;
            set;
        }
    
    	private long hp;

        public long Hp
        {
    		get { return hp; }
            set 
            { 
                hp = value; 
                HpLabel.text = value.ToString();

                //血条动画
                if (TransitionHp != Hp)// && m_hpCoroutine == null)
                {
                    if (m_hpCoroutine != null)
                    {
                        StopCoroutine(m_hpCoroutine);
                        m_hpCoroutine = null;
                    }
                    
                    if(this.mDMono!=null&&this.mDMono.isActiveAndEnabled){
                        m_hpCoroutine = StartHpAnimation(TransitionHp);
                        StartCoroutine(m_hpCoroutine);
                    }
                }
            }
        }
    
        public long TransitionHp
        {
            get;
            set;
        }
    
        public int Shield
        {
            get;
            set;
        }
    
        public int TransitionShield
        {
            get;
            set;
        }
    
    	public Color HpColor
    	{
    		set {
    			if (value == Color.green)
    				healthBarHpColorRenderer.color = new Color32(96,223,56,255); //"Combat_blood_Ziji";
    			else if (value == Color.red)
    				healthBarHpColorRenderer.color = Color.red; //"Combat_blood_Diren";
    			else if (value == Color.blue)
    				healthBarHpColorRenderer.color = LT.Hotfix.Utility.ColorUtility.BlueColor;// "Combat_blood_Duiyou";
    		}
    	}
    
    	private UILabel HpLabel
    	{
    		get { return hpRenderer; }
    		set { hpRenderer = value; }
    	}
    
    	private Hotfix_LT.Data.eRoleAttr attr;
    	public Hotfix_LT.Data.eRoleAttr Attr
    	{
    		get { return attr; }
    		set { attr=value; ShowAttrFlag(value);}		
    	}
    
        public System.Action recycleCallback
        {
            get;
            set;
        }
    
        public bool Visible
        {
            get { return m_visible; }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            StopAllCoroutines();
            m_hpCoroutine = null;
        }
    
        public bool Show(bool show)
        {
            if (m_visible != show&& healthProgressBarRenderer!=null)
            {
                healthProgressBarRenderer.gameObject.CustomSetActive(show);
    			hpRenderer.gameObject.CustomSetActive(show);
    			moveBarProgressRenderer.gameObject.CustomSetActive(show);
    
                flagObj.CustomSetActive(show);
                
                ShowAttrFlag(show?attr:Hotfix_LT.Data.eRoleAttr.None);
                convergeFlag.gameObject.CustomSetActive(show & isConverge ? true : false);

                if (!show)
                {
                    HideRestrainFlag();
                }

    			if (!show)
    			{
                    if (BuffIconItems != null)
                    {
                        for (var i = 0; i < BuffIconItems.Length; i++)
                        {
                            BuffIconItems[i].RootNode.CustomSetActive(false);
                        }
                    }
    			}

                var pss = buffAnchor.GetComponentsInChildren<ParticleSystem>();

                if (pss != null)
                {
                    for (var i = 0; i < pss.Length; i++)
                    {
                        pss[i].transform.localScale = show ? Vector3.one : Vector3.zero;
                    }
                }
    
                m_visible = show;
                return true;
            }
    
            return false;
        }
    
        private IEnumerator StartHpAnimation(long previous_value)
        {
            float time = 0;
            long target_value = Hp;
    
            if (previous_value < target_value)
            {
                while (1 < target_value - TransitionHp)
                {
                    // 注释掉原因：为了让血条动画播放完整
                    //if (target_value != Hp)
                    //{
                    //    m_hpCoroutine = null;
                    //    yield break;
                    //}
    
                    TransitionHp = (long)Mathf.Lerp(previous_value, target_value, 1.0f - Mathf.Cos(time * Mathf.PI * 0.5f));
                    time += Time.deltaTime;
                    healthProgressBarRenderer.value = TransitionHp / (float)MaxHp;
                    yield return null;
                }
            }
            else if (previous_value > target_value)
            {
                while (1 < TransitionHp - target_value)
                {
                    // 注释掉原因：为了让血条动画播放完整
                    //if (target_value != Hp)
                    //{
                    //    m_hpCoroutine = null;
                    //    yield break;
                    //}

                    TransitionHp = (long)Mathf.Lerp(previous_value, target_value, 1.0f - Mathf.Cos(time * Mathf.PI * 0.5f));
                    time += Time.deltaTime;
                    healthProgressBarRenderer.value = TransitionHp / (float)MaxHp;
    				yield return null;
                }
            }

            healthProgressBarRenderer.value = target_value / (float)MaxHp;
            TransitionHp = target_value;
            m_hpCoroutine = null;
            yield break;
        }
    
    	private void ShowAttrFlag(Hotfix_LT.Data.eRoleAttr attr)
    	{
            AttrRenderer.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[attr];
        }
    
    	public void UpdateMoveBar(float value)
    	{
    		moveBarProgressRenderer.value = value;
    	}
    
    	//水克火，火克风，风克水
    	Color cRestrainSpriteColor = LT.Hotfix.Utility.ColorUtility.GreenColor;
    	Color cBeRestrainSpriteColor = Color.red;
    	Color cNoRestrainSpriteColor = LT.Hotfix.Utility.ColorUtility.YellowColor;
    	Color cGainSpriteColor = LT.Hotfix.Utility.ColorUtility.GreenColor;
        /// <summary>
        /// 设置克制箭头
        /// </summary>
        /// <param name="opponent_attr"></param>
    	public void SetRestrainFlag(Hotfix_LT.Data.eRoleAttr opponent_attr)
    	{
    		restrainFlag.gameObject.CustomSetActive(true);
    
    		if (Data.Limits.Contains("underRestrain"))
    		{
    			restrainFlag.color = cRestrainSpriteColor;
    			return;
    		}
    		if (opponent_attr == this.Attr)
    		{
    			restrainFlag.color = cNoRestrainSpriteColor;
    		}
    		else
    		{		
    			if (attr == Hotfix_LT.Data.eRoleAttr.Shui)
    			{
    				if (opponent_attr == Hotfix_LT.Data.eRoleAttr.Huo)
    					restrainFlag.color = cBeRestrainSpriteColor;
    				else
    					restrainFlag.color = cRestrainSpriteColor;
    			}
    			else if (attr == Hotfix_LT.Data.eRoleAttr.Huo)
    			{
    				if (opponent_attr == Hotfix_LT.Data.eRoleAttr.Feng)
    					restrainFlag.color = cBeRestrainSpriteColor;
    				else
    					restrainFlag.color = cRestrainSpriteColor;
    			}
    			else if(attr == Hotfix_LT.Data.eRoleAttr.Feng)
    			{
    				if (opponent_attr == Hotfix_LT.Data.eRoleAttr.Shui)
    					restrainFlag.color = cBeRestrainSpriteColor;
    				else
    				{
    					restrainFlag.color = cRestrainSpriteColor;
    				}
    			}			
    		}
    	}
    
        /// <summary>
        /// 设置克制箭头为克制状态
        /// </summary>
    	public void SetGainFlag()
    	{
    		restrainFlag.color = cGainSpriteColor;
    		restrainFlag.gameObject.CustomSetActive(true);	
    	}
    
        /// <summary>
        /// 隐藏克制箭头
        /// </summary>
    	public void HideRestrainFlag()
    	{
    		restrainFlag.gameObject.CustomSetActive(false);
    	}
    }
}
