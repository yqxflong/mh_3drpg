using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class HitActionState : ReactionActionState
    {
        private static readonly string defaultHitMoveName = "HitReaction";

        public bool RotateToIdle
        {
            get;
            set;
        }

        public float EndNormalizedTime
        {
            get;
            set;
        }

        public bool UseDefaultMove
        {
            get;
            set;
        }

        //private float mFreezeTime = 0.0f;
        private bool mFreeze = false;  //定身 不能行動

        public HitActionState(Combatant combatant)
            : base(combatant)
        {
            MoveName = defaultHitMoveName;
            RotateToIdle = true;
            UseDefaultMove = true;
        }

        public HitActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            EndNormalizedTime = 1.0f - timeEpsilon;
            MoveName = defaultHitMoveName;
            RotateToIdle = true;
            UseDefaultMove = true;
        }

        public override void CleanUp()
        {
            base.CleanUp();

            RotateToIdle = false;
            EndNormalizedTime = 0.0f;
            UseDefaultMove = false;
            //mFreezeTime = 0.0f;
            mFreeze = false;
        }

        public override void Start()
        {
            base.Start();

            var specialBuffState = CombatSyncData.Instance.GetCurrentBuffState(Combatant.Data);
            mFreeze = specialBuffState == eSpecialBuffState.Frozen || Combatant.SpecialBuffState == eSpecialBuffState.Stone; //Combatant.Attributes.GetBoolAttr(CombatantAttributes.eAttribute.FreezeHitReaction);		

            if (!UseDefaultMove)
            {
                SetupMove(MoveName, AnimatorStateName, NormalizedTime);
            }
            else
            {
                SetupMoveUseDefault(MoveName, AnimatorStateName, NormalizedTime);
            }

            // stay in original position
            Combatant.transform.position = Combatant.OriginPosition;

            SetSpecialBuffAction(specialBuffState, Combatant.SpecialBuffState);
            Combatant.SpecialBuffState = specialBuffState;
        }

        public override void Update()
        {
            base.Update();

            if (mFreeze)
            {
                //if (mFreezeTime > 0)
                //{
                //    mFreezeTime -= Time.deltaTime;
                //    if (mFreezeTime <= 0)
                //    {
                //        End();
                //    }
                //}

                if (!Combatant.IsAlive())
                {
                    End();

                    //SwitchState(Combatant.GetActionState<DeathActionState>());
                    Combatant.CallDeath();
                }
                return;
            }

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
                // animator does not start blending
                return;
            }

            if (state_info.normalizedTime > EndNormalizedTime && NormalizedTime > EndNormalizedTime)
            {
                End();
                return;
            }

            NormalizedTime = state_info.normalizedTime;

            if (RotateToIdle)
            {
                TryRotateTowardToIdle();
            }
        }

        public override void Stop()
        {
            StopSpecialBuffAction();

            base.Stop();
        }

        public override void End()
        {
            StopSpecialBuffAction();

            base.End();

            TryReturnToOriginPosition();
        }

        //not use
        public override float CalculateLeftTime()
        {
            //if (Combatant.Attributes.GetBoolAttr(CombatantAttributes.eAttribute.FreezeHitReaction))
            //{
            //    return mFreezeTime;
            //}

            return base.CalculateLeftTime();
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