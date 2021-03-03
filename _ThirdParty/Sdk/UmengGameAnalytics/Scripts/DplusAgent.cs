//#undef UNITY_EDITOR

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Text;



namespace Umeng
{

    public partial class Analytics
    {


#if UNITY_ANDROID

        private static AndroidJavaObject AndroidJavaJsonObject(JSONObject jSONObject)
        {
            return  new AndroidJavaObject("org.json.JSONObject", jSONObject.ToString());
        }

        private static JSONObject jsonObjectFromJava(AndroidJavaObject androidJavaJsonObject)
        {
            return (JSONObject)JSONNode.Parse(androidJavaJsonObject.Call<String>("toString"));
        }

        public static AndroidJavaObject ToJavaObject(object obj)
        {
            if (obj is int)
            {
                return new AndroidJavaObject("java.lang.Integer", obj);
            }
            else if (obj is long)
            {
                return new AndroidJavaObject("java.lang.Long", obj);
            }

            else if (obj is float)
            {
                return new AndroidJavaObject("java.lang.Float", obj);

            }
            else if (obj is double)
            {
                return new AndroidJavaObject("java.lang.Double", obj);
            }
            else if (obj is string)
            {
                return new AndroidJavaObject("java.lang.String", obj);

            }
            else if(obj is bool)
            {
               
                return new AndroidJavaObject("java.lang.Integer", Convert.ToInt32((bool)obj));

            }
            else
            {
               EB.Debug.Log("不支持加入" + obj.GetType()+"类型,此kv对被丢弃");
                return null;
            }
           



        }



        public static AndroidJavaObject ToJavaHashMap(Dictionary<String, object> dic)
        {
            var hashMap = new AndroidJavaObject("java.util.HashMap");


            foreach (var kv in dic)
            {
                var vauleObj = ToJavaObject(kv.Value);
                if (vauleObj != null)
                {

                    hashMap.Call<AndroidJavaObject>("put", kv.Key, vauleObj);
                }

            }

            return hashMap;
        }
        private static AndroidJavaObject ToJavaHashMap(Dictionary<string, string> dic)
        {
            var hashMap = new AndroidJavaObject("java.util.HashMap");
            foreach (var entry in dic)
            {
                hashMap.Call<AndroidJavaObject>("put", entry.Key, entry.Value);
            }
            return hashMap;

        }

        private static AndroidJavaObject ToJavaList(String[] list)
        {
            var javaList = new AndroidJavaObject("java.util.ArrayList");


            foreach (String str in list)
            {
                javaList.Call<bool>("add", str);
            }

            return javaList;
        }

#elif UNITY_IPHONE && USE_UMENG

        private static string ToJsonStr(Dictionary<string, object> dict)
        {

            var builder = new StringBuilder("{");
            foreach (KeyValuePair<string, object> kv in dict)
            {
                if(kv.Value is string)
                {
                    builder.AppendFormat("\"{0}\":\"{1}\",", kv.Key, kv.Value);
                }
                else if(kv.Value is float || kv.Value is double|| kv.Value is int || kv.Value is long)
                {
                    builder.AppendFormat("\"{0}\":{1},", kv.Key, kv.Value.ToString());
                }
                else if( kv.Value is bool)
                {
                    builder.AppendFormat("\"{0}\":{1},", kv.Key, Convert.ToInt32(kv.Value));
                }
                else
                {
                   EB.Debug.Log("不支持此类型");

                }



            }
            builder[builder.Length - 1] = '}';
            return builder.ToString();


        }
#endif




        public static void EventObject(String eventID,Dictionary<string, object> dict)
       {
#if UNITY_EDITOR

#elif UNITY_IPHONE && USE_UMENG
        			UmSDKEventObject(eventID, ToJsonStr(dict));
#elif UNITY_ANDROID
        			Agent.CallStatic("onEventObject", Context, eventID, ToJavaHashMap(dict));
#endif

        }




        public static void RegisterPreProperties(JSONObject jsonObject)
        {
            JSONObject filteredJsonObject = new JSONObject();

            foreach (KeyValuePair<string, JSONNode> kv in jsonObject)
            {
                if (kv.Value.IsObject || kv.Value.IsArray)
                {
                    Debug.LogError("不支持加入Object/Array类型,此kv对被丢弃");


                }
                else if ( kv.Value.IsBoolean)
                {

                    filteredJsonObject.Add(kv.Key, Convert.ToInt32(kv.Value.AsBool));


                }
                else
                {
                    filteredJsonObject.Add(kv.Key, kv.Value);
                }



            }



#if UNITY_EDITOR

#elif UNITY_IPHONE && USE_UMENG
            UmSDKRegisterSuperProperty(filteredJsonObject.ToString());
#elif UNITY_ANDROID

            Agent.CallStatic("registerPreProperties", Context, AndroidJavaJsonObject(filteredJsonObject));
#endif

        }



        public static void UnregisterPreProperty(String propertyName)
        {
#if UNITY_EDITOR

#elif UNITY_IPHONE && USE_UMENG
             UmSDKUnregisterSuperProperty(propertyName);
#elif UNITY_ANDROID
            Agent.CallStatic("unregisterPreProperty", Context, propertyName);
#endif

        }





        public static JSONObject GetPreProperties()
        {
#if UNITY_EDITOR
            return null;
#elif UNITY_IPHONE && USE_UMENG
            return (JSONObject)JSONObject.Parse(UmSDKGetSuperProperties());
#elif UNITY_ANDROID
			return jsonObjectFromJava(Agent.CallStatic<AndroidJavaObject>("getPreProperties", Context) );
#else
     return null;
#endif

        }
        public static void ClearPreProperties()
        {
#if UNITY_EDITOR

#elif UNITY_IPHONE && USE_UMENG
             UmSDKClearSuperProperties();
#elif UNITY_ANDROID
            Agent.CallStatic("clearPreProperties", Context);
#endif

        }


        public static void SetFirstLaunchEvent(string[] trackID)
        {
#if UNITY_EDITOR

#elif UNITY_IPHONE && USE_UMENG

             UmSDKSetFirstLaunchEvent(trackID, trackID.Length);
#elif UNITY_ANDROID

            Agent.CallStatic("setFirstLaunchEvent", Context, ToJavaList(trackID));
#endif


        }

#if UNITY_IPHONE && USE_UMENG
        [DllImport("__Internal")]
        public static extern void UmSDKEventObject(string eventID, string jsonStr);
        [DllImport("__Internal")]
        public static extern void UmSDKRegisterSuperProperty(string jsonStr);
        [DllImport("__Internal")]
		public static extern void UmSDKUnregisterSuperProperty(string propertyName);
        [DllImport("__Internal")]
        public static extern string UmSDKGetSuperProperties();
        [DllImport("__Internal")]
        public static extern void UmSDKClearSuperProperties();
        [DllImport("__Internal")]
        public static extern void UmSDKSetFirstLaunchEvent(string[] propertyName,int len);
#endif


    }






}