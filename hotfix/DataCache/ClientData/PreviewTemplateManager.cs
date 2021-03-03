using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB;
namespace Hotfix_LT.Data
{
    public class PreviewTemplate
    {
        public int id = 0;
        public List<PreviewItem> items = new List<PreviewItem>();

        public static PreviewTemplate Parse(GM.DataCache.PreviewInfo obj)
        {
            PreviewTemplate tpl = new PreviewTemplate();
            tpl.id = obj.Id;
            if (string.IsNullOrEmpty(obj.Data)) return tpl;
            object ob = JSON.Parse(obj.Data);
            if (ob == null || !(ob is ArrayList))
            {
                EB.Debug.LogError("Preview is illegal {0}" , tpl.id);
                return tpl;
            }
            ArrayList array = ob as ArrayList;
            for (int i = 0; i < array.Count; i++)
            {
                tpl.items.Add(new PreviewItem(array[i]));
            }
            return tpl;
        }
    }

    public class PreviewItem
    {
        public int id = 0;
        public string type = string.Empty;
        public int num = 1;
        public PreviewItem(object obj)
        {
            id = EB.Dot.Integer("d", obj, id);
            type = EB.Dot.String("t", obj, type);
            num = EB.Dot.Integer("n", obj, num);
        }

        public PreviewItem(int id, string type, int num)
        {
            this.id = id;
            this.type = type;
            this.num = num;
        }
    }

    public class PreviewTemplateManager
    {
        private static PreviewTemplateManager sInstance = null;

        private Dictionary<int, PreviewTemplate> mPreviewTbl = null;

        public static PreviewTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new PreviewTemplateManager(); }
        }


        public bool InitFromDataCache(GM.DataCache.ConditionGuide tbls)
        {
            if (tbls == null)
            {
                EB.Debug.LogError("InitFromDataCache: tbls is null");
                return false;
            }

            var conditionSet = tbls;
            mPreviewTbl = new Dictionary<int, PreviewTemplate>(conditionSet.PreviewLength);
            for (int i = 0; i < conditionSet.PreviewLength; ++i)
            {
                var tpl = PreviewTemplate.Parse(conditionSet.GetPreview(i));
                if (mPreviewTbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitPreviewTbl: {0} exists", tpl.id);
                    mPreviewTbl.Remove(tpl.id);
                }
                mPreviewTbl.Add(tpl.id, tpl);
            }
            return true;
        }

        public PreviewTemplate GetPreview(int id)
        {
            PreviewTemplate result = null;
            if (!mPreviewTbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetPreview: preview not found, id = {0}", id);
            }
            return result;
        }
    }
}