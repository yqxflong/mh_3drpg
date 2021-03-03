using System;
using UnityEngine;
using System.Collections;
using System.Xml;

namespace Hotfix_LT.UI
{
    public class DialogueDataFrame : DynamicMonoHotfix
    {
        public DialogueTextureCmp m_Icon;
        public CampaignTextureCmp m_SpriteIcon;
        public UILabel m_SpeakName;
        public UILabel m_SpeakContext;
        public UISprite ContextBg;
        public string m_Soujin;
        public string currentAudioName = "";

        private const int CharacterPoolSize = 10;
        public UILazyLabel m_LazySpeakContext;
        public UITexture lobbyTexture;
        public Vector3 dialogueLightRotate;

        private string lastIcon;
        private int lastStepId;
        private int lastDialogueId;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    m_Icon = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<DialogueTextureCmp>();
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    m_SpriteIcon = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<CampaignTextureCmp>();
                }
                if (count > 2 && mDMono.ObjectParamList[2] != null)
                {
                    m_SpeakName = ((GameObject)mDMono.ObjectParamList[2]).GetComponentEx<UILabel>();
                }
                if (count > 3 && mDMono.ObjectParamList[3] != null)
                {
                    m_SpeakContext = ((GameObject)mDMono.ObjectParamList[3]).GetComponentEx<UILabel>();
                }
                if (count > 4 && mDMono.ObjectParamList[4] != null)
                {
                    ContextBg = ((GameObject)mDMono.ObjectParamList[4]).GetComponentEx<UISprite>();
                }
                if (count > 5 && mDMono.ObjectParamList[5] != null)
                {
                    m_LazySpeakContext = ((GameObject)mDMono.ObjectParamList[5]).GetMonoILRComponent<UILazyLabel>();
                }
                if (count > 6 && mDMono.ObjectParamList[6] != null)
                {
                    lobbyTexture = ((GameObject)mDMono.ObjectParamList[6]).GetComponentEx<UITexture>();
                }
            }

            if (mDMono.Vector3ParamList != null)
            {
                var count = mDMono.Vector3ParamList.Count;

                if (count > 0)
                {
                    dialogueLightRotate = mDMono.Vector3ParamList[0];
                }
            }

            if (mDMono.StringParamList != null)
            {
                var count = mDMono.StringParamList.Count;

                if (count > 0)
                {
                    m_Soujin = mDMono.StringParamList[0];
                }
                if (count > 1)
                {
                    currentAudioName = mDMono.StringParamList[1];
                }
            }
        }

        public override void OnDisable()
        {
            lastDialogueId = 0;

            if (lobbyTexture != null)
            {
                lobbyTexture.transform.position = lobbyTexture.transform.GetComponent<TweenPosition>().from;
            }

            if (m_SpriteIcon != null)
            {
                m_SpriteIcon.transform.position = m_SpriteIcon.transform.GetComponent<TweenPosition>().from;
            }

            if (ContextBg != null)
            {
                ContextBg.gameObject.CustomSetActive(false);
            }
        }

        public void Play(DialogueStepData data)
        {
            if (m_Icon != null && data.Icon.StartsWith("Header_"))
            {
                m_Icon.enabled = true;
                m_Icon.spriteName = ReplaceIcon(data.Icon);
            }

            if (data.Layout != (int)eDialogueLayout.Middle)
            {
                m_SpeakName.text = string.Format("{0}:", ReplaceName(data.SpeakName));
            }
            else
            {
                if (m_SpeakName != null) m_SpeakName.gameObject.CustomSetActive(false);
            }

            // 防止同一个人连续多次说话动画还连续播放多次
            if (lastDialogueId != data.DialogueId)
            {
                lastIcon = string.Empty;
                lastStepId = -1;
                lastDialogueId = data.DialogueId;
            }

            if (lastIcon != data.Icon || data.StepId - lastStepId > 1)
            {
                string DLGid = string.Format("{0}/{1}", data.DialogueId, data.StepId);
                string audioEventName = Hotfix_LT.Data.GuideAudioTemplateManager.Instance.GetDLGAudio(DLGid);
                string audioBGMName = Hotfix_LT.Data.GuideAudioTemplateManager.Instance.GetDLGBGM(DLGid);

                if (audioEventName != null)
                {
                    if (!string.IsNullOrEmpty(currentAudioName))
                    {
                        FusionAudio.PostEvent(currentAudioName, false);
                        currentAudioName = audioEventName;
                    }
                    FusionAudio.PostEvent(audioEventName, true);
                    FusionAudio.PostBGMEvent(audioBGMName, true);
                }

                UITweener[] tweeners = mDMono.transform.GetComponentsInChildren<UITweener>();

                for (int i = 0; i < tweeners.Length; i++)
                {
                    tweeners[i].ResetToBeginning();
                    tweeners[i].PlayForward();
                }
            }

            lastIcon = data.Icon;
            lastStepId = data.StepId;

            if (m_LazySpeakContext == null)
            {
                m_LazySpeakContext = m_SpeakContext.transform.GetMonoILRComponent<UILazyLabel>();
            }

            m_LazySpeakContext.Text = string.Format("{0}{1}", (global::UserData.Locale == EB.Language.ChineseSimplified || global::UserData.Locale == EB.Language.ChineseTraditional) ? m_Soujin : "      ", data.Context);
            m_SpeakContext.fontSize = data.FontSize;
        }

        public void Finish()
        {
            m_SpeakName.text = "";
            m_SpeakContext.text = "";
            m_Icon.enabled = false;
            m_SpeakContext.fontSize = 48;
            FusionAudio.PostEvent(currentAudioName, false);
        }

        private string ReplaceName(string name)
        {
            string realname = null;

            if (name.Equals("#name"))
            {
                DataLookupsCache.Instance.SearchDataByID<string>("name", out realname);
            }
            else
            {
                realname = name;
            }

            return realname.Replace("/n", "");
        }

        private string ReplaceIcon(string icon)
        {
            string realicon = null;

            if (icon.Equals("2"))
            {
                string templateid;

                if (!DataLookupsCache.Instance.SearchDataByID<string>("{buddyinventory.pos0.buddy.buddyid}.template_id", out templateid))
                {
                    return icon + "_Half";
                }

                string characterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacterid(templateid);
                var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterid);

                if (charTpl != null)
                {
                    return string.Format("{0}_Half", charTpl.icon);
                }

                return string.Empty;
            }
            else
            {
                realicon = icon;
            }

            return string.Format("{0}_Half", realicon);
        }

        public IEnumerator CreateBuddyModel(int layout, DialogueStepData data)
        {
            string ShowPortraitSprite = null;//是否是显示半身像，否则显示模型
            lobbyTexture.uvRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

            if (data != null)
            {
                int characterId = Convert.ToInt32(data.Icon);
                var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterId);

                if (charTpl == null)
                {
                    EB.Debug.LogError("No Character for id = {0}", characterId);
                    yield break;
                }

                if (!string.IsNullOrEmpty(charTpl.portrait))
                {
                    ShowPortraitSprite = charTpl.portrait;
                }

                if (charTpl.model_name == DialoguePlayUtil.Instance.ModelName && layout == DialoguePlayUtil.Instance.Layout)
                {
                    yield break;
                }

                DialoguePlayUtil.Instance.ModelName = charTpl.model_name;
            }
            else
            {
                yield break;
            }

            ContextBg.gameObject.CustomSetActive(false);

            if (ShowPortraitSprite != null)//显示半身像
            {
                lobbyTexture.gameObject.CustomSetActive(false);
                m_SpriteIcon.gameObject.CustomSetActive(true);
                DialoguePlayUtil.Instance.Layout = layout;
                m_SpriteIcon.spriteName = ShowPortraitSprite;
            }
            else//显示模型
            {
                m_SpriteIcon.gameObject.CustomSetActive(false);
                lobbyTexture.gameObject.CustomSetActive(true);
                DialoguePlayUtil.Instance.Layout = layout;
                Vector3 charLightDirection = Quaternion.Euler(dialogueLightRotate) * Vector3.forward;
                Shader.SetGlobalVector("_CharTopDirectionToLight0", Vector3.zero - charLightDirection.normalized);

                if (DialoguePlayUtil.Instance.Lobby == null && DialoguePlayUtil.Instance.Loader == null)
                {
                    DialoguePlayUtil.Instance.Loader = new GM.AssetLoader<GameObject>("UI3DLobby", mDMono.gameObject);
                    UI3DLobby.Preload(DialoguePlayUtil.Instance.ModelName);
                    yield return DialoguePlayUtil.Instance.Loader;

                    if (DialoguePlayUtil.Instance.Loader.Instance != null)
                    {
                        DialoguePlayUtil.Instance.Loader.Instance.transform.SetParent(mDMono.transform.parent);
                    }

                    if (DialoguePlayUtil.Instance.Loader.Success)
                    {
                        DialoguePlayUtil.Instance.Lobby = DialoguePlayUtil.Instance.Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                        DialoguePlayUtil.Instance.Lobby.CharacterPoolSize = CharacterPoolSize;

                        if (DialoguePlayUtil.Instance.repaceTopShader != null)
                        {
                            DialoguePlayUtil.Instance.Lobby.renderSettings.gameObject.CustomSetActive(false);
                            DialoguePlayUtil.Instance.Lobby.lobbyCamera.SetReplacementShader(DialoguePlayUtil.Instance.repaceTopShader, "RenderType");
                            Shader.SetGlobalFloat("_CharTopOutlineScale", DialoguePlayUtil.Instance.dialogueOutlineScale);
                        }
                    }
                    else
                    {
                        EB.Debug.LogError("DialoguePlayUtil.Instance.Loader Load Fail!");
                        EB.Assets.UnloadAssetByName("UI3DLobby", false);

                        if (DialoguePlayUtil.Instance.Loader.Instance != null)
                        {
                            UnityEngine.Object.Destroy(DialoguePlayUtil.Instance.Loader.Instance.gameObject);
                        }

                        DialoguePlayUtil.Instance.Loader = null;
                    }

                    DialoguePlayUtil.Instance.IsLobbyLoadOk = true;
                }

                if (DialoguePlayUtil.Instance.Lobby != null)
                {
                    DialoguePlayUtil.Instance.Lobby.ConnectorTexture = lobbyTexture;
                    DialoguePlayUtil.Instance.Lobby.mDMono.transform.SetChildLayer(LayerMask.NameToLayer("UI3D"));
                    DialoguePlayUtil.Instance.Lobby.VariantName = DialoguePlayUtil.Instance.ModelName;

                    LobbyCameraData cameraData = null;
                    cameraData = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(data.Icon).lobby_camera;

                    if (cameraData != null)
                    {
                        DialoguePlayUtil.Instance.Lobby.SetCameraPos(cameraData.Position);
                        DialoguePlayUtil.Instance.Lobby.SetCameraRot(cameraData.Rotation);
                        DialoguePlayUtil.Instance.Lobby.SetCameraMode(cameraData.Size, cameraData.Orthographic);
                    }
                    else
                    {
                        EB.Debug.LogError("CreateBuddyModel: lobby_camera = null");
                    }

                    Vector3 iconRotation = lobbyTexture.transform.eulerAngles;

                    if ((eDialogueLayout)layout == eDialogueLayout.Left)
                    {
                        lobbyTexture.transform.GetComponent<TweenPosition>().from.x = -2730;
                        lobbyTexture.transform.GetComponent<TweenPosition>().to.x = 0;
                        lobbyTexture.transform.localPosition = cameraData.IconPosition;
                        lobbyTexture.transform.localEulerAngles = cameraData.IconRotation;
                    }
                    else if ((eDialogueLayout)layout == eDialogueLayout.Right)
                    {
                        lobbyTexture.transform.GetComponent<TweenPosition>().from.x = 2730;
                        lobbyTexture.transform.GetComponent<TweenPosition>().to.x = 0;
                        iconRotation.y = 180;
                        lobbyTexture.transform.localRotation = Quaternion.Euler(iconRotation);
                    }
                }
            }

            ContextBg.gameObject.CustomSetActive(true);
            DialoguePlayUtil.Instance.ShowContext = true;
        }
    }
}
