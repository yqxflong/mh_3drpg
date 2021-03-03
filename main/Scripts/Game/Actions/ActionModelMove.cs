//ActionModelMove
//模型移动
//Johny

using System;
using UnityEngine;

namespace Johny.Action
{
    public class ActionModelMove
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
        private Vector3 _finalPos;
        private bool _isLocal;
        private TimedSplineVector3 _splineMove = new TimedSplineVector3();

        ///during： 单位秒
        ///model:    运动本体
        public ActionModelMove(float delay, float during, GameObject model, Vector3 fromPos, Vector3 toPos, bool isLocal = true)
        {
            _cell = model;
            _delay = delay;
            _during = during;
            _finalPos = toPos;
            _isLocal = isLocal;
            if (_isLocal)
            {
                model.transform.localPosition = fromPos;
            }
            else
            {
                model.transform.position = fromPos;
            }

            //prepare spline
            _splineMove.Clear();
            _splineMove.addKey(0.0f, fromPos);
            _splineMove.addKey(during, _finalPos);
            _splineMove.CalculateGradient();

            //start timer
            _timerSeq = TimerManager.instance.AddTimer(0, 0, Update);
        }

        public void SetFinishHandler(System.Action<FinishStatus> handler)
        {
            _finishHandler = handler;
        }

        public void Stop(FinishStatus status = FinishStatus.Interrupt)
        {
            IsFinished = true;
            TimerManager.instance.RemoveTimerSafely(ref _timerSeq);
            _finishHandler?.Invoke(status);
        }

        private void Update(int seq)
        {
            if(IsFinished)
            {
                return;
            }

            float delta = Time.deltaTime;

            if(_delay > 0)
            {
                _delay -= delta;
                return;
            }
            else
            {
                _timer = Mathf.Min(_timer + delta, _during);
            }

            var pos = _splineMove.getGlobalFrame(_timer);
            if (_isLocal)
            {
                _cell.transform.localPosition = pos;
            }
            else
            {
                _cell.transform.position = pos;
            }

            if(_timer >= _during)
            {
                Stop(FinishStatus.Complete);
            }
        }
    }
}
