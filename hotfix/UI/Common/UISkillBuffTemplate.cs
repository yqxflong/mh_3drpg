using UnityEngine;

namespace Hotfix_LT.UI
{
    public class UISkillBuffTemplate : DynamicMonoHotfix
    {
        public UISprite IconSprite;
        public UILabel NameLabel;
        public UILabel DescLabel;
        public UIWidget BGWidget;
        public int DefBGHeight = 126;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0])
                {
                    IconSprite = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<UISprite>();
                }
                if (count > 1 && mDMono.ObjectParamList[1])
                {
                    NameLabel = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UILabel>();
                }
                if (count > 2 && mDMono.ObjectParamList[2])
                {
                    DescLabel = ((GameObject)mDMono.ObjectParamList[2]).GetComponentEx<UILabel>();
                }
                if (count > 3 && mDMono.ObjectParamList[3])
                {
                    BGWidget = ((GameObject)mDMono.ObjectParamList[3]).GetComponentEx<UIWidget>();
                }
            }

            if (mDMono.IntParamList != null)
            {
                var count = mDMono.IntParamList.Count;

                if (count > 0)
                {
                    DefBGHeight = mDMono.IntParamList[0];
                }
            }
        }

        public void SetData(Hotfix_LT.Data.BuffTemplate buff)
        {
            if (buff != null)
            {
                IconSprite.spriteName = buff.Buff;
                NameLabel.text = buff.Name;
                DescLabel.text = buff.Description;
    
                BGWidget.height = GetBuffTempHeight();
                mDMono.gameObject.CustomSetActive(true);
            }
            else
            {
                Hide();
            }
        }
    
        public void Hide()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    
        public int GetBuffTempHeight()
        {
            return DefBGHeight + DescLabel.height;
        }
    }
}
