using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class LegionManagerHudController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            BGBackButton = t.GetComponent<UIButton>("BG/BackBG");
            limitLevelLabel = t.GetComponent<UILabel>("Left/LevelLimit/Level/Label");
            requestNumLabel = t.GetComponent<UILabel>("Right/RequestNumLabel");
            allRefuseButton = t.GetComponent<UIButton>("Right/AllRefuseButton");
            increaseLimitBtn = t.GetComponent<UIButton>("Left/LevelLimit/IncreaseButton");
            reduceLimitBtn = t.GetComponent<UIButton>("Left/LevelLimit/ReduceButton");
            openApproveBtn = t.GetComponent<UIButton>("Left/Check/ChoiceYesButton");
            closeApproveBtn = t.GetComponent<UIButton>("Left/Check/ChoiceNoButton");
            choiceOpenSpt = t.GetComponent<UISprite>("Left/Check/ChoiceOpenSp");
            choiceCloseSpt = t.GetComponent<UISprite>("Left/Check/ChoiceCloseSp");
            saveLimitBtn = t.GetComponent<UIButton>("Left/SaveButton");
            approveItemTemplate = t.GetMonoILRComponent<LegionApproveItem>("Right/Other/ItemTemplate");
            scrollView = t.GetComponent<UIScrollView>("Right/Scroll View");
            grid = t.GetComponent<UIGrid>("Right/Scroll View/Grid");
            listApproveItem = new List<LegionApproveItem>();
            listApproveItem.Add(t.GetMonoILRComponent<LegionApproveItem>("Right/Scroll View/Grid/Item"));
            isOpen = true;
            controller.backButton = t.GetComponent<UIButton>("BackButton");
            BGBackButton.onClick.Add(new EventDelegate(OnClickBackBtn));
            increaseLimitBtn.onClick.Add(new EventDelegate(OnClickIncreaseLimit));
            reduceLimitBtn.onClick.Add(new EventDelegate(OnClickReduceLimit));
            openApproveBtn.onClick.Add(new EventDelegate(OnClickOpenApprove));
            closeApproveBtn.onClick.Add(new EventDelegate(OnClickCloseApprove));
            allRefuseButton.onClick.Add(new EventDelegate(OnClickTotalReject));
            saveLimitBtn.onClick.Add(new EventDelegate(OnClickSaveLimitBtn));
            LegionEvent.NotifyUpdateLegionData += SetData;
            LegionEvent.NotifyLegionAccount += OnLegionAccount;
            LegionEvent.NotifyByKickOut += OnByKickOut;


        }

        public override void OnDestroy()
        {
            increaseLimitBtn.onClick.Clear();
            reduceLimitBtn.onClick.Clear();
            openApproveBtn.onClick.Clear();
            closeApproveBtn.onClick.Clear();
            allRefuseButton.onClick.Clear();
            saveLimitBtn.onClick.Clear();

            LegionEvent.NotifyUpdateLegionData -= SetData;
            LegionEvent.NotifyLegionAccount -= OnLegionAccount;
            LegionEvent.NotifyByKickOut -= OnByKickOut;

            base.OnDestroy();
        }

        public UIButton BGBackButton;
    
        public UILabel limitLevelLabel;
        public UILabel requestNumLabel;
        public UIButton allRefuseButton;
    
        /// <summary> 增加等级限制按钮 </summary>
        public UIButton increaseLimitBtn;
        /// <summary> 减少等级限制按钮 </summary>
        public UIButton reduceLimitBtn;
    
        public UIButton openApproveBtn;
        public UIButton closeApproveBtn;
        public UISprite choiceOpenSpt;
        public UISprite choiceCloseSpt;
    
        /// <summary>
        /// 保存限制按钮
        /// </summary>
        public UIButton saveLimitBtn;
    
        public LegionApproveItem approveItemTemplate;
    
        public List<LegionApproveItem> listApproveItem;
    
        public UIScrollView scrollView;
    
        public UIGrid grid;
    
        //private int itemBehind = 216;
        public static bool isOpen = false;
    
        private bool _isOpenApprove;
        public bool IsOpenApprove
        {
            get { return _isOpenApprove; }
            set
            {
                _isOpenApprove = value;
                if (choiceOpenSpt != null && choiceCloseSpt != null)
                {
                    choiceOpenSpt.gameObject.SetActive(value);
                    choiceCloseSpt.gameObject.SetActive(!value);
                }
            }
        }
     
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
    
            LegionData data = param as LegionData;
    
            if (data != null)
            {
                SetData(data);
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            isOpen = true;
            return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            isOpen = false;
            yield break;
        }
    
    
        public void SetData(LegionData data)
        {
            limitLevelLabel.text = data.conditionLevel.ToString();
            requestNumLabel.text = data.listRequestJoin.Count.ToString();
    
            IsOpenApprove = data.isNeedApprove;
    
            //int num = data.listRequestJoin.Count;
            //LTUIUtil.SetNumTemplate(approveItemTemplate, listApproveItem, num, itemBehind);
    
            if (data.listRequestJoin.Count > listApproveItem.Count)
            {
                int length = data.listRequestJoin.Count;
                int index = listApproveItem.Count;
    
                for (int i = index; i < length; i++)
                {
                    GameObject go = Object.Instantiate(approveItemTemplate.mDMono.gameObject);
                    go.transform.parent = grid.transform;
                    go.transform.localScale = approveItemTemplate.mDMono.transform.localScale;
                    LegionApproveItem item = go.GetMonoILRComponent<LegionApproveItem>();
                    listApproveItem.Add(item);
                }
                grid.Reposition();
            }
    
            for (int i = 0; i < listApproveItem.Count; i++)
            {
                if (i < data.listRequestJoin.Count)
                {
                    RequestJoinData joinData = data.listRequestJoin[i];
                    LegionApproveItem item = listApproveItem[i];
                    item.SetData(joinData);
                    item.SetAction(OnClickConsentApprove, OnClickRefuseApprove);
                }
                else
                {
                    listApproveItem[i].ShowUI(false);
                }
            }
    
            //权限
            if (data.userMemberData != null)
            {
                switch (data.userMemberData.dutyType)
                {
                    case eAllianceMemberRole.Member:
                    case eAllianceMemberRole.Admin:
                        ChangeOpen(false);
                        break;
                    case eAllianceMemberRole.ExtraOwner:
                    case eAllianceMemberRole.Owner:
                        ChangeOpen(true);
                        break;
                }
            }
    
            if (scrollView != null)
            {
                scrollView.RestrictWithinBounds(true);
            }
        }
    
        private void ChangeOpen(bool isAdminSaveLimit)
        {
            if (saveLimitBtn != null)
            {
                LTUIUtil.SetGreyButtonEnable(saveLimitBtn, isAdminSaveLimit);
            }
        }
    
        private void OnClickBackBtn()
        {
            controller.Close();
        }
    
        private void OnClickIncreaseLimit()
        {
            int primal = int.Parse(limitLevelLabel.text);
            primal += 5;
            limitLevelLabel.text = primal.ToString();
        }
    
        private void OnClickReduceLimit()
        {
            int primal = int.Parse(limitLevelLabel.text);
    
            if (primal > 0)
            {
                primal -= 5;
            }
            limitLevelLabel.text = primal.ToString();
        }
    
        private void OnClickOpenApprove()
        {
            IsOpenApprove = true;
    
        }
    
        private void OnClickCloseApprove()
        {
            IsOpenApprove = false;
        }
    
        private void OnClickTotalReject()
        {
            if (LegionEvent.SendRejectTotalRequestJoin != null)
            {
                LegionEvent.SendRejectTotalRequestJoin();
            }
        }
    
        private void OnClickConsentApprove(long id)
        {
            if (LegionEvent.SendConsentRequestJoin != null)
            {
                LegionEvent.SendConsentRequestJoin(id);
            }
        }
    
        private void OnClickRefuseApprove(long id)
        {
            if (LegionEvent.SendRejectRequestJoin != null)
            {
                LegionEvent.SendRejectRequestJoin(id);
            }
        }
    
        private void OnClickSaveLimitBtn()
        {
            if (LegionEvent.SendSaveLimit != null)
            {
                LegionEvent.SendSaveLimit(int.Parse(limitLevelLabel.text), IsOpenApprove);
            }
        }
    
        void OnLegionAccount(AllianceAccount data)
        {
            if (data.State == eAllianceState.Leaved)
            {
                controller.Close();
            }
        }
    
        private void OnByKickOut()
        {
            // 已废弃，为什么不知道，不会调这里了！
            //需要补充界面告诉其被踢出军团
            controller.Close();
    
            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionMainHudController_12495"));
        }
    }
}
