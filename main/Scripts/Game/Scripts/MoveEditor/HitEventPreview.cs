using UnityEngine;
using System.Collections;



[ExecuteInEditMode]
public class HitEventPreview : MonoBehaviour
{
	private MoveEditor.HitEventInfo _hitEvent = null;
	private Animator _animator = null;
	
	private void Start()
	{
	}
	
	public MoveEditor.HitEventInfo HitEvent 
	{
		get { return _hitEvent; }
		set { _hitEvent = value; }
	}
	
	public Animator Animator
	{
        get { return _animator; }
		set { _animator = value; }
	}

	/*
	private void OnDrawGizmos() 
	{
		if( _hitEvent != null && _animator != null )
		{
			MoveEditor.DamageEventProperties damageProperties = _hitEvent._damageProperties;
			Transform bodyPartTransform = MoveEditor.MoveUtils.GetBodyPartTransform(_animator, damageProperties._pointOfOrigin);

			if (bodyPartTransform != null)
			{
				float rotAngle = transform.forward.x > 0.0f ? damageProperties._angle : damageProperties._angle * -1.0f;
				Vector3 direction = Quaternion.Euler(0.0f, 0.0f, rotAngle) * transform.forward;
				Vector3 origin = bodyPartTransform.TransformPoint(damageProperties._offset);
				float range = damageProperties._range;
				Gizmos.color = Color.red;
				Gizmos.DrawRay( origin, direction * range );
			}
		}
	}
	*/
}
