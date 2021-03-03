using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hotfix_LT.UI
{
    public class AllianceLevelRankItemData : UI.CommonRankItemData
    {
        public int m_IconId;
        public string m_OwnerName; //帮主名字
        public int m_OwnerLevel; //帮主等级
        public string m_OwnerModel;  //帮主领导模型
        public int m_PeopleNum;  //帮派人数

        public AllianceLevelRankItemData()
        {
            m_Index = 0;
            m_Rank = -1;
            m_IconId = 0;
            m_OwnerName = "";
            m_OwnerModel = "";
            m_Parm = "";
            m_DrawLevel = "";
            m_PeopleNum = 0;
        }

        public AllianceLevelRankItemData(Hashtable data, int index)
        {
            m_Index = index;
            m_Rank = EB.Dot.Integer("r", data, m_Rank);
            m_IconId = EB.Dot.Integer("icon_id", data, 0);

            m_Icon = LegionModel.GetInstance().dicLegionSpriteName[m_IconId % 100];
            m_BadgeBGIcon = LegionModel.GetInstance().dicLegionBGSpriteName[m_IconId / 100];

            m_Name = EB.Dot.String("an", data, "");
            m_OwnerName = EB.Dot.String("on", data, "");
            m_OwnerLevel = EB.Dot.Integer("ol", data, 0);
            int tplId = EB.Dot.Integer("t_id", data, 15011);
            int skin = EB.Dot.Integer("os", data, 0);
            if (tplId != 0)
            {
                var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(tplId);
                if (tpl == null)
                {
                    tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(10011);
                }
                if (tpl != null)
                {
                    var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(tpl.character_id, skin);
                    if (heroInfo != null)
                    {
                        m_OwnerModel = heroInfo.model_name;//需添加皮肤
                    }
                    else
                    {
                        EB.Debug.LogError("cannot found heroInfo for c_id={0}", tpl.character_id);
                    }
                }
            }
            else
            {
               EB.Debug.LogError("AllianceLevelRankItemData tplId is null");
            }

            m_Parm = EB.Dot.String("l", data, "");
            m_PeopleNum = EB.Dot.Integer("pm", data, 0);
        }
    }

    //public class AllianceLevelRankItemDataComparer : IComparer<AllianceLevelRankItemData>
    //{
    //    public int Compare(AllianceLevelRankItemData x, AllianceLevelRankItemData y)
    //    {
    //        return x.m_Rank - y.m_Rank;
    //    }
    //}
}
