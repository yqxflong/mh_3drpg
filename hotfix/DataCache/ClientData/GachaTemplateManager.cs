using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.Data
{
    public class CategoriesTemplate
    {
        public string Name;

        public List<CategoriesItemTemplate> Items;
    }

    public class CategoriesItemTemplate
    {
        public string Type;

        public string Data;

        public int Weight;

        public int Min;

        public int Max;
    }

    public class BoxesTemplate
    {
        public List<CategoriesDataTemplate> Categories;

        public string Name;
    }

    public class CategoriesDataTemplate
    {
        public string Category;

        public int Weight;

        public string Group;
    }

    public class GachaTemplateManager
    {
        private static GachaTemplateManager sInstance = null;

        public static GachaTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new GachaTemplateManager(); }
        }

        private Dictionary<string, CategoriesTemplate> mCategoriesTemplateDic = new Dictionary<string, CategoriesTemplate>();

        private Dictionary<string, BoxesTemplate> mBoxesTemplateDic = new Dictionary<string, BoxesTemplate>();

        public bool InitFromDataCache(GM.DataCache.Gacha tbls)
        {
            if (tbls == null)
            {
                EB.Debug.LogError("InitFromDataCache: tbls is null");
                return false;
            }

            var conditionSet = tbls.GetArray(0);

            if (!InitCategoriesTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init categoies table failed");
                return false;
            }

            if (!InitBoxesTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init boxes table failed");
                return false;
            }

            return true;
        }

        private bool InitCategoriesTbl(GM.DataCache.ConditionGacha tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitCategoriesTbl: categories tbl is null");
                return false;
            }

            mCategoriesTemplateDic = new Dictionary<string, CategoriesTemplate>();
            for (int i = 0; i < tbl.CategoriesLength; i++)
            {
                var tpl = ParseCategories(tbl.GetCategories(i));
                if (mCategoriesTemplateDic.ContainsKey(tpl.Name))
                {
                    EB.Debug.LogError("InitCategoriesTbl: {0} exists", tpl.Name);
                    mCategoriesTemplateDic.Remove(tpl.Name);
                }
                mCategoriesTemplateDic.Add(tpl.Name, tpl);
            }

            return true;
        }

        private CategoriesTemplate ParseCategories(GM.DataCache.Categories obj)
        {
            CategoriesTemplate tpl = new CategoriesTemplate();

            tpl.Name = obj.Name;
            tpl.Items = new List<CategoriesItemTemplate>();
            for (int i = 0; i < obj.ItemsLength; i++)
            {
                CategoriesItemTemplate temp = new CategoriesItemTemplate();
                temp.Type = obj.GetItems(i).Type;
                temp.Data = obj.GetItems(i).Data;
                temp.Weight = obj.GetItems(i).Weight;
                temp.Min = obj.GetItems(i).Min;
                temp.Max = obj.GetItems(i).Max;
                tpl.Items.Add(temp);
            }

            return tpl;
        }

        public CategoriesTemplate GetCatergoriesByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!mCategoriesTemplateDic.ContainsKey(name))
            {
                return null;
            }

            return mCategoriesTemplateDic[name];
        }

        private bool InitBoxesTbl(GM.DataCache.ConditionGacha tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitBoxesTbl: boxes tbl is null");
                return false;
            }

            mBoxesTemplateDic = new Dictionary<string, BoxesTemplate>();
            for (int i = 0; i < tbl.BoxesLength; i++)
            {
                var tpl = ParseBoxes(tbl.GetBoxes(i));
                if (mBoxesTemplateDic.ContainsKey(tpl.Name))
                {
                    EB.Debug.LogError("InitBoxesTbl: {0} exists", tpl.Name);
                    mBoxesTemplateDic.Remove(tpl.Name);
                }
                mBoxesTemplateDic.Add(tpl.Name, tpl);
            }

            return true;
        }

        private BoxesTemplate ParseBoxes(GM.DataCache.Boxes obj)
        {
            BoxesTemplate tpl = new BoxesTemplate();

            tpl.Name = obj.Name;
            tpl.Categories = new List<CategoriesDataTemplate>();
            for (int i = 0; i < obj.CategoriesLength; i++)
            {
                CategoriesDataTemplate temp = new CategoriesDataTemplate();
                temp.Category = obj.GetCategories(i).Category;
                temp.Weight = obj.GetCategories(i).Weight;
                temp.Group = obj.GetCategories(i).Group;
                tpl.Categories.Add(temp);
            }

            return tpl;
        }

        public BoxesTemplate GetBoxDataById(string boxId)
        {
            if (string.IsNullOrEmpty(boxId))
            {
                return null;
            }

            if (!mBoxesTemplateDic.ContainsKey(boxId))
            {
                return null;
            }

            return mBoxesTemplateDic[boxId];
        }
    }
}