using System;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace Hotfix_LT.UI
{
    public class ArtifactDetailBehaviour:DynamicMonoHotfix
    {
        public UILabel AttrOneName;
        public UILabel AttrTwoName;
        public UILabel AttrThreeName;
        public UILabel AttrFourName;
        
        
        public UILabel ValueOneName;
        public UILabel ValueTwoName;
        public UILabel ValueThreeName;
        public UILabel ValueFourName;
        
        public UISprite SpriteOneName;
        public UISprite SpriteTwoName;
        public UISprite SpriteThreeName;
        public UISprite SpriteFourName;
        
        public UILabel AddOneName;
        public UILabel AddTwoName;
        public UILabel AddThreeName;
        public UILabel AddFourName;


        public UILabel ArtifactName;
        public UILabel ArtifactLevel;
        public DynamicUISprite icon;
        public GameObject shadow;

        public UILabel SkillName;
        public UILabel SkillDesc;

        public Transform Template;
        public UIGrid Grid;
        
        public UILabel ButtonLabel;

        private LTPartnerData data;
        
        private ArtifactEquipmentTemplate templateNow;
        private ArtifactEquipmentTemplate templateNext;
        private List<ArtifactEquipmentTemplate> templateList;

        private List<Transform> fxLists;

        private bool CanStrength
        {
            get
            {
                if (data != null)
                {
                    return data.ArtifactLevel >= 0;
                }

                return false;
            }
        }

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            icon = t.GetComponent<DynamicUISprite>("Icon");
            shadow = icon.transform.Find("Icon").gameObject;
            ArtifactName = t.GetComponent<UILabel>("Name");
            ArtifactLevel = t.GetComponent<UILabel>("Icon/LevelLabel");
            SkillName = t.GetComponent<UILabel>("Skill/Name");
            SkillDesc = t.GetComponent<UILabel>("Skill/Desc");

            AttrOneName = t.GetComponent<UILabel>("Infor/1/NameLabel");
            AttrTwoName = t.GetComponent<UILabel>("Infor/2/NameLabel");
            AttrThreeName = t.GetComponent<UILabel>("Infor/3/NameLabel");
            AttrFourName = t.GetComponent<UILabel>("Infor/4/NameLabel");
            
            ValueOneName = t.GetComponent<UILabel>("Infor/1/NumLabel");
            ValueTwoName = t.GetComponent<UILabel>("Infor/2/NumLabel");
            ValueThreeName = t.GetComponent<UILabel>("Infor/3/NumLabel");
            ValueFourName = t.GetComponent<UILabel>("Infor/4/NumLabel");
            
            SpriteOneName = t.GetComponent<UISprite>("Infor/1/Icon");
            SpriteTwoName = t.GetComponent<UISprite>("Infor/2/Icon");
            SpriteThreeName = t.GetComponent<UISprite>("Infor/3/Icon");
            SpriteFourName = t.GetComponent<UISprite>("Infor/4/Icon");
            
            AddOneName = t.GetComponent<UILabel>("Infor/1/EquipNumLabel");
            AddTwoName = t.GetComponent<UILabel>("Infor/2/EquipNumLabel");
            AddThreeName = t.GetComponent<UILabel>("Infor/3/EquipNumLabel");
            AddFourName = t.GetComponent<UILabel>("Infor/4/EquipNumLabel");
            
            ButtonLabel = t.GetComponent<UILabel>("UpLevelBtn/Label");

            Template = t.FindEx("SkillDesc/Template",false);
            Grid = t.GetComponent<UIGrid>("SkillDesc/Scroll View/Container");
           
        }


        public override void OnDisable()
        {
            for (int i = 0; i < pool.Count; i++)
            {
                pool[i].gameObject.SetActive(false);
            }
        }

        public void Init(int infoId)
        {
            data = LTPartnerDataManager.Instance.GetPartnerByInfoId(infoId);
            templateNow = CharacterTemplateManager.Instance.GetArtifactEquipmentByLevel(infoId, data.ArtifactLevel,true);
            templateNext = CharacterTemplateManager.Instance.GetArtifactEquipmentByLevel(infoId, data.ArtifactLevel + 1);
            templateList = CharacterTemplateManager.Instance.GetArtifactEquipmentHaveDesc(infoId);
            SetSkillUpLevel(templateList);
            SetAttr(templateNow, templateNext);
           
            string levelText = data.ArtifactLevel > 0 ? $"+{data.ArtifactLevel}" : string.Empty;
            ArtifactLevel.gameObject.SetActive(data.ArtifactLevel > 0);
            ArtifactLevel.text = levelText;
            LTUIUtil.SetText(ArtifactName,$"{templateNow.name}{levelText}");
        }

        public void SetSkill(int skillId)
        {
            //eg:10110
            SkillTemplate item = SkillTemplateManager.Instance.GetTemplate(skillId);
            SkillName.text = item.Name;
            SkillDesc.text = item.Description;
        }

        public void SetSkillUpLevel(List<ArtifactEquipmentTemplate> list)
        {
            if (Template == null)
            {
                return;
            }
            //Button文字
            string buttonText = CanStrength ? "ID_ENCHANT" : "ID_uifont_in_LTPartnerHud_Label_17";
            LTUIUtil.SetText(ButtonLabel, EB.Localizer.GetString(buttonText));
            
            for (int i = 0; i < list.Count; i++)
            {
                var obj = GetObjFromPool();
                obj.transform.SetParent(Grid.transform, false);
                obj.gameObject.SetActive(true);
                UILabel levelLabel = obj.GetComponent<UILabel>();
                UILabel descLabel = obj.GetComponent<UILabel>("Desc");
                UISprite sprite = obj.GetComponent<UISprite>("Sprite");
                if (data.ArtifactLevel >= list[i].enhancementLevel)
                {
                    levelLabel.text = $"[44fe7c]+{list[i].enhancementLevel}";
                    descLabel.text = $"[44fe7c]{list[i].desc}";
                    sprite.color = Color.yellow;
                }
                else
                {
                    levelLabel.text = $"[bcbcbc]+{list[i].enhancementLevel}";
                    descLabel.text = $"[bcbcbc]{list[i].desc}";
                    sprite.color = Color.grey;
                }
            }

            Grid.Reposition();
        }

        public void SetAttr(ArtifactEquipmentTemplate temp,ArtifactEquipmentTemplate next)
        {
            SetSkill(temp.skillId);
            fxLists= new List<Transform>();
            double ValueOne = 0;
            double ValueTwo = 0;
            double ValueThree = 0;
            double ValueFour = 0;
            string attr = temp.AttributeAdd;
            string[] split = attr.Split(';');
            if (split.Length > 0)
            {
                string name = split[0].Split(',')[0];
                LTUIUtil.SetText(AttrOneName,AttrTypeTrans(name));
                SpriteOneName.spriteName = AttrTypeSprite(name);
                ValueOne = Convert.ToDouble(split[0].Split(',')[1]);
                ValueOneName.text = StringFormat(ValueOne);
                
                fxLists.Add(SpriteOneName.transform.FindEx("Fx",false));
            }

            if (split.Length > 1)
            {
                string name = split[1].Split(',')[0];
                LTUIUtil.SetText(AttrTwoName,AttrTypeTrans(name));
                SpriteTwoName.spriteName = AttrTypeSprite(name);
                ValueTwo = Convert.ToDouble(split[1].Split(',')[1]);
                ValueTwoName.text = StringFormat(ValueTwo);
                
                fxLists.Add(SpriteTwoName.transform.FindEx("Fx",false));
            }

            if (split.Length > 2)
            {
                string name = split[2].Split(',')[0];
                LTUIUtil.SetText(AttrThreeName,AttrTypeTrans(name));
                SpriteThreeName.spriteName = AttrTypeSprite(name);
                ValueThree = Convert.ToDouble(split[2].Split(',')[1]);
                ValueThreeName.text =  StringFormat(ValueThree);
                
                fxLists.Add(SpriteThreeName.transform.FindEx("Fx",false));
            }
            SpriteFourName.transform.parent.gameObject.CustomSetActive(split.Length > 3);
            if (split.Length > 3)
            {
                string name = split[3].Split(',')[0];
                LTUIUtil.SetText(AttrFourName,AttrTypeTrans(name));
                SpriteFourName.spriteName = AttrTypeSprite(name);
                ValueFour = Convert.ToDouble(split[3].Split(',')[1]);
                ValueFourName.text =  StringFormat(ValueFour);
                
                fxLists.Add(SpriteFourName.transform.FindEx("Fx",false));
            }
            
            if (next!=null && CanStrength)
            {
                string[] splitNext = next.AttributeAdd.Split(';');
                if (splitNext.Length > 0)
                {
                    AddOneName.text = ADDStringFormat(Convert.ToDouble(splitNext[0].Split(',')[1])-ValueOne);
                }

                if (splitNext.Length > 1)
                {
                    AddTwoName.text = ADDStringFormat(Convert.ToDouble(splitNext[1].Split(',')[1])-ValueTwo) ;
                }

                if (splitNext.Length > 2)
                {
                    AddThreeName.text = ADDStringFormat(Convert.ToDouble(splitNext[2].Split(',')[1])-ValueThree);
                }
                
                if (splitNext.Length > 3)
                {
                    AddFourName.text = ADDStringFormat(Convert.ToDouble(splitNext[3].Split(',')[1])-ValueFour);
                }
            }

            icon.spriteName = temp.iconId;
            shadow.GetComponent<DynamicUISprite>().spriteName = temp.iconId;
            shadow.gameObject.CustomSetActive(data.ArtifactLevel < 0);
        }

        private List<GameObject> pool = new List<GameObject>();
        private GameObject GetObjFromPool()
        {
            if (pool.Count > 0)
            {
                foreach (var VARIABLE in pool)
                {
                    if (VARIABLE.gameObject.activeSelf == false)
                    {
                        return VARIABLE;
                    }
                }
            }

            var obj = GameObject.Instantiate(Template.gameObject);
            pool.Add(obj);
            return obj;
        }
        
        private string AttrTypeTrans(string str)
        {
            switch (str)
            {
                case "IncATK":
                case "ATK": return EB.Localizer.GetString("ID_ATTR_ATK");
                case "IncMaxHP":
                case "MaxHP": return EB.Localizer.GetString("ID_ATTR_HP");
                case "IncDEF":
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
                case "IncATK":
                case "ATK": return "Partner_Properyt_Icon_Gongji";
                case "IncMaxHP":
                case "MaxHP": return "Partner_Properyt_Icon_Shengming";
                case "IncDEF":
                case "DEF": return "Partner_Properyt_Icon_Fangyu";
                case "CritP": return "Partner_Properyt_Icon_Baoji";
                case "CritV": return "Partner_Properyt_Icon_Baoshang";
                case "Speed": return "Partner_Properyt_Icon_Sudu";
                default: return EB.Localizer.GetString("ID_ATTR_Unknown");
            }
        }
        
        private string StringFormat(double num)
        {
            return string.Format("{0}%", (num * 100));
        }
        private string ADDStringFormat(double num)
        {
            return string.Format("+{0}%", (num * 100));
        }

        public void PlayAnim()
        {
            foreach (var VARIABLE in fxLists)
            {
                if (VARIABLE!=null)
                {
                    VARIABLE.gameObject.CustomSetActive(false);
                    VARIABLE.gameObject.CustomSetActive(true);
                }
            }
        }
    }
}