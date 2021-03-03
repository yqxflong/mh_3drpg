using System;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerTitleStateController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            m_Sprite = t.GetComponent<UISprite>("SpriteBg");
            M_CircleSprite = t.GetComponent<UISprite>("Sprite");
            if (mDMono.BoolParamList != null && mDMono.BoolParamList.Count > 0)
            {
                isSelect = mDMono.BoolParamList[0];
            }
            m_Label = mDMono.GetComponentInChildren<UILabel>();
            bool temp = isSelect == true ? SetSelect() : SetUnSelect();
        }
        private static readonly Color m_Color = new Color(10f/255f, 43f/255f, 74f/255f, 1f);
        public UISprite m_Sprite;
        private UILabel m_Label;
        public UISprite M_CircleSprite;
        public bool isSelect;

      
        public bool SetSelect()
        {
            m_Sprite.spriteName = "Huoban_Di_1";
            m_Label.color = Color.white;
            M_CircleSprite.color = Color.white;
            return true;
        }
    
        public bool SetUnSelect()
        {
            m_Sprite.spriteName = "";
            m_Label.color = m_Color;
            M_CircleSprite.color = m_Color;
            return true;
        }
        
    
    }
}
