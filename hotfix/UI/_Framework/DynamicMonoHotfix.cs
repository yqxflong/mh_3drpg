using System.Collections;
using System.Collections.Generic;
using EB;
using UnityEngine;

namespace Hotfix_LT
{
    public class DynamicMonoHotfix : DynamicMonoILRObject
    {
        public DynamicMonoILR mDMono;

        public override void Awake()
        {
        }

        public override void Start()
        {
        }

        public override void OnEnable()
        {
        }

        public override void OnDisable()
        {
        }

        protected virtual void RegisterMonoUpdater()
		{
			if(mDMono!=null) Hotfix_LT.ILRUpdateManager.RegisterNeedUpdateMono(this);
		}

		protected virtual void ErasureMonoUpdater()
        {
            if (mDMono != null) Hotfix_LT.ILRUpdateManager.UnRegisterNeedUpdateMono(this);
		}

		public override void OnDestroy()
        {
            Hotfix_LT.ILRUpdateManager.UnRegisterNeedUpdateMono(this);
        }

        public override void SetMono(DynamicMonoILR mono)
        {
            this.mDMono = mono;
        }

        public static PrefabCreator GetCreator(PrefabCreator[] temps, string name)
        {
            for (int i = 0; i < temps.Length; i++)
            {
                if (temps[i].name.Equals(name))
                {
                    return temps[i];
                }
            }
            return null;
        }

        #region Coroutine
        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            var c = mDMono.StartCoroutine(routine);
            return c;
        }

        protected void StopCoroutine(Coroutine c)
        {
            if(mDMono!=null) mDMono.StopCoroutine(c);
        }

        protected void StopCoroutine(IEnumerator c)
        {
            if (mDMono != null) mDMono.StopCoroutine(c);
        }

        protected void StopAllCoroutines()
        {
            if (mDMono != null) mDMono.StopAllCoroutines();
        }
        
        #endregion

        public override void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
        {
        }

        #region UIPanelUpdate Manul Handler

        protected UIPanel CurrentPanel;

        protected bool Exist(object obj)
        {
	        return obj != null;
        }

        private void FetchPanelWhileNull()
        {
	        if (!Exist(CurrentPanel) && Exist(mDMono))
	        {
		        CurrentPanel = mDMono.GetComponent<UIPanel>();
	        }
        }

		protected void PausePanelUpdate(UIPanel.PauseType type = UIPanel.PauseType.All, bool selfValue = true, float wait = 2f, System.Action callback = null)
        {
	        FetchPanelWhileNull();

	        if (CurrentPanel != null)
	        {
		        EB.Debug.Log("<color=yellow>PausePanelUpdate(Dynamic) (name, type)=> " + CurrentPanel.name + ", " + type + "</color>");
		        if (CurrentPanel != null) CurrentPanel.SetPause(type, selfValue);
		        callback?.Invoke();
			}
        }

		protected void ResumePanelUpdate(bool autoPause = false, UIPanel.PauseType type = UIPanel.PauseType.Others, bool pauseValue = false, float wait = 1f)
        {
	        FetchPanelWhileNull();

			if (CurrentPanel) EB.Debug.Log("<color=yellow>ResumePanelUpdate(Dynamic) (auto_pause, name)=> " + autoPause + "," + CurrentPanel.name + "</color>");
			if (CurrentPanel) CurrentPanel.IsPause = false;
			if (autoPause) PausePanelUpdate(type, pauseValue, wait);
		}
        #endregion
	}
}
