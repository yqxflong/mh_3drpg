///////////////////////////////////////////////////////////////////////
//
//  LogSystem.cs
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
using System.IO;

#if DEBUG

public partial class DebugSystemComponent
{
	private partial class LogSystem : IDebuggable
	{
		public bool showLog = false;

#if UNITY_EDITOR
		private string _logPath = Application.persistentDataPath + "/" + (Replication.IsHost ? "host_" : "client_") + "fusion_log_" + System.DateTime.Now.Ticks + ".txt";
#else
		private string _logPath = Application.persistentDataPath + "/" + (Replication.IsHost ? "host_" : "client_") + "fusion_log.txt";
#endif

		private int _lineCount = 0;

		private StreamReader _reader = null;

		// GUI variables
		private Rect _windowRect = new Rect(0, 0, Screen.width, Screen.height);
		private Vector2 _scrollPos = new Vector2();
		private float windowWidth = Screen.width;
		private float windowHeight = Screen.height;
		private int fontSize = Screen.width / 80;
		private float labelHeight = Screen.width / 40;
		private float _scrollBarWidth = Screen.width / 20.0f;

		private GUIContent _reusableContent = new GUIContent();

		public void OnDrawDebug()
		{
		}

		public void OnDebugPanelGUI()
		{
		}

		public void OnDebugGUI()
		{
			_windowRect = new Rect(0, 0, windowWidth, windowHeight);
			if (showLog)
			{
				_windowRect = GUI.Window(1, _windowRect, DrawLog, "Log");
			}
		}

		public void Close()
		{
			if (_reader != null)
			{
				_reader.Close();
				_reader = null;
			}
		}

		private void Initialize()
		{
			if (File.Exists(_logPath))
			{
				File.Delete(_logPath);
			}

			//File.Create(_logPath);

			// Listen to Unity log
			Application.logMessageReceived += OnUnityLog;
		}

		void OnUnityLog(string message, string trace, LogType type)
		{
			// Just keep exceptions
		    if (Application.isEditor)
		    {
		        return;
		    }
			if (type == LogType.Exception)
			{
				Log(message + "\n" + trace, LogType.Error);
			}
		}

		private void DrawLog(int windowID)
		{
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.verticalScrollbar.fixedWidth = _scrollBarWidth;
			GUI.skin.verticalScrollbarThumb.fixedWidth = _scrollBarWidth;

			if (_reader != null)
			{
				// Reset the reader's position
				_reader.BaseStream.Seek(0, SeekOrigin.Begin);
				_reader.DiscardBufferedData();

				_scrollPos = GUI.BeginScrollView(new Rect(0.0f, 0.0f, windowWidth, windowHeight), _scrollPos, new Rect(0.0f, 0.0f, windowWidth, labelHeight * _lineCount));

				int counter = 0;
				while (!_reader.EndOfStream)
				{
					string line = _reader.ReadLine();
					_reusableContent.text = line;
					float height = GUI.skin.label.CalcHeight(_reusableContent, windowWidth);
					int labelLineSize = Mathf.CeilToInt(height / labelHeight);

					if (!line.Contains("["))
					{
						GUI.color = Color.red;
					}
					else
					{
						int startLogType = line.IndexOf('[') + 1;
						int endLogType = line.IndexOf(']');
						string logType = line.Substring(startLogType, endLogType - startLogType);

						int start = line.LastIndexOf('[') + 1;
						int end = line.LastIndexOf(']');
						string systemName = line.Substring(start, end - start);

						GUI.color = (logType == "Error" || logType == "Exception") ?
							Color.red :
							logType == "Warning" ?
								Color.yellow :
								DebugSystem.GetSystemColor(DebugSystem.GetSystem(systemName));
					}
					GUI.Label(new Rect(0.0f, counter * labelHeight, windowWidth, labelHeight * labelLineSize), line);
					GUI.color = Color.white;

					counter += labelLineSize;
				}

				GUI.EndScrollView();
			}

			GUI.DragWindow();
		}

		public void Log(object message, LogType importance)
		{
			// Cannot read and write at the same time so stop reading
			if (_reader != null)
			{
				_reader.Close();
				_reader = null;
			}

			// Write in the log file
			File.AppendAllText(_logPath, "[" + importance + "] " + message.ToString().Trim('\n', ' ') + "\n");

			// Resume reading
			_reader = new StreamReader(File.OpenRead(_logPath));

			// Adjust scroll bar (if it is at the very bottom, keep it at the very bottom, otherwise leave it alone)
			if (_scrollPos.y >= _lineCount * labelHeight - windowHeight - 0.5f)
			{
				_scrollPos.y += (_lineCount + 1) * labelHeight - windowHeight;
			}

			// Update line number
			_lineCount += message.ToString().Split('\n').Length;

			// Print in Unity's console
			switch (importance)
			{
				case LogType.Log:
				case LogType.Assert:
					EB.Debug.Log(message);
					break;
				case LogType.Warning:
					EB.Debug.LogWarning(message);
					break;
				case LogType.Error:
				case LogType.Exception:
					EB.Debug.LogError(message);
					break;
			}
		}
	}
}
#endif
