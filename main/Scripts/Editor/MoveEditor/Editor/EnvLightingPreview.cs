using UnityEngine;

namespace MoveEditor
{
	public class EnvLightingPreview
	{
		public void Init(EnvironmentLightingProperties properties, float startTime)
		{
			_properties	= properties;
			_startTime 	= startTime;
			_duration	= Mathf.Max(properties._addDuration, properties._multiplyDuration);
		}

		public void Update(float time)
		{
			float t = time - _startTime;

			if (t >= 0 && t <= _duration)
			{
				Color multiply 	= _properties._multiplyGradient.Evaluate(t / _properties._multiplyDuration);
				Color add		= _properties._addGradient.Evaluate(t / _properties._addDuration);
				RenderGlobals.SetEnvironmentAdjustments(multiply, add);
			}
		}

		private EnvironmentLightingProperties	_properties 		= null;
		private float							_startTime			= 0;
		private float							_duration			= 0;
		//private bool							_played				= false;
	}
}