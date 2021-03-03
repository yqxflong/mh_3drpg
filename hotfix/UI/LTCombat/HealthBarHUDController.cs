using UnityEngine;

namespace Hotfix_LT.UI
{
    public class HealthBarHUDController : DynamicMonoHotfix
    {
        public GameObject HUDTemplate;

        public static HealthBarHUDController Instance
        {
            get;
            private set;
        }

        public override void Awake()
        {
            Instance = this;

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    HUDTemplate = (GameObject)mDMono.ObjectParamList[0];
                }
            }
        }

        public override void OnDestroy()
        {
            Instance = null;
        }

        public HealthBarHUD GetHUD()
        {
            return CreateHUD();
        }

        private HealthBarHUD CreateHUD()
        {
            if (HUDTemplate == null)
            {
                Awake();

                if (HUDTemplate == null)
                {
                    EB.Debug.LogError("HUDTemplate is null");
                    return null;
                }
            }

            GameObject go = GameObject.Instantiate(HUDTemplate);
            HealthBarHUD hud = go.GetMonoILRComponent<HealthBarHUD>();
            hud.mDMono.transform.parent = mDMono.transform;
            hud.mDMono.transform.localScale = Vector3.one;
            return hud;
        }
    }
}