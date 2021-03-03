using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
using Hotfix_LT.Data;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerStarUpDesc : DynamicMonoHotfix, IHotfixUpdate
    {

        private class StarProfitDes
        {
            private int star;
            private UILabel title;
            private UILabel profitDes;
            private UILabel isGottip;
            private string colorStr, desStr;
            private SkillTemplate buff;
            public StarProfitDes(int star,Transform t)
            {
                this.star = star;
                title = t.GetComponent<UILabel>(string.Format("BG/{0}/Label", star));
                profitDes = t.GetComponent<UILabel>(string.Format("BG/{0}/Label (2)", star));
                isGottip = t.GetComponent<UILabel>(string.Format("BG/{0}/Label (1)", star));
                isGottip.text = EB.Localizer.GetString(string.Format("ID_PARTNER_STARUP_TIP4"));
            }

            public void SetInfo(int curstar,int buffid = 0)
            {
                if (curstar >= star)
                {
                    colorStr = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                    isGottip.gameObject.CustomSetActive(false);
                }
                else
                {                    
                    isGottip.gameObject.CustomSetActive(true);
                    colorStr = LT.Hotfix.Utility.ColorUtility.GrayColorHexadecimal;
                }
                Hotfix_LT.Data.StarUpInfoTemplate curTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarUpInfo(this.star - 1);
                Hotfix_LT.Data.StarUpInfoTemplate tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarUpInfo(this.star);//获得下一个星级数据
                if (curTpl != null && tpl != null)
                {
                    desStr = string.Format(EB.Localizer.GetString("ID_PARTNER_STARUP_TIP5"),colorStr, ((tpl.IncATK - curTpl.IncATK) * 100).ToString("f0"), ((tpl.IncMaxHp - curTpl.IncMaxHp) * 100).ToString("f0"), ((tpl.IncDEF - curTpl.IncDEF) * 100).ToString("f0"));
                }
                switch (this.star)
                {
                    case 2:
                    case 3:
                        profitDes.text = profitDes.transform.GetChild(0).GetComponent<UILabel>().text = desStr;
                        break;
                    case 4:
                        profitDes.text = profitDes.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}\n{1}",desStr,EB.Localizer.GetString("ID_PARTNER_STARUP_TIP6"));
                        break;
                    case 5:
                    case 6:
                        buff = SkillTemplateManager.Instance.GetTemplate(buffid);
                        if(buff != null)
                        profitDes.text = profitDes.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}\n【{1}】 {2}", desStr, buff.Name, buff.Description);
                        break;
                    default:
                        break;
                }
                title.text = string.Format(EB.Localizer.GetString("ID_PARTNER_STARUP_TIP3"), this.star);

            }
        }

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            box = t.GetComponent<BoxCollider>();
            star2 = new StarProfitDes(2, t);
            star3 = new StarProfitDes(3, t);
            star4 = new StarProfitDes(4, t);
            star5 = new StarProfitDes(5, t);
            star6 = new StarProfitDes(6, t);
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
        private BoxCollider box;
        private StarProfitDes star2, star3, star4, star5, star6;

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
    
        private void OpenUI()
        {
            mDMono.transform.GetComponent<UIPanel>().sortingOrder = mDMono.transform.parent.GetComponent<UIPanel>().sortingOrder + 1;
            mDMono.transform.GetComponent<UIPanel>().depth = mDMono.transform.parent.GetComponent<UIPanel>().depth + 100;
            mDMono.gameObject.CustomSetActive(true);
        }
    
        private void CloseUI()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    
        private void InitInfo(int star, int starskill5, int starskill6)
        {
            star2.SetInfo(star);
            star3.SetInfo(star);
            star4.SetInfo(star);
            star5.SetInfo(star,starskill5);
            star6.SetInfo(star,starskill6);
        }
    
        public void ShowUI(int star,int starskill5,int starskill6)
        {
            InitInfo(star,starskill5,starskill6);
            OpenUI();
        }
    
    }
}
