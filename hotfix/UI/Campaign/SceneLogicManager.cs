using UnityEngine;
using System.Collections;


namespace Hotfix_LT.UI
{
    /// <summary>
    /// 场景逻辑管理器
    /// </summary>
    public class SceneLogicManager : DynamicMonoHotfix
    {
        public static string SCENE_MAINLAND_TYPE = "mainlands";
        public static string SCENE_COMBAT = "combat";
        public static string SCENE_LCCampaign = "LCCampaign";
        public static SceneLogic current_scenelogic;

        public static bool isCombat()
        {
            if (SCENE_COMBAT == getSceneType()) return true;
            return false;
        }

        public static bool isLCCampaign()
        {
            if (SCENE_LCCampaign == getSceneType()) return true;
            return false;
        }

        public static bool isMainlands()
        {
            if (SCENE_MAINLAND_TYPE == getSceneType()) return true;
            return false;
        }

        /// <summary>
        /// 获取当前的场景类型名称
        /// </summary>
        /// <returns></returns>
        public static string getSceneType()
        {
            string state = "";
            DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out state);
            if (state == null) return string.Empty;

            if ("MainLand" == state)
            {
                return SCENE_MAINLAND_TYPE;
            }
            else if ("Combat" == state)
            {
                return SCENE_COMBAT;
            }
            else
            {
                return state;
            }
        }

        public static string getMultyPlayerSceneType()
        {
            string state = "";
            DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out state);
            if (state == null) return string.Empty;
            if ("MainLand" == state)
            {
                return "mainlands";
            }
            else
            {
                return "mainlands";
            }
        }

        public static SceneLogic CurrentSceneLogic
        {
            get
            {
                return MainLandLogic.GetInstance();

                //string state = "";
                //DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out state);
                //if (state.CompareTo("MainLand") == 0)
                //{
                //    return MainLandLogic.GetInstance();
                //}
                //else if (state.CompareTo("Campaign") == 0)
                //{
                //    return CampaignLogic.GetInstance();
                //}
                //else
                //{
                //    return MainLandLogic.GetInstance();
                //}
            }
        }

        public static string GetCurrentSceneName()
        {
            return CurrentSceneLogic.CurrentSceneName;
        }
    }
}