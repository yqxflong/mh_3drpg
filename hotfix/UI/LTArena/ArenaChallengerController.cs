using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class ArenaChallengerController : DynamicMonoHotfix, IHotfixUpdate
    {
        public UILabel nameLabel;
        public UILabel rankLabel;
        public UISprite rankTopFlag;
        public UILabel flagLabel;
        public UIButton startButton;
        public GameObject FxObj;
        private int Index = 0;

        public string DataId
        {
            get; private set;
        }

        public ArenaChallenger Challenger
        {
            get; private set;
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public override void Awake()
        {
            base.Awake();

            nameLabel = mDMono.transform.Find("ChallengerName").GetComponent<UILabel>();
            rankLabel = mDMono.transform.Find("RankStr/ChallengerRank").GetComponent<UILabel>();
            rankTopFlag = mDMono.transform.Find("RankFlag").GetComponent<UISprite>();
            flagLabel = mDMono.transform.Find("RankFlag/FlagRankLabel").GetComponent<UILabel>();
            startButton = mDMono.transform.Find("ChallengeBtn").GetComponent<UIButton>();
            FxObj = mDMono.transform.Find("Fx").gameObject;
            ModelPosition = new Vector3(0, -150, -500);
            ModelScale = new Vector3(230, 230, 230);
            ModelRotation = new Vector3(0, 180, 0);
        }

        public void Fill(ArenaChallenger challenger)
        {
            Challenger = challenger;

            int realRank = challenger.rank + 1;
            LTUIUtil.SetText(rankLabel, string.Format("{0}", realRank));
            LTUIUtil.SetText(nameLabel, challenger.name);
            if (realRank == 1)
            {
                rankTopFlag.spriteName = "Arena_Flag_1";
                flagLabel.text = "1";
            }
            else if (realRank == 2)
            {
                rankTopFlag.spriteName = "Arena_Flag_2";
                flagLabel.text = "2";
            }
            else if (realRank == 3)
            {
                rankTopFlag.spriteName = "Arena_Flag_3";
                flagLabel.text = "3";
            }
            else if (realRank >= 4 && realRank <= 10)
            {
                rankTopFlag.spriteName = "Arena_Flag_4";
                flagLabel.text = "";
            }
            else
            {
                rankTopFlag.spriteName = "Arena_Flag_5";
                flagLabel.text = "";
            }
            startButton.isEnabled = true;

        }

        public void Clean()
        {
            if (!string.IsNullOrEmpty(DataId))
            {
                GameDataSparxManager.Instance.UnRegisterListener(DataId, OnChallengerListener);
                DataId = string.Empty;
            }

            nameLabel.text = string.Empty;
            rankLabel.text = string.Empty;
            rankTopFlag.spriteName = string.Empty;
            startButton.isEnabled = false;
            ModelName = string.Empty;

            if (ModelGO != null)
            {
                PoolModel.DestroyModel(ModelGO);
                ModelGO = null;
            }
        }

        public void Register(string dataId, int index)
        {
            if (DataId == dataId)
            {
                return;
            }
            else if (!string.IsNullOrEmpty(DataId))
            {
                Clean();
            }

            DataId = dataId;
            Index = index;
            GameDataSparxManager.Instance.RegisterListener(DataId, OnChallengerListener);
        }

        private void OnChallengerListener(string dataId, INodeData data)
        {
            ArenaChallenger challenger = data as ArenaChallenger;

            Fill(challenger);

            Hotfix_LT.Data.HeroInfoTemplate info = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(challenger.charId, challenger.skin);
            StartCoroutine(CreateModelCoroutine(info.model_name));
        }

        public void ActiveModel(bool isActive)
        {
            if (ModelGO != null)
                ModelGO.SetActive(isActive);
        }

        public Vector3 ModelPosition = new Vector3(0, 100, 0);
        public Vector3 ModelScale = new Vector3(230, 230, 230);
        public Vector3 ModelRotation = new Vector3(0, 0, 0);
        private GameObject ModelGO;
        private string ModelName;
        private IEnumerator CreateModelCoroutine(string modelName)
        {
            yield return new WaitForSeconds(0.3f * Index);
            FxObj.SetActive(false);
            FusionAudio.PostEvent("SFX/General/CharacterSpawn", true);
            FxObj.SetActive(true);
            if (modelName == ModelName && ModelGO != null)
            {

                InitModel(ModelGO);
            }
            if (string.IsNullOrEmpty(modelName) || modelName == ModelName)
            {
                yield break;
            }
            if (!string.IsNullOrEmpty(ModelName))
            {
                PoolModel.DestroyModel(ModelGO);
            }
            ModelName = modelName;
            string prefab_path = "Bundles/Player/Variants/" + modelName + "-I";

            string variant_name = modelName;

            GameObject variantObj = null;
            var listener = this;

            Coroutine coroutine = PoolModel.GetModelAsync(prefab_path, Vector3.zero, Quaternion.identity, delegate (Object obj, object param)
            {
                variantObj = obj as GameObject;
                ModelGO = variantObj;
                if (variantObj == null)
                {
                    return;
                }

                if (listener == null)
                {
                    EB.Debug.LogError("listener == null");
                    PoolModel.DestroyModel(variantObj);
                    return;
                }

                if (variant_name != ModelName)
                {
                    EB.Debug.LogError("variant_name != ModelName");
                    PoolModel.DestroyModel(variantObj);
                    return;
                }

                InitModel((GameObject)obj);
            }, null);

            yield return coroutine;
            if (variantObj != null)SetObjLayer(variantObj, 28);
        }

        private void InitModel(GameObject variantObj)
        {
            variantObj.transform.SetParent(mDMono.transform, false);
            variantObj.transform.localScale = Vector3.one;
            variantObj.transform.localRotation = Quaternion.identity;
            CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
            variant.InstantiateCharacter();

            GameObject character = variant.CharacterInstance;
            character.transform.parent = variant.transform;
            character.transform.localScale = ModelScale;
            character.transform.localRotation = Quaternion.Euler(ModelRotation);
            character.transform.localPosition = ModelPosition;

            MoveController mc = character.GetComponent<MoveController>();
            if (mc != null)
            {
                mc.enabled = false;
                mc.enabled = true;
                System.Action fn = ()=>{
                    mc.TransitionTo(MoveController.CombatantMoveState.kEntry);
                    needToTransitionToIdle = true;
                };
                if(!mc.IsInitialized){
                    mc.RegisterInitSuccCallBack(fn);
                }
                else{
                    fn();
                }
            }
            StartCoroutine(SetParticleOrder(character));
            SetObjLayer(variantObj, 28);
        }

        IEnumerator SetParticleOrder(GameObject character)
        {
            yield return null;
            UIPanel up = mDMono.GetComponentInParent<UIPanel>();
            if (character!=null)
            {
                ParticleSystem[] sys = character.GetComponentsInChildren<ParticleSystem>(true);
                if (sys!=null)
                {
                    for (int i = 0; i < sys.Length; i++)
                    {
                        if (sys[i] != null && sys[i].GetComponent<Renderer>() != null)
                        {
                            Renderer[] rens = sys[i].GetComponentsInChildren<Renderer>(true);
                            for (int j = 0; j < rens.Length; j++)//此处是为了防止粒子特效嵌套mesh by hzh
                            {
                                rens[j].sortingOrder = up.sortingOrder + 1;
                            }
                        }
                    } 
                }
            }
          
        }

        public void SetObjLayer(GameObject obj, int layer)
        {
            obj.transform.SetChildLayer(layer);
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].gameObject.layer = layer;
                Renderer render = renderers[i];
                Material[] materials = render.materials;
                for (int j = 0; j < materials.Length; j++)
                {
                    Material ImageMaterial = new Material(materials[j]);
                    ImageMaterial.SetColor("_RimColor", new Color(0, 0, 0, 1f));
                    materials[j] = ImageMaterial;
                }
                render.materials = materials;
            }
        }

        public void SetParticleLayer()
        {
            if (ModelGO != null) StartCoroutine(SetParticleOrder(ModelGO));
        }

        private bool needToTransitionToIdle = false;

        public void Update()
        {
            // base.Update();

            if (needToTransitionToIdle)
            {
                if (ModelGO != null && ModelGO.GetComponent<CharacterVariant>().CharacterInstance != null)
                {
                    MoveController moveController = ModelGO.GetComponent<CharacterVariant>().CharacterInstance.GetComponent<MoveController>();
                    AnimatorStateInfo asi = moveController.GetCurrentStateInfo();

                    if (asi.normalizedTime >= 1)
                    {
                        MoveEditor.Move theMove = moveController.GetMoveIfExists("Idle");
                        moveController.TransitionTo(MoveController.CombatantMoveState.kIdle);
                        moveController.m_lobby_hash = Animator.StringToHash("Lobby." + theMove.name);
                        moveController.SetMove(theMove);
                        moveController.CrossFade(MoveController.m_idle_hash, 0.2f, 0, 0f);
                        needToTransitionToIdle = false;
                    }
                }
            }
        }
    }

}