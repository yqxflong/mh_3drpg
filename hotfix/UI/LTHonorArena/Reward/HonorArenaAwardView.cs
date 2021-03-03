using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class HonorArenaAwardView : DynamicMonoHotfix
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
    
        
        //活动表 
        public void SetData(List<HonorArenaReward> rewards)
        {
            LTUIUtil.SetText(recordTipsLabel, EB.Localizer.GetString("ID_HONOR_ARENA_REWARD_TIP"));//"每周日21：00邮件发送奖励"
            LTUIUtil.SetNumTemplateFromMonoILR<HBAwardItem>(listAwardItem[0], listAwardItem, rewards.Count, -awardItemBehind);
    
            for(int i=0;i<rewards.Count;i++)
            {
                LTUIUtil.SetNumTemplateFromMonoILR<LTShowItem>(listAwardItem[i].listShowItem[0], listAwardItem[i].listShowItem, rewards[i].listShowItemData.Count, listAwardItem[i].showItemBehind, false);
                for(int j=0;j< rewards[i].listShowItemData.Count;j++)
                {
                    listAwardItem[i].listShowItem[j].LTItemData = rewards[i].listShowItemData[j];
                }

                string template_01 = EB.Localizer.GetString("ID_HONOR_ARENA_RANK_TIP_1");// "跨服\n第{0}名";
                string template_02 = EB.Localizer.GetString("ID_HONOR_ARENA_RANK_TIP_2"); // "本服\n第{0}名";
                string template_03 = EB.Localizer.GetString("ID_HONOR_ARENA_RANK_TIP_3");  // "本服\n第{0}-{1}名";
                  
                if (rewards[i].id==1)
                {
                    LTUIUtil.SetText(listAwardItem[i].recordLabel, string.Format(template_01, rewards[i].top));
                }else if (rewards[i].top == rewards[i].down)
                {
                    LTUIUtil.SetText(listAwardItem[i].recordLabel, string.Format(template_02, rewards[i].top));
                }
                else
                {
                    LTUIUtil.SetText(listAwardItem[i].recordLabel, string.Format(template_03, rewards[i].top, rewards[i].down));
                }
            }
        }
    }
}
