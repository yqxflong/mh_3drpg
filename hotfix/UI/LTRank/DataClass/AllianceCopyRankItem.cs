using System.Collections;

namespace Hotfix_LT.UI
{
	public class AllianceCopyRankItemData : PersonalRankItemData
	{
		public int m_Damage;
		public double m_DamagePercent;

		public AllianceCopyRankItemData() : base()
		{
			m_Damage = 0;
			m_DamagePercent = 0.00;
		}

		public AllianceCopyRankItemData(Hashtable data, int index) : base(data, index)
		{
			m_Damage = EB.Dot.Integer("d", data, 0);
			m_DamagePercent = EB.Dot.Double("dp", data, 0.00);
		}
	}
}