using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ShowGetNewItem : DynamicCellController<LTShowItemData>
    {
        public LTShowItemData mData;

        public LTShowItem m_Item;
        public NewDrawCardItem m_NewItem;

        public override void Awake()
        {
            base.Awake();

            Transform t = mDMono.transform;
            m_Item = t.GetMonoILRComponent<LTShowItem>("Temp/GeneralItem");
            m_NewItem = t.GetMonoILRComponent<NewDrawCardItem>("Temp/HeroItem");
        }

        public override void Clean()
        {
            mData = null;
            mDMono.gameObject.CustomSetActive(false);
        }

        public override void Fill(LTShowItemData itemData)
        {
			//EB.Debug.Log(mDMono.transform.name + ".say: Item Fill!");

			if (itemData == null)
            {
	            EB.Debug.LogWarning(mDMono.transform.name + ".say: item data is null!");
				mDMono.gameObject.CustomSetActive(false);
                return;
            }
            else
            {
	            //EB.Debug.Log(mDMono.transform.name + ".say: id = " + itemData.id);
				mData = new LTShowItemData(itemData.id, itemData.count, itemData.type, false);
                mDMono.gameObject.CustomSetActive(true);
            }
            m_Item.mDMono.gameObject.CustomSetActive(!LTGetItemUIController.m_isHC);
            m_NewItem.mDMono.gameObject.CustomSetActive(LTGetItemUIController.m_isHC);

            if (LTGetItemUIController.m_isHC)
            {
                int cid = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(mData.id).character_id;
                Hotfix_LT.Data.HeroInfoTemplate temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(cid);
                m_NewItem.Fill(temp);
            }
            else
            {
                m_Item.LTItemData = new LTShowItemData(itemData.id, itemData.count, itemData.type, false); 
            }
        }

        public void ProDeal()
        {
            if (LTGetItemUIController.m_isHC)
            {

            }
            else
            {
                m_Item.mDMono.transform.localScale = (mData.type == LTShowItemType.TYPE_HERO) ? new Vector3(1.1f, 1.15f, 1f) : new Vector3(1, 1, 1);
                m_Item.Count.color = new Color(1, 1, 1);
                m_Item.Name.applyGradient = false;
            }
        }
    }
}
