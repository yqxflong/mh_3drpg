using EB.Sparx;
using Hotfix_LT.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.GameState
{
    public class LTGameStateLoadGame : GameStateUnit
    {
        public override eGameState mGameState { get { return eGameState.LoadGame; } }
        
        public override IEnumerator Start()
        {
            if(!UIStack .Instance .IsLoadingScreenUp) UIStack.Instance.ShowLoadingScreen(null, true, false);
            if (m_FlowControlObject == null)
            {
                LoadingLogic.AddCustomProgress(2);
                string path = "Assets/_GameAssets/Res/MISC/GameFlowControl/GameFlowControl";
                EB.Assets.LoadAsync(path, typeof(GameObject), o=>
                {
                    if(o != null)
                    {
                        var go = GameObject.Instantiate(o) as GameObject;
                        DownloadComplete(path, go);
                    }
                });

                while (m_FlowControlObject == null)
                {
                    yield return null;
                }
            }

            ChatController.instance.Init();

            SparxHub.Instance.PushManager.OnScheduleLocalNotification += ScheduleLocalNotification;
            SparxHub.Instance.ChatManager.OnConnected += RegisterChatChannels;
        }

        public override void End()
        {
            SparxHub.Instance.PushManager.OnScheduleLocalNotification -= ScheduleLocalNotification;
            SparxHub.Instance.ChatManager.OnConnected -= RegisterChatChannels;

            DestroyFlowControlObject();

            HudLoadManager.DestroyAllHud();
            SceneLoadManager.DestroyAllLevel();
            PlayerManagerForFilter.Dispose();
            NpcManager.Dispose();
            MainLandLogic.Dispose();
            CombatLogic.Dispose();
            InputBlockerManager.Instance.ForceUnlockAll();
            LoadingSpinner.Destroy();
            FusionAudio.StopMusic();
            FusionAudio.StopAmbience();
            ChatController.instance.Clean();
        }

        GameObject m_FlowControlObject = null;
        GameFlowControlManager m_FlowControlManager = null;
        
        private void DownloadComplete(string path, GameObject go)
        {
            if (go != null)
            {
                if (Mgr.State == eGameState.LoadGame)
                {
                    if (m_FlowControlObject != null)
                    {
                        EB.Debug.LogError("[GameStateLoadGame] m_FlowControlObject != null!");
                        GameObject.Destroy(m_FlowControlObject);
                        m_FlowControlObject = null;
                        m_FlowControlManager = null;
                    }
                    m_FlowControlObject = go;
                    m_FlowControlObject.transform.SetParent(GameStateManager.Instance.transform);
                    m_FlowControlManager = m_FlowControlObject.GetComponent<GameFlowControlManager>();
                }
                else
                {
                    GameObject.Destroy(go);
                    EB.Assets.UnloadAssetByName("GameFlowControl", true);
                }
            }
            else
            {
                EB.Debug.LogError("[GameStateLoadGame]DownloadComplete: Failed to download {0}.", path);
            }
        }

        private void DestroyFlowControlObject()
        {
            if (m_FlowControlObject != null)
            {
                GameObject.Destroy(m_FlowControlObject);
                m_FlowControlObject = null;
                m_FlowControlManager = null;

                EB.Assets.UnloadAssetByName("GameFlowControl", true);
            }
        }


        private void ScheduleLocalNotification()
        {
            var pm = SparxHub.Instance.GetManager<EB.Sparx.PushManager>();

            /*if (UserData.VigorRecoverFullNotification)
            {
                int nextFullGrowthTime = 0;
                DataLookupsCache.Instance.SearchIntByID("res.vigor.nf", out nextFullGrowthTime, null);
                if (nextFullGrowthTime > 0)
                {
                    System.DateTime dt = EB.Time.FromPosixTime(nextFullGrowthTime).ToLocalTime();
                    pm.ScheduleDailyLocalNotification(EB.Localizer.GetString("ID_SPARX_NOTIFICATION_TITLE"), EB.Localizer.GetString("ID_NOTIFACATION_VIGOR_RECOVER_FULL_TIME"), dt);
                }
            }
            if (UserData.StoreRefreshNotification)
            {
                pm.ScheduleLocalNotification(EB.Localizer.GetString("ID_NOTIFACATION_STORE_REFRESH_PUSH_MESSAGE"), 12, 0, true);
                pm.ScheduleLocalNotification(EB.Localizer.GetString("ID_NOTIFACATION_STORE_REFRESH_PUSH_MESSAGE"), 18, 0, true);
            }
            if (UserData.ReceiveVigorNotification)
            {
                pm.ScheduleLocalNotification(EB.Localizer.GetString("ID_NOTIFACATION_RECEIVE_VIGOR_PUSH_MESSAGE"), 12, 0, true);
                pm.ScheduleLocalNotification(EB.Localizer.GetString("ID_NOTIFACATION_RECEIVE_VIGOR_PUSH_MESSAGE"), 18, 0, true);
            }
            if (UserData.ActivityOpenNotification)
            {
                List<NormalActivityInstanceTemplate> activitys = EventTemplateManager.Instance.GetNormalActivityInstances();
                List<NormalActivityInstanceTemplate> activitysFitler = new List<NormalActivityInstanceTemplate>(activitys).FindAll(a => a.activity_id != 9004);
                RegisterActivityOpenNotification(pm, activitysFitler);
            }
            if (UserData.AllianceBattleOpenNotification)
            {
                List<NormalActivityInstanceTemplate> abs = EventTemplateManager.Instance.GetNormalActivityInstances().FindAll(a => a.activity_id == 9004);
                RegisterActivityOpenNotification(pm, abs);
            }*/ //�ɵ�������Ϣ

            //ToDo: ��ʱע�ͣ��������
            //for (int i = 0; i < UserData.PushMessageDataList.Count; i++)
            //{
            //    if (UserData.PushMessageDataDic.ContainsKey(UserData.PushMessageDataList[i].ActivityData.id) &&UserData.PushMessageDataDic[UserData.PushMessageDataList[i].ActivityData.id])
            //    {
            //        //RegisterActivityOpenNotification(pm, UserData.PushMessageDataList[i]);
            //    }
            //}
        }

        /*private void RegisterActivityOpenNotification(PushManager pm, List<NormalActivityInstanceTemplate> activitys)
        {
            foreach (NormalActivityInstanceTemplate activity in activitys)
            {
                SpecialActivityTemplate specialActivityTemplate = EventTemplateManager.Instance.GetSpecialActivity(activity.activity_id);
                if (string.IsNullOrEmpty(specialActivityTemplate.notification_title) || string.IsNullOrEmpty(specialActivityTemplate.notification_content))
                {
                    continue;
                }

                try
                {
                    string[] start = activity.s.Split(':');
                    string cronFormat = string.Format("{0} {1} * * {2}", start[1], start[0], specialActivityTemplate.open_time);
                    TimerScheduler timerScheduler = new TimerScheduler();
                    timerScheduler.m_TimerRegular = cronFormat;
                    timerScheduler.ParseTimerRegular();
                    if (!timerScheduler.isLegalTimer)
                    {
                        EB.Debug.LogError("ScheduleActivityLocalNotification: activity cronFormat is illegal");
                        continue;
                    }
                    System.DateTime date = EB.Time.LocalNow;
                    for (int i = 0; i < 7; ++i)
                    {
                        timerScheduler.GetNext(date, out date);
                        ScheduleActivityLocalNotification(pm, specialActivityTemplate.display_name, specialActivityTemplate.notification_title, specialActivityTemplate.notification_content, date);
                    }
                }
                catch
                {
                    EB.Debug.LogError("ScheduleActivityLocalNotification: exception activityid = {0}", activity.activity_id);
                }
            }
        }*/

        /*private void RegisterActivityOpenNotification(PushManager pm, LTDailyData activity)
        {
            try
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // ����ʱ��
                DateTime dt = startTime.AddSeconds(activity.OpenTimeValue);
                string[] WeekStr = activity.OpenTimeWeek.Split(',');
                for (int i = 0; i < WeekStr.Length; i++)
                {
                    string cronFormat = string.Format("{0} {1} * * {2}", dt.Minute, dt.Hour, WeekStr[i]);
                    TimerScheduler timerScheduler = new TimerScheduler();
                    timerScheduler.m_TimerRegular = cronFormat;
                    timerScheduler.ParseTimerRegular();
                    if (!timerScheduler.isLegalTimer)
                    {
                        EB.Debug.LogError("ScheduleActivityLocalNotification: activity cronFormat is illegal,index = {0}" , i );
                        return;
                    }
                    DateTime date = EB.Time.LocalNow;
                    for (int j = 0; j < 7; ++j)
                    {
                        timerScheduler.GetNext(date, out date);
                        ScheduleActivityLocalNotification(pm, activity.ActivityData.display_name, activity.ActivityData.notification_title, activity.ActivityData.notification_content, date);
                    }
                }
            }
            catch
            {
                EB.Debug.LogError("ScheduleActivityLocalNotification: exception activityid = {0}", activity.ActivityData.id);
            }
        }*/


        private void ScheduleActivityLocalNotification(PushManager pm, string displayName, string title, string content, System.DateTime time)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(content))
            {
                //EB.Debug.Log("ScheduleActivityLocalNotification: notification activity title={0},time={1}", displayName, time.ToString());
                pm.ScheduleOnceLocalNotification(title, content, time);
            }
            else
            {
                EB.Debug.LogError("ScheduleActivityLocalNotification: activity notification = empty displayName={0}", displayName);
            }
        }

        private void RegisterChatChannels()
        {
            ChatController.instance.RefreshChannel(true);
        }
    }
}
