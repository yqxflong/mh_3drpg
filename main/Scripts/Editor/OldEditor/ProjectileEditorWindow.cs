using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProjectileEditorWindow : EditorWindow 
{
	#if UNITY_EDITOR
	void OnEnable()
	{	
		minSize = new Vector2(350, 250);

		MoveEditor.MoveEditorWindow.InitCameraShakePresets(_cameraShakePresets);
		InitProjectileList();
		
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}
	
	void OnSceneGUI(SceneView view)
	{
	
	}
	
	void OnDisable()
	{
		CleanupPreviewObject();
	}
	
	void OnDestroy()
	{	
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		
		CleanupPreviewObject();
	}
	
	void OnProjectChange()
	{
		MoveEditor.MoveEditorWindow.InitCameraShakePresets(_cameraShakePresets);
		InitProjectileList();
		
		if (_projectile == null)
		{
			Repaint();
		}
	}
	
	void OnHierarchyChange()
	{
		if (_previewGameObject == null)
		{
			CleanupPreviewObject();
			Repaint();
		}
	}
	
	void Update()
	{	
		CheckApplicationState();
	}

	public static string GetProjectilesDirectoryPath()
	{
		return string.Format("{0}/Art/Projectiles/", Application.dataPath);
	}

	private void InitProjectileList()
	{
		_projectileNames.Clear();
		_projectilePaths.Clear();

		_projectileNames.Add("[null]");
		_projectilePaths.Add("");
		
		// 

		List<string> projectileNames = GetAllProjectileNames();
		
		if (projectileNames != null)
		{
			projectileNames.Sort();
			_projectileNames.AddRange( projectileNames);
			
			string path = GetProjectilesDirectoryPath();
			path = path.Substring(path.IndexOf("Assets"));
			
			for (int i = 0; i < projectileNames.Count; i++)
				_projectilePaths.Add(string.Format("{0}{1}.prefab", path, projectileNames[i]));
		}
	}


	private List<string> GetAllProjectileNames()
	{
		string[] files = System.IO.Directory.GetFiles(GetProjectilesDirectoryPath(), "*.prefab");
		for (int i = 0; i < files.Length; i++)
		{
			int startIndex = GetProjectilesDirectoryPath().Length;
			int length = files[i].IndexOf(".prefab") - startIndex;
			files[i] = files[i].Substring(startIndex, length);
		}

		List<string> fileNames = new List<string>( files );
		return fileNames;
	}


	private void CheckApplicationState()
	{
		if (Application.isPlaying != EditorPrefs.GetBool("MoveEdApplicationPlaying"))
			ApplicationStateChanged();
	}
	
	private void ApplicationStateChanged()
	{
		if (_previewParticleSystem != null)
			DestroyImmediate(_previewParticleSystem.gameObject);
		
		Repaint();
	}


	private void OnGUI()
	{
		DrawMain();

		if( _projectile != null )
		{
			DrawMotion();
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
			{
				DrawProjectileEvents( _projectile._onSpawnEvents, "On Spawn Effects", "proj_spawn_effects" );
				DrawProjectileEvents( _projectile._onHitEvents, "On Hit Effects", "proj_hit_effects" );
			}
			GUILayout.EndScrollView();

			EditorUtility.SetDirty(_projectile);
		}
	}

	private void DrawMain()
	{
		if (NGUIEditorTools.DrawHeader("Main Info", "proj_general"))
		{
			NGUIEditorTools.BeginContents();
			
			EditorGUILayout.BeginHorizontal();
			{
				int index = _projectile != null ? _projectileNames.IndexOf(_projectile.name) : 0;
				index = Mathf.Max(index, 0);
				
				int testValue = EditorGUILayout.Popup("Projectiles (List)", index, _projectileNames.ToArray());
				
				if (index != testValue)
				{
					index = testValue;
					if (index != 0)
					{
						GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(_projectilePaths[index], typeof(GameObject));
						SetProjectile(prefab.GetComponent<ProjectileController>());
					}
					else
					{
						SetProjectile(null);
					}
				}
				
				if (GUILayout.Button("X", GUILayout.Width(20)))
					SetProjectile(null);
			}
			EditorGUILayout.EndHorizontal();
			
			NGUIEditorTools.EndContents();
		}
 	}

	private void DrawMotion()
	{
		if(NGUIEditorTools.DrawHeader("Projectile Motion", "proj_motion"))
		{
			NGUIEditorTools.BeginContents();
			EditorGUILayout.BeginHorizontal();
			{
				_projectile._motionCurve = EditorGUILayout.CurveField("Motion Curve", _projectile._motionCurve);
			}
			EditorGUILayout.EndHorizontal();
			NGUIEditorTools.EndContents();
		}
	}

	private void DrawProjectileEvents ( ProjectileController.ProjectileEvent events, string title, string key )
	{
		if (NGUIEditorTools.DrawHeader(title, key))
		{
			NGUIEditorTools.BeginContents();
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// particles
			for( int j = 0; j < events._particles.Count; ++j )
			{
				NGUIEditorTools.DrawHeader("Particle "+j);
				NGUIEditorTools.BeginContents();
				
				bool delete = false;
				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					delete = true;
				}
				MoveEditor.MoveEditorWindow.DrawParticleProperties( ref events._particles[j]._particleProperties, OnParticleFieldAdd, OnParticleFieldSelect, _previewGameObject, false, true );
				
				if( delete )
				{
					events._particles.RemoveAt(j);
					break;
				}
				
				NGUIEditorTools.EndContents();
				GUILayout.Space(5);
			}
			
			if( GUILayout.Button("Add Particle") )
			{
				events._particles.Add( new MoveEditor.ParticleEventInfo() );
			}
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// trails
			for( int j = 0; j < events._trails.Count; ++j )
			{
				NGUIEditorTools.DrawHeader("Trail "+j);
				NGUIEditorTools.BeginContents();
				
				bool delete = false;
				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					delete = true;
				}
				events._trails[j]._isEditCrit = EditorGUILayout.Toggle("Edit Crit?", events._trails[j]._isEditCrit);
				MoveEditor.MoveEditorWindow.DrawTrailRendererProperties( ref events._trails[j]._trailRendererProperties, events._trails[j]._isEditCrit, OnTrailLocatorAdd, OnTrailLocatorSelect, _previewGameObject, false, true );
				
				if( delete )
				{
					events._trails.RemoveAt(j);
					break;
				}

				NGUIEditorTools.EndContents();
				GUILayout.Space(5);
			}
			
			if( GUILayout.Button("Add Trail") )
			{
				events._trails.Add( new MoveEditor.TrailRendererEventInfo() );
			}
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// dynamic lights
			for( int j = 0; j < events._dynamicLights.Count; ++j )
			{
				NGUIEditorTools.DrawHeader("Dynamic Lights "+j);
				NGUIEditorTools.BeginContents();
				
				bool delete = false;
				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					delete = true;
				}
				MoveEditor.MoveEditorWindow.DrawDynamicLightProperties( ref events._dynamicLights[j]._dynamicLightProperties, OnDynamicLightFieldAdd, OnDynamicLightFieldSelect, _previewGameObject, false, true );
				
				if( delete )
				{
					events._dynamicLights.RemoveAt(j);
					break;
				}

				NGUIEditorTools.EndContents();
				GUILayout.Space(5);
			}

			if( GUILayout.Button("Add Dynamic Lights") )
			{
				events._dynamicLights.Add( new MoveEditor.DynamicLightEventInfo() );
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// camera shakes
			for( int j = 0; j < events._cameraShakes.Count; ++j )
			{
				NGUIEditorTools.DrawHeader("Camera Shakes "+j);
				NGUIEditorTools.BeginContents();
				
				bool delete = false;
				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					delete = true;
				}
				MoveEditor.MoveEditorWindow.DrawCameraShakeProperties( ref events._cameraShakes[j]._cameraShakeProperties, _cameraShakePresets );
				
				if( delete )
				{
					events._cameraShakes.RemoveAt(j);
					break;
				}
				
				NGUIEditorTools.EndContents();
				GUILayout.Space(5);
			}
			
			if( GUILayout.Button("Add Camera Shake") )
			{
				events._cameraShakes.Add( new MoveEditor.CameraShakeEventInfo() );
			}
			
			NGUIEditorTools.EndContents();
		}
	}
	
	private void OnParticleFieldAdd(MoveEditor.ParticleEventProperties properties, ParticleSystem ps, bool flipped)
	{
		CleanUpPreviewParticle();
		
		if (ps != null && _previewGameObject != null)
		{
			_previewParticleSystem = MoveEditor.MoveUtils.InstantiateParticle(this,properties, GetAnimator(), flipped, true);
			if (_previewParticleSystem != null)
			{
				AttachTransform.Detach(_previewParticleSystem.gameObject);
				_previewParticleSystem.transform.parent = MoveEditor.MoveUtils.GetBodyPartTransform(GetAnimator(), properties._bodyPart, properties._attachmentPath);
				Selection.activeTransform = _previewParticleSystem.transform;
			}
		}
	}
	
	private void OnParticleFieldSelect()
	{
		if (_previewParticleSystem != null)
		{
			Selection.activeTransform = _previewParticleSystem.transform;
		}
	}
	
	private void CleanUpPreviewParticle()
	{
		if (_previewParticleSystem != null)
		{
			DestroyImmediate(_previewParticleSystem.gameObject);
		}
	}
	
	private void OnTrailLocatorAdd( MoveEditor.TrailRendererEventProperties properties )
	{
		CleanUpTrailLocator();
		
		if (_previewGameObject != null)
		{
			_trailLocator = new GameObject("trail_locator");
			_trailLocator.transform.parent = MoveEditor.MoveUtils.GetBodyPartTransform(GetAnimator(), properties._attachment, properties._attachmentPath);
			_trailLocator.transform.localPosition = properties._rigOffset;
			Selection.activeTransform = _trailLocator.transform;
		}
	}
	
	private void OnTrailLocatorSelect()
	{
		if (_trailLocator != null)
		{
			Selection.activeTransform = _trailLocator.transform;
		}
	}
	
	private void CleanUpTrailLocator()
	{
		if( _trailLocator != null )
		{
			DestroyImmediate(_trailLocator);
		}
	}
	
	private void OnTrailSelectorLocatorCallback()
	{
		if (_trailLocator != null)
			Selection.activeTransform = _trailLocator.transform;
	}
	
	private void CleanUpDynamicLightPreview()
	{
		
	}
	
	private void OnDynamicLightFieldAdd( MoveEditor.DynamicLightEventProperties properties, bool flipped )
	{
		CleanUpDynamicLightPreview();
		
		if ( _previewGameObject != null)
		{
			_previewDynamicLight = MoveEditor.MoveUtils.InstantiateDynamicLight(properties, GetAnimator(), flipped, true);
			if (_previewDynamicLight != null)
			{
				_previewDynamicLight.transform.parent = MoveEditor.MoveUtils.GetBodyPartTransform(GetAnimator(), properties._attachment, properties._attachmentPath);
				Selection.activeTransform = _previewDynamicLight.transform;
			}
		}
	}
	
	private void OnDynamicLightFieldSelect()
	{
		if (_previewDynamicLight != null)
			Selection.activeTransform = _previewDynamicLight.transform;
	}
	
	private Animator GetAnimator()
	{
		return _previewGameObject.GetComponent<Animator>();
	}
	
	private void CleanupPreviewObject()
	{
		CleanUpDynamicLightPreview();
		CleanUpTrailLocator();
		CleanUpPreviewParticle();
		if( _previewGameObject != null )
		{
			DestroyImmediate( _previewGameObject );
		}
	}

	public void SetProjectile(ProjectileController projectile)
	{
		_projectile = projectile;
		CleanupPreviewObject();

		if (projectile != null)
		{
		//	EditorPrefs.SetString("MoveEdLastMove", move.name);
		//	move.SetPendingChanges();
			_previewGameObject = GameObject.Instantiate(projectile.gameObject) as GameObject;//(GameObject)PrefabUtility.InstantiatePrefab( projectile.gameObject );
			EditorUtility.SetDirty(projectile);
		}
	}

	
	private static Vector2	_scrollPosition	= Vector2.zero;
	private ParticleSystem _previewParticleSystem = null;
	private GameObject _previewGameObject = null;
	private GameObject _trailLocator = null;
	private GameObject _previewDynamicLight = null;
	private ProjectileController _projectile = null;

	private List<string> _projectilePaths = new List<string>();
	private List<string> _projectileNames = new List<string>();

	private SortedDictionary<string, MoveEditor.CameraShakePreset> _cameraShakePresets = new SortedDictionary<string, MoveEditor.CameraShakePreset>();

#endif

}
