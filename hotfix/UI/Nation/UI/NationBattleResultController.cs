using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class NationBattleResultController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;

            t.GetComponent<UIEventTrigger>("Bg").onClick.Add(new EventDelegate(OnCancelButtonClick));
            OccupyCityNumEntrys = new List<OccupyCityNumEntry>();
            OccupyCityNumEntry tempitem = new OccupyCityNumEntry();
            tempitem.Name = "persian";
            tempitem.NumLabel = t.GetComponent<UILabel>("Content/NationGrid/GameObject/CityNum");
            OccupyCityNumEntrys.Add(tempitem);
            tempitem.Name = "roman";
            tempitem.NumLabel = t.GetComponent<UILabel>("Content/NationGrid/GameObject (1)/CityNum");
            OccupyCityNumEntrys.Add(tempitem);
            tempitem.Name = "egypt";
            tempitem.NumLabel = t.GetComponent<UILabel>("Content/NationGrid/GameObject (2)/CityNum");
            OccupyCityNumEntrys.Add(tempitem);

        }


    
    	//public override bool ShowUIBlocker { get { return true; } }
    
    	[System.Serializable]
    	public class OccupyCityNumEntry
    	{
    		public string Name;
    		public UILabel NumLabel;
    	}
    
    	public List<OccupyCityNumEntry> OccupyCityNumEntrys;	
    
    	public override IEnumerator OnAddToStack()
    	{
    	    FusionAudio.PostEvent("UI/New/GongXian",true);
    		NationManager.Instance.GetTerritoryInfo(null);
    		GameDataSparxManager.Instance.RegisterListener(NationManager.TerritoryDataId, OnTerritoryListListener);
    		return base.OnAddToStack();
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		GameDataSparxManager.Instance.UnRegisterListener(NationManager.TerritoryDataId, OnTerritoryListListener);
    		DestroySelf();
    		yield break;
    	}
    
    	void OnTerritoryListListener(string path, INodeData data)
    	{
    		for (var i = 0; i < NationManager.Instance.List.Members.Count; i++)
    		{
                var nation = NationManager.Instance.List.Members[i];
                int territory_count = System.Array.FindAll(NationManager.Instance.TerritoryList.TerritoryList, m => m.Owner == nation.Name).Length;
    			var entry = OccupyCityNumEntrys.Find(m => m.Name == nation.Name);
    			if (entry != null)
    				LTUIUtil.SetText(entry.NumLabel, EB.Localizer.GetString("ID_codefont_in_NationBattleResultController_1216") + territory_count);
    			else
    				EB.Debug.LogError("nation name is {0}",nation.Name);
    		}
    	}
    
    	public override void OnCancelButtonClick()
    	{
    		base.OnCancelButtonClick();
    
    		UIStack.Instance.ExitStack(false);
    	}
    }
}
