using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionMessageItem : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            self = mDMono.transform.GetComponent<UIWidget>();
            titleDayLabel = mDMono.transform.Find("NYRLabel").GetComponent<UILabel>();
            contentLabel = mDMono.transform.Find("Label (1)").GetComponent<UILabel>();
        }

        public int year;
        public int month;
        public int day;
        public UIWidget self;
        public UILabel titleDayLabel;
        public UILabel contentLabel;

        public int GetHeight()
        {
            return Mathf.RoundToInt(-contentLabel.transform.localPosition.y + contentLabel.height + 20f);
        }

    }

}
