using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ParticleHeightFade : MonoBehaviour
{
	#if UNITY_EDITOR
	public void Awake()
	{
		SceneView.onSceneGUIDelegate += OnSceneView;
	}

	public void OnSceneView(SceneView sceneView)
	{
		if (sceneView.camera)
		{
			Shader.SetGlobalMatrix("_Camera2World", sceneView.camera.cameraToWorldMatrix);
		}
	}
	
	public void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= OnSceneView;
	}

	static string currentScene;
	static ParticleHeightFade()
	{
		currentScene = string.Empty;
		EditorApplication.update += EditorUpdate;
		EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
	}
	
	static void EditorUpdate()
	{
		ApplyParticleHeightFadeScript(false);
	}
	
	static void PlaymodeStateChanged()
	{
		ApplyParticleHeightFadeScript(true);
	}

	static void ApplyParticleHeightFadeScript(bool force)
	{
		if (!force && (currentScene == UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path))
		{
			return;
		}

		currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;

		Camera[] cameras = GameObject.FindObjectsOfType<Camera>();

		for(int i = 0; i < cameras.Length; ++i)
		{
			if (cameras[i] == null)
			{
				continue;
			}

			ParticleHeightFade particleHeightFade = cameras[i].gameObject.GetComponent<ParticleHeightFade>();
			if (particleHeightFade != null)
			{
				continue;
			}

			cameras[i].gameObject.AddComponent<ParticleHeightFade>();
		}
	}

#endif

    //private Camera mCamera = null;

	//public void OnPreRender() 
	//{
 //       if (mCamera == null)
 //       {
 //           mCamera = this.GetComponent<Camera>();
 //       }
	//	Shader.SetGlobalMatrix("_Camera2World", mCamera.cameraToWorldMatrix);
	//}
}
