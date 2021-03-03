using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI {
    public class FlowEnemyHud : DynamicMonoHotfix {
        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            PointLabel = t.GetComponent<UILabel>("Icon/Label");
            FlagIcon = t.GetComponent<UISprite>("Icon/FlagBG");
            PlayerIcon = t.GetComponent<UISprite>("Icon");
            FrameIcon= t.GetComponent<UISprite>("Icon/Frame");
            FightOutObj = t.FindEx("Icon/FightOut").gameObject;
            FxObj = t.FindEx("Icon/Fx").gameObject;

            t.GetComponent<ConsecutiveClickCoolTrigger>("Icon").clickEvent.Add(new EventDelegate(StartChallenge));
        }

        public UILabel PointLabel;
        public UISprite FlagIcon,PlayerIcon, FrameIcon;
        public GameObject FightOutObj,FxObj;
        private FlowEnemyData m_Data;
    
        public void Fill(FlowEnemyData data)
        {
            m_Data = data;
            if(m_Data != null){
                PlayerIcon.spriteName = m_Data.IconName;
                FrameIcon.spriteName = m_Data.FrameName;
                SetFlag(m_Data.Score);
                FightOutObj.CustomSetActive(m_Data.IsFightOut);

                FxObj.CustomSetActive(!m_Data.IsFightOut);

                mDMono.gameObject.CustomSetActive(true);
                PointLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_FlowEnemyHud_2368"),data.Score);
            }
            else
            {
                mDMono.gameObject.CustomSetActive(false);
            }
        }

        public void ShowFx()
        {
	        if (FxObj && Exist(m_Data))
	        {
		        UIControllerHotfix.Current.CloseCallbacks.Add(() =>
		        {
			        FxObj.GetComponent<ParticleSystemUIComponent>().Stop();
		        });

				if (!m_Data.IsFightOut)
				{
					FxObj.GetComponent<ParticleSystemUIComponent>().Play();
					EffectClip clip = FxObj.GetComponent<EffectClip>();
					if (clip != null && !clip.HasInitialized)
						clip.Init();
				}
			}
        }
    
        private void SetFlag(int Point)
        {
            if (Point <= 60) FlagIcon.spriteName = "Legion_Flag_1";
            else if (Point <= 90) FlagIcon.spriteName = "Legion_Flag_3";
            else FlagIcon.spriteName = "Legion_Flag_4";
        }
    
        public void Clear()
        {
            m_Data = null;
            Fill(m_Data);
        }
    
        public void StartChallenge() {
            if (m_Data.IsFightOut)
            {
                return;
            }
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            System.Action startCombatCallback = delegate () {
                LTLegionWarManager.Instance.Api.ErrorRedultFunc = (EB.Sparx.Response response) =>
                {
                    if (response.str != null)
                    {
                        switch ((string)response.str)
                        {
                            case "target is protected":
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FlowEnemyHud_3732"));
                                    return true;
                                };
                            case "NotReady":
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FlowEnemyHud_3986"));
                                    return true;
                                };
                        }
                    }
                    if (response.error != null)
                    {
                        switch ((string)response.error)
                        {
                            case "Error: service not found":
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FlowEnemyHud_4472"));
                                    return true;
                                };
                        }
                    }
                    return false;
                };
                LTLegionWarManager.Instance.StartChallenge(m_Data.Uid);
            };
            BattleReadyHudController.Open(eBattleType.AllieancePreBattle, startCombatCallback, m_Data.TUid);
        }
    }
}
