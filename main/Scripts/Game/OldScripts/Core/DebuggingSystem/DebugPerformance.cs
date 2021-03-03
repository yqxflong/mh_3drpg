using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if DEBUG
public struct UsageInfo
{
	public string display;
	public int bytes;
}
#endif

public class DebugPerformance : MonoBehaviour
	#if DEBUG
	, IDebuggableEx
		#endif
{
	#if DEBUG
	// public const int MIN_SIGNIFICANT_BYTES = 1048576; // 1 meg
	public const int MIN_SIGNIFICANT_BYTES = 524288; // 512k
	// public const int MIN_SIGNIFICANT_BYTES = 262144; // 256k
	
	public const string SystemName = "Performance";
	public const float UpdateInterval = 0.5f;
	
	public bool displayFps = true;
	public bool displayMemoryUsage = true;
	private float _fps = 0.0f;
	private int _labelFontSize;
	
	private float _accum = 0; // FPS accumulated over the interval
	private int _frames = 0; // Frames drawn over the interval
	private float _timeleft; // Left time for current interval
	
	void OnEnable()
	{
		DebugSystem.RegisterSystem(SystemName, this);
		_timeleft = UpdateInterval;
	}
	
	void OnDisable()
	{
		DebugSystem.UnregisterSystem(this);
	}
	
	void Update()
	{
		_timeleft -= Time.deltaTime;
		_accum += Time.timeScale / Time.deltaTime;
		++_frames;
		
		// Interval ended - update GUI text and start new interval
		if( _timeleft <= 0.0 )
		{
			_fps = _accum / _frames;
			_timeleft = UpdateInterval;
			_accum = 0.0F;
			_frames = 0;
		}
	}
	
	public void OnDrawDebug()
	{
	}
	
	public void OnDebugGUI()
	{
		GUI.skin.label.fontSize = 36;
		Color currentColor = GUI.color;
		
		if (displayFps)
		{
			if (_fps > 30.0f)
			{
				GUI.color = Color.green;
			}
			else if (_fps > 10.0f)
			{
				GUI.color = Color.yellow;
			}
			else
			{
				GUI.color = Color.red;
			}
			GUILayout.Label("", GUILayout.Width(600));
			GUILayout.Label("FPS: " + _fps, GUILayout.Width(600));
			GUI.color = currentColor;
		}
		
		if (displayMemoryUsage)
		{
			GUI.color = Color.green;
			GUILayout.Label("", GUILayout.Width(600));
			string heapsize = "0";
			string monosize = "0";
			if(UnityEngine.Profiling.Profiler.enabled==true)
			{
				heapsize = GameUtils.FormatBytes(UnityEngine.Profiling.Profiler.usedHeapSize);
				monosize =  GameUtils.FormatBytes(UnityEngine.Profiling.Profiler.GetMonoHeapSize());
			}
			else
			{
#if UNITY_IOS &&!UNITY_EDITOR
				heapsize = GameUtils.FormatBytes((long)getMemoryUsage());
#endif

				monosize = GameUtils.FormatBytes( System.GC.GetTotalMemory(false));
			}

			GUILayout.Label("HEAP SIZE: " + heapsize, GUILayout.Width(600));
			GUILayout.Label("MONO SIZE: " + monosize, GUILayout.Width(600));
			GUI.color = currentColor;
			
			if (GUILayout.Button("Take Snapshot"))
			{
				DumpObjectMemoryToConsole();
			}
		}
	}
	
	public void OnDebugPanelGUI()
	{
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("LOD 100"))
		{
			Shader.globalMaximumLOD = 100;
		}
		else if(GUILayout.Button("LOD 200"))
		{
			Shader.globalMaximumLOD = 200;
		}
		else if(GUILayout.Button("LOD 300"))
		{
			Shader.globalMaximumLOD = 300;
		}
		
		GUILayout.EndHorizontal();
	}
	
	public void DumpObjectMemoryToConsole()
	{
		List<UsageInfo> items = new List<UsageInfo>();
		
		Object[] allObjects = Resources.FindObjectsOfTypeAll<Object>() as Object[];
		
		System.Text.StringBuilder stringBuilder;
		
		foreach (Object obj in allObjects)
		{
			int bytes = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(obj);
			
			if (bytes >= MIN_SIGNIFICANT_BYTES)
			{
				string strDescriptor;
				
				// hrrm. Some objects don't have name. Object type is better than nothing?
				strDescriptor = string.IsNullOrEmpty(obj.name)? obj.GetType().ToString() : obj.name;
				
				stringBuilder = new System.Text.StringBuilder("ID: " + strDescriptor + " " + GameUtils.FormatBytes(bytes));
				
				UsageInfo info = new UsageInfo();
				info.display = stringBuilder.ToString();
				info.bytes = bytes;
				
				items.Add(info);
			}
		}
		
		items.Sort((i1, i2) => i1.bytes.CompareTo(i2.bytes));
		items.Reverse();
		
		DebugSystem.Log("**** Object Memory Dump Start ****");
		
		for (int i = 0; i < items.Count; i++)
		{
			DebugSystem.Log(items[i].display);
		}
		
		DebugSystem.Log("**** Object Memory Dump End ****");
	}
	
	public void OnPreviousValuesLoaded()
	{
	}
	
	public void OnValueChanged(System.Reflection.FieldInfo field, object oldValue, object newValue)
	{
	}
#if UNITY_IOS &&!UNITY_EDITOR
	[DllImport ("__Internal")]
	private static extern float getMemoryUsage();
#endif
	#endif
}
