using UnityEngine;
using System.Collections.Generic;

namespace Unity.Standard.ScriptsWarp
{
    public class Delegater : ExtendBehaviour
    {
		[HideInInspector] public string DelegaterName;

		public delegate void TheDelegater(Delegater delegater);

        public System.Action<bool> actionBool { get; set; }
        public System.Action<int> actionInt32 { get; set; }

        protected List<string> defaultKeys = new List<string>() { "default" };
        public List<string> DefaultKeys { get { return defaultKeys; } }
        public List<string> AddDefaultKey(string key) { if (defaultKeys.Contains(key)) defaultKeys.Add(key); return defaultKeys; }

        protected Dictionary<string, TheDelegater> delegaterDict = new Dictionary<string, TheDelegater>();
        private List<KeyValuePair<string, TheDelegater>> methods = new List<KeyValuePair<string, TheDelegater>>();

        public TheDelegater GetDelegater(int index = 0)
        {
            if (delegaterDict.ContainsKey(defaultKeys[index]) == false) return null;
            return delegaterDict[defaultKeys[index]];
        }

        public TheDelegater GetDelegater(string key)
        {
            if (delegaterDict.ContainsKey(key) == false) return null;
            return delegaterDict[key];
        }

        public Dictionary<string, TheDelegater> GetDelegaterDict() { return delegaterDict; }

        public TheDelegater SetDelegater(TheDelegater method, int index = 0)
        {
            if (delegaterDict.ContainsKey(defaultKeys[index]))
            {
                delegaterDict[defaultKeys[index]] = method;
            }
            else
            {
                delegaterDict.Add(defaultKeys[index], method);
            }
            return delegaterDict[defaultKeys[index]];
        }

        public TheDelegater SetDelegater(string key, TheDelegater method)
        {
            if (delegaterDict.ContainsKey(key))
            {
                delegaterDict[key] = method;
            }
            else
            {
                delegaterDict.Add(key, method);
            }
            return delegaterDict[key];
        }

        public bool OnDelegater(int index = 0)
        {
            if (delegaterDict.ContainsKey(defaultKeys[index]) && delegaterDict[defaultKeys[index]] != null)
            {
                delegaterDict[defaultKeys[index]](this);
                return true;
            }
            else
            {
                //			Debug.Log(Console.WarnFormat(gameObject.name+": The Delegater is not exist!"));
                return false;
            }
        }

        public bool OnDelegater(string key)
        {
            if (delegaterDict.ContainsKey(key) && delegaterDict[key] != null)
            {
                delegaterDict[key](this);
            }
            else
            {
                Debug.LogWarning(gameObject.name + ": Invoke a Empty Delegater, there are something wrong?");
                return false;
            }
            return true;
        }

        private int tmpIndex = 0;
        public void OnDelegaters(List<string> exceptKeys, int count = 0, int index = 0)
        {
            if (methods.Count != delegaterDict.Count)
            {
                methods.Clear();
                foreach (KeyValuePair<string, TheDelegater> kvp in delegaterDict)
                    methods.Add(kvp);
            }
            tmpIndex = 0;

            methods.ForEach
            (
                method => {
                    if (!exceptKeys.Contains(method.Key))
                    {
                        if (count > 0 && index < count)
                        {
                            method.Value(this);
                            index++;
                        }
                        else if ((count <= 0 && index > 0) ||
                            (count > 0 && index > 0 && index < count))
                        {
                            if (tmpIndex >= index)
                            {
                                method.Value(this);
                                index++;
                            }
                            else
                                tmpIndex++;
                        }
                        else
                            method.Value(this);
                    }
                }
            );
        }

        public void EmptyDelegater(int index = 0)
        {
            Debug.LogWarning(gameObject.name + ": Emptyed Delegater!");
            if (delegaterDict.ContainsKey(defaultKeys[index]) && delegaterDict[defaultKeys[index]] != null)
            {
                delegaterDict[defaultKeys[index]] = null;
                delegaterDict.Remove(defaultKeys[index]);
            }
        }

        public void EmptyDelegater(string key)
        {
            if (delegaterDict.ContainsKey(key) && delegaterDict[key] != null)
            {
                delegaterDict[key] = null;
                delegaterDict.Remove(key);
            }
        }

        public void ClearDelegater()
        {
            delegaterDict.Clear();
        }

        public void Pause()
        {
            enabled = false;
        }

        public void Resume()
        {
            enabled = true;
        }
    }

    public class Delegator : ExtendCSScript
    {
        public delegate void TheDelegator(Delegator delegator);
        protected TheDelegator delegator;
        public TheDelegator GetDelegater() { return delegator; }

        public void SetDelegator(TheDelegator func)
        {
            delegator = func;
        }

        public void OnDelegator(bool emptying = false)
        {
            if (delegator != null)
            {
                delegator(this);

                if (emptying) EmptyDelegater();
            }
        }

        public void EmptyDelegater()
        {
            delegator = null;
        }
    }
}