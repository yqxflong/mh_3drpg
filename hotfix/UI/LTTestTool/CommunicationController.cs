using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class CommunicationController : UIControllerHotfix
    {
        public UITexture m_LobbyTexture;
        public UITexture right_LobbyTexture;
        public UILabel SpeakNameLabel;

        private UI3DLobby Lobby;
        private UI3DLobby rightLobby;
        private GM.AssetLoader<GameObject> Loader;
        private GM.AssetLoader<GameObject> rightLoader;
        private string ModelName;
        private int char_id;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_LobbyTexture = t.GetComponent<UITexture>("DialogueFrame/Container/Dialogue/Left/Icon");
            right_LobbyTexture = t.GetComponent<UITexture>("DialogueFrame/Container/Dialogue/Right/Icon");
            SpeakNameLabel = t.GetComponent<UILabel>("DialogueFrame/Container/Dialogue/Left/SpeakName");
            controller.backButton = t.GetComponent<UIButton>("DialogueFrame/Container/CancelBtn");
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            char_id = (int)param;
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            yield return StartCoroutine(CreateBuddyModel());
            StartCoroutine(CreateRightBuddyModel());
        }
    
        public IEnumerator CreateBuddyModel()
        {
            m_LobbyTexture.uvRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            var charTpl =Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(char_id);
            LobbyCameraData lobby_cam = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(charTpl.id).lobby_camera;
            ModelName = charTpl.model_name;
            LTUIUtil.SetText(SpeakNameLabel, charTpl.name);
            Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
            UI3DLobby.Preload(ModelName);
            yield return Loader;
    
            if (Loader.Success)
            {
                Loader.Instance.transform.parent = m_LobbyTexture.transform;
                Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
            }
    
            if (Lobby != null)
            {
                Lobby.VariantName = ModelName;
    
                Lobby.SetCameraPos(lobby_cam.Position);
                Lobby.SetCameraRot(lobby_cam.Rotation);
                Lobby.SetCameraMode(lobby_cam.Size, lobby_cam.Orthographic);
    
                m_LobbyTexture.transform.GetComponent<TweenPosition>().from.x = -2730;
                m_LobbyTexture.transform.GetComponent<TweenPosition>().to.x = 0;
                Lobby.ConnectorTexture = m_LobbyTexture;
            }
        }
        public IEnumerator CreateRightBuddyModel()
        {
            right_LobbyTexture.uvRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(15010);
            LobbyCameraData lobby_cam = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(charTpl.id).lobby_camera;
            LTUIUtil.SetText(SpeakNameLabel, charTpl.name);
            rightLoader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
            UI3DLobby.Preload(charTpl.model_name);
            yield return rightLoader;
    
            if (rightLoader.Success)
            {
                rightLoader.Instance.transform.parent = right_LobbyTexture.transform;
                rightLobby = rightLoader.Instance.GetMonoILRComponent<UI3DLobby>();
            }
    
            if (rightLobby != null)
            {
                rightLobby.VariantName = charTpl.model_name;
    
                rightLobby.SetCameraPos(lobby_cam.Position);
                rightLobby.SetCameraRot(lobby_cam.Rotation);
                rightLobby.SetCameraMode(lobby_cam.Size, lobby_cam.Orthographic);
    
                right_LobbyTexture.transform.GetComponent<TweenPosition>().from.x = -2730;
                right_LobbyTexture.transform.GetComponent<TweenPosition>().to.x = 0;
                rightLobby.ConnectorTexture = right_LobbyTexture;
            }
        }
    }
}
