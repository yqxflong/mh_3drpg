using System.Collections;

namespace Hotfix_LT.UI
{
	public class PlayerNameDataLookup
	{

		private HeadBars2D m_HeadBars2D;


		//public override void Awake()
		//{
		//	base.Awake();
		//	if (m_HeadBars2D == null) InitComponent();
		//}

		public void SetHeadBars(HeadBars2D headBars2D)
		{
			m_HeadBars2D = headBars2D;
		}

		public void OnLookupUpdate(string dataID, object value)
		{
			if (dataID != null && value != null)
			{
				string uname = value.ToString();
				Hashtable tmp = Johny.HashtablePool.Claim();
				tmp.Add("Name", uname);
				bool isRedName = false;
				string[] splitArr = dataID.Split('.');
				string redNameStatePath = dataID.Replace(splitArr[splitArr.Length - 1], "") + "state.R";
				DataLookupsCache.Instance.SearchDataByID<bool>(redNameStatePath, out isRedName);
				tmp.Add("RedName", isRedName);
				//if (isRedName)
				//{
				//	tmp.Add("TeamType", eTeamId.Player);
				//	tmp.Add("PlayerType", ePlayerType.EnemyPlayer);  //redName need EnemyPlayer
				//	if (m_HeadBars2D != null)
				//	{
				//		m_HeadBars2D.SetBarHudState(eHeadBarHud.PlayerHeadBarHud, tmp, true);
				//	}
				//}
				//else
				{
					tmp.Add("TeamType", eTeamId.Player);
					string uid = dataID.Split('.')[2];  //mainlands.pl.1003180.un
					if (AllianceUtil.GetLocalUid().ToString().Equals(uid))
					{
						tmp.Add("PlayerType", ePlayerType.LocalPlayer);
					}
					else
					{
						tmp.Add("PlayerType", ePlayerType.OtherPlayer);
					}
					if (m_HeadBars2D != null)
					{
						m_HeadBars2D.SetBarHudState(eHeadBarHud.PlayerHeadBarHud, tmp, true);
					}
				}
			}
		}

		public void OnDestroy()
		{
			if (m_HeadBars2D != null)
			{
				m_HeadBars2D.SetBarHudState(eHeadBarHud.PlayerHeadBarHud, null, false);
			}
		}
	}
}