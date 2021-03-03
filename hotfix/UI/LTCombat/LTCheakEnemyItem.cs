using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTCheakEnemyItem : DynamicCellController<int>
    {
        public UISprite IconSprite;
        public UISprite FrameSprite;
        public UISprite FrameBGSprite;
        public UISprite CharTypeSprite;
        public UILabel LevelLabel;
        public UILabel breakLebel;
        public LTPartnerStarController StarController;

        private Hotfix_LT.Data.MonsterInfoTemplate m_data;
        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            IconSprite = t.GetComponent<UISprite>("Icon");
            FrameSprite = t.GetComponent<UISprite>("Frame");
            FrameBGSprite = t.GetComponent<UISprite>("FrameBG");
            CharTypeSprite = t.GetComponent<UISprite>("Property");
            LevelLabel = t.GetComponent<UILabel>("LevelSprite/LabelLevel");
            breakLebel = t.GetComponent<UILabel>("Break");
            StarController = t.GetMonoILRComponent<LTPartnerStarController>("StarList");

            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => { EnemyInfoItemClick(); }));
        }

        public override void Clean()
        {
            SetItemData(null);
        }
    
        public override void Fill(int itemId)
        {
            Hotfix_LT.Data.MonsterInfoTemplate itemData = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(itemId);
            SetItemData(itemData);
        }
    
    
        private void SetItemData(Hotfix_LT.Data.MonsterInfoTemplate itemData)
        {
            m_data = CreateItemData(itemData);
            //睡梦之塔特殊处理
            if (BattleReadyHudController.sBattleType == eBattleType.SleepTower)
            {
                //得到当前时第几层 
                Hotfix_LT.Data.ClimingTowerTemplate datas = Hotfix_LT.Data.EventTemplateManager.Instance.GetClimingTowerData();
                if (datas!=null)
                {
                    float param = datas.param;
                    int layer = datas.layer;
                    if (m_data!=null && m_data.id!=0)
                    {
                        m_data.base_ATK = itemData.base_ATK*(1+param*layer);
                        m_data.base_DEF = itemData.base_DEF * (1 + param * layer);
                        m_data.base_MaxHP = itemData.base_MaxHP * (1 + param * layer);
                        m_data.speed = itemData.speed*(1 + param * layer);
                        m_data.CritP = itemData.CritP*(1 + param * layer);
                        m_data.CritV = itemData.CritV*(1 + param * layer);
                        m_data.SpExtra = itemData.SpExtra*(1 + param * layer);
                        m_data.SpRes = itemData.SpRes*(1 + param * layer);
                    }
                }
            }
            UpdateItem();
        }
    
    
        public Hotfix_LT.Data.MonsterInfoTemplate CreateItemData(Hotfix_LT.Data.MonsterInfoTemplate itemData)
        {
            if (itemData!=null)
            {
                Hotfix_LT.Data.MonsterInfoTemplate _item = new Hotfix_LT.Data.MonsterInfoTemplate()
                {
                    obj = itemData.obj
                };

                return _item;
            }
    
            return null;
        }
    
        private void UpdateItem()
        {
            if (m_data == null || m_data.id == 0)
            {
                mDMono.transform.gameObject.CustomSetActive(false);
                return;
            }
            mDMono.transform.gameObject.CustomSetActive(true);
            Hotfix_LT.Data.HeroInfoTemplate info=Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(m_data.character_id);
            IconSprite.spriteName = info.icon;
    
            int quality = 0;
            int addLevel = 0;
            LTPartnerDataManager.GetPartnerQuality(m_data.upgrade, out quality, out addLevel);
    
            FrameSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
            GameItemUtil.SetColorfulPartnerCellFrame(quality, FrameBGSprite);
    
            if (addLevel > 0)
            {
                breakLebel.gameObject.CustomSetActive(true);
                breakLebel.text = "+" + addLevel.ToString();
            }
            else
            {
                breakLebel.gameObject.CustomSetActive(false);
            }

            CharTypeSprite.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[info.char_type]; 
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, CharTypeSprite.transform, (PartnerGrade)info.role_grade, info.char_type);
            LevelLabel.text = m_data.level.ToString();
            
            StarController.SetSrarList(m_data.star,0);
        }
    
        public void EnemyInfoItemClick()
        {
            Vector2 screenPos = UICamera.lastEventPosition;
            var ht = Johny.HashtablePool.Claim();
            ht.Add("data", m_data);
            ht.Add("screenPos", screenPos);
            GlobalMenuManager.Instance.Open("LTEnemyTipUI", ht);
        }
    }
}
