namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceLevelDataLookup : DataLookupHotfix
    {
        public UILabel LevelLabel;
        public bool isAlienMaze = false;//由界面上勾选去控制
    
        private int data = 0;

        public override void Awake()
        {
            base.Awake();

            LevelLabel = mDL.transform.GetComponentEx<UILabel>();

            if (mDL.BoolParamList != null)
            {
                var count = mDL.BoolParamList.Count;

                if (count > 0)
                {
                    isAlienMaze = mDL.BoolParamList[0];
                }
            }
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            if (dataID == null || value == null)
            {
                return;
            }
            if(data == int.Parse(value.ToString()))
            {
                return;
            }
            data = int.Parse(value.ToString());
            if (isAlienMaze)
            {
                var temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetAlienMazeById(data);
                if (temp != null)
                {
                    LevelLabel.text = temp.Name;
                }
            }
            else
            {
                var temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterById(data);
                if (temp != null)
                {
                    LevelLabel.text = string.Format(EB.Localizer.GetString("ID_CHALLENGE_LEVELDATA_LOOKUP"), temp.CurChapter, temp.CurLevel);
                }
            }
        }
    }
}
