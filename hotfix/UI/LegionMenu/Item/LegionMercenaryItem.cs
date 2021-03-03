using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LegionMercenaryItem:DynamicCellController<LegionMemberData>
    {
        public UISprite BG;
        public UISprite HeadIcon;
        public UISprite HeadFrame;
        public UILabel Level;
        public UILabel Name;
        public UILabel Duty;
        public UIButton ItemBtn;
        public GameObject SelfBGObj;
        public CombatPartnerCellController CellController;
        
        private long uid;
        private LegionMemberData memberData;
        private UIEventListener listener;
        public override void Awake()
        {
            base.Awake();

            BG = mDMono.transform.Find("BG").GetComponent<UISprite>();
            HeadIcon = mDMono.transform.Find("Border/Icon").GetComponent<UISprite>();
            HeadFrame = mDMono.transform.Find("Border/Icon/Frame").GetComponent<UISprite>();
            Level = mDMono.transform.Find("LevelSprite/LevelLabel").GetComponent<UILabel>();
            Name = mDMono.transform.Find("NameLabel").GetComponent<UILabel>();
            Duty = mDMono.transform.Find("DutyLabel").GetComponent<UILabel>();//职位
            ItemBtn = mDMono.GetComponent<UIButton>();
            SelfBGObj = mDMono.transform.Find("SelfBG").gameObject;
            ItemBtn.onClick.Add(new EventDelegate(OnClickItem));
            CellController = mDMono.transform.GetMonoILRComponent<CombatPartnerCellController>("Item");
            listener = UIEventListener.Get(HeadIcon.gameObject);
            listener.onClick += OnClick;
            Messenger.AddListener(EventName.LegionMercenaryUpdateUIDelay,NeedUpdateUI);
        }

        private void OnClick(GameObject go)
        {
            if (LegionEvent.OnClickMember != null)
            {
                LegionEvent.OnClickMember(uid);
            }
        }

        private void NeedUpdateUI()
        {
            if (memberData != null && memberData.uid == LoginManager.Instance.LocalUserId.Value)
            {
                int heroId = AlliancesManager.Instance.GetMercenaryHeroId();
                CellController.SetItem(LTPartnerDataManager.Instance.GetPartnerByHeroId(heroId));
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.RemoveListener(EventName.LegionMercenaryUpdateUIDelay,NeedUpdateUI);
        }

        public override void Fill(LegionMemberData itemData)
        {
            SetData(itemData);
            SetItemBG(DataIndex);
        }
        
        public void SetData(LegionMemberData data)
        {
            if (data == null)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }

            mDMono.gameObject.CustomSetActive(true);
            memberData = data;
            HeadIcon.spriteName = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(data.templateId, data.skin).icon;
            HeadFrame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(data.headFrame).iconId;
            uid = data.uid;
            Name.text = data.memberName;
            Level.text = data.level.ToString();
            Duty.text = data.dutyTypeStr;
            bool isSelf = data.uid == LoginManager.Instance.LocalUserId.Value;
            SelfBGObj.CustomSetActive(isSelf);

            if (memberData.uid == LoginManager.Instance.LocalUserId.Value)
            {
                int HeroId = AlliancesManager.Instance.GetMercenaryHeroId();
                if (HeroId>0)
                {
                    LTPartnerData parData = LTPartnerDataManager.Instance.GetPartnerByHeroId(HeroId);
                    CellController.SetItem(parData);
                }
                else
                {
                    CellController.SetItem(null);
                }
            }
            else
            {
                List<LTPartnerData> hireDatas = AlliancesManager.Instance.hireDatas;
                LTPartnerData partner = hireDatas.Find((temp) => { return temp.uid == uid; });
                CellController.SetItem(partner);
            }
        }
        
        public void SetItemBG(int index)
        {
            // index: 0：浅底，1：深底
            BG.spriteName = index % 2 == 0 ? "Ty_Mail_Di1" : "Ty_Mail_Di2";
        }

        public override void Clean()
        {
        }
        
        void OnClickItem()
        {
            CellController.OnClick();
        }
    }
}