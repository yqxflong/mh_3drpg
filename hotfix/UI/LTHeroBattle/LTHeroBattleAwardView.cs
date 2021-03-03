using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTHeroBattleAwardView : DynamicMonoHotfix
    {
        public List<HBAwardItem> listAwardItem;
        public int awardItemBehind;
        public UIScrollView scrollView;
        public UIButton backBtn;
        public UILabel recordTipsLabel;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            awardItemBehind = -270;
            scrollView = t.GetComponent<UIScrollView>("Scroll View");
            backBtn = t.GetComponent<UIButton>("LTFrame/Content/Title/CloseBtn");
            recordTipsLabel = t.GetComponent<UILabel>("Record_Label (1)");
            listAwardItem = new List<HBAwardItem>();
            listAwardItem.Add(t.GetMonoILRComponent<HBAwardItem>("Scroll View/AwardItem"));
            
            backBtn.onClick.Add(new EventDelegate(OnClickBack));
            t.GetComponent<UIEventTrigger>("Background").onClick.Add(new EventDelegate(OnClickBack));
        }

        public override void OnDestroy()
        {
            backBtn.onClick.Clear();
        }
    
        public void OnClickBack()
        {
           mDMono.gameObject.CustomSetActive(false);
        }
    
        public void SetData(List<HeroBattleLevelReward> rewards,int winNum)
        {
    
		LTUIUtil.SetText(recordTipsLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LTHeroBattleAwardView_664"),winNum));
            LTUIUtil.SetNumTemplateFromMonoILR<HBAwardItem>(listAwardItem[0], listAwardItem, rewards.Count, -awardItemBehind);
    
            for(int i=0;i<rewards.Count;i++)
            {
                LTUIUtil.SetNumTemplateFromMonoILR<LTShowItem>(listAwardItem[i].listShowItem[0], listAwardItem[i].listShowItem, rewards[i].listShowItemData.Count, listAwardItem[i].showItemBehind, false);
                for(int j=0;j< rewards[i].listShowItemData.Count;j++)
                {
                    listAwardItem[i].listShowItem[j].LTItemData = rewards[i].listShowItemData[j];
                }
    
                LTUIUtil.SetText(listAwardItem[i].recordLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LTHeroBattleAwardView_1296"), rewards[i].id));
            }
        }
    }
}
