using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTWelfareSignInItem : DynamicMonoHotfix
    {
        private LTShowItem Item;
        private ConsecutiveClickCoolTrigger Btn;
        private GameObject TipObj;
        private GameObject FxObj;
        private GameObject hasReceiveObj;
        private SigninAward StageDta;
        private UISprite BgSprite;
        private UILabel DayLabel;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Item = t.GetMonoILRComponent<LTShowItem>("LTShowItem");
            Btn = t.GetComponent<ConsecutiveClickCoolTrigger>("ReceiveBtn");
            Btn.clickEvent.Add(new EventDelegate(OnReceiveBtnClick));
            TipObj = t.FindEx("TipObj").gameObject;
            hasReceiveObj = t.FindEx("SelectUI").gameObject;
            FxObj = t.FindEx("FXObj").gameObject;
            BgSprite = t.GetComponent<UISprite>("BG");
            DayLabel = t.GetComponent<UILabel>("DayLabel");
        }

        public void InitData(SigninAward sd, int index)
        {
            Item.LTItemData = new LTShowItemData(sd.Id, sd.Count, sd.Type, false);
            StageDta = sd;

            BgSprite.spriteName = (index == 3 || index == 10 || index == 17 || index == 24) ? "Welfare_Qiandao_Di2" : "Welfare_Qiandao_Di1";
            DayLabel.text = string.Format(EB.Localizer.GetString("ID_DAY"), index);
        }

        public void UpdateReceiveState(eReceiveState state, bool isResignin = false)
        {
            switch (state)
            {
                case eReceiveState.cannot:
                    FxObj.CustomSetActive(false);
                    hasReceiveObj.CustomSetActive(false);
                    Btn.gameObject.CustomSetActive(false);
                    break;
                case eReceiveState.can:
                    if (isResignin)
                    {
                        TipObj.CustomSetActive(true);
                        FxObj.CustomSetActive(false);
                    }
                    else
                    {
                        TipObj.CustomSetActive(LTWelfareDataManager.Instance.SignInData.IsSigned);
                        FxObj.CustomSetActive(true);
                        FxObj.GetComponent<ParticleSystemUIComponent>().Play();
                        FxObj.GetComponent<EffectClip>().Init();
                    }
                    hasReceiveObj.CustomSetActive(false);
                    Btn.gameObject.CustomSetActive(true);
                    break;
                case eReceiveState.have:
                    TipObj.CustomSetActive(false);
                    FxObj.CustomSetActive(false);
                    hasReceiveObj.CustomSetActive(true);
                    Btn.gameObject.CustomSetActive(false);
                    BgSprite.spriteName = "Welfare_Qiandao_Di3";
                    break;
            }
        }

        public void OnReceiveBtnClick()
        {
            int index = this.mDMono.transform.GetSiblingIndex();
            int num = LTWelfareDataManager.Instance.SignInData.Num;
            if (!LTWelfareDataManager.Instance.SignInData.IsSigned && index == num)
            {
                LTWelfareDataManager.Instance.SignInData.IsSigned = true;
                LTWelfareDataManager.Instance.SignInData.Num++;

                LTWelfareModel.Instance.Signin(delegate (bool successful)
                {
                    if (successful)
                        OnSigninBack();
                });
            }
            else if (TipObj.activeSelf && index == num)
            {
                LTWelfareDataManager.Instance.SignInData.Num++;
                LTWelfareModel.Instance.AdditionalSignin(delegate (bool successful)
                {
                    if (successful)
                    {
                        LTWelfareDataManager.Instance.SignInData.HaveResigninNum++;
                        OnSigninBack();
                    }
                });
            }
            else
            {
                Item.PopToolTip();
            }
        }

        public void OnSigninBack()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            FusionAudio.PostEvent("UI/General/ButtonClick");
            UpdateReceiveState(eReceiveState.have);

            if (StageDta.Type.Equals(LTShowItemType.TYPE_HERO))
            {
                LTShowItemData itemData = new LTShowItemData(StageDta.Id, StageDta.Count, StageDta.Type);
                GlobalMenuManager.Instance.Open("LTShowGetPartnerUI", itemData);
                ILRTimerManager.instance.AddTimer(100,1,delegate { CallBack();});
            }
            else
            {
                List<LTShowItemData> Awards = new List<LTShowItemData>() { new LTShowItemData(StageDta.Id, StageDta.Count, StageDta.Type) };
                for (int i = 0; i < Awards.Count; i++)
                {
                    if (Awards[i].id == "hc")
                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, Awards[i].count, "签到获得");
                    if (Awards[i].id == "gold")
                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, Awards[i].count, "签到获得");
                }
                var ht = Johny.HashtablePool.Claim();
                ht.Add("reward", Awards);
                ht.Add("callback",new System .Action(CallBack));
                GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
            }

        }

        private void CallBack()
        {
            if (LTWelfareEvent.WelfareSignInUpdata != null)
            {
                LTWelfareEvent.WelfareSignInUpdata();
            }
        }
    }
}
