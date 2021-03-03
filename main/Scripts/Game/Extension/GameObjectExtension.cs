using Boo.Lang;
using System.Linq;
using UnityEngine;

public static class GameObjectExtension
{
    public static void CustomSetActive(this GameObject go, bool isActive)
    {
        if (go != null && go.activeSelf != isActive)
        {
            go.SetActive(isActive);
        }
    }

    public static void WaitFrameToSetActive(this GameObject go, bool isActive, int frame)
    {
	    TimerManager.instance.AddFramer(frame, 1, delegate
	    {
		    if (go != null && go.activeSelf != isActive)
		    {
			    go.SetActive(isActive);
		    }
		});
    }

	public static void CustomSetLocalPosition(this GameObject go, Vector3 position)
    {
        if (go != null)
        {
            go.transform.localPosition = position;
        }
    }

    /// <summary>
    /// Finds a child by name and returns it's component.
    /// </summary>
    public static T GetComponent<T>(this GameObject go, string name, bool showErrorTips = true) where T : Component
    {
        return go.transform.GetComponent<T>(name, showErrorTips);
    }

    public static T GetComponentEx<T>(this GameObject go, bool showErrorTips = true) where T : Component
    {
        return go.transform.GetComponentEx<T>(showErrorTips);
    }

    /// <summary>
    /// Finds a child by name and returns it.
    /// </summary>
    public static GameObject FindEx(this GameObject go, string name, bool showErrorTips = true)
    {
        var t = go.transform.FindEx(name, showErrorTips);
        return t?.gameObject;
    }

    public static T[] FindMonoILRObjectsOfType<T>(params string[] typeFullNames) where T : DynamicMonoILRObject
    {
        var list = new List<T>();

        if (typeFullNames == null || typeFullNames.Length < 1)
        {
            return list.ToArray();
        }

        var ilrs = Object.FindObjectsOfType<DynamicMonoILR>();
        var names = typeFullNames.ToList();
    
        for (var i = 0; i < ilrs.Length; i++)
        {
            var ilr = ilrs[i];

            if (ilr != null && names.Contains(ilr.hotfixClassPath))
            {
                list.Add(ilr._ilrObject as T);
            }
        }

        return list.ToArray();
    }

    public static T AddMonoILRComponent<T>(this GameObject go, string typeFullName) where T : DynamicMonoILRObject
    {
        if (go != null)
        {
            var ilr = go.AddComponent<DynamicMonoILR>();
            ilr.hotfixClassPath = typeFullName;
            ilr.ILRObjInit();
            return ilr._ilrObject as T;
        }

        return null;
    }

    public static T GetMonoILRComponent<T>(this GameObject go, bool showErrorTips = true) where T : DynamicMonoILRObject
    {
        return go.transform.GetMonoILRComponent<T>(showErrorTips);
    }

    public static T GetMonoILRComponentByClassPath<T>(this GameObject go, string hotfixClassPath, bool showErrorTips = true) where T : DynamicMonoILRObject
    {
        return go.transform.GetMonoILRComponentByClassPath<T>(hotfixClassPath, showErrorTips);
    }

    public static T GetMonoILRComponent<T>(this GameObject go, string name, bool showErrorTips = true) where T : DynamicMonoILRObject
    {
        return go.transform.GetMonoILRComponent<T>(name, showErrorTips);
    }

    public static T GetMonoILRComponentByClassPath<T>(this GameObject go, string name, string hotfixClassPath, bool showErrorTips = true) where T : DynamicMonoILRObject
    {
        return go.transform.GetMonoILRComponentByClassPath<T>(name, hotfixClassPath, showErrorTips);
    }

    public static T AddUIControllerILRComponent<T>(this GameObject go, string typeFullName) where T : UIControllerILRObject
    {
        if (go != null)
        {
            var ilr = go.AddComponent<UIControllerILR>();
            ilr.hotfixClassPath = typeFullName;
            ilr.ILRObjInit();
            return ilr.ilinstance as T;
        }

        return null;
    }

    public static T GetUIControllerILRComponent<T>(this GameObject go, bool showErrorTips = true) where T : UIControllerILRObject
    {
        return go.transform.GetUIControllerILRComponent<T>(showErrorTips);
    }

    public static T GetUIControllerILRComponentByClassPath<T>(this GameObject go, string hotfixClassPath, bool showErrorTips = true) where T : UIControllerILRObject
    {
        return go.transform.GetUIControllerILRComponentByClassPath<T>(hotfixClassPath, showErrorTips);
    }

    public static T GetUIControllerILRComponent<T>(this GameObject go, string name, bool showErrorTips = true) where T : UIControllerILRObject
    {
        return go.transform.GetUIControllerILRComponent<T>(name, showErrorTips);
    }

    public static T GetUIControllerILRComponentByClassPath<T>(this GameObject go, string name, string hotfixClassPath, bool showErrorTips = true) where T : UIControllerILRObject
    {
        return go.transform.GetUIControllerILRComponentByClassPath<T>(name, hotfixClassPath, showErrorTips);
    }

    public static T AddDataLookupILRComponent<T>(this GameObject go, string typeFullName) where T : DataLookILRObject
    {
        if (go != null)
        {
            var ilr = go.AddComponent<DataLookupILR>();
            ilr.hotfixClassPath = typeFullName;
            ilr.ILRObjInit();
            return ilr.dlinstance as T;
        }

        return null;
    }

    public static T GetDataLookupILRComponent<T>(this GameObject go, bool showErrorTips = true) where T : DataLookILRObject
    {
        return go.transform.GetDataLookupILRComponent<T>(showErrorTips);
    }

    public static T GetDataLookupILRComponentByClassPath<T>(this GameObject go, string hotfixClassPath, bool showErrorTips = true) where T : DataLookILRObject
    {
        return go.transform.GetDataLookupILRComponentByClassPath<T>(hotfixClassPath, showErrorTips);
    }

    public static T GetDataLookupILRComponent<T>(this GameObject go, string name, bool showErrorTips = true) where T : DataLookILRObject
    {
        return go.transform.GetDataLookupILRComponent<T>(name, showErrorTips);
    }

    public static T GetDataLookupILRComponentByClassPath<T>(this GameObject go, string name, string hotfixClassPath, bool showErrorTips = true) where T : DataLookILRObject
    {
        return go.transform.GetDataLookupILRComponentByClassPath<T>(name, hotfixClassPath, showErrorTips);
    }
}
