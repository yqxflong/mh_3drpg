using System.Collections;
using System.Collections.Generic;


namespace Hotfix_LT.UI
{
    /// <summary>
    /// 个人排名项的数据
    /// </summary>
    public class PersonalRankItemData : CommonRankItemData
    {
        public int m_TplId;
        public int m_CharacterId;
        public int m_Skin;
        public int m_HeroType;

        public PersonalRankItemData()
        {
        }

        public PersonalRankItemData(Hashtable data, int index)
        {
            m_Index = index;
            m_Uid = EB.Dot.Long("u", data, m_Uid);/*uid*/
            m_Rank = EB.Dot.Integer("r", data, m_Rank);/*rank*/
            m_Parm = EB.Dot.String("l", data, m_Parm);/*level*/
            m_DrawLevel = EB.Dot.String("l", data, m_DrawLevel);//显示人物等级
            m_TplId = EB.Dot.Integer("t_id", data, 10011);/*template_id*/
            m_Skin = EB.Dot.Integer("s", data, 0);
            m_Frame = EB.Dot.String("hf", data, null);
            if (m_TplId > 0)
            {
                var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(m_TplId);
                if (tpl == null)
                {
                    tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(10011);
                }
                if (tpl != null)
                {
                    m_CharacterId = tpl.character_id;
                    m_HeroType = GetHeroType(m_CharacterId);
                    var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(tpl.character_id, m_Skin);
                    if (heroInfo != null)
                    {
                        m_Icon = heroInfo.icon;
                        m_Model = heroInfo.model_name;
                    }
                    else
                    {
                        EB.Debug.LogError("cannot found heroInfo for c_id = {0}" ,tpl.character_id);
                    }
                }
            }
            else
            {
                m_CharacterId = EB.Dot.Integer("c_id", data, m_CharacterId);
                if (m_CharacterId > 0)
                {
                    var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(m_CharacterId, m_Skin);
                    if (heroInfo != null)
                    {
                        m_Icon = heroInfo.icon;
                        m_Model = heroInfo.model_name;
                    }
                }
            }
            m_Name = EB.Dot.String("un", data, m_Name);/*name*/
        }

        public int GetHeroType(int char_id)
        {
            return BuddyAttributesManager.GetAttributeTypeByCharacterId(char_id.ToString());
        }
    }

    //public class PersonalRankItemDataComparer : IComparer<PersonalRankItemData>
    //{
    //    public int Compare(PersonalRankItemData x, PersonalRankItemData y)
    //    {
    //        return x.m_Rank - y.m_Rank;
    //    }
    //}

    public class PersonalFightRankItemData : PersonalRankItemData
    {
        public int m_Fight;

        public PersonalFightRankItemData() : base()
        {

            m_Fight = 0;
        }

        public PersonalFightRankItemData(Hashtable data, int index) : base(data, index)
        {
            m_Fight = EB.Dot.Integer("f", data, 0);/*fight*/
        }
    }

    // public abstract class PersonalRankItem<T> : CommonRankItem
    // {
    //    
    // }

    public class PersonalFightRankItem : BaseItem<PersonalFightRankItemData>
    {
        public UILabel m_UIName;
        public UILabel m_UILevel;
        public UISprite m_UIProfession;
        public UILabel m_UIFight;
        public RankIconOrNumber m_UIRank;
        public DynamicUISprite m_UIIcon;
        public PersonalFightRankItemData Data;
        public override void UpdateUI()
        {
            base.UpdateUI();
            PersonalFightRankItemData P_Data = Data;
            m_UIName.text = P_Data.m_Name;
            m_UIFight.text = P_Data.m_Fight + "";
            m_UILevel.text = P_Data.m_Parm + "";
            m_UIProfession.spriteName = BuddyAttributesManager.AttributeLogo(P_Data.m_HeroType);
            m_UIRank.Rank = P_Data.m_Rank;
            m_UIIcon.spriteName = BuddyAttributesManager.GetCharIcon(P_Data.m_CharacterId.ToString(), "_Main");
        }

        public override void Fill(PersonalFightRankItemData itemData)
        {
            Data = itemData;
            UpdateUI();
        }

        public override void Clean()
        {
            Data = null;
        }
    }
}