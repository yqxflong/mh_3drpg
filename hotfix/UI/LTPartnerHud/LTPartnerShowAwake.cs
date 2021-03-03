using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerShowAwake : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            LifeShowTween = t.GetComponent<TweenScale>("Content/AttriObj/Attribute/Life");
            AttackShowTween = t.GetComponent<TweenScale>("Content/AttriObj/Attribute/Attack");
            SpeedShowTween = t.GetComponent<TweenScale>("Content/AttriObj/Attribute/Crit");
            DefShowTween = t.GetComponent<TweenScale>("Content/AttriObj/Attribute/Def");
            TextureTween = t.GetComponent<TweenPosition>("Content/ShowTexture");
            LifeNameLabel = t.GetComponent<UILabel>("Content/AttriObj/Attribute/Life/NameLabel");
            AttackNameLabel = t.GetComponent<UILabel>("Content/AttriObj/Attribute/Attack/NameLabel");
            SpeedNameLabel = t.GetComponent<UILabel>("Content/AttriObj/Attribute/Crit/NameLabel");
            DefNameLabel = t.GetComponent<UILabel>("Content/AttriObj/Attribute/Def/NameLabel");
            LifeNumLabel = t.GetComponent<UILabel>("Content/AttriObj/Attribute/Life/NumLabel");
            AttackNumLabel = t.GetComponent<UILabel>("Content/AttriObj/Attribute/Attack/NumLabel");
            SpeedNumLabel = t.GetComponent<UILabel>("Content/AttriObj/Attribute/Crit/NumLabel");
            DefNumLabel = t.GetComponent<UILabel>("Content/AttriObj/Attribute/Def/NumLabel");
            AttriAddLabel = t.GetComponent<UILabel>("Content/AttriAddObj/Label");
            NormalSkillName = t.GetComponent<UILabel>("Content/SkillObj/CurSkill/NameLabel");
            AwakeSkillName = t.GetComponent<UILabel>("Content/SkillObj/AwakeSkill/NameLabel");
            SkinGetTip = t.GetComponent<UILabel>("Content/SkinObj/Label");
            PartnerName = t.GetComponent<UILabel>("Content/ShowTexture/Title (1)");
            SkillAddLabel = t.GetComponent<UILabel>("Content/SkillObj/Label");
            normalskillLevel = t.GetComponent<UILabel>("Content/SkillObj/CurSkill/SkillItem/Sprite/Level");
            awakenSkillLevel = t.GetComponent<UILabel>("Content/SkillObj/AwakeSkill/SkillItem/Sprite/Level");
            BlockBg = t.GetComponent<TweenColor>("Bg");
            Bg2 = t.GetComponent<TweenAlpha>("Bg/BG");
            TitleAnimator = t.GetComponent<Animator>("Title");
            ShowLobbyTexture = t.GetComponent<UITexture>("Content/ShowTexture/LobbyTexture");
            FX_Cast = t.FindEx("Bg/FX_Cast").gameObject;
            Fx_Hit = t.FindEx("Bg/FX_Hit").gameObject;
            AttrObj = t.FindEx("Content/AttriObj").gameObject;
            SkillAwakeObj = t.FindEx("Content/SkillObj").gameObject;
            OnClickTipObj = t.FindEx("Content/Tip").gameObject;
            AttriExtralAddObj = t.FindEx("Content/AttriAddObj").gameObject;
            SkinObj = t.FindEx("Content/SkinObj").gameObject;
            JumpTweenObj = t.FindEx("Jumptween").gameObject;
            NormalSkillIcon = t.GetComponent<DynamicUISprite>("Content/SkillObj/CurSkill/SkillItem/Icon");
            AwakeSkillIcon = t.GetComponent<DynamicUISprite>("Content/SkillObj/AwakeSkill/SkillItem/Icon");
            NormalSkillBtn = t.GetComponent<UIButton>("Content/SkillObj/CurSkill/SkillItem");
            AwakenSkillBtn = t.GetComponent<UIButton>("Content/SkillObj/AwakeSkill/SkillItem");
            PartnerGradeSprite = t.GetComponent<UISprite>("Content/ShowTexture/Sprite");
            BgSprite = t.GetComponent<UISprite>("Bg");
            AttriBg = t.GetComponent<UISprite>("Content/AttriObj/Sprite");
            StarList = t.GetMonoILRComponent<LTPartnerStarController>("Content/ShowTexture/StarList");

            t.GetComponent<UIButton>("Content/SkillObj/CurSkill/SkillItem").onClick.Add(new EventDelegate(SetNormalSkillClick));
            t.GetComponent<UIButton>("Content/SkillObj/AwakeSkill/SkillItem").onClick.Add(new EventDelegate(SetAwakeSkillClick));
            t.GetComponent<UIButton>("Jumptween").onClick.Add(new EventDelegate(OnClickJumpTween));

            t.GetComponent<ConsecutiveClickCoolTrigger>("Bg").clickEvent.Add(new EventDelegate(OnCancelButtonClick));

        }


    
    	public TweenScale LifeShowTween, AttackShowTween, SpeedShowTween, DefShowTween;
        public TweenPosition TextureTween;
        public UILabel LifeNameLabel, AttackNameLabel, SpeedNameLabel, DefNameLabel, LifeNumLabel, AttackNumLabel, SpeedNumLabel, DefNumLabel, AttriAddLabel,
                NormalSkillName, AwakeSkillName, SkinGetTip, PartnerName, SkillAddLabel, normalskillLevel, awakenSkillLevel;
        public TweenColor BlockBg;
        public TweenAlpha Bg2;
    	public Animator TitleAnimator;
        public UITexture ShowLobbyTexture;
        public GameObject FX_Cast,Fx_Hit, AttrObj, SkillAwakeObj, OnClickTipObj, AttriExtralAddObj,SkinObj,JumpTweenObj;
        public WaitForSeconds DelayTimes = new WaitForSeconds(0.4f);
        public DynamicUISprite NormalSkillIcon, AwakeSkillIcon;
        public UIButton NormalSkillBtn, AwakenSkillBtn;
        public UISprite PartnerGradeSprite,BgSprite,AttriBg;
        public LTPartnerStarController StarList;
        private Hotfix_LT.Data.HeroAwakeInfoTemplate curTemplate;
        private LTPartnerData curpartnerData;
        private LTAttributesData BeforeAttriData;
        private LTAttributesData RaiseAttriData;
        private bool canClose = false;
        private int attriAddObjPosWithSkin = -230;
        private int attriAddObjPosWithoutSkin = -95;
        private int skillObjPosWithSkin = -158;
        private int skillObjPosWithoutSkin = -17;
        private int skillLevel = 1;
        private bool isCheckSkin = false;
        private IEnumerator playtween;
        public override bool IsFullscreen()
        {
            return true;
        }
    
    
        public override void SetMenuData(object param)
        {
            if (param != null) curpartnerData = param as LTPartnerData;
            else
            {
                EB.Debug.LogError("Awaken PartnerData is null");
                controller.Close();
                return;
            }
            curTemplate = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(curpartnerData.InfoId);
    
    
        }
    
        public override IEnumerator OnAddToStack()
        {
            ShowAwaken();
            return base.OnAddToStack();
        }
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            return base.OnRemoveFromStack();
        }
        public void ShowAwaken()
        {
            //获取基础属性
            canClose = false;
            hasShow = false;
            isInitLobby = true;
            controller.gameObject.CustomSetActive(true);
            StartCoroutine(CreateBuddyModel(ShowLobbyTexture,curpartnerData.HeroInfo.model_name,true,false));
            BeforeAttriData = AttributesManager.GetPartnerAttributesByParnterData(curpartnerData);
            RaiseAttriData = AttributesManager.GetPartnerAwakenAttributes(curpartnerData);
            BeforeAttriData.Sub(RaiseAttriData);
            //初始化数据
            SetInfo();
            controller.gameObject.CustomSetActive(true);
            playtween = PlayAwakenTween();
            StartCoroutine(playtween);
        }
    
        /// <summary>
        /// 动画序列
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayAwakenTween()
        {    
            JumpTweenObj.CustomSetActive(true);
            FX_Cast.CustomSetActive(true);
            yield return new WaitForSeconds(3.0f);
            Fx_Hit.CustomSetActive(true);
            yield return new WaitForSeconds(0.2f);
            //切换皮肤
            if (!string.IsNullOrEmpty(curTemplate.awakeSkin))
            {
                LTPartnerDataManager.Instance.PartnerUseAwakeSkin(curpartnerData.HeroId, 1, MainLandLogic.GetInstance().SceneId, delegate(bool isccuess)
                {
                    if (isccuess)
                    {
                        curpartnerData = LTPartnerDataManager.Instance.RefreshSkinData(curpartnerData.HeroId);
                        Hotfix_LT.Data.HeroInfoTemplate info = curpartnerData.HeroInfo;
                        StartCoroutine(CreateBuddyModel(ShowLobbyTexture,info.model_name, false,true));
                        SetPartnerStar(curpartnerData.IsAwaken);
                        //修改伙伴界面显示
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerAwakenSucc, curpartnerData);
                        //设置领队
                        if (curpartnerData.StatId == LTMainHudManager.Instance.UserLeaderTID)
                        {
                            if (!AllianceUtil.IsInTransferDart)
                            {
                                Hotfix_LT.Messenger.Raise("SetLeaderEvent");
                            }
                        }
                        isCheckSkin = true;
                    }
    
                });
            }
            else
            {
                SetPartnerStar(curpartnerData.IsAwaken);
                StartCoroutine(CreateBuddyModel(ShowLobbyTexture, curpartnerData.HeroInfo.model_name, false,true));
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerAwakenSucc, curpartnerData);
            }
            yield return new WaitForSeconds(1.2f);
            Fx_Hit.CustomSetActive(false);
            FX_Cast.CustomSetActive(false);
            SetPowerShow();
            yield return new WaitForSeconds(TextureTween.duration);
            yield return new WaitForSeconds(1.2f);
            TextureTween.ResetToBeginning();
            TextureTween.PlayForward();
            BlockBg.PlayForward();
            Bg2.ResetToBeginning();
            Bg2.gameObject.CustomSetActive(true);
            Bg2.PlayForward();
            yield return new WaitForSeconds(TextureTween.duration);
            //播放属性加成
            if(string.IsNullOrEmpty(curTemplate.awakeSkin)&& curTemplate.awakeType == 1)
            {
                AttriBg.height = 839;
            }
            else if(!string.IsNullOrEmpty(curTemplate.awakeSkin) && curTemplate.awakeType == 2)
            {
                AttriBg.height = 759;
            }
            else if (!string.IsNullOrEmpty(curTemplate.awakeSkin) && curTemplate.awakeType == 1)
            {
                AttriBg.height = 975;
            }
            else if (string.IsNullOrEmpty(curTemplate.awakeSkin) && curTemplate.awakeType == 2)
            {
                AttriBg.height = 759;
            }
    
            AttrObj.CustomSetActive(true);
            LifeShowTween.ResetToBeginning();
            LifeShowTween.gameObject.CustomSetActive(true);
            LifeShowTween.PlayForward();
            yield return DelayTimes;
            AttackShowTween.ResetToBeginning();
            AttackShowTween.gameObject.CustomSetActive(true);
            AttackShowTween.PlayForward();
            yield return DelayTimes;
            DefShowTween.ResetToBeginning();
            DefShowTween.gameObject.CustomSetActive(true);
            DefShowTween.PlayForward();
            yield return DelayTimes;
            SpeedShowTween.ResetToBeginning();
            SpeedShowTween.gameObject.CustomSetActive(true);
            SpeedShowTween.PlayForward();
            yield return DelayTimes;
            if (!string.IsNullOrEmpty(curTemplate.awakeSkin))
            {
                SkinObj.transform.GetComponent<TweenScale>().ResetToBeginning();
                SkinObj.CustomSetActive(true);
                SkinObj.transform.GetComponent<TweenScale>().PlayForward();
                yield return DelayTimes;
                AttriExtralAddObj.transform.localPosition = new Vector3(621, attriAddObjPosWithSkin, 0);
                SkillAwakeObj.transform.localPosition = new Vector3(621, skillObjPosWithSkin, 0);
            }
            else
            {
                AttriExtralAddObj.transform.localPosition = new Vector3(621, attriAddObjPosWithoutSkin, 0);
                SkillAwakeObj.transform.localPosition = new Vector3(621, skillObjPosWithoutSkin, 0);
            }
            if (curTemplate.awakeType == 1)
            {
                SkillAwakeObj.transform.GetComponent<TweenScale>().ResetToBeginning();
                SkillAwakeObj.CustomSetActive(true);
                SkillAwakeObj.transform.GetComponent<TweenScale>().PlayForward();
            }
            else
            {
                AttriExtralAddObj.transform.GetComponent<TweenScale>().ResetToBeginning();
                AttriExtralAddObj.gameObject.CustomSetActive(true);
                AttriExtralAddObj.transform.GetComponent<TweenScale>().PlayForward();
            }
            yield return DelayTimes;        
            //SkinGetTip.gameObject.CustomSetActive(true);
            OnClickTipObj.CustomSetActive(true);
            JumpTweenObj.CustomSetActive(false);
            canClose = true;
            yield return null;
        }
    
    
        /// <summary>
        /// 技能点击
        /// </summary>
        public void SetNormalSkillClick()
        {
    
                UITooltipManager.Instance.DisplayTooltipForPress(curTemplate.beforeSkill.ToString() + "," + skillLevel, "Skill", "default", Vector3.zero, ePressTipAnchorType.LeftDown, false, false, false, delegate () { });
        }
        public void SetAwakeSkillClick()
        {
                UITooltipManager.Instance.DisplayTooltipForPress(curTemplate.laterSkill.ToString() + "," + skillLevel, "Skill", "default", Vector3.zero, ePressTipAnchorType.LeftDown, false, false, false, delegate () { });
        }
    
        /// <summary>
        /// 还原面板元素
        /// </summary>
        private void ResetObjCondition()
        {
            isCheckSkin = false;
            Bg2.gameObject.CustomSetActive(false);
            TextureTween.ResetToBeginning();
            BlockBg.ResetToBeginning();
            AttrObj.CustomSetActive(false);
            LifeShowTween.gameObject.CustomSetActive(false);
            AttackShowTween.gameObject.CustomSetActive(false);
            SpeedShowTween.gameObject.CustomSetActive(false);
            DefShowTween.gameObject.CustomSetActive(false);
            SkillAwakeObj.CustomSetActive(false);
            AttriExtralAddObj.CustomSetActive(false);
            SkinObj.CustomSetActive(false);
            JumpTweenObj.CustomSetActive(false);
            OnClickTipObj.CustomSetActive(false);
        }
        public override void OnDisable()
        {
            ResetObjCondition();
        }
        bool hasShow = false;
        private void SetPowerShow()
        {
            if (!hasShow)
            {
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,2,true);
                hasShow = true;
            }
        }
     
        /// <summary>
        /// 设置显示信息
        /// </summary>
        private void SetInfo()
        {
            //属性相关
            PartnerName.text = curpartnerData.HeroInfo.name;
            PartnerGradeSprite.spriteName = "Ty_Strive_Icon_" + ((PartnerGrade)curpartnerData.HeroInfo.role_grade).ToString();
            SetPartnerStar(0);
            string colorStr = "[42fe79]";
            LifeNameLabel.text = LifeNameLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_ATTR_HP");
            LifeNumLabel.text = LifeNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = ((int)BeforeAttriData.m_MaxHP).ToString() + colorStr + " + " + ((int)RaiseAttriData.m_MaxHP).ToString();
            AttackNameLabel.text = AttackNameLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_ATTR_ATK");
            AttackNumLabel.text = AttackNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = ((int)BeforeAttriData.m_ATK).ToString() + colorStr + " + " + ((int)RaiseAttriData.m_ATK).ToString("0");
            SpeedNameLabel.text = SpeedNameLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_ATTR_Speed");
            SpeedNumLabel.text = SpeedNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = ((int)BeforeAttriData.m_Speed).ToString() + colorStr + " + " + ((int)RaiseAttriData.m_Speed).ToString("0");
            DefNameLabel.text = DefNameLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_ATTR_DEF");
            DefNumLabel.text = DefNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = ((int)BeforeAttriData.m_DEF).ToString() + colorStr + " + " + ((int)RaiseAttriData.m_DEF).ToString("0");
            //技能相关
            if (curTemplate.awakeType == 1)
            {
                if (curTemplate.beforeSkill == curpartnerData.HeroStat.active_skill)
                {
                    skillLevel = curpartnerData.ActiveSkillLevel;
                }
                else if (curTemplate.beforeSkill == curpartnerData.HeroStat.common_skill)
                {
                    skillLevel = curpartnerData.CommonSkillLevel;
                }
                else if (curTemplate.beforeSkill == curpartnerData.HeroStat.passive_skill)
                {
                    skillLevel = curpartnerData.PassiveSkillLevel;
                }
                normalskillLevel.text = normalskillLevel.transform.GetChild(0).GetComponent<UILabel>().text = skillLevel.ToString();
                awakenSkillLevel.text = awakenSkillLevel.transform.GetChild(0).GetComponent<UILabel>().text = skillLevel.ToString();
                SkillAddLabel.text = SkillAddLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString("ID_PARTNER_AWAKEN_SKILLADD"), "[ffffff]", "");
                Hotfix_LT.Data.SkillTemplate normalskill = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(curTemplate.beforeSkill);
                Hotfix_LT.Data.SkillTemplate awakeskill = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(curTemplate.laterSkill);
                NormalSkillIcon.spriteName = normalskill.Icon;
                NormalSkillName.text = normalskill.Name;
                //NormalSkillBtn.OnClickAction = new System.Action(SetNormalSkillClick);
                AwakeSkillIcon.spriteName = awakeskill.Icon;
                AwakeSkillName.text = awakeskill.Name;
            }
            else
            {
                //AwakenSkillBtn.OnClickAction = new System.Action(SetAwakeSkillClick);
                //属性提升
                AttriAddLabel.text = string.Format(EB.Localizer.GetString("ID_PARTNER_AWAKEN_ATTRIADD"),"[ffffff]",Hotfix_LT.Data.CharacterTemplateManager.Instance.GetAwakenExtraAttri(curTemplate));
            }
    
            //皮肤获取提示
            SkinGetTip.text = EB.Localizer.GetString("ID_PARTNER_GETSKIN_TIP");
        }
    
    
        /// <summary>
        /// 设置觉醒前后星星显示
        /// </summary>
        /// <param name="isawaken"></param>
        private void SetPartnerStar(int awakenLevel)
        {
            for (int i = 0; i < StarList.StarObjList.Length; i++)
            {
                if (i < curpartnerData.Star)
                {
                    StarList.StarObjList[i].CustomSetActive(true);
                }
                else
                {
                    StarList.StarObjList[i].CustomSetActive(false);
                }
            }
            StarList.SetStarAlpha(curpartnerData.Star, awakenLevel);
            StarList.mDMono.GetComponent<UIGrid>().Reposition();
        }
    
        /// <summary>
        /// 创建人物模型，关联Texture
        /// </summary>
        private UI3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        private string ModelName = null;
        private const int CharacterPoolSize = 10;
        private bool isModelReady = false;
        private bool isInitLobby = false;
        private Max820.Bloom bloom = null;
        private IEnumerator CreateBuddyModel(UITexture LobbyTexture, string modelName,bool isPlayLight,bool isplayEntry)
        {
            isInitLobby = true;
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
                }
                isInitLobby = false;
                if (!isPlayLight) bloom.thresholdGamma = 1.0f;
                yield return new WaitForSeconds(0.3f);
                if (isplayEntry) Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
                yield break;
            }
            LobbyTexture.gameObject.CustomSetActive(false);
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
                    Camera.orthographicSize = 2.0f;
                }
            }
            LobbyTexture.gameObject.CustomSetActive(true);
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
            
            }
            RenderSettings rs = controller.transform.GetComponentInChildren<RenderSettings>();
            if (rs != null)
            {
                EB.Debug.Log("rendersetting set : {0}" , rs.name);
                RenderSettingsManager.Instance.SetActiveRenderSettings(rs.name, rs);
            }
            else
            {
                EB.Debug.LogWarning("rendersetting is null");
            }        
            isInitLobby = false;
            Lobby.SetBloomUITexture(LobbyTexture);
            bloom = Lobby.BloomCamera.GetComponent<Max820.Bloom>();
            while (isPlayLight&& bloom.thresholdGamma>0.15f)
            {
                Lobby.BloomCamera.gameObject.CustomSetActive(true);           
                bloom.thresholdGamma = Mathf.Lerp(bloom.thresholdGamma, 0.1f, 0.1f);
                yield return new WaitForEndOfFrame();
            }
            if (!isPlayLight) bloom.thresholdGamma = 1.0f;
            yield return new WaitForSeconds(0.3f);
            if (isplayEntry) Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
        }
    
    
        //关闭界面
        public override void OnCancelButtonClick()
        {
            if (!canClose) return;
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerAwakenSuccess);
            DestroySelf();
            base.OnCancelButtonClick();
        }
    
    /// <summary>
    /// 点击跳过按钮
    /// </summary>
    public void OnClickJumpTween()
        {
            StopAllCoroutines();
            JumpTweenObj.CustomSetActive(false);
            if (!string.IsNullOrEmpty(curTemplate.awakeSkin)&& isCheckSkin == false)
            {
                LTPartnerDataManager.Instance.PartnerUseAwakeSkin(curpartnerData.HeroId, 1, MainLandLogic.GetInstance().SceneId, delegate (bool isccuess)
                {
                    if (isccuess)
                    {
                        curpartnerData = LTPartnerDataManager.Instance.RefreshSkinData(curpartnerData.HeroId);
                        Hotfix_LT.Data.HeroInfoTemplate info = curpartnerData.HeroInfo;           
                        StartCoroutine(CreateBuddyModel(ShowLobbyTexture, info.model_name, false,false));
                        SetPartnerStar(curpartnerData.IsAwaken);
                        //修改伙伴界面显示
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerAwakenSucc, curpartnerData);
                        //设置领队
                        if (curpartnerData.StatId == LTMainHudManager.Instance.UserLeaderTID)
                        {
                            if (!AllianceUtil.IsInTransferDart)
                            {
                                Hotfix_LT.Messenger.Raise("SetLeaderEvent");
                            }
                        }
                    }
    
                });
            }
            else
            {
                SetPartnerStar(curpartnerData.IsAwaken);
                StartCoroutine(CreateBuddyModel(ShowLobbyTexture, curpartnerData.HeroInfo.model_name, false, false));
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerAwakenSucc, curpartnerData);
            }
            Bg2.gameObject.CustomSetActive(true);
            BgSprite.color = new Color(1, 1, 1, 1);
            Fx_Hit.CustomSetActive(false);
            FX_Cast.CustomSetActive(false);
            SetPowerShow();
            TextureTween.PlayForward();
            AttrObj.CustomSetActive(true);
            LifeShowTween.gameObject.CustomSetActive(true);
            AttackShowTween.gameObject.CustomSetActive(true);
            SpeedShowTween.gameObject.CustomSetActive(true);
            DefShowTween.gameObject.CustomSetActive(true);
            if (!string.IsNullOrEmpty(curTemplate.awakeSkin))
            {
                SkinObj.CustomSetActive(true);
                AttriExtralAddObj.transform.localPosition = new Vector3(621, attriAddObjPosWithSkin, 0);
                SkillAwakeObj.transform.localPosition = new Vector3(621, skillObjPosWithSkin, 0);
            }
            else
            {
                AttriExtralAddObj.transform.localPosition = new Vector3(621, attriAddObjPosWithoutSkin, 0);
                SkillAwakeObj.transform.localPosition = new Vector3(621, skillObjPosWithoutSkin, 0);
            }
            if (curTemplate.awakeType == 1)
            {
                SkillAwakeObj.CustomSetActive(true);
            }
            else
            {
                AttriExtralAddObj.gameObject.CustomSetActive(true);
    
            }
    
            OnClickTipObj.CustomSetActive(true);
            canClose = true;
    
        }
    }
}
