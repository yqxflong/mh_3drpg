using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    public class LTWelfareHudController : UIControllerHotfix
    {
        private GameObject FirstChargeRP;
        private GameObject SevenDayRP;
        private GameObject GrowUpRP;
        private GameObject LevelAwardRP;
        private GameObject HeroMedalRP;
        private GameObject DiamondGiftRP;
        private GameObject MainInstanceGiftRP;

        private bool hadSendRq = false;

        public override bool IsFullscreen()
        {
            return true;
        }
        public static bool isOpen;
        private UITabController tabController;
        private UIProgressBar progresBar;
        private GameObject ArrowObj;
        private bool isMoreThenScrollView;

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            UIButton backBtn= t.Find("FrameBG/Panel/CancelBtn").GetComponent<UIButton>();
            controller.backButton = backBtn;
            tabController = t.Find("Content/ScrollView/Placeholder/ButtonList").GetComponent<UITabController>();
            FirstChargeRP =t.Find("Content/ScrollView/Placeholder/ButtonList/0_shouchong/Btn/RedPoint").gameObject;
            SevenDayRP =t.Find("Content/ScrollView/Placeholder/ButtonList/1_qiri/Btn/RedPoint").gameObject;
            GrowUpRP  = t.Find("Content/ScrollView/Placeholder/ButtonList/2_chengzhang/Btn/RedPoint").gameObject;
            LevelAwardRP = t.Find("Content/ScrollView/Placeholder/ButtonList/3_dengji/Btn/RedPoint").gameObject;
            HeroMedalRP = t.Find("Content/ScrollView/Placeholder/ButtonList/4_yinxiong/Btn/RedPoint").gameObject;
            DiamondGiftRP =t.Find("Content/ScrollView/Placeholder/ButtonList/5_zuanshi/Btn/RedPoint").gameObject;
            MainInstanceGiftRP= t.Find("Content/ScrollView/Placeholder/ButtonList/6_zhuxian/Btn/RedPoint").gameObject;

            progresBar = t.GetComponent<UIProgressBar>("Content/ScrollView/UIScrollBar");
            ArrowObj = t.FindEx("Content/Arrow").gameObject;
            progresBar.onChange.Add(new EventDelegate(OnChange));
        }

        public void OnChange()
        {
            if (isMoreThenScrollView)
            {
                ArrowObj.CustomSetActive(progresBar.value < 0.98f);
            }
            else
            {
                ArrowObj.CustomSetActive(false);
            }
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            ResetView();
        }

        private void ResetView()
        {
            int tabIndex = 0;
            int count = 0;
            //添加按钮隐藏判断条件
            for (int i = 1; i < tabController.TabLibPrefabs.Count; i++)
            {
                if (!LTWelfareModel .Instance.JudgeViewClose(i))
                {
                    if (tabIndex == 0) tabIndex = i;
                    count++;
                }
                else
                {
                    tabController.TabLibPrefabs[i].TabObj.transform.parent.gameObject.CustomSetActive(false);
                }
            }
            isMoreThenScrollView = count > 5;
            tabController.SelectTab(tabIndex);
            tabController.GetComponent<UIGrid>().repositionNow = true;
        }

        


        public override void OnFocus()
        {
            base.OnFocus();

            //LTWelfareModel.Instance.UpdataTasks();

            if (!hadSendRq)
            {
                //图鉴任务需要客户端先计算，再决定是否拉代码
                List<LTWelfareGrowUpTaskData> levelList = LTWelfareModel.Instance.GetHandBookTasks();
                if (LTWelfareGrowUpController.DayJudge())
                {
                    bool needResetHandBookData = false;
                    if (levelList.Count > 0)
                    {
                        for (int i = 0; i < levelList.Count; i++)
                        {
                            if (!levelList[i].Finished)
                            {
                                Hotfix_LT.Data.TaskTemplate TaskTpl = levelList[i].TaskTpl;
                                List<int> levels = LTPartnerHandbookManager.Instance.GetHandbookLevelList();
                                int id = int.Parse(TaskTpl.target_parameter_2);
                                int count = int.Parse(TaskTpl.target_parameter_3);
                                int curCount = 0;
                                for (int j = 0; j < levels.Count; j++)
                                {
                                    if (levels[j] >= id) curCount++;
                                    if (count == curCount)
                                    {
                                        needResetHandBookData = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (needResetHandBookData)
                    {
                        LTWelfareModel.Instance.ResetHandBookData(delegate (bool success)
                        {
                            if (success)
                            {
                                hadSendRq = true;
                                LTWelfareModel.Instance.UpdataTasks();
                            }
                        });
                    }
                }
            }

            if (LTWelfareEvent.WelfareGrowUpUpdata != null)
            {
                LTWelfareEvent.WelfareGrowUpUpdata();
            }
            if (LTWelfareEvent.WelfareOnfocus != null)
            {
                LTWelfareEvent.WelfareOnfocus();
            }

            InitRedPoint();
        }

        public void InitRedPoint()
        {
            LTWelfareModel.Instance.Welfare_FirstCharge(true);
            LTWelfareModel.Instance.Welfare_SevenDay();
            LTWelfareModel.Instance.Welfare_GrowUpAward();
            LTWelfareModel.Instance.Welfare_LevelAward();
            LTWelfareModel.Instance.Welfare_HeroMedal();
            LTWelfareModel.Instance.Welfare_DiamondGift();
            LTWelfareModel.Instance.Welfare_MainInstanceGift();
        }
        
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            isOpen = true;
            LTWelfareEvent.WelfareHadFirstCharge += ResetView;

            Hotfix_LT.Messenger.Raise(EventName.LTWelfareHudOpen);

            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.firstcharge, SetFirstChargeRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.sevenreward , SetSevenDayRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.growupaward, SetGrowUpRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.levelaward, SetLevelAwardRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.heromedal, SetHeroMedalRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.diamondgift, SetDiamondGiftRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.maininstancegift, SetMainInstanceGiftRP);
        }

        private void SetFirstChargeRP(RedPointNode node )
        {
            FirstChargeRP.CustomSetActive(node.num > 0);
        }
        private void SetSevenDayRP(RedPointNode node)
        {
            SevenDayRP.CustomSetActive(node.num > 0);
        }
        private void SetGrowUpRP(RedPointNode node)
        {
            GrowUpRP.CustomSetActive(node.num > 0);
        }
        private void SetLevelAwardRP(RedPointNode node)
        {
            LevelAwardRP.CustomSetActive(node.num > 0);
        }
        private void SetHeroMedalRP(RedPointNode node)
        {
            HeroMedalRP.CustomSetActive(node.num > 0);
        }
        private void SetDiamondGiftRP(RedPointNode node)
        {
            DiamondGiftRP.CustomSetActive(node.num > 0);
        }
        private void SetMainInstanceGiftRP(RedPointNode node)
        {
            MainInstanceGiftRP.CustomSetActive(node.num > 0);
        }

        public override IEnumerator OnRemoveFromStack()
        {
            isOpen = false;
            LTWelfareEvent.WelfareHadFirstCharge -= ResetView;
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.firstcharge, SetFirstChargeRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.sevenreward, SetSevenDayRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.growupaward, SetGrowUpRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.levelaward, SetLevelAwardRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.heromedal, SetHeroMedalRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.diamondgift, SetDiamondGiftRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.maininstancegift, SetMainInstanceGiftRP);
            DestroySelf();
            return base.OnRemoveFromStack();
        }
    }
}