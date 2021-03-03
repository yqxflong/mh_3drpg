using UnityEngine;
using System.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;
namespace Hotfix_LT.Combat
{
    public class CombatActionState
    {
        protected enum eActionStateStatus
        {
            Create,
            Blending,
            Updating,
            Ended
        }

        private Combatant m_combatant = null;

        //private MoveController.CombatantMoveState m_state = MoveController.CombatantMoveState.kIdle;
        //private eActionStateStatus m_status = eActionStateStatus.Create;

        // world space
        private Quaternion m_startRotation = Quaternion.identity;
        private Vector3 m_startPosition = Vector3.zero;
        private float m_startTime = 0.0f;

        // smooth rotate, local space
        private Quaternion m_fromRotation = Quaternion.identity;
        private Quaternion m_toRotation = Quaternion.identity;
        private float m_rotationLerpTime = 0.0f;
        private float m_rotationMaxTime = 0.0f;

        // constants
        public static readonly float distanceEpsilon = 0.1f;
        public static readonly float timeEpsilon = 0.01f;
        public static readonly int defaultLayer = 0;
        public static readonly float defaultBlendTime = 0.0f;

        public Combatant Combatant
        {
            get { return m_combatant; }
            private set { m_combatant = value; }
        }

        public Vector3 StartPosition
        {
            get { return m_startPosition; }
            protected set { m_startPosition = value; }
        }

        public Quaternion StartRotation
        {
            get { return m_startRotation; }
            private set { m_startRotation = value; }
        }

        public float StartTime
        {
            get { return m_startTime; }
            private set { m_startTime = value; }
        }

        public float StateTime
        {
            get { return Time.time - StartTime; }
        }

        public bool IsEnd
        {
            get;
            protected set;
        }

        public MoveController.CombatantMoveState MoveState
        {
            get;
            protected set;
        }

        protected eActionStateStatus ActionStatus
        {
            get;
            set;
        }

        public float NormalizedTime
        {
            get;
            set;
        }

        public CombatActionState(Combatant combatant)
        {
            Combatant = combatant;

            StartPosition = Combatant.transform.position;
            StartRotation = Combatant.transform.rotation;
            StartTime = Time.time;
        }

        public CombatActionState()
        {

        }

        public virtual void Init(Combatant combatant)
        {
            Combatant = combatant;

            StartPosition = Combatant.transform.position;
            StartRotation = Combatant.transform.rotation;
            StartTime = Time.time;
        }

        public virtual void CleanUp()
        {
            Combatant = null;

            StartPosition = Vector3.zero;
            StartRotation = Quaternion.identity;
            StartTime = 0f;

            IsEnd = false;
            MoveState = MoveController.CombatantMoveState.kIdle;
            ActionStatus = eActionStateStatus.Create;
            NormalizedTime = 0.0f;

            m_fromRotation = Quaternion.identity;
            m_toRotation = Quaternion.identity;
            m_rotationLerpTime = 0.0f;
            m_rotationMaxTime = 0.0f;
        }

        public void SwitchState(CombatActionState next_state)
        {
            try
            {
                if (!IsEnd)
                {
                    Stop();
                }

                if (Combatant.ActionState == next_state)
                {
                    EB.Debug.LogWarning("{0}{1}","CombatActionState.SwitchState: switch same action ? ",next_state.GetType());
                }
                Combatant.ActionState = next_state;
                if (Combatant.ActionState != null)
                {
                    Combatant.ActionState.Start();
                }
                else
                {
                EB.Debug.LogError("SwitchState next_state is null");
                }
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
        }

        public void TryReturnToOriginPosition()
        {
            Vector3 current_position = Combatant.transform.position;
            // ignore x axis
            current_position.x = Combatant.OriginPosition.x;

            Vector3 distance_vector = Combatant.OriginPosition - current_position;

            // check distance
            if (distance_vector.magnitude < distanceEpsilon)
            {// returned
                Combatant.ResetPosition();
                SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(false));
                return;
            }

            // y axis
            if (current_position.y < Combatant.OriginPosition.y)
            {// underground
                current_position.y = Combatant.OriginPosition.y;
                Combatant.transform.position = current_position;
                SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(false));
                return;
            }
            else if (current_position.y - Combatant.OriginPosition.y > distanceEpsilon)
            {// overground
                EB.Debug.LogWarning("TryReturnToOriginPosition: to land action, {0} to {1}, delta = {2} > {3}",
                    GetType(), typeof(LandActionState), (current_position.y - Combatant.OriginPosition.y), distanceEpsilon);
                SwitchState(Combatant.GetActionState<LandActionState>().SetHeight(FlowActionState.eHeightType.None));
                return;
            }
            else
            {// fix y axis
                current_position.y = Combatant.OriginPosition.y;
                Combatant.transform.position = current_position;

                distance_vector = Combatant.OriginPosition - current_position;
            }

            // check distance
            if (distance_vector.magnitude < distanceEpsilon)
            {// returned
                Combatant.ResetPosition();
                SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(false));
                return;
            }

            // z axis
            if (Combatant.transform.forward.z * distance_vector.z < 0)
            {// opposite direction
                EB.Debug.LogWarning("TryReturnToOriginPosition: to backward action, {0} to {1}, delta = {2} > {3}",
                    GetType(), typeof(BackwardActionState).ToString(), distance_vector.magnitude, distanceEpsilon.ToString());
                BackwardActionState bs = Combatant.GetActionState<BackwardActionState>();
                bs.DestnationLocation = Combatant.OriginPosition;
                SwitchState(bs);
            }
            else
            {// same direction
                EB.Debug.LogWarning("TryReturnToOriginPosition: to forward action, {0} to {1}, delta = {2} > {3}",
                    GetType(), typeof(ForwardActionState), distance_vector.magnitude.ToString(), distanceEpsilon.ToString());
                ForwardActionState fs = Combatant.GetActionState<ForwardActionState>();
                fs.DestnationLocation = Combatant.OriginPosition;
                SwitchState(fs);
            }
        }

        public void StartRotate(Quaternion from, Quaternion to, float max_time)
        {
            m_fromRotation = from;
            m_toRotation = to;
            m_rotationMaxTime = max_time;
            m_rotationLerpTime = 0.0f;
        }

        public void EndRotate()
        {
            m_rotationMaxTime = 0.0f;
            m_rotationLerpTime = 0.0f;
            m_fromRotation = Quaternion.identity;
            m_toRotation = Quaternion.identity;
        }

        public void UpdateRotation()
        {
            if (!IsRotating())
            {
                return;
            }

            if (m_rotationLerpTime >= m_rotationMaxTime)
            {// rotate end
                EndRotate();
                return;
            }

            mJobHandleUpdateRotation.Complete();

            m_rotationLerpTime = Mathf.Min(m_rotationLerpTime + Time.deltaTime * Combatant.Animator.speed, m_rotationMaxTime);
            float lerp_time = m_rotationMaxTime > 0.0f ? m_rotationLerpTime / m_rotationMaxTime : 1.0f;
            if (Combatant.myRotationSCurve != null)
            {
                lerp_time = Combatant.myRotationSCurve.Evaluate(lerp_time);
            }
            mJobUpdateRotation = new JobUpdateRotation()
            {
                lerp_time = lerp_time,
                m_fromRotation = m_fromRotation,
                m_toRotation = m_toRotation,
            };

            Transform[] transforms = new[] { Combatant.transform };
            TransformAccessArray accessArray = new TransformAccessArray(transforms);
            mJobHandleUpdateRotation = mJobUpdateRotation.Schedule(accessArray);

            JobHandle.ScheduleBatchedJobs();
            accessArray.Dispose();
            //lerp_time = Mathf.Clamp(lerp_time, 0.0f, 1.0f);
            //Quaternion local_rotation = Quaternion.Slerp(m_fromRotation, m_toRotation, lerp_time);
            //if (!float.IsNaN(local_rotation.x))
            //{
            //    Combatant.gameObject.transform.localRotation = local_rotation;
            //}
        }

        JobHandle mJobHandleUpdateRotation;
        JobUpdateRotation mJobUpdateRotation;

        public bool IsRotating()
        {
            return m_rotationMaxTime > 0.0f;
        }

        public virtual float CalculateLeftTime()
        {
            return 0.0f;
        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {
            UpdateRotation();
        }

        public virtual void End()
        {
            IsEnd = true;

            // no default transition
        }

        public virtual void Stop()
        {
            IsEnd = true;

            Combatant.ResetRotation();
            Combatant.ResetPosition();
            SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(false));
        }

        public virtual void TransitionToBlock(ReactionEffectEvent reaction_event)
        {
            // force switch
            SwitchState(Combatant.GetActionState<BlockActionState>());
        }

        public virtual void TransitionToHit(ReactionEffectEvent reaction_event)
        {
            // force switch
            SwitchState(Combatant.GetActionState<HitActionState>());
        }

        public virtual void TransitionToBack(ReactionEffectEvent reaction_event)
        {
            // force switch
            SwitchState(Combatant.GetActionState<BackStartActionState>());
        }

        public virtual void TransitionToDown(ReactionEffectEvent reaction_event)
        {
            // force switch
            SwitchState(Combatant.GetActionState<FallDownActionState>());
        }

        public virtual void TransitionToLittleFlow(ReactionEffectEvent reaction_event)
        {
            // force switch
            FlowStartActionState fs = Combatant.GetActionState<FlowStartActionState>().SetHeight(FlowActionState.eHeightType.LITTLE) as FlowStartActionState;
            SwitchState(fs);
        }

        public virtual void TransitionToLargeFlow(ReactionEffectEvent reaction_event)
        {
            // force switch
            FlowStartActionState fs = Combatant.GetActionState<FlowStartActionState>().SetHeight(FlowActionState.eHeightType.LARGE) as FlowStartActionState;
            SwitchState(fs);
        }

        #region callback
        //public virtual void OnCurrentMotionStateUpdated()
        //{

        //}

        //public virtual void OnAnimatorMove()
        //{

        //}
        #endregion
    }
}