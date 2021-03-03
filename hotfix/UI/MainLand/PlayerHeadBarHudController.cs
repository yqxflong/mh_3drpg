using UnityEngine;

namespace Hotfix_LT.UI
{
    public class PlayerHeadBarHudController : DynamicMonoHotfix
    {
        public GameObject HUDTemplate;
        public PlayerVigourTip vigourTip;
    
        public static PlayerHeadBarHudController Instance
        {
            get;
            private set;
        }

        public override void Awake()
        {
            Instance = this;
            vigourTip = mDMono.transform.parent.GetMonoILRComponent<PlayerVigourTip>("PlayerVigourTipUI");

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

        public override void OnDestroy()
        {
            hudsPool.Clear();
            Instance = null;
        }
    
        public PlayerHeadBarHud GetHUD()
        {
            return GetHUDFromPool();
        }
    
        #region Huds Pool
        private const int MAX_NUM_TASK = 30; // Will pre-pool this number of huds at initialization time 
    
        private EB.Collections.Stack<PlayerHeadBarHud> hudsPool = new EB.Collections.Stack<PlayerHeadBarHud>();
    
        private void InitHUDSPool()
        {
            for (int i = 0; i < MAX_NUM_TASK; i++)
            {
                PutHUDInPool(CreateHUD());
            }
        }

        private PlayerHeadBarHud GetHUDFromPool()
        {
            PlayerHeadBarHud hud = null;
    
            if (hudsPool.Count > 0)
            {
                hud = hudsPool.Pop();
                hud.mDMono.gameObject.CustomSetActive(true);
            }
            else
            {
                EB.Debug.Log("HUD pool length too small. GetHUDFromPool() called but pool is empty. New HUD instance created");
                hud = CreateHUD();
            }
            if (hud == null)
            {
                return null;
            }
            hud.recycleCallback = delegate ()
            {
                PutHUDInPool(hud);
            };
    
            return hud;
        }
    
        private void PutHUDInPool(PlayerHeadBarHud hud)
        {
            hud.mDMono.transform.SetParent(mDMono.transform);
            hud.mDMono.gameObject.CustomSetActive(false);
            hudsPool.Push(hud);
            hud.recycleCallback = null;
        }
    
        private PlayerHeadBarHud CreateHUD()
        {
            if (HUDTemplate == null)
            {
                EB.Debug.LogError("HUDTemplate is null");
                return null;
            }

            PlayerHeadBarHud hud = Object.Instantiate<GameObject>(HUDTemplate).GetMonoILRComponent<PlayerHeadBarHud>();
            hud.mDMono.transform.SetParent(mDMono.transform);
            hud.mDMono.transform.localScale = new Vector3(5000f,5000f,0f);
            return hud;
        }
        #endregion
    }
}
