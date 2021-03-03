using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionCreateView : DynamicMonoHotfix
    {
        public UIButton CloseBtn;
        public UIButton BackBGBtn;
        public UIButton SendCreateBtn;
        public UIButton EditIconBtn;
        public UISprite Icon;
        public UISprite IconBG;
        public UILabel CoinNumLab;
        public LegionIconEditView IconEditView;
        public UIInput BtInput;
        public UIPanel panel;

        public Action<string, int> onClickSendCreateLegion;

        private TweenScale mTs;

        private int iconID;

        public override void Awake()
        {
            base.Awake();
            panel = mDMono.GetComponent<UIPanel>();
            CloseBtn = mDMono.transform.Find("BackButton").GetComponent<UIButton>();
            BackBGBtn = mDMono.transform.Find("BG/BackBG").GetComponent<UIButton>();
            SendCreateBtn = mDMono.transform.Find("EditNameBtn").GetComponent<UIButton>();
            EditIconBtn = mDMono.transform.Find("EditIconBtn").GetComponent<UIButton>();
            Icon = mDMono.transform.Find("Badge/LegionIcon").GetComponent<UISprite>();
            IconBG = mDMono.transform.Find("Badge/IconBG").GetComponent<UISprite>();
            CoinNumLab = mDMono.transform.Find("EditNameBtn/CoinNum").GetComponent<UILabel>();            
            BtInput = mDMono.transform.Find("Name/InputLabel").GetComponent<UIInput>();
            if (CloseBtn != null) CloseBtn.onClick.Add(new EventDelegate(OnClickCloseBtn));
            if (BackBGBtn != null) BackBGBtn.onClick.Add(new EventDelegate(OnClickCloseBtn));
            if (SendCreateBtn != null) SendCreateBtn.onClick.Add(new EventDelegate(OnClickSendCreateLegion));
            if (EditIconBtn != null) EditIconBtn.onClick.Add(new EventDelegate(OnClickEditIconBtn));

           Messenger.AddListener<int>(Hotfix_LT.EventName.LegionIconIDEdit,OnLegionIconEditFunc);
        }

        public override void Start()
        {
            base.Start();
            IconEditView = mDMono.transform.parent.Find("LTLegionIconEditView").GetMonoILRComponent<LegionIconEditView>();
        }
        public override void OnDestroy()
        {
            if (CloseBtn != null) CloseBtn.onClick.Clear();
            if (BackBGBtn != null) BackBGBtn.onClick.Clear();
            if (SendCreateBtn != null) SendCreateBtn.onClick.Clear();
            if (EditIconBtn != null) EditIconBtn.onClick.Clear();

            Messenger.RemoveListener<int>(Hotfix_LT.EventName.LegionIconIDEdit,OnLegionIconEditFunc);

        }

        public void OpenUI()
        {
            mDMono.gameObject.SetActive(true);
            if (mTs == null)
            {
                mTs = mDMono.transform.GetComponent<TweenScale>();
            }
            mTs.ResetToBeginning();
            mTs.PlayForward();

            iconID = 0;
            Icon.spriteName = LegionModel.GetInstance().dicLegionSpriteName[0];
            IconBG.spriteName = LegionModel.GetInstance().dicLegionBGSpriteName[0];
            LTUIUtil.SetText(CoinNumLab, AlliancesManager.Instance.Config.CreateCost.ToString());
            string colorStr = BalanceResourceUtil.GetUserDiamond() >= AlliancesManager.Instance.Config.CreateCost ? LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
            CoinNumLab.text = string.Format("[{0}]{1}[-]", colorStr, AlliancesManager.Instance.Config.CreateCost);
        }

        private void OnClickSendCreateLegion()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (onClickSendCreateLegion != null)
            {
                onClickSendCreateLegion(BtInput.value, iconID);
            }
        }

        private void OnClickEditIconBtn()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            IconEditView.SetData(iconID);
        }

        private void OnClickCloseBtn()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            BtInput.value = "";
            mDMono.gameObject.SetActive(false);
          
        }

        private void OnLegionIconEditFunc(int _iconID)
        {
            iconID = _iconID;
            Icon.spriteName = LegionModel.GetInstance().dicLegionSpriteName[iconID % 100];
            IconBG.spriteName = LegionModel.GetInstance().dicLegionBGSpriteName[iconID / 100];
        }


    }

}