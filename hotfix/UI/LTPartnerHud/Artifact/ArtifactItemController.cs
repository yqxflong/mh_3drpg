using EB;
using Hotfix_LT.Data;
using Umeng;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Hotfix_LT.UI
{
    public class ArtifactItemController: DynamicMonoHotfix
    {
        public DynamicUISprite icon;
        public GameObject shadow;
        public UILabel LevelLabel;
        public GameObject LockObj;
        public GameObject redPoint;

        private int infoId;
        private ArtifactEquipmentTemplate template;
        private LTPartnerData data;
        private string t_camid ;
        private int star ;
        public override void Awake()
        {
            base.Awake();
            t_camid = GetCapLimit();
            star = GetStarLimit();
            var t = mDMono.transform;
            LevelLabel = t.GetComponent<UILabel>("LeaderObj/LevelLabel");
            LockObj = t.Find("LeaderObj/LockObj").gameObject;
            icon = t.GetComponent<DynamicUISprite>("LeaderObj/icon");
            shadow = icon.transform.Find("icon").gameObject;
            redPoint = t.Find("LeaderObj/RedPoint").gameObject;
            t.GetComponent<ConsecutiveClickCoolTrigger>().clickEvent.Add(new EventDelegate(OnClickArtifactBtn));
            icon.gameObject.CustomSetActive(false);
        }

        public void SetArtifact(int infoId)
        {
            this.infoId = infoId;
            data =  LTPartnerDataManager.Instance.GetPartnerByInfoId(infoId);
            template = CharacterTemplateManager.Instance.GetArtifactEquipmentByLevel(infoId,data.ArtifactLevel);
            
            LockObj.gameObject.SetActive(template == null);
            redPoint.SetActive(LTPartnerDataManager.Instance.IsCanArtifact(data));
            LevelLabel.gameObject.SetActive(false);

            if (template != null)
            {
                icon.spriteName = template.iconId;
                shadow.GetComponent<DynamicUISprite>().spriteName = template.iconId;
                if (data.ArtifactLevel > 0)
                {
                    LevelLabel.gameObject.SetActive(true);
                    LevelLabel.text = "+" + data.ArtifactLevel;
                }
            }
            icon.gameObject.SetActive(template != null);
            shadow.CustomSetActive(template !=null && data.ArtifactLevel < 0);
        }
        public void OnClickArtifactBtn()
        {
            if (HaveArtifact())
            {
                if (!LTInstanceUtil.IsCampaignsComplete(t_camid))
                {
                    var t_targetcampaign = Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(t_camid);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_FUNC_OPENTIP_2"), t_targetcampaign.Name));
                    return;
                }

               
                if (data.Star<star)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_ARTIFACT_STAR_TIP"), star));
                    return;
                }
                // GlobalMenuManager.Instance.Open("LTArtifactUIHud");
                GlobalMenuManager.Instance.Open("LTArtifactDetailUIHud",infoId);
                
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, Localizer.GetString("ID_ARTIFACT_NO_TIP"));
            }
        }


        public bool HaveArtifact()
        {
            return template != null;
        }

        public static string GetCapLimit()
        {
            int limit = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("ArtifactCapimit");
            return limit.ToString();
        }
        
        public static int GetStarLimit()
        {
            int limit = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("ArtifactStarLimit");
            return limit;
        }
    }
}