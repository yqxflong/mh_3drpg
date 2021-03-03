using UnityEngine;

namespace Hotfix_LT.UI
{
    public class PlayerVigourTip : DynamicMonoHotfix
    {
        public static bool isShow = true;
        //private Transform playerName;
       // private Transform playerList;

        public void SetVigourPos(Transform parent)
        {
            var t = mDMono.transform;
            //playerList = t.parent.Find("PlayerHeadBarList");
            //playerName = playerList.GetChild(playerList.childCount - 1).Find("BG/UName");
            //t.SetParent(playerName);
            t.SetParent(parent);
            t.localPosition = new Vector3(0, -155, 0);
            t.localScale = Vector3.one;
            
        }

        public override void Awake()
        {
            base.Awake();
            mDMono.transform.GetComponent<ConsecutiveClickCoolTrigger>("Sprite").clickEvent.Add(new EventDelegate(OnClickVigourBtn));
        }

        public void OnClickVigourBtn()
        {
            GlobalMenuManager.Instance.Open("LTPlayerLevelUpTipView", "Vigour");
            mDMono.gameObject.CustomSetActive(false);
            isShow = false;
        }

        public void ShowLevelupVigourTip()
        {
            mDMono.gameObject.CustomSetActive(false);

            if (isShow)
            {
                int curVigour;
                int VigourLimit;
                DataLookupsCache.Instance.SearchIntByID("res.vigor.v", out curVigour);
                DataLookupsCache.Instance.SearchIntByID("res.vigor.max", out VigourLimit);

                //大于或等于体力上限时并且等级大于7
                if (curVigour >= VigourLimit && BalanceResourceUtil.GetUserLevel() > 7)
                {
                    mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    mDMono.gameObject.CustomSetActive(false);
                    isShow = false;
                }
            }
        }
    }
}
