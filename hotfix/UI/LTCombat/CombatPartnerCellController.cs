using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public class CombatPartnerCellController : DynamicCellController<LTPartnerData>
    {
	    public LTPartnerData ItemData { get; private set; } 
	    public UISprite IconSprite;

        public UISprite FrameSprite;
        public UISprite FrameBGSprite;

        public UISprite LevelSprite;
        public UILabel LevelLabel;
        public UISprite LevelBGSprite;
        public UILabel breakLebel;

        public LTPartnerStarController StarController;

        public GameObject SelectSpriteObj;

        public GameObject HideObj;

        public GameObject RecommendSprite;
        public GameObject DeathSprite;

        public GameObject HireSprite;
        public GameObject SleepPS;

        private ParticleSystemUIComponent charFx, upgradeFx;
        private EffectClip efClip, upgradeefclip;

        public UIDragScrollView dragScrollView;
        private System.Action<CombatPartnerCellController> onDragStartFunc = null;
        private System.Action onDragFunc = null;
        private System.Action onDragEndFunc = null;
        private bool isDragingByIcon = false;

		private UIEventListener listener;

		private GameObject MercenaryUseFlag;

		public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            IconSprite = t.GetComponent<UISprite>("Icon");
            FrameSprite = t.GetComponent<UISprite>("Frame");
            FrameBGSprite = t.GetComponent<UISprite>("FrameBG");
            LevelSprite = t.GetComponent<UISprite>("HideObj/Property");
            LevelLabel = t.GetComponent<UILabel>("HideObj/LevelSprite/LabelLevel");
            LevelBGSprite = t.GetComponent<UISprite>("HideObj/LevelSprite");
            breakLebel = t.GetComponent<UILabel>("HideObj/BreakObj/Break");
            StarController = t.GetMonoILRComponent<LTPartnerStarController>("HideObj/StarList");
            SelectSpriteObj = t.FindEx("Select").gameObject;
            HideObj = t.FindEx("HideObj").gameObject;
            RecommendSprite = t.FindEx("RecommendSprite").gameObject;
            DeathSprite = t.FindEx("DeathSprite").gameObject;
            if (t.Find("HireSprite")!=null)
            {
	            HireSprite = t.FindEx("HireSprite").gameObject;
            }          

            Transform ps = t.Find("ps");
            if (ps != null)
            {
                SleepPS = ps.gameObject;
            }

            Transform mercenarytemp = t.Find("Flag");
            if (mercenarytemp!=null)
            {
	              MercenaryUseFlag =mercenarytemp.gameObject;
            }

            Transform dragScroll = t.Find("Icon");
            if (dragScroll != null)
            {
                dragScrollView = dragScroll.GetComponent<UIDragScrollView>();

                DragClassifyEventDispatcher draDispatcher = dragScroll.GetComponent<DragClassifyEventDispatcher>();
                if (draDispatcher != null)
                {
                    draDispatcher.onDragFunc.Add(new EventDelegate(OnDrag));
                    draDispatcher.onDragStartFunc.Add(new EventDelegate(OnDragStart));
                    draDispatcher.onDragEndFunc.Add(new EventDelegate(OnDragEnd));
                }
            }

			if (IconSprite)
			{
				listener = UIEventListener.Get(IconSprite.gameObject);
				listener.onClick += OnClick;
			}
        }
        
        public override void Clean()
        {
            mDMono.gameObject.CustomSetActive(false);
            ItemData = null;
        }
    
    	public override void Fill(LTPartnerData itemData)
    	{
            SetItem(itemData);
    	}
    
        public void SetItem(LTPartnerData itemData)
        {
	        //itemData为空的时候也把ItemData置空,其他地方使用ItemData需要判空
	        ItemData = itemData;
	        
	        if (itemData == null|| (itemData.StatId == 0&& itemData.InfoId == 0))
	        {
		        mDMono.gameObject.CustomSetActive(false);
		        return;
	        }
	      
	        RecommendSprite.CustomSetActive(false);
	        SleepPS.CustomSetActive(false);
           
          
            if (BattleReadyHudController.sBattleType == eBattleType.SleepTower && LTClimingTowerHudController.Instance != null)
            {
	            SleepPS.CustomSetActive(!LTClimingTowerHudController.Instance.CanUpTeam(ItemData.HeroId));
            }
    
            IconSprite.spriteName = ItemData.HeroInfo.icon;
            if (ItemData.HeroId <= 0&&!ItemData.IsHeroBattle || ItemData.IsHeroBattle&&LTNewHeroBattleManager.GetInstance().CurrentType==HeroBattleType.High&&LTNewHeroBattleManager.GetInstance().HasChallengeHeroInfoID.Contains(ItemData.StatId))
            {
                SetGrey(IconSprite);
                SetGrey(FrameSprite);
                SetDark(FrameBGSprite);
                HideObj.CustomSetActive(false);
                return;
            }
            else
            {
                SetNormal(IconSprite);
                SetNormal(FrameSprite);
                HideObj.CustomSetActive(true);
            }
            int quality = 0;
            int addLevel = 0;
            LTPartnerDataManager.GetPartnerQuality(ItemData.UpGradeId, out quality, out addLevel);
    
            FrameSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
            GameItemUtil.SetColorfulPartnerCellFrame(quality, FrameBGSprite);
            HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, FrameSprite.transform, quality, upgradeefclip);
            //FrameBGSprite.color = LTPartnerConfig.QUANTITY_BG_COLOR_DIC[quality];
            LTPartnerConfig.SetLevelSprite(LevelSprite,ItemData.HeroInfo.char_type,ItemData.ArtifactLevel >= 0);
            int index = QualitySettings.GetQualityLevel();
			if(index < 1) HotfixCreateFX.ShowCharTypeFX(charFx, efClip, LevelSprite.transform, (PartnerGrade)ItemData.HeroInfo.role_grade, ItemData.HeroInfo.char_type);
			LTUIUtil.SetLevelText(LevelBGSprite,LevelLabel,ItemData);
			if (itemData.IsHeroBattle)
			{
				LevelBGSprite.spriteName = "Ty_Brand_Di1";
			}
            //英雄交锋特殊处理 其他布阵需要考虑觉醒后星星颜色变化
            if (BattleReadyHudController.sBattleType == eBattleType.HeroBattle)
                StarController.SetSrarList(ItemData.Star, ItemData.HeroBattleAwakenLevel);
            else {
	            //天梯选人界面特殊处理
	            if (ItemData.IsHire) 
                    StarController.SetSrarList(ItemData.Star, ItemData.HireAwakeLevel);
	            else StarController.SetSrarList(ItemData.Star, ItemData.IsAwaken);
            }
            if (addLevel > 0)
            {
                breakLebel.transform.parent.gameObject.CustomSetActive(true);
                breakLebel.text = "+" + addLevel.ToString();
            }
            else
            {
                breakLebel.transform.parent.gameObject.CustomSetActive(false);
            }
    
            if (itemData.Level > 0 && LTResourceInstanceHudController.mChooseLevel != null)
            {
                for (int i = 0; i < LTResourceInstanceHudController.mChooseLevel.recommend_partners_id.Length; i++)
                {
                    if (LTResourceInstanceHudController.mChooseLevel.recommend_partners_id[i] != 0 && ItemData.HeroInfo.id == LTResourceInstanceHudController.mChooseLevel.recommend_partners_id[i])
                    {
                        RecommendSprite.CustomSetActive(true);
                        break;
                    }
                }
            }
    
    		DeathSprite.gameObject.CustomSetActive(false);
            if ((BattleReadyHudController.sBattleType == eBattleType.ChallengeCampaign|| BattleReadyHudController.sBattleType == eBattleType.AlienMazeBattle) && !FormationUtil.IsAlive(itemData.HeroId, itemData.IsHire) && itemData.uid<=0)
    		{
    			DeathSprite.gameObject.CustomSetActive(true);
    		}
    
            if (HireSprite)
            {
                HireSprite.CustomSetActive(itemData.IsHire && ItemData.uid<=0);
            }
            
            MercenaryUseFlag.CustomSetActive(false);
            MercenaryUseFlag.CustomSetActive(ShowUseFlag());

            mDMono.gameObject.CustomSetActive(true);

        }

        public bool ShowUseFlag()
        {
	        if (ItemData.uid>0)
	        {
		        int use;
		        DataLookupsCache.Instance.SearchDataByID($"mercenary.info.used_uids.{ItemData.uid}", out use);
		        return use == 1;
	        }

	        return false;
        }
    
    	private void SetGrey(UIWidget item)
        {
            item.color = new Color(1, 0, 1, 1);
        }
    
        private void SetNormal(UIWidget item)
        {
            item.color = new Color(1, 1, 1, 1);
        }
    
        private void SetDark(UIWidget item)
        {
            item.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    
        public void OnSelect(bool isSelect)
    	{
            SelectSpriteObj.CustomSetActive(isSelect);
    	}
    
    	#region drag 

    
    	public void OnDrag()
    	{
    		if (onDragFunc != null)
    			onDragFunc();
    	}
    
    	public void OnDragStart()
    	{
    		if (dragScrollView != null)
    		{
    			dragScrollView.enabled = false;
    		}
    
    		if (onDragStartFunc != null)
    		{
    			if (!isDragingByIcon)
    			{
    				isDragingByIcon = true;
    				onDragStartFunc(this);
    			}
    		}
    	}
    
    	public void OnDragEnd()
    	{
    		if (dragScrollView != null)
    		{
    			dragScrollView.enabled = true;
    		}
    
    		if (onDragEndFunc != null)
    		{
    			if (isDragingByIcon)
    			{
    				isDragingByIcon = false;
    				onDragEndFunc();
    				OnSelect(false);
    			}
    		}
    	}

        public void OnClick()
        {
	        OnClick(null);
        }

        private void OnClick(GameObject btn)
        {
	        FusionAudio.PostEvent("UI/General/ButtonClick");

	        if (ItemData != null)
	        {
		        if (ItemData.uid > 0)
		        {
			        LTFormationDataManager.Instance.GetMercenaryPlayerData(ItemData.uid.ToString(), ItemData.HeroId,
				        (ha) =>
				        {
					        if (ha != null && ha.Count >= 1)
					        {
						        GlobalMenuManager.Instance.Open("LTPartnerInfoView", ha[0]);
					        }
				        });
			        return;
		        }

		        if (ItemData.IsHire || ItemData.IsHeroBattle)
		        {
			        Vector2 screenPos = UICamera.lastEventPosition;
			        var ht = Johny.HashtablePool.Claim();
			        ht.Add("id", ItemData.InfoId);
			        ht.Add("screenPos", screenPos);
			        GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
		        }
		        else
		        {
			        GlobalMenuManager.Instance.Open("LTPartnerInfoView",
				        LTPartnerDataManager.Instance.Translated(ItemData));
		        }
	        }
        }

		public void SetItemDragStartActionNew(System.Action<CombatPartnerCellController> _onDragStartFunc)
    	{
    		onDragStartFunc = _onDragStartFunc;
    	}
    
    	public void SetItemDragAcionNew(System.Action _onDragFunc)
    	{
    		onDragFunc = _onDragFunc;
    	}
    
    	public void SetItemDragEndActionNew(System.Action _onDragEndFunc)
    	{
    		onDragEndFunc = _onDragEndFunc;
    	}   
    	#endregion
    }
}
