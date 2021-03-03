using UnityEngine;

namespace Hotfix_LT.UI
{
    public class PhoneStatusController : DynamicMonoHotfix
    {
    	public UIProgressBar BatteryLevelBar;
    	public UISprite WifiSprite;
    	public UILabel TimeLabel,TimeLabelChild;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    BatteryLevelBar = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<UIProgressBar>();
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    WifiSprite = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UISprite>();
                }
                if (count > 2 && mDMono.ObjectParamList[2] != null)
                {
                    TimeLabel = ((GameObject)mDMono.ObjectParamList[2]).GetComponentEx<UILabel>();
                    TimeLabelChild = TimeLabel.transform.GetChild(0).GetComponent<UILabel>();
                }
            }
        }

        private int sequence = 0;
        public override void Start () {
            sequence = ILRTimerManager.instance.AddTimer(1000,int.MaxValue,delegate { IntervalUpdate(); });

        }

        public override void OnDestroy()
        {
            ILRTimerManager.instance.RemoveTimer(sequence);
            sequence=0;
            base.OnDestroy();
        }

        void IntervalUpdate()
    	{
            if (WifiSprite == null)
            {
                return;
            }

            TimeLabel.text = TimeLabelChild.text = EB.Time.LocalNow.ToString("HH:mm");
    
            if (SystemInfo.batteryLevel >= 0 && SystemInfo.batteryLevel <= 1)
    		{
    			BatteryLevelBar.value = SystemInfo.batteryLevel;
    		}
            
    		WifiSprite.gameObject.CustomSetActive(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
        }
    }
}
