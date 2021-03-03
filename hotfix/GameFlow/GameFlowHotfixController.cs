using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class GameFlowHotfixController : DynamicMonoHotfix, IHotfixLateUpdate
    {
        public static string ActiveStateName;
        public static string PreviousActiveStateName;

        /// <summary>自行判空 </summary>
        public static GameFlowHotfixController Instance = null;

        private List<FsmStateUnit> FsmList;

        public override void Awake()
        {
            EB.Debug.Log("GameFlowHotfixController.Awake!");
            base.Awake();
            FsmList = new List<FsmStateUnit>();
            FsmList.Add(new IdleFlowAction());
            FsmList.Add(new MainLandViewAction());
            FsmList.Add(new InstanceViewAction());
            FsmList.Add(new CombatViewAction());
            for(int i=0;i<FsmList.Count; ++i)
            {
                FsmList[i].OnAwake();
            }

            if (Instance != null)
            {
                EB.Debug.LogError("GameFlowHotfixController.Instance != null!Has Init!");
                if (Instance.mDMono != null)
                {
                    EB.Debug.LogError("Destroy GameFlowHotfixController.Instance.mDMono!");
                    GameObject.Destroy(Instance.mDMono);
                    Instance = null;
                }
            }

            Instance = this;
            RegisterMonoUpdater();
        }
        
        public override void OnDestroy()
        {
            FsmList.Clear();
            Instance = null;
            EB.Debug.Log("GameFlowHotfixController.OnDestroy!");
            base.OnDestroy();
        }

        public void OnEnter(string viewAction)
        {
            ActiveStateName = viewAction;
            for(int i = 0; i < FsmList.Count; ++i)
            {
                if(ActiveStateName.Equals(FsmList[i].Name))
                {
                    EB.Debug.Log("{0}:Enter!", FsmList[i].Name);
                    FsmList[i].OnEnter();
                    break;
                }
            }
        }

        public void OnExit(string viewAction)
        {
            PreviousActiveStateName = viewAction;
            for (int i = 0; i < FsmList.Count; ++i)
            {
                if (PreviousActiveStateName.Equals(FsmList[i].Name))
                {
                    EB.Debug.Log("{0}:OnExit!", FsmList[i].Name);
                    FsmList[i].OnExit();
                    break;
                }
            }
        }
        
        public void LateUpdate()
        {
            for (int i = 0; i < FsmList.Count; ++i)
            {
                if (FsmList[i].IsOpen) FsmList[i].OnLateUpdate();
            }
        }
    }

    public class FsmStateUnit
    {
        public bool IsOpen;
        public string Name;

        public virtual void OnEnter()
        {
            EB.Debug.Log("FsmStateUnit.OnEnter=====>{0}", Name);
            IsOpen = true;
        }

        public virtual void OnExit()
        {
            EB.Debug.Log("FsmStateUnit.OnExit<====={0}", Name);
            IsOpen = false;
        }

        public virtual void OnAwake()
        {
        }

        public virtual void OnLateUpdate()
        {
        }
    }
}