
namespace Hotfix_LT.UI
{
    /// <summary>
    /// 通用排名数据
    /// </summary>

    public class CommonRankItemData
    {
        public int m_Rank = -1;  //排名
        public string m_Icon;  //ICON
        public string m_Frame; //头像框
        public string m_Name = ""; //名字
        public string m_Parm;    //参数 注意：不是人物等级
        public string m_Model;
        public string m_DrawLevel; //人物等级

        public string m_BadgeBGIcon;
        public long m_Uid;
        public bool m_IsPlayer = true;
        public int m_Index = 0;//在列表中排第几个
        public RankType RankType;
    }

    public enum RankType
    {
        PersonalLevel,
        AllianceLevel,
        PersonalArena,
        PersonalLadder,
        PersonalInfinite,
        PersonalInstance
    }
}
