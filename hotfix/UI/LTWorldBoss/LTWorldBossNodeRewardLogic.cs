using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTWorldBossNodeRewardLogic : DynamicMonoHotfix
    {
        public LTShowItem RewardItem;
        public UILabel DescLabel;
        public UILabel NumLabel;
        public UISprite DiceSprite;
        public UIServerRequest DiceServerRequest;
        public UIServerRequest RewardServerRequest;
        public UILabel MaxNameLabel;
        public UILabel MaxNumLabel;
        public GameObject LockObj;
        public UILabel TipLabel;
    
        private float node;
        private bool isRolling;
        private string rewardId;
        private TweenAlpha mTweenAlpha;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            RewardItem = t.GetMonoILRComponent<LTShowItem>("LTShowItem");
            DescLabel = t.GetComponent<UILabel>("Dice/DescLabel");
            NumLabel = t.GetComponent<UILabel>("Dice/NumLabel");
            DiceSprite = t.GetComponent<UISprite>("Dice/ClickBtn");
            DiceServerRequest = t.GetComponent<UIServerRequest>("Dice/ClickBtn");
            RewardServerRequest = t.GetComponentEx<UIServerRequest>();
            MaxNameLabel = t.GetComponent<UILabel>("NameLabel");
            MaxNumLabel = t.GetComponent<UILabel>("NumLabel");
            LockObj = t.FindEx("Dice/LockSprite").gameObject;
            TipLabel = t.GetComponent<UILabel>("TipLabel");

            t.GetComponent<UIButton>("Dice/ClickBtn").onClick.Add(new EventDelegate(OnDiceBtnClick));
            t.GetComponent<UIServerRequest>("Dice/ClickBtn").onResponse.Add(new EventDelegate(mDMono, "OnFetchData"));

            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnMaxRollUpdate, OnMaxRollUpdateFunc);
            mTweenAlpha = t.GetComponentEx<TweenAlpha>();
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnMaxRollUpdate, OnMaxRollUpdateFunc);
        }
    
        public void InitUI(float node)
        {
            this.node = node;
            if (string.IsNullOrEmpty(rewardId))
            {
                rewardId = Hotfix_LT.Data.EventTemplateManager.Instance.GetBossStageReward(node);
                rewardId = string.IsNullOrEmpty(rewardId) ? "1001" : rewardId;
                RewardItem.LTItemData = new LTShowItemData(rewardId, 0, LTShowItemType.TYPE_GAMINVENTORY, true);
                RewardItem.mDMono.gameObject.CustomSetActive(false);
                TipLabel.text = string.Format(EB.Localizer.GetString("ID_WORLD_BOSS_STAGE_TIP"), (node * 100).ToString("f0"));
            }
    
            UpdateUI();
            OpenUI();
        }
    
        private void UpdateUI()
        {
            bool mCanRoll = LTWorldBossDataManager.Instance.GetRollStageValue() <= node;
            TipLabel.gameObject.CustomSetActive(!mCanRoll);
            LockObj.CustomSetActive(!mCanRoll);
            if (mCanRoll)
            {
                bool canRoll=true;
                if (DataLookupsCache.Instance.SearchDataByID(string.Format("world_boss.myRollData.stage.{0}.canRoll", node.ToString().Replace('.', '_')), out canRoll))
                {
                    if (!isRolling)
                    {
                        DiceSprite.gameObject.CustomSetActive(canRoll);
                        DescLabel.gameObject.CustomSetActive(!canRoll);
                        NumLabel.gameObject.CustomSetActive(!canRoll);
                    }
                    if (!canRoll)
                    {
                        int mRollDice = 0;
                        DataLookupsCache.Instance.SearchIntByID(string.Format("world_boss.myRollData.stage.{0}.rollDice", node.ToString().Replace('.', '_')), out mRollDice);
                        LTUIUtil.SetText(NumLabel, mRollDice.ToString());
                    }
                }
                else
                {
                    if (!isRolling)
                    {
                        DiceSprite.gameObject.CustomSetActive(true);
                        DescLabel.gameObject.CustomSetActive(false);
                        NumLabel.gameObject.CustomSetActive(false);
                    }
                }
                int rollDice = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("world_boss.rollData.stage.{0}.luckyUser.rollDice", node.ToString().Replace('.', '_')), out rollDice);
                LTUIUtil.SetText(MaxNumLabel, rollDice > 0 ? rollDice.ToString() : string.Empty);
                string name = string.Empty;
                DataLookupsCache.Instance.SearchDataByID<string>(string.Format("world_boss.rollData.stage.{0}.luckyUser.name", node.ToString().Replace('.', '_')), out name);
                LTUIUtil.SetText(MaxNameLabel, name);
    
            }
        }
    
        private void OpenUI()
        {
            if (mTweenAlpha == null)
            {
                mTweenAlpha = mDMono.GetComponent<TweenAlpha>();
            }
    
            CloseUI();
    
            if (mTweenAlpha != null)
            {
                if (mTweenAlpha.onFinished.Count == 0)
                {
                    mTweenAlpha.AddOnFinished(delegate
                    {
                        RewardItem.mDMono.gameObject.CustomSetActive(true);
                    });
                }
                mTweenAlpha.ResetToBeginning();
                mTweenAlpha.PlayForward();
            }
        }
    
        public void CloseUI()
        {
            if (RewardItem.mDMono.gameObject.activeSelf)
            {
                RewardItem.mDMono.gameObject.CustomSetActive(false);
                mDMono.GetComponent<UIWidget>().alpha = 0.2f;
            }
        }
    
        public void OnGetRewardResponse()
        {
            
        }
    
        public void OnDiceBtnClick()
        {
            if (!LTWorldBossDataManager.Instance.IsLive())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Concat(EB.Localizer.GetString("ID_uifont_in_LTLegionWarQualify_End_4")));
                return;
            }
    
            if (!LTWorldBossDataManager.Instance.IsWorldBossStart())
            {
                MessageTemplateManager.ShowMessage(902090);
                return;
            }
    
            int d = 0;
            DataLookupsCache.Instance.SearchDataByID("world_boss.rankdata.d", out d);
            if (d<=0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_WORLD_BOSS_STAGE_TIP2"));
                return;
            }
            DiceServerRequest.parameters[0].parameter = node.ToString();
            DiceServerRequest.SendRequest();
            DiceSprite.GetComponent<UIButton>().enabled = false;
        }
    
        private IEnumerator ScrollDide()
        {
            float firstTime = 0;
            while (firstTime < 0.5f)
            {
                firstTime += RealTime.deltaTime;
                DiceSprite.transform.Rotate(new Vector3(0, 0, 50) * firstTime);
                yield return null;
            }
    
            float secondTime = 0;
            while (secondTime < 0.75f)
            {
                secondTime += RealTime.deltaTime;
                DiceSprite.transform.Rotate(new Vector3(0, 0, 50));
                yield return null;
            }
    
            float thirdTime = 0.5f;
            while (thirdTime > 0)
            {
                thirdTime -= RealTime.deltaTime;
                DiceSprite.transform.Rotate(new Vector3(0, 0, 50) * thirdTime);
                yield return null;
            }
    
            DiceSprite.gameObject.CustomSetActive(false);
            DescLabel.gameObject.CustomSetActive(true);
            NumLabel.gameObject.CustomSetActive(true);
    
            UpdateUI();
            isRolling = false;
        }
    
        public override void OnFetchData(EB.Sparx.Response response, int reqInstanceID)
        {
            if (mDMono.gameObject == null || !mDMono.gameObject.activeInHierarchy)
            {
                //防止收到消息的时候，该物品已经被销毁或者隐藏了
                return;
            }
            if (response .sucessful)
            {
                if (response.hashtable != null) DataLookupsCache.Instance.CacheData(response.hashtable);
                isRolling = true;
                StartCoroutine(ScrollDide());
            }
            else if (response.fatal)
            {
                SparxHub.Instance.FatalError(response.localizedError);
            }
            else
            {
                EB.Debug.Log("worldboss/buyChallengeTimes——response =>{0}", response.hashtable.ToString());
            }
        }
    
        private void OnMaxRollUpdateFunc()
        {
            UpdateUI();
            int maxNum = 0;
            DataLookupsCache.Instance.SearchDataByID(string.Format("world_boss.rollDiceData.currentHighest_stage{0}.rollData.stage{0}.rollDice", node * 10), out maxNum);
            if (maxNum > 0)
            {
                string name = EB.Localizer.GetString("ID_codefont_in_LTWorldBossNodeRewardLogic_4331");
                DataLookupsCache.Instance.SearchDataByID(string.Format("world_boss.rollDiceData.currentHighest_stage{0}.playername", node * 10), out name);
                LTUIUtil.SetText(MaxNameLabel, name);

                LTUIUtil.SetText(MaxNumLabel, maxNum.ToString());
                EB.Debug.Log("LTWorldBossNodeRewardLogic + OnMaxRollUpdateFunc + name:{0}", name);
                EB.Debug.Log("LTWorldBossNodeRewardLogic + OnMaxRollUpdateFunc + maxNum:{0}", maxNum);
            }
            DataLookupsCache.Instance.CacheData(string.Format("world_boss.rollDiceData.currentHighest_stage{0}", node * 10), null);
        }
    }
}
