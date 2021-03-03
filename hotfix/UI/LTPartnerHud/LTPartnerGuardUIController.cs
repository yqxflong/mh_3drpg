using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerGuardUIController: UIControllerHotfix
    {
        public LTPartnerGuardAttrController AttrController;
        public LTPartnerGuardHeroItem[] HeroItem;
        public LTPartnerData ownData;
        private LTPartnerData last_ownData;
        public GuardHeroData[] datas = new GuardHeroData[3];
        private Color32 normalColor= new Color32(212, 224, 231, 255); //D4E0E7
        private Color32 selectColor= new Color32(72, 192, 255, 255); //48C0FF
        private int curSelect { get; set; }

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
        
        public override void Awake()
        {
            base.Awake();
            AttrController = controller.transform.GetMonoILRComponent<LTPartnerGuardAttrController>("Right");
            curSelect = -1;
            HeroItem = new LTPartnerGuardHeroItem[3];
            HeroItem[0] = controller.transform.GetMonoILRComponent<LTPartnerGuardHeroItem>("Left/Cur");
            HeroItem[1] = controller.transform.GetMonoILRComponent<LTPartnerGuardHeroItem>("Left/Cur (1)");
            HeroItem[2] = controller.transform.GetMonoILRComponent<LTPartnerGuardHeroItem>("Left/Cur (2)");
            
            HeroItem[0].mDMono.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => { ClickTitle(0);}));
            HeroItem[1].mDMono.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => { ClickTitle(1);}));
            HeroItem[2].mDMono.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => { ClickTitle(2);}));
            
            UIButton backButton = controller.transform.GetComponent<UIButton>("BG/Top/CloseBtn");
            backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
            UIButton ruleButton = controller.transform.GetComponent<UIButton>("BG/Top/RuleBtn");
            ruleButton.onClick.Add(new EventDelegate(OnRuleButtonClick));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            last_ownData = ownData;
            ownData= param as LTPartnerData;
            datas[0] = LTPartnerDataManager.Instance.GetLTPartnerDataByID(ownData.HeroStat.HeroFetter1);
            datas[1] = LTPartnerDataManager.Instance.GetLTPartnerDataByID(ownData.HeroStat.HeroFetter2);
            datas[2] = LTPartnerDataManager.Instance.GetLTPartnerDataByID(ownData.HeroStat.HeroFetter3);
            for (int i = 0; i < AttrController.fxs.Length; i++)
            {
                AttrController.fxs[i].gameObject.SetActive(false);
                AttrController.fxs_label[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < datas.Length; i++)
            { 
                var item = HeroItem[i];
                item.SetHeroIcon(ownData,datas[i],i+1);
            }

            ClickTitle(0);

        }

        public void OnRuleButtonClick()
        {
            string text = EB.Localizer.GetString("ID_RULE_GUARD");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        public void ClickTitle(int index)
        {
            // if (index==curSelect && last_ownData==ownData)
            // {
            //     return;
            // }
            curSelect = index;
            AttrController.SetData(curSelect+1);
            for (int i = 0; i < HeroItem.Length; i++)
            {
                if (i==index)
                {
                     HeroItem[i].BgSprite.color = selectColor;
                }
                else
                {
                     HeroItem[i].BgSprite.color = normalColor;
                }
            }
        }
    }
}