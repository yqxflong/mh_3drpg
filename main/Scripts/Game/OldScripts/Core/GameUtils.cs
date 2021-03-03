using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GameUtils
{
	public static string ColoredString(string s, Color c)
	{
		return "<color=" + ColorToWebColor(c) + ">" + s + "</color>";
	}

	public static UIEventTrigger GetUIEventTrigger(this GameObject go)
	{
		UIEventTrigger evtTrigger = go.GetComponent<UIEventTrigger>();

		if(evtTrigger == null)
		{
			evtTrigger = go.AddComponent<UIEventTrigger>();
		}

		if(evtTrigger == null)
		{
			EB.Debug.LogError("GetUIEventTrigger: Can't Add/Get UIEventTrigger!");
		}

		return evtTrigger;
	}

	// calculates an axis aligned bounding box to encapsulate all the points
	public static Bounds CalculateBounds(List<Vector3> allPoints)
	{
		Vector3 BoundingBoxMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 BoundingBoxMax = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);

		for (int i = 0; i < allPoints.Count; ++i) // go over all points getting the min and max x,y,z elements
		{
			EncapsulatePoint(allPoints[i], ref BoundingBoxMin, ref BoundingBoxMax);
		}

		Vector3 BoundingBoxCenter = Vector3.Lerp(BoundingBoxMin, BoundingBoxMax, 0.5f);
		Vector3 BoundingBoxSize = BoundingBoxMax - BoundingBoxMin;

		return new Bounds(BoundingBoxCenter, BoundingBoxSize);
	}

	#region CalculateBoundsWithOffset
	private static Dictionary<GameObject, CapsuleCollider> _CalculateBoundsWithOffset = new Dictionary<GameObject, CapsuleCollider>();
    
	public static Bounds CalculateBoundsWithOffset(List<GameObject> allPoints)
	{
		Vector3 BoundingBoxMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 BoundingBoxMax = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);
		
		int count = allPoints.Count;
		for (int i = 0; i < count; ++i) // go over all points getting the min and max x,y,z elements
		{
			var go = allPoints[i];
			if(go == null)
			{
				continue;
			}
			Vector3 pos = go.transform.position;
			
			CapsuleCollider collider = null;
			if(!_CalculateBoundsWithOffset.TryGetValue(go, out collider))
			{
				collider = go.GetComponent<CapsuleCollider>();
				_CalculateBoundsWithOffset[go] = collider;
			}

			if(collider != null)
			{
				pos = go.transform.TransformPoint(collider.center);
			}
			
			EncapsulatePoint(pos, ref BoundingBoxMin, ref BoundingBoxMax);
		}
		
		Vector3 BoundingBoxCenter = Vector3.Lerp(BoundingBoxMin, BoundingBoxMax, 0.5f);
		Vector3 BoundingBoxSize = BoundingBoxMax - BoundingBoxMin;
		Bounds bound =  new Bounds(BoundingBoxCenter, BoundingBoxSize);
		return bound;
	}
	#endregion
    
	// get the character model for the local player 
	public static CharacterModel GetLocalPlayerCharacterModel()
	{
		PlayerController localController = PlayerManager.LocalPlayerController();
		if (null != localController)
		{
			return GetCharacterModel(localController.gameObject);			
		}
		return null;		
	}

	// get the character model 
	public static CharacterModel GetCharacterModel(GameObject characterObject)
	{
		if (null != characterObject)
		{
			CharacterComponent character = characterObject.GetComponent<CharacterComponent>();
			if (null != character)
			{
				return character.Model;
			}
		}
		return null;
	}

	// Calculate gameObject render bounds
	public static bool CalculateRenderBounds(GameObject obj, ref Bounds combinedBounds)
	{
		bool isInitialized = false;
		combinedBounds.center = Vector3.zero;
		combinedBounds.size = Vector3.zero;
		if (null != obj)
		{
			if (obj.GetComponent<Renderer>() != null)
			{				
				combinedBounds.center = obj.GetComponent<Renderer>().bounds.center;
				combinedBounds.size = obj.GetComponent<Renderer>().bounds.size;
				isInitialized = true;
			}

			Renderer[] allRenderers = obj.GetComponentsInChildren<Renderer>();
			foreach (Renderer subRenderer in allRenderers)
			{
				if ((subRenderer != obj.GetComponent<Renderer>()) && (null != subRenderer))
				{
					if (!isInitialized)
					{
						combinedBounds.center = subRenderer.bounds.center;
						combinedBounds.size = subRenderer.bounds.size;
						isInitialized = true;
					}
					else
					{
						combinedBounds.Encapsulate(subRenderer.bounds);
					}
				}
			}
		}
		return isInitialized;
	}

	// see if the triangle intersects the bounds specified by minBounds and maxBounds (on the ground plane X,Z)
	public static bool IsTriangleInBoundsXZ(Vector3 triangleVert0, Vector3 triangleVert1, Vector3 triangleVert2, Vector3 minBounds, Vector3 maxBounds)
	{
		// look for a seperating axis
		if ((triangleVert0.x < minBounds.x && triangleVert1.x < minBounds.x && triangleVert2.x < minBounds.x) || // all triangle verts are less than the min.x value of the bounds

			(triangleVert0.x > maxBounds.x && triangleVert1.x > maxBounds.x && triangleVert2.x > maxBounds.x) || // all triangle verts are greater than the max.x value of the bounds

			(triangleVert0.z < minBounds.z && triangleVert1.z < minBounds.z && triangleVert2.z < minBounds.z) || // all triangle verts are less than the min.z value of the bounds

			(triangleVert0.z > maxBounds.z && triangleVert1.z > maxBounds.z && triangleVert2.z > maxBounds.z)) // all triangle verts are greater than the max.z value of the bounds
		{
			return false; // a seperating acis was found - out of bounds
		}
		return true;
	}

	// recursively fine the transform include inRoot and the whole hierarchy under the inRoot, this also include the object that is deactivate
	public static Transform SearchHierarchyForBone(Transform inRoot, string inName, bool ignoreDisabled = false)
	{
		if (inRoot == null || inName.Length <= 0)
		{
			return null;
		}
		// check if the current bone is the bone we're looking for, if so return it
		if (inRoot.name.Equals(inName))
		{
			return inRoot;
		}

		EB.Collections.Queue<Transform> queue = new EB.Collections.Queue<Transform>(16);
		Transform result = null;
		queue.Enqueue(inRoot);
		while (queue.Count > 0)
		{
			Transform it = queue.Dequeue();
			result = it.Find(inName);
			if (result && (!ignoreDisabled || result.gameObject.activeInHierarchy))
			{
				return result;
			}
			
			int childCount = it.childCount;
			for (int i = 0; i < childCount; ++i)
			{
				queue.Enqueue(it.GetChild(i));
			}
		}
		return null;
	}
	
	private static Transform InternalRecursive(Transform inRoot, string inName)
	{
		Transform result = inRoot.Find(inName);
		if (result)
		{
			return result;
		}
		
		// search through child bones for the bone we're looking for
		int childCount = inRoot.childCount;
		for(int i = 0; i < childCount; ++i)
		{
			// the recursive step; repeat the search one step deeper in the hierarchy
			result = InternalRecursive(inRoot.GetChild(i), inName);
			
			// a transform was returned by the search above that is not null,
			// it must be the bone we're looking for
			if (result)
			{
				return result;
			}
		}
		return null;
	}
	
	public static void SetShaderRecursive(Transform parent, string shaderName)
	{
		Shader shader = Shader.Find(shaderName);
		if (shader == null)
		{
			EB.Debug.LogWarning("GameUtils::SetShaderRecursive can't find shader '" + shaderName + "'!");
		}
		
		if (parent.GetComponent<Renderer>())
		{
			parent.GetComponent<Renderer>().material.shader = shader;
		}
	
		foreach (Transform child in parent)
		{
			SetShaderRecursive(child, shaderName);
		}	
	}	
	
	// calculate the forward vector of the quaternion
	public static Vector3 CalculateForward(Quaternion quat)
	{
		Matrix4x4 transformMat = Matrix4x4.TRS(Vector3.zero, quat, Vector3.one);
		const int ForwardColumn = 2;
		Vector3 forward = transformMat.GetColumn(ForwardColumn);
		return forward;
	}

	// normalize a quaternion (as of writing, Quaternion has no normalize function)
	public static void Normalize(ref Quaternion quat)
	{
		quat = Quaternion.Lerp(quat, quat, 1f); // Quaternion.Lerp will normalize the result after doing the lerp
	}

	public static Vector3 SubXZ(Vector3 v1, Vector3 v2)
	{
		Vector3 v = v1 - v2;
		v.y = 0f;
		return v;
	}

	public static float GetDistSqXZ(Vector3 v1, Vector3 v2)
	{
		Vector3 v = v2 - v1;
		v.y = 0;
		return v.sqrMagnitude;
	}

	public static float GetDistXZ(Vector3 v1, Vector3 v2)
	{
		Vector3 v = v2 - v1;
		v.y = 0;
		return v.magnitude;
	}

	public static float DotXZ(Vector3 v1, Vector3 v2)
	{
		return (v1.x * v2.x) + (v1.z * v2.z);
	}

	public static float Square(float input)
	{
		return input * input;
	}

	// remove y element, normalize vector, return true/false based on whether original vecor was large enough to be normalized
	public static bool NormalizeXZ(ref Vector3 toNormalize)
	{
		toNormalize.y = 0f;
		toNormalize.Normalize();
		const float TolSqr = 0.5f * 0.5f;
		return (toNormalize.sqrMagnitude > TolSqr); // if the normalize succeeded, the length of the resultant vector will be 1, else length 0f
	}

	// traverse upwards in the scene hierarchy, return first matching component 
	public static Component FindFirstComponentUpwards<T>(Transform current)
	{
		Component component = null;
		while (current != null)
		{
			component = current.GetComponent(typeof(T));
			if (component != null)
				return component;

			current = current.parent;
		}
		return component;
	}

	public static bool FastRemove<T>(List<T> list, T element)
	{
		int index = list.IndexOf(element);
		if (index != -1)
		{
			FastRemove(list, index);
			return true;
		}
		return false;
	}

	public static void FastRemove<T>(List<T> list, int index)
	{
		if (index < list.Count)
		{
			int lastIndex = list.Count - 1;
			if (list.Count > 1)
			{
				T lastItem = list[lastIndex];
				list[index] = lastItem;
			}
			list.RemoveAt(lastIndex);
		}
	}
	
	public static UnityEngine.Object InstantiateThroughObjectManager(UnityEngine.Object prefab, Vector3 position, Quaternion rotation,string pathName=null)
	{
		return PoolModel.GetModel(prefab, position, rotation);
	}

	public static string ConvertToFriendlyName(string codeName)
	{
		if (codeName == "")
		{
			return "";
		}

		string friendlyName = System.Char.ToUpper(codeName[0]) + "";
		int lastSplit = 1;
		for (int i = 1; i < codeName.Length; i++)
		{
			if (System.Char.IsUpper(codeName[i])) 
			{
				friendlyName += codeName.Substring(lastSplit, i - lastSplit) + " ";
				lastSplit = i;
			}
		}

		return friendlyName + codeName.Substring(lastSplit);
	}
    
	// if the ground height cannot be found, this function will return the y value of the passed in pos
	public static float CalculateGroundHeight(Vector3 pos)
	{
		float result = 0f;
		RaycastHit hit;
		int mask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Ground");
		CalculateGroundHeight(pos, mask, out hit, ref result);
		return result;
	}

	// if the ground height cannot be found, this function will set outGroundHeight to the y value of the passed in pos
	public static bool CalculateGroundHeight(Vector3 pos, int mask, out RaycastHit hit, ref float outGroundHeight)
	{		
		outGroundHeight = pos.y;

		const float SphereCastStartYOffset = 5f; // this needs to be large enough that if a character is going up stairs, the start postion of the sphere case will be above the ground 
												 // but not too large that the start position may be above a higher level of ground like a bridge overhead
		const float SphereCastRadius = 0.35f; // this value ensures the sphere cast does not begin intersecting the ground, but is large enough to not go through seems
		const float GroundClearence = 0.1f; // how much we raise our returned hit point, so things aren't exactly on the ground
		const float SphereCastLength = 10f; // this needs to be long enough to find ground below 'pos'

		if (Physics.SphereCast(pos + (Vector3.up * SphereCastStartYOffset), SphereCastRadius, Vector3.down, out hit, SphereCastLength, mask))
		{
			outGroundHeight = hit.point.y + GroundClearence;
			return true;
		}
		else if (null != AstarPath.active) // the sphere cast should never fail, let's see if starting the sphere cast from the height of the nav mesh makes a difference
		{
			NNInfo info = AstarPath.active.GetNearest(pos);
			if (null != info.node)
			{
				pos.y = info.clampedPosition.y;
				if (Physics.SphereCast(pos + (Vector3.up * SphereCastStartYOffset), SphereCastRadius, Vector3.down, out hit, SphereCastLength, mask))
				{
					outGroundHeight = hit.point.y + GroundClearence;
					return true;
				}
			}
		}
		return false;
	}
    
	public static void SetDefaultLighting()
	{
		GameObject directionalLightGO = GameObject.Find("Main Light");
		if(directionalLightGO != null)
		{
			Light directionalLight = directionalLightGO.GetComponent<Light>();
			
			if(directionalLight != null)
			{
				directionalLight.transform.position = GameVars.MainLightPosition;
				directionalLight.transform.rotation = GameVars.MainLightRotation;
				directionalLight.intensity = GameVars.MainLightIntensity;
				directionalLight.color = GameVars.MainLightColor;
			}
		}
		
		UnityEngine.RenderSettings.ambientLight = GameVars.GlobalAmbient;
	}
	
	public static void ChangeChildrenLayers(GameObject parentObject, int layer)
	{
		Transform[] allChildren = parentObject.GetComponentsInChildren<Transform>(true);
		
		foreach(Transform child in allChildren)
		{
			child.gameObject.layer = layer;
		}	
	}

	private static void EncapsulatePoint(Vector3 point, ref Vector3 BoundingBoxMin, ref Vector3 BoundingBoxMax)
	{
		BoundingBoxMin.x = point.x < BoundingBoxMin.x ? point.x : BoundingBoxMin.x;
		BoundingBoxMax.x = point.x > BoundingBoxMax.x ? point.x : BoundingBoxMax.x;

		BoundingBoxMin.y = point.y < BoundingBoxMin.y ? point.y : BoundingBoxMin.y;
		BoundingBoxMax.y = point.y > BoundingBoxMax.y ? point.y : BoundingBoxMax.y;

		BoundingBoxMin.z = point.z < BoundingBoxMin.z ? point.z : BoundingBoxMin.z;
		BoundingBoxMax.z = point.z > BoundingBoxMax.z ? point.z : BoundingBoxMax.z;
	}

	public static void SetShaderDefaults()
	{
		Vector4 fogColor = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
		Shader.SetGlobalColor("_FogColor", fogColor);
	}

	public static string FormatBytes(long bytes)
	{
		const int scale = 1024;
		string[] orders = new string[] {"GB", "MB", "KB", "Bytes"};
		long max = (long)Mathf.Pow(scale, orders.Length - 1);
 
		foreach(string order in orders)
		{
			if( bytes > max )
			{
				return string.Format("{0:##.##} {1}", decimal.Divide( bytes, max ), order);
			}
 
			max /= scale;
		}
		
		return "0 Bytes";
	}
    
	public static T GetCopyOf<T>(this Component comp, T other) where T : Component
	{
		System.Type type = comp.GetType();
		if (type != other.GetType()) return null; // type mis-match
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		PropertyInfo[] pinfos = type.GetProperties(flags);
		foreach (PropertyInfo pinfo in pinfos) {
			if (pinfo.CanWrite) {
				try {
					pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				}
				catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
			}
		}
		FieldInfo[] finfos = type.GetFields(flags);
		foreach (var finfo in finfos) {
			finfo.SetValue(comp, finfo.GetValue(other));
		}
		return comp as T;
	}

	public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
	{
		return go.AddComponent<T>().GetCopyOf(toAdd) as T;
	}

	public static Camera GetMainCamera()
	{
		var cam = Camera.main;
		if (cam == null)
		{
			GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			if (mainCamera != null)
			{
				cam = mainCamera.GetComponent<Camera>();
			}
		}
		if (cam == null)
		{
			GameObject mainCamera = GameObject.Find("Main Camera");
			if (mainCamera != null)
			{
				cam = mainCamera.GetComponent<Camera>();
			}
		}
		return cam;
	}

	private static object _mainCameraHandle = null;
    public static void SetMainCameraActive(bool active)
	{
        EB.Coroutines.ClearTimeout(_mainCameraHandle);
		//Debug.LogFormat("SetMainCameraActive({0})", active);
		Camera camera = GetMainCamera();
        if (camera != null)
        {
        	if (active)
        	{
        		camera.enabled = active;
        	}
        	else
        	{
        		_mainCameraHandle = EB.Coroutines.SetTimeout(delegate() { if (camera != null) camera.enabled = active; }, 1500);
        	}
        }
    }
	public static string ColorToWebColor(Color c)
	{
		string ret = "#";
		for (int i = 0; i < 4; i++)
		{
			string t = Mathf.FloorToInt(c[i] * 255).ToString("x");
			if (t.Length < 2)
			{
				t = "0" + t;
			}
			ret += t;
		}
		return ret;
	}

#if UNITY_EDITOR
	public static void CleanupEditor()
	{
		EditorUtility.ClearProgressBar();
		EditorApplication.isPaused = false;
		EditorApplication.isPlaying = false;
		EditorApplication.RepaintProjectWindow();
	}
#endif
}
