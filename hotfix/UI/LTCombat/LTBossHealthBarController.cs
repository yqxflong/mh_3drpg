using System;
using Hotfix_LT.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class LTBossHealthBarController : HealthBarBase, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;

            BuffIconItems = new BuffIconItem[10];
            for (int i = 0; i < BuffIconItems.Length; i++)
            {
                BuffIconItems[i] = new BuffIconItem()
                {
                    IconSprite = t.parent.GetComponent<UISprite>(String.Format("BossHeadInfo/BuffIcons/{0}",i)),
                    LeftTurnLabel = t.parent.GetComponent<UILabel>(string.Format("BossHeadInfo/BuffIcons/{0}/Label_Depth1",i)),
                    OverlyingLabel = t.parent.GetComponent<UILabel>(string.Format("BossHeadInfo/BuffIcons/{0}/Label_Depth2",i))
                }; 
            }
            Container = t.FindEx("Container").gameObject;
            NameLabel = t.parent.GetComponent<UILabel>("BossHeadInfo/Name");
            Indicator = t.parent.GetComponent<UILabel>("BossHeadInfo/BuffIcons/Indicator/Label_Depth1");
            Icon = t.parent.GetComponent<UISprite>("BossHeadInfo/Icon/Sprite2");
            NextSkillIcon = t.parent.GetComponent<DynamicUISprite>("BossHeadInfo/NextSkillIcon");
            CurrentHPPipe = t.parent.GetComponent<UISprite>("BossHeadInfo/Hp");
            CurrentHPPipeMask = t.parent.GetComponent<UISprite>("BossHeadInfo/Hp/Hp (1)");
            LerpHPPipe = t.parent.GetComponent<UISprite>("BossHeadInfo/HpLerp");
            NextHPPipe = t.parent.GetComponent<UISprite>("BossHeadInfo/HpNext");
            MonsterType = t.parent.GetComponent<UISprite>("BossHeadInfo/Icon/Type");
            HPPipeNumLabel = t.parent.GetComponent<UILabel>("BossHeadInfo/HipeNum/Label");
            MoveBarProgress = t.parent.GetComponent<UIProgressBar>("BossHeadInfo/MoveBarBG");
            HeadHud = t.parent.FindEx("BossHeadInfo").gameObject;
            SkillListHud = t.FindEx("Container/SkillList").gameObject;
            BSItem = t.GetMonoILRComponent<BossSkillItem>("Container/SkillList/SkillItem");
            SkillGrid = t.GetComponent<UIGrid>("Container/SkillList/Grid");
            BuffGrid = t.parent.GetComponent<UIGrid>("BossHeadInfo/BuffIcons");
            NextSkillTipsHud = t.FindEx("Container/Panel/NextSkillTips").gameObject;
            NextSkillItem = t.GetMonoILRComponent<BossSkillItem>("Container/Panel/NextSkillTips/SkillItem");
            LerpHpSpeed = 0.3f;
            ChestObj = t.parent.FindEx("BossHeadInfo/ChestNum").gameObject;
            ChestNum = t.parent.GetComponent<UILabel>("BossHeadInfo/ChestNum/Label");
            HpObj = t.parent.FindEx("BossHeadInfo/HipeNum").gameObject;
            PSJumpObj = t.parent.FindEx("BossHeadInfo/ChestNum/ps_0").gameObject;
            PSLightObj = t.parent.FindEx("BossHeadInfo/ChestNum/ps_1").gameObject;
            ChestImage = t.parent.GetComponent<UISprite>("BossHeadInfo/ChestNum");
            HurtLabel = t.parent.GetComponent<UILabel>("BossHeadInfo/HurtLabel");

            t.GetComponent<UIEventTrigger>("Container/SkillList").onClick.Add(new EventDelegate(OnCloseSkillTips));
            t.GetComponent<UIEventTrigger>("Container/SkillList/SkillGuideBlar").onClick.Add(new EventDelegate(OnCloseSkillTips));
            t.GetComponent<UIEventTrigger>("Container/Panel/NextSkillTips").onClick.Add(new EventDelegate(OnCloseNextSkillTips));


            Hotfix_LT.Messenger.AddListener<Hotfix_LT.Combat.CombatHitDamageEvent>(Hotfix_LT.EventName.CombatHitDamageEvent, OnHitCombatantListener);

            //EventManager.instance.AddListener<CombatHitDamageEvent>(OnHitCombatantListener);
            //EventManager.instance.AddListener<CombatDamageEvent>(OnDamageListener);  //TODO
            PerPipeSpriteLength = NextHPPipe.width;
            Container.gameObject.SetActive(false);
            HeadHud.gameObject.SetActive(false);
            SkillListHud.gameObject.SetActive(false);
            NextSkillTipsHud.gameObject.SetActive(false);
            playPSList = new List<int>();
        }

        public GameObject Container;
        public UILabel NameLabel;
        public UILabel Indicator;
        public UISprite Icon;
        public DynamicUISprite NextSkillIcon;
        public UISprite CurrentHPPipe;
        public UISprite CurrentHPPipeMask;
        public UISprite LerpHPPipe;
        public UISprite NextHPPipe;
        public UISprite MonsterType;
        public UILabel HPPipeNumLabel;
        public UIProgressBar MoveBarProgress;
        public GameObject HeadHud;
        public GameObject SkillListHud;
        public BossSkillItem BSItem;
        public UIGrid SkillGrid;
        public UIGrid BuffGrid;
    	public GameObject NextSkillTipsHud;
    	public BossSkillItem NextSkillItem;
    	//public List<BossSkillItem> SkillItemList;
    	public bool IsSkillHudVisible { get { return SkillListHud.activeSelf||NextSkillTipsHud.activeSelf; } }
        public int NextSkillID { get; private set; }
        //红，橙，蓝
        public Color[] HipeToColor = new Color[3] { new Color32(233, 25, 56, 255), new Color32(255, 133, 0, 255), new Color32(0, 126, 227, 255) };
        public float LerpHpSpeed;
        /// <summary>
        /// 宝箱对象
        /// </summary>
        public GameObject ChestObj;
        /// <summary>
        /// 宝箱个数
        /// </summary>
        public UILabel ChestNum;
        /// <summary>
        /// 血量对象
        /// </summary>
        public GameObject HpObj;
        /// <summary>
        /// 宝箱跳的特效对象
        /// </summary>
        public GameObject PSJumpObj;
        /// <summary>
        /// 宝箱发光的特效
        /// </summary>
        public GameObject PSLightObj;
        /// <summary>
        /// 宝箱图片
        /// </summary>
        public UISprite ChestImage;
        /// <summary>
        /// 伤害文本
        /// </summary>
        public UILabel HurtLabel;
        /// <summary>
        /// 当前总的伤害值
        /// </summary>
        private long m_CurrentHurtTotal;

        Hotfix_LT.Combat.CombatCharacterSyncData characterData;
        float PerPipeHp;
    	int Hp_Number;
        int PerPipeSpriteLength;
        bool mIsFirstUpdateHp = true;
        int mPreviousHipeNum;
        private long mHp;
        Coroutine LerpHpCoroutine;
    
        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener<Hotfix_LT.Combat.CombatHitDamageEvent>(Hotfix_LT.EventName.CombatHitDamageEvent, OnHitCombatantListener);

            //EventManager.instance.RemoveListener<CombatHitDamageEvent>(OnHitCombatantListener);
            //EventManager.instance.RemoveListener<CombatDamageEvent>(OnDamageListener); //TODO
        }

        public void InitializeData(Hotfix_LT.Combat.CombatCharacterSyncData data /*int tplId,int maxHp*/)
        {
            characterData = data;
            //TplId = tplId;
            //MaxHp = maxHp;
            Hotfix_LT.Data.MonsterInfoTemplate monsterInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(data.TplId);
            if (monsterInfo != null)
            {
                if (monsterInfo.hp_number != 0)
                {
                    Hp_Number = monsterInfo.hp_number;
                    PerPipeHp = data.MaxHp / monsterInfo.hp_number;
                }
    
                Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(monsterInfo.character_id);
                if (heroInfo != null)
                {
                    MonsterType.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[(Hotfix_LT.Data.eRoleAttr)heroInfo.char_type];
                    //UIShowItem.ShowCharTypeFX(charFx, efClip, MonsterType.transform, (PartnerGrade)heroInfo.role_grade, heroInfo.char_type);
                }
            }
            this.ResetCurrentHurt(m_CurrentHurtTotal);
        }
    
        public void Show()
        {
            CurrentHPPipe.gameObject.SetActive(true);
            NextHPPipe.gameObject.SetActive(true);
            LerpHPPipe.gameObject.SetActive(true);
            HeadHud.SetActive(true);
            ChestObj.SetActive(SceneLogic.BattleType == eBattleType.AllianceCampaignBattle);
            HpObj.SetActive(SceneLogic.BattleType != eBattleType.AllianceCampaignBattle);
            HurtLabel.gameObject.SetActive(SceneLogic.BattleType == eBattleType.AllianceCampaignBattle);
            //
            string characterId = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacteridEX(characterData.TplId.ToString());
            Hotfix_LT.Data.HeroInfoTemplate infoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterId, characterData.Skin);
            LTUIUtil.SetText(NameLabel, infoTpl.name);
            Icon.spriteName = infoTpl.icon;
    
            Indicator.transform.parent.gameObject.SetActive(false);
            foreach (var skill in characterData.SkillDataList)
            {
                //BossSkillItem item = GameUtils.InstantiateEx<BossSkillItem>(BSItem, SkillGrid.transform, skill.Value.Index.ToString());

                GameObject go = GameObject.Instantiate(BSItem.mDMono.gameObject) as GameObject;
                go.name = skill.Value.Index.ToString();
                go.transform.SetParent(SkillGrid.transform, false);
                if (false == go.activeSelf) go.gameObject.SetActive(true);
                BossSkillItem item = go.GetMonoILRComponent<BossSkillItem>();

                Hotfix_LT.Data.SkillTemplate tplData = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(skill.Value.ID);
                item.Fill(tplData, skill.Value.Level);
                if (tplData.Type == Hotfix_LT.Data.eSkillType.INDICATOR)
                {
                    Indicator.transform.parent.gameObject.SetActive(true);
                    Indicator.text = "0";
                }
            }
            SkillGrid.Reposition();
            BuffGrid.Reposition();
            if (characterData.SkillDataList.Count >= 5)
            {
                SkillGrid.GetComponent<UIWidget>().enabled = true;
            }
        }
    
        void OnHitCombatantListener(Hotfix_LT.Combat.CombatHitDamageEvent e)
        {
            var combatant = e.TargetCombatant.GetComponent<Hotfix_LT.Combat.Combatant>();
            if (!combatant.Data.IsBoss)
            {
                return;
            }
            long currentHP = combatant.GetHP();
            UpdateHp(currentHP);
        }
    
    	public override void UpdateIndicator(int count)
        {
            Indicator.text = count.ToString();
        }
    
        void OnDamageListener(Hotfix_LT.Combat.CombatDamageEvent e)
        {
            var combatant = e.Target.GetComponent<Hotfix_LT.Combat.Combatant>();
            if (!combatant.Data.IsBoss)
            {
                return;
            }
    
            long currentHP = combatant.GetHP();
            UpdateHp(currentHP);
            Hotfix_LT.Combat.CombatInfoData.GetInstance().LogString(string.Format("BOSS血条更新 target = {0}, dmg = {1}\n", e.Target.name, e.Damage));
        }
    
        public void UpdateHp(long hp)
        {
            if(!this.mDMono.isActiveAndEnabled){
                return;
            }

            if (SceneLogic.BattleType == eBattleType.AllianceCampaignBattle)
            {
                return;
            }
            
    		if (hp < 0)
    		{
    			hp = 0;
    		}
    
    		int pipeNum_total = 0;
    		if (hp >= characterData.MaxHp)
    		{ 
    			pipeNum_total = Hp_Number;
    		}
    		else
    		{
    			pipeNum_total = Mathf.CeilToInt((float)hp / PerPipeHp);
    		}
    		pipeNum_total = pipeNum_total == 0 ? 1 : pipeNum_total;
            int pipeNumWithColor = pipeNum_total % 3;
            if (pipeNumWithColor == 0)
                pipeNumWithColor = 3;
    
            if (hp != 0)
            {
                LTUIUtil.SetText(HPPipeNumLabel, "x" + pipeNum_total);
            }
            else
            {
                LTUIUtil.SetText(HPPipeNumLabel, "x0");
                CurrentHPPipe.gameObject.SetActive(false);
    			CurrentHPPipeMask.gameObject.SetActive(false);
    			LerpHPPipe.gameObject.SetActive(false);
    			NextHPPipe.gameObject.SetActive(false);
            }
    
            CurrentHPPipe.color = HipeToColor[pipeNumWithColor - 1];
            int nextPipeNumWithColor = pipeNumWithColor - 1;
            if (nextPipeNumWithColor == 0 && pipeNum_total > 3)
            {
                nextPipeNumWithColor = 3;
            }
            if (nextPipeNumWithColor == 0)  //最后一管血
            {
                NextHPPipe.gameObject.SetActive(false);
            }
            else
            {
                if (NextHPPipe.gameObject.activeSelf == false)
                {
                    NextHPPipe.gameObject.SetActive(true);
                }

                NextHPPipe.color = HipeToColor[nextPipeNumWithColor - 1];
            }
    
            int curHpSpriteWidth = 0;
    		if (hp >= characterData.MaxHp)
    		{
    			curHpSpriteWidth = PerPipeSpriteLength;
    		}
            else if (hp != 0 && hp % PerPipeHp == 0)
            {
                curHpSpriteWidth = PerPipeSpriteLength;
            }
            else
            {
                curHpSpriteWidth = (int)(((hp % PerPipeHp) / PerPipeHp) * PerPipeSpriteLength);
            }
    
            float previous_hp_value = mHp;
            float previous_hipe_num = mPreviousHipeNum;
            mPreviousHipeNum = pipeNum_total;
            mHp = hp;
            if (!mIsFirstUpdateHp && previous_hp_value != hp)
            {
                float previous_lerp_value = CurrentHPPipeMask.width;
                if (previous_hipe_num != mPreviousHipeNum)
                {
                    previous_lerp_value = PerPipeSpriteLength;
                }
                float target_lerp_value = curHpSpriteWidth;
    
                if (LerpHpCoroutine != null)
                    StopCoroutine(LerpHpCoroutine);
                LerpHpCoroutine = StartCoroutine(LerpHp(previous_lerp_value, target_lerp_value));
            }
            if (mIsFirstUpdateHp)
            {
                LerpHPPipe.width = curHpSpriteWidth;
            }
            mIsFirstUpdateHp = false;
            CurrentHPPipe.width = curHpSpriteWidth;
            CurrentHPPipeMask.width = CurrentHPPipe.width;
            NextHPPipe.width = PerPipeSpriteLength;
    
        }
        List<Hotfix_LT.Data.AllianceFBHurt> currentBossHurt;
        /// <summary>
        /// 用来缓存播放过的特效
        /// </summary>
        private List<int> playPSList;
        private Coroutine aniCoroutine;
        /// <summary>
        /// 设置当前的伤害值
        /// </summary>
        /// <param name="hurt">伤害值</param>
        public void ResetCurrentHurt(long hurt)
        {
            if (SceneLogic.BattleType != eBattleType.AllianceCampaignBattle)
            {
                return;
            }
            m_CurrentHurtTotal = hurt;
            if (characterData == null)
            {
                return;
            }
            int total = 0;
            float rate = 0.0f;
            //
            if (currentBossHurt == null)
            {
                currentBossHurt = Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBHurtList.FindAll(p => p.monsterId == characterData.TplId);
                currentBossHurt.Sort((x, y) =>
                {
                    return x.id - y.id;
                });
            }
            int currentMax = 0;
            int hurtMax = currentBossHurt.Count;
            //读取当前伤害
            for (int i = 0; i < currentBossHurt.Count; i++)
            {
                currentMax = currentBossHurt[i].hurt;
                rate = hurt / (float)currentBossHurt[i].hurt;
                if (rate <= 1.0f)
                {
                    break;
                }
                else
                {
                    hurt -= currentBossHurt[i].hurt;
                    total++;
                }
                if (i == hurtMax - 1)
                {
                    rate = hurt / (float)currentBossHurt[i].hurt;
                    while (rate > 1.0f)
                    {
                        hurt -= currentBossHurt[i].hurt;
                        rate = hurt / (float)currentBossHurt[i].hurt;
                        total++;
                    }
                }
            }
            if (total >= hurtMax && rate > 1.0f)
            {
                rate = 1.0f;
            }
    
            //
            LTUIUtil.SetText(HurtLabel, hurt + "/" + currentMax);
            LTUIUtil.SetText(ChestNum, "x" + total);
            if (total >= 1 && !playPSList.Contains(total))
            {
                if (aniCoroutine != null)
                {
                    StopCoroutine(aniCoroutine);
                    aniCoroutine = null;
                }
                aniCoroutine = StartCoroutine(SetParticlSystem());
                playPSList.Add(total);
            }
            
            //设置血条
            CurrentHPPipe.width = (int)(PerPipeSpriteLength * rate);
            CurrentHPPipeMask.width = CurrentHPPipe.width;
            
            //
            int pipeNumWithColor = total % 3 ;
            int nextPipeNumWithColor = (total + 1) % 3;
    
            CurrentHPPipe.gameObject.SetActive(false);
    
            LerpHPPipe.color = HipeToColor[pipeNumWithColor];
            NextHPPipe.color = HipeToColor[nextPipeNumWithColor];
            NextHPPipe.width = PerPipeSpriteLength;
           
            if (mIsFirstUpdateHp)
            {
                mIsFirstUpdateHp = false;
                LerpHPPipe.width = CurrentHPPipe.width;
            }
            else
            {
                //进度条走
                float previous_lerp_value = mHp;
                float target_lerp_value = CurrentHPPipe.width;
                if (LerpHpCoroutine != null)
                    StopCoroutine(LerpHpCoroutine);
                LerpHpCoroutine = StartCoroutine(LerpHp(previous_lerp_value, target_lerp_value));
    
            }
            mHp = CurrentHPPipe.width;
        }
        
        /// <summary>
        /// 处理特效
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetParticlSystem()
        {
            ChestImage.enabled = false;
            PSLightObj.SetActive(true);
            PSJumpObj.SetActive(true);
            yield return new WaitForSeconds(1.3f);
            PSJumpObj.SetActive(false);
            ChestImage.enabled = true;
        }
        
        /// <summary>
        /// 更新当前的伤害值
        /// </summary>
        /// <param name="hp">伤害值</param>
        public void UpdateHurt(long hurt)
        {
            if (SceneLogic.BattleType != eBattleType.AllianceCampaignBattle || hurt < 0)
            {
                return;
            }
            m_CurrentHurtTotal += (long)Mathf.Abs(hurt);
            this.ResetCurrentHurt(m_CurrentHurtTotal);
        }
    
        IEnumerator LerpHp(float previous_value, float target_value)
        {
            float lerpTime = 0;
            float transition_value = previous_value;
            while (Mathf.Abs(transition_value - target_value) >= 0.01f)
            {
                transition_value = Mathf.Lerp(previous_value, target_value, 1.0f - Mathf.Cos(lerpTime * Mathf.PI * LerpHpSpeed * 2));
                lerpTime += Time.deltaTime;
    
                LerpHPPipe.width = (int)transition_value;
                yield return null;
            }
            LerpHPPipe.width = (int)target_value;
            LerpHpCoroutine = null;
            yield break;
        }
    
        public void UpdateMoveBar(float value)
        {
            MoveBarProgress.value = value;
        }
    
        public void SetNextSkill(int skill_id)
        {
            NextSkillID = skill_id;
            if(skill_id == -1)
            {
                NextSkillIcon.spriteName = "";
                NextSkillIcon.gameObject.CustomSetActive(false);
                return;
            }
            Hotfix_LT.Data.SkillTemplate tpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(skill_id);
    		if (tpl != null)
    		{
    			NextSkillIcon.spriteName = tpl.Icon;
    
                if (NextSkillTipsHud.activeSelf)
    			{
    				int skillLevel = characterData.SkillDataList[NextSkillID].Level;
    				NextSkillItem.Fill(NextSkillID, skillLevel);
                }
                NextSkillIcon.gameObject.CustomSetActive(true);
            }
    		else
    		{
    			EB.Debug.LogError("SetNextBossSkill skill_id error for skill_id={0}", skill_id);
    			NextSkillIcon.spriteName = "";
                NextSkillIcon.gameObject.CustomSetActive(false);
            }
        }
    
        public void OnPopSkillTips()
        {
    		SkillListHud.SetActive(true);
    		Container.gameObject.SetActive(true);
    		UITweener[] tws = SkillListHud.GetComponents<UITweener>();

            if (tws == null)
            {
                return;
            }

    		for (var i = 0; i < tws.Length; i++)
    		{
                var tw = tws[i];
                tw.ResetToBeginning();
    			tw.PlayForward();
    		}
    	}
    
        public void OnCloseSkillTips()
        {
    		SkillListHud.CustomSetActive(false);
    		Container.gameObject.CustomSetActive(false);
    	}
    
    	public void OnPopNextSkillTips()
    	{
    		if (NextSkillID > 0 && characterData.SkillDataList.ContainsKey(NextSkillID))
    		{
    			int skillLevel = characterData.SkillDataList[NextSkillID].Level;
    			NextSkillItem.Fill(NextSkillID, skillLevel);
    			NextSkillTipsHud.CustomSetActive(true);
    			Container.gameObject.CustomSetActive(true);
    			UITweener[] tws = NextSkillTipsHud.GetComponents<UITweener>();

                if (tws != null)
                {
                    for (var i = 0; i < tws.Length; i++)
                    {
                        var tw = tws[i];
                        tw.ResetToBeginning();
                        tw.PlayForward();
                    }
                }
    		}
    	}
    
    	public void OnCloseNextSkillTips()
    	{
    		NextSkillTipsHud.CustomSetActive(false);
            SkillListHud.CustomSetActive(false);
            Container.gameObject.CustomSetActive(false);
    	}
    
    	public void CloseHud()
    	{
    		SkillListHud.CustomSetActive(false);
    		NextSkillTipsHud.CustomSetActive(false);
    		Container.gameObject.CustomSetActive(false);
    	}
    
    	public void CleanUp()
        {
    		NGUITools.DestroyChildren(SkillGrid.transform);
        }
    }
}
