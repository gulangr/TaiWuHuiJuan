using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200042D RID: 1069
	public struct CommonSortUiConfig
	{
		// Token: 0x04002D5C RID: 11612
		public List<short> SortIds;

		// Token: 0x04002D5D RID: 11613
		public List<int> SortNameIndexList;

		// Token: 0x04002D5E RID: 11614
		public List<short> DefaultSortIds;

		// Token: 0x04002D5F RID: 11615
		public SortUiState DefaultSortState;

		// Token: 0x04002D60 RID: 11616
		public Dictionary<ValueTuple<int, int>, List<short>> FilterTypeDic;
	}
}
