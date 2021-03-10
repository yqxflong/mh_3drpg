///////////////////////////////////////////////////////////////////////
//
//  EventManager.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
//  Portions of this code (core event system, excluding commands) written by Will R. Miller (Firaxis)
//   provided from web link: http://www.willrmiller.com/?p=87
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using UnityEngine;

public class EventManager
{
	static EventManager instanceInternal = null;
	public static EventManager instance
	{
		get
		{
			if (instanceInternal == null)
			{
				instanceInternal = new EventManager();
			}
			
			return instanceInternal;
		}
	}
	
	public delegate void EventDelegate<T> (T e) where T : GameEvent;
	private delegate void EventDelegate (GameEvent e);
	
	private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
	private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();
	
	public Dictionary<System.Type, List<System.Type>> commandMap = new Dictionary<System.Type, List<System.Type>>();

	public void CleanUpEvents()
	{

	}

    #region 简单的分发逻辑
    //通用对象，避免内存碎片
    private EventDelegate<GameEvent>[] mSimpleEvents = new EventDelegate<GameEvent>[(int)eSimpleEventID.MAX_Tag];
    private Dictionary<uint, GameEvent> mGameEvents = new Dictionary<uint, GameEvent>();
    /// <summary>
    /// 根据eventid直接分发事件
    /// </summary>
    /// <param name="eventId"></param>
    public void Raise(eSimpleEventID eventId)
    {
        EventDelegate<GameEvent> onEventHandlers = mSimpleEvents[(uint)eventId];

        if (onEventHandlers != null) 
        {
            GameEvent e;
            mGameEvents.TryGetValue((uint)eventId, out e);
            if (e == null)
            {
                e = new GameEvent();
                e.EventID = eventId;
                mGameEvents.Add((uint)eventId, e);
            }
            onEventHandlers(e);
        }
    }

    public void AddListener(eSimpleEventID eventId, EventDelegate<GameEvent> del)
    {
        if (mSimpleEvents[(uint)eventId] == null)
        {
            mSimpleEvents[(uint)eventId] = delegate { };
            mSimpleEvents[(uint)eventId] += del;
        }
        else
        {
            //防止重复添加委托函数
            mSimpleEvents[(uint)eventId] -= del;
            mSimpleEvents[(uint)eventId] += del;
        }
    }
    public void RemoveListener(eSimpleEventID eventId, EventDelegate<GameEvent> del)
    {
        if (mSimpleEvents[(uint)eventId] != null)
        {
            mSimpleEvents[(uint)eventId] -= del;
        }
    }
    #endregion



    public void AddListener<T> (EventDelegate<T> del) where T : GameEvent
	{
		// Early-out if we've already registered this delegate
		if (delegateLookup.ContainsKey(del))
			return;
		
		// Create a new non-generic delegate which calls our generic one.
		// This is the delegate we actually invoke.
		EventDelegate internalDelegate = (e) => del((T)e);
		delegateLookup[del] = internalDelegate;
		
		EventDelegate tempDel;
		if (delegates.TryGetValue(typeof(T), out tempDel))
		{
			delegates[typeof(T)] = tempDel += internalDelegate; 
		}
		else
		{
			delegates[typeof(T)] = internalDelegate;
		}
	}
	
	public void RemoveListener<T> (EventDelegate<T> del) where T : GameEvent
	{
		EventDelegate internalDelegate;
		if (delegateLookup.TryGetValue(del, out internalDelegate))
		{
			EventDelegate tempDel;
			if (delegates.TryGetValue(typeof(T), out tempDel))
			{
				tempDel -= internalDelegate;
				if (tempDel == null)
				{
					delegates.Remove(typeof(T));
				}
				else
				{
					delegates[typeof(T)] = tempDel;
				}
			}
			
			delegateLookup.Remove(del);
		}
	}
	
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
	public void Raise (GameEvent e)
	{
		if (e is ReplicatedEvent && ((ReplicatedEvent)e).ShouldReplicate)
		{
			if (Replication.IsHost)
			{
				RaiseInternal(e);
				EB.Replication.Manager.RPC("OnReplicatedEventRPC", EB.RPCMode.Others, e);
			}
		}
		else
		{
			RaiseInternal(e);
		}
	}
	
	public void RegisterCommand<eventType, commandType>()
	{
		List<System.Type> commands = null;
		
		try 
		{
			commands = commandMap[typeof(eventType)];
		} 
		catch // (KeyNotFoundException exception)
		{
			commands = new List<System.Type>();
		}
		
		if(commands.Contains(typeof(commandType)))
		{
			EB.Debug.LogWarning("Events::RegisterCommand command already registered; ignoring");
			return;
		}
		
		commands.Add(typeof(commandType));
		
		if(!commandMap.ContainsKey(typeof(eventType)))
			commandMap.Add(typeof(eventType), commands);
		else
			commandMap[typeof(eventType)] = commands;
	}
	
	public void UnregisterCommand<eventType, commandType>()
	{
		List<System.Type> commands = null;
		try 
		{
			commands = commandMap[typeof(eventType)];
		} 
		catch // (KeyNotFoundException exception)
		{
			EB.Debug.LogWarning("Events::UnregisterCommand command not found");
			return;
		}

		commands.Remove(typeof(commandType));
	}

	//[RPC]
	public static void OnReplicatedEventRPC(ReplicatedEvent evt)
	{
		instance.RaiseInternal(evt);
	}

	private void RaiseInternal(GameEvent e)
	{
		if (e == null)
		{
			return;
		}

		// Trigger event in PlayMaker if it's visible
		if (e.GetType().GetCustomAttributes(typeof(VisibleAtDesignLevelAttribute), false).Length > 0)
		{
			if (e is IPlayMakerEvent)
			{
				((IPlayMakerEvent)e).UpdateFsmEventData();
				GameObject[] relevantGOs = ((IPlayMakerEvent)e).GetRelevantGameObjects();
				BroadcastToPlayMaker(e, relevantGOs == null ? null : new List<GameObject>(relevantGOs));
			}
			else
			{
				BroadcastToPlayMaker(e);
			}
		}

		EventDelegate del;
		if (delegates.TryGetValue(e.GetType(), out del))
		{
			del.Invoke(e);
		}

		// walk registered commands, compare event
		try
		{
			List<System.Type> commands = null;
			if (commandMap.TryGetValue(e.GetType(), out commands))
			{
				for (int i = 0, cnt = commands.Count; i < cnt; ++i)
				{
					System.Type t = commands[i];
					// instantiate a new object supporting ICommand and call it's execute function with the event as a param
					// note: the object is short lived - after its Execute is called, the object is destroyed
					ICommand instance = (ICommand)System.Activator.CreateInstance(t);
					instance.Execute(e);
				}
			}
			else
			{
				return;
			}
		}
		catch (System.Exception info)
		{
			// no commands registered for this event
			Debug.LogException(info);
		}
	}

	private void BroadcastToPlayMaker(GameEvent e, List<GameObject> relevantTargets = null)
	{
		// no specific target(s) passed, broadcast to all FSMs
		if (relevantTargets == null)
		{
			PlayMakerFSM[] fsms = PlayMakerFSM.FsmList.ToArray();
			foreach (PlayMakerFSM fsm in fsms)
			{
				fsm.SendEvent("FUSION_" + e.GetType().Name);
			}
			
			return;
		}

		foreach (GameObject target in relevantTargets)
		{
			if (target != null)
			{
				PlayMakerFSM[] fsms = target.GetComponentsInChildren<PlayMakerFSM>();
				if (fsms != null)
				{
					foreach (PlayMakerFSM fsm in fsms)
					{
						fsm.SendEvent("FUSION_" + e.GetType().Name);
					}
				}
			}
		}
	}
}
