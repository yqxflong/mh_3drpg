using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTSelectBoxController : UIControllerHotfix
    {
        /// <summary>
        /// 标题
        /// </summary>
        public UILabel TitleLab;
    
        /// <summary>
        /// 描述1
        /// </summary>
        public UILabel Desc1;
    
        /// <summary>
        /// 使用数量
        /// </summary>
        public UILabel CountLab;
    
        /// <summary>
        /// 用于对齐描述
        /// </summary>
        public UITable DescTable;
    
        /// <summary>
        /// 左箭头遮罩
        /// </summary>
        public GameObject LeftArrowMask;
    
        /// <summary>
        /// 右箭头遮罩
        /// </summary>
        public GameObject RightArrowMask;
    
        /// <summary>
        /// 左箭头按钮
        /// </summary>
        public UIButton LeftArrowBtn;
    
        /// <summary>
        /// 右箭头按钮
        /// </summary>
        public UIButton RightArrowBtn;
    
        /// <summary>
        /// 第一个item
        /// </summary>
        public LTSelectBoxCellController FirstCell;
    
        /// <summary>
        /// 物品拖动组件
        /// </summary>
        public LTSelectBoxTableScroll TableScroll;
    
        /// <summary>
        /// 可选物品列表
        /// </summary>
        private List<Hotfix_LT.Data.SelectBox> mSelectBoxList;
    
        /// <summary>
        /// 箱子最大数量
        /// </summary>
        private int mBoxMaxNum;
    
        /// <summary>
        /// 当前选中的箱子的数量
        /// </summary>
        private int mCurSelectBoxNum;
    
        /// <summary>
        /// 当前选中的背包物品的服务器唯一Id
        /// </summary>
        private string mCurInventoryId;
    
        /// <summary>
        /// 当前选中的item
        /// </summary>
        private LTSelectBoxCellController mCurCell;
    
        private static string mCurSelectItemId;
        public static string CurSelectItemId
        {
            get { return mCurSelectItemId; }
        }
    
        public override bool ShowUIBlocker
        {
            get { return true; }
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TitleLab = t.GetComponent<UILabel>("BG/Title");
            Desc1 = t.GetComponent<UILabel>("Desc/Desc1");
            CountLab = t.GetComponent<UILabel>("Use/UseNum");
            LeftArrowMask = t.FindEx("Use/LeftArrow/LeftArrowMask").gameObject;
            RightArrowMask = t.FindEx("Use/RightArrow/RightArrowMask").gameObject;
            LeftArrowBtn = t.GetComponent<UIButton>("Use/LeftArrow");
            RightArrowBtn = t.GetComponent<UIButton>("Use/RightArrow");
            FirstCell = t.GetMonoILRComponent<LTSelectBoxCellController>("ItemList/Scroll View/Placeholder/Grid/Row/Item");
            TableScroll = t.GetMonoILRComponent<LTSelectBoxTableScroll>("ItemList/Scroll View/Placeholder/Grid");
            controller.backButton = t.GetComponent<UIButton>("CloseBtn");

            t.GetComponent<UIButton>("ConfirmBtn").onClick.Add(new EventDelegate(OnClickConfirmBtn));
            t.GetComponent<UIButton>("Use/Max").onClick.Add(new EventDelegate(OnClickMaxBtn));

            t.GetComponent<ContinuePressTrigger>("Use/LeftArrow").m_CallBackPress.Add(new EventDelegate(OnPressLeftArrowBtn));
            t.GetComponent<ContinuePressTrigger>("Use/RightArrow").m_CallBackPress.Add(new EventDelegate(OnPressRightArrowBtn));
        }

        public override void SetMenuData(object param)
        {
            Hashtable tempParam = param as Hashtable;
            if (tempParam != null)
            {
                mBoxMaxNum = int.Parse(tempParam["boxManNum"].ToString());
                mCurInventoryId = tempParam["inventoryId"].ToString();
                mSelectBoxList = tempParam["selectBoxList"] as List<Hotfix_LT.Data.SelectBox>;
                mCurSelectBoxNum = 1;
    
                if (mSelectBoxList == null)
                {
                    EB.Debug.LogError("LTSelectBoxController SetMenuData is Error, param is not List<Hotfix_LT.Data.SelectBox>!!!");
                    controller.Close();
                    return;
                }
    
                if (mSelectBoxList.Count <= 0)
                {
                    EB.Debug.LogError("LTSelectBoxController SetMenuData is Error, mSelectBoxList Count is 0!!!");
                    controller.Close();
                    return;
                }
    
                ShowUI();
            }
            else
            {
                EB.Debug.LogError("LTSelectBoxController SetMenuData is Error, param is not Hashtable!!!");
            }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        /// <summary>
        /// 显示界面
        /// </summary>
        public void ShowUI()
        {
            TableScroll.SetItemDatas(mSelectBoxList.ToArray());
            mCurCell = FirstCell;
            mCurCell.SetSelectSpStatus(true);
            RefreshBtnStatus();
    
            Hotfix_LT.Data.SelectBox firstData = mSelectBoxList[0];
            mCurSelectItemId = firstData.ri1;
            if (firstData.rt1.Equals("hero"))
            {
                TitleLab.text = EB.Localizer.GetString("ID_SELECTBOX_SELECT_PARTNER");
                string str = GetPartnerTypeName(firstData.ri1);
                Desc1.text = string.Format(EB.Localizer.GetString("ID_SELECTBOX_DESC_PARTNER"), str);
            }
            else if (firstData.rt1.Equals("heroshard"))
            {
                TitleLab.text = EB.Localizer.GetString("ID_SELECTBOX_SELECT_PARTNER_CLIP");
                string str = GetPartnerTypeName(firstData.ri1);
                Desc1.text = string.Format(EB.Localizer.GetString("ID_SELECTBOX_DESC_PARTNER_CLIP"), str);
    
            }
            else if (firstData.rt1.Equals("gaminventory"))
            {
                TitleLab.text = EB.Localizer.GetString("ID_SELECTBOX_SELECT_EQUIPMENT");
                Desc1.text = EB.Localizer.GetString("ID_SELECTBOX_DESC_EQUIPMENT");
            }
        }
    
        /// <summary>
        /// 获取伙伴类型的图标名称（sr, ssr）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetPartnerTypeName(string id)
        {
            Hotfix_LT.Data.HeroInfoTemplate tempInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfoByStatId(int.Parse(id));
            if (tempInfo != null)
            {
                PartnerGrade partnerGrade = (PartnerGrade)tempInfo.role_grade;
                if (LTPartnerConfig.PARTNER_GRADE_STR_DIC.ContainsKey(partnerGrade))
                {
                    return LTPartnerConfig.PARTNER_GRADE_STR_DIC[partnerGrade];
                }
            }
    
            return string.Empty;
        }
    
        /// <summary>
        /// 刷新按钮状态
        /// </summary>
        private void RefreshBtnStatus()
        {
            bool leftStatus = mCurSelectBoxNum > 1;
            bool rightStatus = mCurSelectBoxNum < mBoxMaxNum;
    
            LeftArrowMask.CustomSetActive(!leftStatus);
            RightArrowMask.CustomSetActive(!rightStatus);
            LeftArrowBtn.enabled = leftStatus;
            RightArrowBtn.enabled = rightStatus;
    
            CountLab.text = mCurSelectBoxNum.ToString();
        }
    
        /// <summary>
        /// 点击item
        /// </summary>
        /// <param name="cell"></param>
        public void OnClickSelectBoxItem(LTSelectBoxCellController cell)
        {
            if (cell == null)
            {
                EB.Debug.LogError("LTSelectBoxController OnClickSelectBoxItem is Error, cell is Null");
                return;
            }
    
            if (cell.GetCurSelectBoxData() == null)
            {
                return;
            }
    
            if (mCurCell != null)
            {
                mCurCell.SetSelectSpStatus(false);
            }
    
            mCurCell = cell;
            mCurCell.SetSelectSpStatus(true);
            mCurSelectItemId = cell.GetCurSelectBoxData().ri1;
        }
    
        /// <summary>
        /// 点击左箭头按钮
        /// </summary>
        public void OnPressLeftArrowBtn()
        {
            if (mCurSelectBoxNum > 1)
            {
                --mCurSelectBoxNum;
                RefreshBtnStatus();
            }
        }
    
        /// <summary>
        /// 点击右箭头按钮
        /// </summary>
        public void OnPressRightArrowBtn()
        {
            if (mCurSelectBoxNum < mBoxMaxNum)
            {
                ++mCurSelectBoxNum;
                RefreshBtnStatus();
            }
        }
    
        /// <summary>
        /// 点击最大值按钮
        /// </summary>
        public void OnClickMaxBtn()
        {
            mCurSelectBoxNum = mBoxMaxNum;
            RefreshBtnStatus();
        }
    
        /// <summary>
        /// 点击确定按钮
        /// </summary>
        public void OnClickConfirmBtn()
        {
            if (mCurSelectBoxNum < 1 || mCurSelectBoxNum > mBoxMaxNum)
            {
                EB.Debug.LogError("LTSelectBoxController UseItem Error, num Error");
                return;
            }
    
            Hotfix_LT.Data.SelectBox selectBox = mCurCell.GetCurSelectBoxData();
            LTPartnerData partnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(int.Parse(selectBox.ri1));
            bool isHasPartner = partnerData != null && partnerData.HeroId > 0;
            string statId = selectBox.ri1;
            int summonShard = partnerData == null ? 0 :partnerData.HeroInfo.summon_shard;
            LTPartnerDataManager.Instance.UseItem(mCurInventoryId, mCurSelectBoxNum, selectBox.index, delegate 
            {
                if (selectBox.rt1.Equals("hero"))
                {
                    if (!isHasPartner)
                    {
                        LTShowItemData itemData = new LTShowItemData(statId, 1, "hero");
                        GlobalMenuManager.Instance.Open("LTShowGetPartnerUI", itemData);
                    }
                    else
                    {
                        LTShowItemData itemData = new LTShowItemData(statId, summonShard * mCurSelectBoxNum, "heroshard");
                        GlobalMenuManager.Instance.Open("LTShowGetPartnerUI", itemData);
                    }
                }
                else
                {
                    List<LTShowItemData> showItemsList = new List<LTShowItemData>();
    
                    Hotfix_LT.Data.EconemyItemTemplate info = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(selectBox.ri1);
                    if (info is Hotfix_LT.Data.EquipmentItemTemplate)
                    {
                        for (int i = 0; i < mCurSelectBoxNum; i++)
                        {
                            LTShowItemData showItemData = new LTShowItemData(selectBox.ri1, selectBox.rn1, selectBox.rt1);
                            showItemsList.Add(showItemData);
                        }
                    }
                    else
                    {
                        LTShowItemData showItemData = new LTShowItemData(selectBox.ri1, selectBox.rn1 * mCurSelectBoxNum, selectBox.rt1);
                        showItemsList.Add(showItemData);
                    }
    
                    GlobalMenuManager.Instance.Open("LTShowRewardView", showItemsList);
                }
    
                if (mCurSelectBoxNum == mBoxMaxNum)
                {
                    UIInventoryBagLogic.Instance.FirstItem = null;
                }
                UIInventoryBagLogic.Instance.RefeshBag(ShowBagContent.Instance.CurType);

                controller.Close();
            });
        }
    }
}
