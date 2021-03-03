using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTLevelUpFuncItem : DynamicCellController<Hotfix_LT.Data.FuncTemplate>
    {
        Hotfix_LT.Data.FuncTemplate data;
        public Color openColor;
        public Color limitColor;
        public UISprite MainSprite;
        public UILabel FuncNameLabel;
        public UILabel FuncDesLabel;
        public UILabel LimitLabel;
        public GameObject OpenObj;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            MainSprite = t.GetComponent<UISprite>("Sprite");
            FuncNameLabel = t.GetComponent<UILabel>("NameLabel");
            FuncDesLabel = t.GetComponent<UILabel>("DesLabel");
            LimitLabel = t.GetComponent<UILabel>("LockLabel");
            OpenObj = t.FindEx("OpenObj").gameObject;

            openColor = new Color32(125, 202, 255, 255);
            limitColor = new Color32(0, 80, 167, 255);
        }

        public override void Clean()
        {
            if (data == null)
            {
                mDMono.gameObject.CustomSetActive(false);
            }
        }
    
        public override void Fill(Hotfix_LT.Data.FuncTemplate itemData)
        {
            data = itemData;

            if (data == null)
            {
                Clean();
                return;
            }

            MainSprite.spriteName = data.iconName;
            FuncNameLabel.text = FuncNameLabel.transform.GetChild(0).GetComponent<UILabel>().text = data.display_name;
            FuncDesLabel.text = FuncDesLabel.transform.GetChild(0).GetComponent<UILabel>().text = data .discript ;

            //if (int.TryParse(data.condition, out int level))
            //{
            //    locktip = level + EB.Localizer.GetString("ID_codefont_in_DungeonHudController_3912");
            //}else if (data.condition.Contains("m-"))
            //{
            string locktip = data.GetConditionStr();
            //}
            LimitLabel.text = LimitLabel.transform.GetChild(0).GetComponent<UILabel>().text = locktip;
            
            if (!data.IsConditionOK())
            {
                mDMono.GetComponent<UISprite>().color = limitColor;
                OpenObj.SetActive(false);
                LimitLabel.gameObject.SetActive(true);
            }
            else
            {
                mDMono.GetComponent<UISprite>().color = openColor;
                OpenObj.SetActive(true);
                LimitLabel.gameObject.SetActive(false);
            }

            mDMono.gameObject.CustomSetActive(true);
        }
    }
}
