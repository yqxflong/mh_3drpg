using System.Collections;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTHonorArenaSelectRivalController:UIControllerHotfix
    {
        public UILabel StoneLabel;
        public UILabel CombatPowerLabel;
        public UISprite selectUISprite;
        public LTHonorArenaSelectItem[] honorArenaItem;
        private bool selectFast;

        private int challengeCost = 0;
        private int mFillIndex;
        
        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen()
        {
            return false;
        }
        
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            CombatPowerLabel= t.GetComponent<UILabel>("CombatPower/Base");
            StoneLabel = t.GetComponent<UILabel>("HonorStore/Bg/Label");
            selectUISprite = t.GetComponent<UISprite>("QuickBtn/Check");
            
            honorArenaItem = new LTHonorArenaSelectItem[3];
            honorArenaItem[0] = t.GetMonoILRComponent<LTHonorArenaSelectItem>("Grid/0");
            honorArenaItem[1] = t.GetMonoILRComponent<LTHonorArenaSelectItem>("Grid/0 (1)");
            honorArenaItem[2] = t.GetMonoILRComponent<LTHonorArenaSelectItem>("Grid/0 (2)");
            honorArenaItem[0].Register(string.Format("honorarena.challengeList[{0}]", 0), 0);
            honorArenaItem[1].Register(string.Format("honorarena.challengeList[{0}]", 1), 1);
            honorArenaItem[2].Register(string.Format("honorarena.challengeList[{0}]", 2), 2);
            honorArenaItem[0].m_ChangeBtn.onClick.Add(new EventDelegate(() => { ChallengeBtnClick(0); }));
            honorArenaItem[1].m_ChangeBtn.onClick.Add(new EventDelegate(() => { ChallengeBtnClick(1); }));
            honorArenaItem[2].m_ChangeBtn.onClick.Add(new EventDelegate(() => { ChallengeBtnClick(2); }));
            t.GetComponent<UIButton>("HonorStore/Bg").onClick.Add(new EventDelegate(OnBuyTimesButtonClick));
            t.GetComponent<UIButton>("BG/Top/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<ContinueClickCDTrigger>("RefreshChallengersBtn").m_CallBackPress.Add(new EventDelegate(OnRefreshBtnClick));
            t.GetComponent<UIButton>("QuickBtn").onClick.Add(new EventDelegate(OnQuickBtnClick));
        }
        
        
        

        private void OnQuickBtnClick()
        {
            selectFast = !selectFast;
            selectUISprite.gameObject.SetActive(selectFast);
        }
        
        public void OnBuyTimesButtonClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            int buyCost = HonorArenaManager.Instance.Info.quantity;
            //购买花费
            if (BalanceResourceUtil.GetUserDiamond() <buyCost)
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }

            var ht = Johny.HashtablePool.Claim();
            ht.Add("0", buyCost);
            MessageTemplateManager.ShowMessage(902123, ht, delegate (int result)
            {
                if (result == 0)
                {
                    HonorArenaManager.Instance.BuyChallengeTimes(delegate (bool successful)
                    {
                        StoneLabel.text = string.Format("{0}", HonorArenaManager.Instance.Info.ticket);
                    });
                }
            });
            Johny.HashtablePool.Release(ht);
        }
        
        public override IEnumerator OnAddToStack()
        {
            onCombatTeamPowerUpdate(HonorArenaManager.Instance.AllCombatPower);
            StoneLabel.text = HonorArenaManager.Instance.Info.ticket.ToString();
            Messenger.AddListener<int>(EventName.HonorCombatTeamPowerUpdate, onCombatTeamPowerUpdate);
            yield return base.OnAddToStack();
        }
        
        private void onCombatTeamPowerUpdate(int power)
        {
            // LTUIUtil.SetText(CombatbatPowerLabel,);          
            LTUIUtil.SetText(CombatPowerLabel, power.ToString());
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            Messenger.RemoveListener<int>(EventName.HonorCombatTeamPowerUpdate, onCombatTeamPowerUpdate);
            return base.OnRemoveFromStack();
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            selectUISprite.gameObject.SetActive(selectFast);
            OnRefreshBtnClick();
        }

        private void OnRefreshBtnClick()
        {
            HonorArenaManager.Instance.RefreshChallengers(() =>
            {
                mFillIndex = 0;
                for (int i = 0; i < honorArenaItem.Length; i++)
                {
                    honorArenaItem[i].mDMono.gameObject.SetActive(false);
                }
                ILRTimerManager.instance.AddTimer(120, honorArenaItem.Length, OnTimerUpToFill);
            });
        }

        public void ChallengeBtnClick(int index)
        {
            int freetimes = HonorArenaManager.Instance.GetHonorArenaFreeTimes();  
            int usetimes = HonorArenaManager.Instance.Info.usedTimes;  
            if (HonorArenaManager.Instance.Info.ticket<=0 && freetimes-usetimes<=0)
            {
                OnBuyTimesButtonClick();
                return;
            }
            
            HonorArenaManager.Instance.CurrentChallenger = honorArenaItem[index].challenger;
            LTHonorArenaBattleController.Open(false,index,selectFast);
        }
        
        private void OnTimerUpToFill(int seq) {
            if (mFillIndex<honorArenaItem.Length)
            {
                honorArenaItem[mFillIndex].Fill();
                mFillIndex++;
            }
        }
        
    }
}