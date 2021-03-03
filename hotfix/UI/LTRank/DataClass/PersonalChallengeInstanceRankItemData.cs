using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hotfix_LT.UI
{
    public class PersonalChallengeInstanceRankItemData : PersonalRankItemData
    {
        public int m_WinNum;

        public PersonalChallengeInstanceRankItemData(Hashtable data, int index) : base(data, index)
        {
            m_Parm = EB.Dot.String("ly", data, "0");//改用大层后不再需要转化
            //int level = EB.Dot.Integer("ly", data, m_WinNum);
            //var CurChapter = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterById(level - 1);
            //m_Parm = "0";
            //if (CurChapter == null)
            //{
            //    m_Parm = "0";//没办法查到,因此还没开始挑战过的意思
            //}
            //else if (CurChapter.IsBoss)
            //{
            //    m_Parm = CurChapter.CurChapter.ToString();//没下一层表示通关了最高层
            //}
            //else
            //{
            //    m_Parm = (CurChapter.CurChapter - 1).ToString();
            //}
        }
    }
    
}

