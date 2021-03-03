using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceThemeItemTemp : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            NameLabel = t.GetComponent<UILabel>("ChallengeInstance/Title/Desc");
            Tex = t.GetComponent<CampaignTextureCmp>("ChallengeInstance/Texture");
            FX = t.FindEx("ChallengeInstance/FX").gameObject;
            MazeNameLabel = t.GetComponent<UILabel>("AlienMazeInstance/MainIcon/Title/Desc");
            MazeSprite = t.GetComponent<UISprite>("AlienMazeInstance/MainIcon");
            challengeInstanceRoot = t.FindEx("ChallengeInstance").gameObject;
            alienMazeInstanceRoot = t.FindEx("AlienMazeInstance").gameObject;

        }
        
        public UILabel NameLabel;
        public CampaignTextureCmp Tex;
        public GameObject FX;
    
        public UILabel MazeNameLabel;
        public UISprite MazeSprite;
    
        public GameObject challengeInstanceRoot, alienMazeInstanceRoot;
    
        public void InitData(Hotfix_LT.Data.LostChallengeEnv data,bool isAlienMaze=false)
        {
            challengeInstanceRoot.CustomSetActive(!isAlienMaze);
            alienMazeInstanceRoot.CustomSetActive(isAlienMaze);
            if (!isAlienMaze)
            {
                NameLabel.text = data.Name;
                Tex.spriteName = data.Pic;
            }
            else
            {
                MazeNameLabel.text = data.Name;
                MazeSprite.spriteName = data.Pic;
            }
    
        }
    
        public void ShowTween()
        {
            MazeSprite.GetComponent<TweenAlpha>().ResetToBeginning();
            MazeSprite.GetComponent<TweenAlpha>().PlayForward();
        }
    
        public void ShowFx()
        {
            FX.CustomSetActive(true);
        }
    }
}
