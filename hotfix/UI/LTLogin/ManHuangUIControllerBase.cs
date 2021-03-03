using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ManHuangUIControllerBase : DynamicMonoHotfix
    {
        public GameObject m_Container;

        virtual public void Show(bool isShow)
        {
            m_Container.CustomSetActive(isShow);
            if (isShow)
            {
                UITweener[] tweeners =mDMono.transform.GetComponentsInChildren<UITweener>();
                for (int j = 0; j < tweeners.Length; ++j)
                {
                    tweeners[j].tweenFactor = 0;
                    tweeners[j].PlayForward();
                }
            }
        }

        virtual public void OnCloseBtnClick()
        {
            Show(false);
        }
    }
}
