using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _HotfixScripts.Utils;

namespace Hotfix_LT.Instance
{
    public class Instance3DLobby : DynamicMonoHotfix,IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            mCurrent = t.GetMonoILRComponent<UI.UIBuddy3DModelCreater>("CharacterContainer");
        }

        public override void OnEnable()
        {
            RegisterMonoUpdater();
            InitCharacter();
        }
        
        public void Update()
        {
            if (needToTransitionToIdle || needToSetStateAfterMove != MoveController.CombatantMoveState.kIdle)
            {
                if (mCurrent != null && mCurrent.character != null)
                {
                    MoveController moveController = mCurrent.character.GetComponent<MoveController>();
                    AnimatorStateInfo asi = moveController.GetCurrentStateInfo();
                    if (asi.normalizedTime >= 1)
                    {
                        MoveEditor.Move theMove = moveController.GetMoveByState(needToSetStateAfterMove);
                        moveController.TransitionTo(needToSetStateAfterMove);
                        moveController.m_lobby_hash = Animator.StringToHash(string.Format("Lobby.{0}", theMove.name));
                        moveController.SetMove(theMove);
                        moveController.CrossFade(moveController.GetCurrentAnimHash(), 0.2f, 0, 0f);
                        needToTransitionToIdle = false;
                        needToSetStateAfterMove = MoveController.CombatantMoveState.kIdle;
                    }
                }
            }
        }
        
        public override void OnDisable()
        {
            ErasureMonoUpdater();
            ReleaseCharacter();
        }

        protected string mVariantName = null;
        protected UI.UIBuddy3DModelCreater mCurrent = null;
        public UI.UIBuddy3DModelCreater Current
        {
            get
            {
                return mCurrent;
            }
        }
        public string VariantName
        {
            get { return mVariantName; }
            set { mVariantName = value; CreateCharacter(); }
        }
        
        protected void CreateCharacter()
        {
            if (string.IsNullOrEmpty(mVariantName))
            {
                return;
            }
            
            mCurrent.ShowCharacter(true);
            string targetVariantName = string.Format("{0}-I", mVariantName);
            // create
            mCurrent.CharacterVariantTemplate = targetVariantName;
            mCurrent.CreatCharacter(null);
        }

        protected void InitCharacter()
        {
            if (string.IsNullOrEmpty(mVariantName))
            {
                return;
            }

            string targetVariantName = string.Format("{0}-I", mVariantName);
            if (mCurrent != null && !string.IsNullOrEmpty(mCurrent.CharacterVariantTemplate) && mCurrent.CharacterVariantTemplate.Equals(targetVariantName))
            {
                mCurrent.ShowCharacter(true);
            }
        }

        protected void ReleaseCharacter(bool isDestroyCharacter = false)
        {
            if (mCurrent != null)
            {
                mCurrent.ShowCharacter(false);
                if (isDestroyCharacter)
                {
                    mCurrent.DestroyCharacter();
                }
            }
        }

        protected bool needToTransitionToIdle = false;
        protected MoveController.CombatantMoveState needToSetStateAfterMove = MoveController.CombatantMoveState.kIdle;
        protected int tempStateHash = 0;

        public void SetCharMoveState(MoveController.CombatantMoveState moveState, bool needToTransitionToIdle = false, MoveController.CombatantMoveState needToSetStateAfterMove = MoveController.CombatantMoveState.kIdle)
        {
            if (mCurrent != null && mCurrent.character != null)
            {
                MoveController moveController = mCurrent.character.GetComponent<MoveController>();
                System.Action fn = () => {
                    if (moveController.CurrentState == MoveController.CombatantMoveState.kEntry && moveState == MoveController.CombatantMoveState.kEntry)
                    {
                        moveController.CurrentState = MoveController.CombatantMoveState.kIdle;
                    }
                    moveController.TransitionTo(moveState);
                    //激活状态情况才能启动
                    if (mDMono.gameObject.activeInHierarchy)
                    {
                        StartCoroutine(WaitToSetState(needToTransitionToIdle, needToSetStateAfterMove));
                    }
                    else
                    {
                        EB.Debug.LogWarning("UI3DLobby SetCharMoveState this.gameObject.activeSelf = false!!");
                    }
                };

                if (!moveController.IsInitialized)
                {
                    moveController.RegisterInitSuccCallBack(fn);
                }
                else
                {
                    fn();
                }
            }
        }

        private IEnumerator WaitToSetState(bool needToTransitionToIdle = false, MoveController.CombatantMoveState needToSetStateAfterMove = MoveController.CombatantMoveState.kIdle)
        {
            yield return null;
            //等一帧在处理，不然状态动画的状态切换会切换不成功
            this.needToTransitionToIdle = needToTransitionToIdle;
            this.needToSetStateAfterMove = needToSetStateAfterMove;
        }

        public void SetCharRotation(Quaternion rotation)
        {
            if (mCurrent != null)
            {
                mCurrent.mDMono.transform.localRotation = rotation;
            }
        }
        public void SetCharRotation(Vector3 rotation)
        {
            if (mCurrent != null)
            {
                mCurrent.mDMono.transform.localRotation = Quaternion.Euler(rotation);
            }
        }
    }
}