using UnityEngine;
using System.Collections.Generic;
using System;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class LTCombatMoveIconProgress : DynamicMonoHotfix, IHotfixUpdate
    {
        public List<GameObject> listGO;
        private bool _isSavePrimal;
        private List<Vector3> _listPrimalPos = new List<Vector3>();
        private List<Vector3> _listPrimalScale = new List<Vector3>();
        private List<Johny.TimedSplineVector3> _johnySplineScaleTo, _johnySplineMoveTo;
        private float _johnySpline_during = 0.0f, _johnySpline_timer = 0.0f;
        public bool isMove;
        public Action onITweenEnd;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;

            listGO = new List<GameObject>
            {
                t.gameObject,
                t.parent.FindEx("Three").gameObject,
                t.parent.FindEx("Ready").gameObject,
                t.parent.FindEx("Current").gameObject,
                t.parent.FindEx("Back").gameObject
            };

            isMove = false;

            _johnySplineScaleTo = new List<Johny.TimedSplineVector3>{
                new Johny.TimedSplineVector3(),
                new Johny.TimedSplineVector3(),
                new Johny.TimedSplineVector3(),
                new Johny.TimedSplineVector3(),
                new Johny.TimedSplineVector3()
            };

            _johnySplineMoveTo = new List<Johny.TimedSplineVector3>{
                new Johny.TimedSplineVector3(),
                new Johny.TimedSplineVector3(),
                new Johny.TimedSplineVector3(),
                new Johny.TimedSplineVector3(),
                new Johny.TimedSplineVector3()
            };
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public void Move(float moveTime=1)
        {
            #region 记录上一次pos和scale
            if(!_isSavePrimal)
            {
                _isSavePrimal = true;
                _listPrimalPos.Clear();
                _listPrimalScale.Clear();
                for (int i = 0; i < listGO.Count; i++)
                {
                    _listPrimalPos.Add(listGO[i].transform.position);
                    _listPrimalScale.Add(listGO[i].transform.localScale);
                }
            }
            #endregion

            #region JohnySpline
            _johnySpline_timer = 0.0f;
            _johnySpline_during = moveTime;
            for (int i = listGO.Count - 1; i > 0; i--)
            {
                var go = listGO[i];
                go.transform.position = _listPrimalPos[i - 1];
                go.transform.localScale = _listPrimalScale[i - 1];
                //prepare moveto
                _johnySplineMoveTo[i].Clear();
                _johnySplineMoveTo[i].addKey(0.0f, _listPrimalPos[i - 1]);
                _johnySplineMoveTo[i].addKey(_johnySpline_during, _listPrimalPos[i]);
                _johnySplineMoveTo[i].CalculateGradient();
                //prepare scaleto
                _johnySplineScaleTo[i].Clear();
                _johnySplineScaleTo[i].addKey(0.0f, _listPrimalScale[i - 1]);
                _johnySplineScaleTo[i].addKey(_johnySpline_during, _listPrimalScale[i]);
                _johnySplineScaleTo[i].CalculateGradient();
            }
            #endregion
            
            isMove = true;
        }

        public void Update()
        {
            if (!isMove)
            {
                return;
            }

            if (_johnySpline_timer < _johnySpline_during)
            {
                _johnySpline_timer += Time.deltaTime;

                if (listGO != null)
                {
                    for (int i = listGO.Count - 1; i > 0; i--)
                    {
                        var go = listGO[i];

                        if (go == null)
                        {
                            continue;
                        }

                        if (_johnySplineScaleTo[i] != null)
                        { //check scaleto
                            var scaleTo = _johnySplineScaleTo[i].getGlobalFrame(Mathf.Clamp(_johnySpline_timer, 0.0f, _johnySpline_during));
                            go.transform.localScale = scaleTo;
                        }

                        if (_johnySplineMoveTo[i] != null)
                        {
                            //check moveto
                            var moveTo = _johnySplineMoveTo[i].getGlobalFrame(Mathf.Clamp(_johnySpline_timer, 0.0f, _johnySpline_during));
                            go.transform.position = moveTo;
                        }
                    }
                }
            }

            if(_johnySpline_timer > _johnySpline_during){
                isMove = false;
                onITweenEnd?.Invoke();
            }
        }
    }
}
