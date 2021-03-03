using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTGetItemUIController : UIControllerHotfix
    {
        public GameObject TitleObj;
        public GameObject BottomBtnObj;
        public GameObject TipObj;
        public GameObject SkipBtn;
        public LTShowGetNewItemDynamicScroll Scroll;
        public static bool m_isHC;//是否钻石抽奖
        public override bool ShowUIBlocker { get { return false; } }

        public GameObject MainHud;
        public GameObject FxHud;
        public GameObject SliverDrawingFx;
        public GameObject GoldDrawingFx;

        private DrawCardType m_type;
        private bool FxLimit = true;
        private bool isSkip = false;
        private List<ShowGetNewItem> mDrawShowItemList;
        private List<Animator> mDrawAnimatorList;
        private List<LTShowItemData> itemData;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TitleObj = t.FindEx("MainHud/Content/Title").gameObject;
            BottomBtnObj = t.FindEx("MainHud/Content/SureBtn").gameObject;
            TipObj = t.FindEx("MainHud/Content/Tip").gameObject;
            SkipBtn = t.FindEx("SkipHud/SkipBtn").gameObject;
            Scroll = t.GetMonoILRComponent<LTShowGetNewItemDynamicScroll>("MainHud/Center/ScollList/Placeholder/Scroll");
            Scroll.CloseUpdate = true;
			MainHud = t.FindEx("MainHud").gameObject;
            FxHud = t.FindEx("FXHud").gameObject;
            SliverDrawingFx = t.FindEx("FXHud/DrawFX/SliverDrawingFX").gameObject;
            GoldDrawingFx = t.FindEx("FXHud/DrawFX/GoldDrawingFX").gameObject;

            controller.backButton = t.GetComponent<UIButton>("MainHud/Content/SureBtn");
            t.GetComponent<UIButton>("MainHud/Bg").onClick.Add(new EventDelegate(OnBGBtnClick));
            t.GetComponent<UIButton>("SkipHud/SkipBtn").onClick.Add(new EventDelegate(OnSkipBtnClick));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null)
            {
                object[] i = param as object[];
                m_type = (DrawCardType)i[0];
                m_isHC = (m_type == DrawCardType.hc);
            }
            InitView();
        }

        public override void Show(bool isShowing)
        {
            controller.transform.GetComponent<UIPanel>().alpha = (isShowing) ? 1 : 0;
        }

        private Coroutine DrawCor;
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
			//EB.Debug.Log("StartDarw");
            DrawCor = StartCoroutine(StartDarw());
        }

        private void InitView()
        {
            isSkip = false;
            FxLimit = true;
            DrawCor = null;
            MainHud.CustomSetActive(false);
            FxHud.CustomSetActive(true);
            SliverDrawingFx.CustomSetActive(false);
            GoldDrawingFx.CustomSetActive(false);
            BottomBtnObj.CustomSetActive(false);
            TipObj.CustomSetActive(false);
            SkipBtn.CustomSetActive(false);
            TitleObj.CustomSetActive(false);
        }

        public override IEnumerator OnRemoveFromStack()
        {
            LTDrawCardDataManager.Instance.DataRefresh();
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
            if (mDrawAnimatorList != null)
            {
                for (int i = 0; i < mDrawAnimatorList.Count; i++)
                {
                    mDrawAnimatorList[i].enabled = false;
                    mDrawAnimatorList[i].transform.localScale = Vector3.zero;
                }
            }
            MainHud.CustomSetActive(false);//特效需要隐藏掉
            LTPartnerDataManager.Instance.InitPartnerData();
            DestroySelf();
            yield break;
        }

        private WaitForSeconds shortItemWait = new WaitForSeconds(0.2f);
        private WaitForSeconds tweenScaleWait = null;
        private WaitForSeconds long_wait;
        private WaitForSeconds small_wait = new WaitForSeconds(1.2f);

        IEnumerator StartDarw()
        {
            List<LTDrawCardData> Items = LTDrawCardDataManager.Instance.GetDrawCardData();
            if (itemData == null)
            {
                itemData = new List<LTShowItemData>();
            }
            else
            {
                itemData.Clear();
            }
            for (int i = 0; i < Items.Count; i++) //进行显示上的英雄与碎片转换
            {
                bool isHeroShard = false;
                for (int j = 0; j < i; j++)
                {
                    if (Items[i].data == Items[j].data && Items[i].type == LTShowItemType.TYPE_HERO && Items[j].type == LTShowItemType.TYPE_HERO) isHeroShard = true;
                }
                if (Items[i].type == LTShowItemType.TYPE_HERO && LTDrawCardDataManager.Instance.isHeroShard(Items[i].data) || isHeroShard)
                {
                    int Cid = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(Items[i].data).character_id;
                    int Num = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(Cid).summon_shard;
                    itemData.Add(new LTShowItemData(Items[i].data, Num, LTShowItemType.TYPE_HEROSHARD));
                }
                else
                {
                    itemData.Add(new LTShowItemData(Items[i].data, Items[i].quantityNum, Items[i].type));

                    if (Items[i].type.Equals(LTShowItemType.TYPE_HERO))
                    {
                        Hotfix_LT.Data.HeroStatTemplate tempStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(Items[i].data);
                        if (tempStat != null)
                        {
                            Hotfix_LT.Data.HeroInfoTemplate tempInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(tempStat.character_id);
                            if (tempInfo != null)
                            {
                                if (tempInfo.cultivateGift == 1)
                                {
                                    LTChargeManager.Instance.IsShowDrawGift = true;
                                    EB.Debug.Log("LTGetItemUIController OnAddToStack Draw is Success!");
                                }
                            }
                            else
                            {
                                EB.Debug.LogError("LTGetItemUIController OnAddToStack Hotfix_LT.Data.HeroInfoTemplate is Null, id = {0}", tempStat.character_id);
                            }
                        }
                        else
                        {
                            EB.Debug.LogError("LTGetItemUIController OnAddToStack Hotfix_LT.Data.HeroStatTemplate is Null, id = {0}", Items[i].data);
                        }
                    }
                }
            }

            if (m_type == DrawCardType.hc)
            {
                Scroll.mDMono.GetComponent<UIGrid>().cellHeight = 650;
            }
            else
            {
                Scroll.mDMono.GetComponent<UIGrid>().cellHeight = 400;
            }

            Scroll.SetItemDatas(itemData, false);
            if (DrawCardEvent.HideFxParent != null) DrawCardEvent.HideFxParent();

            {//抽奖特效表现
                GameObject DrawingFx;
                if (m_type == DrawCardType.gold)
                {
                    DrawingFx = SliverDrawingFx;
                    long_wait = new WaitForSeconds(2.7f);
                    FusionAudio.PostEvent("UI/New/ChouJiang", true);
                    SkipBtn.CustomSetActive(true);
                }
                else if(m_type == DrawCardType.ur)
                {
                    DrawingFx = GoldDrawingFx;
                    long_wait = new WaitForSeconds(8.2f);
                    FusionAudio.PostEvent("UI/New/ChouJiangZuanShi", true);
                    SkipBtn.CustomSetActive(true);
                }
                else
                {
                    DrawingFx = GoldDrawingFx;
                    long_wait = new WaitForSeconds(8.2f);
                    FusionAudio.PostEvent("UI/New/ChouJiangZuanShi", true);
                    if (!GuideNodeManager.IsGuide) SkipBtn.CustomSetActive(true);
                }
                DrawingFx.CustomSetActive(true);
                yield return long_wait;
                if (isSkip) yield break;
                MainHud.CustomSetActive(true);
            }

            if (isSkip)
            {
                yield break;
            }

            StartCoroutine(ShowItemAmi(itemData));
            yield return small_wait;
            FxHud.CustomSetActive(false);
        }

        /// <summary>
        /// 物品展示的协程
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowItemAmi(List<LTShowItemData> itemData)
        {
            FusionAudio.PostEvent("UI/ShowReward");
            SkipBtn.CustomSetActive(false);
            yield return shortItemWait;

            TitleObj.CustomSetActive(m_type != DrawCardType.hc);
            if (mDrawShowItemList == null || mDrawShowItemList.Count < itemData.Count)
            {
                if (mDrawShowItemList == null)
                {
                    mDrawShowItemList = new List<ShowGetNewItem>();
                    mDrawAnimatorList = new List<Animator>();
                }
                else
                {
                    mDrawShowItemList.Clear();
                    mDrawAnimatorList.Clear();
                }

                Transform t = Scroll.mDMono.transform;

                for (int i = 0; i < t.childCount; ++i)
                {
                    Transform tempTf = t.GetChild(i);

                    for (int j = 0; j < tempTf.childCount; j++)
                    {
                        ShowGetNewItem tempsi = tempTf.GetChild(j).GetMonoILRComponent<ShowGetNewItem>();
                        mDrawShowItemList.Add(tempsi);
                        Animator itemAni = tempsi.mDMono.transform.GetChild(0).GetComponent<Animator>();
                        mDrawAnimatorList.Add(itemAni);
                        itemAni.speed = 6;
                    }
                }
            }

            for (int i = 0; i < itemData.Count; i++)
            {
                if (mDrawShowItemList[i].mDMono.gameObject.activeSelf)
                {
                    Animator ItemAni = mDrawAnimatorList[i];
                    ShowGetNewItem crizyLotteryUIShowItem = mDrawShowItemList[i];
                    crizyLotteryUIShowItem.ProDeal();

                    ItemAni.enabled = true;
                    ItemAni.Play(ItemAni.runtimeAnimatorController.animationClips[0].name, 0, 0);

                    bool isHero = false;
                    if (crizyLotteryUIShowItem.mData.type == LTShowItemType.TYPE_HEROSHARD)
                    {
                        int Cid = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(crizyLotteryUIShowItem.mData.id).character_id;
                        isHero = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(Cid).summon_shard == crizyLotteryUIShowItem.mData.count;
                    }

                    if (tweenScaleWait == null)
                    {
                        AnimationClip[] clips = ItemAni.runtimeAnimatorController.animationClips;
                        float length = 0;
                        for (int k = 0; k < clips.Length; k++)
                        {
                            length += clips[k].length;
                        }
                        tweenScaleWait = new WaitForSeconds(length / 6);
                    }

                    yield return tweenScaleWait;

                    if (!isSkip && (crizyLotteryUIShowItem.mData.type == LTShowItemType.TYPE_HERO || isHero))
                    {
                        yield return shortItemWait;
                        //如果是英雄将弹出获得伙伴界面
                        GlobalMenuManager.Instance.Open("LTShowGetPartnerUI", crizyLotteryUIShowItem.mData);
                        yield return new WaitUntil(() => LTShowGetPartnerController.m_Open);
                        yield return new WaitUntil(() => !LTShowGetPartnerController.m_Open);
                        //等待从伙伴界面回来
                    }
                }
                else
                {
                    break;
                }
            }
            BottomBtnObj.CustomSetActive(m_type != DrawCardType.hc);
            TipObj.CustomSetActive(m_type == DrawCardType.hc);
            for (int i = 0; i < mDrawAnimatorList.Count; i++)
            {
                if (mDrawAnimatorList[i].transform.localScale == Vector3.zero)
                {
                    mDrawAnimatorList[i].transform.localScale = Vector3.one;
                    mDrawAnimatorList[i].enabled = false;
                }
            }
            FxLimit = false;
        }

        public override void OnCancelButtonClick()
        {
            if (FxLimit) return;
            if (GuideNodeManager.IsGuide && !MengBanController.Instance.controller.gameObject.activeSelf && !LTInstanceUtil.IsFirstChapterCompleted())
            {
                InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 1f);
                GlobalMenuManager.Instance.CloseMenu("LTDrawCardTypeUI");
            }
            base.OnCancelButtonClick();
        }

        public void OnBGBtnClick()
        {
            if (m_type == DrawCardType.hc)
            {
                OnCancelButtonClick();
            }
        }

        public void OnSkipBtnClick()
        {
            SkipBtn.CustomSetActive(false);
            isSkip = true;

            StopCoroutine(DrawCor);
            DrawCor = null;

            FxHud.CustomSetActive(false);
            MainHud.CustomSetActive(true);
            FusionAudio.PostEvent("UI/New/ChouJiangZuanShi", false);
            StartCoroutine(ShowItemAmi(itemData));
        }
    }
}
