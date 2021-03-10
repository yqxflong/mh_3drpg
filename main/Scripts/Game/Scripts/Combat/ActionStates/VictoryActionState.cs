using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
	public class VictoryActionState : CombatActionState
	{

		public VictoryActionState()
		{
			MoveState = MoveController.CombatantMoveState.kVictoryDance;
		}

		public override void Init(Combatant combatant)
		{
			base.Init(combatant);
		}

		public override void Start()
		{
			if (Combatant.MoveController.GetMoveByState(MoveController.CombatantMoveState.kVictoryDance) == null)
			{
				EB.Debug.LogError("VictoryDance move not found");
				End();
				return;
			}

			//float blend_time = 0.00f;// 0.15f * Combatant.MoveController.GetCurrentStateInfo().length;
			Combatant.MoveController.TransitionTo(MoveState);
			Combatant.MoveController.CrossFade(MoveController.m_dance_hash, 0f, 0, NormalizedTime);
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