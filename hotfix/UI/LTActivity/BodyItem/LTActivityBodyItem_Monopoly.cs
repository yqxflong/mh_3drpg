using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTActivityBodyItem_Monopoly:LTActivityBodyItem
    {
        public UILabel CountLabel;
        public UIGrid GiftGrid;
        public LTShowItem ItemTemplate;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            CountLabel = t.GetComponent<UILabel>("Num");
            GiftGrid = mDMono.transform.Find("GiftGrid").GetComponent<UIGrid>();
            ItemTemplate = mDMono.transform.Find("GiftGrid/0").GetMonoILRComponent<LTShowItem>();
            ItemTemplate.mDMono.gameObject.CustomSetActive(false);
        }

        public override void SetData(object data)
        {
            base.SetData(data);

            if (desc != null&& desc.text.Contains("{0}"))
            {
                int count = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("FreeNormalDice");
                desc.text = string.Format(desc.text, count);
            }

            CountLabel.text =  LTInstanceMapModel.Instance.GetMonopolyDiceTotleCount().ToString();
            int aid = EB.Dot.Integer("activity_id", data, 0);
            var activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(aid);
            if (!string.IsNullOrEmpty(activity.parameter1))
            {
                ArrayList aList = EB.JSON.Parse(activity.parameter1) as ArrayList;
                List<LTShowItemData> showItemsList = new List<LTShowItemData>();
                if (aList != null)
                {
                    for (int i = 0; i < aList.Count; i++)
                    {
                        string id = EB.Dot.String("data", aList[i], string.Empty);
                        int count = EB.Dot.Integer("quantity", aList[i], 0);
                        string type = EB.Dot.String("type", aList[i], string.Empty);
                        if (!string.IsNullOrEmpty(id))
                        {
                            LTShowItemData showItemData = new LTShowItemData(id, count, type);
                            showItemsList.Add(showItemData);
                        }
                    }
                }
                for (var i = 0; i < showItemsList.Count; ++i)
                {
                    var item = UIControllerHotfix.InstantiateEx(ItemTemplate, ItemTemplate.mDMono.transform.parent, (i + 1).ToString());
                    item.LTItemData = showItemsList[i];
                }

                GiftGrid.Reposition();
            }

            if (!state.Equals("running") || EB.Time.Now > fintime)
            {
                NavButton.gameObject.CustomSetActive(false);
                CountLabel.gameObject.CustomSetActive(false);
                return;
            }
            else
            {
                NavButton.gameObject.FindEx("RedPoint").CustomSetActive(LTMainHudManager.Instance.CheckEventRedPoint(data));
            }
        }

        protected override void OnNavClick()
        {
            if (AllianceUtil.GetIsInTransferDart(null))
            {
                return;
            }

            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION,2);
            System.Action act = new System.Action(delegate
            {
                LTMonopolyInstanceHudController.EnterInstance();
            });
            UIStack.Instance.ShowLoadingScreen(act, false, true, true);
        }

    }
}
