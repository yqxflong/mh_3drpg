using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class StarItem : DynamicMonoHotfix
    {
        public string m_light;
        public string m_gray;
        public UISprite StarIcon;
        public GameObject FX;
        public SetSortingOrder SSO;
        public float FxPlayTime;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            StarIcon = t.GetComponentEx<UISprite>();
            FX = t.FindEx("FX").gameObject;
            SSO = FX.GetComponent<SetSortingOrder>();
            FxPlayTime = 0.45f;
            m_light = "Ty_Icon_Xingxing";
        }

        public void Reset()
        {
            StarIcon.spriteName = m_gray;
    		FX.gameObject.CustomSetActive(false);
    	}
    
    	public void Light()
    	{
    		StarIcon.spriteName = m_light;
    	}
    
    	public Coroutine DynamicLight()
        {
    		return StartCoroutine(DynamicLightCoroutine());
        }
    
    	IEnumerator DynamicLightCoroutine()
    	{
    		Reset();
    		PlayParticle();
    		yield return new WaitForSeconds(FxPlayTime);
    		FusionAudio.PostEvent("UI/CampaignResult/CampaignStar");
    		StarIcon.spriteName = m_light;
    		PlayForward();
    	}
    
    	void PlayForward()
        {
            UITweener[] mTweens = mDMono.GetComponents<UITweener>();
            for (int j = 0; j < mTweens.Length; j++)
            {
                mTweens[j].ResetToBeginning();
                mTweens[j].Play(true);
            }
        }
    
        void PlayParticle()
        {		
    		FX.gameObject.CustomSetActive(true);
    	}
    }
}
