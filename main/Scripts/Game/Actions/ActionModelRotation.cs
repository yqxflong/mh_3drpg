//ActionModelRotation
//模型旋转
//Johny

using UnityEngine;

namespace Johny.Action
{
    public class ActionModelRotation
    {
        public enum FinishStatus
        {
            Complete = 0,
            Interrupt = 1,
        }

        public bool IsFinished{get; private set;}

        private System.Action<FinishStatus> _finishHandler;

        private int _timerSeq;
        private GameObject _model;
        private float _delay;
        private float _during;
        private float _timer;
        private Vector3 _from;
        private Vector3 _to;

        private TimedSplineVector3 _splineRotation = new TimedSplineVector3();


        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="delay">延时</param>
        /// <param name="during">执行时间</param>
        /// <param name="model">本体</param>
        /// <param name="from">localRotation</param>
        /// <param name="to">localRotation</param>
        public ActionModelRotation(float delay, float during, GameObject model, Vector3 from, Vector3 to)
        {
            _model = model;
            _delay = delay;
            _during = during;
            _from = from;
            _to = to;

            Prepare();
        }

        private const float _halfPI = 0.5f * Mathf.PI;
        private void Prepare()
        {
            IsFinished = false;

            #region 找出最小角度(_from和_to中的项不能大于360，赋值给Unity后，会标准化by360,所以不用担心)
            _to.x += Mathf.Sin((int)((_from.x - _to.x) / 180.0f) * _halfPI) * 360.0f;
            _to.y += Mathf.Sin((int)((_from.y - _to.y) / 180.0f) * _halfPI) * 360.0f;
            _to.z += Mathf.Sin((int)((_from.z - _to.z) / 180.0f) * _halfPI) * 360.0f;
            #endregion

            //prepare spline
            var fromVec = new Vector3(_from.x, _from.y, _from.z);
            var toVec = new Vector3(_to.x, _to.y, _to.z);
            _splineRotation.Clear();
            _splineRotation.addKey(0.0f, fromVec);
            _splineRotation.addKey(_during, toVec);
            _splineRotation.CalculateGradient();


            //start timer
            _timer = -_delay;
            _timerSeq = TimerManager.instance.AddTimer(0, 0, Update);
        }

        public void SetFinishHandler(System.Action<FinishStatus> handler)
        {
            _finishHandler = handler;
        }

        public void Stop(FinishStatus status = FinishStatus.Interrupt)
        {
            if(!IsFinished)
            {
                IsFinished = true;
                TimerManager.instance.RemoveTimerSafely(ref _timerSeq);
                _finishHandler?.Invoke(status);
            }
        }

        /// <summary>
        /// 重新开始,可赋予新的from和to
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Restart(Vector3 from, Vector3 to)
        {
            Stop();
            _from = from;
            _to = to;
            Prepare();
        }
        
        private void Update(int seq)
        {
            if(IsFinished)
            {
                return;
            }

            float delta = Time.deltaTime;
            _timer = Mathf.Min(_timer + delta, _during);
            if(_timer < 0)
            {
                return;
            }
            
            var curEuler = _splineRotation.getGlobalFrame(_timer);
            _model.transform.eulerAngles = curEuler;

            if(_timer >= _during)
            {
                Stop(FinishStatus.Complete);
            }
        }
    }
}
