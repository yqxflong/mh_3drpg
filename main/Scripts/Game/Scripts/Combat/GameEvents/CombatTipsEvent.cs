using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
	public class CombatTipsEvent : GameEvent
	{
		public Combatant Combatant
		{
			get; set;
		}

		public TipsEffectEvent EffectEvent
		{
			get; set;
		}

		public CombatTipsEvent(Combatant combatant, TipsEffectEvent effect_event)
		{
			Combatant = combatant;
			EffectEvent = effect_event;
		}
	}
}