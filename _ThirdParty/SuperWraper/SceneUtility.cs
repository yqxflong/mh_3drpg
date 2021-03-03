using UnityEngine;

namespace Unity.Standard.ScriptsWarp
{
	public class SceneUtility
	{
		public static GameObject GetTopObject(string path)
		{
			string name = string.Empty;
			if (path.Contains("/"))
			{
				string[] strs = path.Split('/');
				name = strs[0];
			}
			else
			{
				name = path;
			}
			UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			GameObject[] rootObjs = scene.GetRootGameObjects();
			for (int i = 0; i < rootObjs.Length; i++)
			{
				GameObject obj = rootObjs[i];
				if (obj.name == name)
				{
					if (path.Contains("/") == false)
						return obj;
					else
					{
						string subPath = path.Replace(name + "/", string.Empty);
						Transform target = obj.transform.Find(subPath);
						if (target) return target.gameObject;
					}
				}

			}
			return null;
		}

		public static T GetTopObject<T>(string name)
		{
			UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			GameObject[] rootObjs = scene.GetRootGameObjects();
			for (int i = 0; i < rootObjs.Length; i++)
			{
				GameObject obj = rootObjs[i];
				if (obj.name == name)
					return obj.GetComponent<T>();
			}
			return default(T);
		}
	}

}