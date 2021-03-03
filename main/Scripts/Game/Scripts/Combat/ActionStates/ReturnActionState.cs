using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ReturnActionState : BackActionState
    {
        public ReturnActionState(Combatant combatant)
            : base(combatant)
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = ReturnFrame;
            NormalizedTime = StartFrame / move.NumFrames;
        }

        public ReturnActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = ReturnFrame;
            NormalizedTime = StartFrame / move.NumFrames;
        }

        public override void Start()
        {
            base.Start();

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);

            float returnpoint_offset = Combatant.CurrentMove._zOffsetCurve.Evaluate(ReturnFrame);

            if (Combatant.Index.TeamIndex == CombatLogic.Instance.DefenderTeamIndex)
            {
                returnpoint_offset = -returnpoint_offset;
            }

            float anim_distance = returnpoint_offset;
            float actual_distance = ReturnPoint.z - Combatant.OriginPosition.z;

            Scale = actual_distance / anim_distance;

            Vector3 current_position = Combatant.transform.position;
            current_position.y = ReturnPoint.y;
            current_position.z = ReturnPoint.z;
            Combatant.transform.position = current_position;
        }

        public override void End()
        {
            base.End();

            TryReturnToOriginPosition();
        }

        protected override void UpdateLocation()
        {
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            float current_frame = Combatant.CurrentMove.NumFrames * state_info.normalizedTime;
            float anim_z_offset = Combatant.CurrentMove._zOffsetCurve.Evaluate(current_frame);
            float anim_y_offset = Combatant.CurrentMove._yOffsetCurve.Evaluate(current_frame);
            if (Combatant.Index.TeamIndex == CombatLogic.Instance.DefenderTeamIndex)
            {
                anim_z_offset = -anim_z_offset;
            }
            float z = Combatant.OriginPosition.z + anim_z_offset * Scale;
            float y = Combatant.OriginPosition.y + anim_y_offset;

            Vector3 current_position = Combatant.transform.position;
            current_position.z = z;
            current_position.y = y;
            Combatant.transform.position = current_position;
        }
    }
}