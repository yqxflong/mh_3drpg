using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class BackActionState : ReactionActionState
    {
        public static readonly string backMoveName = "Back";

        public float StartFrame
        {
            get;
            set;
        }

        public float EndFrame
        {
            get;
            set;
        }

        public float EndNormalizedTime
        {
            get;
            set;
        }

        public float BackFrame
        {
            get;
            set;
        }

        public float CollideFrame
        {
            get;
            set;
        }

        public float ReturnFrame
        {
            get;
            set;
        }

        public Vector3 BackPoint
        {
            get;
            set;
        }

        public Vector3 CollidePoint
        {
            get;
            set;
        }

        public Vector3 ReturnPoint
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }

        public BackActionState(Combatant combatant)
            : base(combatant)
        {
            MoveName = backMoveName;

            InitBackPoint();

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            Scale = 1.0f;

            StartFrame = 0.0f;
            NormalizedTime = 0.0f;

            EndFrame = move.NumFrames;
            EndNormalizedTime = 1.0f - timeEpsilon;
        }

        public BackActionState()
        {
        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveName = backMoveName;

            InitBackPoint();

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            Scale = 1.0f;

            StartFrame = 0.0f;
            NormalizedTime = 0.0f;

            EndFrame = move.NumFrames;
            EndNormalizedTime = 1.0f - timeEpsilon;
        }

        public override void CleanUp()
        {
            base.CleanUp();

            StartFrame = 0.0f;
            EndFrame = 0.0f;
            EndNormalizedTime = 0.0f;
            BackFrame = 0.0f;
            CollideFrame = 0.0f;
            ReturnFrame = 0.0f;
            BackPoint = Vector3.zero;
            CollidePoint = Vector3.zero;
            ReturnPoint = Vector3.zero;
            Scale = 0.0f;
        }

        protected void InitBackPoint()
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            BackFrame = Combatant.MoveController.GetFrameByCombatType(MoveEditor.CombatEventInfo.CombatEventType.BackStart, move);
            CollideFrame = Combatant.MoveController.GetFrameByCombatType(MoveEditor.CombatEventInfo.CombatEventType.BackCollideStart, move);
            ReturnFrame = Combatant.MoveController.GetFrameByCombatType(MoveEditor.CombatEventInfo.CombatEventType.BackReturnStart, move);

            if (CombatLogic.Instance.IsPlayerOrChallengerSide(Combatant.Index.TeamIndex))
            {
                float back_y = move._yOffsetCurve.Evaluate(BackFrame) + Combatant.OriginPosition.y;
                float back_z = move._zOffsetCurve.Evaluate(BackFrame) + Combatant.OriginPosition.z;
                BackPoint = new Vector3(0, back_y, back_z);

                float collide_y = move._yOffsetCurve.Evaluate(CollideFrame) + Combatant.OriginPosition.y;
                float collide_z = move._zOffsetCurve.Evaluate(CollideFrame) + Combatant.OriginPosition.z;
                CollidePoint = new Vector3(0, collide_y, collide_z);

                float return_y = move._yOffsetCurve.Evaluate(ReturnFrame) + Combatant.OriginPosition.y;
                float return_z = move._zOffsetCurve.Evaluate(ReturnFrame) + Combatant.OriginPosition.z;
                ReturnPoint = new Vector3(0, return_y, return_z);
            }
            else
            {
                float back_y = move._yOffsetCurve.Evaluate(BackFrame) + Combatant.OriginPosition.y;
                float back_z = -move._zOffsetCurve.Evaluate(BackFrame) + Combatant.OriginPosition.z;
                BackPoint = new Vector3(0, back_y, back_z);

                float collide_y = move._yOffsetCurve.Evaluate(CollideFrame) + Combatant.OriginPosition.y;
                float collide_z = -move._zOffsetCurve.Evaluate(CollideFrame) + Combatant.OriginPosition.z;
                CollidePoint = new Vector3(0, collide_y, collide_z);

                float return_y = move._yOffsetCurve.Evaluate(ReturnFrame) + Combatant.OriginPosition.y;
                float return_z = -move._zOffsetCurve.Evaluate(ReturnFrame) + Combatant.OriginPosition.z;
                ReturnPoint = new Vector3(0, return_y, return_z);
            }

            Transform back_wall_transform = Combatant.transform.parent.parent.Find("BackWall");
            if (back_wall_transform != null)
            {
                Vector3 collide_point = back_wall_transform.position;

                collide_point.x = 0;
                CollidePoint = collide_point;

                collide_point.y = 0;
                ReturnPoint = collide_point;
            }
        }

        public override void Update()
        {
            base.Update();

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int state_hash = Combatant.MoveController.GetCurrentAnimHash();
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (move_state != MoveController.CombatantMoveState.kHitReaction)
            {
                EB.Debug.LogError("invalid move state");
                Stop();
                return;
            }

            if (state_hash != state_info.fullPathHash)
            {
                return;
            }

            UpdateLocation();

            if (state_info.normalizedTime > EndNormalizedTime && NormalizedTime > EndNormalizedTime)
            {
                End();
                return;
            }

            NormalizedTime = state_info.normalizedTime;
        }

        protected virtual void UpdateLocation()
        {
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            float current_frame = Combatant.CurrentMove.NumFrames * state_info.normalizedTime;
            if (current_frame > EndFrame)
            {
                current_frame = EndFrame;
            }
            float y_offset = Combatant.CurrentMove._yOffsetCurve.Evaluate(current_frame);
            float z_offset = Combatant.CurrentMove._zOffsetCurve.Evaluate(current_frame);
            if (Combatant.Index.TeamIndex == CombatLogic.Instance.DefenderTeamIndex)
            {
                z_offset = -z_offset;
            }

            Vector3 current_position = Combatant.transform.position;
            current_position.z = Combatant.OriginPosition.z + z_offset;
            current_position.y = Combatant.OriginPosition.y + y_offset;
            Combatant.transform.position = current_position;
        }
    }
}