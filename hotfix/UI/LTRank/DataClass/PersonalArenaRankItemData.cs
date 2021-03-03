using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class PersonalArenaRankItemData : PersonalRankItemData
    {
        public int m_Fight;


        public PersonalArenaRankItemData(Hashtable data, int index) : base(data, index)
        {
            m_Fight = EB.Dot.Integer("f", data, m_Fight);/*battleRating*/
            m_IsPlayer = EB.Dot.String("t", data, "") == "player";
            m_CharacterId = EB.Dot.Integer("c_id", data, m_CharacterId);
            m_Skin = EB.Dot.Integer("s", data, 0);
            if (m_CharacterId > 0)
            {
                var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(m_CharacterId, m_Skin);
                if (heroInfo != null)
                {
                    m_Icon = heroInfo.icon;
                    m_Model = heroInfo.model_name;//需添加皮肤
                }
            }
        }
    }

    //public class PersonalArenaRankItemDataComparer : IComparer<PersonalArenaRankItemData>
    //{
    //    public int Compare(PersonalArenaRankItemData x, PersonalArenaRankItemData y)
    //    {
    //        return x.m_Rank - y.m_Rank;
    //    }
    //}
}

////点击事件和父物体不一样
// public class PersonalArenaRankItem : PersonalRankItem<PersonalArenaRankItemData>
//{
//    public UILabel m_UIName;
//    public UILabel m_UILevel;
//    public RankIconOrNumber m_UIRank;
//    public UISprite m_UIIcon;
//    //public UISprite m_QualityBorder;

//    public override void UpdateUI()
//    {
//        base.UpdateUI();
//        PersonalArenaRankItemData P_Data = (PersonalArenaRankItemData)Data;
//        m_UIRank.Rank = P_Data.m_Rank;
//        m_UIIcon.spriteName = P_Data.m_Icon;
//        LTUIUtil.SetText(m_UIName, P_Data.m_Name);
//        LTUIUtil.SetText(m_UILevel, P_Data.m_Level.ToString());
//    }

