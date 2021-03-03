using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Max820;
using System;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class UI3DLobby : DynamicMonoHotfix,IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            characterCreator = t.GetMonoILRComponent<UIBuddy3DModelCreater>("UI3DScene/CharacterContainer");
            lobbyCamera = t.GetComponent<Camera>("UI3DCamera");
            renderSettings = t.GetComponent<RenderSettings>("UI3DRenderSetting", false);
            smoothSpeed = 15f;
            useBloom = false;
            BloomCamera = t.GetComponent<Camera>("BloomCamera", false);
        }

        public UIBuddy3DModelCreater characterCreator;
        public Camera lobbyCamera;
        public RenderSettings renderSettings;
        public Light dynamicLight;
        public Vector2 characterRegion = new Vector2(2.5f, 2.5f);
        public float smoothSpeed = 15.0f;
        //public GameObject stage;
    
        protected RenderTexture mRenderTexture = null;
        protected string mVariantName = null;
        protected IDictionary mVariantPartitions = null;
        protected bool mConnected = false;
        protected List<UIBuddy3DModelCreater> mCharacterPool = new List<UIBuddy3DModelCreater>();
        protected int mCharacterPoolSize = 0;
        protected UIBuddy3DModelCreater mCurrent = null;
        protected bool mScaleCharacter = false;
        protected bool mScaled = false;
        protected UITexture mConnectorTexture = null;
    
        public bool useBloom = false;
        public UITexture BloomUiTexture;
        public RenderTexture BloomRT;
        public Camera BloomCamera;
    
        public UIBuddy3DModelCreater Current
        {
            get
            {
                return mCurrent;
            }
        }
    
        public virtual string VariantName
        {
            get { return mVariantName; }
            set { mVariantName = value; CreateCharacter(); }
        }
    
        public IDictionary VariantPartitions
        {
            get { return mVariantPartitions; }
            set { mVariantPartitions = value; }
        }
    
        public  int CharacterPoolSize
        {
            get { return mCharacterPoolSize; }
            set { mCharacterPoolSize = value; }
        }
    
        public bool ScaleCharacter
        {
            get { return mScaleCharacter; }
            set { mScaleCharacter = value; }
        }
    
        public UITexture ConnectorTexture
        {
    
            get { return mConnectorTexture; }
            set { mConnectorTexture = value; Connect(); }
        }
    
        public static void Preload(string variantName)
        {
            PoolModel.PreloadAsync(string.Format("Bundles/Player/Variants/{0}-I", variantName), delegate (UnityEngine.Object o){});
        }
    
        public static void PreloadWithCallback(string variantName, Action callback)
        {
            PoolModel.PreloadAsync(string.Format("Bundles/Player/Variants/{0}-I", variantName), delegate (UnityEngine.Object o) {
                callback?.Invoke();
            });
        }
    
        #region offset handler
        private const int totalOffsets = 100;
        private static int[] mPositionOffsets = new int[totalOffsets];
        private int mCurrentUseOffset = -1;
        //private void PositionOffsetInit()
        //{
        //    for (int i = 0; i < totalOffsets; i++)
        //    {
        //        mPositionOffsets[i,0] = 0;
        //    }
        //}
    
        protected virtual void SetTransformPosition()
        {
            for (int i = 0; i < totalOffsets; i++)
            {
                if (mPositionOffsets[i] == 0)
                {
                    mCurrentUseOffset = i;
                    break;
                }
            }
            if (mCurrentUseOffset == -1)
            {
                mCurrentUseOffset = 0;
            }
    
            mPositionOffsets[mCurrentUseOffset] = 1;
            mDMono.transform.position = new Vector3(100 * (mCurrentUseOffset + 1), 100 * (mCurrentUseOffset + 1), 0);
        }
    
        public override void OnDestroy()
        {
            if (BloomUiTexture != null)
            {
                UnityEngine.Object.Destroy(BloomUiTexture.gameObject);
            }
            DestroyOffsets();
            ReleaseCharacter(true);
        }
    
        private void DestroyOffsets()
        {
    
            if (mCurrentUseOffset >= 0)
            {
                mPositionOffsets[mCurrentUseOffset] = 0;
            }
        }
        #endregion
    
        private Bloom Bloom;
    
        public override void Start()
        {
            SetTransformPosition();
            //OnSetBloomEffect();
        }
    
        /// <summary>
        /// Bloom效果后处理接口，使用的时候必须调用
        /// </summary>
        /// <param name="uiTexture">后处理效果图片，不带原型</param>
        public void SetBloomUITexture(UITexture uiTexture)
        {
            if (Bloom == null)
            {
                Bloom = BloomCamera.gameObject.AddComponent<Bloom>();
                Bloom.thresholdGamma = Bloom.thresholdLinear = 0.99f;
                Bloom.softKnee = 0.05f;
                Bloom.intensity = 1;
                Bloom.radius = 4;
                Bloom.highQuality = false;
                Bloom.antiFlicker = false;
            }
            if (uiTexture && BloomCamera != null)
            {
                BloomCamera.enabled = true;
                BloomCamera.gameObject.SetActive(true);
                CartoonHelp.SetCartoonBrightLight(characterCreator.mDMono.gameObject, -0.05f);
    
                if (BloomUiTexture == null)
                {
                    BloomUiTexture = new GameObject("BloomUiTexture").AddComponent<UITexture>();
                    BloomUiTexture.gameObject.layer = 5;
                    BloomUiTexture.transform.parent = mDMono.transform.parent;
                    BloomUiTexture.shader = Shader.Find("Unlit/RenderTexture");
                    BloomUiTexture.SetRect(0, 0, uiTexture.localSize.x, uiTexture.localSize.y);
                    BloomUiTexture.transform.localScale = new Vector3(1.24f, 1, 0);
                    BloomUiTexture.transform.localPosition = new Vector3(0, 0, 0);
                    BloomUiTexture.depth = uiTexture.depth + 1;
                }
                if (BloomRT == null)
                {
                    BloomRT = new RenderTexture(700, 700, 16, RenderTextureFormat.ARGB32);
                }
                BloomCamera.targetTexture = BloomRT;
                BloomCamera.orthographicSize = 2;
                BloomUiTexture.mainTexture = BloomRT;
            }
            else
            {
                BloomCamera.enabled = false;
            }
        }
    
        public override void OnEnable()
        {
			RegisterMonoUpdater();
			Bloom = mDMono.transform.GetComponentInChildren<Bloom>();
            Connect();
            InitCharacter();
        }
    
        public override void OnDisable()
        {
            ErasureMonoUpdater();
            ReleaseCharacter();
            Disconnect();
        }
    
        public void Update()
        {
            if (mScaleCharacter && !mScaled && mCurrent != null && mCurrent.character != null)
            {
                DoScaleCharacter();
    
                mScaled = true;
            }
    
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
    
        protected void DoScaleCharacter()
        {
            if (mCurrent != null && mCurrent.character != null)
            {
                CapsuleCollider capsule = mCurrent.character.GetComponent<CapsuleCollider>();
                if (capsule == null)
                {
                    return;
                }
    
                float scale = 1.0f;
                if (capsule.direction == 0)
                {
                    scale = characterRegion.x / capsule.height;
                }
                else if (capsule.direction == 1)
                {
                    scale = characterRegion.y / capsule.height;
                }
                else if (capsule.direction == 2)
                {
                    scale = characterRegion.y / (capsule.center.y + capsule.radius);
                }
                if (scale < 1.0f)
                {
                    mCurrent.m_CharacterContainer.transform.localScale *= scale;
                }
            }
        }
    
        protected void CreateCharacter()
        {
            if (string.IsNullOrEmpty(mVariantName))
            {
                return;
            }
    
            // create pool
            if (mCharacterPool.Count != mCharacterPoolSize)
            {
                characterCreator.ShowCharacter(false);
    
                GameObject characterCreatorGO = characterCreator.mDMono.gameObject;
                Transform parentTransform = characterCreator.mDMono.transform.parent;
                for (int i = mCharacterPool.Count; i < mCharacterPoolSize; ++i)
                {
                    GameObject creatorObj = UnityEngine.Object.Instantiate(characterCreatorGO, parentTransform) as GameObject;
                    UIBuddy3DModelCreater creator = creatorObj.GetMonoILRComponent<UIBuddy3DModelCreater>();
                    mCharacterPool.Add(creator);
                }
    
                for (int i = mCharacterPool.Count - 1; i >= mCharacterPoolSize; --i)
                {
                    UIBuddy3DModelCreater creator = mCharacterPool[i];
                    mCharacterPool.RemoveAt(i);
                    creator.DestroyCharacter();
                    UnityEngine.Object.Destroy(creator.mDMono.gameObject);
                }
            }
    
            // find creator
            if (mCurrent != null)
            {
                mCurrent.ShowCharacter(false);
            }
    
            string targetVariantName = string.Format("{0}-I", mVariantName);
            UIBuddy3DModelCreater target = null;
            int targetIndex = -1;
            for (int i = 0, cnt = mCharacterPool.Count; i < cnt; ++i)
            {
                UIBuddy3DModelCreater creator = mCharacterPool[i];
    
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
                mCharacterPool.RemoveAt(targetIndex);
                mCharacterPool.Add(target);
            }
            else
            {// don't use pool
                target = characterCreator;
            }
            target.ShowCharacter(true);
    
            // create
            target.CharacterVariantTemplate = targetVariantName;
            target.CreatCharacter(mVariantPartitions);
    
            mCurrent = target;
            mScaled = false;
        }
    
        public void DestroyStage()
        {
            //GameObject.Destroy(stage.gameObject);
            //stage = null;
        }
    
        /// <summary>
        /// 初始化角色
        /// </summary>
        protected virtual void InitCharacter()
        {
            if (string.IsNullOrEmpty(mVariantName))
            {
                return;
            }
    
            string targetVariantName = string.Format("{0}-I", mVariantName);
            for (int i = 0; i < mCharacterPool.Count; ++i)
            {
                UIBuddy3DModelCreater creator = mCharacterPool[i];
                if (creator != null && !string.IsNullOrEmpty(creator.CharacterVariantTemplate) && creator.CharacterVariantTemplate.Equals(targetVariantName))
                {
                    creator.ShowCharacter(true);
                    mCurrent = creator;
                    mScaled = false;
                    break;
                }
            }
        }
    
        protected virtual void ReleaseCharacter(bool isDestroyCharacter = false)
        {
            for (int i = 0; i < mCharacterPool.Count; ++i)
            {
                UIBuddy3DModelCreater creator = mCharacterPool[i];
                if (creator != null)
                {
                    creator.ShowCharacter(false);
                    if (isDestroyCharacter)
                    {
                         creator.DestroyCharacter();
                    }
                   
                }
            }
    
            mCurrent = null;
            mScaled = true;
        }

        public float ConnectorTextureScale { get; set; } = 0.7f;
    
        protected virtual void Connect()
        {
            if (mConnectorTexture != null)
            {
                if (mRenderTexture == null)
                {
                    mRenderTexture = new RenderTexture((int)(mConnectorTexture.width * ConnectorTextureScale), (int)(mConnectorTexture.height * ConnectorTextureScale), 24, RenderTextureFormat.ARGB32);
                    mRenderTexture.useMipMap = false;
                    mRenderTexture.autoGenerateMips = false;
                }
               
                lobbyCamera.targetTexture = mRenderTexture;
                lobbyCamera.enabled = true;
                mConnectorTexture.mainTexture = mRenderTexture;
    
                if (!mConnected)
                {
                    UIEventListener listener = UIEventListener.Get(mConnectorTexture.gameObject);
                    listener.onClick += OnClick;
                    listener.onDoubleClick += OnDoubleClick;
                    listener.onHover += OnHover;
                    listener.onPress += OnPress;
                    listener.onKey += OnKey;
                    listener.onTooltip += OnTooltip;
                    listener.onDrag += OnDrag;
                }
    
                mConnected = true;
            }
    
            if (renderSettings != null)
            {
                renderSettings.gameObject.CustomSetActive(true);
            }
        }
    
        void Disconnect()
        {
            if (mConnectorTexture != null)
            {
                mConnectorTexture.mainTexture = null;
                lobbyCamera.targetTexture = null;
                lobbyCamera.enabled = false;
    
                if (mConnected)
                {
                    UIEventListener listener = UIEventListener.Get(mConnectorTexture.gameObject);
                    listener.onClick -= OnClick;
                    listener.onDoubleClick -= OnDoubleClick;
                    listener.onHover -= OnHover;
                    listener.onPress -= OnPress;
                    listener.onKey -= OnKey;
                    listener.onTooltip -= OnTooltip;
                    listener.onDrag -= OnDrag;
                }
            }
    
            if (mRenderTexture != null)
            {
                mRenderTexture.Release();
                UnityEngine.Object.Destroy(mRenderTexture);
                mRenderTexture = null;
            }
    
            if (renderSettings != null)
            {
                renderSettings.gameObject.CustomSetActive(false);
            }
    
            mConnected = false;
        }
    
        protected void OnClick(GameObject go)
        {
            if (mCurrent == null)
            {
                return;
            }
    
            if (mCurrent.character == null)
            {
                return;
            }
    
            PlayerEquipmentDataLookup equip = mCurrent.character.GetDataLookupILRComponent<PlayerEquipmentDataLookup>(false);
            if (equip != null)
            {
                equip.ForceTransitionToAlternateIdle();
            }
        }
    
        protected void OnDoubleClick(GameObject go)
        {
    
        }
    
        protected void OnHover(GameObject go, bool isOver)
        {
    
        }
    
        protected void OnPress(GameObject go, bool isPressed)
        {
    
        }
    
        protected void OnKey(GameObject go, KeyCode key)
        {
    
        }
    
        protected void OnTooltip(GameObject go, bool show)
        {
    
        }
    
        protected void OnDrag(GameObject go, Vector2 delta)
        {
            if (mCurrent == null)
            {
                return;
            }
    
            if (mCurrent.character == null)
            {
                return;
            }
    
            float angle = (-1 * delta.x) * smoothSpeed * Time.deltaTime;
            mCurrent.character.transform.Rotate(Vector3.up, angle);
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
    
        protected bool needToTransitionToIdle = false;
        protected MoveController.CombatantMoveState needToSetStateAfterMove = MoveController.CombatantMoveState.kIdle;
        protected int tempStateHash = 0;
    
        public void SetCharMoveState(MoveController.CombatantMoveState moveState, bool needToTransitionToIdle = false, MoveController.CombatantMoveState needToSetStateAfterMove = MoveController.CombatantMoveState.kIdle)
        {
            if (mCurrent != null && mCurrent.character != null)
            {
                MoveController moveController = mCurrent.character.GetComponent<MoveController>();
                System.Action fn = ()=>{
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
                }else{
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
    
        public void SetCameraPos(Vector3 pos){
            lobbyCamera.transform.localPosition = pos;
        }
        public void SetCameraRot(Vector3 rot){
            lobbyCamera.transform.localRotation = Quaternion.Euler(rot);
        }
        
        public void SetCameraMode(float size, bool orthographic)
        {
            lobbyCamera.orthographic = orthographic;

            if (orthographic)
            {
                lobbyCamera.orthographicSize = size;
            }
            else
            {
                lobbyCamera.fieldOfView = size;
            }

            lobbyCamera.nearClipPlane = 0.01f;
            lobbyCamera.farClipPlane = 9f;
        }
    
        public void PlayFx(string fxName)
        {
            if (string.IsNullOrEmpty(fxName))
            {
                return;
            }
            ParticleSystem mFX = PSPoolManager.Instance.Use(mDMono, fxName);
            if (mFX != null)
            {
                mFX.transform.position = mCurrent.character.transform.position;
                mFX.transform.localRotation = Quaternion.identity;
                mFX.transform.localScale = Vector3.one;
                mFX.EnableEmission(true);
                mFX.Simulate(0.0001f, true, true);
                NGUITools.SetLayer(mFX.gameObject, mCurrent.m_CharacterContainer.layer);
                mFX.Play(true);
            }
        }
    }
}
