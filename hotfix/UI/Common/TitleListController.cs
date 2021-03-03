using System;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// Title显示控制器，具体参考商城或日常,无需再在其他脚本中调用OnTitleBtnClick
    /// </summary>
    public class TitleListController : DynamicMonoHotfix {
        public GameObject[] SelectObjList;
        public GameObject[] TitleLabelObjList;
        public bool IsNotPlayOnFirst = false;

        private int curIndex = -1;
        private GameObject curBtnObj;


        public override void Awake() {
            base.Awake();

            if (mDMono.BoolParamList != null) {
                var count = mDMono.BoolParamList.Count;

                if (count > 0) {
                    IsNotPlayOnFirst = mDMono.BoolParamList[0];
                }
            }

            if (mDMono.ObjectParamList != null) {
                var count = mDMono.ObjectParamList.Count;
                SelectObjList = new GameObject[count];
                TitleLabelObjList = new GameObject[count];

                for (int i = 0; i < count; i++) {
                    var obj = mDMono.ObjectParamList[i];

                    if (obj != null) {
                        var go = (GameObject)obj;
                        var goSprite = go.FindEx("Sprite");
                        TitleLabelObjList[i] = go.FindEx("Label");
                        SelectObjList[i] = goSprite;

                        go.GetComponentEx<UIButton>().onClick.Add(new EventDelegate(() => OnTitleBtnClick(goSprite)));
                    }
                }
            }
        }

        public override void Start() {
            if (curBtnObj == null) {
                curBtnObj = SelectObjList[0];
                OnTitleBtnClick(SelectObjList[0]);
            }
        }

        private void Show() {
            for (int i = 0; i < SelectObjList.Length; i++) {
                SelectObjList[i].CustomSetActive(i == curIndex);
                TitleLabelObjList[i].CustomSetActive(i != curIndex);
            }
        }

        public void OnTitleBtnClick(GameObject obj) {
            int index = Array.IndexOf(SelectObjList, obj);

            if (index == curIndex) {
                return;
            }

            if (!IsNotPlayOnFirst && obj.GetComponent<TweenScale>() != null) {
                obj.GetComponent<TweenScale>().ResetToBeginning();
                obj.GetComponent<TweenScale>().PlayForward();
            }

            IsNotPlayOnFirst = false;
            curIndex = index;
            curBtnObj = obj;
            Show();
        }

        public void SetTitleBtn(int index) {
            if (index >= 0 && index < SelectObjList.Length) {
                curIndex = index;
                curBtnObj = SelectObjList[index]; 
            }

            Show();
        }

        public void OnFinishShow(GameObject obj) {
            UITweener[] ts = obj.GetComponents<UITweener>();
            for (int i = 0; i < ts.Length; i++)
            {
                ts[i].ResetToBeginning();
                ts[i].PlayForward();
            }
        }
    }
}