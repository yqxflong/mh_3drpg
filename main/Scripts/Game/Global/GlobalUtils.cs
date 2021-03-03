using EB.Sparx;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Debug = EB.Debug;

public static class GlobalUtils 
{
    /// <summary>
    /// Call ILR的静态方法
    /// </summary>
    /// <param name="hotfixClassPath"></param>
    /// <param name="methodName"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static object CallStaticHotfix(string type, string method, params object[] p)
    {
        if(!Application.isPlaying){
            return null;
        }

        if (!HotfixILRManager.GetInstance().IsInit)
        {
            return null;
        }

#if ILRuntime
        return GetHotfixMthod.Instance.InvokeHotfixMethod(type, null, method, p);
#else
        var t = HotfixILRManager.GetInstance().assembly.GetType(type);
		return t.GetMethod(method).Invoke(null, p);
#endif

    }

    /// <summary>
    /// Call ILR的带instance的class的方法
    /// </summary>
    /// <param name="hotfixClassPath"></param>
    /// <param name="instance"></param>
    /// <param name="methodName"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static object CallStaticHotfixEx(string hotfixClassPath, string instance, string methodName, params object[] p)
    {
        if(!Application.isPlaying){
            return null;
        }

        if (!HotfixILRManager.GetInstance().IsInit)
        {
            return null;
        }

#if ILRuntime
        IType type = HotfixILRManager.GetInstance().appdomain.LoadedTypes[hotfixClassPath];
        PropertyInfo property = type.ReflectionType.GetProperty(instance);
        object obj = property.GetValue(null);
        if (obj == null)  
        {
            return null;
        }
        IMethod m = type.GetMethod(methodName, p.Length);
        return HotfixILRManager.GetInstance().appdomain.Invoke(m, obj, p);
#else
        return UseMonoReflectOneRes(hotfixClassPath, instance, methodName, p);
#endif
    }

    public static int GetTemplateToStringGetInt(string hotfixClassPath, string instance, string methodName, string economyid)
    {
#if ILRuntime
        IType type = HotfixILRManager.GetInstance().appdomain.LoadedTypes[hotfixClassPath];
        Type t = type.ReflectionType;
        PropertyInfo property = t.GetProperty(instance);
        object obj = property.GetValue(null);

        IMethod m = type.GetMethod(methodName, 1);
        using (var ctx = HotfixILRManager.GetInstance().appdomain.BeginInvoke(m))
        {
            ctx.PushObject(obj);
            ctx.PushObject(economyid);
            ctx.Invoke();

            return ctx.ReadInteger();
        }
#else
        return (int)UseMonoReflectOneRes(hotfixClassPath, instance, methodName, new object[] { economyid });
#endif

    }
    
    //参数是string,返回也string,可以直接用上面的变长参数
    public static string GetTemplateToStringGetString(string hotfixClassPath, string instance, string methodName, string economyid)
    {
#if ILRuntime
        IType type = HotfixILRManager.GetInstance().appdomain.LoadedTypes[hotfixClassPath];
        Type t = type.ReflectionType;
        PropertyInfo property = t.GetProperty(instance);
        object obj = property.GetValue(null);

        IMethod m = type.GetMethod(methodName, 1);
        using (var ctx = HotfixILRManager.GetInstance().appdomain.BeginInvoke(m))
        {
            ctx.PushObject(obj);
            ctx.PushObject(economyid);
            ctx.Invoke();

            return ctx.ReadObject<string>();
        }
#else
        return (string)UseMonoReflectOneRes(hotfixClassPath, instance, methodName, new object[] { economyid });
#endif
    }

    public static int GetHotfixToIntGetInt(string hotfixClassPath, string instance, string methodName, int param)
    {
#if ILRuntime
        IType type = HotfixILRManager.GetInstance().appdomain.LoadedTypes[hotfixClassPath];
        Type t = type.ReflectionType;
        PropertyInfo property = t.GetProperty(instance);
        object obj = property.GetValue(null);

        IMethod m = type.GetMethod(methodName, 1);
        using (var ctx = HotfixILRManager.GetInstance().appdomain.BeginInvoke(m))
        {
            ctx.PushObject(obj);
            ctx.PushObject(param);
            ctx.Invoke();

            return ctx.ReadInteger();
        }
#else
        return (int)UseMonoReflectOneRes(hotfixClassPath, instance, methodName, new object[] { param });
#endif
    }

    public static bool GetHotfixToIntGetBool(string hotfixClassPath, string instance, string methodName, int param)
    {
#if ILRuntime
        IType type = HotfixILRManager.GetInstance().appdomain.LoadedTypes[hotfixClassPath];
        Type t = type.ReflectionType;
        PropertyInfo property = t.GetProperty(instance);
        object obj = property.GetValue(null);

        IMethod m = type.GetMethod(methodName, 1);
        using (var ctx = HotfixILRManager.GetInstance().appdomain.BeginInvoke(m))
        {
            ctx.PushObject(obj);
            ctx.PushObject(param);
            ctx.Invoke();

            return ctx.ReadBool();
        }
#else
        return (bool)UseMonoReflectOneRes(hotfixClassPath, instance, methodName, new object[] { param });
#endif
    }

    //mono调用反射返回1个值
    private static object UseMonoReflectOneRes(string hotfixClassPath, string instance, string methodName, object[] ilParams)
    {
#if !ILRuntime 
        Type type = HotfixILRManager.GetInstance().assembly.GetType(hotfixClassPath);
        PropertyInfo property = type.GetProperty(instance);
        object obj = property.GetValue(null);

        if (obj != null)
        {
            return type.GetMethod(methodName).Invoke(obj, ilParams);
        }
        else
        {
            return null;
        }
#else
        return null;
#endif
    }
    
//    //mono调用反射返回多个值，参数object[] ilParams = new object[] { a, b, null, null};
//    private static void UseMonoReflectMoreRes(string hotfixClassPath, string instance, string methodName, ref object[] ilParams)
//    {
//#if !ILRuntime 
//        Type type = HotfixILRManager.GetInstance().assembly.GetType(hotfixClassPath);
//        PropertyInfo property = type.GetProperty(instance);
//        object obj = property.GetValue(null);

//        type.GetMethod(methodName).Invoke(obj, ilParams);
//#endif
//    }

    public static string GetTemplateToStringIntGetString(string hotfixClassPath, string instance, string methodName, string characterid, int skin)
    {
#if ILRuntime
        IType type = HotfixILRManager.GetInstance().appdomain.LoadedTypes[hotfixClassPath];
        Type t = type.ReflectionType;
        PropertyInfo property = t.GetProperty(instance);
        object obj = property.GetValue(null);

        IMethod m = type.GetMethod(methodName, 2);
        using (var ctx = HotfixILRManager.GetInstance().appdomain.BeginInvoke(m))
        {
            ctx.PushObject(obj);
            ctx.PushObject(characterid);
            ctx.PushInteger(skin);
            ctx.Invoke();

            return ctx.ReadObject<string>();
        }
#else
        return (string)UseMonoReflectOneRes(hotfixClassPath, instance, methodName, new object[] { characterid, skin });
#endif
    }

    public static float GetTemplateToStringGetFloat(string hotfixClassPath, string instance, string methodName, string characterid)
    {
#if ILRuntime
        IType type = HotfixILRManager.GetInstance().appdomain.LoadedTypes[hotfixClassPath];
        Type t = type.ReflectionType;
        PropertyInfo property = t.GetProperty(instance);
        object obj = property.GetValue(null);

        IMethod m = type.GetMethod(methodName, 1);
        using (var ctx = HotfixILRManager.GetInstance().appdomain.BeginInvoke(m))
        {
            ctx.PushObject(obj);
            ctx.PushObject(characterid);
            ctx.Invoke();

            return ctx.ReadFloat();
        }
#else
        return (float)UseMonoReflectOneRes(hotfixClassPath, instance, methodName, new object[] { characterid });
#endif
    }

    #region HotfixCalledStaticMethod
    public static void EditorDebugLogErrorStr(string param)
    {
#if UNITY_EDITOR
        EB.Debug.LogError(param);
#endif
    }

    public static void FBSendRecordEvent(string ID)
    {
#if USE_FB
            if(Hub.Instance.FacebookManager!=null) Hub.Instance.FacebookManager.F_SendRecordEvent(ID);
#endif
    }

    #endregion


    #region LTGameSettingController
    public static int GetQualityLevel(string quality)
    {
        switch (quality)
        {
            case "High":
                return 0;
            case "Medium":
                return 1;
            case "Low":
                return 2;
            default:
                return 0;
        }
    }

    public static void SetGameQualityLevel(string level)
    {
        int levelInt = GetQualityLevel(level);
        EB.Sparx.PerformanceManager.PerformanceUserSetting = level;
        UserData.UserQualitySet = level;
        UserData.SerializePrefs();
        EB.Sparx.Hub.Instance.PerformanceManager.ResetPerformanceData(
            delegate
            {
                UserData.SetUserQuality(levelInt);
                var perfInfo = PerformanceManager.Instance.CurrentEnvironmentInfo;
                QualitySettings.SetQualityLevel(levelInt, true);
                Shader.globalMaximumLOD = perfInfo.lod;// default: 0
                QualitySettings.blendWeights = BlendWeights.FourBones;//by:wwh 强制调整为4融合权重
                QualitySettings.antiAliasing = (int)perfInfo.msaa;// default: disabled
                QualitySettings.anisotropicFiltering = (AnisotropicFiltering)perfInfo.aniso;// default: disabled
            });
    }

    public static void SetGameQualityLevel(int level)
    {
        SetGameQualityLevel(level.ToString());
    }
    #endregion


    #region LTUIUtil
    public static void SetText(UILabel label, string str)
    {
        if (label == null) { return; }
        UILabel[] labelArray = label.transform.GetComponentsInChildren<UILabel>();
        for (int i = 0; i < labelArray.Length; i++)
        {
            labelArray[i].text = str;
        }
    }

    public static void SetNumTemplate<T>(T template, List<T> list, int num, int behind, bool isVertical = true) where T : Component
    {
        if (num > list.Count)
        {
            for (int i = list.Count; i < num; i++)
            {
                GameObject go = GameObject.Instantiate(template.gameObject);
                go.transform.parent = template.transform.parent;
                go.transform.localScale = template.transform.localScale;
                go.transform.localPosition = template.transform.localPosition + new Vector3(isVertical ? 0 : behind * i, isVertical ? -behind * i : 0f, 0f);
                list.Add(go.GetComponent<T>());
            }
        }
        for (int i = 0; i < num; i++)
        {
            list[i].gameObject.SetActive(true);
        }
        for (int i = num; i < list.Count; i++)
        {
            list[i].gameObject.SetActive(false);
        }
    }
    #endregion


    #region QuestObjectiveMonitor
    static public GameObject FindCharacter(CharacterModel characterRequired)
    {
        if (null == characterRequired)
        {
            return null;
        }

        PlayerController localPlayer = PlayerManager.LocalPlayerController();
        if (null != localPlayer)
        {
            if (characterRequired.isPlayer) // if any player character is specified
            {
                return localPlayer.gameObject;
            }
            //else if (IsPixieCharacterModel(characterRequired))
            //{
            //    return localPlayer.Pixie;
            //}
            else
            {
                return null;
            }
        }

        CharacterComponent charComp = CharacterComponentManager.sCharacterComponents.Find(comp => comp.Model == characterRequired);
        if (null != charComp)
        {
            return charComp.gameObject;
        }
        return null;
    }

    // is the passed character model the pixie?
    //static private bool IsPixieCharacterModel(CharacterModel model)
    //{
    //    PlayerController localPlayer = PlayerManager.LocalPlayerController();
    //    if (null != localPlayer && null != localPlayer.Pixie)
    //    {
    //        CharacterComponentPixie pixieCharacter = localPlayer.Pixie.GetComponent<CharacterComponentPixie>();
    //        if (null != pixieCharacter && model == pixieCharacter.model)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    #endregion


    #region QuestCatalogEditor
#if UNITY_EDITOR
    public static void DestroyModel(UnityEngine.Object model)
    {
        if (model == null)
            return;

        if (UnityEditor.AssetDatabase.GetAssetPath(model) != "")
        {
            UnityEngine.Object.DestroyImmediate(model, true);
        }
    }

    public static void SaveModelOnAsset(UnityEngine.Object model, UnityEngine.Object asset)
    {
        if (model == null)
        {
            return;
        }

        if (UnityEditor.AssetDatabase.GetAssetPath(model) == "")
        {
            UnityEditor.AssetDatabase.AddObjectToAsset(model, asset);
            UnityEditor.EditorUtility.SetDirty(model);
        }
        UnityEditor.AssetDatabase.SaveAssets();
    }

    public static Texture2D bgTex = null;

    public static GUIStyle CreateDefaultBoxStyle()
    {
        if (bgTex == null)
        {
            bgTex = (Texture2D)Resources.Load("editor_box");
        }

        GUIStyle boxStyle = new GUIStyle();
        boxStyle.normal.background = bgTex;
        boxStyle.margin = new RectOffset(5, 5, 5, 5);
        boxStyle.border = new RectOffset(3, 3, 3, 3);
        boxStyle.padding = new RectOffset(5, 5, 5, 5);
        return boxStyle;
    }

    // fill out the list of characters
    public static CharacterModel CharacterSelect(CharacterModel currentSelection, string label)
    {
        string[] allCharacterNamesFixed;
        CharacterModel[] allCharacterModelsFixed;
        int[] allCharacterIndecisFixed;
        int selectedIndex = -1;

        CalculateCharacterListAndSelection(currentSelection, out selectedIndex, out allCharacterNamesFixed, out allCharacterModelsFixed, out allCharacterIndecisFixed);

        selectedIndex = UnityEditor.EditorGUILayout.IntPopup(label, selectedIndex, allCharacterNamesFixed, allCharacterIndecisFixed);
        return allCharacterModelsFixed[selectedIndex];
    }

    // fill out the list of characters
    public static void CalculateCharacterListAndSelection(CharacterModel currentSelection, out int selectedIndex, out string[] allCharacterNamesFixed, out CharacterModel[] allCharacterModelsFixed, out int[] allCharacterIndecisFixed)
    {
        List<CharacterModel> allCharacters = new List<CharacterModel>();
        List<string> allCharacterNames = new List<string>();
        List<int> allCharacterIndecis = new List<int>();

        foreach (CharacterModel characterModel in CharacterCatalog.Instance.GetAllModels())
        {
            allCharacters.Add(characterModel);
            allCharacterNames.Add(characterModel.name);
            allCharacterIndecis.Add(allCharacterIndecis.Count);
        }
        allCharacters.Add(null);
        allCharacterNames.Add("<Null Character>");
        allCharacterIndecis.Add(allCharacterIndecis.Count);

        allCharacterModelsFixed = allCharacters.ToArray();
        allCharacterNamesFixed = allCharacterNames.ToArray();
        allCharacterIndecisFixed = allCharacterIndecis.ToArray();

        selectedIndex = -1;
        for (int character = 0; character < allCharacters.Count; ++character)
        {
            if (currentSelection == allCharacters[character])
            {
                selectedIndex = character;
                break;
            }
        }
        if (-1 == selectedIndex)
        {
            selectedIndex = allCharacters.Count - 1; // pointing at the null character
        }
    }
#endif
    #endregion


    public static void AndroidCall(string Class,string Static, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass androidClass = new AndroidJavaClass(Class);
        androidClass.CallStatic(Static, args);
#endif
    }

    public static void AndroidCallWithUnityPalyer(string Class, string Static, params object[] args)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaClass util = new AndroidJavaClass(Class);
        if (unityPlayerClass != null)
        {
            AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            util.CallStatic(Static, currentActivity, args);
        }
#endif
    }


    public static bool Comparer<T>(T t)
    {
        // Debug.LogError("typeof:"+typeof(T));
        return EqualityComparer<T>.Default.Equals(t, default(T));
    }
}


#region DungeonModel
public enum eDungeonDifficulty
{
    Easy,
    Normal,
    Hard,
    VeryHard
}
#endregion


#region DungeonGroupModel
public enum eGameAct
{
    Act0,
    Act1,
    Act2,
    Act3,
    Act4,
    Act5
}
#endregion

