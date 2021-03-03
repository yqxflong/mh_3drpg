using System.Collections;
using EB.Sparx;

namespace Hotfix_LT.UI
{
 public class LTPartnerAPI : EB.Sparx.SparxAPI
{
    public System.Func<string,bool> ExceptionFun;

    public LTPartnerAPI()
    {
        endPoint = Hub.Instance.ApiEndPoint;
    }

    private void DefaultDataHandler(Hashtable data)
    {
        EB.Debug.Log("LTPartnerAPI.DefaultDataHandler: call default data handler");
    }

    private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
    {
        dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);

        if (ExceptionFun != null && response.error != null) //处理Lostemp的异常
        {
            string error = response.error.ToString();
            if (ExceptionFun(error))
            {
                ExceptionFun = null;
                return;
            }
        }

        ExceptionFun = null;

        if (ProcessResponse(response))
        {
            dataHandler(response.hashtable);
        }
        else
        {
            dataHandler(null);
        }
    }

    private int BlockService(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
    {
        LoadingSpinner.Show();

        return endPoint.Service(request, delegate (EB.Sparx.Response response)
        {
            LoadingSpinner.Hide();

            ProcessResult(response, dataHandler);
        });
    }

    private int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
    {
        return endPoint.Service(request, delegate (EB.Sparx.Response response)
        {
            ProcessResult(response, dataHandler);
        });
    }

    public void RequestSummonBuddy(int characterId, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/summonBuddy");
        request.AddData("characterId", characterId);
        BlockService(request, dataHandler);
    }
    
    public void RequestLevelupFetterBuddy(int buddyId,int index,int condition,int toLevel, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/levelupFetterBuddy");
        request.AddData("buddyId", buddyId);
        request.AddData("index", index);
        request.AddData("condition", condition);
        request.AddData("toLevel", toLevel);
        BlockService(request, dataHandler);
    }

    public void RequestLevelUp(int heroId,int level, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/levelUpToBuddy");
        request.AddData("heroId", heroId);
        request.AddData("level", level);
        BlockService(request, dataHandler);
    }

    public void RequestUpGrade(int heroId, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/awakeBuddy");
        request.AddData("heroId", heroId);
        BlockService(request, dataHandler);
    }

    public void RequestStarUp(int infoId, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/starUp");
        request.AddData("characterId", infoId);
        BlockService(request, dataHandler);
    }

    public void RequestProficiencyUp(int buddyId, int form, int type, int level, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/proficiencyUpBuddy");
        request.AddData("buddyId", buddyId);
        request.AddData("form", form);
        request.AddData("type", type);
        request.AddData("level", level);
        BlockService(request, dataHandler);
    }

    public void RequestChipTrans(int stateId,int num, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/getHeroShardByUseUpStone");
        request.AddData("template_id", stateId);
        request.AddData("num", num);
        BlockService(request, dataHandler);
    }

    public void RequestSkillBreak(int heroId, int skillType, ArrayList goodsList, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/skillBreakToBuddy");
        request.AddData("heroId", heroId);
        request.AddData("skillId", skillType);
        request.AddData("itemArray", goodsList);
        BlockService(request, dataHandler);
    }
    
    public void RequestActiveOrUpgradeArtifEquip(int buddyId, int level, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/activeOrUpgradeArtifEquip");
        request.AddData("buddyId", buddyId);
        request.AddData("enhancement_level", level);
        BlockService(request, dataHandler);
    }

    public void RequestLeader(int heroId, System.Action<Hashtable> dataHandler)
    {
        int SceneId;
        DataLookupsCache.Instance.SearchIntByID("mainlands.id", out SceneId);
        EB.Sparx.Request request = endPoint.Post("/herostats/changeSceneLeader");
        request.AddData("leaderId", heroId.ToString());
        request.AddData("sceneId", SceneId);
        BlockService(request, dataHandler);//hero stats not found
    }

    /// <summary>
    /// 请求使用物品
    /// </summary>
    /// <param name="inventoryId"></param>
    /// <param name="amount"></param>
    /// <param name="itemId"></param>
    /// <param name="dataHandler"></param>
    public void RequestUseItem(string inventoryId, int amount, int index, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/gaminventory/use");
        request.AddData("inventoryId", inventoryId);
        request.AddData("num", amount);
        request.AddData("index", index);
        BlockService(request, dataHandler);
    }
    public void RequestPartnerTrans(int heroId1,int heroId2,bool switchEquip,bool useReplaceTicket,bool switchPeak,bool switchPo,System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/switchBuddyLevelAndEtc");
        request.AddData("buddyId1",heroId1);
        request.AddData("buddyId2", heroId2);
        request.AddData("switchEquip",switchEquip);
        request.AddData("useReplaceTicket", useReplaceTicket);
        request.AddData("switchPeak", switchPeak);
        request.AddData("switchPo", switchPo);
        BlockService(request, dataHandler);
    }
    public void RequestPartnerAwake(int heroId, System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/awakenBuddy");
        request.AddData("buddyId", heroId);
        BlockService(request, dataHandler);
    }
    public void RequestUseAwakenSkin(int heroId,int skinId,int sceneId,System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/useAwakenBuddySkin");
        request.AddData("buddyId", heroId);
        request.AddData("use", skinId);
        request.AddData("sceneId", sceneId);
        BlockService(request, dataHandler);
    }
    /// <summary>
    /// 请求领取首获奖励
    /// </summary>
    /// <param name="heroId"></param>
    /// <param name="dataHandler"></param>
    public void RequestReceiveFirstGotReward(int heroId,System.Action<Hashtable> dataHandler)
    {
        EB.Sparx.Request request = endPoint.Post("/herostats/getBuddyReward");
        request.AddData("buddyId", heroId);
        BlockService(request, dataHandler);
    }

        /// <summary>
        /// 请求领取等级补偿奖励
        /// </summary>
        public void RequestReceiveLevelCompensatedReward(int Id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/sign_in/getLevelCompensatedReward");
            request.AddData("id", Id);
            BlockService(request, dataHandler);
        }
        /// <summary>
        /// 上报战力信息
        /// </summary>
        /// <param name="power"></param>
        /// <param name="dataHandler"></param>
        public void ReportpowerChanged(int power)
        {
            //EB.Sparx.Request request = endPoint.Post("/sign_in/getLevelCompensatedReward");
            //request.AddData("id", power);
            //BlockService(request, dataHandler);
        }

    }
}