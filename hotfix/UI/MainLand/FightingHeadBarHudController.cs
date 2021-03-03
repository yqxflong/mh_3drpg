using UnityEngine;

namespace Hotfix_LT.UI
{
    public class FightingHeadBarHudController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();
            m_Container = mDMono.transform;
            Instance = this;

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0)
                {
                    HUDTemplate = (GameObject)mDMono.ObjectParamList[0];
                }
            }

            InitHUDSPool();
        }

        public GameObject HUDTemplate;
    	public Transform m_Container;

    	public static FightingHeadBarHudController Instance
        {
            get;
            private set;
        }

        public override void OnDestroy()
        {
            hudsPool.Clear();
            Instance = null;
        }
    
        public FightingHeadBarHud GetHUD()
        {
            return GetHUDFromPool();
        }
    
        #region Huds Pool
        private const int MAX_NUM_TASK = 30; // Will pre-pool this number of huds at initialization time 
    
        private EB.Collections.Stack<FightingHeadBarHud> hudsPool = new EB.Collections.Stack<FightingHeadBarHud>();
    
        private void InitHUDSPool()
        {
            for (int i = 0; i < MAX_NUM_TASK; i++)
            {
                PutHUDInPool(CreateHUD());
            }
        }
    
        private FightingHeadBarHud GetHUDFromPool()
        {
            FightingHeadBarHud hud = null;
    
            if (hudsPool.Count > 0)
            {
                hud = hudsPool.Pop();
                hud.mDMono.gameObject.SetActive(true);
            }
            else
            {
                EB.Debug.Log("HUD pool length too small. GetHUDFromPool() called but pool is empty. New HUD instance created");
                hud = CreateHUD();
            }
    
            hud.recycleCallback = delegate ()
            {
                PutHUDInPool(hud);
            };
    
            return hud;
        }
    
        private void PutHUDInPool(FightingHeadBarHud hud)
        {
            hud.mDMono.transform.SetParent(m_Container);
            hud.mDMono.gameObject.SetActive(false);
            hudsPool.Push(hud);
            hud.recycleCallback = null;
        }
    
        private FightingHeadBarHud CreateHUD()
        {
            if (HUDTemplate == null)
            {
                EB.Debug.LogError("HUDTemplate is null");
                return null;
            }

            FightingHeadBarHud hud = Object.Instantiate<GameObject>(HUDTemplate).GetMonoILRComponent<FightingHeadBarHud>();
            hud.mDMono.transform.SetParent(m_Container);
            hud.mDMono.transform.localScale = new Vector3(1f, 1f, 0f);
            return hud;
        }
        #endregion
    }
}
