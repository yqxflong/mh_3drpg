using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTCreatBGObj : DynamicMonoHotfix, IHotfixUpdate
    {
        //public GameObject BGPrefab;
        private UITexture tex;
        private float mTime;
        private Rect mRect;

        public override void Awake()
        {
            Init();
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }

        public void Init()
        {
            GameObject obj = GameObject.Instantiate(mDMono.ObjectParamList[0] as GameObject, mDMono.transform);
            Transform tf = obj.transform.Find("Texture");

            if (tf != null)
            {
                tex = tf.GetComponent<UITexture>();
                mRect = new Rect(0, 0, 1, 1);
            }
        }

        public void Update()
        {
            if (tex != null)
            {
                mTime += Time.deltaTime;
                float value = -mTime / 10f;
                mRect.y = value;
                tex.uvRect = mRect;
            }
        }
    }
}
