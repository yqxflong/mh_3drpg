using System.Collections;

namespace Hotfix_LT.UI
{
    public class PlayerStateDataLookup
    {
        private HeadBars2D m_HeadBars2D;

        // public override void Awake()
        // {
        //     base.Awake();
        //     if (m_HeadBars2D == null)
        //InitComponent();
        // }

        public void SetHeadBars(HeadBars2D headBars2D)
        {
            m_HeadBars2D = headBars2D;
        }

        private string mDataID = null;
        private bool mFighting = false;
        private bool mIsRedName = false;
        private eDartState mDartState = eDartState.None;

        /// <summary>
        /// 这里单独处理view逻辑
        /// </summary>
        public void OnLookUpUpdateView()
        {
            if (!string.IsNullOrEmpty(mDataID))
            {
                m_HeadBars2D.SetBarHudState(eHeadBarHud.FightStateHud, null, mFighting);

                {
                    Hashtable tmp = Johny.HashtablePool.Claim();
                    tmp.Add("TeamType", eTeamId.Player);
                    string uid = mDataID.Split('.')[2]; 
                    if (AllianceUtil.GetLocalUid().ToString().Equals(uid))
                    {
                        tmp.Add("PlayerType", ePlayerType.LocalPlayer);
                    }
                    else
                    {
                        tmp.Add("PlayerType", ePlayerType.OtherPlayer);
                    }
                    tmp.Add("RedName", mIsRedName);
                    
                    string redNameStatePath = mDataID.Replace("state", "promoid");
                    int promoid = 0;
                    DataLookupsCache.Instance.SearchDataByID<int>(redNameStatePath, out promoid);
                    tmp.Add("Promoid", promoid);

                    m_HeadBars2D.SetBarHudState(eHeadBarHud.PlayerHeadBarHud, tmp, true);
                }
                mDataID = null;
            }
        }

        public void OnLookupUpdate(string dataID, object value, object data)
        {
            if (dataID != null)
            {
                mDataID = dataID;
                mFighting = EB.Dot.Bool("F", value, false);
                mIsRedName = EB.Dot.Bool("R", value, false);
                mDartState = (eDartState)EB.Dot.Integer("TOR", value, 0);
            }
        }
    }
}