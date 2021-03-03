using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    public class UIDynamicShowItem : DynamicMonoHotfix
    {
        public Transform item;
        public UIGrid uiGrid;
        public UIScrollView uiScrollView;
        public WaitForSeconds deltaTime =new WaitForSeconds ( 0.1f);
        public bool needItemVoice = false;
        public Action OnShowFinished;

        private List<LTShowItem> ltShowItems;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                int count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0])
                {
                    item = ((GameObject)mDMono.ObjectParamList[0]).transform;
                }
                if (count > 1 && mDMono.ObjectParamList[1])
                {
                    uiGrid = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UIGrid>();
                }
                if (count > 2 && mDMono.ObjectParamList[2])
                {
                    uiScrollView = ((GameObject)mDMono.ObjectParamList[2]).GetComponentEx<UIScrollView>();
                }
            }
            
            if (mDMono.BoolParamList != null && mDMono.BoolParamList.Count > 0)
            {
                needItemVoice = mDMono.BoolParamList[0];
            }
        }

        public override void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void Clear()
        {
            if (ltShowItems == null)
            {
                ltShowItems = new List<LTShowItem>();
            }
            else
            {
                for (int i = 0; i < ltShowItems.Count; i++)
                {
                    GameObject.Destroy(ltShowItems[i].mDMono.transform.parent.gameObject);
                }
                ltShowItems.Clear();
            }
        }

        public void ShowItems(List<LTShowItemData> data)
        {
            CreateItems(data);
            StartCoroutine(ShowItems());
        }

        public void ShowItemsImediatly(List<LTShowItemData> data)
        {
            CreateItems(data);
            ShowAllItem();
        }

        IEnumerator ShowItems()
        {
            if (ltShowItems == null)
            {
                EB.Debug.LogError("UIDynamicShowItem:_items == null");

                if (OnShowFinished != null)
                {
                    OnShowFinished();
                }

                yield break;
            }

            for (int i = 0; i < ltShowItems.Count; ++i)
            {
                yield return deltaTime;

                if (needItemVoice)
                {
                    FusionAudio.PostEvent("UI/New/BaoXiangWuPin", true);
                }

                ltShowItems[i].mDMono.gameObject.CustomSetActive(true);
                UITweener[] tws = ltShowItems[i].mDMono.transform.parent.GetComponents<UITweener>();

                if (tws != null)
                {
                    for (var j = 0; j < tws.Length; j++)
                    {
                        var tw = tws[j];
                        tw.tweenFactor = 0;
                        tw.PlayForward();
                    }
                }

                uiGrid.Reposition();

                if (ShowRewardScrollFunc.shouldScroll != null)
                {
                    ShowRewardScrollFunc.shouldScroll((i + 1) / 5);
                }
            }

            yield return new WaitForSeconds(0.3f);

            if (OnShowFinished != null)
            {
                OnShowFinished();
            }

            yield break;
        }

        private void ShowAllItem()
        {
            if (ltShowItems == null)
            {
                return;
            }

            for (int i = 0; i < ltShowItems.Count; i++)
            {
                ltShowItems[i].mDMono.gameObject.CustomSetActive(true);
            }

            uiGrid.Reposition();
        }

        void CreateItems(List<LTShowItemData> data)
        {
            if (data == null)
            {
                return;
            }

            for (int i = 0; i < data.Count; i++)
            {
                //FusionAudio.PostEvent("UI/New/BaoXiangWuPin", true);
                var t = GameObject.Instantiate(item);
                t.parent = uiGrid.transform;
                t.localPosition = Vector3.zero;
                t.localScale = Vector3.one;
                t.gameObject.CustomSetActive(true);

                var reward = t.GetChild(0).GetMonoILRComponent<LTShowItem>();
                reward.mDMono.transform.parent.name = i.ToString();
                reward.LTItemData = data[i];
                reward.Name.color = new Color(1, 1, 1);
                reward.Name.applyGradient = false;
                ltShowItems.Add(reward);
            }

            if (data.Count <= 5 && uiScrollView != null)
            {
                uiScrollView.verticalScrollBar.value = 0.5f;
            }

            return;
        }

        public void Test()
        {
            var data = new List<LTShowItemData>(10);
            data.Add(new LTShowItemData("", 1, "", false));
            data.Add(new LTShowItemData("", 1, "", false));
            data.Add(new LTShowItemData("", 1, "", false));
            data.Add(new LTShowItemData("", 1, "", false));
            data.Add(new LTShowItemData("", 1, "", false));
            Clear();
            CreateItems(data);
            StartCoroutine(ShowItems());
        }
    }
}
