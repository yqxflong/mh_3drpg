using UnityEngine;
using System.Collections.Generic;

public class TrailRendererManager : MonoBehaviour 
{
	public enum eTRAIL_LENGTH
	{
		Short = 0,
		Medium =1,
		Long = 2
	};

	static TrailRendererManager _this;
	static bool                 _created;
	public static TrailRendererManager Instance
	{ 
		get 
		{ 
			if (_this == null && !_created)
			{
				//EB.Debug.Log("New Trail Renderer Manager!");
				GameObject go = new GameObject("TrailRendererManager");
				if (Application.isPlaying)
				{
					DontDestroyOnLoad(go);
				}
				_this = go.AddComponent<TrailRendererManager>();
				_created = true;
			}
			
			return _this; 
		} 
	} 

	private int[] _VertSizeMap = {500, 750, 1000};
	private int[] _Capacity = { 1, 2, 1 };
	private Dictionary<eTRAIL_LENGTH, List<TrailRenderer>> _TrailPool;

	private Dictionary<TrailRenderer, eTRAIL_LENGTH> _ActiveTrails;
	GameObject parent;
	public void FakeInit()
	{
		//use this to init this class while loading combat scence,don't need any thing here,keep it empty.
	}

	void Awake()
	{
		parent = new GameObject();
		parent.name = "TrailManagerPool";
		DontDestroyOnLoad(parent);
		_TrailPool = new Dictionary<eTRAIL_LENGTH, List<TrailRenderer>>();
		foreach(eTRAIL_LENGTH length in System.Enum.GetValues(typeof(eTRAIL_LENGTH)))
		{
			int capacity = _Capacity[(int)length];
			int vertSize = _VertSizeMap[(int)length];
			_TrailPool[length] = new List<TrailRenderer>(capacity);
			for (int i = 0; i < capacity; ++i)
			{
				_TrailPool[length].Add(new TrailRenderer(vertSize,parent));
			}
		}
		_ActiveTrails = new Dictionary<TrailRenderer, eTRAIL_LENGTH>();
	}

	void OnDestroy()
	{
		if (!Application.isPlaying)
		{
			_created = false;
		}
	}

	public TrailRenderer GetTrailRenderer(eTRAIL_LENGTH length)
	{
		TrailRenderer trail = null;
		int vertSize = _VertSizeMap[(int)length];
		if(_TrailPool != null && _TrailPool[length].Count > 0) 
		{
			trail = _TrailPool[length][0];
			_TrailPool[length].RemoveAt(0);
			//EB.Debug.Log("Trail "+ length.ToString() +" Requested - Give from pool Capacity: " + _TrailPool[length].Count);
		}
		else 
		{

			trail = new TrailRenderer(vertSize,parent);
			//EB.Debug.Log("Trail "+ length.ToString() +" Requested - New One Created");
		}
		_ActiveTrails.Add(trail,length);
		//trail.SetupTrail();
		return trail;
	}
	
	public void ReturnTrailRenderer(TrailRenderer renderer)
	{
		if(renderer != null && _ActiveTrails != null && _ActiveTrails.ContainsKey(renderer)) 
		{
			eTRAIL_LENGTH length = _ActiveTrails[renderer];
			if(_TrailPool[length].Count < _Capacity[(int)length])
			{
				_TrailPool[length].Add(renderer);
				//_ActiveTrails.Remove(renderer);
				//EB.Debug.Log("Trail "+ length.ToString() +" Returned to Pool");
			}
			else
			{
				renderer.DestroyTrail();

				//EB.Debug.Log("Trail "+ length.ToString() +" Returned. Pool Maxed, Destroy");
			}
			_ActiveTrails.Remove(renderer);
		}
	}

}
