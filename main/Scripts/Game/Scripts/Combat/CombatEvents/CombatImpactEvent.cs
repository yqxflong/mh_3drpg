using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
	public class CombatImpactEvent : CombatEvent
	{
		protected CombatantIndex m_owner = null;

		public CombatantIndex Owner
		{
			get { return m_owner; }
		}

		public ImpactData ImpactData
		{
			get;
			private set;
		}

		public CombatImpactEvent()
		{
			m_type = eCombatEventType.IMPACT;
			m_timing = eCombatEventTiming.AUTO;
		}

		private List<CombatantIndex> cachedInvolved = null;
		public override List<CombatantIndex> GetInvolved()
		{
			if (cachedInvolved != null)
			{
				return cachedInvolved;
			}

			List<CombatantIndex> list = new List<CombatantIndex>();
			list.Add(Owner);

			int len = m_children.Count;
			for (int i = 0; i < len; i++)
            {
				CombatEffectEvent effect = m_children[i] as CombatEffectEvent;
				list.AddRange(effect.GetInvolved());
			}

			list.Sort();
			for (int i = list.Count - 1; i > 0; --i)
			{
				if (list[i].Equals(list[i - 1]))
				{
					list.RemoveAt(i);
				}
			}

			cachedInvolved = list;
			return list;
		}

		public override bool Parse(Hashtable info)
		{
			m_owner = CombatantIndex.Parse(info["sender"] as Hashtable);
			if (m_owner == null)
			{
				EB.Debug.LogWarning("CombatImpactEvent.Parse: owner is empty {0}", EB.JSON.Stringify(info));
				return false;
			}

			ImpactData = CombatUtil.ParseImpactData(info["impact_data"] as Hashtable, null);

			List<CombatEvent> children = CombatUtil.ParseEffects(info);
			if (children != null)
			{
				m_children = children;
			}

			int len = m_children.Count;
			for (int i = 0; i < len; i++)
            {
				CombatEffectEvent effect = m_children[i] as CombatEffectEvent;
				if (effect.Timing == eCombatEventTiming.AUTO)
				{
					effect.Timing = eCombatEventTiming.ON_START;
				}

				if (effect.Sender == null)
				{
					effect.Sender = new CombatantIndex(m_owner.TeamIndex, m_owner.IndexOnTeam);
				}
			}

			return true;
		}
	}
}