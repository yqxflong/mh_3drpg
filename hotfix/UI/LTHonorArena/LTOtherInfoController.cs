using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public enum TitleType
    {
        Info_Title,
        Enemy_Title
    }
    
    public class LTOtherInfoController:UIControllerHotfix
    {
        private static string Info_Title = EB.Localizer.GetString("ID_uifont_in_LTCheckPlayerFormationInfoUI_Title_0");
        private static string Enemy_Title = EB.Localizer.GetString("ID_ENEMY_TEAMINFO");
        
        
        public UILabel titleLabel;
        public UILabel levelLabel;
        public UILabel nameLabel;
        public UILabel combatPowerLabel;
        public UILabel scoreLabel;
        public List<Transform> PosList;
        public List<FormationPartnerItem> PartnerItemList;

        private int ranking;
        //缓存一些数据
        public Dictionary<string,List<OtherPlayerPartnerData>> dict=new Dictionary<string, List<OtherPlayerPartnerData>>();

        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen()
        {
            return false;
        }
        
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            titleLabel = t.GetComponent<UILabel>("BG/Top/Title");
            levelLabel = t.GetComponent<UILabel>("Content/Base/LevelBG/Level");
            nameLabel = t.GetComponent<UILabel>("Content/Base/Name");
            combatPowerLabel = t.GetComponent<UILabel>("Content/Base/CombatPower/Container/Base");
            scoreLabel = t.GetComponent<UILabel>("Content/Base/ScoreLabel");
            
            t.GetComponent<UIButton>("BG/Top/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            
            PosList = controller.FetchComponentList<Transform>(GetArray("Content/Formation/Container/UIGrid/Item","Content/Formation/Container/UIGrid/Item (1)","Content/Formation/Container/UIGrid/Item (2)","Content/Formation/Container/UIGrid/Item (3)","Content/Formation/Container/UIGrid/Item (4)","Content/Formation/Container/UIGrid/Item (5)",
                "Content/Formation/Container (1)/UIGrid/Item","Content/Formation/Container (1)/UIGrid/Item (1)","Content/Formation/Container (1)/UIGrid/Item (2)","Content/Formation/Container (1)/UIGrid/Item (3)","Content/Formation/Container (1)/UIGrid/Item (4)","Content/Formation/Container (1)/UIGrid/Item (5)",
                "Content/Formation/Container (2)/UIGrid/Item","Content/Formation/Container (2)/UIGrid/Item (1)","Content/Formation/Container (2)/UIGrid/Item (2)","Content/Formation/Container (2)/UIGrid/Item (3)","Content/Formation/Container (2)/UIGrid/Item (4)","Content/Formation/Container (2)/UIGrid/Item (5)"));
            
            PartnerItemList = new List<FormationPartnerItem>();
            foreach (var judgeItem in PosList)
            {
                PartnerItemList.Add(judgeItem.GetMonoILRComponent<FormationPartnerItem>());
            }
        }

        public static void Open(TitleType titleType,string _id,string name,int level,int worldId,int score,int m_CombatPower,int ranking=-1)
        {
            Hashtable table = new Hashtable();
            table.Add("titleType",titleType);
            table.Add("_id",_id);
            table.Add("name",name);
            table.Add("level",level);
            //区服
            table.Add("worldId",worldId);
            table.Add("score",score);
            table.Add("ranking",ranking);
            table.Add("m_CombatPower",m_CombatPower);
            GlobalMenuManager.Instance.Open("LTOtherInfoUI",table);
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            
            Hashtable table = (Hashtable)param;
            TitleType type = (TitleType)table["titleType"];
            string _id = (string)table["_id"];
            string name = (string)table["name"];
            int level = (int)table["level"];
            int score = (int)table["score"];
            int worldId = (int)table["worldId"];
            ranking = (int)table["ranking"];
            int m_CombatPower = (int)table["m_CombatPower"];
            if (type==TitleType.Info_Title)
            {
                LTUIUtil.SetText(titleLabel,Info_Title);
                scoreLabel.transform.parent.gameObject.SetActive(true);
                LTUIUtil.SetText(scoreLabel,score.ToString());
                levelLabel.text = level.ToString();
                LTUIUtil.SetText(nameLabel, string.Format("{0}【{1}{2}】", name, worldId, EB.Localizer.GetString("ID_LOGIN_SERVER_NAME")));
                LTUIUtil.SetText(combatPowerLabel,m_CombatPower.ToString());
            }else if (type==TitleType.Enemy_Title)
            {
                LTUIUtil.SetText(titleLabel,Enemy_Title);
                scoreLabel.transform.parent.gameObject.SetActive(false);
            }

            List<OtherPlayerPartnerData> partnerDataList = null;
            if (dict.TryGetValue(_id,out partnerDataList))
            {
                Fill(partnerDataList);
            }
            else
            {
                ShowOtherPlayerData(_id);
            }
        }

        private void ShowOtherPlayerData(string m_Uid)
        {
            Hashtable data=null;
            //表明不是排行榜 此时data不用传
            if (ranking!=-1)
            {
                data = Johny.HashtablePool.Claim();
                data.Add(SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM3_TYPE0, ranking);
            }
            LTFormationDataManager.Instance.GetOtherHonorPlayerData(m_Uid,data,
                delegate(List<OtherPlayerPartnerData> partnerDataList)
                {
                    dict.Add(m_Uid,partnerDataList);
                    Fill(partnerDataList);
                    
                  
                });
        }

        private void Fill(List<OtherPlayerPartnerData> partnerDataList)
        {
            float allpower = 0f;
            for (int i = 0; i < partnerDataList.Count; i++)
            {
                PartnerItemList[i].Fill(partnerDataList[i],true);
                // if (partnerDataList[i]!=null && partnerDataList[i].Attributes != null)
                // {
                //     allpower += partnerDataList[i].GetOtherPower();
                // }
                // LTUIUtil.SetText(combatPowerLabel,((int)allpower).ToString());
            }
        }
    }
}