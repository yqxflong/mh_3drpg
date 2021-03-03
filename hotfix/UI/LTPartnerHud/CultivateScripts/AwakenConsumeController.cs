using EB.IAP;
using UnityEngine;

namespace Hotfix_LT.UI
{
	public class AwakenConsumeController : DynamicMonoHotfix
	{
		public UISprite ItemIcon;
		public UILabel NumLabel, NumShader;
		public bool isShowGetRoad;
		private string curItemId;
		private UIButton itemBtn;
		public override void Awake()
        {
			ItemIcon = mDMono.transform.Find("Sprite").GetComponent<UISprite>();
			NumLabel = mDMono.transform.Find("Label").GetComponent<UILabel>();
			NumShader = mDMono.transform.Find("Label/Label(Clone)").GetComponent<UILabel>();
			itemBtn = mDMono.transform.GetComponent<UIButton>();
            if (itemBtn.onClick != null)
            {
				itemBtn.onClick.Clear();
            }
			itemBtn.onClick.Add(new EventDelegate(OnClickItem));
			isShowGetRoad = true;		
        }

		public void Fill(int templateId, string Icon)
		{
			curItemId = templateId.ToString();
			ItemIcon.spriteName = Icon;
		}
		public void OnClickItem()
		{
			UITooltipManager.Instance.DisplayTooltipSrc(curItemId, "Generic", "default");
		}
	}
}
