///////////////////////////////////////////////////////////////////////
//
//  ParallaxGyroMonitor.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class ParallaxGyroMonitor
{
	private float _scaleX = 1.0f;
	public float ScaleX
	{
		get 
		{
			return _scaleX;
		}
		set
		{
			_scaleX = value;
		}
	}

	private float _scaleY = 1.0f;
	public float ScaleY 
	{
		get
		{
			return _scaleY;
		}
		set 
		{
			_scaleY = value;
		}
	}

	public Vector3 RotationValue 
	{
		get
		{
			return new Vector3(ValueY, ValueX, 0); // Flip axes for rotation around the axes, not position along the axes
		}
	}

	public Vector3 Value
	{
		get 
		{
			return new Vector3(ValueX, ValueY, 0);
		}
	}

	public float ValueX
	{
		get 
		{
			return ScaleX * _normalizedX;
		}
	}

	public float ValueY
	{
		get 
		{
			return ScaleY * _normalizedY;
		}
	}

	private float _inputAngleLimitX = 30.0f;
	public float InputAngleLimitX 
	{
		get 
		{
			return _inputAngleLimitX;
		}
		set 
		{
			_inputAngleLimitX = value;
		}
	}
	private float _inputAngleLimitY = 30.0f;
	public float InputAngleLimitY 
	{
		get 
		{
			return _inputAngleLimitY;
		}
		set 
		{
			_inputAngleLimitY = value;
		}
	}

	private float _normalizedX = 0;
	private float _normalizedY = 0;

	private Quaternion _baseAttitude = Quaternion.identity;

	public void Update(float deltaTime)
	{
		Quaternion currentAttitude = Quaternion.identity;
        //永远不进入这个逻辑
		if (false && Input.gyro.enabled && UserData.MotionEnabled)
		{
			currentAttitude = Input.gyro.attitude;
			if (_baseAttitude == Quaternion.identity)
			{
				_baseAttitude = currentAttitude;
			}

			Quaternion relative = Quaternion.Inverse(_baseAttitude) * currentAttitude;

			Vector3 relativeEuler = relative.eulerAngles;
			if (relativeEuler.x > 180.0f)
			{
				relativeEuler.x = relativeEuler.x - 360.0f;
			}
			if (relativeEuler.y > 180.0f)
			{
				relativeEuler.y = relativeEuler.y - 360.0f;
			}
			
			// X value based on Y-axis rotation, Y value based on X-axis rotation
			_normalizedY = Mathf.Clamp(relativeEuler.x/_inputAngleLimitY, -1.0f, 1.0f);
			_normalizedX = Mathf.Clamp(relativeEuler.y/_inputAngleLimitX, -1.0f, 1.0f);

			//DebugSystem.Log("curr " + currentAttitude.eulerAngles + " | base " + baseAttitude.eulerAngles + " | diff " + relativeEuler + " | x,y " + normalizedParallaxX + "," + normalizedParallaxY);

			Quaternion zRot = Quaternion.identity;
			zRot.eulerAngles = new Vector3(0, 0, relativeEuler.z);
			_baseAttitude *= zRot;

			_baseAttitude = Quaternion.Slerp(_baseAttitude, currentAttitude, deltaTime);
		}
		else
		{
			_normalizedX = 0;
			_normalizedY = 0;
		}
	}
}