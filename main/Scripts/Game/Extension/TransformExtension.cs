using Boo.Lang;
using System.Text;
using UnityEngine;

public static class TransformExtension
{
	public static T GetComponentInParentEx<T>(this Transform t) where T : Component
	{
		if (t != null)
		{
			var c = t.GetComponent<T>();

			if (c != null)
			{
				return c;
			}
			else
			{
				return t.parent.GetComponentInParentEx<T>();
			}
		}

		return null;
	}

    public static Transform FindEx(this Transform t, string name, bool showErrorTips = true)
    {
        var child = string.IsNullOrEmpty(name) ? t : t.Find(name);

        if (child != null)
        {
            return child;
        }

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't find child: {0}/{1}", t.GetPath(), name);
		}

        return null;
    }
    
    public static T GetComponent<T>(this Transform t, string name, bool showErrorTips = true) where T : Component {
		var child = string.IsNullOrEmpty(name) ? t : t.Find(name);

		if (child != null) {
			var comp = child.GetComponent<T>();

			if (comp != null) {
				return comp;
			} 
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {2} component by: {0}/{1}", t.GetPath(), name, typeof(T).FullName);
		}

		return null;
	}

    public static T GetSuperComponent<T>(this Transform t, string name, bool showErrorTips = true) where T : Object
    {
	    var child = string.IsNullOrEmpty(name) ? t : t.Find(name);

	    if (child != null)
	    {
		    var comp = child.GetComponent<T>();

		    if (comp != null)
		    {
			    return comp;
		    }
	    }

	    if (showErrorTips)
	    {
		    EB.Debug.LogError("Can't get {2} component by: {0}/{1}", t.GetPath(), name, typeof(T).FullName);
	    }

	    return null;
	}

    public static T GetComponentEx<T>(this Transform t, bool showErrorTips = true) where T : Component {
		var comp = t.GetComponent<T>();

		if (comp != null) {
			return comp;
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeof(T).FullName);
		}

		return null;
	}

	public static T GetMonoILRComponentInChildren<T>(this Transform t, string typeFullName, bool includeInactive = true, bool showErrorTips = true) where T : DynamicMonoILRObject
	{
		var results = t.GetMonoILRComponentsInChildren<T>(typeFullName, includeInactive, showErrorTips);

		if (results != null && results.Length > 0)
		{
			return results[0];
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeFullName);
		}

		return null;
	}

	public static T[] GetMonoILRComponentsInChildren<T>(this Transform t, string typeFullName, bool includeInactive = true, bool showErrorTips = true) where T : DynamicMonoILRObject
	{
		var ilrs = t.GetComponentsInChildren<DynamicMonoILR>(includeInactive);
		List<T> list = new List<T>();

		for (var i = 0; i < ilrs.Length; i++)
		{
			var ilr = ilrs[i];

			if (ilr != null && ilr.hotfixClassPath.Equals(typeFullName))
			{
				if (ilr._ilrObject == null)
				{
					ilr.ILRObjInit();
				}

				if (ilr._ilrObject != null)
				{
					list.Add(ilr._ilrObject as T);
				}
			}
		}

		if (list.Count > 0)
		{
			return list.ToArray();
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeFullName);
		}

		return null;
	}

	public static T GetMonoILRComponent<T>(this Transform t, bool showErrorTips = true) where T : DynamicMonoILRObject {
		var ilr = t.GetComponent<DynamicMonoILR>();

		if (ilr != null) {
			if (ilr._ilrObject == null) {
				ilr.ILRObjInit();
			}

			if (ilr._ilrObject != null) {
				return ilr._ilrObject as T;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeof(T).FullName);
		}

		return null;
	}

	public static T GetMonoILRComponentByClassPath<T>(this Transform t, string hotfixClassPath, bool showErrorTips = true) where T : DynamicMonoILRObject
	{
		var ilrs = t.GetComponents<DynamicMonoILR>();

		for (var i = 0; i < ilrs.Length; i++)
		{
			var ilr = ilrs[i];

			if (ilr != null && ilr.hotfixClassPath.Equals(hotfixClassPath))
			{
				if (ilr._ilrObject == null)
				{
					ilr.ILRObjInit();
				}

				if (ilr._ilrObject != null)
				{
					return ilr._ilrObject as T;
				}

				break;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeof(T).FullName);
		}
		
		return null;
	}

	public static T GetMonoILRComponent<T>(this Transform t, string name, bool showErrorTips = true) where T : DynamicMonoILRObject {
		var child = string.IsNullOrEmpty(name) ? t : t.Find(name);

		if (child != null) {
			var comp = child.GetMonoILRComponent<T>();

			if (comp != null) {
				return comp;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {2} component by: {0}/{1}", t.GetPath() ,name, typeof(T).FullName);
		}

		return null;
	}

	public static T GetMonoILRComponentByClassPath<T>(this Transform t, string name, string hotfixClassPath, bool showErrorTips = true) where T : DynamicMonoILRObject
	{
		var child = string.IsNullOrEmpty(name) ? t : t.Find(name);

		if (child != null)
		{
			var comp = child.GetMonoILRComponentByClassPath<T>(hotfixClassPath);

			if (comp != null)
			{
				return comp;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {2} component by: {0}/{1}", t.GetPath(), name, typeof(T).FullName);
		}

		return null;
	}

	public static T GetUIControllerILRComponentInChildren<T>(this Transform t, string typeFullName, bool includeInactive = true, bool showErrorTips = true) where T : UIControllerILRObject
	{
		var results = t.GetUIControllerILRComponentsInChildren<T>(typeFullName, includeInactive, showErrorTips);

		if (results != null && results.Length > 0)
		{
			return results[0];
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeFullName);
		}

		return null;
	}

	public static T[] GetUIControllerILRComponentsInChildren<T>(this Transform t, string typeFullName, bool includeInactive = true, bool showErrorTips = true) where T : UIControllerILRObject
	{
		var ilrs = t.GetComponentsInChildren<UIControllerILR>(includeInactive);
		List<T> list = new List<T>();

		for (var i = 0; i < ilrs.Length; i++)
		{
			var ilr = ilrs[i];

			if (ilr != null && ilr.hotfixClassPath.Equals(typeFullName))
			{
				if (ilr.ilinstance == null)
				{
					ilr.ILRObjInit();
				}

				if (ilr.ilinstance != null)
				{
					list.Add(ilr.ilinstance as T);
				}
			}
		}

		if (list.Count > 0)
		{
			return list.ToArray();
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeof(T).FullName);
		}

		return null;
	}

	public static T GetUIControllerILRComponent<T>(this Transform t, bool showErrorTips = true) where T : UIControllerILRObject {
		var ilr = t.GetComponent<UIControllerILR>();
		
		if (ilr != null) {
			if (ilr.ilinstance == null) {
				ilr.ILRObjInit();
			}

			if (ilr.ilinstance != null) {
				return ilr.ilinstance as T;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeof(T).FullName);
		}

		return null;
	}

	public static T GetUIControllerILRComponentByClassPath<T>(this Transform t, string hotfixClassPath, bool showErrorTips = true) where T : UIControllerILRObject
	{
		var ilrs = t.GetComponents<UIControllerILR>();

		for (var i = 0; i < ilrs.Length; i++)
		{
			var ilr = ilrs[i];

			if (ilr != null && ilr.hotfixClassPath.Equals(hotfixClassPath))
			{
				if (ilr.ilinstance == null)
				{
					ilr.ILRObjInit();
				}

				if (ilr.ilinstance != null)
				{
					return ilr.ilinstance as T;
				}

				break;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeof(T).FullName);
		}

		return null;
	}

	public static T GetUIControllerILRComponent<T>(this Transform t, string name, bool showErrorTips = true) where T : UIControllerILRObject {
		var child = string.IsNullOrEmpty(name) ? t : t.Find(name);

		if (child != null) {
			var comp = child.GetUIControllerILRComponent<T>();

			if (comp != null) {
				return comp;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {2} component by: {0}/{1}", t.GetPath(), name, typeof(T).FullName);
		}

		return null;
	}

	public static T GetUIControllerILRComponentByClassPath<T>(this Transform t, string name, string hotfixClassPath, bool showErrorTips = true) where T : UIControllerILRObject
	{
		var child = string.IsNullOrEmpty(name) ? t : t.Find(name);

		if (child != null)
		{
			var comp = child.GetUIControllerILRComponentByClassPath<T>(hotfixClassPath);

			if (comp != null)
			{
				return comp;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {2} component by: {0}/{1}", t.GetPath(), name, typeof(T).FullName);
		}

		return null;
	}

	public static T GetDataLookupILRComponentInChildren<T>(this Transform t, string typeFullName, bool includeInactive = true, bool showErrorTips = true) where T : DataLookILRObject
	{
		var results = t.GetDataLookupILRComponentsInChildren<T>(typeFullName, includeInactive, showErrorTips);

		if (results != null && results.Length > 0)
		{
			return results[0];
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeFullName);
		}

		return null;
	}

	public static T[] GetDataLookupILRComponentsInChildren<T>(this Transform t, string typeFullName, bool includeInactive = true, bool showErrorTips = true) where T : DataLookILRObject
	{
		var ilrs = t.GetComponentsInChildren<DataLookupILR>(includeInactive);
		List<T> list = new List<T>();

		for (var i = 0; i < ilrs.Length; i++)
		{
			var ilr = ilrs[i];

			if (ilr != null && ilr.hotfixClassPath.Equals(typeFullName))
			{
				if (ilr.dlinstance == null)
				{
					ilr.ILRObjInit();
				}

				if (ilr.dlinstance != null)
				{
					list.Add(ilr.dlinstance as T);
				}
			}
		}

		if (list.Count > 0)
		{
			return list.ToArray();
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeFullName);
		}

		return null;
	}

	public static T GetDataLookupILRComponent<T>(this Transform t, bool showErrorTips = true) where T : DataLookILRObject
	{
		var ilr = t.GetComponent<DataLookupILR>();

		if (ilr != null)
		{
			if (ilr.dlinstance == null)
			{
				ilr.ILRObjInit();
			}

			if (ilr.dlinstance != null)
			{
				return ilr.dlinstance as T;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeof(T).FullName);
		}

		return null;
	}

	public static T GetDataLookupILRComponentByClassPath<T>(this Transform t, string hotfixClassPath, bool showErrorTips = true) where T : DataLookILRObject
	{
		var ilrs = t.GetComponents<DataLookupILR>();

		for (var i = 0; i < ilrs.Length; i++)
		{
			var ilr = ilrs[i];

			if (ilr != null && ilr.hotfixClassPath.Equals(hotfixClassPath))
			{
				if (ilr.dlinstance == null)
				{
					ilr.ILRObjInit();
				}

				if (ilr.dlinstance != null)
				{
					return ilr.dlinstance as T;
				}

				break;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {1} component by: {0}", t.GetPath(), typeof(T).FullName);
		}

		return null;
	}

	public static T GetDataLookupILRComponent<T>(this Transform t, string name, bool showErrorTips = true) where T : DataLookILRObject
	{
		var child = string.IsNullOrEmpty(name) ? t : t.Find(name);

		if (child != null)
		{
			var comp = child.GetDataLookupILRComponent<T>();

			if (comp != null)
			{
				return comp;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {2} component by: {0}/{1}", t.GetPath() , name, typeof(T).FullName);
		}

		return null;
	}

	public static T GetDataLookupILRComponentByClassPath<T>(this Transform t, string name, string hotfixClassPath, bool showErrorTips = true) where T : DataLookILRObject
	{
		var child = string.IsNullOrEmpty(name) ? t : t.Find(name);

		if (child != null)
		{
			var comp = child.GetDataLookupILRComponentByClassPath<T>(hotfixClassPath);

			if (comp != null)
			{
				return comp;
			}
		}

		if (showErrorTips)
		{
			EB.Debug.LogError("Can't get {2} component by: {0}/{1}", t.GetPath() , name, typeof(T).FullName);
		}

		return null;
	}

    public static string GetPathWithoutRoot(this Transform t, Transform root = null)
    {
        if (root == null)
        {
            root = t.root;
        }

        return t.GetPath(t.name, root);
    }

    private static string GetPath(this Transform t) {
		return t.GetPath(t.name, null);
	}

    private static string GetPath(this Transform t, string path, Transform root)
    {
        if (t != null && t.parent != null && t.parent != root)
        {
            path = t.parent.name + "/" + path;
            path = GetPath(t.parent, path, root);
        }

        return path;
    }
    
    public static void SetVector(this Transform t, float x, float y, float z) {
	    Vector3 curr = t.position;
	    curr.x = x;
	    curr.y = y;
	    curr.z = z;
	    t.position = curr;
    }
}
