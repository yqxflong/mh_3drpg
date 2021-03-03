using System;
//ActionCellBornMove
//格子出生移动
//Johny

using UnityEngine;

namespace Johny.Action
{
    public class ActionCellBornMove
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
        private Vector3 _middlePos;
        private Vector3 _finalPos;
        private bool _isLocal;
        private TimedSplineVector3 _splineMove = new TimedSplineVector3();

        ///during： 单位秒
        ///cell:    运动本体
        public ActionCellBornMove(float delay, float during, GameObject cell, Vector3 fromPos, Vector3 middlePos, Vector3 finalPos, bool isLocal = true)
        {
            _cell = cell;
            _delay = delay;
            _during = during;
            _middlePos = middlePos;
            _finalPos = finalPos;
            _isLocal = isLocal;
            if (_isLocal)
            {
                cell.transform.localPosition = fromPos;
            }
            else
            {
                cell.transform.position = fromPos;
            }

            //prepare spline
            var middleDis = _middlePos - fromPos;
            _splineMove.Clear();
            _splineMove.addKey(0.0f, fromPos);
            _splineMove.addKey(during * 0.1f, fromPos + middleDis * 0.1f);
            _splineMove.addKey(during * 0.3f, fromPos + middleDis * 0.4f);
            _splineMove.addKey(during * 0.7f, _middlePos);
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
