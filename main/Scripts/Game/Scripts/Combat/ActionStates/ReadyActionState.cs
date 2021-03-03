using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ReadyActionState : CombatActionState
    {
        public bool ManualCrossFade
        {
            get;
            set;
        }

        public ReadyActionState(Combatant combatant, bool auto_cross_fade)
            : base(combatant)
        {
            ManualCrossFade = !auto_cross_fade;
            MoveState = MoveController.CombatantMoveState.kReady;
        }

        public ReadyActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveState = MoveController.CombatantMoveState.kReady;
        }

        public override void CleanUp()
        {
            ManualCrossFade = false;

            base.CleanUp();
        }

        public ReadyActionState SetAutoCrossFade(bool auto_cross_fade)
        {
            ManualCrossFade = !auto_cross_fade;
            return this;
        }

        public override void Start()
        {
            if (Combatant == null)
            {
                EB.Debug.LogError("Hotfix_LT.Combat.ReadyActionState.Start() -> Combatant is null");
                return;
            }

            Combatant.ResetRotation();

            try
            {
                if (!Combatant.IsAlive())
                {
                    if (Combatant.DeathOver)
                    {
                        Combatant.CallDeath();
                        return;
                    }
                    else
                    {
                        if (Combatant.Data != null)
                        {
                            Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnPlayerDeathAnimEnd(Combatant.Data.IngameId);
                        }

                        Combatant.DeathOver = true;
                    }
                } 

                if (Combatant.MoveController != null)
                {
                    Combatant.MoveController.TransitionTo(MoveState);

                    if (ManualCrossFade)
                    {
                        float blend_time = 0.00f;// 0.15f * Combatant.MoveController.GetCurrentStateInfo().length;
                        Combatant.MoveController.CrossFade(MoveController.m_ready_hash, blend_time, defaultLayer, NormalizedTime);
                    }
                }

                if (Combatant.NeedLaunch && Vector3.Distance(Combatant.OriginPosition, Combatant.transform.position) < distanceEpsilon)
                {
                    SwitchState(Combatant.GetActionState<LaunchActionState>());
                }
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
        }

        public override void Update()
        {
            base.Update();

        if (!Combatant.IsAlive())
        {
            if (Combatant.DeathOver)
            {
                End();
                Combatant.CallDeath();
                return;
            }
            else
            {
                Combatant.DeathOver = true;
            }
        }

            //HealthBar2D health_bar = Combatant.HealthBar;
            //if (health_bar != null && health_bar.Hidden && StateTime > 0.1f)
            //{
            //    health_bar.ShowHealthBar();
            //}
            var health_bar = Combatant.HealthBar;
            if (health_bar != null && StateTime > 0.1f)
            {
                health_bar.OnHandleMessage("ShowHealthBar", null);
            }
        }

        public override void End()
        {

        }

        public override void Stop()
        {
            End();
        }

        public override float CalculateLeftTime()
        {
            return -1.0f;
        }

        public override void TransitionToBlock(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<BlockActionState>());
        }

        public override void TransitionToHit(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<HitActionState>());
        }

        public override void TransitionToBack(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<BackStartActionState>());
        }

        public override void TransitionToDown(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<FallDownActionState>());
        }

        public override void TransitionToLittleFlow(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            FlowStartActionState fs = Combatant.GetActionState<FlowStartActionState>().SetHeight(FlowActionState.eHeightType.LITTLE) as FlowStartActionState;
            SwitchState(fs);
        }

        public override void TransitionToLargeFlow(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            FlowStartActionState fs = Combatant.GetActionState<FlowStartActionState>().SetHeight(FlowActionState.eHeightType.LARGE) as FlowStartActionState;
            SwitchState(fs);
        }

        eSpecialBuffState m_buffActionState;
        public void SetSpecialBuffAction(eSpecialBuffState currentBuffState, eSpecialBuffState previousBuffState)
        {
            m_buffActionState = currentBuffState;
            if (currentBuffState != previousBuffState)
            {
                EB.Debug.Log("set buffState:{0} cbt:{1}", currentBuffState, Combatant.Data);
                SkinnedMeshRenderer[] skins = Combatant.GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int i = 0; i < skins.Length; i++)
                {
                    SkinnedMeshRenderer skin = skins[i];
                    skin.material.SetColor("_FinalColor", new Color(1.0f, 1.0f, 1.0f, 0.0f));
                    skin.material.SetFloat("_ContrastIntansity", 1);
                    skin.material.SetFloat("_Brightness", 0);
                    skin.material.SetFloat("_GrayScale", 0);
                }
                Combatant.RestoreMaterialShader();

                Combatant.PauseAnimation(-1.0f);
            }

            if (currentBuffState == eSpecialBuffState.Frozen)
            {
                SkinnedMeshRenderer[] skins = Combatant.GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int i = 0; i < skins.Length; i++)
                {
                    SkinnedMeshRenderer skin = skins[i];
                    skin.material.SetColor("_FinalColor", new Color(1.0f, 0.965f, 0.0f, 0.408f));
                    skin.material.SetFloat("_ContrastIntansity", 1);
                    skin.material.SetFloat("_Brightness", 0);
                    skin.material.SetFloat("_GrayScale", 0);
                }

                //mFreezeTime = Combatant.MoveController.GetMove(MoveName, UseDefaultMove).AdjustedLength;
                Combatant.PauseAnimation(0.0f);
            }
            else if (currentBuffState == eSpecialBuffState.Stone)
            {
                Combatant.ReplaceAsStoneMaterialShader();
                Combatant.PauseAnimation(0.0f);
            }
        }

        public void StopSpecialBuffAction()
        {
            if (m_buffActionState == eSpecialBuffState.None)
                return;

            m_buffActionState = eSpecialBuffState.None;
            SkinnedMeshRenderer[] skins = Combatant.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skins.Length; i++)
            {
                SkinnedMeshRenderer skin = skins[i];
                skin.material.SetColor("_FinalColor", new Color(1.0f, 1.0f, 1.0f, 0.0f));
                skin.material.SetFloat("_ContrastIntansity", 1);
                skin.material.SetFloat("_Brightness", 0);
                skin.material.SetFloat("_GrayScale", 0);
            }
            Combatant.RestoreMaterialShader();

            Combatant.PauseAnimation(-1.0f);
        }
    }
}