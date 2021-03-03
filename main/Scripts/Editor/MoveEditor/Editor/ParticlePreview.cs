using UnityEngine;

namespace MoveEditor
{
	public class ParticlePreview
	{
		public void Init(ParticleEventProperties properties, Animator animator, float startTime, bool flipped, float stopTime, bool clear)
		{
			_properties 	= properties;
			//_flipped 		= flipped;
			_clear			= clear;
			//_animator 		= animator;
			_startTime 		= startTime;
			_stopTime 		= stopTime;
			_lastTime       = startTime;
			_played			= false;
			_stopped		= false;

			if (_particleSystem == null)
			{
				_particleSystem	= MoveUtils.InstantiateParticle(null,properties, animator, flipped, true);
			}

			if (properties._parent)
			{
				_attachTransform = _particleSystem.GetComponent<AttachTransform>();
				if(_attachTransform != null)
				{
					_attachTransform.UpdateManually = true;
				}
			}

			Reset();
		}

		public void SetAtHitPoint(Vector3 hitPoint)
		{
			if (_particleSystem != null && _properties != null)
			{
				AttachTransform.Detach(_particleSystem.gameObject);
				_particleSystem.transform.position = hitPoint;
			}
		}

		public void Reset()
		{
			_played = false;
			_stopped = false;

			if (_particleSystem != null)
            {
                _particleSystem.Stop(true);
				_particleSystem.Clear(true);
				_particleSystem.Simulate(0.0001f, true, true);
				_particleSystem.EnableEmission(true);

				foreach (UnityEngine.TrailRenderer trail in _particleSystem.GetComponentsInChildren<UnityEngine.TrailRenderer>())
				{
					TrailRendererHelper helper = trail.GetComponent<TrailRendererHelper>();
					if (helper == null)
					{
						helper = trail.gameObject.AddComponent<TrailRendererHelper>();
					}
					helper.Cleanup();
				}

				foreach (var anim in _particleSystem.GetComponentsInChildren<Animation>())
				{
					anim.Stop();
					anim.Play();
				}

				foreach (var anim in _particleSystem.GetComponentsInChildren<Animator>())
				{
					anim.Rebind();
				}
			}
		}
		
		public void Cleanup()
		{
			if (_particleSystem != null)
			{
				GameObject.DestroyImmediate(_particleSystem.gameObject);
			}
		}
		
		public void Update(float time)
		{
			float delta = time - _lastTime;
			//float lastTime = _lastTime;
			_lastTime = time;

			if (_particleSystem != null)
			{	
				// Keep updating attachment, even when we're stopped
				if (_attachTransform != null)
					_attachTransform.UpdateAttachment();

				if (!_stopped)
				{
					float t = time - _startTime;

					if (t > 0)
					{
						if (_stopTime > 0 && time > _stopTime)
                        {
                            _stopped = true;
							_particleSystem.Stop(true);
							_particleSystem.EnableEmission(false);

							if (_clear || !Application.isPlaying)
								_particleSystem.Clear(true);
						}
						else
						{
							if (Application.isPlaying)
							{
								if (!_played)
								{
									_particleSystem.Simulate(0.0001f, true, true);
									_particleSystem.Play(true);
									_played = true;
								}
							}
							else
							{
								_particleSystem.Simulate(delta, true, false, true);
								if (_particleSystem.particleCount > 0 || t > _particleSystem.duration)
									_played = true;

								foreach (var anim in _particleSystem.GetComponentsInChildren<Animation>())
								{
									anim.clip.SampleAnimation(anim.gameObject, time);
								}

								foreach (var anim in _particleSystem.GetComponentsInChildren<Animator>())
								{
									anim.Update(delta);
								}
							}
						}
					}
				}
			}
		}

		public bool IsFinished()
		{
			if (_played)
			{
				if (Application.isPlaying)
					return !_particleSystem.IsAlive(true);
				else
					return _particleSystem.particleCount <= 0;
			}

			return false;
		}
		
		public Transform GetParticleTransform()
		{
			if (_particleSystem != null)
				return _particleSystem.transform;
			
			return null;
		}

		public string Name
		{
			get
			{
				return _particleSystem != null ? _particleSystem.name : "MISSING PARTICLE";
			}
		}
		
		public ParticleEventProperties ParticleProperties { get { return _properties; } }
		
		private ParticleEventProperties	_properties;
		private AttachTransform			_attachTransform;
		private ParticleSystem 			_particleSystem;
		//private Animator				_animator;
		private float					_startTime;
		private float					_stopTime;
		private float                   _lastTime;
		//private bool					_flipped;
		private bool					_clear;
		private bool					_played;
		private bool					_stopped;
	}
}