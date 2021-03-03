using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
	public class EntryActionState : ReadyActionState
	{
		public EntryActionState(Combatant combatant)
			: base(combatant, false)
		{
			MoveState = MoveController.CombatantMoveState.kEntry;
		}

		public EntryActionState()
		{

		}

		public override void Init(Combatant combatant)
		{
			base.Init(combatant);

			MoveState = MoveController.CombatantMoveState.kEntry;
		}

		public override void Start()
		{
			//float blend_time = 0.00f;// 0.15f * Combatant.MoveController.GetCurrentStateInfo().length;
			Combatant.MoveController.TransitionTo(MoveState);
			Combatant.MoveController.CrossFade(MoveController.m_entry_hash, 0, defaultLayer, NormalizedTime);
		}

		public override void Stop()
		{
			base.Stop();
		}

		public override void End()
		{
			base.End();
		}
	}
}