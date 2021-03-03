using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTInventoryTitleBtnController : DynamicMonoHotfix
    {
        public enum LTInventoryTitle
        {
            None = -1,
            All,
            Consumables,
            Equipment,
            Partners,
            Materials
        }
    
        public List<GameObject> SelectObjList;
        public List<GameObject> TitleLabelObjList;
    
        private LTInventoryTitle curInvType = LTInventoryTitle.None;
        private GameObject curBtnObj;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;

            SelectObjList = new List<GameObject>();
            SelectObjList.Add(t.FindEx("BtnList/AllBtn/Sprite").gameObject);
            SelectObjList.Add(t.FindEx("BtnList/ConsumablesBtn/Sprite").gameObject);
            SelectObjList.Add(t.FindEx("BtnList/EquipmentBtn/Sprite").gameObject);
            SelectObjList.Add(t.FindEx("BtnList/PartnersBtn/Sprite").gameObject);
            SelectObjList.Add(t.FindEx("BtnList/MaterialsBtn/Sprite").gameObject);

            TitleLabelObjList = new List<GameObject>();
            TitleLabelObjList.Add(t.FindEx("BtnList/AllBtn/Sprite (2)").gameObject);
            TitleLabelObjList.Add(t.FindEx("BtnList/ConsumablesBtn/Sprite (2)").gameObject);
            TitleLabelObjList.Add(t.FindEx("BtnList/EquipmentBtn/Sprite (2)").gameObject);
            TitleLabelObjList.Add(t.FindEx("BtnList/PartnersBtn/Sprite (2)").gameObject);
            TitleLabelObjList.Add(t.FindEx("BtnList/MaterialsBtn/Sprite (2)").gameObject);

            t.GetComponent<UIButton>("BtnList/AllBtn").onClick.Add(new EventDelegate(() => OnTitleBtnClick(t.FindEx("BtnList/AllBtn/Sprite").gameObject)));
            t.GetComponent<UIButton>("BtnList/ConsumablesBtn").onClick.Add(new EventDelegate(() => OnTitleBtnClick(t.FindEx("BtnList/ConsumablesBtn/Sprite").gameObject)));
            t.GetComponent<UIButton>("BtnList/EquipmentBtn").onClick.Add(new EventDelegate(() => OnTitleBtnClick(t.FindEx("BtnList/EquipmentBtn/Sprite").gameObject)));
            t.GetComponent<UIButton>("BtnList/PartnersBtn").onClick.Add(new EventDelegate(() => OnTitleBtnClick(t.FindEx("BtnList/PartnersBtn/Sprite").gameObject)));
            t.GetComponent<UIButton>("BtnList/MaterialsBtn").onClick.Add(new EventDelegate(() => OnTitleBtnClick(t.FindEx("BtnList/MaterialsBtn/Sprite").gameObject)));

            t.GetComponent<TweenScale>("BtnList/AllBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("BtnList/AllBtn/Sprite/Sprite").gameObject)));
            t.GetComponent<TweenScale>("BtnList/ConsumablesBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("BtnList/ConsumablesBtn/Sprite/Sprite").gameObject)));
            t.GetComponent<TweenScale>("BtnList/EquipmentBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("BtnList/EquipmentBtn/Sprite/Sprite").gameObject)));
            t.GetComponent<TweenScale>("BtnList/PartnersBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("BtnList/PartnersBtn/Sprite/Sprite").gameObject)));
            t.GetComponent<TweenScale>("BtnList/MaterialsBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("BtnList/MaterialsBtn/Sprite/Sprite").gameObject)));
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

            curInvType = (LTInventoryTitle)index;
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
