using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTWorldBossMainMenuCtrl : DynamicMonoHotfix
    {
        private static LTWorldBossMainMenuCtrl m_Instance;
        public static LTWorldBossMainMenuCtrl Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    EB.Debug.LogWarning("LTWorldBossMainMenuCtrl is not Init");
                }
                return m_Instance;
            }
        }
    
        public bool isOpen;
        public List<GameObject> BossAreaList;
        public List<GameObject> MainLandAreaList;
        public Transform WorldBossRankUITran;
        public Transform FlagAreaTran;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            var root = t.parent.parent.parent;
            isOpen = false;

            BossAreaList = new List<GameObject>();
            BossAreaList.Add(t.gameObject);
            BossAreaList.Add(root.FindEx("Edge/Right/WorldBossRank/LTWorldBossRankView").gameObject);

            MainLandAreaList = new List<GameObject>();
            MainLandAreaList.Add(root.FindEx("Center/SpeedSnatch").gameObject);
            MainLandAreaList.Add(root.FindEx("Edge/TopLeftAnchor/TopLeft").gameObject);
            MainLandAreaList.Add(root.FindEx("Edge/Right/RightFuncPanel/Content").gameObject);
            MainLandAreaList.Add(root.FindEx("Edge/TopRightAnchor/TopRight/FuncList").gameObject);
            MainLandAreaList.Add(root.FindEx("Edge/TopRightAnchor/TopRight/FuncList2").gameObject);
            MainLandAreaList.Add(root.FindEx("Edge/TopRightAnchor/TopRight/Btn").gameObject);

            WorldBossRankUITran = root.GetComponent<Transform>("Edge/Right/WorldBossRank/LTWorldBossRankView/Rank");
            FlagAreaTran = root.GetComponent<Transform>("Edge/TopRightAnchor/TopRight/FlagArea");

            Messenger.AddListener(Hotfix_LT.EventName.BossDieEvent,OnBossDieFunc);
            HideBossUI();

            m_Instance = this;
        }

        public override void OnDestroy()
        {
            m_Instance = null;
          Messenger.RemoveListener(Hotfix_LT.EventName.BossDieEvent,OnBossDieFunc);
        }
    
        public void ShowBossUI()
        {
            if (LTWorldBossDataManager.Instance.IsWorldBossStart())
            {
                for(int i=0;i< BossAreaList.Count; ++i)
                {
                    BossAreaList[i].CustomSetActive(true);
                }
                for (int i = 0; i < MainLandAreaList.Count; ++i)
                {
                    MainLandAreaList[i].CustomSetActive(false);
                }
    
                FlagAreaTran.gameObject.CustomSetActive(false);
                SetWorldBossFx(false);
                isOpen = true;
            }
    
        }
    
        public void HideBossUI()
        {
            for (int i = 0; i < BossAreaList.Count; ++i)
            {
                BossAreaList[i].CustomSetActive(false);
            }
            for (int i = 0; i < MainLandAreaList.Count; ++i)
            {
                MainLandAreaList[i].CustomSetActive(true);
            }
    
            WorldBossRankUITran.gameObject.CustomSetActive(false);
    
            Hotfix_LT.Data.FuncTemplate FlagAreaft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10074);
            FlagAreaTran.gameObject.CustomSetActive((FlagAreaft != null && FlagAreaft.IsConditionOK() || FlagAreaft == null));
            
            isOpen = false;

            if (LTMainMenuHudController .Instance != null)
            {
                LTMainMenuHudController.Instance.UpdateTopFunc();
            }
        }
    
        private ParticleSystem[] mFx;
        private void SetWorldBossFx(bool isShow)
        {
            if (mFx == null)
            {
                EnemyController ec = MainLandLogic.GetInstance().GetEnemyController("EnemySpawns_11");
                if (ec == null)
                {
                    EB.Debug.Log("LTWorldBossMainMenuCtrl SetWorldBOssFx ec is Null!!!");
                    return;
                }
                mFx = ec.transform.GetComponentsInChildren<ParticleSystem>();
            }
    
            if (mFx != null && mFx.Length > 0 && mFx[0] != null)
            {
                if ((!mFx[0].gameObject.activeInHierarchy && isShow) || (mFx[0].gameObject.activeInHierarchy && !isShow))
                {
                    for (int i = 0; i < mFx.Length; i++)
                    {
                        mFx[i].gameObject.CustomSetActive(isShow);
                        if (isShow)
                        {
                            mFx[i].Play();
                        }
                    }
                }
            }
        }
    
        private void OnBossDieFunc()
        {
            HideBossUI();
            PlayerController.onCollisionExit("EnemySpawns_11");
        }
        
    }
}
