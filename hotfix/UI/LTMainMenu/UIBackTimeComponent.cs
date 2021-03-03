using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class UIBackTimeComponent : DynamicMonoHotfix
    {
        public UILabel m_TimeLabel;
        public System.DateTime m_EndDateTime;
        public string m_TestDateInput;
        public List<EventDelegate> m_Handlers;
        private bool ticking = false;
        private long m_EndTime;

        public long EndTime
        {
            set
            {
                ticking = false;
                StopCoroutine(Tick());
                m_EndTime = value;
                m_EndDateTime = EB.Time.FromPosixTime((int)m_EndTime);
                ticking = true;

                if (mDMono.gameObject.activeSelf)
                {
                    StartCoroutine(Tick());
                }
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_TimeLabel = t.GetComponentEx<UILabel>();
        }

        public IEnumerator Tick()
        {
            while (ticking)
            {
                if (!UpdateDate())
                {
                    EventDelegate.Execute(m_Handlers);
                    yield break;
                }

                yield return new WaitForSeconds(1f);
            }

            yield break;
        }

        public bool UpdateDate()
        {
            System.TimeSpan ts = m_EndDateTime - EB.Time.FromPosixTime(EB.Time.Now);
            int days = ts.Days;
            int hours = ts.Hours + days * 24;
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;

            if (m_EndTime <= EB.Time.Now && hours <= 0 && minutes <= 0 && seconds <= 0)
            {
                m_TimeLabel.text = EB.Localizer.GetString("ID_FINISHED");
                return false;
            }
            else
            {
                m_TimeLabel.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
                return true;
            }
        }

        public void OnTest()
        {
            m_EndTime = long.Parse(m_TestDateInput);
        }
    }
}
