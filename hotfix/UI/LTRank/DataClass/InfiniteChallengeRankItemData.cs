using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Hotfix_LT.UI
{
    public class InfiniteChallengeRankItemData : PersonalRankItemData
    {
        public string m_PassTime;

        public InfiniteChallengeRankItemData() : base()
        {
            m_Parm = "";
            m_DrawLevel = "";
            m_PassTime = string.Empty;
        }

        public InfiniteChallengeRankItemData(Hashtable data, int index) : base(data, index)
        {
            m_Parm = EB.Dot.String("ly", data, "0");/*layer*/
            m_PassTime = EB.Dot.String("pt", data, string.Empty);/*passTime*/
        }
    }

    //public class InifiniteChallengeRankItemDataComparer : IComparer<InfiniteChallengeRankItemData>
    //{
    //    public int Compare(InfiniteChallengeRankItemData x, InfiniteChallengeRankItemData y)
    //    {
    //        return x.m_Rank - y.m_Rank;
    //    }
    //}
}