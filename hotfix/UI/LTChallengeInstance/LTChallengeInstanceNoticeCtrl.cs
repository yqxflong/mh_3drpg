using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceNoticeCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            Sprite = t.GetComponent<UISprite>("Icon");
            NameLabel = t.GetComponent<UILabel>("Bg/Top/Title");
            DescLabel = t.GetComponent<UILabel>("Content/Label");
            controller.backButton = t.GetComponent<UIButton>("Bg/Top/CancelBtn");
        }

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
    
        public UISprite Sprite;
    
        public UILabel NameLabel;
    
        public UILabel DescLabel;
    
        private int mRoleId;
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            mRoleId = (int)param;
            var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeChapterRole(mRoleId);
            if (tpl != null)
            {
                if (tpl.Guide[0] == "#")
                {
                    tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeChapterRole(int.Parse(tpl.Guide[1]));
                }
                NameLabel.text = tpl.Guide[0];
                DescLabel.text = tpl.Guide[1];
                Sprite.spriteName = tpl.Img.Replace("|RoleShake", string.Empty);
            }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    }
}
