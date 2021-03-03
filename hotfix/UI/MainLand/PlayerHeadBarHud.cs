using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;


namespace Hotfix_LT.UI
{
    public enum ePlayerType
    {
        None,
        LocalPlayer,
        OtherPlayer,
        EnemyPlayer
    }

    public class PlayerHeadBarHud : HeadBarHud
    {
        public Color m_LocalColor;//绿色
        public Color m_OtherColor;//白色
        public Color m_NpcColor;//黄色
        public Color m_EnemyColor;//红色

        public GameObject m_RootObj;
        public GameObject m_IconRootObj;

        public UILabel m_DisplayName;

        private UISprite _attrSprite;
        private UISprite _promotionSprite;
        private DynamicUISprite _promotionDynamicSprite;
        private UITable _uiTable;

        public override void Awake() 
        {
            base.Awake();
            m_LocalColor = new Color32(38, 253, 1, 255);
            m_OtherColor = new Color32(255, 255, 255, 255);
            m_NpcColor = new Color32(255, 210, 0, 255);
            m_EnemyColor = new Color32(255, 0, 0, 255);
            mHeadBarHUDMonitor.m_Offset = new Vector2(0, 40);
            
            var t = mDMono.transform;
            m_RootObj = t.Find("Root").gameObject;
            m_IconRootObj = t.Find("Root/IconRoot").gameObject;

            m_DisplayName = t.GetComponent<UILabel>("Root/LabelRoot/NameLabel");

            _uiTable = t.GetComponent<UITable>("Root/LabelRoot");
            _attrSprite = t.GetComponent<UISprite>("Root/IconRoot/AttributeIcon");
            _promotionSprite = t.GetComponent<UISprite>("Root/LabelRoot/PromotionIcon");
            _promotionDynamicSprite = t.GetComponent<DynamicUISprite>("Root/LabelRoot/PromotionDynamicIcon");
            DeactiveIcon();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"> {Name:xxx,TeamType:eTeamId  ,PlayerType:ePlayerType(只有玩家) ,Role:xxx（只有NPC）}  </param>
        /// <param name="state">false 不显示  true显示</param>
        public override void SetBarState(Hashtable data, bool state)
        {
            ///设置数据
            if (!state)
            {
                m_RootObj.CustomSetActive(false);
                DeactiveIcon();
            }
            else
            {
                m_IconRootObj.CustomSetActive(false);
                string name = DataLocalizationLookup.Localize(EB.Dot.String("Name", data, ""), DataLocalizationLookup.eLOCALTYPE.Normal);
                eTeamId team_type = Hotfix_LT.EBCore.Dot.Enum<eTeamId>("TeamType", data, eTeamId.Enemy);
                ePlayerType player_type = Hotfix_LT.EBCore.Dot.Enum<ePlayerType>("PlayerType", data, ePlayerType.None);   //redName need EnemyPlayer
                bool isRedName = EB.Dot.Bool("RedName", data, false);
                string role = EB.Dot.String("Role", data, "");
                int attr = EB.Dot.Integer("Attr", data, 0);

                if (attr == 0)
                {
                    _attrSprite.gameObject.CustomSetActive(false);
                }
                else
                {
                    _attrSprite.gameObject.CustomSetActive(true);
                    _attrSprite.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[(eRoleAttr)attr];
                }

                if (team_type == eTeamId.Player)
                {
                    if (isRedName)
                    {
                        m_DisplayName.color = m_EnemyColor;
                    }
                    else
                    {
                        if (player_type == ePlayerType.LocalPlayer)
                        {
                            m_DisplayName.color = m_LocalColor;
                            m_DisplayName.depth = 2;
                            m_DisplayName.transform.GetChild(0).GetComponent<UILabel>().depth = 1;
                        }
                        else if (player_type == ePlayerType.EnemyPlayer)
                        {
                            m_DisplayName.color = m_EnemyColor;
                        }
                        else
                        {
                            m_DisplayName.color = m_OtherColor;
                        }
                    }
                    
                    int Promoid = EB.Dot.Integer("Promoid", data, 0);

                    if (Promoid > 0)
                    {
                        if(player_type == ePlayerType.LocalPlayer)
                        {
                            var temp = LTPromotionManager.Instance.GetPromotion();  

                            if (temp != null) 
                            {
                                SetIcon(temp.smallIcon, false);
                            }
                            else
                            {
                                DeactiveIcon();
                            }
                        }
                        else
                        {
                            var temp = LTPromotionManager.Instance.GetOtherPromotion(Promoid);

                            if (temp != null)
                            {
                                SetIcon(temp.smallIcon, false);
                            }
                            else
                            {
                                DeactiveIcon();
                            }
                        }
                    }

                    if (player_type == ePlayerType.LocalPlayer && PlayerHeadBarHudController.Instance != null)
                    {
                        PlayerHeadBarHudController.Instance.vigourTip.SetVigourPos(mDMono.transform);
                    }
                }
                else
                {
                    switch (role)
                    {
                        case NPC_ROLE.FUNC:
                            m_DisplayName.color = m_NpcColor;
                            break;
                        case NPC_ROLE.GHOST:
                            m_DisplayName.color = m_EnemyColor;
                            break;
                        case NPC_ROLE.GUARD:
                            m_DisplayName.color = m_NpcColor;
                            break;
                        case NPC_ROLE.WORLD_BOSS:
                            m_DisplayName.color = m_EnemyColor;
                            break;
                        case NPC_ROLE.HANTED:
                            m_DisplayName.color = m_EnemyColor;
                            break;
                        case NPC_ROLE.CAMPAIGN_ENEMY:
                            m_DisplayName.color = m_EnemyColor;
                            break;
                        case NPC_ROLE.BEAT_MONSTER:
                            m_DisplayName.color = m_EnemyColor;
                            break;
                        case NPC_ROLE.ALLIANCE_CAMPAIGN_BOSS:
                        case NPC_ROLE.ALLIANCE_CAMPAIGN_ENEMY:
                            m_DisplayName.color = m_EnemyColor;
                            break;
                        case NPC_ROLE.ARENA_MODLE:
                            m_DisplayName.color = m_OtherColor;
                            break;
                        default:
                            m_DisplayName.color = m_NpcColor;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    m_DisplayName.text = name;
                }
                
                mDMono.transform.localScale = Vector3.one;
                m_IconRootObj.CustomSetActive(true);
                m_RootObj.CustomSetActive(true);
            }
        }

        public override void OnEnable() {
            base.OnEnable();
            _uiTable.Reposition();
        }

        public void DeactiveIcon() {
            _promotionSprite.gameObject.CustomSetActive(false);
            _promotionDynamicSprite.gameObject.CustomSetActive(false);
        }

        public void SetIcon(string spriteName, bool isFromDynamicAtlas = true) 
        {
            _promotionSprite.gameObject.CustomSetActive(!isFromDynamicAtlas);
            _promotionDynamicSprite.gameObject.CustomSetActive(isFromDynamicAtlas);

            if (!isFromDynamicAtlas) {
                _promotionSprite.spriteName = spriteName;
                _promotionSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
                _promotionSprite.MakePixelPerfect();
            } else {
                _promotionDynamicSprite.spriteName = spriteName;
                _promotionDynamicSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
                _promotionDynamicSprite.MakePixelPerfect();
            }

            _uiTable.Reposition();
        }
    }
}
