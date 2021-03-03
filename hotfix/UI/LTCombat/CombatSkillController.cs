using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class CombatSkillController : DynamicMonoHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            SkillItemTemplate = t.GetMonoILRComponent<CombatSkillItem>("Template");
            SkillItemGrid = t.GetComponent<UIGrid>();
            SkillBGSprite = t.parent.GetComponent<UISprite>("SkillBG");
            SkillTipsCtrl = t.parent.parent.parent.parent.GetMonoILRComponent<LTSkillTipsController>("SkillTipsHUDController");


            BagBtn = t.parent.FindEx("ScrollBag").gameObject;
            BagBtn.GetComponent<UIButton>("Bag").onClick.Add(new EventDelegate(OnOpenBagClick));
            BagShadow = t.parent.FindEx("ScrollBag/Panel/Shadow").gameObject;
            BagSelectFrame = t.parent.FindEx("ScrollBag/Selected").gameObject;
            BagPanel = t.parent.parent.parent.parent.GetMonoILRComponent<LTCombatScrollBag>("LTChallengeInstanceBag");

            Hide();
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
        public override void OnDestroy()
        {

        }

        public CombatSkillItem SkillItemTemplate;
        public UIGrid SkillItemGrid;
        public UISprite SkillBGSprite;
        public LTSkillTipsController SkillTipsCtrl;
        public GameObject BagBtn;
        public GameObject BagShadow;
        public GameObject BagSelectFrame;
    	public LTCombatScrollBag BagPanel;
        private Hotfix_LT.Combat.Combatant _convergeTarget;
        private int _myTeam = 0;
        private int _targetTeam = 1;
        private int _selectSkillID;
        private int _selectSkillType;
        private readonly int _COMMON_TYPE = 0;
        private readonly int _SPECIAL_TYPE = 1;
        int UseScrollSkillId;
    	bool _hasSetBagSpace;
    	bool isShow;
        public Hotfix_LT.Combat.CombatCharacterSyncData CharacterData { get; private set; }
    
        public void InitData()
        {
            _targetTeam = 1 - CombatLogic.Instance.LocalPlayerTeamIndex;
        }
    
        public void Hide()
        {
    		isShow = false;
    		for (var i = 0; i < mDMono.transform.childCount; i++)
            {
                mDMono.transform.GetChild(i).gameObject.CustomSetActive(false);
            }
            BagBtn.gameObject.CustomSetActive(false);
            SkillBGSprite.gameObject.CustomSetActive(false);
    		SkillTipsCtrl.ShowUI(false);
        }
    
        public void Update()
        {
            if (GuideNodeManager.IsVirtualBtnGuide||CombatManager.Instance.GetSkillRequestState()) return;
            else
            {
                if (!string.IsNullOrEmpty( GuideNodeManager.VirtualBtnStr))
                {
                    string Str = GuideNodeManager.VirtualBtnStr;
                    GuideUpdateSelect(Str);
                    GuideNodeManager.VirtualBtnStr = null;
                }
                else UpdateSelect();
            }
        }
    
        void SetConvergeTarget(Hotfix_LT.Combat.Combatant target)
        {
            if(target != null)
            {
                var healthBar = target.gameObject.GetMonoILRComponent<Combat.HealthBar2D>();

                if (healthBar != null)
                {
                    healthBar.Converge = true;
                }
            }

            if (_convergeTarget != null && _convergeTarget != target)
            {
                var healthBar = _convergeTarget.gameObject.GetMonoILRComponent<Combat.HealthBar2D>();

                if (healthBar != null)
                {
                    healthBar.Converge = false;
                }
            }

            _convergeTarget = target;
        }

        /// <summary>
        /// 技能选择轮循（有目标才会向服务器发送消息,目前只有手动点击才会有目标）
        /// </summary>
        void UpdateSelect()
        {
            var target = CombatTargetSelectController.Instance.UpdateTargets();

            if (target != null && !GuideNodeManager.combatLimitClick)
            {
                if (LTCombatHudController.Instance != null && LTCombatHudController.Instance.AutoMode)
                {
                    SetConvergeTarget(target);
                }
                else
                {
                    if (CharacterData != null)
                    {
                        if (UseScrollSkillId > 0)
                        {
                            CombatManager.Instance.SetScrollSkill(CharacterData.ID, UseScrollSkillId, Combat.CombatSyncData.Instance.CombatId, _myTeam, CharacterData.IngameId, UseScrollSkillId, target.Data.IngameId);

                            if (BagSelectFrame != null)
                            {
                                BagSelectFrame.gameObject.CustomSetActive(false);
                            }

                            if (BagPanel != null)
                            {
                                BagPanel.OnUse();
                            }

                            UseScrollSkillId = 0;
                        }
                        else
                        {
                            CombatManager.Instance.SetSkill(CharacterData.ID, _selectSkillID, Hotfix_LT.Combat.CombatSyncData.Instance.CombatId, _myTeam, CharacterData.IngameId, GetSkillIndex(CharacterData, _selectSkillID) /*_selectSkillType*/, target.Data.IngameId);
                        }
                    }
                    else
                    {
                        EB.Debug.LogWarning("UpdateSelect: characterData is null ");
                    }
    
                    Hide();

                    if (LTCombatEventReceiver.Instance != null)
                    {
                        LTCombatEventReceiver.Instance.ForEach(combatant => { combatant.HideRestrainFlag(); });
                    }

                    CombatTargetSelectController.Instance.OnSkillSelectionDone();
                }
            }
        }

    /// <summary>
    /// 判断该目标是否可以成为技能释放的对象
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsCouldSetSkill(Hotfix_LT.Combat.Combatant target)
    {
        ///10001，该技能是一个被动技能，又该技能怪物无法被技能选择成为目标
        bool isCould = true;
            
            if (target != null && target.Data != null && target.Data.SkillDataList != null)
            {
                foreach (var skillId in target.Data.SkillDataList.Keys)
                {
                    if (skillId == 10001)
                    {
                        isCould = false;
                    }
                }
            }
    
            return isCould;
        }
    
        void GuideUpdateSelect(string guideStr)
        {
            string[] splits = guideStr.Split(',');
            int TargetId = int.Parse(splits[0]);
            int SkillIndex = GetSkillIndex(CharacterData, (splits.Length > 1) ? int.Parse(splits[1]) : _selectSkillID);
            CombatManager.Instance.SetSkill(CharacterData.ID, _selectSkillID, Hotfix_LT.Combat.CombatSyncData.Instance.CombatId, _myTeam, CharacterData.IngameId, SkillIndex, TargetId);
            Hide();
            LTCombatEventReceiver.Instance.ForEach(combatant => { combatant.HideRestrainFlag(); });
            CombatTargetSelectController.Instance.OnSkillSelectionDone();
            SkillTipsCtrl.ShowUI(false);
        }

        public void ShowSkill(Combat.CombatCharacterSyncData character_data)
        {
            isShow = true;
            if (character_data.Uid != LoginManager.Instance.LocalUserId.Value) return;
            CharacterData = character_data;
            int realSkillCount = 0;
            var emr = character_data.SkillDataList.Values.GetEnumerator();
            while (emr.MoveNext())
            {
                Combat.CombatCharacterSyncData.SkillData skilldata = emr.Current;
                if (skilldata.SkillType != (int)Data.eSkillType.NORMAL && skilldata.SkillType != (int)Data.eSkillType.PASSIVE && skilldata.SkillType != (int)Data.eSkillType.ACTIVE)
                {
                    continue;
                }
                realSkillCount++;
                int skillDataIndex = skilldata.Index;
                CombatSkillItem skillItem = null;
                if (skillDataIndex < mDMono.transform.childCount)
                {
                    skillItem = mDMono.transform.GetChild(skillDataIndex).GetMonoILRComponent<CombatSkillItem>();
                }
                if (skillItem == null)
                {
                    var go = GameObject.Instantiate(SkillItemTemplate.mDMono.gameObject) as GameObject;
                    go.name = "Clone";
                    go.transform.SetParent(mDMono.transform, false);
                    go.SetActive(true);
                    skillItem = go.GetMonoILRComponent<CombatSkillItem>();
                }
                if (skilldata != null)
                {
                    skillItem.mDMono.name = ((Data.eSkillType)skilldata.SkillType).ToString() + skillDataIndex;
                    skillItem.Fill(skilldata, skilldata.SkillType == (int)Data.eSkillType.NORMAL);
                }
                else
                    EB.Debug.LogError("skillData is null");
            }
            if (SceneLogic.BattleType == eBattleType.ChallengeCampaign || SceneLogic.BattleType == eBattleType.AlienMazeBattle)
            {
                realSkillCount++;
                if (!_hasSetBagSpace)
                {
                    _hasSetBagSpace = true;
                    mDMono.transform.localPosition -= new Vector3(SkillItemGrid.cellWidth, 0, 0);
                }
                BagBtn.gameObject.CustomSetActive(true);
                BagShadow.gameObject.CustomSetActive(CharacterData.ScrollTimes > 0);
            }
            SkillBGSprite.width = (779 - (4 - realSkillCount) * 162);
            SkillBGSprite.gameObject.CustomSetActive(true);

            for (int excessIndex = realSkillCount; ++excessIndex < mDMono.transform.childCount; ++excessIndex)
            {
                mDMono.transform.GetChild(excessIndex).gameObject.CustomSetActive(false);
            }
            SkillItemGrid.Reposition();

            if (character_data.NormalSkillData != null)
            {
                SetTargetingInfo(character_data.NormalSkillData.ID, _COMMON_TYPE);
            }
            else
            {
                EB.Debug.LogError("Not Found NormalSkill For Character:" + character_data);
            }
        }

        public void UpdateSkillList(Hotfix_LT.Combat.CombatCharacterSyncData character_data)
    	{
    		if (!isShow)
    			return;
    		if (character_data == CharacterData)
    		{
    			var emr = character_data.SkillDataList.Values.GetEnumerator();
                while (emr.MoveNext())
                {
                    Hotfix_LT.Combat.CombatCharacterSyncData.SkillData skilldata = emr.Current;
                    if (skilldata.SkillType != (int)Hotfix_LT.Data.eSkillType.NORMAL && skilldata.SkillType != (int)Hotfix_LT.Data.eSkillType.PASSIVE && skilldata.SkillType != (int)Hotfix_LT.Data.eSkillType.ACTIVE)
                    {
                        continue;
                    }
                    int skillDataIndex = skilldata.Index;
                    CombatSkillItem skillItem = null;
                    skillItem = mDMono.transform.GetChild(skillDataIndex).GetMonoILRComponent<CombatSkillItem>();

                    if (skilldata != null)
                    {
                        skillItem.mDMono.name = skilldata.SkillType.ToString() + skillDataIndex;
                        skillItem.Fill(skilldata, false, true);
                    }
                }
            }
    	}
    
    	public void AutoSelectSkill(Combat.CombatCharacterSyncData character_data)
    	{
            CharacterData = character_data;
    		Hide();
    		int skillID = 0;

    		if (character_data != null && LTCombatHudController.Instance != null && LTCombatHudController.Instance.AutoCombatItems != null)
    		{
    			AutoCombatItem autoCombatItem = LTCombatHudController.Instance.AutoCombatItems.Find(item => item != null && item.ItemCharSyncData != null && item.ItemCharSyncData.ID == character_data.ID);
    			
                if (autoCombatItem != null && autoCombatItem.IsNormalSkill) 
                {
                    skillID = character_data.GetCanUseSkill(autoCombatItem.IsNormalSkill);
                }
    		}

            if (_convergeTarget != null && _convergeTarget.GetHP() <= 0)
            {
                SetConvergeTarget(null);
            }

            if (character_data != null)
            {
                CombatManager.Instance.SetSkill(character_data.ID, 0, Combat.CombatSyncData.Instance.CombatId, _myTeam,
                  CharacterData.IngameId,
                  skillID == 0 ? -2 : GetSkillIndex(character_data, skillID),
                  _convergeTarget != null ? _convergeTarget.Data.IngameId : -2);
            }
    	}
    
    	public void AttackTargetUseAIWithSpecialSkill(Hotfix_LT.Combat.CombatCharacterSyncData character_data)
    	{
    		Hide();
    		LTCombatEventReceiver.Instance.ForEach(combatant => { combatant.HideRestrainFlag(); });
    		CombatManager.Instance.SetSkill(character_data.ID, 0, Hotfix_LT.Combat.CombatSyncData.Instance.CombatId, _myTeam, CharacterData.IngameId, -2, -2);
    	}
    
    	private int GetAttackTargetActionIDUseAI(int skillid, Hotfix_LT.Combat.Combatant converage)
        {
            Hotfix_LT.Data.SkillTemplate skill_tpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(skillid);
            switch (skill_tpl.SelectTargetType)
            {
                case Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_TEMPLATE:
                    {
                        if(converage != null)
                        {
                            return converage.Data.IngameId;
                        }
                        else
                        {
                            List<Hotfix_LT.Combat.CombatCharacterSyncData> enemyList = Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterList(1 - CombatLogic.Instance.LocalPlayerTeamIndex);
                            return FindMinHpOne(enemyList);
                        }
                    }
                case Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_ALL:
                case Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_RANDOM:
                case Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_ALL:
                case Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_RANDOM:
                case Hotfix_LT.Data.eSkillSelectTargetType.SELF:
                    {
                        return -1;
                    };
                case Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_TEMPLATE:
                    {
                        List<Hotfix_LT.Combat.CombatCharacterSyncData> friendList = Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterList(CombatLogic.Instance.LocalPlayerTeamIndex);
                        return FindMinHpOne(friendList);
                    };
                case Hotfix_LT.Data.eSkillSelectTargetType.All_NOT_SELF:
                    {
                        List<Hotfix_LT.Combat.CombatCharacterSyncData> friendList = Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterList(CombatLogic.Instance.LocalPlayerTeamIndex);
                        return FindMinHpOne(friendList, Hotfix_LT.Combat.CombatSyncData.Instance.ActionId);
                    }
                default:
                    EB.Debug.LogError("skillSelectTargetType error for skillid={0} gskillSelectTargetType={1},characterData={2}" , skillid , skill_tpl.SelectTargetType , CharacterData.ToString());
                    return -2;
            }
        }
    
        /// <summary>
        /// 设置技能可选择的目标信息
        /// </summary>
        /// <param name="skill_id"></param>
        /// <param name="type"></param>
        private void SetTargetingInfo(int skill_id, int type)
        {
            //int target_team = -1;
            //int max_targets = 1;
            _selectSkillID = skill_id;
            _selectSkillType = type;
            List<int> target_indexes = new List<int>();
    
            Hotfix_LT.Data.SkillTemplate skill_tpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(skill_id);
            Hotfix_LT.Data.eSkillSelectTargetType selectTargetType = skill_tpl.SelectTargetType;
            if (selectTargetType == Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_TEMPLATE
                || selectTargetType == Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_ALL
                || selectTargetType == Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_RANDOM)
            {
                _targetTeam = 1 - CombatLogic.Instance.LocalPlayerTeamIndex;
                LTCombatEventReceiver.Instance.ForEach(combatant =>
                {
                    if(combatant.Index == null){
                        return;
                    }
                    if (combatant.Index.TeamIndex == _targetTeam && (!combatant.IsDead() || skill_tpl.CanSelectDeath) && combatant.CanSelect())
                    {
                        combatant.SetRestrainFlag(CharacterData.Attr);
                        target_indexes.Add(combatant.Index.IndexOnTeam);
                    }
                    else
                    {
                        combatant.HideRestrainFlag();
                    }
                });
            }
            else if (selectTargetType == Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_TEMPLATE
                || selectTargetType == Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_ALL
                || selectTargetType == Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_RANDOM)
            {
                _targetTeam = CombatLogic.Instance.LocalPlayerTeamIndex;
                LTCombatEventReceiver.Instance.ForEach(combatant =>
                {
                    if (combatant.Index.TeamIndex == _targetTeam && (!combatant.IsDead() || skill_tpl.CanSelectDeath) && combatant.CanSelect())
                    {
                        combatant.SetGainFlag();
                        target_indexes.Add(combatant.Index.IndexOnTeam);
                    }
                    else
                    {
                        combatant.HideRestrainFlag();
                    }
                });
            }
            else if (selectTargetType == Hotfix_LT.Data.eSkillSelectTargetType.SELF)
            {
                _targetTeam = CombatLogic.Instance.LocalPlayerTeamIndex;
                LTCombatEventReceiver.Instance.ForEach(combatant =>
                {
                    if (combatant.Index.TeamIndex == _targetTeam && combatant.Index.IndexOnTeam == CharacterData.IndexOnTeam && (!combatant.IsDead() || skill_tpl.CanSelectDeath) && combatant.CanSelect())
                    {
                        combatant.SetGainFlag();
                        target_indexes.Add(combatant.Index.IndexOnTeam);
                    }
                    else
                    {
                        combatant.HideRestrainFlag();
                    }
                });
            }
            else if (selectTargetType == Hotfix_LT.Data.eSkillSelectTargetType.All_NOT_SELF)
            {
                _targetTeam = CombatLogic.Instance.LocalPlayerTeamIndex;
                LTCombatEventReceiver.Instance.ForEach(combatant =>
                {
                    if (combatant.Data.IngameId != CharacterData.IngameId && (!combatant.IsDead() || skill_tpl.CanSelectDeath) && combatant.CanSelect())
                    {
                        if (combatant.Index.TeamIndex == _targetTeam)
                        {
                            combatant.SetGainFlag();
                        }
                        else
                        {
                            combatant.SetRestrainFlag(CharacterData.Attr);
                        }
                        target_indexes.Add(combatant.Index.IndexOnTeam);
                    }
                    else
                    {
                        combatant.HideRestrainFlag();
                    }
                });
            }
            else
            {
                EB.Debug.LogError("skillSelectType error for skillid={0}" , skill_id);
            }
            CombatTargetSelectController.Instance.SetTargetingInfo(CharacterData.Index, _targetTeam, 1, target_indexes);
        }

        #region cpu耗时 3ms
        private static List<int> stTargetIndexes = new List<int>();
        public void SetConvergeTargeting()
        {
            stTargetIndexes.Clear();
            int enemyTeam = 1 - CombatLogic.Instance.LocalPlayerTeamIndex;
            LTCombatEventReceiver.Instance.ForEach(combatant =>
            {
                if(combatant.Index == null){
                    return;
                }
                if (combatant.Index.TeamIndex == enemyTeam && (!combatant.IsDead()))
                {
                    stTargetIndexes.Add(combatant.Index.IndexOnTeam);
                }
            });
            var Myteam = Hotfix_LT.Combat.CombatSyncData.Instance.GetMyTeamData();
            if(Myteam != null)
            {
                var charlist = Hotfix_LT.Combat.CombatSyncData.Instance.GetMyTeamData().CharList;
                if(charlist.Count > 0) CharacterData = charlist[0];
            }
            if(CharacterData!=null)CombatTargetSelectController.Instance.SetTargetingInfo(CharacterData.Index, enemyTeam, 1, stTargetIndexes);
        }
        #endregion
    
        private int FindMinHpOne(List<Hotfix_LT.Combat.CombatCharacterSyncData> actorList, int exclude = -1)
        {
            var target = actorList[0];
            long minHp = target.Hp;
            for (int i = 0; i < actorList.Count; ++i)
            {
                var Actor = actorList[i];
                if (Actor.IngameId == exclude) continue;
                if (minHp <= 0)
                {
                    target = Actor;
                    minHp = target.Hp;
                }
                if (Actor.Hp < minHp && Actor.Hp > 0)
                {
                    target = Actor;
                    minHp = target.Hp;
                }
            }
            return target.IngameId;
        }
    
        int GetSkillIndex(Hotfix_LT.Combat.CombatCharacterSyncData chardata, int skillid)
        {
    		if(chardata.SkillDataList.ContainsKey(skillid))
    			return chardata.SkillDataList[skillid].Index;
    		EB.Debug.LogError("SkillDataList Not ContainsKey for skillid={0}",skillid);
    		return 0;
        }
        
        public void ClearConvergeInfo()
        {
            if (_convergeTarget != null)
            {
                var healthBar = _convergeTarget.gameObject.GetMonoILRComponent<Combat.HealthBar2D>();

                if (healthBar != null)
                {
                    healthBar.Converge = false;
                }

                _convergeTarget = null;
            }
        }
    
        public void ClearConvergeInfo(int ingameId)
        {
            if (_convergeTarget != null && _convergeTarget.Data != null && _convergeTarget.Data.IngameId == ingameId)
            {
                var healthBar = _convergeTarget.gameObject.GetMonoILRComponent<Combat.HealthBar2D>();

                if (healthBar != null)
                {
                    healthBar.Converge = false;
                }

                _convergeTarget = null;
            }
        }
    
        public void OnSkillSelectEvent(int skillID, Hotfix_LT.Data.eSkillType skillType)
        {
            UseScrollSkillId = 0;
            BagSelectFrame.gameObject.CustomSetActive(false);
            if (skillType == Hotfix_LT.Data.eSkillType.NORMAL)
            {
                SetTargetingInfo(skillID, _COMMON_TYPE);
            }
            else if (skillType == Hotfix_LT.Data.eSkillType.ACTIVE)
            {
                SetTargetingInfo(skillID, _SPECIAL_TYPE);
            }
            for (var i = 0; i < mDMono.transform.childCount; i++)
            {
                CombatSkillItem item = mDMono.transform.GetChild(i).GetMonoILRComponent<CombatSkillItem>();
                if (item.Data.ID != skillID)
                {
                    item.UnSelect();
                }
            }
        }
    
        public void OnSkillClickEvent(int skillid, int level)
        {
            SkillTipsCtrl.Init(skillid, level);
        }
    
        public void OnOpenBagClick()
        {
            if (CharacterData.ScrollTimes>0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_CombatSkillController_19040"));
                return;
            }
            BagPanel.Open();
        }
    
        public void OnSelectScrollEvent(int skillid)
        {
            UseScrollSkillId = skillid;
            BagSelectFrame.gameObject.CustomSetActive(true);
            for(var i = 0; i < mDMono.transform.childCount; i++)
            {
                CombatSkillItem item = mDMono.transform.GetChild(i).GetMonoILRComponent<CombatSkillItem>();
                item.UnSelect();
            }
            SetTargetingInfo(skillid, -1);
        }
    }
}
