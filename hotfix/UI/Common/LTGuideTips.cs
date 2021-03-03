using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTGuideTips : DynamicMonoHotfix
    {
        public static bool IsEnableGuideTips { get; set; } = false;
        public static int MainInstanceCampaignId { get; set; }

        private Transform _transform;

        public override void Awake()
        {
            base.Awake();

            _transform = mDMono.transform;
            _transform.localScale = Vector3.zero;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            
            if (GuideNodeManager.IsGuide)
            {
                _transform.localScale = Vector3.zero;
                IsEnableGuideTips = false;
                MainInstanceCampaignId = 0;
            }

            Show();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _transform.localScale = Vector3.zero;
            IsEnableGuideTips = false;
        }

        private UILabel _labTips;
        private UIButton _btnGoto;

        private void SetData(string text, string guideFailState)
        {
            if (_labTips == null)
            {
                _labTips = _transform.GetComponent<UILabel>("Lab_Tips");
            }

            if (_labTips != null)
            {
                _labTips.text = text;
            }

            if (_btnGoto == null)
            {
                _btnGoto = _transform.GetComponent<UIButton>("Btn_Goto");
            }

            if (_btnGoto != null)
            {
                _btnGoto.onClick.Add(new EventDelegate(() => {
                    GlobalMenuManager.Instance.ClearCache();
                    LTInstanceMapModel.Instance.ClearInstanceData();
                    LTInstanceMapModel.Instance.RequestLeaveChapter("main", () => {
                        GuideNodeManager.IsGuide = true;
                        GuideNodeManager.GuideFailState = guideFailState;
                        GlobalMenuManager.Instance.ComebackToMianMenu();
                    });
                }));
            }
        }

        private void Show()
        {
            if (!IsEnableGuideTips || Data.FuncTemplateManager.Instance.GetFunc(10097).IsConditionOK())
            {
                return;
            }

            IsEnableGuideTips = false;
            var guideFailState = GetGuideType();

            switch (guideFailState)
            {
                case "Level":
                    _transform.localScale = Vector3.one;
                    SetData(EB.Localizer.GetString("ID_UPGRADE_TIPS_LEVEL"), guideFailState);
                    break;
                case "Equipment":
                    _transform.localScale = Vector3.one;
                    SetData(EB.Localizer.GetString("ID_UPGRADE_TIPS_EQUIPMENT"), guideFailState);
                    break;
                default:
                    _transform.localScale = Vector3.zero;
                    break;
            }
        }

        private string GetGuideType()
        {
            bool isLack = false;

            switch (SceneLogic.BattleType)
            {
                case eBattleType.MainCampaignBattle:
                    var tpl = Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(MainInstanceCampaignId.ToString());

                    if (tpl != null) 
                    {
                        int stars;

                        if (DataLookupsCache.Instance.SearchIntByID(string.Format("userCampaignStatus.normalChapters.{0}.campaigns.{1}.star", tpl.ChapterId, MainInstanceCampaignId), out stars))
                        {
                            isLack = stars < 3;
                        }
                    }
                    
                    break;
            }

            string result = "None";

            if (isLack && LTPartnerDataManager.Instance != null)
            {
                if (LTPartnerDataManager.Instance.HasPartnerCanLevelUp())
                {
                    result = "Level";
                }
                else if (LTPartnerDataManager.Instance.HasEquipmentCanDress())
                {
                    result = "Equipment";
                }
            }

            return result;
        }
    }
}
