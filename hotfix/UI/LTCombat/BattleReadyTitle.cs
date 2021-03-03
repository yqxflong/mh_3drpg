using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class BattleReadyTitle : DynamicMonoHotfix
    {

        public List<GameObject> SelectObjList;
        public List<GameObject> TitleLabelObjList;

        private eAttrTabType curInvType = eAttrTabType.None;
        private GameObject curBtnObj;
        public override void Awake()
        {
            SelectObjList = new List<GameObject>();
            TitleLabelObjList = new List<GameObject>();
            int center = mDMono.ObjectParamList.Count / 2;
            for (int i = 0; i < mDMono.ObjectParamList.Count; i++)
            {
                if (i < center)
                {
                    SelectObjList.Add(mDMono.ObjectParamList[i] as GameObject);
                }
                else
                {
                    TitleLabelObjList.Add(mDMono.ObjectParamList[i] as GameObject);
                }
            }
        }
        public override void Start()
        {
            if (curBtnObj == null)
            {
                curBtnObj = SelectObjList[0];
                OnTitleBtnClick(SelectObjList[0]);
            }
        }

        private void Show()
        {
            for (int i = 0; i < SelectObjList.Count; i++)
            {
                SelectObjList[i].SetActive(i == (int)curInvType);
                TitleLabelObjList[i].SetActive(i != (int)curInvType);
            }
        }

        public void OnTitleBtnClick(GameObject obj)
        {
            int index = SelectObjList.IndexOf(obj);
            if (index == (int)curInvType)
            {
                return;
            }
            if (obj.GetComponent<TweenScale>() != null)
            {
                obj.GetComponent<TweenScale>().ResetToBeginning();
                obj.GetComponent<TweenScale>().PlayForward();
            }
            curInvType = (eAttrTabType)index;
            curBtnObj = obj;
            Show();
        }

        public void OnFinishShow(GameObject obj)
        {
            UITweener[] ts = obj.GetComponents<UITweener>();

            if (ts == null)
            {
                return;
            }

            for (var i = 0; i < ts.Length; i++)
            {
                var t = ts[i];
                t.ResetToBeginning();
                t.PlayForward();
            }
        }
    }
}