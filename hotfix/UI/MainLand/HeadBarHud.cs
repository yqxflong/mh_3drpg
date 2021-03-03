using UnityEngine;
using System.Collections;
using Main.MainLand;

namespace Hotfix_LT.UI
{
    public class HeadBarHud : DynamicMonoHotfix
    {
        public HeadBarHUDMonitor mHeadBarHUDMonitor;

        public override void Awake()
        {
            base.Awake();
            mHeadBarHUDMonitor = mDMono.GetComponent<HeadBarHUDMonitor>();
            if (mHeadBarHUDMonitor == null)
            {
                mHeadBarHUDMonitor = mDMono.gameObject.AddComponent<HeadBarHUDMonitor>();
            }
        }

        public System.Action recycleCallback
        {
            get;
            set;
        }
        
        public virtual void SetBarState(Hashtable data, bool state)
        {

        }
        
        public virtual void Recycle()
        {
            if (recycleCallback != null)
            {
                recycleCallback();
            }
        }
    }
}