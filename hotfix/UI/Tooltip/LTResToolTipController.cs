using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTResToolTipController : UIControllerHotfix, IHotfixUpdate
    {
        public static bool isOpen = false;
        public LTShowItem ShowItem;
        public UILabel HaveNumLabel;
        public UILabel DescTextLabel;
        public Vector4 Margin = Vector4.zero;
        public UIWidget BG;
        private bool CheckMouseClick = false;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            isOpen = false;
            ShowItem = t.GetMonoILRComponent<LTShowItem>("DataPanel/Content/LTShowItem");
            HaveNumLabel = t.GetComponent<UILabel>("DataPanel/Content/HasNum");
            DescTextLabel = t.GetComponent<UILabel>("DataPanel/Content/DescLabel");

            BG = t.GetComponent<UIWidget>("DataPanel/BG");

            t.GetComponent<UIEventTrigger>("Container").onClick.Add(new EventDelegate(OnCancelButtonClick));
        }

        public override void SetMenuData(object param)
        {
            isOpen = true;
            Hashtable hashData = param as Hashtable;
            string type = hashData["type"].ToString();
            string id = hashData["id"].ToString();
            Vector2 screenPos = (Vector2)hashData["screenPos"];
            ShowInfo(type, id);
            SetAnchor(screenPos);
            CheckMouseClick = true;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            CheckMouseClick = false;
            isOpen = false;
            DestroySelf();
            return base.OnRemoveFromStack();
        }

        void ShowInfo(string type, string id)
        {
            if (type == LTShowItemType.TYPE_RES || type == LTShowItemType.TYPE_HEROMEDAL || type == LTShowItemType.TYPE_ACTICKET||type == LTShowItemType.TYPE_VIPPOINT)
            {
                int resNum = BalanceResourceUtil.GetResValue(id);

                ShowItem.LTItemData = new LTShowItemData(id, 1, LTShowItemType.TYPE_RES, true);
                HaveNumLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTResToolTipController_951"), resNum);
                int resIdInEconemy = BalanceResourceUtil.GetResID(id);
                Hotfix_LT.Data.EconemyItemTemplate itemInfo = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(resIdInEconemy);
                if (itemInfo != null)
                {
                    DescTextLabel.text= itemInfo.Desc;
                }
                else
                {
                    DescTextLabel.text= EB.Localizer.GetString("ID_ITEM_DESC");
                }
            }
            else if (type == LTShowItemType.TYPE_HEROSHARD)
            {
                int num = GameItemUtil.GetInventoryItemNum(id);

                ShowItem.LTItemData = new LTShowItemData(id, 1, LTShowItemType.TYPE_HEROSHARD, true);
                HaveNumLabel.text= string.Format(EB.Localizer.GetString("ID_codefont_in_LTResToolTipController_951"), num);
                Hotfix_LT.Data.EconemyItemTemplate itemInfo = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(id);
                if (itemInfo != null)
                {
                    DescTextLabel.text = itemInfo.Desc;
                }
                else
                {
                    DescTextLabel.text= EB.Localizer.GetString("ID_ITEM_DESC");
                }
            }
            else if (type == LTShowItemType.TYPE_HEADFRAME)
            {
                ShowItem.LTItemData = new LTShowItemData(id, 1, LTShowItemType.TYPE_HEADFRAME);
                HaveNumLabel.text= string.Format(string.Empty);
                var data = EconemyTemplateManager.Instance.GetHeadFrame(id, 1);
                DescTextLabel.text= data.desc;
            }
            else if (type == LTShowItemType.TYPE_SCROLL)
            {
                ShowItem.LTItemData = new LTShowItemData(id, 1, LTShowItemType.TYPE_SCROLL);
                HaveNumLabel.text = string.Empty;
                Hotfix_LT.Data.SkillTemplate skillTpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(int.Parse(id));
                DescTextLabel.text = skillTpl.Description;
            }
            else
            {
                int num = GameItemUtil.GetInventoryItemNum(id);
                //运营活动的脚印数量不存背包，数量要特殊获取
                if (type == LTShowItemType.TYPE_ACTIVITY)
                {
                    if (id.Equals("2005")) { id = "2012"; }
                    DataLookupsCache.Instance.SearchDataByID("tl_acs." + id + ".current", out num);
                }

                ShowItem.LTItemData = new LTShowItemData(id, 1, type, true);
                HaveNumLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTResToolTipController_951"), num);
                Hotfix_LT.Data.EconemyItemTemplate itemInfo = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(id);
                if (itemInfo != null)
                {
                    DescTextLabel.text = itemInfo.Desc;
                }
                else
                {
                    DescTextLabel.text = EB.Localizer.GetString("ID_ITEM_DESC");
                }
            }
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            if ((CheckMouseClick && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))))
            {
                if (!BG.GetComponent<BoxCollider>().bounds.Contains(UICamera.lastWorldPosition))
                {
                    CheckMouseClick = false;
                    controller.Close();
                }
            }
        }

        void SetAnchor(Vector2 screenPos)
        {
            Vector3 worldPos = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(Mathf.Clamp(screenPos.x, BG.width * ((float)Screen.width / (float)UIRoot.list[0].manualWidth) / 2, (Screen.width - BG.width * ((float)Screen.width / (float)UIRoot.list[0].manualWidth) / 2)), screenPos.y));
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

        static public void Show(string type, string resid)
        {
            Vector2 screenPos = UICamera.lastEventPosition;
            var ht = Johny.HashtablePool.Claim();
            ht.Add("type", type);
            ht.Add("id", resid);
            ht.Add("screenPos", screenPos);
            GlobalMenuManager.Instance.Open("LTResToolTipUI", ht);
        }
    }
}
