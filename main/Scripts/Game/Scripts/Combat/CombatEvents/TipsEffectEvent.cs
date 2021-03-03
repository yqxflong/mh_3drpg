using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
	public class TipsEffectEvent : CombatEffectEvent
	{
		public enum eTipsType
		{
			None,
			ImmuneDebuff,
			ImmuneControl,
			ImmunePhysicalAttack,
			ImmuneMagicAttack,
			ImmuneImpact,
			ReflectDebuff,
			ControlFailed
		}

		public eTipsType TipsType
		{
			get; private set;
		}

		public int SourceImpactId
		{
			get; private set;
		}

		public int TargetImpactId
		{
			get; private set;
		}

		public TipsEffectEvent()
		{
			m_effect_type = eCombatEffectType.TIPS;
		}

		public override bool Parse(Hashtable info)
		{
			if (!base.Parse(info))
			{
				return false;
			}

			string tips = EB.Dot.String("tips", info, string.Empty);
			if (tips == "immune_debuff")
			{
				TipsType = eTipsType.ImmuneDebuff;
			}
			else if (tips == "immune_control")
			{
				TipsType = eTipsType.ImmuneControl;
			}
			else if (tips == "immune_physical_attack")
			{
				TipsType = eTipsType.ImmunePhysicalAttack;
			}
			else if (tips == "immune_magic_attack")
			{
				TipsType = eTipsType.ImmuneMagicAttack;
			}
			else if (tips == "immune_impact")
			{
				TipsType = eTipsType.ImmuneImpact;
			}
			else if (tips == "reflect_debuff")
			{
				TipsType = eTipsType.ReflectDebuff;
			}
			else if (tips == "control_failed")
			{
				TipsType = eTipsType.ControlFailed;
			}

			if (TipsType == eTipsType.None)
			{
				EB.Debug.LogWarning("TipsEffectEvent.Parse: invalid tips {0}", tips);
				return false;
			}

			SourceImpactId = EB.Dot.Integer("sourceImpactId", info, -1);
			if (SourceImpactId < 0)
			{
				EB.Debug.Log("TipsEffectEvent.Parse: invalid source impact id {0}", SourceImpactId.ToString());
			}

			TargetImpactId = EB.Dot.Integer("targetImpactId", info, -1);
			if (TargetImpactId < 0)
			{
				if (TipsType != eTipsType.ImmuneMagicAttack && TipsType != eTipsType.ImmunePhysicalAttack)
				{
					EB.Debug.LogWarning("TipsEffectEvent.Parse: invalid target impact id {0}", TargetImpactId.ToString());
					return false;
				}
			}

			return true;
		}
	}
}