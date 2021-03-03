using UnityEngine;

namespace Hotfix_LT.UI
{

    public class LTPartnerListCellController : DynamicCellController<LTPartnerData>
    {
        public UISprite IconSprite;
        public UISprite FrameSprite;
        public UISprite FrameBGSprite;
        public UISprite LevelSprite;
        public UILabel LevelLabel;
        public UILabel breakLebel;
        public LTPartnerStarController StarController;
        public UILabel CallLabel;
        public UISlider ShardSlider;
        public UISprite ShardSprite;
        public UILabel ShardLabel;
        public UISprite SelectSprite;
        public GameObject FormationFlagObj;
        public GameObject RedPoint;

        private int curSelectId;
        private float clickTime;
        private bool isCanSummon = false;
        private bool isShowRedPoint = false;
        private bool isGoIntoBattle = false;
        private bool isNeedRefreshEvent = true;

        private ParticleSystemUIComponent charFx,upgradeFx;
        private EffectClip efClip,upgradeefclip;

        public enum SELECT_STATE
        {
            SELECT,
            UN_SELECT,
        }

        private SELECT_STATE mSelectState = SELECT_STATE.UN_SELECT;

        private LTPartnerData partnerData;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            IconSprite = t.GetComponent<UISprite>("Icon");
            FrameSprite = t.GetComponent<UISprite>("Frame");
            FrameBGSprite = t.GetComponent<UISprite>("FrameBG");
            LevelSprite = t.GetComponent<UISprite>("Property");
            LevelLabel = t.GetComponent<UILabel>("LevelSprite/LabelLevel");
            breakLebel = t.GetComponent<UILabel>("BreakObj/Break"); 
            StarController = t.GetMonoILRComponent<LTPartnerStarController>("StarList");
            CallLabel = t.GetComponent<UILabel>("CallLabel");
            ShardSlider = t.GetComponent<UISlider>("ProgressBar");
            ShardSprite = t.GetComponent<UISprite>("ProgressBar/Sprite");
            ShardLabel = t.GetComponent<UILabel>("ProgressBar/Label");
            SelectSprite = t.GetComponent<UISprite>("Select");

            var flag = t.Find("FormationFlag");
            var redPoint = t.Find("RedPoint");
            var btn = t.GetComponent<UIButton>();

            if (flag != null)
            {
                FormationFlagObj = flag.gameObject;
            }

            if (redPoint != null)
            {
                RedPoint = redPoint.gameObject;
            }

            if (btn != null)
            {
                btn.onClick.Add(new EventDelegate(OnBtnClick));
            }
            Hotfix_LT.Messenger.AddListener<int>(Hotfix_LT.EventName.OnPartnerSelect, OnPartnerSelectFunc);
            Hotfix_LT.Messenger.AddListener<bool ,bool >(Hotfix_LT.EventName.OnRefreshPartnerCellRP, OnRefreshPartnerRP);            
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener<int>(Hotfix_LT.EventName.OnPartnerSelect, OnPartnerSelectFunc);
            Hotfix_LT.Messenger.RemoveListener<bool,bool>(Hotfix_LT.EventName.OnRefreshPartnerCellRP, OnRefreshPartnerRP);
        }

        public override void Fill(LTPartnerData itemData)
        {
            SetItemData(itemData);
        }

        public override void Clean()
        {
            SetItemData(null);
        }

        private void SetItemData(LTPartnerData itemData, bool isRefreshRedPoint = true)
        {
            partnerData = itemData;
            UpdateItem(isRefreshRedPoint);
        }

        private void UpdateItem(bool isRefreshRedPoint = true)
        {
            if (partnerData == null)
            {
                if (mDMono != null && mDMono.gameObject != null)
                {
                    mDMono.gameObject.CustomSetActive(false);
                }
                return;
            }
            mDMono.gameObject.CustomSetActive(true);

            int quality = 0;
            int addLevel = 0;
            LTPartnerDataManager.GetPartnerQuality(partnerData.UpGradeId, out quality, out addLevel);

            FrameSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
            GameItemUtil.SetColorfulPartnerCellFrame(quality, FrameBGSprite);
            curSelectId = LTPartnerDataManager.Instance.DropSelectPartnerId;

            if (partnerData.HeroId <= 0)
            {
                SetGrey(IconSprite);
                SetGrey(FrameSprite);
                SetDark(FrameBGSprite);
            }
            else
            {
                SetNormal(IconSprite);
                SetNormal(FrameSprite);
            }

            if (addLevel > 0)
            {
                breakLebel.transform.parent.gameObject.CustomSetActive(true);
                breakLebel.text = "+" + addLevel.ToString();
            }
            else
            {
                breakLebel.transform.parent.gameObject.SetActive(false);
            }
            //BottomSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];

            IconSprite.spriteName = partnerData.HeroInfo.icon;
            LTPartnerConfig.SetLevelSprite(LevelSprite,partnerData.HeroInfo.char_type,partnerData.ArtifactLevel >= 0);
            LTUIUtil.SetLevelText(LevelLabel.transform.parent.GetComponent<UISprite>(),LevelLabel,partnerData);
            if (partnerData.HeroId > 0)
            {
	            PartnerGrade grade = (PartnerGrade) partnerData.HeroInfo.role_grade;
                HotfixCreateFX.ShowCharTypeFX(charFx, efClip, LevelSprite.transform, grade, partnerData.HeroInfo.char_type);
                HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, FrameSprite.transform, quality, upgradeefclip);
            }
            else
            {
                ShardSlider.value = (float)partnerData.ShardNum / (float)partnerData.HeroInfo.summon_shard;
                ShardSprite.spriteName = partnerData.ShardNum >= partnerData.HeroInfo.summon_shard ? "Ty_Strip_Green" : "Ty_Strip_Blue";
                ShardLabel.text = string.Format("{0}/{1}", partnerData.ShardNum, partnerData.HeroInfo.summon_shard);
            }
            isCanSummon = partnerData.HeroId <= 0 && partnerData.ShardNum >= partnerData.HeroInfo.summon_shard;

            isGoIntoBattle = partnerData.IsGoIntoBattle;
            FormationFlagObj.CustomSetActive(isGoIntoBattle);
            SetItemState();
            StarController.SetSrarList(partnerData.Star, partnerData.IsAwaken);
            SetSelectState(partnerData.StatId == curSelectId ? SELECT_STATE.SELECT : SELECT_STATE.UN_SELECT);

            if (isRefreshRedPoint)
            {
                if (isGoIntoBattle)
                {
                    InitRedPoint();
                }
                else
                {
                    RedPoint.CustomSetActive(false);
                    if (partnerData.StatId == curSelectId)
                    {
                        InitRedPoint(false);
                    }
                    ShowSummonRedPoint();
                }
            }
        }

        /// <summary>
        /// 用于构造觉醒前、后图标 暂时只有星星颜色和头像切换
        /// </summary>
        public void SetItemAwakenData(LTPartnerData itemData,string icon, int star, int awakenLevel)
        {
            isNeedRefreshEvent = false;
            SetItemData(itemData, false);
            SelectSprite.gameObject.CustomSetActive(false);
            IconSprite.spriteName = icon;
            StarController.SetStarAlpha(star, awakenLevel);
        }


        /// <summary>
        /// 设置伙伴item的状态（已拥有状态和未拥有状态）
        /// </summary>
        private void SetItemState()
        {
            if (partnerData == null)
            {
                return;
            }
            bool isOwn = partnerData.HeroId > 0;
            LevelSprite.gameObject.CustomSetActive(isOwn);
            LevelLabel.transform.parent.gameObject.CustomSetActive(isOwn);
            StarController.mDMono.gameObject.CustomSetActive(isOwn);
            CallLabel.gameObject.CustomSetActive(!isOwn && partnerData.ShardNum >= partnerData.HeroInfo.summon_shard);
            ShardSlider.gameObject.CustomSetActive(!isOwn);
        }

        /// <summary>
        /// 设置伙伴item数据（非伙伴列表的伙伴数据设置）
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="quality"></param>
        /// <param name="addLevel"></param>
        public void SetItemDataOther(LTPartnerData itemData, int quality = -1, int addLevel = -1)
        {
            isNeedRefreshEvent = false;
            SetItemData(itemData, false);
            SelectSprite.gameObject.CustomSetActive(false);

            if (quality > -1 && itemData.HeroId > 0)
            {
                FrameSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
                GameItemUtil.SetColorfulPartnerCellFrame(quality, FrameBGSprite);
                HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, FrameSprite.transform, quality, upgradeefclip);
                breakLebel.transform.parent.gameObject.CustomSetActive(addLevel > 0);
                if (addLevel > 0)
                {
                    breakLebel.text = "+" + addLevel.ToString();
                }
            }
        }

        /// <summary>
        /// 根据星级设置伙伴item数据（用于非伙伴列表的伙伴数据设置）
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="star"></param>
        public void SetItemDataByStarUp(LTPartnerData itemData, int star)
        {
            isNeedRefreshEvent = false;
            SetItemData(itemData, false);
            SelectSprite.gameObject.CustomSetActive(false);
            StarController.SetSrarList(star, itemData.IsAwaken);
        }

        /// <summary>
        /// 点击伙伴头像
        /// </summary>
        public void OnBtnClick()
        {
            if (Time.time - clickTime < 1)
            {
                return;//防止点击过快
            }
            if(partnerData == null)
            {
                EB.Debug.LogError("LTPartnerListCellController.OnBtnClick partnerData == null");
                return;
            }
            if (partnerData.StatId == curSelectId && partnerData.HeroId > 0)
            {
                EB.Debug.Log("LTPartnerListCellController Select Partner is Same! partnerID = {0}", partnerData.StatId);
                return;
            }
            clickTime = Time.time;
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, partnerData.StatId);
            if (isCanSummon)
            {
                SummonPartner();
            }
        }

        /// <summary>
        /// 置灰（用于图片）
        /// </summary>
        /// <param name="item"></param>
        private void SetGrey(UIWidget item)
        {
            item.color = new Color(1, 0, 1, 1);
        }

        /// <summary>
        /// 还原初始颜色
        /// </summary>
        /// <param name="item"></param>
        private void SetNormal(UIWidget item)
        {
            item.color = new Color(1, 1, 1, 1);
        }

        /// <summary>
        /// 置灰（用于文字）
        /// </summary>
        /// <param name="item"></param>
        private void SetDark(UIWidget item)
        {
            item.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }

        /// <summary>
        /// 伙伴被选中的事件通知
        /// </summary>
        /// <param name="evt"></param>
        private void OnPartnerSelectFunc(int partnerid)
        {
            if (!isNeedRefreshEvent)
            {
                return;
            }

            curSelectId = partnerid;
            if (partnerData != null)
            {
                SetSelectState(partnerData.StatId == curSelectId ? SELECT_STATE.SELECT : SELECT_STATE.UN_SELECT);

                if (partnerData.StatId == curSelectId)
                {
                    if (isGoIntoBattle)
                    {
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PartnerCultivateRP, isShowRedPoint && partnerData.HeroId > 0);
                    }
                    else
                    {
                        InitRedPoint(false);
                    }
                }
            }
        }

        /// <summary>
        /// 设置伙伴的选中状态
        /// </summary>
        /// <param name="selectState"></param>
        private void SetSelectState(SELECT_STATE selectState)
        {
            mSelectState = selectState;
            if (!mDMono.gameObject.activeInHierarchy)
            {
                return;
            }
            if (mSelectState == SELECT_STATE.SELECT)
            {
                OnPartnerSelect();
            }
            else if (mSelectState == SELECT_STATE.UN_SELECT)
            {
                OnPartnerUnSelect();
            }
        }

        /// <summary>
        /// 设置伙伴选中框为选中状态
        /// </summary>
        private void OnPartnerSelect()
        {
            SelectSprite.gameObject.CustomSetActive(true);
        }

        /// <summary>
        /// 设置伙伴选中框为未选中状态
        /// </summary>
        private void OnPartnerUnSelect()
        {
            SelectSprite.gameObject.CustomSetActive(false);
        }

        /// <summary>
        /// 召唤伙伴
        /// </summary>
        private void SummonPartner()
        {
            LTPartnerDataManager.Instance.SummonBuddy(partnerData.StatId, delegate
            {
                LTShowItemData itemData = new LTShowItemData(partnerData.StatId.ToString(), 1, "hero");
                GlobalMenuManager.Instance.Open("LTShowGetPartnerUI", itemData);
                LTPartnerDataManager.Instance.InitPartnerData();
                Hotfix_LT.Messenger.Raise<int,bool>(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, (int)PowerData.RefreshType.All,true);
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSummonSucc, partnerData.StatId);
                partnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(partnerData.StatId);
                UpdateItem();
            });
        }

        /// <summary>
        /// 初始化红点数据
        /// </summary>
        /// <param name="isShow"></param>
        private void InitRedPoint(bool isShow = true)
        {
            isShowRedPoint = LTPartnerDataManager.Instance.IsCanSummon(partnerData) ||
                LTPartnerDataManager.Instance.IsCanCultivate(partnerData);

            RedPoint.CustomSetActive(isShowRedPoint && isShow);

            if (partnerData.StatId == curSelectId)
            {
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PartnerCultivateRP, isShowRedPoint && partnerData.HeroId > 0);
            }
        }

        /// <summary>
        /// 显示可召唤伙伴头像红点
        /// </summary>
        private void ShowSummonRedPoint()
        {
            if (partnerData.HeroId <= 0 && LTPartnerDataManager.Instance.IsCanSummon(partnerData))
            {
                RedPoint.CustomSetActive(true);
            }
        }

        /// <summary>
        /// 刷新伙伴item红点的事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnRefreshPartnerRP(bool isRefreshForward, bool isOnlyRefreshSelf)
        {
            if (!isNeedRefreshEvent)
            {
                return;
            }
            
            if (partnerData != null)
            {
                if (isGoIntoBattle)
                {
                    if ((isRefreshForward && isShowRedPoint) || (!isRefreshForward && !isShowRedPoint))
                    {
                        if (isOnlyRefreshSelf)
                        {
                            if (partnerData.StatId == curSelectId)
                            {
                                InitRedPoint();
                            }
                        }
                        else
                        {
                            InitRedPoint();
                        }
                    }
                }
                else
                {
                    if (partnerData.StatId == curSelectId)
                    {
                        InitRedPoint(false);
                    }
                }
            }
        }

    }

}
