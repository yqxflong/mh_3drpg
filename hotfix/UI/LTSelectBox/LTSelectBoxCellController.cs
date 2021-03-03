using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTSelectBoxCellController : DynamicCellController<Hotfix_LT.Data.SelectBox>
    {
        /// <summary>
        /// 物品item
        /// </summary>
        public LTShowItem ShowItem;
    
        /// <summary>
        /// 选中框
        /// </summary>
        public GameObject SelectObj;
    
        /// <summary>
        /// 当前的数据
        /// </summary>
        private Hotfix_LT.Data.SelectBox mSelectBoxData;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            ShowItem = t.GetMonoILRComponent<LTShowItem>("LTShowItem");
            SelectObj = t.FindEx("Select").gameObject;

            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                var boxController = ((GameObject)mDMono.ObjectParamList[0]).GetUIControllerILRComponent<LTSelectBoxController>();
                var cellController = t.GetMonoILRComponent<LTSelectBoxCellController>();

                t.GetComponent<UIButton>("LTShowItem").onClick.Add(new EventDelegate(() => boxController.OnClickSelectBoxItem(cellController)));
                t.GetComponentEx<UIEventTrigger>().onClick.Add(new EventDelegate(() => boxController.OnClickSelectBoxItem(cellController)));
            }
        }

        public override void Start()
        {
            base.Start();
            ShowItem.PlayFX();
        }

        public override void Fill(Hotfix_LT.Data.SelectBox itemData)
        {
            if (itemData == null)
            {
                mSelectBoxData = null;
                ShowItem.mDMono.gameObject.CustomSetActive(false);
                return;
            }
            ShowItem.mDMono.gameObject.CustomSetActive(true);
    
            mSelectBoxData = itemData;
    
            LTShowItemData showItemData = new LTShowItemData(itemData.ri1, itemData.rn1, itemData.rt1, false);
            ShowItem.LTItemData = showItemData;
            SetSelectSpStatus(itemData.ri1.Equals(LTSelectBoxController.CurSelectItemId));
        }

        public override void Clean()
        {
            mSelectBoxData = null;
            ShowItem.mDMono.gameObject.CustomSetActive(false);
            SetSelectSpStatus(false);
        }
    
        /// <summary>
        /// 设置选中框的状态
        /// </summary>
        /// <param name="isShow">是否显示</param>
        public void SetSelectSpStatus(bool isShow)
        {
            SelectObj.CustomSetActive(isShow);
        }
    
        /// <summary>
        /// 获取当前的数据
        /// </summary>
        /// <returns></returns>
        public Hotfix_LT.Data.SelectBox GetCurSelectBoxData()
        {
            return mSelectBoxData;
        }
    }
}
