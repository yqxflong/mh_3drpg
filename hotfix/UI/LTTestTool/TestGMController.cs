using UnityEngine;
using System.Collections;
using EB.Sparx;
    
using LitJson;
using System.IO;
    
namespace Hotfix_LT.UI
{
    public class TestGMController : UIControllerHotfix
    {
        public UILabel Name;
        public UILabel NameShadow;
        public GameObject StroyUI;
        public UILabel chapterlabel;
        private UILabel challengeLabel;


        private static TestGMController _Instance;
        public static TestGMController Instance {
            get {
                return _Instance;
            }
        }

        public override void Awake()
        {
            base.Awake();
            _Instance = this;

            var t = controller.transform;
            Name = t.GetComponent<UILabel>("Up/Name");
            NameShadow = t.GetComponent<UILabel>("Up/Name/Name (1)");
            StroyUI = t.FindEx("Story").gameObject;
            chapterlabel = t.GetComponent<UILabel>("Btns/MainCampaign/button/InputBtn/Label");
            challengeLabel = t.GetComponent<UILabel>("Btns/ChallengeCampaign/button/InputBtn/Label");
            t.GetComponent<UIButton>("Btns/Upgrade").onClick.Add(new EventDelegate(OnUpgradeClick));
            t.GetComponent<UIButton>("Btns/AddHc").onClick.Add(new EventDelegate(OnAddHcClick));
            t.GetComponent<UIButton>("Btns/AddGold").onClick.Add(new EventDelegate(OnAddGoldClick));
            t.GetComponent<UIButton>("Btns/PartnerUpgrade").onClick.Add(new EventDelegate(OnPartnerUpgradeClick));
            t.GetComponent<UIButton>("Btns/Gift").onClick.Add(new EventDelegate(OnGiftClick));
            t.GetComponent<UIButton>("Btns/FirstCombat").onClick.Add(new EventDelegate(OnFirstCombatClick));
            t.GetComponent<UIButton>("Btns/Story").onClick.Add(new EventDelegate(OpenStoryUI));
            t.GetComponent<UIButton>("Btns/notify").onClick.Add(new EventDelegate(TestNotifyBtnClick));
            t.GetComponent<UIButton>("Btns/Message").onClick.Add(new EventDelegate(OnTestMsgBtnClick));
            t.GetComponent<UIButton>("Btns/MainCampaign/button").onClick.Add(new EventDelegate(OnSendMainCampaignOpenReq));
            t.GetComponent<UIButton>("Btns/ChallengeCampaign/button").onClick.Add(new EventDelegate(OnSendChanllengeCampaignOpenReq));
            t.GetComponent<UIButton>("Up/SystemInfo").onClick.Add(new EventDelegate(() => OnSystemInfoBtnClick(t.GetComponent<Transform>("Up/SystemInfo"))));
            t.GetComponent<UIButton>("Story/Btns/Chapter0_1").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter0_1").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter0_2").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter0_2").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter0_3").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter0_3").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter0_4").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter0_4").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter0_5").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter0_5").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter1_1").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter1_1").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter1_2").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter1_2").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter2_1").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter2_1").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter2_2").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter2_2").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter2_3").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter2_3").gameObject)));
            t.GetComponent<UIButton>("Story/Btns/Chapter2_4").onClick.Add(new EventDelegate(() => OnStoryBtnClick(t.FindEx("Story/Btns/Chapter2_4").gameObject)));

            t.GetComponent<UIEventTrigger>("Story/Sprite").onClick.Add(new EventDelegate(CloseStroyUI));
        }

        public override IEnumerator OnAddToStack() {       
            Name.text = NameShadow.text =string.Format("UID:{0}", LoginManager.Instance.LocalUserId.Value);
            return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack() {
            DestroySelf();
            yield break;
        }
    
        private Hashtable GetHeroStats(long heroId) {
            string dataId = string.Format("heroStats.{0}", heroId);
            Hashtable heroStats = null;
            if (!DataLookupsCache.Instance.SearchDataByID(dataId, out heroStats)) {
                EB.Debug.LogError("HeroPortraitController: heroStats not found, dataId = {0}", dataId);
                return null;
            }
    
            return heroStats;
        }
    
        public void OnUpgradeClick() {
    
            int exp=BalanceResourceUtil.GetResValue("xp");
            EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("xp", exp+5000000);
    
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "您已增加500w经验!");
            
        }
    
        public IEnumerator OnAddTicketClick()
        {
            yield return new WaitForSeconds(1.0f);
            int ticket = BalanceResourceUtil.GetAwakenTicket();
            EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().AddTicket(ticket+50, null);
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "您已增加50觉醒门票!");
        }
    
        public void OnAddHcClick() {
            int hc = BalanceResourceUtil.GetUserDiamond();
            EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("hc", hc+10000);
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "您已增加1w钻石!");
            StartCoroutine(OnAddTicketClick());
        }
    
        public void OnAddGoldClick() {
            int gold = BalanceResourceUtil.GetUserGold();
            EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("gold", gold+100000);
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "您已增加10w金币!");
        }
    
        public void OnPartnerUpgradeClick() {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/userres/debugAddBuddyXp");
            int exp = 5000000;
            request.AddData("xp", exp);
            lookupsManager.Service(request, null);
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "您的所有伙伴已增加500w经验！");
        }
    
        public void OnGiftClick() {
            InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
            invManager.AddItemToInv("5019", 1, BuyEquipCallback);
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "您已获得超级礼包!");
        }
    
        public void OnFirstCombatClick() {
            controller.Close();
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            System.Action startCombatCallback = delegate () {
                UIStack.Instance.ShowLoadingScreen(() =>
                {
                    CombatCamera.isBoss = true;
                    MainLandLogic.GetInstance().RequestFastCombatTransition(eBattleType.FastCampaignBattle, "1_1");
                }, false, true);
            };
            BattleReadyHudController.Open(eBattleType.FastCampaignBattle, startCombatCallback);
        }
    
        public void OnStoryBtnClick(GameObject obj)
        {
            string name = obj.name;
            GlobalMenuManager.Instance.Open("LTStoryHud", name); 
        }
    
        public void OpenStoryUI()
        {
            StroyUI.CustomSetActive(true);
        }
    
        public void CloseStroyUI()
        {
            StroyUI.CustomSetActive(false);
        }
    
        public void OnSystemInfoBtnClick(Transform InfoBtn)
        {
            if (InfoBtn != null)
            {
                UILabel InfoLabel = InfoBtn.GetChild(0).GetComponent<UILabel>();
                if(string.IsNullOrEmpty(InfoLabel.text))
                {
                    string text =
                        "显卡着色器级别：" + SystemInfo.graphicsShaderLevel.ToString() + "\n" +
                        "显卡名称：" + SystemInfo.graphicsDeviceName.ToString() + "\n" +
                        "显存大小：" + SystemInfo.graphicsMemorySize.ToString() + "\n" +
                        "内存大小：" + SystemInfo.systemMemorySize.ToString() + "\n" +
                        "处理器频率：" + SystemInfo.processorFrequency.ToString() + "\n" +
                        "处理器类型：" + SystemInfo.processorType.ToString() + "\n" +
                        "显卡类型：" + SystemInfo.graphicsDeviceType.ToString() + "\n" +
                        "\n" + "当前画质等级：" + QualitySettings.GetQualityLevel().ToString() + "\n" +
                        "EB.Time当前时间：" + EB.Time.LocalNow.Hour + "时" + EB.Time.LocalNow.Minute + "分" + EB.Time.LocalNow.Second + "秒" + "\n" +
                        "Umeng设备ID：" + Umeng.GA.GetTestDeviceInfo();
                    InfoLabel.text = text;
                }
                else
                {
                    InfoLabel.text = string.Empty;
                }
            }
        }
    
        public void TestNotifyBtnClick()
        {
            EB.Debug.Log("测试发通知喽");
            //热更工程去除宏 先屏蔽
    // #if !UNITY_EDITOR && UNITY_ANDROID
    //     using (AndroidJavaClass jc = new AndroidJavaClass("org.manhuang.android.NotificationManager"))
    //         {
    //             EB.Debug.Log("发送至安卓");
    //             jc.CallStatic("scheduleLocalNotification", "测试通知", "该通知内容为null，请无视之", (long)EB.Time.Now+50 ,0);
    //         }
    // #endif
        }
    
        public void OnTestMsgBtnClick()
        {
            Hashtable ht = Johny.HashtablePool.Claim();
            ht.Add("level", 5);
            ht.Add("type", 1);
            GlobalMenuManager.Instance.Open("LTInstanceMessageView", ht);
        }
    
        public void OnSendMainCampaignOpenReq()
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/lostmaincampaign/debugallstar");
            if (int.TryParse(chapterlabel.text, out int chapter))
            {
                request.AddData("chapter", chapter);
                lookupsManager.Service(request, null);
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "已解锁主线副本指定关卡，重新登录后生效!");
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "请输入指定章节，如： 101 ");
            }
        }

        public void OnSendChanllengeCampaignOpenReq()
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/lostchallengecampaign/enterChapter");
            if (int.TryParse(challengeLabel.text, out int chapter))
            {
                request.AddData("level", chapter);
                lookupsManager.Service(request, null);
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "已解锁挑战副本指定关卡，重新登录后生效!");
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, "请输入指定层数，如： 1 ");
            }
        }

        void BuyEquipCallback(bool ifSuccess) {
            if (ifSuccess) {
                //InventoryDialog.Instance.UpdateInventory();
            }
        }
    
        public override bool ShowUIBlocker {
            get {
                return true; 
            }
        }
        public override void StartBootFlash() {
			SetCurrentPanelAlpha(1);
			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j) {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }
    }
}
