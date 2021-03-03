using UnityEngine;

namespace MoveEditor
{
	public class DynamicLightPreview
	{
		public void Init(DynamicLightEventProperties properties, Animator animator, float startTime, float stopTime)
		{
			//_properties 	= properties;
			_startTime 		= startTime;
			_stopTime		= stopTime;
			_dynamicLight 	= MoveUtils.InstantiateDynamicLight(properties, animator, false, true).GetComponent<DynamicPointLightInstance>();
			_played 		= false;
			_stopped		= false;

			if (!Application.isPlaying)
				_dynamicLight.EnableSimMode(true);

			_dynamicLight.Stop();
		}

		public void Reset()
		{
			_played 	= false;
			_stopped	= false;

			if (_dynamicLight != null)
			{
				_dynamicLight.Stop();
			}
		}

		public void Cleanup()
		{
			if (_dynamicLight != null)
			{
				if (!Application.isPlaying)
					_dynamicLight.EnableSimMode(false);

				_dynamicLight.Stop();
				GameObject.DestroyImmediate(_dynamicLight.gameObject);
			}
		}
		
		public void Update(float time)
		{
			if (_dynamicLight != null)
			{	
				if (!_stopped)
				{
					float t = time - _startTime;

					if (t >= 0)
					{
						if (_stopTime > 0 && time > _stopTime)
						{
							_stopped = true;
							_dynamicLight.Stop();
						}
						else
						{
							if (!_played)
							{
								_played = true;
								_dynamicLight.Play();
							}

							if (!Application.isPlaying)
							{
								_dynamicLight.Sim(t);
							}
						}
					}
				}
			}
		}



		public bool IsPastStopTime(float time)
		{
			return _stopTime > 0 && time > _stopTime;
		}

		//private DynamicLightEventProperties		_properties;
		private DynamicPointLightInstance 		_dynamicLight;
		private float							_startTime;
		private float							_stopTime;
		private bool							_played;
		private bool							_stopped;
	}
}