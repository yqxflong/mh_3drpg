using UnityEngine;

namespace Game.Tool
{
    public class Singleton<T> where T : class, new()
	{
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
				}
                return _instance;
            }
        }

        public static void SetInstance(T t)
        {
            _instance = t;
        }
    }
	
	public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Create();
                }
                return _instance;
            }
        }

        public static T GetInstance(bool forcible = false)
        {
            if (_instance == null)
            {
                Create(forcible);
            }
            return _instance;
        }

        public static T GetInstance(GameObject attachment)
        {
            if (_instance != null) return _instance;
            if (attachment)
            {
                _instance = attachment.GetComponent<T>();
                return _instance;
            }
            return null;
        }

        public static void SetInstance(T t)
        {
            _instance = t;
        }

        void Awake()
        {
            _instance = this as T;
        }

        public static T Create(bool forced2Create = true)
        {
            //get it
            T[] instances = GameObject.FindObjectsOfType<T>();
            if (instances.Length == 0)
            {
                if (forced2Create)
                {
                    //there is none, the create the instance
                    GameObject go = new GameObject(typeof(T).Name, typeof(T));
                    _instance = go.GetComponent<T>();
                    GameObject main = GameObject.Find("Client");
                    if (main) _instance.transform.parent = main.transform;
                }
            }
            else if (instances.Length == 1)
            {
                //there is one, then get the instance
                _instance = instances[0];
            }
            else
            {
                // //Main.LogError("You have more than one " + typeof(T).Name + " in the scene. You only need 1, it's a singleton!");
                //more than one, use the first one, destory the left
                _instance = instances[0];
                for (int i = 1; i < instances.Length; ++i)
                {
                    Destroy(instances[i].gameObject);
                }
            }
            return _instance;
        }

        public void Clear()
        {
            if (!_instance) return;
			//destroy instance
#if UNITY_EDITOR && !UNITY_ANDROID && !UNITY_IOS
			DestroyImmediate(_instance.gameObject);
#else
			Destroy(_instance.gameObject);
#endif
			_instance = null;
        }
    }
}