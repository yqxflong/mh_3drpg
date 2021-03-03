using System;
using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class UIBuddyShowItem : DynamicMonoHotfix
    {
        public DynamicUISprite Icon;
        public UISprite QuantityLevelSprite;
        public UISprite QuantityBGSprite;
        public UISprite AttrSprite;
        public UILabel breakLabel;
        public UIGrid StarGrid;
        public GameObject HireObj;
        public CamapignVictoryHpDataSet HpSet;
        public CamapignVictoryExpDataSet ExpSet;

        public GameObject[] StarList;

        private Color GreyColor = new Color(1, 0, 1);
        private Color WhiteColor = new Color(1, 1, 1);
        private ParticleSystemUIComponent charFx,upgradeFx;
        private EffectClip efClip, upgradeefclip;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    Icon = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<DynamicUISprite>();
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    QuantityLevelSprite = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UISprite>();
                }
                if (count > 2 && mDMono.ObjectParamList[2] != null)
                {
                    QuantityBGSprite = ((GameObject)mDMono.ObjectParamList[2]).GetComponentEx<UISprite>();
                }
                if (count > 3 && mDMono.ObjectParamList[3] != null)
                {
                    AttrSprite = ((GameObject)mDMono.ObjectParamList[3]).GetComponentEx<UISprite>();
                }
                if (count > 4 && mDMono.ObjectParamList[4] != null)
                {
                    breakLabel = ((GameObject)mDMono.ObjectParamList[4]).GetComponentEx<UILabel>();
                }
                if (count > 5 && mDMono.ObjectParamList[5] != null)
                {
                    StarGrid = ((GameObject)mDMono.ObjectParamList[5]).GetComponentEx<UIGrid>();
                    StarList = new GameObject[StarGrid.transform.childCount];

                    for (var i = 0; i < StarGrid.transform.childCount; i++)
                    {
                        StarList[i] = StarGrid.transform.GetChild(i).gameObject;
                    }
                }
                if (count > 6 && mDMono.ObjectParamList[6] != null)
                {
                    HireObj = ((GameObject)mDMono.ObjectParamList[6]);
                }
                if (count > 7 && mDMono.ObjectParamList[7] != null)
                {
                    HpSet = ((GameObject)mDMono.ObjectParamList[7]).GetMonoILRComponent<CamapignVictoryHpDataSet>();
                }
                if (count > 8 && mDMono.ObjectParamList[8] != null)
                {
                    ExpSet = ((GameObject)mDMono.ObjectParamList[8]).GetMonoILRComponent<CamapignVictoryExpDataSet>();
                }
            }
        }

        public void ShowUI(TeamMemberData data, bool isShowHp = false, bool isShowTempHp = false)
        {
            ResetStarGrid();
            HireObj.CustomSetActive(true);
            AlliancesManager.Instance.GetCacheAllianceMercenaries((partners) =>
            {
                LTPartnerData temp = partners.Find((partner) => partner.HeroId == data.HeroID);
                if (temp == null)
                {
                    temp = AlliancesManager.Instance.GetAlliancePartnerByHeroId(data.HeroID);
                }

                if (temp != null)
                {
                    OnShow(data.HeroID.ToString(), temp.HeroInfo, temp.Star, temp.UpGradeId, temp.HireAwakeLevel,
                        isShowHp, isShowTempHp, false);
                }
            });
        }
        

        public void ShowUI(string heroId, bool isShowHp = false, bool isShowTempHp = false)
        {
            ResetStarGrid();
            int star = 0;
            var tpl = new Hotfix_LT.Data.HeroStatTemplate();
            var charTpl = new Hotfix_LT.Data.HeroInfoTemplate();
            int upGradeId = 0;
            int awakenLevel = 0;
            if (int.Parse(heroId) < 0)//hire
            {
                var hireInfo = LTInstanceHireUtil.GetHireInfoByHeroId(int.Parse(heroId));
                if (hireInfo == null)
                {
                    EB.Debug.Log("userTeam data heroID <= {0}", heroId);
                    mDMono.gameObject.CustomSetActive(false);
                    return;
                }
                int nInfoID = int.Parse(hireInfo.character_id);
    
                star = hireInfo.star;
                tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStatByInfoId(nInfoID);
                charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(nInfoID);
                upGradeId = hireInfo.upgrade;
                if (HireObj != null)
                {
                    HireObj.CustomSetActive(true);
                }
            }
            else
            {
                HireObj.CustomSetActive(false);
                Hashtable heroStatData = null;
                if (!DataLookupsCache.Instance.SearchDataByID<Hashtable>(SmallPartnerPacketRule.OWN_HERO_STATS + "." + heroId, out heroStatData))
                {
                    EB.Debug.LogError("not hero data for heroId ={0}" , heroId);
                    mDMono.gameObject.CustomSetActive(false);
                    return;
                }
    
                string hero_templateid = EB.Dot.String("template_id", heroStatData, "");
                if (string.IsNullOrEmpty(hero_templateid))
                {
                    EB.Debug.LogError("hero_templateid is IsNullOrEmpty");
                    return;
                }
    
                star = EB.Dot.Integer("star", heroStatData, 0);
                tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(hero_templateid);
                charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(tpl.character_id, EB.Dot.Integer("skin", heroStatData, 0));
                upGradeId = EB.Dot.Integer("stat.awake.level", heroStatData, 0);
                awakenLevel = EB.Dot.Integer("stat.awaken.level", heroStatData, 0);
            }

            OnShow(heroId,charTpl,star,upGradeId,awakenLevel,isShowHp,isShowTempHp);
        }

        public void OnShow(string heroId,HeroInfoTemplate charTpl,int star,int upGradeId,int awakenLevel
            , bool isShowHp = false, bool isShowTempHp = false,bool IsExpSet = true)
        {
            Icon.spriteName = charTpl.icon;
            int quality, addLevel;
            LTPartnerDataManager.GetPartnerQuality(upGradeId, out quality, out addLevel);
    
            QuantityLevelSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
            GameItemUtil.SetColorfulPartnerCellFrame(quality, QuantityBGSprite);
            int.TryParse(heroId, out var intHeroId);
            LTPartnerData data = LTPartnerDataManager.Instance.GetPartnerByHeroId(intHeroId);
            LTPartnerConfig.SetLevelSprite(AttrSprite,charTpl.char_type,data != null && data.ArtifactLevel >= 0);
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, AttrSprite.transform, (PartnerGrade)charTpl.role_grade, charTpl.char_type);
            HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, QuantityLevelSprite.transform, quality, upgradeefclip);
            if (addLevel > 0)
            {
                breakLabel.transform.parent.gameObject.CustomSetActive(true);
                breakLabel.text = "+" + addLevel.ToString();
            }
            else
            {
                breakLabel.transform.parent.gameObject.CustomSetActive(false);
            }
    
            SetStarNumAndType(star, awakenLevel);
            SetProcess(heroId, isShowHp, isShowTempHp,IsExpSet);
        }

        private void ResetStarGrid()
        {
            if (StarGrid)
            {
                StarGrid.enabled = false;
            }
            else
            {
    
                UIGrid[] grids = mDMono.transform.GetComponentsInChildren<UIGrid>();
                for (int i = 0; i < grids.Length; i++)
                {
                    if (grids[i].name.StartsWith("star", StringComparison.CurrentCultureIgnoreCase))
                    {
                        StarGrid = grids[i];
                        StarGrid.enabled = false;
                    }
                }
            }
        }
        
    
        private void SetProcess(string heroId, bool isShowHp, bool isShowTempHp,bool IsExpSet)
        {
            if (isShowHp && HpSet != null)
            {
                HpSet.mDMono.gameObject.CustomSetActive(true);
                ExpSet.mDMono.gameObject.CustomSetActive(false);
                HpSet.SetHp(heroId, isShowTempHp);
                SetColor(heroId, isShowTempHp);
            }
            else
            {
                if (HpSet != null)
                {
                    HpSet.mDMono.gameObject.CustomSetActive(false);
                }

                if (!IsExpSet)
                {
                    ExpSet.mDMono.gameObject.CustomSetActive(false);
                    return;
                }
                ExpSet.mDMono.gameObject.CustomSetActive(true);
    			ExpSet.SetExp(heroId);
            }
        }
    
        private void SetColor(string heroId, bool isShowTempHp)
        {
            float hp = 0;
            if (isShowTempHp)
            {
                hp = LTHotfixManager.GetManager<SceneManager>().GetChallengeTempHp(heroId);
            }
            else
            {
                hp = LTChallengeInstanceHpCtrl.GetCombatHp(heroId);
            }
            if (hp <= 0)
            {
                Icon.color = GreyColor;
                QuantityLevelSprite.color = GreyColor;
                QuantityBGSprite.color = Color.gray;
                AttrSprite.color = GreyColor;
                HpSet.LevelLabel.transform.parent.GetComponent<UISprite>().color = GreyColor;
                for (int i = 0; i < StarList.Length; i++)
                {
                    StarList[i].GetComponent<UISprite>().color = GreyColor;
                }
            }
            else
            {
                Icon.color = WhiteColor;
                QuantityLevelSprite.color = WhiteColor;
                //QuantityBGSprite.color = WhiteColor;
                AttrSprite.color = WhiteColor;
                HpSet.LevelLabel.transform.parent.GetComponent<UISprite>().color = WhiteColor;
                for (int i = 0; i < StarList.Length; i++)
                {
                    StarList[i].GetComponent<UISprite>().color = WhiteColor;
                }
            }
        }
    
    	public static string AttrToLevelBG(Hotfix_LT.Data.eRoleAttr attr)
    	{
    		switch (attr)
    		{
    			case Hotfix_LT.Data.eRoleAttr.Feng:
    				return "Ty_Grade_Icon_Feng";
    			case Hotfix_LT.Data.eRoleAttr.Huo:
    				return "Ty_Grade_Icon_Huo";
    			case Hotfix_LT.Data.eRoleAttr.Shui:
    				return "Ty_Grade_Icon_Shui";
    			default:
    				EB.Debug.LogError("AttrToLevelBG error for attr={0}",attr);
    				return "Ty_Grade_Icon_Feng";
    		}
    	}
    
        public static int AttrToBGNum(Hotfix_LT.Data.eRoleAttr attr)
        {
            switch (attr)
            {
                case Hotfix_LT.Data.eRoleAttr.Shui:
                    return 0;
                case Hotfix_LT.Data.eRoleAttr.Feng:
                    return 1;
                case Hotfix_LT.Data.eRoleAttr.Huo:
                    return 2;
                default:
                    EB.Debug.LogError("AttrToLogo error for attr={0}" , attr);
                    return -1;
            }
        }
    
        //private void SetStarNum(int num)
        //{
        //    for (int i = 0; i < StarList.Length; i++)
        //    {
        //        StarList[i].CustomSetActive(i < num);
        //    }
        //    if (StarGrid != null)
        //    {
        //        StarGrid.Reposition();
        //    }
        //}
    
    
        private int lastStarCount = 0;
        private void SetStarNumAndType(int starCount,int awakenlevel)
        {
            GameObject StarUIGrid = StarGrid.gameObject;
           
            int starWidth;
            switch (starCount)
            {
                case 5:
                    starWidth = 26;
                    break;
                case 6:
                    starWidth = 22;
                    break;
                default:
                    starWidth = 32;
                    break;
            }
            if (starCount != lastStarCount)
            {
                for (int i = 0; i < StarUIGrid.transform.childCount; i++)
                {
                    StarUIGrid.transform.GetChild(i).gameObject.SetActive(false);
                }
                lastStarCount = starCount;
                //single star width
                //for more, can not use single width
                Vector3 firstPos = new Vector3(-(starCount - 1) / 2f * starWidth, 0, 0);
                for (int i = 0; i < starCount; i++)
                {
                    Transform star = StarUIGrid.transform.GetChild(i);
                    star.GetComponent<UISprite>().spriteName = LTPartnerConfig.PARTNER_AWAKN_STAR_DIC[awakenlevel];
                    star.gameObject.SetActive(true);
                    star.localPosition = firstPos + new Vector3(i * starWidth, 0, 0);
                }
            }
        }
    }
}
