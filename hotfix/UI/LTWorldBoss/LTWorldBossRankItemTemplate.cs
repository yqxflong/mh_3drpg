namespace Hotfix_LT.UI
{
    public class UIWorldBossRankScrollItemData
    {
        public int rank;
        public int damage;
        public string name;
    
        public UIWorldBossRankScrollItemData(int r, int d, string n)
        {
            rank = r;
            damage = d;
            name = n;
        }
    }
    
    public class LTWorldBossRankItemTemplate : DynamicMonoHotfix
    {
        public UILabel Damage;
        public UILabel Name;
        public UILabel Percent;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Damage = t.GetComponent<UILabel>("DamageLabel");
            Name = t.GetComponent<UILabel>("NameLabel");
            Percent = t.GetComponent<UILabel>("PercentLabel");
        }

        public void SetData(UIWorldBossRankScrollItemData data)
        {
            LTUIUtil.SetText(Percent, data.damage > 0 ? string.Format("({0}%)", (data.damage / (float)LTWorldBossDataManager.Instance.GetMaxBossHp() * 100).ToString("F1")) : string.Empty);
            LTUIUtil.SetText(Damage, data.damage > 0 ? GetDamageStr(data.damage) : string.Empty);
            LTUIUtil.SetText(Name, data.name);
        }
    
        private string GetDamageStr(int damageValue)
        {
            if (damageValue >= 100000000)
            {
                damageValue /= 100000000;
                return damageValue + EB.Localizer.GetString("ID_codefont_in_LTWorldBossRankItemTemplate_1082");
            }
    
            if (damageValue >= 10000)
            {
                damageValue /= 10000;
                return damageValue + EB.Localizer.GetString("ID_codefont_in_LTWorldBossRankItemTemplate_1215");
            }
            return damageValue.ToString();
        }
    }
}
