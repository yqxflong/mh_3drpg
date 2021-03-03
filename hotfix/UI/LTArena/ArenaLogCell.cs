using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ArenaLogCell : DynamicCellController<ArenaBattleLog>
    {
        public GameObject winGo;
        public GameObject failGo;
        public UISprite opponentIconSprite;
        public UILabel opponentLevelLabel;
        public UISprite opponentLevelBGSprite;
        public UILabel opponentNameLabel;
        public UILabel rankUpLabel;
        public UILabel rankDownLabel;
        public GameObject rankNoChangeGO;
        public GameObject challengerGO;
        public GameObject defenderGO;
        public UISprite opponentFrameSprite;

        public ArenaBattleLog Log { get; private set; }

        public override void Awake()
        {
            base.Awake();

            var list = mDMono.ObjectParamList;
            
            if (list != null)
            {
                var count = list.Count;

                if (count > 0 && list[0] != null)
                {
                    winGo = (GameObject)list[0];
                }
                if (count > 1 && list[1] != null)
                {
                    failGo = (GameObject)list[1];
                }
                if (count > 2 && list[2] != null)
                {
                    opponentIconSprite = ((GameObject)list[2]).GetComponentEx<UISprite>();
                }
                if (count > 3 && list[3] != null)
                {
                    opponentLevelLabel = ((GameObject)list[3]).GetComponentEx<UILabel>();
                }
                if (count > 4 && list[4] != null)
                {
                    opponentLevelBGSprite = ((GameObject)list[4]).GetComponentEx<UISprite>();
                }
                if (count > 5 && list[5] != null)
                {
                    opponentNameLabel = ((GameObject)list[5]).GetComponentEx<UILabel>();
                }
                if (count > 6 && list[6] != null)
                {
                    rankUpLabel = ((GameObject)list[6]).GetComponentEx<UILabel>();
                }
                if (count > 7 && list[7] != null)
                {
                    rankDownLabel = ((GameObject)list[7]).GetComponentEx<UILabel>();
                }
                if (count > 8 && list[8] != null)
                {
                    rankNoChangeGO = (GameObject)list[8];
                }
                if (count > 9 && list[9] != null)
                {
                    challengerGO = (GameObject)list[9];
                }
                if (count > 10 && list[10] != null)
                {
                    defenderGO = (GameObject)list[10];
                }
                if (count > 11 &&  list[11] != null)
                {
                    opponentFrameSprite= ((GameObject)list[11]).GetComponentEx<UISprite>();
                }
            }
        }

        public override void Clean()
        {
            opponentIconSprite.spriteName = opponentFrameSprite.spriteName = string.Empty;
            opponentLevelLabel.text = string.Empty;
            rankUpLabel.text = rankUpLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Empty;
            rankDownLabel.text = rankDownLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Empty;
            rankDownLabel.transform.parent.gameObject.SetActive(false);
            rankUpLabel.transform.parent.gameObject.SetActive(false);
            rankNoChangeGO.SetActive(false);
            challengerGO.SetActive(false);
            defenderGO.SetActive(false);

            Log = null;
        }

        public override void Fill(ArenaBattleLog itemData)
        {
            Log = itemData;

            winGo.SetActive(Log.isWon);
            failGo.SetActive(!Log.isWon);
            //var heroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(Log.opponentTpl);
            var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(Log.opponentCharID, Log.opponentSkin);
            if (string.IsNullOrEmpty(Log.icon))
            {
                opponentIconSprite.spriteName = heroInfo.icon;
            }
            else
            {
                opponentIconSprite.spriteName = Log.icon;
            }
            opponentFrameSprite.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(Log.opponentFrame).iconId;
            opponentLevelLabel.text = Log.opponentLevel.ToString();
            //opponentLevelBGSprite.spriteName = UIBuddyShowItem.AttrToLevelBG(heroInfo.char_type);
            LTUIUtil.SetText(opponentNameLabel, Log.opponentName);
            //battleTimeLabel.text = EB.Localizer.FormatPassedDuration(Log.occurTime);
            if (Log.rankChange > 0)
            {
                rankUpLabel.text = rankUpLabel.transform.GetChild(0).GetComponent<UILabel>().text = "+" + Log.rankChange.ToString();
                rankUpLabel.transform.parent.gameObject.SetActive(true);
                rankDownLabel.transform.parent.gameObject.SetActive(false);
                rankNoChangeGO.SetActive(false);
            }
            else if (Log.rankChange < 0)
            {
                rankDownLabel.text = rankDownLabel.transform.GetChild(0).GetComponent<UILabel>().text = (Log.rankChange).ToString();
                rankDownLabel.transform.parent.gameObject.SetActive(true);
                rankUpLabel.transform.parent.gameObject.SetActive(false);
                rankNoChangeGO.SetActive(false);
            }
            else
            {
                rankDownLabel.transform.parent.gameObject.SetActive(false);
                rankUpLabel.transform.parent.gameObject.SetActive(false);
                rankNoChangeGO.SetActive(true);
            }
            if (Log.type == ArenaBattleLog.eChallengeType.Invalid)
            {
                 challengerGO.SetActive(false);
                 defenderGO.SetActive(false);
            }
            else
            {
                challengerGO.SetActive(Log.isChallenge);
                defenderGO.SetActive(!Log.isChallenge);
            }
        }
    }

}