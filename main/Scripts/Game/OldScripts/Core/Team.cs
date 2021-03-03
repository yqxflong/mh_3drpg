///////////////////////////////////////////////////////////////////////
//
//  Team.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

public enum eTeamId
{
	Player,
    Enemy,
    Destructible,
    Interactable,
}

public static class TeamEnumExtensions 
{
	public static bool IsEnemy(this eTeamId us, eTeamId them)
	{
		if (them == eTeamId.Interactable || us == eTeamId.Interactable)
			return false;

		return us != them;
	}

	public static bool IsAlly(this eTeamId us, eTeamId them)
	{
		if (them == eTeamId.Interactable || us == eTeamId.Interactable)
			return true;

		return us == them;
	}
}
