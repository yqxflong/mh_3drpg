using UnityEditor;
using UnityEngine;
using System.Collections;

public class ListViewWindow : EditorWindow
{
	public static void Open(string title, string[] options, System.Action<string> onSelection = null, bool closeOnSelect = true)
	{
		Vector2 mousePosition;

		if (Event.current != null)
		{
			mousePosition = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
		}
		else
		{
			mousePosition = new Vector2(Screen.height - 100, Screen.width * 0.5f - _defaultWidth * 0.5f);
		}

		//float x 		= mousePosition.x < Screen.width - _defaultWidth ? mousePosition.x : mousePosition.x - _defaultWidth;
		//float y 		= mousePosition.y > _defaultHeight ? mousePosition.y : _defaultHeight;
		Rect windowRect = new Rect(mousePosition.x, mousePosition.y, _defaultWidth, _defaultHeight);

		ListViewWindow window 	= EditorWindow.GetWindowWithRect<ListViewWindow>(windowRect, true, title, true);
		window.position			= windowRect;
		window._options			= options;
		window._onSelection		= onSelection;
		window._closeOnSelect	= closeOnSelect;
	}

	private void OnGUI()
	{
		if (_options != null)
		{
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
			{
				for (int i = 0; i < _options.Length; i++)
				{
					if (GUILayout.Button(_options[i]))
					{
						if (_onSelection != null)
						{
							_onSelection(_options[i]);
						}

						if (_closeOnSelect)
						{
							Close();
						}
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}
	}

	private string[]					_options		= null;
	private System.Action<string> 		_onSelection 	= null;
	private bool						_closeOnSelect	= true;
	private Vector2						_scrollPosition	= Vector2.zero;
	private const	float				_defaultWidth 	= 400;
	private const 	float				_defaultHeight	= 600;
}
