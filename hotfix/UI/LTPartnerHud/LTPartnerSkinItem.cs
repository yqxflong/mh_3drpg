using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTLTPartnerSkinItemData
    {
        public int HeroId;
        public int Index;
        public string Name;
        public string Icon;

        public LTLTPartnerSkinItemData(int HeroId, int Index, string Name, string Icon)
        {
            this.HeroId = HeroId;
            this.Index = Index;
            this.Name = Name;
            this.Icon = Icon;
        }
    }

    public class LTPartnerSkinItem : DynamicCellController<LTLTPartnerSkinItemData>
    {
        public UILabel SkinNameLabel;
        public UILabel GetTipLabel;
        public DynamicUISprite SkinSprite;
        public List<UIWidget> DepthList;

        private LTLTPartnerSkinItemData data;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            SkinNameLabel = t.GetComponent<UILabel>("NameLabel");
            GetTipLabel = t.GetComponent<UILabel>("TipLabel");
            SkinSprite = t.GetComponent<DynamicUISprite>("Icon");
            DepthList = new List<UIWidget>();
            DepthList.Add(t.GetComponentEx<UISprite>());
            DepthList.Add(t.GetComponent<DynamicUISprite>("Icon"));
            DepthList.Add(t.GetComponent<UISprite>("NameLabel/BG"));
            DepthList.Add(t.GetComponent<UISprite>("TipLabel/BG"));
            DepthList.Add(t.GetComponent<UILabel>("NameLabel/Label (1)"));
            DepthList.Add(t.GetComponent<UILabel>("TipLabel/Label (1)"));
            DepthList.Add(t.GetComponent<UILabel>("NameLabel"));
            DepthList.Add(t.GetComponent<UILabel>("TipLabel"));
        }

        public override void Clean()
        {
            data = null;
            mDMono.gameObject.CustomSetActive(false);
        }

        public override void Fill(LTLTPartnerSkinItemData itemData)
        {
            data = itemData;
            SkinNameLabel.text = data.Name;
            SkinSprite.spriteName = data.Icon;
            SetGetTipLabel(data.HeroId, data.Index);
            mDMono.gameObject.CustomSetActive(true);
        }

        public int GetSkinIndex()
        {
            return data.Index;
        }

        public LTPartnerData GetSkinPartnerData()
        {
            var partner = LTPartnerDataManager.Instance.GetPartnerByHeroId(data.HeroId);
            return partner;
        }

        public void SetDepth(int depth)
        {
            for (int i = 0; i < DepthList.Count; ++i)
            {
                DepthList[i].depth = depth + i;
            }
        }

        private void SetGetTipLabel(int heroId, int index)
        {
            var partner = LTPartnerDataManager.Instance.GetPartnerByHeroId(heroId);

            if (index == 0)
            {
                GetTipLabel.text = EB.Localizer.GetString("ID_uifont_in_LTStoreBuy_Num_1");//已拥有
            }
            else if (index == 1)
            {
                GetTipLabel.text = (partner.IsAwaken > 0) ? EB.Localizer.GetString("ID_uifont_in_LTStoreBuy_Num_1") : EB.Localizer.GetString("ID_PARTNER_SKIN_AWAKEN_TIP");//觉醒后获得
            }
            else
            {
                //待处理
            }
        }
    }
}
