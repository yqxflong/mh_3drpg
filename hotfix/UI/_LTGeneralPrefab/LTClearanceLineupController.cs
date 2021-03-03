using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTClearanceLineupController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen() { return false; }

        private FormationPartnerItem[] _items;
        private UILabel _title;
        private UILabel _level;
        private UILabel _playerName;
        private UILabel _power;
        private UILabel _powerName;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("BG/Top/CloseBtn");

            var tGrid = t.GetComponent<UIGrid>("Content/Lineup/UIGrid").transform;
            var childCount = tGrid.childCount;
            _items = new FormationPartnerItem[childCount];

            for (var i = 0; i < childCount; i++)
            {
                var child = tGrid.GetChild(i);
                _items[i] = child.GetMonoILRComponentInChildren<FormationPartnerItem>("Hotfix_LT.UI.FormationPartnerItem");
                _items[i].mDMono.gameObject.SetActive(false);
            }

            SetTitle(EB.Localizer.GetString("ID_LOWEST_CLEARANCE_TITLE"));
            SetPowerName(EB.Localizer.GetString("ID_LOWEST_CLEARANCE_POWER") + ":");
        }

        public void SetTitle(string text)
        {
            if (_title == null)
            {
                _title = controller.transform.GetComponent<UILabel>("BG/Top/Title");
            }

            if (_title != null)
            {
                _title.text = text;
            }
        }

        public void SetLevel(string text)
        {
            if (_level == null)
            {
                _level = controller.transform.GetComponent<UILabel>("Content/Infos/LevelBG/Level");
            }

            if (_level != null)
            {
                _level.text = text;
            }
        }

        public void SetPlayerName(string text)
        {
            if (_playerName == null)
            {
                _playerName = controller.transform.GetComponent<UILabel>("Content/Infos/PlayerName");
            }

            if (_playerName != null)
            {
                _playerName.text = text;
            }
        }

        public void SetPower(string text)
        {
            if (_power == null)
            {
                _power = controller.transform.GetComponent<UILabel>("Content/Infos/Power");
            }

            if (_power != null)
            {
                _power.text = text;
            }
        }

        public void SetPowerName(string text)
        {
            if (_powerName == null)
            {
                _powerName = controller.transform.GetComponent<UILabel>("Content/Infos/PowerName");
            }

            if (_powerName != null)
            {
                _powerName.text = text;
            }
        }

        public void InitData(OtherPlayerPartnerData[] datas)
        {
            if (datas == null)
            {
                return;
            }

            var list = new List<OtherPlayerPartnerData>(datas).FindAll(item => item.Level > 0);

            for (var i = 0; i < _items.Length; i++)
            {
                if (i < list.Count)
                {
                    _items[i].Fill(list[i]);
                    _items[i].mDMono.gameObject.SetActive(true);
                }
                else
                {
                    _items[i].mDMono.gameObject.SetActive(false);
                }
            }
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            var ht = param as Hashtable;
            SetPlayerName(ht["Name"].ToString());
            SetLevel(ht["Level"].ToString());
            SetPower(ht["Power"].ToString());
            InitData(ht["Lineup"] as OtherPlayerPartnerData[]);
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            yield return null;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            yield return base.OnRemoveFromStack();
            DestroySelf();
            yield break;
        }
    }
}
