using UnityEngine;
using System.Collections;
namespace Hotfix_LT.UI
{

	public class HealthBarMainController : DynamicMonoHotfix
	{
	    public override	void Awake()
		{
            //EventManager.instance.AddListener<CombatDamageEvent>(OnDamageListener);
            //EventManager.instance.AddListener<CombatHealEvent>(OnHealListener);
            Hotfix_LT.Messenger.AddListener<Hotfix_LT.Combat.CombatDamageEvent>(Hotfix_LT.EventName.CombatDamageEvent, OnDamageListener);
            Hotfix_LT.Messenger.AddListener<Hotfix_LT.Combat.CombatHealEvent>(Hotfix_LT.EventName.CombatHealEvent, OnHealListener);
            Hotfix_LT.Messenger.AddListener<Hotfix_LT.Combat.CombatHitDamageEvent>(Hotfix_LT.EventName.CombatHitDamageEvent, OnHitCombatantListener);
		}

		public override void OnDestroy()
		{
            //EventManager.instance.RemoveListener<CombatDamageEvent>(OnDamageListener);
            //EventManager.instance.RemoveListener<CombatHealEvent>(OnHealListener);
            Hotfix_LT.Messenger.AddListener<Hotfix_LT.Combat.CombatDamageEvent>(Hotfix_LT.EventName.CombatDamageEvent, OnDamageListener);
            Hotfix_LT.Messenger.AddListener<Hotfix_LT.Combat.CombatHealEvent>(Hotfix_LT.EventName.CombatHealEvent, OnHealListener);
            Hotfix_LT.Messenger.RemoveListener<Hotfix_LT.Combat.CombatHitDamageEvent>(Hotfix_LT.EventName.CombatHitDamageEvent, OnHitCombatantListener);
		}

		void UpdateHP(GameObject target)
		{
			var combatant = target.GetComponent<Hotfix_LT.Combat.Combatant>();
			long currentHP = combatant.GetHP();
			long currentMaxHP = combatant.GetMaxHP();

			//Hotfix_LT.Combat.HealthBar2D health_bar = combatant.HealthBar;
			//if (health_bar != null)
			//{
			//	health_bar.Hp = currentHP;
			//	health_bar.MaxHp = currentMaxHP;
			//}
			var health_bar = combatant.HealthBar;
			if (health_bar != null)
			{
				health_bar.OnHandleMessage("SetHp", currentHP);
				health_bar.OnHandleMessage("SetMaxHp", currentMaxHP);
			}
		}

		void OnHitCombatantListener(Hotfix_LT.Combat.CombatHitDamageEvent e)
		{
			UpdateHP(e.TargetCombatant);
		}

		void OnDamageListener(Hotfix_LT.Combat.CombatDamageEvent e)
		{
			UpdateHP(e.Target);
			Hotfix_LT.Combat.CombatInfoData.GetInstance().LogString(string.Format("target = {0}, dmg = {1}\n", e.Target.name, e.Damage));

		}

		void OnHealListener(Hotfix_LT.Combat.CombatHealEvent e)
		{
			UpdateHP(e.Target);
		}
	}
}