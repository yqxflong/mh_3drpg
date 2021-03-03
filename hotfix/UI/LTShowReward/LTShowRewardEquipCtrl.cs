using LT.Hotfix.Utility;
using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTShowRewardEquipCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            MainAttr = t.GetComponent<Transform>("Content/Infor/MainAttr");

            ExAttr = new Transform[4];
            ExAttr[0] = t.GetComponent<Transform>("Content/Infor/ExAttr (1)");
            ExAttr[1] = t.GetComponent<Transform>("Content/Infor/ExAttr (2)");
            ExAttr[2] = t.GetComponent<Transform>("Content/Infor/ExAttr (3)");
            ExAttr[3] = t.GetComponent<Transform>("Content/Infor/ExAttr (4)");

            Equipcell = t.GetMonoILRComponent<LTPartnerEquipCellController>("Content/Reward/AA");
            BlueBg = t.FindEx("Bg").gameObject;
            AttrPanel = t.GetComponent<Transform>("Content/Infor");

            t.GetComponent<UIEventTrigger>("Bg").onClick.Add(new EventDelegate(OnCancelButtonClick));

        }
    
        public DetailedEquipmentInfo EquipmentInfo;
        public Transform MainAttr;
        public Transform[] ExAttr = new Transform[4];
        public LTPartnerEquipCellController Equipcell;
        public GameObject BlueBg;
        public Transform AttrPanel;
    
        public override void SetMenuData(object param)
        {
            EquipmentInfo = param as DetailedEquipmentInfo;
            if (EquipmentInfo != null)
            {
                Equipcell.Fill(EquipmentInfo);
                InitEquipShow(EquipmentInfo);
            }
           
        }
        public override IEnumerator OnAddToStack()
        {
            //AttrPanel.localPosition = new Vector3(0, -160, 0);      
            //Equipcell.transform.localScale = new Vector3(0.87f, 0.87f, 1);
            AttrPanel.GetComponent<TweenAlpha>().ResetToBeginning();
            AttrPanel.GetComponent<TweenPosition>().ResetToBeginning();
            Equipcell.mDMono.GetComponent<TweenAlpha>().ResetToBeginning();
            Equipcell.mDMono.GetComponent<TweenScale>().ResetToBeginning();
            yield return base.OnAddToStack();
            AttrPanel.GetComponent<TweenAlpha>().PlayForward();
            AttrPanel.GetComponent<TweenPosition>().PlayForward();
            Equipcell.mDMono.GetComponent<TweenAlpha>().PlayForward();
            Equipcell.mDMono.GetComponent<TweenScale>().PlayForward();
            FusionAudio.PostEvent("UI/ShowReward");
            BlueBg.CustomSetActive(true);
        }
          
        public override IEnumerator OnRemoveFromStack()
        {
           
            base.OnRemoveFromStack();
            BlueBg.CustomSetActive(false);        
            if (controller.gameObject != null) DestroySelf();
            yield break;
        }
    
        private void InitEquipShow(DetailedEquipmentInfo info)
        {
              
            MainAttr.GetChild(0).GetComponent<UILabel>().text = "[fff348]" + EquipmentUtility.AttrTypeTrans(info.MainAttributes.Name);
            MainAttr.GetChild(1).GetComponent<UILabel>().text = EquipmentUtility.AttrTypeValue(info.MainAttributes);
            int ExIndex = info.ExAttributes.Count - 1;
            for (int i = 0; i < 4; i++)
            {
                if (i > ExIndex)
                {
                    ExAttr[i].gameObject.CustomSetActive(false);
                }
                else
                {
                    string ExNameStr = EquipmentUtility.AttrTypeTrans(info.ExAttributes[i].Name);
                    ExAttr[i].GetChild(0).GetComponent<UILabel>().text = ExNameStr;
                    ExAttr[i].GetChild(1).GetComponent<UILabel>().text = EquipmentUtility.AttrTypeValue(info.ExAttributes[i]);
                    ExAttr[i].gameObject.CustomSetActive(true);
                }
            }
        }
        public override void OnCancelButtonClick()
        {
            EquipmentInfo = null;
            base.OnCancelButtonClick();
        }
    }
}
