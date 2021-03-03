using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTEquipmentWishItem : DynamicMonoHotfix
    {
        private DynamicUISprite _icon;
        private UILabel _name;
        private UILabel _desc;
        private UIToggle _toggle;
        private LTEquipmentWishController _ewCtrl;

        public int ItemId { get; set; }

        public override void Awake()
        {
            base.Awake();
            InitToggle();

            var ilr = mDMono.transform.GetComponentInParent<UIControllerILR>();

            if (ilr != null)
            {
                _ewCtrl = ilr.transform.GetUIControllerILRComponent<LTEquipmentWishController>();
            }
        }

        private void InitToggle()
        {
            if (_toggle == null)
            {
                _toggle = mDMono.transform.GetComponent<UIToggle>();
            }

            if (_toggle != null)
            {
                _toggle.onChange.Clear();
                _toggle.onChange.Add(new EventDelegate(OnToggleChanged));
            }
        }

        private bool _isFirstToggle = true;

        private void OnToggleChanged()
        {
            if (_isFirstToggle)
            {
                _isFirstToggle = false;
                return;
            }

            if (_toggle != null && _toggle.value)
            {
                var callback = _ewCtrl != null ? _ewCtrl.callback : null;
                SendEquipmentWishReq(ItemId, callback);
            }
        }

        private void SendEquipmentWishReq(int itemId, System.Action<int> callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/equipmentWish");
            request.AddData("activityId", LTEquipmentWishController.equipmentWishActivityId);
            request.AddData("itemId", itemId);
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable data) =>
            {
                DataLookupsCache.Instance.CacheData(string.Format("tl_acs.{0}.current", LTEquipmentWishController.equipmentWishActivityId), itemId);
                callback?.Invoke(itemId);
                TimerManager.instance.AddTimer(300, 1, timeSeq => GlobalMenuManager.Instance.CloseMenu("LTEquipmentWishUI"));
            });
        }

        public void SetIcon(string spriteName)
        {
            if (_icon == null)
            {
                _icon = mDMono.transform.GetComponent<DynamicUISprite>("Icon");
            }

            if (_icon != null)
            {
                _icon.spriteName = spriteName;
            }
        }

        public void SetName(string text)
        {
            if (_name == null)
            {
                _name = mDMono.transform.GetComponent<UILabel>("Name");
            }

            if (_name != null)
            {
                _name.text = text;
            }
        }

        public void SetDesc(string text)
        {
            if (_desc == null)
            {
                _desc = mDMono.transform.GetComponent<UILabel>("Desc");
            }

            if (_desc != null)
            {
                _desc.text = text;
            }
        }
    }
}
