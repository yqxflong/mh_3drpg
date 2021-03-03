using System;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;
///
/// 过渡界面 暂时废弃
namespace Hotfix_LT.UI
{
    public class ArtifactUIController : UIControllerHotfix
    {
        public ArtifactDetailBehaviour DetailBehaviour;
        private int infoId;
        public override bool IsFullscreen()
        {
            return false;
        }

        public override bool ShowUIBlocker
        {
            get { return true; }
        }

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            DetailBehaviour = t.GetMonoILRComponent<ArtifactDetailBehaviour>("Bg");
            t.GetComponent<UIButton>("Bg/Top/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Bg/UpLevelBtn").clickEvent
                .Add(new EventDelegate(OnUpLevelBtnClick));
            Messenger.AddListener(EventName.ArtifactRefresh,ArtifactRefresh);
        }

        private void ArtifactRefresh()
        {
            DetailBehaviour.OnDisable();
            DetailBehaviour.Init(infoId);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.RemoveListener(EventName.ArtifactRefresh,ArtifactRefresh);
        }


        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            infoId = (int)param;
            DetailBehaviour.Init(infoId);
        }
        
        private void OnUpLevelBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTArtifactDetailUIHud",infoId);
        }
    }
}