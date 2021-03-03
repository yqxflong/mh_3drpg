using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class UITaskSystemController : UIControllerHotfix
    {
        static UITaskSystemController _Instance;
        static public UITaskSystemController Instance { get { return _Instance; } }
        public override bool IsFullscreen() { return true; }

        //public GameObject m_Cube;
        public TaskUITabController categoryTabs;
        public List<Transform> RedPointList;
        public UINormalTaskScrollListLogicDataLookup ListLogicDataLookup;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;

            if (categoryTabs == null)
            {
                categoryTabs = t.GetMonoILRComponent<TaskUITabController>("NewBlacksmithView");
            }
            
            RedPointList = new List<Transform>();
            RedPointList.Add(t.FindEx("NewBlacksmithView/UpButtons/ButtonGrid/0_Main/RedPoint"));
            RedPointList.Add(t.FindEx("NewBlacksmithView/UpButtons/ButtonGrid/1_Normal/RedPoint"));
            RedPointList.Add(t.FindEx("NewBlacksmithView/UpButtons/ButtonGrid/2_Week/RedPoint"));
            for (int i = 0; i < RedPointList.Count; i++)
            {
                RedPointList[i].gameObject.SetActive(false);
            }
            ListLogicDataLookup = t.GetDataLookupILRComponent<UINormalTaskScrollListLogicDataLookup>("NewBlacksmithView/TaskViews/NormalTask/Scroll View");

            t.GetComponent<UIButton>("UINormalFrameBG/CancelBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
        }

        public override void OnCancelButtonClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTTaskHud");
            base.OnCancelButtonClick();
        }

        public override void Start()
        {
            if (_Instance == null)
                _Instance = this;
            base.Start();
        }

        public override void SetMenuData(object param)
        {
            string path = param as string;
            var categoryindex = 1;

            if (categoryTabs == null)
            {
                categoryTabs = controller.transform.GetMonoILRComponent<TaskUITabController>("NewBlacksmithView");
            }

            if (!string.IsNullOrEmpty(path))
            {
                categoryindex = categoryTabs.TabLibPrefabs.FindIndex(tab => tab.GameViewObj != null && path.StartsWith(tab.GameViewObj.name));
                if (categoryindex < 0)
                {
                    EB.Debug.LogWarning("UIStoreController: path {0} not found", path);
                    return;
                }
            }
            categoryTabs.SelectTab(categoryindex);
            categoryTabs.PressEventList.Add(new EventDelegate(SetListLogicDataLookup));
        }


        public void SetListLogicDataLookup()
        {
            int index = 0;
            for (int i = 0; i < categoryTabs.TabLibPrefabs.Count; i++)
            {
                if (categoryTabs.m_tabObj == categoryTabs.TabLibPrefabs[i].TabObj)
                {
                    index = i;
                }
            }

            string type = TypeDic[index];
            ListLogicDataLookup.PageSettings[0].InspectableFilter.Query[0].Value = type;
            ListLogicDataLookup.OnEnable();
        }

        public override IEnumerator OnAddToStack()
        {
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.maintask,SetMainRedPoint);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.daytask, SetDayRedPoint);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.weektask, SetWeekRedPoint);

            GlobalMenuManager.Instance.PushCache("LTTaskHud");

            return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.maintask, SetMainRedPoint);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.daytask, SetDayRedPoint);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.weektask, SetWeekRedPoint);
            DestroySelf();
            yield break;
        }

        public override void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;
            base.OnDestroy();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            DrawAllRedPoint();
        }

        public override void OnBlur()
        {
            base.OnBlur();
        }

        public override void OnPrefabSave()
        {
            controller.GetComponentInChildren<UITabController>().InitTab();
        }

        public void DrawAllRedPoint()
        {
            for (int i = 0; i < RedPointList.Count; ++i)
            {
                DrawRedPoint(i);
            }
        }

        public void DrawRedPoint(int index)
        {
            GetIsHaveReward(index);
        }

        private void SetMainRedPoint(RedPointNode node)
        {
            RedPointList[0].gameObject.CustomSetActive(node.num > 0);
        }
        private void SetDayRedPoint(RedPointNode node)
        {
            RedPointList[1].gameObject.CustomSetActive(node.num > 0);
        }
        private void SetWeekRedPoint(RedPointNode node)
        {
            RedPointList[2].gameObject.CustomSetActive(node.num > 0);
        }

        public void SelectTab(int index)
        {
            categoryTabs.SelectTab(index);
        }

        static Dictionary<int, string> TypeDic = new Dictionary<int, string>() { { 0, "1" }, { 1, "3" }, { 2, "8" } };
        static Dictionary<int, string> RPDic = new Dictionary<int, string>() { { 0, RedPointConst.maintask }, { 1, RedPointConst.daytask }, { 2, RedPointConst.weektask } };
        /// <summary>
        /// </summary>
        /// <param name="type">下标0，1，2</param>
        /// <returns></returns>
        static void GetIsHaveReward(int type)
        {
            Hashtable datas;
            if (!DataLookupsCache.Instance.SearchDataByID<Hashtable>("tasks", out datas, null))
            {
                EB.Debug.LogError("DataLookupsCache SearchData tasks fail");
                LTRedPointSystem.Instance.SetRedPointNodeNum(RPDic[type], 0);
                return;
            }
            var enumerator = datas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DictionaryEntry v = enumerator.Entry;
                IDictionary dic = v.Value as IDictionary;
                string t = TypeDic[type];
                if (dic["task_type"].ToString().Equals(t) && dic["state"].ToString().Equals("finished"))
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RPDic[type], 1);
                    return;
                }
            }

            //foreach (DictionaryEntry v in datas)
            //{
            //    IDictionary dic = v.Value as IDictionary;
            //    string t = TypeDic[type];
            //    if (dic["task_type"].ToString().Equals(t) && dic["state"].ToString().Equals("finished"))
            //    {
            //        LTRedPointSystem.Instance.SetRedPointNodeNum(RPDic[type],1);
            //        return;
            //    }
            //}

            //main chest reward
            if (type == 0)
            {
                int currAchievement = 0;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.taskacm.curr", out currAchievement);

                List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> mainstages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(7001);
                int stageCount = mainstages.Count;
                int CurrentIndex = TaskAchievementLogic.GetCurrentStageIndex();
                if (CurrentIndex <= -1 || CurrentIndex >= stageCount)
                {
                   EB.Debug.LogError("CurrentIndex <= -1 || CurrentIndex >= stageCount");
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RPDic[type], 0);
                    return;
                }
                var currentStageData = mainstages[CurrentIndex];
                int maxAchievement = currentStageData.stage;

                bool mainreceived;
                DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.taskacm_reward." + currentStageData.id, out mainreceived);
                if (currAchievement >= currentStageData.stage)
                {
                    if (!mainreceived)
                    {
                        LTRedPointSystem.Instance.SetRedPointNodeNum(RPDic[type], 1);
                        return;
                    }
                }
            } 
            //normal chest reward
            else if (type == 1) 
            {
                if (IsHaveReward(7101, "user_prize_data.taskliveness.curr", "user_prize_data.taskliveness_reward."))
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RPDic[type], 1);
                    return;
                }
            }
            //weeken chest reward
            else if (type == 2)
            {
                if (IsHaveReward(7201, "user_prize_data.taskweekliveness.curr", "user_prize_data.taskliveness_week_reward."))
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RPDic[type], 1);
                    return;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RPDic[type], 0);
        }

        private static bool IsHaveReward(int id, string key1, string key2)
        {
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(id);
            int maxLiveness = stages[stages.Count - 1].stage;
            int currLiveness = 0;
            DataLookupsCache.Instance.SearchIntByID(key1, out currLiveness);
            for (int i = 0; i < stages.Count; ++i)
            {
                var stage = stages[i];

                bool isreceived;
                DataLookupsCache.Instance.SearchDataByID<bool>(key2 + stage.id,
                    out isreceived);

                if (currLiveness >= stage.stage)
                {
                    if (!isreceived)
                        return true;
                }
            }

            return false;
        }

        public int CurrentIndex;
        private int GetCurrentStageIndex()
        {
            IDictionary have_receive_stages;
            if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("user_prize_data.taskacm_reward", out have_receive_stages))
            {
                return 0;
            }

            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stageTmps = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(7001);
            List<int> have_receive_stageIds = new List<int>();
            var iter = have_receive_stages.GetEnumerator();
            while (iter.MoveNext())
            {
                int id = 0;
                string key = iter.Key as string;
                if (!int.TryParse(key, out id))
                    EB.Debug.LogError("parse taskacm_reward.stage.key fail");
                else
                    have_receive_stageIds.Add(id);
            }
            if (have_receive_stageIds.Count <= 0)
            {
                return 0;
            }
            have_receive_stageIds.Sort();
            int maxId = have_receive_stageIds[have_receive_stageIds.Count - 1];
            int index = stageTmps.FindIndex(m => m.id == maxId);
            if (index < stageTmps.Count - 1)
                return index + 1;
            else
                return index;
        }

        public static void UpdateTaskType()
        {
            Hotfix_LT.Data.FuncTemplate m_FuncTpl = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10052);
            if (!m_FuncTpl.IsConditionOK())
            {
                //LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.daytask,0);
                //LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.weektask,0);
                //LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.maintask,0);
                return;
            }

            for (int i = 0; i < TypeDic.Count; ++i)
            {
                GetIsHaveReward(i);
            }
        }
    }
}
