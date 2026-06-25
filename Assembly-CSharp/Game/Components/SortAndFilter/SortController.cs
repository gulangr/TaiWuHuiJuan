using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CCD RID: 3277
	public abstract class SortController<TData>
	{
		// Token: 0x0600A594 RID: 42388 RVA: 0x004D2C38 File Offset: 0x004D0E38
		protected SortStateData GetSortData()
		{
			return this.SortUi.GetSortData();
		}

		// Token: 0x0600A595 RID: 42389 RVA: 0x004D2C55 File Offset: 0x004D0E55
		public void SetSortData(SortStateData data)
		{
			this.SortUi.SetSortData(data);
		}

		// Token: 0x0600A596 RID: 42390 RVA: 0x004D2C65 File Offset: 0x004D0E65
		public void Init(ISortUi sortUi, string saveKey)
		{
			this.SortUi = sortUi;
			this.SaveKey = saveKey;
			this.LoadSavedState();
		}

		// Token: 0x0600A597 RID: 42391
		public abstract Comparison<TData> GenerateComparer(SortStateData sortData);

		// Token: 0x0600A598 RID: 42392
		public abstract SortUiConfig GenerateConfig();

		// Token: 0x0600A599 RID: 42393 RVA: 0x004D2C80 File Offset: 0x004D0E80
		public void SaveState()
		{
			bool flag = this.SaveKey.IsNullOrEmpty();
			if (!flag)
			{
				SingletonObject.getInstance<GameSort>().SetNewSortConfig(this.SaveKey, this.SortUi.GetSortData());
			}
		}

		// Token: 0x0600A59A RID: 42394 RVA: 0x004D2CBC File Offset: 0x004D0EBC
		private void LoadSavedState()
		{
			bool flag = this.SaveKey.IsNullOrEmpty();
			if (!flag)
			{
				SortStateData savedState = SingletonObject.getInstance<GameSort>().GetNewSortConfig(this.SaveKey);
				bool flag2 = savedState != null;
				if (flag2)
				{
					this.SortUi.SetSortData(savedState);
				}
			}
		}

		// Token: 0x040082C3 RID: 33475
		protected ISortUi SortUi;

		// Token: 0x040082C4 RID: 33476
		protected string SaveKey;
	}
}
