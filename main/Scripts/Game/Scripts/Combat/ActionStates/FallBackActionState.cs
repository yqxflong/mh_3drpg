using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public class FallBackActionState : BackActionState
    {
        public FallBackActionState(Combatant combatant)
            : base(combatant)
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = BackFrame;
            NormalizedTime = StartFrame / move.NumFrames;

            EndFrame = CollideFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public FallBackActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = BackFrame;
            NormalizedTime = StartFrame / move.NumFrames;

            EndFrame = CollideFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public override void Start()
        {
            base.Start();

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);

            if (IsReachedEndPoint())
            {
                End();
                return;
            }

            float backpoint_offset = Combatant.CurrentMove._zOffsetCurve.Evaluate(BackFrame);
            float collidepoint_offset = Combatant.CurrentMove._zOffsetCurve.Evaluate(CollideFrame);

            if (Combatant.Index.TeamIndex == CombatLogic.Instance.DefenderTeamIndex)
            {
                backpoint_offset = -backpoint_offset;
                collidepoint_offset = -collidepoint_offset;
            }

            float anim_distance = collidepoint_offset - backpoint_offset;
            float actual_distance = (CollidePoint.z - Combatant.OriginPosition.z) - backpoint_offset;

            Scale = actual_distance / anim_distance;

            Vector3 current_position = Combatant.transform.position;
            current_position.z = BackPoint.z;
            current_position.y = BackPoint.y;
            Combatant.transform.position = current_position;

            //HealthBar2D health_bar = Combatant.HealthBar;
            //if (health_bar != null)
            //{
            //    health_bar.HideHealthBar(false);
            //}
            var health_bar = Combatant.HealthBar;
            if (health_bar != null)
            {
                health_bar.OnHandleMessage("HideHealthBar", false);
            }
        }

        public override void Update()
        {
            base.Update();

            if (!IsEnd)
            {
                int state_hash = Combatant.MoveController.GetCurrentAnimHash();
                AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

                if (state_hash == state_info.fullPathHash)
                {
                    UpdateCollide();
                }
            }
        }

        public override void End()
        {
            base.End();

            SwitchState(Combatant.GetActionState<CollideActionState>());

            //CombatEventReceiver.Instance.OnCombatEventTiming(ReactionEvent, eCombatEventTiming.COLLIDE_QUEUE);
        }

        protected override void UpdateLocation()
        {
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            float current_frame = Combatant.CurrentMove.NumFrames * state_info.normalizedTime;
            if (current_frame > EndFrame)
            {
                current_frame = EndFrame;
            }
            float anim_z_offset = Combatant.CurrentMove._zOffsetCurve.Evaluate(current_frame);
            if (Combatant.Index.TeamIndex == CombatLogic.Instance.DefenderTeamIndex)
            {
                anim_z_offset = -anim_z_offset;
            }
            float z = BackPoint.z + (anim_z_offset - (BackPoint.z - Combatant.OriginPosition.z)) * Scale;

            Vector3 current_position = Combatant.transform.position;
            current_position.z = z;
            Combatant.transform.position = current_position;
        }

        protected bool IsReachedEndPoint()
        {
            Vector3 current = CollidePoint - Combatant.transform.position;
            current.x = current.y = 0;
            Vector3 direction = CollidePoint - BackPoint;
            direction.x = direction.y = 0;
            if (Vector3.Dot(current, direction) < 0)
            {
                return true;
            }

            if (current.magnitude < distanceEpsilon)
            {
                return true;
            }

            return false;
        }

        protected List<CombatEventTree> CalculateCollideList()
        {
            //CombatEventTree node = CombatEventReceiver.Instance.FindEventTree(ReactionEvent);
            //return node.GetChildren(eCombatEventTiming.COLLIDE_QUEUE);
            return null;
        }

        protected void UpdateCollide()
        {
            List<CombatEventTree> collide_queue = CalculateCollideList();
            if (collide_queue == null)
            {
                return;
            }

            Vector3 move_direction = CollidePoint - BackPoint;

            int len = collide_queue.Count;
            for (int i = 0; i < len; i++)
            {
                CombatEventTree node = collide_queue[i];
                if (node.NodeState != CombatEventTreeState.INITED)
                {
                    continue;
                }

                if (node.Event.Type != eCombatEventType.EFFECT)
                {
                    continue;
                }

                CombatEffectEvent effect = node.Event as CombatEffectEvent;
                Combatant target = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(effect.Target);
                Vector3 direction_vector = target.transform.position - Combatant.transform.position;
                if (Vector3.Dot(direction_vector, move_direction) < 0.0f)
                {
                    //CombatEventReceiver.Instance.DoEventNode(node);
                }
            }
        }
    }
}