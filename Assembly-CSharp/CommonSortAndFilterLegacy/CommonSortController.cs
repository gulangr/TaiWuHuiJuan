using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200042E RID: 1070
	public abstract class CommonSortController<TData>
	{
		// Token: 0x06003F69 RID: 16233 RVA: 0x001F9B58 File Offset: 0x001F7D58
		protected SortStateData GetSortData()
		{
			return this._sortUi.GetSortData();
		}

		// Token: 0x06003F6A RID: 16234 RVA: 0x001F9B75 File Offset: 0x001F7D75
		public void SetSortData(SortStateData data)
		{
			this._sortUi.SetSortData(data);
		}

		// Token: 0x06003F6B RID: 16235 RVA: 0x001F9B85 File Offset: 0x001F7D85
		public void Init(ICommonSortUi sortUi, string saveKay)
		{
			this._sortUi = sortUi;
			this._saveKey = saveKay;
			this.LoadSavedState();
		}

		// Token: 0x06003F6C RID: 16236
		public abstract void Sort(List<TData> dataList, SortStateData sortData, Action actionAfterSort);

		// Token: 0x06003F6D RID: 16237
		public abstract CommonSortUiConfig GenerateConfig();

		// Token: 0x06003F6E RID: 16238 RVA: 0x001F9BA0 File Offset: 0x001F7DA0
		public void SaveState()
		{
			bool flag = this._saveKey.IsNullOrEmpty();
			if (!flag)
			{
				SingletonObject.getInstance<GameSort>().SetCommonSortSortConfig(this._saveKey, this._sortUi.GetSortData());
			}
		}

		// Token: 0x06003F6F RID: 16239 RVA: 0x001F9BDC File Offset: 0x001F7DDC
		private void LoadSavedState()
		{
			bool flag = this._saveKey.IsNullOrEmpty();
			if (!flag)
			{
				SortStateData savedState = SingletonObject.getInstance<GameSort>().GetCommonSortSortConfig(this._saveKey);
				bool flag2 = savedState != null;
				if (flag2)
				{
					this._sortUi.SetSortData(savedState);
				}
			}
		}

		// Token: 0x04002D61 RID: 11617
		protected ICommonSortUi _sortUi;

		// Token: 0x04002D62 RID: 11618
		protected string _saveKey;
	}
}
