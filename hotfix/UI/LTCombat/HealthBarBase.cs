using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
	public abstract class HealthBarBase : DynamicMonoHotfix
	{
		public BuffIconItem[] BuffIconItems;

		protected ICollection<Hotfix_LT.Combat.CombatCharacterSyncData.BuffData> mBuffDatas;
		protected bool mBuffAlternate;

		private int visibleBuffCount;

		public override void OnEnable()
		{
			//base.OnEnable();
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public virtual void UpdateIndicator(int count) { }
		public void UpdateBuff(ICollection<Hotfix_LT.Combat.CombatCharacterSyncData.BuffData> buffDatas)
		{
			mBuffDatas = buffDatas;
			visibleBuffCount = 0;
			bool haveindicator = false;
			foreach (var data in mBuffDatas)
			{
				Hotfix_LT.Data.BuffTemplate templateData = Hotfix_LT.Data.BuffTemplateManager.Instance.GetTemplate(data.Id);
				if (!templateData.Invisible)
				{
					visibleBuffCount++;
				}
				if (templateData.Type == Hotfix_LT.Data.BuffType.Indicator)
				{
					haveindicator = true;
					UpdateIndicator(data.LeftTurnArray.Length);
				}
			}
			if (!haveindicator)
			{
				UpdateIndicator(0);
			}
			if (visibleBuffCount <= BuffIconItems.Length)
			{
				mBuffAlternate = false;
				UpdateBuffIcon();
			}
		}

		public void Update()
		{
			if (visibleBuffCount > BuffIconItems.Length)
			{
				bool alter = System.DateTime.Now.Second % 6 < 3;
				if (alter != mBuffAlternate)
				{
					mBuffAlternate = alter;
					UpdateBuffIcon();
				}
			}
		}

		private void UpdateBuffIcon()
		{
			int IconPerPage = BuffIconItems.Length;
			int buffStartDataIndex = mBuffAlternate ? IconPerPage : 0;

			//for (int i = 0; i < 4; ++i)
			for (var i = 0; i < BuffIconItems.Length; i++)
			{
				BuffIconItems[i].RootNode.CustomSetActive(false);
			}
			int index = 0;
			int buffcount = 0;
			foreach (var data in mBuffDatas)
			{
				Hotfix_LT.Combat.CombatCharacterSyncData.BuffData buffData = data;
				Hotfix_LT.Data.BuffTemplate templateData = Hotfix_LT.Data.BuffTemplateManager.Instance.GetTemplate(buffData.Id);
				if (!templateData.Invisible)
				{
					index++;
				}

				if (!templateData.Invisible && index > buffStartDataIndex)
				{
					string buffIcon = templateData.Buff;
					if (string.IsNullOrEmpty(templateData.Buff))
					{
						EB.Debug.LogError("buff icon is null for buffid:{0}" , templateData.ID);
					}

					BuffIconItems[buffcount].RootNode.gameObject.name = buffData.Id.ToString();
					BuffIconItems[buffcount].IconSprite.spriteName = buffIcon;
					BuffIconItems[buffcount].LeftTurnLabel.text = buffData.GetMaxTurnStr();
					BuffIconItems[buffcount].OverlyingLabel.text = buffData.GetOverlying(templateData.StackNum);
					BuffIconItems[buffcount].RootNode.CustomSetActive(true);
					buffcount++;

					if (buffcount >= BuffIconItems.Length)
					{
						return;
					}
				}
			}
		}
	}
}