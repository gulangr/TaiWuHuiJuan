using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CBB RID: 3259
	public interface ITableHead
	{
		// Token: 0x0600A516 RID: 42262
		void BindSortController(ISortAndFilterController sortController, short[] columnSortIds);

		// Token: 0x0600A517 RID: 42263
		void UnbindSortController();

		// Token: 0x0600A518 RID: 42264
		void SyncSortStateFromController();
	}
}
