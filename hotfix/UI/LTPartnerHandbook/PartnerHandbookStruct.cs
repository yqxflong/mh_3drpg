using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 单个图鉴基础数据
    /// </summary>
    public class HandbookData
    {
        public Hotfix_LT.Data.eRoleAttr HandbookId;
        public int BreakLevel;
        public List<HandbookCardData> CardsInfo = new List<HandbookCardData>();
        public bool HasAvailableCard;

        public bool HasRedPoint = false;
    }

    public enum HandbookCardState
    {
        Lock = 0, Empty = 1, Filled = 2
    }

    public enum IHandBookAddAttrType
    {
        ATTR_CritP = 0, //暴击
        ATTR_CRIresist, //暴击抵抗
        ATTR_Speed, //速度加成
        ID_ATTR_DMGincrease, //伤害加深
        ID_ATTR_DMGreduction, //伤害减免
    }

    /// <summary>
    /// 图鉴内的伙伴数据
    /// </summary>
    public class HandbookCardData
    {
        public Hotfix_LT.Data.eRoleAttr handbookId;
        public IHandBookAddAttrType handBookAddAttrType;
        public HandbookCardState State;
        public int index;//图鉴位置
        public int UnlockLevel;
        public string BuddyId;
        public LTPartnerData PartnerData;
        public LTPartnerData SetHandBookCard()
        {
            if (!string.IsNullOrEmpty(BuddyId))
            {
                PartnerData = LTPartnerDataManager.Instance.GetPartnerByHeroId(int.Parse(BuddyId));
                return PartnerData;
            }
            return null;
        }
    }

    public class HandbookList : INodeData
    {
        public void OnUpdate(object obj)
        {
            CleanUp();
            Handbooks = Hotfix_LT.EBCore.Dot.List<HandbookData, int>(null, obj, Handbooks, Parse);
            LTPartnerHandbookManager.isAllHandBookOpen = isAllHandbookUnLock();
        }

        public void OnMerge(object obj)
        {
            Handbooks = Hotfix_LT.EBCore.Dot.List<HandbookData, int>(null, obj, Handbooks, Parse, (item, HandbookId) => (int)item.HandbookId == HandbookId);
        }
        public object Clone()
        {
            return new HandbookList();
        }

        public void CleanUp()
        {
            Handbooks.Clear();
            HandbookBunddysList.Clear();
        }


        public List<HandbookData> Handbooks = new List<HandbookData>();
        private List<string> HandbookBunddysList = new List<string>();
        
         public HandbookCardData Find(string BuddyId)
         {
             for (int i = 0; i < Handbooks.Count; ++i)
             {
                 for (int j = 0; j < Handbooks[i].CardsInfo.Count; ++j)
                 {
                     if (Handbooks[i].CardsInfo[j].BuddyId == BuddyId) return Handbooks[i].CardsInfo[j];
                 }
             }
             return null;
         }

         public HandbookData Find(Hotfix_LT.Data.eRoleAttr HandbookId)
         {
             HandbookData item = Handbooks.Where(m => m.HandbookId == HandbookId).FirstOrDefault();
             return item;
         }

         public void Remove(Hotfix_LT.Data.eRoleAttr HandbookId)
         {
             Handbooks.RemoveAll(m => m.HandbookId == HandbookId);
         }
        /*

         */
         private HandbookData Parse(object value, int HandbookId)
         {
             if (value == null) return null;
             HandbookData item = Find((Hotfix_LT.Data.eRoleAttr)HandbookId) ?? new HandbookData();

             int type = EB.Dot.Integer("type", value, 0);
             item.HandbookId = (Hotfix_LT.Data.eRoleAttr)type;

             item.BreakLevel = EB.Dot.Integer("break", value, item.BreakLevel);

             item.HasAvailableCard = false;
             ArrayList handBookCards = Hotfix_LT.EBCore.Dot.Array("buddyInfo", value, null);
             if (handBookCards != null)
             {
                 item.CardsInfo.Clear();
                 for (int i = 0; i < (LTPartnerConfig.MAX_HANDBOOKPAGE + 1)*5; ++i)
                 {
                    
                    HandbookCardData hbcData = new HandbookCardData();
                    hbcData.handbookId = (Hotfix_LT.Data.eRoleAttr)HandbookId;
                    hbcData.handBookAddAttrType = (IHandBookAddAttrType)(i%5);
                    hbcData.index = i;
                    int state = 0;
                    string buddyId = null;
                    if (i < handBookCards.Count)
                    {
                        IDictionary dic = handBookCards[i] as IDictionary;
                        state = EB.Dot.Integer("useStats", dic, 0);
                        buddyId = EB.Dot.String("id", dic, null);
                    }
                    hbcData.State = GetCardState(item.BreakLevel, i, (HandbookCardState)state, out hbcData.UnlockLevel);
                    if (hbcData.State == HandbookCardState.Empty)
                    {
                        item.HasAvailableCard = true;
                    }                  
                    if (buddyId != null)
                    {
                        hbcData.BuddyId = buddyId;
                        if (!HandbookBunddysList.Contains(buddyId)) HandbookBunddysList.Add(buddyId);
                    }

                    item.CardsInfo.Add(hbcData);
                }
             }
             return item;
         }

         private HandbookCardState GetCardState(int BreakLevel, int index, HandbookCardState cardState, out int UnLockLevel)
        {
            UnLockLevel = 0;
            if (cardState != HandbookCardState.Filled)
             {
                 string unLockLevelKey = "mannual_unlock_" + index + "_level";
                 float unLockLevel = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue(unLockLevelKey);
                 if (BreakLevel >= unLockLevel)
                 {
                     cardState = HandbookCardState.Empty;
                 }
                 else
                 {
                     cardState = HandbookCardState.Lock;
                 }
                 UnLockLevel = (int)unLockLevel;
             }
             return cardState;
         }

         public List<LTPartnerData> GetHandbookPartners(Hotfix_LT.Data.eRoleAttr handBookType, List<LTPartnerData> allPartners)
         {
             List<LTPartnerData> targetList = new List<LTPartnerData>();
             for (int i = 0; i < allPartners.Count; i++)
             {
                 if (allPartners[i].HeroInfo.char_type == handBookType)
                 {
                     if (allPartners[i].HeroId > 0)
                     {
                         targetList.Add(allPartners[i]);
                     }
                 }
             }
             return targetList;
         }

         public bool IsPartnerInField(string heroId)
         {
             return HandbookBunddysList.Contains(heroId);
         }

         public bool isAllHandbookUnLock()
        {
            bool isunlock = true;
            for (int i = 0; i < Handbooks.Count; i++)
            {
                if (Handbooks[i].BreakLevel < LTPartnerHandbookManager.GetAllHandboolUnlockLevel(2))
                {
                    isunlock = false;
                    return isunlock;
                }
            }
            return isunlock;
        }
    }
}