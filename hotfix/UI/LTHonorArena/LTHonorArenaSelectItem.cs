using System.Collections;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTHonorArenaSelectData
    {
            public long m_Uid;
            public string m_Icon;
            public string m_Frame;
            public string m_Name;
            public int m_CombatPower;
            public int m_Score;
    }
    
    public class  LTHonorArenaSelectItem:DynamicMonoHotfix
    {
        public UISprite m_UIBg;
        public UISprite m_UIIcon;
        public UISprite m_FrameIcon;
        public UILabel m_UIName;
        public UILabel m_UICombatPower;
        public UILabel m_UIScore;
        public UILabel m_UICost;
        public UILabel m_UIFree;
        public UIButton m_ChangeBtn;
        public ParticleSystemUIComponent FX;

        private LTHonorArenaSelectData data;
        private int Index = 0;
        public string DataId
        {
            get; private set;
        }

        public HonorArenaChallenger challenger { get; private set; }
        
        public override void Awake()
        {
            base.Awake();
            FX = mDMono.transform.GetComponent<ParticleSystemUIComponent>("BG/FX");
            m_UIBg = mDMono.transform.Find("BG").GetComponent<UISprite>(); 
            m_UIIcon = mDMono.transform.Find("Pic").GetComponent<UISprite>();
            m_FrameIcon = mDMono.transform.Find("Pic/Frame").GetComponent<UISprite>();
            m_UIName = mDMono.transform.Find("Name").GetComponent<UILabel>(); 
            m_UICombatPower = mDMono.transform.Find("CombatPower/Base").GetComponent<UILabel>(); 
            m_UIScore = mDMono.transform.Find("Score").GetComponent<UILabel>();
            m_ChangeBtn = mDMono.transform.Find("ChallengeBtn").GetComponent<UIButton>();
            m_UICost = mDMono.transform.Find("ChallengeBtn/Numbel").GetComponent<UILabel>();
            m_UIFree = mDMono.transform.Find("ChallengeBtn/Free").GetComponent<UILabel>();
            m_UIIcon.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
                {
                    LTOtherInfoController.Open(TitleType.Info_Title,challenger._id,challenger.name,challenger.level,challenger.worldId,challenger.point,challenger.br);
                }));
        }


        public void Register(string dataId, int index)
        {
            if (DataId == dataId)
            {
                return;
            }
            else if (!string.IsNullOrEmpty(DataId))
            {
                Clean();
            }
            
            DataId = dataId;
            Index = index;
            GameDataSparxManager.Instance.RegisterListener(DataId, OnChallengerListener);
        }
        
        public void Clean()
        {
            if (!string.IsNullOrEmpty(DataId))
            {
                GameDataSparxManager.Instance.UnRegisterListener(DataId, OnChallengerListener);
                DataId = string.Empty;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameDataSparxManager.Instance.UnRegisterListener(DataId, OnChallengerListener);
        }

        private void OnChallengerListener(string dataId, INodeData data)
        {
            challenger = data as HonorArenaChallenger;
            LTHonorArenaSelectData selectData = new LTHonorArenaSelectData();

            if (challenger.worldId==0)
            {
                selectData.m_Name = string.Format("{0}【{1}{2}】", challenger.name, challenger.worldId,EB.Localizer .GetString("ID_LOGIN_SERVER_NAME"));
            }
            else
            {
                selectData.m_Name = string.Format("{0}【{1}{2}】", challenger.name, challenger.worldId, EB.Localizer.GetString("ID_LOGIN_SERVER_NAME"));
            }
            selectData.m_Frame = challenger.frame;
            selectData.m_Icon = challenger.icon;
            selectData.m_Score = challenger.changePoint;
            selectData.m_CombatPower = challenger.br;
            this.data = selectData;
            Fill();
        }

        public void Fill()
        {
            mDMono.gameObject.SetActive(true);
            FX.Play();
            m_UIIcon.spriteName = data.m_Icon;
            m_FrameIcon.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(data.m_Frame).iconId;
            LTUIUtil.SetText(m_UIName, data.m_Name);
            LTUIUtil.SetText(m_UICombatPower, data.m_CombatPower.ToString());
            LTUIUtil.SetText(m_UIScore, "+"+data.m_Score);

            int freetimes = HonorArenaManager.Instance.GetHonorArenaFreeTimes();  
            int usetimes = HonorArenaManager.Instance.Info.usedTimes;  
            if (freetimes-usetimes>0)
            {
                m_UICost.color=Color.clear;
                m_UIFree.color=Color.white;
            }
            else
            {
                m_UICost.color = Color.white;
                m_UIFree.color=Color.clear;
                LTUIUtil.SetText(m_UICost, "1");
            }
        }
        
    }
}