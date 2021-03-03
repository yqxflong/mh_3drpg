using GM.DataCache;
using ILRuntime.Runtime;
using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTBossChallengeActivityItem : DynamicMonoHotfix
    {
        private GameObject _lock;
        private GameObject _unlock;
        private UILabel _lockName;
        private UILabel _unlockName;
        private UILabel _unlockCount;
        private Data.BossChallengeActivityConfigTemplate _data;
        private LTActivityTitleItem _title;
        private int _endTime;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            _lock = t.gameObject.FindEx("Lock");
            _unlock = t.gameObject.FindEx("Unlock");
            _lockName = t.GetComponent<UILabel>("Lock/Name");
            _unlockName = t.GetComponent<UILabel>("Unlock/Name");
            _unlockCount = t.GetComponent<UILabel>("Unlock/Count");

            var btn = t.GetComponent<UIButton>("Unlock");

            if (btn != null)
            {
                btn.onClick.Add(new EventDelegate(OnClicked));
            }
        }

        public void SetData(bool isLock, string lastLevelName, Data.BossChallengeActivityConfigTemplate data, LTActivityTitleItem title, int endTime)
        {
            _lock.SetActive(isLock);
            _unlock.SetActive(!isLock);
            _lockName.text = EB.Localizer.GetString("ID_codefont_in_ChapterItemTemplate_1398") + lastLevelName;
            _unlockName.text = data.name;
            _unlockCount.text = data.vigor.ToString();
            _data = data;
            _title = title;
            _endTime = endTime;
        }

        private void OnClicked()
        {
            if (_data == null)
            {
                EB.Debug.LogError("LTBossChallengeActivityItem.OnClicked: _data is null");
                return;
            }

            if (EB.Time.Now >= _endTime)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTLegionWarQualify_End_4"));
                return;
            }

            if (BalanceResourceUtil.GetUserVigor() < _data.vigor)
            {
                BalanceResourceUtil.TurnToVigorGotView();
                return;
            }

			BossChallengeData challengeData = new BossChallengeData();
			challengeData.UnitValue = _data.bonus_add;
			challengeData.BuffName = _data.bonus_name;
			challengeData.DescribeFormat = _data.bonus_describe;

			BattleReadyHudController.Open(eBattleType.BossChallenge, () => {
                LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
                {
                    if (response.error != null)
                    {
                        string strObjects = (string)response.error;
                        string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                        switch (strObject[0])
                        {
                            case "event is not running":
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTLegionWarQualify_End_4"));//活动已结束
                                    return true;
                                }
                            case "nsf":
                                {

                                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamController_8317"), delegate (int result)
                                    {
                                        if (result == 0)
                                        {
                                            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                            GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
                                        }
                                    });
                                    return true;
                                }
                        }
                    }
                    return false;
                };

                EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/startChallenge");
                request.AddData("type", eBattleType.BossChallenge.ToInt32());
                request.AddData("id", _data.id);
                LTHotfixApi.GetInstance().BlockService(request, (Hashtable data) =>
                {
                    if (!GlobalMenuManager.Instance.IsContain("LTActivityHud"))
                    {
                        GlobalMenuManager.Instance.PushCache("LTActivityHud", _title);
                    }
                });
            }, null, (Data.eRoleAttr)_data.partner_attr, challengeData);
        }
    }
}
