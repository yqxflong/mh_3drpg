using System.Text;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class NumAni : DynamicMonoHotfix, IHotfixUpdate
    {
        private UILabel numLabel;
        private bool isPlayAni;
        private int initialNum;
        private int goalNum;
        private int maxNum;
        private float playTime;
        private float curPlayTime;
        private int value;
        private StringBuilder str;

        public override void Start()
        {
            numLabel = mDMono.transform.GetComponent<UILabel>();
            str = new StringBuilder();
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public override void OnDisable()
        {
            ErasureMonoUpdater();
            isPlayAni = false;
        }

        public void Update()
        {
            if (isPlayAni)
            {
                if (numLabel == null)
                {
                    isPlayAni = false;
                    return;
                }

                curPlayTime += Time.deltaTime;
                value = initialNum + (int)((goalNum - initialNum) * (curPlayTime / playTime));
                str.Remove(0, str.Length);
                str.Append(value);
                str.Append("/");
                str.Append(maxNum);
                //numLabel.text = str.ToString();
                LTUIUtil.SetText(numLabel, str.ToString());
                if (curPlayTime >= playTime)
                {
                    isPlayAni = false;
                    //numLabel.text = goalNum + "/" + maxNum;
                    LTUIUtil.SetText(numLabel, goalNum + "/" + maxNum);
                    IsOver();
                }
            }
        }

        //目前只适用于xxx/xxx这种形式的数字滚动
        public void SetAniData(int initialNum, int goalNum, int maxNum, float playTime)
        {
            this.initialNum = initialNum;
            this.goalNum = goalNum;
            this.maxNum = maxNum;
            this.playTime = playTime;
            curPlayTime = 0;
            isPlayAni = true;
        }

        private void IsOver()
        {
            Hotfix_LT.Messenger.Raise("OnPartnerRollAniSucc");
        }

        public void StopAni()
        {
            isPlayAni = false;
        }
    }
}
