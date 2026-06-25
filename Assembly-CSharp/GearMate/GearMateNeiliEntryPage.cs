using System;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;

namespace GearMate
{
	// Token: 0x02000619 RID: 1561
	public class GearMateNeiliEntryPage : GearMateBaseEntryPage
	{
		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06004974 RID: 18804 RVA: 0x00225F10 File Offset: 0x00224110
		public override bool IsLeaf
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004975 RID: 18805 RVA: 0x00225F14 File Offset: 0x00224114
		protected override void InitInternal()
		{
			base.InitInternal();
			CToggleObsolete activeToggle = this.SubTogGroup.GetActive();
			int activePageCount = 0;
			int activePageIndex = 0;
			for (int i = 0; i < this._subPages.Count; i++)
			{
				GearMateSubPageBase subPage = this._subPages[i];
				bool activeSelf = subPage.gameObject.activeSelf;
				if (activeSelf)
				{
					activePageCount++;
					activePageIndex = i;
				}
			}
			bool flag = activeToggle == null || activePageCount != 1 || activeToggle.Key != activePageIndex;
			if (flag)
			{
				this.SubTogGroup.Set(0, true, true);
				base.SwitchToPage(0);
			}
		}

		// Token: 0x06004976 RID: 18806 RVA: 0x00225FBA File Offset: 0x002241BA
		public override void OnGearMateDataChanged()
		{
			this._subPages[this.CurrIndex].OnGearMateDataChanged();
		}

		// Token: 0x06004977 RID: 18807 RVA: 0x00225FD4 File Offset: 0x002241D4
		public override void OnItemChanged(ItemKey itemKey, int amount, bool queueAnim = false, bool isAllSelected = false, bool playItemAnim = true)
		{
			this._subPages[this.CurrIndex].OnItemChanged(itemKey, amount, queueAnim, isAllSelected, playItemAnim);
		}

		// Token: 0x06004978 RID: 18808 RVA: 0x00225FF5 File Offset: 0x002241F5
		public override void PlayUpgradeAnim(Action action)
		{
			this._subPages[this.CurrIndex].PlayUpgradeAnim(action);
		}

		// Token: 0x06004979 RID: 18809 RVA: 0x00226010 File Offset: 0x00224210
		public override void Confirm(ItemKeyAndCount itemKeyAndCount, ItemSourceType itemSourceType)
		{
			this._subPages[this.CurrIndex].Confirm(itemKeyAndCount, itemSourceType);
		}
	}
}
