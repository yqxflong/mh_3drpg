using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ILRuntime.Runtime;

namespace Hotfix_LT.Combat
{
    public class HealthBar2D : Hotfix_LT.DynamicMonoHotfix
    {
        private Hotfix_LT.UI.HealthBarHUD m_healthBar = null;
        private Main.Combat.UI.HealthBarHUDMonitor m_healthBarMonotor = null;
        private Transform m_healthBarTransform = null;
        private Transform m_locator = null;
        private MoveController m_moveCtrl = null;
        private MoveController.CombatantMoveState m_lastMoveState = MoveController.CombatantMoveState.kIdle;
        private MoveEditor.FXHelper m_fxHelper = null;
        private Transform m_cachedTransform = null;
        public Transform CachedTransform
        {
            get { return m_cachedTransform = m_cachedTransform ?? mDMono.transform; }
        }

        public Hotfix_LT.UI.HealthBarHUD HealthBar
        {
            get { return m_healthBar; }
        }

        public long MaxHp
        {
            get { return m_healthBar.MaxHp; }
            set 
            { 
                m_healthBar.MaxHp = value; 
                m_healthBar.Show(!Hidden && value > 0);
            }
        }

        public long Hp
        {
            get { return m_healthBar.Hp; }
            set 
            { 
                m_healthBar.Hp = value;
                m_healthBar.Show(!Hidden && value > 0);
            }
        }

        public long TransitionHp
        {
            get { return m_healthBar.TransitionHp; }
            set 
            { 
                m_healthBar.TransitionHp = value;
                m_healthBar.Show(!Hidden && value > 0);
            }
        }

        public int Shield
        {
            get { return m_healthBar.Shield; }
            set { m_healthBar.Shield = value; }
        }

        public int TransitionShield
        {
            get { return m_healthBar.TransitionShield; }
            set { m_healthBar.TransitionShield = value; }
        }

        public bool Converge
        {
            get { 
                return m_healthBar != null ? m_healthBar.isConverge : false; 
            }
            set {
                if (m_healthBar != null)
                {
                    m_healthBar.isConverge = value;
                    m_healthBar.convergeFlag.CustomSetActive(value);
                }
            }
        }

        public Hotfix_LT.Data.eRoleAttr RoleAttr
        {
            get { return m_healthBar.Attr; }
            set { m_healthBar.Attr = value; }
        }

        private bool m_hiden = false;
        public bool Hidden
        {
            get{
                return m_hiden;
            }
            private set
            {
                m_hiden = value;
            }
        }

        public override void Awake()
        {
            m_moveCtrl = mDMono.gameObject.GetComponent<MoveController>();
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }

        public void UpdateBuff(ICollection<CombatCharacterSyncData.BuffData> buffDatas)
        {
            m_healthBar.UpdateBuff(buffDatas);
        }

        public void UpdateMoveBar(float value)
        {
            m_healthBar.UpdateMoveBar(value);
        }

        /// <summary>
        /// 设置克制箭头
        /// </summary>
        /// <param name="attr"></param>
        public void SetRestrainFlag(Hotfix_LT.Data.eRoleAttr attr)
        {
            m_healthBar.SetRestrainFlag(attr);
        }

        /// <summary>
        /// 设置克制箭头为克制状态
        /// </summary>
        public void SetGainFlag()
        {
            m_healthBar.SetGainFlag();
        }

        /// <summary>
        /// 隐藏克制箭头
        /// </summary>
        public void HideRestrainFlag()
        {
            m_healthBar.HideRestrainFlag();
        }

        public bool IsLeft()
        {
            return Vector3.Dot(CachedTransform.parent.forward, Vector3.forward) > 0;
        }

        public bool IsRight()
        {
            return Vector3.Dot(CachedTransform.parent.forward, Vector3.forward) < 0;
        }

        public void InitHealthBar()
        {
            m_fxHelper = mDMono.gameObject.GetComponent<MoveEditor.FXHelper>();

            if (m_locator == null)
            {
                m_locator = CachedTransform.Find("HealthBarTarget");
            }

            if (m_locator == null)
            {
                if (m_fxHelper != null)
                {
                    m_locator = m_fxHelper.HeadNubTransform;
                }
            }

            if (m_locator == null)
            {
                m_locator = CachedTransform;
            }

            if (m_healthBar == null)
            {
                m_healthBar = Hotfix_LT.UI.HealthBarHUDController.Instance.GetHUD();
            }

            if (m_healthBar == null)
            {
                EB.Debug.LogError("InitHealthBar: m_healthBar is null");
                return;
            }

            if (IsLeft())
            {
                m_healthBar.HpColor = Color.green;
            }
            else if (IsRight())
            {
                m_healthBar.HpColor = Color.red;
            }
            else
            {
                EB.Debug.LogError("InitHealthBar: direction error");
            }

            m_healthBar.MaxHp = 0;
            m_healthBar.Hp = 0;
            m_healthBar.TransitionHp = 0;
            m_healthBar.Shield = 0;
            m_healthBar.TransitionShield = 0;

            m_healthBarTransform = m_healthBar.mDMono.transform;

            if (m_fxHelper != null)
            {
                m_fxHelper.m_HealthBarFXAttachment = m_healthBar.buffAnchor;
            }

            m_healthBarMonotor = m_healthBar.mDMono.GetComponent<Main.Combat.UI.HealthBarHUDMonitor>();
            m_healthBarMonotor.SetLocator(m_locator);
        }

        public void DestroyHealthBar()
        {
            if (m_fxHelper != null)
            {
                m_fxHelper.m_HealthBarFXAttachment = null;
                m_fxHelper = null;
            }

            if (m_healthBar != null&& m_healthBar.mDMono!=null)
            {
                m_healthBar.Show(false);
                GameObject.Destroy(m_healthBar.mDMono.gameObject);
                m_healthBar = null;
                m_healthBarTransform = null;
            }

            m_moveCtrl = null;
            m_lastMoveState = MoveController.CombatantMoveState.kIdle;
            Hidden = false;
        }

        public void HideHealthBar(bool clean_transition)
        {
            Hidden = true;
            m_healthBar.Show(!Hidden);

            if (clean_transition)
            {
                TransitionHp = Hp;
                TransitionShield = Shield;
            }
        }

        public void ShowHealthBar()
        {
            Hidden = false;
            m_healthBar.Show(!Hidden);
        }

        public ParticleSystem PlayParticle(MoveEditor.ParticleEventProperties properties, bool forcePlay)
        {
            if (m_fxHelper != null)
            {
                return m_fxHelper.PlayParticle(properties, forcePlay);
            }
            return null;
        }

        public override object GetValueFrom(string methodName, object args)
        {
            switch(methodName)
            {
                case "PlayParticle":
                    Hashtable ht = args as Hashtable;
                    return PlayParticle(ht["properties"] as MoveEditor.ParticleEventProperties, (bool)ht["forcePlay"]);
            }

            return null;
        }

        public override void OnHandleMessage(string methodName, object value)
        {
            switch (methodName)
            {
                case "ShowHealthBar":
                    if (Hidden)
                    {
                        ShowHealthBar();
                    }
                    break;
                case "HideHealthBar":
                    HideHealthBar((bool)value);
                    break;
                case "SetHp":
                    Hp = (long)value;
                    break;
                case "SetMaxHp":
                    MaxHp = (long)value;
                    break;
                case "SetData":
                    if (HealthBar != null)
                    {
                        HealthBar.Data = value as CombatCharacterSyncData;
                    }
                    break;
                case "InitHealthBar":
                    InitHealthBar();
                    break;
                case "SetRoleAttr":
                    RoleAttr = (Data.eRoleAttr)value.ToInt32();
                    break;
                case "SetContainer":
                    HealthBar.container.CustomSetActive((bool)value);
                    break;
                case "DestroyHealthBar":
                    DestroyHealthBar();
                    break;
                case "SetRestrainFlag":
                    SetRestrainFlag((Data.eRoleAttr)value.ToInt32());
                    break;
                case "SetGainFlag":
                    SetGainFlag();
                    break;
                case "HideRestrainFlag":
                    HideRestrainFlag();
                    break;
                case "UpdateMoveBar":
                    UpdateMoveBar((float)value);
                    break;
                case "UpdateBuff":
                    UpdateBuff(value as ICollection<CombatCharacterSyncData.BuffData>);
                    break;
                case "SetPosition":
                    if (HealthBar.mDMono != null)
                    {
                        HealthBar.mDMono.transform.localPosition = (Vector3)value;
                    }
                    break;
                case "OnHpChange":
                    var data = value as CombatCharacterSyncData;

                    if (Hp != data.Hp)
                    {
                        if (data.Hp>0) Hp = data.Hp;
                        if(data.MaxHp>0) MaxHp = data.MaxHp;

                        if (data.IsBoss)
                        {
                            Hotfix_LT.UI.LTCombatHudController.Instance.BossHealthBarCtrl.UpdateHp(data.Hp);
                        }
                    }
                    break;
            }
        }
    }
}