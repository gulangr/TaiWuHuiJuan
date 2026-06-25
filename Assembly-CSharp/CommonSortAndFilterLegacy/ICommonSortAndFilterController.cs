using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200042A RID: 1066
	public interface ICommonSortAndFilterController
	{
		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06003F3A RID: 16186
		SortAndFilterState SortAndFilterState { get; }

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06003F3B RID: 16187
		IReadOnlyList<object> ReadOnlyOriginalDataList { get; }

		// Token: 0x06003F3C RID: 16188
		void SetSortState(SortStateData sortData);

		// Token: 0x06003F3D RID: 16189
		void RegisterTableHead(CommonTableHead tableHead, short[] columnSortIds);
	}
}
