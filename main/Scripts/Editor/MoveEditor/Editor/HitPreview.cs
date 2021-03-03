using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MoveEditor
{
    
    public class HitPreview
    {
        public class HitPreviewEvent
        {
            public void Cleanup()
            {

            }

            public void Reset()
            {
            }
        }

        public void Init(float startTime, HitEventInfo info, GameObject previewObject,GameObject hitObject, bool flipped)
        {
            if(hitObject ==null)
            {
                return;
            }

            _hitEventInfo = info;
            _previewCharacterObject = previewObject;
            _filpped = flipped;
            _hitObject = hitObject;
            _hitMono = _previewCharacterObject.GetComponent<HitMono>();
            if (_hitMono == null)
            {
                _hitMono = _previewCharacterObject.AddComponent<HitMono>();
            }

            _hitMono.EditorAwake();
            

            if (hitObject == null)
            {
                EB.Debug.LogError("Dont find previewHitObject");
                return;
            }

            _hitMono.targets = new Transform[1];
            _hitMono.targets[0] = hitObject.transform;
            
            _hitObjectMono = hitObject.GetComponent<HitMono>();
            if (_hitObjectMono == null)
            {
                _hitObjectMono = hitObject.AddComponent<HitMono>();
            }

            _hitObjectMono.EditorAwake();
            _hitMono.SpawnHit(_hitEventInfo._particleProperties, _hitEventInfo._audioProperties, startTime);
        }

        public void Reset()
        {
            _spawnEvents.Reset();
        }

        public void Update(float time)
        {
            float t = time;

            if (t >= 0)
            {
                if (_hitMono != null)
                {
                    _hitMono.CommonUpdate(t);
                }
                if (_hitObjectMono != null)
                {
                    _hitObjectMono.CommonUpdate(t);
                }
            }
        }

        public void Cleanup()
        {
            _played = false;

            if (_hitMono != null)
            {
                _hitMono.Cleanup();
            }
            if (_hitObjectMono != null)
            {
                _hitObjectMono.Cleanup();
            }
        }

        private ProjectileMono pmo;

        private Vector3 _startPosition;
        private HitEventInfo _hitEventInfo;
        private bool _played;
        private GameObject _previewCharacterObject;
        private GameObject _hitObject;
        private Animator _ownerAnimator;
        private Animator _animator;
        private bool _filpped;
        private HitMono _hitMono;
        private HitMono _hitObjectMono;
        public Vector3 targetPos;

       private HitPreviewEvent _spawnEvents = new HitPreviewEvent();
        //private ProjectilePreviewEvent			_hitEvents = new ProjectilePreviewEvent();
    }
}
