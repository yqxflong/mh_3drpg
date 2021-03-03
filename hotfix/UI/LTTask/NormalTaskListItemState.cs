using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
	//挂在GameObject： Left
    public class NormalTaskListItemState : DataLookupHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDL.transform;
            m_normalItemFg = t.GetComponent<UISprite>("ProgressBar/Foreground");
            m_BtnGo = t.parent.FindEx("Btns/Go").gameObject;
            m_BtnReceive = t.parent.FindEx("Btns/Receive").gameObject;
            m_CompleteFlag = t.parent.FindEx("Btns/CompleteFlag").gameObject;
            m_TaskContext = t.GetComponent<UILabel>("TaskTips");

        }


    	private UINormalTaskScroll m_GridScroll;
    	
    	public UISprite m_normalItemFg;
    	// Use this for initialization
        public GameObject m_BtnGo;
        public GameObject m_BtnReceive;
    	public GameObject m_CompleteFlag;
    	public UILabel m_TaskContext;
    
    	private string m_lastState;
    	private string m_lastDefaultDataID;
    
    	public override void Start()
    	{
    		m_GridScroll =mDL.transform.parent.parent.GetMonoILRComponent<UINormalTaskScroll>();
    	}
    
    	public override void OnLookupUpdate(string dataID, object value)
    	{
            base.OnLookupUpdate(dataID, value);
            if (dataID != null)
            {
    	        
                string state=mDL.GetDefaultLookupData<string>();
    			if (state != null && state.Equals(TaskSystem.FINISHED))
    			{
    				// m_BG.spriteName = FinishedSpriteName;
    				//m_TaskContext.color = Color.green;
    				m_BtnGo.SetActive(false);
    				m_BtnReceive.SetActive(true);
    				m_CompleteFlag.SetActive(false);
    				if (m_normalItemFg != null)
    				{
    					m_normalItemFg.color = Color.white;
    					m_normalItemFg.spriteName = "Ty_Strip_Yellow";
    				}
    
    				if (m_GridScroll!=null && m_lastDefaultDataID.Equals(dataID) && m_lastState != null &&!m_lastState.Equals(state))
    				{
    					m_GridScroll.MoveTo(0);
    				}
    			}
    			else if (state != null && state.Equals(TaskSystem.COMPLETED))
    			{
    				//m_TaskContext.color = Color.white;
    				m_BtnGo.SetActive(false);
    				m_BtnReceive.SetActive(false);
    				m_CompleteFlag.SetActive(true);
    				if (m_normalItemFg != null)
    				{
    					m_normalItemFg.color = Color.magenta;
    					m_normalItemFg.spriteName = "Ty_Strip_Yellow";
    				}
    			}
    			else
    			{
    				//m_BG.spriteName = UnFinishedSpriteName;
    				//m_TaskContext.color = Color.white;
    				m_BtnGo.SetActive(true);
    				m_BtnReceive.SetActive(false);
    				m_CompleteFlag.SetActive(false);
    				if (m_normalItemFg != null)
    				{
    					m_normalItemFg.color = Color.white;
    					m_normalItemFg.spriteName = "Ty_Strip_Blue";
    				}	
    			
    			}
    
    			m_lastState = state;
    			m_lastDefaultDataID = dataID;
            }
        }
    }
}
