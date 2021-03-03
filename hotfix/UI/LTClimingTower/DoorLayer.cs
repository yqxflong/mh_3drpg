using UnityEngine;

namespace Hotfix_LT.UI
{
    public class DoorLayer:DynamicMonoHotfix
    {
        public UILabel v_LayerLabel;
        public UITexture v_Door;
        public GameObject v_DoorEffet;
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            v_LayerLabel = t.GetComponent<UILabel>("Label");
            v_Door = t.GetComponent<UITexture>("Texture");
            v_DoorEffet = t.FindEx("Ps").gameObject;
            v_DoorEffet.CustomSetActive(false);
        }


        public void SetData(Data.ClimingTowerTemplate data,int type)
        {
            v_LayerLabel.gameObject.CustomSetActive(data != null);
            v_Door.gameObject.CustomSetActive(true);
            v_DoorEffet.gameObject.CustomSetActive(false);
            if (data != null)
            {
                int point = type == 0 ? data.normal_record : data.diff_record;
                LTUIUtil.SetText(v_LayerLabel, "+"+point);
                v_Door.gameObject.CustomSetActive(!data.v_CanChallenge);
                v_DoorEffet.gameObject.CustomSetActive(data.v_CanChallenge);
            }
        }


      
    }
}