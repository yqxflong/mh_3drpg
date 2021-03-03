using Hotfix_LT.Data;
using System;
using UnityEngine;

namespace Hotfix_LT.UI {
    public class LTPromotionAttributeGroupController : DynamicMonoHotfix {
        private LTPromotionAttributeItemController[] _items;
        private UIGrid _uiGrid;

        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            _uiGrid = t.GetComponent<UIGrid>();
            _items = new LTPromotionAttributeItemController[t.childCount];

            for (var i = 0; i < _items.Length; i++) {
                _items[i] = t.GetChild(i).GetMonoILRComponent<LTPromotionAttributeItemController>();
            }
        }

        public void Set(Data.PromotionTemplate info,int pageId) {
            var attrList = LTPromotionManager.Instance.GetAttrList();
            var len = _items.Length;
            var count = attrList.Count;

            for (var i = 0; i < len; i++) {
                var item = _items[i];

                if (i < count) {
                    item.Set(info, attrList[i],pageId);
                } else {
                    item.Set(info, CharacterTemplateManager.Instance.GetPromotionAttributeLevelInfo(i + 1), pageId);
                }
            }

            _uiGrid.Reposition();
        }

        public void PlayPromotiomProgressfx(int addlevel)
        {
            var attrList = LTPromotionManager.Instance.GetAttrList();
            var len = _items.Length;
            var count = attrList.Count;

            for (var i = 0; i < len; i++)
            {
                var item = _items[i];

                if (i < count)
                {
                    float addparam = attrList[i].attrValue;
                    bool ispercent = addparam.ToString().Contains(".");
                    item.PlayProgressfx(true,true, ispercent? addparam * addlevel *100: addparam * addlevel,ispercent);
                }
            }
        }
    }
}
