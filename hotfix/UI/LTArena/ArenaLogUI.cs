namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class ArenaLogUI : UIControllerHotfix
    {
        public UILabel title;
        public ArenaLogScroll logScroll;
        UIButton HotfixBtn0;
        public override void Awake()
        {
            base.Awake();
            title = controller.transform.GetComponent<UILabel>("Frame/BG/Top/Title");
            logScroll = controller.transform.GetMonoILRComponent<ArenaLogScroll>("Content/LogsScrollView/Placehodler/Grid");
            HotfixBtn0 =  controller.transform.Find("Frame/BG/Top/CloseBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnCancelButtonClick));
        }

        public override bool ShowUIBlocker
        {
            get { return true; }
        }

        public string LogId;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            LogId = (string)param;
            switch (LogId)
            {
                case HonorArenaManager.ArenaBattleLogsDataId:
                    LTUIUtil.SetText(title, EB.Localizer.GetString("ID_ARENA_BUTTON_LOG"));
                    break;
                case ArenaManager.ArenaBattleLogsDataId:
                    LTUIUtil.SetText(title, EB.Localizer.GetString("ID_uifont_in_ArenaLogUI_Title_0"));
                    break;
                default:
                    break;
            }
        }

        public override IEnumerator OnAddToStack()
        {
            ArenaManager.Instance.IsRankChanged = false;
            return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }

        public override void OnFocus()
        {
            base.OnFocus();

            GameDataSparxManager.Instance.RegisterListener(LogId, OnArenaLogsListener);
        }

        public override void OnBlur()
        {
            GameDataSparxManager.Instance.UnRegisterListener(LogId, OnArenaLogsListener);

            base.OnBlur();
        }

        private void OnArenaLogsListener(string path, INodeData data)
        {
            ArenaBattleLogs logs = data as ArenaBattleLogs;

            logScroll.SetDataItems(logs.logs);
        }
    }

}