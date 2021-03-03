using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class NationBattleScoreBoardCell : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            NameLabel = t.GetComponent<UILabel>("Name");
            ScoreLabel = t.GetComponent<UILabel>("Score");

        }


    
    	public UILabel NameLabel;
    	public UILabel ScoreLabel;
    
    	public void Fill(NationScore scoreData)
    	{
    		if (scoreData != null)
    		{
    			LTUIUtil.SetText(NameLabel, NationUtil.LocalizeNationName(scoreData.Name));
    			LTUIUtil.SetText(ScoreLabel, scoreData.Score.ToString());
    		}
    		else
    		{
    			LTUIUtil.SetText(NameLabel, EB.Localizer.GetString("ID_codefont_in_NationBattleScoreBoardCell_431"));
    			LTUIUtil.SetText(ScoreLabel, "");
    		}		
    	}
    }
}
