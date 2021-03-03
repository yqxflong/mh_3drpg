//ActionCellUpAndDownLoop
//格子或格子物品循环上下移动
//Johny

using UnityEngine;

namespace Johny.Action
{
    public class ActionCellUpAndDownLoop
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

        #region loop prms
        private int _loopMinCnt, _loopMaxCnt, _loopIntervalCounter;
        private float _loopMinInterval, _loopMaxInterval, _loopInterval;
        #endregion

        #region 物理公式参数
        private Vector3 _upPos, _downPos;
        private float Vmin, Vmax, A, T;
        private bool _down2Up;
        #endregion

        ///during： from->to->from一次循环时间, 单位秒
        ///cell:    运动本体
        ///middle:  first 与 second的中间位置
        public ActionCellUpAndDownLoop(float delay, float during, GameObject cell, Vector3 upPos)
        {
            _cell = cell;
            _delay = delay;
            _during = during;
            _upPos = upPos;
            _downPos = cell.transform.localPosition;
            
            //准备参数
            T = during / 2;
            float h = (upPos - _downPos).magnitude;
            A = 2*h / Mathf.Pow(T, 2);
            Vmin = 0.0f;
            Vmax = Vmin + A * T;

            //start timer
            _timerSeq = TimerManager.instance.AddTimer(0, 0, Update);
        }

        public void SetLoopInterval(int minCnt, int maxCnt, float minInterval, float maxInterval)
        {
            _loopMinCnt = minCnt;
            _loopMaxCnt = maxCnt;
            _loopMinInterval = minInterval;
            _loopMaxInterval = maxInterval;
            GenNextInterval();
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

        private void GenNextInterval()
        {
            _loopIntervalCounter = Random.Range(_loopMinCnt, _loopMaxCnt + 1);
            _loopInterval = Random.Range(_loopMinInterval, _loopMaxInterval + 0.1f); 
        }

        private float GetCurDis(float t)
        {
            if(_down2Up)
            {
                return Vmax * t - 0.5f * A * Mathf.Pow(t, 2);
            }
            else
            {
                return Vmin * t + 0.5f * A * Mathf.Pow(t, 2);
            }
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
            

            if(_timer < 0)
            {
                return;
            }

            if(_timer <= T)
            {
                _down2Up = true;
                var pos = _downPos;
                pos.y += GetCurDis(_timer);
                _cell.transform.localPosition = pos;
            }
            else
            {     
                _down2Up = false;           
                var pos = _upPos;
                pos.y -= GetCurDis(_timer - T);
                _cell.transform.localPosition = pos;
            }

            if(_timer >= _during)
            {
                if(_loopIntervalCounter > 0)
                {
                    _loopIntervalCounter -= 1;
                    if(_loopIntervalCounter == 0)
                    {
                        GenNextInterval();
                        _timer = - _loopInterval;
                    }
                    else
                    {
                        _timer = 0.0f;
                    }
                }
                else
                {
                    _timer = 0.0f;
                }
            }
        }
    }
}
