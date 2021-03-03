using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public enum ResourceInstanceType
    {
        Gold = 0,
        Exp,
        Null,
    };

    public class LTResourceInstanceHudController : UIControllerHotfix
    {
        public override bool IsFullscreen()
        {
            return true;
        }

        public override bool ShowUIBlocker
        {
            get
            {
                return false;
            }
        }

        private List<GameObject> ItemList;
        private UISprite DropSprite;
        private UILabel PartnerLabel;
        private UICenterOnChild CenterOnChild;
        private UILabel LevelDescLabel;
        private UILabel LevelNumLabel;
        private Color GoldColor;
        private Color ExpColor;
        private GameObject GoldFx;
        private GameObject ExpFx;
        private List<LTpartnerInfoItem> mPartnerDataItems;
        private Transform StartPos;
        private UIButton BattleBtn;
        private UILabel LastTimesLabel;
        private UILabel BlitzTimesLabel;
        private UIButton LockBtn;
        private UIButton BlitzBtn;
        private UILabel UnLockLevelLabel;

        private static ResourceInstanceType mInstanceType;
        private static int mActivityId;
        private Hotfix_LT.Data.SpecialActivityTemplate mActivityTbl;
        public static Hotfix_LT.Data.SpecialActivityLevelTemplate mChooseLevel;
        private List<Hotfix_LT.Data.SpecialActivityLevelTemplate> mlevelsTbl = new List<Hotfix_LT.Data.SpecialActivityLevelTemplate>();
        private int mLeftTimes;
        private EnterVigorController C_VigorController, B_VigorController;
        public override void Awake()
        {
            base.Awake();

            var tRoot = controller.transform;
            controller.backButton = tRoot.GetComponent<UIButton>("Edge/TopRight/CancelBtn");
            var tGrid = tRoot.FindEx("Center/Scroll/PlaceHolder/Grid");
            ItemList = new List<GameObject>();

            for (var i = 0; i < tGrid.childCount; i++)
            {
                ItemList.Add(tGrid.GetChild(i).gameObject);
            }
            
            DropSprite = tRoot.GetComponent<UISprite>("Edge/Bottom/LevelDesc/Icon");
            PartnerLabel = tRoot.GetComponent<UILabel>("Edge/BottomRight/Desc");
            CenterOnChild = tGrid.GetComponent<UICenterOnChild>();
            LevelDescLabel = tRoot.GetComponent<UILabel>("Edge/Bottom/LevelDesc/Desc");
            LevelNumLabel = tRoot.GetComponent<UILabel>("Edge/Bottom/LevelDesc/Num");
            GoldColor = new Color32(251, 239, 70, 255);
            ExpColor = new Color32(102, 255, 254, 255);
            GoldFx = tRoot.Find("Center/GoldBG").gameObject;
            ExpFx = tRoot.Find("Center/ExpBG").gameObject;
            var tItemList = tRoot.FindEx("Edge/BottomRight/ItemList");
            mPartnerDataItems = new List<LTpartnerInfoItem>();

            for (var i = 0; i < tItemList.childCount; i++)
            {
                mPartnerDataItems.Add(tItemList.GetChild(i).GetMonoILRComponent<LTpartnerInfoItem>());
            }

            StartPos = tGrid.FindEx("Sprite (3)/StartPos");

            BattleBtn = tRoot.GetComponent<UIButton>("Edge/Bottom/StartBtn");
            BattleBtn.onClick.Clear();
            BattleBtn.onClick.Add(new EventDelegate(OnBattleBtnClick));
            LastTimesLabel = BattleBtn.transform.GetComponent<UILabel>("Num");

            BlitzBtn = tRoot.GetComponent<UIButton>("Edge/Bottom/BlitzBtn");
            BlitzBtn.onClick.Clear();
            BlitzBtn.onClick.Add(new EventDelegate(OnBlitzBtnClick));
            BlitzTimesLabel = BlitzBtn.transform.GetComponent<UILabel>("Num");

            LockBtn = tRoot.GetComponent<UIButton>("Edge/Bottom/LockBtn");
            LockBtn.onClick.Clear();
            LockBtn.onClick.Add(new EventDelegate(OnBattleBtnClick));
            UnLockLevelLabel = LockBtn.transform.GetComponent<UILabel>("Desc");

            var noticeBtn = tRoot.GetComponent<UIButton>("Edge/Notice");
            noticeBtn.onClick.Clear();
            noticeBtn.onClick.Add(new EventDelegate(OnNoticeBtnClick));

            C_VigorController = tRoot.GetMonoILRComponent<EnterVigorController>("Edge/Bottom/StartBtn/Sprite");
            B_VigorController = tRoot.GetMonoILRComponent<EnterVigorController>("Edge/Bottom/BlitzBtn/Sprite");
        }

        public override void Start()
        {
            base.Start();
            controller.backButton.onClick.Add(new EventDelegate(OnCancelBtnClick));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            if (param != null)
            {
                string s = param as string;
                if (s.Contains("Gold"))
                {
                    mInstanceType = ResourceInstanceType.Gold;
                    mActivityId = 9001;
                    ExpFx.CustomSetActive(false);
                    GoldFx.CustomSetActive(true);
                }
                else if (s.Contains("Exp"))
                {
                    mInstanceType = ResourceInstanceType.Exp;
                    mActivityId = 9002;

                    GoldFx.CustomSetActive(false);
                    ExpFx.CustomSetActive(true);
                }
                else
                {
                    EB.Debug.LogError("LTResourceInstanceHudController.SetMenuData: type {0} vaild", param);
                }
            }
        }

        public override IEnumerator OnPrepareAddToStack()
        {
            yield return base.OnPrepareAddToStack();
        }

        public override IEnumerator OnAddToStack()
        {
            FusionAudio.StartBGM();

            //每次进入资源界面清空类型记录防止干扰
            LTDrawCardLookupController.DrawType = DrawCardType.none;
            mChooseLevel = null;
            mActivityTbl = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(mActivityId);
            mlevelsTbl = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivityLevels(mActivityId);
            if (mInstanceType == ResourceInstanceType.Gold)
            {
                ExpFx.CustomSetActive(false);
                GoldFx.CustomSetActive(true);
            }
            else if (mInstanceType == ResourceInstanceType.Exp)
            {
                GoldFx.CustomSetActive(false);
                ExpFx.CustomSetActive(true);
            }
            if (mActivityTbl == null || mlevelsTbl.Count < 0)
            {
                yield break;
            }
            InitUI();
            CenterOnChild.onCenter += OnCenter;
            GlobalMenuManager.Instance.PushCache("LTResourceInstanceUI", (mActivityId == 9001) ? "Gold" : "Exp");
            LTResourceInstanceManager.Instance.GetResourceInstanceTime(mActivityId, delegate
            {
                InitSelect();                             
            });
            yield return null;
            yield return base.OnAddToStack();
            InitSelect();//start后執行一次，防止网络卡顿造成界面异常
        }

        public override IEnumerator OnRemoveFromStack()
        {
            FusionAudio.StopBGM();
            CenterOnChild.onCenter -= OnCenter;
            CenterOnChild.CenterOn(StartPos);
            curObj = null;
            DestroySelf();
            yield break;
        }

        private void InitUI()
        {
            if (mInstanceType == ResourceInstanceType.Gold)
            {
                DropSprite.spriteName = "Ty_Icon_Gold";
                LevelDescLabel.color = GoldColor;
                LevelNumLabel.color = GoldColor;
                PartnerLabel.text = EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_3342");
                ExpFx.CustomSetActive(false);
                GoldFx.CustomSetActive(true);
            }
            else if (mInstanceType == ResourceInstanceType.Exp)
            {
                DropSprite.spriteName = "Ty_Icon_Huobanjingyan";
                LevelDescLabel.color = ExpColor;
                LevelNumLabel.color = ExpColor;
                PartnerLabel.text = EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_3693");
                GoldFx.CustomSetActive(false);
                ExpFx.CustomSetActive(true);
            }
            int OldVigor = NewGameConfigTemplateManager.Instance.GetResourceBattleEnterVigor(mInstanceType);
            C_VigorController.Init(OldVigor, 0, false);
            B_VigorController.Init(OldVigor, 0, false);

            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i] == null)
                {
                    continue;
                }
                LTResourceInstanceItemTmp itemTmp = ItemList[i].GetMonoILRComponent<LTResourceInstanceItemTmp>();
                if (itemTmp != null)
                {
                    itemTmp.InitState(mlevelsTbl[i], mInstanceType);
                }
            }
        }

        private void InitSelect()
        {
            int index = 0;

            if (mInstanceType == ResourceInstanceType.Gold)
            {
                index = GetIndexById(LTResourceInstanceManager.Instance.PassDifficeGlod);
                index = mlevelsTbl != null && mlevelsTbl.Count > index && LTResourceInstanceManager.Instance.IsLevelEnough(mlevelsTbl[index]) ? index : Mathf.Max(index - 1, 0);
            }
            else if (mInstanceType == ResourceInstanceType.Exp)
            {
                index = GetIndexById(LTResourceInstanceManager.Instance.PassDifficeExp);
                index = mlevelsTbl != null && mlevelsTbl.Count > index && LTResourceInstanceManager.Instance.IsLevelEnough(mlevelsTbl[index]) ? index : Mathf.Max(index - 1, 0);
            }
            mLeftTimes = mActivityTbl.times - GetFightTimes(mActivityId);
            LastTimesLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_6377"), mLeftTimes.ToString());
            BlitzTimesLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_6377"), mLeftTimes.ToString());
            if (ItemList != null && index < ItemList.Count && CenterOnChild != null && ItemList[index] != null)
            {
                CenterOnChild.CenterOn(ItemList[index].transform);
            }
        }

        private int GetIndexById(int id)
        {
            for (int i = 0; i < mlevelsTbl.Count; i++)
            {
                if (mlevelsTbl[i].id == id)
                {
                    return i;
                }
            }
            return 0;
        }

        private void OnItemSelect()
        {
            int index = ItemList.IndexOf(curObj);
            mChooseLevel = mlevelsTbl[index];
            for (var i = 0; i < ItemList.Count; i++)
            {
                var item = ItemList[i];
                item.transform.localScale = new Vector3(1f, 1f, 1f);
                item.GetComponent<UIWidget>().alpha = 0.75f;
            }
            curObj.GetComponent<UIWidget>().alpha = 1f;
            curObj.transform.localScale = new Vector3(1.45f, 1.45f, 1.45f);

            LevelDescLabel.text = mChooseLevel.name;
            LevelNumLabel.text = mChooseLevel.gold.ToString();

            UpdatePartnerList();

            SetBtnState();
        }

        private void UpdatePartnerList()
        {
            for (int i = 0; i < mPartnerDataItems.Count; i++)
            {
                if (i < mChooseLevel.recommend_partners_id.Length)
                {
                    Hotfix_LT.Data.HeroInfoTemplate data = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(mChooseLevel.recommend_partners_id[i]);
                    mPartnerDataItems[i].Fill(data);
                }
                else
                {
                    mPartnerDataItems[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
        }

        private void SetBtnState()
        {
            bool isLevelEnough = LTResourceInstanceManager.Instance.IsLevelEnough(mChooseLevel);
            bool isCanBlitz = LTResourceInstanceManager.Instance.IsCanBlitz(mChooseLevel, mInstanceType);
            BattleBtn.gameObject.CustomSetActive(isLevelEnough && !isCanBlitz);
            LockBtn.gameObject.CustomSetActive(!isLevelEnough);
            C_VigorController.mDMono.gameObject.CustomSetActive(isLevelEnough);
            BlitzBtn.gameObject.CustomSetActive(isCanBlitz);
            UnLockLevelLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_BattleReadyHudController_12002"), mChooseLevel.level);
        }

        public void OnBattleBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");

            if (AllianceUtil.GetIsInTransferDart(null))
            {
                return;
            }

            if (BalanceResourceUtil.EnterVigorCheck(NewGameConfigTemplateManager.Instance.GetResourceBattleEnterVigor(mInstanceType)))
            {
                return;
            }

            if (mLeftTimes <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_6819"));
            }
            else if (!LTResourceInstanceManager.Instance.IsLevelEnough(mChooseLevel))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_7027"), mChooseLevel.level));
            }
            else if (LTResourceInstanceManager.Instance.IsLock(mChooseLevel, mInstanceType))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_7254"));
            }
            else
            {
                BattleReadyHudController.Open(mActivityTbl.battle_type, StartBattleRequest, mChooseLevel.layout);
            }
        }

        private void StartBattleRequest()
        {
            LTResourceInstanceManager.Instance.StartBattle(mChooseLevel.id, (int)mActivityTbl.battle_type);
        }

        private bool _startBlitzRequesting = false;

        public void OnBlitzBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (AllianceUtil.GetIsInTransferDart(null))
            {
                return;
            }

            if (BalanceResourceUtil.EnterVigorCheck(NewGameConfigTemplateManager.Instance.GetResourceBattleEnterVigor(mInstanceType)))
            {
                return;
            }

            if (mLeftTimes <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_6819"));
                return;
            }
            else if (!LTResourceInstanceManager.Instance.IsLevelEnough(mChooseLevel))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_7027"), mChooseLevel.level));
                return;
            }
            else if (LTResourceInstanceManager.Instance.IsLock(mChooseLevel, mInstanceType))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_7254"));
                return;
            }

            if (_startBlitzRequesting)
            {
                return;
            }

            _startBlitzRequesting = true;

            LTResourceInstanceManager.Instance.Blitz(mChooseLevel.id, (int)mActivityTbl.battle_type, delegate
            {
                Hashtable table = Johny.HashtablePool.Claim();
                table["isConfirm"] = false;
                Action action = InitUI;
                table["action"] = action;
                table["battleType"] = (mInstanceType == ResourceInstanceType.Gold ? eBattleType.TreasuryBattle : eBattleType.ExpSpringBattle);
                GlobalMenuManager.Instance.Open("LTResInstanceBlitzView", table);
                if (mInstanceType == ResourceInstanceType.Gold)
                {
                    FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.gold_camp_topic,
                    FusionTelemetry.GamePlayData.gold_camp_event_id, FusionTelemetry.GamePlayData.gold_camp_umengId, "reward");
                }
                else
                {
                    FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.exp_camp_topic,
                    FusionTelemetry.GamePlayData.exp_camp_event_id, FusionTelemetry.GamePlayData.exp_camp_umengId, "reward");
                }
                TimerManager.instance.AddTimer(1000, 1, (i) => _startBlitzRequesting = false);
            });
        }

        private GameObject curObj;

        public void OnCenter(GameObject obj)
        {
            if (curObj == obj)
            {
                return;
            }
            curObj = obj;
            OnItemSelect();

            EB.Debug.Log("LTResourceInstanceHudController.OnCenter:curObj = {0}", curObj.name);
        }

        public void OnNoticeBtnClick()
        {
            if (mInstanceType == ResourceInstanceType.Gold)
            {
                GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_RULE_GOLDRESOURSE"));
            }
            else if (mInstanceType == ResourceInstanceType.Exp)
            {
                GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_RULE_EXPRESOURSE"));
            }
        }

        public void OnCancelBtnClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTResourceInstanceUI");
            mActivityId = 0;
            mChooseLevel = null;
            mInstanceType = ResourceInstanceType.Null;
        }

        private int GetFightTimes(int activityid)
        {
            int times = 0;
            string path = string.Format("special_activity.{0}.c_times", activityid);
            DataLookupsCache.Instance.SearchIntByID(path, out times);
            return times;
        }
    }
}
