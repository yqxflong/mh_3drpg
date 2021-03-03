using System.Collections;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTVigorToolTipController : UIControllerHotfix, IHotfixUpdate
    {
        public UILabel NextTimeLabel;
        public UILabel FullTimeLabel;
        public BoxCollider Bg;
    
        private Vector4 Margin = Vector4.zero;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            NextTimeLabel = t.GetComponent<UILabel>("DataPanel/Content/Next/TimeLabel");
            FullTimeLabel = t.GetComponent<UILabel>("DataPanel/Content/Full/TimeLabel");
            Bg = t.GetComponent<BoxCollider>("DataPanel/BG");
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            SetAnchor((Vector2)param);
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        private string nextTipValue;
        private string fullTipValue;

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

        public override void OnDisable()
        {
            ErasureMonoUpdater();
        }

        public void Update()
        {
            nextTipValue = AutoRefreshingManager.Instance.GetDeltaRefresher<VigorDeltaTimeRefresher>().GetVigorRecoverOneCountDown();
            fullTipValue = AutoRefreshingManager.Instance.GetDeltaRefresher<VigorDeltaTimeRefresher>().GetVigorRecoverAllCountDown();
            NextTimeLabel.text = nextTipValue;
            FullTimeLabel.text =string.IsNullOrEmpty (fullTipValue)? nextTipValue : fullTipValue;
    
            if ((Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
            {
                if (!Bg.GetComponent<BoxCollider>().bounds.Contains(UICamera.lastWorldPosition))
                {
                    controller.Close();
                }
            }
        }
    
        void SetAnchor(Vector2 screenPos)
        {
            Vector3 worldPos = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(Mathf.Clamp(screenPos.x, Bg.GetComponent<UIWidget>().width * ((float)Screen.width / (float)UIRoot.list[0].manualWidth) / 2, (Screen.width - Bg.GetComponent<UIWidget>().width * ((float)Screen.width / (float)UIRoot.list[0].manualWidth) / 2)), screenPos.y));
            Bounds abs = NGUIMath.CalculateAbsoluteWidgetBounds(controller.transform.GetChild(0));
            float aspect = (float)Screen.width / (float)Screen.height;
            Vector4 worldMargin = Margin * 2.0f / (float)UIRoot.list[0].manualHeight;
            worldPos.x = Mathf.Clamp(worldPos.x, -aspect + worldMargin.x, aspect - worldMargin.y);
            Vector3 currentPos = controller.transform.GetChild(0).position;
            currentPos.x = worldPos.x;

            if (worldPos.y >= 1f - worldMargin.w - abs.size.y - 0.2f)
            {
                currentPos.y = worldPos.y - abs.size.y / 2 - 0.1f;
            }
            else
            {
                currentPos.y = worldPos.y + abs.size.y / 2 + 0.1f;
            }

            controller.transform.GetChild(0).position = currentPos;
        }
    }
}
