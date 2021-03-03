using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTMainInstancePasswordBoxCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TitleLabel = t.GetComponent<UILabel>("Bg/Top/Title");
            TipLabel = t.GetComponent<UILabel>("Content/Info/Desc");
            RefreshBtn = t.GetComponent<UIButton>("Content/RefreshBtn");

            InputBtnList = new List<UIButton>();
            InputBtnList.Add(t.GetComponent<UIButton>("Content/InputBtnList/Btn"));
            InputBtnList.Add(t.GetComponent<UIButton>("Content/InputBtnList/Btn (1)"));
            InputBtnList.Add(t.GetComponent<UIButton>("Content/InputBtnList/Btn (2)"));
            InputBtnList.Add(t.GetComponent<UIButton>("Content/InputBtnList/Btn (3)"));
            InputBtnList.Add(t.GetComponent<UIButton>("Content/InputBtnList/Btn (4)"));
            InputBtnList.Add(t.GetComponent<UIButton>("Content/InputBtnList/Btn (5)"));


            inputLabelList = new List<UILabel>();
            inputLabelList.Add(t.GetComponent<UILabel>("Content/InputContent/Sprite/Label"));
            inputLabelList.Add(t.GetComponent<UILabel>("Content/InputContent/Sprite (1)/Label"));
            inputLabelList.Add(t.GetComponent<UILabel>("Content/InputContent/Sprite (2)/Label"));
            inputLabelList.Add(t.GetComponent<UILabel>("Content/InputContent/Sprite (3)/Label"));

            controller.backButton = t.GetComponent<UIButton>("Bg/Top/CancelBtn");

            t.GetComponent<UIButton>("Content/RefreshBtn").onClick.Add(new EventDelegate(OnRefreshBtnClick));
            t.GetComponent<UIButton>("Content/InputBtnList/Btn").onClick.Add(new EventDelegate(() =>OnInputBtnClick(t.GetComponent<UIButton>("Content/InputBtnList/Btn"))));
            t.GetComponent<UIButton>("Content/InputBtnList/Btn (1)").onClick.Add(new EventDelegate(() => OnInputBtnClick(t.GetComponent<UIButton>("Content/InputBtnList/Btn (1)"))));
            t.GetComponent<UIButton>("Content/InputBtnList/Btn (2)").onClick.Add(new EventDelegate(() => OnInputBtnClick(t.GetComponent<UIButton>("Content/InputBtnList/Btn (2)"))));
            t.GetComponent<UIButton>("Content/InputBtnList/Btn (3)").onClick.Add(new EventDelegate(() => OnInputBtnClick(t.GetComponent<UIButton>("Content/InputBtnList/Btn (3)"))));
            t.GetComponent<UIButton>("Content/InputBtnList/Btn (4)").onClick.Add(new EventDelegate(() => OnInputBtnClick(t.GetComponent<UIButton>("Content/InputBtnList/Btn (4)"))));
            t.GetComponent<UIButton>("Content/InputBtnList/Btn (5)").onClick.Add(new EventDelegate(() => OnInputBtnClick(t.GetComponent<UIButton>("Content/InputBtnList/Btn (5)"))));

            inputList = new List<int>();
        }
        
        public UILabel TitleLabel, TipLabel;
    
        private LTInstanceNode nodeData;
    
        private List<int> inputList;
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override void SetMenuData(object param)
        {
            nodeData = param as LTInstanceNode;
            if(nodeData == null)
            {
                Hashtable hash = param as Hashtable;
                nodeData = hash["data"] as LTInstanceNode;
                string title = hash["title"] as string;
                string tip = hash["tip"] as string;
                TitleLabel.text = title;
                TipLabel.text = tip;
            }
            else
            {
                TitleLabel.text = EB.Localizer.GetString("ID_codefont_in_ChapterDialogController_2241");
                TipLabel .text = EB.Localizer.GetString("ID_uifont_in_LTMainInstancePasswordBoxView_Desc_0");
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            InitUI();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            OnRefreshBtnClick();
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }
    
        public UIButton RefreshBtn;
    
        public List<UIButton> InputBtnList;
    
        public List<UILabel> inputLabelList;
    
        private void InitUI()
        {
            for (int i = 0; i < inputLabelList.Count; i++)
            {
                if (i < inputList.Count)
                {
                    inputLabelList[i].text = inputList[i].ToString();
                }
                else
                {
                    inputLabelList[i].text = string.Empty;
                }
            }
            RefreshBtn.gameObject.SetActive(inputList.Count > 0);
        }
    
    
        public void OnInputBtnClick(UIButton btn)
        {
            int num = InputBtnList.IndexOf(btn) + 1;
            inputList.Add(num);
            InitUI();
            if (inputList.Count >= 4)
            {
                string password = string.Empty;
                for (var i = 0; i < inputList.Count; i++)
                {
                    password += inputList[i];
                }
    
                if (password == nodeData.RoleData.CampaignData.Password)
                {
                    int dir = (int)LTInstanceMapModel.Instance.CurNode.GetDirByPos(nodeData.x, nodeData.y);
                    if (dir <= 0)
                    {
                        return;
                    }
    
                    LTInstanceMapModel.Instance.RequestMoveChar(dir);
                    OnCancelButtonClick();
                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTMainInstancePasswordBoxCtrl_2334"));
                    OnRefreshBtnClick();
                }
            }
        }
    
        public void OnRefreshBtnClick()
        {
            inputList.Clear();
            InitUI();
        }
    }
}
