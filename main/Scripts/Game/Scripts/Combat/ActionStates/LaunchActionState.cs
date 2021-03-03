using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class LaunchActionState : ReadyActionState
    {
        public LaunchActionState(Combatant combatant)
            : base(combatant, false)
        {
            MoveState = MoveController.CombatantMoveState.kLaunch;
        }

        public LaunchActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveState = MoveController.CombatantMoveState.kLaunch;
        }

        public override void Start()
        {
            float blend_time = 0.00f;// 0.15f * Combatant.MoveController.GetCurrentStateInfo().length;
            Combatant.MoveController.TransitionTo(MoveState);
            Combatant.MoveController.CrossFade(MoveController.m_launch_hash, blend_time, defaultLayer, NormalizedTime);

            string name = Combatant.targetSelectSenderPS.Valiad ? Combatant.targetSelectSenderPS.Name : Combatant.defaultTargetSelectSenderPS;

            MoveEditor.ParticleEventProperties properties = new MoveEditor.ParticleEventProperties();
            properties._eventName = name;
            properties.FlippedParticleName = properties.ParticleName = name;
            properties._bodyPart = MoveEditor.BodyPart.Root;
            properties._parent = false;
            properties._stopOnOverride = false;
            properties._stopOnExit = false;

            Combatant.FXHelper.PlayParticle(properties, false);

            //EventManager.instance.Raise(new CombatLaunchEvent(Combatant));
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void End()
        {
            base.End();

            string name = Combatant.targetSelectSenderPS.Valiad ? Combatant.targetSelectSenderPS.Name : Combatant.defaultTargetSelectSenderPS;
            Combatant.FXHelper.StopParticle(name, true);

            //EventManager.instance.Raise(new CombatLeaveLaunchEvent(Combatant));
        }
    }
}