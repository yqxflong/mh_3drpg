using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTEquipmentWishController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen() { return false; }

        private GameObject _itemTemplate;
        private UILabel _title;
        private UIGrid _uiGrid;      
        private bool _isInit = false;

        public static readonly int equipmentWishActivityId = 6518;   // 套装许愿活动id：6518
        public System.Action<int> callback;

        public override void Awake()
        {
            base.Awake();
            controller.backButton = controller.transform.GetComponent<UIButton>("BG/Top/CloseBtn");      
        }

        public void SetTitle(string text)
        {
            if (_title == null)
            {
                _title = controller.transform.GetComponent<UILabel>("BG/Top/Title");
            }

            if (_title != null)
            {
                _title.text = text;
            }
        }

        public void InitData()
        {
            LTEquipmentWishItem selectedItem = null;

            // 显示周一到周五的掉落奖励
            for (var i = 1; i <= 5; i++)
            {
                Data.LostChallengeRewardTemplate temp = Data.SceneTemplateManager.Instance.GetLostChallengeReward((System.DayOfWeek)i, Data.SceneTemplateManager.Instance.LostChallengeRewardMaxFloor);

                if (temp == null || temp.DropList == null)
                {
                    continue;
                }

                for (int j = 0; j < temp.DropList.Count; j++)
                {
                    int id;
                    
                    if (int.TryParse(temp.DropList[j], out id))
                    {
                        int suit = id % 100;
                        Data.SuitTypeInfo suitInfo = Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(suit);

                        if (suitInfo != null)
                        {
                            var go = GameObject.Instantiate<GameObject>(_itemTemplate, _uiGrid.transform);
                            var equipmentWishItem = go.GetMonoILRComponent<LTEquipmentWishItem>();
                            equipmentWishItem.SetIcon(suitInfo.SuitIcon);
                            equipmentWishItem.SetName(string.Format("{0}{1}",suitInfo.TypeName, EB.Localizer.GetString("ID_SUIT")));
                            equipmentWishItem.ItemId = id;

                            if (suitInfo.SuitAttr2 != 0)
                            {
                                Data.SkillTemplate suitAttr = Data.SkillTemplateManager.Instance.GetTemplate(suitInfo.SuitAttr2);//套装2
                                equipmentWishItem.SetDesc(string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentSuitInfoItem_1348"), suitAttr.Description));
                            }
                            else if (suitInfo.SuitAttr4 != 0)
                            {
                                Data.SkillTemplate suitAttr = Data.SkillTemplateManager.Instance.GetTemplate(suitInfo.SuitAttr4);//套装4
                                equipmentWishItem.SetDesc(string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentSuitInfoItem_1621"), suitAttr.Description));
                            }

                            go.SetActive(true);
                            _uiGrid.Reposition();

                            int itemId;

                            if (DataLookupsCache.Instance.SearchIntByID(string.Format("tl_acs.{0}.current", equipmentWishActivityId), out itemId) && id == itemId)
                            {
                                selectedItem = equipmentWishItem;
                            }
                        }
                    }
                }
            }

            _isInit = true;
            
            if (selectedItem != null)
            {
                var toggle = selectedItem.mDMono.transform.GetComponent<UIToggle>();

                if (toggle != null)
                {
                    toggle.startsActive = true;
                }
            }
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            if (param != null)
            {
                callback = null;
                callback = (System.Action<int>)param;
            }

            if (_isInit)
            {
                return;
            }

            var t = controller.transform;
            _uiGrid = t.GetComponent<UIGrid>("Scroll View/Grid");
            _itemTemplate = t.gameObject.FindEx("Scroll View/Grid/Item");

            SetTitle(EB.Localizer.GetString("ID_EQUIPMENT_WISH_SELECT"));

            if (_uiGrid != null && _itemTemplate != null)
            {
                InitData();
            }
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            yield return null;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            yield return base.OnRemoveFromStack();
            DestroySelf();
        }
    }
}
