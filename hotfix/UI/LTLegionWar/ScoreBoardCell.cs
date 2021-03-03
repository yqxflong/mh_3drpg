using UnityEngine;

namespace Hotfix_LT.UI
{

    //BGÍ¨¹ýobjecparmtlist¸³Öµ
    public class ScoreBoardCell : DynamicCellController<ScoreBoardData>
    {

        public UILabel Rank, Name, Score;
        public UISprite BG;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            Rank = t.GetComponent<UILabel>("Rank");
            Name = t.GetComponent<UILabel>("Name");
            Score = t.GetComponent<UILabel>("Score");
            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                BG = ((GameObject)mDMono.ObjectParamList[0]).GetComponent<UISprite>();
            }
        }
        public override void Fill(ScoreBoardData itemData)
        {
            Rank.text = Rank.transform.GetChild(0).GetComponent<UILabel>().text = itemData.Rank == -1 ? ">100" : itemData.Rank.ToString();
            Name.text = Name.transform.GetChild(0).GetComponent<UILabel>().text = itemData.Name;
            Score.text = Score.transform.GetChild(0).GetComponent<UILabel>().text = itemData.Score.ToString();
            if (BG != null) BG.color = (itemData.Rank % 2 > 0) ? new Color(0.49f, 0.82f, 1) : new Color(0, 0.32f, 0.65f);
        }

        public override void Clean()
        {
            Rank.text = Rank.transform.GetChild(0).GetComponent<UILabel>().text = null;
            Name.text = Name.transform.GetChild(0).GetComponent<UILabel>().text = null;
            Score.text = Score.transform.GetChild(0).GetComponent<UILabel>().text = null;
        }

    }
}
