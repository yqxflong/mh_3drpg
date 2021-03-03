using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerAttrDesc : DynamicMonoHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            IconSp = t.GetComponent<UISprite>("Icon");
            DescLab = t.GetComponent<UILabel>("Label");
            box = t.GetComponent<BoxCollider>();
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
        public UISprite IconSp;
        public UILabel DescLab;
    
        private int index;
    
        private int widthOffset = 596;
        private int heightOffset = -168;
    
        private List<string> mIconNameList = new List<string>()
        {
            "Partner_Properyt_Icon_Gongji",
            "Partner_Properyt_Icon_Fangyu",
            "Partner_Properyt_Icon_Shengming",
            "Partner_Properyt_Icon_Baoji",
            "Partner_Properyt_Icon_Baoshang",
            "Partner_Properyt_Icon_Sudu",
            "Partner_Properyt_Icon_Mingzhong",
            "Partner_Properyt_Icon_Dikang",
        };
    
        private List<string> mDescList = new List<string>()
        {
            "ID_PARTNER_ATTR_DESC1",
            "ID_PARTNER_ATTR_DESC2",
            "ID_PARTNER_ATTR_DESC3",
            "ID_PARTNER_ATTR_DESC4",
            "ID_PARTNER_ATTR_DESC5",
            "ID_PARTNER_ATTR_DESC6",
            "ID_PARTNER_ATTR_DESC7",
            "ID_PARTNER_ATTR_DESC8",
        };
    
        private BoxCollider box;
    
        public void Update()
        {
            if ((Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
            {
                if (!box.bounds.Contains(UICamera.lastWorldPosition))
                {
                    CloseUI();
                }
            }
        }
    
        public void ShowUI(int index)
        {
            if (index >= mIconNameList.Count)
            {
                EB.Debug.LogError("LTpartnerAttrDesc ShowUI is Error, index is More than Count, index : {0}", index);
                return;
            }
    
            this.index = index;
    
            IconSp.spriteName = mIconNameList[index];
            DescLab.text = EB.Localizer.GetString(mDescList[index]);
    
            OpenUI();
            InitPos();
        }
    
        private void OpenUI()
        {
            mDMono.transform.GetComponent<UIPanel>().sortingOrder =  mDMono.transform.parent.GetComponent<UIPanel>().sortingOrder + 1;
            mDMono.transform.GetComponent<UIPanel>().depth =  mDMono.transform.parent.GetComponent<UIPanel>().depth + 1;
            mDMono.gameObject.CustomSetActive(true);
        }
    
        private void CloseUI()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    
        private void InitPos()
        {
            //int x = index % 2 == 0 ? 0 : widthOffset;
            int y = (index / 2) * heightOffset;
    
            mDMono.transform.localPosition = new Vector3(0, y, 0);
        }
    }
}
