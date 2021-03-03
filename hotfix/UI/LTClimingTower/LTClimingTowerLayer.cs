using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
    /// <summary>
    /// 样式层
    /// </summary>
namespace Hotfix_LT.UI
{
    public class LTClimingTowerLayer : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            v_HaveSetData = false;
            v_Layerlabel = t.GetComponent<UILabel>("Sprite/Label");
            v_LeftDoorClick = t.FindEx("Left").gameObject;
            v_RightDoorClick = t.FindEx("Right").gameObject;
            leftDoor = t.GetMonoILRComponent<DoorLayer>("Left");
            rightDoor = t.GetMonoILRComponent<DoorLayer>("Right");
            UIEventListener.Get(v_LeftDoorClick).onClick = OnClickChallengeLeft;
            UIEventListener.Get(v_RightDoorClick).onClick = OnClickChallengeRight;
        }
    
    	public bool v_HaveSetData;
        public DoorLayer leftDoor;
        public DoorLayer rightDoor;
	    public UILabel v_Layerlabel;
        public GameObject v_LeftDoorClick;
        public GameObject v_RightDoorClick;
    	/// <summary>
    	/// 点击挑战 
    	/// </summary>
    	private Action<int,int, Action<Hashtable>> m_OnClickChallenge;
    	/// <summary>
    	/// 当前层的数据
    	/// </summary>
    	private Hotfix_LT.Data.ClimingTowerTemplate m_Data;

        private int diffculty;
    
    	private Stack<int> canOpenBattleReady;
    	private Coroutine m_Coroutine;

        private bool hideTip;

        private void OnClickChallengeLeft(GameObject go)
        {
	        if (m_Data == null || !m_Data.v_CanChallenge)
	        {
		        return;
	        }
	        
	        string key = LoginManager.Instance.LocalUserId.Value + "SleepTower" + EB.Time.LocalMonth+ EB.Time.LocalDay;
	        int temp= PlayerPrefs.GetInt(key, 0);
	        if (temp==0)
	        {
		        string content =string.Format(EB.Localizer.GetString("ID_SLEEPTOWER_TIP"), m_Data.sleep_Num);
		        MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, content, delegate (int r)
		        {
			        if (r == 0)
			        {
				        OnClickChallenge(go,1);
				        PlayerPrefs.SetInt(key, 1);
			        }
		        });
	        }
	        else
	        {
		        OnClickChallenge(go,1);
	        }
        }

        private void OnClickChallengeRight(GameObject go)
        {
	        if (m_Data == null || !m_Data.v_CanChallenge)
	        {
		        return;
	        }
	        OnClickChallenge(go,0);
        }
    
    	/// <summary>
    	/// 点击挑战当前层
    	/// </summary>
    	/// <param name="go"></param>
    	private void OnClickChallenge(GameObject go,int diffculty)
        {
	        this.diffculty = diffculty;
    		
    		//判断我的阵上可有睡眠的英雄
    		List<TeamMemberData> list = LTFormationDataManager.Instance.GetTeamMemList("lt_st");
    		canOpenBattleReady = new Stack<int>();
    		//将睡眠中的英雄下阵
    		if (list.Count != 0)
            {
                for (int i = 0; i < LTClimingTowerHudController.Instance.v_CurrentLayerData.v_SleepHero.Length; i++)
                {
                    int sleepHeroId = LTClimingTowerHudController.Instance.v_CurrentLayerData.v_SleepHero[i];
                    if (list.Find(p => p.HeroID == sleepHeroId) != null)
                    {
                        canOpenBattleReady.Push(sleepHeroId);
                        LTFormationDataManager.Instance.RequestRemoveHeroFormation(LTClimingTowerHudController.Instance.v_CurrentLayerData.v_SleepHero[i], "lt_st", delegate ()
                        {
                            canOpenBattleReady.Pop();
                        });
                    }
                }
            }
    		
    		if (m_Coroutine != null)
    		{
    			StopCoroutine(m_Coroutine);
    			m_Coroutine = null;
    		}
    		m_Coroutine = StartCoroutine(OpenBattleReadyUI());
    	}
    
    	private IEnumerator OpenBattleReadyUI()
    	{
    		while(true)
    		{
    			if (canOpenBattleReady.Count == 0)
    			{
                    //打开防守界面
                    BattleReadyHudController.Open(eBattleType.SleepTower, OnClickStart,LTClimingTowerHudController.Instance.v_CurrentLayerData.v_Layout);
    				yield break;
                }
    			yield return null;
            }
        }
    
    	private void OnClickStart()
    	{
    		List<TeamMemberData> list = LTFormationDataManager.Instance.GetTeamMemList("lt_st");
    		if (list.Count == 0)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_guide_words_902314_context"));
    			return;
    		}
    		if (m_OnClickChallenge != null)
    		{
                UIControllerILR uiController = GlobalMenuManager.Instance.GetMenu<UIControllerILR>("LTCombatReadyUI");
                BattleReadyHudController controller = uiController.ilinstance as BattleReadyHudController;
                if (controller != null)
    			{
    				controller.mStartBattleClick = true;
    			}
    			m_OnClickChallenge(m_Data.layer,diffculty, (re) =>
                {
	                DataLookupsCache.Instance.CacheData(re);
                });
    		}
        }

        public void F_SetData(Action<int, int, Action<Hashtable>> onClickChallenge, Data.ClimingTowerTemplate data,int layerArgs)
        {
	        m_Data = data;
	        m_OnClickChallenge = onClickChallenge;
	        v_HaveSetData = true;
	        v_LeftDoorClick.GetComponent<BoxCollider>().enabled = data != null;
	        v_RightDoorClick.GetComponent<BoxCollider>().enabled = data != null;
	        leftDoor.SetData(data, 1);
	        rightDoor.SetData(data, 0);
	        int curLayer = LTClimingTowerManager.Instance.v_CurrentLayerData.v_CurrentLayer;
            LTUIUtil.SetText(v_Layerlabel, layerArgs.ToString());
            v_Layerlabel.color = layerArgs < curLayer ? Color.green :
                layerArgs > curLayer ? Color.white : Color.yellow;
        }
    }
}
