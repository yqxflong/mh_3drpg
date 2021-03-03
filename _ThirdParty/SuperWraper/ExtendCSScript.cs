using UnityEngine;
using System.Collections.Generic;

namespace Unity.Standard.ScriptsWarp
{
    public class ExtendCSScript
    {
        // Data--------------------------------------------
        private Dictionary<string, object> valueDict;
        private Dictionary<string, UnityEngine.Object> objectDict;
        private Dictionary<string, ExtendCSScript> extendCSDict;

        public void SetObject(string key, UnityEngine.Object component)
        {
            if (objectDict == null) objectDict = new Dictionary<string, UnityEngine.Object>();
            if (objectDict.ContainsKey(key))
            {
                objectDict[key] = component;
            }
            else
            {
                objectDict.Add(key, component);
            }
        }

        public void SetValue(string key, object value)
        {
            if (valueDict == null) valueDict = new Dictionary<string, object>();
            if (valueDict.ContainsKey(key))
            {
                valueDict[key] = value;
            }
            else
            {
                valueDict.Add(key, value);
            }
        }

        public void SetExtendCSScript(string key, ExtendCSScript cs)
        {
            if (extendCSDict == null) extendCSDict = new Dictionary<string, ExtendCSScript>();
            if (extendCSDict.ContainsKey(key))
            {
                extendCSDict[key] = cs;
            }
            else
            {
                extendCSDict.Add(key, cs);
            }
        }

        public ExtendBehaviour GetExtendBehaviour(string key)
        {
            if (objectDict == null) return null;
            if (objectDict.ContainsKey(key))
            {
                return objectDict[key] as ExtendBehaviour;
            }
            return null;
        }

        public Transform GetTransform(string key)
        {
            if (objectDict == null) return null;
            if (objectDict.ContainsKey(key))
            {
                return objectDict[key] as Transform;
            }
            return null;
        }

        public GameObject GetGameObject(string key)
        {
            if (objectDict == null) return null;
            if (objectDict.ContainsKey(key))
            {
                return objectDict[key] as GameObject;
            }
            return null;
        }

        public UnityEngine.Object GetObject(string key)
        {
            if (objectDict == null) return null;
            if (objectDict.ContainsKey(key))
            {
                return objectDict[key];
            }
            return null;
        }

        public object GetValue(string key)
        {
            if (valueDict == null) return null;
            if (valueDict.ContainsKey(key))
            {
                return valueDict[key];
            }
            return null;
        }

        public ExtendCSScript GetExtendCSScript(string key)
        {
            if (extendCSDict == null) return null;
            if (extendCSDict.ContainsKey(key))
            {
                return extendCSDict[key];
            }
            return null;
        }

        public string GetString(string key)
        {
            if (valueDict == null) return "";
            if (valueDict.ContainsKey(key))
            {
                return valueDict[key] as string;
            }
            return "";
        }

        public int GetInt32(string key)
        {
            if (valueDict == null) return 0;
            if (valueDict.ContainsKey(key))
            {
                return (int)valueDict[key];
            }
            return 0;
        }

        public float GetFloat(string key)
        {
            if (valueDict == null) return 0;
            if (valueDict.ContainsKey(key))
            {
                return (float)valueDict[key];
            }
            return 0;
        }

        public Vector3 GetVector3(string key)
        {
            if (valueDict == null) return Vector3.zero;
            if (valueDict.ContainsKey(key))
            {
                return (Vector3)valueDict[key];
            }
            return Vector3.zero;
        }

        public Vector3 GetVector2(string key)
        {
            if (valueDict == null) return Vector2.zero;
            if (valueDict.ContainsKey(key))
            {
                return (Vector2)valueDict[key];
            }
            return Vector2.zero;
        }

        public bool GetBoolean(string key)
        {
            if (valueDict == null) return false;
            if (valueDict.ContainsKey(key))
            {
                return (bool)valueDict[key];
            }
            return false;
        }

        public bool DeleteExtendData(string key, bool isObject = false)
        {
            if (isObject)
            {
                if (objectDict == null) return false;
                if (objectDict.ContainsKey(key))
                {
                    return objectDict.Remove(key);
                }
                else
                {
                    return false;
                }
            }
            if (valueDict == null) return false;
            if (valueDict.ContainsKey(key))
            {
                return valueDict.Remove(key);
            }
            else
            {
                return false;
            }
        }

        public GameObject RequireGameObject(string name, Transform parent = null)
        {
            GameObject tempGameObject = GameObject.Find(name);
            GameObject targetGameObject = tempGameObject == null ? new GameObject(name) : tempGameObject;
            if (parent)
            {
                targetGameObject.transform.SetParent(parent);
            }
            return targetGameObject;
        }

        public bool Exist(MonoBehaviour target)
        {
            return target != null;
        }

        public bool Exist(System.Object target)
        {
            return target != null;
        }

        public bool Not(bool verdict)
        {
            return verdict == false;
        }

        // dataTag--------------------------------------------
        private string dataTag;
        public string GetTag()
        {
            return dataTag;
        }

        public void SetTag(string tag)
        {
            dataTag = tag;
        }

        public void ClearValueContainer()
        {
            valueDict.Clear();
        }

        public void ClearAllContainer()
        {
            valueDict.Clear();
            objectDict.Clear();
            extendCSDict.Clear();
        }

        public void ClearComponentContainer()
        {
            objectDict.Clear();
        }

        public void ClearExtendCSContainer()
        {
            extendCSDict.Clear();
        }

        public Dictionary<TKey, TValue> SetDict<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (Exist(dict))
            {
                if (dict.ContainsKey(key))
                {
                    dict[key] = value;
                    if (value == null)
                    {
                        dict.Remove(key);
                    }
                }
                else
                {
                    if (key != null && value != null)
                        dict.Add(key, value);
                }
                return dict;
            }
            return null;
        }

		private static int loopCount = 0;

		public static bool IsCallingInEndlessRecursion(int maxCount = 50)
		{
			loopCount += 1;			

			return loopCount >= maxCount;
		}

		public static void ResetRecursionCount()
		{
			loopCount = 0;
		}
	}
}
