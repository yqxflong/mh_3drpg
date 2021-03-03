using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTBountyTaskSelectItem : DynamicCellController<LTPartnerData>
{
    public DynamicUISprite MainIcon;
    public UISprite QualityIcon;
    public LTPartnerStarController StarController;
    public UISprite BG;
    public UISprite GradeIcon;
    public UISprite FrameBG;

    private ParticleSystemUIComponent charFx;
    private EffectClip efClip;

    private int tplId = 0;
    
    public override void Awake()
    {
        base.Awake();

        var t = mDMono.transform;
        MainIcon = t.GetComponent<DynamicUISprite>("Icon");
        QualityIcon = t.GetComponent<UISprite>("Quality");
        StarController = t.GetMonoILRComponent<LTPartnerStarController>("Star");
        BG = t.GetComponent<UISprite>("ContentBg");
        GradeIcon = t.GetComponent<UISprite>();
        FrameBG = t.GetComponent<UISprite>("Bg");

        t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnSelectBtnClick));
    }
    
    public override void Clean()
    {
        tplId = 0;
       mDMono.gameObject.CustomSetActive(false);
    }

    public override void Fill(LTPartnerData itemData)
    {
        if (itemData == null)
        {
            Clean();
            return;
        }
        tplId = itemData.HeroStat.id;
        Hotfix_LT.Data.HeroInfoTemplate m_data = itemData.HeroInfo;
        MainIcon.spriteName = m_data.icon;
        if (m_data.char_type == Hotfix_LT.Data.eRoleAttr.None)
        {
            QualityIcon.gameObject.CustomSetActive(false);
        }
        else
        {
                QualityIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[m_data.char_type]; 
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, QualityIcon.transform, (PartnerGrade)m_data.role_grade, (Hotfix_LT.Data.eRoleAttr)m_data.char_type);
        }

        GradeIcon.spriteName = UIItemLvlDataLookup.LvlToStr((m_data.role_grade + 1).ToString());
        FrameBG.color = UIItemLvlDataLookup.GetItemFrameBGColor((m_data.role_grade + 1).ToString());
        StarController.SetSrarList(itemData.Star,itemData.IsAwaken);

        BG.color = (LTBountyTaskSelectController.Target != tplId) ? new Color(213f/255f,223f/ 255f,232f/ 255f) : new Color(125f/ 255f,202f/ 255f,1f);

        mDMono.gameObject.CustomSetActive(true);

    }
    
    public void OnSelectBtnClick()
    {
        if (LTBountyTaskSelectController.Target == tplId) return;
        LTHotfixManager.GetManager<TaskManager>().RequestSetBountyHero(tplId, OnTaskResult);
    }

    public void OnTaskResult(EB.Sparx.Response result)
    {
        if (result.sucessful) {
            DataLookupsCache.Instance.CacheData(result .hashtable);
            Messenger.Raise(Hotfix_LT.EventName.BountyTask_Select);
        }
        else
        {
            EB.Debug.LogError("SparxHub.Instance.GetManager<EB.Sparx.TaskManager>().RequestSetBountyHero => error = {0}" ,result.error);
        }
    }
}

}