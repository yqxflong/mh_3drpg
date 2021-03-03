using UnityEngine;
using System.Collections;
using System;
    
namespace Hotfix_LT.UI
{
    public class LegionApproveItem : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            nameLabel = t.GetComponent<UILabel>("NameLabel");
            levelLabel = t.GetComponent<UILabel>("LevelSprite/LevelLabel");
            headIcon = t.GetComponent<UISprite>("Border/Icon");
            frameIcon = t.GetComponent<UISprite>("Border/Icon/Frame");
            consentBtn = t.GetComponent<UIButton>("ConsentButton");
            refuseBtn = t.GetComponent<UIButton>("RefuseButton");
            consentBtn.onClick.Add(new EventDelegate(OnClickConsent));
            refuseBtn.onClick.Add(new EventDelegate(OnClickRefuse));

        }

        public override void OnDestroy()
        {
            onClickConsentApprove = null;
            onClickRefuseApprove = null;
        }

        private long approveID;
        public UILabel nameLabel;
        public UILabel levelLabel;
        public UISprite headIcon;
        public UISprite frameIcon;
        public UIButton consentBtn;
        public UIButton refuseBtn;
    
        private Action<long> onClickConsentApprove;
        private Action<long> onClickRefuseApprove;
    
        private RequestJoinData data;    
    
        
    
        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.SetActive(isShow);
        }
    
        public void SetData(RequestJoinData data)
        {
            this.data = data;
            approveID = data.approveID;
    
            nameLabel.text = data.name;
            levelLabel.text = data.level.ToString();
            headIcon.spriteName = data.headIcon;// "Partner_Head_Sidatuila"; // 暂时默认
            frameIcon.spriteName = Data .EconemyTemplateManager .Instance .GetHeadFrame( data.headFrame).iconId;
            ShowUI(true);
        }
    
        public void SetAction(Action<long> consent = null, Action<long> refuse = null)
        {
            onClickConsentApprove = consent;
            onClickRefuseApprove = refuse;
        }
    
        private float clickTime;
        private bool isCouldClick()
        {
            if (Time.time - clickTime < 0.5f)
            {
                return false;
            }
            clickTime = Time.time;
            return true;
        }
    
        void OnClickConsent()
        {
            if (!isCouldClick()) return;
    
            if (onClickConsentApprove!=null)
            {
                onClickConsentApprove(approveID);
            }
        }
    
        void OnClickRefuse()
        {
            if (!isCouldClick()) return;
    
            if (onClickRefuseApprove!=null)
            {
                onClickRefuseApprove(approveID);
            }
        }
    }
}
