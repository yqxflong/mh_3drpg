using Hotfix_LT.Data;
using Hotfix_LT.UI;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
using Debug = EB.Debug;

namespace Hotfix_LT.Player
{
    public class EnemyHotfixController : DynamicMonoHotfix, IHotfixUpdate
    {
        /// <summary>储存npc创建时的原预设名，确保可以被object回收释放</summary>
        public string ObjectName;

        private Renderer _renderer = null;
        private EnemyController enemyController;
        private Material _originalMaterial = null;
        private Material _deathMaterial = null;
        private List<Material> MaterialList = null;
        private HeadBars2D _HeadBar = null;
        private Vector3 firstPos;
        private float timer;
        private float posDistrance;
        private float posTime;

//        private PlayMakerFSM _fsm;
        private ConditionComponent _conditionComponent;
        private GameObject _bestTarget;
        private NetworkOwnershipComponent _network = null;

        private MonoBehaviour[] monoArray;

        private int _attr;
        public int Attr
        {
            get
            {
                return _attr;
            }
            set
            {
                _attr = value;
            }
        }
        private string _role;
        public string Role
        {
            get
            {
                return _role;
            }
            set
            {
                _role = value;
                if (_role == UI.NPC_ROLE.WORLD_BOSS)
                {
                    Collider collider = enemyController.GetComponent<Collider>();
                    if (collider != null && collider is BoxCollider)
                    {
                        BoxCollider box = collider as BoxCollider;
                        box.size = new Vector3(4f, 5f, 1.75f) + new Vector3(0, enemyController.CharacterModel.heightOffset, 0);
                        box.center = new Vector3(0, box.size.y / 2.0f, -1);
                    }
                    float m_scale = NewGameConfigTemplateManager.Instance.GetGameConfigValue("WorldBossModleScale");
                    m_scale = m_scale == 0 ? 2 : m_scale;
                    enemyController.SkinnedRigPrefab.transform.localScale = Vector3.one * m_scale;
                }
                else if (_role == UI.NPC_ROLE.ARENA_MODLE)
                {
                    enemyController.gameObject.layer = LayerMask.NameToLayer("Interactive");
                    float m_scale = NewGameConfigTemplateManager.Instance.GetGameConfigValue("ArenaModleScale"); 
                    m_scale = m_scale == 0 ? 2.5f : m_scale;
                    enemyController.SkinnedRigPrefab.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
                }
            }
        }

        public override void Awake()
        {
            base.Awake();

            enemyController = mDMono.transform.GetComponent<EnemyController>();
//            _fsm = enemyController.GetComponent<PlayMakerFSM>();
            _network = enemyController.GetComponent<NetworkOwnershipComponent>();
            enemyController.TargetingComponent = enemyController.GetComponent<CharacterTargetingComponent>();
            _conditionComponent = enemyController.GetComponent<ConditionComponent>();
        }

        public override void OnEnable()
        {
            // EnemyManager.sEnemyControllers.Add(enemyController);

            //base.OnEnable();
			RegisterMonoUpdater();

			timer = 20;
            posDistrance = NewGameConfigTemplateManager.Instance.GetGameConfigValue("GhostMoveDistance"); 
            posTime = NewGameConfigTemplateManager.Instance.GetGameConfigValue("GhostMoveTime");
        }
        public void Update()
        {
            if (!GameEngine.Instance.IsTimeToRootScene)
            {
                return;
            }
            if (firstPos == Vector3.zero)
            {
                firstPos = mDMono.transform.position;
                posDistrance = (posDistrance == 0) ? 5 : posDistrance;
                posTime = (posTime == 0) ? 5 : posTime;
            }
            if (_role == UI.NPC_ROLE.GHOST)
            {

                timer += Time.deltaTime;
                if (timer >= posTime)
                {
                    timer = 0;
                    Vector2 p = UnityEngine.Random.insideUnitCircle * 5f;
                    MoveToPosition(new Vector3(firstPos.x + p.x, firstPos.y, firstPos.z + p.y));

                }
            }
        }

        public override void OnDisable()
        {
            ErasureMonoUpdater();
            // EnemyManager.sEnemyControllers.Remove(enemyController );
            base.OnDisable();
        }

        public override void OnDestroy()
        {
            if (_originalMaterial != null)
            {
                _originalMaterial = null;
            }

            if (_deathMaterial != null)
            {
                GameObject.Destroy(_deathMaterial);
                _deathMaterial = null;
            }
            if (_HeadBar != null) _HeadBar.SetBarHudState(eHeadBarHud.PlayerHeadBarHud, null, false);
        }

        private void MoveToPosition(Vector3 position)
        {
            if (mDMono != null && !NetworkOwnershipComponent.IsGameObjectLocallyOwned(mDMono.gameObject, _network))
            {
                return;
            }

            if (enemyController != null && enemyController.TargetingComponent != null)
            {
                enemyController.TargetingComponent.SetMovementTargetNoRPC(position);
            }
        }

        public void SetBarHudState(eHeadBarHud hudtype, Hashtable data, bool state)
        {
            if (_HeadBar != null) _HeadBar.SetBarHudState(hudtype, data, state);
        }

        public void Destroy()
        {
            ResetModel();
            CleanupMaterials();

            var helpers = mDMono.GetComponentsInChildren<MoveEditor.FXHelper>();
            for(int i=0; i < helpers.Length; ++i)
            {
                helpers[i].StopAll(true);
            }
            if (_HeadBar != null)
            {
                _HeadBar.ClearBars();
            }

            CharacterVariant variant = mDMono.GetComponentInChildren<CharacterVariant>();
            if (variant != null)
            {
                variant.Recycle();
                PoolModel.DestroyModel(variant.gameObject);
            }

            GameObject.Destroy(mDMono.gameObject);

            if (enemyController.onDestroy != null)
            {
                enemyController.onDestroy(enemyController);
            }
        }
        
        public void ChangeArenaModel(string Tid, int skin = 0, bool ChangeModel = false)
        {
            try
            {
                if (ChangeModel)
                {
                    ResetModel();
                    _HeadBar = null;
                    CharacterVariant CV = mDMono.transform.GetComponentInChildren<CharacterVariant>();
                    if (CV != null && GameEngine.Instance != null)
                    {
                        SetObjLayer(CV.CharacterInstance, GameEngine.Instance.defaultLayer);
                        CV.Recycle();
                        PoolModel.DestroyModel(CV.gameObject);
                    }
                    MoveEditor.FXHelper FH = mDMono.transform.GetComponentInChildren<MoveEditor.FXHelper>();
                    if (FH != null)
                    {
                        FH.StopAll(true);
                    }
                    if (enemyController.SkinnedRigPrefab != null)
                    {
                        PoolModel.DestroyModel(enemyController.SkinnedRigPrefab);
                    }

                    string classname = null;
                    string characterid = CharacterTemplateManager.Instance.TemplateidToCharacterid(Tid);
                    var charTpl = CharacterTemplateManager.Instance.GetHeroInfo(characterid, skin);
                    classname = charTpl.model_name;//需添加皮肤
                    if (string.IsNullOrEmpty(classname))
                    {
                        EB.Debug.LogError("OnViewIdAllocated: classname not found for tid = {0}", Tid);
                        return;
                    }
                     enemyController.CharacterModel = CharacterCatalog.Instance.GetModel(classname);

                    OnSpawnFun();

                    Role = _role;
                    SceneLogic scene = MainLandLogic.GetInstance();
                    string sceneName = scene.CurrentSceneName;
                    SetNpcName(sceneName);
                }
            }
            catch (System.Exception e)
            {
                EB.Debug.LogError(e.StackTrace);
            }
        }

        private IEnumerator SetModelWithGoldBodyCoroutine()
        {
            Transform body = Body();
            Animator animator = body.GetComponent<Animator>();
            MoveController moveController = body.GetComponent<MoveController>();

            // 防止雕像创建特效
            var fxHelper = body.GetComponent<MoveEditor.FXHelper>();

            if (fxHelper != null)
            {
                fxHelper.DisableFX = true;
                fxHelper.StopAll(true);
            }

            var characterVariant = body.parent.GetComponentInChildren<CharacterVariant>();
            Renderer[] renderers = body.GetComponentsInChildren<SkinnedMeshRenderer>();
            bool isModelReady = false;

            while (!isModelReady)
            {
                yield return null;

                if (renderers.Length >= characterVariant.Partitions.Count)
                {
                    isModelReady = true;
                }
            }

            MaterialList = new List<Material>();

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer render = renderers[i];
                Material[] materials = render.materials;
                MaterialList.AddRange(render.materials);
                Material goldMat = materials[0];
                for (int j = 0; j < materials.Length; j++)
                {
                    Material GoldMaterial = new Material(goldMat);
                    GoldMaterial.SetColor("_FinalColor", new Color(1f, 178 / 255f, 0f, 184 / 255f));
                    GoldMaterial.SetFloat("_ContrastIntansity", 0.95f);
                    GoldMaterial.SetFloat("_Brightness", 0.01f);
                    GoldMaterial.SetFloat("_GrayScale", 0.15f);
                    GoldMaterial.SetFloat("_SpecularPower", 1f);
                    GoldMaterial.SetFloat("_SpecularFresnel", 1f);
                    GoldMaterial.SetFloat("_Outline", 0f);
                    GoldMaterial.SetFloat("_ToonThreshold", 0.827f);
                    GoldMaterial.SetFloat("_RimPower", 8f);
                    GoldMaterial.SetFloat("_SpecularPower", 8f);
                    GoldMaterial.SetFloat("_SpecularFresnel", 1f);
                    GoldMaterial.SetFloat("_LightScale", 1.4f);
                    GoldMaterial.SetFloat("_UseMatCap", 0.0f);

                    GoldMaterial.EnableKeyword("EBG_COLORFILTER_ON");
                    GoldMaterial.EnableKeyword("EBG_SPECULAR_MAP_ON");
                    //GoldMaterial.SetInt("EBG_COLORFILTER",1);
                    //GoldMaterial.SetInt("EBG_SPECULAR_MAP",1);
                    materials[j] = GoldMaterial;
                }
                render.materials = materials;
            }

            while (moveController.CurrentState != MoveController.CombatantMoveState.kIdle)
            {
                EB.Debug.LogError(moveController.CurrentMove._animationClip.name);
                moveController.SetMove("Idle");
            }

            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            StartCoroutine(SetAnimatorFix(animator));

            // 已经在上面处理了雕像创建特效，以下代码不再使用
            //ParticleSystem[] ps = moveController.GetComponentsInChildren<ParticleSystem>(true);

            //while (ps.Length == 0)
            //{
            //    yield return new WaitForSeconds(0.1f);
            //    ps = moveController.GetComponentsInChildren<ParticleSystem>(true);
            //    yield return new WaitForSeconds(0.1f);
            //}

            //for (int i = 0; i < ps.Length; i++)
            //{
            //    ps[i].StopAll(true);
            //}
        }

        /// <summary>设置模型为金身状态</summary>
        public void SetModelWithGoldBody()
        {
            StartCoroutine(SetModelWithGoldBodyCoroutine());
        }

        /// <summary>还原金身状态，由回收池调用</summary>
        public void ResetModel()
        {
            if (MaterialList == null) return;
            int index = 0;
            Transform body = Body();
            Animator animator = body.GetComponent<Animator>();
            MoveController moveController = body.GetComponent<MoveController>();
            Renderer[] renderers = body.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer render = renderers[i];
                Material[] materials = render.materials;
                Material goldMat = materials[0];
                for (int j = 0; j < materials.Length; j++)
                {
                    Material GoldMaterial = MaterialList[index];
                    index++;
                    materials[j] = GoldMaterial;
                }
                render.materials = materials;
            }

            MaterialList = null;
            animator.enabled = true;
            animator.Play("Idle");

            ParticleSystem[] ps = moveController.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].Play();
            }
        }

        private IEnumerator SetAnimatorFix(Animator animator)
        {
            while (animator.enabled)
            {
                yield return new WaitForEndOfFrame();
                animator.Play("Idle");
                animator.enabled = false;
            }
        }

        private void OnSpawnFun(int spawnAnimationIndex = -1, bool playEffect = false, System.Action callback = null)
        {
            enemyController.SpawnLocation = mDMono .transform.position;
            EB.Assets.LoadAsync(enemyController.CharacterModel.ResourcePrefabNameMain,typeof(GameObject), (obj) =>
            {
                GameObject variantObj=GameObject.Instantiate(obj, mDMono.transform.position, Quaternion.identity) as GameObject;
                variantObj.transform.parent = mDMono.transform;
                CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
                variant.InstantiateCharacter();
                enemyController.SkinnedRigPrefab = variant.CharacterInstance;
                enemyController.SkinnedRigPrefab.name = enemyController.CharacterModel.ResourcePrefabNameMain + "_Character";
                enemyController.SkinnedRigPrefab.tag = "CharacterMesh";
                enemyController.SkinnedRigPrefab.transform.parent = mDMono.transform;
                enemyController.SkinnedRigPrefab.transform.localRotation = Quaternion.identity;
                enemyController.SkinnedRigPrefab.transform.localPosition = Vector3.zero;
                _renderer = enemyController.SkinnedRigPrefab.GetComponentInChildren<Renderer>();
              
                // Create our death material just once
                if (enemyController.CharacterModel.resourceDirectory == eResourceDirectory.Enemies)
                {
                    if (!_renderer.sharedMaterial.shader.name.EndsWith("-Alpha"))
                    {
                        string alphaShaderName = _renderer.sharedMaterial.shader.name + "-Alpha";
                        Shader alphaShader = Shader.Find(alphaShaderName);
                        if (alphaShader != null)
                        {
                            // Cache the original, unmodified material
                            _originalMaterial = _renderer.sharedMaterial;

                            // Cache a material copy (this will make a new instance of the shader material)
                            _deathMaterial = _renderer.material;

                            _renderer.material = _originalMaterial;

                            // Set the new shader and keywords on the material copy
                            _deathMaterial.shader = alphaShader;
                            _deathMaterial.shaderKeywords = new string[] { "ALPHA_ON" };
                            _deathMaterial.shaderKeywords = new string[] { "DEATH_ON" };
                        }
                    }
                    else
                    {
                        DebugSystem.Log("Material's shader is set to -Alpha version.\nMaterial's shader may not be set correctly in source or not set back to original value from pool.{0}EnemyController{1}", enemyController.CharacterModel.prefabName,LogType.Warning);
                    }
                }
              
                CombatController combatController = mDMono.GetComponent<CombatController>();
                combatController.Initialize(enemyController.SkinnedRigPrefab.transform, enemyController.CharacterModel, 1 << LayerMask.NameToLayer("Player"));
                enemyController.CharacterComponent.OnSpawn(enemyController.CharacterModel, enemyController.SkinnedRigPrefab, spawnAnimationIndex, playEffect);

                //回收后再打开组件检测
                if (enemyController.SkinnedRigPrefab != null)
                {
                    monoArray = enemyController.SkinnedRigPrefab.GetComponents<MonoBehaviour>();
                    for (int i = 0; i < monoArray.Length; i++)
                    {
                        if (monoArray[i] != null && !monoArray[i].enabled)
                        {
                            monoArray[i].enabled = true;
                        }
                    }
                    Animator mAnimator = enemyController.SkinnedRigPrefab.GetComponent<Animator>();
                    if (mAnimator != null && !mAnimator.enabled)
                    {
                        mAnimator.enabled = true;
                    }
                }

                //NPC名字
                _HeadBar = enemyController.SkinnedRigPrefab.GetMonoILRComponent<HeadBars2D>(false);
                if (_HeadBar == null && enemyController.SkinnedRigPrefab.transform.parent.name.CompareTo("EnemySpawns_11") != 0) //主城的世界Boss不需要创建
                {
                    _HeadBar = enemyController.SkinnedRigPrefab.AddMonoILRComponent<HeadBars2D>("Hotfix_LT.UI.HeadBars2D");
                }
//                if (_fsm != null)
//                {
//                    _fsm.Fsm.Stop();
//                    _fsm.Reset();
//                    _fsm.SetFsmTemplate(enemyController.CharacterModel.AITrigger);
//                    _fsm.Fsm.Start();
//                }

                _bestTarget = null;

                if (enemyController.CharacterModel.team == eTeamId.Interactable)
                {
                    enemyController.CharacterComponent.UseAdvancedLocomotionAnimation = true;
                }
                else
                {
                    enemyController.CharacterComponent.UseAdvancedLocomotionAnimation = false;
                }

                callback?.Invoke();
            });
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

        public void SetNpcName(string CurrentSceneName)
        {
            string npcname = SceneTemplateManager.GetNPCName(CurrentSceneName, _role, mDMono.gameObject.name); 
            Hashtable namedata = Johny.HashtablePool.Claim();
            namedata.Add("Name", npcname.Replace("/n", "\n"));
            namedata.Add("Role", _role);
            namedata.Add("Attr", Attr);

            if (_HeadBar != null) _HeadBar.SetBarHudState(eHeadBarHud.PlayerHeadBarHud, namedata, true);

            Johny.HashtablePool.Release(namedata);namedata=null;
        }

        private void CleanupMaterials()
        {
            if (_originalMaterial != null)
            {
                _renderer.sharedMaterial = _originalMaterial;
                _originalMaterial = null;
            }

            if (_deathMaterial != null)
            {
                _deathMaterial.shaderKeywords = new string[0];
                _deathMaterial.SetFloat("_ClipFactor", 0.0f);

                GameObject.Destroy(_deathMaterial);
                _deathMaterial = null;
            }
        }

        public void DoAppearingWay()
        {
            StartParticle();
            AppearBody();
        }

        public void StartParticle()
        {
            Vector3 dest = new Vector3();
            dest.x =mDMono.gameObject.transform.localPosition.x;
            dest.y = mDMono.gameObject.transform.localPosition.y;
            dest.z = mDMono.gameObject.transform.localPosition.z;
            ParticleSystem ps = PSPoolManager.Instance.Use(enemyController, "fx_RewardChest");
            if (ps != null)
            {
                ps.transform.position = dest;
                ps.Play();
            }
        }

        public void AppearBody()
        {
            Body().gameObject.SetActive(true);

//            if (_fsm != null)
//            {
//                _fsm.Fsm.Start();
//            }
        }

        public void DisAppearBody()
        {
            Body().gameObject.SetActive(false);

//            if (_fsm != null)
//            {
//                _fsm.Fsm.Stop();
//            }
        }

        private Transform Body()
        {
            return mDMono.gameObject.GetComponentInChildren<Animator>().transform;
        }

        public bool IsBodyActive()
        {

            return Body().gameObject.activeSelf;
        }
        
        public virtual void OnSpawn(CharacterModel characterModel, int spawnAnimationIndex, bool playEffect, System.Action callback)
        {
            try
            {
                enemyController.CharacterModel = characterModel;
                OnSpawnFun(spawnAnimationIndex, playEffect, callback);
            }
            catch (System.Exception e)
            {
                EB.Debug.LogError(e);
            }
        }

    }
}