using UnityEngine;
using System.Collections.Generic;

namespace Unity.Standard.ScriptsWarp
{
	public class ExtendBehaviour : MonoBehaviour
	{
		// Data--------------------------------------------
		private Dictionary<string, object> valueDict;
		private Dictionary<string, UnityEngine.Object> objectDict;
		private Dictionary<string, ExtendCSScript> extendCSDict;

		public void SetObject(string key, UnityEngine.Object component)
		{
			if (objectDict == null) objectDict = new Dictionary<string, UnityEngine.Object>();
			if (objectDict.ContainsKey(key)) {
				objectDict[key] = component;
			} else {
				objectDict.Add(key, component);
			}
		}

		public void SetValue(string key, object value)
		{
			if (valueDict == null) valueDict = new Dictionary<string, object>();
			if (valueDict.ContainsKey(key)) {
				valueDict[key] = value;
			} else {
				valueDict.Add(key, value);
			}
		}

		public void SetExtendCSScript(string key, ExtendCSScript cs)
		{
			if (extendCSDict == null) extendCSDict = new Dictionary<string, ExtendCSScript>();
			if (extendCSDict.ContainsKey(key)) {
				extendCSDict[key] = cs;
			} else {
				extendCSDict.Add(key, cs);
			}
		}

		public ExtendBehaviour GetExtendBehaviour(string key)
		{
			if (objectDict == null) return null;
			if (objectDict.ContainsKey(key)) {
				return objectDict[key] as ExtendBehaviour;
			}
			return null;
		}

		public Transform GetTransform(string key)
		{
			if (objectDict == null) return null;
			if (objectDict.ContainsKey(key)) {
				return objectDict[key] as Transform;
			}
			return null;
		}

		public GameObject GetGameObject(string key)
		{
			if (objectDict == null) return null;
			if (objectDict.ContainsKey(key)) {
				return objectDict[key] as GameObject;
			}
			return null;
		}

		public UnityEngine.Object GetEngineObject(string key)
		{
			if (objectDict == null) return null;
			if (objectDict.ContainsKey(key)) {
				return objectDict[key];
			}
			return null;
		}

		public object GetObjectValue(string key)
		{
			if (valueDict == null) return null;
			if (valueDict.ContainsKey(key)) {
				return valueDict[key];
			}
			return null;
		}

		public ExtendCSScript GetExtendCSScript(string key)
		{
			if (extendCSDict == null) return null;
			if (extendCSDict.ContainsKey(key)) {
				return extendCSDict[key];
			}
			return null;
		}

		public string GetString(string key)
		{
			if (valueDict == null) return "";
			if (valueDict.ContainsKey(key)) {
				return valueDict[key] as string;
			}
			return "";
		}

		public int GetInt32(string key)
		{
			if (valueDict == null) return 0;
			if (valueDict.ContainsKey(key)) {
				return (int)valueDict[key];
			}
			return 0;
		}

		public float GetFloat(string key)
		{
			if (valueDict == null) return 0;
			if (valueDict.ContainsKey(key)) {
				return (float)valueDict[key];
			}
			return 0;
		}

		public Vector3 GetVector3(string key)
		{
			if (valueDict == null) return Vector3.zero;
			if (valueDict.ContainsKey(key)) {
				return (Vector3)valueDict[key];
			}
			return Vector3.zero;
		}

		public Vector3 GetVector2(string key)
		{
			if (valueDict == null) return Vector2.zero;
			if (valueDict.ContainsKey(key)) {
				return (Vector2)valueDict[key];
			}
			return Vector2.zero;
		}

		public bool GetBoolean(string key)
		{
			if (valueDict == null) return false;
			if (valueDict.ContainsKey(key)) {
				return (bool)valueDict[key];
			}
			return false;
		}

		public bool DeleteExtendData(string key, bool isObject = false)
		{
			if (isObject) {
				if (objectDict == null) return false;
				if (objectDict.ContainsKey(key)) {
					return objectDict.Remove(key);
				} else {
					return false;
				}
			}
			if (valueDict == null) return false;
			if (valueDict.ContainsKey(key)) {
				return valueDict.Remove(key);
			} else {
				return false;
			}
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
		    if(Exist(valueDict))valueDict.Clear ();
	    }

	    public void ClearAllContainer()
	    {
		    if(Exist(valueDict))valueDict.Clear ();
		    if(Exist(objectDict))objectDict.Clear ();
		    if(Exist(extendCSDict))extendCSDict.Clear ();
	    }

	    public void ClearComponentContainer()
	    {
		    if(Exist(objectDict))objectDict.Clear ();
	    }

	    public void ClearExtendCSContainer()
	    {
		    if(Exist(extendCSDict))extendCSDict.Clear ();
	    }

	    public T SetPosition<T>(Vector3 postion) where T : ExtendBehaviour
	    {
		    transform.localPosition = postion; return this as T;
	    }

	    // destruct action
	    public void Destruct(bool isEditor = false)
	    {
		    if(this != null)
		    {
			    if(isEditor)
				    DestroyImmediate(this);
			    else
				    Destroy(this);
		    }
	    }

	    public void Dispose(bool recycling = false)
	    {
		    if(this != null && gameObject){
			    if(Application.isPlaying)
				    Destroy(gameObject);
			    else
				    DestroyImmediate(gameObject);

			    //if(recycling)Trashcan.Recycling();
		    }
	    }

	    public void Show()
	    {
		    if (Not(gameObject.activeSelf))gameObject.SetActive(true);
		    OnShow();
	    }
	
	    //public TweenAlpha Appear(float duration = 0.5f, bool doShow = false, EventDelegate.Callback callback = null)
	    //{
	    //	if (doShow) Show();
		
	    //	TweenAlpha ta = TweenAlpha.Begin(gameObject, duration, 1);
	    //	ta.SetOnFinished(callback);
		
	    //	return ta;
	    //}

	    protected virtual void OnShow(){}

	    public ExtendBehaviour ShowWidgets<T>(T t)
	    {
		    if(Hiding)gameObject.SetActive(true);
		    return ShowWidget<T>(t);
	    }

	    protected virtual ExtendBehaviour ShowWidget<T>(T t){ return this; }

	    public ExtendBehaviour ShowByArgument(object arg)
	    {
		    if(Hiding)gameObject.SetActive(true);
		    return ShowWidget(arg);
	    }

	    protected virtual ExtendBehaviour ShowWidget(object arg){ return this; }

	    public T RequireComponent<T>(GameObject target = null) where T : Component
	    {
		    if (target == null)
			    target = gameObject;
		    T t = target.GetComponent<T>();
		    if (t == null) {
			    t = target.AddComponent<T>();
		    }
		    return t;
	    }

	    public static T RequireTComponent<T>(GameObject target) where T : Component
	    {
		    T t = target.GetComponent<T>();
		    if (t == null) {
			    t = target.AddComponent<T>();
		    }
		    return t;
	    }

	    public GameObject RequireGameObject(string name, Transform parent = null)
	    {
		    GameObject tempGameObject = GameObject.Find(name);
		    GameObject targetGameObject = tempGameObject == null ? new GameObject(name) : tempGameObject;
		    if (parent) {
			    targetGameObject.transform.SetParent(parent);
		    }
		    return targetGameObject;
	    }

	    public void Hide(bool active = false)
	    {
		    OnHide();
		    this.gameObject.SetActive(active);
	    }
	
	    //public TweenAlpha Disappear(float duration = 0.5f, bool doHide = false, EventDelegate.Callback callback = null)
	    //{
	    //	TweenAlpha ta = TweenAlpha.Begin(gameObject, duration, 0);
	    //	ta.SetOnFinished( () => { if (doHide) Hide(); callback(); } );
		
	    //	return ta;
	    //}

	    protected virtual void OnHide(){}

	    public void Enable()
	    {
		    this.enabled = true;
	    }

	    public void Disable()
	    {
		    this.enabled = false;
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

	    public bool Hiding
	    {
		    get { return !gameObject.activeSelf; }
	    }

	    public bool Showing
	    {
		    get { return gameObject.activeSelf; }
	    }
	
	    public Transform NormalizeTransform(Transform target = null, Transform parent = null)
	    {
		    if (target == null)target = transform;
		    target.localScale = Vector3.one;
		    target.localPosition = Vector3.zero;
		    target.localEulerAngles = Vector3.zero;
		    if (Exist(parent))target.SetParent(parent);
		    return target;
	    }
	
	    public Dictionary<TKey,TValue> SetDict<TKey, TValue>(Dictionary<TKey,TValue> dict, TKey key, TValue value)
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

		protected int loopCount = 0;
	
	    public bool IsCallingInEndlessLoop(int maxCount = 50)
	    {
		    loopCount += 1;
		
		    if (!IsInvoking("ResetLoopCount"))
			    Invoke("ResetLoopCount", 1);
		
		    return loopCount >= maxCount;
	    }
	
	    protected void ResetLoopCount()
	    {
		    loopCount = 0;
	    }

		public bool Completed { get; set; }
    }
}