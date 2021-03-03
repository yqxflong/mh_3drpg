using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if DEBUG

public interface IDebuggable
{
	/// <summary>
	/// debug Gizmos
	/// called in OnPostRender or OnDrawGizmos
	/// </summary>
	void OnDrawDebug();

	/// <summary>
	/// debug GUI
	/// called in DebugSystemComponent's OnGUI
	/// </summary>
	void OnDebugGUI();

	/// <summary>
	/// debug panel
	/// called in DebugSystemComponent's OnDebugGUI
	/// </summary>
	void OnDebugPanelGUI();
}

public interface IDebuggableEx : IDebuggable
{
	/// <summary>
	/// on previous values loaded
	/// </summary>
	void OnPreviousValuesLoaded();

	/// <summary>
	/// on value changed
	/// </summary>
	void OnValueChanged(FieldInfo field, object oldValue, object newValue);
}

/// <summary>
/// indicate float/int/string attribute show in Debug Panel
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field)]
public class ShowFieldInDebuggerAttribute : System.Attribute { }

/// <summary>
/// indicate don't save/load attribute value
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field)]
public class DontRememberFieldValue : System.Attribute { }

#endif

public class DebugSystem
{
#if DEBUG
	private DebugSystemComponent _component;

	private static bool _isInitialized = false;

	private static DebugSystem _instance = null;

	public static void Initialize()
	{
		if (!_isInitialized)
		{
			_instance = new DebugSystem();
			Camera debugSystemCamera = new GameObject("DebugSystemCamera").AddComponent<Camera>();
			debugSystemCamera.depth = 10000.0f;
			debugSystemCamera.clearFlags = CameraClearFlags.SolidColor;
			debugSystemCamera.cullingMask = 0;
			_instance._component = debugSystemCamera.gameObject.AddComponent<DebugSystemComponent>();
			_instance._component.enabled = true;
			_instance._component.skin = (GUISkin)Resources.Load("DebugSystemAssets/DebugGUISkin");
			_instance._component.toggleOn = (Texture)Resources.Load("DebugSystemAssets/gizmoButton_on");
			_instance._component.toggleOff = (Texture)Resources.Load("DebugSystemAssets/gizmoButton_off");
			_instance._component.boolOn = (Texture)Resources.Load("DebugSystemAssets/pixelBox_on");
			_instance._component.boolOff = (Texture)Resources.Load("DebugSystemAssets/pixelBox_off");
			_isInitialized = true;
		}
	}

	public static void DebugCameraClearNothing()
	{
		if (_isInitialized)
		{
			_instance._component.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
		}
	}

	public static void Destroy()
	{
		if (_isInitialized)
		{
			Component.Destroy(_instance._component);
			_instance = null;
			_isInitialized = false;
		}
	}

	public static GUISkin Skin()
	{
		if (_isInitialized)
		{
			return _instance._component.skin;
		}

		return null;
	}

	public static Texture ToggleOnTexture()
	{
		if (_isInitialized)
		{
			return _instance._component.toggleOn;
		}

		return null;
	}

	public static Texture ToggleOffTexture()
	{
		if (_isInitialized)
		{
			return _instance._component.toggleOff;
		}

		return null;
	}

	public static Texture BoolOnTexture()
	{
		if (_isInitialized)
		{
			return _instance._component.boolOn;
		}

		return null;
	}

	public static Texture BoolOffTexture()
	{
		if (_isInitialized)
		{
			return _instance._component.boolOff;
		}

		return null;
	}

	public static void Close()
	{
		if (_isInitialized)
		{
			_instance._component.ShowPanel(false);
		}
	}

	/// <summary>
	/// Writes log to the in-game console and to the Unity console, but only if the specified system is enabled and its
	/// <code>showLog</code> variable is true.
	/// </summary>
	/// <param name="system">System from which the message originates.</param>
	/// <param name="message">Body of the message.</param>
	/// <param name="importance">Type of message.</param>
	public static void Log(IDebuggable system, object message, LogType importance = LogType.Log)
	{
		if (!_isInitialized)
		{
			return;
		}
		_instance._component.Log(system, message, importance);
	}
#endif

	/// <summary>
	/// Writes log to the in-game console and to the Unity console.
	/// If systemName is the name of a registered system, then the message will only be displayed if that system is
	/// enabled and its <code>showLog</code> variable is true.
	/// If no registered system with the name <code>systemName</code> is found, it will print with <code>systemName</code>
	/// as tag.
	/// </summary>
	/// <param name="message">Body of the message.</param>
	/// <param name="systemName">Name of the system from which the message originates.</param>
	/// <param name="importance">Type of the message.</param>
	public static void Log(string message, string systemName, LogType importance = LogType.Log)
	{
#if DEBUG
		if (!_isInitialized)
		{
			return;
		}
		_instance._component.Log(message, importance, systemName);
#endif
	}

	/// <summary>
	/// Writes log to the in-game console and to the Unity console.
	/// The system will use the [General] tag when displaying the message.
	/// </summary>
	/// <param name="message">Body of the message.</param>
	/// <param name="importance">Type of the message.</param>
	public static void Log(object message, LogType importance = LogType.Log)
	{
#if DEBUG
		if (!_isInitialized)
		{
			return;
		}
		_instance._component.Log(message, importance);
#endif
	}

	public static void LogWarning(string message) 
	{ 
#if DEBUG
		Log(message, LogType.Warning); 
#endif
	}

	public static void LogError(string message) 
	{ 
#if DEBUG
		Log(message, LogType.Error); 
#endif
	}

	public static void LogException(string message) 
	{ 
#if DEBUG
		Log(message, LogType.Exception); 
#endif
	}

#if DEBUG
	/// <summary>
	/// Registers the system uniquely to the debugger.
	/// Registering a system to the debugger means its public bools (and private bools
	/// marked with [ShowFieldInDebugger]) will be displayed in the section
	/// or subsection named <code>systemName</code> in the debugging panel.
	/// It will also allow it to get its IDebuggable interface methods called
	/// at the appropriate moment, and to log to the in-game console under control of the
	/// system (i.e. can be hidden if the user wishes).
	/// If you want to be able to register several objects of the same name, 
	/// use <see cref="RegisterInstance"/>.
	/// </summary>
	/// <param name="systemName">Unique name of the system.</param>
	/// <param name="system"></param>
	/// <param name="parentName"></param>
	public static void RegisterSystem(string systemName, IDebuggable system, string parentName = "", bool showInDebugger = true)
	{
		if (!_isInitialized)
		{
			return;
		}
		if (GetSystem(systemName) != null)
		{
			//Log(_component, "System already registered!", eImportance.Warning);
			return;
		}
		_instance._component.RegisterSystem(system, systemName, GetSystem(parentName), true, showInDebugger);
	}

	/// <summary>
	/// Registers the instance to the debugger.
	/// Registering a system to the debugger means its public bools (and private bools
	/// marked with [ShowFieldInDebugger]) will be displayed in the section
	/// or subsection named <code>systemName</code> in the debugging panel.
	/// It will also allow it to get its IDebuggable interface methods called
	/// at the appropriate moment, and to log to the in-game console under control of the
	/// system (i.e. can be hidden if the user wishes).
	/// Through this function, you can register several objects with the 
	/// same name, as long as this name is not already registered by another system
	/// through <code>RegisterSystem</code>.
	/// If you want to register this system uniquely, use <see cref="RegisterSystem"/>.
	/// </summary>
	/// <param name="instanceName">Name of this system.</param>
	/// <param name="system">Instance to be registered.</param>
	/// <param name="parentName">Name of the parent system of which <code>system</code> is a subsystem.</param>
	public static void RegisterInstance(string instanceName, IDebuggable system, string parentName, bool showInDebugger = true)
	{
		if (!_isInitialized)
		{
			return;
		}
		_instance._component.RegisterSystem(system, instanceName, GetSystem(parentName), false, showInDebugger);
	}

	/// <summary>
	/// Unregisters the instance from the debugger.
	/// </summary>
	/// <param name="obj">System to unregister.</param>
	/// <param name="unregisterSubSystems">True will unregister all the subsystems of <code>obj</code>.</param>
	public static void UnregisterSystem(IDebuggable obj, bool unregisterSubSystems = true)
	{
		if (!_isInitialized)
		{
			return;
		}
		_instance._component.UnregisterSystem(obj, unregisterSubSystems);
	}

	/// <summary>
	/// Gets the system named <code>systemName</code>.
	/// You can specify just the system name or the absolute path
	/// in the hierarchy (e.g. "Pathfinding/Player1").
	/// </summary>
	/// <param name="systemName">Name of the system to look for.</param>
	/// <returns>the first system named <code>systemName</code>, or null if none registered.</returns>
	public static IDebuggable GetSystem(string systemName)
	{
		if (!_isInitialized)
		{
			return null;
		}
		return _instance._component.GetSystem(systemName);
	}

	public static IDebuggable GetSystem(string systemName, IDebuggable parent)
	{
		if (!_isInitialized)
		{
			return null;
		}
		return _instance._component.GetSystem(systemName, parent);
	}

	public static IDebuggable[] GetAllSystems(string systemName)
	{
		if (!_isInitialized)
		{
			return null;
		}
		return _instance._component.GetAllSystems(systemName);
	}

	public static IDebuggable[] GetAllSystems(string systemName, IDebuggable parent)
	{
		if (!_isInitialized)
		{
			return null;
		}
		return _instance._component.GetAllSystems(systemName, parent);
	}

	/// <summary>
	/// Gets the first registered system of type T.
	/// </summary>
	/// <typeparam name="T">Type of the system to look for.</typeparam>
	/// <returns>The first system of type T registered to the debugger.</returns>
	public static T GetSystem<T>() where T : IDebuggable
	{
		if (!_isInitialized)
		{
			return default(T);
		}
		return _instance._component.GetSystem<T>();
	}

	/// <summary>
	/// Gets all the registered systems of the specified type.
	/// </summary>
	/// <typeparam name="T">Type of the systems to look for.</typeparam>
	/// <returns>All systems of type T registered to the debugger.</returns>
	public static T[] GetAllSystems<T>() where T : IDebuggable
	{
		if (!_isInitialized)
		{
			return default(T[]);
		}
		return _instance._component.GetAllSystems<T>();
	}

	/// <summary>
	/// Whether the system is enabled or not.
	/// If any system in the parent hierarchy of <code>system</code> is disabled,
	/// this system will also be disabled.
	/// </summary>
	/// <param name="system">The system for which we want to know whether it is enabled or not.</param>
	/// <returns>True if enabled, false if disabled.</returns>
	public static bool IsEnabled(IDebuggable system)
	{
		if (!_isInitialized)
		{
			return false;
		}
		return _instance._component.IsEnabled(system);
	}

	/// <summary>
	/// Whether the system is enabled or not.
	/// If any system in the parent hierarchy of <code>system</code> is disabled,
	/// this system will also be disabled.
	/// </summary>
	/// <param name="systemName">The system for which we want to know whether it is enabled or not.</param>
	/// <returns>True if enabled, false if disabled.</returns>
	public static bool IsEnabled(string systemName)
	{
		if (!_isInitialized)
		{
			return false;
		}
		return IsEnabled(GetSystem(systemName));
	}

	/// <summary>
	/// Returns the direct parent of <code>system</code>.
	/// </summary>
	/// <param name="system"></param>
	/// <returns>The direct parent of this system, or null if no parent.</returns>
	public static IDebuggable GetParentOf(IDebuggable system)
	{
		if (!_isInitialized)
		{
			return null;
		}
		return _instance._component.GetParentOf(system);
	}

	/// <summary>
	/// Returns all direct subsystems of <code>system</code>.
	/// </summary>
	/// <param name="system"></param>
	/// <returns>The direct registered subsystems of this system, or null if no subsystem.</returns>
	public static IDebuggable[] GetSubSystemsOf(IDebuggable system)
	{
		if (!_isInitialized)
		{
			return null;
		}
		return _instance._component.GetSubSystems(system);
	}

	public static string GetSystemPath(IDebuggable system)
	{
		if (!_isInitialized)
		{
			return "";
		}
		return _instance._component.GetSystemPath(system);
	}

	public static Color GetSystemColor(IDebuggable system)
	{
		if (!_isInitialized)
		{
			return default(Color);
		}
		return _instance._component.GetSystemColor(system);
	}

	public static void SetSystemColor(IDebuggable system, Color color)
	{
		if (!_isInitialized)
		{
			return;
		}
		_instance._component.SetSystemColor(color, system);
	}
#endif
}
