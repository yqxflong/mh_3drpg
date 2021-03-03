
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ArtifactDetailController : UIControllerHotfix
    {
        public DynamicUISprite Icon;
        public GameObject Shadow;
        public UILabel ArtifactName;
        public UILabel ShowItemCount;
        public LTShowItem ShowItem;
        public ArtifactDetailBehaviour DetailBehaviour;
        public ConsecutiveClickCoolTrigger button;
        public GameObject redPoint;
        public GameObject MaxTip;
        
        
        private int infoId;
        private LTPartnerData data;
        private ArtifactEquipmentTemplate templateNow;
        private ArtifactEquipmentTemplate templateNext;

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
         
            Icon = t.GetComponent<DynamicUISprite>("Icon");
            Shadow = Icon.transform.Find("Icon").gameObject;
            ArtifactName = t.GetComponent<UILabel>("Name");
            DetailBehaviour = t.GetMonoILRComponent<ArtifactDetailBehaviour>("Container/Bg");
            ShowItem = t.GetMonoILRComponent<LTShowItem>("LTShowItem");
            ShowItemCount = t.GetComponent<UILabel>("LTShowItem/CountLabel");
            MaxTip = t.FindEx("Container/Bg/MaxTip").gameObject;
            t.GetComponent<UIButton>("BG/CancelBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            button = t.GetComponent<ConsecutiveClickCoolTrigger>("Container/Bg/UpLevelBtn");
            redPoint = button.transform.Find("RedPoint").gameObject;
            button.clickEvent.Add(new EventDelegate(OnClickUpgrade));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            
            infoId =(int?) param ?? infoId;
            data = LTPartnerDataManager.Instance.GetPartnerByInfoId(infoId);
            templateNow = CharacterTemplateManager.Instance.GetArtifactEquipmentByLevel(infoId, data.ArtifactLevel,true);
            templateNext = CharacterTemplateManager.Instance.GetArtifactEquipmentByLevel(infoId, data.ArtifactLevel + 1);
            Icon.spriteName = templateNow.iconId;
            Shadow.gameObject.CustomSetActive(data.ArtifactLevel < 0);
            Shadow.GetComponent<DynamicUISprite>().spriteName = templateNow.iconId;
            ArtifactName.text = templateNow.name;
            SetShowItem(templateNext);
            DetailBehaviour.Init(infoId);
            if (data.ArtifactLevel == CharacterTemplateManager.Instance.GetArtifactEquipmentMaxLevel(infoId))
            {
                button.gameObject.CustomSetActive(false);
                MaxTip.gameObject.CustomSetActive(true);
            }
            else
            {
                button.gameObject.CustomSetActive(true);
                MaxTip.gameObject.CustomSetActive(false);
            }
        }
        
        public void SetShowItem(ArtifactEquipmentTemplate temp)
        {
            if (temp == null)
            {
                ShowItem.mDMono.gameObject.CustomSetActive(false);
                redPoint.gameObject.SetActive(false);
                return;
            }
            ShowItem.mDMono.gameObject.CustomSetActive(true);
            
            string[] args = temp.ItemCost.Split(',');
            if (args.Length >= 2)
            {
                int curCount = GameItemUtil.GetInventoryItemNum(args[0]);
                int.TryParse(args[1], out var needCount);
                ShowItem.LTItemData = new LTShowItemData(args[0], needCount, LTShowItemType.TYPE_GAMINVENTORY);
                string color = curCount < needCount
                    ? LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal
                    : LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                ShowItemCount.text = string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat + "/{2}", color,
                    curCount, needCount);
                redPoint.gameObject.SetActive(curCount >= needCount);
            }
        }

      

        public void OnClickUpgrade()
        {
            string[] args = templateNow.ItemCost.Split(',');
            if (args.Length >= 2)
            {
                int curCount = GameItemUtil.GetInventoryItemNum(args[0]);
                int.TryParse(args[1], out var needCount);
                if (curCount >= needCount)
                {
                    LTPartnerDataManager.Instance.UpgradeArtifact(data.HeroId,data.ArtifactLevel+1, (b) =>
                    {
                        if (b)
                        {
                            DetailBehaviour.OnDisable();
                            DetailBehaviour.PlayAnim();
                            SetMenuData(null);
                            // Messenger.Raise(EventName.ArtifactRefresh);
                            ArtifactEquipmentTemplate artifactEquipmentTemplate=CharacterTemplateManager.Instance.GetArtifactEquipmentByLevel(infoId, data.ArtifactLevel,true);
                            if (!string.IsNullOrEmpty(artifactEquipmentTemplate.desc))
                            {
                                GlobalMenuManager.Instance.Open("LTArtifactLevelUpUIHud",infoId);
                            }
                        }
                    });
                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer.GetString("ID_ERROR_INSUFFICIENT_ITEMS"));
                }
            }
            
            
           
        }

       
    }
}