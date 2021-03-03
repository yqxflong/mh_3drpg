using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.iOS;

public static class Misc
{
	public static bool IsPhone()
	{
#if UNITY_IPHONE
		switch (UnityEngine.iOS.Device.generation)
		{
			case UnityEngine.iOS.DeviceGeneration.iPhone:
			case UnityEngine.iOS.DeviceGeneration.iPhone3G:
			case UnityEngine.iOS.DeviceGeneration.iPhone3GS:
			case UnityEngine.iOS.DeviceGeneration.iPhone4:
			case UnityEngine.iOS.DeviceGeneration.iPhone4S:
			case UnityEngine.iOS.DeviceGeneration.iPhone5:
			case UnityEngine.iOS.DeviceGeneration.iPhone5C:
			case UnityEngine.iOS.DeviceGeneration.iPhone5S:
			case UnityEngine.iOS.DeviceGeneration.iPhone6:
			case UnityEngine.iOS.DeviceGeneration.iPhone7:
			case UnityEngine.iOS.DeviceGeneration.iPhone7Plus:
			case UnityEngine.iOS.DeviceGeneration.iPhone6Plus:
			case UnityEngine.iOS.DeviceGeneration.iPhone6S:
			case UnityEngine.iOS.DeviceGeneration.iPhone6SPlus:
			case UnityEngine.iOS.DeviceGeneration.iPhoneSE1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPhoneUnknown:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouchUnknown:
				return true;
		}
#endif
		return false;
	}

	public static bool IsTablet()
	{
#if UNITY_IPHONE
		switch (UnityEngine.iOS.Device.generation)
		{
			case UnityEngine.iOS.DeviceGeneration.iPad1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad2Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad3Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadAir1:
			case UnityEngine.iOS.DeviceGeneration.iPadAir2:
			case UnityEngine.iOS.DeviceGeneration.iPadMini1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadMini2Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadMini3Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadMini4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadPro1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadUnknown:
				return true;
		}
#endif
		return false;
	}

	public static bool HDAtlas()
	{
#if UNITY_IPHONE
		switch (UnityEngine.iOS.Device.generation)
		{
			case UnityEngine.iOS.DeviceGeneration.iPhone4:
			case UnityEngine.iOS.DeviceGeneration.iPhone4S:
			case UnityEngine.iOS.DeviceGeneration.iPhone5:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad2Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadMini1Gen:
				return false;

			case UnityEngine.iOS.DeviceGeneration.iPhone5C:
			case UnityEngine.iOS.DeviceGeneration.iPhone5S:
			case UnityEngine.iOS.DeviceGeneration.iPhone6:
			case UnityEngine.iOS.DeviceGeneration.iPhone6Plus:
			case UnityEngine.iOS.DeviceGeneration.iPhone6S:
			case UnityEngine.iOS.DeviceGeneration.iPhone6SPlus:
			case UnityEngine.iOS.DeviceGeneration.iPhoneSE1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPhone7:
			case UnityEngine.iOS.DeviceGeneration.iPhone7Plus:
			case UnityEngine.iOS.DeviceGeneration.iPad3Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadAir1:
			case UnityEngine.iOS.DeviceGeneration.iPadAir2:
			case UnityEngine.iOS.DeviceGeneration.iPadMini2Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadMini3Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadMini4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadPro1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadUnknown:
			default:
				return true;
		}
#elif UNITY_ANDROID
		// TODO: what metric to use for Android?
		if(Screen.dpi > 300f)
		{
			return true;
		}
		else
		{
			return false;
		}
#elif UNITY_EDITOR
		return true;
#else
		return false;
#endif
	}

	public static bool IsRetina()
	{
#if UNITY_IPHONE
		switch (UnityEngine.iOS.Device.generation)
		{
			case UnityEngine.iOS.DeviceGeneration.iPhone4:
			case UnityEngine.iOS.DeviceGeneration.iPhone4S:
			case UnityEngine.iOS.DeviceGeneration.iPhone5:
			case UnityEngine.iOS.DeviceGeneration.iPhone5C:
			case UnityEngine.iOS.DeviceGeneration.iPhone5S:
			case UnityEngine.iOS.DeviceGeneration.iPhone6:
			case UnityEngine.iOS.DeviceGeneration.iPhone6Plus:
			case UnityEngine.iOS.DeviceGeneration.iPhoneSE1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPhone7:
			case UnityEngine.iOS.DeviceGeneration.iPhone7Plus:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad3Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadAir1:
			case UnityEngine.iOS.DeviceGeneration.iPadAir2:
			case UnityEngine.iOS.DeviceGeneration.iPadMini2Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadMini3Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadMini4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadPro1Gen:
				return true;
		}
#elif UNITY_ANDROID
		if(Screen.dpi > 300f)
		{
			return true;
		}		
#endif
		return false;
	}

	public static bool IsLowMemoryRetina()
	{
#if UNITY_IPHONE
		switch (UnityEngine.iOS.Device.generation)
		{
			case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:
			case UnityEngine.iOS.DeviceGeneration.iPhone4:
			case UnityEngine.iOS.DeviceGeneration.iPhone4S:
				return true;
		}

		return false;
#else
		return false;
#endif
	}

	public static bool IsSlow()
	{
#if UNITY_IPHONE
		switch (UnityEngine.iOS.Device.generation)
		{
			case UnityEngine.iOS.DeviceGeneration.iPhone4:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:
				EB.Debug.Log("Platform is slow!");
				return true;
		}
#endif
		return false;
	}

	public static Mesh MakeQuadMesh(float xmin, float ymin, float xmax, float ymax)
	{
		var mesh = new Mesh();
		mesh.vertices = new Vector3[] { new Vector3(xmin, ymin, 0), new Vector3(xmin, ymax, 0), new Vector3(xmax, ymax, 0), new Vector3(xmax, ymin, 0), };
		mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), };
		mesh.triangles = new int[] { 0, 1, 3, 1, 2, 3 };

		return mesh;
	}

	public static Mesh MakeCircleMesh(float radius, int segs)
	{
		Mesh m = new Mesh();
		Vector3[] v = new Vector3[segs+1];
		Vector2[] uv = new Vector2[segs+1];
		int[] t = new int[segs*3];

		float delta = (360.0f/segs)*Mathf.Deg2Rad;
		float sina = Mathf.Sin(delta);
		float cosa = Mathf.Cos(delta);

		float x = 1.0f;
		float y = 0.0f;

		int i=0;
		for (; i < segs; ++i)
		{
			v[i] = new Vector3(x * radius, y * radius, 0.0f);

			uv[i] = new Vector2(x * radius + 0.5f, y * radius + 0.5f);
			float x_ = x*cosa - y*sina;
			y = x * sina + y * cosa;
			x = x_;

			int baseIdx = (i*3);
			t[baseIdx] = (i + 1) % segs;
			t[baseIdx + 1] = i % segs;
			t[baseIdx + 2] = segs;
		}

		v[i] = Vector3.zero;
		uv[i] = new Vector2(0.5f, 0.5f);

		m.vertices = v;
		m.triangles = t;
		m.uv = uv;

		return m;
	}


	public static GameObject[] GetObjects(GameObject obj, string name)
	{
		List<GameObject> list = new List<GameObject>();

		if (obj == null) return list.ToArray();

		if (obj.name.Contains(name))
		{
			list.Add(obj);
		}

		foreach (UnityEngine.Transform t in obj.transform)
		{
			foreach (var tt in GetObjects(t.gameObject, name))
			{
				list.Add(tt);
			}
		}

		return list.ToArray();
	}

	public static GameObject GetObject(GameObject obj, string name)
	{
		return GetObject(obj, name, false);
	}

	public static GameObject GetObjectExactMatch(GameObject obj, string name)
	{
		return GetObject(obj, name, true);
	}

	public static GameObject GetObject(GameObject obj, string name, bool bExactMatch)
	{
		if (obj == null) return null;

		if (!bExactMatch)
		{
			if (obj.name.Contains(name))
			{
				return obj;
			}
		}
		else if (obj.name == name)
		{
			return obj;
		}

		foreach (UnityEngine.Transform t in obj.transform)
		{
			GameObject result = GetObject(t.gameObject, name, bExactMatch);
			if (result != null)
			{
				return result;
			}
		}

		return null;
	}

	public static T FindOrAdd<T>(GameObject root) where T : Component
	{
		var t = root.GetComponent<T>();
		if (!t)
		{
			return root.AddComponent<T>();
		}
		return t;
	}

	public static T FindInTreeOrAdd<T>(GameObject root) where T : Component
	{
		var t = root.GetComponentInChildren<T>();
		if (!t)
		{
			return root.AddComponent<T>();
		}
		return t;
	}
}

	

