using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CBA RID: 3258
	public interface ISortAndFilterController
	{
		// Token: 0x17001138 RID: 4408
		// (get) Token: 0x0600A512 RID: 42258
		SortAndFilterState SortAndFilterState { get; }

		// Token: 0x0600A513 RID: 42259
		void SetSortState(SortStateData sortData);

		// Token: 0x0600A514 RID: 42260
		void RegisterTableHead(ITableHead tableHead, short[] columnSortIds);

		// Token: 0x0600A515 RID: 42261
		void UnregisterTableHead(ITableHead tableHead);
	}
}
