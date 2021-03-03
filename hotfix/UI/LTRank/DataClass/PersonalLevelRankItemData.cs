using System.Collections;
using System.Collections.Generic;


namespace Hotfix_LT.UI
{
    public class PersonalLevelRankItemData : PersonalRankItemData
    {
        public int m_Xp;

        public PersonalLevelRankItemData() : base()
        {
            m_Xp = 0;
        }

        public PersonalLevelRankItemData(Hashtable data, int index) : base(data, index)
        {
            m_Xp = EB.Dot.Integer("xp", data, 0);
        }
    }

    //public class PersonalLevelRankItemDataComparer : IComparer<PersonalLevelRankItemData>
    //{
    //    public int Compare(PersonalLevelRankItemData x, PersonalLevelRankItemData y)
    //    {
    //        return x.m_Rank - y.m_Rank;
    //    }
    //}
}

