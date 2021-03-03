using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class UIInventoryBagCellData
    {
        public string m_DataID;

        public UIInventoryBagCellData(string dataids)
        {
            m_DataID = dataids;
        }
    }

    public class UIInventoryBagCellController : DynamicCellController<UIInventoryBagCellData>
    {
        public UISprite Border;
        public UIMHBindableItemDataLookUp m_cell;

        public UIInventoryBagCellData Data { get; set; }

        private bool updataRedpoint = false;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Border = t.GetComponent<UISprite>("Sprite");
            m_cell = t.GetMonoILRComponentByClassPath<UIMHBindableItemDataLookUp>("Hotfix_LT.UI.UIMHBindableItemDataLookUp");

            var btn = t.GetComponentEx<UIButton>();
            btn.onClick.Add(new EventDelegate(OnthisItemClick));

            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                var showBagContent = ((GameObject)mDMono.ObjectParamList[0]).GetMonoILRComponent<ShowBagContent>();
                btn.onClick.Add(new EventDelegate(() => showBagContent.SetBagContent(this)));
            }
        }

        public void UpdateUI()
        {
            IDictionary itemdata = null;

            if (Data != null && !string.IsNullOrEmpty(Data.m_DataID))
            {
                DataLookupsCache.Instance.SearchDataByID<IDictionary>(Data.m_DataID, out itemdata);
            }

            if (updataRedpoint)
            {
                updataRedpoint = false;
                string id = EB.Dot.String("inventory_id", itemdata, "0");
                if (Data.m_DataID != null)
                {
                    LTMainHudManager.Instance.ItemsList.Add(id);
                }
            }

            m_cell.ItemData = itemdata;
        }

        

        public override void Fill(UIInventoryBagCellData itemData)
        {
            Data = itemData;
            UpdateUI();
        }

        public override void Clean()
        {
            //NONE
        }

        public void OnthisItemClick()
        {
            if (LTInventoryAllController._CurrentSelectCell != null)
            { 
                LTInventoryAllController._CurrentSelectCell.Border.gameObject.SetActive(false); 
            }

            LTInventoryAllController._CurrentSelectCell = this;
            LTInventoryAllController._CurrentSelectCellId = Data.m_DataID;
            updataRedpoint = true;
            UpdateUI();
            LTInventoryAllController._CurrentSelectCell.Border.gameObject.SetActive(true);
        }
    }
}
