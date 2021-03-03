//ActionGeneralParticle
//通用粒子
//Johny

using UnityEngine;

namespace Johny.Action
{
    public class ActionGeneralParticle
    {
        public enum FinishStatus
        {
            Complete = 0,
            Interrupt = 1,
        }
        public bool IsFinished{get; private set;}
        private System.Action<FinishStatus> _finishHandler;
        private int _timerSeq;
        private ParticleSystem _effect;
        private bool _hasPlayed;
        private float _delay;
        private float _during;
        private float _timer;
        //依据特效本身是否是loop的,loop的需要手动stop
        private bool _isLoop;


        ///cell:  运动本体
        ///effectName: 特效预制体名字
        ///offset: 特效local偏移量
        public ActionGeneralParticle(float delay, GameObject parent, string effectName, Vector3 worldPos, Vector3 offset)
        {
            _delay = delay;
            //start timer
            _timer = -_delay;
            _timerSeq = TimerManager.instance.AddTimer(0, 0, Update);

            //load effect
            EB.Assets.LoadAsyncAndInit<GameObject>(effectName, (assetName, go, succ)=>
            {
                if(succ)
                {
                    go.transform.position = worldPos;
                    go.transform.localPosition = offset;
                    _effect = go.GetComponent<ParticleSystem>();
                    if(_effect == null)
                    {
                        EB.Debug.LogError("ActionGeneralParticle===>_effect == null, {0}", assetName);
                        Stop();
                        return;
                    }
                    _effect.Stop();
                    _isLoop = _effect.main.loop;
                    if(_during == 0.0f)
                    {
                        _during = _effect.main.duration;
                    }
                }
                else
                {
                    EB.Debug.LogError("ActionGeneralParticle===>Load Failed! {0}", assetName);
                    Stop();
                }
            }, parent, parent, true);
        }

        //设置强制结束时间
        public void SetForceFinishDuring(float during)
        {
            _during = during;
        }

        public void SetFinishHandler(System.Action<FinishStatus> handler)
        {
            _finishHandler = handler;
        }

        public void Stop(FinishStatus status = FinishStatus.Interrupt)
        {
            if(!IsFinished)
            {
                if(_effect)
                {
                    GameObject.Destroy(_effect.gameObject); _effect = null;
                }
                
                IsFinished = true;
                TimerManager.instance.RemoveTimerSafely(ref _timerSeq);
                _finishHandler?.Invoke(status);
            }
        }
        
        private void Update(int seq)
        {
            if(IsFinished)
            {
                return;
            }

            float delta = Time.deltaTime;
            _timer += delta;

            //停止情况：
            //1. 非loop但超时
            //2. loop但特效被外面销毁了
            //3. 非loop未超时，但特效被外面销毁了
            bool isTimeOut = !_isLoop && _timer > _during;
            if ((isTimeOut || _effect == null) && _hasPlayed)
            {
                Stop(FinishStatus.Complete);
                return;
            }

            if (_timer < 0)
            {
                return;
            }

            if (!_hasPlayed && _effect != null)
            {
                _effect.Play();
                _hasPlayed = true;
            }
        }
    }
}
