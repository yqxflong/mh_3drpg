using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class SkillActionState : CombatActionState
    {
        public string MoveName
        {
            get;
            set;
        }

        public int AnimatorStateNameHash
        {
            get { return Animator.StringToHash("Base Layer.Moves." + MoveName); }
        }

        public SkillActionState(Combatant combatant)
            : base(combatant)
        {

        }

        public SkillActionState()
        {

        }

        public override void CleanUp()
        {
            MoveName = string.Empty;

            base.CleanUp();
        }

        private bool skipmove = false;
        public override void Start()
        {
            if (MoveName == "")
            {
                MoveName = "SKILL_normalattack";
                skipmove = true;
            }
            else
            {
                skipmove = false;
            }

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            // set state and default move and auto cross to default animation
            Combatant.MoveController.TransitionTo(MoveController.CombatantMoveState.kAttackTarget);
            // set real move
            Combatant.MoveController.SetMove(move);
            // force cross fade to real animator state
            Combatant.MoveController.m_attack_hash = AnimatorStateNameHash;
            Combatant.MoveController.CrossFade(Combatant.MoveController.m_attack_hash, 0.0f, 0, NormalizedTime);

            //HealthBar2D health_bar = Combatant.HealthBar;
            //if (health_bar != null)
            //{
            //    //health_bar.HideHealthBar(true); //by pj 关闭技能隐藏血条的功能
            //}
            var health_bar = Combatant.HealthBar;
            if (health_bar != null)
            {
                //health_bar.OnHandleMessage("HideHealthBar", true);  //by pj 关闭技能隐藏血条的功能
            }
        }

        public override void Update()
        {
            base.Update();

            if (skipmove)
            {
                Stop();
                return;
            }

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int state_hash = Combatant.MoveController.GetCurrentAnimHash();
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (move_state != MoveController.CombatantMoveState.kAttackTarget)
            {
                EB.Debug.LogError("move state not match");
                Stop();
                return;
            }

            if (state_hash != state_info.fullPathHash)
            {
                EB.Debug.Log("SkillActionState.Update: animator state not ready, {0} != {1}", state_hash.ToString(), state_info.fullPathHash.ToString());
                return; //but we do not need to stay here
            }

            float end_time = 1.0f - timeEpsilon;
            if (state_info.normalizedTime > end_time && NormalizedTime > end_time)
            {
                End();
                return;
            }

            NormalizedTime = state_info.normalizedTime;
        }

        public override void End()
        {
            base.End();

            SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(false));
        }

        public override void Stop()
        {
            End();

            Combatant.transform.position = StartPosition;
        }

        public override float CalculateLeftTime()
        {
            float normalized_time = NormalizedTime;
            float speed = Combatant.Animator.speed;

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int current_state_hash = Combatant.MoveController.GetCurrentAnimHash();
            int this_state_hash = AnimatorStateNameHash;
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (this_state_hash != current_state_hash)
            {
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (move_state != MoveController.CombatantMoveState.kAttackTarget)
            {
                normalized_time = 1.0f;
            }
            else if (current_state_hash != state_info.fullPathHash)
            {
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (NormalizedTime > 0.0f && state_info.normalizedTime >= NormalizedTime)
            {
                normalized_time = state_info.normalizedTime;
            }

            float left_time = move._animationClip.length * (1.0f - normalized_time);
            return left_time / speed;
        }

        public void RotateToward(Combatant target)
        {
            Vector3 direction_vector = target.transform.position - Combatant.transform.position;
            Quaternion target_direction = Quaternion.LookRotation(direction_vector, Vector3.up);
            Quaternion local_target_rotation = target_direction * Combatant.transform.parent.rotation;

            // rotate quickly
            StartRotate(Combatant.transform.localRotation, local_target_rotation, 0.05f);
        }
    }
}