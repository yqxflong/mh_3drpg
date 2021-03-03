using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class PartnerStrategyViewController : UIControllerHotfix
    {

        private UILabel RecommendSuit,RSuitShadow, RecommendAttr,RAttrShadow, PartnerName, PartnerNameShadow, RecommendDes;
        private UISprite DmgLevelSp, DmgParmSp, ExsitLevelSp, ExsitParmSp, AssistLevelSp, AssistParmSp, ControlLevelSp, ControlParmSp,
            RecommondAttriSp, PartnerQualitySp;
        private DynamicUISprite RecommondSuitSp;
        private DialogueTextureCmp Portrait;
        private UIGrid partnerGrid;
        private Transform itemParent;
        private LTpartnerInfoItem lTpartnerInfoItem;
        private List<LTpartnerInfoItem> recommondPartnerList;
        private Data.HeroStrategyInfo currentStategyInfo;
        private Data.HeroInfoTemplate curHeroinfo;
        private int currentInfoid;
        private int MaxWidth = 468;
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            RecommendSuit = t.GetComponent<UILabel>("Content/CompreJudge/Suit/SuitAttrEql/SuitEqlLabel");
            RecommendAttr = t.GetComponent<UILabel>("Content/CompreJudge/Suit/SuitAttr/SuitAttLabel");
            RSuitShadow = t.GetComponent<UILabel>("Content/CompreJudge/Suit/SuitAttrEql/SuitEqlLabel/SuitEqlLabel (1)");
            RAttrShadow = t.GetComponent<UILabel>("Content/CompreJudge/Suit/SuitAttr/SuitAttLabel/SuitAttLabel (1)");
            PartnerName = t.GetComponent<UILabel>("Content/Partner/name");
            PartnerNameShadow = t.GetComponent<UILabel>("Content/Partner/name/Label");
            RecommendDes = t.GetComponent<UILabel>("Content/CompreJudge/SuitTeam/des");

            PartnerQualitySp = t.GetComponent<UISprite>("Content/Partner/quality");
            DmgLevelSp = t.GetComponent<UISprite>("Content/CompreJudge/ShowScroce/Type/Level");
            DmgParmSp = t.GetComponent<UISprite>("Content/CompreJudge/ShowScroce/Type/Fill");
            ExsitLevelSp = t.GetComponent<UISprite>("Content/CompreJudge/ShowScroce/Type (1)/Level");
            ExsitParmSp = t.GetComponent<UISprite>("Content/CompreJudge/ShowScroce/Type (1)/Fill");
            AssistLevelSp = t.GetComponent<UISprite>("Content/CompreJudge/ShowScroce/Type (2)/Level");
            AssistParmSp = t.GetComponent<UISprite>("Content/CompreJudge/ShowScroce/Type (2)/Fill");
            ControlLevelSp = t.GetComponent<UISprite>("Content/CompreJudge/ShowScroce/Type (3)/Level");
            ControlParmSp = t.GetComponent<UISprite>("Content/CompreJudge/ShowScroce/Type (3)/Fill");
            RecommondSuitSp = t.GetComponent<DynamicUISprite>("Content/CompreJudge/Suit/SuitAttrEql/SuitEqlType");
            RecommondAttriSp = t.GetComponent<UISprite>("Content/CompreJudge/Suit/SuitAttr/SuitAttrType");
            Portrait = t.GetComponent<DialogueTextureCmp>("Content/Partner/HalfPortrait");

            lTpartnerInfoItem = t.GetMonoILRComponent<LTpartnerInfoItem>("Content/CompreJudge/SuitTeam/team/InfoItem");
            recommondPartnerList = new List<LTpartnerInfoItem>();
            recommondPartnerList.Add(t.GetMonoILRComponent<LTpartnerInfoItem>("Content/CompreJudge/SuitTeam/team/InfoItem"));
            recommondPartnerList.Add(t.GetMonoILRComponent<LTpartnerInfoItem>("Content/CompreJudge/SuitTeam/team/InfoItem (1)"));
            recommondPartnerList.Add(t.GetMonoILRComponent<LTpartnerInfoItem>("Content/CompreJudge/SuitTeam/team/InfoItem (2)"));
            recommondPartnerList.Add(t.GetMonoILRComponent<LTpartnerInfoItem>("Content/CompreJudge/SuitTeam/team/InfoItem (3)"));
            itemParent = t.GetComponent<Transform>("Content/CompreJudge/SuitTeam/team");
            partnerGrid = t.GetComponent<UIGrid>("Content/CompreJudge/SuitTeam/team");
        }

        public override void OnDestroy()
        {
            recommondPartnerList = null;
            base.OnDestroy();
        }
        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen()
        {
            return false;
        }
        public override void SetMenuData(object param)
        {
            if (param != null)
            {
                curHeroinfo = param as Data.HeroInfoTemplate;
                currentInfoid = curHeroinfo.id;
            }
            if (currentInfoid != 0)
            {
                currentStategyInfo = Data.CharacterTemplateManager.Instance.GetHeroStrategyInfoByInfoId(currentInfoid);
            }
            else
            {
                EB.Debug.LogError("HeroinfoId can not equips 0");
                controller.Close();
                return;
            }
            LTDrawCardLookupController.DrawType = DrawCardType.none;
            SetUIShow();
        }

        public override IEnumerator OnAddToStack()
        {
            //SetRecommondedMatch();
            setForamtion();
            return base.OnAddToStack();
            
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }



        private void SetUIShow()
        {
            if (currentStategyInfo == null)
            {
                EB.Debug.LogError("PartnerStrategyInfo is null");
                return;
            }
            Portrait.spriteName = curHeroinfo.skin;
            string sp = "";
            if (LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC.TryGetValue((PartnerGrade)curHeroinfo.role_grade, out sp))
            {
                PartnerQualitySp.spriteName = sp;
            }
            PartnerName.text = PartnerNameShadow.text = curHeroinfo.name;
            int NameLength = curHeroinfo.name.Length;
            if (LTPartnerConfig.PARTNER_STRATEGY_LEVEL_SPRITE.TryGetValue(currentStategyInfo.dmglevel, out sp))
            {
                DmgLevelSp.spriteName = sp;
                DmgParmSp.color = LTPartnerConfig.PARTNER_STRATEGY_LEVEL_COLOR[currentStategyInfo.dmglevel];
                DmgParmSp.width = MaxWidth * currentStategyInfo.dmglevel / 5;
            }
            if (LTPartnerConfig.PARTNER_STRATEGY_LEVEL_SPRITE.TryGetValue(currentStategyInfo.existlevel, out sp))
            {
                ExsitLevelSp.spriteName = sp;
                ExsitParmSp.color = LTPartnerConfig.PARTNER_STRATEGY_LEVEL_COLOR[currentStategyInfo.existlevel];
                ExsitParmSp.width = MaxWidth * currentStategyInfo.existlevel / 5;
            }
            if (LTPartnerConfig.PARTNER_STRATEGY_LEVEL_SPRITE.TryGetValue(currentStategyInfo.controllevel, out sp))
            {
                ControlLevelSp.spriteName = sp;
                ControlParmSp.color = LTPartnerConfig.PARTNER_STRATEGY_LEVEL_COLOR[currentStategyInfo.controllevel];
                ControlParmSp.width = MaxWidth * currentStategyInfo.controllevel / 5;
            }
            if (LTPartnerConfig.PARTNER_STRATEGY_LEVEL_SPRITE.TryGetValue(currentStategyInfo.assistlevel, out sp))
            {
                AssistLevelSp.spriteName = sp;
                AssistParmSp.color = LTPartnerConfig.PARTNER_STRATEGY_LEVEL_COLOR[currentStategyInfo.assistlevel];
                AssistParmSp.width = MaxWidth * currentStategyInfo.assistlevel / 5;
            }
            SuitTypeInfo info = EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(currentStategyInfo.recommondedsuit);
            if (info != null)
            {
                RecommendSuit.text = RSuitShadow.text = info.TypeName;
                RecommondSuitSp.spriteName = info.SuitIcon;
            }
            RecommendAttr.text =  RAttrShadow.text = AttrTypeTrans(currentStategyInfo.recommondedAttr);
            RecommondAttriSp.spriteName = AttrTypeSprite(currentStategyInfo.recommondedAttr);           
            RecommendDes.text = currentStategyInfo.matchDes;
           
        }

        //private void SetRecommondedMatch()
        //{
        //    if (currentStategyInfo == null)
        //    {
        //        EB.Debug.LogError("PartnerStrategyInfo is null");
        //        return;
        //    }
        //    int heroArrayLength = 0;
        //    if (currentStategyInfo.matchheroArray != null)
        //    {
        //        heroArrayLength = currentStategyInfo.matchheroArray.Length;
        //    }
        //    if (recommondPartnerList == null)
        //    {
        //        recommondPartnerList = new List<LTpartnerInfoItem>();
        //        for (int i = 0; i < heroArrayLength; i++)
        //        {
        //            var infoItem = InstantiateEx<LTpartnerInfoItem>(lTpartnerInfoItem, partnerGrid.transform, string.Format("infoItem{0}", i));
        //            var heroinfo = CharacterTemplateManager.Instance.GetHeroInfo(currentStategyInfo.matchheroArray[i]);
        //            if (heroinfo != null)
        //            {
        //                infoItem.mDMono.gameObject.CustomSetActive(false);
        //                infoItem.Fill(heroinfo);
        //                infoItem.mDMono.gameObject.CustomSetActive(true);
        //                recommondPartnerList.Add(infoItem);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        int _recommondPartnerListCount = recommondPartnerList.Count;
        //        if (_recommondPartnerListCount >= heroArrayLength)
        //        {
        //            for (int i = 0; i < _recommondPartnerListCount; i++)
        //            {
        //                var tempinfoitem = recommondPartnerList[i];
        //                if (i < heroArrayLength)
        //                {
        //                    var heroinfo = CharacterTemplateManager.Instance.GetHeroInfo(currentStategyInfo.matchheroArray[i]);
        //                    if (heroinfo != null)
        //                    {
        //                        tempinfoitem.mDMono.gameObject.CustomSetActive(false);
        //                        tempinfoitem.Fill(heroinfo);
        //                        tempinfoitem.mDMono.gameObject.CustomSetActive(true);

        //                    }
        //                    else
        //                    {
        //                        tempinfoitem.mDMono.gameObject.CustomSetActive(false);
        //                    }
        //                }
        //                else
        //                {
        //                    tempinfoitem.mDMono.gameObject.CustomSetActive(false);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < heroArrayLength; i++)
        //            {
        //                if (i < _recommondPartnerListCount)
        //                {
        //                    var tempinfoitem = recommondPartnerList[i];
        //                    var heroinfo = CharacterTemplateManager.Instance.GetHeroInfo(currentStategyInfo.matchheroArray[i]);
        //                    if (heroinfo != null)
        //                    {
        //                        tempinfoitem.mDMono.gameObject.CustomSetActive(false);
        //                        tempinfoitem.Fill(heroinfo);
        //                        tempinfoitem.mDMono.gameObject.CustomSetActive(true);

        //                    }
        //                    else
        //                    {
        //                        tempinfoitem.mDMono.gameObject.CustomSetActive(false);
        //                    }
        //                }
        //                else
        //                {
        //                    var infoItem = InstantiateEx<LTpartnerInfoItem>(lTpartnerInfoItem, partnerGrid.transform, string.Format("infoItem{0}", i));
        //                    var heroinfo = CharacterTemplateManager.Instance.GetHeroInfo(currentStategyInfo.matchheroArray[i]);
        //                    if (heroinfo != null)
        //                    {
        //                        infoItem.mDMono.gameObject.CustomSetActive(false);
        //                        infoItem.Fill(heroinfo);
        //                        infoItem.mDMono.gameObject.CustomSetActive(true);
        //                        recommondPartnerList.Add(infoItem);
        //                    }
        //                    else
        //                    {
        //                        infoItem.mDMono.gameObject.CustomSetActive(false);
        //                    }

        //                }
        //            }

        //        }

        //    }

        //    partnerGrid.Reposition();
        //}

        private void setForamtion()
        {
            if (currentStategyInfo == null)
            {
                EB.Debug.LogError("PartnerStrategyInfo is null");
                return;
            }
            int heroArrayLength = 0;
            if (currentStategyInfo.matchheroArray != null)
            {
                heroArrayLength = currentStategyInfo.matchheroArray.Length;
            }
            for (int i = 0; i < recommondPartnerList.Count; i++)
            {
                if(i< heroArrayLength)
                {
                    var heroinfo = CharacterTemplateManager.Instance.GetHeroInfo(currentStategyInfo.matchheroArray[i]);
                    recommondPartnerList[i].Fill(heroinfo);

                }
                else
                {
                    recommondPartnerList[i].Fill(null);
                }
            }
            partnerGrid.Reposition();
        }

        private string AttrTypeTrans(string str)
        {
            switch (str)
            {
                case "ATK": return EB.Localizer.GetString("ID_ATTR_ATK");
                case "MaxHP": return EB.Localizer.GetString("ID_ATTR_HP");
                case "DEF": return EB.Localizer.GetString("ID_ATTR_DEF");
                case "CritP": return EB.Localizer.GetString("ID_ATTR_CritP");
                case "CritV": return EB.Localizer.GetString("ID_ATTR_CritV");
                case "Speed": return EB.Localizer.GetString("ID_ATTR_Speed");

                default: return EB.Localizer.GetString("ID_ATTR_Unknown");
            }
        }

        private string AttrTypeSprite(string str)
        {
            switch (str)
            {
                case "ATK": return "Partner_Properyt_Icon_Gongji";
                case "MaxHP": return "Partner_Properyt_Icon_Shengming";
                case "DEF": return "Partner_Properyt_Icon_Fangyu";
                case "CritP": return "Partner_Properyt_Icon_Baoji";
                case "CritV": return "Partner_Properyt_Icon_Baoshang";
                case "Speed": return "Partner_Properyt_Icon_Sudu";
                default: return EB.Localizer.GetString("ID_ATTR_Unknown");
            }
        }

    }
}
