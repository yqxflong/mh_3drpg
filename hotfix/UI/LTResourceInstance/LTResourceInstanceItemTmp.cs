using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTResourceInstanceItemTmp : DynamicMonoHotfix
    {
        private GameObject LockObj;
        private UISprite ItemSprite;
        private Color GreyColor;
        private Color NormalColor;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            LockObj = t.FindEx("Lock").gameObject;
            ItemSprite = t.GetComponent<UISprite>();
            GreyColor = Color.grey;
            NormalColor = Color.white;
        }

        public void InitState(Hotfix_LT.Data.SpecialActivityLevelTemplate data, ResourceInstanceType type)
        {
            if (data == null)
            {
                return;
            }

            bool isLock = LTResourceInstanceManager.Instance.IsLock(data, type);
            ItemSprite.color = isLock ? GreyColor : NormalColor;
            LockObj.SetActive(isLock);
        }
    }
}
