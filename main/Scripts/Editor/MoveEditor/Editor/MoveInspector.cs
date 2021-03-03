using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MoveEditor
{
#if UNITY_EDITOR
	[CustomEditor(typeof(Move))]
	public class MoveInspector : Editor 
	{
		void OnEnable()
		{
			_move = (Move)target;
		}
	
		public override void OnInspectorGUI ()
		{
			if (GUILayout.Button("Open Editor"))
			{
				MoveEditorWindow editor = MoveEditorUtils.ShowMoveEditorWindow();
				editor.SetMove(_move, null, true);
			}
			
			if (GUILayout.Button("Build Move"))
			{
				MoveEditorUtils.BuildMove(_move);
			}

			if (GUILayout.Button("Select In Project"))
			{
				EditorGUIUtility.PingObject(_move.gameObject);
				Selection.activeGameObject = _move.gameObject;
			}
	
			DrawDefaultInspector();

			for (int i = 0; i < _move._hitEvents.Count; ++i)
			{
				EditorGUILayout.ObjectField(_move._hitEvents[i]._particleProperties._particleReference.Value, typeof(ParticleSystem), false);
				EditorGUILayout.ObjectField(_move._hitEvents[i]._particleProperties._flippedParticleReference.Value, typeof(ParticleSystem), false);
			}

			for (int i = 0; i < _move._particleEvents.Count; ++i)
			{
				EditorGUILayout.ObjectField(_move._particleEvents[i]._particleProperties._particleReference.Value, typeof(ParticleSystem), false);
				EditorGUILayout.ObjectField(_move._particleEvents[i]._particleProperties._flippedParticleReference.Value, typeof(ParticleSystem), false);
			}

			//EditorUtility.SetDirty(_move);
		}
	
		private Move _move;
	}
#endif
}
