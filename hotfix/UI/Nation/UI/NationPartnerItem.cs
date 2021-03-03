using UnityEngine;

namespace Hotfix_LT.UI
{
    public class NationPartnerItem : DynamicMonoHotfix
    {
    	public string TeamName;
    	public int IndexInTeam;
    	public BoxCollider Collider;
    	public FormationPartnerItemEx PartnerItem;
    	//public NationPartnerItem ReferenceItem;
    	NationBattleFormationController formationCtrl;
    	GameObject SelectEffect;

        public override void Awake()
    	{
            var t = mDMono.transform;
    		Collider = t.GetComponentInChildren<BoxCollider>();
    		PartnerItem = PartnerItem?? t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Hotfix_LT.UI.FormationPartnerItemEx");
    		formationCtrl = formationCtrl ?? t.GetComponentInParent<UIControllerILR>().transform.GetUIControllerILRComponent<NationBattleFormationController>();		
    	
            if (mDMono.StringParamList != null)
            {
                var count = mDMono.StringParamList.Count;

                if (count > 0)
                {
                    TeamName = mDMono.StringParamList[0];
                }
            }

            if (mDMono.IntParamList != null)
            {
                var count = mDMono.IntParamList.Count;

                if (count > 0)
                {
                    IndexInTeam = mDMono.IntParamList[0];
                }
            }

            t.GetComponentEx<UIEventTrigger>().onDrag.Add(new EventDelegate(mDMono, "OnDragStart"));
            t.GetComponentEx<UIEventTrigger>().onDrag.Add(new EventDelegate(mDMono, "OnDrag"));
            t.GetComponentEx<UIEventTrigger>().onDrag.Add(new EventDelegate(mDMono, "OnDragEnd"));
        }
    
    	public void Fill(OtherPlayerPartnerData partnerData)
    	{
    		PartnerItem.mDMono.gameObject.SetActive(true);
    		PartnerItem.Fill(partnerData);
    	}
    
    	private void OnDrag()
    	{
    		formationCtrl.OnModelDrag();
    	}
    
    	private void OnDragStart()
    	{
    		if(!IsEmpty)
    			formationCtrl.OnModelDragStart(this);
    	}
    
    	private void OnDragEnd()
    	{
    		//if (!IsEmpty)
    			formationCtrl.OnModelDragEnd();
    	}
    
    	public OtherPlayerPartnerData PartnerData { get { return PartnerItem.PartnerData; } }
    	public bool IsEmpty { get { return PartnerItem.EmptyGO.activeSelf == true; } }
    }
}
