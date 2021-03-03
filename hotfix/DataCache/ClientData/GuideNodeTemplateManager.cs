using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
namespace Hotfix_LT.Data
{
    public class GuideNodeTemplateManager
    {
        public List<GuideNodeTemplate> listGuideNode = new List<GuideNodeTemplate>();

        private static GuideNodeTemplateManager _instance;
        public static GuideNodeTemplateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GuideNodeTemplateManager();
                }
                return _instance;
            }
        }

        public static void ClearUp()
        {
            if (_instance != null)
            {
                _instance.listGuideNode.Clear();
            }
        }

        public void InitGuideNode(GM.DataCache.ConditionGuide data)
        {
            if (data == null) return;

            var conditionSet = data;
            listGuideNode.Clear();
            for (int i = 0; i < conditionSet.GuideNodesLength; i++)
            {
                var node = conditionSet.GetGuideNodes(i);
                GuideNodeTemplate tplNode = new GuideNodeTemplate();
                tplNode.umeng_id = node.UmengId;
                tplNode.group_id = node.GroupId;
                tplNode.step_id = node.StepId;
                tplNode.next_id = node.NextId;
                tplNode.fore_id = node.ForeId;
                if (!string.IsNullOrEmpty(node.StepType))
                {
                    string[] sps = node.StepType.Split(',');
                    if (sps.Length > 1)
                    {
                        tplNode.step_type = new int[2] { int.Parse(sps[0]), int.Parse(sps[1]) };
                        tplNode.focus_view = sps[2];
                    }
                    else
                    {
                        EB.Debug.LogError("GuideNodeTemplate step_type Split < 2");
                    }
                }
                tplNode.roll_back_id = node.RollBackId;
                tplNode.skip_to_id = node.SkipToId;
                tplNode.condition_cmd = node.ConditionCmd;
                tplNode.c_parameter = node.CParameter;
                tplNode.c_receipt_type = node.CReceiptType;
                tplNode.c_need_parameter = node.CNeedParameter;
                tplNode.execute_cmd = node.ExecuteCmd;
                tplNode.e_parameter = node.EParameter;
                tplNode.e_fail_type = node.EFailType;
                listGuideNode.Add(tplNode);
            }

            if(listGuideNode.Count>1)
            {
                listGuideNode.Sort((x, y) => { return x.step_id < y.step_id ? -1 : 0; });
            }
            //listGuideNode.Clear();  //TODOX
            Hotfix_LT.UI.GuideNodeManager.GetInstance().SetGuideNodes(listGuideNode);
        }
    }
    public class GuideNodeTemplate
    {
        public string umeng_id;
        public int group_id;
        public int step_id;
        public int next_id;
        public int fore_id;

        public int[] step_type;
        public string focus_view;

        public int roll_back_id;
        public int skip_to_id;
        public string condition_cmd;
        public string c_parameter;
        public string c_receipt_type;
        public string c_need_parameter;
        public string execute_cmd;
        public string e_parameter;
        public int e_fail_type;
    }
}
