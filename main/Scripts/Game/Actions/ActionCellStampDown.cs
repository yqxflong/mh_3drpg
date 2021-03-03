//ActionCellStampDown
//格子踩下弹起
//Johny

using UnityEngine;

namespace Johny.Action
{
    public class ActionCellStampDown
    {
        public enum FinishStatus
        {
            Complete = 0,
            Interrupt = 1,
        }

        public bool IsFinished{get; private set;}

        private System.Action<FinishStatus> _finishHandler;

        private int _timerSeq;
        private GameObject _cell;
        private float _delay;
        private float _during;
        private float _timer;
        private Vector3 _fromPos;
        private Vector3 _toPos;

        private TimedSplineVector3 _splineMove = new TimedSplineVector3();

        ///during： from->to->from一次循环时间, 单位秒
        ///cell:    运动本体
        public ActionCellStampDown(float delay, float during, GameObject cell, Vector3 from, Vector3 to)
        {
            _cell = cell;
            _delay = delay;
            _during = during;
            _fromPos = from;
            _toPos = to;

            //prepare spline
            _splineMove.Clear();
            _splineMove.addKey(0.0f, from);
            _splineMove.addKey(during * 0.3f, to);
            _splineMove.addKey(during, from);
            _splineMove.CalculateGradient();

            
            //reset pos
            cell.transform.localPosition = from;
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

        public void Restart()
        {
            Stop();
            IsFinished = false;
            _cell.transform.localPosition = _fromPos;
            _timer = -_delay;
            _timerSeq = TimerManager.instance.AddTimer(0, 0, Update);
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
            
            _cell.transform.localPosition = _splineMove.getGlobalFrame(_timer);
            if(_timer >= _during)
            {
                Stop(FinishStatus.Complete);
            }
        }
    }
}
