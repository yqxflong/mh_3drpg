using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Data
{
    public class FuncActivedTemplate
    {
        public int id = 0;
        public string name = "";
        public int level = 0;
        public string dialogue = "";
        public string describe = "";

        public static FuncActivedTemplate Parse(GM.DataCache.FunctionsActivedInfo obj)
        {
            FuncActivedTemplate tpl = new FuncActivedTemplate();
            if (obj != null)
            {
                tpl.id = obj.Id;
                tpl.name = obj.Name;
                tpl.level = obj.Level;
                tpl.dialogue = obj.Dialogue;
                tpl.describe = obj.Describe;
            }
            return tpl;
        }
    }

    public class FuncActivedTemplateManager
    {
        private Dictionary<int, FuncActivedTemplate> mFuncActivedDataDic;

        public Dictionary<int, FuncActivedTemplate> FuncActivedDataDic
        {
            get
            {
                return mFuncActivedDataDic;
            }
        }

        private static FuncActivedTemplateManager mInstance;

        public static FuncActivedTemplateManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new FuncActivedTemplateManager();
                }
                return mInstance;
            }
        }

        public bool InitFuncActivedData(GM.DataCache.ConditionGuide data)
        {
            if (data == null)
            {
                return false;
            }

            var conditionSet = data;
            mFuncActivedDataDic = new Dictionary<int, FuncActivedTemplate>();
            for (int i = 0; i < conditionSet.FunctionsActivedLength; i++)
            {
                var tpl = FuncActivedTemplate.Parse(conditionSet.GetFunctionsActived(i));
                if (mFuncActivedDataDic.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitFuncActivedData: {0} exists", tpl.id);
                    mFuncActivedDataDic.Remove(tpl.id);
                }
                mFuncActivedDataDic.Add(tpl.id, tpl);
            }
            return true;
        }
    }
}