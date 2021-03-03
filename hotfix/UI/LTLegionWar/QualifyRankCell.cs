using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI {
    public class QualifyRankCell : DynamicCellController<LegionRankData>  {
        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            Filled = t.FindEx("FillObj").gameObject;
            Empty = t.FindEx("EmptyObj").gameObject;
            Rank = t.GetComponent<UILabel>("FillObj/Rank/RankLabel");
            Name = t.GetComponent<UILabel>("FillObj/NameLabel");
            Score = t.GetComponent<UILabel>("FillObj/ScoreLabel");
            RankSprite = t.GetComponent<UISprite>("FillObj/Rank");
            Icon = t.GetComponent<UISprite>("FillObj/Badge/LegionIcon");
            IconBG = t.GetComponent<UISprite>("FillObj/Badge/IconBG");
        }


        public GameObject Filled,Empty;
        public UILabel Rank,Name,Score;
        public UISprite RankSprite, Icon,IconBG;
    
        public override void Fill(LegionRankData item) {
            if (item==null||item.Name == null) {
                Filled.SetActive(false);
                Empty.SetActive(true);
            }
            else {
                Filled.SetActive(true);
                Empty.SetActive(false);
                Name.text = Name.transform.GetChild(0).GetComponent<UILabel>().text =item.Name;
                Score.text = Score.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("[fff348]{0}", item.Score);
                SetIcon(item.Icon);
                //SetRank(item.Rank);
            }
    
        }
    
        public void SetIcon(string iconStr)
        {
            int iconID = 0;
            int.TryParse(iconStr, out iconID);
            int legionIconIndex = iconID % 100;
            int legionBgIconIndex = iconID / 100;
            if (LegionModel .GetInstance().dicLegionSpriteName.ContainsKey(legionIconIndex))
            {
                Icon.spriteName  = LegionModel.GetInstance().dicLegionSpriteName[legionIconIndex];
            }
            if (LegionModel.GetInstance().dicLegionBGSpriteName.ContainsKey(legionBgIconIndex))
            {
                IconBG.spriteName = LegionModel.GetInstance().dicLegionBGSpriteName[legionBgIconIndex];
            }
        }
    
        public void SetRank(int rank) {
            Rank.text = Rank.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}.", rank);
            switch (rank) {
                case 1:RankSprite.spriteName = "Ty_Arena_Icon_Jin";break;
                case 2: RankSprite.spriteName = "Ty_Arena_Icon_Yin"; break;
                case 3:RankSprite.spriteName = "Ty_Arena_Icon_Tong";break;
                case 4: RankSprite.spriteName = "Ty_Arena_Icon_Lv"; break;
                default:RankSprite.gameObject.SetActive(false);break;
            }
            RankSprite.gameObject.SetActive(true);
        }
    
        public override void Clean() {
            
        }
    }
}
