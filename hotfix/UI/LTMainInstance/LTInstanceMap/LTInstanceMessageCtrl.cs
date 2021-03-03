using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class LTInstanceMessageCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("Bg/Top/CancelBtn");

            ItemList = new List<LTInstanceMessageCell>();
            ItemList.Add (t.GetMonoILRComponent<LTInstanceMessageCell>("Grid/Item"));
            ItemList.Add(t.GetMonoILRComponent<LTInstanceMessageCell>("Grid/Item (1)"));
            ItemList.Add(t.GetMonoILRComponent<LTInstanceMessageCell>("Grid/Item (2)"));
        }
        public List<LTInstanceMessageCell> ItemList;
    
        private int mLevel;
    
        private int mRoleId;
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override void SetMenuData(object param)
        {
            mRoleId = int.Parse(param.ToString());
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            var msgList = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostInstanceMsgList(mRoleId);
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (i < msgList.Count)
                {
                    ItemList[i].mDMono.gameObject .CustomSetActive(true);
                    ItemList[i].SetData(msgList[i]);
                }
                else
                {
                    ItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    }
}
