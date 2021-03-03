namespace Hotfix_LT.UI
{
    public class MainMenuChatItem : DynamicMonoHotfix
    {
    	public UISymbolLabel previewLabel;
        public UISprite ChannelSprite;
        public UILabel NameLabel;
        public bool isChatWindow = false;

        private ChatUIMessage itemData;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            previewLabel = t.GetComponent<UISymbolLabel>("PreviewLabel");
            ChannelSprite = t.GetComponent<UISprite>("Sprite");
            NameLabel = t.GetComponent<UILabel>("NameLabel", false);

            if (mDMono.BoolParamList != null)
            {
                var count = mDMono.BoolParamList.Count;

                if (count > 0)
                {
                    isChatWindow = mDMono.BoolParamList[0];
                }
            }
        }

        public void SetItemData(object _data)
    	{
    		itemData = _data as ChatUIMessage;
    		FreshItem();
    	    //ChannelSprite.transform.localPosition = new Vector3(-360f, -17f, 0f);
    
            //if (previewLabel.height == 200)
    	       // ChannelSprite.transform.localPosition+=new Vector3(0f,-145f,0f);
    	}
    
        public void SetSysItemData(object _data) {
            itemData = _data as ChatUIMessage;
            FreshItem();
        }
    
        private void FreshItem()
        {
            previewLabel.text = null;
            if (itemData != null)
    		{
                if (NameLabel != null)
                {
                    NameLabel.text = itemData.GetNameString();
                    string kg = itemData.Channel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM ? "" : "  ";
                    previewLabel.text = string.Format("[ffffff00]{0}[-]{1}", NameLabel.text, kg); 
                }
                previewLabel.text = string.Format("{0}{1}", previewLabel.text, itemData.GetPreviewString());
                if (isChatWindow)
                {
                    ChannelSprite.gameObject.SetActive(true);
                }
                else
                {
                    ChannelSprite.spriteName = "Ty_Di_2";
    
                    if (ChatItem.ChannelSpriteColor.ContainsKey(itemData.Channel))
                    {
                        ChannelSprite.color = ChatItem.ChannelSpriteColor[itemData.Channel];
                        ChannelSprite.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString(ChatItem.ChannelName[itemData.Channel]);
                    }
                }
            }
    		else
    		{
                if (isChatWindow)
                {
                    ChannelSprite.gameObject.SetActive(false);
                }
                else
                {
                    ChannelSprite.spriteName = null;
                }
            }
    	}
    }
}
