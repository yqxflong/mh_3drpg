using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class NationItem : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                int count = mDMono.ObjectParamList.Count;
                if (mDMono.ObjectParamList[0] != null)
                {
                    FlagBGUpSpriteLeft = (mDMono.ObjectParamList[0] as GameObject).GetComponent<UISprite>();
                }
                if (count>1 && mDMono.ObjectParamList[1] != null)
                {
                    FlagBGUpSpriteRight = (mDMono.ObjectParamList[1] as GameObject).GetComponent<UISprite>();
                }
                if(count >2&& mDMono.ObjectParamList[2] != null)
                {
                    FlagBGDownSpriteLeft = (mDMono.ObjectParamList[2] as GameObject).GetComponent<UISprite>();
                }
                if(count > 3 && mDMono.ObjectParamList[3] != null)
                {
                    FlagBGDownSpriteRight = (mDMono.ObjectParamList[3] as GameObject).GetComponent<UISprite>();
                }
                if (count > 4 && mDMono.ObjectParamList[4] != null)
                {
                    IconSprite = (mDMono.ObjectParamList[4] as GameObject).GetComponent<UISprite>();
                }
                if (count > 4 && mDMono.ObjectParamList[4] != null)
                {
                    IconSprite = (mDMono.ObjectParamList[4] as GameObject).GetComponent<UISprite>();
                }
                if(count > 5 && mDMono.ObjectParamList[5] != null)
                {
                    NameLabel = (mDMono.ObjectParamList[5] as GameObject).GetComponent<UILabel>();
                }
            }

        }


    
    	public UISprite FlagBGUpSpriteLeft, FlagBGUpSpriteRight;
    	public UISprite FlagBGDownSpriteLeft, FlagBGDownSpriteRight;
    	public UISprite IconSprite;
    	public UILabel NameLabel;
    
    	public void Fill()
    	{
    
    	}
    }
}
