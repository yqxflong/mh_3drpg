using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hotfix_LT.UI
{
    public class LegionDonateAni : DynamicMonoHotfix
    {
        public class AniItem
        {
            public GameObject gameObject;
            public Transform transForm;
            public UILabel expValueLab;
            public UILabel coinValueLab;
            public UILabel medalValueLab;
            public UITweener[] tweener;
        }

        private AniItem[] aniItems;

        private void Init()
        {
            aniItems = new AniItem[mDMono.transform.childCount];
            for (int i = 0; i < aniItems.Length; i++)
            {
                AniItem item = new AniItem();
                item.transForm = mDMono.transform.GetChild(i);
                item.gameObject = item.transForm.gameObject;
                item.expValueLab = item.transForm.Find("ExpSp/ExpLab").GetComponent<UILabel>();
                item.coinValueLab = item.transForm.Find("CoinSp/CoinLab").GetComponent<UILabel>();
                item.medalValueLab = item.transForm.Find("MedalSp/CoinLab").GetComponent<UILabel>();
                item.tweener = item.transForm.GetComponentsInChildren<UITweener>();
                aniItems[i] = item;
            }
        }

        public void PlayAni(int expValue, int coinValue)
        {
            AniItem item = GetAniItem();
            if (item == null)
            {
                return;
            }
            string numlab = string.Format("+{0}", expValue);
            item.expValueLab.text = numlab;
            item.coinValueLab.text = numlab;
            item.medalValueLab.text = numlab;
            for (int i = 0; i < item.tweener.Length; i++)
            {
                item.tweener[i].ResetToBeginning();
                item.tweener[i].PlayForward();
            }
        }

        private AniItem GetAniItem()
        {
            if (aniItems == null)
            {
                Init();
            }

            for (int i = 0; i < aniItems.Length; i++)
            {
                bool isReturn = true;
                for (int j = 0; j < aniItems[i].tweener.Length; j++)
                {
                    if (aniItems[i].tweener[j].isActiveAndEnabled)
                    {
                        isReturn = false;
                        break;
                    }
                }
                if (isReturn)
                {
                    return aniItems[i];
                }
            }

            return null;
        }
    }
}