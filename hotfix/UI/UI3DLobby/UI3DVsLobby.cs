using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class UI3DVsLobby : UI3DLobby, IHotfixUpdate
    {
        public override void Awake()
        {
            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    shader = (Shader)mDMono.ObjectParamList[0];
                }

                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    outlineShader = (Shader)mDMono.ObjectParamList[1];
                }
            }

            var t = mDMono.transform;
            heroCamera = t.GetComponent<Camera>("UI3DCameraHero");
            heroRTGO = t.FindEx("UI3DCameraHero/HeroGO").gameObject;
            otherCharaacterCreator = t.GetMonoILRComponent<UIBuddy3DModelCreater>("UI3DCamera/UI3DScene/OtherCharacterContainer");
            selfTurnGO = t.FindEx("UI3DCameraOut/Quad").gameObject;
            otherTurnGO = t.FindEx("UI3DCameraOut/Quad (1)").gameObject;
            HeroPostEffectGO = t.FindEx("UI3DCameraPostEffect/QuadHeroPostEffect").gameObject;

            canChangeModel = true;
            canChangeOtherModel = true;
            characterCreator = t.GetMonoILRComponent<UIBuddy3DModelCreater>("UI3DCamera/UI3DScene/CharacterContainer");
            lobbyCamera = t.GetComponent<Camera>("UI3DCamera");
            renderSettings = t.GetComponent<RenderSettings>("UI3DRenderSetting");

            smoothSpeed = 15f;
            useBloom = false;

            outlineCmdBuffer = lobbyCamera.gameObject.AddComponent<OutlinePostEffectCmdBuffer>();
            selfEmptyGameObject = new GameObject();
            selfEmptyGameObject.transform.parent = t;

            outlineCmdBuffer.outLineColor = new Color(42f / 255, 146f / 255, 1);
            OutlineParams(outlineCmdBuffer);
        }

        public Camera heroCamera;
        public GameObject heroRTGO;

        private RenderTexture mHeroRenderTexture = null;

        public UIBuddy3DModelCreater otherCharaacterCreator;

        protected List<UIBuddy3DModelCreater> mOtherCharacterPool = new List<UIBuddy3DModelCreater>();

        protected UIBuddy3DModelCreater mCurrentOther = null;

        public Shader shader;
        public Shader outlineShader;

        private Coroutine _cWaitAvaterRefresh;
        private Coroutine _cWaitOtherAvatarRefresh;

        public OutlinePostEffectCmdBuffer outlineCmdBuffer;

        public GameObject selfTurnGO;
        public GameObject otherTurnGO;
        public GameObject HeroPostEffectGO;
        public MoveController.CombatantMoveState selfMoveState = MoveController.CombatantMoveState.kEntry;
        public MoveController.CombatantMoveState otherMoveState = MoveController.CombatantMoveState.kReady;

        private string mOtherVariantName = null;

        //为了保证后续的渲染脚本有对象 outlineCmdBuffer  先后如果有空会影响后续 otherOutlineCmdBuffer 的渲染
        private GameObject selfEmptyGameObject;

        private bool misMyTurn = false;

        public bool canChangeModel = true;
        public bool canChangeOtherModel = true;

        public string OtherVariantName
        {
            get { return mOtherVariantName; }
            set
            {
                if (!canChangeOtherModel)
                    return;

                if (otherCharaacterCreator != null && otherCharaacterCreator.loading)//如果在加载不能改变
                {
                    return;
                }

                if (mOtherVariantName == value) //如果已经加载的是这个
                {
                    return;
                }

                outlineCmdBuffer.outLineColor = Color.red;
                mOtherVariantName = value;
                CreateCharacter(mOtherVariantName, otherCharaacterCreator, null);

                if (_cWaitOtherAvatarRefresh != null)
                {
                    StopCoroutine(_cWaitOtherAvatarRefresh);
                }

                if(mDMono.gameObject.activeSelf)_cWaitOtherAvatarRefresh = StartCoroutine(WaitOtherAvatarRefresh());
            }
        }

        protected void CreateCharacter(string variantName, UIBuddy3DModelCreater old, IDictionary variantPartitions)
        {
            if (string.IsNullOrEmpty(variantName))
            {
                old.mDMono.gameObject.CustomSetActive(false);
                return;
            }

            // create pool
            if (mOtherCharacterPool.Count != mCharacterPoolSize)
            {
                otherCharaacterCreator.ShowCharacter(false);

                GameObject characterCreatorGO = otherCharaacterCreator.mDMono.gameObject;
                Transform parentTransform = otherCharaacterCreator.mDMono.transform.parent;
                for (int i = mOtherCharacterPool.Count; i < mCharacterPoolSize; ++i)
                {
                    GameObject creatorObj = Object.Instantiate(characterCreatorGO, parentTransform) as GameObject;
                    UIBuddy3DModelCreater creator = creatorObj.GetMonoILRComponent<UIBuddy3DModelCreater>();
                    mOtherCharacterPool.Add(creator);
                }

                for (int i = mOtherCharacterPool.Count - 1; i >= mCharacterPoolSize; --i)
                {
                    UIBuddy3DModelCreater creator = mOtherCharacterPool[i];
                    mOtherCharacterPool.RemoveAt(i);
                    creator.DestroyCharacter();
                    Object.Destroy(creator.mDMono.gameObject);
                }
            }
            //if (mCharacterPool.Contains(Current)) mCurrent = null;

            if (mCurrentOther != null)
            {
                mCurrentOther.ShowCharacter(false);
            }

            string targetVariantName = string.Format("{0}-I", variantName);
            UIBuddy3DModelCreater target = null;
            int targetIndex = -1;

            for (int i = 0, cnt = mOtherCharacterPool.Count; i < cnt; ++i)
            {
                UIBuddy3DModelCreater creator = mOtherCharacterPool[i];

                if (targetVariantName == creator.CharacterVariantTemplate)
                {
                    if (target != null)
                    {
                        target.ShowCharacter(false);
                    }

                    target = creator;
                    targetIndex = i;
                    break;
                }

                if (target == null)
                {
                    target = creator;
                    targetIndex = i;
                    continue;
                }

                if (creator.idle && !target.idle)
                {
                    target.ShowCharacter(false);

                    target = creator;
                    targetIndex = i;
                }
            }
            if (target != null)
            {// use pool
                mOtherCharacterPool.RemoveAt(targetIndex);
                mOtherCharacterPool.Add(target);
            }
            else
            {// don't use pool
                target = otherCharaacterCreator;
            }
            target.ShowCharacter(true);


            // create
            target.CharacterVariantTemplate = targetVariantName;
            target.CreatCharacter(variantPartitions);

            mCurrentOther = target;
            mScaled = false;
        }

        public override string VariantName
        {
            set
            {
                if (!canChangeModel)
                    return;

                if (characterCreator != null && characterCreator.loading)//如果在加载不能改变
                {
                    return;
                }

                if (mVariantName == value) //如果已经加载的是这个
                {
                    return;
                }
                if (characterCreator != null && characterCreator.mDMono.gameObject != null)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        string targetVariantName = mVariantName + "-I";
                        for (int i = 0, cnt = mCharacterPool.Count; i < cnt; ++i)
                        {
                            UIBuddy3DModelCreater creator = mCharacterPool[i];

                            if (targetVariantName == creator.CharacterVariantTemplate)
                            {
                                if (creator != null)
                                {
                                    creator.ShowCharacter(false);
                                }
                            }
                        }
                        if (!misMyTurn)
                        {
                            RefreshTarget(false);
                            outlineCmdBuffer.SetDir(1f);
                        }
                        else
                        {
                            RefreshTarget(true, false);
                            outlineCmdBuffer.SetDir(0f);
                        }
                        mVariantName = value;
                        return;
                    }
                }
                outlineCmdBuffer.outLineColor = new Color(42f / 255, 146f / 255, 1);
                //if (mOtherCharacterPool.Contains(mCurrent)) mCurrent = null;
                base.VariantName = value;

                if (_cWaitAvaterRefresh != null)
                {
                    StopCoroutine(_cWaitAvaterRefresh);
                }
                _cWaitAvaterRefresh = StartCoroutine(WaitAvatarRefresh());
            }
        }

        public IDictionary OhterVariantPartitions { get; set; } = null;

        public void SetTurn(bool isMyTurn)
        {
            misMyTurn = isMyTurn;

            selfTurnGO.gameObject.CustomSetActive(isMyTurn);
            otherTurnGO.gameObject.CustomSetActive(!isMyTurn);

            if (isMyTurn)
            {
                RefreshNuTarget();
                outlineCmdBuffer.SetDir(0f);
                outlineCmdBuffer.outLineColor = new Color(42f / 255, 146f / 255, 1);
            }
            else
            {
                outlineCmdBuffer.SetDir(1.0f);
                outlineCmdBuffer.outLineColor = Color.red;
            }
        }

        protected override void Connect()
        {
            base.Connect();
            if (mConnectorTexture != null)
            {
                //人物
                if (mHeroRenderTexture == null)
                {
                    mHeroRenderTexture = new RenderTexture((int)(mConnectorTexture.width * ConnectorTextureScale), (int)(mConnectorTexture.height * ConnectorTextureScale), 24, RenderTextureFormat.ARGB32);
                }

                heroCamera.targetTexture = mHeroRenderTexture;
                heroCamera.enabled = true;

                heroRTGO.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mHeroRenderTexture;
                //人物背后的效果
                mRenderTexture = new RenderTexture((int)(mConnectorTexture.width * 0.4f), (int)(mConnectorTexture.height * 0.4f), 24, RenderTextureFormat.ARGB32);
                lobbyCamera.targetTexture = mRenderTexture;
                //
                HeroPostEffectGO.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mRenderTexture;
            }
        }

        private void OutlineParams(OutlinePostEffectCmdBuffer outline)
        {
            outline.shader = shader;
            outline.outlineShader = outlineShader;
            outline.samplerScale = 4.55f;
            outline.iteration = 2;
            outline.outLineStrength = 1;
        }

        protected override void ReleaseCharacter(bool isDestroyCharacter = false)
        {
            for (int i = 0; i < mOtherCharacterPool.Count; ++i)
            {
                UIBuddy3DModelCreater creator = mOtherCharacterPool[i];
                if (creator != null)
                {
                    creator.ShowCharacter(false);
                    if (isDestroyCharacter)
                    {
                        creator.DestroyCharacter();
                    }

                }
            }
            mCurrentOther = null;
            base.ReleaseCharacter(isDestroyCharacter);
        }

        public override void OnEnable()
        {
			RegisterMonoUpdater();
            Connect();
            VariantName = VariantName;
            OtherVariantName = OtherVariantName;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
            StopAllCoroutines();
        }
        public new void Update()
        {
            if (mScaleCharacter && !mScaled && (mCurrent != null && mCurrent.character != null || mCurrentOther != null && mCurrentOther.character != null))
            {
                DoScaleCharacter();

                mScaled = true;
            }

            if (mCurrent != null && mCurrent.character != null)
            {
                MoveController moveController = mCurrent.character.GetComponent<MoveController>();
                AnimatorStateInfo asi = moveController.GetCurrentStateInfo();

                if (!((int)moveController.CurrentState == (int)MoveController.CombatantMoveState.kReady))
                {
                    if (asi.normalizedTime >= 1)
                    {
                        MoveEditor.Move theMove = moveController.GetMoveIfExists("Ready");
                        moveController.TransitionTo(MoveController.CombatantMoveState.kReady);
                        moveController.m_lobby_hash = Animator.StringToHash(string.Format("Lobby.{0}", theMove.name));
                        moveController.SetMove(theMove);
                        moveController.CrossFade(MoveController.m_ready_hash, 0.2f, 0, 0f);
                    }
                }
            }

            if (mCurrentOther != null && mCurrentOther.character != null)
            {
                MoveController moveController = mCurrentOther.character.GetComponent<MoveController>();
                AnimatorStateInfo asi = moveController.GetCurrentStateInfo();

                if (!((int)moveController.CurrentState == (int)MoveController.CombatantMoveState.kReady))
                {
                    if (asi.normalizedTime >= 1)
                    {
                        MoveEditor.Move theMove = moveController.GetMoveIfExists("Ready");
                        moveController.TransitionTo(MoveController.CombatantMoveState.kReady);
                        moveController.m_lobby_hash = Animator.StringToHash(string.Format("Lobby.{0}", theMove.name));
                        moveController.SetMove(theMove);
                        moveController.CrossFade(MoveController.m_ready_hash, 0.2f, 0, 0f);
                    }
                }
            }
        }

        IEnumerator WaitAvatarRefresh()
        {
            while (mCurrent != null && (mCurrent.loading || (mCurrent.Avatar != null && !mCurrent.Avatar.Ready)))
            {
                yield return null;
            }
            if (mCurrent == null) yield break;
            SetCharMoveState(mCurrent.character, selfMoveState);
            RefreshTarget(true, false);
        }

        IEnumerator WaitOtherAvatarRefresh()
        {
            while (mCurrentOther != null && (mCurrentOther.loading || (mCurrentOther.Avatar != null && !mCurrentOther.Avatar.Ready)))
            {
                yield return null;
            }
            if (mCurrentOther == null) yield break;
            SetCharMoveState(mCurrentOther.character, otherMoveState);
            RefreshTarget(false, true);
        }

        private void RefreshNuTarget()
        {
            outlineCmdBuffer.RefreshTargetObject(null, null);
        }

        private void RefreshTarget(bool needMyTarget = true, bool needOtherTarget = true)
        {
            RefreshNuTarget();
            outlineCmdBuffer.RefreshTargetObject(needMyTarget ? (characterCreator.character == null ? selfEmptyGameObject : characterCreator.character) : null,
                needOtherTarget ? (otherCharaacterCreator.character == null ? selfEmptyGameObject : otherCharaacterCreator.character) : null);
        }

        public void SetCharMoveState(GameObject character, MoveController.CombatantMoveState moveState)
        {
            if (character != null)
            {
                MoveController moveController = character.GetComponent<MoveController>();

                if (moveController == null)
                {
                    moveController = character.GetComponentInChildren<MoveController>();
                    if (moveController == null)
                    {
                        return;
                    }
                }

                ParticleSystem[] sys = character.GetComponentsInChildren<ParticleSystem>(true);
                for (int i = 0; i < sys.Length; i++)
                {
                    if (sys[i] != null && sys[i].GetComponent<Renderer>() != null)
                    {
                        //sys[i].scalingMode = ParticleSystemScalingMode.Hierarchy;
                        var main = sys[i].main;
                        main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                    }
                }

                System.Action fn = () =>
                {
                    moveController.TransitionTo(moveState);
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

    }
}
