using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTAlienMazeLockTipController : DynamicMonoHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            TipLabel = t.GetComponent<UILabel>("Label");
            BgBox = t.GetComponent<BoxCollider>("BG");
            t.GetComponent<TweenScale>().onFinished.Add(new EventDelegate(TweenFinish));
        }
        public UILabel TipLabel;
        public BoxCollider BgBox;
        private bool CheckMouseClick = false;

		public override void OnEnable()
		{
			//base.OnEnable();
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public void Update () {
            if (CheckMouseClick && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
            {
                if (!BgBox.bounds.Contains(UICamera.lastWorldPosition))
                {
                    Close();
                }
            }
        }
    
        private void TweenFinish ()
        {
            CheckMouseClick = true;
        }
    
        public void Open(string TipStr)
        {
            TipLabel.text = TipStr;
            mDMono.transform.position = new Vector3(UICamera.lastWorldPosition.x, 0);
            mDMono.gameObject.CustomSetActive(true);
            mDMono.GetComponent<TweenScale>().PlayForward();
        }
    
        public void Close()
        {
            CheckMouseClick = false;
            mDMono.gameObject.CustomSetActive(false);
            mDMono.GetComponent<TweenScale>().ResetToBeginning();
        }
    
    }
}
