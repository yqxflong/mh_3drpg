using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class PersonalLadderRankItemData : PersonalRankItemData
    {
        public string m_Stage;  //段位
        public int m_Points;    //积分

        public PersonalLadderRankItemData() : base()
        {
            m_Stage = string.Empty;
            m_Points = 0;
        }

        public PersonalLadderRankItemData(Hashtable data, int index) : base(data, index)
        {
            string defaultStage = "qingtong"; //EB.Localizer.GetString("ID_LADDER_STAGE_QINGTONG");
            m_Stage = EB.Dot.String("st", data, defaultStage);/*stage*/
            m_Points = EB.Dot.Integer("p", data, 0);/*points*/
            m_Parm = LadderController.GetStageCharacterName(m_Stage) + m_Points;

        }
    }

    //public class PersonalLadderRankItemDataComparer : IComparer<PersonalLadderRankItemData>
    //{
    //    public int Compare(PersonalLadderRankItemData x, PersonalLadderRankItemData y)
    //    {
    //        return x.m_Rank - y.m_Rank;
    //    }
    //}
}

