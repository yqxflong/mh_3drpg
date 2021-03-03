using UnityEngine;

namespace Hotfix_LT.UI
{
	/// <summary>
	/// 展示的伙伴卡片信息
	/// </summary>
	public class LTPVPCardInfoHud
	{
        /// <summary>
        /// 卡片信息控件
        /// </summary>
        private CombatPartnerCellController m_Card; 
        /// <summary>
        /// 选择状态
        /// </summary>
        private GameObject m_SelectState;
		/// <summary>
		/// 本身对象
		/// </summary>
		private GameObject m_SelfObj;
		/// <summary>
		/// 当前数据
		/// </summary>
		private HeroBattleChoiceCellData m_Data;

		public LTPVPCardInfoHud(Transform transform)
		{
			m_SelfObj = transform.gameObject;
            m_Card = transform.Find("Item").GetMonoILRComponent<CombatPartnerCellController>();
            m_SelectState = transform.Find("Select").gameObject;
			UIEventListener.Get(transform.Find("Item/Icon").gameObject).onClick = OnClickCard;
		}

		/// <summary>
		/// 设置伙伴信息
		/// </summary>
		/// <param name="partnerData">伙伴数据</param>
		public void F_SetCardInfo(HeroBattleChoiceCellData partnerData)
		{
			m_Data = partnerData;
			bool isNull = partnerData == null;
			m_SelfObj.SetActive(!isNull);
			if (!isNull)
			{
				//获取指定的伙伴数据
				LTPartnerData data = LTPartnerDataManager.Instance.GetGeneralPartnerList().Find(p => p.HeroStat.id == partnerData.heroTplID);
				if (data != null)
				{
                    m_Card.Fill(data);
                    ////不要设置死亡情况
                    m_Card.DeathSprite.gameObject.SetActive(false);
                }
				else
				{
					EB.Debug.LogError("为什么相应的partnerData.heroTplID:{0},没有相应的伙伴数据？", partnerData.heroTplID);
				}
				//是否选中
				m_SelectState.SetActive(partnerData.isUsed);
				//特效是否显示
				bool fxVisible = !partnerData.isUsed;
				if (fxVisible && data.HeroInfo.role_grade != (int)PartnerGrade.SSR)
				{
					fxVisible = false;
				}
                for (int i = 0; i < m_Card.LevelSprite.transform.childCount; i++)
                {
                    m_Card.LevelSprite.transform.GetChild(i).gameObject.SetActive(fxVisible);
                }
            }
		}

		/// <summary>
		/// 克隆卡片对象
		/// </summary>
		/// <param name="parent">挂载的父级</param>
		/// <returns></returns>
		public LTPVPCardInfoHud F_Clone(Transform parent)
		{
			GameObject cloneObj = GameObject.Instantiate(m_SelfObj);
			cloneObj.transform.parent = parent;
			cloneObj.transform.localPosition = Vector3.zero;
			cloneObj.transform.localScale = Vector3.one;
			return new LTPVPCardInfoHud(cloneObj.transform);
		}

		/// <summary>
		/// 点击按钮
		/// </summary>
		/// <param name="go"></param>
		private void OnClickCard(GameObject go)
		{
			//判断当前的操作执行是否可以点击
			if (LTHeroBattleModel.GetInstance().choiceData.choiceState == 1
				&& LTHeroBattleModel.GetInstance().choiceData.openUid == LTHeroBattleModel.GetInstance().choiceData.selfInfo.uid
				&& !m_Data.isUsed)
			{
				FusionAudio.PostEvent("UI/General/ButtonClick", true);
				if (m_Data != null)
				{
					LTHeroBattleEvent.ChoiceHero(m_Data.heroTplID);
				}
			}
		}
	}
}