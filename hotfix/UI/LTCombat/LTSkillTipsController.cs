using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class LTSkillTipsController : DynamicMonoHotfix
    {
        public GameObject Container;
        public UILabel SkillNameLabel;
        public UILabel SkillDescLabel;
        public UISprite mBG;
        public TweenScale mTween;

        public Vector3 TipNormalPos = new Vector3(20, 0, 0);
        public Vector3 TipInChallengePos = new Vector3(-100, 0, 0);
        public UILabel SkillTargetLabel;
        public UISprite SkillTargetLabelBG;

        public UILabel SkillCooldownLabel;

        private int curSkillID;
        private Color FriendColor = new Color(67 / 255f, 253 / 255f, 122 / 255f, 1);

        private Color EnemyColor = new Color(254 / 255f, 104 / 255f, 154 / 255f, 1);

        private int bgHeight = 140;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Container = t.FindEx("Panel").gameObject;
            SkillNameLabel = t.GetComponent<UILabel>("Panel/Anchor/UpGroup/SkillName");
            SkillDescLabel = t.GetComponent<UILabel>("Panel/Anchor/SkillDesc");
            mBG = t.GetComponent<UISprite>("Panel/Anchor/BGSprite");
            mTween = t.GetComponent<TweenScale>("Panel/Anchor");

            SkillTargetLabel = t.GetComponent<UILabel>("Panel/Anchor/UpGroup/SkillTargetTypeBG/Label");
            SkillTargetLabelBG = t.GetComponent<UISprite>("Panel/Anchor/UpGroup/SkillTargetTypeBG");
            SkillCooldownLabel = t.GetComponent<UILabel>("Panel/Anchor/UpGroup/SkillTime/Label");
        }

        public void Init(int skillid, int level)
        {
    		Container.transform.localPosition = (SceneLogic.BattleType == eBattleType.ChallengeCampaign|| SceneLogic.BattleType == eBattleType.AlienMazeBattle) ? TipInChallengePos : TipNormalPos;
            Container.GetComponent<UIPanel>().depth += 1;
            ShowUI(false);
            if (curSkillID != 0 && curSkillID == skillid && Container.gameObject.activeSelf)
            {
                return;
            }
    
            curSkillID = skillid;
            Hotfix_LT.Data.SkillTemplate skillTpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(skillid);
            LTUIUtil.SetText(SkillNameLabel, skillTpl.Name);
            SkillDescLabel.text = SkillDescTransverter.ChangeDescription(skillTpl.Description, level, skillTpl.ID);
            SetSkillTargetLabel(skillTpl.SelectTargetType);
            string cooldownStr = (skillTpl.MaxCooldown > 0) ? (skillTpl.MaxCooldown + EB.Localizer.GetString("ID_uifont_in_CombatHudV4_TurnFont_4")) : EB.Localizer.GetString("ID_SKILL_COOLDOWN_NOT");
            SkillCooldownLabel.text = string.Format("{0}{1}", EB.Localizer.GetString("ID_SKILL_COOLDOWN"), cooldownStr);
            mBG.height = SkillDescLabel.height + bgHeight;
            ShowUI(true);
        }
    
        public void ShowUI(bool isShow)
        {
    		Container.gameObject.CustomSetActive(isShow);
            if (!isShow)
            {
                curSkillID = 0;
            }
            else
            {
                if (mTween != null)
                {
                    mTween.ResetToBeginning();
                    mTween.PlayForward();
                }
            }
        }
    
        private void SetSkillTargetLabel(Hotfix_LT.Data.eSkillSelectTargetType targetType)
        {
            switch (targetType)
            {
                case Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_ALL:
                    SkillTargetLabel.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_2525");
                    SkillTargetLabelBG.color = EnemyColor;
                    break;
                case Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_TEMPLATE:
                    SkillTargetLabel.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_2711");
                    SkillTargetLabelBG.color = EnemyColor;
                    break;
                case Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_RANDOM:
                    SkillTargetLabel.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_2895");
                    SkillTargetLabelBG.color = EnemyColor;
                    break;
                case Hotfix_LT.Data.eSkillSelectTargetType.SELF:
                    SkillTargetLabel.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3071");
                    SkillTargetLabelBG.color = FriendColor;
                    break;
                case Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_ALL:
                    SkillTargetLabel.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3252");
                    SkillTargetLabelBG.color = FriendColor;
                    break;
                case Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_RANDOM:
                    SkillTargetLabel.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3438");
                    SkillTargetLabelBG.color = FriendColor;
                    break;
                case Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_TEMPLATE:
                    SkillTargetLabel.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3626");
                    SkillTargetLabelBG.color = FriendColor;
                    break;
    			case Hotfix_LT.Data.eSkillSelectTargetType.All_NOT_SELF:
    				SkillTargetLabel.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3811");
    				SkillTargetLabelBG.color = EnemyColor;
    				break;
    		}
        }
    }
}
