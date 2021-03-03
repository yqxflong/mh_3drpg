// #define LOG_ClientDataUtil

using System;
using Hotfix_LT.UI;

namespace Hotfix_LT.Data
{
    public static class ClientDataUtil
    {
        public static void LogWithTime(string msg)
        {
#if LOG_ClientDataUtil
            DateTime now = DateTime.Now;
            EB.Debug.LogError($"{now.Hour}:{now.Minute}:{now.Second}=={msg}");
#endif
        }

        public static void OnFlatBuffersUpdated(FlatBuffers.ByteBuffer buffer)
        {
            LogWithTime("OnFlatBuffersUpdated=====>");
            var config = GM.DataCache.Config.GetRootAsConfig(buffer);
            int configLen = config.ArrayLength;
            LogWithTime($"OnFlatBuffersUpdated=====>configLen: {configLen}");
            for (int i = 0; i < configLen; ++i)
            {
                var entity = config.GetArray(i);
                string name = entity.Name;
                string version = entity.Version;
                var bufferSegment = entity.GetBufferBytes();
                if (bufferSegment == null)
                {
                    EB.Debug.LogWarning("OnFlatBuffersUpdated: buffer is null");
                    continue;
                }

                EB.Debug.Log("{0} offset = {1} count = {2}, array = {3}", name, bufferSegment.Value.Offset, bufferSegment.Value.Count, bufferSegment.Value.Array.Length);
                SparxHub.Instance.DataCacheManager.ProcessCache(name, version, bufferSegment.Value);
            }
            LogWithTime("<=====OnFlatBuffersUpdated");
        }

        public static void OnResetTemplateManager()
        {
            SkillTemplateManager.ClearUp();
            BuffTemplateManager.ClearUp();
            ImpactTemplateManager.ClearUp();
            EconemyTemplateManager.ClearUp();
            TaskTemplateManager.ClearUp();
            BuyResourceTemplateManager.ClearUp();
            SceneTemplateManager.ClearUp();
            CharacterTemplateManager.ClearUp();
            GuideManager.ClearUp();
            GuideTemplateManager.ClearUp();
            DialogueTemplateManager.ClearUp();
            GuideAudioTemplateManager.ClearUp();
            MessageTemplateManager.ClearUp();
            FuncTemplateManager.ClearUp();
            GuideNodeTemplateManager.ClearUp();
            VIPTemplateManager.ClearUp();
            if (AlliancesManager.Instance.Config != null)
            {
                AlliancesManager.Instance.Config.CleanUp();
            }
            AllianceTemplateManager.ClearUp();
            ShopTemplateManager.ClearUp();
            NewGameConfigTemplateManager.ClearUp();
        }


        public static void OnFlatBuffersEntityUpdated(string name, System.ArraySegment<byte> range)
        {
            //EB.Debug.LogError("frameCount : {0} , name : {1}", UnityEngine.Time.frameCount, name);
            //EB.Debug.LogError("OnFlatBuffersEntityUpdated : {0}", name);

            FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(range.Array, range.Offset);
            switch (name)
            {
                case "Combat":
                    var combat = GM.DataCache.Combat.GetRootAsCombat(buffer);
                    SkillTemplateManager.Instance.InitFromDataCache(combat);
                    SkillTemplateManager.Instance.InitSkillLevelUpFromDataCache(combat);
                    BuffTemplateManager.Instance.InitTemplateFromCache(combat);
                    break;
                case "Economy":
                    var economy = GM.DataCache.Economy.GetRootAsEconomy(buffer);
                    EconemyTemplateManager.Instance.InitFromDataCache(economy);
                    break;
                case "Task":
                    var task = GM.DataCache.Task.GetRootAsTask(buffer);
                    TaskTemplateManager.Instance.InitFromDataCache(task);
                    break;
                case "Resource":
                    var resource = GM.DataCache.Resource.GetRootAsResource(buffer);
                    BuyResourceTemplateManager.Instance.InitFromDataCache(resource);
                    break;
                case "Scene":
                    var scene = GM.DataCache.Scene.GetRootAsScene(buffer);
                    SceneTemplateManager.Instance.InitFromDataCache(scene);
                    break;
                case "Character":
                    var character = GM.DataCache.Character.GetRootAsCharacter(buffer);
                    CharacterTemplateManager.Instance.InitFromDataCache(character);
                    break;
                case "Guide":
                    var guide = GM.DataCache.Guide.GetRootAsGuide(buffer);
                    var gd = guide.GetArray(0);
                    GuideManager.Instance.InitConfigData(gd);
                    GuideTemplateManager.Instance.InitFromDataCache(gd);
                    DialogueTemplateManager.Instance.InitDialogueData(gd);
                    GuideAudioTemplateManager.Instance.InitGuideAudioData(gd);
                    MessageTemplateManager.Instance.InitFromDataCache(gd);
                    FuncTemplateManager.Instance.InitFromDataCache(gd);
                    PreviewTemplateManager.Instance.InitFromDataCache(gd);
                    FuncActivedTemplateManager.Instance.InitFuncActivedData(gd);
                    GuideNodeTemplateManager.Instance.InitGuideNode(gd);
                    break;
                case "Event":
                    var evt = GM.DataCache.Event.GetRootAsEvent(buffer);
                    WelfareTemplateManager.Instance.InitTemplateFromCache(evt);
                    EventTemplateManager.Instance.InitFromDataCache(evt);
                    break;
                case "Vip":
                    var vip = GM.DataCache.Vip.GetRootAsVip(buffer);
                    VIPTemplateManager.Instance.InitTemplateFromCache(vip); 
                    break;
                case "Alliance":
                    var alliance = GM.DataCache.Alliance.GetRootAsAlliance(buffer);
                    AlliancesManager.Instance.Config.OnUpdateFunc(alliance);
                    AllianceTemplateManager.Instance.InitFromDataCache(alliance);
                    break;
                case "Shop":
                    var shop = GM.DataCache.Shop.GetRootAsShop(buffer);
                    ShopTemplateManager.Instance.InitFromDataCache(shop);
                    break;
                case "NewGameConfig":
                    var newGameConfig = GM.DataCache.NewGameConfig.GetRootAsNewGameConfig(buffer);
                    NewGameConfigTemplateManager.Instance.InitFromDataCache(newGameConfig);
                    break;

            }
        }
    }
}
