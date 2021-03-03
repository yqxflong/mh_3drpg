using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ActifactActivationBehaviour:DynamicMonoHotfix
    {
        public UILabel AttrOneName;
        public UILabel AttrTwoName;
        public UILabel AttrThreeName;
        public UILabel ValueOneName;
        public UILabel ValueTwoName;
        public UILabel ValueThreeName;


        public UILabel SkillName;
        public UILabel SkillDesc;

        public GameObject Template;
        public UIGrid Grid;
        
        public LTShowItem ShowItem;
        public UILabel ShowItemCount;
        
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
           
            SkillName = t.GetComponent<UILabel>("Skill/Name");
            SkillDesc = t.GetComponent<UILabel>("Skill/Desc");

            AttrOneName = t.GetComponent<UILabel>("Infor/ExAttr/Label");
            AttrTwoName = t.GetComponent<UILabel>("Infor/ExAttr (1)/Label");
            AttrThreeName = t.GetComponent<UILabel>("Infor/ExAttr (2)/Label");
            ValueOneName = t.GetComponent<UILabel>("Infor/ExAttr/ValueLabel");
            ValueTwoName = t.GetComponent<UILabel>("Infor/ExAttr (1)/ValueLabel");
            ValueThreeName = t.GetComponent<UILabel>("Infor/ExAttr (2)/ValueLabel");
            
            ShowItem = t.GetMonoILRComponent<LTShowItem>("LTShowItem");
            ShowItemCount = t.GetComponent<UILabel>("LTShowItem/CountLabel");
            
            Template = t.Find("SkillDesc/Template").gameObject;
            Grid = t.GetComponent<UIGrid>("SkillDesc/Container");
            t.GetComponent<ConsecutiveClickCoolTrigger>("UpLevelBtn").clickEvent
                .Add(new EventDelegate(OnUpLevelBtnClick));
        }
        
        public override void OnDisable()
        {
            base.OnDisable();
            for (int i = 0; i < pool.Count; i++)
            {
                pool[i].gameObject.SetActive(false);
            }
        }

        public void Init(int infoId)
        {
            var list = CharacterTemplateManager.Instance.GetArtifactEquipmentHaveDesc(infoId);

            if (list.Count > 0)
            {
                SetSkillUpLevel(list);
                SetAttr(list[0]);
                SetShowItem(list[0]);
            }
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
            for (int i = 0; i < list.Count; i++)
            {
                var obj = GetObjFromPool();
                obj.transform.SetParent(Grid.transform, false);
                obj.gameObject.SetActive(true);
                UILabel descLabel = obj.GetComponent<UILabel>();
                descLabel.text = string.Format("+{0}：,{1}", list[i].enhancementLevel, list[i].desc);
            }

            Grid.Reposition();
        }

        public void SetAttr(ArtifactEquipmentTemplate temp)
        {
            SetSkill(temp.skillId);

            string attr = temp.AttributeAdd;
            string[] split = attr.Split(';');
            if (split.Length > 0)
            {
                AttrOneName.text = split[0].Split(',')[0];
                ValueOneName.text = split[0].Split(',')[1];
            }

            if (split.Length > 1)
            {
                AttrTwoName.text = split[1].Split(',')[0];
                ValueTwoName.text = split[1].Split(',')[1];
            }

            if (split.Length > 2)
            {
                AttrThreeName.text = split[2].Split(',')[0];
                ValueThreeName.text = split[2].Split(',')[1];
            }

        }

        private void OnUpLevelBtnClick()
        {
          
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

            var obj = GameObject.Instantiate(Template);
            pool.Add(obj);
            return obj;
        }
        
        public void SetShowItem(ArtifactEquipmentTemplate temp)
        {
            string[] args = temp.ItemCost.Split(',');
            if (args.Length >= 2)
            {
                int curCount = GameItemUtil.GetInventoryItemNum(args[0]);
                int.TryParse(args[1], out var needCount);
                ShowItem.LTItemData = new LTShowItemData(args[0], needCount, LTShowItemType.TYPE_GAMINVENTORY);
                string color = curCount < needCount
                    ? LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal
                    : LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                ShowItemCount.text = string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat + "/{2}", color,
                    curCount, needCount);
            }
        }
    }
}