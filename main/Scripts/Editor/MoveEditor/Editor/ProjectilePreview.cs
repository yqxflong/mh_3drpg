using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MoveEditor
{
    //	public class ProjectilePreview
    //	{
    //		public class ProjectilePreviewEvent
    //		{
    //			public List<ParticlePreview> _particles = new List<ParticlePreview>();
    //			public List<TrailPreview> _trails = new List<TrailPreview>();
    //			public List<DynamicLightPreview> _lights = new List<DynamicLightPreview>();

    //			public void Cleanup()
    //			{

    //				for( int i = 0; i < _particles.Count; ++i )
    //				{
    //					ParticlePreview preview = _particles[i];
    //					preview.Cleanup();
    //				}
    //				_particles.Clear();

    //				for( int i = 0; i < _trails.Count; ++i )
    //				{
    //					TrailPreview preview = _trails[i];
    //					preview.Cleanup();
    //				}
    //				_trails.Clear();

    //				for( int i = 0; i < _lights.Count; ++i )
    //				{
    //					DynamicLightPreview preview = _lights[i];
    //					preview.Cleanup();
    //				}
    //				_lights.Clear();

    //			}

    //			public void Reset()
    //			{

    //				for( int i = 0; i < _particles.Count; ++i )
    //				{
    //					ParticlePreview preview = _particles[i];
    //					preview.Reset();
    //				}

    //				for( int i = 0; i < _trails.Count; ++i )
    //				{
    //					TrailPreview preview = _trails[i];
    //					preview.Reset();
    //				}

    //				for( int i = 0; i < _lights.Count; ++i )
    //				{
    //					DynamicLightPreview preview = _lights[i];
    //					preview.Reset();
    //				}

    //			}
    //		}

    //		public void Init( float startTime, MoveEditor.ProjectileEventInfo info, GameObject previewObject, bool flipped )
    //		{
    //			_startTime = startTime;
    //			_projectileEventInfo = info;
    //			_previewCharacterObject = previewObject;
    //			_filpped = flipped;
    //		}

    //		public void Reset()
    //		{
    //			_spawnEvents.Reset();
    //		}

    //		private void SpawnProjectile(float startTime)
    //		{
    //            //by pj 重构弹道飞行物 编辑器使用编辑器时钟 运行模式使用自己Update


    //			//老的
    //            _projectile = GameObject.Instantiate( _projectileEventInfo._projectileProperties._prefab ) as GameObject;
    //			_controller = _projectile.GetComponent<ProjectileController>();
    //			Animator ownerAnimator = _previewCharacterObject.GetComponent<Animator>();
    //            Debug.Log(_previewCharacterObject.name);
    //			_controller.Activate( ownerAnimator, _projectileEventInfo, null, _filpped );

    //			_startPosition = _projectile.transform.position;
    //			_animator = _projectile.GetComponent<Animator>();

    //			ProjectileController.ProjectileEvent projectileEvent = _controller._onSpawnEvents;

    //			for( int j = 0; j < projectileEvent._particles.Count; ++j )
    //			{
    //				MoveEditor.ParticleEventInfo particleEventInfo = projectileEvent._particles[j];

    //				ParticlePreview particle = new ParticlePreview();
    //				particle.Init(particleEventInfo._particleProperties, _animator, startTime, _filpped, -1, false);

    //				_spawnEvents._particles.Add(particle);
    //			}

    //			for( int j = 0; j < projectileEvent._trails.Count; ++j )
    //			{
    //				MoveEditor.TrailRendererEventInfo trailEventInfo = projectileEvent._trails[j];

    //				TrailPreview trail = new TrailPreview();
    //				trail.Init(trailEventInfo._trailRendererProperties, _animator, startTime, _filpped, false);

    //				_spawnEvents._trails.Add(trail);
    //			}

    //			for( int j = 0; j < projectileEvent._dynamicLights.Count; ++j )
    //			{
    //				MoveEditor.DynamicLightEventInfo dynamicLights = projectileEvent._dynamicLights[j];

    //				DynamicLightPreview light = new DynamicLightPreview();
    //				light.Init(dynamicLights._dynamicLightProperties, _animator, startTime, -1);

    //				_spawnEvents._lights.Add(light);
    //			}

    //		}

    //		public void Update(float time)
    //		{
    //			float t = time - _startTime;

    //			if (t >= 0)
    //			{
    //				//if (Application.isPlaying)
    //				if(t > 1)
    //				{
    //					//int i = 0;

    //				}
    //				if (!_played)
    //				{
    //					_played = true;

    //					SpawnProjectile(t);
    //				}
    //                Vector3 dir = /*(targetPos - _previewCharacterObject.transform.position ).normalized; */ _filpped ? Vector3.left : Vector3.right;
    //				Vector3 displacement = dir * _projectileEventInfo._projectileProperties._initialVelocity * t;
    //				ProjectileController controller = _projectile.GetComponent<ProjectileController>();
    //				float y = controller._motionCurve.Evaluate(System.Math.Min(t, 1.0f));
    //				Vector3 new_pos = _startPosition + displacement;
    //				new_pos.y += y;
    //				Quaternion rotation = Quaternion.LookRotation(new_pos - _projectile.transform.position);
    //				_projectile.transform.position = new_pos;
    //				_projectile.transform.rotation = rotation;


    //				// simulate all the effects and shit
    //				for( int i = 0; i < _spawnEvents._particles.Count; ++i )
    //				{
    //					ParticlePreview preview = _spawnEvents._particles[i];
    //					preview.Update(t);
    //				}

    //				for( int i = 0; i < _spawnEvents._trails.Count; ++i )
    //				{
    //					TrailPreview preview = _spawnEvents._trails[i];
    //					preview.Update(t);
    //				}

    //				for( int i = 0; i < _spawnEvents._lights.Count; ++i )
    //				{
    //					DynamicLightPreview preview = _spawnEvents._lights[i];
    //					preview.Update(t);
    //				}
    //			}
    //		}

    //		public void Cleanup()
    //		{
    //			if (_projectile != null)
    //			{
    //				GameObject.DestroyImmediate(_projectile);

    //				_spawnEvents.Cleanup();
    //			}
    //		}

    //		private Vector3 						_startPosition;
    //		private float 							_startTime;
    //		private MoveEditor.ProjectileEventInfo 	_projectileEventInfo;
    //		private bool							_played;
    //		private ProjectileController			_controller;
    //		private GameObject						_projectile;
    //		private GameObject						_previewCharacterObject;
    //		private Animator						_ownerAnimator;
    //		private Animator						_animator; 
    //		private bool							_filpped;

    //        public Vector3                          targetPos;

    //		private ProjectilePreviewEvent			_spawnEvents = new ProjectilePreviewEvent();
    //		//private ProjectilePreviewEvent			_hitEvents = new ProjectilePreviewEvent();
    //	}

    public class ProjectilePreview
    {
        public class ProjectilePreviewEvent
        {
            public void Cleanup()
            {

            }

            public void Reset()
            {
            }
        }

        public void Init(float startTime, MoveEditor.ProjectileEventInfo info, GameObject previewObject, GameObject previewHitObject, bool flipped)
        {
            _startTime = startTime;
            _projectileEventInfo = info;
            _previewCharacterObject = previewObject;
            _filpped = flipped;
            _previewHitObject = previewHitObject;
        }

        public void Reset()
        {
            _spawnEvents.Reset();
            if (pmo != null)
            {
                pmo.Reset();
            }
        }
        private void SpawnProjectile(float startTime)
        {
            //by pj 重构弹道飞行物 编辑器使用编辑器时钟 运行模式使用自己Update
            if (_projectileEventInfo._projectileProperties._prefab == null)
            {
                _projectile = new GameObject("ProjectileMono");
                _projectile.AddComponent<ProjectileMono>();
            }
            else
            {
                _projectile = GameObject.Instantiate(_projectileEventInfo._projectileProperties._prefab) as GameObject;
            }
            pmo = _projectile.GetComponent<ProjectileMono>();
            if (_projectileEventInfo._projectileProperties._changeEffect != null)
            {
                pmo.effectPrefab =_projectileEventInfo._projectileProperties._changeEffect;
            }
            pmo.characterTran = _previewCharacterObject.transform;
            ProjectileEventProperties props =  _projectileEventInfo._projectileProperties;
            if(_previewHitObject!=null) //编辑演示模式使用模拟受击对象
            {
                pmo.mainTarget = _previewHitObject.transform;
                pmo.targets = new Transform[1];
                pmo.targets[0] = pmo.mainTarget;
            }
            pmo.Spawn(_previewCharacterObject.transform, props._spawnOffset,props._spawnAttachment,props._spawnAttachmentPath,props._reattachment, props._reattachmentPath,
                props._initialVelocity,props._flyTime,props._fadeOutTime, props._isTarget,props._isLookAtTarget,props._isOnly);
            pmo.Init();
        }

        public void Update(float time)
        {
            float t = time - _startTime;

            if (t >= 0)
            {
                if(!_played)
                {
                    _played = true;
                    SpawnProjectile(t);
                }
                pmo.CommonUpdate(t);
            }
        }

        public void Cleanup()
        {
            _played = false;
            if (_projectile != null)
            {
                pmo.Stop();
                GameObject.DestroyImmediate(_projectile);
                _spawnEvents.Cleanup();
            }
        }

        private ProjectileMono pmo;

        private Vector3 _startPosition;
        private float _startTime;
        private MoveEditor.ProjectileEventInfo _projectileEventInfo;
        private bool _played;
        private ProjectileController _controller;
        private GameObject _projectile;
        private GameObject _previewCharacterObject;
        private GameObject _previewHitObject;
        private Animator _ownerAnimator;
        private Animator _animator;
        private bool _filpped;

        public Vector3 targetPos;

        private ProjectilePreviewEvent _spawnEvents = new ProjectilePreviewEvent();
        //private ProjectilePreviewEvent			_hitEvents = new ProjectilePreviewEvent();
    }
}
