using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{

    public class HandbookItemCellCtrl : DynamicMonoHotfix
    {
        public Hotfix_LT.Data.eRoleAttr Type;

        private UISprite Icon;
        private UILabel LevelLabel;
        private GameObject LockRootObj;
        private UISprite LockSprite;
        private BoxCollider Box;
        private GameObject RedPoint;
        
        public override void Awake()
        {
            base.Awake();
            Icon = mDMono.transform.Find("Icon").GetComponent<UISprite>();

            LockRootObj = mDMono.transform.Find("LockRoot").gameObject;
            LockSprite = LockRootObj.transform.Find("Lock").GetComponent<UISprite>();
            LevelLabel = mDMono.transform.Find("Level").GetComponent<UILabel>();
            RedPoint = mDMono.transform.Find("RedPoint").gameObject;
            mDMono.transform.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnItemClick));
            Box = mDMono.GetComponent<BoxCollider>();
            Box.enabled = false;
        }

        public void SetType(Hotfix_LT.Data.eRoleAttr e)
        {
            Type = e;
            Icon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[Type]; 
        }

        public void FillData(bool isLock, bool isUplevel)
        {
            if (isLock)
            {
                LockRootObj.CustomSetActive(true);

                Box.enabled = false;
                LevelLabel.gameObject.CustomSetActive(false);
                LevelLabel.text = string.Empty;
            }
            else
            {
                if (isUplevel && LockRootObj.activeSelf)
                {
                    EB.Coroutines .Run(UnLockCoroutine());
                }
                else
                {
                    ResetState();
                }
            }
        }

        private void ResetState()
        {
            LockRootObj.CustomSetActive(false);
            Box.enabled = true;
            isPlaying = false;
            LevelLabel.gameObject.CustomSetActive(true);
            LevelLabel.text = string.Format(EB.Localizer.GetString("ID_LEVEL_FORMAT"), LTPartnerHandbookManager.Instance.GetBeralLevel(Type));
            //UIShowItem.ShowCharTypeFX(charFx, efClip, Icon.transform, PartnerGrade.SSR, Type);
            RedPoint.CustomSetActive(LTPartnerHandbookManager.Instance.IsHandBookCanBreakUp(Type)|| LTPartnerHandbookManager.Instance.IsHandPartnerCanUp(Type));
        }

        bool isPlaying = false;
        IEnumerator UnLockCoroutine()
        {
            if (isPlaying) yield break;
            isPlaying = true;
            yield return new WaitForSeconds(1.5f);
            ResetState();
            yield break;
        }

        public void OnItemClick()
        {
            if (Type == Data.eRoleAttr.None) return;
            GlobalMenuManager.Instance.Open("LTPartnerHandbookDetailView", Type);
        }

    }

}
