using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTActivityBodyItem_Draw : LTActivityBodyItem
    {
        private class DrawInfoBtn
        {
            public UIButton btn;

            public UISprite icon;
            public UISprite roleIcon;
            public UISprite gradeIcon;

            public string heroInfoId;


            private ParticleSystemUIComponent charFx;
            private EffectClip efClip;

            public void FillData(string Id)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    LTShowItemData itemData = new LTShowItemData(Id, 0, LTShowItemType.TYPE_HERO, false);
                    heroInfoId = itemData.id;
                    int cid = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(itemData.id).character_id;
                    Hotfix_LT.Data.HeroInfoTemplate data = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(cid);
                    icon.spriteName = data.icon;
                    roleIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[data.char_type];
                    gradeIcon.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)data.role_grade];

                    HotfixCreateFX.ShowCharTypeFX(charFx, efClip, roleIcon.transform, (PartnerGrade)data.role_grade, data.char_type);
                    btn.gameObject.CustomSetActive(true);
                }
                else
                {
                    btn.gameObject.CustomSetActive(false);
                }
            }

            public DrawInfoBtn(Transform Root)
            {
                btn = Root.GetComponent<UIButton>();
                btn.onClick.Add(new EventDelegate(OnFinalAwardClick));
                icon = Root.Find("Icon").GetComponent<UISprite>();
                roleIcon = Root.Find("Role").GetComponent<UISprite>();
                gradeIcon = Root.Find("Grade").GetComponent<UISprite>();
            }

            private void OnFinalAwardClick()
            {
                Vector2 screenPos = UICamera.lastEventPosition;
                var ht = Johny.HashtablePool.Claim();
                ht.Add("id", heroInfoId);
                ht.Add("screenPos", screenPos);
                GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
            }
        }

        private List<DrawInfoBtn> ItemList;
        public override void Awake()
        {
            base.Awake();
            ItemList = new List<DrawInfoBtn>();
            DrawInfoBtn Info1 = new DrawInfoBtn(mDMono.transform.Find("HeroInfoList/InfoItem"));
            DrawInfoBtn Info2 = new DrawInfoBtn(mDMono.transform.Find("HeroInfoList/InfoItem (1)"));
            DrawInfoBtn Info3 = new DrawInfoBtn(mDMono.transform.Find("HeroInfoList/InfoItem (2)"));
            ItemList.Add(Info1);
            ItemList.Add(Info2);
            ItemList.Add(Info3);
        }

        public override void SetData(object data)
        {
            base.SetData(data);
            SetInfoData();
        }

        private void SetInfoData()
        {
            string[] cmd = NavString;
            if (cmd.Length < 3)
            {
                for (int i = 0; i < ItemList.Count; ++i)
                {
                    ItemList[i].FillData(string.Empty);
                }
                return;
            }
            string[] temp = cmd[2].Split(',');
            for (int i = 0; i < ItemList.Count; ++i)
            {
                if (i < temp.Length)
                {
                    ItemList[i].FillData(temp[i]);
                }
                else
                {
                    ItemList[i].FillData(string.Empty);
                }
            }
        }
    }
}