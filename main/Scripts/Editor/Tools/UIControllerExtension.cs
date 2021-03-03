using UnityEngine;
using UnityEditor;

public static class UIControllerExtension
{
	public static GameObject GetGameObject(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			string assetPath = AssetImporter.GetAtPath(path).assetPath;
			
			GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
			return prefab;
		}

		return null;
	}

	public static T GetILR<T>(GameObject go) where T : MonoBehaviour
	{
		if (go)
		{
			return go.GetComponent<T>();
		}
		return default;
	}

	private static string _warnRect;

	public static bool IsReadable(string path, UIRect.AnchorUpdate anchor = UIRect.AnchorUpdate.OnUpdate)
	{
		GameObject go = GetGameObject(path);

		if (go != null)
		{
			var ctrl = GetILR<UIControllerILR>(go);
			var dyna = GetILR<DynamicMonoILR>(go);

			if (ctrl == null && dyna == null)
			{
				return false;
			}

			bool result = IsWidgetAnchor(go.transform, anchor);
			if (result)
			{
				Debug.LogWarning("路径为[" + path + "]的UI，UIRect.AnchorUpdate Is OnUpdate!，节点为："+ _warnRect);
			}

			return result;
		}

		return false;
	}

	public static bool IsWidgetAnchor(Transform node, UIRect.AnchorUpdate anchor)
	{
		var rects = node.GetComponentsInChildren<UIRect>(true);
		bool result = false;
		foreach (var rect in rects)
		{
			if (rect.isAnchored && rect.updateAnchors == anchor)
			{
				result = true;
				_warnRect = rect.name;
				break;;
			}
		}
		return result;
	}

	public static void SetAvailable(string path, bool isStart)
	{
		GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
		GameObject go = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
		if (go)
		{
			var rects = go.GetComponentsInChildren<UIRect>(true);
			foreach (var rect in rects)
			{
				if (rect.updateAnchors == UIRect.AnchorUpdate.OnUpdate)
				{
					rect.updateAnchors = isStart ? UIRect.AnchorUpdate.OnStart : UIRect.AnchorUpdate.OnEnable;
				}
			}

			PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, UnityEditor.InteractionMode.UserAction);
			//Object.DestroyImmediate(go);
		}
	}
}