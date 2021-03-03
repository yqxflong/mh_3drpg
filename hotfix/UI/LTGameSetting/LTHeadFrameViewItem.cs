using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTHeadFrameViewItem : DynamicCellController<HeadFrame>
    {
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            HeadFrameIcon = t.GetComponent<UISprite>("HeadFrame");
            CurSelectObj = t.Find("Select").gameObject;
            LockObject = t.Find("Lock").gameObject;
            RedPoint = t.Find("RedPoint").gameObject;
            HeadFrameIcon.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnBtnClick));
        }

        public UISprite HeadFrameIcon;
        public GameObject CurSelectObj;
        public GameObject LockObject;
        public GameObject RedPoint;

        private HeadFrame ItemData;

        public override void Fill(HeadFrame itemData)
        {
            ItemData = itemData;
            if (ItemData == null)
            {
                mDMono.gameObject.CustomSetActive(false);
            }
            else
            {
                mDMono.gameObject.CustomSetActive(true);
                HeadFrameIcon.spriteName = ItemData.iconId;

                CurSelectObj.CustomSetActive(itemData.id.Equals(LTHeadFrameViewCtrl.CurId) && itemData.num == LTHeadFrameViewCtrl.CurNum);
                if (isLock())
                {
                    LockObject.CustomSetActive(true);
                    HeadFrameIcon.GetComponent<UISprite>().color = Color.gray;
                    RedPoint.CustomSetActive(false);
                }
                else
                {
                    LockObject.CustomSetActive(false);
                    HeadFrameIcon.GetComponent<UISprite>().color = Color.white;
                    string temp = string.Format("{0}_{1}", ItemData.id, ItemData.num);
                    RedPoint.CustomSetActive(!LTGameSettingManager.Instance.HasFrame(temp));
                }

                mDMono.GetComponent<UISprite>().color = isCurSelect() ? new Color(72f / 255f, 192f / 255f, 1) : new Color(174f / 255f, 185f / 255f, 203f / 255f);
            }
        }

        private bool isLock()
        {
            if (ItemData != null && ItemData.num == 0) return false;
            //添加manager判断
            int num = 0;
            DataLookupsCache.Instance.SearchDataByID<int>(string.Format("userHeadFrame.head_frame.{0}", ItemData.id), out num);
            return num < ItemData.num;
        }

        public override void Clean()
        {
            Fill(null);
        }

        private bool isCurSelect()
        {
            return (ItemData.id.Equals(LTHeadFrameViewCtrl.Id) && ItemData.num == LTHeadFrameViewCtrl.Num);
        }

        public void OnBtnClick()
        {
            string temp = string.Format("{0}_{1}", ItemData.id, ItemData.num);
            if (RedPoint.activeSelf && !LTGameSettingManager.Instance.HasFrame(temp))
            {
                LTGameSettingManager.Instance.ClickFrame(temp);
            }
            if (ItemData == null || isCurSelect()) return;

            if (HeadFrameEvent.SelectEvent != null)
            {
                HeadFrameEvent.SelectEvent(ItemData.id, ItemData.num, isLock());
            }
        }
    }
}