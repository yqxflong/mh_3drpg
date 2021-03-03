using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ArtifactLevelUpController: UIControllerHotfix
    {
        public ArtifactDetailBehaviour DetailBehaviour;
        public GameObject Template;
        private int infoId;
        public override bool IsFullscreen()
        {
            return false;
        }

        public override bool ShowUIBlocker
        {
            get { return true; }
        }

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            DetailBehaviour = t.GetMonoILRComponent<ArtifactDetailBehaviour>("Bg");
            t.GetComponent<UIButton>("Bg/Top/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Bg/UpLevelBtn").clickEvent
                .Add(new EventDelegate(OnCancelButtonClick));
            Template = t.Find("Bg/SkillDesc/Container/Template").gameObject;
        }
        
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            //TODO 传入伙伴Id数据
            infoId = (int) param;
            DetailBehaviour.Init(infoId);
            LTPartnerData data = LTPartnerDataManager.Instance.GetPartnerByInfoId(infoId);
            ArtifactEquipmentTemplate artifactEquipmentTemplate=CharacterTemplateManager.Instance.GetArtifactEquipmentByLevel(infoId, data.ArtifactLevel,true);
            UILabel levelLabel = Template.GetComponent<UILabel>();
            UILabel descLabel = Template.GetComponent<UILabel>("Desc");
            UISprite sprite = Template.GetComponent<UISprite>("Sprite");
            levelLabel.text = $"[44fe7c]+{artifactEquipmentTemplate.enhancementLevel}";
            descLabel.text = $"[44fe7c]{artifactEquipmentTemplate.desc}";
            sprite.color = Color.yellow;
        }
    }
}