using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class NationBattleDamageHUD : DynamicMonoHotfix
    {
    	public Transform MotionTrans;
    	public UILabel DamageLabel;	
    	public UILabel HealLabel;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            MotionTrans = t.GetComponent<Transform>("DamageMotion");
            DamageLabel = t.GetComponent<UILabel>("DamageMotion/Layout/Content/DamageNum");
            HealLabel = t.GetComponent<UILabel>("DamageMotion/Layout/Content/HealNum");

        }

        private int sequence = 0;
        public void Play(bool isRightSide,int damage)
    	{
    		Vector3 pos = mDMono.transform.localPosition;
    		pos.x = isRightSide ? Mathf.Abs(pos.x) : -Mathf.Abs(pos.x);
            mDMono.transform.localPosition = pos;
    
    		if (damage > 0)
    		{
    			DamageLabel.text = "-" + damage;
    			DamageLabel.gameObject.SetActive(true);
    			HealLabel.gameObject.SetActive(false);
    		}
    		else
    		{
    			HealLabel.text = "+" + (-damage);
    			DamageLabel.gameObject.SetActive(false);
    			HealLabel.gameObject.SetActive(true);
    		}
    
    		UITweener[] tweeners = MotionTrans.GetComponents<UITweener>();
    		float duration = 0.0f;
    		for (int i = 0; i < tweeners.Length; ++i)
    		{
    			tweeners[i].tweenFactor = 0;
    			tweeners[i].PlayForward();
    
    			duration = Mathf.Max(duration, tweeners[i].delay + tweeners[i].duration);
    		}

            int timer = (int)((duration + 0.3f) * 1000);
            ILRTimerManager.instance.AddTimer(timer,1 ,delegate { OnShowComplete(); });
    	}
    
    	public void OnShowComplete()
    	{
    		Object.Destroy(mDMono.gameObject);
    	}
    }
    
}
