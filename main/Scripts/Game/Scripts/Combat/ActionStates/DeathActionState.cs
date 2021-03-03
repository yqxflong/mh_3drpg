using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class DeathActionState : CombatActionState
    {
        private bool Disappeared
        {
            get;
            set;
        }

        public bool Revive
        {
            get;
            set;
        }

        public DeathActionState(Combatant combatant)
            : base(combatant)
        {
            MoveState = MoveController.CombatantMoveState.kDeath;
        }

        public DeathActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveState = MoveController.CombatantMoveState.kDeath;
        }

        public override void CleanUp()
        {
            Disappeared = false;
            Revive = false;

            base.CleanUp();
        }

        public static Vector3 HideCharacterPos = new Vector2(10000, 10000);

        public override void Start()
        {
            if (Combatant.IsLaunch())
            {
                Combatant.CleanLaunch();
            }

            float blend_time = 0.00f;// 0.15f * Combatant.MoveController.GetCurrentStateInfo().length;
            Combatant.MoveController.TransitionTo(MoveState);
            Combatant.MoveController.CrossFade(MoveController.m_death_hash, blend_time, defaultLayer, NormalizedTime);

            //HealthBar2D health_bar = Combatant.HealthBar;
            //if (health_bar != null && health_bar.Hidden)
            //{
            //    health_bar.ShowHealthBar();
            //}
            var health_bar = Combatant.HealthBar;
            if (health_bar != null)
            {
                health_bar.OnHandleMessage("ShowHealthBar", null);
            }

            RemoveLoopImpactFX();

            Hotfix_LT.UI.LTCombatEventReceiver.Instance.SetDeath(Combatant, true);
        }

        public override void Update()
        {
            base.Update();

            int state_hash = Combatant.MoveController.GetCurrentAnimHash();
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            if (state_hash != state_info.fullPathHash)
            {// animation not ready
                return;
            }

            NormalizedTime = state_info.normalizedTime;

            if (Disappeared)
            {
                return;
            }

            bool death_anim_end = NormalizedTime > 1.0f - timeEpsilon;
            if (death_anim_end && !Combatant.DeathOver)
            {
                Combatant.DeathOver = true;
                Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnPlayerDeathAnimEnd(Combatant.Data.IngameId);
            }

            if (Combatant == null) return;
            if (death_anim_end && Combatant.ExileEvent != null)
            {
                Combatant.EndExile();
                Disappeared = true;

                // hide character
                Combatant.transform.localPosition = HideCharacterPos;
                return;
            }

            if (Combatant.ReviveEvent != null && death_anim_end)
            {
                //RestoreLoopImpactFX();
                //Combatant.EndRevive();

                IsEnd = true;
                SwitchState(Combatant.GetActionState<ReviveActionState>());
                return;
            }

            if (Combatant.IsAlive() && death_anim_end)
            {
                if (!Revive)
                    SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(true));
                else
                {
                    Revive = false;
                    SwitchState(Combatant.GetActionState<ReviveActionState>());
                }
                return;
            }

            if (!Combatant.IsAlive() && death_anim_end)
            {
                if (Revive)
                {
                    Revive = false;
                    Combatant.ResetPosition();
                    Combatant.SetHP(Combatant.Data.Hp);

                    //Combatant.HealthBar.Hp = Combatant.Data.Hp;
                    Combatant.HealthBar.OnHandleMessage("SetHp", Combatant.Data.Hp);

                    SwitchState(Combatant.GetActionState<ReviveActionState>());
                }
                else
                {
                    // hide character
                    Combatant.transform.localPosition = HideCharacterPos;

                    //if (LTCombatHudController.Instance != null)
                    //	LTCombatHudController.Instance.CombatSkillCtrl.ClearConvergeInfo();
                }
            }

            //这里要移除主要因为又buff的情况下重连回来会显示一个buff在中间
            if (!Combatant.IsAlive() && !Revive)
            {
                //if (Combatant.HealthBar != null && Combatant.HealthBar.HealthBar != null)
                //{
                //    Combatant.HealthBar.HealthBar.transform.localPosition = HideCharacterPos;
                //}
                Combatant.HealthBar.OnHandleMessage("SetPosition", HideCharacterPos);
            }
        }

        public override void End()
        {
            SkinnedMeshRenderer[] skins = Combatant.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skins.Length; i++)
            {
                SkinnedMeshRenderer skin = skins[i];
                skin.material.SetColor("_FinalColor", new Color(1.0f, 1.0f, 1.0f, 0.0f));
                skin.material.SetFloat("_ContrastIntansity", 1);
                skin.material.SetFloat("_Brightness", 0);
                skin.material.SetFloat("_GrayScale", 0);
            }
            
            if (Combatant.ReviveEvent != null)
            {
                RestoreLoopImpactFX();
                Combatant.EndRevive();
                return;
            }

            if (Combatant.ExileEvent != null)
            {
                Combatant.EndExile();
                Disappeared = true;

                Combatant.FXHelper.StopAll();
                // hide character
                Combatant.transform.localPosition = HideCharacterPos;
                return;
            }
        }

        public override void Stop()
        {
            End();
        }

        public override float CalculateLeftTime()
        {
            return -1.0f;
        }

        private void RemoveLoopImpactFX()
        {
            Combatant.RemoveLoopImpactFX();
        }

        private void RestoreLoopImpactFX()
        {
            Combatant.RestoreLoopImpactFX();
        }

        //public bool Death_anim_end()
        //{
        //	return NormalizedTime > 1.0f - timeEpsilon;
        //}
    }
}