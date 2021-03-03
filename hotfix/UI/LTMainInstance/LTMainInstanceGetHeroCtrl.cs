using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTMainInstanceGetHeroCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            ContextLabel = t.GetMonoILRComponent<UILazyLabel>("Container/Context/Desc");
            NameLabel = t.GetComponent<UILabel>("Container/Context/Name");
            QualitySprite = t.GetComponent<UISprite>("Container/Context/Quality");
            HeroTex = t.GetComponent<UITexture>("Container/Icon");
            HeroIcon = t.GetComponent<CampaignTextureCmp>("Container/SpriteIcon");
            BGTex = t.GetComponent<CampaignTextureCmp>("Container/BlurBG");
            GetBtnObj = t.FindEx("Container/GetBtnGrid").gameObject;
            HireBtnObj = t.FindEx("Container/HireBtnGrid").gameObject;
            CostSprite = t.GetComponent<UISprite>("Container/HireBtnGrid/OKBtn/Cost");
            CostLabel = t.GetComponent<UILabel>("Container/HireBtnGrid/OKBtn/Num");
            HireLabel = t.GetComponent<UILabel>("Container/HireBtnGrid/OKBtn/Label");
            NoCostHireLabel = t.GetComponent<UILabel>("Container/HireBtnGrid/OKBtn/Label (1)");

            t.GetComponent<ConsecutiveClickCoolTrigger>("Container/GetBtnGrid/OKBtn").clickEvent.Add(new EventDelegate(OnOkBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Container/GetBtnGrid/CancelBtn").clickEvent.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Container/HireBtnGrid/OKBtn").clickEvent.Add(new EventDelegate(OnOkBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Container/HireBtnGrid/CancelBtn").clickEvent.Add(new EventDelegate(OnCancelButtonClick));

        }
        
        public UILazyLabel ContextLabel;
    
        public UILabel NameLabel;
    
        public UISprite QualitySprite;
    
        public UITexture HeroTex;
    
        public CampaignTextureCmp HeroIcon;
    
        public CampaignTextureCmp BGTex;//用于副本背景
    
        private LTInstanceNode mNodeData;
    
        public GameObject GetBtnObj;
    
        public GameObject HireBtnObj;
    
        public UISprite CostSprite;
    
        public UILabel CostLabel;
    
        public UILabel HireLabel;
    
        public UILabel NoCostHireLabel;
    
        private System.Action<bool> mCallback;
    
        private bool mIsHire;
    
        private string mChapterBg;
    
        public override bool ShowUIBlocker
        {
            get
            {
                return false;
            }
        }
    
        public override bool IsFullscreen()
        {
            return true;
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable data = param as Hashtable;
            if (data != null)
            {
                mNodeData = data["NodeData"] as LTInstanceNode;
                mCallback = data["Callback"] as System.Action<bool>;
                mIsHire = (bool)data["IsHire"];
                mChapterBg = (string)data["ChapterBg"];
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            if (!string .IsNullOrEmpty ( mChapterBg ))
            {
                BGTex.spriteName = mChapterBg;
            }
            else
            {
                BGTex.spriteName = "Game_Background_2";
            }
            yield return base.OnAddToStack();
            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfoByModel(mNodeData.RoleData.Model);
            if (charTpl == null)
            {
                yield break;
            }
            StartCoroutine(CreateBuddyModel(charTpl));
            NameLabel.text = charTpl.name;
            QualitySprite.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)charTpl.role_grade];
            var roleTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeChapterRole(mNodeData.RoleData.Id);
            if (roleTpl != null&& roleTpl.Guide.Length>1)
            {
                ContextLabel.Text = roleTpl.Guide[1];
            }
            else
            {
                EB.Debug.LogError("ContextLabel can't Set,Because: roleTpl != null&& roleTpl.Guide.Length>1");
            }
            GetBtnObj.CustomSetActive(!mIsHire);
            HireBtnObj.CustomSetActive(mIsHire);
            CostSprite.gameObject.CustomSetActive(mIsHire && mNodeData.HireCost != null);
            CostLabel.gameObject.CustomSetActive(mIsHire && mNodeData.HireCost != null);
            HireLabel.gameObject.CustomSetActive(mIsHire && mNodeData.HireCost != null);
            NoCostHireLabel.gameObject.CustomSetActive(mIsHire && mNodeData.HireCost == null);
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            HeroIcon.spriteName = string.Empty;
            BGTex.spriteName = string.Empty;
            DestroySelf();
            yield break;
        }
    
        private UI3DLobby Lobby;
        private GM.AssetLoader<GameObject> Loader;
        private string mModelName;
    
        public IEnumerator CreateBuddyModel(Hotfix_LT.Data.HeroInfoTemplate charTpl)
        {
            if (!string.IsNullOrEmpty(charTpl.portrait))
            {
                HeroTex.gameObject.CustomSetActive(false);
                HeroIcon.gameObject.CustomSetActive(true);
                HeroIcon.spriteName = charTpl.portrait;
    
                TweenPosition TP = HeroIcon.transform.GetComponent<TweenPosition>();
                if (TP != null)
                {
                    TP.ResetToBeginning();
                    TP.PlayForward();
                }
            }
            else
            {
                HeroIcon.gameObject.CustomSetActive(false);
                HeroTex.gameObject.CustomSetActive(true);
                mModelName = charTpl.model_name;
                HeroTex.uvRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                LobbyCameraData lobby_cam = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(charTpl.id).lobby_camera;
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller .gameObject);
                UI3DLobby.Preload(mModelName);
                yield return Loader;
    
                if (Loader.Success)
                {
                    Loader.Instance.transform.parent = HeroTex.transform;
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                }
    
                if (Lobby != null)
                {
                    Lobby.VariantName = mModelName;
    
                    Lobby.SetCameraPos(lobby_cam.Position);
                    Lobby.SetCameraRot(lobby_cam.Rotation);
                    Lobby.SetCameraMode(lobby_cam.Size, lobby_cam.Orthographic);
                    
                    Lobby.ConnectorTexture = HeroTex;
    
                    TweenPosition TP = HeroTex.transform.GetComponent<TweenPosition>();
                    if (TP != null)
                    {
                        TP.ResetToBeginning();
                        TP.PlayForward();
                    }
                }
            }
        }
    
        public void OnOkBtnClick()
        {
            if (mCallback != null)
            {
                mCallback(true);
                controller.Close();
            }
        }
    
        public override void OnCancelButtonClick()
        {
            if (mCallback != null)
            {
                mCallback(false);
            }
            base.OnCancelButtonClick();
        }
    }
}
