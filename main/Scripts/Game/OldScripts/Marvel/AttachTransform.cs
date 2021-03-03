using UnityEngine;

public enum HeightLock
{
	Unlocked,
	Locked,
	LockedToGround,
}

[ExecuteInEditMode]
public class AttachTransform : MonoBehaviour
{
	public Transform _attachment;

#if UNITY_EDITOR
	void Start()
	{
		if (!_attached && _attachment != null)
		{
			Attach(gameObject, _attachment, false, false);
		}
	}
#endif

	void LateUpdate() 
	{
		if (UpdateManually)
		{
			return;
		}

		UpdateAttachment();
	}

	public void UpdateAttachment()
	{
		if (_attachment != null)
		{
			transform.position = _attachment.TransformPoint(_startChildPosition);
			if (!_lockRotation)
			{
				transform.rotation = _attachment.rotation * _startChildRotation;

				if(_flipped)
				{
					//This needs to be integrated at some point! mmcmanus
					//MoveEditor.MoveUtils.FlipWorldRotationByXYPlane(transform);
				}
			}
		}

		LockPosition();
	}

	private void LockPosition()
	{
		Vector3 position = transform.position;

		if (_lockXOffset)
		{
			position.x = _startChildWorldPosition.x;
		}
		
		switch(_lockYOffset)
		{
			case HeightLock.Locked:
				position.y = _startChildWorldPosition.y;
				break;
			case HeightLock.LockedToGround:
				position.y = _groundHeight;
				break;
		}
		
		if (_lockZOffset)
		{
			position.z = _startChildWorldPosition.z;
		}

		transform.position = position;
	}

	public static AttachTransform Attach(GameObject target, Transform attachment)
	{
		return Attach(target, attachment, false, false);
	}

	public static AttachTransform Attach(GameObject target, Transform attachment, bool flipped, bool lockRotation, bool lockX = false, HeightLock lockY = HeightLock.Unlocked, bool lockZ = false)
	{
		AttachTransform at = target.GetComponent<AttachTransform>();
		if (at == null)
		{
			at = target.AddComponent<AttachTransform>();
		}

		// Note: Ideally child rot and child position should be passed in, and not recalculated by applying the inverses. It's unnecessary.
		
		at._attachment 				= attachment;
		at._startChildRotation 		= Quaternion.Inverse(attachment.rotation) * target.transform.rotation;
		at._startChildPosition 		= attachment.InverseTransformPoint(target.transform.position); 
		at._startChildWorldPosition	= target.transform.position;
#if UNITY_EDITOR
		at._attached				= true;
#endif
		at._flipped 				= flipped;
		at._lockRotation			= lockRotation;
		at._lockXOffset				= lockX;
		at._lockYOffset				= lockY;
		at._lockZOffset				= lockZ;

		at.UpdateAttachment();

		return at;
	}

	// TJ: Use this if we only want to lock the position of the thing as it moves, but not use the attach logic to position it
	public static AttachTransform LockPosition(GameObject target, bool lockX = false, HeightLock lockY = HeightLock.Unlocked, bool lockZ = false)
	{
		AttachTransform at = target.GetComponent<AttachTransform>();
		if (at == null)
		{
			at = target.AddComponent<AttachTransform>();
		}

		at._attachment				= null;
		at._startChildWorldPosition = target.transform.position;
		at._lockXOffset 			= lockX;
		at._lockYOffset				= lockY;
		at._lockZOffset				= lockZ;

		return at;
	}
	
    /// <summary>
    /// 清除掉对象上挂载的组件
    /// </summary>
    /// <param name="target">对像</param>
	public static void Detach(GameObject target)
	{
		AttachTransform at = target.GetComponent<AttachTransform>();
		if (at != null)
		{
			if (Application.isPlaying)
			{
				Destroy(at);
			}
			else
			{
				DestroyImmediate(at);
			}
		}
	}

	public static void SetGroundHeight(float height)
	{
		_groundHeight = height;
	}
	
	private static Vector3 DivideVectors(Vector3 num, Vector3 den) 
	{
		return new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);
	}
	
	public bool UpdateManually { get; set; }

#if UNITY_EDITOR
	private 		bool 			_attached 					= false;
#endif
	private 		bool			_flipped					= false;
	private 		bool			_lockRotation				= false;
	private 		bool			_lockXOffset				= false;
	private 		HeightLock		_lockYOffset				= HeightLock.Unlocked;
	private 		bool			_lockZOffset				= false;
	private 		Vector3 		_startChildPosition			= Vector3.zero;
	private 		Vector3			_startChildWorldPosition	= Vector3.zero;
	private 		Quaternion 		_startChildRotation			= Quaternion.identity;
	private static	float			_groundHeight				= 0.0f;
}
