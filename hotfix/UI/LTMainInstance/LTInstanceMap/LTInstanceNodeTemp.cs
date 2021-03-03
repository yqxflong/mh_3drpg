using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTInstanceNodeTemp : DynamicMonoHotfix
    {
        public int Num = 0;
        public LTInstanceNode nodeData;
        public LTInstanceMapCtrl MapCtrl;
    
        public virtual void SetData(int num, LTInstanceNode data)
        {
            Num = num;
            nodeData = data;
        }
    
        public virtual void UpdateData(LTInstanceNode data)
        {
            nodeData = data;
        }
    
        public virtual void Reset() { }

        public virtual void OnFloorClick() { }

        public virtual void OnFloorAni() { }

        #region About Door
        public virtual void OpenTheDoor(){}
        #endregion
    }
    
    public class LTInstanceSmallMapFloorTemp : LTInstanceNodeTemp
    {
        public override void Awake()
        {
            base.Awake();
            Terra = mDMono.transform .GetComponent<UISprite>("Terra");
            Role = mDMono.transform.GetComponent<UISprite>("Role");
            Player = mDMono.transform.Find("Player").gameObject;
        }

        public UISprite Terra;

        public UISprite Role;

        public GameObject Player;

        public override void SetData(int num, LTInstanceNode data)
        {
            base.SetData(num, data);
            SetTexture();
        }

        public override void UpdateData(LTInstanceNode data)
        {
            base.UpdateData(data);
            SetTexture();
        }

        private void SetTexture()
        {
            if (nodeData == null || LTInstanceMapModel.Instance == null || LTInstanceMapModel.Instance.CurNode == null || Role == null || Terra == null)
            {
                return;
            }

            bool isSight = nodeData.IsSight || LTInstanceMapModel.Instance.IsAlwayShowRole(nodeData.RoleData.Id);
            bool isMonster = !string.IsNullOrEmpty(nodeData.RoleData.Model);
            bool isBoss = nodeData.RoleData.CampaignData.CampaignId > 0 ? Hotfix_LT.Data.SceneTemplateManager.Instance.IsCampaignBoss(nodeData.RoleData.CampaignData.CampaignId.ToString()) : false;
            bool isStand = LTInstanceMapModel.Instance.CurNode.x == nodeData.x && LTInstanceMapModel.Instance.CurNode.y == nodeData.y;

            Player.CustomSetActive(false);
            if (!nodeData.CanPass)
            {
                Terra.color = Color.black;
            }
            else if (isStand)
            {
                Player.CustomSetActive(true);
                Terra.color = new Color(191f / 255f, 246f / 255f, 244f / 255f);
            }
            else if (nodeData.IsSight)
            {
                Terra.color = new Color(84f / 255f, 160f / 255f, 213f / 255f);
            }
            else
            {
                Terra.color = new Color(97f / 255f, 85f / 255f, 86f / 255f);
            }

            if (isSight)
            {
                Role.spriteName = nodeData.RoleData.Img;
            }
            else
            {
                Role.spriteName = string.Empty;
            }

            if (isSight && nodeData.CanPass)
            {
                if (nodeData.RoleData.Img.Contains("#"))
                {
                    Role.spriteName = nodeData.RoleData.Img.Split('#')[1];
                }
            }

            if (isMonster&&(isSight || LTInstanceMapModel.Instance.IsHunter()))
            {
                Role.spriteName = string.Empty;
                if (nodeData.RoleData.CampaignData.Star < 3)
                {
                    Role.spriteName = isBoss ? "Copy_Icon_Boss" : "Copy_Icon_Xiaoguai";
                }
                else
                {
                    Role.spriteName = isBoss ? "Copy_Icon_Bosstongguan" : "Copy_Icon_Xiaoguaitongguan";
                }
            }

            if (nodeData.RoleData.CampaignData.IsDoorOpen && !LTInstanceMapModel.Instance.IsAlwayShowRole(nodeData.RoleData.Id))
            {
                if(nodeData.RoleData.Id!=34) Role.spriteName = string.Empty;
            }

            if (nodeData.RoleData.Id > 0 && nodeData.RoleData.Type == 1)
            {
                Role.spriteName = string.Empty;
            }
        }
    }

    public class LTInstanceSmallMapWallTemp : LTInstanceNodeTemp
    {
        public override void Awake()
        {
            base.Awake();
            Terra = mDMono.transform.GetComponent<UISprite>("Terra");
        }


        public UISprite Terra;

        public override void SetData(int num, LTInstanceNode data)
        {
            base.SetData(num, data);
        }

        public override void UpdateData(LTInstanceNode data)
        {
            base.UpdateData(data);
        }
    }
}
