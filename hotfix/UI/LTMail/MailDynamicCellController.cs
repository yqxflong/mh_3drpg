using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class MailDynamicCellController : DynamicCellController<MailItemData>
    {
        public UISprite Icon;
        public UILabel TitleLabel, SenderLabel, TimeLabel;
        public Transform Fujian;
        public UISprite BGSprite;
        public UISprite MailboxBGSprite;
        public MailItemData ItemData;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Icon = t.GetComponent<UISprite>("Icon");
            TitleLabel = t.GetComponent<UILabel>("Title");
            SenderLabel = t.GetComponent<UILabel>("Sender/Name");
            TimeLabel = t.GetComponent<UILabel>("Time");
            Fujian = t.GetComponent<Transform>("Fujian");
            BGSprite = t.GetComponent<UISprite>("BG");
            MailboxBGSprite = t.GetComponent<UISprite>("Icon/BG");

            var mailController = t.parent.parent.parent.parent.parent.GetUIControllerILRComponent<MailController>();
            var eventTrigger = t.GetComponent<UIEventTrigger>();

            if (eventTrigger != null)
            {
                eventTrigger.onClick.Add(new EventDelegate(() => mailController.OnClickMailCell(ItemData)));
                eventTrigger.onClick.Add(new EventDelegate(mailController.RightContentPlayAmi));
            }
        }

    	public override void Clean()
    	{
    
    	}
    
    	public override void Fill(MailItemData itemdata)
    	{
    		this.ItemData = itemdata;
    		Icon.spriteName = itemdata.HasRead ? "Mail_Icon_Yidu" : "Mail_Icon_Weidu";
    		LTUIUtil.SetText(TitleLabel, EB.Localizer.GetString(itemdata.Title));
    		LTUIUtil.SetText(SenderLabel, EB.Localizer.GetString(itemdata.Sender));
    		LTUIUtil.SetText(TimeLabel,EB.Time.FromPosixTime(itemdata.Time).ToLocalTime().ToString("yyyy/MM/dd"));
    		Fujian.gameObject.SetActive(itemdata.ItemCount > 0 && !itemdata.HasReceived);
    		if (itemdata.IsSelect)
    			BGSprite.spriteName = "Ty_Mail_Di3";
    		else
    			BGSprite.spriteName = DataIndex % 2 == 0 ? "Ty_Mail_Di1" : "Ty_Mail_Di2";
    
    		if (!itemdata.IsSelect)
    			MailboxBGSprite.color = new Color32(0, 120, 255, 100);
    		else
    			MailboxBGSprite.color = new Color32(57,255,130,100);
    	}
    }
}
