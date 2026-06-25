using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000441 RID: 1089
	public interface IFilterCustomOrderStore
	{
		// Token: 0x0600401B RID: 16411
		FilterCustomOrderData LoadFilterCustomOrderData(string key);

		// Token: 0x0600401C RID: 16412
		void SaveFilterCustomOrderData(string key, FilterCustomOrderData data);

		// Token: 0x0600401D RID: 16413
		void ClearFilterCustomOrderData(string key);
	}
}
