using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionPageMessage : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            template = mDMono.transform.Find("Scroll View/Container").GetMonoILRComponent<LegionMessageItem>();
            listMessageItem.Add(template);
            templatedynamic = mDMono.transform.Find("Scroll View/Container").GetComponent<Transform>();
            listItem.Add(templatedynamic);
        }



        public LegionMessageItem template;
        public Transform templatedynamic;
        public List<LegionMessageItem> listMessageItem  = new List<LegionMessageItem>();
        public List<Transform> listItem = new List<Transform>();

        public override void OnDestroy()
        {

        }

        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.CustomSetActive(isShow);
        }

        public void SetMessageData(List<MessageItemData> listData)
        {
            if (listData == null)
            {
                return;
            }

            LTUIUtil.SetNumTemplate(templatedynamic, listItem, listData.Count, 0);

            for (int i = 0; i < listMessageItem.Count; i++)
            {
                if (i > listData.Count - 1)
                {
                    listMessageItem[i].mDMono.gameObject.CustomSetActive(false);
                    continue;
                }
                listMessageItem[i].mDMono.gameObject.CustomSetActive(true);
                listMessageItem[i].titleDayLabel.text = string.Format("{0}.{1}.{2}", listData[i].year, listData[i].month, listData[i].day);
                for (int j = 0; j < listData[i].listCell.Count; j++)
                {
                    MessageCellData cell = listData[i].listCell[j];

                    if (j == 0)
                    {
                        listMessageItem[i].contentLabel.text = cell.content == null ? "" : cell.content;
                    }
                    else
                    {
                        if (cell.content==null)
                        {
                            listMessageItem[i].contentLabel.text += "";
                        }
                        else
                        {
                            listMessageItem[i].contentLabel.text += "\n" + cell.content;
                        }
                    }

                }

                if (i > 0)
                {
                    listMessageItem[i].mDMono.transform.localPosition = listMessageItem[i - 1].mDMono.transform.localPosition
                        - new Vector3(0f, listMessageItem[i - 1].GetHeight(), 0f);
                }
            }

        }

    }
}
