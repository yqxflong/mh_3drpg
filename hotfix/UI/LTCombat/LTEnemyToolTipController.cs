using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTEnemyToolTipController : UIControllerHotfix, IHotfixUpdate
    {
        public UILabel NameLabel, AttLabel, DefLabel, HpLabel,CritPLabel,CritVLabel,SpeedLabel,SpExtraLabel,SpResLabel;
        public BoxCollider Bg;
        public static bool m_Open;
        private bool CheckMouseClick;
        private Vector4 Margin = Vector4.zero;

        public override void Awake()
        {
            base.Awake();

            CheckMouseClick = false;
            var t = controller.transform;
            NameLabel = t.GetComponent<UILabel>("Content/NameLabel");
            AttLabel = t.GetComponent<UILabel>("Content/AttrInfo/Att/NumLabel");
            DefLabel = t.GetComponent<UILabel>("Content/AttrInfo/Def/NumLabel");
            HpLabel = t.GetComponent<UILabel>("Content/AttrInfo/HP/NumLabel");
            CritPLabel= t.GetComponent<UILabel>("Content/AttrInfo/CritP/NumLabel");
            CritVLabel= t.GetComponent<UILabel>("Content/AttrInfo/CritV/NumLabel");
            SpeedLabel = t.GetComponent<UILabel>("Content/AttrInfo/Speed/NumLabel");
            SpExtraLabel = t.GetComponent<UILabel>("Content/AttrInfo/SpExtra/NumLabel");
            SpResLabel = t.GetComponent<UILabel>("Content/AttrInfo/SpRes/NumLabel");
            Bg = t.GetComponent<BoxCollider>("Content/BG");
            m_Open = true;
            controller.backButton = t.GetComponent<UIButton>("Container");
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable hashData = param as Hashtable;
            Hotfix_LT.Data.MonsterInfoTemplate data = (Hotfix_LT.Data.MonsterInfoTemplate)hashData["data"];
            Vector2 screenPos = (Vector2)hashData["screenPos"];
            ShowInfo(data);
            SetAnchor(screenPos);
        }
        public override IEnumerator OnAddToStack()
        {
            m_Open = true;
            yield return base.OnAddToStack();
            CheckMouseClick = true;
        }
        public override IEnumerator OnRemoveFromStack()
        {
            m_Open = false;
            CheckMouseClick = false;
            DestroySelf();
            yield break;
        }
        public override void StartBootFlash()
        {
			SetCurrentPanelAlpha(1);
			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            if (CheckMouseClick && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
            {
                if (!Bg.GetComponent<BoxCollider>().bounds.Contains(UICamera.lastWorldPosition))
                {
                    CheckMouseClick = false;
                    controller.Close();
                }
            }
        }
    
        void ShowInfo(Hotfix_LT.Data.MonsterInfoTemplate data)
        {
            NameLabel.text = NameLabel.transform.GetChild(0).GetComponent<UILabel>().text = data.name;
            AttLabel.text = data.base_ATK.ToString ("f0");
            DefLabel.text = data.base_DEF.ToString("f0");
            HpLabel.text = data.base_MaxHP.ToString("f0");
            SpeedLabel.text = data.speed.ToString("f0") ;
            int num = Mathf.FloorToInt(data.CritP * 100);
            CritPLabel.text =  num+ "%";
            num = Mathf.FloorToInt(data.CritV * 100);
            CritVLabel.text =  num+ "%";
            num = Mathf.FloorToInt(data.SpExtra * 100);
            SpExtraLabel.text =  num+ "%";
            num = Mathf.FloorToInt(data.SpRes * 100);
            SpResLabel.text =  num+ "%";
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
            // if (worldPos.y >= 1f - worldMargin.w - abs.size.y - 0.2f)
            // {
            //     currentPos.y = worldPos.y - abs.size.y / 2 - 0.1f;
            // }
            // else
            // {
            //     currentPos.y = worldPos.y + abs.size.y / 2 + 0.1f;
            // }

            currentPos.y = -0.6f;
            controller.transform.GetChild(0).position = currentPos;
        }
    
    }
}
