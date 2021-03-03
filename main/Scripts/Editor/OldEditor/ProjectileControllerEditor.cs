using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(ProjectileController))]
public class ProjectileControllerEditor : Editor 
{
	void OnEnable()
	{
		_projectile = (ProjectileController)target;
	}

	public override void OnInspectorGUI ()
	{
		if (GUILayout.Button("Open Editor"))
		{
			ProjectileEditorWindow editor = MoveEditor.MoveEditorUtils.ShowProjectileEditorWindow();
			editor.SetProjectile(_projectile);
		}
		
		DrawDefaultInspector();
	}

	private ProjectileController _projectile;
}

#endif
