using System.Collections;
using _HotfixScripts.Utils;
using UnityEngine;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    [SerializeField]
    public class ReelItem
    {
        public UILabel title;
        public UILabel desc;
        public DynamicUISprite icon;
    }
    
    
    public class LTShowReelViewCtrl: UIControllerHotfix,IHotfixUpdate
    {
        public ReelItem[] ReelItems;
        public Animator animator;
        private int start = Animator.StringToHash("start");
        private int selct_0 = Animator.StringToHash("selct_0");
        private int selct_1 = Animator.StringToHash("selct_1");
        private int selct_2 = Animator.StringToHash("selct_2");

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            // t.GetComponent<UIEventTrigger>("Bg").onClick.Add(new EventDelegate(OnCancelButtonClick));
            animator = t.GetComponent<Animator>("Content/RewardGrid");
            ReelItems = new ReelItem[3];
            for (int i = 0; i < ReelItems.Length; i++)
            {
                ReelItems[i] = new ReelItem();    
                ReelItems[i].title =
                    t.GetComponent<UILabel>(string.Format("Content/RewardGrid/{0}/Bg/TitleBg/Title",i));
                ReelItems[i].icon =
                    t.GetComponent<DynamicUISprite>(string.Format("Content/RewardGrid/{0}/Bg/Icon",i));
                ReelItems[i].desc =
                    t.GetComponent<UILabel>(string.Format("Content/RewardGrid/{0}/Bg/Label",i));
            }
            t.GetComponent<ContinueClickCDTrigger>("Content/RewardGrid/0/Bg").m_CallBackPress.Add(new EventDelegate(
                () => { AwardBtnClick(0); }));
            t.GetComponent<ContinueClickCDTrigger>("Content/RewardGrid/1/Bg").m_CallBackPress.Add(new EventDelegate(
                () => { AwardBtnClick(1); }));
            t.GetComponent<ContinueClickCDTrigger>("Content/RewardGrid/2/Bg").m_CallBackPress.Add(new EventDelegate(
                () => { AwardBtnClick(2); }));
        }
        
        public void AwardBtnClick(int index)
        {
            StartCoroutine(PlayEndAnimAndClose(index));
        }

        private IEnumerator PlayEndAnimAndClose(int index)
        {
            switch (index)
            {
                case 0:
                    animator.SetTrigger(selct_0);
                    break;
                case 1:
                    animator.SetTrigger(selct_1);
                    break;
                case 2:
                    animator.SetTrigger(selct_2);
                    break;
            }
            
            yield return new WaitForSeconds(1.5f);
            LTInstanceMapModel.Instance.RequestChallengePickScroll(new []{data.x,data.y},data.list[index], delegate
            {
                LTInstanceMapModel.Instance.RequestGetChapterState(() =>
                {
                    LTInstanceMapModel.Instance.scrollData = null;
                    OnCancelButtonClick();
                });
            });
        }

        private void PlayStartAnim()
        {
            animator.SetTrigger(start);
        }

        private ScrollData data =null;
        private bool isFirst;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            data =LTInstanceMapModel.Instance.scrollData;
            for (int i = 0; i < data.list.Length; i++)
            {
               Data.SkillTemplate skillTpl = Data.SkillTemplateManager.Instance.GetTemplate(data.list[i]);
               ReelItems[i].icon.spriteName = skillTpl.Icon;
               LTUIUtil.SetText(ReelItems[i].title,skillTpl.Name);
               LTUIUtil.SetText(ReelItems[i].desc,skillTpl.Description);
            }

            isFirst = true;
            RegisterMonoUpdater();
            // PlayStartAnim();
        }

        public override void OnCancelButtonClick()
        {
            base.OnCancelButtonClick();
            LTInstanceMapModel.Instance.DealFlyScroll();
        }

        public void Update()
        {
            // if (isFirst && UIStack.Instance.IsLoadingScreenUp==false)
            if(isFirst && IsStartWithCloud())
            {
                PlayStartAnim();
                isFirst = false;
            }
        }

        public bool IsStartWithCloud()
        {
            if (LoadingLogic.Instance!=null)
            {
                return LoadingLogic.Instance.IsStartWithCloud;
            }

            return false;
        }
        
    }
}