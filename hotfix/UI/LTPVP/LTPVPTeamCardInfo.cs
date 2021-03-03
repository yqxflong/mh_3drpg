using System;
using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 队伍阵型卡片信息
    /// </summary>
    public class LTPVPTeamCardInfo
    {
        /// <summary>
        /// 卡片信息控件
        /// </summary>
        private CombatPartnerCellController m_Card; 
        /// <summary>
        /// 选中状态
        /// </summary>
        private GameObject m_SelectObj;
        /// <summary>
        /// 锁定状态
        /// </summary>
        private GameObject m_LockState;
        /// <summary>
        /// 选中锁定状态
        /// </summary>
        private GameObject m_SelectLockState;
        /// <summary>
        /// 点击事件
        /// </summary>
        private Action<LTPVPTeamCardInfo> m_OnClickAction;
        /// <summary>
        /// 选择数据
        /// </summary>
        private HeroBattleChoiceCellData m_ChoiceData;
        /// <summary>
        /// 当前所有特效
        /// </summary>
        private Renderer[] m_Renderers;
        /// <summary>
        /// 当前所有特效
        /// </summary>
        private ParticleSystem[] m_ParticleSystems;
        /// <summary>
        /// 设置卡片时的特效
        /// </summary>
        private GameObject m_SetCardFx;

        public LTPVPTeamCardInfo(Transform transform, Action<LTPVPTeamCardInfo> onClickAction)
        {
            m_OnClickAction = onClickAction;
            m_Card = transform.Find("Item").GetMonoILRComponent<CombatPartnerCellController>();
            m_SelectObj = transform.Find("Select").gameObject;
            m_LockState = transform.Find("Lock").gameObject;
            UIEventListener.Get(transform.Find("Item/Icon").gameObject).onClick = OnClickBtn;
            m_SetCardFx = transform.Find("ClickFx").gameObject;
            m_SelectLockState = transform.Find("SelectLock").gameObject;
            m_Renderers = m_SelectObj.GetComponentsInChildren<Renderer>();
            m_ParticleSystems = m_SelectObj.GetComponentsInChildren<ParticleSystem>();
        }

        /// <summary>
        /// 设置伙伴信息
        /// </summary>
        /// <param name="choiceData">伙伴数据</param>
        /// <param name="playerInfo">玩家信息</param>
        /// <param name="isBan">是否被禁用</param>
        /// <param name="isNeedSetFx">是否需要设置特效</param>
        public void F_SetCardInfo(HeroBattleChoiceCellData choiceData, SidePlayerInfoData playerInfo, bool isBan, bool isNeedSetFx)
        {
            m_ChoiceData = choiceData;
            LTPartnerData data = null;
            m_LockState.CustomSetActive(isBan);
            m_SelectLockState.CustomSetActive(false);
            if (choiceData != null)
            {
                data = new LTPartnerData();
                data.StatId = 0;
                data.InfoId = choiceData.heroTplID;
                data.HeroStat = null;
                int characterId = choiceData.heroTplID;
                //判断当前的数据是否为机器人
                if (playerInfo.uid == 0)
                {
                    Hotfix_LT.Data.MonsterInfoTemplate monster = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(choiceData.heroTplID);
                    if (monster != null)
                    {
                        characterId = int.Parse(monster.character_id);
                    }
                    else
                    {
                       EB.Debug.LogError("为什么输入的choiceData.heroTplID:{0},没有相应的怪物数据" ,choiceData.heroTplID);
                    }
                }
                else
                {
                    characterId -= 1;
                }
                //模型皮肤
                data.HeroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterId, choiceData.skin);
                //不能为零
                data.mHeroId = 1;
                //修改这里来调整UI的表现
                data.IsHire = true;
                data.HireHeroId = 1;
                data.HireLevel = choiceData.level;
                data.HireAllRoundLevel = choiceData.peak;
                data.HireArtifactLevel = choiceData.artifactLevel;
                data.HireUpGradeId = choiceData.upGrade;
                data.HireStar = choiceData.star;
                //觉醒
                data.HireAwakeLevel = choiceData.isAwake;
                //设置卡片时的特效
                if (isNeedSetFx)
                {
                    EB.Coroutines.Run(SetCardFx());
                }
            }
            m_Card.Fill(data);
        }

        /// <summary>
        /// 设置卡片时的特效
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetCardFx()
        {
            m_SetCardFx.CustomSetActive(true);
            yield return new WaitForSeconds(2.0f);
            m_SetCardFx.CustomSetActive(false);
        }

        /// <summary>
        /// 设置高亮状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="selfTeam">自身队伍</param>
        public void F_SetHeightLight(bool state, bool selfTeam)
        {
            //判断是否有伙伴了~有的话调整渲染顺序
            if (m_Renderers != null)
            {
                for (int i = 0; i < m_Renderers.Length; i++)
                {
                    m_Renderers[i].sortingOrder = m_ChoiceData != null ? 0 : 100;
                }
            }
            if (m_ParticleSystems != null)
            {
                for (int i = 0; i < m_ParticleSystems.Length; i++)
                {
                    ParticleSystem.MainModule mainModule = m_ParticleSystems[i].main;
                    mainModule.startColor = new ParticleSystem.MinMaxGradient(selfTeam ? Color.white : new Color(1.0f, 0.43f, 0));
                }
            }
            m_SelectObj.CustomSetActive(state);
        }

        /// <summary>
        /// 设置锁定状态
        /// </summary>
        /// <param name="state">状态</param>
        public void F_SetLockState(bool state)
        {
            //m_LockState.CustomSetActive(state);
            m_SelectLockState.CustomSetActive(state);
            for (int i = 0; i < m_Card.LevelSprite.transform.childCount; i++)
            {
                m_Card.LevelSprite.transform.GetChild(i).gameObject.CustomSetActive(!state);
            }
        }

        /// <summary>
        /// 获取当前伙伴的数据
        /// </summary>
        /// <returns></returns>
        public HeroBattleChoiceCellData F_GetCurrentPartnerData()
        {
            return m_ChoiceData;
        }

        /// <summary>
        /// 点击按钮
        /// </summary>
        /// <param name="btn"></param>
        private void OnClickBtn(GameObject btn)
        {
            if (m_OnClickAction != null)
            {
                m_OnClickAction(this);
            }
        }
    }
}