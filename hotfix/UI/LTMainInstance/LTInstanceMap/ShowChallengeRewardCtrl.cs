using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class ShowChallengeRewardCtrl : DynamicMonoHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            DesLabel = t.GetComponent<UILabel>("Label");
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public UILabel DesLabel;
        private BoxCollider box;
    	public override void Start()
        {
            box = mDMono.transform.GetComponent<BoxCollider>();
        }
    	public void Update () {
            if ((Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
            {
                if (!box.bounds.Contains(UICamera.lastWorldPosition))
                {
                    CloseUI();
                }
            }
        }
    
        public void ShowUI(string rate,int quality)
        {
            DesLabel.text = string.Format(EB.Localizer.GetString("ID_CHALLENGE_DROPTIP"), rate, EB.Localizer.GetString("ID_uifont_in_LTPartnerEquipmentHud_Label_2" + (quality - 1)));
    
            mDMono.gameObject.CustomSetActive(true);
        }
    
        private  void CloseUI()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    }
}
