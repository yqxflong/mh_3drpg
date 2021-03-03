using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI {
    public class LTLegionWarGMTool : DynamicMonoHotfix
    {
        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            GmView = t.FindEx("GMView").gameObject;
            GmLabel = t.GetComponent<UILabel>("Label");

            ToolList = new List<GameObject>();
            if (mDMono.ObjectParamList != null)
            {
                for (int i = 0; i < mDMono.ObjectParamList.Count; i++)
                {
                    ToolList.Add((GameObject)mDMono.ObjectParamList[i]);
                }               
            }
            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(Open));
        }


        public GameObject  GmView;
        public UILabel GmLabel;
        public List<GameObject> ToolList; 
        private bool isOpen=false;
        public void Open()
        {
            if (isOpen)
            {
                isOpen = !isOpen;
                GmLabel.text = "Close";
            }
            else
            {
                isOpen = !isOpen;
                GmLabel.text = "Open";
            }
            GmView.CustomSetActive(isOpen);
            SetToolList(isOpen);
        }
    
        private void SetToolList(bool isOpen)
        {
            for (int i = 0; i < ToolList.Count; i++)
            {
                ToolList[i].CustomSetActive(isOpen);
            }
        }
    }
}
