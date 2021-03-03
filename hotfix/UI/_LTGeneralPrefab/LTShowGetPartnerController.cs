using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTShowGetPartnerController : UIControllerHotfix
    {
        public UISprite QualityIcon;
        public UILabel NameLabel;
        public UIGrid StarGrid;
        public UILabel HeroshardLabel;

        private bool isSSR = false;
        public GameObject FxObj;
        public GameObject SSRFxObj;

        public override bool IsFullscreen()
        {
            return true;
        }

        public static bool m_Open;
        public override bool ShowUIBlocker { get { return true; } }

        private bool isInitLobby;
        private string mModelName = null;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            QualityIcon = t.GetComponent<UISprite>("Panel/Bottom/NameLabel/Quality");
            NameLabel = t.GetComponent<UILabel>("Panel/Bottom/NameLabel");
            StarGrid = t.GetComponent<UIGrid>("Panel/Bottom/Star");
            HeroshardLabel = t.GetComponent<UILabel>("Panel/Bottom/HeroshardLabel");
            FxObj = t.FindEx("FX").gameObject;
            SSRFxObj = t.FindEx("FX_SSR").gameObject;
            LobbyTexture = t.GetComponent<UITexture>("Panel/LobbyTexture");

            controller.backButton = t.GetComponent<UIButton>("Panel/CloseBtn");
            t.GetComponent<UIButton>("Panel/TestBtn (1)").onClick.Add(new EventDelegate(TestBtn1));
            t.GetComponent<UIButton>("Panel/TestBtn (2)").onClick.Add(new EventDelegate(TestBtn2));
        }

        public override void SetMenuData(object param)
        {
            controller.gameObject.CustomSetActive(true);
            base.SetMenuData(param);
            if (param != null)
            {
                LTShowItemData Data = param as LTShowItemData;
                LTIconNameQuality inl = LTItemInfoTool.GetInfo(Data.id, Data.type);

                if (Data.type.Equals(LTShowItemType.TYPE_HEROSHARD))
                {
                    HeroshardLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTShowGetPartnerController_1021"), Data.count);
                    HeroshardLabel.gameObject.SetActive(true);
                }
                else
                {
                    HeroshardLabel.gameObject.SetActive(false);
                }

                int charTp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(int.Parse(Data.id)).character_id;
                var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(charTp);

                if (charTpl == null)
                {
                    EB.Debug.LogError("No Character for id = {0}", Data.id);
                    return;
                }

                mModelName = charTpl.model_name;
                isSSR = charTpl.role_grade >= 4;

                if (isSSR)
                {
                    FusionAudio.PostEvent("UI/New/SSR", true);
                }
                else
                {
                    FusionAudio.PostEvent("UI/New/N", true);
                }

                NameLabel.text = NameLabel.transform.GetChild(0).GetComponent<UILabel>().text = charTpl.name;
                QualityIcon.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)charTpl.role_grade];

                for (int i = 0; i < StarGrid.transform.childCount; i++)
                {
                    StarGrid.transform.GetChild(i).gameObject.SetActive(i < charTpl.init_star);
                }

                StarGrid.Reposition();
            }
        }

        private bool m_guideToolState = false;

        public override IEnumerator OnAddToStack()
        {
            m_Open = true;
            isInitLobby = true;
            controller.gameObject.CustomSetActive(true);
            yield return base.OnAddToStack();
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);

            if (MengBanController.Instance.controller.gameObject.activeSelf)
            {
                m_guideToolState = true;
                MengBanController.Instance.controller.transform.parent.gameObject.CustomSetActive(false);
            }

            if (isSSR)
            {
                SSRFxObj.CustomSetActive(true);
            }
            else
            {
                FxObj.CustomSetActive(true);
            }

            yield return null;
            EB.Coroutines.Run(CreateBuddyModel(mModelName));
        }

        public override IEnumerator OnRemoveFromStack()
        {
            m_Open = false;
            controller.gameObject.CustomSetActive(false);
            LobbyTexture.enabled = false;
            FxObj.CustomSetActive(false);
            SSRFxObj.CustomSetActive(false);

            if (m_guideToolState)
            {
                m_guideToolState = false;
                MengBanController.Instance.controller.transform.parent.gameObject.CustomSetActive(true);
            }

            DestroySelf();

            if (GlobalMenuManager.Instance != null)
            {
                GlobalMenuManager.Instance.RemoveOpenController(controller);
            }

           
            Messenger.Raise(Hotfix_LT.EventName.InventoryEvent);
            yield break;
        }

        public override void OnFocus()
        {
            base.OnFocus();
            RenderSettings rs = controller.transform.GetComponentInChildren<RenderSettings>();
            if (rs != null)RenderSettingsManager.Instance.SetActiveRenderSettings(rs.name, rs);

            if (MengBanController.Instance.controller.gameObject.activeSelf && !m_guideToolState)
            {
                m_guideToolState = true;
                MengBanController.Instance.controller.transform.parent.gameObject.CustomSetActive(false);
            }
        }

        public UITexture LobbyTexture;
        private UI3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        private string ModelName = null;
        private const int CharacterPoolSize = 10;
        private bool isModelReady = false;

        private IEnumerator CreateBuddyModel(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                isInitLobby = false;
                yield break;
            }

            if (modelName == ModelName)
            {
                if (Lobby != null)
                {
                    if (!Lobby.mDMono.gameObject.activeSelf)
                    {
                        Lobby.mDMono.gameObject.CustomSetActive(true);
                    }

                    Lobby.SetCharMoveState(MoveController.CombatantMoveState.kIdle);
                    yield return null;
                    LobbyTexture.enabled = true;
                    Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
                }

                isInitLobby = false;
                yield break;
            }

            ModelName = modelName;
            isModelReady = false;
            UI3DLobby.PreloadWithCallback(modelName, delegate { isModelReady = true; });

            if (Lobby == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
                yield return Loader;

                if (Loader.Success)
                {
                    Loader.Instance.transform.SetParent(LobbyTexture.transform);
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                    Lobby.ConnectorTexture = LobbyTexture;
                    Lobby.CharacterPoolSize = CharacterPoolSize;
                    Camera Camera = Lobby.mDMono.transform.Find("UI3DCamera").GetComponent<Camera>();
                    Camera.orthographicSize = 1.25f;
                }
            }

            while (!isModelReady)
            {
                yield return null;
            }

            if (Lobby != null)
            {
                if (!Lobby.mDMono.gameObject.activeSelf)
                {
                    Lobby.mDMono.gameObject.CustomSetActive(true);
                }

                Lobby.VariantName = modelName;
                Lobby.SetCharMoveState(MoveController.CombatantMoveState.kIdle);
                yield return null;
                LobbyTexture.enabled = true;
                Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
            }

            isInitLobby = false;
        }

        public override void OnCancelButtonClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
            if (isInitLobby)
            {
                // 防止在加载模型的时候按了esc退出，导致摄像机看不见主城；
                return;
            }

            if (this != null)
            {
                base.OnCancelButtonClick();
            }
        }

        #region 测试
        public void TestBtn1()
        {
            Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
        }
        public void TestBtn2()
        {
            EB.Coroutines.Run(TestIE());
        }

        private IEnumerator TestIE()
        {
            Lobby.SetCharMoveState(MoveController.CombatantMoveState.kIdle);
            yield return null;
            Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
        }
        #endregion
    }
}
