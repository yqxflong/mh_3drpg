using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTClearanceLineupBtn : DynamicMonoHotfix
    {
        private UISprite _icon;
        private UISprite _frame;
        private UILabel _name;
        private int _level;
        private System.Action<int, System.Action<Hashtable>> _onClicked;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnBtnClicked));

            SetName(EB.Localizer.GetString("ID_LOWEST_CLEARANCE_BUTTON_NAME"));
        }

        private void OnBtnClicked()
        {
            _onClicked?.Invoke(_level, data => 
            {
                string playerLevel = "0";
                string playerName = "";
                string playerArea = "";
                string playerPower = "";
                OtherPlayerPartnerData[] partnerDatas = null;

                if (data != null)
                {
                    playerLevel = EB.Dot.String("lowestTeamView.info.level", data, null);
                    playerName = EB.Dot.String("lowestTeamView.info.name", data, null);
                    playerArea = EB.Dot.String("lowestTeamView.info.world_id", data, null);
                    playerPower = EB.Dot.String("lowestTeamView.battleRating", data, null);
                    Hashtable ht = EB.Dot.Object("lowestTeamView.teamView", data, null);
       
                    if (ht != null && ht.Values != null)
                    {
                        foreach (Hashtable val in ht.Values)
                        {
                            partnerDatas = LTFormationDataManager.Instance.GetOtherPalyerPartnerDataList("normal", val).ToArray();
                        }
                    }
                }

                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);

                var param = Johny.HashtablePool.Claim();
                param.Add("Level", playerLevel);
                param.Add("Name", string.Format("{0}【{1}{2}】", playerName, playerArea, EB.Localizer.GetString("ID_LOGIN_SERVER_NAME")));
                param.Add("Power", playerPower);
                param.Add("Lineup", partnerDatas);
                GlobalMenuManager.Instance.Open("LTClearanceLineupUI", param);
            });
        }

        public void RegisterClickEvent(System.Action<int, System.Action<Hashtable>> act)
        {
            _onClicked = act;
        }

        public void SetLevel(int level)
        {
            _level = level;
        }

        public void SetName(string text)
        {
            if (_name == null)
            {
                _name = mDMono.transform.GetComponent<UILabel>("Bg/Bottom/Name");
            }

            if (_name != null)
            {
                _name.text = text;
            }
        }

        public void SetIcon(string iconName)
        {
            if (_icon == null)
            {
                _icon = mDMono.transform.GetComponent<UISprite>("Bg/Icon");
            }

            if (_icon != null)
            {
                _icon.spriteName = iconName;
            }
        }

        public void SetFrame(string iconName)
        {
            if (_frame == null)
            {
                _frame = mDMono.transform.GetComponent<UISprite>("Bg/Frame");
            }

            if (_frame != null)
            {
                _frame.spriteName = iconName;
            }
        }
    }
}
