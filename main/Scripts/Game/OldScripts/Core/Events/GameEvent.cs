///////////////////////////////////////////////////////////////////////
//
//  GameEvent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

//--------------------------------------------------
/// UI事件ID
//--------------------------------------------------    
public enum eSimpleEventID
{
    Invalid = -1,//无效事件
    None = 0,

    CombatCleanUpEvent = 1001,
    
    Combat_Enter = 4001,
    Combat_Exit = 4002,

    MAX_Tag,
    //别在后面加东西
};

public class GameEvent : IPoolable
{

    protected eSimpleEventID mEventID = eSimpleEventID.Invalid;
    public eSimpleEventID EventID
    {
        get { return mEventID; }
        set { mEventID = value; }
    }

    // derived clases implement event properties here
    public void OnPoolActivate()
	{
		Reset();
	}

	public void OnPoolDeactivate()
	{
		Reset();
	}

	protected virtual void Reset() //Clear State
	{

	}
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class VisibleAtDesignLevelAttribute : System.Attribute {}
