using UnityEngine;

namespace Hotfix_LT.UI {
    public class LTPromotionStarGroupController : DynamicMonoHotfix {
        private LTPromotionStarItemController[][] _items;
        private GameObject[] _groups;

        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            _groups = new GameObject[t.childCount];
            _items = new LTPromotionStarItemController[t.childCount][];

            for (var i = 0; i < _groups.Length; i++) {
                var child = t.GetChild(i);
                _groups[i] = child.gameObject;
                _groups[i].SetActive(false);
                _items[i] = new LTPromotionStarItemController[child.childCount];

                for (var j = 0; j < child.childCount; j++) {
                    _items[i][j] = child.GetChild(j).GetMonoILRComponent<LTPromotionStarItemController>();
                }
            }
        }

        public void Set(int starCount, int lightCount, bool playFx = false) {
            ActiveGroup(starCount);
            if (starCount > 0)
            {
                ActiveStar(_items[starCount - 1], lightCount, playFx);
            }
        }

        private void ActiveStar(LTPromotionStarItemController[] items, int lightCount, bool playFx) {
            for (var i = 0; i < items.Length; i++) {
                var count = i + 1;
                items[i].Set(count <= lightCount);
            }

            if (playFx && lightCount - 1 >= 0) {
                items[lightCount - 1].Play();
            }
        }

        private void ActiveGroup(int starCount) {
            for (var i = 0; i < _groups.Length; i++) {
                var count = i + 1;
                _groups[i].SetActive(count == starCount);
            }
        }
    }
}
