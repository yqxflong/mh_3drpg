using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTInstanceMessageCell : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            IconSprite = t.GetComponent<DynamicUISprite>("Icon");
            NameLabel = t.GetComponent<UILabel>("Name");
            DescLabel = t.GetComponent<UILabel>("Message");
        }
        
        public DynamicUISprite IconSprite;
    
        public UILabel NameLabel;
    
        public UILabel DescLabel;
    
        public void SetData(Hotfix_LT.Data.LostInstanceMessage data)
        {
            if (data != null)
            {
                IconSprite.spriteName = data.Icon;
                NameLabel.text = data.Name;
                DescLabel.text = data.Desc;
            }
        }
    }
}
