using EB;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class QuicklyGetUpgradeMaterialController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
           t.GetComponent<UIButton>("Content/Bg/TopSprite/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
        }
    
        private LTPartnerData curPartnerData;
        private Dictionary<string,int> materialList = new Dictionary<string, int>();
        private ShowUpgradeMaterialState[] showitemArray = new ShowUpgradeMaterialState[4];
        private bool isExDouble = false;
        private bool isSceneDouble = false;
        public const string GoldBoxId = "1010";

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
        public override void SetMenuData(object param)
        {
            if(param == null)
            {
                return;
            }
            for (int i = 0; i < showitemArray.Length; i++)
            {
                showitemArray[i] = controller.transform.Find("Content/UpgradeMaterial/Grid/Material (" + i + ")").GetMonoILRComponent<ShowUpgradeMaterialState>();
            }
          
            curPartnerData = param as LTPartnerData;
            Hotfix_LT.Data.UpGradeInfoTemplate evoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(curPartnerData.UpGradeId + 1, curPartnerData.HeroInfo.char_type);
            materialList = evoTpl.materialDic;
            isExDouble = ActivityUtil.isTimeLimitActivityOpen(1003);
            isSceneDouble = ActivityUtil.isTimeLimitActivityOpen(1002);
        }
    
        public override IEnumerator OnAddToStack()
        {
            Show();
            yield return base.OnAddToStack();
        }
       
        public void Show()
        {
            int i = 0;
            foreach (var item in materialList)
            {
                showitemArray[i].mDMono.transform.gameObject.CustomSetActive(true);
                //showitemArray[i].Fill(int.Parse(item.Key), item.Value);
                i++;
            }
            for (; i < showitemArray.Length; i++)
            {
                showitemArray[i].mDMono.transform.gameObject.CustomSetActive(false);
            }
            //RefreshVigour();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            yield return base.OnRemoveFromStack();
        }
    
        public override void OnCancelButtonClick()
        {
            base.OnCancelButtonClick();
        }
        public void FillMaterial()
        {
            int i = 0;
            foreach (var item in materialList)
            {
                showitemArray[i].Fill(int.Parse(item.Key), item.Value, isExDouble, isSceneDouble);
                i++;
            }
        }
    }
    
    
}
