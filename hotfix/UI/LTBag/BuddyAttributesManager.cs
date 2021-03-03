using UnityEngine;
using System.Collections;
using Hotfix_LT.Player;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 通过传入角色inventoryid获取该装备的所有属怿
    /// </summary>
    public class BuddyAttributesManager
    {
        public static int GetAttributeType(string templateid)
        {
            //int attributeType;
            string characterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacterid(templateid);
            //if (!LocalDataLookupsCache.Instance.SearchIntByID("hero_infos." + characterid + ".attribute", out attributeType)) return -1;
            //else return attributeType;

            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterid);
            if (charTpl != null)
            {
                return (int)charTpl.race;
            }
            return 0;
        }

        public static int GetAttributeTypeByCharacterId(string characterId)
        {
            //int attributeType;
            //if (!LocalDataLookupsCache.Instance.SearchIntByID("hero_infos." + characterId + ".attribute", out attributeType)) return -1;
            //else return attributeType;

            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterId);
            if (charTpl != null)
            {
                return (int)charTpl.race;
            }
            return 0;
        }

        public static string GetModelClass(string userid)
        {
            string economyid = PlayerManagerForFilter.Instance.GetPlayerTempalteid(long.Parse(userid));
            if (economyid == null)
            {
                string userEconomyIDPath = PlayerDataLookupSet.getSceneType() + "." + "pl" + "." + userid + ".tid";
                if (!DataLookupsCache.Instance.SearchDataByID<string>(userEconomyIDPath, out economyid)) return null;
            }
            string characterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacterid(economyid);
            int skin = 0;
            DataLookupsCache.Instance.SearchIntByID(PlayerDataLookupSet.getSceneType() + "." + "pl" + "." + userid + ".skin", out skin);
            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterid, skin);
            if (charTpl != null)
            {
                return charTpl.model_name;
            }
            return string.Empty;
        }

        //heroinfo
        public static string GetCharIcon(string characterid, string tail)
        {
            //string icon;
            //if (!LocalDataLookupsCache.Instance.SearchDataByID<string>("hero_infos." + characterid + ".icon", out icon)) return null;
            //return icon + tail;

            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterid);
            if (charTpl != null)
            {
                return charTpl.icon + tail;
            }
            return string.Empty;
        }

        //herostats
        public static string GetTemplateIcon(string template, string tail)
        {
            //string icon;
            string characterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacterid(template);
            //if (!LocalDataLookupsCache.Instance.SearchDataByID<string>("hero_infos." + characterid + ".icon", out icon)) return null;
            //return icon + tail;

            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterid);
            if (charTpl != null)
            {
                return charTpl.icon + tail;
            }
            return string.Empty;
        }

        public static string AttributeLogo(int attribute)
        {
            switch ((CharacterConstants.eRaceAttribute)attribute)
            {
                case CharacterConstants.eRaceAttribute.Jin:
                    return CharacterConstants.Atribute.JIN;
                case CharacterConstants.eRaceAttribute.Mu:
                    return CharacterConstants.Atribute.MU;
                case CharacterConstants.eRaceAttribute.Shui:
                    return CharacterConstants.Atribute.SHUI;
                case CharacterConstants.eRaceAttribute.Huo:
                    return CharacterConstants.Atribute.HUO;
                case CharacterConstants.eRaceAttribute.Tu:
                    return CharacterConstants.Atribute.TU;
                default:
                    return "";
            }
        }

        public static string AttributeLogo(CharacterConstants.eRaceAttribute attribute)
        {
            switch (attribute)
            {
                case CharacterConstants.eRaceAttribute.Jin:
                    return CharacterConstants.Atribute.JIN;
                case CharacterConstants.eRaceAttribute.Mu:
                    return CharacterConstants.Atribute.MU;
                case CharacterConstants.eRaceAttribute.Shui:
                    return CharacterConstants.Atribute.SHUI;
                case CharacterConstants.eRaceAttribute.Huo:
                    return CharacterConstants.Atribute.HUO;
                case CharacterConstants.eRaceAttribute.Tu:
                    return CharacterConstants.Atribute.TU;
                default:
                    return "";
            }
        }

        public static Color32 AttributeColor(CharacterConstants.eRaceAttribute attribute)
        {
            switch (attribute)
            {
                case CharacterConstants.eRaceAttribute.Jin:
                    return new Color32(44, 142, 255, 255);
                case CharacterConstants.eRaceAttribute.Mu:
                    return new Color32(34, 255, 53, 255);
                case CharacterConstants.eRaceAttribute.Shui:
                    return new Color32(241, 77, 243, 255);
                case CharacterConstants.eRaceAttribute.Huo:
                    return new Color32(249, 33, 33, 255);
                case CharacterConstants.eRaceAttribute.Tu:
                    return new Color32(255, 188, 44, 255);
                default:
                    return new Color32(165, 92, 45, 255);
            }
        }

        public static string AttributeLabel(CharacterConstants.eRaceAttribute attr)
        {
            string attrText = string.Empty;
            switch (attr)
            {
                case CharacterConstants.eRaceAttribute.Jin:
                    attrText = EB.Localizer.GetString("ID_ATTRIBUTE_GOLD");
                    break;
                case CharacterConstants.eRaceAttribute.Mu:
                    attrText = EB.Localizer.GetString("ID_ATTRIBUTE_WOOD");
                    break;
                case CharacterConstants.eRaceAttribute.Shui:
                    attrText = EB.Localizer.GetString("ID_ATTRIBUTE_WATER");
                    break;
                case CharacterConstants.eRaceAttribute.Huo:
                    attrText = EB.Localizer.GetString("ID_ATTRIBUTE_FIRE");
                    break;
                case CharacterConstants.eRaceAttribute.Tu:
                    attrText = EB.Localizer.GetString("ID_ATTRIBUTE_EARTH");
                    break;
            }
            return attrText;
        }

        public static string AttributeLine(string attribute)
        {
            switch (attribute)
            {
                case "1":
                    return "Ty_Header_Di_Blue";
                case "2":
                    return "Ty_Header_Di_Green";
                case "3":
                    return "Ty_Header_Di_Violet";
                case "4":
                    return "Ty_Header_Di_Red";
                case "5":
                    return "Ty_Header_Di_Yellow";
                default:
                    return "";
            }
        }

        public static double CalcBaseAttribute(double baseAttri, double inc, int level)
        {
            return baseAttri + CalcBaseAttributeInc(inc) * level;
        }

        public static double CalcBaseAttributeInc(double inc)
        {
            return inc / 100;
        }

        public static void GetTraningAttriButsByBuddyInventoryID(string dataID, AttributesData attData)
        {
            //判定角色类型  
            IDictionary trainingdata;
            if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>(dataID + ".stat.training", out trainingdata))
            {
                return;
            }
            else
            {
                int patk_level = 0;
                int matk_level = 0;
                int maxhp_level = 0;
                int mdef_level = 0;
                int pdef_level = 0;

                patk_level = EB.Dot.Integer("PATK.level", trainingdata, 0);
                matk_level = EB.Dot.Integer("MATK.level", trainingdata, 0);
                maxhp_level = EB.Dot.Integer("MaxHP.level", trainingdata, 0);
                mdef_level = EB.Dot.Integer("MDEF.level", trainingdata, 0);
                pdef_level = EB.Dot.Integer("PDEF.level", trainingdata, 0);


                attData.m_PATK += attData.m_Training_PATK * patk_level;
                attData.m_MATK += attData.m_Training_MATK * matk_level;
                attData.m_MaxHP += attData.m_Training_MaxHP * maxhp_level;
                attData.m_MDEF += attData.m_Training_MDEF * mdef_level;
                attData.m_PDEF += attData.m_Training_PDEF * pdef_level;

            }

        }

        private static float[] star_R = new float[] { 0, 0, 1, 1.05f, 1.15f, 1.20f, 1.25f };

        public static void GetStarAttriButsByBuddyInventoryID(string dataID, AttributesData attData, int star)
        {
            attData.m_PATK = attData.m_PATK * star_R[star];
            attData.m_MATK = attData.m_MATK * star_R[star];
            attData.m_MaxHP = attData.m_MaxHP * star_R[star];
            attData.m_MDEF = attData.m_MDEF * star_R[star];
            attData.m_PDEF = attData.m_PDEF * star_R[star];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataID"> heroStats.11111</param>
        /// <param name="ishero"></param>
        /// <returns></returns>
        public static AttributesData GetAttributsByBuddyInventoryID(string dataID, bool ishero, int star = -1)
        {
            //通过dataID 获取角色配表id和等线
            string buddyid;
            int level;
            //通过配表id获取所有的属怿

            //获取计算结果

            AttributesData attData = new AttributesData();
            //通过dataID 获取角色配表id和等线
            IDictionary inventorydata;
            if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>(dataID, out inventorydata))
            {
                return null;
            }
            else
            {
                buddyid = inventorydata["template_id"].ToString();
                if (!ishero)
                {
                    level = EB.Dot.Integer("stat.level", inventorydata, 1);
                }
                else
                {
                    if (!DataLookupsCache.Instance.SearchIntByID("level", out level))
                    {
                        level = 1;
                    }
                }

                if (star < 0)
                {
                    star = 0;
                    star = EB.Dot.Integer("star", inventorydata, 0);
                }
            }

            //通过配表id获取所有的属怿
            Hotfix_LT.Data.HeroStatTemplate hero = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(buddyid);
            if (hero == null)
            {
            }
            else
            {
                SetBaseAttribute(hero, star, level, ref attData);

                //将修为的数据计算进去
                GetTraningAttriButsByBuddyInventoryID(dataID, attData);
            }

            //计算升星
            {
                GetStarAttriButsByBuddyInventoryID(dataID, attData, star);
            }
            return attData;
        }

        private static float[] star_BR = new float[] { 0, 0, 1, 1.5f, 2.5f, 2.5f, 2.5f };
        /// <summary>
        /// 基础属性和升星有关
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="star"></param>
        /// <param name="level"></param>
        /// <param name="attData"></param>
        public static void SetBaseAttribute(Hotfix_LT.Data.HeroStatTemplate hero, int star, int level, ref AttributesData attData)
        {
            attData.SetDataByBuddy(hero);
            //获取计算结果
            attData.m_MaxHP = CalcBaseAttribute(attData.m_MaxHP * star_BR[star], attData.m_Inc_MaxHP, level);
            attData.m_PATK = CalcBaseAttribute(attData.m_PATK * star_BR[star], attData.m_Inc_PATK, level);
            attData.m_MATK = CalcBaseAttribute(attData.m_MATK * star_BR[star], attData.m_Inc_MATK, level);
            attData.m_PDEF = CalcBaseAttribute(attData.m_PDEF * star_BR[star], attData.m_Inc_PDEF, level);
            attData.m_MDEF = CalcBaseAttribute(attData.m_MDEF * star_BR[star], attData.m_Inc_MDEF, level);
        }
    }
}