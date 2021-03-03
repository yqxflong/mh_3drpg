namespace Hotfix_LT.UI
{
    public class CurrencyDisplay : DynamicMonoHotfix
    {
        private UISprite _sprite;
        private UILabel _label;

        public void SetData(string count, string spriteName)
        {
            SetCount(count);
            SetIcon(spriteName);
        }

        public void SetCount(string count)
        {
            if (_label == null)
            {
                _label = mDMono.transform.GetComponent<UILabel>("Label");
            }

            if (_label != null)
            {
                _label.text = string.IsNullOrEmpty(count) ? "0" : count;
            }
        }

        public void SetIcon(string spriteName)
        {
            if (_sprite == null)
            {
                _sprite = mDMono.transform.GetComponent<UISprite>("Sprite");
            }

            if (_sprite != null)
            {
                _sprite.spriteName = spriteName;
            }
        }

		public void SetPopTip(string itemType, string id)
		{
			UIEventListener trigger = mDMono.transform.GetComponent<UIEventListener>("Sprite", false);

			if (trigger == null)
			{
				trigger = mDMono.transform.Find("Sprite").gameObject.AddComponent<UIEventListener>();
			}

			trigger.onClick = (go) =>
			{
				FusionAudio.PostEvent("UI/General/ButtonClick");
				LTResToolTipController.Show(itemType, id);
			};			
		}
	}
}