using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class PlayerUpgradeTipController : UIControllerHotfix
    {
        private int maxwidth = 578;
        private UISprite preFrame, curFrame, progressSp;
        private UILabel preUpgrade, curUpgrade, tipTitle, tipTitleShadow, progressLabel, progressLabelShadow;
        private GameObject preUpObj, curUpObj, progressObj;
        private int delayCloseTimer;
        public override void Awake()
        {
            Transform t = controller.transform;
            curFrame = t.GetComponent<UISprite>("UpgradeTip/PartnerUpgradeTip/Item (1)/Frame");
            preFrame = t.GetComponent<UISprite>("UpgradeTip/PartnerUpgradeTip/Item (2)/Frame");
            progressSp = t.GetComponent<UISprite>("ps/psbar");
            curUpgrade = t.GetComponent<UILabel>("UpgradeTip/PartnerUpgradeTip/Item (1)/BreakObj/Break");
            preUpgrade = t.GetComponent<UILabel>("UpgradeTip/PartnerUpgradeTip/Item (2)/BreakObj/Break");
            tipTitle = t.GetComponent<UILabel>("TextureBg/DesLabel");
            tipTitleShadow = t.GetComponent<UILabel>("TextureBg/DesLabel/DesLabel (1)");
            progressLabel = t.GetComponent<UILabel>("ps/Label");
            progressLabelShadow = t.GetComponent<UILabel>("ps/Label/Label (1)");
            curUpObj = t.GetComponent<Transform>("UpgradeTip/PartnerUpgradeTip/Item (1)/BreakObj").gameObject;
            preUpObj = t.GetComponent<Transform>("UpgradeTip/PartnerUpgradeTip/Item (2)/BreakObj").gameObject;
            progressObj = t.GetComponent<Transform>("ps/psbar").gameObject;
            t.GetComponent<UIButton>("UpgradeTip/GoButton").onClick.Add(new EventDelegate(GoToUpgrade));
        }

        public override void OnDestroy()
        {
            if (delayCloseTimer != 0)
            {
                ILRTimerManager.instance.RemoveTimerSafely(ref delayCloseTimer);
            }
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
            int[] data = param as int[];
            if (data == null || data.Length < 3)
            {
                EB.Debug.LogError("data is null or not complete");
                controller.Close();
                return;
            }
            SetData(data[0], data[1], data[2]);
        }
        private void SetData(int Upgradeid, int targetNum, int curNum)
        {
            string tempStr;
            string gradename = Data.CharacterTemplateManager.Instance.GetUpGradeInfo(Upgradeid, Data.eRoleAttr.Feng).name;
            if (EB.Localizer.TryFetchString("ID_PARTNER_UPGRADE_TIP_13", out tempStr))
            {
                tipTitle.text = tipTitleShadow.text =  string.Format(tempStr, targetNum, gradename);
            }
            SetItemInfo(Upgradeid-1,curFrame, curUpgrade, curUpObj);
            SetItemInfo(Upgradeid,preFrame,preUpgrade, preUpObj);
            if (curNum > 0)
            {
                progressSp.width = maxwidth * curNum / targetNum;
                progressObj.CustomSetActive(true);
            }
            else
            {
                progressObj.CustomSetActive(false);
            }
            progressLabel.text =  progressLabelShadow .text= string.Format("{0}/{1}", curNum, targetNum);
        }


        private void SetItemInfo(int Upgradeid, UISprite frame,UILabel upgradelable, GameObject upgradeobj)
        {
            int quality;
            int addLevel;
            string spName;
            LTPartnerDataManager.GetPartnerQuality(Upgradeid, out quality, out addLevel);
            if (LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC.TryGetValue(quality, out spName))
            {
                frame.spriteName = spName;
            }
            if (addLevel > 0)
            {
                upgradelable.text = string.Format("+{0}", addLevel);
                upgradeobj.CustomSetActive(true);
            }
            else
            {
                upgradeobj.CustomSetActive(false);
            }
        }

        private void GoToUpgrade()
        {
            GlobalMenuManager.Instance.Open("LTPartnerHud", "Develop_Upgrade");
            delayCloseTimer = ILRTimerManager.instance.AddTimer(1000, 1, CloseView);
        }

        private void CloseView(int seq)
        {
            controller.Close();
            delayCloseTimer = 0;
        }
    }
}