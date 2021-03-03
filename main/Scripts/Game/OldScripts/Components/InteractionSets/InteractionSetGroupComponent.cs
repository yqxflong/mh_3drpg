///////////////////////////////////////////////////////////////////////
//
//  InteractionSetGroupComponent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This exists because we need a default inspector.
// Interaction Set Group info will be accessible here
// once we know how it'll be done.
public class InteractionSetGroupComponent : MonoBehaviour
#if DEBUG
	, IDebuggable 
#endif
{
	public InteractionSetComponent defaultInteractionSet;

	private InteractionSetComponent[] _interactionSets;

#if DEBUG
	private class InteractionSets : IDebuggable
	{
		public void OnDrawDebug() { }

		public void OnDebugGUI() { }

		public void OnDebugPanelGUI() { }
	}

	private InteractionSetComponent _current;
#endif

	void Awake()
	{
		if (GameEngine.Instance != null && GameEngine.Instance.SparxSocketData == null)
		{
			LoadDefaultSet();
		}
	}

	public void LoadDefaultSet()
	{
		_interactionSets = GetComponentsInChildren<InteractionSetComponent>();

		if (_interactionSets.Length == 0)
		{
			return;
		}
  
		// If the default interaction set is null, set it to something sensible
		if (defaultInteractionSet == null && _interactionSets != null)
		{
			int highestWeight = 0;
			foreach (InteractionSetComponent comp in _interactionSets)
			{
				if (comp.Weight >= highestWeight)
				{
					highestWeight = comp.Weight;
					defaultInteractionSet = comp;
				}
			}
		}

		if (defaultInteractionSet != null)
		{
			SetInteractionSet(defaultInteractionSet);
		}
	}

	public void SetInteractionSet(InteractionSetComponent activeSet)
	{
		if (activeSet != null)
		{
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
			{
				renderer.enabled = renderer.gameObject.activeSelf;
			}
		}

		// Destroy the rest (or disable it if we're in debug mode so that we can switch in between interaction sets
		// in the debugging panel)
		foreach (InteractionSetComponent comp in GetComponentsInChildren<InteractionSetComponent>())
		{
			if (comp != activeSet)
			{
#if DEBUG
				comp.gameObject.SetActive(false);
#else
				GameObject.Destroy(comp.gameObject);
#endif
			}
		}

#if DEBUG
		_current = activeSet;
#endif
	}

	void OnEnable()
	{
#if DEBUG
	   // DebugSystem.RegisterSystem("Interaction Sets", new InteractionSets(), null, true);
	   // DebugSystem.RegisterInstance(transform.parent.gameObject.name, this, "Interaction Sets", true);
#endif
	}

	void OnDisable()
	{
#if DEBUG
	   // DebugSystem.UnregisterSystem(this, true);
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
		GUILayout.Label("Current Interaction Set: " + _current.gameObject.name);

		foreach (InteractionSetComponent comp in _interactionSets)
		{
			if (GUILayout.Button(comp.gameObject.name))
			{
				_current.gameObject.SetActive(false);
				_current = comp;
				_current.gameObject.SetActive(true);
			}
		}
		GUILayout.EndVertical();
	}
#endif
}
