using UnityEngine;

namespace Hotfix_LT.UI
{
    public class HonorArenaResultInfo : DynamicMonoHotfix
    {
        private GameObject _itemObj;
        private Transform _left;
        private Transform _right;
        private UIGrid _uiGridLeft;
        private UIGrid _uiGridRitht;
        private UISprite _resultIconLeft;
        private UISprite _resultIconRitht;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            _itemObj = t.parent.FindEx("Item").gameObject;
            _left = t.FindEx("Left");
            _right = t.FindEx("Right");
            _uiGridLeft = t.GetComponent<UIGrid>("Left");
            _uiGridRitht = t.GetComponent<UIGrid>("Right");
            _resultIconLeft = t.GetComponent<UISprite>("Left/ResultIcon");
            _resultIconRitht = t.GetComponent<UISprite>("Right/ResultIcon");       
        }

        public void SetData(bool winLeft, bool winRight, OtherPlayerPartnerData[] leftDatas, OtherPlayerPartnerData[] rightDatas)
        {
            RefreshLeft(winLeft, leftDatas);
            RefreshRight(winRight, rightDatas);

            _resultIconLeft.spriteName = winLeft ? "Ty_Legion_Shengli" : "Ty_Country_Shibai";
            _resultIconLeft.MakePixelPerfect();

            _resultIconRitht.spriteName = winRight ? "Ty_Legion_Shengli" : "Ty_Country_Shibai";
            _resultIconRitht.MakePixelPerfect();

            _uiGridLeft.Reposition();
            _uiGridRitht.Reposition();
        }

        private void RefreshLeft(bool winLeft, OtherPlayerPartnerData[] leftDatas)
        {
            if (leftDatas == null)
            {
                return;
            }

            if (_left.childCount > 1)
            {
                var exsitCount = _left.childCount - 1;
                var needCount = leftDatas.Length - exsitCount;

                if (needCount >= 0)
                {
                    for (var i = exsitCount - 1; i >= 0; i--)
                    {
                        SetItemData(winLeft, _left.GetChild(i), leftDatas[exsitCount - 1 - i]);
                    }

                    for (var i = exsitCount; i < exsitCount + needCount; i++)
                    {
                        CreateItem(winLeft, _left, leftDatas[i]);
                    }
                }
                else
                {
                    for (var i = exsitCount - 1; i >= 0; i--)
                    {
                        if (needCount >= 0)
                        {
                            SetItemData(winLeft, _left.GetChild(i), leftDatas[exsitCount - 1 - i]);
                        }
                        else
                        {
                            _left.GetChild(i).gameObject.SetActive(false);
                            needCount++;
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < leftDatas.Length; i++)
                {
                    CreateItem(winLeft, _left, leftDatas[i]);
                }
            }
        }

        private void RefreshRight(bool winRight, OtherPlayerPartnerData[] rightDatas)
        {
            if (rightDatas == null)
            {
                return;
            }

            if (_right.childCount > 1)
            {
                var exsitCount = _right.childCount - 1;
                var needCount = rightDatas.Length - exsitCount;

                if (needCount >= 0)
                {
                    for (var i = 0; i < exsitCount; i++)
                    {
                        SetItemData(winRight, _right.GetChild(i), rightDatas[i]);
                    }

                    for (var i = exsitCount; i < exsitCount + needCount; i++)
                    {
                        CreateItem(winRight, _right, rightDatas[i], i);
                    }
                }
                else
                {
                    for (var i = 0; i < exsitCount; i++)
                    {
                        if (i < rightDatas.Length)
                        {
                            SetItemData(winRight, _right.GetChild(i), rightDatas[i]);
                        }
                        else
                        {
                            _right.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                for (var i = rightDatas.Length - 1; i >= 0; i--)
                {
                    CreateItem(winRight, _right, rightDatas[i]);
                }
            }
        }

        private void SetItemData(bool isWin, Transform t, OtherPlayerPartnerData data)
        {
            var item = t.GetMonoILRComponent<FormationPartnerItem>();
            item.Fill(data);
            item.mDMono.gameObject.SetActive(true);

            ShowMaskOrPlaceholder(t, (data == null), isWin);
        }

        private void CreateItem(bool isWin, Transform parent, OtherPlayerPartnerData data, int index = 0)
        {
            var item = GameObject.Instantiate(_itemObj, parent).GetMonoILRComponent<FormationPartnerItem>();
            item.Fill(data);
            item.mDMono.gameObject.SetActive(true);
            item.mDMono.transform.SetSiblingIndex(index);

            ShowMaskOrPlaceholder(item.mDMono.transform, (data == null), isWin);
        }

        private void ShowMaskOrPlaceholder(Transform t, bool isPlaceholder, bool isWin)
        {
            if (t == null)
            {
                return;
            }

            if (isPlaceholder)
            {
                var childCount = t.childCount;

                for (var i = 0; i < childCount; i++)
                {
                    var child = t.GetChild(i);

                    if (child.name.Equals("Mask"))
                    {
                        child.gameObject.SetActive(true);
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                t.FindEx("Mask").gameObject.SetActive(!isWin);
            }
        }
    }
}
