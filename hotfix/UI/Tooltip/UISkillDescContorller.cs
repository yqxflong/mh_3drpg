using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class UISkillDescContorller : DynamicMonoHotfix
    {
        //技能icon 技能名称
        public UIWidget m_Container;
        public DynamicUISprite m_Icon;
        public UILabel m_SkillName;
        public UILabel m_SkillLevel;
        public UILabel m_SkillType;
        public UISprite m_SkillTypeBG;
        public UISprite m_BG;
        public UISprite m_SkillFrame;
        public int BGBaseHeight = 440;

        //详细描述
        public UILabel m_Context;
        public UILabel m_ContextAdditional;
        private Hotfix_LT.Data.SkillTemplate m_SkillData;
        private System.Action m_UseCallback;
        private int skillLevel = 0;

        public UISprite SkillTargetLabelBG;
        public UILabel SkillTargetLabel;

        public UILabel SkillCooldownLabel;

        private Color FriendColor = new Color(67 / 255f, 253 / 255f, 122 / 255f, 1);
        private Color EnemyColor = new Color(254 / 255f, 104 / 255f, 154 / 255f, 1);

        private Vector3 uiPos;

        private List<UISkillBuffTemplate> BuffTempList;
        public UISkillBuffTemplate BuffTemp;
        public GameObject BuffTipObj;
        public UITable BuffParentTable;
        public GameObject BuffDropObj;
        public UIScrollView BuffScrollView;

        public void SetData(Vector3 pos)
        {
            uiPos = pos;
        }

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Container = t.GetComponent<UIWidget>("Container");
            m_Icon = t.GetComponent<DynamicUISprite>("Container/SkillDes/BG/SkillItem/Icon");
            m_SkillName = t.GetComponent<UILabel>("Container/SkillDes/BG/NameLabel");
            m_SkillLevel = t.GetComponent<UILabel>("Container/SkillDes/BG/SkillItem/Sprite/Level");
            m_SkillType = t.GetComponent<UILabel>("Container/SkillDes/BG/SkillIconBG/Label");
            m_SkillTypeBG = t.GetComponent<UISprite>("Container/SkillDes/BG/SkillIconBG");
            m_BG = t.GetComponent<UISprite>("Container/SkillDes/BG");
            m_SkillFrame = t.GetComponent<UISprite>("Container/SkillDes/BG/SkillItem");
            BGBaseHeight = 440;
            m_Context = t.GetComponent<UILabel>("Container/SkillDes/SkillDes");
            m_ContextAdditional = t.GetComponent<UILabel>("Container/SkillDes/SkillDescAdditional");
            SkillTargetLabelBG = t.GetComponent<UISprite>("Container/SkillDes/BG/SkillTargetTypeBG");
            SkillTargetLabel = t.GetComponent<UILabel>("Container/SkillDes/BG/SkillTargetTypeBG/Label");
            SkillCooldownLabel = t.GetComponent<UILabel>("Container/SkillDes/BG/SkillTime/Label");
            BuffTemp = t.GetMonoILRComponent<UISkillBuffTemplate>("Container/SkillDes/BuffTemplate");
            BuffTipObj = t.FindEx("Container/SkillDes/BuffTip").gameObject;
            BuffParentTable = t.GetComponent<UITable>("Container/SkillDes/BuffTip/BuffList/Placeholder/Table");
            BuffDropObj = t.FindEx("Container/SkillDes/BuffTip/Drop").gameObject;
            BuffScrollView = t.GetComponent<UIScrollView>("Container/SkillDes/BuffTip/BuffList");
        }

        public override void Start()
        {
            UpdateUI();
        }

        public override void OnEnable()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            GetSkillData();
            if (m_SkillData == null) return;
            m_Icon.spriteName = m_SkillData.Icon;
            m_SkillName.text = m_SkillData.Name;
            m_SkillLevel.text = skillLevel.ToString();
            //觉醒框显示，利用觉醒技能list查找
            SkillSetTool.SkillFrameStateSet(m_SkillFrame, Hotfix_LT.Data.CharacterTemplateManager.Instance.IsAwakenSkill(m_SkillData.ID));
            SetSkillType(m_SkillData.Type);
            SetSkillTargetLabel(m_SkillData.SelectTargetType);
            string cooldownStr = (m_SkillData.MaxCooldown > 0) ? (m_SkillData.MaxCooldown + EB.Localizer.GetString("ID_uifont_in_CombatHudV4_TurnFont_4")) : EB.Localizer.GetString("ID_SKILL_COOLDOWN_NOT");
            SkillCooldownLabel.text = string.Format("{0}{1}", EB.Localizer.GetString("ID_SKILL_COOLDOWN"), cooldownStr);
            m_Context.text = GetContext();
            m_ContextAdditional.text = LTPartnerSkillBreakController.GetSkillAdditional(m_SkillData.ID, skillLevel);

            int buffHeight = 0;
            if (m_SkillData.BuffDescribleID != null)
            {
                BuffTipObj.CustomSetActive(true);
                if (BuffTempList == null) BuffTempList = new List<UISkillBuffTemplate>();
                for (int i = 0; i < BuffTempList.Count; i++)
                {
                    BuffTempList[i].Hide();
                }
                BuffDropObj.CustomSetActive(m_SkillData.BuffDescribleID.Count >= 3);
                for (int i = 0; i < m_SkillData.BuffDescribleID.Count; i++)
                {
                    var buff = Hotfix_LT.Data.BuffTemplateManager.Instance.GetTemplate(m_SkillData.BuffDescribleID[i]);
                    if (buff != null)
                    {
                        if (i >= BuffTempList.Count)
                        {
                            //新建buffTemp
                            UISkillBuffTemplate temp = Object.Instantiate(BuffTemp.mDMono, Vector3.zero, Quaternion.Euler(0, 0, 0), BuffParentTable.transform).transform.GetMonoILRComponent<UISkillBuffTemplate>();
                            temp.SetData(buff);
                            BuffTempList.Add(temp);
                        }
                        else
                        {
                            //复用buffTemp
                            BuffTempList[i].SetData(buff);
                        }
                        if (m_SkillData.BuffDescribleID.Count >= 3)
                        {
                            buffHeight = 525;
                        }
                        else
                        {
                            buffHeight += BuffTempList[i].GetBuffTempHeight() + 15;
                        }
                    }
                }
                BuffParentTable.Reposition();
                BuffScrollView.verticalScrollBar.value = 0;
            }
            else
            {
                BuffTipObj.CustomSetActive(false);
            }
            StartCoroutine(UpdatePos(buffHeight));
        }

        public void SetSkillTargetLabel(Hotfix_LT.Data.eSkillSelectTargetType targetType)
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

        private void SetSkillType(Hotfix_LT.Data.eSkillType skillType)
        {
            if (skillType == Hotfix_LT.Data.eSkillType.ACTIVE)
            {
                m_SkillType.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4070");
                m_SkillTypeBG.color = LT.Hotfix.Utility.ColorUtility.RedColor;
            }
            else if (skillType == Hotfix_LT.Data.eSkillType.NORMAL)
            {
                m_SkillType.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4440");
                m_SkillTypeBG.color = LT.Hotfix.Utility.ColorUtility.PurpleColor;
            }
            else if (skillType == Hotfix_LT.Data.eSkillType.PASSIVE)
            {
                m_SkillType.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4751");
                m_SkillTypeBG.color = LT.Hotfix.Utility.ColorUtility.BlueColor;
            }
        }

        private IEnumerator UpdatePos(int buffHeight)
        {
            m_Container.transform.localPosition = new Vector3(30000, 30000, 0);//先移到屏幕外，等到锚点对好位置之后在移回来
            yield return null;
            m_BG.height = BGBaseHeight + m_Context.height + m_ContextAdditional.height + buffHeight;

            //底图会根据文字长短做自适应，所以只能在一下帧的时候才开始做位置计算，使该界面不会超出屏幕外
            if (m_Context.height + m_ContextAdditional.height + buffHeight - uiPos.y > UIRoot.list[0].manualHeight / 2)
            {
                uiPos.y += (m_Context.height + m_ContextAdditional.height + buffHeight + 20) - UIRoot.list[0].manualHeight / 2;
            }
            m_Container.transform.localPosition = uiPos;
        }

        private void GetSkillData()
        {
            string input;
            if (!DataLookupsCache.Instance.SearchDataByID<string>("tooltip.target", out input))
            {
                m_SkillData = null;
                return;
            }

            string[] result = input.Split(',');
            int id = 0;
            if (result.Length > 1)
            {
                if (!int.TryParse(result[1], out skillLevel))
                {
                    skillLevel = 1;
                }
            }


            if (!int.TryParse(result[0], out id))
            {
                m_SkillData = null;
                return;
            }

            m_SkillData = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(id);
        }

        private string GetContext()
        {
            if (m_SkillData == null) return "";

            if (!string.IsNullOrEmpty(m_SkillData.Description))
            {
                return SkillDescTransverter.ChangeDescription(m_SkillData.Description, skillLevel, m_SkillData.ID);
            }
            return "";
        }
    }
}
