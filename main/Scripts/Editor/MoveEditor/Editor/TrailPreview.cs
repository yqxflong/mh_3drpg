using UnityEngine;

namespace MoveEditor
{
	public class TrailPreview
	{
		public void Init(TrailRendererEventProperties properties, Animator animator, float startTime, bool flipped, bool isCrit)
		{
			//_properties 	= properties;
			_startTime 		= startTime;
			_animClip 		= properties._rigAnimClip;
			_trailRenderer 	= MoveUtils.InstantiateTrailInstance(properties, animator, flipped, isCrit, true).GetComponent<TrailRendererInstance>();
			_animRoot		= EB.Util.GetObjectExactMatch(_trailRenderer.gameObject, "anim_root");
			_played 		= false;

			if (properties._parent)
			{
				_attachTransform = _trailRenderer.GetComponent<AttachTransform>();
			}
		}

		public void Reset()
		{
			_played = false;

			if (_trailRenderer != null)
			{
				if (Application.isPlaying)
					_trailRenderer.Stop();
				else
					_trailRenderer.ClearSimulate();
			}
		}

		public void Cleanup()
		{
			if (_trailRenderer != null)
			{
				if (Application.isPlaying)
					_trailRenderer.Stop();
				else
					_trailRenderer.ClearSimulate();

				GameObject.DestroyImmediate(_trailRenderer.gameObject);
			}
		}
		
		public void Update(float time)
		{
			if (_trailRenderer != null)
			{	
				float t = time - _startTime;

				if (_attachTransform != null)
					_attachTransform.UpdateAttachment();

				if (t >= 0)
				{
					if (_animRoot != null && _animRoot.GetComponent<Animation>() != null)
					{
						if(_animClip != null)
						{
							_animClip.SampleAnimation(_animRoot, t);
						}
						else
						{
							_animRoot.GetComponent<Animation>().clip.SampleAnimation(_animRoot, t);
						}
					}

					if (Application.isPlaying)
					{
						if (!_played)
						{
							_played = true;
							_trailRenderer.Play();
						}
					}
					else
					{
						_trailRenderer.Simulate(t);
					}
				}
			}
		}

		//private TrailRendererEventProperties	_properties;
		private TrailRendererInstance 			_trailRenderer;
		private GameObject						_animRoot;
		private float							_startTime;
		private AnimationClip					_animClip;
		private bool							_played;
		AttachTransform 						_attachTransform;
	}
}