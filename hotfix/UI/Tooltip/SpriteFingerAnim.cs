using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class SpriteFingerAnim : DynamicMonoHotfix
    {
        public Transform originalPoint;
        public Transform endPoint;
        public UISprite sprite;
        protected Coroutine c_finger;
        private string _originalSptName;
        private bool _isShow;
        public GameObject Fxobj;
    
        public TweenAlpha TA;
        public TweenPosition TP;
        public TweenRotation TR;

		public static Vector3 ArrowPoint;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            originalPoint = t.parent.GetComponent<Transform>("Container");
            endPoint = t.parent.GetComponent<Transform>("Container1");
            sprite = t.GetComponentEx<UISprite>();
            Fxobj = t.parent.parent.FindEx("FX").gameObject;
            TA = t.GetComponentEx<TweenAlpha>();
            TP = t.GetComponentEx<TweenPosition>();
            TR = t.GetComponentEx<TweenRotation>();
        }

        public override void Start()
        {
            Play();
        }
    
        public void Play()
        {
            _originalSptName = sprite.spriteName;
            _isShow = true;

            if (c_finger != null)
            {
                EB.Coroutines.Stop(c_finger);
            }
    
            c_finger= EB.Coroutines.Run(FingerDo());
            
        }
    
        public void Stop()
        {
            sprite.spriteName = _originalSptName;
            _isShow = false;

            if (c_finger != null)
            {
                EB.Coroutines.Stop(c_finger);
            }
    
            c_finger = null;
        }

        public override void OnDisable()
        {
            Fxobj.SetActive(false);
            TA.ResetToBeginning();
            TA.enabled =true;
        }

        public override void OnDestroy()
        {
            Stop();
        }
    
        private void Move()
        {
            TP.from = originalPoint.localPosition;
            TP.to = endPoint.localPosition;
            TP.ResetToBeginning();
    
            TR.from = originalPoint.localRotation.eulerAngles;
            TR.to = endPoint.localRotation.eulerAngles;
            TR.ResetToBeginning();
    
            TP.PlayForward();
            TR.PlayForward();
            sprite.spriteName = _originalSptName;

			Transform arrow = mDMono.transform.parent.Find("DragArrow");
			if(arrow.gameObject.activeSelf)
			{
				arrow.localPosition = ArrowPoint;

				TweenWidth tWidth = arrow.GetComponent<TweenWidth>();
				tWidth.duration = TP.duration;
				tWidth.ResetToBeginning();
				tWidth.PlayForward();
				tWidth.style = UITweener.Style.Once;

				TweenHeight tHeight = arrow.GetComponent<TweenHeight>();
				tHeight.duration = TP.duration;
				tHeight.ResetToBeginning();
				tHeight.PlayForward();
				tHeight.style = UITweener.Style.Once;
			}
        }
    
        void AnimationEnd()
        {
            sprite.spriteName = "Ty_Guide_Shouzhi2";
            Fxobj.SetActive(false);
            Fxobj.SetActive(true);
        }
        
        IEnumerator FingerDo()
        {
            while (_isShow)
            {
                while (!sprite.gameObject.activeInHierarchy)
                {
                    yield return null;
                }
    
                Move();
                float beforeTime = Time.realtimeSinceStartup;

                while (Time.realtimeSinceStartup - beforeTime < 0.9f)
                {
                    if (!sprite.gameObject.activeInHierarchy)
                    {
                        break;
                    }

                    yield return null;
                }

                AnimationEnd();

                while (Time.realtimeSinceStartup - beforeTime < 1.1f)
                {
                    if (!sprite.gameObject.activeInHierarchy)
                    {
                        break;
                    }

                    yield return null;
                }
            }
        }
    }
}
