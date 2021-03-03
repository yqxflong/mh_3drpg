using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LabelAniData
    {
        public int CurNum;//当前数
        public int NextNum;//到达数
        public int MaxNum;//最大数
        public float Timer;//用时
        public LabelAniData(int curNum, int nextNum, int maxNum, float timer)
        {
            CurNum = curNum;
            NextNum = nextNum;
            MaxNum = maxNum;
            Timer = timer;
        }
    }

    public class LTEquipLabelAni : DynamicMonoHotfix, IHotfixUpdate
    {
        public Queue<LabelAniData> TimesList = new Queue<LabelAniData>();
        private bool isShow = false;
        private UILabel m_label;
        private LabelAniData curData;
        private float curTimer;

        public override void Start()
        {
            m_label = mDMono.transform.GetComponent<UILabel>();
            curTimer = 0;
        }

        public override void OnEnable()
        {
			RegisterMonoUpdater();
			isShow = false;
            TimesList = new Queue<LabelAniData>();
        }
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public void ResetList()
        {
            TimesList.Clear();
            isShow = false;
        }

        public void Update()
        {
            if (TimesList.Count > 0 && !isShow)
            {
                curTimer = 0;
                curData = TimesList.Dequeue();
                isShow = true;
            }

            if (isShow)
            {
                if (curData.NextNum == -1)//到达最大值
                {
                    m_label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipLabelAni_1251"));
                    isShow = false;
                }
                else
                {
                    curTimer += Time.deltaTime;
                    int value = curData.CurNum + (int)((curData.NextNum - curData.CurNum) * Mathf.Clamp((curTimer / curData.Timer), 0f, 1f));
                    m_label.text = string.Format("[42fe79]{0}/{1}", value, curData.MaxNum);

                    if (curTimer >= curData.Timer)
                    {
                        isShow = false;
                    }
                }
            }
        }
    }
}
