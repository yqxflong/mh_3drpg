
using System;
using EB;
using Hotfix_LT.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix_LT.UI
{
    public class EnterVigorController : DynamicMonoHotfix
    {
        private UILabel OldVigorText;
        private UILabel NewVigorText;
        private Vector3 OldVigorTextPosition;
        private Vector3 NewVigorTextPosition;

        private Vector3 CenterPosition;

        public override void Awake()
        {
            OldVigorText = mDMono.transform.Find("Old").GetComponent<UILabel>();
            NewVigorText = mDMono.transform.Find("New").GetComponent<UILabel>();
            OldVigorTextPosition = OldVigorText.transform.localPosition;
            NewVigorTextPosition = NewVigorText.transform.localPosition;

            CenterPosition = OldVigorTextPosition;
        }

        public void Init(int OldVigor, int NewVigor, bool isExtramReward)
        {
            if (OldVigorText == null || NewVigorText == null)
            {
                return;
            }

            if (isExtramReward)
            {
                LTUIUtil.SetText(OldVigorText, OldVigor.ToString());
                LTUIUtil.SetText(NewVigorText, NewVigor.ToString());
                OldVigorText.gameObject.CustomSetActive(true);
                NewVigorText.transform.localPosition = NewVigorTextPosition;
            }
            else
            {
                LTUIUtil.SetText(NewVigorText, OldVigor.ToString());
                OldVigorText.gameObject.CustomSetActive(false);
                NewVigorText.transform.localPosition = CenterPosition;
            }
        }
    }
}