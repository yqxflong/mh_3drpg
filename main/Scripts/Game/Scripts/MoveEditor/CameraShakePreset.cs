using UnityEngine;
using System.Collections;

namespace MoveEditor
{
	public class CameraShakePreset : MonoBehaviour 
	{
		public int		_numberOfShakes			= 0;
		public Vector3 	_shakeAmount			= Vector3.zero;
		public Vector3 	_rotationAmount			= Vector3.zero;
		public float 	_distance				= 0;
		public float 	_speed					= 0;
		public float 	_decay					= 0;
		public bool 	_multiplyByTimeScale	= true;
	}
}
