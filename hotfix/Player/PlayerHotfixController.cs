using Hotfix_LT.Data;
using Hotfix_LT.UI;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.Player
{
    public class PlayerHotfixController : DynamicMonoHotfix, IHotfixUpdate
    {
        private HeadBars2D _HeadBar = null;
        private CombatController _combatController;
        private PlayerController playerController;
        private GameListenerFusion _gameListener;
        private MoveReticleComponent _moveReticle;
        private FXPoolable _moveReticleFX;
        private AvatarComponent _avatar;
        private Transform _muzzle;
        private Camera mCameraMain = null;
        private bool selectingTarget = false;
        private bool _shouldSetHideColor;
        private int _initialPlayerIndex = 0;
        private float _lastMoveUpdate;
        private float _lastMoveFailedUpdate;

        public void InitDataLookupSet()
        {
            PlayerDataLookupSet pdls = mDMono.transform.GetDataLookupILRComponentInChildren<PlayerDataLookupSet>("Hotfix_LT.UI.PlayerDataLookupSet", true, false);

            if (pdls != null)
            {
                pdls.UserId = playerController.playerUid;
            }
        }

        public override void Awake()
        {
            base.Awake();

            playerController = mDMono.transform.GetComponent<PlayerController>();

            if (Replication.IsLocalGame)
            {
                OnViewIdAllocated(playerController.ViewRPC);
            }

            playerController.TargetingComponent.OnMovementTargetChangeRequest += UpdateMovementReticle;
            //playerController.TargetingComponent.OnAttackTargetChanged += UpdateAttackReticle;
            //playerController.TargetingComponent.OnAttackTargetDeath += OnAttackTargetDeath;

            playerController.CharacterComponent.UseAdvancedLocomotionAnimation = true;

            if (GameEngine.Instance != null)
            {
                _gameListener = GameEngine.Instance.GameListener;
            }

            PlayerManager.RegisterPlayerController(playerController);
        }

        public override void Start()
        {
            if (playerController._isLocalPlayer)
            {
                Hotfix_LT.Messenger.AddListener("SetLeaderEvent", ChangeLeaderModel);
            }
        }

        public override void OnEnable()
        {
            RegisterMonoUpdater();

            if (_gameListener != null)
            {
                _gameListener.PlayerLeftEvent += OnPlayerLeft;
            }

            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                EventManager.instance.AddListener<TouchStartEvent>(OnTouchStartEvent);
                EventManager.instance.AddListener<TouchUpdateEvent>(OnTouchUpdateEvent);
                EventManager.instance.AddListener<TouchEndEvent>(OnTouchEndEvent);

                EventManager.instance.AddListener<TapEvent>(OnTapEvent);
                EventManager.instance.AddListener<DoubleTapEvent>(OnDoubleTapEvent);

                EventManager.instance.AddListener<TwoFingerTouchStartEvent>(OnTwoFingerTouchStartEvent);
                EventManager.instance.AddListener<TwoFingerTouchUpdateEvent>(OnTwoFingerTouchUpdateEvent);
                EventManager.instance.AddListener<TwoFingerTouchEndEvent>(OnTwoFingerTouchEndEvent);
            }
        }

        public void Update()
        {
            if (!GameEngine.Instance.IsTimeToRootScene)
            {
                return;
            }
            if (null != _moveReticle)
            {
                if (!playerController.TargetingComponent.HasMovementTarget()) // Both drop and reticle disappear after a time interval
                {
                    _moveReticleFX.OnPoolDeactivate();
                    _moveReticle.gameObject.CustomSetActive(false);
                }
            }

            if (_shouldSetHideColor && _avatar != null && _avatar.Ready)
            {
                GameEngine.Instance.SetHideColorTarget(playerController.SkinnedRigPrefab);

                _shouldSetHideColor = false;
            }
        }

        public override void OnDisable()
        {
            ErasureMonoUpdater();

            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                EventManager.instance.RemoveListener<TouchStartEvent>(OnTouchStartEvent);
                EventManager.instance.RemoveListener<TouchUpdateEvent>(OnTouchUpdateEvent);
                EventManager.instance.RemoveListener<TouchEndEvent>(OnTouchEndEvent);

                EventManager.instance.RemoveListener<TapEvent>(OnTapEvent);
                EventManager.instance.RemoveListener<DoubleTapEvent>(OnDoubleTapEvent);

                EventManager.instance.RemoveListener<TwoFingerTouchStartEvent>(OnTwoFingerTouchStartEvent);
                EventManager.instance.RemoveListener<TwoFingerTouchUpdateEvent>(OnTwoFingerTouchUpdateEvent);
                EventManager.instance.RemoveListener<TwoFingerTouchEndEvent>(OnTwoFingerTouchEndEvent);

            }

            if (_gameListener != null)
            {
                _gameListener.PlayerLeftEvent -= OnPlayerLeft;
            }

            var helpers = mDMono.GetComponentsInChildren<MoveEditor.FXHelper>();
            if (helpers != null && helpers.Length > 0)
            {
                for (var i = 0; i < helpers.Length; i++)
                {
                    helpers[i].StopAll(true);
                }
            }

            //ToDo:看起来无用，暂时屏蔽
            //PlayMakerGlobals.Instance.Variables.FindFsmGameObject("Player" + (_initialPlayerIndex + 1)).Value = null;

            StopAllCoroutines();
        }

        public override void OnDestroy()
        {
            _gameListener = null;
            playerController.TargetingComponent.OnMovementTargetChangeRequest -= UpdateMovementReticle;
            PlayerManager.UnregisterPlayerController(playerController);
            if (_moveReticle != null)
                GameObject.Destroy(_moveReticle.gameObject);
            if (playerController._isLocalPlayer)
            {
                Hotfix_LT.Messenger.RemoveListener("SetLeaderEvent", ChangeLeaderModel);
            }
        }

        public void Destroy()
        {
            var helpers = mDMono.GetComponentsInChildren<MoveEditor.FXHelper>();
            if (helpers != null && helpers.Length > 0)
            {
                for (var i = 0; i < helpers.Length; i++)
                {
                    helpers[i].StopAll(true);
                }
            }

            PlayerDataLookupSet pdls = mDMono.transform.GetDataLookupILRComponentInChildren<PlayerDataLookupSet>("Hotfix_LT.UI.PlayerDataLookupSet", showErrorTips: false);
            if (pdls != null) pdls.Destroy();

            CharacterVariant variant = mDMono.GetComponentInChildren<CharacterVariant>();
            if (variant != null)
            {
                variant.Recycle();
                PoolModel.DestroyModel(variant.gameObject);
            }

            GameObject.Destroy(mDMono.gameObject);
        }

        private void OnViewIdAllocated(ReplicationView view)
        {
            if (playerController.ViewRPC != view)
            {
                return;
            }

            if (!playerController._isLocalPlayer)
            {
                return;
            }
            mDMono.gameObject.name = (playerController.ViewRPC != null ? "Player" + playerController.ViewRPC.viewId.p : "Player");

            playerController.ReplicationPlayer = playerController.ViewRPC != null ? playerController.ViewRPC.instantiatorPlayer : null;

            _initialPlayerIndex = (Replication.IsLocalGame) ? 0 : playerController.ReplicationPlayer.Index;

            TransferDartMember dartData = AlliancesManager.Instance.TransferDartInfo.GetCurrentDart();
            if (AllianceUtil.IsInTransferDart && dartData != null)
            {
                StartTransfer(dartData.DartName, dartData.TargetNpc, false, true);
            }
            else
            {
                string userid = LoginManager.Instance.LocalUserId.Value.ToString();
                string classname = UI.BuddyAttributesManager.GetModelClass(userid);
                if (string.IsNullOrEmpty(classname))
                {
                    EB.Debug.LogError("OnViewIdAllocated: classname not found for uid = {0}", userid);
                    return;
                }
                ChangeModel(classname, true);
            }

            if (_initialPlayerIndex == Replication.LocalPlayerIndex)
            {
                EB.Assets.LoadAsync("Bundles/VFX/ReticleFX/Reticle_TargetLocation", typeof(GameObject), o =>
                 {
                     if (o) {
                         GameObject go = GameObject.Instantiate(o as GameObject);
                         _moveReticle = go.GetComponent<MoveReticleComponent>();
                         _moveReticleFX = _moveReticle.GetComponent<FXPoolable>();
                         _moveReticle.gameObject.CustomSetActive(false);
                         SetInstantiatedParents();
                     }
                 });
            }
        }

        public void StartTransfer(string dartName, string npc_id, bool allPath, bool firstCreate)
        {
            if (PlayerController.onCollisionExit != null && PlayerController.CurNpcCollision != null)
            {
                PlayerController.onCollisionExit(PlayerController.CurNpcCollision.gameObject.name);
                PlayerController.CurNpcCollision = null;
            }
            string modelName = AllianceEscortUtil.GetTransportCartModel(dartName);
            float scale_size = (modelName.IndexOf("M1003") >= 0 || modelName.IndexOf("M1004") >= 0) ? 0.6f : 1;
            ChangeModel(modelName, firstCreate, scale_size);
            if (!mDMono.gameObject.activeSelf)
                mDMono.gameObject.SetActive(true);
            StartCoroutine(WaitforRegisterGameobject(npc_id, allPath));
        }

        private IEnumerator WaitforRegisterGameobject(string npc_id, bool allPath)
        {
            Vector3 startPoint;
            EB.Collections.Queue<Vector3> posQue = FindTransferPath(npc_id, allPath, out startPoint);
            if (allPath)
            {
                mDMono.transform.position = startPoint;
                Vector3 newPos = mDMono.transform.position;
                float newDir = mDMono.transform.rotation.eulerAngles.y;
                ((SceneManager)LTHotfixManager.GetManager("SceneManager")).UpdatePlayerMovement(MainLandLogic.GetInstance().SceneId, newPos, newDir, null);
            }
            mDMono.transform.LookAt(posQue.Peek());

            while (!PlayerManager.IsLocalPlayer(mDMono.gameObject) || !SceneLogic.FuncNpcLoadCompleted)//确保NPC加载完成再寻路
            {
                yield return null;
            }
            playerController.TargetingComponent.SetMovementTargetQueue(posQue, delegate () {
                ChangeLeaderModel();
            });
        }

        private EB.Collections.Queue<Vector3> FindTransferPath(string npc_id, bool allPath, out Vector3 startPoint)
        {
            startPoint = Vector3.zero;
            SceneRootEntry sceneRoot = SceneLoadManager.GetSceneRoot(SceneLoadManager.CurrentSceneName);
            if (sceneRoot == null)
            {
                EB.Debug.LogError("sceneRoot is null");
                return null;
            }

            Transform pathRoot = sceneRoot.m_SceneRoot.transform.GetComponentInChildren<LevelHelper>().transform.Find("TransportPaths/" + npc_id);
            startPoint = new Vector3(pathRoot.GetChild(0).position.x, mDMono.transform.position.y, pathRoot.GetChild(0).position.z);
            EB.Collections.Queue<Vector3> posQue = new EB.Collections.Queue<Vector3>();

            int nextpointIndex = AlliancesManager.Instance.TransferDartInfo.NextTransferPoint;
            if (allPath)
                nextpointIndex = pathRoot.childCount - 2;
            //else nextpointIndex = nextpointIndex == 0 ? 0 : nextpointIndex-1;//没到第一个检查点强行设置为第一个
            float localPlayerY = mDMono.transform.position.y;
            for (int pointindex = pathRoot.childCount - nextpointIndex - 1; pointindex < pathRoot.childCount; ++pointindex)
            {
                Transform t = pathRoot.GetChild(pointindex);
                Vector3 pos = new Vector3(t.position.x, localPlayerY, t.position.z);
                posQue.Enqueue(pos);
            }
            return posQue;

        }

        public void ChangeLeaderModel()
        {
            string tid;
            DataLookupsCache.Instance.SearchDataByID<string>("user.leaderId", out tid);
            int skin;
            DataLookupsCache.Instance.SearchIntByID("user.skin", out skin);
            ChangeLeaderModel(tid, skin);
        }

        public void ChangeLeaderModel(string tid, int skin = 0)
        {
            string model_name = null;
            string characterid = CharacterTemplateManager.Instance.TemplateidToCharacterid(tid);
            if (characterid == null)
            {
                EB.Debug.LogError("ChangeLeaderModel:characterid == null For tid={0}", tid);
                return;
            }
            var charTpl = CharacterTemplateManager.Instance.GetHeroInfo(characterid, skin);
            model_name = charTpl.model_name;//需添加皮肤
            if (string.IsNullOrEmpty(model_name))
            {
                EB.Debug.LogError("OnViewIdAllocated: classname not found for tid = {0}", tid);
                return;
            }
            ChangeModel(model_name, false);
        }

        public void ChangeModel(string modelName, bool firstCreate, float scale = 1)
        {
            if (playerController == null)
            {
                EB.Debug.LogWarning("playerController is null");
                return;
            }

            if (playerController.CharacterModel != null && playerController.CharacterModel.ResourcePrefabNameMain.IndexOf(modelName) >= 0)
            {
                EB.Debug.LogWarning("ChangeModel: Aready Exist model={0}", modelName);
                return;
            }

            if (_HeadBar != null)
            {
                _HeadBar.ClearBars();
                _HeadBar = null;
            }

            try
            {
                CharacterVariant CV = mDMono.transform.GetComponentInChildren<CharacterVariant>();
                MoveEditor.FXHelper FH = mDMono.transform.GetComponentInChildren<MoveEditor.FXHelper>();

                if (FH != null)
                {
                    FH.StopAll(true);
                }

                if (CV != null && GameEngine.Instance != null)
                {
                    SetObjLayer(CV.CharacterInstance, GameEngine.Instance.defaultLayer);
                    CV.Recycle();
                    PoolModel.DestroyModel(CV.gameObject);
                }

                if (playerController.SkinnedRigPrefab != null)
                {
                    PoolModel.DestroyModel(playerController.SkinnedRigPrefab);
                }
                ///
                playerController.Gender = eGender.Male;
                playerController.CharacterModel = CharacterCatalog.Instance.GetModel(modelName);
                string prefabName = playerController.CharacterModel.PrefabNameFromGenderMain(playerController.Gender);
                PoolModel.GetModelAsync(prefabName, mDMono.transform.position, Quaternion.identity, (o, prm) =>
                {
                    var variantObj = o as GameObject;
                    variantObj.transform.SetParent(mDMono.transform);
                    variantObj.transform.localPosition = Vector3.zero;

                    CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
                    variant.SyncLoad = true;
                    IDictionary partions = GetPartitionsData(LoginManager.Instance.LocalUserId.Value);
                    variant.InstantiateCharacter(partions);
                    InitDataLookupSet();

                    playerController.SkinnedRigPrefab = variant.CharacterInstance;
                    playerController.SkinnedRigPrefab.name = prefabName + "_Character";
                    playerController.SkinnedRigPrefab.tag = "CharacterMesh";
                    playerController.SkinnedRigPrefab.transform.SetParent(mDMono.transform);
                    playerController.SkinnedRigPrefab.transform.localRotation = Quaternion.identity;
                    playerController.SkinnedRigPrefab.transform.localScale = new Vector3(scale, scale, scale);
                    _combatController = mDMono.GetComponent<CombatController>();
                    _combatController.Initialize(playerController.SkinnedRigPrefab.transform, playerController.CharacterModel, 1 << LayerMask.NameToLayer("Enemy"));

                    playerController.CharacterComponent = mDMono.GetComponent<CharacterComponent>();
                    playerController.CharacterComponent.OnSpawn(playerController.CharacterModel, playerController.SkinnedRigPrefab, -1, false);

                    _HeadBar = playerController.SkinnedRigPrefab.GetMonoILRComponent<HeadBars2D>(false);
                    if (_HeadBar == null)
                    {
                        _HeadBar = playerController.SkinnedRigPrefab.AddMonoILRComponent<HeadBars2D>("Hotfix_LT.UI.HeadBars2D");
                    }

                    if (mDMono.GetComponent<Collider>() != null && mDMono.GetComponent<Collider>() is BoxCollider)
                    {
                        BoxCollider box = mDMono.GetComponent<Collider>() as BoxCollider;
                        box.size = new Vector3(1.5f, 1.5f, 1.5f) + new Vector3(0, playerController.CharacterModel.heightOffset, 0);
                        box.center = new Vector3(0, box.size.y / 2.0f, 0);
                    }

                    _muzzle = GameUtils.SearchHierarchyForBone(playerController.SkinnedRigPrefab.transform, "muzzle");
                    if (null == _muzzle)
                    {
                        _muzzle = mDMono.transform;
                    }

                    if (firstCreate)
                        LevelOwnerComponent.AssignLevelOwner(mDMono.gameObject); // this must be called after PlayerManager.RegisterPlayerController(this);

                    _avatar = playerController.SkinnedRigPrefab.GetComponent<AvatarComponent>();
                        _shouldSetHideColor = !PerformanceManager.Instance.CurrentEnvironmentInfo.slowDevice;

                    if (!firstCreate)
                    {
                        PlayerDataLookupSet PDLS = playerController.SkinnedRigPrefab.transform.GetDataLookupILRComponent<PlayerDataLookupSet>();
                        if (PDLS != null)
                        {
                            PDLS.UserId = playerController.playerUid;
                        }
                    }

                    MoveController mc = playerController.SkinnedRigPrefab.GetComponent<MoveController>();
                    if (mDMono != null && mDMono.GetComponent<CharacterComponent>().State != eCampaignCharacterState.Idle)
                    {
                        mc.TransitionTo(MoveController.CombatantMoveState.kLocomotion);
                    }
                }, null);
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
        }

        public static IDictionary GetPartitionsData(long userid)
        {
            string dataid = string.Format("{0}.pl.{1}.equip", SceneLogicManager.getMultyPlayerSceneType(), userid);
            IDictionary user_equip_data;
            Dictionary<string, string> partitions = new Dictionary<string, string>();
            DataLookupsCache.Instance.SearchDataByID<IDictionary>(dataid, out user_equip_data);
            if (user_equip_data == null)
            {
                return partitions;
            }
            foreach (DictionaryEntry entry in user_equip_data)
            {
                if (entry.Value == null)
                {
                    continue;
                }
                else
                {
                    string equipmentType = EconomyConstants.AbToEquipmentType(entry.Key.ToString());
                    string economyid = entry.Value.ToString();
                    if (!PlayerEquipmentDataLookup.VALID_EQUIPMENT_SLOTS.Contains(equipmentType)) continue;
                    if (string.IsNullOrEmpty(economyid) || string.IsNullOrEmpty(equipmentType))
                    {
                        continue;
                    }
                    string raceModel = PlayerEquipmentDataLookup.GetModeAtributeName(userid.ToString());
                    if (string.IsNullOrEmpty(raceModel))
                    {
                        continue;
                    }
                    string equipmentName = EconemyTemplateManager.GetPartitionName(raceModel, economyid);
                    if (string.IsNullOrEmpty(equipmentName))
                    {
                        continue;
                    }
                    else
                    {
                        partitions.Add(equipmentType, equipmentName);
                    }
                }
            }
            return partitions;
        }

        private void OnPlayerLeft(EB.Sparx.Game game, EB.Sparx.Player player)
        {
            if (LevelOwnerComponent.IsLevelOwner(player))
            {
                LevelOwnerComponent.AssignLevelOwner(mDMono.gameObject);
            }
        }

        private void SlowUpdate()
        {
            if (playerController.TargetingComponent != null && playerController.TargetingComponent.AttackTarget != null && playerController.CharacterComponent != null && playerController.CharacterComponent.CurrentHandler != null && (GameEngine.Instance.OutlineColor == GameEngine.eOutlineColor.EnemyInRange || GameEngine.Instance.OutlineColor == GameEngine.eOutlineColor.EnemyOutOfRange))
            {
                GameEngine.Instance.SetOutlineColor(playerController.CharacterComponent.CurrentHandler.CachedIsInRange ? GameEngine.eOutlineColor.EnemyInRange : GameEngine.eOutlineColor.EnemyOutOfRange);
            }
        }

        private void SetObjLayer(GameObject _obj, int _nLayer)
        {
            if (_obj != null)
            {
                _obj.layer = _nLayer;
                Renderer[] renderers = _obj.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].gameObject.layer = _nLayer;
                }
            }
        }

        private void OnTouchStartEvent(TouchStartEvent evt)
        {
            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                if (AllianceUtil.IsInTransferDart)
                {
                    return;
                }

                SelectionLogic.MaxTouches = 1;
                playerController.CharacterComponent.LocomotionComponent.TouchStarted();
            }
        }

        private void OnTouchUpdateOrTap(Transform target, Vector3 groundPosition, bool hasValidNavPoint, bool isTouchUpdateEvent = false)
        {
            if (AllianceUtil.IsInTransferDart || playerController == null)
            {
                return;
            }

            StopAutoPathMove(false);

            if (selectingTarget && target != null)
            {
                CharacterComponent targetCharacter = target.GetComponent<CharacterComponent>();

                if (targetCharacter != null && targetCharacter.Model != null)
                {
                    if (playerController.CharacterModel != null && playerController.CharacterModel.team.IsEnemy(targetCharacter.Model.team))
                    {
                        if (playerController.TargetingComponent != null)
                        {
                            playerController.TargetingComponent.SetAttackTarget(target.gameObject);
                        }

                        GameEngine.Instance.SetOutlineColor(playerController.CharacterComponent.CurrentHandler.IsInRangeOfTarget() ? GameEngine.eOutlineColor.EnemyInRange : GameEngine.eOutlineColor.EnemyOutOfRange);
                        
                        if (_combatController != null && _combatController.IsMelee && playerController.TargetingComponent != null)
                        {
                            playerController.TargetingComponent.SetMovementTarget(target.position, false,false);
                        }

                        //在这里发出消息 隐掉hud右上角显示目标
                        Hotfix_LT.Messenger.Raise<GameObject, GameObject>("OnImmediatelyInteractEvent", mDMono.gameObject, null);
                    }
                    else if (targetCharacter.Model.team == eTeamId.Interactable)
                    {
                        InteractableComponent interactable = target.GetComponent<InteractableComponent>();
                        InteractableStatsComponent interactableStats = target.GetComponent<InteractableStatsComponent>();

                        if ((interactable != null && interactable.interactOnTap) || (interactableStats != null && interactableStats.IsInRange(mDMono.gameObject.transform)))
                        {
                            interactableStats.Interact(mDMono.gameObject);
                        }
                        else if (playerController.TargetingComponent != null)
                        {
                            playerController.TargetingComponent.SetAttackTarget(target.gameObject);
                        }
                        //在这里发出消息 隐掉hud右上角显示目标
                        Hotfix_LT.Messenger.Raise<GameObject, GameObject>("OnImmediatelyInteractEvent", mDMono.gameObject, null);
                    }
                    else if (targetCharacter.Model.team == eTeamId.Player && targetCharacter.gameObject != mDMono.gameObject)
                    {
                        InteractableComponent interactable = target.GetComponent<InteractableComponent>();
                        InteractableStatsComponent interactableStats = target.GetComponent<InteractableStatsComponent>();

                        if ((interactable != null && interactable.interactOnTap) || (interactableStats != null && interactableStats.IsInRange(mDMono.gameObject.transform)))
                        {
                            interactableStats.Interact(mDMono.gameObject);
                        }
                        else if (playerController.TargetingComponent != null)
                        {
                            playerController.TargetingComponent.SetAttackTarget(target.gameObject);

                        }
                        Hotfix_LT.Messenger.Raise<GameObject, GameObject>("OnImmediatelyInteractEvent", mDMono.gameObject, target.gameObject);
                    }
                }
            }
            else if (hasValidNavPoint)
            {
                if (target == null || (_combatController != null && _combatController.IsMelee))
                {
                    if (playerController.TargetingComponent != null)
                    {
                        if (playerController.TargetingComponent.AttackTarget != null)
                        {
                            playerController.TargetingComponent.SetAttackTarget(null);
                        }
                        playerController.TargetingComponent.SetMovementTarget(groundPosition, false, true, !isTouchUpdateEvent);
                    }

                    //在这里发出消息 隐掉hud右上角显示目标
                    Hotfix_LT.Messenger.Raise<GameObject, GameObject>("OnImmediatelyInteractEvent",mDMono.gameObject, null);
                }
            }
            else
            {
                if (playerController.TargetingComponent != null && playerController.TargetingComponent.AttackTarget != null)
                {
                    playerController.TargetingComponent.SetAttackTarget(null);
                }
            }
        }

        private void OnTouchUpdateEvent(TouchUpdateEvent evt)
        {
            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                if (AllianceUtil.IsInTransferDart)
                {
                    return;
                }

                SelectionLogic.MaxTouches = 1;
                selectingTarget = false;
                OnTouchUpdateOrTap(evt.target, evt.groundPosition, evt.hasValidNavPoint, true);
            }
        }

        private void OnTouchEndEvent(TouchEndEvent evt)
        {
            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                if (AllianceUtil.IsInTransferDart)
                {
                    return;
                }

                //HideSelfSelection();
                SelectionLogic.MaxTouches = SelectionLogic.DEFAULT_MAX_TOUCHES;

                if (evt.target == mDMono.transform)
                {
                    playerController .TargetingComponent.SetMovementTarget(Vector3.zero, true);
                }

                playerController.CharacterComponent.LocomotionComponent.TouchEnded();
            }
        }

        private void OnTapEvent(TapEvent evt)
        {
            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                if (AllianceUtil.IsInTransferDart)
                {
                    return;
                }

                SelectionLogic.MaxTouches = SelectionLogic.DEFAULT_MAX_TOUCHES;
                if (evt.hasValidNavPoint)
                {
                    playerController.CharacterComponent.LocomotionComponent.TouchTap();
                }
                selectingTarget = true;
                OnTouchUpdateOrTap(evt.target, evt.groundPosition, evt.hasValidNavPoint);

                if (evt.target == mDMono.transform)
                {
                    AutoPickupLoot();
                    playerController.TargetingComponent.SetMovementTarget(Vector3.zero, true);
                }

                //HideSelfSelection();
            }
        }

        private void OnDoubleTapEvent(DoubleTapEvent evt)
        {
            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                if (AllianceUtil.IsInTransferDart)
                {
                    return;
                }

                SelectionLogic.MaxTouches = SelectionLogic.DEFAULT_MAX_TOUCHES;
            }
        }

        private void OnTwoFingerTouchStartEvent(TwoFingerTouchStartEvent evt)
        {
            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                SelectionLogic.MaxTouches = 2;
            }
        }

        private void OnTwoFingerTouchUpdateEvent(TwoFingerTouchUpdateEvent evt)
        {
            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                SelectionLogic.MaxTouches = 2;
            }
        }

        private void OnTwoFingerTouchEndEvent(TwoFingerTouchEndEvent evt)
        {
            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                SelectionLogic.MaxTouches = SelectionLogic.DEFAULT_MAX_TOUCHES;
            }
        }

        private void StopAutoPathMove(bool immediately = true)
        {
            WorldMapPathManager.Instance.StopPath();
        }

        private ConditionComponent mConditionComponent = null;

        public void OnJoystickAxis(float XAxis, float YAxis)
        {
            if (mDMono == null)
            {
                return;
            }

            if (PlayerManager.IsLocalPlayer(mDMono.gameObject))
            {
                if (mConditionComponent == null)
                {
                    mConditionComponent = mDMono.gameObject.GetComponent<ConditionComponent>();
                }

                if (mConditionComponent != null && !mConditionComponent.CanMove)
                {
                    return;
                }

                if (XAxis == 0 && YAxis == 0)
                {
                    return;
                }

                if (getMainCamera() == null)
                {
                    return;
                }

                Vector3 movePos = mDMono.transform.position;
                Vector3 forwardComponent = getMainCamera().transform.forward * YAxis;
                forwardComponent.y = 0;
                Vector3 rightComponent = getMainCamera().transform.right * XAxis;
                rightComponent.y = 0;
                Vector3 relativeMove = (forwardComponent + rightComponent).normalized * 2.5f;
                movePos += relativeMove;

                if (playerController != null && playerController.TargetingComponent != null)
                {
                    playerController.TargetingComponent.SetMovementTarget(movePos, false, true, false);
                }
            }
        }
        
        private void AutoPickupLoot()
        {
            //Collider[] hits = Physics.OverlapSphere(mDMono.transform.position, GlobalBalanceData.Instance.selfSelectRadius, 1 << GameEngine.Instance.interactableLayer);

            //foreach (Collider c in hits)
            //{
            //    FlippyComponent flippy = c.GetComponent<FlippyComponent>();
            //    if (flippy != null)
            //    {
            //        flippy.Interact(mDMono .gameObject);
            //    }
            //}
        }

        private Camera getMainCamera()
        {
            if (mCameraMain == null)
            {
                mCameraMain = Camera.main;
            }
            return mCameraMain;
        }
        
        public void HideMoveReticle()
        {
            if (_moveReticle != null)
                _moveReticle.gameObject.CustomSetActive(false);
            if (_moveReticleFX != null)
                _moveReticleFX.OnPoolDeactivate();
        }

        public override void OnHandleMessage(string methodName, object value)
        {
            if (string.Equals(methodName, "OnTriggerEnter"))
            {
                OnTriggerEnter(value as Collider);
            }
            else if (string.Equals(methodName, "OnTriggerExit"))
            {
                OnTriggerExit(value as Collider);
            }
            else if (string.Equals(methodName, "Destroy"))
            {
                Destroy();
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (!playerController._isLocalPlayer || SceneLogicManager.CurrentSceneLogic == null)
            {
                return;
            }
            GameObject other = collision.gameObject;
            string npc_id = other.name;
            string scene_name = SceneLogicManager.CurrentSceneLogic.CurrentSceneName;
            
            if (IsFunctionNPC(npc_id, scene_name))
            {
                if (HasUIModel(npc_id, scene_name, "DeliveryDart") && AlliancesManager.Instance.DartData.State == eAllianceDartCurrentState.Transfering)
                {
                    GlobalMenuManager.Instance.CloseMenu("LTApplyHelpUI");
                    NpcColliderUI.DeliveryDart();
                }
                else
                {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("npc", npc_id);
                    ht.Add("scene", scene_name);
                    MainLandEncounterTemplate met = SceneTemplateManager.GetMainLandsNPCData(scene_name, npc_id);
                    if (met.role == "area")
                    {
                        ht.Add("area", true);
                    }
                    if (PlayerController.onCollisionOpen != null)
                    {
                        PlayerController.CurNpcCollision = collision;
                        PlayerController.onCollisionOpen(ht);
                    }
                }

                LTCameraTrigger ct = other.GetComponent<LTCameraTrigger>();
                if (ct != null)
                {
                    Camera mainCam = getMainCamera();
                    if (mainCam != null)
                    {
                        PlayerCameraComponent pcc = mainCam.GetComponent<PlayerCameraComponent>();
                        if (pcc != null)
                        {
                            if (!string.IsNullOrEmpty(ct.triggerGameCameraParamName))
                            {
                                GameCameraParams gameCameraParams = GlobalCameraData.Instance.FindGameCameraParamsByName(ct.triggerGameCameraParamName);
                                CameraLerp motion_lerp = GlobalCameraData.Instance.FindGameCameraLerpByName("dialogue lerp");
                                List<GameObject> listGO = new List<GameObject>();
                                listGO.Add(mDMono.gameObject);
                                pcc.EnterInteractionCamera(ref listGO, ref gameCameraParams, motion_lerp);
                            }
                        }
                    }
                    else
                    {
                        GameObject cam = GameObject.Find("Main Camera");
                        if (cam != null)
                        {
                            PlayerCameraComponent pcc = cam.GetComponent<PlayerCameraComponent>();
                            if (pcc != null)
                            {
                                EB.Debug.Log("Can Find Main Camera!");
                                if (!string.IsNullOrEmpty(ct.triggerGameCameraParamName))
                                {
                                    GameCameraParams gameCameraParams = GlobalCameraData.Instance.FindGameCameraParamsByName(ct.triggerGameCameraParamName);
                                    CameraLerp motion_lerp = GlobalCameraData.Instance.FindGameCameraLerpByName("dialogue lerp");
                                    List<GameObject> listGO = new List<GameObject>();
                                    listGO.Add(mDMono .gameObject);
                                    pcc.EnterInteractionCamera(ref listGO, ref gameCameraParams, motion_lerp);
                                }
                            }
                        }
                    }
                }
            }
            else if (other.name.Equals("NPCSpawns_F"))//与决斗场模型碰撞，需移动到出生点
            {
                playerController.transform.position = SceneManager.HeroStart;//如果是角斗场雕像，跳转到初始位置
            }
            else
            {
                LTCameraTrigger ct = other.GetComponent<LTCameraTrigger>();
                if (ct != null)
                {
                    Camera mainCam = getMainCamera();//Camera.main;

                    if (mainCam != null)
                    {
                        PlayerCameraComponent pcc = mainCam.GetComponent<PlayerCameraComponent>();
                        if (pcc != null)
                        {
                            if (!string.IsNullOrEmpty(ct.triggerGameCameraParamName))
                            {
                                GameCameraParams gameCameraParams = GlobalCameraData.Instance.FindGameCameraParamsByName(ct.triggerGameCameraParamName);
                                CameraLerp motion_lerp = GlobalCameraData.Instance.FindGameCameraLerpByName("dialogue lerp");
                                List<GameObject> listGO = new List<GameObject>();
                                listGO.Add(mDMono . gameObject);
                                pcc.EnterInteractionCamera(ref listGO, ref gameCameraParams, motion_lerp);
                            }
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (!playerController._isLocalPlayer)
            {
                return;
            }
            GameObject other = collision.gameObject;
            string npc_id = other.name;
            if (SceneLogicManager.CurrentSceneLogic != null)
            {
                string scene_name = SceneLogicManager.CurrentSceneLogic.CurrentSceneName;
                if (IsFunctionNPC(npc_id, scene_name))
                {
                    if (PlayerController.onCollisionExit != null)
                    {
                        PlayerController. CurNpcCollision = null;
                        PlayerController.onCollisionExit(collision.gameObject.name);
                    }
                }
            }

            LTCameraTrigger ct = other.GetComponent<LTCameraTrigger>();
            if (ct != null)
            {
                Camera mainCam = getMainCamera();
                if (mainCam != null)
                {
                    PlayerCameraComponent pcc = mainCam.GetComponent<PlayerCameraComponent>();
                    if (pcc != null)
                    {
                        //pcc.ResetCameraState();
                        GameCameraParams gameCameraParams = GlobalCameraData.Instance.FindGameCameraParamsByName("game cam default");
                        CameraLerp motion_lerp = GlobalCameraData.Instance.FindGameCameraLerpByName("dialogue lerp");
                        List<GameObject> listGO = new List<GameObject>();
                        listGO.Add(mDMono.gameObject);
                        pcc.EnterInteractionCamera(ref listGO, ref gameCameraParams, motion_lerp);
                    }
                }
            }
        }


        private bool IsFunctionNPC(string npc_id, string scene_name)
        {
            MainLandEncounterTemplate met = SceneTemplateManager.GetMainLandsNPCData(scene_name, npc_id);
            if (met != null)
            {
                if (met.func_id_1 > 0 || met.func_id_2 > 0 || met.func_id_3 > 0 || met.dialogue_id > 0) return true;
            }
            return false;
        }

        private bool HasUIModel(string npc_id, string scene_name, string uiModelName)
        {
            MainLandEncounterTemplate met = SceneTemplateManager.GetMainLandsNPCData(scene_name, npc_id);
            if (met != null)
            {
                if (met.func_id_1 > 0 && FuncTemplateManager.Instance.GetFunc(met.func_id_1).ui_model == uiModelName)
                {
                    return true;
                }
                else if (met.func_id_2 > 0 && FuncTemplateManager.Instance.GetFunc(met.func_id_2).ui_model == uiModelName)
                {
                    return true;
                }
                else if (met.func_id_3 > 0 && FuncTemplateManager.Instance.GetFunc(met.func_id_3).ui_model == uiModelName)
                {
                    return true;
                }
            }
            return false;
        }

        private void SetInstantiatedParents()
        {
            if (mDMono != null && mDMono.gameObject == null)
            {
                EB.Debug.LogError("SetInstantiatedParents gameObject = null");
            }

            if (mDMono != null && _moveReticle != null)
            {
                _moveReticle.transform.SetParent(mDMono.gameObject.transform.parent);
            }
        }

        public void SetPlayerSpawnLocation(Vector3 pos)
        {
            playerController .SpawnLocation = pos;
        }


        public void StopTransfer()
        {
            playerController.TargetingComponent.ClearMovementQueue();
            ChangeLeaderModel();
        }
        
        #region Create Other Player
        /// <summary>
        /// ATTENTION: 已改为异步，待观察是否有问题
        /// </summary>
        /// <param name="characterClass"></param>
        /// <param name="userid"></param>
        /// <param name="size"></param>
        public void CreateOtherPlayer(string characterClass, long userid, float size)
        {
            try
            {
                playerController .Gender = eGender.Male;
                playerController.CharacterModel = CharacterCatalog.Instance.GetModel(characterClass);
                string prefabName = playerController.CharacterModel.PrefabNameFromGenderMain(playerController.Gender);
                IDictionary partions = GetPartitionsData(userid);
                PoolModel.GetModelAsync(prefabName, mDMono.transform.position, Quaternion.identity, (o, prm) =>
            {
                var variantObj = o as GameObject;
                variantObj.transform.SetParent(mDMono.transform);
                variantObj.transform.localPosition = Vector3.zero;
                
                CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
                variant.InstantiateCharacter(partions);

                playerController.SkinnedRigPrefab = variant.CharacterInstance;
                if (playerController.SkinnedRigPrefab == null)
                {
                    EB.Debug.LogError("SkinnedRigPrefab is NULL");
                    return;
                }

                #region 其他玩家的一些设置After Create
                //SkinnedRigPrefab = PoolModel.GetNext(prefabName, transform.position, Quaternion.identity) as GameObject;
                //_skinnedRigPrefabInstance = Instantiate(EB.Assets.Load(prefabName), transform.position, Quaternion.identity) as GameObject;
                playerController.SkinnedRigPrefab.name = prefabName + "_Character";
                playerController.SkinnedRigPrefab.tag = "CharacterMesh";
                playerController.SkinnedRigPrefab.transform.SetParent(mDMono.transform);
                playerController.SkinnedRigPrefab.transform.localRotation = Quaternion.identity;
                playerController.SkinnedRigPrefab.transform.localScale = new Vector3(size, size, size);

                // only enable once we have our skinned rig prefab instance resolved
                _combatController = mDMono.GetComponent<CombatController>();
                _combatController.Initialize(playerController.SkinnedRigPrefab.transform, playerController.CharacterModel, 1 << LayerMask.NameToLayer("Enemy"));

                playerController.CharacterComponent = mDMono.GetComponent<CharacterComponent>();
                playerController.CharacterComponent.OnSpawn(playerController.CharacterModel, playerController.SkinnedRigPrefab, -1, false);
                //bool isUseCharacterRecord = (_viewRPC == null || _viewRPC.isMine);
                //CharacterRecord characterRecord = null;
                //NPC名字
                _HeadBar = playerController.SkinnedRigPrefab.GetMonoILRComponent<HeadBars2D>(false);
                if (_HeadBar == null)
                {
                    _HeadBar = playerController.SkinnedRigPrefab.AddMonoILRComponent<HeadBars2D>("Hotfix_LT.UI.HeadBars2D");
                }
                if (mDMono.GetComponent<Collider>() != null && mDMono.GetComponent<Collider>() is BoxCollider)
                {
                    BoxCollider box = mDMono.GetComponent<Collider>() as BoxCollider;
                    box.size = new Vector3(1.5f, 1.5f, 1.5f) + new Vector3(0, playerController.CharacterModel.heightOffset, 0);
                    box.center = new Vector3(0, box.size.y / 2.0f, 0);
                }
                //if (isUseCharacterRecord)
                //{
                //	characterRecord = CharacterManager.Instance.CurrentCharacter;
                //}
                _muzzle = GameUtils.SearchHierarchyForBone(playerController.SkinnedRigPrefab.transform, "muzzle");
                if (null == _muzzle)
                {
                    _muzzle = mDMono.transform;
                }

                //ToDo:看起来无用，暂时屏蔽
                //PlayMakerGlobals.Instance.Variables.FindFsmGameObject("Player" + (_initialPlayerIndex + 1)).Value =mDMono . gameObject;
                #endregion
            }, null);
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
        }
        #endregion

        private void UpdateMovementReticle(Vector3 movementTarget, bool isNull)
        {
            if (_moveReticle == null)
                return;

            if (isNull)
            {
                _moveReticle.gameObject.CustomSetActive(false);
                _moveReticleFX.OnPoolDeactivate();
            }
            else
            {
                _lastMoveUpdate = Time.time;
                if (_moveReticle.gameObject.activeSelf == false)
                {
                    _moveReticle.gameObject.CustomSetActive(true);
                    _moveReticleFX.OnPoolActivate();
                }
                _moveReticle.gameObject.transform.position = movementTarget;
            }
        }


        public bool IsTargetInCombatRange(GameObject target)
        {
            if (_combatController == null)
                return false;

            return _combatController.IsInCombatRange(target);
        }

    }
}
