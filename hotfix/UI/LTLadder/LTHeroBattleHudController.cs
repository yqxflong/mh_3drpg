using _HotfixScripts.Utils;
using DG.Tweening;

namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 英雄交锋UI界面
    /// 要是修改这里的代码要小心~因为天梯已经继承
    /// </summary>
    public class LTHeroBattleHudController : UIControllerHotfix, IHotfixUpdate
    {
        public override bool IsFullscreen()
        {
            return true;
        }

        public UITexture vsTexture;
        public UILabel
            //heroNameLabel,
            //otherHeroNameLabel,
            myPointLabel,
            otherPointLabel;
        public UIProgressBar myLeftPointBar;
        public UIProgressBar otherLeftPointBar;
        //第一个倒计时
        public UILabel timeLabel;
        public UILabel startTimeLabel;
        public UILabel choiceStateTipsLabel;

        public UIWidget turnGO, otherTurnGO;
        public float changeTurnTweenTime = 1;
        public UILabel selfNameLabel, OtherNameLabel;

        public UILabel selfLevelLabel, otherLevelLabel;
        public UISprite selfHeadIconSpt, otherHeadIconSpt;
        public UISprite selfFrameIconSpt, otherFrameIconSpt;
        public GameObject myModelShadow;
        public GameObject otherModelShadow;

        protected UI3DVsLobby _vsLobby;
        protected bool _isOpen;
        protected GM.AssetLoader<GameObject> _loader;
        protected string _showModelName;
        protected string _showOtherModelName;
        protected string _selfHeroName;
        protected string _otherHeroName;
        protected string _selfHeroTypeSptName;
        protected string _otherHeroTypeSptName;
        protected int _lessTime;
        protected bool _otherTurnTipsTweenOver;

        protected const int CharacterPoolSize = 5;

        public override void Awake()
        {
            base.Awake();
            StartCoroutine(CreateVsLobby());
            LTHeroBattleEvent.NotifyRefreshChoiceState += OnNotifyRefreshChoiceState;
            LTHeroBattleEvent.NotifyChangeChoiceHero += OnNotifyChangeChoiceHero;
            LTHeroBattleEvent.NotifyHeroBattleHudFinish += OnNotifyHeroBattleFinish;
        }

        public override void OnDestroy()
        {
            if (_vsLobby != null && _vsLobby.mDMono!=null)
            {
                GameObject.Destroy(_vsLobby.mDMono.gameObject);
            }

            if (_loader != null)
            {
                EB.Assets.UnloadAssetByName("UI3DVsLobby", false);
            }
            _vsLobby = null;
            _loader = null;
            _isOpen = false;

            LTHeroBattleEvent.NotifyRefreshChoiceState -= OnNotifyRefreshChoiceState;
            LTHeroBattleEvent.NotifyChangeChoiceHero -= OnNotifyChangeChoiceHero;
            LTHeroBattleEvent.NotifyHeroBattleHudFinish -= OnNotifyHeroBattleFinish;

            if (SceneLogic.SceneState == SceneLogic.eSceneState.DelayCombatTransition) SceneLogic.SceneState = SceneLogic.eSceneState.RequestingCombatTransition;
            base.OnDestroy();
        }

        public override IEnumerator OnAddToStack()
        {
            yield return new WaitUntil(() => _isOpen);
            if (_vsLobby != null) _vsLobby.mDMono.gameObject.CustomSetActive(true);
            yield return null;
            yield return base.OnAddToStack();
            yield return null;
            if (LadderMatchSuccessUIController.isOpen) GlobalMenuManager.Instance.CloseMenu("LTMatchSuccessUI");
        }

        public override IEnumerator OnRemoveFromStack()
        {
            if (_vsLobby != null) _vsLobby.mDMono.gameObject.CustomSetActive(false);
            DestroySelf();
            return base.OnRemoveFromStack();
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            HeroBattleChoiceData data = param as HeroBattleChoiceData;
            if (data != null)
            {
                SetChoiceData(data);
            }
            else if ((bool)param)
            {
                LTHeroBattleEvent.GetReloadData();
            }
        }

        protected IEnumerator CreateVsLobby()
        {
            if (_vsLobby == null && _loader == null)
            {
                _loader = new GM.AssetLoader<GameObject>("UI3DVsLobby", controller.gameObject);
                yield return _loader;
            }
            if (_loader.Success)
            {
                _vsLobby = _loader.Instance.GetMonoILRComponent<UI3DVsLobby>();
                _vsLobby.mDMono.gameObject.transform.position = new Vector3(0, 10000, 0);
                _vsLobby.ConnectorTexture = vsTexture;
                _isOpen = true;
            }
            else
            {
                EB.Debug.LogError("CreateVsLobby false ");
            }
        }

        protected bool ChangeModel(string name)
        {
            if (!_isOpen)
            {
                return false;
            }
            if (_vsLobby != null)
            {
                _vsLobby.CharacterPoolSize = CharacterPoolSize;
                _vsLobby.VariantName = name;

                if (_vsLobby.VariantName != null && _vsLobby.VariantName.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        protected bool ChangeOtherModel(string name)
        {
            if (!_isOpen)
            {
                return false;
            }
            _vsLobby.OtherVariantName = name;

            if (_vsLobby.OtherVariantName == name)
            {
                return true;
            }
            return false;
        }

        void OnClickChoiceSuit01(GameObject go)
        {
            if (LTHeroBattleEvent.ChoiceSuitIndex != null)
            {
                LTHeroBattleEvent.ChoiceSuitIndex(0);
            }
        }

        void OnClickChoiceSuit02(GameObject go)
        {
            if (LTHeroBattleEvent.ChoiceSuitIndex != null)
            {
                LTHeroBattleEvent.ChoiceSuitIndex(1);
            }
        }

        protected float _PressedSuitTime;

		private bool _IsPressed01;

		private  bool _IsPressed02;

		protected bool IsPressed01
		{
			get => _IsPressed01;
			set
			{
				if (value || IsPressed02)
					RegisterMonoUpdater();
				else
					ErasureMonoUpdater();

				_IsPressed01 = value;
			}
		}

		protected bool IsPressed02
		{
			get => _IsPressed02;
			set
			{
				if (value || IsPressed01)
					RegisterMonoUpdater();
				else
					ErasureMonoUpdater();

				_IsPressed02 = value;
			}
		}

        void OnHoverSuit01(GameObject go, bool isOver)
        {
            IsPressed01 = isOver;
            if (isOver)
            {
                _PressedSuitTime = 0;
            }
        }

        void OnHoverSuit02(GameObject go, bool isOver)
        {
            IsPressed02 = isOver;
            if (isOver)
            {
                _PressedSuitTime = 0;
            }
        }

		//public override void OnEnable()
		//{
		//	RegisterMonoUpdater();
		//}

		public void Update()
        {
            // base.Update();

            if (IsPressed01)
            {
                _PressedSuitTime += Time.unscaledDeltaTime;
                if (_PressedSuitTime > 1f)
                {
                    _PressedSuitTime = 0;
                    IsPressed01 = false;
                    if (LTHeroBattleEvent.TouchSuitIndexTips != null)
                        LTHeroBattleEvent.TouchSuitIndexTips(0);
                }
            }
            if (IsPressed02)
            {
                _PressedSuitTime += Time.unscaledDeltaTime;
                if (_PressedSuitTime > 1f)
                {
                    _PressedSuitTime = 0;
                    IsPressed02 = false;
                    if (LTHeroBattleEvent.TouchSuitIndexTips != null)
                        LTHeroBattleEvent.TouchSuitIndexTips(1);
                }
            }
        }

        void OnClickConfirm()
        {
            //      confirmBtn.gameObject.CustomSetActive(false);
            //confirmBanBtn.gameObject.CustomSetActive(false);

            //choiceSuitTips.gameObject.CustomSetActive(false);
            //choiceSuit01Btn.gameObject.CustomSetActive(false);
            //choiceSuit02Btn.gameObject.CustomSetActive(false);
            if (LTHeroBattleEvent.ConfirmChoiceHero != null)
            {
                LTHeroBattleEvent.ConfirmChoiceHero();
            }
        }

        void OnClickConfirmGray()
        {

        }

        void OnClickConfirmBan()
        {
            //confirmBanBtn.gameObject.CustomSetActive(false);

            //choiceSuitTips.gameObject.CustomSetActive(false);
            //choiceSuit01Btn.gameObject.CustomSetActive(false);
            //choiceSuit02Btn.gameObject.CustomSetActive(false);
            if (LTHeroBattleEvent.ConfirmBanHero != null)
            {
                FusionAudio.PostEvent("UI/New/JinRen", true);
                LTHeroBattleEvent.ConfirmBanHero();
            }
        }

        void OnNotifyRefreshChoiceState(HeroBattleChoiceData data)
        {
            SetChoiceData(data);
        }

        void OnNotifyChangeChoiceHero(HeroBattleChoiceCellData choiceHeroCellData)
        {
            if (choiceHeroCellData == null)
            {
                StartChangShowModel("", "", "");
                //LTUIUtil.SetText(heroNameLabel, "");
                //confirmBtn.gameObject.CustomSetActive(false);
                //confirmBanBtn.gameObject.CustomSetActive(false);

                //choiceSuitTips.gameObject.CustomSetActive(false);
                //choiceSuit01Btn.gameObject.CustomSetActive(false);
                //choiceSuit02Btn.gameObject.CustomSetActive(false);
            }
            else
            {
                StartChangShowModel(choiceHeroCellData.modelName, choiceHeroCellData.heroName,
                    LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[choiceHeroCellData.type]);

                //LTUIUtil.SetText(heroNameLabel, choiceHeroCellData.heroName);
                switch (_choiceState)
                {
                    case 0:
                        //confirmBtn.gameObject.CustomSetActive(false);
                        //confirmBanBtn.gameObject.CustomSetActive(true);

                        //choiceSuitTips.gameObject.CustomSetActive(false);
                        //suitLightSpt.gameObject.CustomSetActive(false);
                        //               choiceSuit01Btn.gameObject.CustomSetActive(false);
                        //               choiceSuit02Btn.gameObject.CustomSetActive(false);
                        break;
                    case 1:
                        //confirmBtn.gameObject.CustomSetActive(true);
                        //confirmBanBtn.gameObject.CustomSetActive(false);

                        //choiceSuitTips.gameObject.CustomSetActive(true);
                        //suitLightSpt.gameObject.CustomSetActive(true);
                        //               choiceSuit01Btn.gameObject.CustomSetActive(true); //选择伙伴后显示套装按钮
                        //               choiceSuit02Btn.gameObject.CustomSetActive(true);
                        break;
                }
            }
        }


        protected int _choiceState;
        protected virtual void SetChoiceData(HeroBattleChoiceData data)
        {
   
        }

        protected void StartChangeTurn(bool isSelfTurn, int lessTime)
        {
            FusionAudio.PostEvent("UI/New/XuanRen", true);
            StartChangeTurnE(isSelfTurn, lessTime);


			ILRTimerManager.instance.AddTimer(1000, 1, delegate (int sequence) {
				if(startTimeLabel.alpha < 1)timeLabel.alpha = 1;
			});
        }

        void StartChangeTurnE(bool isSelfTurn, int lessTime)
        {
	        UIWidget tg = isSelfTurn ? turnGO : otherTurnGO;
	        tg.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
//	        iTween.ScaleTo(tg.gameObject, Vector3.one, changeTurnTweenTime);
	        tg.gameObject.transform.DOScale(Vector3.zero, changeTurnTweenTime);
	        timeLabel.alpha = 0;
		}

        private int _scaleLabelHandler;
        private TweenScale _ts;

		public void StartShowTime(int lessTime)
        {
            _lessTime = 0;
            _lessTime = lessTime;
            //强制设为10s
            _lessTime=10;
            ShowTime();

			if(_scaleLabelHandler != 0)
			{
				ILRTimerManager.instance.RemoveTimer(_scaleLabelHandler);
				_scaleLabelHandler = 0;
			}

			_scaleLabelHandler = ILRTimerManager.instance.AddTimer(10, 1, DelayPlayTimeLabelScaleTween);
			
        }

        void ShowTime()
        {
	        if (_ts != null)
	        {
		        _ts.enabled = false;
		        LTUIUtil.SetText(timeLabel, "");
				Object.Destroy(_ts);
	        }
	        // LTUIUtil.SetText(timeLabel, _lessTime.ToString());
            timeLabel.transform.localScale = Vector3.one;
        }

        void DelayPlayTimeLabelScaleTween(int index)
        {
	        float time = Time.unscaledTime;

			if (_lessTime > 0)
				UpdateTimeLabelContent(ref time);

			_ts = TweenScale.Begin(timeLabel.gameObject, 0.4f, new Vector3(0.5f, 0.5f, 1));
			_ts.delay = 0.6f;
			_ts.ignoreTimeScale = true;
			_ts.from = Vector3.one;
			_ts.SetOnFinished(() =>
			{
				if (_lessTime > 0)
				{
					UpdateTimeLabelContent(ref time);
					_ts.ResetToBeginning();
					_ts.delay = 0.6f;
					_ts.PlayForward();
				}
				else
				{
					LTUIUtil.SetText(timeLabel, "");
					Object.Destroy(_ts);
				}
			});

			_scaleLabelHandler = 0;
        }

        protected virtual void UpdateTimeLabelContent(ref float time)
        {
	        float buff = Time.unscaledTime - time;
	        if (Mathf.RoundToInt(buff) >= 1)
	        {
		        time = time + 1;
		        _lessTime -= 1;
		        //等功能完成后在此后加入放大缩小的动画 
		        LTUIUtil.SetText(timeLabel, _lessTime.ToString());
		        if (_lessTime < 10)
		        {
			        FusionAudio.PostEvent("UI/New/DaoShu", true);
		        }
	        }
		}

        protected int _changeShowModel;

        /// <summary>
        /// 开启改变模型 直到成功 modelName ==""即删除
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="heroName"></param>
        /// <param name="heroTypeSptName"></param>
        public void StartChangShowModel(string modelName, string heroName, string heroTypeSptName)
        {
            if (_changeShowModel != 0)
            {
                ILRTimerManager.instance.RemoveTimer(_changeShowModel);
                _changeShowModel = 0;
            }

            _showModelName = modelName;
            _selfHeroName = heroName;
            _selfHeroTypeSptName = heroTypeSptName;
            myModelShadow.gameObject.CustomSetActive(string.IsNullOrEmpty(modelName));

            CheckChangeShowModel();
        }

        public bool canChangeMyModel = true;
        public bool canChangeOtherModel = true;

        protected int _cChangeOtherShowModel;

        /// <summary>
        /// 开启改变对方模型 直到成功 modelName ==""即删除
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="heroName"></param>
        /// <param name="heroTypeSptName"></param>
        public void StartChangeOtherShowModel(string modelName, string heroName, string heroTypeSptName)
        {
            if (_cChangeOtherShowModel != 0)
            {
	            ILRTimerManager.instance.RemoveTimer(_cChangeOtherShowModel);
	            _cChangeOtherShowModel = 0;
            }

            _showOtherModelName = modelName;
            _otherHeroName = heroName;
            _otherHeroTypeSptName = heroTypeSptName;
            otherModelShadow.gameObject.CustomSetActive(string.IsNullOrEmpty(modelName));

            CheckChangeShowOtherModel();
        }

        void CheckChangeShowModel()
        {
	        if (!ChangeModel(_showModelName))
	        {
		        _changeShowModel = ILRTimerManager.instance.AddTimer(200, int.MaxValue, TimeCheckChangeShowModel);
	        }
        }

        void TimeCheckChangeShowModel(int index)
        {
	        if(!ChangeModel(_showModelName))
				return;

	        if (_changeShowModel != 0)
	        {
		        ILRTimerManager.instance.PauseTimer(_changeShowModel);
				ILRTimerManager.instance.RemoveTimer(_changeShowModel);
				_changeShowModel = 0;
	        }
		}

		void CheckChangeShowOtherModel()
		{
			if (!ChangeOtherModel(_showOtherModelName))
			{
				_cChangeOtherShowModel = ILRTimerManager.instance.AddTimer(200, int.MaxValue, TimeCheckChangeShowOtherModel);
			}
		}

		void TimeCheckChangeShowOtherModel(int index)
		{
			if (!ChangeOtherModel(_showOtherModelName))
				return;

			if(_cChangeOtherShowModel != 0)
			{
				ILRTimerManager.instance.PauseTimer(_cChangeOtherShowModel);
				ILRTimerManager.instance.RemoveTimer(_cChangeOtherShowModel);
				_cChangeOtherShowModel = 0;
			}
		}

        void OnNotifyHeroBattleFinish()
        {
	        turnGO.alpha = 0;
	        otherTurnGO.alpha = 0;
	        timeLabel.alpha = 0;

	        startTimeLabel.alpha = 1;
	        float lessTime = 3;
	        FusionAudio.PostEvent("UI/New/DaoShu", true);
	        
	        LTUIUtil.SetText(startTimeLabel, lessTime.ToString());

			StartBattle(lessTime);
        }

        void StartBattle(float lessTime)
        {
	        float time = Time.unscaledTime;

	        timeLabel.alpha = 0;

	        TweenScale ts = TweenScale.Begin(startTimeLabel.gameObject, 0.4f, new Vector3(0.5f, 0.5f, 1));
			ts.delay = 0.6f;
			ts.from = Vector3.one;
			ts.ResetToBeginning();
			ts.enabled = true;
			ts.PlayForward();
			ts.SetOnFinished(() =>
			{
				if (lessTime > 1)
				{
					float buff = Time.unscaledTime - time;
					if ( Mathf.RoundToInt(buff) >= 1)
					{
						lessTime--;
						time = time + 1;

						FusionAudio.PostEvent("UI/New/DaoShu", true);
						LTUIUtil.SetText(startTimeLabel, lessTime.ToString());

						ts.ResetToBeginning();
						ts.delay = 0.6f;
						ts.PlayForward();
						ts.enabled = true;
					}
					else
						EB.Debug.LogError("time pass buff is not enough one second! buff is {0}", buff);
				}
				else
				{
					Object.Destroy(ts);
					startTimeLabel.alpha = 0;
				}
			});
        }
    }
}