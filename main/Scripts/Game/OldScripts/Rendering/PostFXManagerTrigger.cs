using UnityEngine;

[ExecuteInEditMode]
public class PostFXManagerTrigger : MonoBehaviour
{
	Camera _camera = null;

	public void Start()
	{
		_camera = GetComponent<Camera>();
		//turn off the warp layer
		_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Warp"));
	}

	//public void OnRenderImage(RenderTexture src, RenderTexture dst)
	//{
	//	PostFXManager.Instance.PostRender(_camera, src, dst);
	//}
}

