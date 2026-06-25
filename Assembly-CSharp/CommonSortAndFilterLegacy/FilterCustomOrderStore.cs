using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200043D RID: 1085
	public class FilterCustomOrderStore : IFilterCustomOrderStore
	{
		// Token: 0x06003FFC RID: 16380 RVA: 0x001FCF86 File Offset: 0x001FB186
		public FilterCustomOrderStore(GameSort gameSort)
		{
			this._gameSort = gameSort;
		}

		// Token: 0x06003FFD RID: 16381 RVA: 0x001FCF98 File Offset: 0x001FB198
		public FilterCustomOrderData LoadFilterCustomOrderData(string key)
		{
			return this._gameSort.LoadFilterCustomOrderData(key);
		}

		// Token: 0x06003FFE RID: 16382 RVA: 0x001FCFB6 File Offset: 0x001FB1B6
		public void SaveFilterCustomOrderData(string key, FilterCustomOrderData data)
		{
			this._gameSort.SaveFilterCustomOrderData(key, data);
		}

		// Token: 0x06003FFF RID: 16383 RVA: 0x001FCFC7 File Offset: 0x001FB1C7
		public void ClearFilterCustomOrderData(string key)
		{
			this._gameSort.ClearFilterCustomOrderData(key);
		}

		// Token: 0x04002DC3 RID: 11715
		private readonly GameSort _gameSort;
	}
}
