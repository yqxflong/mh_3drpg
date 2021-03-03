using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class UIBuddy3DModelCreater : DynamicMonoHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            m_CharacterContainer = mDMono.transform.gameObject;
            modelScale = Vector3.one;
            needLowBright = false;
        }

        public GameObject m_CharacterContainer;
        public Vector3 modelScale;
        public bool needLowBright = false;

        public string CharacterVariantTemplate
        {
            get; set;
        }

        public GameObject character
        {
            get { return m_Character; }
        }

        public bool loading
        {
            get { return m_Loading; }
        }

        public bool idle
        {
            get { return !m_Loading && m_Character == null; }
        }

        public string variantName
        {
            get { return m_VariantName; }
        }

        public string variantPath
        {
            get { return string.Format("Bundles/Player/Variants/{0}", m_VariantName); }
        }

        public bool SyncLoad
        {
            get; set;
        }

        public AvatarComponent Avatar
        {
            get { return m_Avatar; }
        }

        protected bool m_Loading;
        protected string m_VariantName;
        protected GameObject m_Character;
        protected AvatarComponent m_Avatar;
        protected bool ShouldSetMipMapBias
		{
			get { return m_ShouldSetMipMapBias; }
			set
			{
				if (value) { RegisterMonoUpdater(); } else { ErasureMonoUpdater(); }
				if (value != m_ShouldSetMipMapBias) m_ShouldSetMipMapBias = value;
			}
		}
		private bool m_ShouldSetMipMapBias;

        protected IDictionary m_Partitions;
        /// <summary>
        /// 设置当前材质的协程
        /// </summary>
        private Coroutine m_Coroutine;

        public void CreatCharacter(IDictionary partitions)
        {
            if (m_Partitions != null && partitions != null)
            {
                foreach (DictionaryEntry entry in partitions)
                {
                    if (!m_Partitions.Contains(entry.Key) || !object.Equals(m_Partitions[entry.Key], entry.Value))
                    {
                        DestroyCharacter();
                        break;
                    }
                }
            }

            m_Partitions = partitions;
            CreatCharacter();
        }

        public void CreatCharacter()
        {
            //判断创建的模型资源是否已经被释放了
            IsHaveModel(CharacterVariantTemplate);
            //其他的模型设置不可见
            OtherInVisible(CharacterVariantTemplate);
            CreatCharacterAsync();
        }

        /// <summary>
        /// 将其他的非模型设置不可见
        /// </summary>
        /// <param name="CharacterVariantTemplate">模型名称</param>
        private void OtherInVisible(string CharacterVariantTemplate)
        {
            CharacterVariantTemplate = CharacterVariantTemplate.Split('-')[0];

            for (int i = 0; i < mDMono.transform.childCount; i++)
            {
                Transform trans = mDMono.transform.GetChild(i);
                string transName = trans.name.Split('-')[0];

                if (CharacterVariantTemplate.StartsWith("P"))
                {
                    trans.gameObject.CustomSetActive(transName.Equals(CharacterVariantTemplate));//伙伴不进行换色判断
                }
                else
                {
                    trans.gameObject.CustomSetActive(transName.Contains(CharacterVariantTemplate) || CharacterVariantTemplate.Contains(transName));//怪物换色时特殊处理
                }
            }
        }

        /// <summary>
        /// 这个角色变体是否被清除了
        /// </summary>
        /// <param name="CharacterVariantTemplate">模型名称</param>
        private void IsHaveModel(string CharacterVariantTemplate)
        {
            if (m_Character != null)
            {
                PoolModel.DestroyModel(m_Character);
            }

            m_Character = null;
            m_Loading = false;
        }

        private void CreatCharacterAsync()
        {
            if (character != null || m_Loading)
            {
                if (m_VariantName == CharacterVariantTemplate)
                {
                    EB.Debug.Log("already loaded variant {0}", CharacterVariantTemplate);
                    return;
                }
                else
                {
                    EB.Debug.Log("destroy last instance {0}", m_VariantName);
                    DestroyCharacter();
                }
            }

            if (string.IsNullOrEmpty(CharacterVariantTemplate)) return;

            m_Loading = true;
            m_VariantName = CharacterVariantTemplate;

            var listener = this;
            string variant_name = m_VariantName;
            string prefab_path = variantPath;
            PoolModel.GetModelAsync(prefab_path, Vector3.zero, Quaternion.identity, delegate (Object obj, object param)
            {
                GameObject variantObj = obj as GameObject;
                if (variantObj == null)
                {
                    EB.Debug.LogError("UIBuddy3DModelCreater.CreatCharacter: No Resources for {0}", prefab_path);
                    return;
                }

                if (listener == null)
                {
                    EB.Debug.LogWarning("UIBuddy3DModelCreater.CreatCharacter: creator already destroyed");
                    PoolModel.DestroyModel(variantObj);
                    return;
                }

                if (variant_name != m_VariantName)
                {
                    EB.Debug.LogWarning("UIBuddy3DModelCreater.CreatCharacter: resource not match");
                    PoolModel.DestroyModel(variantObj);
                    return;
                }

                if (m_Character != null)
                {
                    EB.Debug.LogWarning("UIBuddy3DModelCreater.CreatCharacter: character set");
                    PoolModel.DestroyModel(variantObj);
                    return;
                }

                if (!m_Loading)
                {
                    EB.Debug.LogWarning("UIBuddy3DModelCreater.CreatCharacter: load canceled");
                    PoolModel.DestroyModel(variantObj);
                    return;
                }

                // everything is ok
                m_Loading = false;

                variantObj.transform.SetParent(m_CharacterContainer.transform);
                CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();

                if (variant != null)
                {
                    variant.SyncLoad = true;

                    if (m_Partitions == null)
                    {
                        variant.InstantiateCharacter();
                    }
                    else
                    {
                        variant.InstantiateCharacter(m_Partitions);
                    }

                    m_Character = variant.CharacterInstance;
                }

                if (m_Character != null)
                {
                    m_Character.transform.SetParent(m_CharacterContainer.transform);
                    m_Character.transform.localScale = modelScale;
                    m_Character.transform.localRotation = Quaternion.identity;
                    m_Character.transform.localPosition = Vector3.zero;
                    SetObjLayer(m_Character, m_CharacterContainer.gameObject.layer);
                    //StartCoroutine(SetParticleScaleMode(m_Character));
                    m_Avatar = m_Character.GetComponent<AvatarComponent>();
                    SetNeedToTransitionToIdle(m_Character, true);
                    //StartCoroutine(ScaleOutLine(m_Avatar)); //by pj 异步 开启协程判断
                }

                ShouldSetMipMapBias = true;

                if (mDMono.gameObject.activeInHierarchy)// && HuDState.IsLTLegionFBHudOpen)
                {
                    if (m_Coroutine != null)
                    {
                        StopCoroutine(m_Coroutine);
                        m_Coroutine = null;
                    }
                    m_Coroutine = StartCoroutine(SetMaterial());
                }

            }, null);
        }

        private IEnumerator SetMaterial()
        {
            yield return null;
            //判断如果是乌龟就调整参数要不会出现效果不正的问题
            if (m_VariantName.Contains("M067"))
            {
                Renderer[] renderers = m_Character.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].gameObject.name.Contains("M067_Body"))
                    {
                        renderers[i].material.SetFloat("_SpecularPower", 17.0f);
                    }
                }
            }
            //如果是两大BOSS的话~就调Salse要不，太大了
            if (m_VariantName.Contains("M067") || m_VariantName.Contains("M025"))
            {
                mDMono.transform.localPosition = Vector3.zero;
                mDMono.transform.localScale = Vector3.one * 0.342f;
                mDMono.transform.localRotation = Quaternion.Euler(0, 2, 0);

                MoveController moveController = mDMono.gameObject.GetComponentInChildren<MoveController>();
                while (!moveController.IsInitialized)//等待MoveController初始化完成，不然会报错
                {
                    yield return null;
                }
                moveController.TransitionTo(MoveController.CombatantMoveState.kReady);
            }
        }

        private IEnumerator ScaleOutLine(AvatarComponent m_Avatar) //by pj 
        {
            while (m_Avatar != null && m_Avatar.GetToLoadCount() != 0)
            {
                yield return null;
            }

            if (m_Character != null)
            {
                CartoonHelp.ScaleCartoonOutLine(m_Character, 0.005f);
            }
        }

        IEnumerator SetParticleScaleMode(GameObject character)
        {
            yield return null;
            yield return null;
            ParticleSystem[] sys = character.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < sys.Length; i++)
            {
                if (sys[i] != null)
                {
                    var main = sys[i].main;
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                }
            }
        }

		//public override void OnEnable()
		//{
		//	RegisterMonoUpdater();
		//}

		public void DestroyCharacter()
        {
            if (m_Character != null)
            {
                if (!ShouldSetMipMapBias)
                {
                    UpdateMipMapBias(m_Avatar, 0.0f);
                }

                MoveEditor.FXHelper fxHelper = m_Character.GetComponent<MoveEditor.FXHelper>();
                if (fxHelper != null)
                {
                    fxHelper.StopAll(true);
                    //if (PSPoolManager.Instance != null)
                    //{
                    //    PSPoolManager.Instance.Update();
                    //}
                }

                CharacterVariant variant = m_CharacterContainer.GetComponentInChildren<CharacterVariant>();
                if (variant != null && GameEngine.Instance != null)
                {
                    SetObjLayer(variant.CharacterInstance, GameEngine.Instance.defaultLayer);
                    variant.Recycle();
                    PoolModel.DestroyModel(variant.gameObject);
                }

                m_Character = null;
            }
            /*
            if (!string.IsNullOrEmpty(m_VariantName))
            {
                if (PoolModel != null)
                {
                    PoolModel.RemoveResource(variantPath, false);
                }
    
                m_VariantName = null;
            }
            */
            m_Loading = false;
        }

        protected void SetNeedToTransitionToIdle(GameObject character, bool needToTransitionToIdle)
        {
            if (character != null)
            {
                PlayerEquipmentDataLookup lookup = character.GetDataLookupILRComponent<PlayerEquipmentDataLookup>("Hotfix_LT.UI.PlayerEquipmentDataLookup",false);
                if (lookup == null)
                {
                    MoveController mc = character.GetComponent<MoveController>();
                    if (mc != null && mc.GetMoveByState(MoveController.CombatantMoveState.kLobby) != null)
                    {
                        System.Action fn = () =>
                        {
                            lookup = character.AddDataLookupILRComponent<PlayerEquipmentDataLookup>("Hotfix_LT.UI.PlayerEquipmentDataLookup");
                            lookup.needToTransitionToIdle = needToTransitionToIdle;
                        };
                        if (!mc.IsInitialized)
                        {
                            mc.RegisterInitSuccCallBack(fn);
                        }
                        else
                        {
                            fn();
                        }

                    }
                }
                else
                {
                    lookup.needToTransitionToIdle = needToTransitionToIdle;
                }
            }
        }

        private bool mIsShow = false;
        public void ShowCharacter(bool show)
        {
            if (mIsShow == show)
            {
                return;
            }
            mIsShow = show;
            if (show)
            {
                m_CharacterContainer.CustomSetActive(true);
            }
            else
            {
                if (m_Character != null)
                {
                    MoveEditor.FXHelper fxHelper = m_Character.GetComponent<MoveEditor.FXHelper>();
                    if (fxHelper != null)
                    {
                        fxHelper.StopAll(true);
                        //if (PSPoolManager.Instance != null)
                        //{
                        //    PSPoolManager.Instance.Update();
                        //}
                    }
                }

                m_CharacterContainer.CustomSetActive(false);
            }
        }

        public override void OnDisable()
        {
            if (m_Character != null)
            {
                MoveEditor.FXHelper fxHelper = m_Character.GetComponent<MoveEditor.FXHelper>();
                if (fxHelper != null)
                {
                    fxHelper.StopAll(true);
                    //if (PSPoolManager.Instance != null)
                    //{
                    //    PSPoolManager.Instance.Update();
                    //}
                }
            }
			ErasureMonoUpdater();
        }

        public void Update()
        {
            if (ShouldSetMipMapBias && m_Avatar != null && m_Avatar.Ready)
            {
                UpdateMipMapBias(m_Avatar, -2.0f);

                ShouldSetMipMapBias = false;
            }
        }

        /// <summary>
        /// 特效在调整层级的时候，记得调整一下放缩模式，否则放缩的时候子物体特效会异常
        /// </summary>
        /// <param name="_obj"></param>
        /// <param name="_nLayer"></param>
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

        private void UpdateMipMapBias(AvatarComponent avatar, float mipMapBias)
        {
            if (m_Avatar != null)
            {
                var iter = m_Avatar.Partitions.GetEnumerator();
                while (iter.MoveNext())
                {
                    iter.Current.Value.UpdateMipMapBias(mipMapBias);
                }
                iter.Dispose();
            }
        }
    }

    public static class HuDState
    {
        /// <summary>
        /// 军团副本界面是否打开
        /// </summary>
        public static bool IsLTLegionFBHudOpen;
    }
}
