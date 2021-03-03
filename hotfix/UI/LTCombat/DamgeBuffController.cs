using System;
using EB;
using UnityEngine;
using Debug = UnityEngine.Debug;
    
namespace Hotfix_LT.UI
{
    public class DamgeBuffController : DynamicMonoHotfix
    {
        //觉醒副本相关
        public UISprite AttrAddSprite;
        public UILabel AttrAddNameLabel;
        public UILabel[] AttrAddNameLabel_Subs;
        public UILabel AttrAddNumLabel;
        public UILabel[] AttrAddNumLabel_Subs;
        public UISprite AttrAddSprite2;
        public UILabel AttrAddNameLabel2;
        public UILabel[] AttrAddNameLabel2_Subs;
        public UILabel AttrAddNumLabel2;
        public UILabel[] AttrAddNumLabel2_Subs;
        public UILabel WindowMid;
        public UILabel[] WindowMid_Subs;
        public UILabel WindowBottom1;
        public UILabel WindowBottom2;
        
        public GameObject ToastWindow;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            AttrAddSprite = t.GetComponent<UISprite>("Bg");
            AttrAddSprite2 = t.GetComponent<UISprite>("ToastWindow/Sprite/Bg");

            AttrAddNameLabel = t.GetComponent<UILabel>("NameLabel");
            AttrAddNameLabel_Subs = AttrAddNameLabel.GetComponentsInChildren<UILabel>();

            AttrAddNumLabel = t.GetComponent<UILabel>("LevelSprite/LabelLevel");
            AttrAddNumLabel_Subs = AttrAddNumLabel.GetComponentsInChildren<UILabel>();

            AttrAddNameLabel2 = t.GetComponent<UILabel>("ToastWindow/Sprite/NameLabel");
            AttrAddNameLabel2_Subs = AttrAddNameLabel2.GetComponentsInChildren<UILabel>();

            AttrAddNumLabel2 = t.GetComponent<UILabel>("ToastWindow/Sprite/LevelSprite/LabelLevel");
            AttrAddNumLabel2_Subs = AttrAddNumLabel2.GetComponentsInChildren<UILabel>();

            WindowMid = t.GetComponent<UILabel>("ToastWindow/Sprite/Mid");
            WindowMid_Subs = WindowMid.GetComponentsInChildren<UILabel>();;

            WindowBottom1 = t.GetComponent<UILabel>("ToastWindow/Sprite/Bottom1");
            WindowBottom2 = t.GetComponent<UILabel>("ToastWindow/Sprite/Bottom2");
            ToastWindow = t.FindEx("ToastWindow").gameObject;

            UIButton closeBtn = t.GetComponent<UIButton>("ToastWindow/Panel/CloaseBtn");
            closeBtn.onClick.Add(new EventDelegate(CloseWindow));

            UIButton openBtn = t.GetComponent<UIButton>();
            openBtn.onClick.Add(new EventDelegate(OnClick));
        }

        public void AwakeningSetting(int num, Hotfix_LT.Data.eRoleAttr sRoleAttr, int unitValue = 50, string name = default, string format = default)
        {
            LTUIUtil.SetText(AttrAddNumLabel_Subs, num.ToString());
            LTUIUtil.SetText(AttrAddNumLabel2_Subs, num.ToString());
			string totalFormat = string.Empty;


			switch (sRoleAttr)
            {
                case Hotfix_LT.Data.eRoleAttr.Feng:
					// icon
                    AttrAddSprite.spriteName = "Juexing_Shuxing_Feng_1";
					AttrAddSprite2.spriteName = "Juexing_Shuxing_Feng_1";
					// name
					LTUIUtil.SetText(AttrAddNameLabel_Subs, string.IsNullOrEmpty(name) ? Localizer.GetString("ID_JUEXING_SHUXING_FENG") : name);                    
                    LTUIUtil.SetText(AttrAddNameLabel2_Subs, string.IsNullOrEmpty(name) ? Localizer.GetString("ID_JUEXING_SHUXING_FENG") : name);
					// desc
                    LTUIUtil.SetText(WindowMid_Subs, string.IsNullOrEmpty(format) ? Localizer.GetString("ID_JUEXING_DAMAGE_EVERY_FENG") : string.Format(format, unitValue));
					// num
					WindowBottom1.text = string.Format(Localizer.GetString("ID_JUEXING_DAMAGE_NUM_FENG"), num );
					// total value
					totalFormat = string.IsNullOrEmpty(format) ? "ID_JUEXING_DAMAGE_ADD_FENG" : "ID_BOSSCHALLENGE_REWARD_ADD";
                    WindowBottom2.text = string.Format(Localizer.GetString(totalFormat), num * unitValue);
					// show
					mDMono.gameObject.CustomSetActive(true);
                    break;
                case Hotfix_LT.Data.eRoleAttr.Shui:
                    AttrAddSprite.spriteName = "Juexing_Shuxing_Shui_1";
					AttrAddSprite2.spriteName = "Juexing_Shuxing_Shui_1";

					LTUIUtil.SetText(AttrAddNameLabel_Subs, string.IsNullOrEmpty(name) ? Localizer.GetString("ID_JUEXING_SHUXING_SHUI") : name);                 
                    LTUIUtil.SetText(AttrAddNameLabel2_Subs, string.IsNullOrEmpty(name) ? Localizer.GetString("ID_JUEXING_SHUXING_SHUI") : name);

                    LTUIUtil.SetText(WindowMid_Subs, string.IsNullOrEmpty(format) ? Localizer.GetString("ID_JUEXING_DAMAGE_EVERY_SHUI") : string.Format(format, unitValue));
                    WindowBottom1.text = string.Format(Localizer.GetString("ID_JUEXING_DAMAGE_NUM_SHUI"), num );

					totalFormat = string.IsNullOrEmpty(format) ? "ID_JUEXING_DAMAGE_ADD_SHUI" : "ID_BOSSCHALLENGE_REWARD_ADD";
					WindowBottom2.text = string.Format(Localizer.GetString(totalFormat), num * unitValue);

                    mDMono.gameObject.CustomSetActive(true);
                    break;
                case Hotfix_LT.Data.eRoleAttr.Huo:
                    AttrAddSprite.spriteName = "Juexing_Shuxing_Huo_1";
					AttrAddSprite2.spriteName = "Juexing_Shuxing_Huo_1";

					LTUIUtil.SetText(AttrAddNameLabel_Subs, string.IsNullOrEmpty(name) ? Localizer.GetString("ID_JUEXING_SHUXING_HUO") : name);                    
                    LTUIUtil.SetText(AttrAddNameLabel2_Subs, string.IsNullOrEmpty(name) ? Localizer.GetString("ID_JUEXING_SHUXING_HUO") : name);

                    LTUIUtil.SetText(WindowMid_Subs, string.IsNullOrEmpty(format) ? Localizer.GetString("ID_JUEXING_DAMAGE_EVERY_HUO") : string.Format(format, unitValue));
                    WindowBottom1.text = String.Format(Localizer.GetString("ID_JUEXING_DAMAGE_NUM_HUO"), num );

					totalFormat = string.IsNullOrEmpty(format) ? "ID_JUEXING_DAMAGE_ADD_HUO" : "ID_BOSSCHALLENGE_REWARD_ADD";
					WindowBottom2.text = String.Format(Localizer.GetString(totalFormat), num * unitValue);

					mDMono.gameObject.CustomSetActive(true);
                    break;
                default:
                    mDMono.gameObject.CustomSetActive(false);
                    break;
            }
        }
    
        public void OnClick()
        {
            ToastWindow.gameObject.CustomSetActive(true);
        }
    
    
        public void CloseWindow()
        {
            ToastWindow.gameObject.CustomSetActive(false);
        }
            
    }
}
