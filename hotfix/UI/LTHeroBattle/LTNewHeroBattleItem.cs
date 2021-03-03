using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTNewHeroBattleItem : DynamicMonoHotfix
    {
        public Action<LTNewHeroBattleItem> OnAtkClick;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            CanChallenge = t.FindEx("CanChallenge").gameObject;
            HasChallenge = t.FindEx("HasChallenge").gameObject;
            
            EmenyList = new List<LTNewHeroBattleEnemyItem>();
            EmenyList.Add(t.GetMonoILRComponent<LTNewHeroBattleEnemyItem>("Team/Item"));
            EmenyList.Add(t.GetMonoILRComponent<LTNewHeroBattleEnemyItem>("Team/Item (1)"));
            EmenyList.Add(t.GetMonoILRComponent<LTNewHeroBattleEnemyItem>("Team/Item (2)"));
            EmenyList.Add(t.GetMonoILRComponent<LTNewHeroBattleEnemyItem>("Team/Item (3)"));
            
            t.GetComponent<UIButton>("CanChallenge/AtkBtn").onClick.Add(new EventDelegate(() =>
            {
                OnAtkClick?.Invoke(this);
            }));
        }


    
        public GameObject CanChallenge;
        public GameObject HasChallenge;
        public List<LTNewHeroBattleEnemyItem> EmenyList;
        private int index = -1;
        public int Index
        {
            private set { index = value; }
            get { return index; }
        }
    
        public void Fill(int inde,bool isFinish,List<string> emenyList)
        {
            index = inde;
            CanChallenge.CustomSetActive(!isFinish);
            HasChallenge.CustomSetActive(isFinish);
            for (int i = 0; i < emenyList.Count; i++)
            {
                EmenyList[i].Fill(emenyList[i]);
            }
            for (int i = emenyList.Count; i<EmenyList.Count; i++)
            {
                EmenyList[i].Clear();
            }
            mDMono.gameObject.CustomSetActive(true);
            mDMono.gameObject.GetComponent<TweenAlpha>().ResetToBeginning();
            mDMono.gameObject.GetComponent<TweenAlpha>().PlayForward();
        }
    
        public void Clear()
        {
            index = -1;
            mDMono.gameObject.GetComponent<UIWidget>().alpha =0;
            mDMono.gameObject.CustomSetActive(false);
        }
    
        
    }
}
