#if DEBUG

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public partial class DebugSystemComponent : MonoBehaviour, IDebuggable
{
	// Holds the metadata for each system
	public class SystemInfo
	{
		public string systemName;
		public bool isDebuggingEnabled = true;
		public Color systemColor = Color.white;
		private bool _isShowing = false;
		private bool _isUnique = false;
		private bool _showInDebugger = false;
		public bool showLog = true;
		public IDebuggable parent = null;
		public List<IDebuggable> subSystems = null;

		/// <summary>
		/// Is Expand on Debug Panel
		/// </summary>
		public bool IsShowing
		{
			get { return _isShowing; }
		}

		/// <summary>
		/// Is Unique System Instance
		/// </summary>
		public bool IsUnique
		{
			get { return _isUnique; }
		}

		/// <summary>
		/// Is Show on Debug Panel
		/// </summary>
		public bool ShowInDebugger
		{
			get { return _showInDebugger; }
		}

		/// <summary>
		/// Is SubSystem
		/// </summary>
		public bool IsSubSystem
		{
			get { return parent != null; }
		}

		public SystemInfo(bool isUnique = false, bool showInDebugger = true)
		{
			_isUnique = isUnique;
			_showInDebugger = showInDebugger;
		}

		public void ToggleIsShowing()
		{
			_isShowing = !_isShowing;
		}
	}

	// To make sure no one else creates a LogSystem,
	// with this only this component can create it
	private partial class LogSystem
	{
		public LogSystem()
		{
			Initialize();
		}
	}

	// Bigger icons for the toggle.
	// Unfortunately Unity doesn't scale icons, so they'll be tiny on an actual device.
	public Texture toggleOn;
	public Texture toggleOff;
	public Texture boolOn;
	public Texture boolOff;

	// GUI variables
	public GUISkin skin;
	private static float windowSizeFactor = .97f;
	private Rect _windowRect = new Rect(Screen.width / 100.0f, Screen.height * 0.01f, Screen.width * windowSizeFactor, Screen.height * windowSizeFactor);
	private int fontSize = Screen.width / 35;
	private int tabSize = Screen.width / 35;
	private float scrollBarWidth = Screen.width / 20.0f;
	private float buttonHeight = Screen.width / 30.0f;
	private Vector2 scrollPos = new Vector2();
	private bool _isPanelShowing = false;

	// Custom GL rendering
	private Material _lineMaterial = null;

	// Systems management
	private Dictionary<IDebuggable, SystemInfo> _gameSystems = new Dictionary<IDebuggable, SystemInfo>();
	private LogSystem _log;

	#region Public System Management

	public bool IsEnabled(IDebuggable system)
	{
		if (system == null || !_gameSystems.ContainsKey(system))
		{
			Log(this, "IsEnabled: Invalid system. Returning false", LogType.Error);
			return false;
		}
		SystemInfo info = _gameSystems[system];
		return info.isDebuggingEnabled && (info.parent == null ? true : IsEnabled(info.parent));
	}

	public void RegisterSystem(IDebuggable system, string name = null, IDebuggable parent = null, bool isUnique = false, bool showInDebugger = true)
	{
		if (_gameSystems.ContainsKey(system))
		{
			return;
		}

		if (isUnique && GetSystemFromName(name) != null)
		{
			Log(this, "The system name " + name + " already exists. Please find another name or use RegisterInstance.", LogType.Error);
			return;
		}
		if (!isUnique)
		{
			IDebuggable otherSys = GetSystemFromName(name);
			if (otherSys != null && _gameSystems[otherSys].IsUnique)
			{
				Log(this, "The instance name " + name + " is already used by a system. Please find another name.", LogType.Error);
				return;
			}
		}

		SystemInfo info = new SystemInfo(isUnique, showInDebugger);

		// If name is null, default to type name
		if (name == null)
		{
			name = system.GetType().Name;
		}
		info.systemName = name;

		// If parent is not null, check if it exists;
		// if it doesn't, default to null
		if (parent != null)
		{
			if (!_gameSystems.ContainsKey(parent))
			{
				parent = null;
				Log(this, "Specified parent of type " + parent.GetType().Name + " is not a registered system; defaulted to null", LogType.Warning);
			}
			else
			{
				if (_gameSystems[parent].subSystems == null)
				{
					_gameSystems[parent].subSystems = new List<IDebuggable>();
				}
				_gameSystems[parent].subSystems.Add(system);
			}
		}
		info.parent = parent;

		// If non-sub system, create list of subsystems
		if (parent == null)
		{
			info.subSystems = new List<IDebuggable>();
		}

		_gameSystems.Add(system, info);

		// Load default settings from PlayerPrefs
		DebuggerUtils.LoadSettings(system, info, GetSystemPrefix(system));
	}

	public void UnregisterSystem(IDebuggable system, bool unregisterSubSystems = true)
	{
		if (system == null)
		{
			return;
		}

		// issue a warning if not registered
		if (!_gameSystems.ContainsKey(system))
		{
			//Log(this, "Trying to unregister a non-registered system of type " + system.GetType().Name, eImportance.Warning);
			return;
		}

		SystemInfo info = _gameSystems[system];
		_gameSystems.Remove(system);

		// Clean up from parent's subsystems
		if (info.parent != null)
		{
			_gameSystems[info.parent].subSystems.Remove(system);
		}

		// If unregister subsystems, do it
		// otherwise, clear out their parent
		if (info.subSystems != null && info.subSystems.Count > 0)
		{
			for (int i = 0, cnt = info.subSystems.Count; i < cnt; ++i)
			{
				IDebuggable subSystem = info.subSystems[i];
				_gameSystems[subSystem].parent = null;
				if (unregisterSubSystems)
				{
					UnregisterSystem(subSystem, unregisterSubSystems);
				}
			}
		}
	}

	public IDebuggable GetSystem(string systemName)
	{
		if (systemName == null)
		{
			return null;
		}
		else if (systemName.Split('/').Length > 1)
		{
			return GetSystemFromPath(systemName);
		}
		else
		{
			return GetSystemFromName(systemName);
		}
	}

	public IDebuggable GetSystem(string systemName, IDebuggable parent)
	{
		return GetSystemFromPath(systemName, parent);
	}

	public IDebuggable[] GetAllSystems(string systemName)
	{
		if (systemName.Split('/').Length > 1)
		{
			return GetAllSystemsFromPath(systemName);
		}
		else
		{
			return GetAllSystemsFromName(systemName);
		}
	}

	public IDebuggable[] GetAllSystems(string systemName, IDebuggable parent)
	{
		return GetAllSystemsFromPath(systemName, parent);
	}

	public T GetSystem<T>() where T : IDebuggable
	{
		Dictionary<IDebuggable, SystemInfo>.Enumerator iter = _gameSystems.GetEnumerator();
		while (iter.MoveNext())
		{
			IDebuggable system = iter.Current.Key;
			if (system is T)
			{
				iter.Dispose();
				return (T)system;
			}
		}
		iter.Dispose();
		return default(T);
	}

	public T[] GetAllSystems<T>() where T : IDebuggable
	{
		List<T> result = new List<T>();

		Dictionary<IDebuggable, SystemInfo>.Enumerator iter = _gameSystems.GetEnumerator();
		while (iter.MoveNext())
		{
			T system = (T)iter.Current.Key;
			result.Add(system);
		}
		iter.Dispose();

		return result.ToArray();
	}

	public IDebuggable[] GetSubSystems(IDebuggable system)
	{
		if (_gameSystems.ContainsKey(system))
		{
			return _gameSystems[system].subSystems.ToArray();
		}
		return new IDebuggable[0];
	}

	public IDebuggable GetParentOf(IDebuggable system)
	{
		if (_gameSystems.ContainsKey(system))
		{
			return _gameSystems[system].parent;
		}
		return null;
	}

	private IDebuggable GetSystemFromName(string systemName)
	{
		foreach (IDebuggable sys in _gameSystems.Keys)
		{
			if (_gameSystems[sys].systemName == systemName)
			{
				return sys;
			}
		}
		return null;
	}

	private IDebuggable GetSystemFromPath(string systemPath, IDebuggable parent = null)
	{
		return GetAllSystemsFromPath(systemPath, parent)[0];
	}

	private IDebuggable[] GetAllSystemsFromName(string systemName)
	{
		List<IDebuggable> result = new List<IDebuggable>();

		Dictionary<IDebuggable, SystemInfo>.Enumerator iter = _gameSystems.GetEnumerator();
		while (iter.MoveNext())
		{
			if (iter.Current.Value.systemName == systemName)
			{
				result.Add(iter.Current.Key);
			}
		}
		iter.Dispose();

		return result.ToArray();
	}

	private IDebuggable[] GetAllSystemsFromPath(string systemPath, IDebuggable parent = null)
	{
		if (systemPath == null)
		{
			return new IDebuggable[0];
		}

		string[] names = systemPath.Split('/');
		if (names.Length == 0)
		{
			return new IDebuggable[0];
		}

		EB.Collections.Queue<IDebuggable> currents = new EB.Collections.Queue<IDebuggable>();
		foreach (IDebuggable sys in GetAllSystemsFromName(names[0]))
		{
			if (_gameSystems[sys].parent == parent && _gameSystems[sys].systemName == names[0])
			{
				currents.Enqueue(sys);
			}
		}

		for (int i = 1; i < names.Length; i++)
		{
			while (currents.Count > 0 && _gameSystems[currents.Peek()].systemName == names[i - 1])
			{
				foreach (IDebuggable sub in _gameSystems[currents.Dequeue()].subSystems)
				{
					if (_gameSystems[sub].systemName == names[i])
					{
						currents.Enqueue(sub);
					}
				}
			}
		}

		return currents.ToArray();
	}

	public void SetSystemColor(Color color, IDebuggable system)
	{
		if (system != null && _gameSystems.ContainsKey(system))
		{
			_gameSystems[system].systemColor = color;
		}
	}

	public Color GetSystemColor(IDebuggable system)
	{
		if (system != null && _gameSystems.ContainsKey(system))
		{
			return _gameSystems[system].systemColor;
		}
		return Color.white;
	}

	#endregion

	#region LogRouting

	public void Log(IDebuggable system, object message, LogType importance)
	{
		if (!_gameSystems.ContainsKey(system))
		{
			Log(message, importance, "Missing System");
			return;
		}

		SystemInfo info = _gameSystems[system];

		if (IsEnabled(system) && info.showLog)
		{
			string newMessage = "[" + info.systemName + "] " + message;
			if (info.IsSubSystem)
			{
				Log(info.parent, newMessage, importance);
			}
			else
			{
				_log.Log(newMessage, importance);
			}
		}
	}

	public void Log(object message, LogType importance)
	{
		Log(this, message, importance);
	}

	public void Log(object message, LogType importance, string systemName)
	{
		IDebuggable system = GetSystem(systemName);
		if (system != null)
		{
			Log(system, message, importance);
		}
		else
		{
			string newMessage = "[" + systemName + "] " + message;
			_log.Log(newMessage, importance);
		}
	}

	#endregion

	void Awake()
	{
		DontDestroyOnLoad(this);

		// Register itself
		RegisterSystem(this, "General", null, true);
		
		// Those should not be changed...
		_gameSystems[this].isDebuggingEnabled = true;
		_gameSystems[this].showLog = true;

		// Create and register log subsystem
		_log = new LogSystem();
		RegisterSystem(_log, "LogSystem", this, true);
		// Log(this, "Log open!", LogType.Log);

		// Register GLRendering subsystem
		GLRenderingUtils render = new GLRenderingUtils();
		RegisterSystem(render, "RenderingUtils", this, true, false);
		_gameSystems[render].isDebuggingEnabled = true;
		_gameSystems[render].showLog = true;

	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1) || GetBeginningTouchesCount() > 3)
		{
			ShowPanel(true);
		}
	}

	private int GetBeginningTouchesCount()
	{
		int result = 0;
		for (int i = 0; i < Input.touchCount; i++)
		{
			if (Input.touches[i].phase == TouchPhase.Began)
			{
				result++;
			}
		}
		return result;
	}

	void OnPostRender()
	{
		GL.PushMatrix();

		if (Camera.main != null)
		{
			GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
			GL.modelview = Camera.main.worldToCameraMatrix;
		}

		if (_lineMaterial == null)
		{
			_lineMaterial = new Material(Shader.Find("LinesHelper/ColoredBlended"));
			_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		}

		_lineMaterial.SetPass(0);

		// Avoid GC Alloc
		_systems.Clear();
		Dictionary<IDebuggable, SystemInfo>.Enumerator iter = _gameSystems.GetEnumerator();
		while (iter.MoveNext())
		{
			_systems.Add(iter.Current.Key);
		}
		iter.Dispose();
		for (int i = 0, cnt = _systems.Count; i < cnt; ++i)
		{
			IDebuggable system = _systems[i];
			if (IsEnabled(system))
			{
				system.OnDrawDebug();
			}
		}

		GL.PopMatrix();
	}

	List< IDebuggable > _systems = new List<IDebuggable>();
	void OnGUI()
	{
		// Show only enabled and non-hidden systems
		// Avoid GC Alloc
		_systems.Clear();
		Dictionary<IDebuggable, SystemInfo>.Enumerator iter = _gameSystems.GetEnumerator();
		while (iter.MoveNext())
		{
			_systems.Add(iter.Current.Key);
		}
		iter.Dispose();
		for (int i = 0, cnt = _systems.Count; i < cnt; ++i)
		{
			IDebuggable system = _systems[i];
			if (IsEnabled(system))
			{
				system.OnDebugGUI();
			}
		}

		if (_isPanelShowing)
		{
			GUI.BringWindowToFront(0);
			GUI.FocusWindow(0);
		}
	}

	void OnDestroy()
	{
		_log.Close();
	}

	#region Panel Rendering

	public void ShowPanel(bool show)
	{
		_isPanelShowing = show;
		GameVars.paused = show;
		GameObject cam = GameObject.Find("MainHudUI/OrthoCam");
		if(null != cam)
		{
			cam.GetComponent<UICamera>().enabled = !show;
		}

	}

	private bool CanWriteLog(IDebuggable system)
	{
		return _gameSystems[system].showLog;
	}

	private List<IDebuggable> _debuggables = new List<IDebuggable>();

	private void DrawPanel(int windowID)
	{
		// Make sure we have our own GUI,
		// otherwise things are going to look bad
		// (and we wouldn't want that!)
		GUI.skin = skin;

		GUI.skin.toggle.fontSize = fontSize;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.button.fontSize = fontSize;
		GUI.skin.verticalScrollbar.fixedWidth = scrollBarWidth;
		GUI.skin.verticalScrollbarThumb.fixedWidth = scrollBarWidth;
		GUI.skin.button.fixedHeight = buttonHeight;

		scrollPos = GUILayout.BeginScrollView(scrollPos);

		_debuggables.Clear();
		_debuggables.AddRange(_gameSystems.Keys);

		foreach (IDebuggable system in _debuggables)
		{
			if (!_gameSystems[system].IsSubSystem && _gameSystems[system].ShowInDebugger)
			{
				RenderSystemRecursive(system);
			}
		}

		GUILayout.EndScrollView();
		
//		if (GUILayout.Button("Reset Player Preferences"))
//		{
//			UserData.ClearPreferences();
//		}

		//if (GUILayout.Button("Load Default Settings"))
		//{
		//	foreach (IDebuggable system in _gameSystems.Keys)
		//	{
		//		DebuggerUtils.LoadDefaultSettings(system, GetSystemPrefix(system));
		//	}
		//}

		//if (GUILayout.Button("Save System's Settings As Default"))
		//{
		//	foreach (IDebuggable system in _gameSystems.Keys)
		//	{
		//		if (_gameSystems[system].IsUnique)
		//		{
		//			DebuggerUtils.SaveDefaultSettings(system, GetSystemPrefix(system));
		//		}
		//	}
		//}

		if (GUILayout.Button("Close"))
		{
			ShowPanel(false);
		}

		GUI.DragWindow();
	}

	private void RenderSystemRecursive(IDebuggable system)
	{
		SystemInfo info = _gameSystems[system];
		bool isDirty = false;

		GUILayout.BeginVertical();

		if (GUILayout.Button(new GUIContent(info.systemName, info.IsShowing ? toggleOn : toggleOff), "Toggle"))
		{
			info.ToggleIsShowing();
		}

		if (info.IsShowing)
		{
			GUI.skin.toggle.fontSize = (int)(fontSize * 0.9f);
			GUI.skin.label.fontSize = (int)(fontSize * 0.9f);
			GUI.skin.button.fontSize = (int)(fontSize * 0.9f);

			// Show all SystemInfo bools
			foreach (FieldInfo field in typeof(SystemInfo).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!DebuggerUtils.CanDisplayField(field))
				{
					continue;
				}

				// Only way to make a tab...
				GUILayout.BeginHorizontal();
				GUILayout.Space(tabSize);
				if (!System.Object.ReferenceEquals(system, this) && !(system == _log && (field.Name == "showLog")) && field.Name != "systemName")
				{
					bool oldValue = (bool)field.GetValue(info);
					bool newValue = Toggle(oldValue, DebuggerUtils.FormatFieldName(field.Name));

					// Save if is changed
					if (oldValue != newValue)
					{
						DebuggerUtils.SaveVariable(GetSystemPrefix(system) + "_" + field.Name, newValue);
						isDirty = true;
					}

					field.SetValue(info, newValue);
				}

				GUILayout.EndHorizontal();
			}

			GUILayout.BeginVertical();

			// Draw public bools of the class
			foreach (FieldInfo field in system.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				GUILayout.BeginHorizontal();

				GUILayout.Space(tabSize);

				if (DebuggerUtils.CanDisplayField(field))
				{
					object oldValue = field.GetValue(system);
					object newValue;

					if (field.FieldType == typeof(bool))
					{
						newValue = Toggle((bool)oldValue, DebuggerUtils.FormatFieldName(field.Name));
					}
					else
					{
						GUILayout.BeginHorizontal();
						GUILayout.Label(DebuggerUtils.FormatFieldName(field.Name));
						newValue = GUILayout.TextField(oldValue.ToString());
						GUILayout.EndHorizontal();

						try
						{
							if (field.FieldType == typeof(float))
							{
								newValue = float.Parse(newValue.ToString());
							}
							else if (field.FieldType == typeof(int))
							{
								newValue = int.Parse(newValue.ToString());
							}
						} catch
						{
							newValue = oldValue;
						}
					}

					// If value changed
					if (!newValue.Equals(oldValue))
					{
						field.SetValue(system, newValue);
						DebuggerUtils.SaveVariable(GetSystemPrefix(system) + "_" + field.Name, newValue);
						isDirty = true;

						if (system is IDebuggableEx)
						{
							((IDebuggableEx)system).OnValueChanged(field, oldValue, newValue);
						}
					}

				}

				GUILayout.EndHorizontal();
			}

			// If the system's designer wants to add anything
			GUILayout.BeginHorizontal();
			GUILayout.Space(tabSize);
			system.OnDebugPanelGUI();
			GUILayout.EndHorizontal();

			GUI.skin.toggle.fontSize = fontSize;
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.button.fontSize = fontSize;

			if (!info.IsUnique && GUILayout.Button("Save Instance's Settings As Default"))
			{
				DebuggerUtils.SaveDefaultSettings(system, GetSystemPrefix(system));
			}

			// Draw sub-systems
			if (info.subSystems != null && info.subSystems.Count > 0)
			{
				GUILayout.BeginHorizontal();

				GUILayout.Space(tabSize);

				GUILayout.BeginVertical();

				foreach (IDebuggable subSystem in info.subSystems)
				{
					if (_gameSystems[subSystem].ShowInDebugger)
					{
						RenderSystemRecursive(subSystem);
					}
				}

				GUILayout.EndVertical();

				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
		}

		GUILayout.EndVertical();

		if (isDirty)
		{
			PlayerPrefs.Save();
		}
	}

	#endregion

	#region Utils

	public string GetSystemPath(IDebuggable system)
	{
		EB.Collections.Stack<string> hierarchy = new EB.Collections.Stack<string>();
		IDebuggable currentSystem = system;
		while (currentSystem != null)
		{
			hierarchy.Push(_gameSystems[currentSystem].systemName);
			currentSystem = _gameSystems[currentSystem].parent;
		}

		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		while (hierarchy.Count > 0)
		{
			builder.Append(hierarchy.Pop());

			if (hierarchy.Count > 0)
			{
				builder.Append("/");
			}
		}

		return builder.ToString();
	}

	private void DeletePrefs()
	{
		foreach (KeyValuePair<IDebuggable, SystemInfo> pair in _gameSystems)
		{
			DebuggerUtils.DeletePrefs(pair.Key, pair.Value, GetSystemPrefix(pair.Key));
		}
	}

	// Automatically takes care of the toggle on/off images
	private bool Toggle(bool value, string text)
	{
		return GUILayout.Toggle(value, new GUIContent(text, value ? boolOn : boolOff));
	}

	private string GetSystemPrefix(IDebuggable system)
	{
		if (system == null || !_gameSystems.ContainsKey(system))
			return "";
		return GetSystemPrefix(_gameSystems[system].parent) + "/" + system.GetType().Name;
	}

	#endregion

	#region IDebuggable implementation

	public void OnDrawDebug()
	{

	}

	public void OnDebugGUI()
	{
		if (_isPanelShowing)
		{
			Rect newWindowRect = GUILayout.Window(0, _windowRect, DrawPanel, "DebugPanel");
			_windowRect = new Rect(newWindowRect.x, newWindowRect.y, Screen.width * windowSizeFactor, Screen.height * windowSizeFactor);
		}
	}

	public void OnDebugPanelGUI()
	{

	}

	#endregion
}
#endif
