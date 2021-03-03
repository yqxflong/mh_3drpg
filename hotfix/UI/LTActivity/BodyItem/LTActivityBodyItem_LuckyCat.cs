using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 招财猫活动面板
    /// </summary>
    public class LTActivityBodyItem_LuckyCat : LTActivityBodyItem
    {
        public UILabel TimesLabel;
        public UILabel CostLabel;
        public UILabel GetLabel;

        public UIEventListener Listener;

        private TweenPosition Qui;

        private TweenScale Mao;

        public GameObject Fx;

        private UILabel[] CurValueLabels;
        private UILabel[] NextValueLabels;
        private Coroutine mCurrentDecelerateCoroutine;

        private int InitRollSpeed = 2000;
        private int curScrollIndex;
        private int isFinish;
        private Vector3 NextVec = new Vector3(0, 300);

        private int curIndex = -1;
        private int activity_id;
        private Hotfix_LT.Data.TimeLimitActivityStageTemplate Cur;
        private bool isRequire = false;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            UIEventTrigger trigger = t.GetComponent<UIEventTrigger>("FramePanel/Joystick/Ctrl/Qiu");
            trigger.onDragStart.Add(new EventDelegate(OnJoystickDragStart));

            Listener = t.GetComponent<UIEventListener>("BG/Icon");
            TimesLabel = t.GetComponent<UILabel>("CONTENT/TimesBG/Times");
            CostLabel = t.GetComponent<UILabel>("CONTENT/CostLabel");
            GetLabel = t.GetComponent<UILabel>("CONTENT/GetLabel");

            Qui = t.GetComponent<TweenPosition>("FramePanel/Joystick/Ctrl/Qiu");
            Mao = t.GetComponent<TweenScale>("BG/Mao");

            Fx = t.Find("BG/Fx").gameObject;

            CurValueLabels = new UILabel[5];
            CurValueLabels[0] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/1/cur");
            CurValueLabels[1] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/2/cur");
            CurValueLabels[2] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/3/cur");
            CurValueLabels[3] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/4/cur");
            CurValueLabels[4] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/5/cur");
            NextValueLabels = new UILabel[5];
            NextValueLabels[0] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/1/next");
            NextValueLabels[1] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/2/next");
            NextValueLabels[2] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/3/next");
            NextValueLabels[3] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/4/next");
            NextValueLabels[4] = t.GetComponent<UILabel>("ClipPanel/RandomGrid/5/next");
        }

        public override void OnDestroy()
        {
            if (mCurrentDecelerateCoroutine != null)
            {
                StopCoroutine(mCurrentDecelerateCoroutine);
                FusionAudio.PostEvent("UI/New/LaoHuJi", false);
                isRequire = false;
            }
        }

        public override void SetData(object data)
        {
            base.SetData(data);
            activity_id = EB.Dot.Integer("activity_id", data, 0);
            UpdateData();
        }

        private void UpdateData()
        {
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(activity_id);
            Hotfix_LT.Data.TimeLimitActivityTemplate activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(activity_id);
            string[] rewardStrs = activity.parameter1.Split(';');
            if (rewardStrs.Length < stages.Count)
            {
                EB.Debug.LogError("LuckyCat Activity Error!Events TimeLimitActivity Parameter is lower");
                return;
            }

            Hashtable activityData;
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + activity_id, out activityData);
            curIndex = -1;
            for (int i = 0; i < stages.Count; ++i)
            {
                int selfgot = EB.Dot.Integer(string.Format("stages.{0}", stages[i].id), activityData, 0);
                if (selfgot == 0)
                {
                    curIndex = i;
                    break;
                }
            }
            if (curIndex == -1)
            {
                //已领取完毕
                TimesLabel.text = "0";
                CostLabel.text = GetLabel.text = EB.Localizer.GetString("ID_LEGION_MEDAL_NOT");
                CostLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
            }
            else
            {
                Cur = stages[curIndex];
                TimesLabel.text = (VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.LuckyCat) - curIndex).ToString();
                CostLabel.color = (BalanceResourceUtil.GetUserDiamond() < Cur.stage) ? LT.Hotfix.Utility.ColorUtility.RedColor : LT.Hotfix.Utility.ColorUtility.GreenColor;
                CostLabel.text = Cur.stage.ToString();
                GetLabel.text = rewardStrs[curIndex].Replace(",", " - ");
            }
        }

        public void OnJoystickDragStart()
        {
            if (state.Equals("pending"))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_NOT_OPEN"));
                return;
            }

            if (curIndex == -1)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_LOCKYCAT_TIP5"));
                return;
            }
            //次数判断（是否购买特权）
            if (VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.LuckyCat) - curIndex <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_ACTIVITY_LOCKYCAT_TIP6"), delegate (int r)
                {
                    if (r == 0)
                    {
                        GlobalMenuManager.Instance.Open("LTVIPRewardHud");
                    }
                });
                return;
            }
            //材料判断
            if (BalanceResourceUtil.GetUserDiamond() < Cur.stage)
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }

            GetReward(Cur.id, Success, Fail);
        }

        private void GetReward(int stage, System.Action<int> Success, System.Action Fail = null)
        {
            if (isRequire) return;
            isRequire = true;

            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "insufficient num":
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_REALM_CHALLENGE_ERROR"));
                            LTMainHudManager.Instance.UpdateActivityLoginData(Fail);
                            return true;
                        }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/gotReward");
            request.AddData("activityId", activity_id);
            request.AddData("stageId", stage);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                ArrayList array = (ArrayList)data["reward"];
                int getNum = 0;
                for (int i = 0; i < array.Count; ++i)
                {
                    int num = EB.Dot.Integer("quantity", array[i], 0);
                    if (num > 0)
                    {
                        getNum = num;
                        string id = EB.Dot.String("data", array[i], null);
                        string type = EB.Dot.String("type", array[i], null);
                        if (!string.IsNullOrEmpty(id)) GameUtils.ShowAwardMsgOnlySys(new LTShowItemData(id, num, type));
                        break;
                    }
                }
                Success(getNum);
                title.UpdateRedPoint();
            });
        }

        private void Success(int num)
        {
            mCurrentDecelerateCoroutine = StartCoroutine(SetupConstantSpeedRollCoroutine(num));
        }

        private void Fail()
        {
            isRequire = false;
            EB.Debug.LogError("LTActivityBodyItem_LuckyCat Fail!");
        }

        /// <summary>开始摇老虎机 </summary>
        IEnumerator SetupConstantSpeedRollCoroutine(int hc)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 4f);
            curScrollIndex = isFinish = CurValueLabels.Length;
            float timer = 0;
            float detial = 1f;
            Fx.CustomSetActive(false);
            Qui.ResetToBeginning();
            Qui.PlayForward();
            FusionAudio.PostEvent("UI/New/LaoHuJi", true);
            ResetLabel();
            while (isFinish > 0)
            {
                timer += Time.deltaTime;
                for (int i = isFinish - 1; i >= 0; --i)
                {
                    if (CurValueLabels[i].transform.localPosition.y < -300)
                    {
                        CurValueLabels[i].transform.localPosition = NextVec;
                        CurValueLabels[i].text = Random.Range(0, 9).ToString();
                        if (curScrollIndex > i && timer > detial)
                        {
                            detial = 0.3f;
                            timer = 0;
                            int num = GetNum(hc, curScrollIndex);
                            CurValueLabels[i].text = num.ToString();
                            curScrollIndex--;
                        }
                    }
                    else
                    {
                        CurValueLabels[i].transform.localPosition -= new Vector3(0f, InitRollSpeed * Time.deltaTime);
                        if (curScrollIndex <= i && CurValueLabels[i].transform.localPosition.y < 0)
                        {
                            CurValueLabels[i].transform.localPosition = Vector3.zero;
                            NextValueLabels[i].transform.localPosition = NextVec;
                            isFinish--;
                            FusionAudio.PostEvent("UI/New/ZhonJiang", true);
                            continue;
                        }
                    }

                    if (NextValueLabels[i].transform.localPosition.y < -300)
                    {
                        NextValueLabels[i].transform.localPosition = NextVec;
                        NextValueLabels[i].text = Random.Range(0, 9).ToString();
                    }
                    if (NextValueLabels[i].transform.localPosition.y > CurValueLabels[i].transform.localPosition.y)
                    {
                        NextValueLabels[i].transform.localPosition = CurValueLabels[i].transform.localPosition + NextVec;
                    }
                    else
                    {
                        NextValueLabels[i].transform.localPosition = CurValueLabels[i].transform.localPosition - NextVec;
                    }

                }
                yield return null;
            }
            ResetLabel();
            UpdateData();
            yield return null;
            Mao.ResetToBeginning();
            Mao.PlayForward();
            FusionAudio.PostEvent("UI/New/LaoHuJi", false);
            FusionAudio.PostEvent("UI/New/ZhonDaJiang", true);
            Listener.onClick(Listener.gameObject);
            Fx.CustomSetActive(true);
            isRequire = false;
            mCurrentDecelerateCoroutine = null;
        }

        private void ResetLabel()
        {
            for (int i = 0; i < CurValueLabels.Length; ++i)
            {
                CurValueLabels[i].transform.localPosition = Vector3.zero;
                NextValueLabels[i].transform.localPosition = NextVec;
            }
        }

        private int GetNum(int num, int index)
        {
            int d = 0;
            switch (index)
            {
                case 5: { d = num % 10; }; break;
                case 4: { d = (num % 100) / 10; }; break;
                case 3: { d = (num % 1000) / 100; }; break;
                case 2: { d = (num % 10000) / 1000; }; break;
                case 1: { d = num / 10000; }; break;
            }
            return d;
        }

    }
}