using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class NationDonateData
    {
    	public int Rank=-1;
    	public long Uid;
    	public string Name;
    	public int Level;
    	public string Portrait;
    	public int WeekDonate;
    
    	public NationDonateData(Hashtable value)
    	{
    		Rank = EB.Dot.Integer("r", value, Rank);
    		Uid = EB.Dot.Long("u", value, Uid);
    		Name = EB.Dot.String("un", value, "");
    		Level = EB.Dot.Integer("l", value, 0);
    		int tplId = EB.Dot.Integer("t_id", value, 10011);
    		if (tplId > 0)
    		{
    			var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(tplId);
    			if (tpl == null)
    			{
    				tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(10011);
    			}
    			if (tpl != null)
    			{
    				var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(tpl.character_id);
    				if (heroInfo != null)
    				{
    					Portrait = heroInfo.icon;
    				}
    				else
    				{
    					EB.Debug.LogError("cannot found heroInfo for c_id = {0}", tpl.character_id);
    				}
    			}
    		}
    		WeekDonate = EB.Dot.Integer("donate", value, 0);
    	}
    }
    
    public class NationBattleDonateListController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            MyDonateInfo = t.GetComponent<UILabel>("Content/MyInfo");
            controller.backButton = t.GetComponent<UIButton>("LTPopFrame/CloseBtn");
            DonateCellArray = new NationDonateItemCell[4];
            DonateCellArray[0] = t.GetMonoILRComponent<NationDonateItemCell>("Content/List/0");
            DonateCellArray[1] = t.GetMonoILRComponent<NationDonateItemCell>("Content/List/1");
            DonateCellArray[2] = t.GetMonoILRComponent<NationDonateItemCell>("Content/List/2");
            DonateCellArray[3] = t.GetMonoILRComponent<NationDonateItemCell>("Content/List/3");
        }


    
    	public override bool ShowUIBlocker { get { return true; } }
    
        public UILabel MyDonateInfo;
    	public NationDonateItemCell[] DonateCellArray;
    
    	public override IEnumerator OnAddToStack()
    	{
    		var coroutine = EB.Coroutines.Run(base.OnAddToStack());
    		NationManager.Instance.DonateRank(delegate (Hashtable result)
    		{
    			if (result != null)
    			{
    				UpdateUI(result);
    			}
    			else
    			{
    				controller.Close();
    			}
    		});
    		yield return coroutine;
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		DestroySelf();
    		yield break;
    	}
    
    	void UpdateUI(Hashtable result)
    	{
    		ArrayList array = Hotfix_LT.EBCore.Dot.Array("rank.personal.nationDegree", result, null);
    		List<NationDonateData> rankdatas = new List<NationDonateData>();
    		long localPlayerId = LoginManager.Instance.LocalUserId.Value;
    		NationDonateData localPlayerRankData = null;
    		for (int i = 0; i < array.Count; i++)
    		{
    			var data = new NationDonateData(array[i] as Hashtable);
    			if (data.Uid == localPlayerId)
    			{
    				localPlayerRankData = data;
    			}
    			else if (data.Rank >= 0 && data.Rank < 4)
    			{
    				rankdatas.Add(data);
    			}
    		}
    		if (localPlayerRankData != null && localPlayerRankData.Rank >= 0 && localPlayerRankData.Rank< 4)
    		{
    			rankdatas.Add(localPlayerRankData);
    		}
    		rankdatas.Sort(delegate(NationDonateData x,NationDonateData y) { return x.Rank - y.Rank; });
    
    		while (rankdatas.Count < 4)
    		{
    			rankdatas.Add(null);
    		}
    		for (int dataIndex = 0; dataIndex < rankdatas.Count; ++dataIndex)
    		{
    			DonateCellArray[dataIndex].Fill(rankdatas[dataIndex]);
    		}
    		if (localPlayerRankData != null)
    		{
    			LTUIUtil.SetText(MyDonateInfo, string.Format("{0}.   {1}   {2}",localPlayerRankData.Rank+1,localPlayerRankData.Name,localPlayerRankData.WeekDonate));
    		}
    		else
    		{
    		    LTUIUtil.SetText(MyDonateInfo, string.Format(EB.Localizer.GetString("ID_codefont_in_NationBattleDonateListController_2991"), BalanceResourceUtil.GetUserName()));
    		}
        }
    }
}
