using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class FlowActionState : ReactionActionState
    {
        public static readonly string flowMoveName = "Flow";

        public enum eHeightType
        {
            None,
            LITTLE,
            LARGE
        }

        public eHeightType FlowHeight
        {
            get;
            set;
        }

        public Vector3 FlowUpPoint
        {
            get;
            set;
        }

        public Vector3 FlowTopPoint
        {
            get;
            set;
        }

        public Vector3 FlowLandPoint
        {
            get;
            set;
        }

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

        public float FlowUpFrame
        {
            get;
            set;
        }

        public float FlowTopFrame
        {
            get;
            set;
        }

        public float FlowLandFrame
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }

        public FlowActionState(Combatant combatant, eHeightType height)
            : base(combatant)
        {
            MoveName = flowMoveName;
            FlowHeight = height;

            //InitFlowPoint();

            //MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            //Scale = 1.0f;

            //StartFrame = 0.0f;
            //NormalizedTime = 0.0f;

            //EndFrame = move.NumFrames;
            //EndNormalizedTime = 1.0f - timeEpsilon;
        }

        public FlowActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveName = flowMoveName;
        }

        public override void CleanUp()
        {
            base.CleanUp();

            FlowHeight = eHeightType.LITTLE;
            FlowUpPoint = Vector3.zero;
            FlowTopPoint = Vector3.zero;
            FlowLandPoint = Vector3.zero;
            StartFrame = 0.0f;
            EndFrame = 0.0f;
            EndNormalizedTime = 0.0f;
            FlowUpFrame = 0.0f;
            FlowTopFrame = 0.0f;
            FlowLandFrame = 0.0f;
            Scale = 0.0f;
        }

        public FlowActionState SetHeight(eHeightType height)
        {
            FlowHeight = height;

            return this;
        }

        public override void Start()
        {
            base.Start();

            InitFlowPoint();

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            Scale = 1.0f;

            StartFrame = 0.0f;
            NormalizedTime = 0.0f;

            EndFrame = move.NumFrames;
            EndNormalizedTime = 1.0f - timeEpsilon;
        }

        protected void InitFlowPoint()
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            FlowUpFrame = Combatant.MoveController.GetFrameByCombatType(MoveEditor.CombatEventInfo.CombatEventType.FlowUpStart, move);
            FlowTopFrame = Combatant.MoveController.GetFrameByCombatType(MoveEditor.CombatEventInfo.CombatEventType.FlowDownStart, move);
            FlowLandFrame = Combatant.MoveController.GetFrameByCombatType(MoveEditor.CombatEventInfo.CombatEventType.FlowLandStart, move);

            float flowup_y = move._yOffsetCurve.Evaluate(FlowUpFrame) + Combatant.OriginPosition.y;
            FlowUpPoint = new Vector3(0, flowup_y, 0);

            float flowtop_y = move._yOffsetCurve.Evaluate(FlowTopFrame) + Combatant.OriginPosition.y;
            FlowTopPoint = new Vector3(0, flowtop_y, 0);

            Transform flow_height_transform = null;
            if (FlowHeight == eHeightType.LITTLE)
            {
                flow_height_transform = Combatant.transform.parent.parent.Find("LittleFlow");
            }
            else if (FlowHeight == eHeightType.LARGE)
            {
                flow_height_transform = Combatant.transform.parent.parent.Find("LargeFlow");
            }
            if (flow_height_transform != null)
            {
                Vector3 flow_point = flow_height_transform.position;
                flow_point.x = 0;
                flow_point.z = 0;

                FlowTopPoint = flow_point;
            }

            float flowland_y = move._yOffsetCurve.Evaluate(FlowLandFrame) + Combatant.OriginPosition.y;
            FlowLandPoint = new Vector3(0, flowland_y, 0);
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
                OnEnd();
                return;
            }

            NormalizedTime = state_info.normalizedTime;
            //EB.Debug.Log("FlowActionState.Update: NormalizedTime = {0}, EndNormalizedTime = {1}", NormalizedTime, EndNormalizedTime);
        }

        protected virtual void OnEnd()
        {
            End();
        }

        protected virtual void UpdateLocation()
        {
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            float current_frame = Combatant.CurrentMove.NumFrames * state_info.normalizedTime;
            if (current_frame > EndFrame)
            {
                current_frame = EndFrame;
            }
            float anim_y_offset = Combatant.CurrentMove._yOffsetCurve.Evaluate(current_frame);
            float anim_z_offset = Combatant.CurrentMove._zOffsetCurve.Evaluate(current_frame);
            if (Combatant.Index.TeamIndex == CombatLogic.Instance.DefenderTeamIndex)
            {
                anim_z_offset = -anim_z_offset;
            }
            float y = Combatant.OriginPosition.y + anim_y_offset * Scale;
            float z = Combatant.OriginPosition.z + anim_z_offset;

            Vector3 current_position = Combatant.transform.position;
            current_position.y = y;
            current_position.z = z;
            Combatant.transform.position = current_position;
        }

        public override float CalculateComboTime()
        {
            float normalized_time = NormalizedTime;
            float speed = Combatant.Animator.speed;

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int current_state_hash = Combatant.MoveController.GetCurrentAnimHash();
            int this_state_hash = AnimatorStateNameHash;
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (this_state_hash != current_state_hash)
            {// just new
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (move_state != MoveController.CombatantMoveState.kHitReaction)
            {// leaving
                normalized_time = 1.0f;
            }
            else if (current_state_hash != state_info.fullPathHash)
            {// blending
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (NormalizedTime > 0.0f && state_info.normalizedTime >= NormalizedTime)
            {// not repeat
                normalized_time = state_info.normalizedTime;
            }

            float current_frame = normalized_time * move.NumFrames;
            if (current_frame >= FlowLandFrame)
            {
                return 0.0f;
            }

            float combo_time = (FlowLandFrame - current_frame) / move.NumFrames * move._animationClip.length;
            return combo_time / speed;
        }
    }
}