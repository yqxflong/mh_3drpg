//ActionAlphaChange
//alpha变化动作
//Johny

using UnityEngine;

namespace Johny.Action
{
    public class ActionAlphaChange
    {
        public enum FinishStatus
        {
            Complete = 0,
            Interrupt = 1,
            LoopOnce = 2,
        }

        public bool IsFinished{get; private set;}

        ///正->反 （loop）
        public bool ForwardAndReverseLoop{get; set;}

        private System.Action<FinishStatus> _finishHandler;

        private int _timerSeq;
        private SpriteRenderer _cell;
        private float _delay;
        private float _during;
        private float _timer;
        private float _alphaFrom, _alphaTo;

        private bool _isForward;

        private TimedSplineFloat _splineAlpha = new TimedSplineFloat();

        ///during： 单位秒
        ///cell:    运动本体
        ///from: 
        ///to: 
        public ActionAlphaChange(float delay, float during, SpriteRenderer cell, float from, float to)
        {
            _cell = cell;
            _delay = delay;
            _during = during;
            _alphaFrom = from;
            _alphaTo = to;

            var oriColor = cell.color;
            oriColor.a = from;
            cell.color = oriColor;

            _isForward = true;
            Prepare(from, to);
        }

        private void Prepare(float from, float to)
        {
            IsFinished = false;

            //prepare spline
            _splineAlpha.Clear();
            _splineAlpha.addKey(0.0f, from);
            _splineAlpha.addKey(_during, to);
            _splineAlpha.CalculateGradient();

            //start timer
            _timer = - _delay;
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
                if(status != FinishStatus.LoopOnce)
                {
                    _finishHandler?.Invoke(status);
                }
            }
        }

        private void Reverse()
        {
            Stop(FinishStatus.LoopOnce);
            _isForward = !_isForward;
            float from = _isForward? _alphaFrom : _alphaTo;
            float to = _isForward? _alphaTo : _alphaFrom;
            Prepare(from, to);
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
            
            var color = _cell.color;
            color.a = _splineAlpha.getGlobalFrame(_timer);
            _cell.color = color;

            if(_timer >= _during)
            {
                if(ForwardAndReverseLoop)
                {
                    Reverse();
                }
                else
                {
                    Stop(FinishStatus.Complete);
                }
            }
        }
    }
}
