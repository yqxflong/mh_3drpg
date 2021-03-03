using UnityEngine;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    public class LTUIUtil
    {
        public static Color NormalColor = new Color(1, 1, 1, 1);
        public static Color DisenableColor = new Color(1, 0, 1, 1);

        //[System.Obsolete("请自己找出所有子Label再传过来！！")]
        public static void SetText(UILabel label, string str)
        {
            if (label == null) { return; }
            UILabel[] labelArray = label.transform.GetComponentsInChildren<UILabel>();
            SetText(labelArray, str);
        }

        public static void SetText(UILabel[] labelArray, string str){
            for (int i = 0; i < labelArray.Length; i++)
            {
                labelArray[i].text = str;
            }
        }

        public static void SetLevelText(UISprite uiSprite,UILabel label,LTPartnerData partnerData)
        {
            string spriteName = "";
            string level = "";
           
            if (partnerData != null && partnerData.Level >= LTPartnerDataManager.Instance.GetPeakOpenLevel()
            && partnerData.AllRoundLevel>0)
            {
                level = partnerData.AllRoundLevel.ToString();
                spriteName = "Ty_Quality_Dianfeng_Di";
            }
            else
            {
                level = partnerData.Level.ToString();
                spriteName = "Ty_Icon_Grade";
            }

            uiSprite.spriteName = spriteName;
            // label.text = level;
            SetText(label,level);
        }

        public static void SetLevelText(UISprite uiSprite,UILabel label, int level)
        {
            string spriteName = "";
           
            if (level>LTPartnerDataManager.Instance.GetPeakOpenLevel())
            {
                spriteName = "Ty_Quality_Dianfeng_Di";
                level = level - LTPartnerDataManager.Instance.GetPeakOpenLevel();
            }
            else
            {
                spriteName = "Ty_Icon_Grade";
            }
            uiSprite.spriteName = spriteName;
            // label.text = level.ToString();
            SetText(label,level.ToString());
        }

        /// <summary>
        /// 设置模板长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="list"></param>
        /// <param name="num"></param>
        /// <param name="behind"></param>
        /// <param name="isVertical">是否垂直布局 false就水平</param>
        public static void SetNumTemplate<T>(T template, List<T> list, int num, int behind, bool isVertical = true) where T : Component
        {
            if (num > list.Count)
            {
                for (int i = list.Count; i < num; i++)
                {
                    GameObject go = GameObject.Instantiate(template.gameObject);
                    go.transform.parent = template.transform.parent;
                    go.transform.localScale = template.transform.localScale;
                    go.transform.localPosition = template.transform.localPosition + new Vector3(isVertical ? 0 : behind * i, isVertical ? -behind * i : 0f, 0f);
                    list.Add(go.GetComponent<T>());
                }
            }
            for (int i = 0; i < num; i++)
            {
                list[i].gameObject.SetActive(true);
            }
            for (int i = num; i < list.Count; i++)
            {
                list[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 设置模板长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="list"></param>
        /// <param name="num"></param>
        /// <param name="behind"></param>
        /// <param name="isVertical">是否垂直布局 false就水平</param>
        public static void SetNumTemplateFromMonoILR<T>(T template, List<T> list, int num, int behind, bool isVertical = true) where T : DynamicMonoHotfix
        {
            if (num > list.Count)
            {
                for (int i = list.Count; i < num; i++)
                {
                    GameObject go = GameObject.Instantiate(template.mDMono.gameObject);
                    go.transform.parent = template.mDMono.transform.parent;
                    go.transform.localScale = template.mDMono.transform.localScale;
                    go.transform.localPosition = template.mDMono.transform.localPosition + new Vector3(isVertical ? 0 : behind * i, isVertical ? -behind * i : 0f, 0f);
                    list.Add(go.GetMonoILRComponent<T>());
                }
            }
            for (int i = 0; i < num; i++)
            {
                list[i].mDMono.gameObject.SetActive(true);
            }
            for (int i = num; i < list.Count; i++)
            {
                list[i].mDMono.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 设置按钮不可用就变灰 及恢复
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="isEnable"></param>
        public static void SetGreyButtonEnable(UIButton btn, bool isEnable)
        {
            UISprite sprite = btn.GetComponent<UISprite>();
            btn.enabled = isEnable; //必须先关闭btn再改变颜色		
            var col = btn.GetComponent<BoxCollider>();
            if (col != null)
            {
                col.enabled = isEnable;
            }
            if (isEnable)
            {
                sprite.color = NormalColor;
            }
            else
            {
                sprite.color = DisenableColor;
            }
        }

        /// <summary>
        /// 设置ConsecutiveClickCoolTrigger类sprite在父节点上 按钮不可用就变灰 及恢复
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="isEnable"></param>
        public static void SetGreyButtonEnable(ConsecutiveClickCoolTrigger btn, bool isEnable)
        {
            UISprite sprite = btn.GetComponent<UISprite>();
            btn.enabled = isEnable; //必须先关闭btn再改变颜色		
            var col = btn.GetComponent<BoxCollider>();
            if (col != null)
            {
                col.enabled = isEnable;
            }
            if(sprite == null)
            {
                return;
            }
            if (isEnable)
            {
                sprite.color = NormalColor;
            }
            else
            {
                sprite.color = DisenableColor;
            }
        }

        /// <summary>
        /// 设置ConsecutiveClickCoolTrigger类sprite不在父节点上 按钮不可用就变灰 及恢复
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="isEnable"></param>
        public static void SetGreyButtonEnable(ConsecutiveClickCoolTrigger btn,UISprite sprite, bool isEnable)
        {
            btn.enabled = isEnable; //必须先关闭btn再改变颜色		
            var col = btn.GetComponent<BoxCollider>();
            if (col != null)
            {
                col.enabled = isEnable;
            }
            if (sprite == null)
            {
                return;
            }
            if (isEnable)
            {
                sprite.color = NormalColor;
            }
            else
            {
                sprite.color = DisenableColor;
            }
        }
        /// <summary>
        /// RGBA
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Color GetColorByHexadecimal(string str)
        {

            UInt32 num = Convert.ToUInt32(str);
            Color color = new Color(((int)num >> 24 & 0xff) / 255f, ((int)num >> 16 & 0xff) / 255f, ((int)num >> 8 & 0xff) / 255f, ((int)num & 0xff) / 255f);
            return color;
        }

        public static void AddBlackOutLineToLabel(UILabel label, string text)
        {
            UILabel parent = label;
            if (parent.transform.childCount <= 0)
            {
                UnityEngine.Object.Instantiate(parent.gameObject, parent.transform);
            }
            UILabel son = parent.transform.GetChild(0).GetComponent<UILabel>();
            son.depth = parent.depth - 1;
            son.color = Color.black;
            Vector3 Position = parent.transform.localPosition;
            Position.y = -parent.fontSize * 0.1f;
            Position.x = 0;
            Position.z = 0;
            son.transform.localPosition = Position;
            parent.text = text;
            son.text = text;
        }

        public static int[] ToInt(string[] sArray)
        {
            int[] iArray = new int[8];
            for (int i = 0; i < sArray.Length; i++)
            {
                iArray[i] = Convert.ToInt32(sArray[i]);
            }
            return iArray;
        }

        public static float[] ToFloat(string[] sArray)
        {
            float[] fArray = new float[sArray.Length];
            for (int i = 0; i < sArray.Length; i++)
            {
                fArray[i] = Convert.ToSingle(sArray[i]);
            }
            return fArray;
        }

        public static List<LTShowItemData> GetLTShowItemDataFromStr(string args,bool coloring=true)
        {
            List<LTShowItemData> temp = new List<LTShowItemData>();
            
            string[] rewardStrs = args.Split(';');
            for (int j = 0; j < rewardStrs.Length; j++)
            {
                string[] rewardSingleStrs = rewardStrs[j].Split(',');
                if (rewardSingleStrs.Length < 3)
                {
                    EB.Debug.LogError("RewardSingleStrs Length Less Than 3. Error!!!");
                    return null;
                }
                LTShowItemData data1 = new LTShowItemData(rewardSingleStrs[0], int.Parse(rewardSingleStrs[1]), rewardSingleStrs[2], coloring);
                temp.Add(data1);
            }

            return temp;
        }
        

    }
}