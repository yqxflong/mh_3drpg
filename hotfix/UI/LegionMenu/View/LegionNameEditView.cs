using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionNameEditView : DynamicMonoHotfix
    {

        public UIButton CloseBtn;
        public UIButton BackBGBtn;
        public UIButton EditNameBtn;
        public UIButton EditIconBtn;
        public UISprite Icon;
        public UISprite IconBG;
        public UILabel CoinNumLab;
        public LegionIconEditView IconEditView;
        public UIInput BtInput;

        private LegionData data;

        private int iconID;

        public bool IsShow { get { return mDMono.gameObject.activeInHierarchy; } }

        public override void Awake()
        {
            base.Awake();

            CloseBtn = mDMono.transform.Find("BackButton").GetComponent<UIButton>();
            BackBGBtn = mDMono.transform.Find("BG/BackBG").GetComponent<UIButton>();
            EditNameBtn = mDMono.transform.Find("EditNameBtn").GetComponent<UIButton>();
            EditIconBtn = mDMono.transform.Find("EditIconBtn").GetComponent<UIButton>();
            Icon = mDMono.transform.Find("Badge/LegionIcon").GetComponent<UISprite>();
            IconBG = mDMono.transform.Find("Badge/IconBG").GetComponent<UISprite>();
            CoinNumLab = mDMono.transform.Find("EditNameBtn/CoinNum").GetComponent<UILabel>();
            IconEditView = mDMono.transform.parent.Find("LTLegionIconEditView").GetMonoILRComponent<LegionIconEditView>();
            BtInput = mDMono.transform.Find("Name/InputLabel").GetComponent<UIInput>();
            if (CloseBtn != null) CloseBtn.onClick.Add(new EventDelegate(OnClickCloseBtn));
            if (BackBGBtn != null) BackBGBtn.onClick.Add(new EventDelegate(OnClickCloseBtn));
            if (EditNameBtn != null) EditNameBtn.onClick.Add(new EventDelegate(OnClickChangeLegionName));
            if (EditIconBtn != null) EditIconBtn.onClick.Add(new EventDelegate(OnClickEditIconBtn));

           Messenger.AddListener<int>(Hotfix_LT.EventName.LegionIconIDEdit,OnLegionIconEditFunc);
        }



        public override void OnDestroy()
        {
            if (CloseBtn != null) CloseBtn.onClick.Clear();
            if (BackBGBtn != null) BackBGBtn.onClick.Clear();
            if (EditNameBtn != null) EditNameBtn.onClick.Clear();
            if (EditIconBtn != null) EditIconBtn.onClick.Clear();

           Messenger.RemoveListener<int>(Hotfix_LT.EventName.LegionIconIDEdit,OnLegionIconEditFunc);
        }

        public void ShowUI(bool isShow)
        {
            if (isShow)
            {
                TweenScale TS = mDMono.transform.GetComponent<TweenScale>();
                TS.ResetToBeginning();
                TS.PlayForward();
            }
            mDMono.gameObject.SetActive(isShow);
        }

        public void SetData(LegionData legionData)
        {
            data = legionData;
            ShowUI(true);

            iconID = data.legionIconID;
            BtInput.value = data.legionName;
            Icon.spriteName = data.legionIconSptName;
            IconBG.spriteName = data.legionIconBGSptName;
            string colorStr = BalanceResourceUtil.GetUserDiamond() >= AlliancesManager.Instance.Config.RenameCost ? LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
            CoinNumLab.text = string.Format("[{0}]{1}[-]", colorStr, AlliancesManager.Instance.Config.RenameCost);
        }
        /// <summary>
        /// 修改军团名称和图标的核心方法
        /// </summary>
        private void OnClickChangeLegionName()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (BtInput.value.Length <= 1)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_11096"));
                return;
            }
            if (iconID == data.legionIconID && BtInput.value == data.legionName)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionNameEditView_2604"));
                mDMono.gameObject.SetActive(false);
                return;
            }
            if (!EB.ProfanityFilter.Test(BtInput.value))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_10580"));
                return;
            }
            if (BalanceResourceUtil.GetUserDiamond() < AlliancesManager.Instance.Config.RenameCost)
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }

            LTHotfixApi.GetInstance().ExceptionFunc = FuncProcess;//截取异常进行处理
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/alliances/rename");
            request.AddData("aid", LegionModel.GetInstance().legionData.legionID);
            request.AddData("name", BtInput.value);
            request.AddData("iconID", iconID);
            LTHotfixApi.GetInstance().BlockService(request, ResProcess/* (Hashtable resData) => { }*/);//向服务器发送请求上传修改后的信息
                                                                                                                                              //AlliancesManager.Instance.Detail.IconID = iconID;//容错步骤，如果发0给服务器，服务器不会下发该数据，这里先存储，如果服务器不下发就用这里设置的，服务器下发了用服务器的
            mDMono.gameObject.SetActive(false);
        }
        private void ResProcess(Hashtable hs)
        {
            LegionLogic.GetInstance().SendGetAlliance();//向服务器发送请求获得军团信息
        }

        private bool FuncProcess(EB.Sparx.Response response)
        {
            if (response.error == null) return false;
            string error = response.error.ToString();
            if (error == "noChanges")
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionNameEditView_3771"));
                return true;
            }
            if (error == "name_min_len" || error == "need top set name or notice")
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_11096"));
                return true;
            }
            if (error.Equals("ID_ERROR_NAME_EXIST"))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_NAME_EXIST"));
                return true;
            }
            if (error.Equals("allianceWarService Exit"))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionNameEditView_4415"));
                return true;
            }
            return false;
        }

        private void OnClickEditIconBtn()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            IconEditView.SetData(Icon.spriteName, IconBG.spriteName, iconID);
        }

        private void OnClickCloseBtn()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            mDMono.gameObject.SetActive(false);
        }

        private void OnLegionIconEditFunc(int iconId)
        {
            iconID = iconId;
            Icon.spriteName = LegionModel.GetInstance().dicLegionSpriteName[iconID % 100];
            IconBG.spriteName = LegionModel.GetInstance().dicLegionBGSpriteName[iconID / 100];
        }
    }
}

