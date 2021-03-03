using UnityEngine;
using System.Collections;
using System;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class EquipmentInfoEven
    {
        public static Action<int> SelectEquipmentID;
        public static Action<int> SelectEquipIDBySyn;
    }
    
    
    public class LTEquipmentInforUIController : UIControllerHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            firstController = t.GetMonoILRComponent<LTEquipmentFirstInfo>("EquipmentInfo");

            UIPos = new Transform[4];
            UIPos[0] = t.GetComponent<Transform>("Pos0");
            UIPos[1] = t.GetComponent<Transform>("Pos1");
            UIPos[2] = t.GetComponent<Transform>("Pos2");
            UIPos[3] = t.GetComponent<Transform>("Pos3");

            t.GetComponent<ConsecutiveClickCoolTrigger>("EquipmentInfo/UpLevelBtn").clickEvent.Add(new EventDelegate(OnEquipLevelUpBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("EquipmentInfo/EquipSynInOutBtn").clickEvent[0] = new EventDelegate(OnEquipSynPutInOutBtnClick);
            t.GetComponent<ConsecutiveClickCoolTrigger>("EquipmentInfo/ReplaceBtn").clickEvent.Add(new EventDelegate(OnEquipmentOrReplaceBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("EquipmentInfo/GetOffBtn").clickEvent.Add(new EventDelegate(OnGetOffBtnClick));
        }

        public LTEquipmentFirstInfo firstController;
        public Transform[] UIPos;
        private EquipPartType equipType = EquipPartType.none;
        private bool CheckMouseClick = false;
        public override bool ShowUIBlocker { get { return false; } }
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable ht = param as Hashtable;
            int FromType = (int)ht["fromType"];//0为装备背包栏,1为已装备栏,2为升级背包栏,3为合成装备栏
            int Eid = (int)ht["eid"];
            int Pos=(int)ht["pos"];//"pos"为初始化时的位置，需要自己定
            if (ht["equipType"] != null)
            {
                equipType = (EquipPartType)ht["equipType"];
            }
            switch (Pos)
            {
                case 0: { firstController.mDMono.transform.localPosition = UIPos[0].localPosition; } break;
                case 1: { firstController.mDMono.transform.localPosition = UIPos[1].localPosition; } break;
                case 2: { firstController.mDMono.transform.localPosition = UIPos[2].localPosition; } break;
                case 3: { firstController.mDMono.transform.localPosition = UIPos[3].localPosition; } break;
            }
            firstController.Show(FromType, Eid);
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            firstController.UpdateCollider();//更新碰撞体的区域
            if (SelectEquipEven.SelectEquipment != null)
            {
                SelectEquipEven.SelectEquipment(firstController.data.Eid);
            }
        }
        public override IEnumerator OnRemoveFromStack()
        {
            StopAllCoroutines();
            if (SelectEquipEven.SelectEquipment != null)
            {
                SelectEquipEven.SelectEquipment(-1);
            }
            DestroySelf();
            yield break;
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
            CheckMouseClick = true;
        }

        public override void OnDisable()
        {
            ErasureMonoUpdater();
            CheckMouseClick = false;
        }


        public void Update()
        {
            if(!GuideNodeManager.IsGuide &&( CheckMouseClick&&(Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))))
            {
                if (!firstController.mDMono.GetComponent<BoxCollider>().bounds.Contains(UICamera.lastWorldPosition) && !firstController.SecondInfoUI.mDMono.GetComponent<BoxCollider>().bounds.Contains(UICamera.lastWorldPosition))
                {
                    CheckMouseClick = false;
                    controller. Close();
                }
            }
        }
    
        public void OnEquipmentOrReplaceBtnClick()
        {
            FusionAudio.PostEvent("UI/Equipment/Equipped");

            if (LTPartnerEquipMainController.instance != null)
            {
                if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetEditView)
                {
                    LTPartnerEquipmentInfoController.isSuitTextShow = true;
                    LTPartnerEquipmentInfoController.isChangeColor = true;
                    LTPartnerEquipmentInfoController.isSuitTypeShow = firstController.data.SuitType;
                    LTPartnerEquipMainController.instance.AddToEquipmentInfoList((int)firstController.data.Type, firstController.data.Eid);
                    controller.Close();
                    return;
                }
            }

            LTPartnerEquipDataManager.Instance .RequireEquip(firstController.data.Eid,LTPartnerEquipMainController .CurrentPartnerData.HeroId , firstController.data.Type,delegate(bool success) {
                if (success)
                {
                    LTPartnerEquipPartnerInfoController.isSuitTextShow = true;
                    LTPartnerEquipPartnerInfoController.isChangeColor = true;
                    LTPartnerEquipPartnerInfoController.isSuitTypeShow = firstController.data.SuitType;                                  
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                    Hotfix_LT.Messenger.Raise<int,bool>(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,3,true);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, (firstController.ReplaceBtn.transform.GetChild(0).GetComponent<UILabel>().text == EB.Localizer.GetString("ID_EQUIP_TIPS_LOAD")) ? EB.Localizer.GetString("ID_EQUIP_DRESS_SUCCESS") : EB.Localizer.GetString("ID_EQUIP_REPLACE_SUCCESS"));
                    controller.Close();
                }
            });
        }
        public void OnGetOffBtnClick()
        {
            if (!LTPartnerEquipMainController.m_Open)
            {
                Hashtable table = Johny.HashtablePool.Claim();
                table["equipType"] = equipType;
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                GlobalMenuManager.Instance.Open("LTPartnerEquipmentUI", table);
                controller.Close();
                return;
            }

            if(LTPartnerEquipDataManager.Instance.isMaxEquipNum)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailApi_1124"));
                return;
            }

            if (LTPartnerEquipMainController.instance != null)
            {
                if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetView)
                {
                    LTPartnerEquipMainController.instance.OnClickEquipmentInfoItem();
                    controller.Close();
                    return;
                }

                if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetEditView)
                {
                    LTPartnerEquipmentInfoController.isChangeColor = true;
                    LTPartnerEquipMainController.instance.RemoveFromEquipmentInfoList((int)firstController.data.Type);
                    controller.Close();
                    return;
                }
            }

            LTPartnerEquipDataManager.Instance.RequireUnEquip(firstController.data.Eid, LTPartnerEquipMainController.CurrentPartnerData.HeroId, delegate (bool success) {
                if (success)
                {
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,3,true);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTEquipmentInforUIController_4512"));
                    LTPartnerEquipPartnerInfoController.isChangeColor = true;
                    controller.Close();
                }
            });
        }
    
        public void OnEquipLevelUpBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            controller.Close();

            if (!LTPartnerEquipMainController.m_Open)
            {
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                var ht = Johny.HashtablePool.Claim();
                ht.Add("partnerData", LTPartnerEquipMainController.CurrentPartnerData);
                ht.Add("equipType", equipType);
                ht.Add("equipId", firstController.data.Eid);
                GlobalMenuManager.Instance.Open("LTPartnerEquipmentUI", ht);
                return;
            }

            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.SynthesisView)
            {
                EquipmentInfoEven.SelectEquipIDBySyn(firstController.data.Eid);
                return;
            }
    
            EquipmentInfoEven.SelectEquipmentID(firstController.data.Eid);
        }

        public void OnEquipSynPutInOutBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            controller. Close();
            EquipmentInfoEven.SelectEquipmentID(firstController.data.Eid);
        }
    }
    
}
