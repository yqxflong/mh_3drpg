using System;
using UnityEngine;

namespace Hotfix_LT.UI {
    public class LTPromotionStarItemController : DynamicMonoHotfix {
        private UISprite _sprite;
        private ParticleSystemUIComponent _particleSystemUIComponent;
        private Transform _transform;

        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            _transform = t;
            _sprite = t.GetComponent<UISprite>();
            _particleSystemUIComponent = t.GetComponent<ParticleSystemUIComponent>();

            Reset();
        }

        public void Set(bool isLight) {
            _sprite.color = isLight ? Color.white : new Color(0f, 0f, 0f, 0.5f);
        }

        public void Reset() {
            _particleSystemUIComponent.Stop();
            _sprite.color = new Color(0f, 0f, 0f, 0.5f);
        }

        public void Play() {
            _particleSystemUIComponent.Play();
            _sprite.color = Color.white;

            //旋转星星特效
            if (_transform.childCount > 0) {
                var ps = _transform.GetChild(_transform.childCount - 1).GetChild(0).GetComponent<ParticleSystem>();

                if (ps != null) {
                    var main = ps.main;
                    main.startRotation = Mathf.Deg2Rad * _transform.localEulerAngles.z * -1;
                }
            }
        }
    }
}
