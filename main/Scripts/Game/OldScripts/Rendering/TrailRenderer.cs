using UnityEngine;

public class TrailRenderer  
{
	public enum eTRAIL_TYPE 
	{
		Uniform,
		Drag,
		Catchup
	}

	public enum eTIME_UNITS
	{
		Seconds,
		Frames,
	}

	private int _MaxVerts;
	private int _MaxIndices;
	public float _DistanceThreshold = 2f;
	private float _SegmentLength = 3f;

	public eTRAIL_TYPE _TrailType;
	public float _TrailTime = 4.0f;
	public float _CurveTension = 0.75f;
	public float _FadeStartTime = 1f;
	public float _FadeDuration = 0.5f;
	public float _TextureRepeat = 1;
	public float _TextureMetersSecond = 0;
	public AnimationCurve _WidthCurve;
	public bool _IgnoreZ = false;
	public bool _SpanOverTrail = false;
	public Gradient _ColorGradient;
	public bool _AddColor = false;
	public Gradient _LifeGradient;
	public Material _Material;
	public GameObject _Point1;
	public GameObject _Point2;
	public Vector3 _Offset1;
	public Vector3 _Offset2;
	public bool _UseLocalOffsets = false;
	public int _TextureYSplit = 0;
	public float _MidpointAdjust = 0.77f;

	public GameObject _GameObject;
	private Mesh _Mesh;
	private Renderer _Renderer;
	private Vector4[] _HermiteBasis;
	private float avgZ = 0;
	private bool _isAlive = false;
	private int _TotalSubsections;
	private float _TimeToStartFade;
	private Vector3 _LastAddedTopPoint = Vector3.zero;
	private Vector3 _LastAddedBottomPoint = Vector3.zero;
	private int _TextureYCount = 0;
	private int _LastSegmentOffset = 0;
	private float _TextureMoveTimer = 0;
	private float _PrevTick = 0.0F;
	private float _InitialTrailTime = 0;
	private float timeAtStart = 0;
	private Vector3[] _vertices;
	private Vector3[] _uvs;
	private Color[] _colors;
	private int[] _indices;
	private EB.CircularBuffer<Vector3> _SegmentPositionTopBuffer;
	private EB.CircularBuffer<Vector3> _SegmentPositionBottomBuffer;
	private EB.CircularBuffer<int> _SegmentSubsectionsBuffer;
	private EB.CircularBuffer<float> _SegmentTimesBuffer;
	private MeshFilter meshFilter;
	
	private PerformanceInfo.eTRAIL_QUALITY _Quality = PerformanceInfo.eTRAIL_QUALITY.High;

	public TrailRenderer(int max_vert, GameObject parent)
	{
        if( PerformanceManager.Instance.CurrentEnvironmentInfo!=null) //by pj 重新修正 当当前有配置的时候使用配置 否则就不改变
        {
            _Quality = PerformanceManager.Instance.CurrentEnvironmentInfo.trailQuality;
        }

		_GameObject = new GameObject("TrailRenderer", typeof(MeshFilter), typeof(MeshRenderer));
		_GameObject.transform.parent = parent.transform;
		meshFilter = _GameObject.GetComponent<MeshFilter>();
		_Renderer = meshFilter.GetComponent<Renderer>();
		if(!Application.isPlaying)
		{
			meshFilter.sharedMesh = new Mesh();
			_Mesh = meshFilter.sharedMesh;
		}
		else
		{
			meshFilter.mesh = new Mesh();
			_Mesh = meshFilter.mesh;
		}
		_TotalSubsections = 0;
		_TextureYCount = _TextureYSplit;
		_HermiteBasis = new Vector4[4];
		_HermiteBasis[0] = new Vector4(2, -3, 0, 1);
		_HermiteBasis[1] = new Vector4(-2, 3, 0, 0);
		_HermiteBasis[2] = new Vector4(1, -2, 1, 0);
		_HermiteBasis[3] = new Vector4(1, -1, 0, 0);

		int max_indices = max_vert * 3 - 6;
		_MaxVerts = max_vert;
		_MaxIndices = max_indices;
		_SegmentPositionTopBuffer = new EB.CircularBuffer<Vector3>(max_vert);
		_SegmentPositionBottomBuffer = new EB.CircularBuffer<Vector3>(max_vert);
		_SegmentSubsectionsBuffer = new EB.CircularBuffer<int>(max_vert);
		_SegmentTimesBuffer = new EB.CircularBuffer<float>(max_vert);

		//move vert,etc. array creation here
		_vertices = new Vector3[max_vert];
		_uvs = new Vector3[max_vert];
		_colors = new Color[max_vert];
		_indices = new int[max_indices];

		for(int i = 0; i < max_indices; ++i)
		{
			_indices[i] = 0;
		}
	}

	public void Reset()
	{
		_isAlive = false;
		_TotalSubsections = 0;
		_LastAddedBottomPoint = Vector3.zero;
		_LastAddedTopPoint = Vector3.zero;
		_LastSegmentOffset = 0;
		_TextureMoveTimer = 0;
		avgZ = 0;
		_TrailTime = _InitialTrailTime;
		_SegmentPositionTopBuffer.Reset();
		_SegmentPositionBottomBuffer.Reset();
		_SegmentSubsectionsBuffer.Reset();
		_SegmentTimesBuffer.Reset();

		_vertices = new Vector3[_MaxVerts];
		_uvs = new Vector3[_MaxVerts];
		_colors = new Color[_MaxVerts];
		_indices = new int[_MaxIndices];

		_Mesh.vertices = _vertices;
		_Mesh.normals = _uvs;		
		_Mesh.colors = _colors;
		_Mesh.triangles = _indices;

		_PrevTick = Time.time;
	}
		
	public void SetupTrail(float currentTime) 
	{
		_isAlive = (_Quality != PerformanceInfo.eTRAIL_QUALITY.Off);
		_Renderer.material = _Material;
		timeAtStart = currentTime;
		_TimeToStartFade = currentTime + _FadeStartTime;
		_InitialTrailTime = _TrailTime;
		_TextureYCount = _TextureYSplit;
		_SegmentLength = _DistanceThreshold * 3f;
		_PrevTick = currentTime;
	}

	public void DestroyTrail()
	{
		if(Application.isPlaying)
		{
			GameObject.Destroy(_GameObject);
			GameObject.Destroy(_Mesh);
		}
		else
		{
			GameObject.DestroyImmediate(_GameObject);
			GameObject.DestroyImmediate(_Mesh);
		}
		_SegmentPositionBottomBuffer = null;
		_SegmentPositionTopBuffer = null;
		_SegmentSubsectionsBuffer = null;
		_SegmentTimesBuffer = null;
		_HermiteBasis = null;

	}

	~TrailRenderer()
	{
		_Mesh = null;
		_GameObject = null;
		_SegmentPositionBottomBuffer = null;
		_SegmentPositionTopBuffer = null;
		_SegmentSubsectionsBuffer = null;
		_SegmentTimesBuffer = null;
		_HermiteBasis = null;
	}

	public bool Sim(float tick)
	{
		float deltaTick = tick-_PrevTick;

		_PrevTick = tick;
		if (_UseLocalOffsets)
		{
			UpdateSections(_Point1.transform.TransformPoint(_Offset1), _Point2.transform.TransformPoint(_Offset2), tick);
		}
		else
		{
			UpdateSections(_Point1.transform.position+_Offset1, _Point2.transform.position+_Offset2, tick);
		}

		UpdateMesh( tick, deltaTick );
		return true;
	}

	public bool Update(float timeOffset) 
	{	
		if(!_isAlive) 
		{
			return false;
		}

		if (_UseLocalOffsets)
		{
			UpdateSections(_Point1.transform.TransformPoint(_Offset1), _Point2.transform.TransformPoint(_Offset2), Time.time - timeOffset);
		}
		else
		{
			UpdateSections(_Point1.transform.position+_Offset1, _Point2.transform.position+_Offset2, Time.time - timeOffset);
		}

		UpdateMesh(Time.time - timeOffset, Time.deltaTime);
		return true;
	}

	private void InsertPoints(Vector3 currentTopPosition, Vector3 currentBottomPosition, float time, int subsections = 1)
	{
		if (_Quality == PerformanceInfo.eTRAIL_QUALITY.Low)
		{
			subsections = 1;
		}
		_SegmentPositionTopBuffer.Enqueue(currentTopPosition);
		_SegmentPositionBottomBuffer.Enqueue (currentBottomPosition);
		_SegmentSubsectionsBuffer.Enqueue(subsections);
		_SegmentTimesBuffer.Enqueue(time);
		_TotalSubsections += subsections;
		//EB.Debug.Log("_TotalSubsections !!!! " + _TotalSubsections);
	}
	
	private void RemoveLastPoint()
	{
		_SegmentPositionTopBuffer.Dequeue();
		_SegmentPositionBottomBuffer.Dequeue();
		_SegmentSubsectionsBuffer.Dequeue();
		_SegmentTimesBuffer.Dequeue();
		if (_SegmentSubsectionsBuffer.Count > 0)
		{
			_TotalSubsections -= _SegmentSubsectionsBuffer.Peek();
		}
		//EB.Debug.Log("_TotalSubsections !!!! " + _TotalSubsections);
	}
	
	private void UpdateSections(Vector3 currentTopPosition, Vector3 currentBottomPosition, float time)
	{
		//EB.Debug.Log("Updating sections " +_SegmentPositionTopBuffer.Count + " " + _SegmentPositionBottomBuffer.Count);
		if (_SegmentPositionTopBuffer.Count == 0 || _SegmentPositionBottomBuffer.Count == 0)
		{
			if(_IgnoreZ) 
			{
				avgZ = (currentBottomPosition.z + currentTopPosition.z) / 2;
				currentBottomPosition = new Vector3(currentBottomPosition.x, currentBottomPosition.y, avgZ);
				currentTopPosition = new Vector3(currentTopPosition.x, currentTopPosition.y, avgZ);
			}
			_LastAddedBottomPoint = currentBottomPosition;
			_LastAddedTopPoint = currentTopPosition;
		
			InsertPoints(currentTopPosition, currentBottomPosition, time);

			return;
		}

		if(_IgnoreZ) 
		{
			currentBottomPosition = new Vector3(currentBottomPosition.x, currentBottomPosition.y, avgZ);
			currentTopPosition = new Vector3(currentTopPosition.x, currentTopPosition.y, avgZ);
		}
		float distanceBottom = Vector3.Distance(currentBottomPosition, _LastAddedBottomPoint);
		float distanceTop = Vector3.Distance(currentTopPosition, _LastAddedTopPoint);
		float maxDistance = Mathf.Max(distanceTop, distanceBottom);
		//EB.Debug.Log("distanceTop " +distanceTop + "distanceBottom " + distanceBottom);
		if (maxDistance > _DistanceThreshold)
		{
			int segments = Mathf.CeilToInt(Mathf.Min(distanceTop, distanceBottom) / _SegmentLength);
			//Debug.g("SEGMENT DIS " +segments);
			InsertPoints(currentTopPosition, currentBottomPosition, time, segments);
			_LastAddedBottomPoint = currentBottomPosition;
			_LastAddedTopPoint = currentTopPosition;
		}

		while(_SegmentPositionTopBuffer.Count > 0)
		{
			//EB.Debug.Log("time " + time + " _TrailTime " + _TrailTime + "_SegmentTimesBuffer.Peek() " +_SegmentTimesBuffer.Peek()); 
			if (time - _TrailTime > _SegmentTimesBuffer.Peek())
			{
				RemoveLastPoint();
			}
			else
			{
				break;
			}
		}
	}

	private void UpdateMesh(float time, float deltaTime)
	{
		_Mesh.Clear();
		
		if (_TotalSubsections < 1)
		{
			return;
		}
		//draw a quad for each segment 
		
		int vertexCount = (_TotalSubsections + 1) * 2;
		int currSegmentOffest = 0;
		int positionCount = _SegmentPositionTopBuffer.Count;
		float ratio = 1.0f;
		if( vertexCount >= _MaxVerts ) 
		{
			ratio = (float)_MaxVerts / (float)vertexCount;
		}

		int positionMax = Mathf.Max(0, positionCount - 1);
		float fadeMutiplier = 0;
		float overallTime = 0;
		if (_TrailType == eTRAIL_TYPE.Catchup) 
		{ 
			overallTime = (time - timeAtStart) / _FadeDuration;
			if(_TimeToStartFade < time && _TrailTime > 0) 
			{
				fadeMutiplier = (time - _TimeToStartFade)/ _FadeDuration;
				_TrailTime -= _TrailTime * fadeMutiplier;
			}
			_TrailTime = Mathf.Max(_TrailTime,0);
			if(_TrailTime <= 0)
			{
				_isAlive = false;
				return;
			}
		}

		if(_TrailType == eTRAIL_TYPE.Uniform) 
		{
			overallTime = (time - timeAtStart) / _FadeDuration;
			if(_TimeToStartFade < time) 
			{
				
				
				fadeMutiplier = (time - _TimeToStartFade)/ _FadeDuration;
				if(fadeMutiplier >= 1)
				{
					_isAlive = false;
				}
			}
		}

		if(_TrailType == eTRAIL_TYPE.Drag) 
		{
			
			fadeMutiplier = System.Math.Min( time / _FadeDuration, 1f) ;

		}

		float totalDistance = 0;
		if(_TextureMetersSecond > 0) 
		{
			_TextureMoveTimer += deltaTime * _TextureMetersSecond;
		}
		Color color = new Color();
		Color lifeColor = new Color();
		lifeColor = _LifeGradient.Evaluate(overallTime);

		int total = 0;
		for (int i = 0; i < positionMax; ++i)
		{
			total += Mathf.Max(Mathf.FloorToInt(ratio * _SegmentSubsectionsBuffer[i]),1);
		}
		for (int i = 0; i < positionMax; ++i)
		{

			Vector3 p1t0 = _SegmentPositionTopBuffer[(Mathf.Max(i - 1, 0))];
			Vector3 p1t1 = _SegmentPositionTopBuffer[(Mathf.Max(i + 0, 0))];
			Vector3 p1t2 = _SegmentPositionTopBuffer[(Mathf.Min(i + 1, positionMax))];
			Vector3 p1t3 = _SegmentPositionTopBuffer[(Mathf.Min(i + 2, positionMax))];

			Vector3 p2t0 = _SegmentPositionBottomBuffer[(Mathf.Max(i - 1, 0))];
			Vector3 p2t1 = _SegmentPositionBottomBuffer[(Mathf.Max(i + 0, 0))];
			Vector3 p2t2 = _SegmentPositionBottomBuffer[(Mathf.Min(i + 1, positionMax))];
			Vector3 p2t3 = _SegmentPositionBottomBuffer[(Mathf.Min(i + 2, positionMax))];
			
			Vector3 t_p1t1 = _CurveTension * (p1t2 - p1t0);
			Vector3 t_p1t2 = _CurveTension * (p1t3 - p1t1);
			
			Vector3 t_p2t1 = _CurveTension * (p2t2 - p2t0);
			Vector3 t_p2t2 = _CurveTension * (p2t3 - p2t1);
			
			float time1 = (time - _SegmentTimesBuffer[i] ) / _TrailTime;
			float time2 = (time - _SegmentTimesBuffer[(Mathf.Min(i + 1, positionMax))] ) / _TrailTime;
			time1 = Mathf.Clamp01(time1);
			time2 = Mathf.Clamp01(time2);

			int segments = Mathf.Max(Mathf.FloorToInt(ratio * _SegmentSubsectionsBuffer[i]),1);
			float segDist = Vector3.Magnitude(((p1t1 + p2t1)/2.0f) - ((p1t2 + p2t2)/2.0f));

			if ((currSegmentOffest + segments) * 6 > _MaxIndices)
			{
				//we are out of space
				break;
			}
			for (int j = 0; j < segments; ++j)
			{

				float t = ((float)j)/((float)segments);

				//POSITION

				float timeAtSegment = Mathf.Lerp(time1, time2, t);
				int offset = (currSegmentOffest + j) * 2;

				float width = _WidthCurve.Evaluate(timeAtSegment);
				float halfWidth = width * 0.5f;

				Vector3 interpolatedp1 = InterpolateHermite(p1t1, p1t2, t_p1t1, t_p1t2, t); // interpolate the top point
				Vector3 interpolatedp2 = InterpolateHermite(p2t1, p2t2, t_p2t1, t_p2t2, t); // interpolate the bottom point
				
				//here’s where we “combine” the top and bottom, to get our direction and midpoint
				Vector3 mid = (interpolatedp1 + interpolatedp2)/2.0f; //midpoint
				Vector3 dir = interpolatedp1-interpolatedp2; //direction between interpolated points

				//“push” the vertices by the direction around the midpoint

				_vertices[offset + 0] = mid + dir * halfWidth;
				_vertices[offset + 1] = mid - dir * halfWidth;

				// UVS
				
				Vector3 midpoint = GetPlaneMidpoint(p1t1, p1t2, p2t1, p2t2);

				float p1t1_p2t2_distance = Vector3.Distance(p2t2, p1t1);
				float mid_p2t2_distance = Vector3.Distance(p2t2, midpoint); 
				float p1t2_p2t1_distance = Vector3.Distance(p1t2, p2t1);
				float mid_p1t2_distance = Vector3.Distance(p1t2, midpoint); 

				float currentDistance = totalDistance + t * segDist;
				float uvOffsetU = currentDistance  / _TextureRepeat + _TextureMoveTimer;

				float uvOffsetV1 = 0;
				float uvOffsetV2 = 1;

				if(_TextureYSplit != 0)
				{
					uvOffsetV1 = ((float)_TextureYCount-1.0f) /(float)_TextureYSplit;
					uvOffsetV2 = (float)_TextureYCount/(float)_TextureYSplit;
				}

				float perspectiveCorrection0 = p1t1_p2t2_distance / mid_p2t2_distance;
				float perspectiveCorrection1 = p1t2_p2t1_distance / mid_p1t2_distance;

				_uvs[offset + 0] = new Vector3(uvOffsetU * perspectiveCorrection0, uvOffsetV1, perspectiveCorrection0);
				_uvs[offset + 1] = new Vector3(uvOffsetU * perspectiveCorrection1, uvOffsetV2, perspectiveCorrection1);

				//COLOR

				float _t = ((float)j + (float)currSegmentOffest + 1) / (float)total;

				if(_SpanOverTrail) 
				{
					if((float)j + (float)currSegmentOffest == 0)
					{
						_t = 0;
					}
					color = _ColorGradient.Evaluate(_t);
				}
				else
				{
					color = _ColorGradient.Evaluate(timeAtSegment);
				}

				if(_AddColor)
				{
					color.r += lifeColor.r;
					color.g += lifeColor.g;
					color.b += lifeColor.b;
					color.a *= lifeColor.a;
				}
				else
				{
					color.a *= lifeColor.a;
				}
			
				_colors[offset + 0] = color;
				_colors[offset + 1] = color;
			}
			currSegmentOffest += segments;
			totalDistance += segDist;
		}
		_TextureYCount ++;
		
		for (int i = 0; i < currSegmentOffest - 1; ++i)
		{
			_indices[i * 6 + 0] = i * 2;
			_indices[i * 6 + 1] = i * 2 + 1;
			_indices[i * 6 + 2] = i * 2 + 2;
			_indices[i * 6 + 3] = i * 2 + 2;
			_indices[i * 6 + 4] = i * 2 + 1;
			_indices[i * 6 + 5] = i * 2 + 3;
		}

		for (int i = Mathf.Max(currSegmentOffest - 1, 0); i < Mathf.Max(_LastSegmentOffset - 1, 0); ++i)
		{
			_indices[i * 6 + 0] = 0;
			_indices[i * 6 + 1] = 0;
			_indices[i * 6 + 2] = 0;
			_indices[i * 6 + 3] = 0;
			_indices[i * 6 + 4] = 0;
			_indices[i * 6 + 5] = 0;
		}

		//EB.Debug.Log("_TotalSubsections " +_TotalSubsections + " currSegmentOffest " + currSegmentOffest);

		_LastSegmentOffset = currSegmentOffest;
		_Mesh.vertices = _vertices;
		_Mesh.normals = _uvs;		//interpret normals as UVs as we need to pass up at vector 3
		_Mesh.colors = _colors;
		_Mesh.triangles = _indices;
	}
	
	private Vector3 InterpolateHermite(Vector3 p1, Vector3 p2, Vector3 t1, Vector3 t2, float step)
	{
		Vector4 s = new Vector4(step * step * step, step * step, step, 1);
		return p1 * Vector4.Dot(s, _HermiteBasis[0]) + p2 * Vector4.Dot(s, _HermiteBasis[1]) + t1 * Vector4.Dot(s, _HermiteBasis[2]) + t2 * Vector4.Dot(s, _HermiteBasis[3]);
	}  
	
	private Vector3 GetPlaneMidpointCheap(Vector3 p1t1, Vector3 p1t2, Vector3 p2t1, Vector3 p2t2)
	{
		float p1d = Vector3.Distance(p1t1,p1t2);
		float p2d = Vector3.Distance(p2t1,p2t2);

		float ratio = p1d > p2d ? p2d/p1d : p1d/p2d;

		//EB.Debug.LogWarning("current ratio " + ratio);
		Vector3 avg1 = (p1t1 + p1t2) * 0.5f;
		Vector3 avg2 = (p2t1 + p2t2) * 0.5f;
		Vector3 dir = Vector3.zero;
		Vector3 mid = (avg1 + avg2) * 0.5f;

		if(p1d > p2d) 
		{
			dir = avg2 - mid;  
		}
		else 
		{
			dir = avg1 - mid;
		}

		Vector3 target = mid + dir * (1.0f - ratio) * _MidpointAdjust;

		return target;
	}

	private Vector3 GetPlaneMidpoint(Vector3 p1t1, Vector3 p1t2, Vector3 p2t1, Vector3 p2t2)
	{
		Vector3 v1 = p2t1 - p1t1;
		Vector3 v2 = p1t2 - p1t1;
		Vector3 n = Vector3.Cross(v1, v2);
		
		//plane
		Vector3 x1 = p1t1;
		Vector3 x2 = p2t2;
		Vector3 x3 = p1t1 + n;
		//line
		Vector3 x4 = p1t2;
		Vector3 x5 = p2t1;
		
		Matrix4x4 top = new Matrix4x4();
		top.SetColumn(0, new Vector4(1, x1.x, x1.y, x1.z));
		top.SetColumn(1, new Vector4(1, x2.x, x2.y, x2.z));
		top.SetColumn(2, new Vector4(1, x3.x, x3.y, x3.z));
		top.SetColumn(3, new Vector4(1, x4.x, x4.y, x4.z));
		
		Matrix4x4 bottom = new Matrix4x4();
		bottom.SetColumn(0, new Vector4(1, x1.x, x1.y, x1.z));
		bottom.SetColumn(1, new Vector4(1, x2.x, x2.y, x2.z));
		bottom.SetColumn(2, new Vector4(1, x3.x, x3.y, x3.z));
		bottom.SetColumn(3, new Vector4(0, x5.x - x4.x, x5.y - x4.y, x5.z - x4.z));
		
		float detTop = DeterminantTop(top);
		float detBot = DeterminantBottom(bottom);
		
		float t = -detTop/detBot;
		
		return x4 + (x5 - x4) * t;
	}

	public static float DeterminantTop(Matrix4x4 m) 
	{
		return
			m.m12 * m.m21 * m.m30 - m.m13 * m.m21 * m.m30 - m.m11 * m.m22 * m.m30 + m.m13 * m.m22 * m.m30 +
			m.m11 * m.m23 * m.m30 - m.m12 * m.m23 * m.m30 - m.m12 * m.m20 * m.m31 + m.m13 * m.m20 * m.m31 +
			m.m10 * m.m22 * m.m31 - m.m13 * m.m22 * m.m31 - m.m10 * m.m23 * m.m31 + m.m12 * m.m23 * m.m31 +
			m.m11 * m.m20 * m.m32 - m.m13 * m.m20 * m.m32 - m.m10 * m.m21 * m.m32 + m.m13 * m.m21 * m.m32 +
			m.m10 * m.m23 * m.m32 - m.m11 * m.m23 * m.m32 - m.m11 * m.m20 * m.m33 + m.m12 * m.m20 * m.m33 +
			m.m10 * m.m21 * m.m33 - m.m12 * m.m21 * m.m33 - m.m10 * m.m22 * m.m33 + m.m11 * m.m22 * m.m33;
	}

	public static float DeterminantBottom(Matrix4x4 m) 
	{
		return
			-m.m13 * m.m21 * m.m30 + m.m13 * m.m22 * m.m30 +
			m.m11 * m.m23 * m.m30 - m.m12 * m.m23 * m.m30 + m.m13 * m.m20 * m.m31 +
			-m.m13 * m.m22 * m.m31 - m.m10 * m.m23 * m.m31 + m.m12 * m.m23 * m.m31 +
			-m.m13 * m.m20 * m.m32 + m.m13 * m.m21 * m.m32 +
			m.m10 * m.m23 * m.m32 - m.m11 * m.m23 * m.m32 - m.m11 * m.m20 * m.m33 + m.m12 * m.m20 * m.m33 +
			m.m10 * m.m21 * m.m33 - m.m12 * m.m21 * m.m33 - m.m10 * m.m22 * m.m33 + m.m11 * m.m22 * m.m33;
	}

}
