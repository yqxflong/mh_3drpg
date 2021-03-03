using LT.Hotfix.Utility;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTEquipmentSortItem : DynamicCellController<EquipmentSortType>
    {
        private UILabel _labName;
        private EquipmentSortType _type;
        private GameObject _chooseObj;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            _labName = t.GetComponent<UILabel>("Label");
            _chooseObj = t.FindEx("SelectBg/Sprite").gameObject;

            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList[0] != null)
            {
                var controller =(mDMono.ObjectParamList[0] as GameObject).GetUIControllerILRComponent<LTPartnerEquipMainController>();
                t.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => controller.SetSortType(_type)));
            }
        }

        public override void Clean()
        {
            _chooseObj.SetActive(false);
            _labName.text = "";
            mDMono.gameObject.SetActive(false);
        }
    
        public override void Fill(EquipmentSortType type)
        {
            _type = type;
            mDMono.gameObject.SetActive(true);
            _chooseObj.SetActive(_type == LTPartnerEquipDataManager.Instance.CurSortType);
            _labName.text = EquipmentUtility.AttrTypeTrans(_type.ToString(), false);
        }
    }
}
