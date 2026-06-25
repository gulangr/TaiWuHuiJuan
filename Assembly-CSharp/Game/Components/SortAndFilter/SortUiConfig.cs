using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CCB RID: 3275
	public struct SortUiConfig
	{
		// Token: 0x040082BC RID: 33468
		public List<short> SortIds;

		// Token: 0x040082BD RID: 33469
		public List<int> SortNameIndexList;

		// Token: 0x040082BE RID: 33470
		public List<short> DefaultSortIds;

		// Token: 0x040082BF RID: 33471
		public SortUiState DefaultSortState;

		// Token: 0x040082C0 RID: 33472
		public Dictionary<ValueTuple<int, int>, List<short>> FilterTypeDic;

		// Token: 0x040082C1 RID: 33473
		public List<short> FixedSortId;
	}
}
