using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTMainInstanceCampaignCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TitleLabel = t.GetComponent<UILabel>("Bg/Top/Title");
            DescLabel = t.GetComponent<UILabel>("Content/Info/Desc");

            StarList = new List<UISprite>();
            StarList.Add(t.GetComponent<UISprite>("Content/Info/List/Star"));
            StarList.Add(t.GetComponent<UISprite>("Content/Info/List/Star (1)"));
            StarList.Add(t.GetComponent<UISprite>("Content/Info/List/Star (2)"));

            VigorCostLabel = t.GetComponent<UILabel>("Content/Vigor/Cost");

            DropItemList = new List<LTShowItem>();
            DropItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/Drop/List/Item"));
            DropItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/Drop/List/Item (1)"));
            DropItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/Drop/List/Item (2)"));
            DropItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/Drop/List/Item (3)"));

            BlitzLabel = t.GetComponent<UILabel>("Content/QuickBatlleLabel");
            BattleBtn = t.GetComponent<UIButton>("Content/EnterBtn");
            BlitzBtn = t.GetComponent<UIButton>("Content/BlitzBtn");
            MultiBlitzBtn = t.GetComponent<UIButton>("Content/MultiBlitzBtn");
            FreeObj = t.FindEx("Content/Free").gameObject;
            CostObj = t.FindEx("Content/Vigor").gameObject;
            controller.backButton = t.GetComponent<UIButton>("Bg/Top/CancelBtn");

            t.GetComponent<UIButton>("Content/EnterBtn").onClick.Add(new EventDelegate(OnEnterBtnClick));

            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/BlitzBtn").clickEvent.Add(new EventDelegate(OnBlitzBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/MultiBlitzBtn").clickEvent.Add(new EventDelegate(OnMultiBlitzBtnClick));

        }
        
        public static int CampaignId
        {
            get { return campaignId; }
        }
    
        private System.Action mCallback;
    
        private static int campaignId = 0;
    
        private string mTargetItemId = string.Empty;
    
        private int m_StarNum = 0;
    
        private bool m_IsComplete = false;
    
        private Hotfix_LT.Data.LostMainCampaignsTemplate mainTpl;
    
        private bool isShowFree;
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override void SetMenuData(object param)
        {
            Hashtable data = param as Hashtable;
            if (data != null)
            {
                campaignId = (int)data["id"];
                mCallback = data["callback"] as System.Action;
                mTargetItemId = EB.Dot.String("targetItemId", data, string.Empty);
            }
            InitUI();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }
    
        public override void OnCancelButtonClick()
        {
            base.OnCancelButtonClick();
            campaignId = 0;
            mTargetItemId = string.Empty;
            mCallback = null;
        }
    
        private void InitUI()
        {
            if (campaignId <= 0)
            {
                return;
            }
    
            mainTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(campaignId.ToString());
            if (mainTpl == null)
            {
                return;
            }
    
            DataLookupsCache.Instance.SearchIntByID(string.Format("userCampaignStatus.normalChapters.{0}.campaigns.{1}.star", mainTpl.ChapterId, campaignId), out m_StarNum);
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userCampaignStatus.normalChapters.{0}.campaigns.{1}.complete", mainTpl.ChapterId, campaignId), out m_IsComplete);
    
            TitleLabel.text = mainTpl.Name;
            DescLabel.text = mainTpl.Desc;
            for (int i = 0; i < StarList.Count; i++)
            {
                if (i < m_StarNum)
                {
                    StarList[i].gameObject.CustomSetActive(true);
                }
                else
                {
                    StarList[i].gameObject.CustomSetActive(false);
                }
            }
    
            isShowFree = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("DungeonFirstPassFree") > 0 && m_StarNum <= 0;
            FreeObj.CustomSetActive(isShowFree);
            CostObj.CustomSetActive(!isShowFree);
            VigorCostLabel.text = mainTpl.CostVigor.ToString();
    
            for (int i = 0; i < DropItemList.Count; i++)
            {
                if (i < mainTpl.AwardIconList.Count)
                {
                    DropItemList[i].mDMono.gameObject.CustomSetActive(true);
                    int tempID = int.Parse(mainTpl.AwardIconList[i]);
                    string id = tempID < 1000 ? BalanceResourceUtil.GetResStrID(tempID) : tempID.ToString();
                    string type = tempID < 1000 ? LTShowItemType.TYPE_RES : LTShowItemType.TYPE_GAMINVENTORY;
                    DropItemList[i].LTItemData = new LTShowItemData(id, 0, type, false);
                }
                else
                {
                    DropItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
    
            bool isCanBlitz = m_IsComplete && m_StarNum >= 3;
    
            BlitzLabel.gameObject.CustomSetActive(!isCanBlitz);
            BlitzBtn.gameObject.CustomSetActive(isCanBlitz);
            MultiBlitzBtn.gameObject.CustomSetActive(isCanBlitz);
            BattleBtn.transform.localPosition = isCanBlitz ? RightBattlePos : LeftBattlePos;
            BattleBtn.gameObject.CustomSetActive(true);
        }
    
        public UILabel TitleLabel;
    
        public UILabel DescLabel;
    
        public List<UISprite> StarList;
    
        public UILabel VigorCostLabel;
    
        public List<LTShowItem> DropItemList;
    
        public UILabel BlitzLabel;
    
        public UIButton BattleBtn;
    
        private Vector3 LeftBattlePos = new Vector3(0, -394, 0);
    
        private Vector3 RightBattlePos = new Vector3(336, -394, 0);
    
        public UIButton BlitzBtn;
    
        public UIButton MultiBlitzBtn;
    
        public GameObject FreeObj;
        public GameObject CostObj;


        private bool isRequest = false;
        public void OnBlitzBtnClick()
        {
            if (campaignId <= 0)
            {
                return;
            }
    
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10077);
            if (ft!=null&&!ft.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                return;
            }
    
            int vaildTimes = IsVigorEnough(1);
            if (vaildTimes <= 0)
            {
                BalanceResourceUtil.TurnToVigorGotView();
                return;
            }

            if (isRequest) return;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            isRequest = true;

            if (mCallback != null)
            {
                mCallback();
            }
    
            LTInstanceMapModel.Instance.RequestMainBlitzCampaign(campaignId, vaildTimes, delegate 
            {
                isRequest = false;
                List<LTMainInstanceBlitzData> list = LTInstanceUtil.GetBlitzData();
                Hashtable data = Johny.HashtablePool.Claim();
                data.Add("list", list);
                data.Add("num", 1);
                data.Add("ItemId", mTargetItemId);
                GlobalMenuManager.Instance.Open("LTMainInstanceBlitzView", data);
    
                if (!string.IsNullOrEmpty(mTargetItemId))
                {
                    controller.Close();
                }
            });
        }
    
        public void OnMultiBlitzBtnClick()
        {
            if (campaignId <= 0)
            {
                return;
            }
    
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10077);
            if (ft != null && !ft.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                return;
            }
    
            int vaildTimes = IsVigorEnough(10);
            if (vaildTimes <= 0)
            {
                BalanceResourceUtil.TurnToVigorGotView();
                return;
            }

            if (isRequest) return;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            isRequest = true;

            if (mCallback != null)
            {
                mCallback();
            }
    
            LTInstanceMapModel.Instance.RequestMainBlitzCampaign(campaignId, vaildTimes, delegate
            {
                isRequest = false;
                List<LTMainInstanceBlitzData> list = LTInstanceUtil.GetBlitzData();
                for(int i=0;i<list.Count; ++i)
                {
                    FusionTelemetry.ItemsUmengCurrency(list[i].ItemList, "主线扫荡");
                }
    
                Hashtable data = Johny.HashtablePool.Claim();
                data.Add("list", list);
                data.Add("num", 10);
                data.Add("ItemId", mTargetItemId);
                GlobalMenuManager.Instance.Open("LTMainInstanceBlitzView", data);
    
                if (!string.IsNullOrEmpty(mTargetItemId))
                {
                    controller . Close();
                }
            });
        }
    
        public void OnEnterBtnClick()
        {
            if (campaignId <= 0)return;
    
            if (mainTpl == null) return;
            
            if (AllianceUtil.GetIsInTransferDart("")) return;

            if (isRequest) return;

            if (!isShowFree)
            {
                int vaildTimes = IsVigorEnough(1);
                if (vaildTimes <= 0)
                {
                    BalanceResourceUtil.TurnToVigorGotView();
                    return;
                }
            }

            Hashtable data = Johny.HashtablePool.Claim();
            data.Add("startCombatCallback", new System.Action(()=> {
                if (campaignId <= 0)
                {
                    return;
                }

                if (mCallback != null)
                {
                    mCallback();
                }
                LTInstanceMapModel.Instance.RequestMainFightCampaign(campaignId, SceneLogicManager.isMainlands()?1:0);
            }));
            data.Add("enemyLayout", mainTpl.EncounterGroupId);
            data.Add("battleType", eBattleType.MainCampaignBattle);

            GlobalMenuManager.Instance.Open("LTCombatReadyUI", data);
        }
    
        private int IsVigorEnough(int times)
        {
            int curVigor = 0;
            DataLookupsCache.Instance.SearchIntByID("res.vigor.v", out curVigor);
            if (curVigor >= (mainTpl.CostVigor * times))
            {
                return times;
            }
            else
            {
                return curVigor / mainTpl.CostVigor;
            }
        }
    }
}
