using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using Hotfix_LT.Player;

namespace Hotfix_LT.UI
{
    public class PlayerFashionDataLookUp : DataLookupHotfix, IHotfixUpdate
    {
        public static readonly List<string> VALID_FASHION_PART = new List<string>
        {
            "Head",
            "Armor",
            "Weapon"
        };

        private string sceneType;

        public string SceneType
        {
            set
            {
                sceneType = value;
            }
        }

        private long userId;

        public long UserId
        {
            set
            {
                userId = value;

                if (sceneType == null)
                {
                    //string dataId = "buddyinventory.pos0.fashion";
                    //string dataId = FashionPacketRule.CUR_PLAYER_MODELID;
                    //this.RegisterDataID(dataId);
                }
                else
                {
                    //string dataId = sceneType + "." + "pl" + "." + userId + "." + "fashion";
                    //string dataId = string.Format(FashionPacketRule.OTHER_PLAYER_MODELID, sceneType, userId);
                    //this.RegisterDataID(dataId);
                }
            }
        }

        private bool isLoadFashion;
        public bool IsLoadFashion
        {
            get
            {
                return isLoadFashion;
            }
            set
            {
                isLoadFashion = value;
                if (!IsLoadFashion)
                {
                    PlayerEquipmentDataLookup pedl = mDL.gameObject.GetDataLookupILRComponent<PlayerEquipmentDataLookup>();
                    if (pedl != null && pedl.UserId != 0)
                    {
                        pedl.SceneType = sceneType;
                        pedl.UserId = userId;

                        IDictionary incomingData = null;
                        DataLookupsCache.Instance.SearchDataByID(pedl.GetDataId(), out incomingData);
                        if (sceneType == null)
                        {
                            for (var i = 0; i < PlayerEquipmentDataLookup.VALID_EQUIPMENT_SLOTS.Count; i++)
                            {
                                var equipType = PlayerEquipmentDataLookup.VALID_EQUIPMENT_SLOTS[i];
                                if (incomingData == null || !incomingData.Contains(equipType))
                                {
                                    pedl.LoadDeafultEquip(equipType);
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i < PlayerEquipmentDataLookup.VALID_SCENE_EQUIP_SLOTS.Count; i++)
                            {
                                var simpleEquipType = PlayerEquipmentDataLookup.VALID_SCENE_EQUIP_SLOTS[i];
                                if (incomingData == null || !incomingData.Contains(simpleEquipType))
                                {
                                    pedl.LoadDeafultEquip(EconomyConstants.AbToEquipmentType(simpleEquipType));
                                }
                            }
                        }
                    }
                }
            }
        }


        private AvatarComponent _avatar = null;
        private bool _shouldSetDisableFlowLight = false;
        public override void OnEnable()
        {
            mDL.hasStarted = true;
            IsLoadFashion = true;
            _avatar = mDL.transform.GetComponent<AvatarComponent>();
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            if (!mDL.hasStarted)
            {
                IsLoadFashion = false;
                return;
            }

            if (dataID == null || value == null)
            {
                IsLoadFashion = false;
                return;
            }

            base.OnLookupUpdate(dataID, value);

            int modelId = 0;
            int.TryParse(value.ToString(), out modelId);
            if (modelId <= 0)
            {
                IsLoadFashion = false;
                return;
            }
            UpdateFashiion(modelId);
        }

        private void UpdateFashiion(int modelId)
        {
            /*FashionModelInfo fashionModelInfo = EconemyTemplateManager.Instance.GetFashionModelInfo(modelId);
            if (fashionModelInfo == null)
            {
                IsLoadFashion = false;
                return;
            }
            Dictionary<string, string> modelData = new Dictionary<string, string>();
            string raceModel = GetModeAtributeName(userId.ToString());
            if (string.IsNullOrEmpty(raceModel))
            {
                IsLoadFashion = false;
                return;
            }
            modelData.Add(VALID_FASHION_PART[0], raceModel + fashionModelInfo.HeadModelName);
            modelData.Add(VALID_FASHION_PART[1], raceModel + fashionModelInfo.ArmorModelName);
            modelData.Add(VALID_FASHION_PART[2], raceModel + fashionModelInfo.WeaponModelName);
            foreach (KeyValuePair<string, string> data in modelData)
            {
                if (VALID_FASHION_PART.Contains(data.Key))
                {
                    AvatarComponent avatar = GetComponent<AvatarComponent>();
                    if (avatar != null)
                    {
                        avatar.LoadEquipment(data.Key, data.Value);
                        _shouldSetDisableFlowLight = PerformanceManager.Instance.CurrentEnvironmentInfo.slowDevice;
                    }
                }
            }

            if (userId == LoginManager.Instance.LocalUserId.Value && !PerformanceManager.Instance.CurrentEnvironmentInfo.slowDevice)
            {
                GameEngine.Instance.SetHideColorTarget(this.gameObject);
            }
            IsLoadFashion = true;*/
        }

        public static string GetModeAtributeName(string userid)
        {
            string model_name = BuddyAttributesManager.GetModelClass(userid);
            if (string.IsNullOrEmpty(model_name))
            {
                return string.Empty;
            }
            string attribute_name = model_name.Replace("-Variant", "");
            return attribute_name + "_";
        }

        public void Update()
        {

            if (!GameEngine.Instance.IsTimeToRootScene)
            {
                return;
            }
            if (_shouldSetDisableFlowLight && _avatar != null && _avatar.Ready)
            {
                DisableFlowLight();

                _shouldSetDisableFlowLight = false;
            }
        }

        void DisableFlowLight()
        {
            SkinnedMeshRenderer[] renderers = mDL.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                if (renderers[i] == null || renderers[i].gameObject.layer == GameEngine.Instance.transparentFXLayer || renderers[i].gameObject.layer == GameEngine.Instance.ui3dLayer || renderers[i].gameObject.layer == GameEngine.Instance.uiLayer)
                {
                    continue;
                }
                Material[] currentMaterials = renderers[i].materials;
                //Must fix if FlowLight material not in the second index
                if (currentMaterials.Length >= 2 && !currentMaterials[1].name.Contains("Hide"))
                {
                    Material[] newMaterails = new Material[currentMaterials.Length - 1];
                    int index = 0;
                    for (int m = 0; m < currentMaterials.Length; ++m)
                    {
                        if (m == 1) continue;
                        newMaterails[index] = currentMaterials[m];
                        index++;
                    }
                    renderers[i].materials = newMaterails;
                }
            }
        }
    }
}