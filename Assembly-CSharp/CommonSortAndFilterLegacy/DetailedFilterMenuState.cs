using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000420 RID: 1056
	public struct DetailedFilterMenuState
	{
		// Token: 0x06003EA6 RID: 16038 RVA: 0x001F6090 File Offset: 0x001F4290
		public DetailedFilterMenuState(IReadOnlyCollection<int> selectedIndices, bool isActive)
		{
			this.SelectedIndices = selectedIndices;
			this.IsActive = isActive;
		}

		// Token: 0x04002D0A RID: 11530
		public readonly IReadOnlyCollection<int> SelectedIndices;

		// Token: 0x04002D0B RID: 11531
		public readonly bool IsActive;
	}
}
