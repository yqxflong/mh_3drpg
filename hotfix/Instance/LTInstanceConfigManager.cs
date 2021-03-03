//LTInstanceConfigManager
//副本表现数据管理里
//Johny

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.Instance
{
    public static class LTInstanceConfigManager
    {
        private const string perfConfigPath = "Bundles/Config/InstanceConfig";
        private static Dictionary<string, string> stCache = new Dictionary<string, string>();

        public static void LoadDatas()
        {
            stCache.Clear();
            EB.Assets.LoadAsync(perfConfigPath, typeof(TextAsset), o=>
            {
                if(o != null)
                {
                    var ta = o as TextAsset;
                    var jnode = Johny.JSONNode.Parse(ta.text);
                    var jobject = jnode.AsObject;
                    foreach(var item in jobject.Children)
                    {
                        stCache[item.Key.ToString()] = item.Value.ToString();
                    }
                }
                EB.Assets.Unload(perfConfigPath);
            });
        }

        public static void UnloadDatas()
        {
            stCache.Clear();
        }

        public static int GetIntValue(string key, int defaultValue = 0)
        {
            if(stCache.TryGetValue(key, out var v))
            {
                if(int.TryParse(v, out int ret))
                {
                    return ret;
                }
            }

            EB.Debug.LogError("LTInstanceConfigManager-GetIntValue===>Can not Find Key: {0}", key);
            return defaultValue;
        }

        public static float GetFloatValue(string key, float defaultValue = 0.0f)
        {
            if(stCache.TryGetValue(key, out var v))
            {
                if(float.TryParse(v, out float ret))
                {
                    return ret;
                }
            }

            EB.Debug.LogError("LTInstanceConfigManager-GetFloatValue===>Can not Find Key: {0}", key);
            return defaultValue;
        }

        public static string GetStringValue(string key, string defaultValue = "")
        {
            if(stCache.TryGetValue(key, out string ret))
            {
                return ret;
            }

            EB.Debug.LogError("LTInstanceConfigManager-GetStringValue===>Can not Find Key: {0}", key);
            return defaultValue;
        }
        
        public static bool GetBoolValue(string key, bool defaultValue = false)
        {
            if(stCache.TryGetValue(key, out var v))
            {
                if(Boolean.TryParse(v, out bool ret))
                {
                    return ret;
                }
            }

            EB.Debug.LogError("LTInstanceConfigManager-GetBoolValue===>Can not Find Key: {0}", key);
            return defaultValue;
        }
    }
}
