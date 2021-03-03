using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MoveEditor
{
	public class NewMoveEditorWindow : EditorWindow 
	{
#if UNITY_EDITOR
		void OnGUI()
		{
			_name = EditorGUILayout.TextField("Name", _name);
			_clip = (AnimationClip) EditorGUILayout.ObjectField("Animation Clip", _clip, typeof(AnimationClip), false);
            _isCommon = EditorGUILayout.ToggleLeft("Is Common Only", _isCommon);

            if (GUILayout.Button("Create"))
				CreateNewMove();
		}
	
		void CreateNewMove()
		{
			GameObject go = new GameObject(_name);
			Move move = go.AddComponent<Move>();
			move._animationClip = _clip;
			
			if (MoveEditorUtils.IsClipShared(move))
			{
				EditorUtility.DisplayDialog("ERROR", "Another move is already using this animation clip! Only one move may be assigned to an animation clip", "OK");
				return;
			}
				
			if (System.IO.File.Exists(MoveEditorUtils.GetMoveFilePath(_name)))
			{
				if (!EditorUtility.DisplayDialog("Overwriting Prefab", "A prefab with this name already exists. Are you sure you want to overwrite it?", "Yes", "No"))
				{
					DestroyImmediate(go);
					return;
				}
			}
			
			if (!System.IO.Directory.Exists(MoveEditorUtils.GetMovesDirectoryPath()))
				System.IO.Directory.CreateDirectory(MoveEditorUtils.GetMovesDirectoryPath());

            string prefabPath = _isCommon ? MoveEditorUtils.GetCommonMovePrefabPath(_name) : MoveEditorUtils.GetMovePrefabPath(_name);
			GameObject prefab = PrefabUtility.CreatePrefab(prefabPath, go);
			DestroyImmediate(go);
			
			if (prefab != null)
			{
				EditorGUIUtility.PingObject(prefab);
				
				if (EditorUtility.DisplayDialog("Success!", "New move created successfully! Open it in the Move Editor now?", "Yes", "No"))
				{
					MoveEditorWindow editor = MoveEditorUtils.ShowMoveEditorWindow();
					editor.SetMove(prefab.GetComponent<Move>(), null, true);
				}
			}
		}
	
		private string 			_name;
		private AnimationClip 	_clip;

        private bool _isCommon;
#endif
	}
}
