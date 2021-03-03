using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 战斗界面类型
/// </summary>
public enum eCombatHudMode
{
    /// <summary>
    /// 普通
    /// </summary>
    Normal,
    /// <summary>
    /// 只有互动?
    /// </summary>
    OnlyInteract,
    /// <summary>
    /// 自动战斗
    /// </summary>
    AutoInteract,
    /// <summary>
    /// 观战
    /// </summary>
    Observe,
    /// <summary>
    /// 回放
    /// </summary>
    Playback,
    None,
}

/// <summary>
/// 战斗业务逻辑管理器
/// </summary>
public class CombatLogic
{
    /// <summary>
    /// 战斗阵容
    /// </summary>
	public enum CombatTeam
	{
        /// <summary>
        /// 挑战
        /// </summary>
		kChallenger,
        /// <summary>
        /// 防守
        /// </summary>
		kDefender,
        /// <summary>
        /// 观战
        /// </summary>
		kObserver
	}
    /// <summary>
    /// 阵容类型
    /// </summary>
	public enum FormationSide
	{
        /// <summary>
        /// 玩家或者挑战者
        /// </summary>
		PlayerOrChallenger,
        /// <summary>
        /// 对手（配置表的敌人?）
        /// </summary>
		Opponent
	}
    
	private static CombatLogic s_instance = null;

	private CombatLogic()
	{
		CombatId = -1;
	}

	public static CombatLogic Instance
	{
		get { return s_instance = s_instance ?? new CombatLogic(); }
	}

	public bool Ready
	{
		get { return CombatId >= 0; }
	}
    
	public FormationSide[] Sides
	{
		get;
		private set;
	}

	public int LocalPlayerTeamIndex
	{
		get;
		private set;
	}
    
	public int ChallengerTeamIndex
	{
		get { return (int)CombatTeam.kChallenger; }
	}

	public int DefenderTeamIndex
	{
		get { return (int)CombatTeam.kDefender; }
	}

	public int ObserverTeamIndex
	{
		get { return (int)CombatTeam.kObserver; }
	}

	public bool LocalPlayerIsChallenger
	{
		get { return LocalPlayerTeamIndex == ChallengerTeamIndex; }
	}

	public bool LocalPlayerIsDefender
	{
		get { return LocalPlayerTeamIndex == DefenderTeamIndex; }
	}

	public bool LocalPlayerIsObserver
	{
		get { return LocalPlayerTeamIndex == ObserverTeamIndex; }
	}
    
        /// <summary>
        /// 当前的战斗唯一id
        /// </summary>
	public int CombatId
	{
		get;
		private set;
	}
    
	public eCombatHudMode DefaultHudMode
	{
		get;
		private set;
	}

	public bool Paused
	{
		get;
		set;
	}
    
	public FormationSide GetSide(int teamIndex)
	{		
        if(Sides==null)
        {
			return FormationSide.PlayerOrChallenger;
		}
		return Sides[teamIndex];
	}
    
	public bool IsPlayerOrChallengerSide(int teamIndex)
	{
		return GetSide(teamIndex) == FormationSide.PlayerOrChallenger;
	}
    
	public bool IsOpponentSide(int teamIndex)
	{
		return GetSide(teamIndex) == FormationSide.Opponent;
	}

    /// <summary>
    /// 初始化一场战斗的数据
    /// </summary>
    /// <param name="transition_data">服务器传递的数据</param>
	public void SetCombatDataFromCampaign(IDictionary transition_data)
	{
		if (transition_data is Hashtable == false)
		{
			EB.Debug.LogError("Wrong format for combat transition data!");
			return;
		}

		Hashtable data = transition_data as Hashtable;
		if (!data.ContainsKey("combat"))
		{
			EB.Debug.LogError("Wrong combat transition data format!");
			return;
		}

		Hashtable combat_data = data["combat"] as Hashtable;
		if (combat_data == null)
		{
			EB.Debug.LogError("Wrong combat transition data format!");
			return;
		}

		// Collect Combat Id
		int combatID = EB.Dot.Integer("combatId", combat_data, -1);
		if (combatID <= 0)
		{
			EB.Debug.LogError("combatId not found");
			return;
        }
        
		LocalPlayerTeamIndex = int.Parse(combat_data["myTeamIndex"].ToString());
		//DefaultHudMode = CombatUtil.ParseHudMode(EB.Dot.String("hudMode", combat_data, string.Empty));
		DefaultHudMode = ParseHudMode(EB.Dot.String("hudMode", combat_data, string.Empty));

		Sides = new FormationSide[System.Enum.GetValues(typeof(FormationSide)).Length];
		if (LocalPlayerIsChallenger || LocalPlayerIsObserver)
		{
			Sides[ChallengerTeamIndex] = FormationSide.PlayerOrChallenger;
			Sides[DefenderTeamIndex] = FormationSide.Opponent;
		}
		else if (LocalPlayerIsDefender)
		{
			Sides[ChallengerTeamIndex] = FormationSide.Opponent;
			Sides[DefenderTeamIndex] = FormationSide.PlayerOrChallenger;
		}

		if (combatID == 0)
		{
			DefaultHudMode = eCombatHudMode.OnlyInteract;
		}
		else if (LocalPlayerIsObserver)
		{
			DefaultHudMode = eCombatHudMode.Observe;
		}
        CombatId = combatID;
    }
	
	public  eCombatHudMode ParseHudMode(string mode)
	{
		switch (mode)
		{
			case "autoInteract":
				return eCombatHudMode.AutoInteract;
			case "onlyInteract":
				return eCombatHudMode.OnlyInteract;
			default:
				return eCombatHudMode.Normal;
		}
	}

	/// <summary>
	/// 退出战斗
	/// </summary>
	public void ExitCombat()
	{
		if (CombatId < 0)
		{
			return;
		}

		GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "ExitCombat");
		CombatId = -1;
		LocalPlayerTeamIndex = -1;
		Sides = null;
		DefaultHudMode = eCombatHudMode.Normal;
	}

	public static void Dispose()
	{

	}
}
