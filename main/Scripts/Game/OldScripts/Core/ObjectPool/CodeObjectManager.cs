///////////////////////////////////////////////////////////////////////
//
//  CodeObjectManager.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

public class CodeObjectManager 
#if DEBUG
		: IDebuggable 
#endif
{
	public class InternalObjManager<T> where T : IPoolable, new()
	{
		private EB.Collections.Queue<T> _inactive = new EB.Collections.Queue<T>();
		private List<T> _active = new List<T>();

		public int TotalCount 
		{
			get 
			{
				return _inactive.Count + _active.Count;
			}
		}

		public T GetNext()
		{
			T next = default(T);
			if (_inactive.Count == 0)
			{
				next = new T();
			}
			else
			{
				next = _inactive.Dequeue();
			}

			next.OnPoolActivate();
			_active.Add(next);

			return next;
		}

		public void Destroy(T obj)
		{
			GameUtils.FastRemove<T>(_active, obj);
			_inactive.Enqueue(obj);

			obj.OnPoolDeactivate();
		}
	}

	public class InternalObjManagerNonGeneric
	{
		private EB.Collections.Queue<object> _inactive = new EB.Collections.Queue<object>();
		private List<object> _active = new List<object>();

		private System.Type _type;

		public int TotalCount 
		{
			get 
			{
				return _inactive.Count + _active.Count;
			}
		}

		public InternalObjManagerNonGeneric(System.Type t)
		{
			_type = t;
		}

		public object GetNext()
		{
			object next = null;
			if (_inactive.Count == 0)
			{
				next = System.Activator.CreateInstance(_type);
			}
			else
			{
				next = _inactive.Dequeue();
			}

			(next as IPoolable).OnPoolActivate();
			_active.Add(next);

			return next;
		}

		public void Destroy(object obj)
		{
			GameUtils.FastRemove<object>(_active, obj);
			_inactive.Enqueue(obj);

			(obj as IPoolable).OnPoolDeactivate();
		}
	}

	private Dictionary<System.Type, InternalObjManagerNonGeneric> _effectManagers = new Dictionary<System.Type, InternalObjManagerNonGeneric>();
	private Dictionary<System.Type, InternalObjManagerNonGeneric> _characterStateManagers = new Dictionary<System.Type, InternalObjManagerNonGeneric>();
	private Dictionary<System.Type, InternalObjManagerNonGeneric> _eventManagers = new Dictionary<System.Type, InternalObjManagerNonGeneric>();
	
	//private InternalObjManager<EffectContext> _contextManager = new InternalObjManager<EffectContext>();

	public static InternalObjManagerNonGeneric GetEffectManager(System.Type t)
	{
		CodeObjectManager instance = GameStateManager.Instance.ObjectManager;
		if (!instance._effectManagers.ContainsKey(t))
		{
			instance._effectManagers.Add(t, new InternalObjManagerNonGeneric(t));
		}
		return instance._effectManagers[t];
	}

	public static InternalObjManagerNonGeneric GetCharacterStateManager(System.Type t)
	{
		CodeObjectManager instance = GameStateManager.Instance.ObjectManager;
		if (!instance._characterStateManagers.ContainsKey(t))
		{
			instance._characterStateManagers.Add(t, new InternalObjManagerNonGeneric(t));
		}
		return instance._characterStateManagers[t];
	}

	public static InternalObjManagerNonGeneric GetEventManager(System.Type t)
	{
		CodeObjectManager instance = GameStateManager.Instance.ObjectManager;
		if (!instance._eventManagers.ContainsKey(t))
		{
			instance._eventManagers.Add(t, new InternalObjManagerNonGeneric(t));
		}
		return instance._eventManagers[t];	
	}

	//public static InternalObjManager<EffectContext> EffectContext
	//{
	//	get
	//	{
	//		return GameStateManager.Instance.ObjectManager._contextManager;
	//	}
	//}

	public void Start()
	{
#if DEBUG
		//DebugSystem.RegisterInstance("Code Object Pool", this, "Pool");
#endif
	}

	public void End() 
	{
#if DEBUG
		//DebugSystem.UnregisterSystem(this);
#endif
	}

#if DEBUG
	public void OnDrawDebug()
	{

	}

	public void OnDebugGUI()
	{

	}

	public void OnDebugPanelGUI()
	{
		GUILayout.BeginVertical();

		int totalEffects = 0;
		foreach (System.Type t in _effectManagers.Keys)
		{
			GUILayout.Label(t + " Pool: " + _effectManagers[t].TotalCount);
			totalEffects += _effectManagers[t].TotalCount;
		}
		GUILayout.Label("Effects Pool Total: " + totalEffects);

		int totalCharacterStates = 0;
		foreach (System.Type t in _characterStateManagers.Keys)
		{
			GUILayout.Label(t + " Pool: " + _characterStateManagers[t].TotalCount);
			totalCharacterStates += _characterStateManagers[t].TotalCount;
		}
		GUILayout.Label("Character State Pool Total: " + totalCharacterStates);

		int totalGameEvents = 0;
		foreach (System.Type t in _eventManagers.Keys)
		{
			GUILayout.Label(t + " Pool: " + _eventManagers[t].TotalCount);
			totalGameEvents += _eventManagers[t].TotalCount;
		}
		GUILayout.Label("Character State Pool Total: " + totalGameEvents);

		GUILayout.EndVertical();
	}
#endif
}