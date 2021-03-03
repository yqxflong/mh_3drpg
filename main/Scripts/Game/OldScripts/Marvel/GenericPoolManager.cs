using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GenericPoolManager<T>  where T : GenericPoolType
{
	public enum Persistence{Always, Temporary};
	
	public void Start()
	{
		_pools[0] = new Dictionary<string, GenericPool<T>>(); 	// Persistent pools
		_pools[1] = new Dictionary<string, GenericPool<T>>();	// Temp pools
	}

	public void Register(GameObject psobject, int count, Persistence ePersistence)
	{
		if (!_pools[(int)ePersistence].ContainsKey(psobject.name))
		{
			_pools[(int)ePersistence][psobject.name] = new GenericPool<T>(psobject, count);
		}
	}

	public List<GameObject> GetAllInReady(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		
		GenericPool<T> pool = Find(name);
		if (pool != null)
		{
			return pool.GetAllInReady();
		}
		
		return null;
	}

	public GameObject Use(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
			
		GenericPool<T> pool = Find(name);
		if (pool != null)
		{
			return pool.Use();
		}

		return null;
	}
	
	private GenericPool<T> Find(string name)
	{
		GenericPool<T> pool = null;
		for (int i=0; i<_pools.Length; ++i)
		{
			if (_pools[i] != null)
			{
				if (_pools[i].TryGetValue(name, out pool))
				{
					return pool;
				}
			}
		}
		return null;
	}
	
	public void Recycle(GameObject go)
	{
		GenericPool<T> pool = Find(go.name);

		if (pool != null)
		{
			pool.Recycle(go);
		}
	}

	public void PlayAt(string name, Vector3 pos)
	{
		GameObject go = Use(name);
		T trInst = go.GetComponent<T>();
		if (go && trInst != null)
		{
			go.transform.position = pos;
			trInst.Play();
		}
	}

	public void Update()
	{
		for (int i=0; i<_pools.Length; ++i)
		{
			Dictionary<string, GenericPool<T>>.Enumerator iter = _pools[i].GetEnumerator();
			while (iter.MoveNext())
			{
				iter.Current.Value.Update();
			}
			iter.Dispose();
		}
	}

	public void Retire()
	{
		for (int i=0; i<_pools.Length; ++i)
		{
			Dictionary<string, GenericPool<T>>.Enumerator iter = _pools[i].GetEnumerator();
			while (iter.MoveNext())
			{
				iter.Current.Value.Retire();
			}
			iter.Dispose();
		}
	}

	public void Clean(Persistence ePersistence)
	{
		Dictionary<string, GenericPool<T>>.Enumerator iter = _pools[(int)ePersistence].GetEnumerator();
		while (iter.MoveNext())
		{
			iter.Current.Value.Clean();
		}
		iter.Dispose();

		_pools[(int)ePersistence].Clear();
	}
	
	public void CleanAll()
	{
		Clean(Persistence.Always);
		Clean(Persistence.Temporary);
	}
	
	private Dictionary<string, GenericPool<T>>[]	_pools = new Dictionary<string, GenericPool<T>>[2];
}

public class GenericPool<U> where U:GenericPoolType
{
	public GenericPool(GameObject psObject, int count)
	{
		_ready = new List<GameObject>(count);
		_active = new List<GameObject>(count);
		
		for (int i=0; i<count; ++i)
		{
			GameObject go = (GameObject)(GameObject.Instantiate(psObject));

			go.transform.SetParent(GenericPoolSingleton.Instance.transform);

			_ready.Add(go);

			go.GetComponent<U>().Init();
		}
	}
	
	public GameObject Use()
	{
		if (_ready.Count > 0)
		{
			GameObject go = _ready[_ready.Count-1];
			_active.Add(go);
			_ready.RemoveAt(_ready.Count-1);
			
			return go;
		}
		
		return null;
	}

	public List<GameObject> GetAllInReady()
	{
		if (_ready != null && _ready.Count > 0)
		{
			return _ready;
		}
		return null;
	}
	
	public void Recycle(GameObject go)
	{
		if (_active.Contains(go))
		{
			AttachTransform.Detach(go);

			_ready.Add(go);
			_active.Remove(go);
			go.transform.SetParent(GenericPoolSingleton.Instance.transform);
		}
	}
	
	public void Update()
	{
		for (int i=_active.Count-1; i>=0; --i)
		{
			if (_active[i] == null)
				continue;
			U tri = _active[i].GetComponent<U>();
			if (tri != null && !tri.IsPlaying)
			{
				AttachTransform.Detach(_active[i]);

				_ready.Add(_active[i]);
				_active[i].transform.SetParent(GenericPoolSingleton.Instance.transform);
				_active.RemoveAt(i);
			}
		}
	}
	
	// Shut down all systems
	public void Retire()
	{
		for (int i=_active.Count-1; i>=0; --i)
		{
			if (_active[i] != null)
			{
				U tri = _active[i].GetComponent<U>();
				tri.Stop();

				AttachTransform.Detach(_active[i]);

				_active[i].transform.SetParent(GenericPoolSingleton.Instance.transform);
				_ready.Add(_active[i]);
			}
			_active.RemoveAt(i);
		}
	}

	// Free the structures
	public void Clean()
	{
		// Make sure they're all in the ready list
		Retire();

		// Destroy them all
		int count = _ready.Count;
		for (int i=0; i<count; ++i)
		{
			Object.Destroy(_ready[i]);
		}
	}
	
	private string					_name;
	
	private List<GameObject> 	_ready;
	private List<GameObject>	_active;
}
